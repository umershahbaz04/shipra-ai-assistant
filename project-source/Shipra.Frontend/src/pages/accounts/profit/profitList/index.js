import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Avatar,
  Box,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  Stack,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { styleSheet } from "../../../../assets/styles/style";
import BatchOutScanModal from "../../../../components/modals/myCarrierModals/BatchOutScanModal";
import UpdateOutScanModal from "../../../../components/modals/myCarrierModals/UpdateOutscanModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import { usePagination } from "../../../../utilities/helpers/Helpers";
const rows = [
  {
    id: 1,
    order: "#123",
    trackingNo: "#554",
    date: "Oct 20, 22",
    lastName: "Roy",
    cod: "200",
    firstName: "Jason",
    total: "$2,00,000",
    customer: "Ahmed Faraz",
    remarks: "Good",
    phone: "91069281",
    age: 35,
    Status: "",
    payment: "",
    Item: "x2",
    Carrier: "Expo",
    action: "",
    status: "Completed",
  },
  {
    id: 2,
    order: "#123",
    date: "Oct 20, 22",
    lastName: "s",
    trackingNo: "#554",
    remarks: "Bad",
    cod: "400",
    firstName: "Salma",
    customer: "Junaid Ahmed",
    phone: "91069281",
    total: "$2,00,000",
    age: 35,
    Status: "",
    payment: "",
    Item: "x2",
    Carrier: "Expo",
    action: "",
    status: "Pending",
  },
  {
    id: 3,
    order: "#123",
    date: "Oct 20, 22",
    trackingNo: "#854",
    lastName: "Khan",
    firstName: "Ali",
    total: "$2,00,000",
    cod: "500",
    remarks: "Good",
    customer: "Ahsan khan",
    age: 35,
    Status: "",
    payment: "",
    Item: "x2",
    Carrier: "Expo",
    phone: "91069281",
    action: "",
    status: "Completed",
  },
  {
    id: 4,
    order: "#123",
    date: "Oct 20, 22",
    remarks: "Bad",
    lastName: "",
    firstName: "Buttler",
    trackingNo: "#528",
    total: "$2,00,000",
    cod: "600",
    age: 35,
    phone: "91069281",
    customer: "Kashif khan",
    Status: "",
    payment: "",
    Item: "x2",
    Carrier: "Expo",
    action: "",
    status: "Pending",
  },
  {
    id: 5,
    order: "#123",
    date: "Oct 20, 22",
    lastName: "Ahmed",
    firstName: "Raheel",
    cod: "700",
    phone: "91069281",
    trackingNo: "#594",
    total: "$2,00,000",
    age: 35,
    Status: "",
    remarks: "Good",
    payment: "",
    Item: "x2",
    Carrier: "Expo",
    customer: "Fahad Ahmed",
    action: "",
    status: "Completed",
  },
];

