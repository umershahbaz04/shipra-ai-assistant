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
  Typography,
} from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import AccountBalanceIcon from "@mui/icons-material/AccountBalance";
import RequestPageIcon from "@mui/icons-material/RequestPage";
import CreditScoreIcon from "@mui/icons-material/CreditScore";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetActiveCarriersForSelection,
  GetAllCarrierWithCodPending,
  GetAllCODPendings,
  GetAllOrderTypeLookup,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import CreateSettlementModelWithSelection from "../../../components/modals/accountModals/CreateSettlementModelWithSelection";
import CreateSettlementModal from "../../../components/modals/accountModals/createSettlementModal";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import initialStateFilter from "../../../utilities/filterState";
import { warningNotification } from "../../../utilities/toast";
import CODPendingList from "./codPendingList";
import {
  CustomColorLabelledOutline,
  greyBorder,
} from "../../../utilities/helpers/Helpers";
import wareHouseImage from "../../../assets/images/WareHouse.png";
import SearchInputAutoCompleteMultiple from "../../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";

function CODPending(props) {
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const [allCODPendings, setAllCODPendings] = useState([]);
  const [open, setOpen] = useState(false);
  const [openCreateSettle, setOpenCreateSettleOpen] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isGridLoading, setIsGridLoading] = useState(false);
  const [carrier, setCarrier] = useState(initialStateFilter.carrier);
  const [allActiveCarriersForSelection, setAllActiveCarriersForSelection] =
    useState([]);
  const [storeId, setStoreId] = useState(initialStateFilter.stores);
  const [orderTypeId, setOrderTypeId] = useState(initialStateFilter.orderType);
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allOrderTypeLookup, setAllOrderTypeLookup] = useState([]);
  const [selectedItems, setSelectedItems] = useState([]);
  const [carrierWithCodPending, setCarrierWithCodPending] = useState([]);
  const [inputFields, setInputFields] = useState([]);
  const MAX_TAGS = 200;
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  let getAllCODPendings = async (searchValue = "") => {
    setIsGridLoading(true);
    let res = await GetAllCODPendings({
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 10000,
        search: searchValue,
        sortDir: "desc",
        sortCol: 0,
      },
      storeId: UtilityClass.convertArrIntoCommaSeperatedIds(storeId, "storeId"),
      carrierIds: carrier?.CarrierId?.toString(),
      orderTypeId: orderTypeId?.orderTypeId,
    });
    setIsGridLoading(false);

    if (res.data.result !== null) setAllCODPendings(res.data.result.list);
  };
  const getAllActiveCarriersForSelection = async () => {
    try {
      const response = await GetActiveCarriersForSelection();
      // console.log("response of api of GetAllOrderTypeLookup", response);
      setAllActiveCarriersForSelection(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const getAllCarrierWithCodPending = async () => {
    try {
      const response = await GetAllCarrierWithCodPending();
      setCarrierWithCodPending(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  const handleFilterRest = () => {
    resetDates();
    setOrderTypeId(initialStateFilter.orderType);
    setStoreId(initialStateFilter.stores);
    setCarrier(initialStateFilter.carrier);
    setIsFilterReset(true);
  };
  useEffect(() => {
    getAllActiveCarriersForSelection();
    getAllCarrierWithCodPending();
  }, []);
  useEffect(() => {
    getAllCODPendings();
  }, [carrier]);
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

  useEffect(() => {
    getStoresForSelection();
    getAllOrderTypeLookup();
  }, []);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleCreateSettlement = () => {
    if (selectedItems && selectedItems.length > 0) {
      if (!carrier?.carrierId || carrier?.carrierId == 0) {
        warningNotification(
          LanguageReducer?.languageType
            ?.ACCOUNTS_COD_PENDING_PLEASE_SELECT_THE_CARRIER_FROM_FILTER
        );
      } else {
        setOpenCreateSettleOpen(true);
      }
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    }
  };
  return (
    <>
      <Box display="flex" gap={2} p={"10px"} pb={0}>
        <CustomColorLabelledOutline
          isCollapse={true}
          label={"Carrier COD"}
          discription={
            <>
              You can filter carriers using the{" "}
              <a
                href="/generic-setting"
                style={{ color: "#007bff", textDecoration: "underline" }}
              >
                generic settings
              </a>{" "}
              configuration.
            </>
          }
        >
          <Grid container spacing={2} alignItems="center">
            {carrierWithCodPending.map((dt, index) => (
              <Grid item xs={12} sm={6} md={3} key={index}>
                <Card
                  sx={{
                    bgcolor: "#f5f5f5",
                    border: greyBorder,
                    boxShadow: "none",
                    borderRadius: 2,
                  }}
                >
                  <CardContent
                    sx={{ padding: 1, paddingBottom: "8px !important" }}
                  >
                    <Box display="flex" alignItems="center">
                      <Avatar
                        variant="rounded"
                        src={
                          dt?.CarrierImage === ""
                            ? wareHouseImage
                            : dt.CarrierImage
                        }
                        alt={dt?.CarrierName}
                        sx={{
                          width: 50,
                          height: 50,
                          bgcolor: "#fff",
                          borderRadius: "8px",
                          p: 0.5,
                          "& img": {
                            objectFit: "contain !important",
                          },
                        }}
                      />
                      <Box ml={2}>
                        <Typography variant="body2" color="textSecondary">
                          {dt.Name}
                        </Typography>
                        <Typography
                          variant="h5"
                          component="div"
                          fontWeight="bold"
                        >
                          {dt.TotalAmount} AED
                        </Typography>
                      </Box>
                    </Box>
                    <Typography variant="body2" color="textSecondary">
                      {""}
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </CustomColorLabelledOutline>
      </Box>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <Grid
            item
            xl={12}
            lg={12}
            md={12}
            sm={12}
            xs={12}
            p={1}
            sx={{
              background: "#f8f8f8",
              border: "1px solid rgba(0, 0, 0, 0.12)",
              borderRadius: "10px!important",
              borderBottomLeftRadius: "0px !important",
              borderBottomRightRadius: "0px !important",
            }}
          >
            <Box flexGrow={1}>
              <SearchInputAutoCompleteMultiple
                onChange={(e, value) => {
                  if (value.length <= MAX_TAGS) {
                    const limitedValues = value.slice(0, MAX_TAGS);
                    setInputFields(limitedValues);
                    getAllCODPendings(String(limitedValues));
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
          </Grid>
          <DataGridTabs
            customBorderRadius="0px!impotant"
            tabsSmWidth={"1px"}
            tabsMdWidth={"1px"}
            otherBtns={
              <>
                <ButtonComponent
                  p={"8px"}
                  title={
                    LanguageReducer?.languageType
                      ?.ACCOUNTS_COD_PENDING_CREATE_SETTLEMENT
                  }
                  onClick={() => handleCreateSettlement()}
                />
                <ButtonComponent
                  p={"8px"}
                  title={
                    LanguageReducer?.languageType
                      ?.ACCOUNTS_COD_PENDING_CREATE_SETTLEMENT_FROM_FILE
                  }
                  onClick={() => setOpen(true)}
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
                  <Grid container spacing={1} sx={{ p: "15px" }}>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {
                            LanguageReducer?.languageType
                              ?.ACCOUNTS_COD_PENDING_START_DATE
                          }
                        </InputLabel>

                        <CustomReactDatePickerInputFilter
                          maxDate={UtilityClass.todayDate()}
                          value={startDate}
                          onClick={(date) => setStartDate(date)}
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
                          {
                            LanguageReducer?.languageType
                              ?.ACCOUNTS_COD_PENDING_END_DATE
                          }
                        </InputLabel>
                        <CustomReactDatePickerInputFilter
                          maxDate={UtilityClass.todayDate()}
                          value={endDate}
                          onClick={(date) => setEndDate(date)}
                          size="small"
                          minDate={startDate}
                          disabled={!startDate ? true : false}
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
                          {
                            LanguageReducer?.languageType
                              ?.ACCOUNTS_COD_PENDING_STORE
                          }
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
                      <Grid>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {
                            LanguageReducer?.languageType
                              ?.ACCOUNTS_COD_PENDING_ORDER_TYPE
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
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.ACCOUNTS_COD_PENDING_CARRIER
                        }
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={allActiveCarriersForSelection}
                        value={carrier}
                        optionLabel={EnumOptions.CARRIER.LABEL}
                        optionValue={EnumOptions.CARRIER.VALUE}
                        getOptionLabel={(option) => option.Name}
                        onChange={(e, val) => {
                          setCarrier(val);
                        }}
                      />
                    </Grid>
                    <Grid item xs={12}>
                      <Stack
                        justifyContent={"end"}
                        alignItems="flex-end"
                        direction="row"
                        spacing={1}
                        sx={{
                          ...styleSheet.filterButtonMargin,
                          display: "row",
                        }}
                      >
                        <Button
                          sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                          color="inherit"
                          variant="outlined"
                          onClick={() => {
                            handleFilterRest();
                          }}
                        >
                          {LanguageReducer?.languageType?.CLEAR_FILTER}
                        </Button>
                        <Button
                          sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                          variant="contained"
                          onClick={() => {
                            getAllCODPendings();
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
          ) : null}{" "}
          <CODPendingList
            loading={isGridLoading}
            allCODPendings={allCODPendings}
            getOrdersRef={getOrdersRef}
            getAllCODPendings={getAllCODPendings}
            resetRowRef={resetRowRef}
            setSelectedItems={setSelectedItems}
            isFilterOpen={isFilterOpen}
          />
        </div>
        {open && (
          <CreateSettlementModal
            getAllCODPendings={getAllCODPendings}
            open={open}
            setOpen={setOpen}
            carrierId={carrier?.carrierId}
          />
        )}
        {openCreateSettle && (
          <CreateSettlementModelWithSelection
            open={openCreateSettle}
            setOpen={setOpenCreateSettleOpen}
            getAllCODPendings={getAllCODPendings}
            orderNosData={selectedItems}
            allCODPendings={allCODPendings}
            carrierId={carrier?.carrierId}
            resetRowRef={resetRowRef}
          />
        )}
      </Box>
    </>
  );
}
export default CODPending;
