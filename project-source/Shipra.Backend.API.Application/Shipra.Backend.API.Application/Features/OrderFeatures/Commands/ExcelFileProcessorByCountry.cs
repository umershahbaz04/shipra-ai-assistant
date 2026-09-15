using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
public static class UploadExcelFileConstant
{
  public const string AddProductDataWithCityAndArea = "AddProductDataWithCityAndArea";
  public const string AddProductDataWithStateCityPincode = "AddProductDataWithStateCityPincode";
  public const string AddProductDataWithProvinceCityPincode = "AddProductDataWithProvinceCityPincode";
  public const string AddProductDataWithStatCity = "AddProductDataWithStatCity";
}
public class ExcelFileProcessorByCountry
{
  // Define delegates that match the signature of the data processing methods
  private delegate string ProcessDataMethod();

  // Dictionary mapping country IDs to their corresponding data processing methods
  private readonly Dictionary<int, ProcessDataMethod> _countryDataProcessors;

  public ExcelFileProcessorByCountry()
  {
    _countryDataProcessors = new Dictionary<int, ProcessDataMethod>();

    // Map multiple country IDs to the same processing method
    AddCountryMappings(new[] { (int)EnumCountry.UAE, (int)EnumCountry.SaudiArabia, (int)EnumCountry.Qatar }, AddProductDataWithCityAndArea);
    AddCountryMappings(new[] { (int)EnumCountry.India }, AddProductDataWithStateCityPincode);
    AddCountryMappings(new[] { (int)EnumCountry.Pakistan }, AddProductDataWithProvinceCityPincode);
    AddCountryMappings(new[] { (int)EnumCountry.Oman }, AddProductDataWithStatCity);
  }

  // Method to add multiple countries to the dictionary
  private void AddCountryMappings(IEnumerable<int> countryIds, ProcessDataMethod method)
  {
    foreach (var countryId in countryIds)
    {
      _countryDataProcessors[countryId] = method;
    }
  }

  // Method to process Excel data based on CountryId
  public string ProcessExcelData(int countryId)
  {
    if (_countryDataProcessors.TryGetValue(countryId, out ProcessDataMethod? processMethod))
    {
      return processMethod()!;
    }
    else
    {
      throw new ArgumentException("Invalid CountryId provided.");
    }
  }

  // Example methods for different countries (replace these with your actual methods)
  private string AddProductDataWithCityAndArea()
  {
    // Implementation for UAE and Saudi Arabia
    return UploadExcelFileConstant.AddProductDataWithCityAndArea; // Replace with actual implementation
  }

  private string AddProductDataWithStateCityPincode()
  {
    // Implementation for India
    return UploadExcelFileConstant.AddProductDataWithStateCityPincode; // Replace with actual implementation
  }

  private string AddProductDataWithProvinceCityPincode()
  {
    // Implementation for Pakistan
    return UploadExcelFileConstant.AddProductDataWithProvinceCityPincode; // Replace with actual implementation
  }
  private string AddProductDataWithStatCity()
  {
    // Implementation for Pakistan
    return UploadExcelFileConstant.AddProductDataWithStatCity; // Replace with actual implementation
  }
}
