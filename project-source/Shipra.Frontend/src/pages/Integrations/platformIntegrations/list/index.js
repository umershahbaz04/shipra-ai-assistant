import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Box,
  CircularProgress,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  Typography,
  Pagination,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  GetAllSaleChannelLookupForSelection,
  GetSaleChannelConfigForUpdateById,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UpdatePlatFormModal from "../../../../components/modals/integrationModals/UpdatePlatFormModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import UtilityClass from "../../../../utilities/UtilityClass";
import {
  ActionButtonCustom,
  CicrlesLoading,
  CodeBox,
  GridContainer,
  GridItem,
  navbarHeight,
  ReusableCardComponent,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../../utilities/toast";
import {
  EnumChangeFilterModelApiUrls,
  viewTypesEnum,
} from "../../../../utilities/enum";
import Colors from "../../../../utilities/helpers/Colors";
import { PaginationComponent } from "../../../../utilities/helpers/paginationSchema";

function PlatFormIntegrationList(props) {
  const { rows, GridLoading, getAllSaleChannelConfig, viewMode, currentPage, pageSize } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const [openUpdate, setOpenUpdate] = useState(false);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [editData, setEditData] = useState();
  const [openEditModal, setOpenEditModal] = useState(false);
  const [isGridLoading, setIsGridLoading] = useState();
  const [allPlatformForSelection, setAllPlatformForSelection] = useState([]);

  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });

  const gtAllSaleChannelLookupForSelection = async () => {
    setIsGridLoading(true);
    let res = await GetAllSaleChannelLookupForSelection();
    if (res.data.result != null) {
      const filterResult = res.data.result.filter((_, index) => index !== 0);
      setAllPlatformForSelection(filterResult);
    }
    setIsGridLoading(false);
  };

  const handleEditClick = (data) => {
    if (data) {
      setInfoModal((prev) => ({
        ...prev,
        loading: { [data.SaleChannelConfigId]: true },
      }));

      GetSaleChannelConfigForUpdateById(data.SaleChannelConfigId)
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
            loading: { [data.SaleChannelConfigId]: false },
          }));
        });
    }
  };
  const columns = [
    {
      field: "SaleChannelName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {
            LanguageReducer?.languageType
              ?.INTEGRATION_SALE_CHANNEL_SALE_CHANNEL_NAME
          }
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            {infoModal.loading[params.row.SaleChannelConfigId] ? (
              <CircularProgress size={20} />
            ) : (
              <>
                <CodeBox
                  title={params.row.SaleChannelName}
                  onClick={() => handleEditClick(params.row)}
                />
              </>
            )}
          </>
        );
      },
    },
    {
      field: "StoreName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_STORE_NAME}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return <>{params.row.StoreName}</>;
      },
    },
    {
      field: "SaleChannelKey",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_DISPLAY_NAME}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return <>{params.row.SaleChannelKey}</>;
      },
    },

    {
      field: "CreatedOn",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_CREATE_DATE}
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
      field: "Status",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_STATUS}
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
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_ACTION}
        </Box>
      ),
      hide: true,
      renderCell: (params) => {
        return (
          <Box>
            <IconButton>
              {/* onClick={(e) => setAnchorEl(e.currentTarget)} */}
              <MoreVertIcon />
            </IconButton>
          </Box>
        );
      },
      flex: 1,
    },
  ];
  const calculatedHeight = windowHeight - navbarHeight - 65;

  useEffect(() => {
    gtAllSaleChannelLookupForSelection();
  }, []);

  return (
    <>
      {GridLoading ? (
        <CicrlesLoading />
      ) : viewMode === viewTypesEnum.GRID ? (
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "space-between",
            overflowX: "auto",
            bgcolor: "#F8F8F8",
            marginTop: "0px !important",
            height: calculatedHeight,
            border: "1px solid rgba(0, 0, 0, 0.12)",
            borderBottomLeftRadius: "8px",
            borderBottomRightRadius: "8px",
            padding: "8px",
          }}
        >
          <GridContainer>
            {(rows?.list || []).map((dt, index) => (
              <GridItem md={3} sm={4} xs={12} key={index}>
                <ReusableCardComponent
                  image={dt.ImageUrl}
                  title={dt.SaleChannelName}
                  description={
                    <>
                      <Typography paddingBottom={0.5} variant="body2">
                        {`Store: ${dt.StoreName}`}
                      </Typography>
                    </>
                  }
                  cardActionsChildren={
                    <ActionButtonCustom
                      sx={{
                        ...styleSheet.integrationactivatedButton,
                        width: "100%",
                        height: "28px",
                        borderRadius: "4px",
                        background: Colors.primary,
                      }}
                      variant="contained"
                      label={"Update"}
                      onClick={() => handleEditClick(dt)}
                    />
                  }
                  status={{
                    label: dt.Active
                      ? LanguageReducer?.languageType?.ACTIVE
                      : LanguageReducer?.languageType?.IN_ACTIVE,
                    color: dt.Active === false ? Colors.danger : Colors.succes,
                  }}
                />
              </GridItem>
            ))}
          </GridContainer>
          <PaginationComponent
            name={"Sale Channel"}
            dataCount={rows.TotalCount}
            paginationChangeMethod={getAllSaleChannelConfig}
            paginationMethodUrl={
              EnumChangeFilterModelApiUrls.GET_ALL_SALE_CHANNEL_ACTIVE.url
            }
            defaultRowsPerPage={pageSize}
            currentPage={currentPage}
            color="primary"
          />
        </Box>
      ) : (
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            height: calculatedHeight,
          }}
        >
          <DataGrid
            loading={GridLoading}
            rowHeight={40}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row?.SaleChannelConfigId}
            rows={rows?.list ? rows.list : []}
            columns={columns}
            disableSelectionOnClick
            pagination
            page={currentPage}
            pageSize={pageSize}
            rowsPerPageOptions={[5, 10, 15, 25]}
            paginationMode="server"
            rowCount={rows.TotalCount || 0}
            onPageChange={(newPage) => {
              getAllSaleChannelConfig(newPage, pageSize);
            }}
            onPageSizeChange={(newPageSize) => {
              getAllSaleChannelConfig(0, newPageSize);
            }}
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
                      primary={
                        LanguageReducer?.languageType?.BATCH_OUTSCAN_TEXT
                      }
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
              </List>
            </Box>
          </Menu>
        </Box>
      )}
      {openEditModal && (
        <UpdatePlatFormModal
          open={openEditModal}
          setOpen={setOpenEditModal}
          getAll={getAllSaleChannelConfig}
          isGridLoading={isGridLoading}
          allPlatformForSelection={allPlatformForSelection}
          data={editData}
        />
      )}
    </>
  );
}
export default PlatFormIntegrationList;
