using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;
public class UpdateOrderClientSideRequestModel : CreateUpdateOrderCommonRequestModel
{
  public OrderAddressClientSideModel? OrderAddress { get; set; } = new(); 
}
