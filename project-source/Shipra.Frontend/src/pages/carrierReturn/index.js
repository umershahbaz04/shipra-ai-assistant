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
import { ExcelExportReturnReportById } from "../../api/AxiosInterceptors";
import { GetReturnRerports } from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import GeneralTabBar from "../../components/shared/tabsBar";
import UtilityClass from "../../utilities/UtilityClass";

import { errorNotification } from "../../utilities/toast";
import CarrierReturnDashboardList from "./dashboardList";
import CustomReactDatePickerInputFilter from "../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import useDateRangeHook from "../../.reUseableComponents/CustomHooks/useDateRangeHook";

const MAX_TAGS = 150;

function ReturnCarrierDashboardPage(props) {
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  const [allReportData, setAllReportData] = useState([]);
  const [gridLoading, setGridLoading] = useState([]);

  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inputFields, setInputFields] = useState([]);
  const [isShowFilter, setIsShowFilter] = useState(true);
  let defaultValues = {
    startDate: null,
    endDate: null,
    productStationIds: [],
    productSKUs: "",
    trackingStatusID: "0",
    fullFillmentStatusId: 0,
    inTransit: 0,
    cities: 0,
  };
  // const { register, getValues, setValue, reset, control } = useForm({
  //   defaultValues,
  // });

  const [storeId, setStoreId] = useState(defaultValues.storeId);
  const [fullFillmentStatusId, setFullFillmentStatusId] = useState(
    defaultValues.fullFillmentStatusId
  );
  const [productSkus, setProductSkus] = useState(defaultValues.productSKUs);

  const [stationId, setStationId] = useState(defaultValues.stationId);

  const handleFocus = (event) => event.target.select();
  //////filters
  ///

  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);

  useEffect(() => {}, []);
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
        createdFrom: startDate ? startDate : null,
        createdTo: endDate ? endDate : null,
        start: 0,
        length: 1000,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
      productStationIds: stationId,
      productSKUs: productSkus,
      trackingStatusID: 0,
      fullFillmentStatusId: fullFillmentStatusId,
      inTransit: 0,
      cities: "",
    };
    return filters;
  };
  let getAllReturnRerports = async () => {
    setGridLoading(true);
    let params = getFiltersFromState();
    let res = await GetReturnRerports(params);
    if (res.data.result !== null) {
      setAllReportData(res.data.result);
    }
    setGridLoading(false);
    resetRowRef.current = false;
  };

  const [isfilterClear, setIsfilterClear] = useState(false);
  const handleFilterClear = async () => {
    handleFilterRest();
    setIsfilterClear(true);
  };
  const handleFilterRest = () => {
    setStartDate(defaultValues.startDate);
    setEndDate(defaultValues.endDate);
    setIsfilterClear(true);
  };
  useEffect(() => {
    if (isfilterClear) {
      getAllReturnRerports();
      setIsfilterClear(false);
    }
  }, [isfilterClear]);
  useEffect(() => {
    getAllReturnRerports();
  }, []);

  /////actions

  const EnumDropDownAction = {
    EXCELEXPORT: "ExcelExport",
  };
  //#region action button region
  const handleActionUpdateDetail = async (type, actionValue) => {
    let selectedTrNos = getOrdersRef.current;

    if (actionValue === EnumDropDownAction.EXCELEXPORT) {
      downloadExcel();
      //export excel
    }
  };

  const downloadExcel = (id) => {
    ExcelExportReturnReportById(id)
      .then((res) => {
        UtilityClass.downloadExcel(res.data, "returnReport");
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
        <GeneralTabBar
          id="dashboard-dropdown"
          tabScreen="carrier-return"
          // options={[
          //   {
          //     title: "Excel Report",
          //     value: EnumDropDownAction.EXCELEXPORT,
          //   }
          // ]}
          placeholder="Action"
          onChangeMenu={(value) => handleActionUpdateDetail("type", value)}
          placement={"bottom"}
          isFilterOpen={isFilterOpen}
          setIsFilterOpen={setIsFilterOpen}
          tabData={[]}
          disableSearch
          {...props}
        />
        {isFilterOpen && isShowFilter ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableRow>
              <Grid container spacing={2} sx={{ p: "15px 10px" }}>
                <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                  <Grid>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.CARRIER_RETURNS_START_DATE
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
                      {LanguageReducer?.languageType?.CARRIER_RETURNS_END_DATE}
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
                        handleFilterClear();
                      }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.CARRIER_RETURNS_CLEAR_FILTER
                      }
                    </Button>
                    <Button
                      sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                      variant="contained"
                      onClick={() => {
                        getAllReturnRerports();
                      }}
                    >
                      {LanguageReducer?.languageType?.ORDERS_FILTER}
                    </Button>
                  </Stack>
                </Grid>
              </Grid>
            </TableRow>
          </Table>
        ) : null}
        {
          <CarrierReturnDashboardList
            loading={gridLoading}
            allReportData={allReportData}
            getOrdersRef={getOrdersRef}
            resetRowRef={resetRowRef}
            getAllReturnRerports={getAllReturnRerports}
            isFilterOpen={isFilterOpen}
          />
        }
      </div>
    </Box>
  );
}
export default ReturnCarrierDashboardPage;
