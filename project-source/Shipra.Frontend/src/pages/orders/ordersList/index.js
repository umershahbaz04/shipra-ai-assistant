import {
  Avatar,
  Box,
  Chip,
  CircularProgress,
  IconButton,
  Popover,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { forwardRef, useEffect, useImperativeHandle, useState } from "react";
import { useSelector } from "react-redux";
import OrderDetailModal from "../../../.reUseableComponents/Modal/InfoModal";
import {
  CreateClientOrderLabel,
  CreateClientOrderLabelLookup,
  DeleteClientOrderLabel,
  GetOrderItemByOrderNo,
  GetShipmentInfoByOrderNo,
  RefreshCarrierStatus,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import OrderItemDetailModal from "../../../components/modals/orderModals/OrderItemDetailModal";
import AdvanceSearchModal from "../../../components/modals/orderModals/AdvanceSearchModal";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import ModeEditIcon from "@mui/icons-material/ModeEdit";
import DataGridProComponent from "../../../.reUseableComponents/DataGrid/DataGridProComponent";
import CreateAbleSelectComponent from "../../../.reUseableComponents/TextField/CreateAbleSelectComponent";
import { OrderCardComponent } from "../../../.reUseableComponents/cardComponent/orderCardComponent";
import StatusBadge from "../../../components/shared/statudBadge";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  EnumChangeFilterModelApiUrls,
  EnumDetailsModalType,
  EnumFullFillmentStatus,
  EnumOptions,
  EnumTableName,
  viewTypesEnum,
} from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  ClipboardIcon,
  CodeBox,
  DescriptionBoxWithChild,
  DialerBox,
  GridContainer,
  GridItem,
  MenuComponent,
  amountFormat,
  centerColumn,
  checkRefArrayValue,
  fetchMethod,
  greyBorder,
  orderLabelColor,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  useMenuForLoop,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import {
  useGetAllEmployeeColumnConfiguration,
  useSaveColumnConfig,
} from "../../../utilities/helpers/HelpersFilter";
import { PaginationComponent } from "../../../utilities/helpers/paginationSchema";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

function TruncatedDescription({ text }) {
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);

  if (!text || !text.toString().trim()) return "-";

  const strText = text.toString();
  const isLong = strText.length > 30;
  const shortText = isLong ? strText.substring(0, 30) + "..." : strText;

  return (
    <Box sx={{ display: "flex", alignItems: "center", width: "100%", justifyContent: "space-between" }}>
      <Box sx={{ fontSize: "12px", whiteSpace: "normal", wordBreak: "break-word", flex: 1 }}>
        {shortText}
      </Box>
      {isLong && (
        <>
          <IconButton
            size="small"
            sx={{ p: 0.2, ml: 0.5 }}
            onClick={(e) => {
              e.stopPropagation();
              setAnchorEl(e.currentTarget);
            }}
          >
            {open ? (
              <ExpandLessIcon sx={{ fontSize: 16, color: "var(--primary-color)" }} />
            ) : (
              <ExpandMoreIcon sx={{ fontSize: 16, color: "var(--primary-color)" }} />
            )}
          </IconButton>
          <Popover
            open={open}
            anchorEl={anchorEl}
            onClose={(e) => {
              if (e) e.stopPropagation();
              setAnchorEl(null);
            }}
            anchorOrigin={{
              vertical: "bottom",
              horizontal: "left",
            }}
            transformOrigin={{
              vertical: "top",
              horizontal: "left",
            }}
            PaperProps={{
              sx: {
                p: 1.5,
                maxWidth: 350,
                maxHeight: 250,
                fontSize: "12px",
                whiteSpace: "normal",
                wordBreak: "break-word",
                boxShadow: "0px 4px 20px rgba(0,0,0,0.15)",
                borderRadius: "8px",
              },
            }}
          >
            <Typography variant="body2" sx={{ fontSize: "12px", color: "#333", whiteSpace: "pre-wrap" }}>
              {strText}
            </Typography>
          </Popover>
        </>
      )}
    </Box>
  );
}

