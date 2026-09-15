using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.AssignStationToVariants;

public class AssignStationToVariantsCommand : IRequest<ServiceResultDTO>
{
    public int ProductStationId { get; set; }
    public List<VariantQuantityDto> Variants { get; set; } = new();
}

public class VariantQuantityDto
{
    public long ProductVariantId { get; set; }
    public string Sku { get; set; } = null!;
    public int Quantity { get; set; }
    public int? LowQuantityLimit { get; set; }
}

public class AssignStationToVariantsCommandHandler : RequestHandlerBase<AssignStationToVariantsCommand, ServiceResultDTO>
{
    private readonly IProductRepository _productRepository;

    public AssignStationToVariantsCommandHandler(
        IProductRepository productRepository, 
        IServiceProvider serviceProvider, 
        ILogger<AssignStationToVariantsCommandHandler> logger) : base(serviceProvider, logger)
    {
        _productRepository = productRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(AssignStationToVariantsCommand request, CancellationToken cancellationToken)
    {
        var response = new ServiceResultDTO();
        try
        {
            if (request.ProductStationId <= 0 || request.Variants == null || !request.Variants.Any())
            {
                response.Errors?.Add("InvalidRequest", new string[] { "Invalid station or no variants provided." });
                response.IsSuccess = false;
                return response;
            }

            // Create InventoryBalances and InventoryTransactions
            var balances = new List<InventoryBalance>();
            var transactions = new List<InventoryTransaction>();

            foreach (var variant in request.Variants)
            {
                if (variant.LowQuantityLimit.HasValue && variant.LowQuantityLimit.Value >= 0)
                {
                    var existingVariant = await _productRepository.GetProductVariantByIdAsync(variant.ProductVariantId);
                    if (existingVariant != null)
                    {
                        existingVariant.Update(
                            existingVariant.SKU,
                            existingVariant.Barcode,
                            existingVariant.Price,
                            existingVariant.PurchasePrice,
                            existingVariant.Weight,
                            existingVariant.Length,
                            existingVariant.Width,
                            existingVariant.Height,
                            variant.LowQuantityLimit.Value,
                            existingVariant.VariantOptionText,
                            existingVariant.ImageGalleryId,
                            existingVariant.ProductVariantStatusId,
                            _currentUser.EmployeeId
                        );
                        await _productRepository.UpdateProductVariantAsync(existingVariant);
                    }
                }

                if (variant.Quantity >= 0)
                {
                    var balance = InventoryBalance.Create(variant.ProductVariantId, request.ProductStationId, variant.Quantity);
                    balances.Add(balance);

                    if (variant.Quantity > 0)
                    {
                        var transaction = InventoryTransaction.Create(
                            variant.ProductVariantId,
                            request.ProductStationId,
                            (int)InventoryTransactionType.Receipt, // Receipt transaction type
                            variant.Quantity,
                            0, // previous quantity was 0 since it had no station
                            variant.Quantity,
                            "Initial Station Assignment",
                            _currentUser.EmployeeId);
                            
                        transactions.Add(transaction);
                    }
                }
            }

            if (balances.Any())
            {
                await _productRepository.CreateInventoryBalancesAsync(balances);
            }

            if (transactions.Any())
            {
                await _productRepository.CreateInventoryTransactionsAsync(transactions);
            }

            response = new ServiceResultDTO(new BaseResponseDto
            {
                Message = "Stations assigned successfully."
            });
            response.IsSuccess = true;
            return response;
        }
        catch (Exception ex)
        {
            response.CreateErrorResponse(ex);
            throw;
        }
    }
}
