import { Avatar, Box, Menu, Stack, Dialog, DialogTitle, DialogContent, DialogActions, TextField, Button, CircularProgress, IconButton, InputLabel, Tooltip } from "@mui/material";
import ModeEditIcon from "@mui/icons-material/ModeEdit";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  ExportPDFShipmentsByDriverReceivableId,
  GetAllShipmentsByDriverReceivableId,
  UpdateDriverReceivableStatusPaid,
  UpdateDriverReceivableStatusUnPaid,
  UpdateDriverReceivable,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import OrdersModal from "../../../../components/modals/myCarrierModals/OrdersModal";
import UpdateOutScanModal from "../../../../components/modals/myCarrierModals/UpdateOutscanModal";
import UtilityClass from "../../../../utilities/UtilityClass";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { EnumPaymentStatus } from "../../../../utilities/enum";
import Colors from "../../../../utilities/helpers/Colors";
import {
  ActionStack,
  ClipboardIcon,
  CodeBox,
  DescriptionBox,
  PDFButton,
  PaidButton,
  UnPaidButton,
  ViewButton,
  amountFormat,
  centerColumn,
  navbarHeight,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
  purple,
  EditButton,
} from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";

function DriverRecievableList(props) {
  const {
    allRecieveable,
    getOrdersRef,
    loading,
    resetRowRef,
    getAllDriverReceivable,
    isFilterOpen,
  } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [openUpdate, setOpenUpdate] = useState(false);
  const [markPaid, setMarkPaid] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [orderIteminfoModal, setOrderIteminfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [orderIteminfoPdf, setOrderIteminfoPdf] = useState({
    open: false,
    loading: {},
    data: [],
  });

  const [editModalOpen, setEditModalOpen] = useState(false);
  const [editRowData, setEditRowData] = useState(null);
  const [editTotal, setEditTotal] = useState("");
  const [editExpense, setEditExpense] = useState("");
  const [editLoading, setEditLoading] = useState(false);

  const handleOpenEditModal = (row) => {
    setEditRowData(row);
    setEditTotal(row.Total || "");
    setEditExpense(row.Expense || row.CreditCard || 0);
    setEditModalOpen(true);
  };

  const handleUpdateReceivable = async () => {
    try {
      setEditLoading(true);
      const totalAmount = Number(editTotal) || 0;
      const expenseAmount = Number(editExpense) || 0;
      const cashAmount = totalAmount - expenseAmount;

      const payload = {
        DriverReceivableId: editRowData?.DriverReceivableId,
        DriverId: editRowData?.DriverId,
        ReceiveDate: editRowData?.ReceiveDate || new Date().toISOString(),
        Expense: expenseAmount,
        Cash: cashAmount,
        Total: totalAmount,
        Active: editRowData?.Active !== false,
        DeliveryNoteId: editRowData?.DeliveryNoteId,
        expensesList: null,
      };

      const res = await UpdateDriverReceivable(payload);
      if (res.data?.isSuccess) {
        successNotification("Driver receivable updated successfully");
        setEditModalOpen(false);
        getAllDriverReceivable();
      } else {
        errorNotification(res.data?.message || "Failed to update driver receivable");
      }
    } catch (error) {
      console.error(error);
      errorNotification("Something went wrong");
    } finally {
      setEditLoading(false);
    }
  };

  const downloadPDF = (rId) => {
    let params = {
      filterModel: {
        createdFrom: null,
        createdTo: null,
        start: 0,
        length: 1000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      DriverReceivableId: rId,
    };

    setOrderIteminfoPdf((prev) => ({
      ...prev,
      loading: { [rId]: true },
    }));

    ExportPDFShipmentsByDriverReceivableId(params)
      .then((res) => {
        UtilityClass.downloadPdf(res.data, "receiveable report");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_DRIVER_RECIEVEABLE_UNABLE_TO_DOWNLOAD
        );
      })
      .finally(() => {
        setOrderIteminfoPdf((prev) => ({
          ...prev,
          loading: { [rId]: false },
        }));
      });
  };
  const handleGetShipmentByReceiveableId = async (rId) => {
    let param = {
      filterModel: {
        createdFrom: null,
        createdTo: null,
        start: 0,
        length: 1000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      DriverReceivableId: rId,
    };
    setOrderIteminfoModal((prev) => ({
      ...prev,
      loading: { [rId]: true },
    }));
    await GetAllShipmentsByDriverReceivableId(param)
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
          loading: { [rId]: false },
        }));
      });
  };
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  //#region table colum
  const columns = [
    {
      field: "DriverReceivableNo",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DRIVER_RECIEVEABLE_TEXT}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            sx={{ textAlign: "center", cursor: "pointer" }}
            disableRipple
          >
            <>
              <Stack direction={"row"} sx={{ alignItems: "center" }}>
                <CodeBox
                  title={params.row.DriverReceivableNo}
                  hasColor={false}
                />
                <ClipboardIcon text={params.row.DriverReceivableNo} />
                <DescriptionBox
                  description={params.row.Comment}
                  title="Comments"
                />
              </Stack>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "DriverName",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DRIVER_RECIEVEABLE_DRIVER}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          params.row.DriverName && (
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
                  {params.row.DriverName?.slice(0, 1)}
                </Avatar>
                <Box>{params.row.DriverName}</Box>
              </Stack>
            </Box>
          )
        );
      },
    },

    {
      ...centerColumn,
      field: "ReceiveDate",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_RECEIVE_DATE
          }
        </Box>
      ),
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.ReceiveDate)}
        </Box>
      ),
      flex: 1,
    },

    {
      ...rightColumn,
      field: "Total",
      minWidth: 80,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_COD_TOTAL
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <>{amountFormat(params?.row.Total)}</>;
      },
    },
    // {
    //   field: "Comment",
    //   headerName: <Box sx={{ fontWeight: "600" }}> {"Comment"}</Box>,
    //   flex: 1,
    // },
    {
      ...rightColumn,
      field: "Expense",
      minWidth: 80,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DRIVER_RECIEVEABLE_EXPENSE}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <>{amountFormat(params?.row.Expense !== undefined ? params?.row.Expense : params?.row.CreditCard)}</>;
      },
    },

    {
      ...rightColumn,
      field: "OtherCollections",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_OTHER_COLLECTIONS
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <>{amountFormat(params?.row.OtherCollections)}</>;
      },
    },
    {
      ...rightColumn,
      minWidth: 100,
      field: "Cash",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DRIVER_RECIEVEABLE_CASH_RECEIVABLE
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <>{amountFormat(params?.row.Cash)}</>;
      },
    },

    {
      ...centerColumn,
      minWidth: 150,
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        // onClick={(e) => setAnchorEl(e.currentTarget)}
        return (
          <ActionStack direction={"row"} spacing={1}>
            <Tooltip title="Edit Receivable" arrow>
              <span>
                <EditButton
                  onClick={() => handleOpenEditModal(row)}
                />
              </span>
            </Tooltip>
            <PDFButton
              loading={orderIteminfoPdf.loading[row.DriverReceivableId]}
              onClick={() => downloadPDF(row?.DriverReceivableId)}
            />
            <ViewButton
              loading={orderIteminfoModal.loading[row.DriverReceivableId]}
              onClick={() =>
                handleGetShipmentByReceiveableId(row?.DriverReceivableId)
              }
            />
            {row?.DriverPaidStatusId != EnumPaymentStatus.Paid ? (
              <Box>
                <PaidButton
                  loading={markPaid.loading[row?.DriverReceivableId]}
                  onClick={() => handleMarkAsPaid(row)}
                />
              </Box>
            ) : (
              <Box>
                <UnPaidButton
                  background={Colors.danger}
                  loading={markPaid.loading[row?.DriverReceivableId]}
                  onClick={() => handleMarkAsUnPaid(row)}
                />
              </Box>
            )}
          </ActionStack>
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
  };
  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
    }
  }, [resetRowRef.current]);
  const handleMarkAsPaid = async (data) => {
    try {
      var param = {
        DriverReceivableId: data?.DriverReceivableId,
      };
      setMarkPaid((prev) => ({
        ...prev,
        loading: { [data.DriverReceivableId]: true },
      }));

      const response = await UpdateDriverReceivableStatusPaid(param);
      if (response.data?.isSuccess) {
        getAllDriverReceivable();
        successNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_DRIVER_RECIEVEABLE_PAID_SUCCESSFULLY
        );
      } else {
        errorNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_DRIVER_RECIEVEABLE_PAID_UNSUCCESSFULLY
        );
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      if (error?.response.data.errors) {
        UtilityClass.showErrorNotificationWithDictionary(
          error?.response?.data?.errors
        );
      }
    } finally {
      setMarkPaid((prev) => ({
        ...prev,
        loading: { [data.DriverReceivableId]: false },
      }));
    }
  };
  const handleMarkAsUnPaid = async (data) => {
    try {
      var param = {
        DriverReceivableId: data?.DriverReceivableId,
      };
      setMarkPaid((prev) => ({
        ...prev,
        loading: { [data.DriverReceivableId]: true },
      }));

      const response = await UpdateDriverReceivableStatusUnPaid(param);
      if (response.data?.isSuccess) {
        getAllDriverReceivable();
        successNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_DRIVER_RECIEVEABLE_PAID_SUCCESSFULLY
        );
      } else {
        errorNotification(
          LanguageReducer?.languageType
            ?.MY_CARRIER_DRIVER_RECIEVEABLE_PAID_UNSUCCESSFULLY
        );
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      if (error?.response.data.errors) {
        UtilityClass.showErrorNotificationWithDictionary(
          error?.response?.data?.errors
        );
      }
    } finally {
      setMarkPaid((prev) => ({
        ...prev,
        loading: { [data.DriverReceivableId]: false },
      }));
    }
  };

  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 140.5
    : windowHeight - navbarHeight - 65;

  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: calculatedHeight,
      }}
    >
      <DataGrid
        loading={loading}
        getRowHeight={() => "43px"}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        rows={allRecieveable?.list ? allRecieveable?.list : []}
        getRowId={(row) => row.DriverReceivableId}
        columns={columns}
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        selectionModel={selectionModel}
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
      <UpdateOutScanModal
        open={openUpdate}
        setOpen={setOpenUpdate}
        {...props}
      />
      {orderIteminfoModal.data?.result && (
        <OrdersModal
          onClose={() =>
            setOrderIteminfoModal((prev) => ({ ...prev, open: false }))
          }
          data={orderIteminfoModal?.data?.result}
          open={orderIteminfoModal.open}
        />
      )}
      <ModalComponent
        open={editModalOpen}
        onClose={() => setEditModalOpen(false)}
        maxWidth="xs"
        title={`Edit Driver Receivable - ${editRowData?.DriverReceivableNo || ""}`}
        actionBtn={
          <ModalButtonComponent
            title="Save"
            bg={purple}
            onClick={handleUpdateReceivable}
            disabled={editLoading}
          />
        }
      >
        <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5, mt: 1 }}>
          <InputLabel sx={styleSheet.inputLabel}>Total COD Amount</InputLabel>
          <TextField
            type="number"
            value={editTotal}
            onChange={(e) => setEditTotal(e.target.value)}
            fullWidth
            variant="outlined"
            size="small"
          />
          <InputLabel sx={styleSheet.inputLabel}>Expense</InputLabel>
          <TextField
            type="number"
            value={editExpense}
            onChange={(e) => setEditExpense(e.target.value)}
            fullWidth
            variant="outlined"
            size="small"
          />
          <InputLabel sx={styleSheet.inputLabel}>Cash Receivable</InputLabel>
          <TextField
            type="number"
            value={(Number(editTotal) || 0) - (Number(editExpense) || 0)}
            disabled
            fullWidth
            variant="outlined"
            size="small"
          />
        </Box>
      </ModalComponent>
    </Box>
  );
}
export default DriverRecievableList;
