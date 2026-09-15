using Shipra.Backend.API.Application.Common.Enums;

namespace Shipra.Backend.API.Application.Common.Constants;
public static class GetAirwaysBill
{
  public static List<AwbTypeModel> GetList()
  {
    return new List<AwbTypeModel>()
    {
      new AwbTypeModel(){ AwbTypeId = (int)EnumAwbType.Awb4x6TypeId,Value = "GetAwb4x6Label",DisplayOrder=1},
      new AwbTypeModel(){ AwbTypeId = (int)EnumAwbType.CarrierAwbTypeId,Value = "GetCarrierLabel",DisplayOrder=4},

    };
  }
}
public class AwbTypeModel
{
  public int AwbTypeId { get; set; }
  public string? Value { get; set; }
  public int DisplayOrder { get; set; }
}
