using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;
namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.CreateShipperInvoice;

// Command
public class CreateInvoiceCommand : IRequest<ServiceResultDTO>
{
  public int SaleChannelConfigId { get; set; }
  public string? ClientId { get; set; }
  public string? ShipperInvoiceAdjustmentIds { get; set; }
  // Child items
  public List<CreateInvoiceDetailDto> Details { get; set; } = new();

  // DTO for details
  public class CreateInvoiceDetailDto
  {
    public string? OrderId { get; set; }
    public decimal? Rate { get; set; } 
    public string? OrderNo { get; set; } 
  }

  // ================= HANDLER =================
  public class CreateInvoiceCommandHandler : RequestHandlerBase<CreateInvoiceCommand, ServiceResultDTO>
  {
    private readonly IClientRepository _clientRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IShipperInvoiceRepository _repo;

    public CreateInvoiceCommandHandler(IClientRepository clientRepository, IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IShipperInvoiceRepository repo, IServiceProvider serviceProvider, ILogger<CreateInvoiceCommandHandler> logger) : base(serviceProvider, logger)
    {
      _clientRepository = clientRepository;
      _employeeRepository = employeeRepository;
      _orderRepository = orderRepository;
      _repo = repo;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
      var serviceResult = new ServiceResultDTO();

      try
      {
        // Totals
        var totalOrders = request.Details.Count;
        var totalAmount = request.Details.Sum(x => x.Rate);

        #region MyRegion

        var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
        if (client == null)
        {
          throw new EntityNotFoundException("Client", _currentUser.ClientIdStr!);
        }
        var invoiceCount = await _repo.GetShipperInvoiceCount(request.SaleChannelConfigId, _currentUser.ClientId!.Value);
        invoiceCount = invoiceCount + 1;
        var date = DateTime.Now.ToString("yyyyMMdd"); // e.g., 20250716
        var invoiceNo = $"D{client?.ClientIdentifier}{date}-{invoiceCount}";

        #endregion
        #region adjustments
        //Task<List<ShipperInvoiceAdjustmentModel>> GetAllShipperInvoiceAdjustment(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? clientId, int transactionTypeId = 0, int? InvoiceCreateId = 0)
        List<ShipperInvoiceAdjustment> oDInvoiceAdjustment = new();
        if (!string.IsNullOrEmpty(request.ShipperInvoiceAdjustmentIds))
        {
          var idsToSelect = request.ShipperInvoiceAdjustmentIds
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(id => int.Parse(id.Trim()))
            .ToList();

          //List<ShipperInvoiceAdjustmentModel> oDInvoiceAdjustmentDtos = await _repo.GetAllShipperInvoiceAdjustment(null, null, 0, 10000, null!, 0, "Desc", request.ClientId, 0, (int)EnumInvoiceCreateId.NotCreated);

          oDInvoiceAdjustment = await _repo.GetAllDeliveryAdjustmentByIds(idsToSelect!, _currentUser.ClientId!.Value);

          //selectedDInvoices = oDInvoiceAdjustmentDtos.Where(dto => idsToSelect.Contains(dto.ShipperInvoiceAdjustmentId.GetValueOrDefault())).ToList();
          var totalDebit = oDInvoiceAdjustment.Where(x => x.TransactionTypeId == (int)EnumTransactionType.Debit).Sum(x => x.Amount);
          var totalCredit = oDInvoiceAdjustment.Where(x => x.TransactionTypeId == (int)EnumTransactionType.Credit).Sum(x => x.Amount);

          // Apply debit/credit to the total amount
          totalAmount = totalAmount + totalDebit.GetValueOrDefault();
          totalAmount = totalAmount - totalCredit.GetValueOrDefault();
        }

        #endregion
        // Who is creating (you can replace this with your base method / token user)
        var employee = await _employeeRepository.GetEmployeeById(_currentUser.EmployeeId!, _currentUser.ClientId!);
        var createdBy = employee?.EmployeeName ?? "";

        // 1) Create Invoice FIRST (Factory)
        var invoice = ShipperInvoice.Create(invoiceNo: invoiceNo, clientId: _currentUser.ClientId!.Value, saleChannelConfigId: request.SaleChannelConfigId, amount: totalAmount, totalOrder: totalOrders, createdBy: createdBy);

        // Repo saves & returns invoice with generated Id
        var savedInvoice = await _repo.CreateShipperInvoice(invoice);

        if (savedInvoice is null)
        {
          serviceResult.CreateError("Faild", new string[] { "Invoice creation failed." });
          return serviceResult;
        }

        // IMPORTANT: get InvoiceId after save
        // Replace property name based on your Invoice model
        var invoiceId = savedInvoice.ShipperInvoiceId; // or savedInvoice.InvoiceId

        if (invoiceId > 0)
        {
          #region invoice adjustment
          if (oDInvoiceAdjustment.Count > 0)
          {
            #region update adjustment with invoice id 
            foreach (var item in oDInvoiceAdjustment)
            {
              item.UpdateShipperInvoiceId(invoiceId, createdBy);
              await _repo.UpdateShipperInvoiceAdjustment(item);
            }
            #endregion
          }
          #endregion
          // 2) Create Invoice Details (Factory) USING invoiceId
          foreach (var d in request.Details)
          {
            var detail = ShipperInvoiceDetail.Create(
                          orderId: Guid.TryParse(d.OrderId, out var orderGuid) ? orderGuid : (Guid?)null,
                          rate: d.Rate,
                          clientId: _currentUser.ClientId!.Value,
                          shipperInvoiceId: invoiceId,
                          orderNo: d.OrderNo 
                      );


            var savedDetail = await _repo.CreateShipperInvoiceDetail(detail);

            #region MyRegion
            var order = await _repo.GetShiperOrderByOrderNo(d.OrderNo!, _currentUser.ClientId!.Value);
            if (order is not null)
            {


              order!.UpdateInvoice(invoiceId, (int)EnumInvoiceStatus.Paid);
              await _repo.UpdateShipperOrder(order);
            }
            #endregion 
          }
        }
        // Success response (return invoiceId + totals)
        serviceResult = new ServiceResultDTO(new
        {
          InvoiceId = invoiceId,
          TotalOrders = totalOrders,
          Amount = totalAmount
        });

        return serviceResult;
      }
      catch (Exception ex)
      {
        serviceResult.CreateErrorResponse(ex);
        return serviceResult;
      }
    }
  }



  // Validator
  public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
  {
    public CreateInvoiceCommandValidator()
    {
      RuleFor(x => x.SaleChannelConfigId).GreaterThan(0);

      RuleFor(x => x.Details)
          .NotNull()
          .Must(x => x.Count > 0)
          .WithMessage("At least 1 invoice detail is required.");

      RuleForEach(x => x.Details).ChildRules(d =>
      {
        d.RuleFor(x => x.OrderId).NotEmpty();
        d.RuleFor(x => x.Rate).GreaterThan(0);
        d.RuleFor(x => x.OrderNo).MaximumLength(100);
      });
    }
  }
}
