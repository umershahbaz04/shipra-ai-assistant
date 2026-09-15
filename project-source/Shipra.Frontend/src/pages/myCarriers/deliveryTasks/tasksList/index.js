import EditIcon from "@mui/icons-material/Edit";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import WhatsAppIcon from "@mui/icons-material/WhatsApp";
import {
  Alert,
  Box,
  CircularProgress,
  IconButton,
  Menu,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import { DataGridPro, useGridApiRef } from "@mui/x-data-grid-pro";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { styleSheet } from "../../../../assets/styles/style";
import UpdateOutScanModal from "../../../../components/modals/myCarrierModals/UpdateOutscanModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import UtilityClass from "../../../../utilities/UtilityClass";
import {
  EnumDeliveryTaskStatus,
  EnumNavigateState,
  EnumRoutesUrls,
  EnumTableName,
  EnumDetailsModalType,
  EnumOptions,
} from "../../../../utilities/enum";
import Colors, {
  Danger,
  Success,
  Warning,
} from "../../../../utilities/helpers/Colors";
import {
  ClipboardIcon,
  CodeBox,
  DeleteButton,
  DescriptionBox,
  DescriptionBoxWithChild,
  DialerBox,
  MapButton,
  SmsButton,
  amountFormat,
  centerColumn,
  handleCopyToClipBoard,
  navbarHeight,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  useNavigateSetState,
  usePagination,
  downloadWayBillsByOrderNos,
  getElementByInnerHTML,
  useMenuForLoop,
  orderLabelColor,
  MenuComponent,
  ActionButtonCustom,
} from "../../../../utilities/helpers/Helpers";

import PrintIcon from "@mui/icons-material/Print";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";
import UpdateOrderAddressLatLangModal from "../../../../components/modals/myCarrierModals/UpdateOrderAddressLatLangModal";
import useOpenStreetmapGetLatLng from "../../../../.reUseableComponents/CustomHooks/useOpenStreetmapGetLatLng";
import OrderDetailModal from "../../../../.reUseableComponents/Modal/InfoModal";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import AdvanceSearchModal from "../../../../components/modals/orderModals/AdvanceSearchModal";
import { GetShipmentInfoByOrderNo, CreateClientOrderLabelLookup, CreateClientOrderLabel, DeleteClientOrderLabel, RevertDeliveryNoteDetailByOrderId, DeleteDeliveryNoteDetailByOrderId, BatchUpdateOrderStatus, UpdateCustomerEmail } from "../../../../api/AxiosInterceptors";
import { Chip, Popover, Typography, TextField } from "@mui/material";
import CreateAbleSelectComponent from "../../../../.reUseableComponents/TextField/CreateAbleSelectComponent";
import SelectComponent from "../../../../.reUseableComponents/TextField/SelectComponent";
import ModeEditIcon from "@mui/icons-material/ModeEdit";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import { errorNotification, successNotification } from "../../../../utilities/toast";

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

function TasksList(props) {
  const {
    allDeliveryTask,
    getOrdersRef,
    loading,
    resetRowRef,
    setSelectedDeliveryTasks,
    getAllDeliveryTask,
    isFilterOpen,
    setOpenTransferModal,
    orderLabel,
    setOrderLabel,
    documentSetting,
    openAdvanceSearchModal,
    setOpenAdvanceSearchModal,
    allCarrierTrackingStatus,
  } = props;
  const apiRef = useGridApiRef();
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [openUpdate, setOpenUpdate] = useState(false);
  const [openUpdatelanLong, setopenUpdatelanLong] = useState(false);
  const [selectedItem, setSelectedItem] = useState();
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [selectedRowToDelete, setSelectedRowToDelete] = useState(null);
  const navigate = useNavigate();
  const [addressCoordinates, setAddressCoordinates] = useState(null);
  const { setNavigateState } = useNavigateSetState();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [columnVisibilityModel, setColumnVisibilityModel] = useState(() => {
    try {
      const saved = localStorage.getItem("deliveryTasksColumnVisibility");
      return saved ? JSON.parse(saved) : {};
    } catch (e) {
      return {};
    }
  });

  const handleColumnVisibilityModelChange = (newModel) => {
    setColumnVisibilityModel(newModel);
    try {
      localStorage.setItem("deliveryTasksColumnVisibility", JSON.stringify(newModel));
    } catch (e) { }
  };
  const [columnOrder, setColumnOrder] = useState(() => {
    try {
      const saved = localStorage.getItem("deliveryTasksColumnOrder");
      return saved ? JSON.parse(saved) : [];
    } catch (e) {
      return [];
    }
  });

  const handleColumnWidthChange = (params) => {
    if (params?.colDef?.field && params?.width) {
      try {
        const savedWidthsStr = localStorage.getItem("deliveryTasksColumnWidths");
        const savedWidths = savedWidthsStr ? JSON.parse(savedWidthsStr) : {};
        savedWidths[params.colDef.field] = params.width;
        localStorage.setItem("deliveryTasksColumnWidths", JSON.stringify(savedWidths));
      } catch (e) { }
    }
  };
  const [flag, setFlag] = useState(false);
  const [showContent, setShowContent] = useState(false);
  const [slectedAddress, setSelectedAddress] = useState({
    open: false,
    isLoading: {},
    coordinates: null,
    selectedItem: null,
  });

  const { anchorEl: labelAnchorEl, openElem: labelOpenElem, handleOpen: handleLabelOpen, handleClose: handleLabelClose } = useMenuForLoop();
  const [selectedOrderLabels, setSelectedOrderLabels] = useState();
  const [labelLoading, setLabelLoading] = useState(false);

  const { anchorEl: carrierAnchorEl, openElem: carrierOpenElem, handleOpen: handleCarrierOpen, handleClose: handleCarrierClose } = useMenuForLoop();
  const [carrierLoading, setCarrierLoading] = useState(false);
  const [selectedCarrier, setSelectedCarrier] = useState(null);
  const [carrierComment, setCarrierComment] = useState("");

  const handleOpenCarrierMenu = (data) => (e) => {
    setSelectedCarrier(null);
    setCarrierComment("");
    handleCarrierOpen(e, data.OrderId);
    const existingStatus = allCarrierTrackingStatus?.find(status => status.trackingStatus === data.TrackingStatus);
    if(existingStatus){
        setSelectedCarrier(existingStatus);
    }
  };

  const handleSaveCarrierStatus = async (row) => {
    if (!selectedCarrier?.carrierTrackingStatusId || selectedCarrier.carrierTrackingStatusId === 0) {
      errorNotification("Please choose carrier status");
      return;
    }
    const param = {
      carrierStatusId: selectedCarrier.carrierTrackingStatusId,
      orderNos: row.OrderNo,
      comments: carrierComment,
      fromDeliveryTaskScreen: true,
    };
    setCarrierLoading(true);
    try {
      const res = await BatchUpdateOrderStatus(param);
      if (!res.data.isSuccess) {
        let errJson = res.data.errors?.InvalidOrders?.[0];
        if(errJson) {
            let jsonData = JSON.parse(errJson);
            jsonData?.forEach((obj, i) => {
                let message = `${obj.Message} \n ${obj.OrderNo}`;
                errorNotification(message);
            });
        }
      } else {
        successNotification("Carrier status updated successfully");
        getAllDeliveryTask();
        handleCarrierClose();
      }
    } catch (e) {
        errorNotification(LanguageReducer?.languageType?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST || "Something went wrong");
    } finally {
        setCarrierLoading(false);
    }
  };

  const handleChipDelete = async (id, lable) => {
    const response = await DeleteClientOrderLabel(id, lable);
    if (response?.data?.isSuccess) {
      successNotification("Labels Delete Successfully");
      getAllDeliveryTask();
    }
  };
  const [revertLoading, setRevertLoading] = useState({});

  const handleRevertDeliveryNoteDetail = async (row) => {
    if (!row?.OrderId) return;
    setRevertLoading((prev) => ({ ...prev, [row.OrderId]: true }));
    try {
      const res = await DeleteDeliveryNoteDetailByOrderId({ OrderId: row.OrderId });
      if (res?.data?.isSuccess) {
        successNotification("Order deleted from Delivery Note successfully!");
        getAllDeliveryTask();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
      }
    } catch (err) {
      errorNotification("Failed to delete order from Delivery Note.");
    } finally {
      setRevertLoading((prev) => ({ ...prev, [row.OrderId]: false }));
    }
  };

  const handleOpenMenu = (data) => (e) => {
    setSelectedOrderLabels(null);
    handleLabelOpen(e, data.OrderId);
    const existOrderLabel = data?.OrderLabels?.split(",").map((label) =>
      label.trim().toLowerCase()
    );
    const defaultOrderLabel = orderLabel?.filter((dt) =>
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
        getAllDeliveryTask();
        setSelectedOrderLabels(null);
        handleLabelClose();
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

  const handleCloseUpdateMap = () => {
    setSelectedAddress({
      open: false,
      isLoading: {},
      coordinates: null,
      selectedItem: null,
    });
  };

  const [isMetaFieldExist, setIsMetaFieldExist] = useState();
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [isNoteAdded, setIsNoteAdded] = useState({
    loading: false,
    orderNo: "",
    isAdded: false,
  });

  const handleGetShipmentInfoByOrderNo = async (orderNum) => {
    setInfoModal((prev) => ({ ...prev, loading: { [orderNum]: true } }));
    setIsNoteAdded((prev) => ({ ...prev, loading: { [orderNum]: true } }));
    await GetShipmentInfoByOrderNo(orderNum)
      .then((res) => {
        const response = res.data;
        setInfoModal((prev) => ({ ...prev, open: true, data: response }));
        setIsMetaFieldExist(response?.result?.IsMetaFieldExist);
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
  //#region table colum
  const { getAddressCoordinates, isLoadingCoordinates, error } =
    useOpenStreetmapGetLatLng();
  const handleOpenModal = (row) => {
    const getCordinates = async () => {
      if (row) {
        try {
          const coordinates = await getAddressCoordinates(row.CityName);
          setSelectedAddress((prev) => ({
            open: true,
            isLoading: isLoadingCoordinates,
            coordinates: coordinates
              ? {
                lat: parseFloat(coordinates?.lat)
                  ? parseFloat(coordinates?.lat)?.toFixed(5)
                  : 0,
                lng: parseFloat(coordinates?.lng)
                  ? parseFloat(coordinates?.lng)?.toFixed(5)
                  : 0,
              }
              : {},
            selectedItem: row,
          }));
        } catch (error) {
          console.error("Error:", error.message);
        }
      }
    };
    getCordinates();
  };

  const handleSendSms = (row) => {
    // TODO: Add API call for send sms
    console.log("Send SMS to row:", row);
  };

  const handleCopyWhatsAppMessage = (row) => {
    const customer = row.Customer || "PERSON NAME";
    const orderNo = row.OrderNo || "ORDER NO";
    const description = row.Description || "DESCRIPTION";
    const amount = amountFormat(row.Amount) || "AMOUNT";
    const currency = row.CurrencyCode || "AED";
    const address = row.CustomerFullAddress || row.DropOfAddress || row.CustomerAddress || "";
    const mobile1 = row.Mobile1 || row.Phone || "";
    const mobile2 = row.Mobile2 || "";

    let workingNumberText = "";
    if (mobile1 && mobile2) {
      workingNumberText = `*${mobile1}* And *${mobile2}*`;
    } else if (mobile1) {
      workingNumberText = `*${mobile1}*`;
    } else if (mobile2) {
      workingNumberText = `*${mobile2}*`;
    }

    const message = `Dear *${customer}*
Your Order Number *${orderNo}*
Your Product *${description}* Is Out For Delivery ,
Total Amount Is *${amount} ${currency}*
Address: ${address}
Kindly Send Your Whats App Location For Earleist Delivery
And Your Working Numebr Is : ${workingNumberText}`;

    navigator.clipboard.writeText(message).then(() => {
      successNotification("WhatsApp message copied to clipboard!");
    }).catch(err => {
      errorNotification("Failed to copy message");
    });
  };

  const columns = [
    {
      field: "OrderNo",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ORDER_NO}
        </Box>
      ),
      minWidth: 175,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            // display={"flex"}
            // flexDirection={"column"}
            // justifyContent={"center"}
            // sx={{ textAlign: "center", cursor: "pointer" }}
            disableRipple
          >
            <>
              <Stack direction={"row"} sx={{ alignItems: "center" }}>
                {infoModal?.loading?.[params.row.OrderNo] ? (
                  <CircularProgress size={16} sx={{ mr: 1 }} />
                ) : (
                  <CodeBox
                    title={params.row.OrderNo}
                    onClick={(e) => {
                      e.stopPropagation();
                      handleGetShipmentInfoByOrderNo(params.row.OrderNo);
                      setIsNoteAdded((prev) => ({
                        ...prev,
                        orderNo: params.row.OrderNo,
                      }));
                    }}
                  />
                )}
                <ClipboardIcon text={params.row.OrderNo} />
                <IconButton
                  onClick={(e) => {
                    e.stopPropagation();
                    handleCopyWhatsAppMessage(params.row);
                  }}
                  sx={{
                    borderRadius: "5px",
                    minWidth: "25px",
                    height: "25px",
                    padding: "2px",
                    color: "#fff",
                    background: Colors.succes,
                    "&:hover": { background: Colors.succes, color: "#fff" },
                    boxShadow: "none",
                    marginLeft: "5px",
                  }}
                  title="Copy WhatsApp Message"
                >
                  <WhatsAppIcon sx={{ fontSize: "16px" }} />
                </IconButton>
              </Stack>
              <Box sx={{ mt: 0.5, display: "flex", alignItems: "center", gap: 0.5, flexWrap: "wrap" }}>
                <CodeBox title={params.row.TrackingNo} onClick={() => { }} />
                {params?.row?.RefNo && (
                  <CodeBox title={params?.row?.RefNo} color={Colors.purple} />
                )}
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "DeliveryNoteNo",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Delivery Note"}</Box>,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        const noteNo = params.row.DeliveryNoteNo || params.row.deliveryNoteNo;
        const noteStatus = params.row.DeliveryNoteStatus || params.row.deliveryNoteStatus || "In Progress";
        if (!noteNo) return null;

        const isCompleted =
          params.row.DeliveryTaskStatus === "Completed" ||
          params.row.DeliveryTaskStatusId === EnumDeliveryTaskStatus.Completed ||
          params.row.DeliveryTaskStatusId === 3 ||
          params.row.DeliveryNoteStatusId === 2 ||
          noteStatus === "Completed";

        return revertLoading?.[params?.row?.OrderId] ? (
          <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%" }}>
            <CircularProgress size={16} />
          </div>
        ) : (
          <Box sx={{ display: "flex", alignItems: "center", justifyContent: "center", width: "100%" }}>
            <Box sx={{ display: "flex", flexDirection: "column", alignItems: "flex-start", gap: 0.5, marginLeft: !isCompleted ? "30px" : "0px" }}>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                <CodeBox title={noteNo} />
                <ClipboardIcon text={noteNo} />
              </Box>
              <StatusBadge
                title={noteStatus}
                maxWidth="90px"
                color="#fff"
                bgColor={isCompleted ? Colors.succes : Colors.warning}
              />
            </Box>
            {!isCompleted && (
              <Box sx={{ marginLeft: "10px" }}>
                <DeleteButton
                  loading={revertLoading?.[params?.row?.OrderId]}
                  onClick={(e) => {
                    e?.stopPropagation?.();
                    setSelectedRowToDelete(params.row);
                    setDeleteModalOpen(true);
                  }}
                />
              </Box>
            )}
          </Box>
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
      valueGetter: (value, row) => {
        const rowData = row || value?.row || value;
        return rowData?.OrderLabels || "";
      },
      renderCell: ({ row }) => {
        const labelNames = row?.OrderLabels
          ? row.OrderLabels.split(",").map((label) => label.trim().toLowerCase())
          : [];

        const matchedLabels = orderLabel?.filter((label) =>
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
            {matchedLabels?.map((label, index) => (
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
              open={labelOpenElem === row.OrderId}
              onClose={handleLabelClose}
              anchorEl={labelAnchorEl}
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
      field: "SettingConfigData",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          Meta Data
        </Box>
      ),
      flex: 1,
      renderCell: ({ row }) => {
        let has = row.HasAdditionalField || row.hasAdditionalField;

        // If the row is not marked as having additional fields, show nothing.
        if (!has) {
          return <Box></Box>;
        }

        let content;
        if (!row.SettingConfigData || row.SettingConfigData.length === 0) {
          content = <Box sx={{ fontSize: "10px", color: "red", flex: 1 }}>Empty Config</Box>;
        } else {
          content = (
            <Box sx={{ whiteSpace: "normal", wordWrap: "break-word", flex: 1, fontSize: "12px", lineHeight: "1.5" }}>
              {row.SettingConfigData.map((f, i) => {
                let val = f.value;
                if (typeof val === "object" && val !== null) {
                  val = val.label || val.value || val.name || JSON.stringify(val);
                }
                return (
                  <span key={i} style={{ display: 'block', marginBottom: '2px' }}>
                    <strong>{f.name}:</strong> {val || ""}
                  </span>
                );
              })}
            </Box>
          );
        }

        return (
          <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ width: "100%", pr: 1 }}>
            {content}
            <IconButton
              size="small"
              onClick={(e) => {
                e.stopPropagation();
                if (props.setSelectedDeliveryTasks && props.setOpenEditMetaField) {
                  props.setSelectedDeliveryTasks([row.OrderNo]);
                  props.setOpenEditMetaField(true);
                }
              }}
              title="Edit Additional Fields"
            >
              <EditIcon fontSize="small" />
            </IconButton>
          </Stack>
        );
      },
    },
    {
      field: "Description",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          Description
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <TruncatedDescription text={params.row.Description} />;
      },
    },
    {
      ...centerColumn,
      field: "TrackingStatus",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_CARRIER_STATUS
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              flexWrap: "nowrap",
              gap: "3px",
              p: 0.5,
            }}
          >
            <StatusBadge
              title={params.row.TrackingStatus}
              color="#1E1E1E;"
              bgColor="#EAEAEA"
            />
            <IconButton
              id={`edit-carrier-button-${params.row.OrderId}`}
              onClick={handleOpenCarrierMenu(params.row)}
            >
              <ModeEditIcon color="primary" fontSize="small" />
            </IconButton>
            <MenuComponent
              open={carrierOpenElem === params.row.OrderId}
              onClose={handleCarrierClose}
              anchorEl={carrierAnchorEl}
            >
              <Box sx={{ p: 1, width: "300px" }}>
                <SelectComponent
                  name="carrier"
                  options={allCarrierTrackingStatus}
                  value={selectedCarrier}
                  height={40}
                  getOptionLabel={(option) => option.trackingStatus}
                  optionLabel={EnumOptions.CARRIER_TRACKING_STATUS.LABEL}
                  optionValue={EnumOptions.CARRIER_TRACKING_STATUS.VALUE}
                  onChange={(e, newValue) => {
                    const resolvedId = newValue ? newValue : null;
                    setSelectedCarrier(resolvedId);
                  }}
                  size={"md"}
                />
                <TextField
                  multiline
                  placeholder="Comment"
                  value={carrierComment}
                  onChange={(e) => setCarrierComment(e.target.value)}
                  size="small"
                  fullWidth
                  variant="outlined"
                  rows={2}
                  sx={{ mt: 1 }}
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
                  onClick={() => handleSaveCarrierStatus(params.row)}
                  loading={carrierLoading}
                />
              </Box>
            </MenuComponent>
          </Box>
        );
      },
    },

    {
      field: "Customer",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_CUSTOMER}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      editable: true,
    },
    {
      field: "Mobile1",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          Mobile 1
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      editable: true,
      renderCell: (params) => {
        return params.value ? <DialerBox phone={params.value} /> : "-";
      }
    },
    {
      field: "Mobile2",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          Mobile 2
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      editable: true,
      renderCell: (params) => {
        return params.value ? <DialerBox phone={params.value} /> : "-";
      }
    },

    {
      ...centerColumn,
      minWidth: 120,
      field: "Location",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_LOCATION}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <Box sx={{ textAlign: "center" }} disableRipple>
              <>
                <Box>{params.row.CustomerFullAddress}</Box>
              </>
            </Box>
          </Box>
        );
      },
    },

    {
      field: "DriverName",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_DRIVER}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", width: "100%" }}>
            <Box sx={{ textAlign: "left", flex: 1, overflow: "hidden", textOverflow: "ellipsis" }} title={params.row.DriverName}>
              {params.row.DriverName}
            </Box>
            <IconButton
              size="small"
              onClick={(e) => {
                e.stopPropagation();
                setSelectedDeliveryTasks([params.row.OrderNo]);
                if (setOpenTransferModal) setOpenTransferModal(true);
              }}
              title="Transfer / Create Delivery Task"
            >
              <EditIcon sx={{ fontSize: "16px", color: Colors.primary }} />
            </IconButton>
          </Box>
        );
      },
    },

    {
      ...centerColumn,
      field: "DeliveryTaskStatus",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_TASK_STATUS}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        let bgClr = Success;

        if (EnumDeliveryTaskStatus.Pending == params.row.DeliveryTaskStatusId) {
          bgClr = Danger;
        } else if (
          EnumDeliveryTaskStatus.Started == params.row.DeliveryTaskStatusId
        ) {
          bgClr = Warning;
        } else if (
          EnumDeliveryTaskStatus.Attempted == params.row.DeliveryTaskStatusId
        ) {
          bgClr = Colors.purple;
        }
        return (
          <StatusBadge
            title={params.row?.DeliveryTaskStatus}
            borderColor="rgba(0, 186, 119, 0.2)"
            color={
              EnumDeliveryTaskStatus.Completed ==
                params.row.DeliveryTaskStatusId
                ? "#fff;"
                : "#fff;"
            }
            bgColor={bgClr}
          />
        );
      },
    },
    {
      ...centerColumn,
      field: "OrderDate",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ORDER_DATE}
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
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_COD}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <>{amountFormat(params.row.Amount)}</>;
      },
    },
    {
      ...rightColumn,
      field: "SalePersonName",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" SalePersonName "}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <>{params.row.SalePersonName}</>;
      },
    },
    {
      field: "latLng",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_COORDINATES}
        </Box>
      ),
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            {row?.IsValidLatLng == 0 ? (
              <Box
                sx={{
                  color: `${Colors.danger}`,
                  fontSize: "10px",
                }}
              >
                Invalid location
              </Box>
            ) : (
              ""
            )}
          </>
        );
      },
    },

    {
      field: "Action",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <>
            <IconButton
              onClick={() => downloadWayBillsByOrderNos(row.OrderNo, documentSetting?.DocumentTemplateId || 1)}
              sx={{
                borderRadius: "5px",
                minWidth: "25px",
                height: "25px",
                padding: "2px",
                color: "#fff",
                background: Colors.linkColor,
                "&:hover": { background: Colors.linkColor, color: "#fff" },
                boxShadow: "none",
                marginRight: "5px",
              }}
              title="Print Waybill"
            >
              <PrintIcon sx={{ fontSize: "16px" }} />
            </IconButton>
            {row?.IsValidLatLng == 0 && (
              <MapButton onClick={() => handleOpenModal(row)} />
            )}
          </>
        );
      },
      flex: 1,
    },
  ];
  //#endregion
  const [selectionModel, setSelectionModel] = useState([]);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
    getOrdersRef.current = oNos;
    setSelectedDeliveryTasks(oNos);
  };



  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
    }
  }, [resetRowRef.current]);

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

  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 222
    : windowHeight - navbarHeight - 67;

  const handleColumnOrderChange = (params) => {
    setTimeout(() => {
      let newOrder = [];

      if (apiRef.current && apiRef.current.getAllColumns) {
        const cols = apiRef.current.getAllColumns();
        if (cols && cols.length > 0) {
          newOrder = cols.map((c) => c.field);
        }
      }

      if (!newOrder.length && apiRef.current && apiRef.current.exportState) {
        const state = apiRef.current.exportState();
        newOrder = state?.columns?.orderedFields || [];
      }

      if (!newOrder.length && params && typeof params.oldIndex === "number" && typeof params.targetIndex === "number") {
        const currentFields = (columnOrder && columnOrder.length > 0
          ? [...columns].sort((a, b) => {
            const indexA = columnOrder.indexOf(a.field);
            const indexB = columnOrder.indexOf(b.field);
            const weightA = indexA !== -1 ? indexA : 999;
            const weightB = indexB !== -1 ? indexB : 999;
            return weightA - weightB;
          })
          : columns
        ).map((c) => c.field);

        const movedField = currentFields[params.oldIndex];
        if (movedField) {
          currentFields.splice(params.oldIndex, 1);
          currentFields.splice(params.targetIndex, 0, movedField);
          newOrder = currentFields;
        }
      }

      if (newOrder && newOrder.length > 0) {
        try {
          localStorage.setItem(
            "deliveryTasksColumnOrder",
            JSON.stringify(newOrder)
          );
        } catch (e) { }
        setColumnOrder(newOrder);
      }
    }, 150);
  };

  const filteredColumns = React.useMemo(() => {
    let cols = columns.map((col) => {
      try {
        const savedWidthsStr = localStorage.getItem("deliveryTasksColumnWidths");
        if (savedWidthsStr) {
          const savedWidths = JSON.parse(savedWidthsStr);
          if (savedWidths && savedWidths[col.field]) {
            return { ...col, width: savedWidths[col.field], flex: undefined };
          }
        }
      } catch (e) { }
      return col;
    });

    const activeOrder = columnOrder && columnOrder.length > 0 ? columnOrder : (() => {
      try {
        const s = localStorage.getItem("deliveryTasksColumnOrder");
        return s ? JSON.parse(s) : [];
      } catch (e) {
        return [];
      }
    })();

    if (activeOrder && Array.isArray(activeOrder) && activeOrder.length > 0) {
      cols = [...cols].sort((a, b) => {
        const indexA = activeOrder.indexOf(a.field);
        const indexB = activeOrder.indexOf(b.field);
        const weightA = indexA !== -1 ? indexA : 999;
        const weightB = indexB !== -1 ? indexB : 999;
        return weightA - weightB;
      });
    }

    return cols;
  }, [columns, columnOrder]);

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
        apiRef={apiRef}
        loading={loading}
        getRowHeight={() => "auto"}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
          "& .row-duplicate-3days": {
            backgroundColor: "rgba(255, 0, 0, 0.12) !important",
            "&:hover": {
              backgroundColor: "rgba(255, 0, 0, 0.2) !important",
            }
          }
        }}
        rows={allDeliveryTask?.list ? allDeliveryTask?.list : []}
        getRowId={(row) => row.OrderNo}
        columns={filteredColumns}
        getRowClassName={(params) => {
          return params.row.IsDuplicateDays ? "row-duplicate-3days" : "";
        }}
        editMode="cell"
        processRowUpdate={async (newRow, oldRow) => {
          console.log("newRow data: ", newRow);

          if (
            newRow.Customer === oldRow.Customer &&
            newRow.Mobile1 === oldRow.Mobile1 &&
            newRow.Mobile2 === oldRow.Mobile2 &&
            newRow.Email === oldRow.Email
          ) {
            return oldRow;
          }

          const body = {
            OrderAddressId: newRow.OrderAddressId || newRow.orderAddressId || null,
            OrderNo: newRow.OrderNo || "",
            CustomerName: newRow.Customer || "",
            Mobile1: newRow.Mobile1 || "",
            Mobile2: newRow.Mobile2 || "",
            Email: newRow.Email || ""
          };

          try {
            const res = await UpdateCustomerEmail(body);
            if (res.data.isSuccess) {
              successNotification("Updated successfully");
              if (getAllDeliveryTask) getAllDeliveryTask();
              return newRow;
            } else {
              errorNotification("Failed to update");
              return oldRow;
            }
          } catch (err) {
            console.error("Save error: ", err);
            errorNotification("Error updating row");
            return oldRow;
          }
        }}
        onProcessRowUpdateError={(error) => {
          console.error("Row Update Error: ", error);
        }}
        disableSelectionOnClick={true}
        disableRowSelectionOnClick={true}
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        checkboxSelection
        rowSelectionModel={selectionModel}
        onRowSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
        columnVisibilityModel={columnVisibilityModel}
        onColumnVisibilityModelChange={handleColumnVisibilityModelChange}
        onColumnOrderChange={handleColumnOrderChange}
        onColumnWidthChange={handleColumnWidthChange}
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
      ></Menu>
      {openUpdate && (
        <UpdateOutScanModal
          open={openUpdate}
          setOpen={setOpenUpdate}
          {...props}
        />
      )}

      {slectedAddress.open && (
        <UpdateOrderAddressLatLangModal
          open={slectedAddress.open}
          setOpen={() => handleCloseUpdateMap()}
          slectedAddress={slectedAddress}
          setSelectedAddress={setSelectedAddress}
          getAllDeliveryTask={getAllDeliveryTask}

        // addressCoordinates={addressCoordinates}
        />
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

      {deleteModalOpen && (
        <DeleteConfirmationModal
          open={deleteModalOpen}
          setOpen={setDeleteModalOpen}
          loading={revertLoading?.[selectedRowToDelete?.OrderId]}
          heading="Are you sure you want to delete this order from Delivery Note?"
          message={`Order ${selectedRowToDelete?.OrderNo || ""} will be removed from Delivery Note ${selectedRowToDelete?.DeliveryNoteNo || selectedRowToDelete?.deliveryNoteNo || ""}.`}
          buttonText="Delete"
          handleDelete={async () => {
            await handleRevertDeliveryNoteDetail(selectedRowToDelete);
            setDeleteModalOpen(false);
          }}
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
export default TasksList;
