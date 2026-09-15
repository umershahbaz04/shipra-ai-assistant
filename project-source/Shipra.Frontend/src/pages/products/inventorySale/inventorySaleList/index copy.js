import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";
import DisplaySettingsIcon from "@mui/icons-material/DisplaySettings";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Avatar,
  Box,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  Stack,
  Typography,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import OrderDetailModal from "../../../../components/modals/orderModals/OrderDetailModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import OrderItemDetailModal from "../../../../components/modals/orderModals/OrderItemDetailModal";
import {
  CodeBox,
  amountFormat,
  centerColumn,
  rightColumn,
} from "../../../../utilities/helpers/Helpers";

function InventoryList123(props) {
  const [anchorEl, setAnchorEl] = React.useState(null);
  const { alInventorySale, getOrdersRef, resetRowRef, loading } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [orderIteminfoModal, setOrderIteminfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });

  const columns = [
    {
      field: "OrderNo",
      ...centerColumn,
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDER_TEXT}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: ({ row }) => {
        return <CodeBox title={row.OrderNo} />;
      },
    },
    {
      field: "StationName",
      headerName: <Box sx={{ fontWeight: "600" }}>Station</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{row.StationName}</Box>;
      },
    },
    {
      field: "FullFillmentStatus",
      headerName: <Box sx={{ fontWeight: "600" }}>FullFillmentStatus</Box>,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return <Box>{params.row.FullFillmentStatus}</Box>;
      },
    },
    {
      field: "Quantity",
      // headerAlign: "center",
      // align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>Quantity</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return <CodeBox title={params.row.Quantity} />;
      },
    },
    {
      field: "Discount",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>Discount</Box>,
      minWidth: 80,
      flex: 1,
      renderCell: (params) => {
        return <CodeBox title={amountFormat(params.row.Discount)} />;
      },
    },
    {
      field: "Price",
      headerAlign: "center",
      align: "center",
      minWidth: 80,
      flex: 1,
      headerName: <Box sx={{ fontWeight: "600" }}>Price</Box>,
      renderCell: (params) => (
        <CodeBox title={amountFormat(params.row.Price)} />
      ),
    },
  ];
  const [selectionModel, setSelectionModel] = useState([]);

  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
    getOrdersRef.current = oNos;
  };
  ////////////

  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
    }
  }, [resetRowRef.current]);

  return (
    <Box sx={styleSheet.allOrderTable}>
      <DataGrid
        getRowHeight={() => "auto"}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        getRowId={(row) => row.OrderNo}
        rows={alInventorySale}
        columns={columns}
        disableSelectionOnClick
        pageSize={10}
        rowsPerPageOptions={[10]}
        checkboxSelection
        selectionModel={selectionModel}
        onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
        loading={loading}
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
        <Box sx={{ width: "200px" }}>
          <List disablePadding>
            <ListItem
              disablePadding
              onClick={() => {
                setAnchorEl(null);
              }}
            >
              <ListItemButton>
                <ListItemIcon sx={{ minWidth: "30px" }}>
                  <DisplaySettingsIcon />
                </ListItemIcon>
                <ListItemText primary="Order Detail" />
              </ListItemButton>
            </ListItem>
            <ListItem disablePadding>
              <ListItemButton>
                <ListItemIcon sx={{ minWidth: "30px" }}>
                  <DeleteOutlineIcon />
                </ListItemIcon>
                <ListItemText primary="Delete Order" />
              </ListItemButton>
            </ListItem>
          </List>
        </Box>
      </Menu>
      {infoModal.data?.result && (
        <OrderDetailModal
          data={infoModal?.data?.result}
          open={infoModal.open}
          onClose={() => setInfoModal((prev) => ({ ...prev, open: false }))}
        />
      )}
      {orderIteminfoModal.data?.result && (
        <OrderItemDetailModal
          onClose={() =>
            setOrderIteminfoModal((prev) => ({ ...prev, open: false }))
          }
          data={orderIteminfoModal?.data?.result}
          open={orderIteminfoModal.open}
        />
      )}
    </Box>
  );
}
export default InventoryList;
