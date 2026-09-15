import { Box, Card, Grid, InputLabel } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateCarrierReturnReport,
  GetActiveCarriersForSelection,
  GetAllPendingForReturnShipment,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import StatusBadge from "../../../components/shared/statudBadge";
import { EnumOptions } from "../../../utilities/enum";
import {
  ActionButtonCustom,
  CodeBox,
  amountFormat,
  centerColumn,
  rightColumn,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

const PendingCarrierReturn = () => {
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [errorsList, setErrorsList] = useState([]);
  const [returnShipment, setReturnShipment] = useState([]);
  const [allCarrierLookup, setallCarrierLookup] = useState([]);
  const [carrierId, setCarrierId] = useState({
    CarrierId: 0,
    Name: "Select Please",
  });
  const getAllPendingForReturnShipment = async () => {
    try {
      const response = await GetAllPendingForReturnShipment();
      setReturnShipment(response.data.result || []);
    } catch (error) {
      console.error("Error fetching Pending for Return:", error.response);
    }
  };
  const getActiveCarriersForSelection = async () => {
    try {
      const response = await GetActiveCarriersForSelection();
      setallCarrierLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetallCarrierLookup:", error.response);
    }
  };
  useEffect(() => {
    getAllPendingForReturnShipment();
    getActiveCarriersForSelection();
  }, []);
  const getRowClassName = (params) => {
    for (let i = 0; i < errorsList.length; i++) {
      if (
        params.row.index == errorsList[i].Row &&
        errorsList[i].IsSuccessed === false
      )
        return "active-row";
    }
    return "";
  };
  const createCarrierReturnReport = async () => {
    let param = {
      carrierId: carrierId?.CarrierId,
      trackingNos: returnShipment?.list.map((x) => x.OrderNo)?.join(","),
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
          setReturnShipment([]);
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
  const columns = [
    {
      field: "OrderNo",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Order No."}</Box>,
      minWidth: 90,
      flex: 1,
      renderCell: (params) => (
        <Box sx={{ textAlign: "center" }} disableRipple>
          <CodeBox bold={"bold"} title={params.row.OrderNo} />
        </Box>
      ),
    },
    {
      field: "Customer",

      headerName: <Box sx={{ fontWeight: "600" }}>Customer Name</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Box>{params.row.Customer}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "CarrierName",

      headerName: <Box sx={{ fontWeight: "600" }}>Carrier Name</Box>,
      minWidth: 150,
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
              <Box sx={{ fontWeight: "bold" }}>{params.row.CarrierName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "PaymentMethodStatus",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>Payment Status</Box>,
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
              <StatusBadge title={params.row.PaymentMethodStatus} />
            </>
          </Box>
        );
      },
    },

    {
      field: "TrackingNo",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Tracking No."}</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <StatusBadge
            title={params.row.TrackingNo}
            color="#1E1E1E;"
            bgColor="#EAEAEA"
          />
        );
      },
    },
    {
      field: "CarrierTrackingStatus",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Tracking Status"}</Box>,
      minWidth: 100,
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
      field: "Remarks",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Remarks"}</Box>,
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return <StatusBadge title={params.row.Remarks} color="#1E1E1E;" />;
      },
    },
    {
      field: "Amount",
      ...rightColumn,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Amount"}</Box>,
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
          {LanguageReducer?.languageType?.ACTION}
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
            {/* <ActionButtonDelete
              label=""
              onClick={(e) => handleDelete(params?.row)}
            /> */}
          </Box>
        );
      },
      minWidth: 60,
      flex: 1,
    },
  ];

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <Card sx={styleSheet.createOrderCard} variant="outlined">
            <Grid container spacing={2}>
              <Grid item md={12} sm={12} xs={12}>
                <InputLabel required sx={styleSheet.inputLabel}>
                  {"Carrier"}
                </InputLabel>
                <SelectComponent
                  name="sms"
                  options={allCarrierLookup}
                  value={carrierId}
                  optionLabel={EnumOptions.CARRIER.LABEL}
                  optionValue={EnumOptions.CARRIER.VALUE}
                  onChange={(e, newValue) => {
                    const resolvedId = newValue ? newValue : null;
                    setCarrierId(resolvedId);
                  }}
                  size={"md"}
                />
              </Grid>
            </Grid>
          </Card>
          <Card sx={styleSheet.createOrderCard} variant="outlined">
            <Grid display={"flex"} justifyContent={"right"} mb={1}>
              <ActionButtonCustom
                variant="contained"
                onClick={() => {
                  createCarrierReturnReport();
                }}
                disabled={returnShipment.length === 0 && true}
                label={"Create Return Report"}
                Report
              />
            </Grid>
            <Box
              sx={{
                ...styleSheet.allOrderTable,
                "& .MuiDataGrid-root": {
                  borderRadius: "8px 8px 8px 8px !important",
                },
              }}
            >
              <DataGrid
                getRowHeight={() => "35px"}
                headerHeight={40}
                sx={{
                  fontFamily:
                    "'Lato Regular', 'Inter Regular', 'Arial' !important",
                  fontSize: "12px",
                  fontWeight: "500",
                }}
                getRowId={(row) => `${row.OrderNo}`}
                rows={
                  returnShipment.list && returnShipment.list.length > 0
                    ? returnShipment.list
                    : []
                }
                columns={columns}
                disableSelectionOnClick
                pagination
                checkboxSelection
                page={currentPage}
                pageSize={pageSize}
                rowsPerPageOptions={[5, 10, 15, 25]}
                paginationMode="client"
                onPageChange={handlePageChange}
                onPageSizeChange={handlePageSizeChange}
                getRowClassName={getRowClassName}
              />
            </Box>
          </Card>
        </div>
      </Box>
    </>
  );
};

export default PendingCarrierReturn;
