import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  DeleteReturnCommand,
  GetClientReturnReasonId,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UpdateReasonModal from "../../../../components/modals/settingsModals/UpdateReasonModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import {
  ActionButtonEdit,
  centerColumn,
  DisableButton,
  EnableButton,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination
} from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
const ReturnReasonList = (props) => {
  const { allReturnReason, loading, getAllClientReturnReason, isFilterOpen } =
    props;
  const [openDelete, setOpenDelete] = useState(false);
  const [loadingStates, setLoadingStates] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [clientReturnReason, setClientReturnReason] = useState({
    data: [],
    loading: {},
  });
  const [updateOpenReason, setUpdateOpenReason] = useState({
    open: false,
    loading: {},
  });
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;
  const handleDeleteConfirmation = (data) => {
    setOpenDelete(true);
    setDeleteItemObject(data);
  };
  const handleDelete = async (data) => {
    try {
      let param = {
        isActive: false,
        clientReturnReasonId: deleteItemObject.ClientReturnReasonId,
      };
      setLoadingStates(true);
      const response = await DeleteReturnCommand(param);
      if (response.data?.isSuccess) {
        getAllClientReturnReason();
        successNotification("deactivated successfully");
      } else {
        errorNotification("deactivated unsuccessfull");
      }
    } catch (error) {
      if (error?.response.data.errors) {
        errorNotification(error?.response?.data?.errors);
      }
      console.error("Something went wrong", error.response);
    } finally {
      setLoadingStates(false);
      setOpenDelete(false);
    }
  };
  const handleEnableReason = async (data) => {
    try {
      let param = {
        isActive: true,
        clientReturnReasonId: data.ClientReturnReasonId,
      };
      setIsLoading((prev) => ({
        ...prev,
        [data.ClientReturnReasonId]: true,
      }));
      const response = await DeleteReturnCommand(param);
      if (response.data?.isSuccess) {
        getAllClientReturnReason();
        successNotification("activated successfully");
      } else {
        errorNotification("activated unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      if (error?.response.data.errors) {
        errorNotification(error?.response?.data?.errors);
      }
    } finally {
      setIsLoading((prev) => ({
        ...prev,
        [data.ClientReturnReasonId]: false,
      }));
    }
  };
  const handleEditReason = async (data) => {
    try {
      setClientReturnReason((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data.ClientReturnReasonId]: true },
      }));

      const response = await GetClientReturnReasonId(data.ClientReturnReasonId);
      if (response?.data?.isSuccess) {
        const result = response?.data.result;
        setClientReturnReason((prev) => ({
          ...prev,
          data: result,
          loading: { ...prev.loading, [data.ClientReturnReasonId]: false },
        }));
        setUpdateOpenReason({ open: true });
      } else {
        setClientReturnReason((prev) => ({
          ...prev,
          loading: { ...prev.loading, [data.ClientReturnReasonId]: false },
        }));
      }
    } catch (error) {
      console.error("Error fetching client return reason:", error);
      setClientReturnReason((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data.ClientReturnReasonId]: false },
      }));
    }
  };
  const columns = [
    {
      field: "createdby",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTING_RETURN_REASON_CREATED_BY}
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return <Box>{params.row.CreatedBy}</Box>;
      },
    },
    {
      field: "reason",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTING_RETURN_REASON_REASON}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return <Box>{params.row.Reason}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "reasondetail",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_RETURN_REASON_REASON_DETAIL}
        </Box>
      ),
      renderCell: (params) => {
        return <Box>{params.row.ReasonDetail}</Box>;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "status",
      minWidth: 100,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_RETURN_REASON_STATUS}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box>
            <StatusBadge
              title={
                params.row.Active
                  ? LanguageReducer?.languageType?.SETTING_RETURN_REASON_ACTIVE
                  : LanguageReducer?.languageType
                      ?.SETTING_RETURN_REASON_IN_ACTIVE
              }
              color="#fff"
              bgColor={params.row.Active ? "#28a745" : "#dc3545"}
            />
          </Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_RETURN_REASON_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box display={"flex"} gap={1}>
            {row?.Active ? (
              <DisableButton onClick={() => handleDeleteConfirmation(row)} />
            ) : (
              <EnableButton
                loading={isLoading[row.ClientReturnReasonId]}
                onClick={() => handleEnableReason(row)}
              />
            )}
            <ActionButtonEdit
              onClick={() => handleEditReason(row)}
              loading={clientReturnReason.loading[row.ClientReturnReasonId]}
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
          getRowId={(row) => row.RowNum}
          rows={allReturnReason}
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
      {updateOpenReason && (
        <UpdateReasonModal
          open={updateOpenReason.open}
          clientReturnReason={clientReturnReason}
          getAllClientReturnReason={getAllClientReturnReason}
          onClose={() =>
            setUpdateOpenReason((prev) => ({ ...prev, open: false }))
          }
        />
      )}
      <DeleteConfirmationModal
        open={openDelete}
        setOpen={setOpenDelete}
        loading={loadingStates}
        handleDelete={handleDelete}
        heading={
          LanguageReducer?.languageType
            ?.SETTING_RETURN_REASON_ARE_YOU_SURE_TO_DISABLE_THIS_ITEM
        }
        message={
          LanguageReducer?.languageType
            ?.SETTING_RETURN_REASON_AFTER_THIS_ACTION_THE_ITEM_WILL_BE_DISABLED_IMMEDIATELY_BUT_YOU_CAN_UNDO_THIS_ACTION_ANYTIME
        }
        buttonText={
          LanguageReducer?.languageType?.SETTING_RETURN_REASON_DISABLE
        }
      />
    </>
  );
};

export default ReturnReasonList;
