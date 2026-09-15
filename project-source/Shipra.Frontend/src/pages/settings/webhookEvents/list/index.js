import DeleteIcon from "@mui/icons-material/Delete";
import { Box, Button, Tooltip } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  DeleteWebhookEvent,
  GetClientWebhookEventById,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UpdateWebhookUrlModal from "../../../../components/modals/settingsModals/UpdateWebhookUrlModal";
import {
  ActionButtonEdit,
  centerColumn,
  navbarHeight,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";
const WebhookEventList = (props) => {
  const {
    loading,
    allClientWebhookEvents,
    getAllClientWebhookEvents,
    isFilterOpen,
  } = props;
  const [openDelete, setOpenDelete] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [loadingStates, setLoadingStates] = useState(false);
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 152.5
    : windowHeight - 95 - 36;
  const handleDeleteConfirmation = (data) => {
    setOpenDelete(true);
    setDeleteItemObject(data);
  };
  const [webhookUrl, setWebhookUrl] = useState({});
  const [webhookLoading, setWebhookLoading] = useState({});
  const [updateOpenWebhookUrlModal, setUpdateOpenWebhookUrlModal] = useState({
    open: false,
    loading: {},
  });
  const handleDelete = async () => {
    try {
      setLoadingStates(true);
      const response = await DeleteWebhookEvent(deleteItemObject);
      if (response.data?.isSuccess) {
        successNotification("Delete successfully");
        getAllClientWebhookEvents();
      } else {
        errorNotification("Delete unsuccessfull");
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

  const handleEditWebhook = async (data) => {
    setWebhookLoading((prevState) => ({
      ...prevState,
      [data.ClientWebhookEventId]: true,
    }));

    try {
      const response = await GetClientWebhookEventById(
        data.ClientWebhookEventId
      );

      if (response?.data?.isSuccess) {
        const result = response?.data.result;
        setWebhookUrl(result);
        setUpdateOpenWebhookUrlModal({ open: true });
      } else {
        errorNotification("Error fetching Webhook Url");
      }
    } catch (error) {
      console.error("Error fetching Webhook Url:", error);
      errorNotification("Failed to fetch Webhook Url");
    } finally {
      setWebhookLoading((prevState) => ({
        ...prevState,
        [data.ClientWebhookEventId]: false,
      }));
    }
  };

  const columns = [
    {
      field: "WebhookUrl",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTING_WEBHOOK_WEBHOOK_URL}
        </Box>
      ),
      minWidth: 250,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Tooltip title={row.WebhookUrl} arrow>
            <Box
              sx={{
                whiteSpace: "nowrap",
                overflow: "hidden",
                textOverflow: "ellipsis",
                fontWeight: "bold",
                maxWidth: 250,
              }}
            >
              {row.WebhookUrl}
            </Box>
          </Tooltip>
        );
      },
    },
    {
      field: "eventName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTING_WEBHOOK_EVENT_NAME}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{row.EventName}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_WEBHOOK_CREATED_ON}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row.CreatedOn)}</Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "action",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_WEBHOOK_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box display={"flex"} gap={1} alignItems={"center"}>
            <Box>
              <Button
                sx={styleSheet.deleteProductButton}
                variant="outlined"
                onClick={() =>
                  handleDeleteConfirmation(row.ClientWebhookEventId)
                }
              >
                <DeleteIcon />
              </Button>
            </Box>
            <Box>
              <ActionButtonEdit
                onClick={() => handleEditWebhook(row)}
                loading={webhookLoading[row.ClientWebhookEventId] || false}
              />
            </Box>
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
          rows={allClientWebhookEvents || []}
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
      <DeleteConfirmationModal
        open={openDelete}
        setOpen={setOpenDelete}
        loading={loadingStates}
        handleDelete={handleDelete}
        heading={
          LanguageReducer?.languageType
            ?.SETTING_WEBHOOK_ARE_YOU_SURE_YOU_WANT_TO_DELETE_THIS_ITEM
        }
        message={
          LanguageReducer?.languageType
            ?.SETTING_WEBHOOK_AFTER_THIS_ACTION_THE_ITEM_WILL_BE_DELETED_IMMEDIATELY_AND_YOU_CANT_UNDO_THIS_ACTION
        }
        buttonText={LanguageReducer?.languageType?.SETTING_WEBHOOK_DELETE}
      />
      {updateOpenWebhookUrlModal.open && (
        <UpdateWebhookUrlModal
          open={updateOpenWebhookUrlModal.open}
          getAllClientWebhookEvents={getAllClientWebhookEvents}
          webhookUrl={webhookUrl}
          onClose={() =>
            setUpdateOpenWebhookUrlModal((prev) => ({ ...prev, open: false }))
          }
        />
      )}
    </>
  );
};

export default WebhookEventList;
