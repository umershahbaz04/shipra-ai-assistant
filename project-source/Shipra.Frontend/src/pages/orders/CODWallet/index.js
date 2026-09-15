import AccountBalanceIcon from "@mui/icons-material/AccountBalance";
import AccountBalanceWalletIcon from "@mui/icons-material/AccountBalanceWallet";
import LocalAtmIcon from "@mui/icons-material/LocalAtm";
import PaymentsIcon from "@mui/icons-material/Payments";
import {
  Box,
  Button,
  ButtonGroup,
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
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  AddUpdateWallet,
  GetAllPayouts,
  GetTotalProcessingPaymentStatus,
  GetWallet,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import {
  ActionButtonCustom,
  fetchMethod,
  greyBorder,
} from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import CODWalletList from "./list";
import WalletTransctionModal from "./modals/WalletTransctionModal";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import RequestPageIcon from "@mui/icons-material/RequestPage";
import CreditScoreIcon from "@mui/icons-material/CreditScore";
const WalletPage = () => {
  const navigate = useNavigate();
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const [searchParams] = useSearchParams();
  const transactionId = searchParams.get("id");

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [loading, setLoading] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allwallets, setAllwallets] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [allPayoutStatus, setAllPayoutStatus] = useState([]);
  const [openTransctionModal, setOpenTransctionModal] = useState(false);

  const handleFilterClear = async () => {
    setIsfilterClear(true);
    resetDates();
  };

  const getWallets = async () => {
    setLoading(true);
    try {
      const { response } = await fetchMethod(() => GetWallet());
      if (response.isSuccess) {
        setAllwallets(response.result);
      }
    } catch (error) {
      console.error("Error fetching client return reasons:", error);
    }
    setLoading(false);
  };

  const handleFilter = () => {
    getAllPayouts();
  };

  const getAllPayouts = async () => {
    setIsLoading(true);
    try {
      const { response } = await fetchMethod(() => GetAllPayouts());
      if (response.isSuccess) {
        setAllPayoutStatus(response?.result);
      }
    } catch (error) {
      console.error("Error fetching:", error);
    }
    setIsLoading(false);
  };

  const handleGetTotalProcessingPaymentStatus = async (transactionId) => {
    const { response: totalProcessingResponse } = await fetchMethod(() =>
      GetTotalProcessingPaymentStatus(transactionId)
    );
    const code =
      process.env.NODE_ENV === "production" ? "000.000.000" : "000.100.110";
    if (totalProcessingResponse?.result?.result?.code === code) {
      const { response: walletResponse } = await fetchMethod(() =>
        AddUpdateWallet(Number(totalProcessingResponse?.result?.amount))
      );
      if (walletResponse?.isSuccess) {
        getWallets();
        successNotification(walletResponse?.result?.message);
        navigate("/wallet");
      }
      return;
    }
    errorNotification(totalProcessingResponse?.result?.result?.description);
  };

  useEffect(() => {
    getWallets();
    getAllPayouts();
  }, []);

  useEffect(() => {
    if (transactionId) {
      handleGetTotalProcessingPaymentStatus(transactionId);
    }
  }, [transactionId]);

  return (
    <>
      <Box display="flex" gap={2} p={"10px"} pb={0}>
        <Grid container spacing={2} alignItems="center">
          {/* <Grid item xs={12} sm={6} md={3}>
            <Card
              sx={{
                bgcolor: "#f5f5f5",
                border: greyBorder,
                boxShadow: "none",
                borderRadius: 2,
              }}
            >
              <CardContent>
                <Box display="flex" alignItems="center">
                  {<LocalAtmIcon style={{ fontSize: 40 }} />}
                  <Box ml={2}>
                    <Typography variant="body2" color="textSecondary">
                      {
                        LanguageReducer?.languageType
                          ?.ORDERS_WALLET_AVAILABLE_BALANCE
                      }
                    </Typography>
                    <Typography variant="h5" component="div" fontWeight="bold">
                      {allwallets.availableBalance} AED
                    </Typography>
                  </Box>
                </Box>
                <Typography variant="body2" color="textSecondary">
                  {""}
                </Typography>
              </CardContent>
            </Card>
          </Grid> */}
          <Grid item xs={12} sm={6} md={4}>
            <Card
              sx={{
                bgcolor: "#f5f5f5",
                border: greyBorder,
                boxShadow: "none",
                borderRadius: 2,
              }}
            >
              <CardContent>
                <Box display="flex" alignItems="center">
                  {<AccountBalanceIcon style={{ fontSize: 40 }} />}
                  <Box ml={2}>
                    <Typography variant="body2" color="textSecondary">
                      {
                        LanguageReducer?.languageType
                          ?.ORDERS_WALLET_CURRENT_BALANCE
                      }
                    </Typography>
                    <Typography variant="h5" component="div" fontWeight="bold">
                      {allwallets.currentBalance} AED
                    </Typography>
                  </Box>
                </Box>
                <Typography variant="body2" color="textSecondary">
                  {""}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <Card
              sx={{
                bgcolor: "#f5f5f5",
                border: greyBorder,
                boxShadow: "none",
                borderRadius: 2,
              }}
            >
              <CardContent>
                <Box display="flex" alignItems="center">
                  {<CreditScoreIcon style={{ fontSize: 40 }} />}
                  <Box ml={2}>
                    <Typography variant="body2" color="textSecondary">
                      {
                        LanguageReducer?.languageType
                          ?.ORDERS_WALLET_PAYOUT_COMPLETED
                      }
                    </Typography>
                    <Typography variant="h5" component="div" fontWeight="bold">
                      {allwallets?.totalCompletedPayout || 0} AED
                    </Typography>
                  </Box>
                </Box>
                <Typography variant="body2" color="textSecondary">
                  {""}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <Card
              sx={{
                bgcolor: "#f5f5f5",
                border: greyBorder,
                boxShadow: "none",
                borderRadius: 2,
              }}
            >
              <CardContent>
                <Box display="flex" alignItems="center">
                  {<RequestPageIcon style={{ fontSize: 40 }} />}
                  <Box ml={2}>
                    <Typography variant="body2" color="textSecondary">
                      {
                        LanguageReducer?.languageType
                          ?.ORDERS_WALLET_PAYOUT_REQUEST
                      }
                    </Typography>
                    <Typography variant="h5" component="div" fontWeight="bold">
                      {allwallets?.totalRequestedPayout || 0} AED
                    </Typography>
                  </Box>
                </Box>
                <Typography variant="body2" color="textSecondary">
                  {""}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Box>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <Box sx={styleSheet.topNavBar}>
            <Box
              sx={{ ...styleSheet.topNavBarLeft, fontWeight: "900 !important" }}
            ></Box>
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
                label: LanguageReducer?.languageType?.ORDERS_WALLET_ALL,
                route: "/wallet",
                children: (
                  <CODWalletList
                    isLoading={isLoading}
                    isFilterOpen={isFilterOpen}
                    allPayoutStatus={allPayoutStatus}
                  />
                ),
              },
            ]}
            otherBtns={
              <ActionButtonCustom
                label={LanguageReducer?.languageType?.ORDERS_WALLET_TRANSCTION}
                onClick={() => setOpenTransctionModal(true)}
                startIcon={<PaymentsIcon fontSize="small" />}
              />
            }
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
          />
        </div>
      </Box>
      {openTransctionModal && (
        <WalletTransctionModal
          open={openTransctionModal}
          onClose={() => setOpenTransctionModal(false)}
        />
      )}
    </>
  );
};

export default WalletPage;
