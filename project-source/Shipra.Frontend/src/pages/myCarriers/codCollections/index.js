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
import { useSelector } from "react-redux";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  ExcelExportCODCollectionsMyCarrierQuery,
  GetActiveCarriersForSelection,
  GetAllCODCollectionsMyCarrier,
  GetAllOrderTypeLookup,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import GeneralTabBar from "../../../components/shared/tabsBar";
import { EnumOptions } from "../../../utilities/enum";
import initialStateFilter from "../../../utilities/filterState";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import CODCollectionMyCarrierList from "./list";

const EnumOrderDashboardAction = {
  EXCELEXPORT: "ExcelExport",
};
function MyTaskCODCollections(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [storeId, setStoreId] = useState(initialStateFilter.store);
  const [orderTypeId, setOrderTypeId] = useState(initialStateFilter.orderType);
  const [carrierIds, setCarrierIds] = useState("");
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [allCodCollectionPending, setAllCodCollectionPending] = useState([]);
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [isGridLoading, setIsGridLoading] = useState(false);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const [inputFields, setInputFields] = useState([]);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allOrderTypeLookup, setAllOrderTypeLookup] = useState([]);
  const [allActiveCarriersForSelection, setAllActiveCarriersForSelection] =
    useState([]);
  const getFiltersFromState = () => {
    let search = inputFields.join();
    let filters = {
      filterModel: {
        createdFrom: startDate,
        createdTo: endDate,
        start: 0,
        length: 10,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      storeId: storeId?.storeId,
      carrierIds: carrierIds,
      orderTypeId: orderTypeId?.orderTypeId,
    };

    return filters;
  };
  // handleGetAllCODCollectionsMyCarrier
  const handleGetAllCODCollectionsMyCarrier = async () => {
    let params = getFiltersFromState();

    setIsGridLoading(true);
    await GetAllCODCollectionsMyCarrier(params)
      .then((res) => {
        const response = res.data;
        if (response.result) {
          setAllCodCollectionPending(response.result);
        }
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setIsGridLoading(false);
      });
  };
  useEffect(() => {
    handleGetAllCODCollectionsMyCarrier();
  }, []);

  useEffect(() => {
    if (isFilterReset) {
      handleGetAllCODCollectionsMyCarrier();
    }
  }, [isFilterReset]);
  const handleFilterRest = () => {
    setStartDate(null);
    setEndDate(null);
    setOrderTypeId(initialStateFilter.orderType);
    setStoreId(initialStateFilter.store);
    setIsFilterReset(true);
  };
  const handleActionUpdateDetail = async (type, actionValue) => {
    let selectedTrNos = getOrdersRef.current;
    // if (selectedTrNos.length > 0) {
    //   if (actionValue === EnumOrderDashboardAction.ASSIGNTOCARRIER) {
    //   }
    // } else {
    //   warningNotification(
    //     LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
    //   );
    // }

    //for export
    if (actionValue === EnumOrderDashboardAction.EXCELEXPORT) {
      //export excel api
      await downloadExcel();
    }
  };
  const downloadExcel = () => {
    let params = getFiltersFromState();
    ExcelExportCODCollectionsMyCarrierQuery(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadExcel(res.data, "orders");
          // setIsLoading(false);
        } else {
          successNotification("Order not found");
          // setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
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
  const getAllActiveCarriersForSelection = async () => {
    try {
      const response = await GetActiveCarriersForSelection();
      setAllActiveCarriersForSelection(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  useEffect(() => {
    getStoresForSelection();
    getAllOrderTypeLookup();
  }, []);
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <GeneralTabBar
          isFilterOpen={isFilterOpen}
          setIsFilterOpen={setIsFilterOpen}
          tabData={[]}
          {...props}
          disableFilter
          tabScreen="orders-dashboard"
          options={[
            {
              title: "Excel Export",
              value: EnumOrderDashboardAction.EXCELEXPORT,
            },
          ]}
          placeholder="Action"
          onChangeMenu={(value) => handleActionUpdateDetail("type", value)}
          placement={"bottom"}
          // disableSearch
          // width="auto"
        />
        {isFilterOpen ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <TableRow>
                <Grid container spacing={2} sx={{ p: "6px" }}>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {"Start Date"}
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
                        {"End Date"}
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
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_COD_PENDING_STORE
                        }
                      </InputLabel>
                      <SelectComponent
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
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_COD_PENDING_ORDER_TYPE
                        }
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={allOrderTypeLookup}
                        value={orderTypeId}
                        optionLabel={EnumOptions.ORDER_TYPE.LABEL}
                        optionValue={EnumOptions.ORDER_TYPE.VALUE}
                        getOptionLabel={(option) => option?.orderTypeName}
                        onChange={(e, val) => {
                          setOrderTypeId(val);
                        }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Stack
                      alignItems="flex-end"
                      direction="row"
                      spacing={1}
                      sx={{ ...styleSheet.filterButtonMargin, display: "row" }}
                    >
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          handleFilterRest();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER_TEXT}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          handleGetAllCODCollectionsMyCarrier();
                        }}
                      >
                        {"Filter"}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}
        <CODCollectionMyCarrierList
          rows={allCodCollectionPending}
          getOrdersRef={getOrdersRef}
          resetRowRef={resetRowRef}
          loading={isGridLoading}
        />
      </div>
    </Box>
  );
}
export default MyTaskCODCollections;
