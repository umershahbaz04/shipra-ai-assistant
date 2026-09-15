import axios from "axios";
import setLogoutCookie from "../pages/loginSignup/setLogoutCookie";
import UtilityClass from "../utilities/UtilityClass";
import { getThisKeyCookie } from "../utilities/cookies";
import {
  EnumChangeFilterModelApiUrls,
  EnumRoutesUrls,
} from "../utilities/enum";

let baseURL = `${process.env.REACT_APP_Prod_BaseUrl}/api/`;

if (process.env.NODE_ENV === "development") {
  // baseURL = "http://localhost:5000/api/";
} else if (process.env.NODE_ENV === "production") {
  //#region staging url set
  baseURL = `${process.env.REACT_APP_Prod_BaseUrl}/api/`;
  //#endregion
}
baseURL = "https://stage-api.shipra.io/api/";

// baseURL = "http://localhost:5000/api/";
// baseURL = "http://stage-api.shipra.io/api";
// baseURL = "https://api.shipra.io/api";
export const Axios = axios.create({
  baseURL: baseURL,
  signal: "",
});
export const reMoveStorageOnUnathorization = () => {
  setLogoutCookie();
};

Axios.interceptors.request.use((req) => {
  const token = getThisKeyCookie("access_token");
  const IdToken = getThisKeyCookie("id_token");
  localStorage.isDeploying = false;
  if (JSON.parse(localStorage.isDeploying)) {
    window.location = EnumRoutesUrls.MAINTENACE;
  }
  const isLoginUrl = req.url === "/UserManagement/Login";
  const isLogoutUrl = req.url === "/UserManagement/Logout";
  if (isLoginUrl || isLogoutUrl) {
    return req;
  }
  if (token) {
    req.headers["Authorization"] = "Bearer " + token; // for Spring Boot back-end
    req.headers["x-access-token"] = token; // for Node.js Express back-end
    req.headers["x-id-token"] = IdToken;
  }
  return req;
});

///////////////////////////////

Axios.interceptors.response.use(
  (res) => {
    const response = res.data;
    const isConnectionStringError = response?.errors?.Error?.includes(
      "The ConnectionString property has not been initialized.",
    );
    if (!response?.isSuccess && isConnectionStringError) {
      reMoveStorageOnUnathorization();
      window.location.pathname = EnumRoutesUrls.LOG_IN;
    }
    return res;
  },
  async (err) => {
    const originalConfig = err?.config;
    const statusCode = err?.response?.status;
    const isAuthError = statusCode === 401 || statusCode === 419;
    if (originalConfig?.url !== "/login" && err?.response) {
      if (isAuthError) {
        reMoveStorageOnUnathorization();
        window.location.pathname = EnumRoutesUrls.LOG_IN;
      }
    }
    return Promise.reject(err);
  },
);

// Axios.interceptors.response.use(
//   (res) => {
//     return res;
//   },
//   async (err) => {
//     const originalConfig = err.config;
//     if (originalConfig.url !== "/login" && err.response) {
//       // Access Token was expired
//       if (err.response.status === 401 && !originalConfig._retry) {
//         originalConfig._retry = true;
//         try {
//           let body = {
//             UserName: getThisKeyCookie("user_name"),
//             RefreshToken: getThisKeyCookie("refresh_token"),
//           };
//           const res = await Axios.post(
//             `/UserManagement/GetAccessTokenWithRefreshToken`,
//             body,
//             {
//               headers: {
//                 "Content-Type": "application/json",
//               },
//             }
//           );
//           if (res.data.isSuccess) {
//             setThisKeyCookie("access_token", res.data.result.access_token);
//             setThisKeyCookie("refresh_token", res.data.result.refresh_token);
//             setThisKeyCookie("user_name", res.data.result.user_name);
//             setThisKeyCookie("id_token", res.data.result.id_token);
//             setThisKeyCookie(
//               "expires_in",
//               res.data.result.expires_in * 1000 + Date.now()
//             );
//           }
//           if (Object.keys(res.data.errors).length > 0) {
//             if (res.data.errors.NotAuthorizedException) {
//               if (
//                 res.data.errors.NotAuthorizedException[0]?.includes("revoked")
//               ) {
//                 //logout
//                 let body = {
//                   accessToken: getThisKeyCookie("access_token"),
//                 };
//                 logout(body)
//                   .then((res) => {
//                     reMoveStorageOnUnathorization();
//                   })
//                   .catch((e) => {
//                     console.log("e", e);
//                   });
//               }
//             } else {
//               reMoveStorageOnUnathorization();
//             }
//             return;
//           }
//           return Axios(originalConfig);
//         } catch (_error) {
//           console.log(_error);
//           return Promise.reject(_error);
//         }
//       }
//       if (err.response.status === 403) {
//         window.location.pathname = EnumRoutesUrls.NOTFOUND_403;
//       }
//     }
//     return Promise.reject(err);
//   }
// );

//////////////////////////////
// console.log("getThisKeyCookie", getThisKeyCookie("access_token"));

// GetCarrierActivityWithDetail
export const GetCarrierActivityWithDetail = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetCarrierActivityWithDetail`, body);
};

export const GetCarrierStats = (filterModel) => {
  let createdFrom = UtilityClass.getFormatedDateWithoutTime(
    filterModel.createdFrom,
  );
  let createdTo = UtilityClass.getFormatedDateWithoutTime(
    filterModel.createdTo,
  );
  const body = {
    filterModel: {
      createdFrom: createdFrom,
      createdTo: createdTo,
    },
  };
  return Axios.post(`/Dashboard/GetCarrierStats`, body);
};
// GetTotalInprogressOrderCountWithCarrier
export const GetTotalInprogressOrderCountWithCarrier = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalInprogressOrderCountWithCarrier`, body);
};
// GetTotalCompletedOrderCountWithCarrier
export const GetTotalCompletedOrderCountWithCarrier = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalCompletedOrderCountWithCarrier`, body);
};
// GetTotalStoreCount
export const GetTotalStoreCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalStoreCount`, body);
};
// GetDelieveredOrderCount
export const GetDelieveredOrderCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetDelieveredOrderCount`, body);
};
// GetFulFillableOrderCount
export const GetFulFillableOrderCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetFulFillableOrderCount`, body);
};
// GetRegularOrderCount
export const GetRegularOrderCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetRegularOrderCount`, body);
};
// GetInProgressOrderCount
export const GetInProgressOrderCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetInProgressOrderCount`, body);
};
// GetReturnedOrderCount
export const GetReturnedOrderCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetReturnedOrderCount`, body);
};
// GetTotalCollected
export const GetTotalCollected = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalCollected`, body);
};
// GetTotalStockValue
export const GetTotalStockValue = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalStockValue`, body);
};

// GetTotalPurchaseStockValue
export const GetTotalPurchaseStockValue = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalPurchaseStockValue`, body);
};
// GetTotalUncollected
export const GetTotalUncollected = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalUncollected`, body);
};
// GetTotalCarriers
export const GetTotalCarriers = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalCarriers`, body);
};
// GetActiveCarriers
export const GetActiveCarriers = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetActiveCarriers`, body);
};
// GetProductCounts
export const GetProductCounts = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetProductCounts`, body);
};
export const GetSaleDashboardChannels = (body) => {
  return Axios.post(`/Dashboard/GetSaleDashboardChannels`, body);
};
export const GetSaleDashboardProducts = (body) => {
  return Axios.post(`/Dashboard/GetSaleDashboardProducts`, body);
};
export const GetSaleDashboardStores = (body = {}) => {
  return Axios.post(`/Dashboard/GetSaleDashboardStores`, body);
};
export const GetCarrierDashboardStats = (body) => {
  return Axios.post(`/Dashboard/GetCarrierDashboardStats`, body);
};
// GetAllProductVariantsCount
export const GetAllProductVariantsCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetAllProductVariantsCount`, body);
};
// GetTotalNoofOrdersPlacedCounts
export const GetTotalNoofOrdersPlacedCounts = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTotalNoofOrdersPlacedCounts`, body);
};
// GetToBeShippedCount
export const GetToBeShippedCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetToBeShippedCount`, body);
};
// GetToBePackedCount
export const GetToBePackedCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetToBePackedCount`, body);
};
// GetTopSellingItemsCount
export const GetTopSellingItemsCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetTopSellingItemsCount`, body);
};
// GetRegularShipmentCount
export const GetRegularShipmentCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetRegularShipmentCount`, body);
};
// GetLowStockItemsCount
export const GetLowStockItemsCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetLowStockItemsCount`, body);
};
// GetDeliveryRatioCount
export const GetDeliveryRatioCount = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Dashboard/GetDeliveryRatioCount`, body);
};
// GetAllCODCollectionPendingsMyCarrier
export const GetAllCODCollectionPendingsMyCarrier = (params) => {
  return Axios.post(`/Delivery/GetAllCODCollectionPendingsMyCarrier`, params);
};
export const GetOrderAddressLatLngByDeliveryNoteId = (DeliveryNoteId) => {
  return Axios.get(
    `/Delivery/GetOrderAddressLatLngByDeliveryNoteId?DeliveryNoteId=${DeliveryNoteId}`,
  );
};
export const DeleteDeliveryNoteById = (body) => {
  return Axios.post(`/Delivery/DeleteDeliveryNoteById`, body);
};
export const MergeDeliveryNotes = (body) => {
  return Axios.post(`/Delivery/MergeDeliveryNotes`, body);
};

// GetAllCODCollectionsMyCarrier
export const GetAllCODCollectionsMyCarrier = (filterModel) => {
  const body = {
    filterModel: filterModel,
  };
  return Axios.post(`/Delivery/GetAllCODCollectionsMyCarrier`, body);
};

