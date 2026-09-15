import React, { useEffect, useState } from "react";
import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { GetOrdersByContactMobile } from "../../../api/AxiosInterceptors";
import UtilityClass from "../../../utilities/UtilityClass";
import { styleSheet } from "../../../assets/styles/style";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";

const ContactOrdersModal = ({ open, onClose, mobileNumber }) => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (open && mobileNumber) {
      fetchOrders();
    }
  }, [open, mobileNumber]);

  const fetchOrders = async () => {
    setLoading(true);
    try {
      const response = await GetOrdersByContactMobile({ MobileNumber: mobileNumber });
      if (response?.data?.result?.list) {
        setOrders(response.data.result.list);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  const columns = [
    {
      field: "OrderNo",
      headerName: <Box sx={{ fontWeight: "600" }}>Order Number</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.OrderNo}</span>
      ),
    },
    {
      field: "CustomerName",
      headerName: <Box sx={{ fontWeight: "600" }}>Name</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.CustomerName || "-"}</span>
      ),
    },
    {
      field: "Description",
      headerName: <Box sx={{ fontWeight: "600" }}>Description</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.Description || "-"}</span>
      ),
    },
    {
      field: "CustomerFullAddress",
      headerName: <Box sx={{ fontWeight: "600" }}>Full Address</Box>,
      minWidth: 250,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.CustomerFullAddress || "-"}</span>
      ),
    },
    {
      field: "OrderDate",
      headerName: <Box sx={{ fontWeight: "600" }}>Order Date</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}</span>
      ),
    },
  ];

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      title={`Orders for ${mobileNumber}`}
      maxWidth="md"
    >
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: 400,
          width: "100%",
          mt: 2,
        }}
      >
        <DataGrid
          loading={loading}
          rows={orders}
          columns={columns}
          getRowId={(row) => row.OrderNo}
          disableRowSelectionOnClick
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
        />
      </Box>
    </ModalComponent>
  );
};

export default ContactOrdersModal;
