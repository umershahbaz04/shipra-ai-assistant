using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
public class LowQuantityProductStockFilterModel : CommonFilterModel
{ 
  public string? StoreId { get; set; }
  public int? ProductStationId { get; set; }
  public bool? IsActive { get; set; }
  public int? AvailableQty { get; set; }
}
