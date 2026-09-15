using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
public class SyncInventoryModel
{
  public int ProductStockId { get; set; }
  public string? Sku { get; set; }
  public int Quantity { get; set; }
  public int ProdStationId { get; set; } 
}
public class SyncInventoryResponseModel : SyncInventoryModel
{
  public bool IsSuccess { get; set; }
  public string? Error { get; set; }
}
