using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.DeleteProductCategoryCommand;
public class DeleteProductCategoryCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public int ProductCategoryId { get; set; }
}
