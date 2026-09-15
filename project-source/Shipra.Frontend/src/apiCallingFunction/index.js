import {
  GetAllCountry,
  GetAllRegionbyCountryId,
  GetAllStationLookup,
  GetCityByRegionId,
  GetStoresForSelection,
} from "../api/AxiosInterceptors";

export const getAllCountryFunc = async () => {
  let data = [];
  let res = await GetAllCountry({});
  if (res.data.result != null) data = res?.data?.result;
  return data;
};
export const getAllRegionbyCountryIdFunc = async (countryId) => {
  let data = [];
  let res = await GetAllRegionbyCountryId(countryId);
  if (res.data.result != null) data = res?.data?.result;
  return data;
};
export const getCityByRegionIdFunc = async (regionId) => {
  let data = [];

  let res = await GetCityByRegionId(regionId);
  if (res.data.result != null) data = res?.data?.result;
  return data;
};

export const getAllStoresForSelectionFunc = async () => {
  let data = [];

  let res = await GetStoresForSelection();
  if (res.data.result != null) data = res?.data?.result;
  return data;
};
export const getAllStationLookupFunc = async () => {
  let data = [];

  let res = await GetAllStationLookup();
  if (res.data.result != null) data = res?.data?.result;
  return data;
};
