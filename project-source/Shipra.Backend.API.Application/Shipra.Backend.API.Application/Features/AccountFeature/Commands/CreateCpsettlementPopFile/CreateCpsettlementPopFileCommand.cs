using System.Dynamic;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateProduct;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.CreateCpsettlementPopFile;
public class CreateCpsettlementPopFileCommand : IRequest<ServiceResultDTO>
{
  public List<IFormFile>? Files { get; set; }
  public string? CarrierPaymentSettlementId { get; set; }
}
public class CreateCpsettlementPocFileCommandHandler : RequestHandlerBase<CreateCpsettlementPopFileCommand, ServiceResultDTO>
{
  private readonly IAccountRepository _accountRepository;
  private readonly IS3Service _s3Service;

  public CreateCpsettlementPocFileCommandHandler(IAccountRepository accountRepository, IS3Service s3Service, IServiceProvider serviceProvider, ILogger<CreateCpsettlementPocFileCommandHandler> logger) : base(serviceProvider, logger)
  {
    _accountRepository = accountRepository;
    _s3Service = s3Service;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateCpsettlementPopFileCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var resultList = new List<dynamic>(); // List to store results
      var carrierPaymentSettlementId = new CarrierPaymentSettlementId(new Guid(request.CarrierPaymentSettlementId!));
      foreach (var sFile in request.Files!)
      {
        var s3path = ApplicationConstants.GetS3ClientFolderPattern(_currentUser.ClientId!.Value!.ToString(), ApplicationConstants.CarrierPaymentSettlement);

        var requestResponse = await _s3Service.UploadFileAsync(sFile!, s3path);
        var s3responseDto = new S3ResponseDTO();
        s3responseDto.CreateS3ResponseDTO(requestResponse);

        var extension = Path.GetExtension(sFile!.FileName);
        if (!string.IsNullOrEmpty(s3responseDto.Url))
        {
          var oCpsettlementPodFile = CPSettlementPopFile.Create(s3responseDto.Url, extension, carrierPaymentSettlementId, _currentUser.EmployeeId!);
          await _accountRepository.CreateCpsettlementPopFile(oCpsettlementPodFile);
          resultList.Add(new
          {
            request.CarrierPaymentSettlementId,
            s3responseDto.Url
          });
        } 
      }
      if (resultList.Count > 0)
      {
        serviceResult = new ServiceResultDTO(resultList);
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
public class CreateCpsettlementPopFileCommandValidator : AbstractValidator<CreateCpsettlementPopFileCommand>
{
  public CreateCpsettlementPopFileCommandValidator()
  {
    RuleFor(x => x.CarrierPaymentSettlementId).NotEmpty().NotNull().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(x => x.Files).Must(x => x != null);
    RuleForEach(model => model.Files).SetValidator(model => new CreateCpsettingFileValidator());

  }
}
public class CreateCpsettingFileValidator : AbstractValidator<IFormFile>
{
  public CreateCpsettingFileValidator()
  {
    RuleFor(v => v.FileName).NotNull().NotEmpty();
  }
}
