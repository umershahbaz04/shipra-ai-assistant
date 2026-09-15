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
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import { Route, Routes } from "react-router-dom";
import { styleSheet } from "../../../assets/styles/style";
import ReturnOrderList from "./list";
import { fetchMethod } from "../../../utilities/helpers/Helpers";
import {
  GetAllClientReturnReasonForSelection,
  GetAllOrderReturn,
  GetAllReturnStatusForSelection,
} from "../../../api/AxiosInterceptors";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import UtilityClass from "../../../utilities/UtilityClass";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { EnumOptions } from "../../../utilities/enum";

const ReturnOrderPage = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isShowFilter, setIsShowFilter] = useState(true);
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [allOrderReturnData, setAllOrderReturnData] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [allReturnStatus, setAllReturnStatus] = useState([]);
  const [selectedReasonId, setSelectedReasonId] = useState();
  const [returnReasonForSelection, setReturnReasonForSelection] = useState([]);
  const [tabData, setTabData] = useState([]);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const [loading, setLoading] = useState(true);
  const [selectedTab, setSelectedTab] = useState(0);
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();

  const resetFilter = () => {
    resetDates();
    setSelectedReasonId(null);
  };

  const handleFilterClear = async () => {
    setIsfilterClear(true);
    resetFilter();
    getAllOrderReturn();
  };
  const handleFilter = () => {
    getAllOrderReturn();
  };

  const getAllClientReturnReasonForSelection = async () => {
    const { response } = await fetchMethod(() =>
      GetAllClientReturnReasonForSelection()
    );
    if (response?.result) {
      setReturnReasonForSelection(response?.result);
    }
  };

  const getAllReturnStatusForSelection = async () => {
    try {
      const { response } = await fetchMethod(GetAllReturnStatusForSelection);
      if (response?.result) {
        const data = response.result;
        const filterData = data.filter(
          (data) => data.returnStatusLookupId !== 0
        );
        const tabData = filterData.map((data) => ({
          label: data.returnStatus,
          value: data?.returnStatusLookupId,
        }));

        setTabData([
          {
            label: "All",
            value: 0,
          },
          ...tabData,
        ]);
        setAllReturnStatus(filterData);
      } else {
        console.log("No data found");
      }
    } catch (error) {
      console.error("Error fetching order returns:", error);
    } finally {
      setLoading(false);
    }
  };

  const getAllOrderReturn = async (StatusID) => {
    setIsLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetAllOrderReturn(
          startDate,
          endDate,
          selectedReasonId?.clientReturnReasonId,
          StatusID
        )
      );
      if (response?.result?.list) {
        setAllOrderReturnData(response.result.list);
      } else {
        console.log("No data found");
      }
    } catch (error) {
      console.error("Error fetching order returns:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleTabChange = async (event, filterValue) => {
    setSelectedTab(filterValue);
    getAllOrderReturn(filterValue);
  };

  useEffect(() => {
    getAllOrderReturn();
    getAllReturnStatusForSelection();
    getAllClientReturnReasonForSelection();
  }, []);

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <DataGridTabs
          handleTabChange={handleTabChange}
          selectedTab={selectedTab}
          loading={loading}
          tabData={tabData}
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
          isRouteChangeable={false}
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
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {LanguageReducer?.languageType?.ORDER_START_DATE}
                    </InputLabel>

                    <CustomReactDatePickerInputFilter
                      value={startDate}
                      onClick={(date) => setStartDate(date)}
                      size="small"
                      isClearable
                      maxDate={UtilityClass.todayDate()}
                    />
                  </Grid>

                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {LanguageReducer?.languageType?.ORDER_END_DATE}
                    </InputLabel>
                    <CustomReactDatePickerInputFilter
                      value={endDate}
                      onClick={(date) => setEndDate(date)}
                      size="small"
                      minDate={startDate}
                      disabled={!startDate}
                      isClearable
                      maxDate={UtilityClass.todayDate()}
                    />
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <InputLabel
                      required
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {LanguageReducer?.languageType?.ORDER_RETURN_REASON}
                    </InputLabel>
                    <SelectComponent
                      name="reason"
                      height={28}
                      options={returnReasonForSelection}
                      value={selectedReasonId}
                      optionLabel={EnumOptions.RETURN_REASON.LABEL}
                      optionValue={EnumOptions.RETURN_REASON.VALUE}
                      onChange={(e, val) => {
                        setSelectedReasonId(val);
                      }}
                    />
                  </Grid>

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
        <ReturnOrderList
          getOrdersRef={getOrdersRef}
          resetRowRef={resetRowRef}
          isLoading={isLoading}
          getAllOrderReturn={getAllOrderReturn}
          allOrderReturnData={allOrderReturnData}
          isFilterOpen={isFilterOpen}
        />
      </div>
    </Box>
  );
};

export default ReturnOrderPage;
