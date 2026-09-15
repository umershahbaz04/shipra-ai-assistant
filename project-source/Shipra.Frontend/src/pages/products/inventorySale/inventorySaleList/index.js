import KeyboardArrowDown from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUp from '@mui/icons-material/KeyboardArrowUp';
import {
  Box,
  Checkbox,
  Collapse,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  Typography,
} from "@mui/material";
import { Stack } from "@mui/system";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import OrderDetailModal from "../../../../components/modals/orderModals/OrderDetailModal";
import OrderItemDetailModal from "../../../../components/modals/orderModals/OrderItemDetailModal";
import { EnumNavigateState, EnumRoutesUrls } from "../../../../utilities/enum";
import {
  ClipboardIcon,
  CodeBox,
  amountFormat,
  grey,
  greyBorder,
  selectedRow,
  useGetWindowHeight,
  navbarHeight,
  useNavigateSetState,
  useClientSubscriptionReducer,
  CicrlesLoading,
} from "../../../../utilities/helpers/Helpers";

function InventoryList(props) {
  const [anchorEl, setAnchorEl] = useState(null);
  const {
    alInventorySale,
    getOrdersRef,
    resetRowRef,
    loading,
    setSelectedRows,
    selectedRows,
    isFilterOpen,
  } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
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
  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 130 - 150
    : windowHeight - navbarHeight - 130;
  const [selected, setSelected] = useState([]);
  const isSelected = (id) => selected.indexOf(id) !== -1;
  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
      setSelectedRows([]);
    }
  }, [resetRowRef.current]);

  function Row(props) {
    const { row } = props;
    const [open, setOpen] = useState(false);
    const labelId = `enhanced-table-checkbox-${row?.OrderNo}`;
    useEffect(() => {
      getOrdersRef.current = selectedRows;
    }, [selectedRows]);
    const { setNavigateState } = useNavigateSetState();

    return (
      <>
        <TableRow
          sx={{
            "& > *": { borderBottom: "unset" },
            background:
              selectedRows.some((data) => data === row.OrderNo) && selectedRow,
            borderBottom: "0px",
          }}
        >
          <TableCell>
            <Checkbox
              color="primary"
              checked={selectedRows.some((data) => data === row.OrderNo)}
              onChange={(e) => {
                const val = e.target.checked;
                if (val) {
                  if (!selectedRows.some((data) => data === row.OrderNo)) {
                    setSelectedRows([...selectedRows, row.OrderNo]);
                  }
                } else {
                  const _filteredRowsData = selectedRows.filter(
                    (data) => data !== row.OrderNo,
                  );
                  setSelectedRows(_filteredRowsData);
                }
              }}
              inputProps={{
                "aria-labelledby": labelId,
              }}
            />
          </TableCell>
          <TableCell component="th" scope="row">
            <Stack flexDirection={"row"} alignItems={"center"}>
              <CodeBox
                title={row?.OrderNo}
                onClick={() => {
                  setNavigateState(EnumRoutesUrls.ORDERS, {
                    [EnumNavigateState.DELIVERY_TASKS.pageName]: {
                      [EnumNavigateState.DELIVERY_TASKS.state.selectedOrderNo]:
                        row?.OrderNo,
                    },
                  });
                  // handleCopyToClipBoard(params.row.OrderNo);
                }}
              />
              <ClipboardIcon text={row?.OrderNo} />
            </Stack>
          </TableCell>
          <TableCell align="left">{row?.StationName}</TableCell>
          <TableCell align="left">{row?.FullFillmentStatus}</TableCell>
          <TableCell align="center">
            <Stack
              justifyContent={"center"}
              alignItems={"center"}
              direction={"row"}
            >
              <Box>{row?.Quantity}</Box>
              <Box>
                <IconButton
                  aria-label="expand row"
                  onClick={() => setOpen(!open)}
                >
                  {open ? (
                    <KeyboardArrowUp sx={{ fontSize: 14 }} />
                  ) : (
                    <KeyboardArrowDown sx={{ fontSize: 14 }} />
                  )}
                </IconButton>
              </Box>
            </Stack>
          </TableCell>
          <TableCell align="right">{amountFormat(row?.Discount)}</TableCell>
          <TableCell align="right">{amountFormat(row?.Price)}</TableCell>
        </TableRow>
        <TableRow>
          <TableCell
            style={{ paddingBottom: 0, paddingTop: 0, background: "white" }}
            colSpan={12}
          >
            <Collapse in={open} timeout="auto" unmountOnExit>
              <Box sx={{ margin: 1 }}>
                <Typography variant="h5" gutterBottom component="div">
                  Order Items
                </Typography>
                <Table size="small" aria-label="history">
                  <TableHead>
                    <TableRow>
                      <TableCell sx={{ fontWeight: "bold" }}>
                        Product Name
                      </TableCell>
                      <TableCell sx={{ fontWeight: "bold" }}>
                        Varient Option(s)
                      </TableCell>
                      <TableCell sx={{ fontWeight: "bold" }} align="center">
                        Item Qty
                      </TableCell>
                      <TableCell sx={{ fontWeight: "bold" }} align="right">
                        Item Discount
                      </TableCell>
                      <TableCell sx={{ fontWeight: "bold" }} align="right">
                        Item Price{" "}
                      </TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {row?.InventorySalesDetail?.map((historyRow) => (
                      <TableRow
                        key={historyRow?.OrderId}
                        sx={{
                          "&:last-child td, &:last-child th": { border: 0 },
                        }}
                      >
                        <TableCell component="th" scope="row">
                          {historyRow?.ProductName}
                        </TableCell>
                        <TableCell>{historyRow?.VarientOption}</TableCell>
                        <TableCell align="center">
                          {historyRow?.Quantity}
                        </TableCell>
                        <TableCell align="right">
                          {amountFormat(historyRow?.Discount)}
                        </TableCell>
                        <TableCell align="right">
                          {amountFormat(historyRow?.Price)}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </Box>
            </Collapse>
          </TableCell>
        </TableRow>
      </>
    );
  }
  const [selectionModel, setSelectionModel] = useState([]);

  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
    getOrdersRef.current = oNos;
  };
  ////////////
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(+event.target.value);
    setPage(0);
  };

  useEffect(() => {
    // console.log(selectedRows);
  }, [selectedRows]);
  return (
    <>
      {loading ? (
        <Box
          sx={{
            width: "100%",
            border: greyBorder,
            fontSize: "12px !important",
            background: grey,
            borderRadius: "0px 0px 8px 8px",
            height: 550,
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <Box
            mx={1}
            my={2}
            sx={{
              color: "var(--primary-color) !important",
              fontSize: 14,
              fontWeight: "bold",
            }}
          >
            <CicrlesLoading />
          </Box>
        </Box>
      ) : (
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            height: calculatedHeight,
          }}
        >
          <Paper
            sx={{
              width: "100%",
              border: greyBorder,
              fontSize: "12px !important",
              background: grey,
              borderRadius: "0px 0px 8px 8px",
            }}
            elevation={0}
          >
            <TableContainer
              sx={{
                maxHeight: calculatedHeight,
                "& .MuiTableCell-root": {
                  fontSize: "12px",
                  padding: "2px 16px",
                },
                background: grey,
              }}
            >
              <Table
                stickyHeader
                size="small"
                aria-label="collapsible a dense table"
              >
                <TableHead>
                  <TableRow>
                    <TableCell sx={{ fontWeight: "bold", background: "#fff" }}>
                      <Checkbox
                        color="primary"
                        checked={
                          alInventorySale?.list?.length === selectedRows.length
                        }
                        onChange={(e) => {
                          const val = e.target.checked;
                          if (val) {
                            const _selectedRowsOrderNo =
                              alInventorySale?.list?.map((data) => {
                                return data.OrderNo;
                              });
                            setSelectedRows(_selectedRowsOrderNo);
                          } else {
                            setSelectedRows([]);
                          }
                        }}
                      />
                    </TableCell>
                    <TableCell sx={{ fontWeight: "bold", background: "#fff" }}>
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_ORDER_NO
                      }
                    </TableCell>
                    <TableCell
                      sx={{ fontWeight: "bold", background: "#fff" }}
                      align="left"
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_STATION_NAME
                      }
                    </TableCell>
                    <TableCell
                      sx={{ fontWeight: "bold", background: "#fff" }}
                      align="left"
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_FULFILLMENT_STATUS
                      }
                    </TableCell>
                    <TableCell
                      sx={{ fontWeight: "bold", background: "#fff" }}
                      align="center"
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_TOTAL_ITEMS_QTY
                      }
                    </TableCell>
                    <TableCell
                      sx={{ fontWeight: "bold", background: "#fff" }}
                      align="right"
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_TOTAL_ITEMS_DISCOUNT
                      }
                    </TableCell>
                    <TableCell
                      sx={{ fontWeight: "bold", background: "#fff" }}
                      align="right"
                    >
                      {
                        LanguageReducer?.languageType
                          ?.PRODUCT_INVENTORY_SALES_TOTAL_ITEMS_PRICE
                      }
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {alInventorySale?.list?.map((row) => {
                    return <Row key={row.name} row={row} />;
                  })}
                </TableBody>
              </Table>
            </TableContainer>
            <TablePagination
              sx={{
                background: "#fff",
                border: "1px none",
                borderRadius: "0px 0px 8px 8px",
              }}
              rowsPerPageOptions={[10, 25, 100]}
              component="div"
              count={alInventorySale?.list?.length}
              rowsPerPage={rowsPerPage}
              page={page}
              onPageChange={handleChangePage}
              onRowsPerPageChange={handleChangeRowsPerPage}
            />
          </Paper>
          {/* <DataGrid
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
      </Menu> */}
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
      )}
    </>
  );
}
export default InventoryList;
