import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Avatar,
  Box,
  CircularProgress,
  IconButton,
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
import { useNavigate } from "react-router-dom";
import { styleSheet } from "../../../../assets/styles/style";
import UtilityClass from "../../../../utilities/UtilityClass";
import DeleteOutline from '@mui/icons-material/DeleteOutline';
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  DeleteSMSActivate,
  GetSMSActivateById,
  GetSaleChannelConfigById,
} from "../../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import { useEffect } from "react";
import StatusBadge from "../../../../components/shared/statudBadge";
import {
  CodeBox,
  centerColumn,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import UpdateSmsConfigModal from "../../../../components/modals/integrationModals/UpdateSmsConfigModal";

function SmsIntegrationList(props) {
  const { rows, getOrdersRef, resetRowRef, loading, getAllSMSActivate } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [openDeleteStore, setOpenDeleteStore] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [loadingStates, setLoadingStates] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [editData, setEditData] = useState();
  const [openEditModal, setOpenEditModal] = useState(false);

  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const handleEditClick = (data) => {
    if (data) {
      setInfoModal((prev) => ({
        ...prev,
        loading: { [data.SMSActivateId]: true },
      }));

      GetSMSActivateById(data.SMSActivateId)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
          } else {
            setEditData(res?.data?.result);
            setInfoModal((prev) => ({
              ...prev,
              open: true,
              data: res?.data?.result,
            }));
            setOpenEditModal(true);
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Something went wrong");
        })
        .finally(() => {
          setInfoModal((prev) => ({
            ...prev,
            loading: { [data.SMSActivateId]: false },
          }));
        });
    }
  };

  const columns = [
    {
      field: "ServiceName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {
            LanguageReducer?.languageType
              ?.INTEGRATION_SMS_INTEGRATION_SERVICE_NAME
          }
        </Box>
      ),
      minWidth: 110,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            {infoModal.loading[params.row.SMSActivateId] ? (
              <CircularProgress size={20} />
            ) : (
              <>
                <CodeBox
                  title={params.row.ServiceName}
                  onClick={() => handleEditClick(params.row)}
                />
              </>
            )}
          </>
        );
      },
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.INTEGRATION_SMS_INTEGRATION_CREATE_DATE
          }
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            {UtilityClass.convertUtcToLocalAndGetDate(params.row.CreatedOn)}
          </Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Status",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.INTEGRATION_SMS_INTEGRATION_STATUS}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        let isActive = params.row.Active;
        let title = isActive ? "Active" : "InActive";
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <StatusBadge
                title={title}
                color={isActive == false ? "#fff;" : "#fff;"}
                bgColor={isActive === false ? "#dc3545;" : "#28a745;"}
              />
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "IsDefault",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.INTEGRATION_SMS_INTEGRATION_DEFAULT}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        let isDefault = params.row.IsDefault;
        let title = isDefault ? "yes" : "no";
        return (
          isDefault && (
            <Box
              display={"flex"}
              flexDirection={"column"}
              justifyContent={"center"}
              sx={{ textAlign: "center" }}
              disableRipple
            >
              <>
                <StatusBadge
                  title={title}
                  color={isDefault == false ? "#fff;" : "#fff;"}
                  bgColor={isDefault && "#28a745;"}
                />
              </>
            </Box>
          )
        );
      },
    },
    {
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACTION}
        </Box>
      ),
      hide: true,
      renderCell: (params) => {
        return (
          <Box>
            <IconButton
              onClick={(e) => handleActionButton(e.currentTarget, params.row)}
            >
              {/* onClick={(e) => setAnchorEl(e.currentTarget)} */}
              <MoreVertIcon />
            </IconButton>
          </Box>
        );
      },
      flex: 1,
    },
  ];
  const handleActionButton = (cTarget, data) => {
    setAnchorEl(cTarget);
    setDeleteItemObject(data);
  };
  const handleDeleteConfirmation = () => {
    setOpenDeleteStore(true);
  };
  const handleDelete = async () => {
    setLoadingStates(true);
    try {
      let param = {
        SMSActivateId: deleteItemObject.SMSActivateId,
      };
      const response = await DeleteSMSActivate(param);
      if (response.data?.isSuccess) {
        getAllSMSActivate();
        successNotification("service deleted successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(response.data?.errors);
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
      setLoadingStates(false);
      setIsDeletedConfirm(false);
      setOpenDeleteStore(false);
      setDeleteItemObject({});
    }
  };
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  const calculatedHeight = windowHeight - navbarHeight - 65;

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
        getRowId={(row) => row?.SMSActivateId}
        rows={rows?.list ? rows.list : []}
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
        <Box sx={{ width: "180px" }}>
          <List disablePadding>
            <ListItem
              onClick={() => {
                handleDeleteConfirmation();
                setAnchorEl(null);
              }}
              disablePadding
            >
              <ListItemButton>
                <ListItemIcon sx={{ minWidth: "30px" }}>
                  <DeleteOutline />
                </ListItemIcon>
                <ListItemText primary="Delete Service" />
              </ListItemButton>
            </ListItem>
          </List>
        </Box>
      </Menu>
      {openDeleteStore && (
        <DeleteConfirmationModal
          open={openDeleteStore}
          setOpen={setOpenDeleteStore}
          setIsDeletedConfirm={setIsDeletedConfirm}
          loading={loadingStates}
          handleDelete={handleDelete}
          {...props}
        />
      )}
      {openEditModal && (
        <UpdateSmsConfigModal
          open={openEditModal}
          setOpen={setOpenEditModal}
          getAll={getAllSMSActivate}
          data={editData}
        />
      )}
    </Box>
  );
}
export default SmsIntegrationList;
