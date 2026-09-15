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
  DialogActions,
  Button,
} from "@mui/material";
import React from "react";
import { useSelector } from "react-redux";
import { EnumOrderType } from "../../../utilities/enum";
import { DataGrid } from "@mui/x-data-grid";
import StatusBadge from "../../shared/statudBadge";
import UtilityClass from "../../../utilities/UtilityClass";
import { styleSheet } from "../../../assets/styles/style";
import { makeStyles } from "@material-ui/core";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { usePagination } from "../../../utilities/helpers/Helpers";

const useStyles = makeStyles({
  topScrollPaper: {
    alignItems: "flex-start",
  },
  topPaperScrollBody: {
    verticalAlign: "top",
  },
});
export default function ReturnReportShipmentDetailModal(props) {
  const { open, onClose, data } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const classes = useStyles();

  const columns = [
    {
      field: "OrderNo",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.CARRIER_RETURNS_ORDER}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }} disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{params.row.OrderNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Store",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.CARRIER_RETURNS_STORE}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }} disableRipple>
            <>
              <Box>{params.row.StoreName}</Box>
              <Box>{params.row.CustomerServiceNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "DropOfAddress",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.CARRIER_RETURNS_DROP_OF_ADDRESS}
        </Box>
      ),
      minWidth: 250,
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
              <Box sx={{ fontWeight: "bold" }}>{params.row.CustomerName}</Box>
              <Box sx={{ fontWeight: "bold" }}>{params.row.Mobile1}</Box>
              <Box noWrap={false}>{params.row.CustomerFullAddress}</Box>
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
          {LanguageReducer?.languageType?.CARRIER_RETURNS_PAYMENT}
        </Box>
      ),
      minWidth: 150,
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
    // {
    //   field: "FulfillmentStatus",
    //   headerAlign: "center",
    //   align: "center",
    //   headerName: <Box sx={{ fontWeight: "600" }}>Fulfillment Status</Box>,
    //   minWidth: 150,
    //   flex: 1,
    //   renderCell: (params) => {
    //     return (
    //       <Box
    //         display={"flex"}
    //         flexDirection={"column"}
    //         justifyContent={"center"}
    //         sx={{ textAlign: "center" }}
    //         disableRipple
    //       >
    //         <>
    //           <StatusBadge
    //             title={params.row.FullFillmentStatus}
    //             color={
    //               params.row.FullFillmentStatus === "UnFulfillment"
    //                 ? "#fff;"
    //                 : "#fff;"
    //             }
    //             bgColor={
    //               params.row.FullFillmentStatus === "UnFulfillment"
    //                 ? "#dc3545;"
    //                 : "#28a745;"
    //             }
    //           />
    //         </>
    //       </Box>
    //     );
    //   },
    // },
    {
      field: "orderDate",
      headerAlign: "center",
      align: "center",
      minWidth: 90,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.CARRIER_RETURNS_ORDER_DATE}
        </Box>
      ),
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
        </Box>
      ),
    },
    {
      field: "Amount",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Amount"}</Box>,
      minWidth: 70,
      flex: 1,
    },
    {
      field: "TrackingStatus",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.CARRIER_RETURNS_TRACKING_STATUS}
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
      field: "OrderTypeName",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.CARRIER_RETURNS_ORDER_TYPE}
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row.OrderTypeName}
            color="#1E1E1E;"
            bgColor="#EAEAEA"
          />
        );
      },
    },
    {
      field: "ItemsCount",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.CARRIER_RETURNS_ITEM_COUNT}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return <Box>{params.row.ItemsCount}</Box>;
      },
    },

    // {
    //   field: "Action",
    //   headerName: (
    //     <Box sx={{ fontWeight: "600" }}>
    //       {" "}
    //       {LanguageReducer?.languageType?.ACTION}
    //     </Box>
    //   ),
    //   renderCell: (params) => {
    //     return (
    //       <Box>
    //         <IconButton
    //           onClick={(e) => handleActionButton(e.currentTarget, params.row)}
    //         >
    //           <MoreVertIcon />
    //         </IconButton>
    //       </Box>
    //     );
    //   },
    //   minWidth: 60,
    //   flex: 1,
    // },
  ];
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="lg"
      fullWidth
      title={"Carrier Return Shipments"}
    >
      <Box height={500} pt={1}>
        <DataGrid
          getRowHeight={() => "auto"}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          rows={data}
          getRowId={(row) => row.OrderId}
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
