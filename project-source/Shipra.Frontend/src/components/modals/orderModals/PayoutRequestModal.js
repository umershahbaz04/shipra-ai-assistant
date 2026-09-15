import { usePagination } from "@mui/lab";
import {
  Box,
  Button,
  InputLabel,
  Stack,
  Tooltip,
  Typography,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  CreatePayoutRequest,
  GetAllOrderPaymentLinksForModal,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import {
  centerColumn,
  ClipboardIcon,
  fetchMethod,
  getTrimValue,
  GridContainer,
  GridItem,
  purple,
  useGetWindowHeight,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import StatusBadge from "../../shared/statudBadge";

const PayoutRequestModal = (props) => {
  const { open, onClose } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const today = UtilityClass.todayDate();
  const sevenDaysAfter = new Date(today);
  sevenDaysAfter.setDate(sevenDaysAfter.getDate() + 7);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const calculatedHeight = windowHeight - 273;
  const [loading, setLoading] = useState(false);
  const [allPaymentLinkForModal, setAllPaymentLinkForModal] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const handleRequestPayout = async () => {
    setLoading(true);
    try {
      const response = await CreatePayoutRequest(startDate, endDate);

      if (response?.data?.isSuccess) {
        successNotification("Payout request successful.");
        onClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(response.data.errors);
      }
    } catch (error) {
      errorNotification("An error occurred while requesting a payout:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleFilter = () => {
    getAllOrderPaymentLinksForModal(startDate, endDate, true);
  };

  const getAllOrderPaymentLinksForModal = async (start, end) => {
    setIsLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetAllOrderPaymentLinksForModal(start, end, true)
      );
      if (response.isSuccess) {
        setAllPaymentLinkForModal(response?.result?.list);
      }
    } catch (error) {
      console.error("Error fetching:", error);
    }
    setIsLoading(false);
  };
  const columns = [
    {
      field: "orderNo",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_ORDER_NO}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box sx={{ fontWeight: "bold" }}>{row.orderNo}</Box>;
      },
    },
    {
      field: "Order Links",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_ORDER_LINK}
        </Box>
      ),
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box display="flex" alignItems="center">
            <Box>
              <Tooltip title={row.paymentLinkUrl} arrow>
                <Box
                  sx={{
                    whiteSpace: "nowrap",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    maxWidth: 200,
                  }}
                >
                  {row.paymentLinkUrl}
                </Box>
              </Tooltip>
            </Box>
            <Box>
              <ClipboardIcon text={row.paymentLinkUrl} />
            </Box>
          </Box>
        );
      },
    },
    {
      field: "Paymentstatus",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_PAYMENT_STATUS}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <StatusBadge
                title={getTrimValue(params.row.statusName)}
                color={
                  getTrimValue(params.row.statusName) === "unpaid"
                    ? "#fff;"
                    : "#fff;"
                }
                bgColor={
                  getTrimValue(params.row.statusName) === "paid"
                    ? "#28a745;"
                    : "#dc3545;"
                }
              />
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_CREATED_ON}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row?.createdOn)}</Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "schedulepayment",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.ORDERS_PAYMENT_LINK_PAYMNT_SEHEDULE_DATE
          }
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>
            {UtilityClass.convertUtcToLocalAndGetDate(row?.scheduledPayoutDate)}
          </Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "PaidOn",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_PAID_ON}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row?.paidOn)}</Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Amount",
      minWidth: 100,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_PAYMNT_Amount}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.amount}</Box>;
      },
      flex: 1,
    },
  ];

  useEffect(() => {
    setStartDate(today);
    setEndDate(sevenDaysAfter);
    getAllOrderPaymentLinksForModal(today, sevenDaysAfter);
  }, []);

  const totalAmount = allPaymentLinkForModal.reduce(
    (sum, item) => sum + item.amount,
    0
  );
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="lg"
        title={
          LanguageReducer?.languageType
            ?.ORDERS_PAYMENT_LINK_CREATE_PAYOUT_REQUEST
        }
        actionBtn={
          <ModalButtonComponent
            title={
              LanguageReducer?.languageType
                ?.ORDERS_PAYMENT_LINK_CREATE_PAYOUT_REQUEST
            }
            bg={purple}
            loading={loading}
            onClick={(e) => handleRequestPayout()}
          />
        }
      >
        <GridContainer spacing={1}>
          <GridItem sm={12} md={3} lg={3}>
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
            />
          </GridItem>
          <GridItem sm={12} md={3} lg={3}>
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
            />
          </GridItem>
          <GridItem sm={12} md={3} lg={3}>
            <Stack
              alignItems="flex-end"
              direction="row"
              spacing={1}
              sx={{
                ...styleSheet.filterButtonMargin,
                height: "100%",
              }}
            >
              <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
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
                <Typography variant="body2" fontWeight={600}>
                  Total Value : {totalAmount}
                </Typography>
              </Box>
            </Stack>
          </GridItem>
        </GridContainer>
        <Box
          sx={{
            marginTop: "8px",
            ...styleSheet.allOrderTable,
            height: calculatedHeight,
          }}
        >
          <DataGrid
            loading={isLoading}
            rowHeight={40}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row.rowNum}
            rows={allPaymentLinkForModal || []}
            columns={columns}
            disableSelectionOnClick
            pagination
            page={currentPage}
            pageSize={pageSize}
            rowsPerPageOptions={[5, 10, 15, 25]}
            paginationMode="client"
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
          />
        </Box>
      </ModalComponent>
    </>
  );
};

export default PayoutRequestModal;
