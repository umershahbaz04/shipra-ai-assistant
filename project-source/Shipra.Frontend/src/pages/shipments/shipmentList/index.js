import ContentCopyOutlined from '@mui/icons-material/ContentCopyOutlined';
import ExpandMore from '@mui/icons-material/ExpandMore';
import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Avatar,
  Box,
  CircularProgress,
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
  Typography,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useState,
} from "react";
import { useSelector } from "react-redux";
import InfoModal from "../../../.reUseableComponents/Modal/InfoModal.js";
import {
  GetShipmentInfoByOrderNo,
  GetWayBillsByOrderNos,
  MoveDashboardStatusFromOneTabToAnother,
  RefreshCarrierStatus,
} from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";
import StatusBadge from "../../../components/shared/statudBadge";
import UtilityClass from "../../../utilities/UtilityClass.js";
import Colors from "../../../utilities/helpers/Colors.js";
import {
  ActionButtonCustom,
  ClipboardIcon,
  CodeBox,
  DescriptionBoxWithChild,
  DialerBox,
  RefreshBtn,
  centerColumn,
  downloadWayBillsByOrderNos,
  fetchMethod,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import { EnumChangeFilterModelApiUrls } from "../../../utilities/enum/index.js";
import DataGridProComponent from "../../../.reUseableComponents/DataGrid/DataGridProComponent.js";
function ShipmentList(props, ref) {
  const {
    allShipments,
    getOrdersRef,
    resetRowRef,
    isAllDataListLoading,
    shipmentTabsCountConfig,
    setStatusMoved,
    setAllShipments,
    allCarrierTrackingStatus,
    setAllCarrierTrackingStatusDependcy,
    getShipments,
    isFilterOpen,
    selectionModel,
    setSelectionModel,
  } = props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [anchorEl, setAnchorEl] = useState(null);
  const [stripeHostedUrl, setStripeHostedUrl] = useState("");
  const [orderNo, setOrderNo] = useState("");

  const [isLoading, setIsLoading] = useState(false);

  const handleActionButton = (target, orderData) => {
    setAnchorEl(target);
    setStripeHostedUrl(orderData.StripeInvoiceHostURL);
    setOrderNo(orderData.OrderNo);
  };
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [moveToModel, setMoveToModel] = useState({
    loading: {},
  });
  const [isNoteAdded, setIsNoteAdded] = useState({
    loading: false,
    orderNo: "",
    isAdded: false,
  });
  const [refreshCarrierStatusLoading, setRefreshCarrierStatusLoading] =
    useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  // handleGetShipmentInfoByOrderNo
  const handleGetShipmentInfoByOrderNo = async (orderNum) => {
    setInfoModal((prev) => ({ ...prev, loading: { [orderNum]: true } }));
    setIsNoteAdded((prev) => ({ ...prev, loading: { [orderNum]: true } }));
    await GetShipmentInfoByOrderNo(orderNum)
      .then((res) => {
        const response = res.data;
        setInfoModal((prev) => ({ ...prev, open: true, data: response }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setInfoModal((prev) => ({ ...prev, loading: { [orderNum]: false } }));
        setIsNoteAdded((prev) => ({ ...prev, loading: { [orderNum]: false } }));
      });
  };

  useEffect(() => {
    if (isNoteAdded.orderNo && isNoteAdded.isAdded) {
      handleGetShipmentInfoByOrderNo(isNoteAdded.orderNo);
      setIsNoteAdded((prev) => ({ ...prev, isAdded: false }));
    }
  }, [isNoteAdded.isAdded]);
  const [anchorElStatusMenu, setAnchorElStatusMenu] = React.useState(null);
  const openStatusMenu = Boolean(anchorElStatusMenu);
  const [selectedRow, setSelectedRow] = React.useState(null);

  const handleClickStatusUpdateMenu = (event, data) => {
    setAnchorElStatusMenu(event.currentTarget);
    setSelectedRow(data);
    //backend request if needed
  };
  const handleCloseStatusMenu = () => {
    setAnchorElStatusMenu(null);
  };
  const handleMoveToStatus = (moveToGridId) => {
    let params = {
      shipmentGridColumnId: moveToGridId,
      carrierTrackingStatusId: selectedRow?.CarrierTrackingStatusId,
    };
    setMoveToModel((prev) => ({ ...prev, loading: { [moveToGridId]: true } }));

    MoveDashboardStatusFromOneTabToAnother(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
          // setIsLoading(false);
        } else {
          successNotification("Status moved successfully");
          handleCloseStatusMenu();
          setSelectedRow(null);
          setStatusMoved(true);
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      })
      .finally((e) => {
        setMoveToModel((prev) => ({
          ...prev,
          loading: { [moveToGridId]: false },
        }));
      });
  };
  // never remove this code
  // const statusExistInAnyTabAndGetText = (carrierStatusId) => {
  //   let str = "";
  //   // Check if any object has dashboardStatusValue containing
  //   const containsValue = shipmentTabsCountConfig?.some((item) =>
  //     item?.dashboardStatusValue
  //       ?.split(",")
  //       .includes(carrierStatusId?.toString())
  //   );
  //   if (!containsValue) {
  //     str = "Unassigned in any tab";
  //   }
  //   return str;
  // };

  const handleRefresh = (rowArray) => {
    (rowArray || selectionModel).forEach(async (ordNo) => {
      const selectedOrder = allShipments?.list.find(
        (ord) => ord.OrderNo === ordNo
      );
      const hasRefreshBtn =
        !selectedOrder?.IsClientCarrier && selectedOrder?.CarrierId != null;
      if (hasRefreshBtn) {
        const { response } = await fetchMethod(
          () => RefreshCarrierStatus(selectedOrder.OrderId),
          setRefreshCarrierStatusLoading,
          true,
          selectedOrder.OrderId
        );
        if (response && response.result && response.result.data) {
          const updatedStatus = response.result.data;
          setAllShipments((prev) => {
            const _shipments = [...prev.list];
            const _selectedOrderIndex = _shipments.findIndex(
              (ord) => ord.OrderId == selectedOrder.OrderId
            );
            _shipments[_selectedOrderIndex].TrackingStatus =
              updatedStatus.trackingStatus;
            return { ...prev, list: _shipments };
          });
          const isStatusExist = allCarrierTrackingStatus.some(
            (dt) =>
              dt.carrierTrackingStatusId === updatedStatus.trackingStatusId
          );

          if (!isStatusExist) {
            setAllCarrierTrackingStatusDependcy((prev) => !prev);
          }
        }
      }
    });
  };
  // for call method in parent
  useImperativeHandle(ref, () => ({
    Refresh() {
      handleRefresh();
    },
  }));
  const columns = [
    {
      field: "OrderNo",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SHIPMENTS_ORDER_NO_REF_NO}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ cursor: "pointer" }}
            disableRipple
          >
            <>
              <Stack direction={"column"}>
                <CodeBox
                  title={
                    infoModal.loading[params.row.OrderNo]
                      ? "loading..."
                      : params.row.OrderNo
                  }
                  onClick={(e) => {
                    handleGetShipmentInfoByOrderNo(params.row.OrderNo);
                    setIsNoteAdded((prev) => ({
                      ...prev,
                      orderNo: params.row.OrderNo,
                    }));
                  }}
                  copyBtn
                  sx={{
                    textDecoration: infoModal.loading[params.row.OrderNo]
                      ? "none"
                      : "underline",
                  }}
                />
                {params?.row?.RefNo && (
                  <Box paddingTop="5px">
                    <CodeBox title={params?.row?.RefNo} color={Colors.purple} />
                  </Box>
                )}
              </Stack>
              {!params?.row?.RefNo && (
                <Box
                  sx={{
                    opacity: "0",
                  }}
                >
                  {params?.row?.RefNo}
                </Box>
              )}
            </>
          </Box>
        );
      },
    },
    {
      field: "CarrierName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SHIPMENTS_CARRIER_NAME}
        </Box>
      ),
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            {params.row?.CarrierImage && (
              <Avatar
                variant="rounded"
                src={params.row?.CarrierImage}
                alt={params.row?.CarrierName}
                sx={{
                  width: 35,
                  height: 35,
                  bgcolor: "#fff",
                  borderRadius: "8px",
                  p: 0.5,
                  "& img": {
                    objectFit: "contain !important",
                  },
                }}
              />
            )}
            <Box>
              <Box
                sx={{
                  color: `${!params.row?.CarrierId ? Colors.danger : ""}`,
                  fontSize: "10px",
                }}
              >
                {params?.row?.CarrierName}
              </Box>
              {params.row?.CarrierTrackingNo && (
                <Typography variant="caption" color="textSecondary">
                  {params.row?.CarrierTrackingNo}
                  <ClipboardIcon text={params.row.CarrierTrackingNo} />
                </Typography>
              )}
            </Box>
          </Box>
        );
      },
    },
    {
      field: "Store",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SHIPMENTS_STORE_INFO}
        </Box>
      ),
      minWidth: 130,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Box>{params.row.StoreName}</Box>
              <Box>
                <DialerBox phone={params.row.CustomerServiceNo} />
              </Box>
              <Box sx={{ fontSize: "10px" }}>{params.row?.SaleChannelName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "DropOfAddress",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SHIPMENTS_DROP_OFF_ADDRESS}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            // sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row.CustomerName}
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
                              // width: "150px",
                            }}
                          >
                            Address
                          </TableCell>
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
                            {params.row.CustomerName}
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="right"
                          >
                            <DialerBox phone={params.row.Mobile1} />
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              // width: "150px",
                            }}
                          >
                            {params.row.CustomerFullAddress}
                          </TableCell>
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
      ...centerColumn,
      field: "TrackingStatus",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SHIPMENTS_TRACKING_STATUS}
        </Box>
      ),
      minWidth: 250,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box display="flex" alignItems={"center"} py={1}>
            <Box display="flex" flexDirection="column">
              <StatusBadge
                title={row.TrackingStatus}
                color="#1E1E1E;"
                bgColor="#EAEAEA"
                showBtn={row?.CarrierTrackingNo}
                btnLoading={refreshCarrierStatusLoading[row.OrderId]}
                btnOnClick={() => handleRefresh([row.OrderNo])}
              />
              {row?.CarrierLastUpdateDateTime && (
                <Box sx={{ textAlign: "center", display: "inline" }}>
                  <Typography fontSize="10px">
                    {UtilityClass.convertUtcToLocalAndGetTime(
                      row?.CarrierLastUpdateDateTime
                    )}
                  </Typography>
                </Box>
              )}
            </Box>
          </Box>
        );
      },
    },
    {
      field: "OrderTypeName",
      headerAlign: "center",
      align: "center",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SHIPMENTS_ORDER_TYPE}
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack direction={"column"}>
            <Box>
              <StatusBadge
                title={params.row.OrderTypeName}
                color="#1E1E1E;"
                bgColor="#EAEAEA"
              />
            </Box>
            <Box>
              {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
            </Box>
          </Stack>
        );
      },
    },
    {
      ...centerColumn,
      field: "Payment Status",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SHIPMENTS_PAYMENT_STATUS}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            className={"flex_center"}
            flexDirection={"column"}
            sx={{ fontWeight: "bold", textAlign: "right" }}
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
      field: "Remarks",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SHIPMENTS_REMARKS}
        </Box>
      ),
    },
  ];
  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
    getOrdersRef.current = oNos;
  };

  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  const calculatedHeight = isFilterOpen
    ? windowHeight - 140 - 86 - 200
    : windowHeight - 140 - 91;

  return (
    <>
      <Box width={"100%"}>
        <DataGridProComponent
          paddingBottom={0}
          getRowId={(row) => row.OrderNo}
          rows={allShipments.list ? allShipments.list : []}
          rowsCount={allShipments?.TotalCount}
          columns={columns}
          checkboxSelection
          selectionModel={selectionModel}
          onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
          loading={isAllDataListLoading}
          paginationChangeMethod={getShipments}
          height={calculatedHeight}
          paginationMethodUrl={
            EnumChangeFilterModelApiUrls.GET_ALL_SHIPMENTS.url
          }
          defaultRowsPerPage={
            EnumChangeFilterModelApiUrls.GET_ALL_SHIPMENTS.length
          }
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
              <ListItem
                onClick={() => {
                  setAnchorEl(null);
                }}
                disablePadding
              >
                {stripeHostedUrl !== "" && (
                  <ListItemButton>
                    <ListItemText
                      primary={
                        <Box>
                          Payment Link
                          <ContentCopyOutlined
                            fontSize="inherit"
                            sx={{ cursor: "pointer", margin: "0px 5px" }}
                            onClick={() =>
                              UtilityClass.copyToClipboard(stripeHostedUrl)
                            }
                          />
                        </Box>
                      }
                    ></ListItemText>
                  </ListItemButton>
                )}
              </ListItem>
              <ListItem disablePadding>
                <ListItemButton>
                  <ListItemText
                    onClick={() => downloadWayBillsByOrderNos(orderNo, 1)}
                    primary={
                      <Box>
                        {isLoading ? (
                          <CircularProgress size={20} />
                        ) : (
                          <Box>Print Label</Box>
                        )}
                      </Box>
                    }
                  />
                </ListItemButton>
              </ListItem>
            </List>
          </Box>
        </Menu>
        <Menu
          id="long-menu"
          MenuListProps={{
            "aria-labelledby": "long-button",
          }}
          anchorOrigin={{
            vertical: "bottom",
            horizontal: "left",
          }}
          transformOrigin={{
            vertical: "top",
            horizontal: "left",
          }}
          anchorEl={anchorElStatusMenu}
          open={openStatusMenu}
          onClose={handleCloseStatusMenu}
          PaperProps={{
            sx: {
              px: 1.25,
              maxHeight: 500,
            },
          }}
        >
          <TableContainer sx={{ minWidth: 275 }} aria-label="simple table">
            <Table>
              <TableBody>
                {shipmentTabsCountConfig
                  ?.filter(
                    (item, index) =>
                      index !== 0 || item.dashboardStatusName !== "ALL"
                  )
                  .map((item) => {
                    return (
                      <>
                        {" "}
                        {item?.dashboardStatusName != "All" && (
                          <TableRow
                            key={"1"}
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
                              {item?.dashboardStatusName}
                            </TableCell>
                            <TableCell
                              sx={{
                                padding: "7px",
                                fontSize: "11px",
                                // width: "150px",
                              }}
                            >
                              <ActionButtonCustom
                                onClick={(e) =>
                                  handleMoveToStatus(item?.shipmentGridColumnId)
                                }
                                label={"Move to"}
                                loading={
                                  moveToModel?.loading[
                                    item?.shipmentGridColumnId
                                  ]
                                }
                                variant="outlined"
                              >
                                Move to
                              </ActionButtonCustom>
                            </TableCell>
                          </TableRow>
                        )}
                      </>
                    );
                  })}
              </TableBody>
            </Table>
          </TableContainer>
        </Menu>
      </Box>
      {infoModal.data?.result && (
        <InfoModal
          data={infoModal?.data?.result}
          open={infoModal.open}
          onClose={() => setInfoModal((prev) => ({ ...prev, open: false }))}
          isSendNotification={true}
          isUpdateOrderBtn={false}
          printCarrier={true}
          setIsNoteAdded={setIsNoteAdded}
          isNoteAdded={isNoteAdded}
        />
      )}
    </>
  );
}
export default forwardRef(ShipmentList);
