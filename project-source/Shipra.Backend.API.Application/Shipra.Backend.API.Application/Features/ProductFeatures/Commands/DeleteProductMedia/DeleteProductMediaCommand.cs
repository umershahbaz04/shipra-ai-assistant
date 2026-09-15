using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ExampleFeatures.Commands;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.DeleteProductMedia;
public class DeleteProductMediaCommand : IRequest<ServiceResultDTO>
{
  public long? ProductMediaId { get; set; }
}
public class DeleteProductMediaCommandHandler : Common.RequestHandlerBase<DeleteProductMediaCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public DeleteProductMediaCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<DeleteProductMediaCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteProductMediaCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var productMedia = await _productRepository.GetProductMediaById(request.ProductMediaId.GetValueOrDefault());
      if (productMedia is null)
      {
        throw new EntityNotFoundException("ProductMedia ", request.ProductMediaId!);
      }
      await _productRepository.DeleteProductMediaByID(productMedia);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      return serviceResult;
    }

  }
}
public class DeleteProductMediaCommandValidator : AbstractValidator<DeleteProductMediaCommand>
{
  public DeleteProductMediaCommandValidator()
  {
    RuleFor(e => e.ProductMediaId).NotEmpty();
  }
}
