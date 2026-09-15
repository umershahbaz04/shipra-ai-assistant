import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";
import {
  Avatar,
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  Stack,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import {
  DeleteCarrierPaymentSettlement,
  ExcelExportCarrierSettlementById,
  MarkCarrierSettlementPaid,
  MarkCarrierSettlementUnPaid,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import CarrierSettlementPOPViewImagesModal from "../../../components/modals/carrierModals/CarriedSattlementOfEditModal.js";
import UploadCarrierSettlementFileModal from "../../../components/modals/carrierModals/CarrierSattlementsOfModal.js";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumPaymentStatus } from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  DeleteButton,
  ExcelButton,
  EyeIconLoadingButton,
  FileUploadIconButton,
  PaidButton,
  StatusBadgeCustom,
  UnPaidButton,
  amountFormat,
  centerColumn,
  navbarHeight,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import useDeleteConfirmation from "../../../.reUseableComponents/CustomHooks/useDeleteConfirmation.js";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal.js";

function CarrierSettlementList(props) {
  const [uploadImageOpen, setDialogOpen] = useState(false);
  const [viewFileOpen, setEditDialogOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState(null);
  const [selctedRow, setSelectedRow] = useState(null);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const [excelLoading, setExcelLoading] = useState({});

  const {
    allCarrierPaymentSettlements,
    getAllCarrierPaymentSettlements,
    loading,
  } = props;

  const {
    openDelete,
    setOpenDelete,
    handleDeleteClose,
    handleSetDeletedItem,
    deletedItem,
    setLoadingConfirmation,
    loadingConfirmation,
  } = useDeleteConfirmation();
  const [markPaid, setMarkPaid] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const handleDialogOpen = (row) => {
    setSelectedRow(row);
    setDialogOpen(true);
  };
  const handleUploadImageClose = () => {
    setDialogOpen(false);
  };

  const handleEditClick = (row) => {
    setSelectedRow(row);
    setEditDialogOpen(true);
  };
  const handleViewFileOpenClose = () => {
    setEditDialogOpen(false);
  };
  const handleMarkAsPaid = async (data) => {
    try {
      var param = {
        CarrierPaymentSettlementId: data?.CarrierPaymentSettlementId,
      };

      setMarkPaid((prev) => ({
        ...prev,
        loading: { [data.CarrierPaymentSettlementId]: true },
      }));

      const response = await MarkCarrierSettlementPaid(param);
      if (response.data?.isSuccess) {
        getAllCarrierPaymentSettlements();
        successNotification(
          LanguageReducer?.languageType
            ?.ACCOUNTS_CARRIER_SETTLEMENT_PAID_SUCCESSFULLY
        );
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
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
        loading: { [data.CarrierPaymentSettlementId]: false },
      }));
    }
  };
  const handleMarkAsUnPaid = async (data) => {
    try {
      var param = {
        CarrierPaymentSettlementId: data?.CarrierPaymentSettlementId,
      };
      setMarkPaid((prev) => ({
        ...prev,
        loading: { [data.CarrierPaymentSettlementId]: true },
      }));

      const response = await MarkCarrierSettlementUnPaid(param);
      if (response.data?.isSuccess) {
        getAllCarrierPaymentSettlements();
        successNotification(
          LanguageReducer?.languageType
            ?.ACCOUNTS_CARRIER_SETTLEMENT_UNPAID_SUCCESSFULLY
        );
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
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
        loading: { [data.CarrierPaymentSettlementId]: false },
      }));
    }
  };
  const handleDeleteCarrierSettlement = async () => {
    try {
      let param = {
        CarrierPaymentSettlementId: deletedItem?.CarrierPaymentSettlementId,
      };
      setLoadingConfirmation(true);
      const response = await DeleteCarrierPaymentSettlement(param);
      if (!response.data?.isSuccess) {
        UtilityClass.showErrorNotificationWithDictionary(response.data?.errors);
      } else {
        getAllCarrierPaymentSettlements();
        successNotification(
          LanguageReducer?.languageType
            ?.ACCOUNTS_CARRIER_SETTLEMENT_CARRIER_SETTLEMENT_DELETED_SUCCESSFULLY
        );
        handleDeleteClose();
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
      setLoadingConfirmation(false);
    }
  };

  const downloadExcel = (id) => {
    setExcelLoading((prev) => ({ ...prev, [id]: true }));

    ExcelExportCarrierSettlementById(id)
      .then((res) => {
        UtilityClass.downloadExcel(res.data, "carrierSettlement");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      })
      .finally(() => {
        setAnchorEl(null);
        setExcelLoading((prev) => ({ ...prev, [id]: false }));
      });
  };

  const columns = [
    {
      field: "name",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACCOUNTS_CARRIER_SETTLEMENT_NAME}
        </Box>
      ),
      minWidth: 160,
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
                src={params.row.CarrierImage}
                sx={{
                  width: 25,
                  height: 25,
                  fontSize: "13px",
                  color: "var(--primary-color)",
                  background: "rgba(86, 58, 213, 0.3)",
                }}
              >
                {params.row.CarrierName?.slice(0, 1)}
              </Avatar>
              <Box>{params.row.CarrierName || ""}</Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      field: "PaymentRef",
      minWidth: 160,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_PAYMENT_REF
          }
        </Box>
      ),
    },
    {
      field: "CarrierWebsite",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_CARRIER_WEBSITE
          }
        </Box>
      ),
      minWidth: 200,
      flex: 1,
      valueGetter: (params) => `${params.row.CarrierWebsite || ""} `,
    },
    {
      ...centerColumn,
      field: "PaymentDate",
      minWidth: 150,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_PAYMENT_DATE
          }
        </Box>
      ),
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.PaymentDate)}
        </Box>
      ),
    },
    {
      ...centerColumn,
      field: "PaymentStatusName",
      minWidth: 140,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_PAYMENT_STATUS
          }
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <>
            <StatusBadgeCustom
              isSuccess={row.PaymentStatusId != EnumPaymentStatus.Unpaid}
              title={row.PaymentStatusName}
            />
          </>
        );
      },
    },

    {
      ...rightColumn,
      field: "Amount",
      minWidth: 70,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACCOUNTS_CARRIER_SETTLEMENT_AMOUNT}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{amountFormat(row.Amount)}</Box>;
      },
    },
    {
      ...rightColumn,
      field: "AmountReceived",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_RECEIVE_AMOUNT
          }
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{amountFormat(row.AmountReceived)}</Box>;
      },
    },

    {
      ...centerColumn,
      field: "Action",
      minWidth: 180,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ACCOUNTS_CARRIER_SETTLEMENT_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Stack direction={"row"} spacing={0.5}>
            <ExcelButton
              onClick={() => downloadExcel(row?.CarrierPaymentSettlementId)}
              loading={excelLoading[row?.CarrierPaymentSettlementId] || false}
            />
            {row.PaymentStatusId == EnumPaymentStatus.Unpaid ? (
              <Box>
                <PaidButton
                  loading={markPaid.loading[row.CarrierPaymentSettlementId]}
                  onClick={() => handleMarkAsPaid(row)}
                />
              </Box>
            ) : (
              <Box>
                <UnPaidButton
                  background={Colors.danger}
                  loading={markPaid.loading[row?.CarrierPaymentSettlementId]}
                  onClick={() => handleMarkAsUnPaid(row)}
                />
              </Box>
            )}
            <DeleteButton onClick={() => handleSetDeletedItem(row)} />
            <FileUploadIconButton onClick={() => handleEditClick(row)} />
            <EyeIconLoadingButton onClick={() => handleDialogOpen(row)} />
          </Stack>
        );
      },
    },
  ];
  const calculatedHeight = windowHeight - navbarHeight - 21.5;
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
        getRowId={(row) => row.CarrierPaymentSettlementId}
        rows={allCarrierPaymentSettlements}
        columns={columns}
        checkboxSelection={false}
        disableSelectionOnClick
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
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
              onClick={() => {
                setAnchorEl(null);
              }}
              disablePadding
            >
              <ListItemButton>
                <ListItemIcon sx={{ minWidth: "30px" }}>
                  <DeleteOutlineIcon />
                </ListItemIcon>
                <ListItemText primary="Delete Store" />
              </ListItemButton>
            </ListItem>
          </List>
        </Box>
      </Menu>
      {uploadImageOpen && (
        <UploadCarrierSettlementFileModal
          dialogOpen={uploadImageOpen}
          handleDialogClose={handleUploadImageClose}
          selctedRow={selctedRow}
        />
      )}
      {viewFileOpen && (
        <CarrierSettlementPOPViewImagesModal
          editdialogOpen={viewFileOpen}
          handleEditDialogClose={handleViewFileOpenClose}
          selctedRow={selctedRow}
          // CarrierPaymentSettlementId={}
        />
      )}
      {openDelete && (
        <DeleteConfirmationModal
          open={openDelete}
          setOpen={setOpenDelete}
          handleDelete={handleDeleteCarrierSettlement}
          loading={loadingConfirmation}
          heading={
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_ARE_YOU_SURE_YOU_WANT_TO_DELETE_CARRIER_SETTLEMENT
          }
          message={
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_RECORD_WILL_BE_DELETED_IMMEDIATELY_CANNOT_UNDO
          }
          buttonText={
            LanguageReducer?.languageType?.ACCOUNTS_CARRIER_SETTLEMENT_DELETE
          }
        />
      )}
    </Box>
  );
}
export default CarrierSettlementList;
