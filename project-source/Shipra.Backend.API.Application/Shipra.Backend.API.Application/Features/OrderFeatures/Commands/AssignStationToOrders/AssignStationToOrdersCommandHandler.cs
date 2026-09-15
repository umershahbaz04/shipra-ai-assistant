using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Application.Common;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.AssignStationToOrders;

public class AssignStationToOrdersCommandHandler : RequestHandlerBase<AssignStationToOrdersCommand, ServiceResultDTO>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderTrackingHistoryRepository _orderNoteRepository;

    public AssignStationToOrdersCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IOrderTrackingHistoryRepository orderNoteRepository,
        IServiceProvider serviceProvider,
        ILogger<AssignStationToOrdersCommandHandler> logger) : base(serviceProvider, logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _orderNoteRepository = orderNoteRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(AssignStationToOrdersCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();

        try
        {
            var clientId = _currentUser.ClientId!.Value;
            var missingSkus = new List<string>();
            var validOrders = new List<Order>();

            foreach (var orderIdStr in request.OrderIds)
            {
                if (!Guid.TryParse(orderIdStr, out var gOrderId)) continue;
                var orderId = new OrderId(gOrderId);

                var order = await _orderRepository.GetOrderById(orderId, new ClientId(clientId));
                if (order == null) continue;

                var orderItems = await _orderRepository.GetOrderItemsByOrderId(orderId);
                bool orderValid = true;

                foreach (var item in orderItems)
                {
                    if (item.ProductId == null) continue;
                    
                    var balances = await _productRepository.GetInventoryBalancesByProductIdAsync(item.ProductId);
                    
                    if (!balances.Any(b => b.ProductStationId == request.StationId))
                    {
                        var variants = await _productRepository.GetProductVariantsByProductIdAsync(item.ProductId);
                        var variant = variants.FirstOrDefault();
                        var sku = variant != null ? variant.SKU : item.ProductId.Value.ToString();
                        
                        if (!missingSkus.Contains(sku!))
                            missingSkus.Add(sku!);
                            
                        orderValid = false;
                    }
                }

                if (orderValid)
                {
                    validOrders.Add(order);
                }
            }

            if (missingSkus.Any())
            {
                serviceResult.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest);
                serviceResult.CreateError("ValidationFailed", new string[] { $"The following SKUs are not available at the selected station: {string.Join(", ", missingSkus)}" });
                return serviceResult;
            }

            foreach (var order in validOrders)
            {
                var oldStationId = order.StationId;
                order.UpdateOrderStationId(request.StationId);
                await _orderRepository.UpdateOrder(order);

                var orderItems = await _orderRepository.GetOrderItemsByOrderId(order.OrderId);
                foreach (var item in orderItems)
                {
                    if (item.ProductId == null) continue;
                    var product = await _productRepository.GetProductByIdAsync(item.ProductId, new ClientId(clientId));
                    if (product != null && product.TrackInventory.GetValueOrDefault())
                    {
                        // 1. De-commit from old station if one existed and it was different
                        if (oldStationId.HasValue && oldStationId.Value > 0 && oldStationId.Value != request.StationId)
                        {
                            var oldBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(item.ProductVariantId.GetValueOrDefault(), oldStationId.Value);
                            if (oldBalance != null)
                            {
                                var committed = oldBalance.QuantityCommitted - item.Quantity.GetValueOrDefault();
                                var quantityAvailable = oldBalance.QuantityAvailable + item.Quantity.GetValueOrDefault();
                                oldBalance.UpdateQuantities(oldBalance.QuantityOnHand, quantityAvailable, committed < 0 ? 0 : committed);
                                await _productRepository.UpdateInventoryBalanceAsync(oldBalance);
                            }
                        }

                        // 2. Commit to new station
                        var inventoryBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(item.ProductVariantId.GetValueOrDefault(), request.StationId);
                        if (inventoryBalance != null)
                        {
                            var quantityCommitted = inventoryBalance.QuantityCommitted + item.Quantity.GetValueOrDefault();
                            var quantityAvailable = inventoryBalance.QuantityAvailable - item.Quantity.GetValueOrDefault();
                            inventoryBalance.UpdateQuantities(inventoryBalance.QuantityOnHand, quantityAvailable, quantityCommitted);
                            await _productRepository.UpdateInventoryBalanceAsync(inventoryBalance);
                        }
                    }
                }

                // Add OrderNote
                var note = OrderNote.CreateOrderNote(
                    order.OrderId!,
                    $"Assigned to Station (ID: {request.StationId})",
                    _currentUser.EmployeeId!
                );
                await _orderNoteRepository.CreateOrderNote(note);
            }

            serviceResult.CreateSuccessResponse();
            return serviceResult;
        }
        catch (Exception ex)
        {
            serviceResult.CreateErrorResponse(ex);
            return serviceResult;
        }
    }
}
