import React, { useEffect, useState } from "react";
import {
  Card,
  Dialog,
  DialogContent,
  DialogTitle,
  Button,
  Box,
  Stack,
  Avatar,
  DialogActions,
  TextField,
  MenuItem,
  IconButton,
  Autocomplete,
  Grid,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import { styleSheet } from "../../../assets/styles/style";
import Slide from "@mui/material/Slide";
import StatusBadge from "../../shared/statudBadge";
import { useSelector } from "react-redux";

import { DataGrid } from "@mui/x-data-grid";
import DatePicker from "react-datepicker";
import "../../../assets/styles/datePickerCustomStyles.css";

import { makeStyles } from "@material-ui/core";

import {
  amountFormat,
  ClipboardIcon,
  DescriptionBoxWithChild,
  DialerBox,
  rightColumn,
  centerColumn,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";

const useStyles = makeStyles({
  topScrollPaper: {
    alignItems: "flex-start",
  },
  topPaperScrollBody: {
    verticalAlign: "top",
  },
});

//#endregion
const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function OrdersModal(props) {
  let { open, onClose, data } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  //#region  table col
  const columns = [
    {
      field: "OrderNo",
      align: "center",
      headerAlign: "center",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_ORDER_NO
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center", cursor: "pointer" }}
            disableRipple
          >
            <>
              <Stack flexDirection={"row"} alignItems={"center"}>
                <Box sx={{ fontWeight: "bold" }}>{params.row.OrderNo}</Box>
                <ClipboardIcon text={params.row.OrderNo} />
              </Stack>
              <Box>{params.row.TrackingNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Store",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_STORE_INFO
          }
        </Box>
      ),
      minWidth: 140,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Box>{params.row.StoreName}</Box>
              <Box>
                <DialerBox phone={params.row.CustomerServiceNo} />
              </Box>
              <Box sx={{ fontSize: "10px" }}>{params.row?.SaleChannelName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Customer",
      align: "center",
      headerAlign: "center",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_CUSTOMER
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row.CustomerName}
                <DescriptionBoxWithChild>
                  <TableContainer>
                    <Table sx={{ minWidth: 275 }} aria-label="simple table">
                      <TableHead>
                        <TableRow>
                          <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                              // width: "110px",
                            }}
                            align="left"
                          >
                            Name
                          </TableCell>
                          <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                            }}
                          >
                            Mobile
                          </TableCell>
                          <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                              // width: "150px",
                            }}
                          >
                            Address
                          </TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        <TableRow
                          key={params.row.CustomerName}
                          sx={{
                            "&:last-child td, &:last-child th": { border: 0 },
                          }}
                        >
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                            }}
                            align="left"
                          >
                            {params.row.CustomerName}
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="right"
                          >
                            <DialerBox phone={params.row.Mobile1} />
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              // width: "150px",
                            }}
                          >
                            {params.row.CustomerFullAddress}
                          </TableCell>
                        </TableRow>
                      </TableBody>
                    </Table>
                  </TableContainer>
                </DescriptionBoxWithChild>
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Payment",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_PAYMENT_STATUS
          }
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
              <Box sx={{ fontWeight: "bold" }}>{params.row.PaymentMethod}</Box>
              <StatusBadge
                title={params.row.PaymentStatus}
                color={
                  params.row.PaymentStatus === "Unpaid" ? "#fff;" : "#fff;"
                }
                bgColor={
                  params.row.PaymentStatus === "Unpaid"
                    ? "#dc3545;"
                    : "#28a745;"
                }
              />
            </>
          </Box>
        );
      },
    },
    {
      field: "OrderTypeName",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_ORDER_TYPE
          }
        </Box>
      ),
      minWidth: 110,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack direction={"column"}>
            <StatusBadge
              title={params.row.OrderTypeName}
              color="#1E1E1E;"
              bgColor="#EAEAEA"
            />
            <Box>
              {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
            </Box>
          </Stack>
        );
      },
    },
    {
      field: "TrackingStatus",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_TRACKING_STATUS
          }
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row.TrackingStatus}
            color="#1E1E1E;"
            bgColor="#EAEAEA"
          />
        );
      },
    },

    {
      field: "Remarks",
      align: "center",
      headerAlign: "center",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DRIVER_RECIEVEABLE_REMARKS}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          params.row.Remarks && (
            <StatusBadge
              title={params.row.Remarks}
              color="#1E1E1E;"
              bgColor="#EAEAEA"
            />
          )
        );
      },
    },
    {
      field: "Discount",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_DISCOUNT
          }
        </Box>
      ),
      minWidth: 70,
      flex: 1,
      ...rightColumn,
      renderCell: (params) => {
        return <Box>{amountFormat(params.row.Discount)}</Box>;
      },
    },
    {
      field: "Amount",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DRIVER_RECIEVEABLE_AMOUNT}
        </Box>
      ),
      minWidth: 70,
      flex: 1,
      ...rightColumn,
      renderCell: (params) => {
        return <Box>{amountFormat(params.row.Amount)}</Box>;
      },
    },
  ];
  //#endregion
  const classes = useStyles();
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="lg"
        title={LanguageReducer?.languageType?.MY_CARRIER_RETURN_ORDERS}
      >
        <Box height={"500px"} pt={1}>
          <DataGrid
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            rows={data.list ? data?.list : []}
            getRowId={(row) => row.OrderNo}
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
}
export default OrdersModal;
