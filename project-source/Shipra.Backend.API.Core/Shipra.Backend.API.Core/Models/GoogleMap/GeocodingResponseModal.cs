namespace Shipra.Backend.API.Core.Models.GoogleMap;
public class GeocodingResponseModal
{
  public GeocodingResultModal[]? results { get; set; }
  public string? status { get; set; }
}
public class GeocodingResultModal
{
  public GeocodingAddressComponent[]? address_components { get; set; }
  public string? formatted_address { get; set; }
  public GeocodingGeometry? geometry { get; set; }
  public string? place_id { get; set; } 
  public string[]? types { get; set; }
}
public class GeocodingAddressComponent
{
  public string? long_name { get; set; }
  public string? short_name { get; set; }
  public string[]? types { get; set; }
}
public class GeocodingGeometry
{
  public GeocodingLocation? location { get; set; }
  public string? location_type { get; set; } 
}
public class GeocodingLocation
{
  public double lat { get; set; }
  public double lng { get; set; }
}

public class CombinedAddressModal
{
  public string? Country { get; set; } = string.Empty;
  public string? City { get; set; } = string.Empty;
  public string? Province { get; set; } = string.Empty;
  public string? State { get; set; } = string.Empty;
  public string? PinCode { get; set; } = string.Empty;
  public string? Neighborhood { get; set; } = string.Empty; 
  public string? Street { get; set; } = string.Empty;
}
public class CombinedAddressResponseModal
{
  //never change this pattren
  public dynamic? Country { get; set; }
  public dynamic? City { get; set; }
  public dynamic? Province { get; set; }
  public dynamic? State { get; set; }
  public dynamic? PinCode { get; set; }
  public dynamic? Area { get; set; } 
  public string? StreetAddress { get; set; }
}
