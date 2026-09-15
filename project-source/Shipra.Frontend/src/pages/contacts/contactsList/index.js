import { Box, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { styleSheet } from "../../../assets/styles/style";
import { 
  usePagination,
  centerColumn,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  getElementByInnerHTML,
} from "../../../utilities/helpers/Helpers";

function ContactsList(props) {
  const { loading, allContacts, onContactClick } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [flag, setFlag] = useState(false);
  const [showContent, setShowContent] = useState(false);

  useEffect(() => {
    setFlag((prev) => !prev);
  }, [loading]);

  useEffect(() => {
    const selectedElement = getElementByInnerHTML("MUI X Missing license key");
    if (selectedElement) {
      setShowContent(true);
      selectedElement.style.display = "none";
    }
  }, [flag]);

  const columns = [
    {
      field: "Mobile",
      headerName: <Box sx={{ fontWeight: "600" }}>Mobile</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <Box 
          sx={{ cursor: "pointer", color: "blue", textDecoration: "underline" }}
          onClick={() => onContactClick(params.row.Mobile)}
        >
          <span style={styleSheet.tableText}>{params.row.Mobile}</span>
        </Box>
      ),
    },
    {
      field: "CustomerName",
      headerName: <Box sx={{ fontWeight: "600" }}>Name</Box>,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.CustomerName || params.row.Name || "-"}</span>
      ),
    },
    {
      field: "TotalOrders",
      headerName: <Box sx={{ fontWeight: "600" }}>Total Orders</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.TotalOrders}</span>
      ),
    },
  ];

  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  const rows = allContacts?.list ?? [];

  const calculatedHeight = windowHeight - navbarHeight - 67;

  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: calculatedHeight,
        "& .MuiDataGrid-main div": {
          opacity: showContent ? 1 : 0,
        },
      }}
    >
      <DataGridPro
        loading={loading}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        rows={rows}
        getRowId={(row) => row.Mobile}
        columns={columns}
        disableRowSelectionOnClick
        checkboxSelection
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />
    </Box>
  );
}

export default ContactsList;
