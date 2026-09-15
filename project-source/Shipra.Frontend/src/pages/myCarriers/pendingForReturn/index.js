import React, { useEffect, useRef, useState } from "react";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent.js";
import {
  Avatar,
  Box,
  Button,
  Grid,
  IconButton,
  InputLabel,
  MenuItem,
  MenuList,
  Popover,
  Stack,
  Table,
  TableHead,
  TableRow,
  Typography,
  TableBody,
  TableCell,
  TableContainer,
} from "@mui/material";
import { styleSheet } from "../../../assets/styles/style.js";
import { useSelector } from "react-redux";
import StatusBadge from "../../../components/shared/statudBadge/index.js";
import { EnumDeliveryTaskStatus } from "../../../utilities/enum/index.js";
import UtilityClass from "../../../utilities/UtilityClass.js";
import {
  ActionButton,
  ActionFilterButton,
  ClearFilterButton,
  DataGridHeaderBox,
  DialerBox,
  FilterButton,
  MailtoBox,
  pagePadding,
  rightColumn,
  centerColumn,
  CodeBox,
  DescriptionBoxWithChild,
  useGetWindowHeight,
  navbarHeight,
  useClientSubscriptionReducer,
} from "../../../utilities/helpers/Helpers.js";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter.js";
import DataGridFilters from "../../../.reUseableComponents/DataGridFilters/DataGridFilters.js";
import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader.js";
import TextFieldLabelComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent.js";
import { GetAllPendingForReturnShipments } from "../../../api/AxiosInterceptors.js";
import { warningNotification } from "../../../utilities/toast/index.js";
import BatchCreateReturnReportMyCarrierModal from "../../../components/modals/myCarrierModals/BatchCreateReturnReportMyCarrierModal.js";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook.js";

