import {
  Box,
  CircularProgress,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { GetProductStationById } from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UpdateProductStationModal from "../../../../components/modals/productStationModals/UpdateProductStationModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import UtilityClass from "../../../../utilities/UtilityClass";
import {
  CodeBox,
  centerColumn,
  purple,
  usePagination,
  ClipboardIcon,
  useGetWindowHeight,
  navbarHeight,
  useClientSubscriptionReducer,
} from "../../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../../utilities/toast";

function ProductStationList(props) {
  const { rows, getOrdersRef, resetRowRef, loading, getAllStations } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [open, setOpen] = useState(false);
  const [openUpdate, setOpenUpdate] = useState(false);
  const [openEditModal, setOpenEditModal] = useState(false);
  const [stationData, setStationData] = useState();
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const handleEdit = (data) => {
    if (data) {
      setInfoModal((prev) => ({
        ...prev,
        loading: { [data.ProductStationId]: true },
      }));

      GetProductStationById(data.ProductStationId)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.errors);
          } else {
            setStationData(res?.data?.result);
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
            loading: { [data.ProductStationId]: false },
          }));
        });
    }
  };

  const columns = [
    {
      field: "stationCode",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.PRODUCT_PRODUCT_STATIONS_CODE}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }} disableRipple>
            <>
              {infoModal.loading[params.row.ProductStationId] ? (
                <CircularProgress size={20} />
              ) : (
                <Box display={"flex"}>
                  <CodeBox
                    onClick={(e) => {
                      handleEdit(params.row);
                    }}
                    title={params.row.StationCode}
                  />
                  <ClipboardIcon text={params.row.StationCode} />
                </Box>
              )}
            </>
          </Box>
        );
      },
    },
    {
      field: "Name",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.PRODUCT_PRODUCT_STATIONS_NAME}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }} disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{params.row.Name}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "CreatedOn",
      ...centerColumn,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.PRODUCT_PRODUCT_STATIONS_CREATEDON}
        </Box>
      ),
      minWidth: 120,
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.CreatedOn)}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "Status",
      ...centerColumn,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.PRODUCT_PRODUCT_STATIONS_STATUS}
        </Box>
      ),
      minWidth: 80,
      flex: 1,
      renderCell: (params) => {
        let isActive = params.row.Active;
        let title = isActive ? "Active" : "InActive";
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            // justifyContent={"center"}
            // sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <StatusBadge
                title={title}
                color={isActive == false ? "#fff;" : "#fff;"}
                bgColor={isActive == false ? "#dc3545;" : "#28a745;"}
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
          {LanguageReducer?.languageType?.PRODUCT_PRODUCT_STATIONS_DEFAULT}
        </Box>
      ),
      minWidth: 80,
      flex: 1,
      renderCell: (params) => {
        let isDefault = params.row.IsDefault;
        let title = isDefault ? "yes" : "no";
        return (
          isDefault && (
            <Box display={"flex"} flexDirection={"column"} disableRipple>
              <>
                <StatusBadge
                  title={title}
                  color={isDefault == false ? "#fff;" : "#fff;"}
                  bgColor={purple}
                  // bgColor={isDefault && "#28a745;"}
                />
              </>
            </Box>
          )
        );
      },
    },
  ];
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
        getRowId={(row) => row?.ProductStationId}
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
            <ListItem disablePadding>
              <ListItemButton
                onClick={() => {
                  setAnchorEl(null);
                }}
              >
                <ListItemText
                  primary={LanguageReducer?.languageType?.BATCH_OUTSCAN_TEXT}
                />
              </ListItemButton>
            </ListItem>
            <ListItem
              onClick={() => {
                setOpenUpdate(true);
                setAnchorEl(null);
              }}
              disablePadding
            >
              <ListItemButton>
                <ListItemText
                  primary={LanguageReducer?.languageType?.BATCH_UPDATE_TEXT}
                />
              </ListItemButton>
            </ListItem>
            {/* <ListItem
              onClick={() => {
                setAnchorEl(null);
                navigate("/create-order");
              }}
              disablePadding
            >
              <ListItemButton>
                <ListItemText primary={LanguageReducer?.languageType?.PRINT_TEXT} />
              </ListItemButton>
            </ListItem>
            <ListItem disablePadding>
              <ListItemButton>
                <ListItemText primary={LanguageReducer?.languageType?.EDIT_TEXT} />
              </ListItemButton>
            </ListItem>
            <ListItem disablePadding>
              <ListItemButton>
                <ListItemText primary={LanguageReducer?.languageType?.DEBERIF_TEXT} />
              </ListItemButton>
            </ListItem> */}
          </List>
        </Box>
      </Menu>
      {openEditModal && (
        <UpdateProductStationModal
          open={openEditModal}
          setOpen={setOpenEditModal}
          getAll={getAllStations}
          stationData={stationData}
          setStationData={setStationData}
        />
      )}
    </Box>
  );
}
export default ProductStationList;
