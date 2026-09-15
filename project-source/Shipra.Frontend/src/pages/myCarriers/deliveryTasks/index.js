import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";

import React, { useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import { Route, Routes } from "react-router-dom";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  GetAllCarrierTrackingStatusForSelection,
  GetAllDeliveryTask,
  GetAllDeliveryTaskStatusForSelection,
  GetDriversForSelection,
  GetStoresForSelection,
  GetAllSalePersonForSelection,
  GetMetaFieldsByOrderIds,
  ExcelExportDeliveryTasks,
  GetAllClientOrderLabelLookupForSelection,
  GetValidateDocumentSetting,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import BatchOutScanModal from "../../../components/modals/myCarrierModals/BatchOutScanModal";
import TransferDeliveryTaskModal from "../../../components/modals/myCarrierModals/TransferDeliveryTaskModal";
import BatchRevertModal from "../../../components/modals/myCarrierModals/BatchRevertModal";
import EditMetaFieldModal from "../../../components/modals/myCarrierModals/EditMetaFieldModal";
import BatchUpdateOrderStatusModal from "../../../components/modals/orderModals/BatchUpdateOrderStatusModal";
import GeneralTabBar from "../../../components/shared/tabsBar";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  errorNotification,
  warningNotification,
  successNotification,
} from "../../../utilities/toast";
import { isShowCountryInTabbarFlag } from "../../../utilities/cookies";
import TasksList from "./tasksList";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import { EnumOptions } from "../../../utilities/enum";
import initialStateFilter from "../../../utilities/filterState";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  useGetAllCountries,
  useGetBreakPoint,
  handleFilterAddressSchemaSelectDataIncExcValues,
  downloadWayBillsByOrderNos,
  amountFormat,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema.js";
import CountrySchema from "../../../utilities/helpers/countryschema.js";
import LabelWithCheckBox from "../../../.reUseableComponents/TextField/LabelWithCheckBox";
import DeliveryTaskMapComponent from "./MapComponent";
import Colors from "../../../utilities/helpers/Colors";
import ExistingNoteModal from "../../../components/modals/myCarrierModals/ExistingNoteModal";
import MenuIconComponent from "../../../.reUseableComponents/Buttons/MenuIconComponent";
import SearchInputAutoCompleteMultiple from "../../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";
import SwitchMui from "../../../.reUseableComponents/Switch/SwitchMui";

const MAX_TAGS = 200;

