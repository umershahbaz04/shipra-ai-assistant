using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;
public class UpdateStoreAddressRequestModel : AddressRequestDTO
{
  public int StoreAddressId { get; set; }
}
