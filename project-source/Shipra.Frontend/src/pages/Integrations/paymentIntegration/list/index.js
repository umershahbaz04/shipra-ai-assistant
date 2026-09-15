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
  DeletePPActivate,
  DeleteSMSActivate,
  GetPPActivateById,
  GetSaleChannelConfigById,
} from "../../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import StatusBadge from "../../../../components/shared/statudBadge";
import {
  CodeBox,
  centerColumn,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import UpdatePaymentConfigModal from "../../../../components/modals/integrationModals/UpdatePaymentConfigModal";

function PaymentIntegrationList(props) {
  const { rows, getOrdersRef, resetRowRef, loading, getAllPPActivate } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [openDeleteStore, setOpenDeleteStore] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [loadingStates, setLoadingStates] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const [openEditModal, setOpenEditModal] = useState(false);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [editData, setEditData] = useState();
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const handleEditClick = (data) => {
    if (data) {
      setInfoModal((prev) => ({
        ...prev,
        loading: { [data.PPActivateId]: true },
      }));

      GetPPActivateById(data.PPActivateId)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.errors);
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
            loading: { [data.PPActivateId]: false },
          }));
        });
    }
  };

  const columns = [
    {
      field: "PPName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {
            LanguageReducer?.languageType
              ?.INTEGRATION_PAYMENT_INTEGRATION_PAYMENT_PROCESS_NAME
          }
        </Box>
      ),
      minWidth: 140,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            {infoModal.loading[params.row.PPActivateId] ? (
              <CircularProgress size={20} />
            ) : (
              <>
                <CodeBox
                  title={params.row.PPName}
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
              ?.INTEGRATION_PAYMENT_INTEGRATION_CREATE_DATE
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
      minWidth: 90,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.INTEGRATION_PAYMENT_INTEGRATION_STATUS
          }
        </Box>
      ),
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
      minWidth: 90,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.INTEGRATION_PAYMENT_INTEGRATION_DEFAULT
          }
        </Box>
      ),
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
      minWidth: 100,
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
        ppactivateId: deleteItemObject.PPActivateId,
      };
      const response = await DeletePPActivate(param);
      if (response.data?.isSuccess) {
        getAllPPActivate();
        successNotification("payment process deleted successfully");
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
        getRowId={(row) => row?.PPActivateId}
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
                <ListItemText primary="Delete" />
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
        <UpdatePaymentConfigModal
          open={openEditModal}
          setOpen={setOpenEditModal}
          getAll={getAllPPActivate}
          data={editData}
        />
      )}
    </Box>
  );
}
export default PaymentIntegrationList;
