import {
  Box,
  Button,
  ButtonGroup,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  GetAllCarrierWithServiceAndLocation,
  GetAllOrderStatusReport,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import StatusReportList from "./list";
import {
  EnumChangeFilterModelApiUrls,
  EnumOptions,
} from "../../../utilities/enum";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";

const StatusReport = () => {
  const [isFilterOpen, setIsFilterOpen] = useState(true);
  const [loading, setLoading] = useState(false);
  const [allOrderStatusReport, setAllOrderStatusReport] = useState([]);
  const [selectedCarrierId, setSelectedCarrierId] = useState();
  const [carriersForSelection, setCarriersForSelection] = useState([]);
  const { startDate, setStartDate, resetDates, startDateFormated } =
    useDateRangeHook();
  const handleFilterRest = () => {
    resetDates();
  };
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleFilterClear = async () => {
    handleFilterRest();
    setSelectedCarrierId("");
  };

  const getAllCarriersData = async () => {
    try {
      const response = await GetAllCarrierWithServiceAndLocation({
        filterModel: {
          start: 0,
          length: EnumChangeFilterModelApiUrls.GET_ALL_CARRIER.length,
          search: "",
          sortDir: "desc",
          sortCol: 0,
        },
      });
      setCarriersForSelection(
        response?.data?.result?.list?.map((dt, index) => ({ ...dt, index }))
      );
    } catch (error) {
      console.error("Error in updating getting these carriers", error.response);
    } finally {
    }
  };

  const getAllOrderStatusReport = async () => {
    setLoading(true);
    const body = {
      Date: startDateFormated,
      CarrierId: selectedCarrierId?.CarrierId || 0,
    };
    try {
      const response = await GetAllOrderStatusReport(body);
      if (response?.data?.isSuccess) {
        const transformedList = response.data.result.List.map((item, index) => {
          const convertedCurrent = UtilityClass.convertUtcToLocalWithDashedDate(
            item.CurrentStatusUpdatedDateTime
          );
          const convertedLast = UtilityClass.convertUtcToLocalWithDashedDate(
            item.LastStatusUpdatedDateTime
          );
          const convertedOrderDate =
            UtilityClass.convertUtcToLocalAndGetShortDate(item.OrderDate);
          return {
            ...item,
            index,
            CurrentStatusUpdatedDateTime: convertedCurrent,
            LastStatusUpdatedDateTime: convertedLast,
            OrderDate: convertedOrderDate,
          };
        });
        setAllOrderStatusReport(transformedList);
      }
    } catch (error) {
      console.error("Error fetching order status report:", error);
    } finally {
      setLoading(false);
    }
  };

  const StatTusReportTableRef = useRef();
  const handlePDF = () => StatTusReportTableRef.current.generatePDF();
  const handleExcel = () => StatTusReportTableRef.current.generateExcel();

  useEffect(() => {
    getAllOrderStatusReport();
    getAllCarriersData();
  }, []);

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <Box sx={styleSheet.topNavBar}>
            <Stack
              sx={styleSheet.topNavBarRight}
              direction="row"
              justifyContent="flex-end"
              alignItems="center"
              spacing={1}
            >
              <ButtonGroup
                variant="outlined"
                aria-label="split button"
              ></ButtonGroup>
            </Stack>
          </Box>
          <DataGridTabs
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
            tabsSmWidth="10px"
            tabsMdWidth="10px"
            otherBtns={
              allOrderStatusReport.length > 0 && (
                <>
                  <ButtonComponent
                    title={"Export Data to PDF"}
                    btnMdWidth={"130px"}
                    btnLgWidth={"155px"}
                    onClick={handlePDF}
                  />
                  <ButtonComponent
                    title={"Export Orders to Excel"}
                    btnMdWidth={"140px"}
                    btnLgWidth={"160px"}
                    onClick={handleExcel}
                  />
                </>
              )
            }
            filterData={
              isFilterOpen ? (
                <Table
                  sx={{ ...styleSheet.generalFilterArea }}
                  size="small"
                  aria-label="a dense table"
                >
                  <TableHead>
                    <TableRow>
                      <Grid container spacing={2} sx={{ p: "15px 10px" }}>
                        <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                          <Grid>
                            <InputLabel
                              sx={{
                                ...styleSheet.inputLabel,
                                overflow: "unset",
                              }}
                            >
                              {"Date"}
                            </InputLabel>

                            <CustomReactDatePickerInputFilter
                              maxDate={UtilityClass.todayDate()}
                              value={startDate}
                              onClick={(date) => setStartDate(date)}
                              size="small"
                              isClearable
                            />
                          </Grid>
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
                            options={carriersForSelection}
                            value={selectedCarrierId}
                            optionLabel={EnumOptions.ALL_CARRIER.LABEL}
                            optionValue={EnumOptions.ALL_CARRIER.VALUE}
                            onChange={(e, val) => {
                              setSelectedCarrierId(val);
                            }}
                          />
                        </Grid>
                        <Grid item md={4} sm={6} xs={12} alignSelf="end">
                          <Stack
                            direction={"row"}
                            sx={{
                              ...styleSheet.filterButtonMargin,
                              display: "row",
                            }}
                            spacing={1}
                          >
                            <Button
                              sx={{
                                ...styleSheet.filterIcon,
                                minWidth: "100px",
                              }}
                              color="inherit"
                              variant="outlined"
                              onClick={() => {
                                handleFilterClear();
                              }}
                            >
                              {LanguageReducer?.languageType?.CLEAR_FILTER}
                            </Button>
                            <Button
                              sx={{
                                ...styleSheet.filterIcon,
                                minWidth: "100px",
                              }}
                              variant="contained"
                              onClick={() => {
                                getAllOrderStatusReport();
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
              ) : null
            }
            tabData={[
              {
                label:
                  LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_ALL,
                route: "/status-report",
                children: (
                  <StatusReportList
                    loading={loading}
                    allOrderStatusReport={allOrderStatusReport}
                    isFilterOpen={isFilterOpen}
                    StatTusReportTableRef={StatTusReportTableRef}
                    startDateFormated={UtilityClass.convertUtcToLocalAndGetShortDate(
                      startDate
                    )}
                  />
                ),
              },
            ]}
          />
        </div>
      </Box>
    </>
  );
};

export default StatusReport;
