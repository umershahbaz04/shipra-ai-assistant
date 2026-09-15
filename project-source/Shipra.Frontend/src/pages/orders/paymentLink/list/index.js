import SyncIcon from "@mui/icons-material/Sync";
import { LoadingButton } from "@mui/lab";
import { Box, Link, Tooltip } from "@mui/material";
import React, { useState } from "react";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import { FetchPaybylinkbyServiceUUId } from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import StatusBadge from "../../../../components/shared/statudBadge";
import { EnumChangeFilterModelApiUrls } from "../../../../utilities/enum";
import {
  centerColumn,
  ClipboardIcon,
  getTrimValue,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import { successNotification } from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";
import { useSelector } from "react-redux";

const PaymentLinkList = (props) => {
  const { isFilterOpen, isLoading, allPaymentLink } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [loading, setLoading] = useState({});
  const handleRefresh = async (data) => {
    try {
      setLoading((prev) => ({ ...prev, [data.rowNum]: true }));
      const response = await FetchPaybylinkbyServiceUUId(data?.paymentLinkId);
      if (response.isSuccess) {
        successNotification("Status Refresh Successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
    } finally {
      setLoading((prev) => ({ ...prev, [data.rowNum]: false }));
    }
  };

  const columns = [
    {
      field: "orderNo",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_ORDER_NO}
        </Box>
      ),
      minWidth: 130,
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
      minWidth: 250,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box display="flex" alignItems="center">
            <Box
              sx={{
                whiteSpace: "nowrap",
                overflow: "hidden",
                textOverflow: "ellipsis",
                maxWidth: 180,
              }}
            >
              <Tooltip title={row.trackingUrl} arrow>
                <Link
                  href={row.trackingUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  sx={{
                    textDecoration: "none",
                    display: "block", // Ensure block-level element for ellipsis
                  }}
                >
                  {row.trackingUrl}
                </Link>
              </Tooltip>
            </Box>
            <Box>
              <ClipboardIcon text={row.trackingUrl} />
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
                title={params.row.statusName}
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
      minWidth: 150,
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
      field: "PaymentSeheduleeDate",
      minWidth: 150,
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
      field: "PaymentReleaseDate",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.ORDERS_PAYMENT_LINK_PAYMENT_RELEASE_DATE
          }
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>
            {UtilityClass.convertUtcToLocalAndGetDate(row?.paymentReleaseDate)}
          </Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "PaidOn",
      minWidth: 150,
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
      field: "Action",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>
            {row?.allowRefreshPaymentStatus && (
              <Tooltip
                title={loading ? "Refresh Payment Status" : "Refreshing..."}
                arrow
              >
                <LoadingButton
                  onClick={() => handleRefresh(row)}
                  loading={loading[row.rowNum]}
                  variant="contained"
                  sx={{
                    backgroundColor: "#1976d2",
                    color: "#fff",
                    padding: "2px 6px !important",
                    minWidth: "20px !important",
                    minHeight: "20px !important",
                    "&:hover": {
                      backgroundColor: "#1976d2",
                    },
                  }}
                >
                  {<SyncIcon fontSize={"small"} />}
                </LoadingButton>
              </Tooltip>
            )}
          </Box>
        );
      },
      flex: 1,
    },
  ];
  const calculatedHeightTable = isFilterOpen
    ? windowHeight - 405
    : windowHeight - 320;
  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: calculatedHeightTable,
        paddingBottom: "20px",
      }}
    >
      <DataGridProComponent
        rowPadding={8}
        rows={allPaymentLink}
        columns={columns}
        loading={isLoading}
        headerHeight={40}
        getRowId={(row) => row.rowNum}
        checkboxSelection={false}
        disableSelectionOnClick
        rowsCount={allPaymentLink.length}
        paginationChangeMethod={allPaymentLink}
        paginationMethodUrl={
          EnumChangeFilterModelApiUrls.GET_ALL_ORDER_PAYMENT_LINK.url
        }
        defaultRowsPerPage={
          EnumChangeFilterModelApiUrls.GET_ALL_ORDER_PAYMENT_LINK.length
        }
        height={calculatedHeightTable}
      />
    </Box>
  );
};

export default PaymentLinkList;
