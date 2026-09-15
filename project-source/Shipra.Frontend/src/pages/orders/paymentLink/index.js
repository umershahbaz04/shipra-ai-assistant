import {
  Box,
  Button,
  Card,
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
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import {
  GenerateTotalProcessPaymentLink,
  GetAllOrderPaymentLinks,
  GetAllPaymentLinkStatusLookup,
  GetShipmentInfoByOrderNo,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import PayoutRequestModal from "../../../components/modals/orderModals/PayoutRequestModal";
import UpdateEmail from "../../../components/modals/orderModals/UpdateEmail";
import { EnumOptions } from "../../../utilities/enum";
import {
  ActionButtonCustom,
  fetchMethod,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import PaymentLinkList from "./list";

const PaymentLink = () => {
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [linkLoading, setLinkLoading] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState("");
  const [openUpdateEmail, setOpenUpdateEmail] = useState(false);
  const [openPayoutModal, setOpenPayoutModal] = useState(false);
  const [selectedOrderId, setSelectedOrderId] = useState();
  const [allLinkStatus, setAllLinkStatus] = useState([]);
  const [selectedLinkStatus, setSelectedLinkStatus] = useState();
  const [allPaymentLink, setAllPaymentLink] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const handleFilterClear = async () => {
    resetDates();
  };
  const handleFilter = () => {
    getAllOrderPaymentLinks();
  };

  const getAllOrderPaymentLinks = async () => {
    setIsLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetAllOrderPaymentLinks(
          startDate,
          endDate,
          selectedLinkStatus?.paymentLinkStatusId,
          false
        )
      );
      if (response.isSuccess) {
        setAllPaymentLink(response?.result?.list);
      }
    } catch (error) {
      console.error("Error fetching:", error);
    }
    setIsLoading(false);
  };

  const getAllPaymentLinkStatusLookup = async () => {
    try {
      const { response } = await fetchMethod(GetAllPaymentLinkStatusLookup);
      if (response?.result) {
        setAllLinkStatus(response?.result);
      } else {
        console.log("No data found");
      }
    } catch (error) {
      console.error("Error fetching PaymentLink:", error);
    }
  };

  const generateTotalProcessPaymentLink = async () => {
    setLinkLoading(true);
    const body = {
      orderNo: selectedOrder,
    };
    try {
      const res = await GenerateTotalProcessPaymentLink(body);
      if (!res?.data?.isSuccess) {
        UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
      } else {
        successNotification("Payment Link Generate successfully.");
        setSelectedOrder("");
        getAllOrderPaymentLinks();
      }
    } catch (e) {
      errorNotification("An error occurred while Generating Payment Link.");
    } finally {
      setLinkLoading(false);
    }
  };
  const GeneratePaymentLink = async () => {
    const response = await GetShipmentInfoByOrderNo(selectedOrder);
    const customerEmail = response?.data?.result?.order?.CustomerEmail;
    const OrderAddressId = response?.data?.result.order?.OrderAddressId;
    console.log(response?.data?.result.order);
    if (!customerEmail) {
      setSelectedOrderId(OrderAddressId);
      setOpenUpdateEmail(true);
    } else {
      generateTotalProcessPaymentLink();
    }
  };
  useEffect(() => {
    getAllPaymentLinkStatusLookup();
    getAllOrderPaymentLinks();
  }, []);

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <Box sx={{ padding: "10px" }}>
          <Card sx={styleSheet.uploadOrderCard} variant="outlined">
            <Grid
              container
              spacing={1.5}
              sx={{ paddingTop: 0, alignItems: "flex-end" }}
            >
              <Grid item xs={12}>
                <Box width="100%" display="flex" justifyContent="space-between">
                  <Grid
                    item
                    md={4}
                    sm={12}
                    xs={12}
                    sx={{ paddingLeft: "12px" }}
                  >
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {
                        LanguageReducer?.languageType
                          ?.ORDERS_PAYMENT_LINK_ORDER_NO
                      }
                    </InputLabel>
                    <TextFieldComponent
                      sx={{
                        fontsize: "12px",
                        marginTop: "4px",
                        backgroundColor: "#fff",
                        "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input":
                          {
                            padding: "8px!important",
                          },
                      }}
                      placeholder={"Enter Order No."}
                      value={selectedOrder || ""}
                      name="boxName"
                      onChange={(e) => setSelectedOrder(e.target.value)}
                    />
                  </Grid>
                </Box>
              </Grid>
              <Grid item xs={12}>
                <Box
                  sx={{
                    width: "100%",
                    display: "flex",
                    justifyContent: "flex-end",
                    mt: "auto",
                  }}
                >
                  <ActionButtonCustom
                    label={
                      LanguageReducer?.languageType
                        ?.ORDERS_PAYMENT_LINK_GENERATE_PAYMENT_LINK
                    }
                    disabled={
                      selectedOrder === null || selectedOrder === undefined
                    }
                    loading={linkLoading}
                    onClick={GeneratePaymentLink}
                  />
                </Box>
              </Grid>
            </Grid>
          </Card>
        </Box>
      </Box>
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
                          <InputLabel
                            sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                          >
                            {
                              LanguageReducer?.languageType
                                ?.ORDERS_PAYMENT_LINK_PAYMENT_STATUS
                            }
                          </InputLabel>
                          <SelectComponent
                            name="PaymentStatus"
                            height={28}
                            options={allLinkStatus}
                            value={selectedLinkStatus}
                            optionLabel={EnumOptions.LINK_STATUS.LABEL}
                            optionValue={EnumOptions.LINK_STATUS.VALUE}
                            getOptionLabel={(option) => option.text}
                            onChange={(e, val) => {
                              setSelectedLinkStatus(val);
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
                label: LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_ALL,
                route: "/payment-link",
                children: (
                  <PaymentLinkList
                    isLoading={isLoading}
                    isFilterOpen={isFilterOpen}
                    allPaymentLink={allPaymentLink}
                  />
                ),
              },
            ]}
            otherBtns={
              <ActionButtonCustom
                label={
                  LanguageReducer?.languageType
                    ?.ORDERS_PAYMENT_LINK_CREATE_PAYOUT_REQUEST
                }
                onClick={() => setOpenPayoutModal(true)}
              />
            }
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
          />
        </div>
      </Box>
      {openUpdateEmail && (
        <UpdateEmail
          open={openUpdateEmail}
          onClose={() => setOpenUpdateEmail(false)}
          OrderId={selectedOrderId}
          generateTotalProcessPaymentLink={generateTotalProcessPaymentLink}
        />
      )}
      {openPayoutModal && (
        <PayoutRequestModal
          open={openPayoutModal}
          onClose={() => setOpenPayoutModal(false)}
        />
      )}
    </>
  );
};

export default PaymentLink;
