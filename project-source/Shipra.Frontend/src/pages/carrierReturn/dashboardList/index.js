import DisplaySettingsIcon from "@mui/icons-material/DisplaySettings";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
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
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style.js";
import OrderDetailModal from "../../../components/modals/orderModals/OrderDetailModal";
import {
  DeleteMyCarrierReturnReport,
  ExcelExportReturnReportById,
  GetShipmentsByReturnRerportId,
} from "../../../api/AxiosInterceptors.js";
import { LoadingButton } from "@mui/lab";
import ReturnReportShipmentDetailModal from "../../../components/modals/carrierReturnModals/ReturnReportShipmentDetailModal.js";
import UtilityClass from "../../../utilities/UtilityClass.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import FileDownloadOutlined from '@mui/icons-material/FileDownloadOutlined';
import {
  ActionStack,
  CodeBox,
  DeleteButton,
  ExcelButton,
  centerColumn,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers.js";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal.js";

function CarrierReturnDashboardList(props) {
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [carrierRRId, setCarrierRRId] = React.useState("");
  const [openDeleteConfirmation, setOpenDeleteConfirmation] = useState(false);
  const {
    allReportData,
    getOrdersRef,
    resetRowRef,
    getAllReturnRerports,
    loading,
    isFilterOpen,
  } = props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [orderIteminfoModal, setOrderIteminfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [excelLoading, setExcelLoading] = useState({});

  const downloadExcel = (id) => {
    setExcelLoading((prev) => ({ ...prev, [id]: true }));
    ExcelExportReturnReportById(id)
      .then((res) => {
        UtilityClass.downloadExcel(res.data, "returnReportShipments");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      })
      .finally(() => {
        setAnchorEl(null);
        setExcelLoading((prev) => ({ ...prev, [id]: false }));
      });
  };
  const handleGetShipmentByTrturnReport = async (rrId) => {
    setOrderIteminfoModal((prev) => ({
      ...prev,
      loading: { [rrId]: true },
    }));
    await GetShipmentsByReturnRerportId(rrId)
      .then((res) => {
        const response = res.data;
        setOrderIteminfoModal((prev) => ({
          ...prev,
          open: true,
          data: response,
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setOrderIteminfoModal((prev) => ({
          ...prev,
          loading: { [rrId]: false },
        }));
      });
  };
  const handleActionButton = (cTarget, data) => {
    setAnchorEl(cTarget);
    setCarrierRRId(data.CarrierRRId);
  };
  const handleConfirmation = (data) => {
    setCarrierRRId(data);
    setOpenDeleteConfirmation(true);
  };
  const handleDelete = async (data) => {
    try {
      const param = {
        carrierRRId: carrierRRId,
      };
      const response = await DeleteMyCarrierReturnReport(param);
      if (response.data?.isSuccess) {
        setOpenDeleteConfirmation(false);
        getAllReturnRerports();
        successNotification("Record deleted successfully");
      } else {
        errorNotification("Record deleted unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
    }
  };
  const columns = [
    {
      field: "ReturnReportNo",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.CARRIER_RETURNS_RETURN_REPORT_NO}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }} disableRipple>
            <>
              <Box>
                <CodeBox
                  title={params.row.ReturnReportNo}
                  copyBtn={true}
                />
              </Box>
            </>
          </Box>
        );
      },
    },

    {
      ...centerColumn,
      field: "TotalOrders",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.CARRIER_RETURNS_TOTAL_ORDERS}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <Stack flexDirection={"row"}>
              {orderIteminfoModal?.loading[params.row.CarrierRRId] ? (
                <CircularProgress size={20} />
              ) : (
                <CodeBox
                  title={params.row.TotalOrders}
                  onClick={(e) => {
                    handleGetShipmentByTrturnReport(params.row.CarrierRRId);
                  }}
                  eyeBtn={true}
                />
              )}
            </Stack>
          </>
        );
      },
    },
    {
      ...centerColumn,
      field: "CarrierName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.CARRIER_RETURNS_CARRIER_NAME}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }} disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{params.row.CarrierName}</Box>
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
          {LanguageReducer?.languageType?.CARRIER_RETURNS_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <ActionStack>
            <ExcelButton
              loading={excelLoading[row?.CarrierRRId] || false}
              onClick={() => {
                downloadExcel(row.CarrierRRId);
              }}
            />
            {/* <DeleteButton
              onClick={(e) => handleConfirmation(row.CarrierRRId)}
            /> */}
          </ActionStack>
        );
      },
      minWidth: 90,
      flex: 1,
    },
  ];
  const [selectionModel, setSelectionModel] = useState([]);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
    getOrdersRef.current = oNos;
  };
  ////////////

  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
    }
  }, [resetRowRef.current]);
  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 65 - 86
    : windowHeight - navbarHeight - 65;
  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: calculatedHeight,
      }}
    >
      <DataGrid
        getRowHeight={() => "35px"}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        getRowId={(row) => row.CarrierRRId}
        rows={allReportData?.list ? allReportData?.list : []}
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
        selectionModel={selectionModel}
        onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
        loading={loading}
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
              disablePadding
              onClick={() => {
                downloadExcel();
              }}
            >
              <ListItemButton>
                <ListItemIcon sx={{ minWidth: "30px" }}>
                  <FileDownloadOutlined />
                </ListItemIcon>
                <ListItemText primary="Export Excel" />
              </ListItemButton>
            </ListItem>
          </List>
        </Box>
      </Menu>
      {infoModal.data?.result && (
        <OrderDetailModal
          data={infoModal?.data?.result}
          open={infoModal.open}
          onClose={() => setInfoModal((prev) => ({ ...prev, open: false }))}
        />
      )}
      {openDeleteConfirmation && (
        <DeleteConfirmationModal
          open={openDeleteConfirmation}
          setOpen={setOpenDeleteConfirmation}
          handleDelete={handleDelete}
        />
      )}
      {orderIteminfoModal.data?.result && (
        <ReturnReportShipmentDetailModal
          onClose={() =>
            setOrderIteminfoModal((prev) => ({ ...prev, open: false }))
          }
          data={orderIteminfoModal?.data?.result.list}
          open={orderIteminfoModal.open}
        />
      )}
    </Box>
  );
}
export default CarrierReturnDashboardList;
