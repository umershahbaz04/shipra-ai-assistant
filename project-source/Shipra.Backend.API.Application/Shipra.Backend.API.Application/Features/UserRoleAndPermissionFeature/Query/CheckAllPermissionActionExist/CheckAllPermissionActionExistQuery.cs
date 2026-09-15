using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.PermissionUseCase;
using Shipra.Backend.API.Application.DTOs.ShipmentUseCase;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.UpdateShipmentGridColumn;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.CheckAllPermissionActionExist;
public class CheckAllPermissionActionExistQuery : IRequest<ServiceResultDTO>
{
  public List<PermissionActionRequestModel>? listItems { get; set; }
  public bool IsAllowInsertMissing { get; set; }
}
public class CheckAllPermissionActionExistQueryHandler : RequestHandlerBase<CheckAllPermissionActionExistQuery, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public CheckAllPermissionActionExistQueryHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<CheckAllPermissionActionExistQueryHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CheckAllPermissionActionExistQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var filterdPermissionList = GetAllValidController(request.listItems!);
      var allPermissionActions = await _permissionRepository.GetAllPermissionActions();
      var notExistList = new List<PermissionActionRequestModel>();
      foreach (var item in filterdPermissionList!)
      {
        var controller = item.Controller!.ToLower();
        var obj = new PermissionActionRequestModel()
        {
          Controller = controller,
        };
        //foreach (var action in item?.Actions!)
        //{
        //  if (!allPermissionActions.Any(x => x.ControllerName?.ToLower() == controller && x.ActionName?.ToLower() == action.ToLower()))
        //  {
        //    //do break or insert into database
        //    if (request.IsAllowInsertMissing)
        //    {
        //      int permissionGroupId = 7;
        //      if (true)
        //      {
        //        PermissionAction objPermissionAction=  await _permissionRepository.GetPermissionActionBy(controller, action); 
        //        if (objPermissionAction == null)
        //        {
        //          PermissionAction oPermissionAction = PermissionAction.Create(controller, action, permissionGroupId);
        //          await _permissionRepository.CreatePermissionAction(oPermissionAction);
        //        }
        //      }
              
        //    }
        //    else
        //    {
        //      // we will retun at the end
        //      obj.Actions!.Add(action);
        //    }
        //  }
        //}
        notExistList.Add(obj);
      }
      if (notExistList.Count > 0)
      {
        string result = string.Join(",", notExistList
                        .SelectMany(item => item?.Actions!)
                        .Select(action => action.ToString()));
        if (!string.IsNullOrEmpty(result))
        {
          serviceResult.CreateError("Errors", new string[] { "Not exit actions methods " + result });
        }
      }
      else
      {
        serviceResult.IsSuccess = true;
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private List<PermissionActionRequestModel> GetAllValidController(List<PermissionActionRequestModel> permissionList)
  {
    string filterWithout = "";

    // Split the filterWithout string into individual filters
    string[] filters = filterWithout.Split(',');

    // Filter out objects based on the conditions
    List<PermissionActionRequestModel> filteredList = permissionList
        .Where(model => !filters.Any(filter => model.Controller!.Contains(filter)) ||
                        !model.Actions!.Any(action => filters.Any(filter => action.Contains(filter))))
        .ToList(); 
    return filteredList; 
  }
}
public class CheckAllPermissionActionExistQueryValidator : AbstractValidator<CheckAllPermissionActionExistQuery>
{
  public CheckAllPermissionActionExistQueryValidator()
  {
    RuleFor(x => x.listItems).Must(x => x != null).WithMessage("Items list must contain at least one item."); 
  }
} 