export const ExcelExportCODCollectionPendingsMyCarrier = (body) => {
  return Axios.post(
    `/DriverAccount/ExcelExportCODCollectionPendingsMyCarrier`,
    body,
    {
      headers: {
        "Content-Type": "application/json",
      },
      responseType: "blob",
    },
  );
};
export const UpdateDriverReceivableStatusPaid = (body) => {
  return Axios.post(`/DriverAccount/UpdateDriverReceivableStatusPaid`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateExpense = (body) => {
  return Axios.post(`/DriverAccount/UpdateExpense`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateDriverReceivableStatusUnPaid = (body) => {
  return Axios.post(`/DriverAccount/UpdateDriverReceivableStatusUnPaid`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateDriverReceivable = (body) => {
  return Axios.post(`/DriverAccount/UpdateDriverReceivable`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const ExcelExportCODCollectionsMyCarrierQuery = (body) => {
  return Axios.post(
    `/DriverAccount/ExcelExportCODCollectionsMyCarrierQuery`,
    body,
    {
      headers: {
        "Content-Type": "application/json",
      },
      responseType: "blob",
    },
  );
};
//Client Endpoints

export const Login = (body) => {
  return Axios.post(`/UserManagement/Login`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const AdminSetUserPassword = (body) => {
  return Axios.post(`/UserManagement/AdminSetUserPassword`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetClientProfile = () => {
  return Axios.get(`/Client/GetClientProfile`);
};
// UpdateClient
export const UpdateClient = (body) => {
  return Axios.post(`/Client/UpdateClient`, body);
};
export const UpdateClientImage = (body) => {
  return Axios.post(`/UserManagement/UploadClientImage`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const UpdatePassword = (oldPassword, newPassword) => {
  const body = {
    oldPassword: oldPassword,
    newPassword: newPassword,
  };
  return Axios.post(`/UserManagement/UpdatePassword`, body);
};

export const GetAccessTokenWithRefreshToken = (body) => {
  return Axios.post(`/UserManagement/GetAccessTokenWithRefreshToken`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const logout = (body) => {
  return Axios.post(`/UserManagement/Logout`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const Signup = (body) => {
  return Axios.post(`/UserManagement/Signup`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CheckEmailAvailability = (email) => {
  const body = {
    email,
  };
  return Axios.post(`/UserManagement/CheckEmailAvailability`, body);
};
export const ConfirmSignUpUser = (body) => {
  return Axios.post(`/UserManagement/ConfirmSignUpUser`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CheckUsernameAvailability = (body) => {
  return Axios.post(`/UserManagement/CheckUsernameAvailability`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const ResendConfirmationCode = (body) => {
  return Axios.post(`/UserManagement/ResendConfirmationCode`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UploadClientImage = (body) => {
  return Axios.post(`/Client/UploadClientImage`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const ForgotPassword = (body) => {
  return Axios.post(`/UserManagement/ForgotPassword`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const ConfirmForgotPassword = (body) => {
  return Axios.post(`/UserManagement/ConfirmForgotPassword`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

// Country Endpoints

export const GetAllCountry = () => {
  return Axios.get(`/Country/GetAllCountry`);
};

export const GetAllCitiesByRegionIds = (id) => {
  return Axios.get(`/Country/GetAllCitiesByRegionIds/?RegionIds=${id}`);
};
export const GetAllRegionbyCountryId = (id) => {
  return Axios.get(`/Country/GetCitiesByCountryId/?CountryId=${id}`);
};
export const GetAllCitiesbyCountryId = (id) => {
  return Axios.get(`/Country/GetCitiesByCountryId/?CountryId=${id}`);
};
export const GetCitiesByProvinceAndCountryId = (CountryId, ProvinceId) => {
  return Axios.get(
    `/Country/GetCitiesByProvinceAndCountryId?CountryId=${CountryId}&ProvinceId=${ProvinceId}`,
  );
};
export const GetCitiesByStateAndCountryId = (CountryId, StateId) => {
  return Axios.get(
    `/Country/GetCitiesByStateAndCountryId?CountryId=${CountryId}&StateId=${StateId}`,
  );
};
export const GetAllRegionbyCountryIds = (id) => {
  return Axios.get(`/Country/GetAllRegionbyCountryIds/?CountryIds=${id}`);
};

export const GetCityByRegionId = (id) => {
  return Axios.get(`/Country/GetCityByRegionId/?RegionId=${id}`);
};
export const GetAreasByCityId = (id) => {
  return Axios.get(`/Country/GetAreasByCityId/?CityId=${id}`);
};
export const GetStatesByCountryId = (id) => {
  return Axios.get(`/Country/GetStatesByCountryId?CountryId=${id}`);
};
export const GetProvinceByCountryId = (id) => {
  return Axios.get(`/Country/GetProvinceByCountryId?CountryId=${id}`);
};
export const GetPinCodesByCityId = (id) => {
  return Axios.get(`/Country/GetPinCodesByCityId?CityId=${id}`);
};

export const GetAllRegions = () => {
  return Axios.get(`/Country/GetAllRegions`);
};

// Store Endpoints

export const UploadStoreImage = (body) => {
  return Axios.post(`/Store/UploadStoreImage`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};
export const UploadEmployeeImage = (body) => {
  return Axios.post(`/Employee/UploadEmployeeImage`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const GetAllEmployeeType = () => {
  return Axios.get(`/Employee/GetAllEmployeeType`);
};

export const GetNextEmployeeUserName = (Name) => {
  const body = {
    name: Name,
  };
  return Axios.post(`/Employee/GetNextEmployeeUserName`, body);
};
export const GetGoogleMapRestrictedCountry = (body) => {
  return Axios.get(`/CommonLookup/GetGoogleMapRestrictedCountry`);
};
export const GetClientInfo = (body) => {
  return Axios.get(`/CommonLookup/GetClientInfo`);
};
export const CreateStore = (body) => {
  return Axios.post(`/Store/CreateStore`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CreateStoreChannel = (body) => {
  return Axios.post(`/Channel/CreateChannel`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllStores = (body) => {
  return Axios.post(`/Store/GetAllStores`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const DeleteStoreById = (body) => {
  return Axios.post(`/Store/DeleteStoreById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const ActivateStoreById = (body) => {
  return Axios.post(`/Store/EnableStoreById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetStoreById = (body) => {
  return Axios.get(`/Store/GetStoreById?StoreId=${body}`);
};

export const UpdateStore = (body) => {
  return Axios.post(`/Store/UpdateStore`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetStoresForSelection = (body) => {
  return Axios.get(`/Store/GetStoresForSelection`);
};
export const GetChannelListByStoreIdForSelection = (body) => {
  return Axios.get(
    `/SaleChannel/GetSaleChannelByStoreIdForSelection?StoreId=${body}`,
  );
};
export const GetAllSaleChannelByLookupIdForSelection = (body) => {
  return Axios.get(
    `/SaleChannel/GetAllSaleChannelByLookupIdForSelection?saleChannelLookupId=${body}`,
  );
};
export const GetAllSaleChannelsByStoreId = (body) => {
  return Axios.get(`/SaleChannel/GetAllSaleChannelsByStoreId?StoreId=${body}`);
};
export const GetSaleChannelConfigById = (id) => {
  return Axios.get(
    `/SaleChannel/GetSaleChannelConfigById?SaleChannelConfigId=${id}`,
  );
};
export const GetSaleChannelConfigForUpdateById = (id) => {
  return Axios.get(
    `/SaleChannel/GetSaleChannelConfigForUpdateById?SaleChannelConfigId=${id}`,
  );
};

// Common Lookup Endpoints

export const GetAllReasons = () => {
  return Axios.get(`CommonLookup/GetAllLookupAdjustReason`);
};
export const GetAllCarrierTrackingStatusForSelection = () => {
  return Axios.get(
    `CommonLookup/GetAllCarrierTrackingStatusLookupForSelection`,
  );
};
export const GetAllStationLookup = (body) => {
  return Axios.get(`/CommonLookup/GetAllStationLookup`);
};

export const GetAllProductOptionLookup = (body) => {
  return Axios.get(`/CommonLookup/GetAllProductOptionLookup`);
};

export const GetAllPaymentMethodLookup = (body) => {
  return Axios.get(`/CommonLookup/GetAllPaymentMethodLookup`);
};

export const GetAllOrderTypeLookup = (body) => {
  return Axios.get(`/CommonLookup/GetAllOrderTypeLookup`);
};
export const GetAllExpenseCategoryLookup = () => {
  return Axios.get(`CommonLookup/GetAllExpenseCategoryLookup`);
};
// Product Category Endpoints

export const GetAllProductCategoryLookup = () => {
  return Axios.get(`/ProductCategory/GetAllProductCategoryLookup`);
};
export const GetProductById = (productId) => {
  return Axios.get(`Product/GetProductById?ProductId=${productId}`);
};

export const CreateProductCategory = (body) => {
  return Axios.post(`/ProductCategory/CreateProductCategory`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateProductStockQuantityByReason = (body) => {
  return Axios.post(`Product/UpdateProductStockQuantityByReason`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteProductById = (body) => {
  return Axios.post(`/Product/DisableProductById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteProductMediaById = (productMediaId) => {
  const body = {
    ProductMediaId: productMediaId,
  };
  return Axios.post(`/Product/DeleteProductMediaById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const EnableProductById = (body) => {
  return Axios.post(`/Product/EnableProductById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DisableProductStock = (body) => {
  return Axios.post(`/Product/DisableProductStock`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const EnableProductStock = (body) => {
  return Axios.post(`/Product/EnableProductStock`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const AddProductOption = (body) => {
  return Axios.post(`Product/QuickAddProductOption`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateProduct = (body) => {
  return Axios.post(`Product/UpdateProduct`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

// Product Endpoints

export const CreateProduct = (body) => {
  return Axios.post(`/Product/CreateProduct`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CreateBulkProduct = (body) => {
  return Axios.post(`/Product/CreateBulkProduct`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CheckUniqueProductStockSku = (id) => {
  return Axios.get(`/Product/CheckUniqueProductStockSku?Sku=${id}`);
};
export const CheckUniqueProductStockSkus = (Skus) => {
  return Axios.get(`/Product/CheckUniqueProductStockSkus?Skus=${Skus}`);
};

export const UploadProductFile = (body) => {
  return Axios.post(`/Product/UploadProductFile`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const GetAllProductInventory = (body) => {
  return Axios.post(`/Product/GetAllProductInventory`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetProductStocksForSelection = (stationId, storeId) => {
  return Axios.get(
    `/Product/GetProductStocksForSelection?StationId=${stationId || 0
    }&storeId=${storeId || 0}`,
  );
};

export const DownloadInventoryHistorySummaryExcel = (body) => {
  return Axios.post(`/Product/DownloadInventoryHistorySummaryExcel`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};

export const ExcelExportProductStockHistory = (body) => {
  return Axios.post(`/Product/ExcelExportProductStockHistory`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const ExcelExportProductInventorySummary = (body) => {
  return Axios.post(`/Product/ExcelExportProductInventorySummary`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetProductStockHistoryByStockId = (body) => {
  return Axios.post(`/Product/GetProductStockHistoryByStockId`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
//#region ProductStation
export const GetProductStations = (body) => {
  return Axios.post(`/ProductStation/GetProductStations`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetSaleChannelProductStationsWithQtyForSelection = () => {
  return Axios.get(`/ProductStation/GetSaleChannelProductStationsWithQtyForSelection`);
};
export const GetProductsWithStationInventoryForSelection = (body) => {
  return Axios.post(`/Product/GetProductsWithStationInventoryForSelection`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetSaleChannelProductStationsForSelection = GetSaleChannelProductStationsWithQtyForSelection;
export const GetProductStationsForSelection = GetSaleChannelProductStationsWithQtyForSelection;
export const CreateProductStation = (body) => {
  return Axios.post(`/ProductStation/CreateProductStation`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateProductStation = (body) => {
  return Axios.post(`/ProductStation/UpdateProductStation`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const MakeDefaultProductStation = (body) => {
  return Axios.post(`/ProductStation/MakeDefaultProductStation`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteProductStationById = (body) => {
  return Axios.post(`/ProductStation/DeleteProductStationById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const ActiveProductStationById = (body) => {
  return Axios.post(`/ProductStation/ActiveProductStationById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetProductStationById = (id) => {
  return Axios.get(
    `/ProductStation/GetProductStationById?ProductStationId=${id}`,
  );
};
//#endregion
// Order Endpoint

export const DeleteOrder = (body) => {
  return Axios.post(`/Order/DeleteOrders`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreateOrder = (body) => {
  return Axios.post(`/Order/CreateOrder`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreateOrderWithInvoice = (body) => {
  return Axios.post(`/Order/CreateOrderWithInvoice`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateOrderWithInvoice = (body) => {
  return Axios.post(`/Order/UpdateOrderWithInvoice`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateOrder = (body) => {
  return Axios.post(`/Order/UpdateOrder`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const RefreshCarrierStatus = (OrderId) => {
  const body = {
    OrderId: OrderId,
  };
  return Axios.post(`/Order/RefreshCarrierStatus`, body);
};

export const GetAllOrders = (createdFrom,
  createdTo,
  storeId,
  salePersonIds,
  orderTypeId,
  carrierId,
  fullFillmentStatusId,
  paymentStatusId,
  paymentMethodId,
  stationId,
  carrierAssign,
  searchParams,
  countryIds,
  cityIds,
  provinceIds,
  stateIds,
  pinCodeIds,
  areaIds,
  carrierTrackingStatusIds,
  IncludeRegion,
  IncludeCity,
  orderFromDate,
  orderToDate,
  orderAddressFilter = {},
  OrderLabels,
  isWithoutStation,
) => {
  const body = {
    filterModel: {
      createdFrom: createdFrom ? createdFrom : null,
      createdTo: createdTo ? createdTo : null,
      start: 0,
      length: EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length,
      search: searchParams,
      sortDir: "desc",
      sortCol: 0,
    },
    storeId: storeId,
    orderTypeId: orderTypeId,
    carrierId: carrierId,
    fullFillmentStatusId: fullFillmentStatusId,
    paymentStatusId: paymentStatusId,
    orderRequestVia: 0,
    paymentMethodId: paymentMethodId,
    stationId: stationId,
    readyForAssignment: true,
    carrierAssign: carrierAssign,
    salePersonIds: salePersonIds,
    countryId: countryIds,
    cityIds: cityIds,
    provinceIds: provinceIds,
    stateIds: stateIds,
    pinCodeIds: pinCodeIds,
    areaIds: areaIds,
    carrierTrackingStatusIds: carrierTrackingStatusIds,
    IncludeRegion: IncludeRegion,
    IncludeCity: IncludeCity,
    orderFromDate: orderFromDate || null,
    orderToDate: orderToDate || null,
    OrderAddressFilter: orderAddressFilter,
    OrderLabels: OrderLabels,
    isWithoutStation: isWithoutStation,
  };
  return Axios.post(`/Order/GetAllOrders`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllProducts = (
  createdFrom,
  createdTo,
  storeId,
  length,
  searchText = "",
) => {
  const body = {
    filterModel: {
      createdFrom: createdFrom,
      createdTo: createdTo,
      start: 0,
      length: length || EnumChangeFilterModelApiUrls.GET_ALL_PRODUCTS.length,
      search: searchText,
      sortDir: "desc",
      sortCol: 0,
    },
    storeId: storeId,
  };
  return Axios.post(`/Product/GetAllProducts`, body);
};

export const MoveDashboardStatusFromOneTabToAnother = (body) => {
  return Axios.post(`/Shipment/MoveDashboardStatusFromOneTabToAnother`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateShipmentTabDisplayOrder = (body) => {
  return Axios.post(`/Shipment/UpdateShipmentTabDisplayOrder`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreateShipmentGridColumn = (body) => {
  return Axios.post(`/Shipment/CreateShipmentGridColumn`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const ExcelExportOrders = (body) => {
  return Axios.post(`/Order/ExcelExportOrders`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob", // important
  });
};
export const ExcelExportDeliveryTasks = (body) => {
  return Axios.post(`/Delivery/ExcelExportDeliveryTasks`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob", // important
  });
};
export const ExcelExportShipments = (body) => {
  return Axios.post(`/Shipment/ExcelExportShipments`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob", // important
  });
};
export const ExcelExportLeads = (body) => {
  return Axios.post(`/Lead/ExcelExportLeads`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob", // important
  });
};
export const GetOrderFileByOrderTypeId = (id, c_id) => {
  return Axios.post(
    `/Order/GetOrderFileByOrderTypeId?OrderTypeId=${id}&CountryId=${c_id}`,
  );
};

export const GetSampleExcelFileForStoreUpload = (id) => {
  return Axios.post(`/Store/GetSampleExcelFileForStoreUpload?CountryId=${id}`);
};

export const GetProductSampleFile = (id) => {
  return Axios.get(`/Product/GetProductSampleFile`);
};
export const GetAddressEntitiesByType = (
  SelectedEntityIds,
  SelectedEntityType,
  NextEntityType,
  CarrierId = 0,
  countryID,
) => {
  return Axios.get(
    `/Country/GetAddressEntitiesByType?SelectedEntityIds=${SelectedEntityIds}&SelectedEntityType=${SelectedEntityType}&NextEntityType=${NextEntityType}&CarrierId=${CarrierId}&countryID=${countryID}`,
  );
};

export const ExcelExportAddressEntitiesByType = (CountryId, NextEntityType) => {
  return Axios.get(
    `/Country/ExcelExportAddressEntitiesByType?CountryId=${CountryId}&NextEntityType=${NextEntityType}`,
    {
      headers: {
        "Content-Type": "application/json",
      },
      responseType: "blob",
    },
  );
};

export const UploadFileRegular = (body) => {
  return Axios.post(`/Order/UploadFileRegular`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};
export const AssignToCarrier = (body) => {
  return Axios.post(`/Order/AssignToCarrier`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetOrderById = (orderId) => {
  return Axios.get(`Order/GetOrderById?OrderId=${orderId}`);
};
//assign in house
export const CreateDeliveryTask = (body) => {
  return Axios.post(`/Delivery/CreateDeliveryTask`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const BatchOrderFulfillment = (body) => {
  return Axios.post(`/Order/BatchOrderFulfillment`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const BatchUpdateOrderStatus = (body) => {
  return Axios.post(`/Order/BatchUpdateOrderStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UploadFileFullfilable = (body) => {
  return Axios.post(`/Order/UploadFileFullfilable`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const UpdateOrderAmount = (body) => {
  return Axios.post(`/Order/UpdateOrderAmount`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const DeleteOrderItemById = (body) => {
  return Axios.post(`/Order/DeleteOrderItemById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
//Carriers Endpoint
export const getAllCarriers = (body) => {
  return Axios.post(`/Carrier/GetAllCarriers`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const activateCarrier = (body) => {
  return Axios.post(`/Carrier/CreateActiveCarrier`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllActivedCarrier = (body) => {
  return Axios.post(`/Carrier/GetAllActiveCarrier`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllDriverCtssetting = (body) => {
  return Axios.get(`/Driver/GetAllDriverCtssetting`);
};
export const CreateDriverStatues = (body) => {
  return Axios.post(`/Driver/CreateDriverCtssetting`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteDriverCtssetting = (body) => {
  return Axios.post(`/Driver/DeleteDriverCtssetting`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const getInputRequiredConfig = (id) => {
  return Axios.get(`/Carrier/GetInputRequiredConfigById?CarrierId=${id}`);
};
export const DeleteActiveCarrierById = (body) => {
  return Axios.post(`/Carrier/DeleteActiveCarrierById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetCarrierWebHookUrlByCarrierId = (carrierId, activeCarrierId) => {
  return Axios.get(
    `/Carrier/GetCarrierWebHookUrlByCarrierId?CarrierId=${carrierId}&ActiveCarrierId=${activeCarrierId}`,
  );
};

export const UpdateCarrierAlias = (body) => {
  return Axios.post(`/Carrier/UpdateCarrierAlias`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CheckDuplicationCarrierAlias = (body) => {
  return Axios.post(`/Carrier/CheckDuplicationCarrierAlias`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UnAssignFromCarrier = (body) => {
  return Axios.post(`/Carrier/UnAssignFromCarrier`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
//Shipment Endpoints

export const GetShipmentTabsCountConfig = () => {
  return Axios.get(`/Shipment/GetShipmentTabsCountConfig`);
};

export const GetAllShipmentsByDriverReceivableId = (body) => {
  return Axios.post(`/Shipment/GetAllShipmentsByDriverReceivableId`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const ExportPDFShipmentsByDriverReceivableId = (body) => {
  return Axios.post(`/Shipment/ExportShipmentsByDriverReceivableId`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetPDFCarrierReturnReportById = (body) => {
  return Axios.post(`/Shipment/GetPDFCarrierReturnReportById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetShipments = (body) => {
  return Axios.post(`/Shipment/GetShipments`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetShipmentTabsCount = (body) => {
  return Axios.post(`/Shipment/GetShipmentTabsCount`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

//Account Endpoints

export const GetCarrierSettlementSamplefile = () => {
  return Axios.get(`/Account/GetCarrierSettlementSamplefile`);
};
export const GetAllCarrierTrackingStatusLookupForSelection = () => {
  return Axios.get(`/Common/GetAllCarrierTrackingStatusLookupForSelection`);
};
export const CreateClientCarrierTrackingStatus = (body) => {
  return Axios.post(`/Client/CreateClientCarrierTrackingStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllRegionTimeZone = () => {
  return Axios.get(`/CommonLookup/GetAllRegionTimeZone`);
};

export const GetAllCarrierPaymentSettlements = (body) => {
  return Axios.post(`/Account/GetAllCarrierPaymentSettlements`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllCODPendings = (body) => {
  return Axios.post(`/Account/GetAllCODPendings`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UplaodCarrierSettlementFile = (body) => {
  return Axios.post(`/Account/UplaodCarrierSettlementFile`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const CreateCarrierPaymentSettlementWithFile = (body) => {
  return Axios.post(`/Account/CreateCarrierPaymentSettlement`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreateCarrierPaymentSettlement = (body) => {
  return Axios.post(`/Account/CreateCarrierPaymentSettlement`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteCarrierPaymentSettlement = (body) => {
  return Axios.post(`/Account/DeleteCarrierPaymentSettlement`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
// GetShipmentInfoByOrderNo
export const GetShipmentInfoByOrderNo = (orderNo) => {
  const body = {
    orderNo: orderNo,
  };
  return Axios.post(`/Shipment/GetShipmentInfoByOrderNo`, body);
};
export const AdvanceSearchOrders = (body) => {
  return Axios.post(`/Order/AdvanceSearchOrders`, body);
};
export const GetOrderItemByOrderNo = (param) => {
  return Axios.get(`/Order/GetOrderItemByOrderNo?OrderNo=${param}`);
};
export const GetActiveCarriersForSelection = () => {
  return Axios.get(`/Carrier/GetActiveCarriersForSelection`);
};

export const GetAllFullFillmentStatusLookup = () => {
  return Axios.get(`/CommonLookup/GetAllFullFillmentStatusLookup`);
};

export const GetAllPaymentStatusLookup = () => {
  return Axios.get(`/CommonLookup/GetAllPaymentStatusLookup`);
};
export const UpdateCustomerEmail = (body) => {
  return Axios.post(`/Order/UpdateCustomerEmailAddress`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CheckMobileNoDuplicate = (mobileNo) => {
  return Axios.get(`/Order/CheckMobileNoDuplicate?mobileNo=${encodeURIComponent(mobileNo)}`);
};

export const SendNotificationToCustomer = (body) => {
  return Axios.post(`/Notification/SendNotificationToCustomer`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetWayBillsByOrderNos = (body) => {
  return Axios.post(`/Order/GetWayBillsByOrderNos`, body, {
    responseType: "blob",
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetCarrierWayBillsByOrderNos = (body) => {
  return Axios.post(`/Order/GetCarrierWayBillsByOrderNos`, body, {
    responseType: "blob",
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetLeadContacts = (body) => {
  return Axios.post(`/Lead/GetLeadContacts`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetOrdersByContactMobile = (body) => {
  return Axios.post(`/Lead/GetOrdersByContactMobile`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetOrderInvoiceByOrderNos = (orderNo) => {
  const body = {
    OrderNos: orderNo,
  };
  return Axios.post(`/Order/GetOrderInvoiceByOrderNos`, body, {
    responseType: "blob",
  });
};
export const GetStripeInvoiceByUrl = (stripUrl) => {
  return Axios.get(`${stripUrl}`, {
    responseType: "blob",
  });
};
export const CreateInvoiceByOrderNumber = (body) => {
  return Axios.post(`/Order/CreateInvoiceByOrderNumber`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetValidateClientPPActivate = (body) => {
  return Axios.get(`/Client/GetValidateClientPPActivate`);
};
export const GetUniqueAutogeneratedSku = (body) => {
  return Axios.get(`/Product/GetUniqueAutogeneratedSku`);
};
export const ExcelExportProductInventory = (body) => {
  return Axios.post(`/Product/ExcelExportProductInventory`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob", // important
  });
};
export const GetAllStoreChannel = (body) => {
  return Axios.post(`/Channel/GetAllChannel`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllChannelByStoreId = (body) => {
  return Axios.get(`/Channel/GetAllChannelByStoreId?StoreId=${body}`);
};
// inventory sale
export const GetAllInventorySales = (body) => {
  return Axios.post(`/Product/GetAllInventorySales`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const ExcelExportInventorySales = (body) => {
  return Axios.post(`/Product/ExcelExportInventorySales`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetInventorySalesSummary = (body) => {
  return Axios.post(`/Product/GetInventorySalesSummary`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const ExcelExportInventorySalesSummary = (body) => {
  return Axios.post(`/Product/ExcelExportInventorySalesSummary`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetPDFInventorySales = (body) => {
  return Axios.post(`/Product/GetPDFInventorySales`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetPDFInventorySalesSummary = (body) => {
  return Axios.post(`/Product/GetPDFInventorySalesSummary`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
////carrier return
export const GetReturnReportSampleFile = (body) => {
  return Axios.get(`/ReturnReport/GetReturnReportSampleFile`);
};
export const UplaodReturnReportFile = (body) => {
  return Axios.post(
    `/ReturnReport/GetOrderDetailByUplaodReturnReportFile`,
    body,
    {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    },
  );
};
export const CreateMyCarrierReturnReport = (body) => {
  return Axios.post(`/Delivery/CreateMyCarrierReturnReport`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CreateCarrierReturnReport = (body) => {
  return Axios.post(`/ReturnReport/CreateCarrierReturnReport`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetOrderDetailForReturnReport = (body) => {
  return Axios.post(`/ReturnReport/GetOrderDetailForReturnReport`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetReturnRerports = (body) => {
  return Axios.post(`/ReturnReport/GetReturnRerports`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetShipmentsByReturnRerportId = (body) => {
  return Axios.get(
    `/ReturnReport/GetShipmentsByReturnReportId?CarrierRRId=${body}`,
  );
};
export const ExcelExportReturnReportById = (body) => {
  return Axios.get(
    `/ReturnReport/ExcelExportReturnReportById?CarrierRrid=${body}`,
    { responseType: "blob" },
  );
};

///// leads
export const GetAllLead = (body) => {
  return Axios.post(`/Lead/GetAllLeads`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const DeleteBulkLeads = (body) => {
  return Axios.post(`/Lead/DeleteBulkLeads`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllLeadStatusForSelection = (body) => {
  return Axios.get(`Lead/GetAllLeadStatusForSelection`, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UploadLeadsData = (body) => {
  return Axios.post(`/Lead/UploadLeads`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const UpdateLeadStatus = (body) => {
  return Axios.post(`/Lead/UpdateLeadStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const AssignSalespersonToLead = (body) => {
  return Axios.post(`/Lead/AssignSalesperson`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CreateBulkLeads = (body) => {
  return Axios.post(`/Lead/CreateBulkLeads`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateLead = (body) => {
  return Axios.post(`/Lead/UpdateLead`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateOrderIds = (body) => {
  return Axios.post(`/Lead/UpdateOrderIds`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

// Lead Tabs
export const GetAllLeadTabsCount = (body) => {
  return Axios.post(`/Lead/GetAllLeadTabsCount`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetSalesPersonLeadStats = (body) => {
  return Axios.post(`/Lead/GetSalesPersonLeadStats`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetLeadTabsCountConfig = () => {
  return Axios.get(`/Lead/GetLeadTabsCountConfig`, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

// Client Lead Status CRUD
export const CreateClientLeadStatus = (body) => {
  return Axios.post(`/Lead/CreateClientLeadStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateClientLeadStatus = (body) => {
  return Axios.post(`/Lead/UpdateClientLeadStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const ActiveDeactiveClientLeadStatus = (body) => {
  return Axios.post(`/Lead/ActiveDeactiveClientLeadStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllClientLeadStatus = (body) => {
  return Axios.post(`/Lead/GetAllClientLeadStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

// Lead Tab Management (mirrors CreateShipmentGridColumn / UpdateShipmentTabDisplayOrder)
export const CreateLeadGridColumn = (body) => {
  return Axios.post(`/Lead/CreateLeadGridColumn`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateLeadTabDisplayOrder = (body) => {
  return Axios.post(`/Lead/UpdateLeadTabDisplayOrder`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateLeadTab = (body) => {
  return Axios.post(`/Lead/UpdateLeadGridColumn`, body, {
    headers: { "Content-Type": "application/json" },
  });
};

export const DeleteLeadTab = (body) => {
  return Axios.post(`/Lead/DeleteLeadGridColumn`, body, {
    headers: { "Content-Type": "application/json" },
  });
};

export const GetAllClientLeadStatusForSelection = () => {
  return Axios.get(`/Lead/GetAllClientLeadStatusForSelection`);
};

/////delivery task
export const GetAllDeliveryTask = (body) => {
  return Axios.post(`/Delivery/GetAllDeliveryTask`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreateDeliveryTaskAndAddToExistingNote = (body) => {
  return Axios.post(`/Delivery/CreateDeliveryTaskAndAddToExistingNote`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const RevertDeliveryTaskByOrderNos = (body) => {
  return Axios.post(`/Delivery/RevertDeliveryTaskByOrderNos`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteDeliveryNoteDetailByOrderId = (body) => {
  return Axios.post(`/Delivery/DeleteDeliveryNoteDetailByOrderId`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllDeliveryNote = (body) => {
  return Axios.post(`/Delivery/GetAllDeliveryNote`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetDeliveryNoteDetailForDebrief = (body) => {
  return Axios.post(`/Delivery/GetDeliveryNoteDetailForDebrief`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetCompletedDeliveryNoteExpenses = (body) => {
  return Axios.post(`/Delivery/GetCompletedDeliveryNoteExpenses`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteMyCarrierReturnReport = (body) => {
  return Axios.post(`/Delivery/DeleteMyCarrierReturnReport`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const RevertDeliveryNoteDetailByOrderId = (body) => {
  return Axios.post(`/Delivery/RevertDeliveryNoteDetailByOrderId`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UncompleteDeliveryNote = (body) => {
  return Axios.post(`/Delivery/UncompleteDeliveryNote`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CompleteDeliveryNote = (body) => {
  return Axios.post(`/Delivery/CompleteDeliveryNote`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllPendingForReturnShipments = (body) => {
  return Axios.post(`/Delivery/GetAllPendingForReturnShipments`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllMyCarrierReturnReports = (body) => {
  return Axios.post(`/Delivery/GetAllMyCarrierReturnReports`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetPDFMyCarrierReturnReport = (body) => {
  return Axios.post(`/Delivery/GetPDFMyCarrierReturnReport`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const UpdateOrderStatusOnDebrief = (body) => {
  return Axios.post(`/Delivery/UpdateOrderStatusOnDebrief`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateDeliveryNoteInOperation = (body) => {
  return Axios.post(`/Delivery/UpdateDeliveryNoteInOperation`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreateDeliveryNoteDetailPendingForReturnStatus = (body) => {
  return Axios.post(
    `/Delivery/CreateDeliveryNoteDetailPendingForReturnStatus`,
    body,
  );
};
export const GetPDFDeliveryRunSheet = (body) => {
  return Axios.get(`/Delivery/GetPDFDeliveryRunSheet?DeliveryNoteId=${body}`, {
    responseType: "blob",
  });
};
export const GetDeliveryNoteByNo = (body) => {
  return Axios.post(`/Delivery/GetDeliveryNoteByNo`, body);
};
export const GetDriverLatestDeliveryNoteToday = (driverId) => {
  return Axios.get(`/Delivery/GetDriverLatestDeliveryNoteToday/${driverId}`);
};

//////////employee
export const GetAllEmployees = (body) => {
  return Axios.post(`/Employee/GetAllEmployees`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreateEmployee = (body) => {
  return Axios.post(`/Employee/CreateEmployee`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateEmployee = (body) => {
  return Axios.post(`/Employee/UpdateEmployee`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const EnableDisableEmploye = (body) => {
  return Axios.get(`/Employee/EnableDisableEmploye?EmployeeId=${body}`);
};
export const GetEmployeeById = (body) => {
  return Axios.get(`/Employee/GetEmployeeById?EmployeeId=${body}`);
};
export const GetAllSalePersonForSelection = (body) => {
  return Axios.get(`/Employee/GetAllSalePersonForSelection`);
};

export const GetAllEmployeesForSelection = (body) => {
  return Axios.get(`/Employee/GetAllEmployeesForSelection`);
};
export const GetGenderForSelection = (body) => {
  return Axios.get(`/Employee/GetGenderForSelection`);
};
///////////driver
export const CreateDriver = (body) => {
  return Axios.post(`/Driver/CreateDriver`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetDriversForSelection = (body) => {
  return Axios.get(`/Driver/GetDriversForSelection`);
};
export const GetAllDrivers = (body) => {
  return Axios.post(`/Driver/GetAllDrivers`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteDriver = (body) => {
  return Axios.post(`/Driver/DeleteDriver`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllDriverReceivable = (body) => {
  return Axios.post(`/DriverAccount/GetAllDriverReceivable`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllDriverExpense = (body) => {
  return Axios.post(`/DriverAccount/GetAllDriverExpense`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
////////////expense
export const GetPDFDriverExpenseReport = (body) => {
  return Axios.post(`/Expense/GetPDFDriverExpenseReport`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const ExcelExportDriverExpense = (body) => {
  return Axios.post(`/DriverAccount/ExcelExportDriverExpense`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const CreateExpenseCategory = (body) => {
  return Axios.post(`/Expense/CreateExpenseCategory`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetExpenseById = (body) => {
  return Axios.get(`/DriverAccount/GetExpenseById?ExpenseId=${body}`);
};
export const GetAllExpenseAccount = (body) => {
  return Axios.post(`/Expense/GetAllExpenseAccount`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
////////platform
export const GetAllPlatformForSelection = (body) => {
  return Axios.get(`/Platform/GetAllPlatformLookupForSelection`);
};
export const GetAllPlatformConfig = (body) => {
  return Axios.post(`/Platform/GetAllPlatformConfig`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CreatePlatformConfig = (body) => {
  return Axios.post(`/Platform/CreatePlatformConfig`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

//////SaleChannel
export const GetSaleChannelByStoreIdForSelection = (id) => {
  return Axios.get(
    `/SaleChannel/GetSaleChannelByStoreIdForSelection?StoreId=${id}`,
  );
};

export const CreateSaleChannelConfig = (body) => {
  return Axios.post(`/SaleChannel/CreateSaleChannelConfig`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const SaleChannelInventorySync = (body) => {
  return Axios.post(`/SaleChannel/SaleChannelInventorySync`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateSaleChannelConfig = (body) => {
  return Axios.post(`/SaleChannel/UpdateSaleChannelConfig`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllSaleChannelConfig = (body) => {
  return Axios.post(`/SaleChannel/GetAllSaleChannelConfig`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const SaleChannelOrderPreProcessor = (body) => {
  return Axios.post(`/SaleChannel/SaleChannelOrderPreProcessor`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const SaleChannelProductPreProcessor = (body) => {
  return Axios.post(`/SaleChannel/SaleChannelProductPreProcessor`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const SaleChannelProductPostProcessor = (body) => {
  return Axios.post(`/SaleChannel/SaleChannelProductPostProcessor`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const SaleChannelOrderPostProcessor = (body) => {
  return Axios.post(`/SaleChannel/SaleChannelOrderPostProcessor`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllSaleChannelLookupForSelection = (body) => {
  return Axios.get(`/SaleChannel/GetAllSaleChannelLookupForSelection`);
};

////// sms process
export const GetAllSMSActivate = (body) => {
  return Axios.post(`/Sms/GetAllSMSActivate`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllSMSLookupForSelection = (body) => {
  return Axios.get(`/Sms/GetAllSMSLookupForSelection`);
};
export const ActivateSmsProcess = (body) => {
  return Axios.post(`/Sms/ActivateSmsProcess`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteSMSActivate = (body) => {
  return Axios.post(`/Sms/DeleteSMSActivate`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetSMSActivateById = (id) => {
  return Axios.get(`/Sms/GetSMSActivateById?PpactivateId=${id}`);
};
//// payment process
export const GetAllPPActivate = (body) => {
  return Axios.post(`/PaymentProcess/GetAllPPActivate`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllPPLookupForSelection = (body) => {
  return Axios.get(`/PaymentProcess/GetAllPPLookupForSelection`);
};
export const ActivatePaymentProcess = (body) => {
  return Axios.post(`/PaymentProcess/ActivatePaymentProcess`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeletePPActivate = (body) => {
  return Axios.post(`/PaymentProcess/DeletePPActivate`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetPPActivateById = (id) => {
  return Axios.get(`/PaymentProcess/GetPPActivateById?PPactivateId=${id}`);
};
export const ExcekExportLowQuantityProductStock = (body) => {
  return Axios.post(`/Product/ExcekExportLowQuantityProductStock`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetAllLowQuantityProductStock = (body) => {
  return Axios.post(`/Product/GetAllLowQuantityProductStock`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const MarkCarrierSettlementPaid = (body) => {
  return Axios.post(`/Account/MarkCarrierSettlementPaid`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const MarkCarrierSettlementUnPaid = (body) => {
  return Axios.post(`/Account/MarkCarrierSettlementUnPaid`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

// =============postman
export const DownloadApiPostManCollection = (body) => {
  return Axios.get(`/CommonLookup/DownloadApiPostManCollection`, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};

export const DeleteShipmentTab = (body) => {
  return Axios.post(`/Shipment/DeleteShipmentGridColumn`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateShipmentTab = (body) => {
  return Axios.post(`/Shipment/UpdateShipmentGridColumn`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllCarrierWithServiceAndLocation = (body) => {
  return Axios.post(`/Carrier/GetAllCarrierWithServiceAndLocation`, body);
};
export const GetAllDeliveryService = (body) => {
  return Axios.get(`Carrier/GetAllDeliveryService`);
};
export const GetCarrierWithServiceAndLocationByCarrierId = (id) => {
  return Axios.get(
    `Carrier/GetCarrierWithServiceAndLocationByCarrierId?CarrierId=${id}`,
  );
};

export const GetAllClientUserRole = (body) => {
  return Axios.get(`Permission/GetAllClientUserRole`, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllClientRolePermissionGroup = (id) => {
  return Axios.get(
    `Permission/GetAllClientRolePermissionGroup?clientUserRoleId=${id}`,
    {
      headers: {
        "Content-Type": "application/json",
      },
    },
  );
};
export const AddUpdateClientRolePermissionGroup = (body) => {
  return Axios.post(`Permission/AddUpdateClientRolePermissionGroup`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetRegionTimeZone = () => {
  return Axios.get(`Client/GetRegionTimeZone`);
};

export const UpdateOrderLatLng = (body) => {
  return Axios.post(`/Order/UpdateOrderLatLng`, body);
};

export const CreateOrderNote = (body) => {
  return Axios.post(`/Order/CreateOrderNote`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllDeliveryTaskStatusForSelection = (body) => {
  return Axios.get(`Delivery/GetAllDeliveryTaskStatusForSelection`, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllClientTax = (createdFrom, createdTo) => {
  const body = {
    filterModel: {
      createdFrom: createdFrom || null,
      createdTo: createdTo || null,
      start: 0,
      length: 10000,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`Tax/GetAllClientTax`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllTaxForSelection = () => {
  return Axios.get(`Tax/GetAllTaxForSelection`);
};

export const CreateClientTax = (body) => {
  return Axios.post(`Tax/CreateClientTax`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetClientTaxById = (body) => {
  return Axios.post(`Tax/GetClientTaxById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateClientTax = (body) => {
  return Axios.post(`Tax/UpdateClientTax`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const EnableDisableTax = (body) => {
  return Axios.post(`Tax/EnableDisableTax`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllCpsettlementPopFilesById = (body) => {
  return Axios.post(`Account/GetAllCpsettlementPopFilesById`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const DeleteCpsettlementPopFile = (body) => {
  return Axios.post(`Account/DeleteCpsettlementPopFile`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreateCpsettlementPopFile = (body) => {
  return Axios.post(`Account/CreateCpsettlementPopFile`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};
// export const GetAllExcelExportAllCity = (id) => {
//   return Axios.get(`/Country/ExcelExportAllCity`);
// };
// export const GetAllExcelExportAllCountry = (id) => {
//   return Axios.get(`/Country/ExcelExportAllCountry`);
// };
// export const GetAllExcelExportAllRegion = (id) => {
//   return Axios.get(`/Country/ExcelExportAllRegion`);
// };
export const GetAllExcelExportAllCity = () => {
  return Axios.get(`/Country/ExcelExportAllCity`, { responseType: "blob" });
};

export const GetAllExcelExportAllCountry = () => {
  return Axios.get(`/Country/ExcelExportAllCountry`, { responseType: "blob" });
};

export const GetAllExcelExportAllRegion = () => {
  return Axios.get(`/Country/ExcelExportAllRegion`, { responseType: "blob" });
};
export const createClientUserRole = (roleData) => {
  return Axios.post(`/Permission/CreateClientUserRole`, roleData);
};
export const GetAllPendingForReturnShipment = () => {
  const body = {
    filterModel: {
      createdFrom: null,
      createdTo: null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(
    `CarrierPendingForReturnReport/GetAllPendingForReturnShipment`,
    body,
  );
};

export const CreateCarrierPendingForReturn = (body) => {
  return Axios.post(`/ReturnReport/CreateCarrierPendingForReturn`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetValidateDocumentSetting = (body) => {
  return Axios.get(`/DocumentSetting/GetValidateDocumentSetting`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateDocumentSetting = (body) => {
  return Axios.post(`/DocumentSetting/UpdateDocumentSetting`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetDocumentSetting = (body) => {
  return Axios.get(`/DocumentSetting/GetDocumentSetting`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetSettingConfigByActiveCarrierId = (ActiveCarrierId) => {
  const body = {
    ActiveCarrierId,
  };
  return Axios.post(`/Carrier/GetSettingConfigByActiveCarrierId`, body);
};
export const UpdateActiveCarrierClientSettingConfig = (
  activeCarrierId,
  CarrierId,
  settingConfig,
  InputParameters,
) => {
  const body = {
    activeCarrierId,
    CarrierId,
    settingConfig,
    InputParameters,
  };
  return Axios.post(`/Carrier/UpdateActiveCarrierClientSettingConfig`, body);
};

export const GetAddressFromLatAndLong = (body) => {
  return Axios.post(`/Country/GetCivilEntityIdByLatitudeAndLongitude`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const CreaetNotificationConfig = (body) => {
  return Axios.post(`/NotificationConfig/CreaetNotificationConfig`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateNotificationConfig = (body) => {
  return Axios.post(`NotificationConfig/UpdateNotificationConfig`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetNotificationConfigByTypeId = (id) => {
  return Axios.get(
    `NotificationConfig/GetNotificationConfigByTypeId?NotificationTypeId=${id}`,
  );
};
export const DeleteNotificationConfig = (id) => {
  const body = {
    notificationConfigId: id,
  };
  return Axios.post(`NotificationConfig/DeleteNotificationConfig`, body);
};
export const GetNotificationTypes = () => {
  return Axios.get(`NotificationConfig/GetNotificationTypes`);
};
export const GetNotificationEvents = (body) => {
  return Axios.get(`NotificationConfig/GetNotificationEvents`);
};
export const GetAllInvoiceHistory = () => {
  const body = {
    filterModel: {
      createdFrom: null,
      createdTo: null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`Invoice/GetAllInvoiceHistory`, body);
};

export const GetClientPaymentMethods = () => {
  return Axios.get(`/Client/GetClientPaymentMethods`);
};
export const UpdateClientDefaultPaymentMethod = (paymentMethodId) => {
  const body = {
    paymentMethodId,
  };
  return Axios.post(`/Client/UpdateClientDefaultPaymentMethod`, body);
};
export const ClientPaymentMethodDetach = (paymentMethodId) => {
  const body = {
    paymentMethodId,
  };
  return Axios.post(`/Client/ClientPaymentMethodDetach`, body);
};
export const CancelClientSubscription = (
  StripeSubscriptionId,
  feedback,
  comments,
) => {
  const body = {
    StripeSubscriptionId,
    feedback,
    comments,
  };
  return Axios.post(`/Client/CancelClientSubscription`, body);
};
export const GetClientSubscription = () => {
  return Axios.get(`/Client/GetClientSubscription`);
};
export const StripeGetAllProducts = () => {
  return Axios.get(`/Client/StripeGetAllProducts`);
};
export const CheckIsUserConfirmed = (username) => {
  const body = {
    username,
  };
  return Axios.post(`/UserManagement/CheckIsUserConfirmed`, body);
};
export const GetDefaultPaymentMethod = () => {
  return Axios.get(`/Client/GetDefaultPaymentMethod`);
};
export const CreateSubscription = (ProductId) => {
  const body = {
    ProductId,
  };
  return Axios.post(`/Client/CreateSubscription`, body);
};
export const UpdateSubscriptionRemoveTrial = (stripeSubscriptionId) => {
  const body = {
    stripeSubscriptionId,
  };
  return Axios.post(`/Client/UpdateSubscriptionRemoveTrial`, body);
};

export const GetAllClientReturnReason = (startDate, endDate) => {
  const body = {
    filterModel: {
      createdFrom: startDate || null,
      createdTo: endDate || null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`/Return/GetAllClientReturnReason`, body);
};

export const CreateReturnCommand = (reason, reasonDetail) => {
  const body = {
    reason,
    reasonDetail,
  };
  return Axios.post(`Return/CreateClientReturnReason`, body);
};
export const GetClientReturnReasonId = (ClientReturnReasonId) => {
  return Axios.get(
    `Return/GetClientReturnReasonId?ClientReturnReasonId=${ClientReturnReasonId}`,
  );
};

export const DeleteReturnCommand = (body) => {
  return Axios.post(`/Return/DeleteReturnCommand`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const UpdateReturnCommand = (
  reason,
  reasonDetail,
  clientReturnReasonId,
) => {
  const body = {
    reason,
    reasonDetail,
    clientReturnReasonId,
  };
  return Axios.post(`/Return/UpdateReturnCommand`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpdateReturnStatus = (returnId, returnStatusId, comment) => {
  const body = {
    returnId,
    returnStatusId,
    comment,
  };
  return Axios.post(`/Return/UpdateReturnStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllOrderReturn = (startDate, endDate, ReasonId, StatusId) => {
  const body = {
    filterModel: {
      createdFrom: startDate || null,
      createdTo: endDate || null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
    ReturnReasonId: ReasonId || 0,
    ReturnStatusId: StatusId || 0,
  };
  return Axios.post(`/Return/GetAllOrderReturn`, body);
};

export const GetAllReturnStatusForSelection = () => {
  return Axios.get(`/Tracking/GetAllReturnStatusForSelection`);
};
export const GetAllClientReturnReasonForSelection = () => {
  return Axios.get(`Tracking/GetAllClientReturnReasonForSelection?ClientId`, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CreateClientOrderBox = (body) => {
  return Axios.post(`OrderBox/CreateClientOrderBox`, body);
};
export const UpdateClientOrderBox = (body) => {
  return Axios.post(`OrderBox/UpdateClientOrderBox`, body);
};
export const EnableDisableClientOrderBox = (body) => {
  return Axios.post(`OrderBox/EnableDisableClientOrderBox`, body);
};
export const GetAllClientOrderBox = () => {
  return Axios.get(`OrderBox/GetAllClientOrderBox`);
};
export const GetClientOrderBoxById = (ClientOrderBoxId) => {
  return Axios.get(
    `OrderBox/GetClientOrderBoxById?ClientOrderBoxId=${ClientOrderBoxId}`,
  );
};
export const GetAllWebhookEventLookup = () => {
  return Axios.get(`/WebhookEvent/GetAllWebhookEventLookup`);
};
export const CreateClientWebhookEvent = (body) => {
  return Axios.post(`WebhookEvent/CreateClientWebhookEvent`, body);
};

export const DeleteWebhookEvent = (id) => {
  return Axios.get(
    `WebhookEvent/DeleteWebhookEvent?ClientWebhookEventId=${id}`,
  );
};
export const GetAllClientWebhookEvents = (id) => {
  return Axios.get(`WebhookEvent/GetAllClientWebhookEvents`);
};
export const GetClientWebhookEventById = (id) => {
  return Axios.get(
    `WebhookEvent/GetClientWebhookEventById?ClientWebhookEventId=${id}`,
  );
};
export const UpdateClientWebhookEvent = (body) => {
  return Axios.post(`WebhookEvent/UpdateClientWebhookEvent`, body);
};

export const GetAllWebhookEventLog = (startDate, endDate) => {
  const body = {
    filterModel: {
      createdFrom: startDate || null,
      createdTo: endDate || null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`/WebhookEvent/GetAllWebhookEventLog`, body);
};
export const GetAllOrdersForGeneratePaymentLink = (search, length) => {
  const body = {
    filterModel: {
      createdFrom: null,
      createdTo: null,
      search: search,
      start: "0",
      length: length,
    },
  };
  return Axios.post(`Order/GetAllOrdersForGeneratePaymentLink`, body);
};
export const GenerateTotalProcessPaymentLink = (body) => {
  return Axios.post(`Order/GenerateTotalProcessPaymentLink`, body);
};
export const GetAllPaymentLinkStatusLookup = () => {
  return Axios.get(`/CommonLookup/GetAllPaymentLinkStatusLookup`);
};
export const GetAllOrderPaymentLinks = (
  startDate,
  endDate,
  PaymentStatusId,
  IsForPayoutRequest,
) => {
  const body = {
    filterModel: {
      createdFrom: startDate,
      createdTo: endDate,
      search: "",
      start: "0",
      length: 100,
    },
    paymentStatusId: PaymentStatusId || 0,
    IsForPayoutRequest,
  };
  return Axios.post(`Order/GetAllOrderPaymentLinks`, body);
};
export const GetAllOrderPaymentLinksForModal = (
  startDate,
  endDate,
  IsForPayoutRequest,
) => {
  const body = {
    filterModel: {
      createdFrom: startDate,
      createdTo: endDate,
      search: "",
      start: "0",
      length: 100,
    },
    IsForPayoutRequest,
  };
  return Axios.post(`Order/GetAllOrderPaymentLinks`, body);
};
export const AddUpdateClientPayoutBank = (body) => {
  return Axios.post(`Wallet/AddUpdateClientPayoutBank`, body);
};
export const GetAllPayoutBanks = () => {
  return Axios.get(`Wallet/GetAllPayoutBanks`);
};
export const GetAllWallets = () => {
  return Axios.get(`Wallet/GetAllWallets`);
};
export const GetWallet = () => {
  return Axios.get(`Wallet/GetWallet`);
};
export const GetClientPayoutBank = () => {
  return Axios.get(`Wallet/GetClientPayoutBank`);
};
export const AddUpdateWallet = (balance) => {
  const body = {
    balance: balance || 0,
  };
  return Axios.post(`Wallet/AddUpdateWallet`, body);
};
export const CreatePayoutRequest = (createdFrom, createdTo) => {
  const body = {
    createdFrom: createdFrom,
    createdTo: createdTo,
  };
  return Axios.post(`/Wallet/CreatePayoutRequest`, body);
};

export const GetAllPayouts = (payoutStatusId) => {
  const body = {
    filterModel: {
      createdFrom: null,
      createdTo: null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
    payoutStatusId: payoutStatusId || 0,
  };
  return Axios.post(`Wallet/GetAllPayouts`, body);
};
export const ExcelExportAllTransaction = (
  startDate,
  endDate,
  transactionTypeId,
) => {
  const body = {
    filterModel: {
      createdFrom: startDate,
      createdTo: endDate,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
    transactionTypeId: transactionTypeId || 0,
  };
  return Axios.post(`Wallet/ExcelExportAllTransaction`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetAllPayoutStatusHistory = (payoutId) => {
  const body = {
    payoutId: payoutId,
  };
  return Axios.post(`Wallet/GetAllPayoutStatusHistory`, body);
};

export const GetAllPayoutFileByPayoutId = (PayoutId) => {
  return Axios.get(`Wallet/GetAllPayoutFileByPayoutId?PayoutId=${PayoutId}`);
};

export const GetAllTransaction = (startDate, endDate, transactionTypeId) => {
  const body = {
    filterModel: {
      createdFrom: startDate,
      createdTo: endDate,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
    transactionTypeId: transactionTypeId || 0,
  };
  return Axios.post(`Wallet/GetAllTransaction`, body);
};

export const GetAllShipraContractCarrierWithServiceAndLocation = (body) => {
  return Axios.post(
    `/Carrier/GetAllShipraContractCarrierWithServiceAndLocation`,
    body,
  );
};

export const GetShipraContractByShipraContractCarrierId = (
  ShipraContractCarrierId,
) => {
  return Axios.get(
    `Carrier/GetShipraContractByShipraContractCarrierId?ShipraContractCarrierId=${ShipraContractCarrierId}`,
  );
};

export const PrepareTotalProcessingCheckout = (amount) => {
  const body = {
    amount,
  };
  return Axios.post(`/Wallet/PrepareTotalProcessingCheckout`, body);
};
export const GetTotalProcessingPaymentStatus = (CheckoutId) => {
  return Axios.get(
    `/Wallet/GetTotalProcessingPaymentStatus?CheckoutId=${CheckoutId}`,
  );
};

export const UploadProducts = (body) => {
  return Axios.post(`/Product/UploadProducts`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};
export const FetchPaybylinkbyServiceUUId = (PaymentLinkId) => {
  const body = {
    PaymentLinkId: PaymentLinkId,
  };
  return Axios.post(`/Wallet/FetchPaybylinkbyServiceUUId`, body);
};
export const GetClientKeys = () => {
  return Axios.get(`/Client/GetClientKeys`);
};
export const RotateKeys = (body) => {
  return Axios.post(`Client/RotateKeys`, body);
};
export const GetPDFAllOrdersByPayoutId = (body) => {
  return Axios.post(`/Wallet/GetPDFAllOrdersByPayoutId`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};
export const GetAllOrdersByPayoutId = (startDate, endDate, payoutId) => {
  const body = {
    filterModel: {
      createdFrom: startDate || null,
      createdTo: endDate || null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
    payoutId: payoutId || 0,
  };
  return Axios.post(`Wallet/GetAllOrdersByPayoutId`, body);
};

export const GetAllActiveCarrierForCreateOrderSelection = (CountryId) => {
  return Axios.get(
    `/Carrier/GetAllActiveCarrierForCreateOrderSelection?CountryId=${CountryId}`,
  );
};
export const CreateActiveCarrierPickupLocation = (body) => {
  return Axios.post(`Carrier/CreateActiveCarrierPickupLocation`, body);
};
export const GetValidatedOrderAddressByActiveCarrier = (body) => {
  return Axios.post(`Order/GetValidatedOrderAddressByActiveCarrier`, body);
};

export const GetActiveCarrierPickupLocationForSelection = (
  ActiveCarrierId,
  CarrierId,
) => {
  return Axios.get(
    `/Carrier/GetActiveCarrierPickupLocationForSelection?ActiveCarrierId=${ActiveCarrierId}&CarrierId=${CarrierId}`,
  );
};

export const GetCalculatedRateByCarrier = (body) => {
  return Axios.post(`Order/GetCalculatedRateByCarrier`, body);
};
export const UpdateValidatedOrderForCarrier = (body) => {
  return Axios.post(`Order/UpdateValidatedOrderForCarrier`, body);
};

export const GetCarrierLocationByCarrier = (CarrierId) => {
  return Axios.get(
    `/Carrier/GetCarrierLocationByCarrier?CarrierId=${CarrierId}`,
  );
};
export const GetGenericSetting = () => {
  return Axios.get(`/Client/GetGenericSetting`);
};
export const CreateUpdateClientGenericSetting = (settingConfig) => {
  const body = {
    settingConfig: settingConfig,
  };
  return Axios.post(`Client/CreateUpdateClientGenericSetting`, body);
};
export const GetDynamicApiCall = (url) => {
  return Axios.get(`${url}`);
};
export const GetAllCarrierWithCodPending = (startDate, endDate) => {
  const body = {
    filterModel: {
      createdFrom: startDate || null,
      createdTo: endDate || null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`/Account/GetAllCarrierWithCodPending`, body);
};

export const GenerateManifest = (orderNos) => {
  const body = {
    orderNos: orderNos,
  };
  return Axios.post(`/Order/GenerateManifest`, body, {
    responseType: "blob",
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GenerateProductLinkToken = (ProductId, StoreId) => {
  return Axios.get(
    `Product/GenerateProductLinkToken?ProductId=${ProductId}&StoreId=${StoreId}`,
  );
};

export const CreateClientOrderLabelLookup = (label, colorCode) => {
  const body = {
    model: {
      label: label,
      colorCode: colorCode,
    },
  };
  return Axios.post(`Order/CreateClientOrderLabelLookup`, body);
};
export const GetAllClientOrderLabelLookup = (startDate, endDate) => {
  const body = {
    filterModel: {
      createdFrom: startDate || null,
      createdTo: endDate || null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`/Order/GetAllClientOrderLabelLookup`, body);
};
export const UpdateClientOrderLabelLookup = (
  clientOrderLabelLookupId,
  labelName,
  colorCode,
) => {
  const body = {
    clientOrderLabelLookupId: clientOrderLabelLookupId,
    labelName: labelName,
    colorCode: colorCode,
  };
  return Axios.post(`/Order/UpdateClientOrderLabelLookup`, body);
};
export const DeleteClientOrderLabelLookup = (clientOrderLabelLookupId) => {
  const body = {
    clientOrderLabelLookupId: clientOrderLabelLookupId,
  };
  return Axios.post(`/Order/DeleteClientOrderLabelLookup`, body);
};
export const GetClientOrderLabelLookupId = (clientOrderLabelLookupId) => {
  const body = {
    clientOrderLabelLookupId: clientOrderLabelLookupId,
  };
  return Axios.post(`/Order/GetClientOrderLabelLookupId`, body);
};
export const GetAllClientOrderLabelLookupForSelection = () => {
  return Axios.get(`/Order/GetAllClientOrderLabelLookupForSelection`);
};
export const CreateClientOrderLabel = (body) => {
  return Axios.post(`Order/CreateClientOrderLabel`, body);
};
export const DeleteClientOrderLabel = (orderId, label) => {
  const body = {
    orderId: orderId,
    label: label,
  };
  return Axios.post(`/Order/DeleteClientOrderLabel`, body);
};
export const UpdateOrderWithSaleChannel = (orderNos, id, isRemove) => {
  const body = {
    orderNos: orderNos,
    saleChannelConfigId: id,
    isRemove: isRemove,
  };
  return Axios.post(`/Order/UpdateOrderWithSaleChannel`, body);
};

export const AddUpdateTableConfiguration = (tableName, dataModel = {}) => {
  const body = {
    tableName: tableName,
    dataModel: dataModel,
  };
  return Axios.post(`/Employee/AddUpdateTableConfiguration`, body);
};

export const GetAllEmployeeColumnConfiguration = () => {
  return Axios.get(`/Employee/GetAllEmployeeColumnConfiguration`);
};
export const GetAllCarrierLocationsByCarrier = (id) => {
  return Axios.get(`/Carrier/GetAllCarrierLocationsByCarrier?CarrierId=${id}`);
};
export const AssignStoreProduct = (body) => {
  return Axios.post(`Product/AssignStoreProduct`, body);
};

export const GetAllStoreWithTokenByProductId = (ProductId) => {
  return Axios.get(
    `Product/GetAllStoreWithTokenByProductId?ProductId=${ProductId}`,
  );
};

export const CreateSGenerateEncryptedKeyAgainstStoreAndStationtore = (
  storeId,
  StationId,
) => {
  const body = {
    storeId: storeId,
    StationId: StationId,
  };
  return Axios.post(`/Store/GenerateEncryptedKeyAgainstStoreAndStation`, body);
};

export const CreateImageGallery = (body) => {
  return Axios.post(`/Product/CreateImageGallery`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const GetAllImageGalleries = () => {
  return Axios.get(`/Product/GetAllImageGalleries`);
};
export const AddUpdateCustomDomain = (DomainName) => {
  const body = {
    DomainName: DomainName,
  };
  return Axios.post(`/Branding/AddUpdateCustomDomain`, body);
};
export const AddUpdateBranding = (logoUrl, faviconUrl) => {
  const body = {
    logoUrl: logoUrl,
    faviconUrl: faviconUrl,
  };
  return Axios.post(`/Branding/AddUpdateBranding`, body);
};
export const GetBrandingById = (ClientId) => {
  const body = { ClientId: ClientId };
  return Axios.post(`Branding/GetBrandingById`, body);
};
export const GetAllCustomDomains = (ClientId) => {
  return Axios.get(`Branding/GetAllCustomDomains`);
};

export const GetCustomDomainRecordByCustomDomainId = (CustomDomainId) => {
  return Axios.get(
    `Branding/GetCustomDomainRecordByCustomDomainId?CustomDomainId=${CustomDomainId}`,
  );
};
export const ExcelExportCarrierSettlementById = (
  CarrierPaymentSettlementId,
) => {
  return Axios.get(
    `/Account/ExcelExportCarrierSettlementById?CarrierPaymentSettlementId=${CarrierPaymentSettlementId}`,
    {
      responseType: "blob",
    },
  );
};
export const GetAllOrderStatusReport = (body) => {
  return Axios.post(`/Order/GetAllOrderStatusReport`, body);
};

export const GetAllOrderDrafts = () => {
  return Axios.get(`/Order/GetAllOrderDrafts`);
};

export const GetOrderDraftByDraftId = (draftId) => {
  return Axios.get(`Order/GetOrderDraftByDraftId?OrderDraftId=${draftId}`);
};

export const DeleteOrderDraft = (OrderDraftId) => {
  const body = {
    OrderDraftId: OrderDraftId,
  };
  return Axios.post(`/Order/DeleteOrderDraft`, body);
};

export const UploadPaperlessDocByCarrier = (body) => {
  return Axios.post(`/Order/UploadPaperlessDocByCarrier`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const GetActiveCarrierPickupLocationbyid = (id) => {
  return Axios.get(
    `/Carrier/GetActiveCarrierPickupLocationbyid?ActiveCarrierPickupLocationId=${id}`,
  );
};
export const DeletepickLocationById = (ActiveCarrierPickupLocationId) => {
  const body = {
    activeCarrierPickupLocationId: ActiveCarrierPickupLocationId,
  };
  return Axios.post(`/Carrier/deletepickLocationById`, body);
};
export const CreateThirdPartyPickupLocation = (body) => {
  return Axios.post(`/Order/CreateThirdPartyPickupLocation`, body);
};

export const GetAllUpsSettingSelectionByType = (type, Carrierid) => {
  return Axios.get(
    `CarrierSetting/GetAllUpsSettingSelectionByType?type=${type}&Carrierid=${Carrierid}`,
  );
};
export const GetDynamicApiCallWithURL = (url) => {
  return Axios.get(url);
};

export const GetAllWhatsappActivate = (createdFrom, createdTo) => {
  const body = {
    filterModel: {
      createdFrom: createdFrom,
      createdTo: createdTo,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`Whatsapp/GetAllWhatsappActivate`, body);
};
export const GetAllWhatsappLookupForSelection = () => {
  return Axios.get(`Whatsapp/GetAllWhatsappLookupForSelection`);
};

export const ActivateWhatsappProcess = (body) => {
  return Axios.post(`/Whatsapp/ActivateWhatsappProcess`, body);
};
export const GetWhatsappActivateById = (id) => {
  return Axios.get(
    `/Whatsapp/GetWhatsappActivateById?WhatsappactivateId=${id}`,
  );
};
export const GetMetaFieldByEntityId = (orderId) => {
  return Axios.get(`/MetaField/GetMetaFieldByEntityId?orderId=${orderId}`);
};
export const GetAllEntityMetaFieldLookup = () => {
  return Axios.get(`MetaField/GetAllEntityMetaFieldLookup`);
};
export const CreateUpdateClientMetaField = (body) => {
  return Axios.post(`/MetaField/CreateUpdateClientMetaField`, body);
};

export const GetAllClientRate = (body) => {
  return Axios.post(`/Carrier/GetAllClientRate`, body);
};

export const OrdersArchive = (body) => {
  return Axios.post(`/Order/OrdersArchive`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllArchiveOrders = (createdFrom, createdTo) => {
  const body = {
    FilterModel: {
      createdFrom: createdFrom || null,
      createdTo: createdTo || null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`/Order/GetAllArchiveOrders`, body);
};

export const ExcelOrdersArchive = (date) => {
  const body = {
    date: date,
  };
  return Axios.post(`/Order/ExcelOrdersArchive`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};

export const RestoreOrderArchive = (ArchiveNo) => {
  const body = {
    ArchiveNo: ArchiveNo,
  };
  return Axios.post(`/Order/RestoreOrderArchive`, body);
};

export const GetAllOrdersDraftCount = () => {
  return Axios.get(`Order/GetAllOrderDraftsCount`);
};

export const GetProductNameByOrderDraftId = (orderDraftId) => {
  const body = {
    OrderDraftId: orderDraftId,
  };
  return Axios.post(`/Order/GetProductNameByOrderDraftId`, body);
};

export const UpsertServiceRateGroup = (body) => {
  return Axios.post(`/ShipperContract/UpsertServiceRateGroup`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const UpsertShipperRatesWithSlabs = (body) => {
  return Axios.post(`/ShipperContract/UpsertShipperRatesWithSlabs`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const CreateInvoice = (body) => {
  return Axios.post(`/ShipperContract/CreateInvoice`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllServiceRateGroup = (body) => {
  return Axios.post(`/ShipperContract/GetAllServiceRateGroup`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};
export const GetAllShipperRateWithContract = (body) => {
  return Axios.post(`/ShipperContract/GetAllShipperRateWithContract`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllCivilEntityType = () => {
  return Axios.get(`/Country/GetAllCivilEntityType`);
};

export const GetAllSalePersons = () => {
  return Axios.get(`/Employee/GetAllSalePersons`);
};

export const GetShipperRateBySaleChannelConfig = (saleChannelConfigId) => {
  const body = {
    SaleChannelConfigId: saleChannelConfigId,
  };
  return Axios.post(`/ShipperContract/GetShipperRateBySaleChannelConfig`, body);
};

export const GetShipperInvoiceAdjustmentById = (shipperInvoiceAdjustmentId) => {
  const body = {
    ShipperInvoiceAdjustmentId: shipperInvoiceAdjustmentId,
  };
  return Axios.post(`/ShipperContract/GetShipperInvoiceAdjustmentById`, body);
};
export const GetShipperInvoiceAdjustmentBySCId = (saleChannelConfigId) => {
  const body = {
    SaleChannelConfigId: saleChannelConfigId,
  };
  return Axios.post(`/ShipperContract/GetShipperInvoiceAdjustmentBySCId`, body);
};

export const GetShipperRateBySaleChannelConfigGroup = () => {
  return Axios.get(`/ShipperContract/GetShipperRateBySaleChannelConfigGroup`);
};

export const GetAllShipperForGenerateInvocice = () => {
  return Axios.get(`/ShipperContract/GetAllShipperForGenerateInvocice`);
};

export const GetShipperOrderForInvoices = (
  createdFrom,
  createdTo,
  saleChannelConfigId,
) => {
  const body = {
    FilterModel: {
      createdFrom: createdFrom || null,
      createdTo: createdTo || null,
      start: 0,
      length: 100,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
    SaleChannelConfigId: saleChannelConfigId,
  };
  return Axios.post(`/ShipperContract/GetShipperOrderForInvoices`, body);
};

export const GetAllShipperInvoice = (body) => {
  return Axios.post(`/ShipperContract/GetAllShipperInvoice`, body);
};

export const ExportShipperInvoiceDetailsToExcel = (shipperInvoiceId) => {
  return Axios.get(
    `/ShipperContract/ExportShipperInvoiceDetailsToExcel?ShipperInvoiceId=${shipperInvoiceId}`,
    { responseType: "blob" },
  );
};

export const GetAllInvoiceStatus = () => {
  return Axios.get(`/ShipperContract/GetAllInvoiceStatus`);
};

export const GetAllTransactionType = () => {
  return Axios.get(`/ShipperContract/GetAllTransactionType`);
};

export const UpdateShipperInvoiceStatus = (body) => {
  return Axios.post(`/ShipperContract/UpdateShipperInvoiceStatus`, body, {
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const GetAllShipperInvoiceAdjustment = (body) => {
  return Axios.post(`/ShipperContract/GetAllShipperInvoiceAdjustment`, body);
};

export const AddUpdateShipperInvoiceAdjustment = (body) => {
  return Axios.post(`/ShipperContract/AddUpdateShipperInvoiceAdjustment`, body);
};

export const DeleteShipperInvoiceAdjustmentById = (
  shipperInvoiceAdjustmentId,
) => {
  const body = {
    ShipperInvoiceAdjustmentId: shipperInvoiceAdjustmentId,
  };
  return Axios.post(
    `/ShipperContract/DeleteShipperInvoiceAdjustmentById`,
    body,
  );
};
export const GetServiceRateGroupById = (serviceRateGroupId) => {
  return Axios.get(
    `/ShipperContract/GetServiceRateGroupById?ServiceRateGroupId=${serviceRateGroupId}`,
  );
};

export const GetShipperInvoiceReportPdf = (body) => {
  return Axios.post(`/ShipperContract/GetShipperInvoiceReportPdf`, body, {
    headers: {
      "Content-Type": "application/json",
    },
    responseType: "blob",
  });
};

export const GetAllCarrierShipmentPodFiles = (body) => {
  return Axios.post(`/Shipment/GetAllCarrierShipmentPodFiles`, body);
};

export const GetAllPermissionActionForSelection = () => {
  return Axios.get(`/Permission/GetAllPermissionActionForSelection`);
};

export const GetAllServiceRateGroupForSelection = () => {
  return Axios.get(`/ShipperContract/GetAllServiceRateGroupForSelection`);
};

export const GetAllPermissionGroupLookup = () => {
  return Axios.get(`/Permission/GetAllPermissionGroupLookup`);
};

export const UpdatePermissionActions = (body) => {
  return Axios.post(`/Permission/UpdatePermissionAction`, body);
};

export const CreatePermissionGroups = (body) => {
  return Axios.post(`/Permission/CreatePermissionGroup`, body);
};

export const UpdateControllerAndActionsRecord = () => {
  return Axios.get(`/Permission/UpdateControllerAndActionsRecord`);
};

export const UploadStoreFile = (body) => {
  return Axios.post(`/Store/UploadStoreFile`, body, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const CreateBulkStore = (body) => {
  return Axios.post(`/Store/CreateBulkStore`, body);
};

export const GetAllCustomers = (createdFrom, createdTo) => {
  const body = {
    FilterModel: {
      createdFrom: createdFrom || null,
      createdTo: createdTo || null,
      start: 0,
      length: EnumChangeFilterModelApiUrls.GET_ALL_CDP_CUSTOMER.length,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`/CDP/GetAllCustomers`, body);
};

export const RefreshCDPCustomers = () => {
  return Axios.get(`/CDP/RefreshCDPCustomers`);
};

export const GetCDPCustomerInfoByCustomerId = (id) => {
  return Axios.get(`/CDP/GetCDPCustomerInfoByCustomerId?CustomerId=${id}`);
};

export const CreateCDPCustomerSegmentsAndLabels = (body) => {
  return Axios.post(`/CDP/CreateCDPCustomerSegmentsAndLabels`, body);
};

export const CreateCDPCustomerNotesAndTasks = (body) => {
  return Axios.post(`/CDP/CreateCDPCustomerNotesAndTasks`, body);
};

export const GetMapApiKey = () => {
  return Axios.get(`/Order/GetMapApiKey`);
};

export const GetMetaFieldsByOrderIds = (body) => {
  return Axios.post(`/MetaField/GetMetaFieldsByOrderIds`, body);
};

export const UpdateMetaFieldForOrders = (body) => {
  return Axios.post(`/MetaField/UpdateMetaFieldForOrders`, body);
};

export const GetRoleForPerFormanceReport = (
  roleId,
  createdFrom,
  createdTo,
  countryIds,
) => {
  const cId =
    countryIds?.countryId || countryIds?.id || countryIds?.value || countryIds;
  const body = {
    RoleId: roleId,
    CountryIds: cId ? String(cId) : null,
    FilterModel: {
      createdFrom: createdFrom || null,
      createdTo: createdTo || null,
      start: 0,
      length: 1000,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`/PerformanceReport/GetRoleForPerFormanceReport`, body);
};

export const GetOrderByRole = (
  employeeId,
  roleId,
  createdFrom,
  createdTo,
  countryIds,
) => {
  const cId =
    countryIds?.countryId || countryIds?.id || countryIds?.value || countryIds;
  const body = {
    EmployeeId: employeeId,
    RoleId: roleId,
    CountryIds: cId ? String(cId) : null,
    FilterModel: {
      createdFrom: createdFrom || null,
      createdTo: createdTo || null,
      start: 0,
      length: 1000,
      search: "",
      sortDir: "desc",
      sortCol: 0,
    },
  };
  return Axios.post(`/PerformanceReport/GetOrderByRole`, body);
};

export const GetWipeOutSectionsLookup = () => {
  return Axios.get(`/Client/GetWipeOutSectionsLookup`);
};

export const WipeOutClientData = (section) => {
  const body = {
    Section: section,
  };
  return Axios.post(`/Client/WipeOutClientData`, body);
};

export const SendFactoryResetOtp = () => {
  return Axios.post(`/Client/SendFactoryResetOtp`);
};

export const GetAllClientMenuByRoleId = (roleId) => {
  return Axios.get(`/Permission/GetAllClientMenuByRoleId`);
};

export const GetMenuItemPermissions = (roleId) => {
  const clientUserRoleId = roleId || 83;
  return Axios.get(
    `/Permission/GetMenuItemPermissions?ClientUserRoleId=${clientUserRoleId}`,
  );
};

export const SaveMenuItemPermissions = (roleId, menuItemIds, menuIds) => {
  const body = {
    clientUserRoleId: roleId,
    menuItemIds: menuItemIds,
    menuIds: menuIds,
  };
  return Axios.post(`/Permission/SaveMenuItemPermissions`, body);
};

export const GetAllZonesWithCoords = () => {
  return Axios.post(`/Country/GetAllZonesWithCoords`);
};

export const SaveNewZone = (body) => {
  return Axios.post(`/Country/SaveNewZone`, body);
};

export const UpdateZoneFromZoneBoundries = (body) => {
  return Axios.post(`/Country/UpdateZoneFromZoneBoundries`, null, {
    params: body,
  });
};

export const DeleteZoneByID = (zoneID) => {
  return Axios.post(`/Country/DeleteZoneByID?zoneID=${zoneID}`);
};

export const GetAllCities = () => {
  return Axios.post(`/Distribution/GetAllCities`);
};

export const getVariantsWithoutStation = () => {
  return Axios.get("/Product/GetVariantsWithoutStation");
};

export const assignStationToVariants = (body) => {
  return Axios.post("/Product/AssignStationToVariants", body);
};

export const AssignStationToOrders = async (data) => {
  return await Axios.post("/Order/AssignStationToOrders", data);
};

export const SaveInventorySyncPolicy = (body) => {
  return Axios.post(`/InventorySyncPolicy/SaveInventorySyncPolicy`, body);
};

export const GetAllInventorySyncPolicies = (params) => {
  return Axios.get(`/InventorySyncPolicy/GetAllInventorySyncPolicies`, { params });
};

export const DeleteInventorySyncPolicy = (policyId) => {
  return Axios.post(`/InventorySyncPolicy/DeleteInventorySyncPolicy`, { policyId });
};

export const ToggleInventorySyncPolicyStatus = (policyId, active) => {
  return Axios.post(`/InventorySyncPolicy/ToggleInventorySyncPolicyStatus`, { policyId, active });
};

export const GetInventorySyncModeLookups = () => {
  return Axios.get(`/InventorySyncPolicy/GetInventorySyncModeLookups`);
};

export const GetInventorySyncDirectionLookups = () => {
  return Axios.get(`/InventorySyncPolicy/GetInventorySyncDirectionLookups`);
};

export const GetInventorySyncPolicyById = (policyId) => {
  return Axios.get(`/InventorySyncPolicy/GetInventorySyncPolicyById?policyId=${policyId}`);
};
