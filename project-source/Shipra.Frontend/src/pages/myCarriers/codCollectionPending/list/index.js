import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  Stack,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { styleSheet } from "../../../../assets/styles/style";
import UpdateOrderAmountModal from "../../../../components/modals/orderModals/updateOrderAmountModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import UtilityClass from "../../../../utilities/UtilityClass";
import { EnumNavigateState, EnumRoutesUrls } from "../../../../utilities/enum";
import {
  ActionButtonCustom,
  ClipboardIcon,
  CodeBox,
  DialerBox,
  amountFormat,
  centerColumn,
  navbarHeight,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  useNavigateSetState,
  usePagination,
} from "../../../../utilities/helpers/Helpers";

function CODCollectionPendingList(props) {
  const { rows, getOrdersRef, resetRowRef, loading, getAll, isFilterOpen } =
    props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const [openUpdate, setOpenUpdate] = useState(false);
  const [selectedRowData, setSelectedRowData] = useState();
  const navigate = useNavigate();
  const { setNavigateState } = useNavigateSetState();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleUpdate = (data) => {
    setSelectedRowData(data);
  };
  useEffect(() => {
    if (selectedRowData) {
      setOpenUpdate(true);
    }
  }, [selectedRowData]);
  const columns = [
    {
      field: "OrderNo",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_ORDER_NO}
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
            sx={{ textAlign: "center", cursor: "pointer" }}
            disableRipple
          >
            <>
              <Stack flexDirection={"row"} alignItems={"center"}>
                <CodeBox
                  title={params.row.OrderNo}
                  onClick={() => {
                    setNavigateState(EnumRoutesUrls.ORDERS, {
                      [EnumNavigateState.DELIVERY_TASKS.pageName]: {
                        [EnumNavigateState.DELIVERY_TASKS.state
                          .selectedOrderNo]: params.row.OrderNo,
                      },
                    });
                  }}
                />
                <ClipboardIcon text={params.row?.OrderNo} />
              </Stack>
              <Box>{params.row?.CarrierTrackingNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "CarrierName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_CARRIER_NAME}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
    },
    {
      field: "Store",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_STORE_REF}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Box>{params.row.StoreName}</Box>
              <Box>
                <DialerBox phone={params.row?.CustomerServiceNo} />
              </Box>
              <Box sx={{ fontSize: "10px" }}>{params.row?.SaleChannelName}</Box>
            </>
          </Box>
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
              ?.MY_CARRIER_COD_PENDING_TRACKING_STATUS
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
      field: "Payment",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_PAYMENT}
        </Box>
      ),
      minWidth: 100,
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
      headerAlign: "center",
      align: "center",
      field: "PaymentMethod",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_PAYMENT_METHOD}
        </Box>
      ),
      flex: 1,
    },

    {
      ...centerColumn,
      field: "orderDate",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_ORDER_DATE}
        </Box>
      ),
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
        </Box>
      ),
      flex: 1,
    },
    {
      ...rightColumn,
      field: "Amount",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_AMOUNT}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => <Box>{amountFormat(params?.row.Amount)}</Box>,
    },

    {
      ...centerColumn,
      field: "Action",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_ACTION}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Stack direction={"row"} gap={1}>
            <ActionButtonCustom
              onClick={() => handleUpdate(params?.row)}
              label={
                LanguageReducer?.languageType
                  ?.MY_CARRIER_COD_PENDING_UPDATE_AMOUNT
              }
              width={"50px"}
            />
          </Stack>
        );
      },
      flex: 1,
    },
  ];

  const [selectionModel, setSelectionModel] = useState([]);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
    props.setSelectedItems(oNos);
    getOrdersRef.current = oNos;
  };

  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 152.5
    : windowHeight - navbarHeight - 67;

  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: calculatedHeight,
      }}
    >
      <DataGrid
        loading={loading}
        rowHeight={40}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        getRowId={(row) => row.OrderNo}
        rows={rows?.list ? rows.list : []}
        columns={columns}
        disableSelectionOnClick
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
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
      <UpdateOrderAmountModal
        setSelectedRowData={setSelectedRowData}
        open={openUpdate}
        setOpen={setOpenUpdate}
        getAll={getAll}
        selectedRowData={selectedRowData}
        codText={
          LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_NEW_COD_AMOUNT
        }
      />
    </Box>
  );
}
export default CODCollectionPendingList;
