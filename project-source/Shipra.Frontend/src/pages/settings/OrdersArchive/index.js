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
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import { GetAllArchiveOrders } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import OrderArchiveList from "./list";

const OrderArchivePage = () => {
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
  const [loading, setLoading] = useState(false);
  const [archiveOrder, setArchiveOrder] = useState([]);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isfilterClear, setIsfilterClear] = useState(false);

  const handleFilterClear = async () => {
    handleFilterRest();
    setIsfilterClear(true);
  };

  const handleFilterRest = () => {
    resetDates();
  };

  const getAllArchiveOrders = async () => {
    setLoading(true);
    try {
      const response = await GetAllArchiveOrders(
        startDateFormated || null,
        endDateFormated || null
      );

      if (response?.data?.isSuccess) {
        const withRowNum = response.data.result.map((item, index) => ({
          ...item,
          rowNum: index + 1,
        }));
        setArchiveOrder(withRowNum);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getAllArchiveOrders();
  }, []);

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <DataGridTabs
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
            tabsSmWidth="10px"
            tabsMdWidth="10px"
            tabData={[
              {
                label: "All",
                route: "/archive-orders",
                children: (
                  <OrderArchiveList
                    loading={loading}
                    archiveOrder={archiveOrder}
                    isFilterOpen={isFilterOpen}
                    getAllArchiveOrders={getAllArchiveOrders}
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
                          <Grid>
                            <InputLabel
                              sx={{
                                ...styleSheet.inputLabel,
                                overflow: "unset",
                              }}
                            >
                              {"Create From"}
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
                        <Grid item md={2} sm={6} xs={12}>
                          <Grid>
                            <InputLabel
                              sx={{
                                ...styleSheet.inputLabel,
                                overflow: "unset",
                              }}
                            >
                              {"Create To"}
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
                              onClick={() => {
                                getAllArchiveOrders();
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
          />
        </div>
      </Box>
    </>
  );
};

export default OrderArchivePage;
