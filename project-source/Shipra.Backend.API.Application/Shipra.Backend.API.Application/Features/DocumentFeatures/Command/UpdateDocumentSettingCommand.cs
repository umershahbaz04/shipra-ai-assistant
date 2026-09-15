using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DocumentUseCase;
using Shipra.Backend.API.Core.DocumentAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.MiscFeatures.Command;
public class UpdateDocumentSettingCommand : IRequest<ServiceResultDTO>
{
  public List<UpdateDocumentSettingRequestModel>? list { get; set; }
}
public class UpdateDocumentSettingCommandHandler : RequestHandlerBase<UpdateDocumentSettingCommand, ServiceResultDTO>
{
  private readonly IDocumentRepository _documentRepository;

  public UpdateDocumentSettingCommandHandler(IDocumentRepository documentRepository, IServiceProvider serviceProvider, ILogger<UpdateDocumentSettingCommandHandler> logger) : base(serviceProvider, logger)
  {
    _documentRepository = documentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateDocumentSettingCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      foreach (var item in request.list!)
      { 
        if (!string.IsNullOrEmpty(item.DocumentTemplateConfigId))
        { 
          if (GuidHelper.Validator(item.DocumentTemplateConfigId))
          { 
            var oDocumentTemplateConfig = await _documentRepository.GetDocumentTemplateConfigById(new DocumentTemplateConfigId(new Guid(item.DocumentTemplateConfigId!)), _currentUser.ClientId);
            oDocumentTemplateConfig.Update(item.TemplateName, item.DocumentTemplateId,item.IsAskEveryTime, _currentUser.EmployeeId);
            var res = await _documentRepository.UpdateDocumentTemplateConfig(oDocumentTemplateConfig);
            if (res)
            {
              serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = res, Message = "Update successfully!" });
            }
          }
          else
          {
            serviceResult.CreateError("DocumentTemplateConfigId", new string[] { "Invalid Template config id" });
          }
        }
        else
        {
          var oDocumentTemplateConfig = DocumentTemplateConfig.Create(_currentUser.ClientId, item.DocumentTemplateId, item.TemplateName, _currentUser.EmployeeId!);
          bool isCreated = await _documentRepository.CreateDocumentTemplateConfig(oDocumentTemplateConfig);
          if (isCreated)
          {
            serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = isCreated, Message = "Update successfully!" });
          }
        }
      }

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class UpdateDocumentSettingCommandValidator : AbstractValidator<UpdateDocumentSettingCommand>
{
  public UpdateDocumentSettingCommandValidator()
  {
    RuleFor(x => x.list).Must(x => x != null).WithMessage("Document setting list must contain at least one item.");
    RuleForEach(model => model.list).SetValidator(model => new CreateDocSettingValidator());
  }
}
public class CreateDocSettingValidator : AbstractValidator<UpdateDocumentSettingRequestModel>
{
  public CreateDocSettingValidator()
  {
    RuleFor(v => v.TemplateName).NotNull().NotEmpty(); 
  }
}
