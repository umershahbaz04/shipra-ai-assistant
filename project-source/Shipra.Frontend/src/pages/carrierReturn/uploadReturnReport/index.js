import Download from '@mui/icons-material/Download';
import Search from '@mui/icons-material/Search';
import { LoadingButton } from "@mui/lab";
import {
  Box,
  Card,
  Checkbox,
  FormControlLabel,
  Grid,
  InputLabel,
  TextField,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateCarrierReturnReport,
  GetActiveCarriersForSelection,
  GetOrderDetailForReturnReport,
  GetReturnReportSampleFile,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import CarrierReturnReportModal from "../../../components/modals/carrierReturnModals/CarrierReturnReportModal";
import StatusBadge from "../../../components/shared/statudBadge";
import { EnumOptions } from "../../../utilities/enum";
import {
  ActionButtonCustom,
  ActionButtonDelete,
  amountFormat,
  centerColumn,
  CodeBox,
  navbarHeight,
  placeholders,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

export default function UploadCarrierReturnReport(props) {
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);
  const [errorsList, setErrorsList] = useState([]);
  const [selectedData, setSelectedData] = useState();
  const [reportData, setReportData] = useState([]);
  const [allCarrierLookup, setallCarrierLookup] = useState([]);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [orderNo, setOrderNo] = useState("");
  const [carrierId, setCarrierId] = useState({
    CarrierId: 0,
    Name: "Select Please",
  });
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const {
    formState: { errors },
    control,
    register,
    handleSubmit,
    setValue,
    getValues,
    reset,
  } = useForm();
  useWatch({ name: "orderNo", control });
  useWatch({ name: "trackingNo", control });
  const downloadSample = async () => {
    let res = await GetReturnReportSampleFile();
    const link = document.createElement("a");
    link.href = res.data.result.url;
    link.download = "carrierReturnSample.xlsx";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };
  const getActiveCarriersForSelection = async () => {
    try {
      const response = await GetActiveCarriersForSelection();
      setallCarrierLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetallCarrierLookup:", error.response);
    }
  };
  const createCarrierReturnReport = async () => {
    let param = {
      carrierId: carrierId?.carrierId,
      trackingNos: reportData?.map((x) => x.OrderNo)?.join(","),
    };
    await CreateCarrierReturnReport(param)
      .then((res) => {
        console.log("res:::", res);
        if (!res?.data?.isSuccess) {
          for (let i = 0; i < res?.data?.errorCombined.length; i++) {
            errorNotification(res?.data?.errorCombined[i]);
          }

          if (res.data.errors?.AlreadyCreatedReturnReport) {
            errorNotification(res.data.errors?.AlreadyCreatedReturnReport[0]);
          } else {
            errorNotification("Unable to create return report");
            errorNotification(res?.data?.customErrorMessage);
          }
        } else {
          successNotification("Return report created successfully");
          setReportData([]);
        }
      })
      .catch((e) => {
        console.log("e", e);
        let msg = [];
        let count = 0;
        for (const key in e.response?.data?.errorCombined) {
          if (e.response?.data?.errorCombined.hasOwnProperty(key)) {
            const errorMessage = e.response?.data?.errorCombined[key];
            console.log(errorMessage);
            errorNotification(errorMessage);
            msg.push({
              Row: parseInt(key) + 1,
              Msg: errorMessage,
              IsSuccessed: false,
            });
            // Do something with the error message
          } else {
            msg.push({ Row: count, Msg: "errorMessage", IsSuccessed: true });
          }
          count++;
        }
        console.log("msgmsg", msg);
        setErrorsList(msg);

        errorNotification(
          LanguageReducer?.languageType?.UNABLE_TO_CREATE_PRODUCT_TOAST
        );
      });
  };
  useEffect(() => {
    getActiveCarriersForSelection();
  }, []);
  const handleActionButton = (cTarget, orderData) => {
    setAnchorEl(cTarget);
    setOrderNo(orderData.OrderNo);
  };
  const handleDelete = (orderData) => {
    const updatedItems = reportData.filter(
      (i) => i.OrderNo !== orderData.OrderNo
    );
    setReportData(updatedItems);
  };
  //#region
  const columns = [
    {
      field: "OrderNo",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_ORDER_NO
          }
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }} disableRipple>
            <>
              <CodeBox bold={"bold"} title={params.row.OrderNo} />
            </>
          </Box>
        );
      },
    },
    {
      field: "Store",

      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_STORE_INFO
          }
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Box>{params.row.StoreName}</Box>
              <Box>{params.row.CustomerServiceNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "DropOfAddress",

      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_DROP_OF_ADDRESS
          }
        </Box>
      ),
      minWidth: 250,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            // sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <Box sx={{ fontWeight: "bold" }}>{params.row.CustomerName}</Box>
              <Box sx={{ fontWeight: "bold" }}>{params.row.Mobile1}</Box>
              <Box noWrap={false}>{params.row.CustomerFullAddress}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Payment",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_PAYMENT_STATUS
          }
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <Box sx={{ fontWeight: "bold" }}>{params.row.PaymentMethod}</Box>
              <StatusBadge
                title={params.row.PaymentStatus}
                color={
                  params.row.PaymentStatus === "Unpaid" ? "#fff;" : "#fff;"
                }
                bgColor={
                  params.row.PaymentStatus === "Unpaid"
                    ? "#dc3545;"
                    : "#28a745;"
                }
              />
            </>
          </Box>
        );
      },
    },

    {
      field: "TrackingStatus",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_TRACKING_STATUS
          }
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row.TrackingStatus}
            color="#1E1E1E;"
            bgColor="#EAEAEA"
          />
        );
      },
    },
    {
      field: "OrderTypeName",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_ORDER_TYPE
          }
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row.OrderTypeName}
            color="#1E1E1E;"
            bgColor="#EAEAEA"
          />
        );
      },
    },
    {
      field: "ItemsCount",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_ITEM_COUNT
          }
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <LoadingButton>{params.row.ItemsCount}</LoadingButton>
          </Box>
        );
      },
    },
    {
      field: "orderDate",
      headerAlign: "center",
      align: "center",
      minWidth: 90,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_ORDER_DATE
          }
        </Box>
      ),
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
        </Box>
      ),
    },
    {
      field: "VAT",
      headerAlign: "center",
      align: "center",
      hide: true,
      headerName: <Box sx={{ fontWeight: "600" }}>{"VAT"}</Box>,
      minWidth: 40,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <Box sx={{ textAlign: "right" }}>
              {amountFormat(params?.row?.VAT)}
            </Box>
          </>
        );
      },
    },
    {
      field: "Discount",
      headerAlign: "center",
      align: "center",
      hide: true,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Discount"}</Box>,
      minWidth: 80,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <Box sx={{ textAlign: "right" }}>
              {amountFormat(params?.row?.Discount)}
            </Box>
          </>
        );
      },
    },
    {
      field: "Amount",
      ...rightColumn,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_AMOUNT
          }
        </Box>
      ),
      minWidth: 70,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <Box sx={{ textAlign: "right" }}>
              {amountFormat(params?.row?.Amount)}
            </Box>
          </>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_ACTION
          }
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box width={100}>
            {/* <IconButton
              onClick={(e) => handleActionButton(e.currentTarget, params.row)}
            >
              <MoreVertIcon />
            </IconButton> */}
            <ActionButtonDelete
              label=""
              onClick={(e) => handleDelete(params?.row)}
            />
          </Box>
        );
      },
      minWidth: 60,
      flex: 1,
    },
  ];
  //#endregion
  const getRowClassName = (params) => {
    for (let i = 0; i < errorsList.length; i++) {
      if (
        params.row.index == errorsList[i].Row &&
        errorsList[i].IsSuccessed === false
      )
        return "active-row"; // CSS class name for active rows
    }
    return "";
  };

  const handleValidateSingle = async (data) => {
    if (carrierId == 0) {
      errorNotification("Please select carrier");
      return;
    } else {
      let param = {
        CarrierId: carrierId?.carrierId,
        orderNo: data.orderNo,
        trackingNo: data.trackingNo,
      };
      await GetOrderDetailForReturnReport(param)
        .then((res) => {
          if (res?.data?.result && res?.data?.result?.length > 0) {
            let updatedItems = res.data.result[0];
            if (reportData.length > 0) {
              let existing = reportData.reduce(
                (max, shot) => {
                  return shot.index > max.index ? shot : max;
                },
                { index: 0 }
              );
              if (existing) {
                updatedItems.index = existing.index + 1;
              }
            } else {
              updatedItems.index = 1;
            }

            if (!reportData.find((x) => x.OrderNo == data.orderNo)) {
              setValue("orderNo", "");
              setValue("trackingNo", "");

              setReportData((prevItems) => [...prevItems, updatedItems]);
            } else {
              errorNotification("Order already added");
            }
          }
          if (res?.data?.errors?.FormatException) {
            console.log(
              "res?.data?.errors?.FormatException",
              res?.data?.errors?.FormatException
            );
            for (
              let j = 0;
              j < res?.data?.errors?.FormatException.length;
              j++
            ) {
              errorNotification(res?.data?.errors?.FormatException[j]);
            }
            return;
          }
          if (res?.data?.errors?.OrderNotFound) {
            for (let j = 0; j < res?.data?.errors?.OrderNotFound.length; j++) {
              errorNotification(res?.data?.errors?.OrderNotFound[j]);
            }
            return;
          }
          if (res?.data?.errors?.Exception) {
            for (let j = 0; j < res?.data?.errors?.Exception.length; j++) {
              errorNotification(res?.data?.errors?.Exception[j]);
            }
            return;
          }
          if (res?.data?.errors?.InvalidParameter) {
            const errorMessages = JSON.parse(
              res?.data?.errors?.InvalidParameter
            );
            console.log("errorMessages", errorMessages);
            setErrorsList(errorMessages);
            for (let index = 0; index < errorMessages.length; index++) {
              if (!errorMessages[index].IsSuccessed) {
                errorNotification(
                  `Error at row ${index}: ${errorMessages[index].Msg} `
                );
              }
            }
          }
        })
        .catch((e) => {
          errorNotification("Unable to get order");
        });
    }
  };
  const [selectedType, setSelectedType] = useState(1);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const optionTypeId = {
    UploadFile: 1,
    SingleCreate: 2,
  };
  const typeOptions = [
    {
      id: optionTypeId.UploadFile,
      label:
        LanguageReducer?.languageType
          ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_FIND_BY_FILE,
    },
    {
      id: optionTypeId.SingleCreate,
      label:
        LanguageReducer?.languageType
          ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_FIND_BY_NUMBER,
    },
  ];
  const handleCheckboxChange = (optionId) => {
    setSelectedType(optionId);
  };
  const handleImportOpenModel = () => {
    if (carrierId?.CarrierId == 0) {
      errorNotification("Plase select carrier");
      return;
    }
    setOpen(true);
  };

  const calculatedHeight = windowHeight - navbarHeight - 262;

  return (
    <Box sx={styleSheet.pageRoot}>
      {/* <Container maxWidth="xl" fixed sx={{ paddingLeft: "0px" }}> */}
      <div style={{ padding: "10px" }}>
        <Card sx={styleSheet.createOrderCard} variant="outlined">
          <Grid container spacing={2}>
            <Grid item md={4} sm={4} xs={4}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {
                  LanguageReducer?.languageType
                    ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_CARRIER
                }
              </InputLabel>
              <SelectComponent
                name="sms"
                options={allCarrierLookup}
                value={carrierId}
                optionLabel={EnumOptions.CARRIER.LABEL}
                optionValue={EnumOptions.CARRIER.VALUE}
                isRefesh={true}
                handleRefreshClick={getActiveCarriersForSelection}
                onChange={(e, newValue) => {
                  const resolvedId = newValue ? newValue : null;
                  setCarrierId(resolvedId);
                }}
              />
            </Grid>
            <Grid container item md={12} sm={12} xs={12} spacing={2}>
              {typeOptions.map((option) => (
                <Grid item>
                  <FormControlLabel
                    sx={{ margin: "0px" }}
                    control={
                      <Checkbox
                        sx={{
                          color: "var(--primary-color)",
                          "&.Mui-checked": {
                            color: "var(--primary-color)",
                          },
                        }}
                        checked={selectedType === option.id}
                        onChange={() => handleCheckboxChange(option.id)}
                        edge="start"
                        disableRipple
                        defaultChecked
                      />
                    }
                    label={option.label}
                  />
                </Grid>
              ))}
            </Grid>
          </Grid>
          {selectedType == optionTypeId.UploadFile ? (
            <Grid container spacing={2}>
              <Grid
                item
                md={12}
                sm={6}
                sx={{ display: "flex", marginTop: "0px" }}
                gap={1}
              >
                <ActionButtonCustom
                  variant="contained"
                  onClick={() => {
                    handleImportOpenModel(true);
                  }}
                  label={
                    LanguageReducer?.languageType
                      ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_IMPORT
                  }
                />
                <ActionButtonCustom
                  variant="contained"
                  onClick={() => {
                    downloadSample(1);
                  }}
                  startIcon={<Download />}
                  label={
                    LanguageReducer?.languageType
                      ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_SAMPLE_DOWNLOAD
                  }
                />
                {/* <Button
                  sx={{
                    ...styleSheet.orderButton,
                    marginLeft: "10px",
                    marginTop: "15px",
                  }}
                  variant="contained"
                  onClick={() => {
                    downloadSample(1);
                  }}
                >
                  {" "}
                  <FileDownloadOutlinedIcon />
                  {"Sample Download"}
                </Button> */}
              </Grid>
            </Grid>
          ) : (
            <Grid mt={0}>
              <form onSubmit={handleSubmit(handleValidateSingle)}>
                <Grid container spacing={2} sx={{ mt: "5px" }}>
                  <Grid item lg={3} md={4} sm={12} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {
                        LanguageReducer?.languageType
                          ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_ORDER_NO
                      }
                    </InputLabel>
                    <TextField
                      placeholder={placeholders.order_no}
                      size="small"
                      id="orderNo"
                      name="orderNo"
                      fullWidth
                      variant="outlined"
                      {...register("orderNo", {
                        required: {
                          value: true,
                          message:
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                        },
                        pattern: {
                          value: /^(?!\s*$).+/,
                          message:
                            LanguageReducer?.languageType
                              ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                        },
                      })}
                      error={Boolean(errors.orderNo)} // set error prop
                      helperText={errors.orderNo?.message}
                    />
                  </Grid>
                  <Grid item lg={3} md={4} sm={12} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {
                        LanguageReducer?.languageType
                          ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS_TRACKING_NO
                      }
                    </InputLabel>
                    <TextField
                      placeholder={placeholders.order_no}
                      size="small"
                      id="trackingNo"
                      name="trackingNo"
                      fullWidth
                      variant="outlined"
                      {...register("trackingNo", {
                        required: {
                          value: true,
                          message:
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                        },
                        pattern: {
                          value: /^(?!\s*$).+/,
                          message:
                            LanguageReducer?.languageType
                              ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                        },
                      })}
                      error={Boolean(errors.trackingNo)} // set error prop
                      helperText={errors.trackingNo?.message}
                    />
                  </Grid>
                  <Grid item lg={2} md={2} sm={12} xs={12}>
                    <LoadingButton
                      loading={false}
                      loadingPosition="start"
                      startIcon={<Search />}
                      variant="outlined"
                      sx={{
                        ...styleSheet.orderButton,
                        marginTop: "26px",
                      }}
                      type="submit"
                    >
                      Find
                    </LoadingButton>
                  </Grid>
                </Grid>
              </form>
            </Grid>
          )}
        </Card>
        <Card
          sx={{ ...styleSheet.createCarrierReturns, padding: "8px" }}
          variant="outlined"
        >
          <Grid display={"flex"} justifyContent={"right"}>
            <ActionButtonCustom
              variant="contained"
              onClick={() => {
                createCarrierReturnReport();
              }}
              disabled={reportData.length === 0 && true}
              label={"Create Return Report"}
              Report
            />
          </Grid>
        </Card>
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            height: calculatedHeight,
          }}
        >
          <DataGrid
            getRowHeight={() => "auto"}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
              "& .MuiDataGrid-footerContainer": {
                background: "#fff",
                BorderBottom: "1px none!important",
                borderRadius: "0px 0px 8px 8px",
              },
            }}
            getRowId={(row) => `${row.OrderNo}`}
            rows={reportData}
            columns={columns}
            disableSelectionOnClick
            pagination
            page={currentPage}
            pageSize={pageSize}
            rowsPerPageOptions={[5, 10, 15, 25]}
            paginationMode="client"
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
            getRowClassName={getRowClassName}
          />

          {open && (
            <CarrierReturnReportModal
              open={open}
              setCarrierId={setCarrierId}
              setOpen={setOpen}
              carrierId={carrierId?.carrierId}
              setReportData={setReportData}
              setErrorsList={setErrorsList}
            />
          )}
        </Box>
      </div>
    </Box>
  );
}
