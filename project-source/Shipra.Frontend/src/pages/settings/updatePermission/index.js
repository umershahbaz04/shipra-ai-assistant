import { usePagination } from "@mui/lab";
import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import {
  GetAllPermissionActionForSelection,
  GetAllPermissionGroupLookup,
  UpdateControllerAndActionsRecord,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import CreatePermissionGroup from "../../../components/modals/PermissionModals/CreatePermissionGroup";
import UpdatePermissionAction from "../../../components/modals/PermissionModals/UpdatePermissionAction";
import StatusBadge from "../../../components/shared/statudBadge";
import { EnumRoutesUrls } from "../../../utilities/enum";
import {
  ActionButtonCustom,
  centerColumn,
  useGetWindowHeight,
} from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import { successNotification } from "../../../utilities/toast";

const UpdatePermission = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { height: windowHeight } = useGetWindowHeight();
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const calculatedHeight = windowHeight - 130;
  const [loading, setLoading] = useState(false);
  const [allPermissionAction, setAllPermissionAction] = useState();
  const [allPermissionGroupLookup, setAllPermissionGroupLookup] = useState([]);
  const [openPermissionActionModel, setOpenPermissionActionModel] =
    useState(false);
  const [openCreatePermissionGroupModel, setOpenCreatePermissionGroupModel] =
    useState(false);

  const getAllPermissionActionForSelection = async () => {
    setLoading(true);
    try {
      const response = await GetAllPermissionActionForSelection();
      if (response?.data?.isSuccess) {
        setAllPermissionAction(response?.data?.result);
      }
    } catch (e) {
    } finally {
      setLoading(false);
    }
  };

  const getAllPermissionGroupLookup = async () => {
    try {
      const response = await GetAllPermissionGroupLookup();
      if (response?.data?.isSuccess) {
        setAllPermissionGroupLookup(response?.data?.result);
      }
    } catch (e) {}
  };
  const handleUpdatePermission = async () => {
    try {
      const response = await UpdateControllerAndActionsRecord();
      if (response?.data?.isSuccess) {
        successNotification("Permission Update Successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {}
  };

  const columns = [
    {
      field: "controllerName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Controller Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.controllerName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "actionName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Action Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.actionName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "active",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Active"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
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
                title={row.active ? "Active" : "InActive"}
                color={row.active === true ? "#fff;" : "#fff;"}
                bgColor={row.active === true ? "#28a745;" : "#dc3545;"}
              />
            </>
          </Box>
        );
      },
    },
  ];

  useEffect(() => {
    getAllPermissionActionForSelection();
    getAllPermissionGroupLookup();
  }, []);

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <DataGridTabs
          tabsSmWidth="100px"
          tabsMdWidth="100px"
          otherBtns={
            <Box display={"flex"} gap={1}>
              <ActionButtonCustom
                label={"Create Permission Group"}
                onClick={() => setOpenCreatePermissionGroupModel(true)}
              />
              <ActionButtonCustom
                label={"Update Permission Action"}
                onClick={() => setOpenPermissionActionModel(true)}
              />
              <ActionButtonCustom
                label={"Update Permission"}
                onClick={handleUpdatePermission}
              />
            </Box>
          }
          tabData={[
            {
              label: "All",
              route: EnumRoutesUrls.UPDATE_PERRMISSION,
              children: (
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
                      fontFamily:
                        "'Lato Regular', 'Inter Regular', 'Arial' !important",
                      fontSize: "12px",
                      fontWeight: "500",
                    }}
                    getRowId={(row) => row.permissionActionId}
                    rows={allPermissionAction || []}
                    columns={columns}
                    disableSelectionOnClick
                    pagination
                    page={currentPage}
                    pageSize={pageSize}
                    rowsPerPageOptions={[10, 25, 50, 100]}
                    paginationMode="client"
                    onPageChange={handlePageChange}
                    onPageSizeChange={handlePageSizeChange}
                  />
                </Box>
              ),
            },
          ]}
        />
      </div>
      {openPermissionActionModel && (
        <UpdatePermissionAction
          open={openPermissionActionModel}
          allPermissionAction={allPermissionAction}
          allPermissionGroupLookup={allPermissionGroupLookup}
          onClose={() => setOpenPermissionActionModel(false)}
        />
      )}
      {openCreatePermissionGroupModel && (
        <CreatePermissionGroup
          open={openCreatePermissionGroupModel}
          onClose={() => setOpenCreatePermissionGroupModel(false)}
        />
      )}
    </Box>
  );
};

export default UpdatePermission;
