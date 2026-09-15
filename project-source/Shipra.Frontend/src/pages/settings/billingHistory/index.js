import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Divider,
  Grid,
  IconButton,
  InputLabel,
  MenuItem,
  Stack,
  Table,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { React, useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  ClientPaymentMethodDetach,
  GetAllInvoiceHistory,
  GetClientPaymentMethods,
  UpdateClientDefaultPaymentMethod,
  UpdateSubscriptionRemoveTrial,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import StatusBadge from "../../../components/shared/statudBadge";
import GeneralTabBar from "../../../components/shared/tabsBar";
import UtilityClass from "../../../utilities/UtilityClass";
import Colors from "../../../utilities/helpers/Colors";
import {
  colorSpan,
  fetchMethod,
  GridContainer,
  GridItem,
  handleGetClientSubscription,
  MenuComponent,
  PageMainBox,
  purple,
  TextLoader,
  useClientSubscriptionReducer,
  useMenuForLoop,
} from "../../../utilities/helpers/Helpers";
import UpgradeOrCancelPlanModal from "./Modals/UpgradeOrCancelPlan";
import BillingHistoryList from "./list";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import UpgradeCancelEndTrialConfirmationModal from "./Modals/UpgradeCancelEndTrialConfirmation";
export const modalTypes = Object.freeze({
  UPGRADE: 1,
  CANCEL: 2,
  ACTIVE: 3,
});

const BillingHistory = (props) => {
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const clientSubscriptionData = useClientSubscriptionReducer();
  const dispatch = useDispatch();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [invoiceHistory, setInvoiceHistory] = useState([]);
  const [paymentMethods, setPaymentMethods] = useState({
    loading: true,
    data: [],
    hasPaymentMethods: true,
  });

  const [paymentMethodDefault, setPaymentMethodDefault] = useState({
    loading: false,
  });
  const [detachPaymentMethod, setDetachPaymentMethod] = useState({
    loading: false,
  });
  const [upgradeOrCancelPlan, setUpgradeOrCancelPlan] = useState({
    open: false,
    type: null,
  });
  const [clientSubscriptionLoading, setClientSubscriptionLoading] =
    useState(true);
  const [endTrialAndPay, setEndtrialAndPay] = useState({
    open: false,
    loading: false,
    stripeSubscriptionId: null,
  });
  const [isLoading, setIsLoading] = useState(false);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { anchorEl, openElem, handleOpen, handleClose } = useMenuForLoop();
  const getAllInvoiceHistory = async () => {
    setIsLoading(true); // Start loading
    const { response } = await fetchMethod(() => GetAllInvoiceHistory());
    if (response?.isSuccess) {
      setInvoiceHistory(response?.result?.list);
    }
    setIsLoading(false); // End loading
  };

  const hanldeGetClientPaymentMethods = async () => {
    const { response } = await fetchMethod(
      GetClientPaymentMethods,
      setPaymentMethods
    );
    if (response?.isSuccess) {
      if (response?.result) {
        const sortedData = response?.result?.sort((a, b) => {
          if (
            a.defaultPaymentMethodId !== null &&
            b.defaultPaymentMethodId === null
          )
            return -1;
          if (
            a.defaultPaymentMethodId === null &&
            b.defaultPaymentMethodId !== null
          )
            return 1;
          return 0;
        });
        setPaymentMethods((prev) => ({
          ...prev,
          data: sortedData || [],
        }));
        return;
      } else {
        setPaymentMethods((prev) => ({
          ...prev,
          hasPaymentMethods: false,
        }));
      }
    }
  };
  const handleSetPaymentMethodAsDefault = async (paymentMethodId) => {
    const { response } = await fetchMethod(
      () => UpdateClientDefaultPaymentMethod(paymentMethodId),
      setPaymentMethodDefault
    );
    if (response?.isSuccess) {
      hanldeGetClientPaymentMethods();
    }
  };
  const handleDetachPaymentMethodAsDefault = async (paymentMethodId) => {
    const { response } = await fetchMethod(
      () => ClientPaymentMethodDetach(paymentMethodId),
      setDetachPaymentMethod
    );
    if (response?.isSuccess) {
      hanldeGetClientPaymentMethods();
    } else {
      UtilityClass.showErrorNotificationWithDictionary(response.errors);
    }
  };
  const handleUpdateSubscriptionRemoveTrial = async (stripeSubscriptionId) => {
    const { response } = await fetchMethod(
      () => UpdateSubscriptionRemoveTrial(stripeSubscriptionId),
      setEndtrialAndPay
    );
    if (response?.isSuccess) {
      handleGetClientSubscription(dispatch);
    } else {
      UtilityClass.showErrorNotificationWithDictionary(response.errors);
    }
  };

  const handleFilterRest = () => {
    resetDates();
  };

  const handleGetClientSubscriptionWithLoading = async () => {
    setClientSubscriptionLoading(true);
    await handleGetClientSubscription(dispatch);
    setClientSubscriptionLoading(false);
  };

  useEffect(() => {
    getAllInvoiceHistory();
    hanldeGetClientPaymentMethods();
    handleGetClientSubscriptionWithLoading();
  }, []);
  return (
    <>
      <PageMainBox>
        <GridContainer justifyContent="center">
          <GridItem xs={12} md={12}>
            <Box sx={{ maxWidth: 1232, mx: "auto" }}>
              <Box
                sx={{
                  display: "flex",
                  gap: 1,
                  justifyContent: "end",
                }}
              >
                {clientSubscriptionLoading ? (
                  <Box sx={{ width: 200 }}>
                    <TextLoader columnsNum={2} rowsNum={1} height={50} />
                  </Box>
                ) : (
                  <>
                    {!clientSubscriptionData?.data?.isSubscriptionCancel && (
                      <ButtonComponent
                        title={
                          LanguageReducer?.languageType
                            ?.SETTINGS_BILLING_HISTORY_ACTIVE_PLAN
                        }
                        bg={Colors.succes}
                        onClick={() =>
                          setUpgradeOrCancelPlan((prev) => ({
                            ...prev,
                            type: modalTypes.ACTIVE,
                            open: true,
                          }))
                        }
                      />
                    )}
                    {!clientSubscriptionData?.data?.isSubscriptionCancel && (
                      <ButtonComponent
                        title={
                          LanguageReducer?.languageType
                            ?.SETTINGS_BILLING_HISTORY_CANCEL_PLAN
                        }
                        bg={Colors.danger}
                        onClick={() =>
                          setUpgradeOrCancelPlan((prev) => ({
                            ...prev,
                            type: modalTypes.CANCEL,
                            open: true,
                          }))
                        }
                      />
                    )}
                    {clientSubscriptionData?.data?.isOnTrail && (
                      <ButtonComponent
                        title={
                          LanguageReducer?.languageType
                            ?.SETTINGS_BILLING_HISTORY_END_TRIAL_AND_PAY
                        }
                        bg={Colors.warning}
                        onClick={() =>
                          setEndtrialAndPay((prev) => ({
                            ...prev,
                            stripeSubscriptionId:
                              clientSubscriptionData?.data?.subscriptionId,
                            open: true,
                          }))
                        }
                      />
                    )}
                    <ButtonComponent
                      title={
                        clientSubscriptionData?.data?.isSubscriptionCancel
                          ? LanguageReducer?.languageType
                              ?.SETTINGS_BILLING_HISTORY_SUBSCRIBE_PLAN
                          : LanguageReducer?.languageType
                              ?.SETTINGS_BILLING_HISTORY_CHANGE_PLAN
                      }
                      onClick={() =>
                        setUpgradeOrCancelPlan((prev) => ({
                          ...prev,
                          type: modalTypes.UPGRADE,
                          open: true,
                        }))
                      }
                    />
                  </>
                )}
              </Box>
            </Box>
          </GridItem>

          <GridItem xs={12} md={12}>
            <Box sx={{ maxWidth: 1232, mx: "auto" }}>
              <Card
                style={{
                  marginTop: "10px",
                  borderRadius: "5px",
                }}
              >
                <CardHeader
                  title={
                    LanguageReducer?.languageType
                      ?.SETTINGS_BILLING_HISTORY_PAYMENT_METHODS
                  }
                  titleTypographyProps={{ align: "start", variant: "h4" }}
                  style={{ backgroundColor: "#fff", textAlign: "start" }}
                  subheader={colorSpan(
                    "The following information pertains to Stripe. Please note that we do not store any sensitive information within our system.",
                    "red"
                  )}
                />
                <Divider />
                <CardContent sx={{ p: 0 }}>
                  {paymentMethods.loading ? (
                    <TextLoader columnsNum={4} rowsNum={1} height={20} />
                  ) : paymentMethods.hasPaymentMethods ? (
                    <Box>
                      {paymentMethods.data?.map((dt, index, arr) => (
                        <Box
                          className={"flex_between"}
                          sx={{
                            borderBottom:
                              index + 1 !== arr.length && "1px solid #e0e0e0",
                            px: 2,
                          }}
                        >
                          <Box
                            sx={{
                              display: "flex",
                              alignItems: "center",
                              minWidth: { xs: 100, md: 200 },
                            }}
                          >
                            <Typography
                              variant="h5"
                              sx={{
                                textTransform: "capitalize",
                              }}
                            >
                              {dt.displayBrand}
                            </Typography>
                            <Typography
                              variant="h5"
                              sx={{
                                position: "relative",
                                top: 3,
                              }}
                            >
                              {`******`}
                            </Typography>
                            <Typography variant="h5">{dt.last4}</Typography>
                            <Typography sx={{ pl: 1 }}>
                              {dt.defaultPaymentMethodId && (
                                <StatusBadge
                                  title={"Default"}
                                  color={"#fff"}
                                  bgColor={purple}
                                />
                              )}
                            </Typography>
                          </Box>
                          <Typography
                            variant="body2"
                            sx={{
                              textTransform: "capitalize",
                            }}
                          >
                            <Box sx={{ fontWeight: 600 }} component={"span"}>
                              Cardtype
                            </Box>
                            : {dt.funding}
                          </Typography>
                          <Typography
                            variant="body2"
                            sx={{
                              textTransform: "capitalize",
                            }}
                          >
                            {dt.cvcCheckStatus && (
                              <>
                                <Box
                                  sx={{ fontWeight: 600 }}
                                  component={"span"}
                                >
                                  CVCCheck
                                </Box>
                                : {dt.cvcCheckStatus}
                                <CheckCircleOutlineIcon
                                  sx={{
                                    ml: 0.5,
                                    color: Colors.succes,
                                    position: "relative",
                                    top: 5,
                                  }}
                                  fontSize={"small"}
                                />
                              </>
                            )}
                          </Typography>
                          <Box className={"flex_between"} key={index}>
                            <Typography variant="body2">
                              <Box sx={{ fontWeight: 600 }} component={"span"}>
                                Expires
                              </Box>
                              : {dt.expMonth}/{dt.expYear}
                            </Typography>

                            <IconButton
                              disabled={dt.defaultPaymentMethodId}
                              onClick={(e) => handleOpen(e, dt.paymentMethodId)}
                            >
                              <MoreVertIcon />
                            </IconButton>
                            <MenuComponent
                              open={openElem === dt.paymentMethodId}
                              anchorEl={anchorEl}
                              onClose={handleClose}
                            >
                              <MenuItem
                                onClick={() => {
                                  handleSetPaymentMethodAsDefault(
                                    dt.paymentMethodId
                                  );
                                  handleClose();
                                }}
                              >
                                Set as default
                              </MenuItem>
                              <MenuItem
                                onClick={() => {
                                  handleDetachPaymentMethodAsDefault(
                                    dt.paymentMethodId
                                  );
                                  handleClose();
                                }}
                              >
                                Detach
                              </MenuItem>
                            </MenuComponent>
                          </Box>
                        </Box>
                      ))}
                    </Box>
                  ) : (
                    <Box sx={{ py: 2 }}>
                      <Typography variant="h4" className="flex_center">
                        N/A
                      </Typography>
                    </Box>
                  )}
                </CardContent>
                <Divider />
                <CardActions
                  style={{ justifyContent: "space-between", padding: "8px" }}
                >
                  {/* <Box style={{ display: "flex", gap: "8px" }}>
              <Button variant="text">
                <AddIcon /> <b>Add new payment method</b>
              </Button>
            </Box> */}
                </CardActions>
              </Card>
            </Box>
          </GridItem>
          <GridItem xs={12} md={12}>
            <Box sx={{ maxWidth: 1232, mx: "auto" }}>
              <Card
                style={{
                  marginTop: "10px",
                  borderRadius: "5px",
                }}
              >
                <CardHeader
                  title={
                    LanguageReducer?.languageType
                      ?.SETTINGS_BILLING_HISTORY_INVOICE_HISTORY
                  }
                  titleTypographyProps={{ align: "start", variant: "h4" }}
                  style={{
                    backgroundColor: "#fff",
                    textAlign: "start",
                  }}
                />
                <Divider />
                <Box sx={styleSheet.pageRoot}>
                  <GeneralTabBar
                    tabScreen={
                      LanguageReducer?.languageType
                        ?.SETTINGS_BILLING_HISTORY_BILLING
                    }
                    isFilterOpen={isFilterOpen}
                    setIsFilterOpen={setIsFilterOpen}
                    disableSearch
                    {...props}
                  />
                  {isFilterOpen && (
                    <Table
                      sx={{ ...styleSheet.generalFilterArea }}
                      size="small"
                      aria-label="a dense table"
                    >
                      <TableHead>
                        <TableRow>
                          <Grid container spacing={2} sx={{ p: "15px 10px" }}>
                            <Grid item xl={2} lg={2} md={4} sm={6} xs={12}>
                              <Grid>
                                <InputLabel
                                  sx={{
                                    ...styleSheet.inputLabel,
                                    overflow: "unset",
                                  }}
                                >
                                  {LanguageReducer?.languageType?.START_DATE}
                                </InputLabel>

                                <CustomReactDatePickerInputFilter
                                  value={startDate}
                                  maxDate={UtilityClass.todayDate()}
                                  onClick={(date) => setStartDate(date)}
                                  size="small"
                                  isClearable

                                  // inputProps={{ style: { padding: "2px 5px" } }}
                                />
                              </Grid>
                            </Grid>
                            <Grid item xl={2} lg={2} md={4} sm={6} xs={12}>
                              <Grid>
                                <InputLabel
                                  sx={{
                                    ...styleSheet.inputLabel,
                                    overflow: "unset",
                                  }}
                                >
                                  {LanguageReducer?.languageType?.END_DATE}
                                </InputLabel>
                                <CustomReactDatePickerInputFilter
                                  value={endDate}
                                  onClick={(date) => setEndDate(date)}
                                  size="small"
                                  minDate={startDate}
                                  maxDate={UtilityClass.todayDate()}
                                  disabled={!startDate ? true : false}
                                  isClearable
                                  // inputProps={{ style: { padding: "2px 5px" } }}
                                />
                              </Grid>
                            </Grid>
                            <Grid item xl={2} lg={2} md={4} sm={6} xs={12}>
                              <Stack
                                alignItems="flex-end"
                                direction="row"
                                spacing={1}
                                sx={{
                                  ...styleSheet.filterButton,
                                  height: "100%",
                                  border: "none !important",
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
                                    handleFilterRest();
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
                                  onClick={() => {}}
                                >
                                  {LanguageReducer?.languageType?.FILTER}
                                </Button>
                              </Stack>
                            </Grid>
                          </Grid>
                        </TableRow>
                      </TableHead>
                    </Table>
                  )}

                  {/* </Container> */}
                </Box>
                <CardContent sx={{ padding: "0px", paddingBottom: "20px" }}>
                  <BillingHistoryList
                    invoiceHistory={invoiceHistory || []}
                    getOrdersRef={getOrdersRef}
                    resetRowRef={resetRowRef}
                    isLoading={isLoading}
                  />
                </CardContent>
              </Card>
            </Box>
          </GridItem>
        </GridContainer>
      </PageMainBox>

      {upgradeOrCancelPlan.open && (
        <UpgradeOrCancelPlanModal
          handleGetClientSubscriptionWithLoading={
            handleGetClientSubscriptionWithLoading
          }
          open={upgradeOrCancelPlan.open}
          type={upgradeOrCancelPlan.type}
          onClose={() =>
            setUpgradeOrCancelPlan((prev) => ({ ...prev, open: false }))
          }
        />
      )}

      <UpgradeCancelEndTrialConfirmationModal
        open={endTrialAndPay.open}
        onClose={() => setEndtrialAndPay((prev) => ({ ...prev, open: false }))}
        loading={endTrialAndPay.loading}
        onConfirm={async () => {
          await handleUpdateSubscriptionRemoveTrial(
            endTrialAndPay.stripeSubscriptionId
          );
          setEndtrialAndPay((prev) => ({ ...prev, loading: true }));
          setTimeout(async () => {
            await handleGetClientSubscriptionWithLoading();
            setEndtrialAndPay((prev) => ({
              ...prev,
              loading: false,
              open: false,
            }));
          }, 5000);
        }}
        heading={
          LanguageReducer?.languageType
            ?.SETTINGS_BILLING_HISTORY_ARE_YOU_SURE_TO_CANCEL_YOUR_TRIAL_PERIOD_AND_PAY_NOW
        }
        message={
          LanguageReducer?.languageType
            ?.SETTINGS_BILLING_HISTORY_THIS_ACTION_WILL_REMOVE_YOUR_TRIAL_PERIOD_AND_CHARGE_FROM_YOUR_STRIPE_DEFAULT_PAYMENT_METHOD_AND_REFLECT_IN_INVOICE_HISTORY
        }
        buttonText={
          LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_END_TRIAL
        }
        buttonColor={purple}
      />
    </>
  );
};

export default BillingHistory;
