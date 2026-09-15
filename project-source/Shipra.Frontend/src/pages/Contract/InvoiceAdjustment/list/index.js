import {
  ActionButtonCustom,
  centerColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { DataGrid } from "@mui/x-data-grid";
import { Box, Button } from "@mui/material";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import { useState } from "react";
import {
  DeleteShipperInvoiceAdjustmentById,
  GetShipperInvoiceAdjustmentById,
} from "../../../../api/AxiosInterceptors";
import AddUpdateShipperInvoiceAdjustmentModal from "../../../../components/modals/ContractModals/AddUpdateShipperInvoiceAdjustmentModal";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import { successNotification } from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";

const InvoiceAdjustmentList = (props) => {
  const {
    loading,
    isFilterOpen,
    allInvoiceAdjustment,
    getAllShipperInvoiceAdjustment,
  } = props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const [invoiceAdjustmentData, setInvoiceAdjustmentData] = useState({
    open: false,
    loading: {},
    data: {},
  });
  const [openDelete, setOpenDelete] = useState(false);
  const [loadingStates, setLoadingStates] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});

  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;

  const handleEditOrderLabel = async (data) => {
    const shipperInvoiceAdjustmentId = data?.shipperInvoiceAdjustmentId;
    try {
      setInvoiceAdjustmentData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [shipperInvoiceAdjustmentId]: true },
      }));

      const response = await GetShipperInvoiceAdjustmentById(
        shipperInvoiceAdjustmentId
      );
      if (response?.data?.isSuccess) {
        const result = response?.data.result;
        setInvoiceAdjustmentData((prev) => ({
          ...prev,
          open: true,
          data: result,
        }));
      } else {
        setInvoiceAdjustmentData((prev) => ({
          ...prev,
          loading: { ...prev.loading, [shipperInvoiceAdjustmentId]: false },
        }));
      }
    } catch (error) {
      console.error("Error fetching client order label:", error);
    } finally {
      setInvoiceAdjustmentData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [shipperInvoiceAdjustmentId]: false },
      }));
    }
  };

  const handleDeleteConfirmation = (data) => {
    setOpenDelete(true);
    setDeleteItemObject(data);
  };

  const handleDeleteInvoiceAdjustment = async () => {
    try {
      setLoadingStates(true);
      const response = await DeleteShipperInvoiceAdjustmentById(
        deleteItemObject
      );
      if (response.data?.isSuccess) {
        getAllShipperInvoiceAdjustment();
        successNotification("Invoice adjustment Delete successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoadingStates(false);
      setOpenDelete(false);
    }
  };

  const columns = [
    {
      field: "name",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Transaction Type"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.name}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "amount",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Amount"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{(row?.amount).toFixed(2)}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "comment",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Comment"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.comment}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Action"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box display={"flex"} gap={1} width="100%" justifyContent={"center"}>
            <Button
              sx={styleSheet.deleteProductButton}
              variant="outlined"
              onClick={() =>
                handleDeleteConfirmation(row.shipperInvoiceAdjustmentId)
              }
              aria-label={`Delete lable ${row.LabelName}`}
            >
              <DeleteIcon />
            </Button>
            <ActionButtonCustom
              onClick={() => handleEditOrderLabel(row)}
              loading={
                invoiceAdjustmentData.loading[row.shipperInvoiceAdjustmentId]
              }
              sx={styleSheet.editProductButton}
              label={<EditIcon />}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];
  return (
    <>
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
          getRowId={(row) => row.rowNum}
          rows={allInvoiceAdjustment || []}
          columns={columns}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 15, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
        />
      </Box>
      {invoiceAdjustmentData.open && (
        <AddUpdateShipperInvoiceAdjustmentModal
          open={invoiceAdjustmentData.open}
          onClose={() =>
            setInvoiceAdjustmentData((prev) => ({
              ...prev,
              open: false,
              data: {},
            }))
          }
          rowData={invoiceAdjustmentData.data}
          getAllShipperInvoiceAdjustment={getAllShipperInvoiceAdjustment}
        />
      )}
      <DeleteConfirmationModal
        open={openDelete}
        setOpen={setOpenDelete}
        loading={loadingStates}
        handleDelete={handleDeleteInvoiceAdjustment}
        heading={"Are you sure you want to delete this invoice adjustment"}
        message={
          "The selected invoice will be permanently deleted. This action cannot be undone."
        }
        buttonText={"Delete"}
      />{" "}
    </>
  );
};

export default InvoiceAdjustmentList;
