import { Box, Stack } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useSelector } from "react-redux";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { EnumOrderType } from "../../../utilities/enum";
import {
  ClipboardIcon,
  CodeBox,
  amountFormat,
  rightColumn,
  usePagination,
} from "../../../utilities/helpers/Helpers";

export default function OrderDrafItemDetailModal(props) {
  const { open, onClose, data, rowData } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const columns = [
    {
      field: "ProductName",
      headerName: <Box sx={{ fontWeight: "600" }}>Product Name</Box>,
      maxWidth: 150,
      flex: 1,
      hide: data.orderTypeId == EnumOrderType.Regular && true,
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
      headerName: <Box sx={{ fontWeight: "600" }}>Product SKU</Box>,
      maxWidth: 150,
      flex: 1,
      hide: data.orderTypeId == EnumOrderType.Regular && true,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Stack sx={{ textAlign: "center" }} direction={"column"}>
              <CodeBox hasColor={false} title={params.row.StockSku} />
            </Stack>
            <ClipboardIcon text={params.row.StockSku} />
          </Stack>
        );
      },
    },

    {
      field: "Description",
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
      hide: data.orderTypeId == EnumOrderType.Regular && true,
      renderCell: (params) => (
        <Box>
          <CodeBox hasColor={false} title={params.row.Quantity} />
        </Box>
      ),
    },
    {
      field: "OrderItemDiscount",
      ...rightColumn,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Discount"}</Box>,
      maxWidth: 90,
      flex: 1,
      hide: data.orderTypeId == EnumOrderType.Regular && true,
      renderCell: (params) => <>{amountFormat(params.row.Discount)}</>,
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
          rows={rowData}
          getRowId={(row) => row.rowNum}
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
