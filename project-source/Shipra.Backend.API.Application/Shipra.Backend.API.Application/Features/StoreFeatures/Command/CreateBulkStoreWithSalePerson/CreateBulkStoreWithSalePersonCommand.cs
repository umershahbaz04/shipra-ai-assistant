using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.CreateEmployee;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.CreateStore;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientUserRole;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.CreateBulkStoreWithSalePerson;
public class CreateBulkStoreWithSalePersonCommand : IRequest<ServiceResultDTO>
{
  public List<CreateStoreRequestModel>? List { get; set; } = new List<CreateStoreRequestModel>();
}
public class CreateBulkStoreWithSalePersonCommandHandler : RequestHandlerBase<CreateBulkStoreWithSalePersonCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;

  public CreateBulkStoreWithSalePersonCommandHandler(IMediator mediator, IServiceProvider serviceProvider, ILogger<CreateBulkStoreWithSalePersonCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateBulkStoreWithSalePersonCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();

    try
    {
      if (request.List == null || !request.List.Any())
      {
        serviceResultDTO.CreateError(
          "Request",
          new[] { "Request list is empty." }
        );
        serviceResultDTO.CreateErrorResponse(HttpStatusCode.BadRequest);
        return serviceResultDTO;
      }
      bool hasError = false;
      // 1. Map input to store commands
      var storeCommands = request.List.Select(x => new CreateStoreCommand
      {
        StoreName = x.StoreName,
        StoreCompany = x.StoreCompany,
        CustomerServiceNo = x.CustomerServiceNo,
        Phone = x.Phone,
        Email = x.Email,
        Urls = x.Urls,
        LicenseNo = x.LicenseNo,
        StoreImage = x.StoreImage,
        StoreAddress = x.StoreAddress,
        UserName = x.UserName,
        Password = x.Password,
        DateOfBirth = x.DateOfBirth
      }).ToList();

      // 2. Resolve Sale Person role dynamically
      int salePersonRoleId = 0;

      if (storeCommands.Any(x => !string.IsNullOrEmpty(x.UserName)))
      {
        var roleResponse =
          await _mediator.Send(new GetAllClientUserRoleQuery(), cancellationToken);

        if (!roleResponse.IsSuccess)
        {
          serviceResultDTO.CreateError(
            "Roles",
            new[] { "Failed to fetch client roles." }
          );
          serviceResultDTO.CreateErrorResponse(HttpStatusCode.BadRequest);
          return serviceResultDTO;
        }

        var salePersonRole = ((IEnumerable<object>)roleResponse.Result!)
          .FirstOrDefault(x =>
            x.GetType().GetProperty("RoleName")?.GetValue(x)?.ToString() == "Sale Person"
          );

        if (salePersonRole == null)
        {
          serviceResultDTO.CreateError(
            "Role",
            new[] { "Sale Person role not found." }
          );
          serviceResultDTO.CreateErrorResponse(HttpStatusCode.BadRequest);
          return serviceResultDTO;
        }

        salePersonRoleId = Convert.ToInt32(
          salePersonRole.GetType()
            .GetProperty("ClientUserRoleId")?
            .GetValue(salePersonRole)
        );
      }

      // 3. Create stores & employees
      var createdStoresInfo = new List<object>();

      foreach (var storeCommand in storeCommands)
      {
        var storeResponse = await _mediator.Send(storeCommand, cancellationToken);
        if (!storeResponse.IsSuccess)
        {
          hasError = true;
          var errorMessage =
              storeResponse.Errors != null && storeResponse.Errors.Any()
                ? string.Join(
                    ", ",
                    storeResponse.Errors.SelectMany(e => e.Value)
                  )
                : "Store creation failed.";
          serviceResultDTO.CreateError(
            storeCommand.StoreName!,
            new[] { errorMessage }
          );
          continue;
        }

        var storeId =  (int)storeResponse.Result!.Data!;

        createdStoresInfo.Add(new
        {
          StoreName = storeCommand.StoreName,
          StoreId = storeId,
          UserName = storeCommand.UserName,
          Password = storeCommand.Password
        });

        if (!string.IsNullOrEmpty(storeCommand.UserName))
        {
          var employeeCommand = new CreateEmployeeCommand
          {
            EmployeeName = storeCommand.StoreName,
            GenderId = (int)EnumGender.Male,
            DateOfBirth = storeCommand.DateOfBirth,
            UserName = storeCommand.UserName,
            StoreId = storeId,
            Mobile = UtilityHelper.RemoveAllSpaces(storeCommand.CustomerServiceNo!),
            WorkEmail = storeCommand.Email,
            Password = storeCommand.Password,
            Phone = storeCommand.Phone,
            IsPreverifyEmail = true,
            UserRoleId = salePersonRoleId,
            EmployeeTypeId = (int)EnumEmployeeType.Shipper,
            EmployeeAddress = storeCommand.StoreAddress,
          };

          var empResponse =
            await _mediator.Send(employeeCommand, cancellationToken);

          if (!empResponse.IsSuccess)
          {
            hasError = true;

            var errorMessage =
              empResponse.Errors != null && empResponse.Errors.Any()
                ? string.Join(", ", empResponse.Errors.SelectMany(e => e.Value))
                : "Employee creation failed.";

            serviceResultDTO.CreateError(
              storeCommand.StoreName!,
              new[] { errorMessage }
            );
          }
        }
      }
      var finalResponse = new
      {
        SuccessList = createdStoresInfo,
        Errors = serviceResultDTO.Errors
      };

      serviceResultDTO = new ServiceResultDTO(finalResponse, isSuccess: !hasError);


    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
    }

    return serviceResultDTO;
  }
}
public class CreateBulkStoreWithSalePersonCommandValidator
  : AbstractValidator<CreateBulkStoreWithSalePersonCommand>
{
  public CreateBulkStoreWithSalePersonCommandValidator()
  {
    RuleFor(x => x.List)
      .NotNull() 
      .WithMessage("Request list must contain at least one store.");
  }
}
