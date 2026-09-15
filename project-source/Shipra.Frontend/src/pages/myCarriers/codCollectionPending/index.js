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
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  ExcelExportCODCollectionPendingsMyCarrier,
  GetActiveCarriersForSelection,
  GetAllCODCollectionPendingsMyCarrier,
  GetAllOrderTypeLookup,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import CreateSettlementModelWithSelection from "../../../components/modals/accountModals/CreateSettlementModelWithSelection";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import initialStateFilter from "../../../utilities/filterState";
import {
  errorNotification,
  successNotification,
  warningNotification,
} from "../../../utilities/toast";
import CODCollectionPendingList from "./list";
const EnumOrderDashboardAction = {
  EXCELEXPORT: "ExcelExport",
};

function CODCollectionPending(props) {
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [storeId, setStoreId] = useState(initialStateFilter.store);
  const [orderTypeId, setOrderTypeId] = useState(initialStateFilter.orderType);
  const [carrierIds, setCarrierIds] = useState("");
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [allCodCollectionPending, setAllCodCollectionPending] = useState([]);
  const [carrier, setCarrier] = useState(initialStateFilter.carrier);
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
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 10,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      storeId: storeId?.storeId,
      orderTypeId: orderTypeId?.orderTypeId,
    };

    return filters;
  };
  const [isfilterClear, setIsfilterClear] = useState(false);

  const handleFilterClear = async () => {
    handleFilterRest();
    setIsfilterClear(true);
  };
  // handleGetAllCODCollectionPendingsMyCarrier
  const getAllCodPendingMyCarrier = async () => {
    let params = getFiltersFromState();
    setIsGridLoading(true);
    await GetAllCODCollectionPendingsMyCarrier(params)
      .then((res) => {
        const response = res.data;
        if (response.result) {
          setCarrier({
            CarrierId: response.result?.list[0].CarrierId,
            Name: "Default",
          });
          setAllCodCollectionPending(response.result);
        }
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setIsGridLoading(false);
      });
  };
  useEffect(() => {
    getAllCodPendingMyCarrier();
  }, []);

  useEffect(() => {
    if (isFilterReset) {
      getAllCodPendingMyCarrier();
      setIsFilterReset(false);
    }
  }, [isFilterReset]);
  const handleFilterRest = () => {
    resetDates();
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
    ExcelExportCODCollectionPendingsMyCarrier(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadExcel(res.data, "orders");
          // setIsLoading(false);
        } else {
          successNotification(
            LanguageReducer?.languageType
              ?.MY_CARRIER_COD_PENDING_ORDER_NOT_FOUND
          );
          // setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_COD_PENDING_UNABLE_TO_DOWNLOAD
        );
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
    getAllActiveCarriersForSelection();
  }, []);
  const [selectedItems, setSelectedItems] = useState([]);
  const [openCreateSettle, setOpenCreateSettleOpen] = useState(false);
  const handleCreateSettlement = () => {
    if (selectedItems && selectedItems.length > 0) {
      setOpenCreateSettleOpen(true);
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    }
  };
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <DataGridTabs
          tabsMdWidth={"10px"}
          tabsSmWidth={"10px"}
          otherBtns={
            <>
              <ButtonComponent
                title={
                  LanguageReducer?.languageType
                    ?.MY_CARRIER_COD_PENDING_CREATE_SETTLEMENT
                }
                onClick={() => handleCreateSettlement(true)}
              />
            </>
          }
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
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
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_COD_PENDING_START_DATE
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
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_COD_PENDING_END_DATE
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
                        {LanguageReducer?.languageType?.ORDER_CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllCodPendingMyCarrier();
                        }}
                      >
                        {LanguageReducer?.languageType?.ORDERS_FILTER}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
                {/* <Grid sx={{ display: "flex", justifyContent: "end" }}>
                  <Grid sx={{ p: "6px" }}>
                    <Stack alignItems="flex-end" direction="row" spacing={1}>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          handleFilterClear();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER_TEXT}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllDeliveryTask();
                        }}
                      >
                        {"Filter"}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid> */}
              </TableRow>
            </TableHead>
          </Table>
        ) : null}
        <CODCollectionPendingList
          rows={allCodCollectionPending}
          getOrdersRef={getOrdersRef}
          resetRowRef={resetRowRef}
          loading={isGridLoading}
          getAll={getAllCodPendingMyCarrier}
          setSelectedItems={setSelectedItems}
          isFilterOpen={isFilterOpen}
        />
        {openCreateSettle && (
          <CreateSettlementModelWithSelection
            open={openCreateSettle}
            setOpen={setOpenCreateSettleOpen}
            orderNosData={selectedItems}
            getAllCODPendings={getAllCodPendingMyCarrier}
            allCODPendings={allCodCollectionPending?.list}
            carrierId={carrier?.CarrierId}
            resetRowRef={resetRowRef}
          />
        )}
      </div>
    </Box>
  );
}
export default CODCollectionPending;