function OrderList(props, ref) {
  const {
    allOrders,
    setAllOrders,
    getOrdersRef,
    resetRowRef,
    loading,
    allCarrierTrackingStatus,
    getAllCarrierTrackingStatusForSelection,
    setShowActionToolbarButtons,
    getAllOrders,
    allOrdersCount,
    isFilterOpen,
    viewMode,
    setAllOrderLoading,
    orderLabel,
    setOrderLabel,
    openAdvanceSearchModal,
    setOpenAdvanceSearchModal,
  } = props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { anchorEl, openElem, handleOpen, handleClose } = useMenuForLoop();
  const [selectedOrderLabels, setSelectedOrderLabels] = useState();
  const [labelLoading, setLabelLoading] = useState(false);
  const [orderIteminfoModal, setOrderIteminfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [isMetaFieldExist, setIsMetaFieldExist] = useState();
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [orderNo, setOrderNo] = useState("");
  // const [isNoteAdded, setIsNoteAdded] = useState(false)
  const [isNoteAdded, setIsNoteAdded] = useState({
    loading: false,
    orderNo: "",
    isAdded: false,
  });

  const [refreshCarrierStatusLoading, setRefreshCarrierStatusLoading] =
    useState({});

  const [columnVisibilityModel, setColumnVisibilityModel] = useState({});

  const handleGetShipmentInfoByOrderNo = async (data) => {
    const orderNum = typeof data === "string" ? data : data?.OrderNo;
    setInfoModal((prev) => ({ ...prev, loading: { [orderNum]: true } }));
    setIsNoteAdded((prev) => ({ ...prev, loading: { [orderNum]: true } }));
    await GetShipmentInfoByOrderNo(orderNum)
      .then((res) => {
        const response = res.data;
        setInfoModal((prev) => ({ ...prev, open: true, data: response }));
        setIsMetaFieldExist(typeof data === "object" ? data?.IsMetaFieldExist : response?.result?.IsMetaFieldExist);
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
  const handleActionButton = (cTarget, orderData) => {
    // setAnchorEl(cTarget);
    setOrderNo(orderData.OrderNo);
  };
  const handleGetShipmentItemInfoByOrderNo = async (orderNum) => {
    setOrderIteminfoModal((prev) => ({
      ...prev,
      loading: { [orderNum]: true },
    }));

    await GetOrderItemByOrderNo(orderNum)
      .then((res) => {
        const response = res.data;
        setOrderIteminfoModal((prev) => ({
          ...prev,
          open: true,
          data: response,
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setOrderIteminfoModal((prev) => ({
          ...prev,
          loading: { [orderNum]: false },
        }));
      });
  };

  const getCarrierText = (orderNo) => {
    let str = "Unassigned";
    // Check if any object has carrier
    const order = allOrders.find((x) => x.OrderNo == orderNo);
    if (order?.CarrierId) {
      str = order?.CarrierName || "";
    }
    return str;
  };

  const handleRefresh = (rowArray) => {
    console.log("first");
    (rowArray || getOrdersRef?.current).forEach(async (ordNo) => {
      const selectedOrder = allOrders.find((ord) => ord.OrderNo === ordNo);
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
          setAllOrders((prev) => {
            const _orders = [...prev];
            const _selectedOrderIndex = _orders.findIndex(
              (ord) => ord.OrderId == selectedOrder.OrderId
            );
            _orders[_selectedOrderIndex].TrackingStatus =
              updatedStatus.trackingStatus;
            _orders[_selectedOrderIndex].CarrierLastUpdateDateTime =
              updatedStatus.carrierLastUpdateDateTime;
            return _orders;
          });
          const isStatusExist = allCarrierTrackingStatus.some(
            (dt) =>
              dt.carrierTrackingStatusId === updatedStatus.trackingStatusId
          );

          if (!isStatusExist) {
            getAllCarrierTrackingStatusForSelection();
          }
        }
      }
    });
  };

  const handleChipDelete = async (id, lable) => {
    const response = await DeleteClientOrderLabel(id, lable);
    if (response?.data?.isSuccess) {
      successNotification("Labels Delete Successfully");
      getAllOrders();
    } else {
      UtilityClass.showErrorNotificationWithDictionary(response?.data?.errors);
    }
  };

  const handleOpenMenu = (data) => (e) => {
    setSelectedOrderLabels(null);
    handleOpen(e, data.OrderId);
    const existOrderLabel = data?.OrderLabels?.split(",").map((label) =>
      label.trim().toLowerCase()
    );
    const defaultOrderLabel = orderLabel.filter((dt) =>
      existOrderLabel?.includes(dt.labelName?.toLowerCase())
    );
    setSelectedOrderLabels(defaultOrderLabel);
  };

  const handleCreateLabel = async (inputValue) => {
    const randomIndex = Math.floor(Math.random() * orderLabelColor.length);
    const newOption = {
      clientOrderLabelLookupId: Date.now(),
      labelName: inputValue,
      colorCode: orderLabelColor[randomIndex],
    };
    setOrderLabel((prev) => [...prev, newOption]);
    setSelectedOrderLabels((prev) => [...(prev || []), newOption]);
    try {
      const response = await CreateClientOrderLabelLookup(
        newOption?.labelName,
        newOption?.colorCode
      );
      if (response?.data?.isSuccess) {
        successNotification(response?.data?.result?.message);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleSave = async (data) => {
    if (!selectedOrderLabels) {
      errorNotification("Select an existing order or create a new one.");
      return;
    }
    const ordrLabels = selectedOrderLabels.map((item) => ({
      label: item.labelName,
      colorCode: item.colorCode,
    }));
    const body = {
      OrderNos: data?.OrderNo,
      labels: ordrLabels,
    };
    setLabelLoading(true);
    try {
      const response = await CreateClientOrderLabel(body);
      if (response?.data?.isSuccess) {
        successNotification(response?.data?.result?.message);
        getAllOrders();
        setSelectedOrderLabels(null);
        handleClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLabelLoading(false);
    }
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
        <Stack direction={"column"}>
          <Box sx={{ fontWeight: "bold" }}>
            {LanguageReducer?.languageType?.ORDERS_ORDER_NO_REF_NO}
          </Box>
        </Stack>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Stack sx={{ textAlign: "" }} direction={"column"}>
              <CodeBox
                title={
                  infoModal.loading[params.row.OrderNo]
                    ? "loading..."
                    : params.row.OrderNo
                }
                onClick={(e) => {
                  handleGetShipmentInfoByOrderNo(params.row);
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
                <CodeBox title={params?.row?.RefNo} color={Colors.purple} />
              )}
            </Stack>
          </Stack>
        );
      },
    },
    {
      field: "Carrier",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SHIPMENTS_CARRIER_NAME}
        </Box>
      ),
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        const trackingNo = params.row?.CarrierTrackingNo;
        const truncatedTrackingNo =
          trackingNo?.length > 14
            ? `${trackingNo.slice(0, 14)}...`
            : trackingNo;
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
                {getCarrierText(params?.row?.OrderNo)}
              </Box>
              <CodeBox title={truncatedTrackingNo} copyBtn color="#000000de" />
            </Box>
          </Box>
        );
      },
    },
    {
      field: "Store",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_STORE_INFO}
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
                <DialerBox phone={params.row.CustomerServiceNo} />
              </Box>
              <Box sx={{ fontSize: "10px" }}>{params.row?.SaleChannelName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "DropOfAddress",
      ...centerColumn,

      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_CUSTOMER_INFO}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box display={"flex"} flexDirection={"column"} disableRipple>
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
      field: "CustomerAdress",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Customer Adress"}</Box>,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return <Box>{params.row.CustomerFullAddress}</Box>;
      },
    },
    {
      field: "Description",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Description"}</Box>,
      minWidth: 160,
      flex: 1,
      renderCell: (params) => {
        return <TruncatedDescription text={params.row.Description} />;
      },
    },
    {
      field: "Payment",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_STATUS}
        </Box>
      ),
      minWidth: 120,
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
      field: "FulfillmentStatus",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_FULFILLMENT_STATUS}
        </Box>
      ),
      minWidth: 120,
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
              {params.row.OrderTypeName != "Regular" && (
                <StatusBadge
                  title={params.row.FullFillmentStatus}
                  color={"#fff;"}
                  bgColor={
                    params.row?.FullFillmentStatusId ==
                    EnumFullFillmentStatus.UnFulfillment
                      ? "#dc3545;"
                      : "#28a745;"
                  }
                />
              )}
            </>
            <Box>
              {UtilityClass.convertUtcToLocalAndGetDate(
                params.row?.FulFilledDate
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
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_ORDER_TYPE}
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack direction={"column"}>
            <StatusBadge
              title={params.row.OrderTypeName}
              color="#1E1E1E;"
              bgColor="#EAEAEA"
            />
            <Box>
              {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
            </Box>
          </Stack>
        );
      },
    },
    {
      field: "OrderLabel",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Order label"}</Box>,
      minWidth: 220,
      flex: 1,
      renderCell: ({ row }) => {
        const labelNames = row?.OrderLabels
          ? row.OrderLabels.split(",").map((label) => label.trim().toLowerCase())
          : [];

        const matchedLabels = orderLabel.filter((label) =>
          labelNames.includes(label.labelName?.toLowerCase())
        );

        return (
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              flexWrap: "wrap",
              gap: "3px",
              p: 0.5,
            }}
          >
            {matchedLabels.map((label, index) => (
              <Chip
                key={index}
                label={label.labelName}
                size="small"
                onDelete={() => handleChipDelete(row?.OrderId, label.labelName)}
                variant="outlined"
                sx={{
                  borderColor: label.colorCode,
                  background: label.colorCode,
                  color: "#fff",
                  fontSize: "11px !important",
                  height: "22px !important",
                  display: "flex",
                  alignItems: "center",
                  "& .MuiChip-deleteIcon": {
                    color: "#fff",
                  },
                }}
              />
            ))}
            <IconButton
              id={`edit-button-${row.OrderId}`}
              onClick={handleOpenMenu(row)}
            >
              <ModeEditIcon color="primary" fontSize="small" />
            </IconButton>
            <MenuComponent
              open={openElem === row.OrderId}
              onClose={handleClose}
              anchorEl={anchorEl}
            >
              <Box sx={{ p: 1, width: "400px" }}>
                <CreateAbleSelectComponent
                  name="orderLabel"
                  options={orderLabel}
                  value={selectedOrderLabels}
                  height={40}
                  isMulti
                  onCreateOption={handleCreateLabel}
                  optionLabel={EnumOptions.ORDER_LABELS.LABEL}
                  optionValue={EnumOptions.ORDER_LABELS.VALUE}
                  onChange={(e, val) => {
                    setSelectedOrderLabels(val);
                  }}
                />
              </Box>
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "flex-end",
                  p: 1,
                  pt: 0,
                }}
              >
                <ActionButtonCustom
                  label={"Save"}
                  onClick={() => handleSave(row)}
                  loading={labelLoading}
                />
              </Box>
            </MenuComponent>
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
          {LanguageReducer?.languageType?.ORDERS_TRACKING_STATUS}
        </Box>
      ),
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box className={"flex_center"} py={1}>
            <Box display="flex" flexDirection="column">
              <StatusBadge
                title={row.TrackingStatus}
                color="#1E1E1E;"
                bgColor="#EAEAEA"
                showBtn={!row?.IsClientCarrier && row?.CarrierId != null}
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
      field: "ItemsCount",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_ITEM_COUNT}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <Box className={"flex_center"} flexDirection={"column"}>
              {orderIteminfoModal?.loading[params.row.OrderNo] ? (
                <CircularProgress size={20} />
              ) : (
                <CodeBox
                  title={params.row.ItemsCount}
                  onClick={(e) => {
                    handleGetShipmentItemInfoByOrderNo(params.row.OrderNo);
                  }}
                  eyeBtn={true}
                />
              )}
            </Box>
          </>
        );
      },
    },
    {
      field: "Amount",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_AMOUNT}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      ...rightColumn,
      renderCell: (params) => {
        return <Box>{amountFormat(params.row.Amount)}</Box>;
      },
    },
    // {
    //   field: "Action",
    //   hide: true,
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

  const [selectionModel, setSelectionModel] = useState([]);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const handleSelectedRow = (oNos) => {
    getOrdersRef.current = oNos;
    setSelectionModel(oNos);
    props.setSelectionModel(oNos);
    setShowActionToolbarButtons(checkRefArrayValue(oNos));
  };

  const getAllEmployeeColumnConfiguration =
    useGetAllEmployeeColumnConfiguration(
      setColumnVisibilityModel,
      EnumTableName.ORDERTABLE
    );

  const handleColumnVisibilityModelChange = useSaveColumnConfig(
    setColumnVisibilityModel,
    EnumTableName.ORDERTABLE
  );
  ////////////
  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
    }
  }, [resetRowRef.current]);

  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 59 - 62.5 - 210
    : windowHeight - 95 - 59 - 63;
  const calculatedGridHeight = isFilterOpen
    ? windowHeight - 395.5
    : windowHeight - 182;
  return (
    <Box>
      {viewMode === viewTypesEnum.TABLE ? (
        <DataGridProComponent
          rows={allOrders}
          columns={columns}
          loading={loading}
          getRowId={(row) => row.OrderNo}
          selectionModel={selectionModel}
          onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
          checkboxSelection
          rowsCount={allOrdersCount}
          paginationChangeMethod={getAllOrders}
          paginationMethodUrl={EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.url}
          defaultRowsPerPage={
            EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length
          }
          height={calculatedHeight}
          columnVisibilityModel={columnVisibilityModel}
          onColumnVisibilityModelChange={handleColumnVisibilityModelChange}
        />
      ) : (
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "space-between",
            overflowX: "auto",
            bgcolor: "#F8F8F8",
            height: calculatedGridHeight,
            border: greyBorder,
            borderBottomLeftRadius: "8px",
            borderBottomRightRadius: "8px",
            padding: "8px",
          }}
        >
          {loading ? (
            <Box
              sx={{
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                height: "100%",
              }}
            >
              <CircularProgress />
            </Box>
          ) : (
            <GridContainer spacing={2}>
              {allOrders.map((dt, index) => (
                <GridItem xs={12} sm={6} md={4} lg={3} key={index}>
                  <OrderCardComponent
                    onEyeClick={() =>
                      handleGetShipmentItemInfoByOrderNo(dt.OrderNo)
                    }
                    loading={!!orderIteminfoModal.loading[dt.OrderNo]}
                    orderData={
                      <>
                        <Box display="flex" justifyContent="space-between">
                          <Typography variant="body2">Order Number</Typography>
                          <Typography variant="body2">
                            {dt.OrderNo} <ClipboardIcon text={dt.OrderNo} />
                          </Typography>
                        </Box>
                        <Box>
                          <Box display="flex" justifyContent="space-between">
                            <Typography variant="body2">
                              Customer Name
                            </Typography>
                            <Typography variant="body2">
                              {dt.CustomerName}
                            </Typography>
                          </Box>
                        </Box>
                        <Box>
                          <Box display="flex" justifyContent="space-between">
                            <Typography variant="body2">Store Name</Typography>
                            <Typography variant="body2">
                              {dt.StoreName}
                            </Typography>
                          </Box>
                        </Box>
                        <Box>
                          <Box display="flex" justifyContent="space-between">
                            <Typography variant="body2">Order Type</Typography>
                            <Typography variant="body2">
                              {dt.OrderTypeName}
                            </Typography>
                          </Box>
                          <Box>
                            <Box>
                              <Box
                                display="flex"
                                justifyContent="space-between"
                              >
                                <Typography variant="body2">
                                  Assign Carrier
                                </Typography>
                                <Typography
                                  variant="body2"
                                  sx={{
                                    color: !dt?.CarrierId ? Colors.danger : "",
                                  }}
                                >
                                  {getCarrierText(dt?.OrderNo)}
                                </Typography>
                              </Box>
                            </Box>
                          </Box>
                        </Box>
                        <Box>
                          <Box display="flex" justifyContent="space-between">
                            <Typography variant="body2">Amount</Typography>
                            <Typography variant="body2">{dt.Amount}</Typography>
                          </Box>
                        </Box>
                        <Box display="flex" justifyContent="space-between">
                          <Typography variant="body2">
                            Tracking Status
                          </Typography>
                          <Box
                            display="flex"
                            flexDirection={"column"}
                            alignItems="center"
                          >
                            <StatusBadge
                              title={dt.TrackingStatus}
                              color="#1E1E1E"
                              bgColor="#EAEAEA"
                              showBtn={
                                !dt?.IsClientCarrier && dt?.CarrierId != null
                              }
                              btnLoading={
                                refreshCarrierStatusLoading[dt.OrderId]
                              }
                              btnOnClick={() => handleRefresh([dt.OrderNo])}
                            />
                            {dt?.CarrierLastUpdateDateTime && (
                              <Box
                                sx={{ textAlign: "center", display: "inline" }}
                              >
                                <Typography fontSize="10px">
                                  {UtilityClass.convertUtcToLocalAndGetTime(
                                    dt?.CarrierLastUpdateDateTime
                                  )}
                                </Typography>
                              </Box>
                            )}
                          </Box>
                        </Box>
                      </>
                    }
                    cardActionsChildren={
                      <ActionButtonCustom
                        sx={{
                          ...styleSheet.integrationactivatedButton,
                          width: "100%",
                          height: "28px",
                          borderRadius: "4px",
                        }}
                        variant="contained"
                        loading={infoModal.loading?.[dt?.OrderNo] ?? false}
                        onClick={() =>
                          handleGetShipmentInfoByOrderNo(dt.OrderNo)
                        }
                        label={"View Details"}
                      />
                    }
                    status={{
                      label: dt.PaymentStatus,
                      color:
                        dt.PaymentStatus === "Unpaid"
                          ? Colors.danger
                          : Colors.succes,
                    }}
                  />
                </GridItem>
              ))}
            </GridContainer>
          )}
          <PaginationComponent
            name={"orders"}
            dataCount={allOrdersCount}
            paginationChangeMethod={getAllOrders}
            paginationMethodUrl={
              EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.url
            }
            defaultRowsPerPage={
              EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length
            }
            setLoading={setAllOrderLoading}
            color="primary"
          />
        </Box>
      )}
      {infoModal.data?.result && (
        <OrderDetailModal
          data={infoModal?.data?.result}
          open={infoModal.open}
          onClose={() => setInfoModal((prev) => ({ ...prev, open: false }))}
          isSendNotification={false}
          isUpdateOrderBtn={true}
          printCarrier={infoModal?.data?.result?.order?.CarrierTrackingNo}
          modelType={EnumDetailsModalType.ORDER}
          setIsNoteAdded={setIsNoteAdded}
          isNoteAdded={isNoteAdded}
          isMetaFieldExist={isMetaFieldExist}
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
      {openAdvanceSearchModal && (
        <AdvanceSearchModal
          open={openAdvanceSearchModal}
          onClose={() => setOpenAdvanceSearchModal(false)}
          onSelectOrder={async (orderNo) => {
            if (props.setInputFields) {
              props.setInputFields([orderNo]);
            }
            await handleGetShipmentInfoByOrderNo(orderNo);
          }}
        />
      )}
    </Box>
  );
}
export default forwardRef(OrderList);
