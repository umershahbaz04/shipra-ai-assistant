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
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  GetAllCustomers,
  RefreshCDPCustomers,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumChangeFilterModelApiUrls } from "../../../utilities/enum";
import UtilityClass from "../../../utilities/UtilityClass";
import { useFilterModelReducer } from "../../../utilities/helpers/Helpers";
import CDPCustomerList from "./list";

const CDPCustomer = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const filterModel = useFilterModelReducer();
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const handleFilterRest = () => {
    resetDates();
  };
  const [loading, setLoading] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allCustomers, setallCustomers] = useState([]);
  const [refreshCustomerLoading, setRefreshCustomerLoading] = useState(false);
  const handleFilterClear = async () => {
    handleFilterRest();
  };

  const getallCustomer = async () => {
    setLoading(true);
    try {
      const response = await GetAllCustomers();
      if (response?.data?.isSuccess) {
        const customers = response?.data?.result || [];
        setallCustomers(customers);
      }
    } catch (e) {
      console.error("Error fetching customers:", e);
    } finally {
      setLoading(false);
    }
  };

  const handleRefreshCustomers = async () => {
    setRefreshCustomerLoading(true);
    try {
      const response = await RefreshCDPCustomers();
      if (response?.data?.isSuccess) {
        const customers = response?.data?.result || [];
        return customers;
      }
    } catch (e) {
      console.error("Error refreshing customers:", e);
    } finally {
      setRefreshCustomerLoading(false);
    }
  };

  useEffect(() => {
    getallCustomer();
  }, []);

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <DataGridTabs
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
            tabsSmWidth={125}
            tabsMdWidth={125}
            otherBtns={
              <ButtonComponent
                loading={refreshCustomerLoading}
                title={"Refresh Customers"}
                onClick={handleRefreshCustomers}
              />
            }
            tabData={[
              {
                label: "All",
                route: "/cdp-customer",
                children: (
                  <CDPCustomerList
                    isFilterOpen={isFilterOpen}
                    loading={loading}
                    allCustomers={allCustomers}
                    getallCustomer={getallCustomer}
                  />
                ),
              },
            ]}
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
                          <InputLabel
                            sx={{
                              ...styleSheet.inputLabel,
                              overflow: "unset",
                            }}
                          >
                            {LanguageReducer?.languageType?.START_DATE}
                          </InputLabel>

                          <CustomReactDatePickerInputFilter
                            maxDate={UtilityClass.todayDate()}
                            value={startDate}
                            onClick={(date) => setStartDate(date)}
                            size="small"
                            isClearable
                          />
                        </Grid>
                        <Grid item md={2} sm={6} xs={12}>
                          <InputLabel
                            sx={{
                              ...styleSheet.inputLabel,
                              overflow: "unset",
                            }}
                          >
                            {LanguageReducer?.languageType?.END_DATE}
                          </InputLabel>
                          <CustomReactDatePickerInputFilter
                            maxDate={UtilityClass.todayDate()}
                            value={endDate}
                            onClick={(date) => setEndDate(date)}
                            size="small"
                            minDate={startDate}
                            disabled={!startDate ? true : false}
                            isClearable
                          />
                        </Grid>
                        <Grid item md={2} sm={6} xs={12} alignSelf="end">
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
                              //   onClick={() => {}}
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
          />
        </div>
      </Box>
    </>
  );
};

export default CDPCustomer;
