using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;
public class CreateStoreRequestModel
{
  public string? StoreName { get; set; }
  public string? StoreCompany { get; set; }
  public string? CustomerServiceNo { get; set; }
  public string? Phone { get; set; }
  public string? Email { get; set; }
  public string? Urls { get; set; }
  public string? LicenseNo { get; set; }
  public string? StoreImage { get; set; }
  public string? UserName { get; set; }
  public string? Password { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public AddressRequestDTO? StoreAddress { get; set; }
}
