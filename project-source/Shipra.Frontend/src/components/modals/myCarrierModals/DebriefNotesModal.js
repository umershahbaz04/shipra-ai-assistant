import { makeStyles } from "@material-ui/core";
import Add from '@mui/icons-material/Add';
import Delete from '@mui/icons-material/Delete';
import { LoadingButton } from "@mui/lab";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import Popover from "@mui/material/Popover";
import ModeEditIcon from "@mui/icons-material/ModeEdit";
import {
  Box,
  Button,
  Chip,
  Grid,
  IconButton,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import { red } from "@mui/material/colors";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import SearchInputAutoCompleteMultiple from "../../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";
import CreateAbleSelectComponent from "../../../.reUseableComponents/TextField/CreateAbleSelectComponent";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  CompleteDeliveryNote,
  CreateDeliveryNoteDetailPendingForReturnStatus,
  CreateExpenseCategory,
  GetAllExpenseCategoryLookup,
  GetDeliveryNoteDetailForDebrief,
  GetCompletedDeliveryNoteExpenses,
  RevertDeliveryNoteDetailByOrderId,
  UncompleteDeliveryNote,
  UpdateDeliveryNoteInOperation,
  UpdateOrderStatusOnDebrief,
  CreateClientOrderLabel,
  CreateClientOrderLabelLookup,
  DeleteClientOrderLabel,
  GetAllClientOrderLabelLookupForSelection,
} from "../../../api/AxiosInterceptors";
import "../../../assets/styles/datePickerCustomStyles.css";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  EnumCarrierTrackingStatus,
  EnumDeliveryNoteDetailStatusLookup,
  EnumOptions,
  EnumPaymentMethod,
} from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  CodeBox,
  DescriptionBoxWithChild,
  DialerBox,
  amountFormat,
  purple,
  rightColumn,
  usePagination,
  ClipboardIcon,
  orderLabelColor,
  MenuComponent,
  useMenuForLoop,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import StatusBadge from "../../shared/statudBadge";

const useStyles = makeStyles({
  topScrollPaper: {
    alignItems: "flex-start",
  },
  topPaperScrollBody: {
    verticalAlign: "top",
  },
});

