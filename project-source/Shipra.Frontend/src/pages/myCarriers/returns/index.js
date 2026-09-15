import { LoadingButton } from "@mui/lab";
import {
  Box,
  Button,
  Grid,
  InputLabel,
  MenuItem,
  MenuList,
  Popover,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook.js";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent.js";
import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader.js";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal.js";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter.js";
import {
  DeleteMyCarrierReturnReport,
  GetAllMyCarrierReturnReports,
  GetPDFCarrierReturnReportById,
  GetPDFMyCarrierReturnReport,
  GetShipmentsByReturnRerportId,
} from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";
import OrdersModal from "../../../components/modals/myCarrierModals/OrdersModal.js";
import UtilityClass from "../../../utilities/UtilityClass.js";
import {
  ActionButton,
  ActionFilterButton,
  ActionStack,
  ClipboardIcon,
  CodeBox,
  DeleteButton,
  PDFButton,
  ViewButton,
  centerColumn,
  navbarHeight,
  pagePadding,
  useClientSubscriptionReducer,
  useGetWindowHeight,
} from "../../../utilities/helpers/Helpers.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import DataGridProComponent from "../../../.reUseableComponents/DataGrid/DataGridProComponent.js";

export default function ReturnsPage() {
  const initialFilterFields = {
    loading: true,
    show: false,
    startDate: null,
    endDate: null,
  };
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [filterModal, setFilterModal] = useState(initialFilterFields);
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [allReturnReports, setAllReturnReports] = useState([]);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [carrierRRId, setCarrierRRId] = React.useState("");
  const [openDeleteConfirmation, setOpenDeleteConfirmation] = useState(false);
  const [returnReportData, setReturnReportData] = useState({});
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [orderIteminfoModal, setOrderIteminfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const handleActionButton = (cTarget, data) => {
    setAnchorEl(cTarget);
    setCarrierRRId(data.CarrierRRId);
  };
  const handleConfirmation = (data) => {
    setReturnReportData(data);
    setOpenDeleteConfirmation(true);
  };
  const handleDelete = async (data) => {
    try {
      let param = {
        carrierRRId: returnReportData?.CarrierRRId,
      };
      const response = await DeleteMyCarrierReturnReport(param);
      if (response.data?.isSuccess) {
        getAllMyCarrierReturnReports();
        setOpenDeleteConfirmation(false);
        setReturnReportData({});
        successNotification("Record deleted successfully");
      } else {
        errorNotification("Record deleted unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
    }
  };
  const getFiltersFromState = () => {
    let search = "";
    let filters = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 1000,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
    };
    return filters;
  };
  const downloadPDF = () => {
    let params = getFiltersFromState();
    GetPDFMyCarrierReturnReport(params)
      .then((res) => {
        handleClose();
        UtilityClass.downloadPdf(res.data, "return reports");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  const [orderIteminfoPdf, setOrderIteminfoPdf] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const downloadPDFReturnShipment = (rId) => {
    let params = {
      filterModel: {
        createdFrom: null,
        createdTo: null,
        start: 0,
        length: 1000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      CarrierRRId: rId,
    };

    setOrderIteminfoPdf((prev) => ({
      ...prev,
      loading: { [rId]: true },
    }));
    GetPDFCarrierReturnReportById(params)
      .then((res) => {
        UtilityClass.downloadPdf(res.data, "return report");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      })
      .finally(() => {
        setOrderIteminfoPdf((prev) => ({
          ...prev,
          loading: { [rId]: false },
        }));
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
  const getAllMyCarrierReturnReports = async () => {
    setFilterModal((prev) => ({ ...prev, loading: true }));

    let params = getFiltersFromState();
    let res = await GetAllMyCarrierReturnReports(params);
    if (res.data.result !== null) {
      setAllReturnReports(res.data.result);
    }
    setFilterModal((prev) => ({ ...prev, loading: false }));

    resetRowRef.current = false;
  };
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);

  const columns = [
    {
      field: "ReturnReportNo",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_RETURN_RETURN_REPORT_NO}
        </Box>
      ),
      minWidth: 167,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            sx={{ textAlign: "center", cursor: "pointer" }}
            disableRipple
            paddingLeft={1.5}
          >
            <>
              <Stack direction={"row"} sx={{ alignItems: "center" }}>
                <CodeBox title={params.row.ReturnReportNo} />
                <ClipboardIcon text={params.row.ReturnReportNo} />
              </Stack>
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
          {LanguageReducer?.languageType?.MY_CARRIER_RETURN_TOTAL_ORDERS}
        </Box>
      ),
      minWidth: 140,
      flex: 1,
      renderCell: (params) => {
        return <>{params.row.TotalOrders}</>;
      },
    },
    {
      field: "CarrierName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_RETURN_CARRIER_NAME}
        </Box>
      ),
      minWidth: 140,
      flex: 1,
      hide: true,
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
      field: "CreatedOn",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_RETURN_CREATE_DATE}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }} disableRipple>
            <>
              <Box>
                {UtilityClass.convertUtcToLocalAndGetDate(params.row.CreatedOn)}
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 170,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_RETURN_ACTION}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <ActionStack>
            <PDFButton
              onClick={(e) => downloadPDFReturnShipment(params.row.CarrierRRId)}
              loading={orderIteminfoPdf.loading[params.row.CarrierRRId]}
            />
            <ViewButton
              loading={orderIteminfoModal.loading[params.row.CarrierRRId]}
              onClick={(e) =>
                handleGetShipmentByTrturnReport(params.row.CarrierRRId)
              }
            />

            <DeleteButton onClick={(e) => handleConfirmation(params.row)} />

            {/* <Stack direction={"row"} spacing={0.5}>
        <ActionButtonDelete onClick={(e) => console.log("de")} />
        <ActionButtonEdit onClick={(e) => console.log("de")} />
      </Stack> */}
            {/* <IconButton
          onClick={(e) => handleActionButton(e.currentTarget, params.row)}
        >
          <MoreVertIcon />
        </IconButton> */}
          </ActionStack>
        );
      },
      flex: 1,
    },
  ];

  useEffect(() => {
    getAllMyCarrierReturnReports();
  }, []);
  useEffect(() => {
    if (isFilterReset) {
      getAllMyCarrierReturnReports();
    }
  }, [isFilterReset]);
  const handleFilterRest = () => {
    resetDates();
  };
  const open = Boolean(anchorEl);
  const id = open ? "simple-popover" : undefined;
  const handleClose = () => {
    setAnchorEl(null);
  };

  const calculatedHeight = filterModal.show
    ? windowHeight - navbarHeight - 179
    : windowHeight - navbarHeight - 103;

  return (
    <Box sx={styleSheet.pageRoot}>
      <Box sx={{ padding: pagePadding }}>
        <DataGridHeader tabs={false}>
          <Box className={"flex_center"} gap={1}>
            <ActionButton
              onClick={(event) => {
                setAnchorEl(event.currentTarget);
              }}
            />
            <ActionFilterButton
              onClick={() => {
                setFilterModal((prev) => ({
                  ...prev,
                  show: !filterModal.show,
                }));
              }}
            />
          </Box>
        </DataGridHeader>
        {filterModal.show ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <TableRow>
                <Grid container spacing={2} sx={{ p: "10px 6px" }}>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {"Start Date"}
                      </InputLabel>

                      <CustomReactDatePickerInputFilter
                        value={startDate}
                        onClick={(date) => setStartDate(date)}
                        size="small"
                        isClearable
                        maxDate={UtilityClass.todayDate()}

                        // inputProps={{ style: { padding: "4px 5px" } }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {"End Date"}
                      </InputLabel>
                      <CustomReactDatePickerInputFilter
                        value={endDate}
                        onClick={(date) => setEndDate(date)}
                        size="small"
                        minDate={startDate}
                        disabled={!startDate ? true : false}
                        isClearable
                        maxDate={UtilityClass.todayDate()}

                        // inputProps={{ style: { padding: "4px 5px" } }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Stack
                      alignItems="flex-end"
                      direction="row"
                      spacing={1}
                      sx={{ ...styleSheet.filterButtonMargin, height: "100%" }}
                    >
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          handleFilterRest();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllMyCarrierReturnReports();
                        }}
                      >
                        {LanguageReducer?.languageType?.FILTER}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}
        <DataGridProComponent
          columns={columns}
          rows={allReturnReports?.list ? allReturnReports?.list : []}
          loading={filterModal.loading}
          getRowId={(row) => row.CarrierRRId}
          height={calculatedHeight}
        />
        {/* <Popover
          id={id}
          open={open}
          anchorEl={anchorEl}
          onClose={handleClose}
          anchorOrigin={{
            vertical: "bottom",
            horizontal: "left",
          }}
        >
          <MenuList
            sx={{
              minWidth: "162px !important",
            }}
            id={id}
            autoFocusItem
          >
            <MenuItem onClick={(event) => downloadPDF()}>Download PDF</MenuItem>
          </MenuList>
        </Popover> */}
        {orderIteminfoModal.data?.result && (
          <OrdersModal
            onClose={() =>
              setOrderIteminfoModal((prev) => ({ ...prev, open: false }))
            }
            data={orderIteminfoModal?.data?.result}
            open={orderIteminfoModal.open}
          />
        )}
        {openDeleteConfirmation && (
          <DeleteConfirmationModal
            open={openDeleteConfirmation}
            setOpen={setOpenDeleteConfirmation}
            handleDelete={handleDelete}
            heading={
              LanguageReducer?.languageType
                ?.ORDER_ARE_YOU_SURE_YOU_WANT_TO_DELETE_SELECTED_ORDERS
            }
            message={
              LanguageReducer?.languageType
                ?.ORDER_SELECTED_ORDERS_WILL_BE_DELETE_IMMEDIATELY_YOU_CANT_UNDO_THIS_ACTION
            }
            buttonText={LanguageReducer?.languageType?.ORDER_DELETE}
          />
        )}
      </Box>
    </Box>
  );
}
