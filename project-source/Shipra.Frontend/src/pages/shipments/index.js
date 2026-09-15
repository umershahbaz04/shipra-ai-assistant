import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes, useLocation } from "react-router-dom";
import useDateRangeHook from "../../.reUseableComponents/CustomHooks/useDateRangeHook.js";
import useDeleteConfirmation from "../../.reUseableComponents/CustomHooks/useDeleteConfirmation.js";
import DeleteConfirmationModal from "../../.reUseableComponents/Modal/DeleteConfirmationModal.js";
import CustomReactDatePickerInputFilter from "../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SearchInputAutoCompleteMultiple from "../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple.js";
import SelectComponent from "../../.reUseableComponents/TextField/SelectComponent.js";
import wareHouseImage from "../../assets/images/WareHouse.png";
import {
  ExcelExportShipments,
  GenerateManifest,
  GetActiveCarriersForSelection,
  GetAllFullFillmentStatusLookup,
  GetAllOrderTypeLookup,
  GetAllPaymentMethodLookup,
  GetAllPaymentStatusLookup,
  GetAllSalePersonForSelection,
  GetAllStationLookup,
  GetCarrierWayBillsByOrderNos,
  GetShipmentTabsCount,
  GetShipmentTabsCountConfig,
  GetShipments,
  GetStoresForSelection,
  GetValidateDocumentSetting,
  UnAssignFromCarrier,
  UpdateShipmentTabDisplayOrder,
} from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style.js";
import AddBillingModal from "../../components/modals/orderModals/AddBillingModal.js";
import AddShipmentTabModal from "../../components/modals/orderModals/AddShipmentTabModal.js";
import GeneralTabBar from "../../components/shared/shipmentTabsBar";
import UtilityClass from "../../utilities/UtilityClass";
import {
  EnumAwbType,
  EnumChangeFilterModelApiUrls,
  EnumOptions,
} from "../../utilities/enum";
import {
  CicrlesLoading,
  CopyButton,
  CustomColorLabelledOutline,
  downloadWayBillsByOrderNos,
  graphColorsArr,
  greyBorder,
  handleDownloadMenifestPdf,
  handleDownloadPdf,
  handleFilterAddressSchemaSelectDataIncExcValues,
  useGetAllCarrierTrackingStatus,
  useGetAllCountries,
} from "../../utilities/helpers/Helpers.js";
import {
  useGetAllCitiesByRegionIds,
  useGetAllRegionbyCountryIds,
} from "../../utilities/helpers/HelpersFilter.js";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../utilities/helpers/addressSchema.js";
import CountrySchema from "../../utilities/helpers/countryschema.js";
import {
  errorNotification,
  successNotification,
  warningNotification,
} from "../../utilities/toast";
import ShipmentList from "./shipmentList/index.js";
import ReactApexChart from "react-apexcharts";

import { isShowCountryInTabbarFlag } from "../../utilities/cookies";

