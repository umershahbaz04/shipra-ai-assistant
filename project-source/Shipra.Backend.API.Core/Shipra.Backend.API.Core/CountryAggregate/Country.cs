namespace Shipra.Backend.API.Core.CountryAggregate;
public class Country
{
  public Country()
  {
  }
  public int CountryId { get; set; }
  public string? Code { get; set; }
  public string? ISOA2Code { get; set; }
  public string? MapCountryCode { get; set; }
  public string? MobileCode { get; set; }
  public string? Name { get; set; }
  public string? NameArabic { get; set; }
  public string? AddressingScheme { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
}
