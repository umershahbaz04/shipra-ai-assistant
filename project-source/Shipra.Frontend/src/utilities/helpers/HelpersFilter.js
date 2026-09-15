import { useEffect, useState } from "react";
import {
  AddUpdateTableConfiguration,
  GetAllActivedCarrier,
  GetAllCarrierTrackingStatusForSelection,
  GetAllCitiesByRegionIds,
  GetAllEmployeeColumnConfiguration,
  GetAllEntityMetaFieldLookup,
  GetAllFullFillmentStatusLookup,
  GetAllOrderTypeLookup,
  GetAllPaymentMethodLookup,
  GetAllPaymentStatusLookup,
  GetAllRegionbyCountryIds,
  GetAllSaleChannelsByStoreId,
  GetAllStationLookup,
  GetBrandingById,
  GetStoresForSelection,
} from "../../api/AxiosInterceptors";
import { getThisKeyCookie, setThisKeyCookie } from "../cookies";
import { successNotification } from "../toast";
import { fetchMethod } from "./Helpers";
import { EnumMetaField } from "../enum";
export const useGetAllCarrierStatus = () => {
  const [allCarrierStatus, setAllCarrierStatus] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selectedCarrierStatus, setSelectedCarrierStatus] = useState();
  const [carrierStatusId, setCarrierStatusId] = useState();
  // handleGetAllCities
  const handleAllCarrierTrackingStatusForSelection = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() =>
      GetAllCarrierTrackingStatusForSelection(),
    );
    if (response) {
      setAllCarrierStatus(response.result);
    }
    setLoading(false);
  };
  const reset = () => {
    if (allCarrierStatus && allCarrierStatus.length > 0) {
      setSelectedCarrierStatus(allCarrierStatus[0]);
    }
  };
  useEffect(() => {
    reset();
  }, [allCarrierStatus]);
  useEffect(() => {
    if (selectedCarrierStatus) {
      setCarrierStatusId(selectedCarrierStatus?.carrierTrackingStatusId);
    }
  }, [selectedCarrierStatus]);

  useEffect(() => {
    handleAllCarrierTrackingStatusForSelection();
  }, []);
  return {
    loading,
    allCarrierStatus,
    selectedCarrierStatus,
    setSelectedCarrierStatus,
    carrierStatusId,
    handleResetCarrier: reset,
  };
};
export const handleGetAllSaleChannelsByStoreId = async (storeId) => {
  let data = [];
  const { response } = await fetchMethod(() =>
    GetAllSaleChannelsByStoreId(storeId),
  );
  if (response) {
    data = response.result;
  }
  return data;
};

export const useGetAllEmployeeColumnConfiguration = (
  setColumnVisibilityModel,
  targetTable,
) => {
  useEffect(() => {
    const cookieKey = targetTable;
    const fetchColumnConfig = async () => {
      try {
        const cookieData = getThisKeyCookie(cookieKey);
        if (cookieData) {
          const parsedConfig = JSON.parse(cookieData);
          setColumnVisibilityModel(parsedConfig);
          return;
        }
        const response = await GetAllEmployeeColumnConfiguration();
        const tableData = response.data.result;
        const findTable = tableData.find((dt) => dt.tableName === targetTable);
        if (findTable) {
          const configData = JSON.parse(findTable.config);
          const columnVisibility = configData?.columnVisibilityModel || {};
          setColumnVisibilityModel(columnVisibility);
          setThisKeyCookie(cookieKey, JSON.stringify(columnVisibility));
        }
      } catch (err) {
        console.error("Failed to load column configuration:", err);
      }
    };
    fetchColumnConfig();
  }, [setColumnVisibilityModel, targetTable]);
};

export const getBranding = async () => {
  const ClientId = window?.__CONTEXT__?.ClientId;
  const brandingData = sessionStorage.getItem("Branding");
  const decoded = brandingData ? decodeURIComponent(brandingData) : null;
  const brandingSettings = {
    brandingSettingId: 1,
    clientId: {
      value: "c103dd6a-a081-70c6-dbc4-a09ca2b08fdc",
    },
    logoUrl:
      "/logo.svg",
    faviconUrl: null,
    primaryColor: null,
    secondaryColor: null,
    theme: "light",
  };

  try {
    sessionStorage.setItem("Branding", JSON.stringify(brandingSettings));
    // if (!decoded) {
    //   const respose = await GetBrandingById(ClientId);
    //   if (respose?.data?.isSuccess) {
    //     sessionStorage.setItem("Branding", JSON.stringify(respose.data.result));
    //   }
    // }
  } catch (e) { }
};
export const getBrandingAfterDataChange = async () => {
  const ClientId = window?.__CONTEXT__?.ClientId;
  localStorage.setItem("Branding", null);
  try {
    const respose = await GetBrandingById(ClientId);
    localStorage.setItem("Branding", JSON.stringify(respose.data.result));
  } catch (e) { }
};

export const useSaveColumnConfig = (setColumnVisibilityModel, targetTable) => {
  const handleColumnVisibilityModelChange = async (data) => {
    setColumnVisibilityModel(data);
    try {
      const response = await AddUpdateTableConfiguration(targetTable, data);
      if (response.data.isSuccess) {
        successNotification("Column config saved");
      }
    } catch (e) {
      console.error("Failed to save column config:", e);
    }
  };

  return handleColumnVisibilityModelChange;
};

