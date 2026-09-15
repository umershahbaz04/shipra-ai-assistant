using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.GenerateProductLinkToken;
public class GenerateProductLinkTokenCommand : IRequest<ServiceResultDTO>
{
  public string? ProductId { get; set; }
  public int? StoreId { get; set; }
}
public class GenerateProductLinkTokenCommandHandler : RequestHandlerBase<GenerateProductLinkTokenCommand, ServiceResultDTO>
{
  private readonly ICatalogueRepository _catalogueRepository;
  private readonly IProductRepository _productRepository;

  public GenerateProductLinkTokenCommandHandler(ICatalogueRepository catalogueRepository, IProductRepository productRepository,IServiceProvider serviceProvider, ILogger<GenerateProductLinkTokenCommandHandler> logger) : base(serviceProvider, logger)
  {
    _catalogueRepository = catalogueRepository;
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GenerateProductLinkTokenCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var orderTackingPageUrl = _configuration.GetValue<string>("OrderTackingPageUrl");

      var productId = new ProductId(new Guid(request!.ProductId!));

      ProductLinkToken? productLinkToken = await _productRepository.GetProductLinkTokenByProductId(productId,request.StoreId, _currentUser.ClientId!);

      string token = string.Empty;

      if (productLinkToken is null)
      {
        token = await _productRepository.GenerateUniqueShortTokenForProductAsync(_currentUser.ClientId!);
        // create
        productLinkToken = ProductLinkToken.Create(productId,request.StoreId,_currentUser.ClientId!,token,_currentUser.EmployeeId!);
        await _productRepository.CreateProductLinkToken(productLinkToken);
      }
      else
      {
        token = productLinkToken.Token!;
      } 
      var url = $"{orderTackingPageUrl}/{token}";

      serviceResult = new ServiceResultDTO(new 
      {
        OrderLink = url,
      });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GenerateProductLinkTokenCommandValidator : AbstractValidator<GenerateProductLinkTokenCommand>
{
  public GenerateProductLinkTokenCommandValidator()
  {
    RuleFor(x => x.ProductId).NotEmpty().NotNull();
  } 
}
