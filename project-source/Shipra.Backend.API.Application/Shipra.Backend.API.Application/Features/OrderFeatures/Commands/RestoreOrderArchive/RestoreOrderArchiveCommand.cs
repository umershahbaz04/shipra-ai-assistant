using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.OrdersArchive;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.RestoreOrderArchive;
public class RestoreOrderArchiveCommand : IRequest<ServiceResultDTO>
{
  public string? ArchiveNo { get; set; }
}

public class RestoreOrderArchiveCommandHandler : RequestHandlerBase<RestoreOrderArchiveCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public RestoreOrderArchiveCommandHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<RestoreOrderArchiveCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(RestoreOrderArchiveCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var archiveOrders = await _orderRepository.GetArchiveOrdersByArchiveNo(_currentUser.ClientId!, request.ArchiveNo!);

      if (archiveOrders == null || archiveOrders!.Count == 0)
      {
        serviceResult.CreateErrorResponse();
        return serviceResult;
      }

      foreach (var archive in archiveOrders!)
      {
        if (string.IsNullOrWhiteSpace(archive.OrderJson))
          continue;


        var wrapper = JsonConvert.DeserializeObject<OrderJsonWrapper>(archive.OrderJson);

        if (wrapper?.Order == null)
          continue;

        // Map DTO to domain Order if needed
        var order = wrapper?.Order!;

        // Assuming 'order' is your domain entity
        var orderDto = Order.CreateOrderWithArchiveData(
            orderId: order.OrderId,
            clientId: order.ClientId,
            storeId: order.StoreId,
            saleChannelConfigId: order.SaleChannelConfigId,
            orderTypeId: order.OrderTypeId,
            orderNo: order.OrderNo,
            orderDate: order.OrderDate,
            orderAddressId: order.OrderAddressId,
            amount: order.Amount,
            carrierId: order.CarrierId,
            activeCarrierId: order.ActiveCarrierId,
            activeCarrierPickupLocationId: order.ActiveCarrierPickupLocationId,
            carrierTrackingNo: order.CarrierTrackingNo,
            carrierTrackingStatus: order.CarrierTrackingStatus,
            carrierTrackingStatusId: order.CarrierTrackingStatusId,
            carrierAssignDate: order.CarrierAssignDate,
            carrierLastUpdateDateTime: order.CarrierLastUpdateDateTime,
            carrierPaymentSettlementId: order.CarrierPaymentSettlementId,
            carrierPaymentSettlementDate: order.CarrierPaymentSettlementDate,
            carrierRRId: order.CarrierRRId,
            description: order.Description,
            remarks: order.Remarks,
            fullFillmentStatusId: order.FullFillmentStatusId,
            fulFilledById: order.FulFiledById,
            fulFilledDate: order.FulFilledDate,
            itemsCount: order.ItemsCount,
            deliveryCharges: order.DeliveryCharges,
            paymentStatusId: order.PaymentStatusId,
            paymentRef: order.PaymentRef,
            returnRef: order.ReturnRef,
            weight: order.Weight,
            itemValue: order.ItemValue,
            orderRequestVia: order.OrderRequestVia,
            paymentMethodId: order.PaymentMethodId,
            stationId: order.StationId,
            discount: order.Discount,
            vat: order.Vat,
            totalTax: order.TotalTax,
            cShippingCharges: order.CShippingCharges,
            trackingLock: order.TrackingLock,
            refNo: order.RefNo,
            orderLabels: order.OrderLabels,
            createdOn: order.CreatedOn,
            createdBy: order.CreatedBy,
            updatedOn: order.UpdatedOn,
            updatedBy: order.UpdatedBy,
            stripeCustomerId: order.StripeCustomerId,
            stripeInvoiceHostURL: order.StripeInvoiceHostURL,
            stripeInvoicePDFURL: order.StripeInvoicePDFURL,
            stripeInvoiceId: order.StripeInvoiceId,
            orderDeliveryTypeId: order.OrderDeliveryTypeId,
            saleChannelOrderId: order.SaleChannelOrderId,
            saleChannelLookupId: order.SaleChannelLookupId,
            carrierContractTypeId: order.CarrierContractTypeId,
            isShipraInvoiceCharged: order.IsShipraInvoiceCharged,
            invoiceId: order.InvoiceId,
            dInvoiceId: order.DInvoiceId,
            pInvoiceId: order.PInvoiceId
        );
        var createdOrder = await _orderRepository.CreateOrder(orderDto);

        if (wrapper!.OrderItems?.Any() == true)
        {
          foreach (var item in wrapper.OrderItems)
          {
            var oItem = OrderItem.CreateOrderItemWithArchiveData(
              orderItemId: item.OrderItemId,
              orderId: item.OrderId!,
              productId: item.ProductId!,
              price: item.Price,
              description: item.Description,
              remarks: item.Remarks,
              quantity: item.Quantity,
              itemBarcode: item.ItemBarcode,
              discount: item.Discount,
              referenceNo: item.ReferenceNo,
              productVariantId: item.ProductVariantId
            );
            await _orderRepository.CreateOrderItem(oItem);
          }
        }
      
        bool isCreated = await _orderRepository.DeleteArchiveOrder(archive);
      }


      serviceResult.CreateSuccessResponse();
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
    }

    return serviceResult;
  }


}