const EnumTabFilter = Object.freeze({
  All: "/delivery-tasks",
  Unassigned: "/delivery-tasks/unassigned",
  Assigned: "/delivery-tasks/assigned",
});
const EnumDashboardAction = {
  BATCHOUTSCAN: "BatchOutScan",
  BATCHREVERT: "BatchRevert",
  EXISTINGNOTEMODAL: "ExistingNoteModal",
};
function DeliveryTasks(props) {
  const [inputFields, setInputFields] = useState([]);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allDeliveryTask, setAllDeliveryTask] = useState([]);
  const [isAllListLoading, setIsAllListLoading] = useState([]);
  const [open, setOpen] = useState(false);
  const [revertOpen, setRevertOpen] = useState(false);
  const [openUpdateStatus, setOpenUpdateStatus] = useState(false);
  const [allDrivers, setAllDrivers] = useState([]);
  const [driverId, setDriverId] = useState([]);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const [selectedDeliveryTasks, setSelectedDeliveryTasks] = useState([]);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [storeId, setStoreId] = useState(initialStateFilter.multiple);
  const [allSalesPerson, setAllSalesPerson] = useState([]);
  const [selectedSalesPerson, setSelectedSalesPerson] = useState(initialStateFilter.multiple);
  const [allCarrierTrackingStatus, setAllCarrierTrackingStatus] = useState([]);
  const [selectedCarrierTrackingStatus, setSelectedCarrierTrackingStatus] =
    useState([]);
  const [selectedOrderLabels, setselectedOrderLabels] = useState(initialStateFilter.multiple);
  const [driverIncludeChecked, setDriverIncludeChecked] = useState(true);
  const [duplicateFilter, setDuplicateFilter] = useState({ text: "All", id: 3 });
  const duplicateOptions = [
    { text: "All", id: 3 },
    { text: "Duplicate", id: 1 },
    { text: "Not Duplicate", id: 2 },
  ];
  const [noteCompleteFilter, setNoteCompleteFilter] = useState(false);

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
  const [StatusTaskForSelection, setStatusTaskForSelection] = useState([]);
  const [deliveryTaskStatusId, setdeliveryTaskStatusId] = useState([]);
  const [openExistingNote, setopenExistingNote] = useState(false);
  const [openTransferModal, setOpenTransferModal] = useState(false);
  const [showMap, setShowMap] = useState(false);
  const [defaultCard, setDefaultCard] = useState({
    total: 0,
    totalRegion: 0,
    assignedShipment: 0,
    unassigned: 0,
  });
  const belowMdScreen = useGetBreakPoint("md");
  const { register, reset, control } = useForm({
    defaultValues: { startDate: null, endDate: null },
  });
  let defaultValues = {
    startDate: null,
    endDate: null,
    driverAssignedStatus: 0,
  };
  const [isFilterReset, setIsFilterReset] = useState(false);
  const mapChildRef = useRef(null);
  // Method to reset the child state

  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  let getDriversForSelection = async () => {
    let res = await GetDriversForSelection();

    if (res.data.result != null) {
      setAllDrivers(res.data.result);
    }
  };

  const [orderLabel, setOrderLabel] = useState([]);
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

  const handleToggleMap = () => {
    setShowMap(!showMap);
  };
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const getFiltersFromState = (overrideCountryId) => {
    let search = inputFields.join();
    let filters = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 1000,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
      DriverAssignedStatus: driverAssignedStatus,
      StoreIds: storeId?.map((data) => data.storeId).toString(),
      SalePersonIds: selectedSalesPerson?.map((data) => data.id).toString(),
      CarrierTrackingStatusIds: selectedCarrierTrackingStatus
        ?.map((data) => data.carrierTrackingStatusId)
        .toString(),
      OrderLabels: selectedOrderLabels
        ?.map((data) => data.labelName)
        .toString(),
      CountryId: overrideCountryId !== undefined
        ? overrideCountryId
        : (isShowCountryInTabbarFlag()
            ? (reduxSelectedCountry
                ? String(reduxSelectedCountry.countryId || reduxSelectedCountry.id || reduxSelectedCountry)
                : "")
            : (selectedAddressSchema.country
                ? String(selectedAddressSchema.country)
                : "")),
      OrderAddressFilter: handleFilterAddressSchemaSelectDataIncExcValues(addressSchemaSelectDataIncExcValues),
      DriverIds: driverId?.map((data) => data?.DriverId).toString(),
      IncludeDriver: driverIncludeChecked,
      DeliveryTaskStatusIds: deliveryTaskStatusId
        ?.map((data) => data?.deliveryTaskStatusId)
        .toString(),
      DuplicateStatus: duplicateFilter?.id === 3 ? null : duplicateFilter?.id,
      DeliveryNoteStatusId: noteCompleteFilter ? 2 : null,
    };
    return filters;
  };

  let getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    // console.log("getStoresForSelection", res.data);
    if (res.data.result != null) {
      setStoresForSelection(res.data.result);
    }
  };
  let getStatusTaskForSelection = async () => {
    let res = await GetAllDeliveryTaskStatusForSelection();
    // console.log("getStoresForSelection", res.data);
    if (res.data.result != null) {
      setStatusTaskForSelection(res.data.result);
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
  const { countries } = useGetAllCountries();
  const handleDefaultCardSet = () => {
    const allDTask = allDeliveryTask?.list;
    if (allDTask) {
      // Grouping delivery tasks by RegionName and calculating count
      const groupedByRegionCount = allDTask?.reduce((acc, task) => {
        const { RegionName } = task;
        if (!acc[RegionName]) {
          acc[RegionName] = { totalRegion: 0 };
        }
        acc[RegionName].totalRegion++;
        return acc;
      }, {});
      const regionCount = Object.keys(groupedByRegionCount).length;
      // Update the total state
      setDefaultCard((prevState) => ({
        ...prevState,
        total: allDeliveryTask?.TotalCount,
        unassigned: allDTask?.filter((x) => (x.DeliveryTaskStatusId || x.deliveryTaskStatusId) === 1).length,
        assignedShipment: allDTask?.filter((x) => x.DriverName != "").length,
        totalRegion: regionCount,
      }));
    }
  };

  let getAllDeliveryTask = async (overrideCountryId) => {
    setIsAllListLoading(true);
    let params = getFiltersFromState(overrideCountryId);
    let res = await GetAllDeliveryTask(params);
    if (res.data.result !== null) {
      let list = res.data.result.list;
      let hasAdditionalFieldRows = list.filter((r) => r.HasAdditionalField || r.hasAdditionalField);

      if (hasAdditionalFieldRows.length > 0) {
        let orderIds = hasAdditionalFieldRows.map((r) => r.OrderId || r.orderId);
        console.log("Found rows with HasAdditionalField", orderIds);
        try {
          let metaRes = await GetMetaFieldsByOrderIds({ OrderIds: orderIds });
          if (metaRes.data.result) {
            let metaData = metaRes.data.result;
            console.log("Fetched MetaFields", metaData);
            list = list.map((row) => {
              let meta = metaData.find((m) => {
                let mId = m.OrderId || m.orderId;
                let rId = row.OrderId || row.orderId;
                return mId && rId && mId.toLowerCase() === rId.toLowerCase();
              });
              if (meta && meta.settingConfig) {
                try {
                  row.SettingConfigData = JSON.parse(meta.settingConfig);
                } catch (e) {
                  console.error("Error parsing settingConfig", e);
                  row.SettingConfigData = [];
                }
              } else {
                row.SettingConfigData = [];
              }
              return row;
            });
            res.data.result.list = list;
          }
        } catch (e) {
          console.error("Error fetching MetaFields", e);
        }
      } else {
        list = list.map(row => { row.SettingConfigData = []; return row; });
        res.data.result.list = list;
      }
      setAllDeliveryTask(res.data.result);
    }
    setIsAllListLoading(false);
    resetRowRef.current = false;
  };
  const [documentSetting, setDocumentSetting] = useState({});
  let getValidateDocumentSetting = async () => {
    try {
      let res = await GetValidateDocumentSetting();
      if (res?.data?.result !== null && res?.data?.result !== undefined) {
        setDocumentSetting(res.data.result);
      }
    } catch (err) {
      console.error("Error fetching document setting:", err);
    }
  };

  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry
  );
  const isInitialCountryMountTasks = useRef(true);
  useEffect(() => {
    if (!isShowCountryInTabbarFlag()) return;
    if (isInitialCountryMountTasks.current) {
      isInitialCountryMountTasks.current = false;
      return;
    }
    const resolvedCountryId =
      reduxSelectedCountry?.countryId ||
      reduxSelectedCountry?.id ||
      (typeof reduxSelectedCountry === "number" || typeof reduxSelectedCountry === "string"
        ? reduxSelectedCountry
        : "");
    handleSetSchema("country", reduxSelectedCountry || null);
    getAllDeliveryTask(resolvedCountryId ? String(resolvedCountryId) : "");
  }, [reduxSelectedCountry]);

  useEffect(() => {
    getAllCarrierTrackingStatusForSelection();
    getStoresForSelection();
    getDriversForSelection();
    getStatusTaskForSelection();
    getAllClientOrderLabelLookupForSelection();
    getValidateDocumentSetting();
  }, []);
  useEffect(() => {
    if (isFilterReset) {
      getAllDeliveryTask();
    }
  }, [isFilterReset]);
  useEffect(() => {
    getAllDeliveryTask();
  }, [noteCompleteFilter]);

  const getAllSalePersonForSelection = async () => {
    try {
      const response = await GetAllSalePersonForSelection();
      setAllSalesPerson(response.data.result || []);
    } catch (error) {
      console.error("Error fetching GetAllSalePersonForSelection:", error?.response);
    }
  };

  useEffect(() => {
    getAllSalePersonForSelection();
  }, []);
  useEffect(() => {
    let timeoutId;
    if (inputFields.length > 0) {
      setIsAllListLoading(true);
      timeoutId = setTimeout(() => {
        getAllDeliveryTask();
      }, 500);
    } else {
      getAllDeliveryTask();
    }
    return () => {
      clearTimeout(timeoutId);
    };
  }, [inputFields]);
  useEffect(() => {
    handleDefaultCardSet();
  }, [allDeliveryTask]);
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [isTabFilter, setIsTabFilter] = useState(false);
  const [isShowFilter, setIsShowFilter] = useState(false);
  const [driverAssignedStatus, setDriverAssignedStatus] = useState(
    defaultValues.driverAssignedStatus
  );

  useEffect(() => {
    if (isfilterClear) {
      getAllDeliveryTask();
      resetDates();
      setIsfilterClear(false);
    }
  }, [isfilterClear]);
  useEffect(() => {
    if (isTabFilter) {
      getAllDeliveryTask();
      setIsTabFilter(false);
    }
  }, [driverAssignedStatus]);
  const handleFilterRest = () => {
    setDriverAssignedStatus(defaultValues.driverAssignedStatus);
    setStartDate(defaultValues.startDate);
    setEndDate(defaultValues.endDate);
    setSelectedCarrierTrackingStatus(initialStateFilter.multiple);
    setselectedOrderLabels(initialStateFilter.multiple);
    handleReset();
    setStoreId(initialStateFilter.multiple);
    setSelectedSalesPerson(initialStateFilter.multiple);
    setDriverId(initialStateFilter.multiple);
    setdeliveryTaskStatusId(initialStateFilter.multiple);
    setDuplicateFilter({ text: "All", id: 3 });
    setNoteCompleteFilter(false);

    if (mapChildRef.current) {
      mapChildRef?.current.Rest();
    }
    setInputFields([]);
  };
  const handleFilterClear = async () => {
    handleFilterRest();
    setIsfilterClear(true);
  };
  const handleTabChange = (event, filterValue) => {
    handleFilterRest();
    setIsFilterOpen(false);
    if (filterValue === EnumTabFilter.All) {
      setDriverAssignedStatus(0); //0 for all shipments
      setIsShowFilter(true);
    } else if (filterValue == EnumTabFilter.Unassigned) {
      setDriverAssignedStatus(1); //1 for unassigned shipments
      setIsShowFilter(false);
    } else if (filterValue == EnumTabFilter.Assigned) {
      setDriverAssignedStatus(2); //2 for Assigned shipments
      setIsShowFilter(false);
    }
    resetRowRef.current = true;
    setIsTabFilter(true);
  };
  const isAllUnassignedOrdersAndShowMsg = () => {
    let isUnAssign = false;
    if (selectedDeliveryTasks.length > 0) {
      let selectedRowsData = allDeliveryTask?.list?.filter((item) =>
        selectedDeliveryTasks.includes(item.OrderNo)
      );
      const orderNosWithoutDriver = selectedRowsData
        .filter((item) => (item.DeliveryTaskStatusId || item.deliveryTaskStatusId) !== 1) // Filter objects with status not equal to Unallocated
        .map((item) => item.OrderNo); // Extract OrderNo values

      if (orderNosWithoutDriver.length > 0) {
        const commaSeparatedOrderNos = orderNosWithoutDriver.join(", ");
        errorNotification(
          "Please remove the below-mentioned orders from the selection because they have already been in the Delivery note. " +
          commaSeparatedOrderNos
        );
      } else {
        isUnAssign = true;
      }
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    }
    return isUnAssign;
  };
  const [openEditMetaField, setOpenEditMetaField] = useState(false);
  const [openAdvanceSearchModal, setOpenAdvanceSearchModal] = useState(false);

  const getRoutComponent = (
    <TasksList
      loading={isAllListLoading}
      allDeliveryTask={allDeliveryTask}
      getOrdersRef={getOrdersRef}
      resetRowRef={resetRowRef}
      getAllDeliveryTask={getAllDeliveryTask}
      setSelectedDeliveryTasks={setSelectedDeliveryTasks}
      isFilterOpen={isFilterOpen}
      setOpenEditMetaField={setOpenEditMetaField}
      setOpenTransferModal={setOpenTransferModal}
      orderLabel={orderLabel}
      setOrderLabel={setOrderLabel}
      documentSetting={documentSetting}
      openAdvanceSearchModal={openAdvanceSearchModal}
      setOpenAdvanceSearchModal={setOpenAdvanceSearchModal}
      setInputFields={setInputFields}
      allCarrierTrackingStatus={allCarrierTrackingStatus}
    />
  );
  const handleBatchRevert = () => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length === 0) {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    } else {
      setRevertOpen(true);
    }
  };
  const handleBatchTransfer = () => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length === 0) {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST || "Please select at least one row."
      );
    } else {
      setSelectedDeliveryTasks(selectedTrNos);
      setOpenTransferModal(true);
    }
  };
  const handleResetOutscanModel = () => {
    if (mapChildRef.current) {
      mapChildRef?.current.Rest();
    }
  };

  const handleEditMetaField = () => {
    const selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length === 0) {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST || "Please select at least one row."
      );
    } else {
      setOpenEditMetaField(true);
    }
  };

  const handleResetSeelctedRows = () => {
    resetRowRef.current = true;
  };

  const downloadExcel = () => {
    const selectedTrNos = getOrdersRef.current;

    // Get base filters from state
    let params = getFiltersFromState();

    // Override search parameter if there are selected order numbers
    if (selectedTrNos.length > 0) {
      params.filterModel.search = selectedTrNos.join();
    }

    ExcelExportDeliveryTasks(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadExcel(res.data, "DeliveryTasks");
          handleResetSeelctedRows();
        } else {
          successNotification("Order not found");
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download Excel");
      });
  };

  const actionBtnMenuData = !belowMdScreen
    ? [
      {
        title: LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_BATCH_REVERT,
        onClick: () => {
          handleBatchRevert();
        },
      },
      {
        title: "Transfer Tasks",
        onClick: () => {
          handleBatchTransfer();
        },
      },

      {
        title: LanguageReducer?.languageType?.ORDER_BATCH_UPDATE_ORDER_STATUS || "Batch Update Status",
        onClick: () => {
          const selectedTrNos = getOrdersRef.current;
          if (selectedTrNos.length === 0) {
            warningNotification(LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST || "Please select at least one row.");
          } else {
            setOpenUpdateStatus(true);
          }
        },
      },
      {
        title: LanguageReducer?.languageType?.ORDER_EXCEL_EXPORT || "Excel Export",
        onClick: () => {
          const selectedTrNos = getOrdersRef.current;
          if (selectedTrNos.length === 0) {
            warningNotification(LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST || "Please select at least one row.");
          } else {
            downloadExcel();
          }
        },
      },
      {
        title: LanguageReducer?.languageType?.SHIPMENTS_PRINT_LABELS || "Print Labels",
        onClick: () => {
          const selectedTrNos = getOrdersRef.current;
          if (selectedTrNos.length === 0) {
            warningNotification(LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST || "Please select at least one row.");
          } else {
            downloadWayBillsByOrderNos(selectedTrNos.join(","), documentSetting?.DocumentTemplateId || 1);
          }
        },
      },
    ]
    : null;
  const menuItems = [
    {
      label: LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_BATCH_REVERT,
      onClick: () => {
        handleBatchRevert();
      },
    },
    {
      label: "Transfer Tasks",
      onClick: () => {
        handleBatchTransfer();
      },
    },

    {
      label: LanguageReducer?.languageType?.ORDER_BATCH_UPDATE_ORDER_STATUS || "Batch Update Status",
      onClick: () => {
        const selectedTrNos = getOrdersRef.current;
        if (selectedTrNos.length === 0) {
          warningNotification(LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST || "Please select at least one row.");
        } else {
          setOpenUpdateStatus(true);
        }
      },
    },
    {
      label: LanguageReducer?.languageType?.ORDER_EXCEL_EXPORT || "Excel Export",
      onClick: () => {
        const selectedTrNos = getOrdersRef.current;
        if (selectedTrNos.length === 0) {
          warningNotification(LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST || "Please select at least one row.");
        } else {
          downloadExcel();
        }
      },
    },
    {
      label: LanguageReducer?.languageType?.SHIPMENTS_PRINT_LABELS || "Print Labels",
      onClick: () => {
        const selectedTrNos = getOrdersRef.current;
        if (selectedTrNos.length === 0) {
          warningNotification(LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST || "Please select at least one row.");
        } else {
          downloadWayBillsByOrderNos(selectedTrNos.join(","), documentSetting?.DocumentTemplateId || 1);
        }
      },
    },
    {
      label:
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_TASKS_CREATE_DELIVERY_NOTE,
      onClick: () => {
        if (!isAllUnassignedOrdersAndShowMsg()) {
          return;
        } else {
          setopenExistingNote(true);
        }
      },
    },
    {
      label:
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_TASKS_ADD_TO_EXISTING_NOTE,
      onClick: () => {
        if (!isAllUnassignedOrdersAndShowMsg()) {
          return;
        } else {
          setopenExistingNote(true);
        }
      },
    },
    {
      label: showMap
        ? LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_TASKS_CHOOSE_FROM_GRID
        : LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_TASKS_CHOOSE_FROM_MAP,
      onClick: () => {
        handleToggleMap();
      },
    },
  ];

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Box
          sx={{
            background: "#F8F8F8",
            border: "1px solid rgba(224, 224, 224, 1)",
            borderBottom: "none",
            borderRadius: "8px 8px 0px 0px!important",
            paddingTop: "5px",
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
          </Box>
        </Box>
        <DataGridTabs
          handleTabChange={handleTabChange}
          tabData={[
            {
              label:
                LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_All,
              route: EnumTabFilter.All,
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.MY_CARRIER_DELIVERY_TASKS_ASSIGNED,
              route: EnumTabFilter.Assigned,
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.MY_CARRIER_DELIVERY_TASKS_UNASSIGNED,
              route: EnumTabFilter.Unassigned,
            },
          ]}
          actionBtnMenuData={actionBtnMenuData}
          otherBtns={
            !belowMdScreen ? (
              <>
                <SwitchMui
                  checked={noteCompleteFilter}
                  onChange={() => setNoteCompleteFilter((prev) => !prev)}
                  label={"Completed"}
                />
                 {selectedDeliveryTasks.length > 0 && (
                  <ButtonComponent
                    btnMdWidth={"120px"}
                    btnLgWidth={"140px"}
                    bg={"#25D366"}
                    title={"Copy WhatsApp"}
                    onClick={() => {
                      const selectedRowsData = allDeliveryTask?.list?.filter((item) =>
                        selectedDeliveryTasks.includes(item.OrderNo)
                      );
                      if (!selectedRowsData || selectedRowsData.length === 0) return;

                      const messages = selectedRowsData.map((row) => {
                        const customer = row.Customer || "PERSON NAME";
                        const orderNo = row.OrderNo || "ORDER NO";
                        const description = row.Description || "DESCRIPTION";
                        const amount = amountFormat(row.Amount) || "AMOUNT";
                        const currency = row.CurrencyCode || "AED";
                        const address = row.CustomerFullAddress || row.DropOfAddress || row.CustomerAddress || "";
                        const mobile1 = row.Mobile1 || row.Phone || "";
                        const mobile2 = row.Mobile2 || "";

                        let workingNumberText = "";
                        if (mobile1 && mobile2) {
                          workingNumberText = `*${mobile1}* And *${mobile2}*`;
                        } else if (mobile1) {
                          workingNumberText = `*${mobile1}*`;
                        } else if (mobile2) {
                          workingNumberText = `*${mobile2}*`;
                        }

                        return `Dear *${customer}*\nYour Order Number *${orderNo}*\nYour Product *${description}* Is Out For Delivery ,\nTotal Amount Is *${amount} ${currency}*\nAddress: ${address}\nKindly Send Your Whats App Location For Earleist Delivery\nAnd Your Working Numebr Is : ${workingNumberText}`;
                      });

                      const combinedMessage = messages.join("\n\n\n");
                      navigator.clipboard.writeText(combinedMessage).then(() => {
                        successNotification("WhatsApp messages copied to clipboard!");
                      }).catch((err) => {
                        errorNotification("Failed to copy messages");
                      });
                    }}
                  />
                )}
                {selectedDeliveryTasks.length > 0 && (
                  <>
                    <ButtonComponent
                      btnMdWidth={"130px"}
                      btnLgWidth={"155px"}
                      title={
                        LanguageReducer?.languageType
                          ?.MY_CARRIER_DELIVERY_TASKS_CREATE_DELIVERY_NOTE
                      }
                      onClick={() => {
                        if (!isAllUnassignedOrdersAndShowMsg()) {
                          return;
                        } else {
                          setOpen(true);
                        }
                      }}
                    />
                  </>
                )}
                {selectedDeliveryTasks.length > 0 && (
                  <>
                    <ButtonComponent
                      btnMdWidth={"130px"}
                      btnLgWidth={"155px"}
                      title={
                        LanguageReducer?.languageType
                          ?.MY_CARRIER_DELIVERY_TASKS_ADD_TO_EXISTING_NOTE
                      }
                      onClick={() => {
                        if (!isAllUnassignedOrdersAndShowMsg()) {
                          return;
                        } else {
                          setopenExistingNote(true);
                        }
                      }}
                    />
                  </>
                )}
                <ButtonComponent
                  btnMdWidth={"115px"}
                  btnLgWidth={"130px"}
                  bg={Colors.succes}
                  title={
                    showMap
                      ? LanguageReducer?.languageType
                        ?.MY_CARRIER_DELIVERY_TASKS_CHOOSE_FROM_GRID
                      : LanguageReducer?.languageType
                        ?.MY_CARRIER_DELIVERY_TASKS_CHOOSE_FROM_MAP
                  }
                  onClick={() => {
                    handleToggleMap();
                  }}
                />
              </>
            ) : null
          }
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
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
                <Grid container spacing={2} sx={{ p: "15px" }}>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_START_DATE
                        }
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
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_END_DATE
                        }
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
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_STORE
                        }
                      </InputLabel>
                      <SelectComponent
                        multiple={true}
                        name="reason"
                        height={28}
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
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.ORDER_SALES_PERSON || "Sales Person"}
                      </InputLabel>
                      <SelectComponent
                        multiple={true}
                        name="salePerson"
                        height={28}
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
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <LabelWithCheckBox
                        isShowSwitch={true}
                        height={28}
                        checked={driverIncludeChecked}
                        checkedLabel={
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_INCLUDE
                        }
                        unCheckedLabel={
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_EXCLUDE
                        }
                        inputLabel={
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_DRIVER
                        }
                        onChange={(e) =>
                          setDriverIncludeChecked(e.target.checked)
                        }
                      />
                      <SelectComponent
                        multiple={true}
                        name="reason"
                        options={allDrivers}
                        height={28}
                        value={driverId}
                        optionLabel={EnumOptions.DRIVER.LABEL}
                        optionValue={EnumOptions.DRIVER.VALUE}
                        getOptionLabel={(option) => option?.DriverName}
                        onChange={(e, val) => {
                          setDriverId(val);
                        }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.MY_CARRIER_DELIVERY_TASKS_CARRIER_TRACKING_STATUS
                      }
                    </InputLabel>
                    <SelectComponent
                      multiple={true}
                      height={28}
                      optionLabel={EnumOptions.CARRIER_TRACKING_STATUS.LABEL}
                      optionValue={EnumOptions.CARRIER_TRACKING_STATUS.VALUE}
                      name="carrierStatus"
                      options={allCarrierTrackingStatus}
                      value={selectedCarrierTrackingStatus}
                      getOptionLabel={(option) => option.text}
                      onChange={(e, val) => {
                        setSelectedCarrierTrackingStatus(val);
                      }}
                    />
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.MY_CARRIER_DELIVERY_TASKS_DELIVERY_TASK_STATUS
                      }
                    </InputLabel>
                    <SelectComponent
                      multiple={true}
                      height={28}
                      optionLabel={EnumOptions.DELIVRRY_TASK_STATUS.LABEL}
                      optionValue={EnumOptions.DELIVRRY_TASK_STATUS.VALUE}
                      name="carrierStatus"
                      options={StatusTaskForSelection}
                      value={deliveryTaskStatusId}
                      getOptionLabel={(option) => option.text}
                      onChange={(e, val) => {
                        setdeliveryTaskStatusId(val);
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
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_COUNTRY
                        }
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
                        key={index}
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
                              () => { },
                              input.key
                            );
                          }}
                        />
                      </Grid>
                    ))}
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        Order Label
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
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        Duplicate Orders
                      </InputLabel>
                      <SelectComponent
                        name="duplicateOrders"
                        options={duplicateOptions}
                        value={duplicateFilter}
                        height={28}
                        multiple={false}
                        optionLabel="text"
                        optionValue="id"
                        addPleaseSelectOptionOnClear={false}
                        onChange={(e, val) => {
                          setDuplicateFilter(val);
                        }}
                      />
                    </Grid>
                  </>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Stack
                      alignItems="flex-end"
                      direction="row"
                      spacing={1}
                      sx={{ ...styleSheet.filterButtonMargin, height: "100%" }}
                    >
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          handleFilterClear();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllDeliveryTask();
                        }}
                      >
                        {LanguageReducer?.languageType?.FILTER}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}
        {showMap ? (
          <DeliveryTaskMapComponent
            mapChildRef={mapChildRef}
            setSelectedDeliveryTasks={setSelectedDeliveryTasks}
            allDeliveryTask={allDeliveryTask}
            setAllDeliveryTask={setAllDeliveryTask}
            defaultCard={defaultCard}
          />
        ) : (
          <Routes>
            <Route path="/" element={getRoutComponent} />
            <Route path="/unassigned" element={getRoutComponent} />
            <Route path="/assigned" element={getRoutComponent} />
          </Routes>
        )}
      </div>

      {open && (
        <BatchOutScanModal
          open={open}
          setOpen={setOpen}
          {...props}
          orderNosData={selectedDeliveryTasks}
          allDrivers={allDrivers}
          getAllDeliveryTask={getAllDeliveryTask}
          resetRowRef={resetRowRef}
          allDeliveryTask={allDeliveryTask}
          handleResetOutscanModel={handleResetOutscanModel}
          getDriversForSelection={getDriversForSelection}
        />
      )}
      {openTransferModal && (
        <TransferDeliveryTaskModal
          open={openTransferModal}
          setOpen={setOpenTransferModal}
          orderNosData={selectedDeliveryTasks}
          allDrivers={allDrivers}
          getAllDeliveryTask={getAllDeliveryTask}
          resetRowRef={resetRowRef}
          handleResetOutscanModel={handleResetOutscanModel}
          allDeliveryTask={allDeliveryTask}
          getDriversForSelection={getDriversForSelection}
        />
      )}
      {openExistingNote && (
        <ExistingNoteModal
          open={openExistingNote}
          setOpen={setopenExistingNote}
          setOpenTransferModal={setOpenTransferModal}
          setRevertOpen={setRevertOpen}
          getAllDeliveryTask={getAllDeliveryTask}
          handleResetOutscanModel={handleResetOutscanModel}
          orderNosData={selectedDeliveryTasks}
          resetRowRef={resetRowRef}
          allDeliveryTask={allDeliveryTask}
        />
      )}
      {revertOpen && (
        <BatchRevertModal
          open={revertOpen}
          setOpen={setRevertOpen}
          {...props}
          orderNosData={getOrdersRef.current}
          allDrivers={allDrivers}
          getAllDeliveryTask={getAllDeliveryTask}
          resetRowRef={resetRowRef}
        />
      )}
      {openEditMetaField && (
        <EditMetaFieldModal
          open={openEditMetaField}
          setOpen={setOpenEditMetaField}
          selectedDeliveryTasks={selectedDeliveryTasks}
          allDeliveryTask={allDeliveryTask}
          getAllDeliveryTask={getAllDeliveryTask}
        />
      )}

      {openUpdateStatus && (
        <BatchUpdateOrderStatusModal
          resetRowRef={resetRowRef}
          getAllOrders={getAllDeliveryTask}
          orderNosData={getOrdersRef.current}
          allCarrierTrackingStatus={allCarrierTrackingStatus}
          open={openUpdateStatus}
          setOpen={setOpenUpdateStatus}
        />
      )}
    </Box>
  );
}
export default DeliveryTasks;
