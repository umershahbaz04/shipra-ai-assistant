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
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { styleSheet } from "../../../../assets/styles/style";
import BatchOutScanModal from "../../../../components/modals/myCarrierModals/BatchOutScanModal";
import UpdateOutScanModal from "../../../../components/modals/myCarrierModals/UpdateOutscanModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import UtilityClass from "../../../../utilities/UtilityClass";
import {
  ClipboardIcon,
  CodeBox,
  DescriptionBox,
  DialerBox,
  rightColumn,
  centerColumn,
  DescriptionBoxWithChild,
  amountFormat,
} from "../../../../utilities/helpers/Helpers";

function CODCollectionMyCarrierList(props) {
  const { rows, getOrdersRef, resetRowRef, loading } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const [openUpdate, setOpenUpdate] = useState(false);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const columns = [
    {
      field: "OrderNo",

      headerName: <Box sx={{ fontWeight: "bold" }}>Order No.</Box>,
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Stack direction={"row"} sx={{ alignItems: "center" }}>
                <CodeBox title={params.row.OrderNo} />
                <ClipboardIcon text={params.row.OrderNo} />
                <DescriptionBox
                  description={params.row.Remarks}
                  title="Remarks"
                />
              </Stack>
            </>
          </Box>
        );
      },
    },
    {
      field: "Store",
      headerName: <Box sx={{ fontWeight: "600" }}>Store</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Box>{params.row.StoreName}</Box>
              <Box>
                <DialerBox phone={params.row.CustomerServiceNo} />
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Customer",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Customer"}</Box>,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box display={"flex"} flexDirection={"column"} disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row.Customer}
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
                            }}
                          >
                            Mobile 2
                          </TableCell>
                          {/* <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                              // width: "150px",
                            }}
                          >
                            Address
                          </TableCell> */}
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
                            {params.row.Customer}
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="left"
                          >
                            <DialerBox phone={params.row.Mobile1} />
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="left"
                          >
                            <DialerBox phone={params.row.Mobile2} />
                          </TableCell>
                          {/* <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              // width: "150px",
                            }}
                          >
                            {params.row.CustomerFullAddress}
                          </TableCell> */}
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
      field: "TrackingStatus",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Tracking Status"}</Box>,
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
      headerName: <Box sx={{ fontWeight: "600" }}>Payment</Box>,
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
    {
      field: "orderDate",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Order Date"}</Box>,
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "Amount",
      ...rightColumn,
      headerName: <Box sx={{ fontWeight: "600" }}>{" Ammount"}</Box>,
      minWidth: 70,
      flex: 1,
      renderCell: (params) => {
        <>{amountFormat(params.row?.Amount)}</>;
      },
    },

    {
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACTION}
        </Box>
      ),
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
      hide: true,
    },
  ];
  return (
    <Box sx={styleSheet.allOrderTable}>
      <DataGrid
        loading={loading}
        rowHeight={40}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        getRowId={(row) => row?.RowNum}
        rows={rows?.list ? rows.list : []}
        columns={columns}
        disableSelectionOnClick
        pageSize={10}
        rowsPerPageOptions={[10]}
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
export default CODCollectionMyCarrierList;
