import { Box, Grid } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  DeleteActiveCarrierById,
  DeleteDriverCtssetting,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UtilityClass from "../../../../utilities/UtilityClass";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import { DataGrid } from "@mui/x-data-grid";
import {
  ActionButtonDelete,
  centerColumn,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";

function DriverStatusList(props) {
  let { allDriverStatusData, loading, getAllDriverStatus } = props;
  const [modalOpen, setModalOpen] = useState(false);
  const [loadingStates, setLoadingStates] = useState(false);

  const [openDeleteStore, setOpenDeleteStore] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const handleDeleteConfirmation = (data) => {
    setDeleteItemObject(data);
    setOpenDeleteStore(true);
  };
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const handleDelete = async () => {
    setLoadingStates(true);
    try {
      let param = {
        driverCtsid: deleteItemObject.DriverCTSId,
      };
      const response = await DeleteDriverCtssetting(param);
      if (response.data?.isSuccess) {
        getAllDriverStatus();
        successNotification("Driver Status deleted successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(response.data?.errors);
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      errorNotification("Something went wrong");
    } finally {
      setLoadingStates(false);
      setIsDeletedConfirm(false);
      setOpenDeleteStore(false);
      setDeleteItemObject();
    }
  };
  // handleCopyToClipBoard
  const columns = [
    {
      field: "TrackingStatus",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTING_DRIVER_STATUSES_STATUS_NAME}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
    },
    {
      field: "TrackingStatusAr",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {
            LanguageReducer?.languageType
              ?.SETTING_DRIVER_STATUSES_STATUS_NAME_AR
          }
        </Box>
      ),
      minWidth: 90,
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_DRIVER_STATUSES_ACTION}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box>
            <ActionButtonDelete
              label=""
              onClick={(e) => handleDeleteConfirmation(params?.row)}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];

  useEffect(() => {
    if (isDeletedConfirm) {
      handleDelete();
    }
  }, [isDeletedConfirm]);
  const calculatedHeight = windowHeight - navbarHeight - 70;
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
        getRowId={(row) => row?.DriverCTSId}
        rows={allDriverStatusData?.list ? allDriverStatusData.list : []}
        columns={columns}
        disableSelectionOnClick
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        // checkboxSelection
      />

      {openDeleteStore && (
        <DeleteConfirmationModal
          open={openDeleteStore}
          setOpen={setOpenDeleteStore}
          setIsDeletedConfirm={setIsDeletedConfirm}
          loading={loadingStates}
        />
      )}
    </Box>
  );
}
export default DriverStatusList;
