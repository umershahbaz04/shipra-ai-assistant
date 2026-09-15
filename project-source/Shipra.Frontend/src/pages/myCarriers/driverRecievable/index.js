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
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  GetAllDriverReceivable,
  GetDriversForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import GeneralTabBar from "../../../components/shared/tabsBar";
import UtilityClass from "../../../utilities/UtilityClass";
import { warningNotification } from "../../../utilities/toast";
import DriverRecievableList from "./driverRecievableList";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";

const EnumTabFilter = Object.freeze({
  All: "/delivery-tasks",
  Unassigned: "/delivery-tasks/unassigned",
  Assigned: "/delivery-tasks/assigned",
});
const EnumDashboardAction = {
  BATCHOUTSCAN: "BatchOutScan",
};
function DriverReciveablePage(props) {
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allRecieveable, setAllRecieveable] = useState([]);
  const [isAllListLoading, setIsAllListLoading] = useState([]);
  const [open, setOpen] = useState(false);
  const [allDrivers, setAllDrivers] = useState([]);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);

  const { register, reset, control } = useForm({
    defaultValues: { startDate: null, endDate: null },
  });
  let defaultValues = {
    startDate: null,
    endDate: null,
    driverAssignedStatus: 0,
  };
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  let getDriversForSelection = async () => {
    let res = await GetDriversForSelection();
    if (res.data.result != null) {
      setAllDrivers(res.data.result);
    }
  };

  const getFiltersFromState = () => {
    let search = "";
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
      driverAssignedStatus: driverAssignedStatus,
    };
    return filters;
  };
  let getAllDriverReceivable = async () => {
    setIsAllListLoading(true);
    let params = getFiltersFromState();
    let res = await GetAllDriverReceivable(params);
    if (res.data.result !== null) {
      setAllRecieveable(res.data.result);
    }
    setIsAllListLoading(false);
    resetRowRef.current = false;
  };
  useEffect(() => {
    getAllDriverReceivable();
  }, []);

  const [isfilterClear, setIsfilterClear] = useState(false);
  const [isTabFilter, setIsTabFilter] = useState(false);
  const [isShowFilter, setIsShowFilter] = useState(false);
  const [driverAssignedStatus, setDriverAssignedStatus] = useState(
    defaultValues.driverAssignedStatus
  );

  useEffect(() => {
    if (isfilterClear) {
      getAllDriverReceivable();
      setIsfilterClear(false);
    }
  }, [isfilterClear]);

  const handleActionUpdateDetail = async (type, actionValue) => {
    let selectedTrNos = getOrdersRef.current;
    console.log(selectedTrNos);
    if (selectedTrNos.length > 0) {
      if (actionValue === EnumDashboardAction.BATCHOUTSCAN) {
        setOpen(true);
      }
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    }
  };
  const handleFilterRest = () => {
    setStartDate(defaultValues.startDate);
    setEndDate(defaultValues.endDate);
    setIsfilterClear(true);
  };

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <GeneralTabBar
          isFilterOpen={isFilterOpen}
          setIsFilterOpen={setIsFilterOpen}
          tabData={[]}
          {...props}
          disableSearch
          placeholder="Action"
          onChangeMenu={(value) => handleActionUpdateDetail("type", value)}
          placement={"bottom"}
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
                <Grid container spacing={2} sx={{ p: "10px 8px" }}>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
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
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
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
                          handleFilterRest();
                        }}
                      >
                        {LanguageReducer?.languageType?.ORDER_CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllDriverReceivable();
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
        <DriverRecievableList
          loading={isAllListLoading}
          allRecieveable={allRecieveable}
          getOrdersRef={getOrdersRef}
          resetRowRef={resetRowRef}
          getAllDriverReceivable={getAllDriverReceivable}
          isFilterOpen={isFilterOpen}
        />
      </div>
    </Box>
  );
}
export default DriverReciveablePage;