//#endregion
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

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function DebriefNotesModal(props) {
  let {
    open,
    setOpen,
    selectedRowData,
    setSelectedRowData,
    getAllDeliveryNote,
  } = props;
  const classes = useStyles();

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inOpsloadingState, setInOpsloadingState] = React.useState({});
  const [revertloadingState, setRevertloadingState] = React.useState({});
  const [pfrloadingState, setPfrloadingState] = React.useState({});
  const [markCompleteState, setMarkCompleteState] = React.useState({});
  const [isCompleteLoading, setIsCompleteLoading] = React.useState(false);
  const [isOpenAddExpense, setIsOpenAddExpense] = React.useState(false);
  const [isDeliveryNotCompleted, setIDeliveryNotCompleted] =
    React.useState(true);
  const [allExpenseCategory, setAllExpenseCategory] = useState([]);
  const [openUncompleteModal, setOpenUncompleteModal] = useState(false);

  const [allDeliveryNoteDetailForDebrief, setAllDeliveryNoteDetailForDebrief] =
    useState([]);
  const [isAllListLoading, setIsAllListLoading] = useState(false);
  const [totalDebriefAmount, setTotalDebriefAmount] = useState(0);
  const [isTotalOverridden, setIsTotalOverridden] = useState(false);

  const [orderLabel, setOrderLabel] = useState([]);
  const { anchorEl: labelAnchorEl, openElem: labelOpenElem, handleOpen: handleLabelOpen, handleClose: handleLabelClose } = useMenuForLoop();
  const [selectedOrderLabels, setSelectedOrderLabels] = useState();
  const [labelLoading, setLabelLoading] = useState(false);
  const [searchTags, setSearchTags] = useState([]);
  const [selectedRowIds, setSelectedRowIds] = useState([]);

  const filteredList = React.useMemo(() => {
    if (!allDeliveryNoteDetailForDebrief?.list) return [];
    if (!searchTags || searchTags.length === 0) return allDeliveryNoteDetailForDebrief.list;

    return allDeliveryNoteDetailForDebrief.list.filter((item) => {
      return searchTags.some((tag) => {
        const lowerTag = tag.toLowerCase().trim();
        const orderNo = (item.OrderNo || "").toLowerCase();
        const phone = (item.Phone || "").toLowerCase();
        const phone2 = (item.Phone2 || "").toLowerCase();

        return orderNo.includes(lowerTag) || phone.includes(lowerTag) || phone2.includes(lowerTag);
      });
    });
  }, [allDeliveryNoteDetailForDebrief?.list, searchTags]);

  const getAllClientOrderLabelLookupForSelection = async () => {
    try {
      const response = await GetAllClientOrderLabelLookupForSelection();
      if (response) {
        setOrderLabel(response?.data?.result);
      }
    } catch (e) {
      console.error(e);
    }
  };

  useEffect(() => {
    getAllClientOrderLabelLookupForSelection();
  }, []);

  const handleChipDelete = async (id, lable) => {
    const response = await DeleteClientOrderLabel(id, lable);
    if (response?.data?.isSuccess) {
      successNotification("Labels Delete Successfully");
      getDeliveryNoteDetailForDebrief();
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
        getDeliveryNoteDetailForDebrief();
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

  useEffect(() => {
    if (allDeliveryNoteDetailForDebrief?.list && !isTotalOverridden) {
      const sum = allDeliveryNoteDetailForDebrief.list
        .filter(
          (item) =>
            item.CarrierTrackingStatusId === EnumCarrierTrackingStatus.Delivered &&
            (item.PaymentMethodId === EnumPaymentMethod?.CashOnDelivery ||
              item.PaymentMethodId === 2 ||
              !item.PaymentMethodId)
        )
        .reduce((acc, item) => acc + (Number(item.Amount) || 0), 0);
      setTotalDebriefAmount(sum);
    }
  }, [allDeliveryNoteDetailForDebrief, isTotalOverridden]);

  useEffect(() => {
    if (Object.keys(selectedRowData).length > 0) {
      let isCompleted = selectedRowData.IsCompleted
        ? selectedRowData.IsCompleted
        : false;
      setIDeliveryNotCompleted(isCompleted);
      getDeliveryNoteDetailForDebrief();
    }
  }, [selectedRowData]);

  let getDeliveryNoteDetailForDebrief = async () => {
    let params = {
      deliveryNoteId: selectedRowData.DeliveryNoteId,
    };
    setIsAllListLoading(true);
    let res = await GetDeliveryNoteDetailForDebrief(params);
    if (res.data.result !== null) {
      setIsTotalOverridden(false);
      setAllDeliveryNoteDetailForDebrief(res.data.result);
      if (selectedRowData.IsCompleted) {
        let expenseRes = await GetCompletedDeliveryNoteExpenses(params);
        if (expenseRes.data.result !== null) {
          const { Expenses, TotalAmount } = expenseRes.data.result;
          if (Expenses && Expenses.length > 0) {
            const loadedExpenses = Expenses.map((exp, index) => {
              return {
                id: index + 1,
                expenseCategoryId: { id: exp.expenseCategoryId, text: exp.expenseCategoryName || "" },
                amount: exp.amount,
                expenseDate: exp.expenseDate ? new Date(exp.expenseDate) : null,
                detail: exp.details || "",
              };
            });
            setExpenseRows(loadedExpenses);
          } else {
            setExpenseRows([]);
          }
          setTotalDebriefAmount(TotalAmount || 0);
        }
      } else {
        setExpenseRows([]);
      }
    }
    setIsAllListLoading(false);
  };

  const handleAmountChange = (orderNo, value) => {
    setAllDeliveryNoteDetailForDebrief((prev) => {
      if (!prev || !prev.list) return prev;
      const newList = prev.list.map((item) => {
        if (item.OrderNo === orderNo) {
          return {
            ...item,
            Amount: value === "" ? "" : Number(value),
          };
        }
        return item;
      });
      return {
        ...prev,
        list: newList,
      };
    });
  };

  const updateRowLocalStatus = (orderNo, type) => {
    setAllDeliveryNoteDetailForDebrief((prev) => {
      if (!prev || !prev.list) return prev;
      const newList = prev.list.map((item) => {
        if (item.OrderNo === orderNo) {
          let detailStatusId = item.DeliveryNoteDetailStatusId;
          let detailStatus = item.DeliveryNoteDetailStatus;
          let trackingStatusId = item.CarrierTrackingStatusId;
          let trackingStatus = item.CarrierTrackingStatus;

          if (type === 1) { // In Operation
            detailStatusId = 3; // Attempted
            detailStatus = "Attempted";
            trackingStatusId = EnumCarrierTrackingStatus.InOperation;
            trackingStatus = "In Operation";
          } else if (type === 2) { // Cancelled
            detailStatusId = 2; // Completed
            detailStatus = "Completed";
            trackingStatusId = EnumCarrierTrackingStatus.Cancelled;
            trackingStatus = "Cancelled";
          } else if (type === 3) { // Delivered
            detailStatusId = 2; // Completed
            detailStatus = "Completed";
            trackingStatusId = EnumCarrierTrackingStatus.Delivered;
            trackingStatus = "Delivered";
          }

          return {
            ...item,
            DeliveryNoteDetailStatusId: detailStatusId,
            DeliveryNoteDetailStatus: detailStatus,
            CarrierTrackingStatusId: trackingStatusId,
            CarrierTrackingStatus: trackingStatus,
          };
        }
        return item;
      });
      return {
        ...prev,
        list: newList,
      };
    });
  };

  const handleInOpsButtonClick = (OrderNo) => {
    updateRowLocalStatus(OrderNo, 1);
  };

  const handlePendingForReturnButtonClick = (OrderNo) => {
    updateRowLocalStatus(OrderNo, 2);
  };

  const handleMarkCompleteButtonClick = (OrderNo) => {
    updateRowLocalStatus(OrderNo, 3);
  };


  const handleClose = () => {
    setInOpsloadingState({});
    setOpen(false);
  };
  const getAllExpenseCategoryLookup = async () => {
    let res = await GetAllExpenseCategoryLookup();
    if (res.data.result !== null) {
      setAllExpenseCategory(res.data.result);
    }
  };
  useEffect(() => {
    getAllExpenseCategoryLookup();
  }, []);
  const handleComplete = async () => {
    const itemsToProceed = selectedRowIds.length > 0 
      ? allDeliveryNoteDetailForDebrief.list.filter((item) => selectedRowIds.includes(item.OrderNo))
      : allDeliveryNoteDetailForDebrief.list;

    if (
      itemsToProceed.filter(
        (x) =>
          x.DeliveryNoteDetailStatusId ==
          EnumDeliveryNoteDetailStatusLookup.Pending,
      ).length == 0
    ) {
      var modifiedExpense = expenseRows?.map((row) => {
        return {
          amount: row.amount,
          expenseDate: UtilityClass.getFormatedDateWithoutTime(row.expenseDate),
          detail: row.detail,
          expenseCategoryId: row.expenseCategoryId?.id,
        };
      });
      var debriefItems = itemsToProceed.map((item) => {
        let statusId = 1; // InOperation default
        if (item.CarrierTrackingStatusId === EnumCarrierTrackingStatus.Delivered) {
          statusId = 3; // Delivered
        } else if (item.CarrierTrackingStatusId === EnumCarrierTrackingStatus.Cancelled) {
          statusId = 2; // Cancelled
        }
        return {
          deliveryNoteDetailId: item.DeliveryNoteDetailId,
          orderId: item.OrderId,
          amount: Number(item.Amount) || 0,
          statusId: statusId,
        };
      });
      let params = {
        deliveryNoteId: selectedRowData?.DeliveryNoteId,
        totalAmount: Number(totalDebriefAmount) || 0,
        expenseList: modifiedExpense || [],
        debriefItems: debriefItems,
      };
      setIsCompleteLoading(true);
      let res = await CompleteDeliveryNote(params);
      setIsCompleteLoading(false);
      if (res.data.isSuccess) {
        setInOpsloadingState({});
        handleClose();
        getAllDeliveryNote();
      } else {
        errorNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_DELIVERY_NOTES_SOMETHING_WENT_WRONG,
        );
      }
    } else {
      errorNotification(
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_NOTES_PLEASE_MAKE_SURE_ALL_ORDERS_ARE_MARKED_TO_COMPLETE_THIS_NOTE,
      );
    }
  };
  //#region  table col
  const columns = [
    {
      field: "OrderNo",
      align: "center",
      headerAlign: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_ORDER_NO}
        </Box>
      ),
      flex: 1,
      minWidth: 90,
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
              <Box sx={{ fontWeight: "bold", display: "flex", alignItems: "center", justifyContent: "center", gap: 0.5 }}>
                <CodeBox title={params.row.OrderNo} hasColor={false} />
                <ClipboardIcon text={params.row.OrderNo} />
              </Box>
              <Box>{params.row.TrackingNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "DropOfAddress",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_NOTES_CUSTOMER_INFO
          }
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ fontWeight: "bold" }}>
            {params.row.Customer}
          </Box>
        );
      },
    },
    {
      field: "Phone",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MOBILE_NUMBER_TEXT || "Mobile No."}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            {params.row.Phone && (
              <DialerBox phone={params.row.Phone} />
            )}
          </>
        );
      },
    },

    {
      field: "CarrierTrackingStatus",
      align: "center",
      headerAlign: "center",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_NOTES_TRACKING_STATUS
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <Box>{params?.row.CarrierTrackingStatus}</Box>;
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
      field: "DriverLastUpdatedStatus",
      align: "center",
      headerAlign: "center",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_NOTES_UPDATE_BY_DRIVER
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          params?.row.DriverLastUpdatedStatus && (
            <Box>
              <StatusBadge
                title={params?.row.DriverLastUpdatedStatus}
                maxWidth="88px"
                borderColor="rgba(0, 186, 119, 0.2)"
                color="#1E1E1E;"
                bgColor="#EAEAEA"
              />
            </Box>
          )
        );
      },
    },
    {
      field: "DeliveryNoteDetailStatus",
      align: "center",
      headerAlign: "center",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_NOTES_NOTE_DETAIL_STATUS
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        let bgColor = "rgb(86 58 213)";
        if (
          params?.row.DeliveryNoteDetailStatusId ===
          EnumDeliveryNoteDetailStatusLookup.Completed
        ) {
          bgColor = Colors.succes;
        } else if (
          params?.row.DeliveryNoteDetailStatusId ===
          EnumDeliveryNoteDetailStatusLookup.Pending
        ) {
          bgColor = Colors.warning;
        }

        return (
          <Box>
            <StatusBadge
              title={params?.row.DeliveryNoteDetailStatus}
              maxWidth="88px"
              borderColor="rgba(0, 186, 119, 0.2)"
              color={"#ffff"}
              bgColor={bgColor}
            />
          </Box>
        );
      },
    },

    {
      field: "Description",
      align: "center",
      headerAlign: "center",
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
      field: "Remarks",
      align: "center",
      headerAlign: "center",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_REMARKS}
        </Box>
      ),
      flex: 1,
      minWidth: 100,
      renderCell: (params) => {
        return (
          params.row.Remarks && (
            <Tooltip title={params.row.Remarks} placement="top" arrow>
              <Box sx={{
                overflow: "hidden",
                textOverflow: "ellipsis",
                whiteSpace: "nowrap",
                maxWidth: "100px",
                cursor: "pointer"
              }}>
                <StatusBadge
                  title={params.row.Remarks}
                  color="#1E1E1E;"
                  bgColor="#EAEAEA"
                />
              </Box>
            </Tooltip>
          )
        );
      },
    },
    {
      field: "Amount",
      align: "center",
      headerAlign: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_AMOUNT}
        </Box>
      ),
      flex: 1,
      minWidth: 130,
      renderCell: (params) => {
        const pm = params.row.PaymentMethod || params.row.PaymentMethodName || (params.row.PaymentMethodId === 1 ? "Prepaid" : "COD");
        const isCOD = params.row.PaymentMethodId === 2 || String(pm).toLowerCase().includes("cod") || String(pm).toLowerCase().includes("cash");
        return (
          <Box display="flex" flexDirection="column" alignItems="center" justifyContent="center" gap={0.5} width="100%" sx={{ mt: 1 }}>
            {isCOD ? (
              <TextField
                type="number"
                size="small"
                disabled={isDeliveryNotCompleted}
                value={params.row.Amount ?? ""}
                onChange={(e) => handleAmountChange(params.row.OrderNo, e.target.value)}
                sx={{
                  width: "90px",
                  "& .MuiOutlinedInput-input": {
                    padding: "4px 8px",
                    textAlign: "center",
                    fontWeight: 600,
                    fontSize: "12px",
                  },
                }}
              />
            ) : (
              <Box sx={{ fontWeight: 600 }}>{amountFormat(params.row.Amount)}</Box>
            )}
            {pm && (
              <StatusBadge
                title={pm}
                maxWidth="80px"
                color={isCOD ? "#0288d1" : "#7b1fa2"}
                bgColor={isCOD ? "#e1f5fe" : "#f3e5f5"}
              />
            )}
          </Box>
        );
      },
    },
    {
      field: "Action",
      align: "center",
      headerAlign: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_ACTION}
        </Box>
      ),
      minWidth: 330,
      flex: 1,
      hide: isDeliveryNotCompleted,
      renderCell: (params) => {
        return (
          <>
            <Stack direction={"row"} gap={1}>
                <Button
                  size="small"
                  sx={{
                    background: params?.row.CarrierTrackingStatusId == EnumCarrierTrackingStatus.InOperation ? "rgba(0, 0, 0, 0.12) !important" : "#6DC2FF !important",
                    color: params?.row.CarrierTrackingStatusId == EnumCarrierTrackingStatus.InOperation ? "rgba(0, 0, 0, 0.26) !important" : "#fff !important",
                    textTransform: "capitalize !important",
                    padding: "0px 5px",
                  }}
                  disabled={
                    params?.row.CarrierTrackingStatusId ==
                    EnumCarrierTrackingStatus.InOperation
                  }
                  onClick={() => handleInOpsButtonClick(params.row.OrderNo)}
                  variant="contained"
                >
                  {inOpsloadingState[params.row.OrderNo]
                    ? LanguageReducer?.languageType
                        ?.MY_CARRIER_DELIVERY_NOTES_PENDING
                    : LanguageReducer?.languageType
                        ?.MY_CARRIER_DELIVERY_NOTES_IN_OPERATION}
                </Button>
                 <LoadingButton
                  disabled={
                    params?.row.CarrierTrackingStatusId ==
                    EnumCarrierTrackingStatus.Cancelled
                  }
                  loading={pfrloadingState[params.row.OrderNo]}
                  size="small"
                  color="warning"
                  sx={{
                    textTransform: "capitalize !important",
                    padding: "0px 5px",
                  }}
                  onClick={() =>
                    handlePendingForReturnButtonClick(params.row.OrderNo)
                  }
                  variant="contained"
                >
                  {"Cancelled"}
                </LoadingButton>
                <LoadingButton
                  disabled={
                    params?.row.CarrierTrackingStatusId ==
                    EnumCarrierTrackingStatus.Delivered
                  }
                  loading={markCompleteState[params.row.OrderNo]}
                  size="small"
                  color="success"
                  sx={{
                    // background: "#dc3545 !important",
                    textTransform: "capitalize !important",
                    padding: "0px 5px",
                  }}
                  onClick={() =>
                    handleMarkCompleteButtonClick(params.row.OrderNo)
                  }
                  variant="contained"
                >
                  {"Delivered"}
                </LoadingButton>
              </Stack>
          </>
        );
      },
    },
  ];
  //#endregion

  //#region dynamic expense row handles
  const [selectedCategoryValue, setSelectedCategoryValue] = useState("");
  const [isSubmiting, setIsSubmiting] = useState("");

  const createCategoey = async () => {
    const body = {
      expenseName: selectedCategoryValue,
    };
    console.log("body::", body);
    setIsSubmiting(true);

    CreateExpenseCategory(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification(
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_NOTES_CATEGORY_CREATED_SUCCESSFULLY,
          );
          getAllExpenseCategoryLookup();
          setSelectedCategoryValue("");
        }
      })
      .catch((e) => {
        console.log("e", e);
        if (!e?.response?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors,
          );
        } else {
          errorNotification(
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_NOTES_SOMETHING_WENT_WRONG,
          );
        }
      })
      .finally((e) => {
        setIsSubmiting(false);
      });
  };

  const defaultExpenseListItem = {
    id: 1,
    expenseCategoryId: 0,
    amount: 0,
    expenseDate: null,
    detail: "",
  };
  const [expenseRows, setExpenseRows] = useState([]);
  const handleAddRow = () => {
    const newRow = {
      id: expenseRows.length + 1,
      expenseCategoryId:
        allExpenseCategory.length > 0
          ? allExpenseCategory[0]
          : { id: 0, text: "Please select" },
      amount: 0,
      expenseDate: new Date(),
      detail: "",
    };
    let validate = expenseRows?.filter(
      (x) => x.expenseCategoryId?.id == 0 || !x.expenseCategoryId?.id,
    );
    let validateDate = expenseRows?.filter((x) => x.expenseDate == null);
    if (validate?.length > 0) {
      errorNotification(
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_NOTES_PLEASE_CHOOSE_CATEGORY,
      );
    } else if (validateDate?.length > 0) {
      errorNotification(
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_NOTES_PLEASE_SELECT_DATE,
      );
    } else {
      setExpenseRows([...expenseRows, newRow]);
    }
  };

  const handleDeleteRow = (id) => {
    const updatedRows = expenseRows.filter((row) => row.id !== id);
    setExpenseRows(updatedRows);
  };
  const handleCategorySelectChange = (id, newValue) => {
    const updatedRows = expenseRows.map((row) =>
      row.id === id ? { ...row, expenseCategoryId: newValue } : row,
    );
    setExpenseRows(updatedRows);
  };

  const handleInputChange = (id, field, value) => {
    const updatedRows = expenseRows.map((row) => {
      if (row.id === id) {
        return { ...row, [field]: value };
      }
      return row;
    });
    setExpenseRows(updatedRows);
  };
  //#endregion
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 50);

  const totalExpenseSum = expenseRows.reduce((sum, item) => sum + (Number(item.amount) || 0), 0);
  const grandTotal = (Number(totalDebriefAmount) || 0) - totalExpenseSum;

  return (
    <>
      <ModalComponent
        open={open}
        onClose={handleClose}
        maxWidth="xl"
        paperSx={{ maxWidth: '95%', width: '95%' }}
        title={`${
          LanguageReducer?.languageType?.DEBRIEF_TEXT
        } - ${selectedRowData?.NoteNo || ""}`}
        actionBtn={
          isDeliveryNotCompleted ? (
            <ModalButtonComponent
              title="Uncomplete Note"
              loading={isCompleteLoading}
              bg="#d32f2f"
              onClick={() => {
                setOpenUncompleteModal(true);
              }}
            />
          ) : (
            <ModalButtonComponent
              title={
                LanguageReducer?.languageType?.COMPLETE_TEXT + " " + "Note"
              }
              loading={isCompleteLoading}
              bg={purple}
              onClick={handleComplete}
              disabled={
                allDeliveryNoteDetailForDebrief?.list?.some(
                  (x) =>
                    x.DeliveryNoteDetailStatusId ==
                    EnumDeliveryNoteDetailStatusLookup.Pending
                )
              }
            />
          )
        }
      >
        <Box sx={{ px: 2, pb: 1, mb: 1, borderBottom: "1px solid #eee" }}>
          <Typography variant="body2" sx={{ color: isDeliveryNotCompleted ? "#2e7d32" : "#d32f2f", fontWeight: 500 }}>
            {isDeliveryNotCompleted
              ? "* Note: This delivery note is completed. The details and expenses below are read-only."
              : "* Note: You must perform an action for all orders below; otherwise, the delivery note cannot be completed."
            }
          </Typography>
        </Box>
        {/* <Grid mx={3} mb={2}>
            <Stack direction={"column"}>
              <Stack direction={"row"}>
                <Box sx={{ fontWeight: "bold" }}> Total: 10</Box>
              </Stack>
            </Stack>
          </Grid> */}
        {(expenseRows.length > 0 || !isDeliveryNotCompleted) && (
          <>
            <Box sx={{ px: 2, mt: 1, mb: 1 }}>
              <Typography variant="subtitle2" sx={{ fontWeight: 600, color: "var(--primary-color)" }}>
                Driver Expenses:
              </Typography>
            </Box>
            {expenseRows.map((row) => (
              <Grid
                // sx={{
                //   "& > :not(style)": { m: 1, width: "25ch" },
                // }}
                px={2}
                container
                my={1}
                noValidate
                autoComplete="off"
                direction="row"
                key={row.id}
                spacing={1}
              >
                <Grid item xs={12} sm={12} md={3} lg={3}>
                  <Grid direction={"row"}>
                    <SelectComponent
                      name="category"
                      options={allExpenseCategory}
                      value={row.expenseCategoryId}
                      optionLabel={EnumOptions.EXPENSE_TYPE.LABEL}
                      optionValue={EnumOptions.EXPENSE_TYPE.VALUE}
                      disabled={isDeliveryNotCompleted}
                      onChange={(e, val) => {
                        handleCategorySelectChange(row.id, val);
                      }}
                      label={
                        LanguageReducer?.languageType
                          ?.MY_CARRIER_DELIVERY_NOTES_CATEGORY
                      }
                      onInputChange={(e, val) => {
                        setSelectedCategoryValue(val);
                      }}
                      noOptionsText={
                        <LoadingButton
                          disabled={selectedCategoryValue?.trim() == "" || isDeliveryNotCompleted}
                          loading={isSubmiting}
                          color="warning"
                          variant="contained"
                          startIcon={<Add />}
                          disableRipple
                          disableElevation
                          disableFocusRipple
                          onMouseDown={(e) => {
                            createCategoey();
                          }}
                        >
                          No results! Add
                        </LoadingButton>
                      }
                    />
                  </Grid>
                </Grid>
                <Grid item xs={12} sm={12} md={3} lg={3}>
                  <TextField
                    id="amount"
                    size="small"
                    label="Amount"
                    variant="outlined"
                    fullWidth
                    disabled={isDeliveryNotCompleted}
                    value={row.amount}
                    onChange={(e) =>
                      handleInputChange(row.id, "amount", e.target.value)
                    }
                    sx={{ style: { width: "120px" } }}
                  />
                </Grid>
                <Grid item xs={12} sm={12} md={3} lg={3}>
                  <Box>
                    <CustomReactDatePickerInput
                      value={row.expenseDate}
                      disabled={isDeliveryNotCompleted}
                      onClick={(date) =>
                        handleInputChange(row.id, "expenseDate", date)
                      }
                      size="small"
                      label="Expense Date"
                      inputProps={{ style: { width: "200px" } }}
                    />
                  </Box>
                </Grid>
                <Grid item xs={12} sm={12} md={3} lg={3}>
                  <Stack direction={"row"}>
                    <TextField
                      id="outlined-basidetail"
                      size="small"
                      label="Detail"
                      variant="outlined"
                      fullWidth
                      disabled={isDeliveryNotCompleted}
                      value={row.detail}
                      onChange={(e) =>
                        handleInputChange(row.id, "detail", e.target.value)
                      }
                      inputProps={{ style: { width: "120px" } }}
                    />
                    {/* <ActionButtonDelete onClick={() => handleDeleteRow(row.id)} /> */}
                    {!isDeliveryNotCompleted && (
                      <IconButton
                        aria-label="delete"
                        size="small"
                        onClick={() => handleDeleteRow(row.id)}
                      >
                        <Delete fontSize="small" sx={{ color: red[500] }} />
                      </IconButton>
                    )}
                  </Stack>
                </Grid>
              </Grid>
            ))}
            {!isDeliveryNotCompleted && (
              <Stack
                gap={1}
                mx={2}
                mb={2}
                direction={"row"}
                justifyContent={"right"}
              >
                <ActionButtonCustom
                  label="Add Expense"
                  onClick={(e) => handleAddRow()}
                />
                {/* <Button
                    variant="outlined"
                    size="small"
                    onClick={(e) => handleAddRow()}
                  >
                    <Add fontSize="small" />
                    Add Expense
                  </Button> */}
              </Stack>
            )}
          </>
        )}

        <Box sx={{ px: 0, pb: 1 }}>
          <SearchInputAutoCompleteMultiple
            placeholder="Search by Mobile 1, Mobile 2, Order No"
            inputFields={searchTags}
            onChange={(e, val) => setSearchTags(val)}
          />
        </Box>

        <DataGrid
          loading={isAllListLoading}
          rowHeight={55}
          headerHeight={45}
          autoHeight
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          rows={
            filteredList
          }
          getRowId={(row) => row.OrderNo}
          columns={columns}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[50, 100, 150]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          selectionModel={selectedRowIds}
          onSelectionModelChange={(newSelection) => setSelectedRowIds(newSelection)}
          rowSelectionModel={selectedRowIds}
          onRowSelectionModelChange={(newSelection) => setSelectedRowIds(newSelection)}
        />

        <Stack
          direction="column"
          gap={1.5}
          sx={{
            px: 2,
            py: 1.5,
            borderTop: "1px solid #e0e0e0",
            backgroundColor: "#fafafa",
          }}
        >
          {/* Row 1: Total Amount */}
          <Box sx={{ display: "flex", justifyContent: "flex-end", alignItems: "center", gap: 2 }}>
            <Box sx={{ display: "flex", flexDirection: "column", alignItems: "flex-end" }}>
              <Box sx={{ fontWeight: 600, fontSize: "14px", color: "#333" }}>
                Total Amount:
              </Box>
              <Box sx={{ fontSize: "10px", color: "#555", fontWeight: 500 }}>
                (Sum of Completed COD Orders)
              </Box>
            </Box>
            <TextField
              id="totalDebriefAmount"
              size="small"
              type="number"
              disabled={isDeliveryNotCompleted}
              variant="outlined"
              value={totalDebriefAmount}
              onChange={(e) => {
                setIsTotalOverridden(true);
                setTotalDebriefAmount(e.target.value === "" ? "" : Number(e.target.value));
              }}
              inputProps={{
                style: {
                  textAlign: "right",
                  fontWeight: "bold",
                  fontSize: "14px",
                  width: "130px",
                },
              }}
            />
          </Box>

          {/* Row 2: Total Expense */}
          <Box sx={{ display: "flex", justifyContent: "flex-end", alignItems: "center", gap: 2 }}>
            <Box sx={{ display: "flex", flexDirection: "column", alignItems: "flex-end" }}>
              <Box sx={{ fontWeight: 600, fontSize: "14px", color: "#333" }}>
                Total Expense:
              </Box>
              <Box sx={{ fontSize: "10px", color: "#555", fontWeight: 500 }}>
                (Sum of Driver Expenses)
              </Box>
            </Box>
            <TextField
              id="totalExpenseAmount"
              size="small"
              type="number"
              disabled={true}
              variant="outlined"
              value={totalExpenseSum}
              inputProps={{
                style: {
                  textAlign: "right",
                  fontWeight: "bold",
                  fontSize: "14px",
                  width: "130px",
                },
              }}
            />
          </Box>

          {/* Row 3: Grand Total */}
          <Box sx={{ display: "flex", justifyContent: "flex-end", alignItems: "center", gap: 2 }}>
            <Box sx={{ display: "flex", flexDirection: "column", alignItems: "flex-end" }}>
              <Box sx={{ fontWeight: 700, fontSize: "15px", color: "#111" }}>
                Grand Total:
              </Box>
              <Box sx={{ fontSize: "10px", color: "#555", fontWeight: 500 }}>
                (Total Amount - Total Expense)
              </Box>
            </Box>
            <TextField
              id="grandTotalAmount"
              size="small"
              type="number"
              disabled={true}
              variant="outlined"
              value={grandTotal}
              inputProps={{
                style: {
                  textAlign: "right",
                  fontWeight: "bold",
                  fontSize: "15px",
                  color: "var(--primary-color)",
                  width: "130px",
                },
              }}
            />
          </Box>
        </Stack>
      </ModalComponent>
      {/* {isOpenAddExpense && (
        <AddDriverExpenseModal
          open={isOpenAddExpense}
          setOpen={setIsOpenAddExpense}
          getAllExpenseCategoryLookup={getAllExpenseCategoryLookup}
        />
      )} */}
      {openUncompleteModal && (
        <DeleteConfirmationModal
          open={openUncompleteModal}
          setOpen={setOpenUncompleteModal}
          loading={isCompleteLoading}
          heading="WARNING"
          message={`Reverting this completed delivery note will delete all associated Driver Receivables, locked payments, and expenses. This action cannot be undone. Are you sure you want to revert/uncomplete the entire note?`}
          buttonText="Yes, Revert"
          handleDelete={async () => {
            setIsCompleteLoading(true);
            try {
              const response = await UncompleteDeliveryNote({ DeliveryNoteId: selectedRowData?.DeliveryNoteId });
              if (response?.data?.isSuccess) {
                successNotification("Delivery Run Sheet reverted to In Operation successfully");
                handleClose();
                getAllDeliveryNote();
              } else {
                errorNotification(response?.data?.result?.message || "Failed to uncomplete the delivery note");
              }
            } catch (e) {
              console.error(e);
              errorNotification("Something went wrong while reverting the delivery note");
            } finally {
              setIsCompleteLoading(false);
              setOpenUncompleteModal(false);
            }
          }}
        />
      )}
    </>
  );
}
export default DebriefNotesModal;
