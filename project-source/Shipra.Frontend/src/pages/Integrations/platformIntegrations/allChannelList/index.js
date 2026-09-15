import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  Typography,
  Pagination,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import SaleChannelConnectModal from "../../../../components/modals/integrationModals/saleChannelConnectModal";
import {
  EnumChangeFilterModelApiUrls,
  viewTypesEnum,
} from "../../../../utilities/enum";
import Colors from "../../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  CicrlesLoading,
  greyBorder,
  GridContainer,
  GridItem,
  navbarHeight,
  ReusableCardComponent,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import { PaginationComponent } from "../../../../utilities/helpers/paginationSchema";

function AllSaleChannelList(props) {
  const { allPlatformForSelection, GridLoading, viewMode } = props;
  const [anchorEl, setAnchorEl] = useState(null);
  const [openConnectModal, setOpenConnectModal] = useState(false);
  const [selectedSaleChannel, setSelectedSaleChannel] = useState({});
  const [settingConfig, setSettingConfig] = useState({});
  const [inputConfig, setInputConfig] = useState(false);
  const [loading, setLoading] = useState();
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

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
            <Typography variant="h6">{params.row.saleChannelName}</Typography>
          </>
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
      renderCell: ({ row }) => {
        return (
          <Box>
            <ActionButtonCustom
              label={"Activate"}
              onClick={(event) => {
                setSelectedSaleChannel(row.saleChannelLookupId);
                setSettingConfig(row.settingConfig);
                setInputConfig(row.inputRequiredConfig);
                setOpenConnectModal(true);
              }}
            ></ActionButtonCustom>
          </Box>
        );
      },
      flex: 1,
    },
  ];
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  const calculatedHeight = windowHeight - navbarHeight - 70;

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
            {(allPlatformForSelection.slice(currentPage * pageSize, (currentPage + 1) * pageSize) || []).map((dt, index) => (
              <GridItem md={3} sm={4} xs={12} key={index}>
                <ReusableCardComponent
                  image={dt.imageUrl}
                  title={dt.saleChannelName}
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
                      onClick={(event) => {
                        setSelectedSaleChannel(dt.saleChannelLookupId);
                        setSettingConfig(dt.settingConfig);
                        setInputConfig(dt.inputRequiredConfig);
                        setOpenConnectModal(true);
                      }}
                      label={
                        LanguageReducer?.languageType
                          ?.INTEGRATION_SALE_CHANNEL_ACTIVATE
                      }
                    />
                  }
                />
              </GridItem>
            ))}
          </GridContainer>
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              mt: 2,
              bgcolor: "white",
              border: "1px solid rgba(224, 224, 224, 1)",
              py: 1,
              px: 2,
              borderRadius: "4px"
            }}
          >
            <Typography variant="body2" sx={{ fontWeight: 500, color: "#555" }}>
              Total Sale Channel: {allPlatformForSelection.length}
            </Typography>
            <Pagination
              count={Math.ceil(allPlatformForSelection.length / pageSize)}
              color="primary"
              page={currentPage + 1}
              onChange={(event, val) => handlePageChange(val - 1)}
            />
          </Box>
        </Box>
      ) : (
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
            getRowId={(row) => row?.saleChannelLookupId}
            rows={allPlatformForSelection || []}
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
                <ListItem disablePadding>
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
      {openConnectModal && (
        <SaleChannelConnectModal
          open={openConnectModal}
          setOpen={setOpenConnectModal}
          selectedSaleChannel={selectedSaleChannel}
          settingConfig={settingConfig}
          inputConfig={inputConfig}
        />
      )}
    </>
  );
}
export default AllSaleChannelList;
