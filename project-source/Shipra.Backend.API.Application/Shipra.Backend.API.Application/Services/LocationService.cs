using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Services;
public class LocationManager
{
  private readonly ICountryRepository _countryRepository;

  public LocationManager(ICountryRepository countryRepository)
  {
    _countryRepository = countryRepository;
  }

  public async Task<List<Country>> GetAllCountriesAsync()
  {
    return await _countryRepository.GetAllCountries();
  }

  public async Task<List<City>> GetAllCitiesAsync()
  {
    return await _countryRepository.GetAllCities();
  }

  public async Task<List<Province>> GetAllProvincesAsync()
  {
    return await _countryRepository.GetAllProvinces();
  }

  public async Task<List<State>> GetAllStatesAsync()
  {
    return await _countryRepository.GetAllStates();
  }

  public async Task<List<Area>> GetAllAreasAsync()
  {
    return await _countryRepository.GetAllAreas();
  }

  public async Task<List<PinCode>> GetAllPinCodesAsync()
  {
    return await _countryRepository.GetAllPinCodes();
  }

  // Optionally, you could create a method to get all data at once
  public async Task<(List<Country> countries, List<City> cities, List<Province> provinces, List<State> states, List<Area> areas, List<PinCode> pinCodes)> GetAllLocationDataAsync()
  {
    var countries = await GetAllCountriesAsync();
    var cities = await GetAllCitiesAsync();
    var provinces = await GetAllProvincesAsync();
    var states = await GetAllStatesAsync();
    var areas = await GetAllAreasAsync();
    var pinCodes = await GetAllPinCodesAsync();

    return (countries, cities, provinces, states, areas, pinCodes);
  }
}
