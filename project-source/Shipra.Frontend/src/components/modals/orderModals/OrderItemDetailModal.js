import {
  TableContainer,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Dialog,
  DialogTitle,
  DialogContent,
  Box,
  Paper,
  Slide,
  DialogActions,
  Button,
  Stack,
} from "@mui/material";
import React from "react";
import { useSelector } from "react-redux";
import { EnumOrderType } from "../../../utilities/enum";
import { DataGrid } from "@mui/x-data-grid";
import { styleSheet } from "../../../assets/styles/style";
import {
  ClipboardIcon,
  CodeBox,
  amountFormat,
  rightColumn,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
export default function OrderItemDetailModal(props) {
  const { open, onClose, data } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const columns = [
    {
      field: "ProductName",
      headerName: <Box sx={{ fontWeight: "600" }}>Product Name</Box>,
      maxWidth: 150,
      flex: 1,
      hide: data[0]?.OrderTypeId == EnumOrderType.Regular && true,
      renderCell: (params) => {
        return (
          <Stack direction={"column"} disableRipple>
            <>
              <Box>{params.row.ProductName}</Box>
            </>
          </Stack>
        );
      },
    },
    {
      field: "ProductStockSku",
      headerName: <Box sx={{ fontWeight: "600" }}>Product Name</Box>,
      maxWidth: 150,
      flex: 1,
      hide: data[0]?.OrderTypeId == EnumOrderType.Regular && true,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Stack sx={{ textAlign: "center" }} direction={"column"}>
              <CodeBox hasColor={false} title={params.row.ProductStockSku} />
            </Stack>
            <ClipboardIcon text={params.row.ProductStockSku} />
          </Stack>
        );
      },
    },

    {
      field: "OrderItemDescription",
      headerName: <Box sx={{ fontWeight: "600" }}>Description</Box>,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => (params.value ? params.value : "N/A"),
    },
    {
      field: "OrderItemQuantity",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}> Quantity</Box>,
      maxWidth: 90,
      flex: 1,
      hide: data[0]?.OrderTypeId == EnumOrderType.Regular && true,
      renderCell: (params) => (
        <Box>
          <CodeBox hasColor={false} title={params.row.OrderItemQuantity} />
        </Box>
      ),
    },
    {
      field: "OrderItemDiscount",
      ...rightColumn,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Discount"}</Box>,
      maxWidth: 90,
      flex: 1,
      hide: data[0]?.OrderTypeId == EnumOrderType.Regular && true,
      renderCell: (params) => <>{amountFormat(params.row.OrderItemDiscount)}</>,
    },

    {
      field: "Price",
      ...rightColumn,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Amount"}</Box>,
      maxWidth: 120,
      flex: 1,
      renderCell: (params) => <>{amountFormat(params.row.Price)}</>,
    },
  ];
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="lg"
      title={"Order Items"}
      height={"50vh"}
    >
      <Box height="100%">
        <DataGrid
          style={{ minHeight: "100%" }}
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          rows={data}
          getRowId={(row) => row.OrderItemId}
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
  );
}
