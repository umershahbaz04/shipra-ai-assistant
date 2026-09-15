import AddCircleOutlineIcon from "@mui/icons-material/AddCircleOutline";
import RemoveCircleOutlineIcon from "@mui/icons-material/RemoveCircleOutline";
import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
  Tooltip,
} from "@mui/material";
import React, { useCallback, useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes, useLocation, useNavigate } from "react-router-dom";
import ButtonComponent from "../../.reUseableComponents/Buttons/ButtonComponent.js";
import MenuIconComponent from "../../.reUseableComponents/Buttons/MenuIconComponent.js";
import useDateRangeHook from "../../.reUseableComponents/CustomHooks/useDateRangeHook.js";
import useDeleteConfirmation from "../../.reUseableComponents/CustomHooks/useDeleteConfirmation.js";
import DataGridTabs from "../../.reUseableComponents/DataGridTabs/DataGridTabs.js";
import DeleteConfirmationModal from "../../.reUseableComponents/Modal/DeleteConfirmationModal.js";
import CustomReactDatePickerInputFilter from "../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SearchInputAutoCompleteMultiple from "../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";
import SelectComponent from "../../.reUseableComponents/TextField/SelectComponent.js";
import {
  DeleteOrder,
  ExcelExportOrders,
  GenerateManifest,
  GetActiveCarriersForSelection,
  GetAllCarrierTrackingStatusForSelection,
  GetAllClientOrderLabelLookupForSelection,
  GetAllFullFillmentStatusLookup,
  GetAllOrderTypeLookup,
  GetAllOrders,
  GetAllPaymentMethodLookup,
  GetAllPaymentStatusLookup,
  GetAllSalePersonForSelection,
  GetAllStationLookup,
  GetCarrierWayBillsByOrderNos,
  GetOrderInvoiceByOrderNos,
  GetStoresForSelection,
  GetValidateDocumentSetting,
  OrdersArchive,
  UnAssignFromCarrier,
  UpdateOrderWithSaleChannel,
} from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import AddBillingModal from "../../components/modals/orderModals/AddBillingModal.js";
import BatchAssigntoCarrierModal from "../../components/modals/orderModals/BatchAssigntoCarrierModal";
import BatchOrderFulfillmentModal from "../../components/modals/orderModals/BatchOrderFulfillmentModal";
import AssignOrderStationModal from "../../components/modals/orderModals/AssignOrderStationModal";
import BatchUpdateOrderStatusModal from "../../components/modals/orderModals/BatchUpdateOrderStatusModal";
import UtilityClass from "../../utilities/UtilityClass";
import {
  EnumAwbType,
  EnumCarrierAssign,
  EnumChangeFilterModelApiUrls,
  EnumFullFillmentStatus,
  EnumNavigateState,
  EnumOptions,
  EnumOrderType,
  EnumPaymentStatus,
  viewTypesEnum,
} from "../../utilities/enum";
import initialStateFilter from "../../utilities/filterState";
import {
  ActionButtonCustom,
  CopyButton,
  ToggleButtonComponent,
  downloadWayBillsByOrderNos,
  handleDownloadMenifestPdf,
  handleDownloadPdf,
  handleFilterAddressSchemaSelectDataIncExcValues,
  useGetAllCountries,
  useGetBreakPoint,
  useGetNavigateState,
  useSetNavigateStateData,
} from "../../utilities/helpers/Helpers";
import {
  useGetAllCitiesByRegionIds,
  useGetAllRegionbyCountryIds,
} from "../../utilities/helpers/HelpersFilter.js";
import {
  FulfillableOrderIcon,
  RegularOrderIcon,
  UploadOrderIcon,
} from "../../utilities/helpers/SvgIcons.js";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../utilities/helpers/addressSchema.js";
import {
  errorNotification,
  successNotification,
  warningNotification,
} from "../../utilities/toast";
import OrderList from "./ordersList";
import AddOrderLabelModal from "../../components/modals/orderModals/AddOrderLabelModal.js";
import AddSaleChannelForOrderModal from "../../components/modals/orderModals/AddSaleChannelForOrderModal.js";
import CountrySchema from "../../utilities/helpers/countryschema.js";
import { isShowCountryInTabbarFlag } from "../../utilities/cookies";

const carrierAssignModel = [
  {
    id: 0,
    text: "Please select option",
  },
  {
    id: EnumCarrierAssign.UnAssigne,
    text: "UnAssign",
  },
  {
    id: EnumCarrierAssign.Assign,
    text: "Assign",
  },
];