const MAX_TAGS = 200;
function ShipmentPage(props) {
  const orderListRef = useRef(null);
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  const {
    startDate: orderStartDate,
    endDate: orderEndDate,
    setStartDate: setOrderStartDate,
    setEndDate: setOrderEndDate,
    startDateFormated: orderStartDateFormated,
    endDateFormated: orderEndDateFormate,
  } = useDateRangeHook();

  const {
    selectedAddressSchema,
    selectedAddressSchemaForMutiple,
    selectedAddressSchemaWithObjValue,
    selectedAddressSchemaWithObjValueForMultiple,
    addressSchemaSelectData,
    addressSchemaSelectDataIncExcValues,
    handleSetSchema,
    handleChangeSelectAddressSchemaIncExcSwitch,
    handleChangeSelectAddressSchemaAndGetOptionsForMultiple,
    handleReset,
  } = useGetAddressSchema();
  const [allCounts, setAllCounts] = useState([]);
  const [carrierGroupedListCounts, setCarrierGroupedListCounts] = useState([]);
  const [allShipments, setAllShipments] = useState([]);
  const [allShipmentsLoading, setAllShipmentsLoading] = useState(false);

  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [shipmentTabsCountConfig, setShipmentTabsCountConfig] = useState([]);
  const [isTabsConfigLoading, setIsTabsConfigLoading] = useState(true);
  const [inputFields, setInputFields] = useState([]);
  const [updateShipmentTabs, setUpdateShipmentTabs] = useState([]);

  //#region
  let defaultValues = {
    startDate: null,
    endDate: null,
    orderStartDate: null,
    orderEndDate: null,
    storeId: [],
    orderTypeId: null,
    carrierId: null,
    fullFillmentStatusId: null,
    paymentStatusId: null,
    paymentMethodId: null,
    stationId: null,
  };
  //console.log("inputFields", inputFields);
  // const { register, getValues, setValue, reset, control } = useForm({
  //   defaultValues,
  // });

  const [storeId, setStoreId] = useState();
  const [orderTypeId, setOrderTypeId] = useState();
  const [carrierId, setCarrierId] = useState();
  const [fullFillmentStatusId, setFullFillmentStatusId] = useState(
    defaultValues.fullFillmentStatusId,
  );
  const [paymentStatusId, setPaymentStatusId] = useState(
    defaultValues.paymentStatusId,
  );
  const [paymentMethodId, setPaymentMethodId] = useState(
    defaultValues.paymentMethodId,
  );
  const [stationId, setStationId] = useState();
  const [isfilterClear, setIsfilterClear] = useState(false);

  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allOrderTypeLookup, setAllOrderTypeLookup] = useState([]);
  const [allActiveCarriersForSelection, setAllActiveCarriersForSelection] =
    useState([]);
  const [allFullFillmentStatusLookup, setAllFullFillmentStatusLookup] =
    useState([]);
  const [allPaymentStatusLookup, setAllPaymentStatusLookup] = useState([]);
  const [allPaymentMethodLookup, setAllPaymentMethodLookup] = useState([]);
  const [allStationLookup, setAllStationLookup] = useState([]);
  const [trackingStatusIds, setTrackingStatusIds] = useState([]);
  const [isTabFilter, setIsTabFilter] = useState(false);
  const [isUseSelectedOrderNo, setIsUseSelectedOrderNo] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [statusMoved, setStatusMoved] = useState(false);
  const [isMovingStatus, setIsMovingStatus] = useState(false);
  const [allSalesPerson, setAllSalesPerson] = useState([]);
  const [selectedSalesPerson, setSelectedSalesPerson] = useState([]);
  const [selectedCarrierTrackingStatus, setSelectedCarrierTrackingStatus] =
    useState([]);
  const [inputValue, setinputValue] = useState("");
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const [openBillingOrder, setOpenBillingOrder] = useState(false);
  const [documentSetting, setDocumentSetting] = useState({});
  const [selectionModel, setSelectionModel] = useState([]);
  const [lastCarrierTrackingIds, setLastCarrierTrackingIds] = useState(0);
  const [showCarrierGraph, setShowCarrierGraph] = useState(false);
  let getValidateDocumentSetting = async () => {
    let res = await GetValidateDocumentSetting();
    if (res.data.result !== null) {
      setDocumentSetting(res.data.result);
    }
  };

  let getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    //console.log("getStoresForSelection", res.data);
    if (res.data.result != null) {
      setStoresForSelection(res.data.result);
    }
  };
  const getAllOrderTypeLookup = async () => {
    try {
      const response = await GetAllOrderTypeLookup();
      //console.log("response of api of GetAllOrderTypeLookup", response);
      setAllOrderTypeLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllActiveCarriersForSelection = async () => {
    try {
      const response = await GetActiveCarriersForSelection();
      //console.log("response of api of GetAllOrderTypeLookup", response);
      setAllActiveCarriersForSelection(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllFullFillmentStatusLookup = async () => {
    try {
      const response = await GetAllFullFillmentStatusLookup();
      //console.log("response of api of GetAllOrderTypeLookup", response);
      setAllFullFillmentStatusLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllPaymentStatusLookup = async () => {
    try {
      const response = await GetAllPaymentStatusLookup();
      //console.log("response of api of GetAllOrderTypeLookup", response);
      setAllPaymentStatusLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllPaymentMethodLookup = async () => {
    try {
      const response = await GetAllPaymentMethodLookup();
      //console.log("response of api of GetAllOrderTypeLookup", response);
      setAllPaymentMethodLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllSalePersonForSelection = async () => {
    try {
      const response = await GetAllSalePersonForSelection();
      //console.log("response of api of GetAllOrderTypeLookup", response);
      setAllSalesPerson(response.data.result || []);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllStationLookup = async () => {
    try {
      const response = await GetAllStationLookup();
      //console.log("response of api of GetAllOrderTypeLookup", response);
      setAllStationLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  useEffect(() => {
    getStoresForSelection();
    getAllOrderTypeLookup();
    getAllActiveCarriersForSelection();
    getAllFullFillmentStatusLookup();
    getAllPaymentStatusLookup();
    getAllPaymentMethodLookup();
    getAllStationLookup();
    getAllSalePersonForSelection();
    getValidateDocumentSetting();
  }, []);
  //#region location filter
  const {
    loading: countryLoading,
    countries,
    selectedCountryIds,
    setSelectedCountryIds,
  } = useGetAllCountries();
  const {
    loading: citiesLoading,
    allCities,
    handleGetAllCitiesByIds,
    selectedCities,
    setSelectedCities,
    setAllCities,
  } = useGetAllCitiesByRegionIds();
  const {
    loading: regionLoading,
    allRegions,
    handleGetAllRegionsByIds,
    selectedRegions,
    setSelectedRegions,
    setAllRegions,
  } = useGetAllRegionbyCountryIds();
  const [
    allCarrierTrackingStatusDependcy,
    setAllCarrierTrackingStatusDependcy,
  ] = useState(false);
  const { loading: abdjbsdf, allCarrierTrackingStatus } =
    useGetAllCarrierTrackingStatus([allCarrierTrackingStatusDependcy]);
  useEffect(() => {
    if (selectedCountryIds.length > 0) {
      const countryIds = selectedCountryIds
        .map((country) => country.countryId)
        .join(",");

      handleGetAllRegionsByIds(countryIds);
    } else {
      setAllRegions([]);
      setAllCities([]);
    }
  }, [selectedCountryIds]);
  const [regionIncludeChecked, setRegionIncludeChecked] = useState(true);
  const getIncludeExcludeRegionIds = () => {
    let newVal = selectedRegions;
    if (!regionIncludeChecked) {
      newVal = allRegions.filter(
        (region) =>
          !selectedRegions.some((selected) => region.id === selected.id),
      );
    }
    return newVal.map((country) => country.id).join(",");
  };
  const [cityIncludeChecked, setCityIncludeChecked] = useState(true);
  const getIncludeExcludeCityIds = () => {
    let newVal = selectedCities;
    if (!cityIncludeChecked) {
      newVal = allCities.filter(
        (region) =>
          !selectedCities.some((selected) => region.id === selected.id),
      );
    }
    return newVal.map((cty) => cty.id).join(",");
  };
  useEffect(() => {
    if (selectedRegions.length > 0) {
      let regionIds = selectedRegions.map((country) => country.id).join(",");
      handleGetAllCitiesByIds(regionIds);
      setAllCities([]);
    } else {
      setAllCities([]);
    }
  }, [selectedRegions]);
  //#endregion

  const resetFilter = () => {
    resetDates();
    setStoreId(null);
    setSelectedSalesPerson(null);
    setOrderTypeId(null);
    setCarrierId(null);
    setFullFillmentStatusId(null);
    setPaymentStatusId(null);
    setPaymentMethodId(null);
    setStationId(null);
    setSelectedCarrierTrackingStatus(null);
    handleReset();
  };
  const location = useLocation();
  useEffect(() => {
    if (location.pathname === "/shipments") {
      resetFilter();
    }
  }, [location.pathname]);
  const handleOptionSelect = (event) => {
    const selectedValues = event.target.value;
    setStoreId(selectedValues);
  };
  //#endregion
  const handleFocus = (event) => event.target.select();
  const getFiltersFromStateForExport = (carrierTrackingStatusIds) => {
    if (carrierTrackingStatusIds == 0) {
      carrierTrackingStatusIds = 0; //dropdown value
    }
    let search = inputFields.join();

    if (getOrdersRef.current && getOrdersRef.current?.length > 0) {
      search = getOrdersRef.current?.join();
    } else {
      search = inputFields.join();
    }

    let param = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: EnumChangeFilterModelApiUrls.GET_ALL_SHIPMENTS.length,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
      storeId: storeId?.map((data) => data.storeId).toString(),
      SalePersonIds: selectedSalesPerson?.map((data) => data.id).toString(),
      orderTypeId: orderTypeId?.orderTypeId,
      carrierId: carrierId?.carrierId,
      fullFillmentStatusId: fullFillmentStatusId?.id,
      paymentStatusId: paymentStatusId?.id,
      orderRequestVia: 0,
      paymentMethodId: paymentMethodId?.paymentMethodId,
      carrierTrackingStatusIds: carrierTrackingStatusIds,
      CountryIds: selectedCountryIds?.map((data) => data.countryId).toString(),
      RegionIds: selectedRegions?.map((data) => data.id).toString(),
      CityIds: selectedCities?.map((data) => data.id).toString(),
      carrierTrackingStatusIds: selectedCarrierTrackingStatus
        ?.map((data) => data.carrierTrackingStatusId)
        .toString(),
      IncludeRegion: regionIncludeChecked,
      IncludeCity: cityIncludeChecked,
      orderFromDate: orderStartDateFormated || null,
      orderToDate: orderEndDateFormate || null,
      OrderAddressFilter: handleFilterAddressSchemaSelectDataIncExcValues(
        addressSchemaSelectDataIncExcValues,
      ),
    };
    return param;
  };
  const getFiltersFromState = (carrierTrackingStatusIds, overrideCountryId) => {
    if (carrierTrackingStatusIds == 0) {
      carrierTrackingStatusIds = 0; //dropdown value
    }
    let search = inputFields.join();

    if (
      getOrdersRef.current &&
      getOrdersRef.current?.length > 0 &&
      isUseSelectedOrderNo
    ) {
      search = getOrdersRef.current?.join();
    } else {
      search = inputFields.join();
    }
    let param = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: EnumChangeFilterModelApiUrls.GET_ALL_SHIPMENTS.length,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
      storeId: storeId?.map((data) => data.storeId).toString(),
      orderTypeId: orderTypeId?.orderTypeId,
      carrierId: carrierId?.carrierId,
      fullFillmentStatusId: fullFillmentStatusId?.id,
      paymentStatusId: paymentStatusId?.id,
      orderRequestVia: 0,
      paymentMethodId: paymentMethodId?.paymentMethodId,
      stationId:
        stationId?.productStationId === "undefined"
          ? ""
          : stationId?.productStationId,
      carrierAssign: 0,
      SalePersonIds: selectedSalesPerson?.map((data) => data.id).toString(),
      countryId: overrideCountryId !== undefined
        ? overrideCountryId
        : (isShowCountryInTabbarFlag()
            ? (reduxSelectedCountry
                ? String(reduxSelectedCountry.countryId || reduxSelectedCountry.id || reduxSelectedCountry)
                : "")
            : (selectedAddressSchema.country
                ? String(selectedAddressSchema.country)
                : "")),
      cityIds: String(selectedAddressSchemaForMutiple.city),
      provinceIds: String(selectedAddressSchemaForMutiple.province),
      stateIds: String(selectedAddressSchemaForMutiple.state),
      pinCodeIds: String(selectedAddressSchemaForMutiple.pinCode),
      areaIds: String(selectedAddressSchemaForMutiple.area),
      carrierTrackingStatusIds:
        selectedCarrierTrackingStatus?.length > 0
          ? selectedCarrierTrackingStatus
              ?.map((data) => data.carrierTrackingStatusId)
              .toString()
          : carrierTrackingStatusIds,
      IncludeRegion: regionIncludeChecked,
      IncludeCity: cityIncludeChecked,
      orderFromDate: orderStartDateFormated || null,
      orderToDate: orderEndDateFormate || null,
      OrderAddressFilter: handleFilterAddressSchemaSelectDataIncExcValues(
        addressSchemaSelectDataIncExcValues,
      ),
    };
    return param;
  };
  let getShipmentTabsCount = async (carrierTrackingStatusIds, overrideCountryId) => {
    let param = getFiltersFromState(carrierTrackingStatusIds, overrideCountryId);

    let res = await GetShipmentTabsCount(param);
    if (res.data.result !== null) {
      setAllCounts(res.data.result?.list);
      const carrierList = res.data.result?.carrierGoupedList || [];

      const updatedList = carrierList.map((item) => {
        const deliveryRatio =
          item.TotalShipment > 0
            ? (item.COMPLETED / item.TotalShipment) * 100
            : 0;

        return {
          ...item,
          DeliveryRatio: Number(deliveryRatio.toFixed(2)),
        };
      });

      setCarrierGroupedListCounts(updatedList);
    }
  };

  let getShipments = async (carrierTrackingStatusIds, overrideCountryId) => {
    const idsToUse =
      carrierTrackingStatusIds === 0
        ? lastCarrierTrackingIds
        : carrierTrackingStatusIds;
    if (idsToUse !== 0) setLastCarrierTrackingIds(idsToUse);
    let param = getFiltersFromState(idsToUse, overrideCountryId);
    setAllShipmentsLoading(true);
    try {
      let res = await GetShipments(param);
      if (res?.data?.result !== null && res?.data?.result !== undefined) {
        setAllShipments(res.data.result);
      }
    } catch (err) {
      console.error("Error fetching shipments:", err);
    } finally {
      setAllShipmentsLoading(false);
    }
  };
  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry
  );
  const isInitialCountryMountShipments = useRef(true);
  useEffect(() => {
    if (!isShowCountryInTabbarFlag()) return;
    if (isInitialCountryMountShipments.current) {
      isInitialCountryMountShipments.current = false;
      return;
    }
    const resolvedCountryId =
      reduxSelectedCountry?.countryId ||
      reduxSelectedCountry?.id ||
      (typeof reduxSelectedCountry === "number" || typeof reduxSelectedCountry === "string"
        ? reduxSelectedCountry
        : "");
    handleSetSchema("country", reduxSelectedCountry || null);
    getShipments(0, resolvedCountryId ? String(resolvedCountryId) : "");
    getShipmentTabsCount(0, resolvedCountryId ? String(resolvedCountryId) : "");
  }, [reduxSelectedCountry]);

  let getShipmentTabsCountConfig = async () => {
    setIsTabsConfigLoading(true);
    try {
      let res = await GetShipmentTabsCountConfig();
      if (res?.data?.result && Array.isArray(res.data.result) && res.data.result.length > 0) {
        setShipmentTabsCountConfig(res.data.result);
      } else {
        setShipmentTabsCountConfig([
          { dashboardStatusName: "All", dashboardStatusNameForKey: "ALL", displayOrder: 1, shipmentGridColumnId: 0 }
        ]);
      }
    } catch (err) {
      console.error("Error fetching shipment tabs config:", err);
      setShipmentTabsCountConfig([
        { dashboardStatusName: "All", dashboardStatusNameForKey: "ALL", displayOrder: 1, shipmentGridColumnId: 0 }
      ]);
    } finally {
      setIsTabsConfigLoading(false);
    }
  };
  const handleFilterClear = () => {
    resetFilter();
    setIsfilterClear(true);
  };

  const handleFilter = () => {
    getShipments(0);
    getShipmentTabsCount();
  };
  useEffect(() => {
    if (inputValue.length <= MAX_TAGS) {
      getShipments(0);
      getShipmentTabsCount(0);
      getShipmentTabsCountConfig();
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MAXMIUM_NUMBER_REACHED,
      );
    }
  }, [inputFields]);
  useEffect(() => {
    if (isfilterClear) {
      getShipments(0);
      setIsfilterClear(false);
      getShipmentTabsCount(0);
      getShipmentTabsCountConfig();
      // downloadExcel(0);
    }
  }, [isfilterClear]);
  useEffect(() => {
    if (statusMoved) {
      const url = location.pathname;
      const lastPart = url.split("/").filter(Boolean).pop();
      let carrierStatusId = 0;
      if (lastPart) {
        let filterData = shipmentTabsCountConfig.find(
          (item) => item.dashboardStatusNameForKey === lastPart?.toUpperCase(),
        );
        carrierStatusId = filterData?.dashboardStatusValue || 0;
      }
      getShipmentTabsCount(0);
      getShipmentTabsCountConfig();

      setStatusMoved(false);
      // downloadExcel(0);
    }
  }, [statusMoved]);
  useEffect(() => {
    if (updateShipmentTabs.length > 0) {
      var newUpdatedTabs = updateShipmentTabs.map((status, index) => {
        return {
          shipmentGridColumnId: status?.shipmentGridColumnId,
          displayOrder: status?.displayOrder,
        };
      });

      let params = {
        list: newUpdatedTabs,
      };
      setIsMovingStatus(true);
      UpdateShipmentTabDisplayOrder(params)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
          } else {
            setStatusMoved(true);
          }
        })
        .catch((e) => {
          console.log("e", e);
        })
        .finally((e) => {
          setIsMovingStatus(false);
        });
    }
  }, [updateShipmentTabs]);

  const updatedResult = shipmentTabsCountConfig.map((status) => {
    let shipmentGridColumnId = status.shipmentGridColumnId;
    let label = status.dashboardStatusName;
    let displayOrder = status.displayOrder;
    let statusIds = status.dashboardStatusValue;
    let route;
    let count;
    let mainRoute = "shipments";
    if (status.dashboardStatusName.toLowerCase() == "All".toLowerCase()) {
      route = `/${mainRoute}`;
      count = allCounts[0]?.TotalShipment;
    } else {
      let urlRoute = status.dashboardStatusNameForKey.toLocaleLowerCase();
      route = `/${mainRoute}/${urlRoute}`;
      count =
        (allCounts.length > 0 &&
          allCounts[0][status.dashboardStatusNameForKey]) ||
        0;
    }

    return {
      label,
      route,
      count,
      statusIds,
      displayOrder,
      shipmentGridColumnId,
    };
  });
  const handleOnClickActionBtn = (method) => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length === 0) {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST,
      );
    } else {
      method();
    }
  };
  // useEffect(() => {
  //   getShipments();
  // }, []);
  /////actions
  const EnumShipmentDashboardAction = {
    EXCELEXPORT: "ExcelExport",
    PRINTLABEL: "PrintLabel",
    PRINTCARRIERLABEL: "PrintCarrierLabel",
    UNaSSIGNFROMCARRIER: "UNaSSIGNFROMCARRIER",
    REFRESHTRACKINGSTATUS: "REFRESHTRACKINGSTATUS",
    GENERATEMENIFEST: "GENERATEMENIFEST",
  };
  const EnumShipmentDashboardTabs = {
    EXCELEXPORT: "ExcelExport",
    PRINTLABEL: "PrintLabel",
  };
  const handleActionUpdateDetail = async (type, actionValue) => {
    let selectedTrNos = getOrdersRef.current;
    //for export
    if (actionValue === EnumShipmentDashboardAction.EXCELEXPORT) {
      //export excel api
      await downloadExcel(trackingStatusIds);
      setIsUseSelectedOrderNo(true);
    }
    if (actionValue === EnumShipmentDashboardAction.PRINTLABEL) {
      if (documentSetting.DocumentTemplateId !== null) {
        const selectedTrNos = getOrdersRef.current;
        handleOnClickActionBtn(() =>
          downloadWayBillsByOrderNos(
            selectedTrNos.join(","),
            documentSetting.DocumentTemplateId,
          ),
        );
      } else {
        handleOnClickActionBtn(() => setOpenBillingOrder(true));
      }
    }

    if (actionValue === EnumShipmentDashboardAction.PRINTCARRIERLABEL) {
      handleOnClickActionBtn(() => {
        downloadCarrierWayBillsByOrderNos(
          selectedTrNos.join(","),
          EnumAwbType.Awb4x6TypeId,
        );
      });
    }
    if (actionValue === EnumShipmentDashboardAction.UNaSSIGNFROMCARRIER) {
      const selectedTrNos = getOrdersRef.current;
      handleOnClickAction(() =>
        handleUnAssignConfirmation(selectedTrNos.join(",")),
      );
    }
    if (actionValue === EnumShipmentDashboardAction.GENERATEMENIFEST) {
      const selectedTrNos = getOrdersRef.current;
      handleGenerateMenifest(() =>
        handleGenerateMenifestPDF(selectedTrNos.join(",")),
      );
    }
    if (actionValue === EnumShipmentDashboardAction.REFRESHTRACKINGSTATUS) {
      console.log("Click");
      orderListRef.current.Refresh();
    }
  };
  // useEffect(() => {
  //   if (isUseSelectedOrderNo) {
  //     downloadExcel(trackingStatusIds);
  //     setIsUseSelectedOrderNo(false);
  //   }
  // }, [isUseSelectedOrderNo]);

  const downloadCarrierWayBillsByOrderNos = async (orderNo, awbTypeId) => {
    const params = {
      orderNos: orderNo,
      awbTypeId: awbTypeId,
    };
    await GetCarrierWayBillsByOrderNos(params)
      .then((res) => handleDownloadPdf(res, "AWB", handleResetSeelctedRows))
      .catch((e) => {
        console.log("e", e);
      })
      .finally((e) => {});
  };
  const handleOnClickAction = (method) => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length > 0) {
      method();
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST,
      );
    }
  };
  const handleGenerateMenifest = (method) => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length > 0) {
      method();
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST,
      );
    }
  };
  const {
    openDelete: unassignOpen,
    setOpenDelete: unassignSetOpenDelete,
    handleDeleteClose: unassignHandleDeleteClose,
    handleSetDeletedItem: unassignHandleSetDeletedItem,
    deletedItem: unassignDeletedItem,
    setLoadingConfirmation: unassignSetLoading,
    loadingConfirmation: unassignLoading,
  } = useDeleteConfirmation();
  const handleUnAssignConfirmation = (orderNos) => {
    unassignHandleSetDeletedItem(orderNos);
    unassignSetOpenDelete(true);
  };

  const handleGenerateMenifestPDF = async () => {
    const selectedTrNos = getOrdersRef.current;
    console.log(selectedTrNos);
    const response = await GenerateManifest(selectedTrNos.join(","));
    console.log(response);
    handleDownloadMenifestPdf(response, "menifest", handleResetSeelctedRows);
  };

  const handleResetSeelctedRows = () => {
    resetRowRef.current = true;
  };
  const downloadExcel = (carrierTrackingStatusIds) => {
    let params = getFiltersFromStateForExport(carrierTrackingStatusIds);
    ExcelExportShipments(params)
      .then((res) => {
        getOrdersRef.current = [];
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadExcel(res.data, "shipments");
          // setIsLoading(false);
        } else {
          successNotification("Order not found");
          // setIsLoading(false);
        }
      })
      .catch((e) => {
        //console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  useEffect(() => {
    setStationId();
    setCarrierId();
    setPaymentStatusId();
    setFullFillmentStatusId();
    setOrderTypeId();
    setPaymentMethodId();
    setStoreId();
  }, [
    isFilterOpen,
    allStationLookup,
    allActiveCarriersForSelection,
    allFullFillmentStatusLookup,
    allPaymentStatusLookup,
    allOrderTypeLookup,
    allPaymentMethodLookup,
    storesForSelection,
  ]);
  const [openAddTabModel, setOpenAddTabModel] = useState(false);
  const handleAddTab = (e) => {
    setOpenAddTabModel(true);
  };
  const handleUnAssignFromCarrier = async () => {
    try {
      let param = {
        orderNos: unassignDeletedItem,
      };
      unassignSetLoading(true);
      const response = await UnAssignFromCarrier(param);
      if (!response.data?.isSuccess) {
        UtilityClass.showErrorNotificationWithDictionary(response.data?.errors);
      } else {
        getShipments(0);
        handleResetSeelctedRows();
        unassignHandleDeleteClose();
        successNotification("Order unassign successfully");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
      unassignSetLoading(false);
    }
  };
  const handleCopyOrder = async () => {
    const copyText = selectionModel.join(" ").trim();
    if (!copyText) {
      warningNotification("Please Select Order From Table");
      return false;
    }
    await navigator.clipboard.writeText(copyText);
    successNotification("Order Number Copied");
    return true;
  };

  const handleGetShipmentConfigStatusNameByKey = (key) => {
    const statusObj = shipmentTabsCountConfig.find(
      (item) =>
        item.dashboardStatusNameForKey ===
        (key === "TotalShipment" ? "All" : key),
    );
    const statusName = statusObj?.dashboardStatusName;
    return statusName || key;
  };

  const handleShowCarrierGraph = () => {
    setShowCarrierGraph(true);
  };

  const EXCLUDED_KEYS = [
    "CarrierId",
    "CarrierName",
    "CarrierImage",
    "DeliveryRatio",
    "TotalShipment",
  ];

  const getPieSeries = (dt) => {
    return Object.entries(dt || {})
      .filter(([key]) => !EXCLUDED_KEYS.includes(key))
      .map(([_, value]) => Number(value) || 0);
  };

  const getPieOption = (dt) => {
    const entries = Object.entries(dt).filter(
      ([key]) => !EXCLUDED_KEYS.includes(key),
    );

    return {
      colors: [
        "#1f77b4",
        "#ff7f0e",
        "#2ca02c",
        "#e2ce1c",
        "#9467bd",
        "#8c564b",
        "#e377c2",
      ],

      chart: {
        type: "pie",
        sparkline: {
          enabled: false,
        },
        margins: {
          left: 0,
          right: 0,
          top: 0,
          bottom: 0,
        },
        padding: {
          left: 0,
          right: 0,
          top: 0,
          bottom: 0,
        },
      },

      labels: entries.map(([key]) =>
        handleGetShipmentConfigStatusNameByKey(key),
      ),

      legend: {
        show: true,
        position: "bottom",
        horizontalAlign: "center",
        floating: false,
        fontSize: "12px",
        offsetY: 5,
        markers: {
          width: 12,
          height: 12,
        },
      },

      dataLabels: {
        enabled: true,
        formatter: function (_, opts) {
          return opts.w.config.series[opts.seriesIndex];
        },
        style: {
          fontSize: "12px",
          fontWeight: 600,
        },
      },

      plotOptions: {
        pie: {
          size: "100%",
          offsetX: 0,
          offsetY: 0,
          donut: {
            size: "100%",
          },
        },
      },

      tooltip: {
        y: {
          formatter: function (value) {
            return value;
          },
        },
      },

      responsive: [
        {
          breakpoint: 480,
          options: {
            chart: {
              width: "100%",
            },
            legend: {
              position: "bottom",
              offsetY: 5,
            },
          },
        },
      ],
    };
  };

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <CustomColorLabelledOutline
          isCollapse={true}
          label={"Carriers"}
          defaultCollapseState={false}
          onClick={handleShowCarrierGraph}
        >
          {showCarrierGraph && (
            <Grid container spacing={2}>
              {carrierGroupedListCounts?.map((dt) => {
                return (
                  <Grid item xs={12} sm={6} md={3} key={dt.CarrierId}>
                    <Card
                      sx={{
                        height: "100%",
                        display: "flex",
                        flexDirection: "column",
                        bgcolor: "#f5f5f5",
                        border: "1px solid #e8e8e8",
                        boxShadow: "none",
                        transition: "all 0.2s ease",
                        "&:hover": {
                          boxShadow: "0 2px 8px rgba(0, 0, 0, 0.08)",
                          borderColor: "#ddd",
                        },
                        borderRadius: 1.5,
                      }}
                    >
                      <CardContent
                        sx={{ p: 1.5, flexGrow: 1, pb: "12px !important" }}
                      >
                        <Box display="flex" alignItems="center" mb={1}>
                          <Avatar
                            variant="rounded"
                            src={
                              dt?.CarrierImage === ""
                                ? wareHouseImage
                                : dt.CarrierImage
                            }
                            alt={dt?.CarrierName}
                            sx={{
                              width: 48,
                              height: 48,
                              bgcolor: "#fff",
                              borderRadius: 1,
                              color: "#555",
                              border: "1px solid #e0e0e0",
                              "& img": {
                                objectFit: "contain !important",
                              },
                            }}
                          />
                          <Box ml={2} minWidth={0}>
                            <Typography
                              variant="subtitle2"
                              sx={{
                                fontWeight: 600,
                                color: "#212121",
                                fontSize: "0.95rem",
                                wordBreak: "break-word",
                              }}
                            >
                              {dt.CarrierName}
                            </Typography>
                          </Box>
                        </Box>
                        <Stack spacing={1.2}>
                          <ReactApexChart
                            options={getPieOption(dt)}
                            series={getPieSeries(dt)}
                            type="pie"
                            height={200}
                          />
                        </Stack>
                        <Box
                          sx={{
                            display: "flex",
                            flexDirection: "column",
                          }}
                        >
                          <Typography variant="subtitle2" alignSelf={"end"}>
                            Delivery Ratio : {dt?.DeliveryRatio}
                          </Typography>
                          <Typography variant="subtitle2" alignSelf={"end"}>
                            Total Shipment : {dt?.TotalShipment}
                          </Typography>
                        </Box>
                      </CardContent>
                    </Card>
                  </Grid>
                );
              })}
            </Grid>
          )}
        </CustomColorLabelledOutline>
        <Box
          sx={{
            mt: "10px",
            background: "#F8F8F8",
            border: "1px solid rgba(224, 224, 224, 1)",
            borderRadius: "8px 8px 0px 0px!important",
          }}
        >
          <SearchInputAutoCompleteMultiple
            handleFocus={handleFocus}
            onChange={(e, value) => {
              setInputFields(value.slice(0, MAX_TAGS));
              setinputValue(value);
            }}
            inputFields={inputFields}
            MAX_TAGS={MAX_TAGS}
          />
          {!isTabsConfigLoading && !isMovingStatus ? (
            <GeneralTabBar
              isFilterOpen={isFilterOpen}
              setIsFilterOpen={setIsFilterOpen}
              handleAddTab={handleAddTab}
              tabData={updatedResult.length > 0 ? updatedResult : [
                {
                  label: "All",
                  route: "/shipments",
                  count: allCounts?.[0]?.TotalShipment || 0,
                  statusIds: 0,
                  displayOrder: 1,
                  shipmentGridColumnId: 0,
                }
              ]}
              getShipments={getShipments}
              setTrackingStatusIds={setTrackingStatusIds}
              setUpdateShipmentTabs={setUpdateShipmentTabs}
              {...props}
              // width="auto"
              placeholder="Action"
              placement={"bottom"}
              copybtn={
                <Tooltip title="Select order from table" arrow>
                  <CopyButton label="Order No" onClick={handleCopyOrder} />
                </Tooltip>
              }
              options={[
                {
                  title: LanguageReducer?.languageType?.SHIPMENTS_EXCEL_EXPORT,
                  value: EnumShipmentDashboardAction.EXCELEXPORT,
                },
                {
                  title: LanguageReducer?.languageType?.SHIPMENTS_PRINT_LABELS,
                  value: EnumShipmentDashboardAction.PRINTLABEL,
                },
                {
                  title:
                    LanguageReducer?.languageType
                      ?.SHIPMENTS_PRINT_CARRIER_LABELS,
                  value: EnumShipmentDashboardAction.PRINTCARRIERLABEL,
                },
                {
                  title:
                    LanguageReducer?.languageType
                      ?.SHIPMENTS_REFRESH_TRACKING_STATUS,
                  value: EnumShipmentDashboardAction.REFRESHTRACKINGSTATUS,
                },
                {
                  title:
                    LanguageReducer?.languageType
                      ?.SHIPMENTS_UNASSIGN_FROM_CARRIER,
                  value: EnumShipmentDashboardAction.UNaSSIGNFROMCARRIER,
                  //   onClick: () => {
                  //     const selectedTrNos = getOrdersRef.current;
                  //     handleOnClickAction(() =>
                  //       handleUnAssignConfirmation(selectedTrNos.join(","))
                  //     );
                  //   },
                },
                {
                  title: "Generate manifest",
                  value: EnumShipmentDashboardAction.GENERATEMENIFEST,
                },
              ]}
              optionsTabSetting={[
                {
                  title: LanguageReducer?.languageType?.SHIPMENTS_ADD_TAB,
                  value: EnumShipmentDashboardAction.EXCELEXPORT,
                },
              ]}
              onChangeMenu={(value) => handleActionUpdateDetail("type", value)}
            />
          ) : (
            <Box
              mx={1}
              my={2}
              sx={{
                color: "var(--primary-color) !important",
                fontSize: 14,
                fontWeight: "bold",
              }}
            >
              <CicrlesLoading />
            </Box>
          )}
          {isFilterOpen ? (
            <Table
              sx={{ ...styleSheet.generalFilterArea }}
              size="small"
              aria-label="a dense table"
            >
              <TableHead>
                <TableRow>
                  <Grid container spacing={1} sx={{ p: "20px" }}>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {/* {LanguageReducer?.languageType?.SHIPMENTS_START_DATE} */}
                          Created From
                        </InputLabel>

                        <CustomReactDatePickerInputFilter
                          value={startDate}
                          onClick={(date) => setStartDate(date)}
                          size="small"
                          isClearable
                          maxDate={UtilityClass.todayDate()}

                          // inputProps={{ style: { padding: "4px 5px" } }}
                        />
                      </Grid>
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          Created From
                          {/* {LanguageReducer?.languageType?.SHIPMENTS_END_DATE} */}
                        </InputLabel>
                        <CustomReactDatePickerInputFilter
                          value={endDate}
                          onClick={(date) => setEndDate(date)}
                          size="small"
                          minDate={startDate}
                          disabled={!startDate ? true : false}
                          isClearable
                          maxDate={UtilityClass.todayDate()}

                          // inputProps={{ style: { padding: "4px 5px" } }}
                        />
                      </Grid>
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {/* {LanguageReducer?.languageType?.SHIPMENTS_START_DATE} */}
                          Order From
                        </InputLabel>

                        <CustomReactDatePickerInputFilter
                          value={orderStartDate}
                          onClick={(date) => setOrderStartDate(date)}
                          size="small"
                          isClearable

                          // inputProps={{ style: { padding: "4px 5px" } }}
                        />
                      </Grid>
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          Order To
                          {/* {LanguageReducer?.languageType?.SHIPMENTS_END_DATE} */}
                        </InputLabel>
                        <CustomReactDatePickerInputFilter
                          value={orderEndDate}
                          onClick={(date) => setOrderEndDate(date)}
                          size="small"
                          isClearable

                          // inputProps={{ style: { padding: "4px 5px" } }}
                        />
                      </Grid>
                    </Grid>
                    <Grid
                      item
                      minWidth={"16.66%"}
                      xl={2}
                      lg={2}
                      md={2}
                      sm={6}
                      xs={12}
                    >
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {LanguageReducer?.languageType?.SHIPMENTS_STORE}
                        </InputLabel>
                        <SelectComponent
                          multiple={true}
                          height={28}
                          name="reason"
                          options={storesForSelection}
                          value={storeId}
                          optionLabel={EnumOptions.STORE.LABEL}
                          optionValue={EnumOptions.STORE.VALUE}
                          getOptionLabel={(option) => option?.storeName}
                          onChange={(e, val) => {
                            setStoreId(val);
                          }}
                        />
                      </Grid>
                    </Grid>

                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.SHIPMENTS_ORDER_TYPE}
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={allOrderTypeLookup}
                        value={orderTypeId}
                        optionLabel={EnumOptions.ORDER_TYPE.LABEL}
                        optionValue={EnumOptions.ORDER_TYPE.VALUE}
                        getOptionLabel={(option) => option.orderTypeName}
                        onChange={(e, val) => {
                          setOrderTypeId(val);
                        }}
                      />
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.SHIPMENTS_CARRIER}
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={allActiveCarriersForSelection}
                        value={carrierId}
                        optionLabel={EnumOptions.CARRIER.LABEL}
                        optionValue={EnumOptions.CARRIER.VALUE}
                        getOptionLabel={(option) => option.Name}
                        onChange={(e, val) => {
                          setCarrierId(val);
                        }}
                      />
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.SHIPMENTS_FULFILLMENT_STATUS
                        }
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={allFullFillmentStatusLookup}
                        value={fullFillmentStatusId}
                        optionLabel={EnumOptions.FULL_FILLMENT_STATUS.LABEL}
                        optionValue={EnumOptions.FULL_FILLMENT_STATUS.VALUE}
                        getOptionLabel={(option) => option.text}
                        onChange={(e, val) => {
                          setFullFillmentStatusId(val);
                        }}
                      />
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.SHIPMENTS_PAYMENT_STATUS
                        }
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={allPaymentStatusLookup}
                        value={paymentStatusId}
                        optionLabel={EnumOptions.PAYMENT_STATUS.LABEL}
                        optionValue={EnumOptions.PAYMENT_STATUS.VALUE}
                        getOptionLabel={(option) => option.text}
                        onChange={(e, val) => {
                          setPaymentStatusId(val);
                        }}
                      />
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.SHIPMENTS_PAYMENT_METHOD
                        }
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={allPaymentMethodLookup}
                        value={paymentMethodId}
                        optionLabel={EnumOptions.PAYMENT_METHOD.LABEL}
                        optionValue={EnumOptions.PAYMENT_METHOD.VALUE}
                        getOptionLabel={(option) => option.pmName}
                        onChange={(e, val) => {
                          setPaymentMethodId(val);
                        }}
                      />
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.SHIPMENTS_STATION}
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={allStationLookup}
                        value={stationId}
                        optionLabel={EnumOptions.SELECT_STATION.LABEL}
                        optionValue={EnumOptions.SELECT_STATION.VALUE}
                        getOptionLabel={(option) => option.sname}
                        onChange={(e, val) => {
                          setStationId(val);
                        }}
                      />
                    </Grid>
                    <Grid
                      item
                      minWidth={"16.66%"}
                      xl={2}
                      lg={2}
                      md={2}
                      sm={6}
                      xs={12}
                    >
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.SHIPMENTS_SALES_PERSON}
                      </InputLabel>
                      <SelectComponent
                        multiple={true}
                        height={28}
                        name="reason"
                        options={allSalesPerson}
                        value={selectedSalesPerson}
                        optionLabel={EnumOptions.SALES_PERSON.LABEL}
                        optionValue={EnumOptions.SALES_PERSON.VALUE}
                        getOptionLabel={(option) => option.text}
                        onChange={(e, val) => {
                          setSelectedSalesPerson(val);
                        }}
                      />
                    </Grid>
                    <>
                      <Grid
                        item
                        minWidth={"16.66%"}
                        xl={2}
                        lg={2}
                        md={2}
                        sm={6}
                        xs={12}
                      >
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {LanguageReducer?.languageType?.SHIPMENTS_COUNTRY}
                        </InputLabel>
                        <CountrySchema
                          name="country"
                          required={true}
                          height={28}
                          value={selectedAddressSchemaWithObjValue.country}
                          onChange={(name, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            handleSetSchema("country", resolvedId);
                          }}
                        />
                      </Grid>

                      {[...addressSchemaSelectData].map((input, index) => (
                        <Grid
                          item
                          minWidth={"16.66%"}
                          xl={2}
                          lg={2}
                          md={2}
                          sm={6}
                          xs={12}
                        >
                          <SchemaTextField
                            loading={input.loading}
                            disabled={input.disabled}
                            multiple={true}
                            height={28}
                            type={input.type}
                            name={input.key}
                            required={false}
                            optionLabel={addressSchemaEnum[input.key]?.LABEL}
                            optionValue={addressSchemaEnum[input.key]?.VALUE}
                            options={input.options}
                            label={input.label}
                            hasIncExcFilter={input?.hasIncExcFilter}
                            incExcValue={
                              addressSchemaSelectDataIncExcValues[input.key]
                                ?.include
                            }
                            onChangeIncExcSwitch={handleChangeSelectAddressSchemaIncExcSwitch(
                              input.key,
                            )}
                            value={
                              selectedAddressSchemaWithObjValueForMultiple[
                                input.key
                              ]
                            }
                            onChange={(name, value) => {
                              handleChangeSelectAddressSchemaAndGetOptionsForMultiple(
                                input.key,
                                index,
                                value,
                                () => {},
                                input.key,
                              );
                            }}
                          />
                        </Grid>
                      ))}
                      <Grid
                        item
                        minWidth={"16.66%"}
                        xl={2}
                        lg={2}
                        md={2}
                        sm={6}
                        xs={12}
                      >
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {
                            LanguageReducer?.languageType
                              ?.SHIPMENTS_CARRIER_TRACKING_STATUS
                          }
                        </InputLabel>
                        <SelectComponent
                          multiple={true}
                          height={28}
                          optionLabel={
                            EnumOptions.CARRIER_TRACKING_STATUS.LABEL
                          }
                          optionValue={
                            EnumOptions.CARRIER_TRACKING_STATUS.VALUE
                          }
                          name="carrierStatus"
                          options={allCarrierTrackingStatus}
                          value={selectedCarrierTrackingStatus}
                          getOptionLabel={(option) => option.text}
                          onChange={(e, val) => {
                            setSelectedCarrierTrackingStatus(val);
                          }}
                        />
                      </Grid>
                    </>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Stack
                        alignItems="flex-end"
                        direction="row"
                        spacing={1}
                        sx={{
                          ...styleSheet.filterButtonMargin,
                          height: "100%",
                        }}
                      >
                        <Button
                          sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                          color="inherit"
                          variant="outlined"
                          onClick={() => {
                            handleFilterClear();
                          }}
                        >
                          {
                            LanguageReducer?.languageType
                              ?.SHIPMENTS_CLEAR_FILTER
                          }
                        </Button>
                        <Button
                          sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                          variant="contained"
                          onClick={() => {
                            handleFilter();
                          }}
                        >
                          {LanguageReducer?.languageType?.ORDERS_FILTER}
                        </Button>
                      </Stack>
                    </Grid>
                  </Grid>
                </TableRow>
              </TableHead>
            </Table>
          ) : null}
          <Grid container xs={12}>
            <Routes>
              {updatedResult.map((result, index) => {
                //console.log("result.route", result.route);
                const modifiedRoute =
                  result?.route?.replace("/shipments", "") || "";
                return (
                  <Route
                    key={index}
                    path={modifiedRoute}
                    element={
                      <ShipmentList
                        ref={orderListRef}
                        isFilterOpen={isFilterOpen}
                        allShipments={allShipments}
                        isAllDataListLoading={allShipmentsLoading}
                        getOrdersRef={getOrdersRef}
                        resetRowRef={resetRowRef}
                        shipmentTabsCountConfig={shipmentTabsCountConfig}
                        setStatusMoved={setStatusMoved}
                        setAllShipments={setAllShipments}
                        allCarrierTrackingStatus={allCarrierTrackingStatus}
                        selectionModel={selectionModel}
                        setSelectionModel={setSelectionModel}
                        setAllCarrierTrackingStatusDependcy={
                          setAllCarrierTrackingStatusDependcy
                        }
                        getShipments={() => getShipments(0)}
                      />
                    }
                  />
                );
              })}
            </Routes>
          </Grid>
        </Box>
      </div>
      {openAddTabModel && (
        <AddShipmentTabModal
          open={openAddTabModel}
          setOpen={setOpenAddTabModel}
          setStatusMoved={setStatusMoved}
        />
      )}
      {/* {openDelete && (
        <DeleteConfirmationModal
          open={openDelete}
          setOpen={setOpenDelete}
          handleDelete={handleDelete}
        />
      )} */}
      {unassignOpen && (
        <DeleteConfirmationModal
          open={unassignOpen}
          setOpen={unassignSetOpenDelete}
          handleDelete={handleUnAssignFromCarrier}
          loading={unassignLoading}
          heading={
            LanguageReducer?.languageType
              ?.ORDER_ARE_YOU_SURE_YOU_WANT_TO_UNASSIGN_SELECTED_ORDERS
          }
          message={
            LanguageReducer?.languageType
              ?.SHIPMENTS_SELECTED_ORDERS_WILL_BE_UNASSIGN_IMMEDIATELY_YOU_CANT_UNDO_THIS_ACTION
          }
          buttonText={LanguageReducer?.languageType?.SHIPMENTS_UNASSIGN}
        />
      )}
      {openBillingOrder && (
        <AddBillingModal
          open={openBillingOrder}
          orderNosData={getOrdersRef.current}
          onClose={() => setOpenBillingOrder(false)}
          documentSetting={documentSetting}
        />
      )}
    </Box>
  );
}
export default ShipmentPage;
