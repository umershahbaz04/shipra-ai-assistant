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
import { useNavigate } from "react-router-dom";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import { styleSheet } from "../../../assets/styles/style";
import { ActionButtonCustom } from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import DraftOrderList from "./list";
import { GetAllOrderDrafts, GetStoresForSelection } from "../../../api/AxiosInterceptors";

const DraftOrder = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const navigate = useNavigate();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allOrders, setAllOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [allStores, setAllStores] = useState([]);

  const getAllOrderDrafts = async () => {
    setLoading(true);
    try {
      const response = await GetAllOrderDrafts();

      if (response?.data?.isSuccess) {
        const storeMap = {};
        allStores.forEach((store) => {
          storeMap[store.storeId] = store.storeName;
        });

        const parsedOrders = response.data.result.map((order) => {
          let parsedOrderInfo = null;

          try {
            parsedOrderInfo = JSON.parse(order.orderInfo);
          } catch (err) {
            console.error("Invalid orderInfo JSON", err);
          }

          return {
            ...order,
            orderInfo: parsedOrderInfo,
            storeName: storeMap[parsedOrderInfo?.StoreId] || "",
          };
        });

        setAllOrders(parsedOrders);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  let getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    if (res.data.result != null) {
      setAllStores(res.data.result);
    }
  };

  const handleFilterClear = async () => {
    resetDates();
  };
  const handleFilter = () => {};

  useEffect(() => {
    if (allStores.length > 0) {
      getAllOrderDrafts();
    }
  }, [allStores]);

  useEffect(() => {
    getStoresForSelection();
  }, []);

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <DataGridTabs
            tabsSmWidth={125}
            filterData={
              isFilterOpen ? (
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
                              sx={{
                                ...styleSheet.inputLabel,
                                overflow: "unset",
                              }}
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
                        </Grid>
                        <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                          <Grid>
                            <InputLabel
                              sx={{
                                ...styleSheet.inputLabel,
                                overflow: "unset",
                              }}
                            >
                              {LanguageReducer?.languageType?.ORDER_END_DATE}
                            </InputLabel>
                            <CustomReactDatePickerInputFilter
                              value={endDate}
                              onClick={(date) => setEndDate(date)}
                              size="small"
                              minDate={startDate}
                              disabled={!startDate ? true : false}
                              isClearable
                              maxDate={UtilityClass.todayDate()}
                            />
                          </Grid>
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
                                minWidth: "100px",
                              }}
                              color="inherit"
                              variant="outlined"
                              onClick={() => {
                                handleFilterClear();
                              }}
                            >
                              {
                                LanguageReducer?.languageType
                                  ?.ORDER_CLEAR_FILTER
                              }
                            </Button>
                            <Button
                              sx={{
                                ...styleSheet.filterIcon,
                                minWidth: "100px",
                              }}
                              variant="contained"
                              onClick={handleFilter}
                            >
                              {LanguageReducer?.languageType?.ORDERS_FILTER}
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
                label: "All",
                route: "/draft-order-dashboard",
                children: (
                  <DraftOrderList
                    isFilterOpen={isFilterOpen}
                    allOrders={allOrders}
                    loading={loading}
                    getAllOrderDrafts={getAllOrderDrafts}
                  />
                ),
              },
            ]}
            otherBtns={
              <>
                <ActionButtonCustom
                  label={"Draft Regular Order"}
                  onClick={() => navigate("/draft-regular-order")}
                />
                <ActionButtonCustom
                  label={"Draft Fullfillable Order"}
                  onClick={() => navigate("/draft-fullfillable-order")}
                />
              </>
            }
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
          />
        </div>
      </Box>
    </>
  );
};

export default DraftOrder;