export const useGetAllStationLookup = () => {
  const [allProductStations, setAllProductStations] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleAllGetAllStationLookup = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetAllStationLookup());
    if (response) {
      setAllProductStations(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleAllGetAllStationLookup();
  }, []);
  return { loading, allProductStations };
};
export const useGetAllOrderType = () => {
  const [allOrderType, setAllOrderType] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleAllGetAllOrderType = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetAllOrderTypeLookup());
    if (response) {
      setAllOrderType(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleAllGetAllOrderType();
  }, []);
  return { loading, allOrderType };
};
export const useGetAllActivedCarrier = () => {
  const [allActiveCarriers, setAllActiveCarriers] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleGetAllActivedCarrier = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetAllActivedCarrier());
    if (response) {
      setAllActiveCarriers(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllActivedCarrier();
  }, []);
  return { loading, allActiveCarriers };
};
export const useGetAllFullFillmentStatus = () => {
  const [allAllFullFillmentStatus, setAllAllFullFillmentStatus] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleGetAllFullFillmentStatus = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() =>
      GetAllFullFillmentStatusLookup(),
    );
    if (response) {
      setAllAllFullFillmentStatus(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllFullFillmentStatus();
  }, []);
  return { loading, allAllFullFillmentStatus };
};
export const useGetAllPaymentStatus = () => {
  const [allPaymentStatus, setAllPaymentStatus] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleGetAllPaymentStatus = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetAllPaymentStatusLookup());
    if (response) {
      setAllPaymentStatus(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllPaymentStatus();
  }, []);
  return { loading, allPaymentStatus };
};
export const useAllPaymentMethod = () => {
  const [allPaymentMethod, setAllPaymentMethod] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleAllPaymentMethod = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetAllPaymentMethodLookup());
    if (response) {
      setAllPaymentMethod(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleAllPaymentMethod();
  }, []);
  return { loading, allPaymentMethod };
};
export const useGetAllStores = () => {
  const [allStores, setAllStores] = useState([]);
  const [loading, setLoading] = useState(false);

  const [storeId, setStoreId] = useState();

  const [selectedStore, setSelctedStore] = useState();

  // handleGetAllCities
  const handleGetAllStores = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetStoresForSelection());
    if (response) {
      setAllStores(response.result);
    }
    setLoading(false);
  };
  const resetSelctedStore = () => {
    if (allStores && allStores.length > 0) {
      setSelctedStore([]);
    }
  };
  useEffect(() => {
    handleGetAllStores();
  }, []);
  useEffect(() => {
    if (selectedStore) {
      setStoreId(selectedStore.storeId);
    }
  }, [selectedStore]);
  useEffect(() => {
    if (allStores && allStores.length > 0) {
      resetSelctedStore();
    }
  }, [allStores]);
  return {
    loading,
    allStores,
    selectedStore,
    setSelctedStore,
    storeId,
    resetSelctedStore,
  };
};
export const useAllStations = () => {
  const [allStations, setAllStations] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleAllStations = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetAllStationLookup());
    if (response) {
      setAllStations(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleAllStations();
  }, []);
  return { loading, allStations };
};
//#region all region by country ids
export const useGetAllRegionbyCountryIds = () => {
  const [allRegions, setAllRegions] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selectedRegions, setSelectedRegions] = useState([]);

  const handleGetAllRegionsByIds = async (contryIds) => {
    setLoading(true);
    const data = await handleGetAllRegionbyCountryIds(contryIds);
    if (data) {
      setAllRegions(data);
    }
    setLoading(false);
  };
  return {
    loading,
    allRegions,
    handleGetAllRegionsByIds,
    selectedRegions,
    setSelectedRegions,
    setAllRegions,
  };
};

export const handleGetAllRegionbyCountryIds = async (selectedCountry) => {
  let data = [];
  const { response } = await fetchMethod(() =>
    GetAllRegionbyCountryIds(selectedCountry),
  );
  if (response) {
    data = response.result;
  }
  return data;
};
//#endregion
//#region get all city by ids
export const useGetAllCitiesByRegionIds = () => {
  const [allCities, setAllCities] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selectedCities, setSelectedCities] = useState([]);
  // handleGetAllCities
  const handleGetAllCitiesByIds = async (regionIds) => {
    setLoading(true);
    const data = await handleGetAllCitiesByRegionIds(regionIds);
    if (data) {
      setAllCities(data);
    }
    setLoading(false);
  };
  return {
    loading,
    allCities,
    handleGetAllCitiesByIds,
    selectedCities,
    setSelectedCities,
    setAllCities,
  };
};

export const handleGetAllCitiesByRegionIds = async (selectedRegion) => {
  let data = [];
  const { response } = await fetchMethod(() =>
    GetAllCitiesByRegionIds(selectedRegion),
  );
  if (response) {
    data = response.result;
  }
  return data;
};
//#endregion
export const useGetAllMetafields = (id) => {
  const [metafields, setMetafields] = useState([]);
  const [loading, setLoading] = useState(false);
  const showMetaField = getThisKeyCookie("isShowMetafield") === "true";
  const handleGetAllMetafields = async () => {
    setLoading(true);
    // if (!showMetaField && id) {
    //   setMetafields([]);
    //   setLoading(false);
    //   return;
    // }
    const response = await GetAllEntityMetaFieldLookup();
    if (response?.data?.isSuccess) {
      let result = response?.data?.result;
      const parsedMetafields = result.map((item) => ({
        ...item,
        settingConfig: JSON.parse(item.settingConfig),
      }));
      const filteredMetafields = id
        ? parsedMetafields.filter((item) => item.entityMetaFieldId === id)
        : parsedMetafields;

      setMetafields(filteredMetafields);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllMetafields();
  }, [id, showMetaField]);

  return { loading, metafields, setMetafields };
};
