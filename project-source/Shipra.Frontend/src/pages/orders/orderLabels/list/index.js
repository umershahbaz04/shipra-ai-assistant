import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { Box, Button } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  DeleteClientOrderLabelLookup,
  GetClientOrderLabelLookupId,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import CreateOrderLabelsModal from "../../../../components/modals/orderModals/CreateOrderLabelsModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import {
  ActionButtonCustom,
  centerColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import { successNotification } from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";

const OrderLabelsList = (props) => {
  const {
    loading,
    isFilterOpen,
    allClientOrderLabels,
    getAllClientOrderLabelLookup,
  } = props;
  const [openDelete, setOpenDelete] = useState(false);
  const [loadingStates, setLoadingStates] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [orderLabelData, setOrderLabelData] = useState({
    data: [],
    loading: {},
  });
  const [updateOpenOrderLabel, setUpdateOpenOrderLabel] = useState(false);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;
  const handleDeleteConfirmation = (data) => {
    setOpenDelete(true);
    setDeleteItemObject(data);
  };
  const handleEditOrderLabel = async (data) => {
    const clientOrderLabelLookupId = data?.ClientOrderLabelLookupId;
    try {
      setOrderLabelData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [clientOrderLabelLookupId]: true },
      }));

      const response = await GetClientOrderLabelLookupId(
        clientOrderLabelLookupId
      );
      if (response?.data?.isSuccess) {
        const result = response?.data.result;
        setOrderLabelData((prev) => ({
          ...prev,
          data: result,
          loading: { ...prev.loading, [clientOrderLabelLookupId]: false },
        }));
        setUpdateOpenOrderLabel(true);
      } else {
        setOrderLabelData((prev) => ({
          ...prev,
          loading: { ...prev.loading, [clientOrderLabelLookupId]: false },
        }));
      }
    } catch (error) {
      console.error("Error fetching client order label:", error);
    } finally {
      setOrderLabelData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [clientOrderLabelLookupId]: false },
      }));
    }
  };

  const handleDeleteOrderLabel = async () => {
    try {
      setLoadingStates(true);
      const response = await DeleteClientOrderLabelLookup(deleteItemObject);
      console.log(response);
      if (response.data?.isSuccess) {
        getAllClientOrderLabelLookup();
        successNotification("Labels Delete successfully");
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
      field: "Orderlabel",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Order Labels"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.LabelName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Color",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Color"}</Box>,
      minWidth: 90,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box display="flex" alignItems="center" gap={1}>
            <Box
              sx={{
                width: 20,
                height: 20,
                borderRadius: "50%",
                backgroundColor: row.ColorCode,
                border: "1px solid #ccc",
              }}
            />
            <Box>{row.ColorCode}</Box>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Status",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Status"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box>
            <StatusBadge
              title={row.Active ? "Active" : "Inactive"}
              color="#fff"
              bgColor={row.Active ? "#28a745" : "#dc3545"}
            />
          </Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK_CREATED_ON}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row?.CreatedOn)}</Box>
        );
      },
      flex: 1,
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
                handleDeleteConfirmation(row.ClientOrderLabelLookupId)
              }
              aria-label={`Delete lable ${row.LabelName}`}
            >
              <DeleteIcon />
            </Button>
            <ActionButtonCustom
              onClick={() => handleEditOrderLabel(row)}
              loading={orderLabelData.loading[row.ClientOrderLabelLookupId]}
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
          getRowId={(row) => row.ClientOrderLabelLookupId}
          rows={allClientOrderLabels}
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
      {updateOpenOrderLabel && (
        <CreateOrderLabelsModal
          open={updateOpenOrderLabel}
          onClose={() => setUpdateOpenOrderLabel(() => false)}
          getAllClientOrderLabelLookup={getAllClientOrderLabelLookup}
          orderLabelData={orderLabelData.data}
        />
      )}
      <DeleteConfirmationModal
        open={openDelete}
        setOpen={setOpenDelete}
        loading={loadingStates}
        handleDelete={handleDeleteOrderLabel}
        heading={"Are you sure you want to delete this order label"}
        message={
          "The selected order label will be permanently deleted. This action cannot be undone."
        }
        buttonText={"Delete"}
      />{" "}
    </>
  );
};

export default OrderLabelsList;