const MAX_TAGS = 200;
const EnumOrderTabFilter = Object.freeze({
  All: "/orders-dashboard",
  Unfulfilled: "/orders-dashboard/Unfullfilled",
  Fullfilled: "/orders-dashboard/fullfilled",
  Regular: "/orders-dashboard/regular",
  FullFillable: "/orders-dashboard/fullFillable",
  Unpaid: "/orders-dashboard/Unpaid",
  paid: "/orders-dashboard/paid",
  UnAssigned: "/orders-dashboard/unassigned",
  ToBeFulfilled: "/orders-dashboard/to-be-fulfilled",
});
const EnumOrderCreate = Object.freeze({
  FullFilable: "/create-fulfillable-order",
  Regular: "/create-regular-order",
  UploadOrders: "/upload-orders",
});
function OrderPage(props) {
  const orderListRef = useRef(null);
  const belowMdScreen = useGetBreakPoint("md");
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
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
    openDelete,
    setOpenDelete,
    handleDeleteClose,
    handleSetDeletedItem,
    deletedItem,
    loadingConfirmation: unassignLoading,
    setLoadingConfirmation: unassignSetLoading,
  } = useDeleteConfirmation();
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

  const [showOrdersBtn, setShowOrdersBtn] = useState(true);
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
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [openAdvanceSearchModal, setOpenAdvanceSearchModal] = useState(false);
  const [regionIncludeChecked, setRegionIncludeChecked] = useState(true);
  const getIncludeExcludeRegionIds = () => {
    let newVal = selectedRegions;
    if (!regionIncludeChecked) {
      newVal = allRegions.filter(
        (region) =>
          !selectedRegions.some((selected) => region.id === selected.id)
      );
    }
    return newVal.map((country) => country.id).join(",");
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

  const [cityIncludeChecked, setCityIncludeChecked] = useState(true);
  const getIncludeExcludeCityIds = () => {
    let newVal = selectedCities;
    if (!cityIncludeChecked) {
      newVal = allCities.filter(
        (region) =>
          !selectedCities.some((selected) => region.id === selected.id)
      );
    }
    return newVal.map((cty) => cty.id).join(",");
  };
  //#endregion
  const {
    openDelete: unassignOpen,
    setOpenDelete: unassignSetOpenDelete,
    handleDeleteClose: unassignHandleDeleteClose,
    handleSetDeletedItem: unassignHandleSetDeletedItem,
    deletedItem: unassignDeletedItem,
  } = useDeleteConfirmation();
  const handleConfirmationData = (selectedTrNos) => {
    let orderids = getOrderIdsByOrderNos(selectedTrNos);
    handleSetDeletedItem(orderids);
    setOpenDelete(true);
  };
  function getOrderIdsByOrderNos(orderNos) {
    const orderIds = allOrders
      ?.filter((order) => orderNos?.includes(order.OrderNo))
      .map((order) => order.OrderId);

    return orderIds.join(",");
  }

  const [allOrders, setAllOrders] = useState([]);
  const [allOrderLoading, setAllOrderLoading] = useState(true);

  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inputFields, setInputFields] = useState([]);
  const [isShowFilter, setIsShowFilter] = useState(true);
  const [openOrderLabelModal, setOpenOrderLabelModal] = useState(false);
  const [orderLabel, setOrderLabel] = useState([]);
  const [selectedOrderLabels, setselectedOrderLabels] = useState();
  const [openAddOrderSaleChannelModal, setOpenAddOrderSaleChannelModal] =
    useState({
      open: false,
      openUnassignSaleChannel: false,
      storeId: null,
      openUnassignSaleChannelLoading: false,
    });

  const navigate = useNavigate();

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

  const handleCreateOrderButtons = (val) => {
    if (val == EnumOrderCreate.Regular) {
      navigate(EnumOrderCreate.Regular);
    }
    if (val == EnumOrderCreate.FullFilable) {
      navigate(EnumOrderCreate.FullFilable);
    }
    if (val == EnumOrderCreate.UploadOrders) {
      navigate(EnumOrderCreate.UploadOrders);
    }
  };
  const [viewMode, setViewMode] = useState(viewTypesEnum.TABLE);
  const [storeId, setStoreId] = useState(initialStateFilter.multiple);
  const [orderTypeId, setOrderTypeId] = useState(initialStateFilter.orderType);
  const [carrierId, setCarrierId] = useState(initialStateFilter.carrier);
  const [fullFillmentStatusId, setFullFillmentStatusId] = useState(
    initialStateFilter.common
  );
  const [paymentStatusId, setPaymentStatusId] = useState(
    initialStateFilter.common
  );
  const [paymentMethodId, setPaymentMethodId] = useState(
    initialStateFilter.paymentMethod
  );
  const [allSalesPerson, setAllSalesPerson] = useState([]);
  const [selectedSalesPerson, setSelectedSalesPerson] = useState([]);

  const [carrierAssigned, setCarrierAssigned] = useState(carrierAssignModel[0]);
  const [stationId, setStationId] = useState(initialStateFilter.station);

  const handleFocus = (event) => event.target.select();
  const location = useLocation();
  //////filters
  ///
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allOrderTypeLookup, setAllOrderTypeLookup] = useState([]);
  const [allActiveCarriersForSelection, setAllActiveCarriersForSelection] =
    useState([]);
  const [allFullFillmentStatusLookup, setAllFullFillmentStatusLookup] =
    useState([]);
  const [allCarrierTrackingStatus, setAllCarrierTrackingStatus] = useState([]);
  const [selectedCarrierTrackingStatus, setSelectedCarrierTrackingStatus] =
    useState([]);
  const [allPaymentStatusLookup, setAllPaymentStatusLookup] = useState([]);
  const [allPaymentMethodLookup, setAllPaymentMethodLookup] = useState([]);
  const [allStationLookup, setAllStationLookup] = useState([]);
  const [selectionModel, setSelectionModel] = useState([]);
  const [isTabFilter, setIsTabFilter] = useState(false);
  const [documentSetting, setDocumentSetting] = useState({});
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  let getValidateDocumentSetting = async () => {
    let res = await GetValidateDocumentSetting();
    if (res.data.result !== null) {
      setDocumentSetting(res.data.result);
    }
  };
  //#region
  let getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    if (res.data.result != null) {
      setStoresForSelection(res.data.result);
    }
  };
  const getAllOrderTypeLookup = async () => {
    try {
      const response = await GetAllOrderTypeLookup();
      setAllOrderTypeLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllSalePersonForSelection = async () => {
    try {
      const response = await GetAllSalePersonForSelection();
      setAllSalesPerson(response.data.result || []);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllActiveCarriersForSelection = async () => {
    try {
      const response = await GetActiveCarriersForSelection();
      setAllActiveCarriersForSelection(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllFullFillmentStatusLookup = async () => {
    try {
      const response = await GetAllFullFillmentStatusLookup();
      setAllFullFillmentStatusLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllCarrierTrackingStatusForSelection = async () => {
    try {
      const response = await GetAllCarrierTrackingStatusForSelection();
      setAllCarrierTrackingStatus(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllPaymentStatusLookup = async () => {
    try {
      const response = await GetAllPaymentStatusLookup();
      setAllPaymentStatusLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllPaymentMethodLookup = async () => {
    try {
      const response = await GetAllPaymentMethodLookup();
      setAllPaymentMethodLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllStationLookup = async () => {
    try {
      const response = await GetAllStationLookup();
      setAllStationLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };

  const getAllClientOrderLabelLookupForSelection = async () => {
    try {
      const response = await GetAllClientOrderLabelLookupForSelection();
      if (response) {
        setOrderLabel(response?.data?.result);
      }
    } catch (e) {
      console.error(e);
    }
  };
  //#endregion
  useEffect(() => {
    getStoresForSelection();
    getAllOrderTypeLookup();
    getAllSalePersonForSelection();
    getAllActiveCarriersForSelection();
    getAllFullFillmentStatusLookup();
    getAllCarrierTrackingStatusForSelection();
    getAllPaymentStatusLookup();
    getAllPaymentMethodLookup();
    getAllStationLookup();
    getValidateDocumentSetting();
    getAllClientOrderLabelLookupForSelection();
  }, []);

  const getFiltersFromStateForExport = () => {
    let search = inputFields.join();
    if (getOrdersRef.current && getOrdersRef.current?.length > 0) {
      search = getOrdersRef.current?.join();
    } else {
      search = inputFields.join();
    }
    let filters = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
      storeId: storeId?.map((data) => data.storeId).toString(),
      OrderLabels: selectedOrderLabels
        ?.map((data) => data.labelName)
        .toString(),
      SalePersonIds: selectedSalesPerson?.map((data) => data.id).toString(),
      orderTypeId: orderTypeId?.orderTypeId,
      carrierId: carrierId?.carrierId,
      fullFillmentStatusId: fullFillmentStatusId?.id,
      paymentStatusId: paymentStatusId?.id,
      orderRequestVia: 0,
      paymentMethodId: paymentMethodId?.paymentMethodId,
      stationId: stationId?.productStationId,
      carrierAssign: carrierAssigned?.id,
      isWithoutStation: location.pathname.includes("to-be-fulfilled"),
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
        addressSchemaSelectDataIncExcValues
      ),
    };
    return filters;
  };
  const [allOrdersCount, setAllOrdersCount] = useState(0);
  const getAllOrders = async ({
    _storeId = storeId?.length > 0
      ? storeId?.map((data) => data.storeId).toString()
      : "",
    _salePersonIds = selectedSalesPerson.length > 0
      ? selectedSalesPerson?.map((data) => data.id).toString()
      : "",
    _orderLabels = selectedOrderLabels
      ?.map((data) => data.labelName)
      .toString(),
    _orderTypeId = orderTypeId?.orderTypeId,
    _carrierId = carrierId?.carrierId ? String(carrierId?.carrierId) : "",
    _fullFillmentStatusId = fullFillmentStatusId?.id,
    _paymentStatusId = paymentStatusId?.id,
    _paymentMethodId = paymentMethodId?.paymentMethodId,
    _stationId = String(stationId?.productStationId),
    _carrierAssigned = carrierAssigned?.id,
    searchParams = "",
    _countryIds = isShowCountryInTabbarFlag()
      ? (reduxSelectedCountry
          ? String(reduxSelectedCountry.countryId || reduxSelectedCountry.id || reduxSelectedCountry)
          : "")
      : (selectedAddressSchema.country
          ? (typeof selectedAddressSchema.country === "object"
              ? String(selectedAddressSchema.country?.countryId || selectedAddressSchema.country?.id || selectedAddressSchema.country?.[EnumOptions.COUNTRY.VALUE] || "")
              : String(selectedAddressSchema.country))
          : ""),
    _cityIds = String(selectedAddressSchemaForMutiple.city),
    _provinceIds = String(selectedAddressSchemaForMutiple.province),
    _stateIds = String(selectedAddressSchemaForMutiple.state),
    _pinCodeIds = String(selectedAddressSchemaForMutiple.pinCode),
    _areaIds = String(selectedAddressSchemaForMutiple.area),
    _carrierTrackingStatusId = selectedCarrierTrackingStatus
      .map((data) => data.carrierTrackingStatusId)
      .toString(),
    _IncludeRegion = regionIncludeChecked,
    _IncludeCity = cityIncludeChecked,
    isWithoutStation = location.pathname.includes("to-be-fulfilled"),
  }) => {
    let data = [];
    setAllOrderLoading(true);
    const res = await GetAllOrders(
      startDateFormated,
      endDateFormated,
      _storeId,
      _salePersonIds,
      _orderTypeId,
      _carrierId,
      _fullFillmentStatusId,
      _paymentStatusId,
      _paymentMethodId,
      _stationId,
      _carrierAssigned,
      searchParams,
      _countryIds,
      _cityIds,
      _provinceIds,
      _stateIds,
      _pinCodeIds,
      _areaIds,
      _carrierTrackingStatusId,
      _IncludeRegion,
      _IncludeCity,
      orderStartDateFormated,
      orderEndDateFormate,
      handleFilterAddressSchemaSelectDataIncExcValues(
        addressSchemaSelectDataIncExcValues
      ),
      _orderLabels,
      isWithoutStation
    );
    if (res.data.result !== null) {
      data = res.data.result.list;
      setAllOrders(res.data.result.list);
      setAllOrdersCount(res.data?.result?.TotalCount);
    }
    setAllOrderLoading(false);
    resetRowRef.current = false;

    return data;
  };

  const resetFilter = () => {
    setStartDate(defaultValues.startDate);
    setEndDate(defaultValues.endDate);
    setOrderStartDate(defaultValues.orderStartDate);
    setOrderEndDate(defaultValues.orderEndDate);
    setStoreId(initialStateFilter.multiple);
    setSelectedSalesPerson(initialStateFilter.multiple);
    setOrderTypeId(initialStateFilter.orderType);
    setCarrierId(initialStateFilter.carrier);
    setFullFillmentStatusId(initialStateFilter.fulfillment);
    setPaymentStatusId(initialStateFilter.paymentStatus);
    setPaymentMethodId(initialStateFilter.paymentMethod);
    setStationId(initialStateFilter.station);
    setCarrierAssigned(carrierAssignModel[0]);
    setSelectedCarrierTrackingStatus(initialStateFilter.multiple);
    handleReset();
  };
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [selectedItems, setSelectedItems] = useState([]);
  const handleFilterClear = async () => {
    resetFilter();
    setIsfilterClear(true);
  };
  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry
  );
  const isInitialCountryMountOrders = useRef(true);
  useEffect(() => {
    if (!isShowCountryInTabbarFlag()) return;
    if (isInitialCountryMountOrders.current) {
      isInitialCountryMountOrders.current = false;
      return;
    }
    const resolvedCountryId =
      reduxSelectedCountry?.countryId ||
      reduxSelectedCountry?.id ||
      (typeof reduxSelectedCountry === "number" || typeof reduxSelectedCountry === "string"
        ? reduxSelectedCountry
        : "");
    handleSetSchema("country", reduxSelectedCountry || null);
    getAllOrders({
      _countryIds: resolvedCountryId ? String(resolvedCountryId) : "",
    });
  }, [reduxSelectedCountry]);

  useEffect(() => {
    if (isfilterClear) {
      getAllOrders({});
      setIsfilterClear(false);
    }
  }, [isfilterClear]);

  const handleTabChange = async (event, filterValue) => {
    resetFilter();
    setIsFilterOpen(false);
    if (filterValue === EnumOrderTabFilter.All) {
      getAllOrders({});
    } else if (filterValue === EnumOrderTabFilter.Unfulfilled) {
      setFullFillmentStatusId({ id: EnumFullFillmentStatus.UnFulfillment });
      getAllOrders({
        _fullFillmentStatusId: EnumFullFillmentStatus.UnFulfillment,
      });
    } else if (filterValue === EnumOrderTabFilter.Fullfilled) {
      setFullFillmentStatusId({ id: EnumFullFillmentStatus.Fulfillment });
      getAllOrders({
        _fullFillmentStatusId: EnumFullFillmentStatus.Fulfillment,
        _orderTypeId: 0,
      });
    } else if (filterValue === EnumOrderTabFilter.Regular) {
      setOrderTypeId({ orderTypeId: EnumOrderType.Regular });
      getAllOrders({
        _orderTypeId: EnumOrderType.Regular,
      });
    } else if (filterValue === EnumOrderTabFilter.FullFillable) {
      setOrderTypeId({ orderTypeId: EnumOrderType.FullFillable });
      getAllOrders({
        _orderTypeId: EnumOrderType.FullFillable,
      });
    } else if (filterValue === EnumOrderTabFilter.Unpaid) {
      setPaymentStatusId({ id: EnumPaymentStatus.Unpaid });
      getAllOrders({
        _paymentStatusId: EnumPaymentStatus.Unpaid,
      });
    } else if (filterValue === EnumOrderTabFilter.paid) {
      setPaymentStatusId({ id: EnumPaymentStatus.Paid });
      getAllOrders({
        _paymentStatusId: EnumPaymentStatus.Paid,
      });
    } else if (filterValue === EnumOrderTabFilter.ToBeFulfilled) {
      getAllOrders({
        isWithoutStation: true,
      });
    } else if (filterValue === EnumOrderTabFilter.UnAssigned) {
      //do work relaed unassigned
      setCarrierAssigned({ id: EnumCarrierAssign.UnAssigne });
      getAllOrders({
        _carrierAssigned: EnumCarrierAssign.UnAssigne,
      });
    }
    if (filterValue !== EnumOrderTabFilter.All) {
      setIsShowFilter(false);
    }
    resetFilter();
    resetRowRef.current = true;
    setIsTabFilter(true);
  };
  useEffect(() => {
    if (location.pathname === EnumOrderTabFilter.All) {
      resetFilter();
      setIsShowFilter(true);
    }
  }, [location.pathname]);

  /////actions
  const EnumOrderDashboardAction = {
    ASSIGNTOCARRIER: "AssigntoCarrier",
    ASSIGNINHOUSE: "AssignInHouse",
    FULLFILLMENT: "Fulfillment",
    UPDATESTATUS: "UpdateStatus",
    EXCELEXPORT: "ExcelExport",
    PRINTLABEL: "PrintLabel",
  };
  const [open, setOpen] = useState(false);
  const [openBillingOrder, setOpenBillingOrder] = useState(false);
  const [openFulfillment, setOpenFulfillment] = useState(false);
  const [openUpdateStatus, setOpenUpdateStatus] = useState(false);
  const [isAssignInHouse, setIsAssignInHouse] = useState(false);

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

  const downloadExcel = () => {
    let params = getFiltersFromStateForExport();
    ExcelExportOrders(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadExcel(res.data, "orders");
          handleResetSeelctedRows();
        } else {
          successNotification("Order not found");
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  const handleOnClickAction = (method) => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length > 0) {
      method();
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    }
  };
  const handleOnClickActionBtn = (method) => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length === 0) {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    } else {
      method();
    }
  };

  useEffect(() => {
    if (getOrdersRef.current && getOrdersRef.current.complete) {
      // console.log("first");
    }
  });
  const { navigateStateData } = useGetNavigateState(
    EnumNavigateState.DELIVERY_TASKS.pageName
  );
  const [openAssignStationModal, setOpenAssignStationModal] = useState(false);
  const [selectedOrderNosForStation, setSelectedOrderNosForStation] = useState([]);
  const [selectedOrderIdsForStation, setSelectedOrderIdsForStation] = useState([]);
  const [showActionToolbarButtons, setShowActionToolbarButtons] =
    useState(false);

  const getOrderListData = () => {
    return (
      <OrderList
        ref={orderListRef}
        isFilterOpen={isFilterOpen}
        loading={allOrderLoading}
        orderLabel={orderLabel}
        setOrderLabel={setOrderLabel}
        setAllOrderLoading={setAllOrderLoading}
        viewMode={viewMode}
        allOrders={allOrders}
        allOrdersCount={allOrdersCount}
        setAllOrders={setAllOrders}
        getOrdersRef={getOrdersRef}
        resetRowRef={resetRowRef}
        setSelectedItems={setSelectedItems}
        setShowActionToolbarButtons={setShowActionToolbarButtons}
        allCarrierTrackingStatus={allCarrierTrackingStatus}
        getAllCarrierTrackingStatusForSelection={
          getAllCarrierTrackingStatusForSelection
        }
        setSelectionModel={setSelectionModel}
        openAdvanceSearchModal={openAdvanceSearchModal}
        setOpenAdvanceSearchModal={setOpenAdvanceSearchModal}
        setInputFields={setInputFields}
        getAllOrders={(paginationModel) => {
          getAllOrders({
            _storeId:
              storeId.length > 0
                ? storeId?.map((data) => data.storeId).toString()
                : "",
            _salePersonIds:
              selectedSalesPerson.length > 0
                ? selectedSalesPerson?.map((data) => data.id).toString()
                : "",
            _orderTypeId: orderTypeId?.orderTypeId,
            _carrierId: carrierId?.CarrierId,
            _fullFillmentStatusId: fullFillmentStatusId?.id,
            _paymentStatusId: paymentStatusId?.id,
            _paymentMethodId: paymentMethodId?.paymentMethodId,
            _stationId: stationId?.productStationId,
            _carrierAssigned: carrierAssigned?.id,
            searchParams: "",
            _countryIds: isShowCountryInTabbarFlag()
              ? (reduxSelectedCountry
                  ? String(reduxSelectedCountry.countryId || reduxSelectedCountry.id || reduxSelectedCountry)
                  : "")
              : (selectedCountryIds?.length > 0
                  ? selectedCountryIds?.map((data) => data.countryId).toString()
                  : (selectedAddressSchema?.country
                      ? (typeof selectedAddressSchema.country === "object"
                          ? String(selectedAddressSchema.country?.countryId || selectedAddressSchema.country?.id || selectedAddressSchema.country?.[EnumOptions.COUNTRY.VALUE] || "")
                          : String(selectedAddressSchema.country))
                      : "")),
            _regionIds: String(selectedAddressSchemaForMutiple.region),
            _cityIds: String(selectedAddressSchemaForMutiple.city),
            _stateIds: String(selectedAddressSchemaForMutiple.state),
            _provinceIds: String(selectedAddressSchemaForMutiple.province),
            _pinCodeIds: String(selectedAddressSchemaForMutiple.pinCode),
            _areaIds: String(selectedAddressSchemaForMutiple.area),
            _carrierTrackingStatusId: selectedCarrierTrackingStatus
              .map((data) => data.carrierTrackingStatusId)
              .toString(),
            _IncludeRegion: regionIncludeChecked,
            _IncludeCity: cityIncludeChecked,
            isWithoutStation: location.pathname.includes("to-be-fulfilled"),
          });
        }}
      />
    );
  };

  const handleDelete = async () => {
    try {
      let param = {
        orderids: deletedItem,
      };
      setDeleteLoading(true);
      const response = await DeleteOrder(param);
      if (!response.data?.isSuccess) {
        UtilityClass.showErrorNotificationWithDictionary(response.data?.errors);
      } else {
        getAllOrders({});
        handleResetSeelctedRows();
        handleDeleteClose();
        successNotification("Order Deleted successfully");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
      setDeleteLoading(false);
    }
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
        getAllOrders({});
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
  const handleResetSeelctedRows = () => {
    resetRowRef.current = true;
  };
  const handleUnAssignConfirmation = (orderNos) => {
    unassignHandleSetDeletedItem(orderNos);
    unassignSetOpenDelete(true);
  };

  useSetNavigateStateData(navigateStateData, () => {
    const selectedOrder =
      navigateStateData[EnumNavigateState.DELIVERY_TASKS.state.selectedOrderNo];
    setInputFields([selectedOrder]);
  });

  useEffect(() => {
    const fetchResponse = async () => {
      const response = await getAllOrders({
        searchParams: String(inputFields),
      });
      setAllOrders(response);
    };
    fetchResponse();
  }, [inputFields]);

  const handleWayBillsByOrderNos = () => {
    if (documentSetting.DocumentTemplateId !== null) {
      const selectedTrNos = getOrdersRef.current;
      handleOnClickActionBtn(() =>
        downloadWayBillsByOrderNos(
          selectedTrNos.join(","),
          documentSetting.DocumentTemplateId
        )
      );
    } else {
      handleOnClickActionBtn(() => setOpenBillingOrder(true));
    }
  };

  const unassignSaleChannel = async () => {
    const selectedTrNos = getOrdersRef.current;
    const orderNos = selectedTrNos.join(",");
    setOpenAddOrderSaleChannelModal((prev) => ({
      ...prev,
      openUnassignSaleChannelLoading: true,
    }));
    try {
      const response = await UpdateOrderWithSaleChannel(orderNos, 0, true);
      if (response?.data?.isSuccess) {
        successNotification(
          "Sales channel unassign successfully for the selected orders."
        );
        getAllOrders({});
        setOpenAddOrderSaleChannelModal((prev) => ({
          ...prev,
          openUnassignSaleChannel: false,
        }));
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setOpenAddOrderSaleChannelModal((prev) => ({
        ...prev,
        openUnassignSaleChannelLoading: false,
      }));
    }
  };

  const handleUnassignSaleChannel = () => {
    const selectedTrNos = getOrdersRef.current;

    if (selectedTrNos.length === 0) {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    } else {
      setOpenAddOrderSaleChannelModal((prev) => ({
        ...prev,
        openUnassignSaleChannel: true,
      }));
    }
  };

  const handleOpenAddSaleChannel = () => {
    const selectedTrNos = getOrdersRef.current;
    const filterRow = allOrders.filter((order) =>
      selectedTrNos.includes(order.OrderNo)
    );
    if (filterRow.length === 0) {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
      return;
    }
    const firstStoreId = filterRow[0]?.StoreId;
    const allSameStoreId = filterRow.every(
      (order) => order.StoreId === firstStoreId
    );
    if (allSameStoreId) {
      setOpenAddOrderSaleChannelModal((prev) => ({
        ...prev,
        open: true,
        storeId: firstStoreId,
      }));
    } else {
      errorNotification("Selected orders must belong to the same store.");
    }
  };

  const downloadOrderInvoice = () => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length === 0) {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
      return;
    }
    const orderNo = selectedTrNos.join(",");
    GetOrderInvoiceByOrderNos(orderNo)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadPdf(res.data, "Awb");
        } else {
          successNotification("Invoice Print successfully");
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to Print Invoice");
      });
  };

  const handleGenerateMenifestPDF = async () => {
    const selectedTrNos = getOrdersRef.current;
    console.log(selectedTrNos);
    const response = await GenerateManifest(selectedTrNos.join(","));
    handleDownloadMenifestPdf(response, "manifest", handleResetSeelctedRows);
  };

  const handleOrderArchive = async () => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length == 0) {
      errorNotification("Please Select Order You Want To Archive");
    }
    console.log(selectedTrNos);
    const body = {
      OrderNo: selectedTrNos.join(","),
    };
    try {
      const response = await OrdersArchive(body);
      if (response?.data?.isSuccess) {
        successNotification("Order Archive Create Successfully");
        getAllOrders({});
      }
    } catch (error) {}
  };

  const menuItems = [
    {
      label: LanguageReducer?.languageType?.ORDER_ASSIGN_TO_CARRIER,
      onClick: () => {
        setOpen(true);
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDER_ASSIGN_IN_HOUSE,
      onClick: () => {
        setIsAssignInHouse(true);
        setOpen(true);
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDER_UPDATE_STATUS,
      onClick: () => {
        setOpenUpdateStatus(true);
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDER_EXCEL_EXPORT,
      onClick: () => handleOnClickAction(() => downloadExcel()),
    },
    {
      label: LanguageReducer?.languageType?.ORDER_ADD_ORDER_LABELS,
      onClick: () => {
        setOpenOrderLabelModal(true);
      },
    },
    {
      label: "Generate Manifest",
      onClick: handleGenerateMenifestPDF,
    },
    {
      label: "Archive Orders",
      onClick: handleOrderArchive,
    },
    {
      label: "Assign Station",
      onClick: () => handleOnClickAction(() => {
        const selectedNos = getOrdersRef.current;
        const selectedIds = allOrders
          ?.filter((order) => selectedNos?.includes(order.OrderNo))
          .map((order) => order.OrderId);
        setSelectedOrderNosForStation(selectedNos);
        setSelectedOrderIdsForStation(selectedIds);
        setOpenAssignStationModal(true);
      }),
    },
    {
      label: LanguageReducer?.languageType?.ORDER_ADD_SALE_CHANNEL,
      onClick: handleOpenAddSaleChannel,
    },
    {
      label: LanguageReducer?.languageType?.ORDER_UNASSIGN_SALE_CHANNEL,
      onClick: handleUnassignSaleChannel,
    },
    {
      label: LanguageReducer?.languageType?.ORDER_BATCH_FULLFILLMENT,
      onClick: () => handleOnClickAction(() => setOpenFulfillment(true)),
    },
    {
      label: LanguageReducer?.languageType?.ORDER_PRINT_LABELS,
      onClick: handleWayBillsByOrderNos,
    },
    {
      label: "Print Invoice",
      onClick: () => downloadOrderInvoice(),
    },
    {
      label: LanguageReducer?.languageType?.ORDER_PRINT_CARRIER_LABELS,
      onClick: () => {
        const selectedTrNos = getOrdersRef.current;
        handleOnClickActionBtn(() =>
          downloadCarrierWayBillsByOrderNos(
            selectedTrNos.join(","),
            EnumAwbType.Awb4x6TypeId
          )
        );
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDER_UNASSIGN_FROM_CARRIER,
      onClick: () => {
        const selectedTrNos = getOrdersRef.current;
        handleOnClickAction(() =>
          handleUnAssignConfirmation(selectedTrNos.join(","))
        );
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDER_REFRESH_TRACKING_STATUS,
      onClick: () => {
        orderListRef.current.Refresh();
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDER_DELETE_ORDER,
      onClick: () => {
        const selectedTrNos = getOrdersRef.current;
        handleOnClickAction(() =>
          handleConfirmationData(selectedTrNos.join(","))
        );
      },
    },
  ];
  const menuItems2 = [
    {
      label: LanguageReducer?.languageType?.ORDERS_CREATE_REGULAR_ORDER,
      onClick: () => {
        handleCreateOrderButtons(EnumOrderCreate.Regular);
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDERS_CREATE_FULFILLABLE_ORDER,
      onClick: () => {
        handleCreateOrderButtons(EnumOrderCreate.FullFilable);
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDERS_UPLOAD_ORDER,
      onClick: () => {
        handleCreateOrderButtons(EnumOrderCreate.UploadOrders);
      },
    },
  ];

  const actionBtnMenuData = !belowMdScreen
    ? [
        {
          title: LanguageReducer?.languageType?.ORDER_BATCH_FULLFILLMENT,
          onClick: () => handleOnClickAction(() => setOpenFulfillment(true)),
        },
        {
          title: LanguageReducer?.languageType?.ORDER_EXCEL_EXPORT,
          onClick: () => handleOnClickAction(() => downloadExcel()),
        },
        {
          title: LanguageReducer?.languageType?.ORDER_PRINT_LABELS,
          onClick: handleWayBillsByOrderNos,
        },
        {
          title: "Print Invoice",
          onClick: () => downloadOrderInvoice(),
        },
        {
          title: "Generate Manifest",
          onClick: handleGenerateMenifestPDF,
        },
        {
          title: "Archive Orders",
          onClick: handleOrderArchive,
        },
        {
          title: LanguageReducer?.languageType?.ORDER_ADD_ORDER_LABELS,
          onClick: () =>
            handleOnClickAction(() => setOpenOrderLabelModal(true)),
        },
        {
          title: LanguageReducer?.languageType?.ORDER_ADD_SALE_CHANNEL,
          onClick: handleOpenAddSaleChannel,
        },
        {
          title: LanguageReducer?.languageType?.ORDER_UNASSIGN_SALE_CHANNEL,
          onClick: handleUnassignSaleChannel,
        },
        {
          title: LanguageReducer?.languageType?.ORDER_PRINT_CARRIER_LABELS,
          onClick: () => {
            const selectedTrNos = getOrdersRef.current;
            handleOnClickActionBtn(() =>
              downloadCarrierWayBillsByOrderNos(
                selectedTrNos.join(","),
                EnumAwbType.Awb4x6TypeId
              )
            );
          },
        },
        {
          title: LanguageReducer?.languageType?.ORDER_UNASSIGN_FROM_CARRIER,
          onClick: () => {
            const selectedTrNos = getOrdersRef.current;
            handleOnClickAction(() =>
              handleUnAssignConfirmation(selectedTrNos.join(","))
            );
          },
        },
        {
          title: LanguageReducer?.languageType?.ORDER_REFRESH_TRACKING_STATUS,
          onClick: () => {
            orderListRef.current.Refresh();
          },
        },
        {
          title: LanguageReducer?.languageType?.ORDER_DELETE_ORDER,
          onClick: () => {
            const selectedTrNos = getOrdersRef.current;
            handleOnClickAction(() =>
              handleConfirmationData(selectedTrNos.join(","))
            );
          },
        },
      ]
    : null;

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

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Box
          sx={{
            background: "#F8F8F8",
            border: "1px solid rgba(224, 224, 224, 1)",
            borderRadius: "8px 8px 0px 0px!important",
          }}
        >
          <Box
            sx={{ display: "flex", alignItems: "center", gap: 1, pr: "7px" }}
          >
            <Box flexGrow={1}>
              <SearchInputAutoCompleteMultiple
                onChange={(e, value) => {
                  if (value.length <= MAX_TAGS) {
                    const limitedValues = value.slice(0, MAX_TAGS);
                    setInputFields(limitedValues);
                  } else {
                    warningNotification(
                      LanguageReducer?.languageType?.MAXMIUM_NUMBER_REACHED
                    );
                  }
                }}
                inputFields={inputFields}
                MAX_TAGS={MAX_TAGS}
              />
            </Box>
            <ButtonComponent
              btnMdWidth={"135px"}
              btnLgWidth={"150px"}
              bg={"var(--primary-color)"}
              title={"Advance Search"}
              onClick={() => setOpenAdvanceSearchModal(true)}
            />
            {!belowMdScreen ? (
              showOrdersBtn ? (
                <ActionButtonCustom
                  onClick={() => {
                    setShowOrdersBtn(false);
                  }}
                  label={<RemoveCircleOutlineIcon />}
                  width={10}
                />
              ) : (
                <ActionButtonCustom
                  onClick={() => {
                    setShowOrdersBtn(true);
                  }}
                  label={<AddCircleOutlineIcon />}
                  width={10}
                />
              )
            ) : (
              <>
                <MenuIconComponent menuItems={menuItems2} />
              </>
            )}

            {showOrdersBtn && !belowMdScreen && (
              <>
                <ActionButtonCustom
                  onClick={(event) => {
                    handleCreateOrderButtons(EnumOrderCreate.Regular);
                  }}
                  label={
                    LanguageReducer?.languageType?.ORDERS_CREATE_REGULAR_ORDER
                  }
                  width="120px"
                  startIcon={<RegularOrderIcon />}
                />
                <ActionButtonCustom
                  onClick={(event) => {
                    handleCreateOrderButtons(EnumOrderCreate.FullFilable);
                  }}
                  label={
                    LanguageReducer?.languageType
                      ?.ORDERS_CREATE_FULFILLABLE_ORDER
                  }
                  width="120px"
                  startIcon={<FulfillableOrderIcon />}
                />
                <ActionButtonCustom
                  onClick={(event) => {
                    handleCreateOrderButtons(EnumOrderCreate.UploadOrders);
                  }}
                  label={LanguageReducer?.languageType?.ORDERS_UPLOAD_ORDER}
                  width="120px"
                  startIcon={<UploadOrderIcon />}
                />
              </>
            )}
          </Box>

          <DataGridTabs
            customBorderRadius="0px!impotant"
            handleTabChange={handleTabChange}
            tabData={[
              {
                label: LanguageReducer?.languageType?.ORDERS_ALL,
                route: "/orders-dashboard",
              },
              {
                label: LanguageReducer?.languageType?.ORDERS_REGULAR,
                route: "/orders-dashboard/regular",
              },
              {
                label: LanguageReducer?.languageType?.ORDERS_FULFILLABLE,
                route: "/orders-dashboard/fullFillable",
              },
              {
                label: LanguageReducer?.languageType?.ORDERS_UNFULFILLED,
                route: "/orders-dashboard/Unfullfilled",
              },
              {
                label: LanguageReducer?.languageType?.ORDERS_UNASSIGNED,
                route: "/orders-dashboard/unassigned",
              },
              {
                label: LanguageReducer?.languageType?.ORDERS_FULFILLED,
                route: "/orders-dashboard/fullfilled",
              },

              {
                label: LanguageReducer?.languageType?.ORDERS_UNPAID,
                route: "/orders-dashboard/Unpaid",
              },
              {
                label: LanguageReducer?.languageType?.ORDERS_PAID,
                route: "/orders-dashboard/paid",
                },
                {
                  label: "To be fulfilled",
                  route: "/orders-dashboard/to-be-fulfilled",
                },
            ]}
            actionBtnMenuData={actionBtnMenuData}
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
            otherBtns={
              <>
                <ToggleButtonComponent
                  value={viewMode}
                  onChange={(prevevent, nextView) => {
                    setViewMode(nextView);
                  }}
                  reverse
                />
                <Tooltip title="Select order from table" arrow>
                  <CopyButton label="Order No" onClick={handleCopyOrder} />
                </Tooltip>
                {selectionModel.length > 0 && !belowMdScreen && (
                  <>
                    {location.pathname === "/orders-dashboard/to-be-fulfilled" && (
                      <ButtonComponent
                        p={1}
                        title="Assign Station"
                        onClick={() => handleOnClickAction(() => {
                          const selectedNos = getOrdersRef.current;
                          const selectedIds = allOrders
                            ?.filter((order) => selectedNos?.includes(order.OrderNo))
                            .map((order) => order.OrderId);
                          setSelectedOrderNosForStation(selectedNos);
                          setSelectedOrderIdsForStation(selectedIds);
                          setOpenAssignStationModal(true);
                        })}
                      />
                    )}
                    <ButtonComponent
                      p={1}
                      title={
                        LanguageReducer?.languageType?.ORDER_ASSIGN_TO_CARRIER
                      }
                      onClick={() => setOpen(true)}
                    />
                    <ButtonComponent
                      p={1}
                      title={
                        LanguageReducer?.languageType?.ORDER_ASSIGN_IN_HOUSE
                      }
                      onClick={() => {
                        setIsAssignInHouse(true);
                        setOpen(true);
                      }}
                    />
                    <ButtonComponent
                      p={1}
                      title={LanguageReducer?.languageType?.ORDER_UPDATE_STATUS}
                      onClick={() => setOpenUpdateStatus(true)}
                    />
                  </>
                )}
              </>
            }
            responsiveButton={
              <>
                {belowMdScreen && menuItems.length > 0 ? (
                  <MenuIconComponent menuItems={menuItems} />
                ) : null}
              </>
            }
          />
          {isFilterOpen ? (
            <Table
              sx={{ ...styleSheet.generalFilterArea }}
              size="small"
              aria-label="a dense table"
            >
              <TableHead>
                <TableRow>
                  <Grid container spacing={1} sx={{ p: "15px" }}>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {"Created From"}
                          {/* {LanguageReducer?.languageType?.ORDER_START_DATE} */}
                        </InputLabel>

                        <CustomReactDatePickerInputFilter
                          value={startDate}
                          onClick={(date) => setStartDate(date)}
                          size="small"
                          isClearable
                          maxDate={UtilityClass.todayDate()}

                          // inputProps={{ style: { padding: "2px 5px" } }}
                        />
                      </Grid>
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {"Created To"}
                          {/* {LanguageReducer?.languageType?.ORDER_END_DATE} */}
                        </InputLabel>
                        <CustomReactDatePickerInputFilter
                          value={endDate}
                          onClick={(date) => setEndDate(date)}
                          size="small"
                          minDate={startDate}
                          disabled={!startDate ? true : false}
                          isClearable
                          maxDate={UtilityClass.todayDate()}
                        />
                      </Grid>
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {"Order From"}
                          {/* {LanguageReducer?.languageType?.ORDER_START_DATE} */}
                        </InputLabel>

                        <CustomReactDatePickerInputFilter
                          value={orderStartDate}
                          onClick={(date) => setOrderStartDate(date)}
                          size="small"
                          isClearable

                          // inputProps={{ style: { padding: "2px 5px" } }}
                        />
                      </Grid>
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {"Order To"}
                          {/* {LanguageReducer?.languageType?.ORDER_END_DATE} */}
                        </InputLabel>
                        <CustomReactDatePickerInputFilter
                          value={orderEndDate}
                          onClick={(date) => setOrderEndDate(date)}
                          size="small"
                          isClearable
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
                          {LanguageReducer?.languageType?.ORDER_STORE}
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
                        {LanguageReducer?.languageType?.ORDER_ORDER_TYPE}
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
                        {LanguageReducer?.languageType?.ORDER_CARRIER}
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
                            ?.ORDER_FULFILLMENT_STATUS
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
                        {LanguageReducer?.languageType?.ORDER_PAYMENT_STATUS}
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
                        {LanguageReducer?.languageType?.ORDER_PAYMENT_METHOD}
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
                        {LanguageReducer?.languageType?.ORDER_STATION}
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
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.ORDER_ASSIGN}
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={carrierAssignModel}
                        value={carrierAssigned}
                        optionLabel="text"
                        optionValue="id"
                        onChange={(e, val) => {
                          setCarrierAssigned(val);
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
                        {LanguageReducer?.languageType?.ORDER_SALES_PERSON}
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
                    <Grid
                      item
                      minWidth={"16.66%"}
                      xl={2}
                      lg={2}
                      md={2}
                      sm={6}
                      xs={12}
                    >
                      <InputLabel sx={styleSheet.inputLabel}>
                        {"Order Label"}
                      </InputLabel>
                      <SelectComponent
                        name="orderLabel"
                        options={orderLabel}
                        value={selectedOrderLabels}
                        height={28}
                        multiple={true}
                        optionLabel={EnumOptions.ORDER_LABELS.LABEL}
                        optionValue={EnumOptions.ORDER_LABELS.VALUE}
                        onChange={(e, val) => {
                          setselectedOrderLabels(val);
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
                          {LanguageReducer?.languageType?.ORDER_COUNTRY}
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
                              input.key
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
                                input.key
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
                              ?.ORDER_CARRIER_TRACKING_STATUS
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
                          sx={{
                            ...styleSheet.filterIcon,
                            minWidth: "80px",
                          }}
                          color="inherit"
                          variant="outlined"
                          onClick={() => {
                            handleFilterClear();
                          }}
                        >
                          {LanguageReducer?.languageType?.ORDER_CLEAR_FILTER}
                        </Button>
                        <Button
                          sx={{
                            ...styleSheet.filterIcon,
                            minWidth: "80px",
                          }}
                          variant="contained"
                          onClick={() => {
                            getAllOrders({});
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
        </Box>

        <Routes>
          <Route path="/" element={getOrderListData()} />
          <Route path="/Unfullfilled" element={getOrderListData()} />
          <Route
            loading={allOrderLoading}
            path="/fullfilled"
            element={getOrderListData()}
          />
          <Route
            loading={allOrderLoading}
            path="/regular"
            element={getOrderListData()}
          />
          <Route
            loading={allOrderLoading}
            path="/fullFillable"
            element={getOrderListData()}
          />
          <Route
            loading={allOrderLoading}
            path="/Unpaid"
            element={getOrderListData()}
          />
          <Route
            loading={allOrderLoading}
            path="/paid"
            element={getOrderListData()}
          />
          <Route
              loading={allOrderLoading}
              path="/unassigned"
              element={getOrderListData()}
            />
            <Route
              loading={allOrderLoading}
              path="/to-be-fulfilled"
              element={getOrderListData()}
            />
        </Routes>
      </div>
      {openAddOrderSaleChannelModal.open && (
        <AddSaleChannelForOrderModal
          open={openAddOrderSaleChannelModal.open}
          onClose={() =>
            setOpenAddOrderSaleChannelModal((prev) => ({
              ...prev,
              open: false,
              storeId: null,
            }))
          }
          storeId={openAddOrderSaleChannelModal?.storeId}
          getAllOrders={() => getAllOrders({})}
          orderNosData={getOrdersRef.current}
        />
      )}
      {openAddOrderSaleChannelModal.openUnassignSaleChannel && (
        <DeleteConfirmationModal
          open={openAddOrderSaleChannelModal.openUnassignSaleChannel}
          onClose={() =>
            setOpenAddOrderSaleChannelModal((prev) => ({
              ...prev,
              openUnassignSaleChannel: false,
            }))
          }
          loading={openAddOrderSaleChannelModal.openUnassignSaleChannelLoading}
          handleDelete={unassignSaleChannel}
          heading={
            "The selected orders will be unassigned from the sales channel."
          }
          message={
            "Selected order will be unassigned. You can reassign it from the Order Dashboard."
          }
          buttonText={LanguageReducer?.languageType?.ORDERS_UNASSIGN}
        />
      )}
      {open && (
        <BatchAssigntoCarrierModal
          isAssignInHouse={isAssignInHouse}
          setIsAssignInHouse={setIsAssignInHouse}
          resetRowRef={resetRowRef}
          getAllOrders={() => getAllOrders({})}
          orderNosData={getOrdersRef.current}
          activeCarriers={allActiveCarriersForSelection}
          open={open}
          setOpen={setOpen}
          {...props}
        />
      )}
      {openOrderLabelModal && (
        <AddOrderLabelModal
          open={openOrderLabelModal}
          onClose={() => setOpenOrderLabelModal(false)}
          orderNosData={getOrdersRef.current}
          getAllOrders={() => getAllOrders({})}
          orderLabel={orderLabel}
          setOrderLabel={setOrderLabel}
        />
      )}
      {openFulfillment && (
        <BatchOrderFulfillmentModal
          resetRowRef={resetRowRef}
          getAllOrders={() => getAllOrders({})}
          orderNosData={getOrdersRef.current}
          allFullFillment={allFullFillmentStatusLookup}
          open={openFulfillment}
          setOpen={setOpenFulfillment}
          {...props}
        />
      )}
      {openUpdateStatus && (
        <BatchUpdateOrderStatusModal
          resetRowRef={resetRowRef}
          getAllOrders={() => getAllOrders({})}
          orderNosData={getOrdersRef.current}
          allCarrierTrackingStatus={allCarrierTrackingStatus}
          open={openUpdateStatus}
          setOpen={setOpenUpdateStatus}
          {...props}
        />
      )}
      {openDelete && (
        <DeleteConfirmationModal
          open={openDelete}
          setOpen={setOpenDelete}
          handleDelete={handleDelete}
          loading={deleteLoading}
          heading={
            LanguageReducer?.languageType
              ?.ORDER_ARE_YOU_SURE_YOU_WANT_TO_DELETE_SELECTED_ORDERS
          }
          message={
            LanguageReducer?.languageType
              ?.ORDER_SELECTED_ORDERS_WILL_BE_DELETE_IMMEDIATELY_YOU_CANT_UNDO_THIS_ACTION
          }
          buttonText={LanguageReducer?.languageType?.ORDER_DELETE}
        />
      )}
      
      {openAssignStationModal && (
        <AssignOrderStationModal
          open={openAssignStationModal}
          handleClose={() => setOpenAssignStationModal(false)}
          handleRefresh={() => {
            if (resetRowRef) resetRowRef.current = true;
            setSelectionModel([]);
            getAllOrders({
              isWithoutStation: location.pathname.includes("to-be-fulfilled"),
            });
          }}
          selectedOrderIds={selectedOrderIdsForStation}
          selectedOrderNos={selectedOrderNosForStation}
        />
      )}

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
              ?.ORDER_SELECTED_ORDERS_WILL_BE_UNASSIGN_IMMEDIATELY_YOU_CANT_UNDO_THIS_ACTION
          }
          buttonText={LanguageReducer?.languageType?.ORDERS_UNASSIGN}
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
export default OrderPage;
