import {
  Autocomplete,
  Box,
  Button,
  Chip,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes, useLocation } from "react-router-dom";
import {
  ExcelExportInventorySales,
  ExcelExportInventorySalesSummary,
  GetAllInventorySales,
  GetAllRegions,
  GetAllStationLookup,
  GetPDFInventorySales,
  GetPDFInventorySalesSummary,
  GetProductStocksForSelection,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";

import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import SwitchMui from "../../../.reUseableComponents/Switch/SwitchMui";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { EnumOptions } from "../../../utilities/enum";
import {
  fetchMethod,
  useGetBreakPoint,
} from "../../../utilities/helpers/Helpers";
import {
  handleGetAllSaleChannelsByStoreId,
  useGetAllCarrierStatus,
} from "../../../utilities/helpers/HelpersFilter";
import { errorNotification } from "../../../utilities/toast";
import InventoryList from "./inventorySaleList";

const MAX_TAGS = 150;

function InventorySalePage(props) {
  const [alInventorySale, setAlInventorySale] = useState([]);
  const [gridLoading, setGridLoading] = useState([]);

  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inputFields, setInputFields] = useState([]);
  const [selectedRows, setSelectedRows] = useState([]);
  const [isCheckedInTransit, setIsCheckedInTransit] = useState(true);
  const [isCheckedFulfilledOnly, setIsCheckedFulfilledOnly] = useState(true);
  const [selectedRegion, setSelectedRegion] = useState([]);

  const {
    loading: carrierLoading,
    allCarrierStatus,
    handleResetCarrier,
  } = useGetAllCarrierStatus();

  const [selectedCarrierStatus, setSelectedCarrierStatus] = useState([]);
  const [allSalePersons, setAllSalePersons] = useState([]);

  const [selectedSalePerson, setSelectedSalePerson] = useState([]);
  const [selectedStoreIds, setSelectedStoreIds] = useState([]);

  let defaultValues = {
    startDate: null,
    endDate: null,
    productStationIds: [],
    productSKUs: "",
    trackingStatusID: "0",
    fullFillmentStatusId: {
      id: 0,
      text: "Select Please",
    },
    inTransit: 0,
    regionIds: 0,
    stationId: {
      productStationId: 0,
      sname: "Select Please",
    },
  };
  // const { register, getValues, setValue, reset, control } = useForm({
  //   defaultValues,
  // });
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const [storeId, setStoreId] = useState([]);
  const [fullFillmentStatusId, setFullFillmentStatusId] = useState(
    defaultValues.fullFillmentStatusId,
  );
  const [productSkus, setProductSkus] = useState(defaultValues.productSKUs);

  const [stationId, setStationId] = useState([]);

  const belowSmScreen = useGetBreakPoint("sm");
  const handleFocus = (event) => event.target.select();
  //////filters
  ///
  const [storesForSelection, setStoresForSelection] = useState([]);

  const [allProductStocks, setAllProductStocks] = useState([]);
  const [selectedProductStocks, setSelectedProductStocks] = useState([]);
  const [allStationLookup, setAllStationLookup] = useState([]);
  const [isTabFilter, setIsTabFilter] = useState(false);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  //#region
  let getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    if (res.data.result != null) {
      setStoresForSelection(res.data.result);
    }
  };
  // const getAllFullFillmentStatusLookup = async () => {
  //   try {
  //     const response = await GetAllFullFillmentStatusLookup();
  //     setAllFullFillmentStatusLookup(response.data.result);
  //   } catch (error) {
  //     console.error("Error fetching GetAllOrderTypeLookup:", error.response);
  //   }
  // };

  const getProductStocksForSelection = async () => {
    try {
      const response = await GetProductStocksForSelection(0);
      setAllProductStocks(response.data.result);
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
  //#endregion
  useEffect(() => {
    getStoresForSelection();
    // getAllFullFillmentStatusLookup();
    getAllStationLookup();
    getProductStocksForSelection();
  }, []);
  const getFiltersFromState = () => {
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
      ProductStationIds:
        stationId.length > 0
          ? stationId.map((station) => station.productStationId).join(",")
          : "",
      ProductSKUs:
        selectedProductStocks.length > 0
          ? selectedProductStocks.map((product) => product.SKU).join(",")
          : "",
      TrackingStatusID:
        selectedCarrierStatus.length > 0
          ? selectedCarrierStatus
              ?.map((item) => item.carrierTrackingStatusId)
              ?.join(",")
          : "",
      saleChannelConfigIds:
        selectedSalePerson.length > 0
          ? selectedSalePerson?.map((item) => item.id)?.join(",")
          : "",
      StoreIds:
        selectedStoreIds.length > 0
          ? selectedStoreIds?.map((item) => item.storeId)?.join(",")
          : "",
      IsFulfilled: isCheckedFulfilledOnly,
      IsInTransit: isCheckedInTransit,
      regionIds:
        selectedRegion.length > 0
          ? selectedRegion?.map((item) => item.id)?.join(",")
          : "",
    };
    return filters;
  };
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
        length: 1000,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
      ProductStationIds:
        stationId.length > 0
          ? stationId.map((station) => station.productStationId).join(",")
          : "",
      ProductSKUs:
        selectedProductStocks.length > 0
          ? selectedProductStocks.map((product) => product.SKU).join(",")
          : "",
      TrackingStatusID:
        selectedCarrierStatus.length > 0
          ? selectedCarrierStatus
              ?.map((item) => item.carrierTrackingStatusId)
              ?.join(",")
          : "",
      saleChannelConfigIds:
        selectedSalePerson.length > 0
          ? selectedSalePerson?.map((item) => item.id)?.join(",")
          : "",
      StoreIds:
        selectedStoreIds.length > 0
          ? selectedStoreIds?.map((item) => item.storeId)?.join(",")
          : "",
      IsFulfilled: isCheckedFulfilledOnly,
      IsInTransit: isCheckedInTransit,
      regionIds:
        selectedRegion.length > 0
          ? selectedRegion?.map((item) => item.id)?.join(",")
          : "",
    };
    return filters;
  };
  useEffect(() => {
    if (selectedStoreIds.length > 0) {
      let storeIds =
        selectedStoreIds.length > 0
          ? selectedStoreIds?.map((item) => item.storeId)?.join(",")
          : "";
      getAllSaleChannelsByStoreId(storeIds);
    } else {
      setAllSalePersons([]);
    }
  }, [selectedStoreIds]);

  const getAllSaleChannelsByStoreId = async (storeIds) => {
    var result = await handleGetAllSaleChannelsByStoreId(storeIds);

    if (result) {
      setAllSalePersons(result);
    }
  };
  let getAllInventorySales = async () => {
    setGridLoading(true);
    let params = getFiltersFromState();
    let res = await GetAllInventorySales(params);
    if (res.data.result !== null) {
      setAlInventorySale(res.data.result);
    }
    setGridLoading(false);
    resetRowRef.current = false;
  };
  const handleFilterRest = () => {
    setStartDate(defaultValues.startDate);
    setEndDate(defaultValues.endDate);
    setFullFillmentStatusId(defaultValues.fullFillmentStatusId);
    setProductSkus([]);
    setStationId([]);
    setSelectedCarrierStatus([]);
    setSelectedSalePerson([]);
    setSelectedRegion([]);
    setIsCheckedFulfilledOnly(true);
    setIsCheckedInTransit(true);
  };
  const [isfilterClear, setIsfilterClear] = useState(false);
  const handleFilterClear = async () => {
    handleFilterRest();
    setIsfilterClear(true);
  };

  useEffect(() => {
    if (isfilterClear) {
      getAllInventorySales();
      resetDates();
      setIsfilterClear(false);
    }
  }, [isfilterClear]);
  useEffect(() => {
    getAllInventorySales();
  }, []);
  useEffect(() => {
    getAllInventorySales();
  }, [isCheckedFulfilledOnly, isCheckedInTransit]);

  /////actions

  const EnumDropDownAction = {
    EXCELEXPORT: "ExcelExport",
    PDFREPORT: "PDFReport",
    EXPORTEXCELREPORTSUMMARY: "ExportExcelReportSummary",
    EXPORTPDFREPORTSUMMARY: "ExportPDFReportSummary",
  };
  const [open, setOpen] = useState(false);
  const [openFulfillment, setOpenFulfillment] = useState(false);

  //#region action button region
  const handleActionUpdateDetail = async (type, actionValue) => {
    let selectedTrNos = getOrdersRef.current;

    if (actionValue === EnumDropDownAction.EXCELEXPORT) {
      downloadExcel();
      //export excel
    }
    if (actionValue === EnumDropDownAction.PDFREPORT) {
      //PDFREPORT
      downloadPDFInventorySales();
    }
    if (actionValue === EnumDropDownAction.EXPORTEXCELREPORTSUMMARY) {
      //EXPORTEXCELREPORTSUMMARY
      downloadExcelExportInventorySalesSummary();
    }
    if (actionValue === EnumDropDownAction.EXPORTPDFREPORTSUMMARY) {
      //EXPORTPDFREPORTSUMMARY
      downloadGetPDFInventorySalesSummary();
    }
  };

  const downloadExcel = () => {
    // if (selectedRows.length > 0) {
    let params = getFiltersFromStateForExport();
    ExcelExportInventorySales(params)
      .then((res) => {
        UtilityClass.downloadExcel(res.data, "inventorySale");
        setSelectedRows([]);
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
    // } else {
    //   errorNotification("Please select rows");
    // }
  };
  const downloadExcelExportInventorySalesSummary = () => {
    let params = getFiltersFromStateForExport();
    ExcelExportInventorySalesSummary(params)
      .then((res) => {
        UtilityClass.downloadExcel(res.data, "inventorySummary");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  const downloadPDFInventorySales = () => {
    let params = getFiltersFromStateForExport();
    GetPDFInventorySales(params)
      .then((res) => {
        UtilityClass.downloadPdf(res.data, "inventorySummary");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  const downloadGetPDFInventorySalesSummary = () => {
    let params = getFiltersFromStateForExport();
    GetPDFInventorySalesSummary(params)
      .then((res) => {
        UtilityClass.downloadPdf(res.data, "inventorySummary");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  //#endregion
  const handleOptionSelect = (event) => {
    const selectedValues = event.target.value;
    setStoreId(selectedValues);
  };

  const location = useLocation();
  useEffect(() => {
    if (location.pathname === "/inventory-sales") {
      // handleFilterClear();
    }
  }, [location.pathname]);
  useEffect(() => {
    if (isfilterClear) {
      getAllInventorySales();
      setIsfilterClear(false);
    }
  }, [isfilterClear]);

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <Autocomplete
          multiple
          sx={{ marginBottom: "5px", display: "none" }}
          id="tags-filled"
          variant="outlined"
          size="small"
          onFocus={handleFocus}
          onChange={(e, value) => {
            setInputFields(value.slice(0, MAX_TAGS));
          }}
          value={inputFields}
          options={[]}
          freeSolo
          renderTags={(value, getTagProps) => {
            return value
              .slice(0, MAX_TAGS)
              .map((option, index) => (
                <Chip
                  key={index}
                  variant="outlined"
                  size="small"
                  label={option}
                  {...getTagProps({ index })}
                />
              ));
          }}
          renderInput={(params) => (
            <TextField
              {...params}
              sx={{
                "& fieldset": { border: "none" },
                background: "#F8F8F8",
                boxShadow: 1,
              }}
              variant="outlined"
              size="small"
              placeholder="Search"
            />
          )}
        />
        <DataGridTabs
          tabsSmWidth={10}
          tabsMdWidth={10}
          tabData={[]}
          actionBtnMenuData={[
            // {
            //   title: "Stock Report PDF",
            //   onClick: () => {},
            // },
            {
              title: "Excel Report",
              onClick: () => {
                downloadExcel();
              },
            },
            {
              title: "PDF Report",
              onClick: () => {
                downloadPDFInventorySales();
              },
            },
            {
              title: "Export Excel Report Summary",
              onClick: () => {
                downloadExcelExportInventorySalesSummary();
              },
            },
            {
              title: "Export PDF Report Summary",
              onClick: () => {
                downloadGetPDFInventorySalesSummary();
              },
            },
          ]}
          otherBtns={
            <>
              {!belowSmScreen && (
                <>
                  <SwitchMui
                    checked={isCheckedFulfilledOnly}
                    onChange={() => setIsCheckedFulfilledOnly((prev) => !prev)}
                    label={
                      LanguageReducer?.languageType
                        ?.PRODUCT_INVENTORY_SALES_FULFILLED_ONLY
                    }
                  />
                  <SwitchMui
                    checked={isCheckedInTransit}
                    onChange={() => setIsCheckedInTransit((prev) => !prev)}
                    label={
                      LanguageReducer?.languageType
                        ?.PRODUCT_INVENTORY_SALES_IN_TRANSIT_ONLY
                    }
                  />
                </>
              )}
            </>
          }
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
        />
        {isFilterOpen && (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <TableRow>
                <Grid container spacing={1} sx={{ p: "15px" }}>
                  <Grid item lg={2} md={2} sm={12} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_START_DATE
                      }
                    </InputLabel>

                    <CustomReactDatePickerInputFilter
                      value={startDate}
                      onClick={(date) => setStartDate(date)}
                      size="small"
                      isClearable
                      maxDate={UtilityClass.todayDate()}
                    />
                  </Grid>
                  <Grid item lg={2} md={2} sm={12} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_END_DATE
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
                    />
                  </Grid>

                  <Grid item lg={2} md={2} sm={12} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_STATION
                      }
                    </InputLabel>
                    <SelectComponent
                      height={28}
                      disableClearable={true}
                      multiple={true}
                      name="station"
                      options={allStationLookup}
                      value={stationId}
                      optionLabel={EnumOptions.SELECT_STATION.LABEL}
                      optionValue={EnumOptions.SELECT_STATION.VALUE}
                      getOptionLabel={(option) => option?.sname}
                      onChange={(e, val) => {
                        setStationId(val);
                      }}
                    />
                  </Grid>
                  <Grid item lg={2} md={2} sm={12} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_CARRIER_STATUS
                      }
                    </InputLabel>
                    <SelectComponent
                      height={28}
                      disableClearable={true}
                      multiple={true}
                      name="status"
                      options={allCarrierStatus}
                      value={selectedCarrierStatus}
                      optionLabel={EnumOptions.CARRIER_TRACKING_STATUS.LABEL}
                      optionValue={EnumOptions.CARRIER_TRACKING_STATUS.VALUE}
                      getOptionLabel={(option) => option?.trackingStatus}
                      onChange={(e, val) => {
                        setSelectedCarrierStatus(val);
                      }}
                    />
                  </Grid>
                  <Grid item lg={2} md={2} sm={12} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_STORE
                      }
                    </InputLabel>
                    <SelectComponent
                      height={28}
                      disableClearable={true}
                      multiple={true}
                      name="store"
                      options={storesForSelection}
                      value={selectedStoreIds}
                      optionLabel={EnumOptions.STORE.LABEL}
                      optionValue={EnumOptions.STORE.VALUE}
                      getOptionLabel={(option) => option?.storeName}
                      onChange={(e, val) => {
                        setSelectedStoreIds(val);
                      }}
                    />
                  </Grid>
                  <Grid item lg={2} md={2} sm={12} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_SALES_PERSON
                      }
                    </InputLabel>
                    <SelectComponent
                      height={28}
                      disableClearable={true}
                      multiple={true}
                      name="salePerson"
                      options={allSalePersons}
                      value={selectedSalePerson}
                      optionLabel={EnumOptions.SALES_PERSON.LABEL}
                      optionValue={EnumOptions.SALES_PERSON.VALUE}
                      getOptionLabel={(option) => option?.text}
                      onChange={(e, val) => {
                        setSelectedSalePerson(val);
                      }}
                    />
                  </Grid>

                  <Grid item lg={2} md={2} sm={12} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_PRODUCTS
                      }
                    </InputLabel>
                    <SelectComponent
                      height={28}
                      multiple={true}
                      name="productStock"
                      options={allProductStocks}
                      value={selectedProductStocks}
                      optionLabel={EnumOptions.PRODUCTS.LABEL}
                      optionValue={EnumOptions.PRODUCTS.VALUE}
                      getOptionLabel={(option) => option?.SKUOption}
                      onChange={(e, val) => {
                        setSelectedProductStocks(val);
                      }}
                    />
                  </Grid>
                  <Grid
                    item
                    lg={4}
                    md={4}
                    sm={12}
                    xs={12}
                    alignSelf={"end"}
                    display={"flex"}
                    gap={1}
                  >
                    <Button
                      sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                      color="inherit"
                      variant="outlined"
                      onClick={() => {
                        handleFilterClear();
                      }}
                    >
                      {LanguageReducer?.languageType?.PRODUCTS_CLEAR_FILTER}
                    </Button>
                    <Button
                      sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                      variant="contained"
                      onClick={() => {
                        getAllInventorySales();
                      }}
                    >
                      {LanguageReducer?.languageType?.PRODUCT_FILTER}
                    </Button>
                  </Grid>
                  {belowSmScreen && (
                    <Grid item alignSelf={"end"} lg={2} md={2} sm={12} xs={12}>
                      <Stack direction={"row"} spacing={1}>
                        <SwitchMui
                          checked={isCheckedFulfilledOnly}
                          onChange={() =>
                            setIsCheckedFulfilledOnly((prev) => !prev)
                          }
                          label={
                            LanguageReducer?.languageType
                              ?.PRODUCT_INVENTORY_SALES_FULFILLED_ONLY
                          }
                        />
                        <SwitchMui
                          checked={isCheckedInTransit}
                          onChange={() =>
                            setIsCheckedInTransit((prev) => !prev)
                          }
                          label={
                            LanguageReducer?.languageType
                              ?.PRODUCT_INVENTORY_SALES_IN_TRANSIT_ONLY
                          }
                        />
                      </Stack>
                    </Grid>
                  )}
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        )}
        <Routes>
          <Route
            path="/"
            element={
              <InventoryList
                loading={gridLoading}
                alInventorySale={alInventorySale}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                selectedRows={selectedRows}
                setSelectedRows={setSelectedRows}
                isFilterOpen={isFilterOpen}
              />
            }
          />
        </Routes>
      </div>
    </Box>
  );
}
export default InventorySalePage;
