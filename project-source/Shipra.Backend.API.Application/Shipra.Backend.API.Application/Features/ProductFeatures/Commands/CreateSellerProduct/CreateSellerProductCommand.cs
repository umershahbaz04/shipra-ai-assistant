using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateSellerProduct;
public class CreateSellerProductCommand : IRequest<ServiceResultDTO>
{
  public List<SellerProductRequestModel>? Products { get; set; }
}

public class SellerProductRequestModel
{
  public string? ClientId { get; set; }
  public string? ProductId { get; set; }
}
