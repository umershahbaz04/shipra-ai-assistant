import {
  Box,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import useGoogleMapGeocodeGetLatLng from "../../../.reUseableComponents/CustomHooks/useGoogleMapGeocodeGetLatLng";
import useOpenStreetmapGetLatLng from "../../../.reUseableComponents/CustomHooks/useOpenStreetmapGetLatLng";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { CreateDeliveryTaskAndAddToExistingNote } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumDeliveryTaskStatus, EnumOptions } from "../../../utilities/enum";
import Colors, {
  Danger,
  Success,
  Warning,
} from "../../../utilities/helpers/Colors";
import {
  ClipboardIcon,
  CodeBox,
  DescriptionBox,
  DescriptionBoxWithChild,
  DialerBox,
  ValidateButton,
  amountFormat,
  centerColumn,
  purple,
  rightColumn,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import StatusBadge from "../../shared/statudBadge";

function BatchOutScanModal(props) {
  let {
    open,
    setOpen,
    orderNosData,
    allDrivers,
    getAllDeliveryTask,
    resetRowRef,
    allDeliveryTask,
    handleResetOutscanModel,
    getDriversForSelection,
  } = props;
  const [isLoading, setIsLoading] = React.useState(false);
  const [driverId, setDriverId] = React.useState(0);
  const [deliveryDate, setDeliveryDate] = React.useState(new Date());
  const [allSelecetdDeliveryTask, setAllSelecetdDeliveryTask] = React.useState(
    []
  );

  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  useEffect(() => {
    const foundItems = allDeliveryTask?.list?.filter((item) =>
      orderNosData.includes(item.OrderNo)
    );
    setAllSelecetdDeliveryTask(foundItems);
  }, [orderNosData]);
  const handleClose = () => {
    setOpen(false);
  };
  const handleSubmit = () => {
    if (driverId == 0) {
      errorNotification("Please choose driver");
      return false;
    }
    let param = {
      driverId: driverId.DriverId,
      orderNos: orderNosData.join(),
      assigningDate: deliveryDate,
      AddToExisting: false,
      NoteNo: "",
    };
    setIsLoading(true);
    CreateDeliveryTaskAndAddToExistingNote(param)
      .then((res) => {
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Delivery note created successfully");
          resetRowRef.current = true;
          getAllDeliveryTask();
          handleResetOutscanModel();
          setOpen(false);
        }
      })
      .catch((e) => {
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
        );
        console.log("e", e);
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  const {
    getAddressCoordinates,
    isLoading: isOpenStreamLoading,
    error,
  } = useOpenStreetmapGetLatLng();
  const {
    getLatLng,
    isLoading: isLoadingGMap,
    error: gMapError,
  } = useGoogleMapGeocodeGetLatLng();

  const handleValidateLatLng = async (row) => {
    await handleGetLatLngFromAddress(row);
  };
  const handleGetLatLngFromAddress = async (row) => {
    try {
      let cityName = "";
      if (row?.CityName && row?.CityName != null) {
        cityName = `,${row?.CityName}`;
      }
      const address = `${row?.CountryName},${row?.RegionName}${cityName}`;

      // const { lat, lng } = await getLatLng(address);
      // console.log(lat, lng);
      const { lat: latitude, lng: longitude } = await getAddressCoordinates(
        address
      );
      console.log("Latitude:", latitude);
      console.log("Longitude:", longitude);
    } catch (error) {
      console.error("Error:", error.message);
    }
  };

  //#region table colum
  const columns = [
    {
      field: "OrderNo",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ORDER_NO}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            // display={"flex"}
            // flexDirection={"column"}
            // justifyContent={"center"}
            // sx={{ textAlign: "center", cursor: "pointer" }}
            disableRipple
          >
            <>
              <Stack direction={"row"} sx={{ alignItems: "center" }}>
                <CodeBox title={params.row.OrderNo} />
                <ClipboardIcon text={params.row.OrderNo} />
                <DescriptionBox
                  description={params.row.Remarks}
                  title="Remarks"
                />
              </Stack>
              <Box>
                <CodeBox title={params.row.TrackingNo} onClick={() => {}} />
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "TrackingStatus",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_CARRIER_STATUS
          }
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <StatusBadge
              title={params.row.TrackingStatus}
              color="#1E1E1E;"
              bgColor="#EAEAEA"
            />
          </>
        );
      },
    },
    {
      field: "Customer",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_CUSTOMER}
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
                          <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                            }}
                          >
                            Mobile 2
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
                            <DialerBox phone={params.row.Mobile1} />
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="left"
                          >
                            <DialerBox phone={params.row.Mobile2} />
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
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_DRIVER}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <Box sx={{ textAlign: "center" }} disableRipple>
              <>
                <Box>{params.row.DriverName}</Box>
              </>
            </Box>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "DeliveryTaskStatus",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_TASK_STATUS}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        let bgClr = Success;

        if (EnumDeliveryTaskStatus.Pending == params.row.DeliveryTaskStatusId) {
          bgClr = Danger;
        } else if (
          EnumDeliveryTaskStatus.Started == params.row.DeliveryTaskStatusId
        ) {
          bgClr = Warning;
        } else if (
          EnumDeliveryTaskStatus.Attempted == params.row.DeliveryTaskStatusId
        ) {
          bgClr = Colors.purple;
        }
        return (
          <StatusBadge
            title={params.row?.DeliveryTaskStatus}
            borderColor="rgba(0, 186, 119, 0.2)"
            color={
              EnumDeliveryTaskStatus.Completed ==
              params.row.DeliveryTaskStatusId
                ? "#fff;"
                : "#fff;"
            }
            bgColor={bgClr}
          />
        );
      },
    },
    {
      ...centerColumn,
      field: "OrderDate",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ORDER_DATE}
        </Box>
      ),
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
        </Box>
      ),
      flex: 1,
    },
    {
      ...rightColumn,
      field: "Amount",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_COD}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <>{amountFormat(params.row.Amount)}</>;
      },
    },
    {
      field: "Action",
      hide: true,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>
            <ValidateButton onClick={() => handleValidateLatLng(row)} />
          </Box>
        );
      },
      flex: 1,
    },
  ];
  //#endregion
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="xl"
      title={
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_TASKS_CREATE_DELIVERY_NOTE
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_CREATE_DELIVERY_NOTE
          }
          bg={purple}
          onClick={(e) => handleSubmit()}
          loading={isLoading}
        />
      }
    >
      {/* <Card variant="outlined" sx={styleSheet.tagsCard}>
        <Typography sx={styleSheet.tagsCardHeading}>{"Order Nos"}</Typography>
        <Paper
          sx={{
            display: "flex  !important",
            justifyContent: "flex-start  !important",
            flexWrap: "wrap  !important",
            p: 0.5,
            m: 0,
          }}
          elevation={0}
        >
          {chipData.map((data) => {
            return (
              <Box key={data.key} sx={{ mr: "10px", mb: "8px" }}>
                <Chip
                  sx={styleSheet.tagsChipStyle}
                  size="small"
                  icon={
                    <CheckCircleIcon
                      fontSize="small"
                      sx={{ color: "white  !important" }}
                    />
                  }
                  deleteIcon={<CloseIcon sx={{ color: "white  !important" }} />}
                  label={data.label}
                  onDelete={() => {}}
                />
              </Box>
            );
          })}
        </Paper>
      </Card>
      <br /> */}
      <Grid container spacing={1} sx={{ p: "15px" }}>
        <Grid item xl={6} lg={6} md={6}>
          <Grid>
            <InputLabel required sx={styleSheet.inputLabel}>
              {
                LanguageReducer?.languageType
                  ?.MY_CARRIER_DELIVERY_TASKS_DELIVERY_DATE
              }
            </InputLabel>
            <CustomReactDatePickerInput
              value={deliveryDate}
              onClick={(date) => setDeliveryDate(date)}
              size="small"
            />
          </Grid>
        </Grid>
        <Grid item xl={6} lg={6} md={6}>
          <Grid>
            <InputLabel required sx={styleSheet.inputLabel}>
              {
                LanguageReducer?.languageType
                  ?.MY_CARRIER_DELIVERY_TASKS_SELECT_DRIVER
              }
            </InputLabel>
            <SelectComponent
              name="reason"
              options={allDrivers}
              value={driverId}
              isRefesh={true}
              handleRefreshClick={getDriversForSelection}
              optionLabel={EnumOptions.DRIVER.LABEL}
              optionValue={EnumOptions.DRIVER.VALUE}
              onChange={(e, val) => {
                setDriverId(val);
              }}
            />
          </Grid>
        </Grid>
      </Grid>
      <Grid>
        <DataGrid
          rowHeight={40}
          headerHeight={40}
          autoHeight
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          rows={allSelecetdDeliveryTask}
          getRowId={(row) => row.OrderNo}
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
      </Grid>
    </ModalComponent>
  );
}
export default BatchOutScanModal;
