import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Avatar,
  Box,
  CircularProgress,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  Stack,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { styleSheet } from "../../../../assets/styles/style";
import DebriefNotesModal from "../../../../components/modals/myCarrierModals/DebriefNotesModal";
import EditNotesModal from "../../../../components/modals/myCarrierModals/EditNotesModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import UtilityClass from "../../../../utilities/UtilityClass";
import { EnumDeliveryNoteStatus } from "../../../../utilities/enum";
import {
  GetInventorySalesSummary,
  GetOrderAddressLatLngByDeliveryNoteId,
  GetPDFDeliveryRunSheet,
  DeleteDeliveryNoteById,
} from "../../../../api/AxiosInterceptors";
import { errorNotification, successNotification } from "../../../../utilities/toast";
import {
  ClipboardIcon,
  DialerBox,
  rightColumn,
  centerColumn,
  CodeBox,
  DeleteButton,
  ActionButtonCustom,
  usePagination,
  navbarHeight,
  useGetWindowHeight,
  useClientSubscriptionReducer,
} from "../../../../utilities/helpers/Helpers";
import { Warning, Success } from "../../../../utilities/helpers/Colors";
import ButtonComponent from "../../../../.reUseableComponents/Buttons/ButtonComponent";
import MergeDeliveryNotesModal from "../../../../components/modals/myCarrierModals/MergeDeliveryNotesModal";
import DrawerComponent from "../../../../.reUseableComponents/Modal/DrawerComponent";
import MapComponent from "../MapComponent";
import { warningNotification } from "../../../../utilities/toast";
import { purple } from "../../../../utilities/helpers/Helpers";

