import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableCell,
  TableContainer,
  TableBody,
  TableHead,
  TableRow,
  Typography,
  TextField,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import { styleSheet } from "../../../assets/styles/style";
import { DataGrid } from "@mui/x-data-grid";
import UtilityClass from "../../../utilities/UtilityClass";
import { usePagination } from "@mui/lab";
import { EnumDeliveryTaskStatus, EnumOptions } from "../../../utilities/enum";
import {
  CodeBox,
  DescriptionBox,
  DescriptionBoxWithChild,
  ClipboardIcon,
  DialerBox,
  ValidateButton,
  amountFormat,
  centerColumn,
  rightColumn,
} from "../../../utilities/helpers/Helpers";
import Colors, {
  Danger,
  Success,
  Warning,
} from "../../../utilities/helpers/Colors";
import StatusBadge from "../../shared/statudBadge";
import { useSelector } from "react-redux";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import {
  CreateDeliveryTaskAndAddToExistingNote,
  GetDeliveryNoteByNo,
} from "../../../api/AxiosInterceptors";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import { useForm } from "react-hook-form";

const ExistingNoteModal = (props) => {
  let {
    open,
    setOpen,
    orderNosData,
    allDrivers,
    getAllDeliveryTask,
    resetRowRef,
    allDeliveryTask,
    handleResetOutscanModel,
  } = props;
  const [isLoading, setIsLoading] = useState(false);
  const [searchText, setsearchText] = useState();
  const [deliveryDate, setDeliveryDate] = React.useState(new Date());
  const [allSelecetdDeliveryTask, setAllSelecetdDeliveryTask] = React.useState(
    []
  );
  const [deliveryNote, setDeliveryNote] = useState();

  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
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
  const handleChangeSearch = (e) => {
    setsearchText(e.target.value);
  };
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    reset,
    control,
  } = useForm();
  useEffect(() => {
    if (!getValues("searchNote")) {
      setDeliveryNote();
    }
  }, [getValues("searchNote")]);

  const columns = [
    {
      field: "OrderNo",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ORDER_NO}{" "}
          No.
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
            {/* <ValidateButton onClick={() => handleValidateLatLng(row)} /> */}
          </Box>
        );
      },
      flex: 1,
    },
  ];
  const handleAddExistingNoteSubmit = () => {
    if (deliveryNote?.driverId == 0) {
      errorNotification(
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_TASKS_PLEASE_CHOOSE_DRIVER
      );
      return false;
    }

    let param = {
      driverId: deliveryNote?.driverId,
      orderNos: orderNosData.join(),
      assigningDate: deliveryDate,
      AddToExisting: true,
      NoteNo: deliveryNote?.noteNo,
    };
    setIsLoading(true);
    CreateDeliveryTaskAndAddToExistingNote(param)
      .then((res) => {
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification(
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_DELIVERY_NOTE_CREATE_SUCCESSFULLY
          );
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

  const getDeliveryNoteByNo = async (data) => {
    const body = {
      NoteNo: data.searchNote,
    };
    await GetDeliveryNoteByNo(body)
      .then((res) => {
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          setDeliveryNote(res.data.result);
        }
      })
      .catch((e) => {
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
        );
      })
      .finally((e) => {
        setIsLoading(false);
      });
    // console.log("res", res.data.result);
    // if (res.data.result != null) setDeliveryNote(res.data.result);
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_TASKS_ADD_TO_EXISTING_NOTE
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_ADD_TO_EXISTING_NOTE
          }
          bg={purple}
          onClick={(e) => handleAddExistingNoteSubmit()}
          loading={isLoading}
          disabled={!deliveryNote}
        />
      }
    >
      <form onSubmit={handleSubmit(getDeliveryNoteByNo)}>
        <Grid container spacing={2} sx={{ p: "15px", paddingLeft: "0" }}>
          <Grid item xl={4} md={6} sm={12} xs={12}>
            <Grid>
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.DELIVERY_DATE_TEXT}
              </InputLabel>
              <CustomReactDatePickerInput
                value={deliveryDate}
                onClick={(date) => setDeliveryDate(date)}
                size="small"
              />
            </Grid>
          </Grid>
          <Grid item xl={4} md={6} sm={12} xs={8}>
            <Grid>
              <InputLabel required sx={styleSheet.inputLabel}>
                {
                  LanguageReducer?.languageType
                    ?.MY_CARRIER_DELIVERY_TASKS_NOTE_NUMBER
                }
              </InputLabel>
              <TextField
                placeholder="Search Here"
                size="small"
                id="searchNote"
                name="searchNote"
                fullWidth
                variant="outlined"
                {...register("searchNote", {
                  required: {
                    value: true,
                    message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                  },
                  pattern: {
                    value: /^(?!\s*$).+/,
                    message:
                      LanguageReducer?.languageType
                        ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                  },
                })}
                error={Boolean(errors.searchNote)} // set error prop
                helperText={errors.searchNote?.message}
              />
            </Grid>
          </Grid>
          <Grid item xl={4} md={6} alignSelf={"flex-end"}>
            <ButtonComponent title={"Search"} type="submit" p={"17px"} />
          </Grid>
          {deliveryNote && (
            <Grid item xs={12}>
              <Grid>
                <Box>
                  <Typography variant="h3">
                    Name: {deliveryNote?.noteNo}
                  </Typography>
                  <Typography variant="body1">
                    Shipment Count: {deliveryNote?.shipmentCount}
                  </Typography>
                </Box>
              </Grid>
            </Grid>
          )}
        </Grid>
      </form>

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
        const
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
    </ModalComponent>
  );
};

export default ExistingNoteModal;
