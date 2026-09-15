import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
} from "@mui/material";

import React, { useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  ExcelExportDriverExpense,
  GetAllDriverExpense,
  GetAllExpenseCategoryLookup,
  GetDriversForSelection,
  GetPDFDriverExpenseReport,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import GeneralTabBar from "../../../components/shared/tabsBar";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import { errorNotification } from "../../../utilities/toast";
import DriverExpenseList from "./driverExpenseList";

const EnumTabFilter = Object.freeze({
  All: "/delivery-tasks",
  Unassigned: "/delivery-tasks/unassigned",
  Assigned: "/delivery-tasks/assigned",
});
const EnumDashboardAction = {
  EXCELEXPORT: "ExcelExport",
  PDFREPORT: "PdfReport",
};
function DriverExpensePage(props) {
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allDriverExpense, setAllDriverExpense] = useState([]);
  const [isAllListLoading, setIsAllListLoading] = useState([]);
  const [open, setOpen] = useState(false);
  const [allDrivers, setAllDrivers] = useState([]);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);

  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  const { register, reset, control } = useForm({
    defaultValues: { startDate: null, endDate: null },
  });
  let defaultValues = {
    startDate: null,
    endDate: null,
    driverAssignedStatus: 0,
  };
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

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
      driverId: driver?.DriverId,
      expenseCategoryId: expenseCategory?.id,
    };
    return filters;
  };
  let getAllDriverExpense = async () => {
    setIsAllListLoading(true);
    let params = getFiltersFromState();
    let res = await GetAllDriverExpense(params);
    if (res.data.result !== null) {
      setAllDriverExpense(res.data.result);
    }
    setIsAllListLoading(false);
    resetRowRef.current = false;
  };
  useEffect(() => {
    getDriversForSelection();
    getAllDriverExpense();
  }, []);

  const [isfilterClear, setIsfilterClear] = useState(false);
  const [isTabFilter, setIsTabFilter] = useState(false);
  const [isShowFilter, setIsShowFilter] = useState(false);
  const defaultState = {
    driver: {
      DriverId: 0,
      DriverName: "Select Please",
    },
    expenseCategory: {
      id: 0,
      text: "Select Please",
    },
  };

  const [driver, setDriver] = useState(defaultState.driver);
  const [expenseCategory, setExpenseCategory] = useState(
    defaultState.expenseCategory
  );
  const [allExpenseCategory, setAllExpenseCategory] = useState([]);

  const [driverAssignedStatus, setDriverAssignedStatus] = useState(
    defaultValues.driverAssignedStatus
  );

  useEffect(() => {
    if (isfilterClear) {
      getAllDriverExpense();
      resetDates();
      setIsfilterClear(false);
    }
  }, [isfilterClear]);

  const handleActionUpdateDetail = async (type, actionValue) => {
    let selectedTrNos = getOrdersRef.current;
    console.log(selectedTrNos);
    if (actionValue === EnumDashboardAction.EXCELEXPORT) {
      await downloadExcel();
    }
    if (actionValue === EnumDashboardAction.PDFREPORT) {
      await downloadPdf();
    }
  };
  const handleFilterRest = () => {
    setStartDate(defaultValues.startDate);
    setEndDate(defaultValues.endDate);
    setDriver(defaultState.driver);
    setExpenseCategory(defaultState.expenseCategory);

    setIsfilterClear(true);
  };
  const getAllExpenseCategoryLookup = async () => {
    let res = await GetAllExpenseCategoryLookup();
    if (res.data.result !== null) {
      setAllExpenseCategory(res.data.result);
    }
  };
  useEffect(() => {
    getAllExpenseCategoryLookup();
  }, []);
  const downloadPdf = () => {
    let params = getFiltersFromState();
    GetPDFDriverExpenseReport(params)
      .then((res) => {
        UtilityClass.downloadPdf(res.data, "driverExpense");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_EXPENSES_TASKS_UNABLE_TO_DOWNLOAD
        );
      });
  };
  const downloadExcel = () => {
    let params = getFiltersFromState();
    ExcelExportDriverExpense(params)
      .then((res) => {
        UtilityClass.downloadExcel(res.data, "driverExpense");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_EXPENSES_TASKS_UNABLE_TO_DOWNLOAD
        );
      });
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
          options={[
            {
              title:
                LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_PDF_REPORT,
              value: EnumDashboardAction.PDFREPORT,
            },
            {
              title:
                LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_EXCEL_EXPORT,
              value: EnumDashboardAction.EXCELEXPORT,
            },
          ]}
          placeholder={
            LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_TASKS_ACTION
          }
          placement={"bottom"}
          onChangeMenu={(value) => handleActionUpdateDetail("type", value)}
          // width="auto"
        />
        {isFilterOpen ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <Stack direction={"column"} m={1}>
                <Grid
                  container
                  direction={{ xs: "column", sm: "row" }}
                  spacing={1}
                  sx={{ p: "10px 0px" }}
                >
                  <Grid item sm={12} xs={12} md={2} lg={2}>
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

                      // inputProps={{ style: { padding: "3px 5px" } }}
                    />
                  </Grid>
                  <Grid item sm={12} xs={12} md={2} lg={2}>
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
                      width="180px"
                      size="small"
                      minDate={startDate}
                      disabled={!startDate ? true : false}
                      isClearable
                      maxDate={UtilityClass.todayDate()}

                      // inputProps={{ style: { padding: "3px 5px" } }}
                    />
                  </Grid>
                  <Grid item sm={12} xs={12} md={2} lg={2}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.MY_CARRIER_EXPENSES_DRIVER
                      }
                    </InputLabel>
                    <SelectComponent
                      name="reason"
                      options={allDrivers}
                      height={28}
                      value={driver}
                      optionLabel={EnumOptions.DRIVER.LABEL}
                      optionValue={EnumOptions.DRIVER.VALUE}
                      getOptionLabel={(option) => option?.DriverName}
                      onChange={(e, val) => {
                        let selectedValue = val ? val : defaultState.driver;
                        setDriver(selectedValue);
                      }}
                    />
                  </Grid>
                  <Grid item sm={12} xs={12} md={2} lg={2}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.MY_CARRIER_EXPENSES_EXPENSE_TYPE
                      }
                    </InputLabel>
                    <SelectComponent
                      name="reason"
                      options={allExpenseCategory}
                      height={28}
                      value={expenseCategory}
                      optionLabel={EnumOptions.EXPENSE_TYPE.LABEL}
                      optionValue={EnumOptions.EXPENSE_TYPE.VALUE}
                      getOptionLabel={(option) => option?.text}
                      onChange={(e, val) => {
                        let selectedValue = val
                          ? val
                          : defaultState.expenseCategory;
                        setExpenseCategory(selectedValue);
                      }}
                    />
                  </Grid>
                  <Grid item sm={12} xs={12} md={2} lg={2}>
                    <Stack
                      sx={{ ...styleSheet.filterButtonMargin, height: "100%" }}
                      direction={"row"}
                      alignItems={"flex-end"}
                    >
                      <Button
                        sx={{
                          ...styleSheet.filterIcon,
                          minWidth: "100px",
                          marginLeft: "5px",
                        }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          handleFilterRest();
                        }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_EXPENSES_CLEAR_FILTER
                        }
                      </Button>

                      <Button
                        sx={{
                          ...styleSheet.filterIcon,
                          minWidth: "100px",
                          marginLeft: "5px",
                        }}
                        variant="contained"
                        onClick={() => {
                          getAllDriverExpense();
                        }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_EXPENSES_FILTER
                        }
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </Stack>
            </TableHead>
          </Table>
        ) : null}
        <DriverExpenseList
          loading={isAllListLoading}
          rows={allDriverExpense}
          getOrdersRef={getOrdersRef}
          resetRowRef={resetRowRef}
          getAllDriverExpense={getAllDriverExpense}
          isFilterOpen={isFilterOpen}
        />
      </div>
    </Box>
  );
}
export default DriverExpensePage;