function NotesList(props) {
  const {
    allDeliveryNote,
    getOrdersRef,
    loading,
    resetRowRef,
    getAllDeliveryNote,
    selectedNoteIds = [],
    setSelectedNoteIds,
  } = props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const [openDbrief, setOpenDebrief] = useState(false);
  const [selectedRowData, setSelectedRowData] = useState({});
  const [allOrderNote, setAllOrderNote] = useState({});
  const [openMapDrawer, setOpenMapDrawer] = useState({
    open: false,
    data: [],
    loading: {},
    title: "",
  });
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const handleItemActionDropDown = (e, data) => {
    setSelectedRowData(data);
    setAnchorEl(e.currentTarget);
  };
  const handlePrint = (data) => {
    GetPDFDeliveryRunSheet(data.DeliveryNoteId)
      .then((res) => {
        UtilityClass.downloadPdf(res.data, "deliveryRunSheet");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };

  const handleDelete = (data) => {
    if (window.confirm("Are you sure you want to delete this delivery note?")) {
      const body = { DeliveryNoteId: data.DeliveryNoteId };
      DeleteDeliveryNoteById(body)
        .then((res) => {
          if (res.data.isSuccess) {
            successNotification("Delivery note deleted successfully");
            getAllDeliveryNote();
          } else {
            errorNotification("Unable to delete");
          }
        })
        .catch((err) => {
          console.log(err);
          errorNotification("Failed to delete Delivery Note");
        });
    }
  };

  const handleOpenDrawer = async (data) => {
    try {
      setOpenMapDrawer((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data?.DeliveryNoteId]: true },
        title: data?.NoteNo,
      }));

      const response = await GetOrderAddressLatLngByDeliveryNoteId(
        data?.DeliveryNoteId
      );

      setOpenMapDrawer((prev) => ({
        ...prev,
        open: true,
        data: response?.data?.result || [],
        loading: { ...prev.loading, [data?.DeliveryNoteId]: false },
      }));
    } catch (error) {
      console.error(error);
      setOpenMapDrawer((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data?.DeliveryNoteId]: false },
      }));
    }
  };

  //#region
  const columns = [
    {
      field: "NoteNo",
      // align: "center",
      // headerAlign: "center",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_NOTE_NO}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Box>
              <CodeBox
                title={
                  openMapDrawer.loading[params.row.DeliveryNoteId] ? (
                    <CircularProgress size={20} />
                  ) : (
                    params.row.NoteNo
                  )
                }
                onClick={() => handleOpenDrawer(params?.row)}
              />
            </Box>
            <ClipboardIcon text={params.row.NoteNo} />
          </Stack>
        );
      },
    },

    {
      field: "DriverName",
      // align: "center",
      // headerAlign: "center",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_DRIVER}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          params.row.DriverName && (
            <Box>
              <Box>{params.row?.DriverName}</Box>
              <Box>
                <DialerBox phone={params.row?.Phone} />
              </Box>
            </Box>
          )
        );
      },
    },

    // {
    //   field: "Phone",
    //   // align: "center",
    //   // headerAlign: "center",
    //   headerName: (
    //     <Box sx={{ fontWeight: "600" }}>
    //       {" "}
    //       {LanguageReducer?.languageType?.PHONE_TEXT}
    //     </Box>
    //   ),
    //   flex: 1,
    //   renderCell: (params) => {
    //     return (
    //       <Box>
    //         <DialerBox phone={params.row.Phone} />
    //       </Box>
    //     );
    //   },
    // },
    {
      field: "ShipmentCount",
      ...centerColumn,
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_NOTES_SHIPMENTS_COUNT
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.ShipmentCount} hasColor={false} />
          </>
        );
      },
    },
    {
      field: "Status",
      ...centerColumn,
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_NOTE_STATUS}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <StatusBadge
              title={params?.row.Status}
              maxWidth="88px"
              borderColor="rgba(0, 186, 119, 0.2)"
              color={
                params?.row.DeliveryNoteStatusId ===
                EnumDeliveryNoteStatus.Completed
                  ? "#fff"
                  : "#fff"
              }
              bgColor={
                params?.row.DeliveryNoteStatusId ===
                EnumDeliveryNoteStatus.InProgress
                  ? Warning
                  : Success
              }
            />
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "orderDate",
      // align: "center",
      // headerAlign: "center",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_DATE}
        </Box>
      ),
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.CreatedDate)}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "Action",
      align: "center",
      headerAlign: "center",
      minWidth: 160,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_ACTION}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Stack direction={"row"} gap={1}>
            <ActionButtonCustom
              onClick={() => {
                setSelectedRowData(params.row);
                setOpenDebrief(true);
              }}
              label={
                LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_DRBRIEF
              }
              width={"50px"}
            />
            <ActionButtonCustom
              onClick={() => handlePrint(params?.row)}
              label={
                LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_PRINT
              }
              width={"50px"}
              background={Success}
            />
            {/* <DeleteButton
              onClick={() => handleDelete(params?.row)}
              label="Delete"
              width={"50px"}
            /> */}
            {/* <Box>
              <IconButton
                onClick={(e) => handleItemActionDropDown(e, params.row)}
              >
                <MoreVertIcon />
              </IconButton>
            </Box> */}
          </Stack>
        );
      },
      flex: 1,
    },
  ];
  //#endregion

  const calculatedHeight = windowHeight - navbarHeight - 62;

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
        rows={allDeliveryNote.list ? allDeliveryNote.list : []}
        getRowId={(row) => row.DeliveryNoteId}
        columns={columns}
        disableSelectionOnClick
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        checkboxSelection
        onSelectionModelChange={(newSelectionModel) => {
          if (setSelectedNoteIds) setSelectedNoteIds(newSelectionModel);
        }}
        onRowSelectionModelChange={(newSelectionModel) => {
          if (setSelectedNoteIds) setSelectedNoteIds(newSelectionModel);
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
            <ListItem
              onClick={(e) => {
                handlePrint(e);
                setAnchorEl(null);
              }}
              disablePadding
            >
              <ListItemButton>
                <ListItemText
                  primary={LanguageReducer?.languageType?.PRINT_TEXT}
                />
              </ListItemButton>
            </ListItem>
            <ListItem
              onClick={() => {
                setAnchorEl(null);
                setOpenDebrief(true);
              }}
              disablePadding
            >
              <ListItemButton>
                <ListItemText
                  primary={LanguageReducer?.languageType?.DEBERIF_TEXT}
                />
              </ListItemButton>
            </ListItem>
          </List>
        </Box>
      </Menu>
      <EditNotesModal open={open} setOpen={setOpen} {...props} />
      {openDbrief && (
        <DebriefNotesModal
          open={openDbrief}
          setOpen={setOpenDebrief}
          {...props}
          selectedRowData={selectedRowData}
          setSelectedRowData={setSelectedRowData}
          allOrderNote={allOrderNote}
          getAllDeliveryNote={getAllDeliveryNote}
        />
      )}
      {openMapDrawer.open && (
        <DrawerComponent
          title={openMapDrawer?.title}
          open={openMapDrawer.open}
          onClose={() =>
            setOpenMapDrawer((prev) => ({
              ...prev,
              open: false,
            }))
          }
        >
          <MapComponent locations={openMapDrawer?.data} />
        </DrawerComponent>
      )}
    </Box>
  );
}
export default NotesList;
