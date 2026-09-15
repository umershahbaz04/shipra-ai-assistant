import Edit from '@mui/icons-material/Edit';
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";
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
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import {
  GetAllChannelByStoreId,
  GetStoreById,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import EditStoreModal from "../../../components/modals/storeModals/EditStoreModal";
import StatusBadge from "../../../components/shared/statudBadge";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  ActionButtonCustom,
  AnchorBox,
  centerColumn,
  ClipboardIcon,
  CodeBox,
  DescriptionBoxWithChild,
  DialerBox,
  MailtoBox,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../utilities/toast";
import GenerateEncryptionKeyModal from "../../../components/modals/storeModals/GenerateEncryptionKeyModal";

function StoreList(props) {
  const {
    stores,
    setOpenDeleteStore,
    setOpenAddChannel,
    getAllStores,
    isGridLoading,
    isFilterOpen,
  } = props;

  const [anchorEl, setAnchorEl] = React.useState(null);
  const [selectedRow, setSelectedRow] = React.useState(null);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [openEditStore, setOpenEditStore] = useState(false);
  const [storeData, setStoreData] = useState();

  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [openGenerateKeyModal, setOpenGenerateKeyModal] = useState({
    open: false,
    StoreID: null,
  });
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);

  const handleUpdateStoreNo = (data) => {
    if (data) {
      setInfoModal((prev) => ({ ...prev, loading: { [data.StoreId]: true } }));

      GetStoreById(data.StoreId)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.errors);
          } else {
            setStoreData(res?.data?.result);
            setInfoModal((prev) => ({
              ...prev,
              open: true,
              data: res?.data?.result,
            }));
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Something went wrong");
        })
        .finally(() => {
          setInfoModal((prev) => ({
            ...prev,
            loading: { [data.StoreId]: false },
          }));
        });
    }
  };
  useEffect(() => {
    if (storeData) {
      setOpenEditStore(true);
    }
  }, [storeData]);

  const handleGetChannelByStoreId = async (storeId) => {
    setInfoModal((prev) => ({ ...prev, loading: { [storeId]: true } }));
    await GetAllChannelByStoreId(storeId)
      .then((res) => {
        const response = res.data.result?.data?.list;
        setInfoModal((prev) => ({ ...prev, open: true, data: response }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setInfoModal((prev) => ({ ...prev, loading: { [storeId]: false } }));
      });
  };

  const columns = [
    {
      field: "Store Code",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_CODE}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <>
            {infoModal.loading[params.row.StoreId] ? (
              <CircularProgress size={20} />
            ) : (
              <>
                <CodeBox
                  title={params.row.StoreCode}
                  onClick={() => handleUpdateStoreNo(params.row)}
                />
              </>
            )}
            <ClipboardIcon text={params.row.StoreCode} />
          </>
        );
      },
    },
    {
      field: "name",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_NAME}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              {params.row.StoreImage ? (
                <Avatar
                  src={params.row.StoreImage}
                  sx={{
                    width: 30,
                    height: 30,
                    fontSize: "13px",
                    color: "var(--primary-color)",
                    background: "rgba(86, 58, 213, 0.3)",
                  }}
                />
              ) : (
                <Avatar
                  sx={{
                    width: 25,
                    height: 25,
                    fontSize: "13px",
                    color: "var(--primary-color)",
                    background: "rgba(86, 58, 213, 0.3)",
                  }}
                >
                  {params.row.StoreName?.slice(0, 1)}
                </Avatar>
              )}
              <Box>{params.row.StoreName || ""}</Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "countryName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_COUNTRY}
        </Box>
      ),
      minWidth: 180,
      flex: 1,
      renderCell: (params) => {
        return <Box>{params.row.CountryName || ""}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "StoreCompany",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_COMPANY}
        </Box>
      ),
    },
    {
      field: "CustomerServiceNo",
      minWidth: 140,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.STORE_SERVICE_NO}
        </Box>
      ),
      renderCell: (params) => {
        return <DialerBox phone={params.row.CustomerServiceNo} />;
      },
    },
    {
      field: "email",
      minWidth: 160,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_EMAIL}
        </Box>
      ),
      // description: "This column has a value getter and is not sortable.",
      sortable: false,
      renderCell: (params) => {
        return <MailtoBox email={params.row.Email} />;
      },
    },
    {
      field: "URLs",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.STORE_URL}
        </Box>
      ),
      minWidth: 60,
      flex: 1,
      renderCell: (params) => {
        return <AnchorBox href={params.row.URLs} />;
      },
    },
    {
      field: "SaleChannelConfigCount",
      minWidth: 80,
      flex: 1,
      description: "Sale Channel Count",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.STORE_SC_COUNT}
        </Box>
      ),
      // description: "This column has a value getter and is not sortable.",
      sortable: false,
      headerAlign: "center",
      align: "center",
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.SaleChannelConfigCount || "0"} />
            {params?.row?.SalesChannel?.length > 0 && (
              <DescriptionBoxWithChild>
                <TableContainer>
                  <Table sx={{ minWidth: 275 }} aria-label="simple table">
                    <TableHead>
                      <TableRow>
                        <TableCell
                          sx={{
                            fontWeight: "bold",
                            fontSize: "11px",
                            padding: "5px",
                            width: "110px",
                          }}
                          align="left"
                        >
                          Sale Channel Name
                        </TableCell>
                        <TableCell
                          sx={{
                            fontWeight: "bold",
                            fontSize: "11px",
                            padding: "5px",
                            width: "110px",
                          }}
                          align="left"
                        >
                          Sale Channel Key
                        </TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {params?.row?.SalesChannel?.map((row) => (
                        <TableRow
                          key={row.name}
                          sx={{
                            "&:last-child td, &:last-child th": { border: 0 },
                          }}
                          // style={customRowStyle}
                        >
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              width: "110px",
                            }}
                            align="left"
                          >
                            {/* <CodeBox title={row?.SKU} /> */}
                            <Box>
                              <Stack direction={"column"}>
                                <Box>{row?.saleChannelName}</Box>
                              </Stack>
                            </Box>
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              width: "90px",
                            }}
                            align="left"
                          >
                            {row?.saleChannelKey}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              </DescriptionBoxWithChild>
            )}
          </>
        );
      },
    },
    {
      field: "Status",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.STORE_STATUS}
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
      ...centerColumn,
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACTION}
        </Box>
      ),
      minWidth: 120,
      renderCell: ({ row }) => {
        return (
          // <Box>
          //   <IconButton
          //     onClick={(e) => {
          //       setSelectedRow(row);
          //       setAnchorEl(e.currentTarget);
          //     }}
          //   >
          //     <MoreVertIcon />
          //   </IconButton>
          // </Box>
          <Box>
            <ActionButtonCustom
              label={"Generate Key"}
              onClick={() =>
                setOpenGenerateKeyModal((prev) => ({
                  ...prev,
                  open: true,
                  StoreID: row?.StoreId,
                }))
              }
            />
          </Box>
        );
      },
    },
  ];
  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 65 - 86
    : windowHeight - navbarHeight - 65;
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  return (
    <>
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: calculatedHeight,
        }}
      >
        <DataGrid
          loading={isGridLoading}
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row.RowNum}
          rows={stores}
          columns={columns}
          checkboxSelection={false}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          height={calculatedHeight}
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
          <Box sx={{ width: "200px" }}>
            <List disablePadding>
              <ListItem
                onClick={() => {
                  setOpenEditStore(true);
                  setAnchorEl(null);
                }}
                disablePadding
              >
                <ListItemButton>
                  <ListItemIcon sx={{ minWidth: "30px" }}>
                    <Edit />
                  </ListItemIcon>
                  <ListItemText primary="Update Store" />
                </ListItemButton>
              </ListItem>
              {selectedRow && !selectedRow?.IsDefault && (
                <ListItem
                  onClick={() => {
                    setOpenDeleteStore(true);
                    setAnchorEl(null);
                  }}
                  disablePadding
                >
                  <ListItemButton>
                    <ListItemIcon sx={{ minWidth: "30px" }}>
                      <DeleteOutlineIcon />
                    </ListItemIcon>
                    <ListItemText primary="Delete Store" />
                  </ListItemButton>
                </ListItem>
              )}
              {/* <ListItem
                onClick={() => {
                  setOpenAddChannel(true);
                  setAnchorEl(null);
                }}
                disablePadding
              >
                <ListItemButton>
                  <ListItemIcon sx={{ minWidth: "30px" }}>
                    <AddOutlinedIcon />
                  </ListItemIcon>
                  <ListItemText primary="Add Channel" />
                </ListItemButton>
              </ListItem> */}
            </List>
          </Box>
        </Menu>
      </Box>
      {/* {infoModal.open && (
        <StoreChannelInfoModal
          data={infoModal?.data}
          open={infoModal.open}
          onClose={() => setInfoModal((prev) => ({ ...prev, open: false }))}
        />
      )} */}

      {openEditStore && (
        <EditStoreModal
          open={openEditStore}
          setOpen={setOpenEditStore}
          getAllStores={getAllStores}
          selectedRow={selectedRow}
          storeData={storeData}
          setStoreData={setStoreData}
          {...props}
        />
      )}
      {openGenerateKeyModal.open && (
        <GenerateEncryptionKeyModal
          open={openGenerateKeyModal.open}
          onClose={() =>
            setOpenGenerateKeyModal((prev) => ({ ...prev, open: false }))
          }
          StoreID={openGenerateKeyModal.StoreID}
        />
      )}
    </>
  );
}
export default StoreList;