export default function PendingForReturn() {
  const initialFilterFields = {
    loading: false,
    show: false,
    startDate: null,
    endDate: null,
  };
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [allPendingForReturn, setAllPendingForReturn] = useState([]);
  const [filterModal, setFilterModal] = useState(initialFilterFields);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [anchorEl, setAnchorEl] = React.useState(null);

  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const handleClose = () => {
    setAnchorEl(null);
  };
  const open = Boolean(anchorEl);
  const id = open ? "simple-popover" : undefined;

  const handleGetPendinForReturnsData = () => {
    setFilterModal((prev) => ({ ...prev, loading: false }));
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
  let getAllPendingForReturnShipments = async () => {
    setFilterModal((prev) => ({ ...prev, loading: true }));

    let params = getFiltersFromState();
    let res = await GetAllPendingForReturnShipments(params);
    if (res.data.result !== null) {
      setAllPendingForReturn(res.data.result);
    }
    setFilterModal((prev) => ({ ...prev, loading: false }));

    resetRowRef.current = false;
  };

  // columns
  const columns = [
    {
      field: "OrderNo",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_PENDING_FOR_RETURN_ORDER_NO
          }
        />
      ),
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center", cursor: "pointer" }}
            disableRipple
          >
            <>
              <CodeBox title={params.row.OrderNo} />
              <Box>{params.row.TrackingNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Customer",
      minWidth: 80,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_PENDING_FOR_RETURN_CUSTOMER
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box display={"flex"} flexDirection={"column"} disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row.Customer}
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
                              // width: "110px",
                            }}
                            align="left"
                          >
                            Name
                          </TableCell>
                          <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                            }}
                          >
                            Mobile
                          </TableCell>
                          {/* <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                              // width: "150px",
                            }}
                          >
                            Address
                          </TableCell> */}
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        <TableRow
                          key={params.row.CustomerName}
                          sx={{
                            "&:last-child td, &:last-child th": { border: 0 },
                          }}
                        >
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                            }}
                            align="left"
                          >
                            {params.row.Customer}
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="left"
                          >
                            <DialerBox phone={params.row.Phone} />
                          </TableCell>
                          {/* <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              // width: "150px",
                            }}
                          >
                            {params.row.CustomerFullAddress}
                          </TableCell> */}
                        </TableRow>
                      </TableBody>
                    </Table>
                  </TableContainer>
                </DescriptionBoxWithChild>
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "DriverName",
      minWidth: 80,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.MY_CARRIER_PENDING_FOR_RETURN_DRIVER
          }
        />
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            {/* <Box>{params.row.DriverCode}</Box> */}
            <Box>{params.row.DriverName}</Box>
            <DialerBox phone={params.row.DriverMobile} />
          </Box>
        );
      },
    },
    {
      field: "Driver Email",
      minWidth: 100,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_PENDING_FOR_RETURN_DRIVER_EMAIL
          }
        />
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <MailtoBox email={params.row.DriverEmail} />
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "CarrierTrackingStatus",
      minWidth: 120,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_PENDING_FOR_RETURN_CARRIER_STATUS
          }
        />
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row.CarrierTrackingStatus}
            color="#1E1E1E;"
            bgColor="#EAEAEA"
          />
        );
      },
    },
    {
      ...centerColumn,
      field: "PaymentMethodStatus",
      minWidth: 120,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_PENDING_FOR_RETURN_PAYMENT_STATUS
          }
        />
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row?.PaymentMethodStatus}
            borderColor="rgba(0, 186, 119, 0.2)"
            color={
              params.row?.PaymentMethodStatus ===
              EnumDeliveryTaskStatus.Completed
                ? "#00BA77"
                : "#FF9A23"
            }
            bgColor={
              params.row?.PaymentMethodStatus === EnumDeliveryTaskStatus.Pending
                ? "rgba(255, 154, 35, 0.1)"
                : "rgba(0, 186, 119, 0.1)"
            }
          />
        );
      },
    },

    // {
    //   field: "Mobile1",
    //   headerName: (
    //     <DataGridHeaderBox title={LanguageReducer?.languageType?.PHONE_TEXT} />
    //   ),
    //   flex: 1,
    //   renderCell: (params) => {
    //     return <Box>{params.row.Phone}</Box>;
    //   },
    // },
    {
      field: "Remarks",
      minWidth: 100,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.MY_CARRIER_PENDING_FOR_RETURN_REMARKS
          }
        />
      ),
      flex: 1,
      renderCell: (params) => {
        return <Box>{params.row.Remarks}</Box>;
      },
    },
    {
      field: "Action",
      minWidth: 150,
      hide: true,
      headerName: <DataGridHeaderBox title={"Action"} />,
      renderCell: (params) => {
        return (
          <Box>
            <IconButton>
              <MoreVertIcon />
            </IconButton>
          </Box>
        );
      },
      flex: 1,
    },
  ];
  useEffect(() => {
    getAllPendingForReturnShipments();
  }, []);
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [openCreateModel, setOpenCreateModel] = useState(false);

  useEffect(() => {
    if (isFilterReset) {
      getAllPendingForReturnShipments();
      setIsFilterReset(false);
    }
  }, [isFilterReset]);
  const handleFilterRest = () => {
    resetDates();
  };
  const handleCreateReturnReport = () => {
    let selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length > 0) {
      console.log(selectedTrNos);
      setOpenCreateModel(true);
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    }
    setAnchorEl(null);
  };
  const calculatedHeight = filterModal.show
    ? windowHeight - navbarHeight - 151.5
    : windowHeight - navbarHeight - 66;

  return (
    <Box sx={styleSheet.pageRoot}>
      <Box sx={{ padding: pagePadding }}>
        <DataGridHeader tabs={false} checkboxSelection>
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
                <Grid container spacing={2} sx={{ p: "15px" }}>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_PENDING_FOR_RETURN_START_DATE
                        }
                      </InputLabel>

                      <CustomReactDatePickerInputFilter
                        maxDate={UtilityClass.todayDate()}
                        value={startDate}
                        onClick={(date) => setStartDate(date)}
                        size="small"
                        isClearable
                        // inputProps={{ style: { padding: "4px 5px" } }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_PENDING_FOR_RETURN_END_DATE
                        }
                      </InputLabel>
                      <CustomReactDatePickerInputFilter
                        maxDate={UtilityClass.todayDate()}
                        value={endDate}
                        onClick={(date) => setEndDate(date)}
                        size="small"
                        minDate={startDate}
                        disabled={!startDate ? true : false}
                        isClearable
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
                        {LanguageReducer?.languageType?.ORDER_CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllPendingForReturnShipments();
                        }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_PENDING_FOR_RETURN_FILTER
                        }
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}
        <DataGridComponent
          getRowHeight={() => "auto"}
          columns={columns}
          rows={allPendingForReturn}
          loading={filterModal.loading}
          getRowId={(row) => row.OrderNo}
          getOrdersRef={getOrdersRef}
          resetRowRef={resetRowRef}
          checkboxSelection
          height={calculatedHeight}
        />
      </Box>
      <Popover
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
          <MenuItem onClick={(event) => handleCreateReturnReport()}>
            {
              LanguageReducer?.languageType
                ?.MY_CARRIER_PENDING_FOR_RETURN_CREATE_RETURN_REPORT
            }
          </MenuItem>
        </MenuList>
      </Popover>

      {openCreateModel && (
        <BatchCreateReturnReportMyCarrierModal
          resetRowRef={resetRowRef}
          getAll={getAllPendingForReturnShipments}
          orderNosData={getOrdersRef.current}
          open={openCreateModel}
          setOpen={setOpenCreateModel}
        />
      )}
    </Box>
  );
}