function ProfitList(props) {
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const [openUpdate, setOpenUpdate] = useState(false);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const columns = [
    {
      field: "order",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Order No"}</Box>,
      flex: 1,
    },
    {
      field: "trackingNo",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Tracking No"}</Box>,
      flex: 1,
    },
    {
      field: "status",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Status"}</Box>,
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row?.status}
            borderColor="rgba(0, 186, 119, 0.2)"
            color={params.row?.status === "Completed" ? "#00BA77" : "#FF9A23"}
            bgColor={
              params.row?.status === "Pending"
                ? "rgba(255, 154, 35, 0.1)"
                : "rgba(0, 186, 119, 0.1)"
            }
          />
        );
      },
    },
    {
      field: "name",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Driver"}</Box>,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              <Avatar
                sx={{
                  width: 25,
                  height: 25,
                  fontSize: "13px",
                  color: "var(--primary-color)",
                  background: "rgba(86, 58, 213, 0.3)",
                }}
              >
                {params.row.firstName?.slice(0, 1)}
              </Avatar>
              <Box>
                {params.row.firstName || ""} {params.row.lastName || ""}
              </Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      field: "customer",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Customer"}</Box>,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              <Avatar
                sx={{
                  width: 25,
                  height: 25,
                  fontSize: "13px",
                  color: "#D53AB3",
                  background: "rgba(213, 58, 179, 0.3)",
                }}
              >
                {params.row.customer?.slice(0, 1)}
              </Avatar>
              <Box>{params.row.customer || ""}</Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      field: "phone",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Phone"}</Box>,
      flex: 1,
    },
    {
      field: "remarks",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Remarks"}</Box>,
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row.remarks}
            color="#1E1E1E;"
            bgColor="#EAEAEA"
          />
        );
      },
    },
    {
      field: "orderDate",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Order Date"}</Box>,
      renderCell: (params) => (
        <Box>
          <b>Oct 20</b>, 22
        </Box>
      ),
      flex: 1,
    },
    {
      field: "cod",
      headerName: <Box sx={{ fontWeight: "600" }}> COD</Box>,
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row.cod}
            color="#1E1E1E;"
            bgColor="#EAEAEA"
          />
        );
      },
    },
    {
      field: "Action",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Action"}</Box>,
      renderCell: (params) => {
        return (
          <Box>
            <IconButton onClick={(e) => setAnchorEl(e.currentTarget)}>
              <MoreVertIcon />
            </IconButton>
          </Box>
        );
      },
      flex: 1,
    },
  ];
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  return (
    <Box sx={styleSheet.allOrderTable}>
      <DataGrid
        rowHeight={40}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        rows={rows}
        columns={columns}
        disableSelectionOnClick
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        checkboxSelection
      />
      <Menu
        anchorEl={anchorEl}
        id="power-search-menu"
        open={Boolean(anchorEl)}
        onClose={() => {
          setAnchorEl(null);
        }}
        PaperProps={{
          elevation: 0,
          sx: {
            overflow: "visible",
            filter: "drop-shadow(0px 2px 8px rgba(0,0,0,0.32))",
            mt: 1.5,
            "& .MuiAvatar-root": {
              width: 32,
              height: 32,
              ml: -0.5,
              mr: 1,
            },
            "&:before": {
              content: '""',
              display: "block",
              position: "absolute",
              top: 0,
              right: 14,
              width: 10,
              height: 10,
              bgcolor: "background.paper",
              transform: "translateY(-50%) rotate(45deg)",
              zIndex: 0,
            },
          },
        }}
        transformOrigin={{ horizontal: "right", vertical: "top" }}
        anchorOrigin={{ horizontal: "right", vertical: "bottom" }}
      >
        <Box sx={{ width: "180px" }}>
          <List disablePadding>
            <ListItem disablePadding>
              <ListItemButton
                onClick={() => {
                  setOpen(true);
                  setAnchorEl(null);
                }}
              >
                <ListItemText
                  primary={LanguageReducer?.languageType?.BATCH_OUTSCAN_TEXT}
                />
              </ListItemButton>
            </ListItem>
            <ListItem
              onClick={() => {
                setOpenUpdate(true);
                setAnchorEl(null);
              }}
              disablePadding
            >
              <ListItemButton>
                <ListItemText
                  primary={LanguageReducer?.languageType?.BATCH_UPDATE_TEXT}
                />
              </ListItemButton>
            </ListItem>
            {/* <ListItem
              onClick={() => {
                setAnchorEl(null);
                navigate("/create-order");
              }}
              disablePadding
            >
              <ListItemButton>
                <ListItemText primary={LanguageReducer?.languageType?.PRINT_TEXT} />
              </ListItemButton>
            </ListItem>
            <ListItem disablePadding>
              <ListItemButton>
                <ListItemText primary={LanguageReducer?.languageType?.EDIT_TEXT} />
              </ListItemButton>
            </ListItem>
            <ListItem disablePadding>
              <ListItemButton>
                <ListItemText primary={LanguageReducer?.languageType?.DEBERIF_TEXT} />
              </ListItemButton>
            </ListItem> */}
          </List>
        </Box>
      </Menu>
      <BatchOutScanModal open={open} setOpen={setOpen} {...props} />
      <UpdateOutScanModal
        open={openUpdate}
        setOpen={setOpenUpdate}
        {...props}
      />
    </Box>
  );
}
export default ProfitList;
