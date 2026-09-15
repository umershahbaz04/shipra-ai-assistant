import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  EnableDisableClientOrderBox,
  GetClientOrderBoxById,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UpdateOrderBoxModal from "../../../../components/modals/settingsModals/UpdateOrderBoxModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import {
  ActionButtonEdit,
  centerColumn,
  DisableButton,
  EnableButton,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
const OrderBoxList = (props) => {
  const { loading, getAllClientOrderBox, isFilterOpen, allClientOrderBox } =
    props;
  const [openDelete, setOpenDelete] = useState(false);
  const [loadingStates, setLoadingStates] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [orderBoxData, setOrderBoxData] = useState({
    data: [],
    loading: {},
  });
  const [updateOpenOrderBox, setUpdateOpenOrderBox] = useState({
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
  const handleDisable = async (data) => {
    try {
      let param = {
        isActive: false,
        clientOrderBoxId: deleteItemObject.clientOrderBoxId,
      };
      setLoadingStates(true);
      const response = await EnableDisableClientOrderBox(param);
      console.log(response);
      if (response.data?.isSuccess) {
        getAllClientOrderBox();
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
        clientOrderBoxId: data.clientOrderBoxId,
      };
      setIsLoading((prev) => ({
        ...prev,
        [data.clientOrderBoxId]: true,
      }));
      const response = await EnableDisableClientOrderBox(param);
      if (response.data?.isSuccess) {
        getAllClientOrderBox();
        successNotification("activate successfully");
      } else {
        errorNotification("activate unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      if (error?.response.data.errors) {
        errorNotification(error?.response?.data?.errors);
      }
    } finally {
      setIsLoading((prev) => ({
        ...prev,
        [data.clientOrderBoxId]: false,
      }));
    }
  };
  const handleEditOrderBox = async (data) => {
    try {
      setOrderBoxData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data.clientOrderBoxId]: true },
      }));

      const response = await GetClientOrderBoxById(data.clientOrderBoxId);
      if (response?.data?.isSuccess) {
        const result = response?.data.result;
        setOrderBoxData((prev) => ({
          ...prev,
          data: result,
          loading: { ...prev.loading, [data.clientOrderBoxId]: false },
        }));
        setUpdateOpenOrderBox({ open: true });
      } else {
        setOrderBoxData((prev) => ({
          ...prev,
          loading: { ...prev.loading, [data.clientOrderBoxId]: false },
        }));
      }
    } catch (error) {
      console.error("Error fetching client return reason:", error);
      setOrderBoxData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data.clientOrderBoxId]: false },
      }));
    }
  };
  const columns = [
    {
      field: "boxName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTING_ORDER_BOX_BOX_NAME}
        </Box>
      ),
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.boxName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "length",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTING_ORDER_BOX_LENGTH}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{row.length}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "width",
      minWidth: 90,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_ORDER_BOX_WIDTH}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row.width}</Box>;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "height",
      minWidth: 90,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_ORDER_BOX_HEIGHT}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row.height}</Box>;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Status",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_ORDER_BOX_STATUS}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>
            <StatusBadge
              title={row.active ? "Active" : "Inactive"}
              color="#fff"
              bgColor={row.active ? "#28a745" : "#dc3545"}
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
          {LanguageReducer?.languageType?.SETTING_ORDER_BOX_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        const showOtherButtons = !row.isDefault;

        return (
          <Box
            display={"flex"}
            gap={1}
            justifyContent={showOtherButtons ? "flex-end" : "flex-end"}
            width="100%"
            sx={{ mr: { md: 7, sm: 3, xs: 3 } }}
          >
            {showOtherButtons &&
              (row?.active ? (
                <DisableButton onClick={() => handleDeleteConfirmation(row)} />
              ) : (
                <EnableButton
                  loading={isLoading[row.clientOrderBoxId]}
                  onClick={() => handleEnableReason(row)}
                />
              ))}
            <ActionButtonEdit
              onClick={() => handleEditOrderBox(row)}
              loading={orderBoxData.loading[row.clientOrderBoxId]}
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
          getRowId={(row) => row.clientOrderBoxId}
          rows={allClientOrderBox}
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
      {updateOpenOrderBox.open && (
        <UpdateOrderBoxModal
          open={updateOpenOrderBox.open}
          orderBoxData={orderBoxData}
          getAllClientOrderBox={getAllClientOrderBox}
          onClose={() =>
            setUpdateOpenOrderBox((prev) => ({ ...prev, open: false }))
          }
        />
      )}
      <DeleteConfirmationModal
        open={openDelete}
        setOpen={setOpenDelete}
        loading={loadingStates}
        handleDelete={handleDisable}
        heading={
          LanguageReducer?.languageType
            ?.SETTING_ORDER_BOX_ARE_YOU_SURE_TO_DISABLE_THIS_ITEM
        }
        message={
          LanguageReducer?.languageType
            ?.SETTING_ORDER_BOX_AFTER_THIS_ACTION_THE_ITEM_WILL_BE_DISABLED_IMMEDIATELY_BUT_YOU_CAN_UNDO_THIS_ACTION_ANYTIME
        }
        buttonText={LanguageReducer?.languageType?.SETTING_ORDER_BOX_DISABLE}
      />
    </>
  );
};

export default OrderBoxList;
