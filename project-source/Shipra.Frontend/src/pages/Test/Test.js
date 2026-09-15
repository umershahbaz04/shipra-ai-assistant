import {
  Box,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
// import OrderItemDetailModal from "../../../.reUseableComponents/Modal/InfoModal";
import { useSelector } from "react-redux";
import DataGridProComponent from "../../.reUseableComponents/DataGrid/DataGridProComponent";
import { Axios } from "../../api/AxiosInterceptors";
import StatusBadge from "../../components/shared/statudBadge";
import UtilityClass from "../../utilities/UtilityClass";
import { EnumFullFillmentStatus } from "../../utilities/enum";
import Colors from "../../utilities/helpers/Colors";
import {
  CodeBox,
  DescriptionBoxWithChild,
  DialerBox,
  amountFormat,
  fetchMethod,
  rightColumn,
} from "../../utilities/helpers/Helpers";

export default function Test() {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [orders, setOrders] = useState([]);
  const columns = [
    {
      field: "OrderNo",
      headerName: (
        <Stack direction={"column"}>
          <Box sx={{ fontWeight: "bold" }}>{"Order No"} / Ref No.</Box>
        </Stack>
      ),
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            {/* {infoModal.loading[params.row.OrderNo] ? (
              <CircularProgress size={20} />
            ) : ( */}
            <Stack sx={{ textAlign: "" }} direction={"column"}>
              <CodeBox title={params.row.OrderNo} copyBtn />
              {params?.row?.RefNo && (
                <CodeBox title={params?.row?.RefNo} color={Colors.purple} />
              )}
              <Box
                sx={{
                  color: `${!params.row?.CarrierId ? Colors.danger : ""}`,
                  fontSize: "10px",
                }}
              ></Box>
              {!params?.row?.RefNo && (
                <Box sx={{ opacity: "0" }}>
                  <CodeBox title={"..."} color={Colors.purple} />
                </Box>
              )}
            </Stack>
            {/* )} */}
          </Stack>
        );
      },
    },
    {
      field: "Store",
      // headerAlign: "center",
      // align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>Store Info.</Box>,
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Box>{params.row.StoreName}</Box>
              <Box>
                <DialerBox phone={params.row.CustomerServiceNo} />
              </Box>
              <Box sx={{ fontSize: "10px" }}>{params.row?.SaleChannelName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "DropOfAddress",
      // headerAlign: "center",
      // align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>Customer Info.</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box display={"flex"} flexDirection={"column"} disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row.CustomerName}
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
                              // width: "150px",
                            }}
                          >
                            Address
                          </TableCell>
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
                            {params.row.CustomerName}
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="right"
                          >
                            <DialerBox phone={params.row.Mobile1} />
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              // width: "150px",
                            }}
                          >
                            {params.row.CustomerFullAddress}
                          </TableCell>
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
      field: "Payment",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>Payment Status</Box>,
      minWidth: 120,
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
      field: "FulfillmentStatus",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>Fulfillment Status</Box>,
      minWidth: 120,
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
              {params.row.OrderTypeName != "Regular" && (
                <StatusBadge
                  title={params.row.FullFillmentStatus}
                  color={"#fff;"}
                  bgColor={
                    params.row?.FullFillmentStatusId ==
                    EnumFullFillmentStatus.UnFulfillment
                      ? "#dc3545;"
                      : "#28a745;"
                  }
                />
              )}
            </>
            <Box>
              {UtilityClass.convertUtcToLocalAndGetDate(
                params.row?.FulFilledDate
              )}
            </Box>
          </Box>
        );
      },
    },
    {
      field: "OrderTypeName",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Order Type"}</Box>,
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack direction={"column"}>
            <StatusBadge
              title={params.row.OrderTypeName}
              color="#1E1E1E;"
              bgColor="#EAEAEA"
            />
            <Box>
              {UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}
            </Box>
          </Stack>
        );
      },
    },
    {
      field: "TrackingStatus",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Tracking Status"}</Box>,
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box className={"flex_center"} py={1}>
            <Box display="flex" flexDirection="column">
              <StatusBadge
                title={row.TrackingStatus}
                color="#1E1E1E;"
                bgColor="#EAEAEA"
                showBtn={!row?.IsClientCarrier && row?.CarrierId != null}
              />
              {row?.CarrierLastUpdateDateTime && (
                <Box sx={{ textAlign: "center", display: "inline" }}>
                  <Typography fontSize="10px">
                    {UtilityClass.convertUtcToLocalAndGetTime(
                      row?.CarrierLastUpdateDateTime
                    )}
                  </Typography>
                </Box>
              )}
            </Box>
          </Box>
        );
      },
    },
    {
      field: "ItemsCount",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Item Count"}</Box>,
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <Box className={"flex_center"} flexDirection={"column"}>
              <CodeBox title={params.row.ItemsCount} eyeBtn={true} />
            </Box>
          </>
        );
      },
    },

    {
      field: "VAT",
      headerAlign: "center",
      align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{" VAT"}</Box>,
      minWidth: 60,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <Box sx={{ textAlign: "center" }}>{amountFormat(row.VAT)} </Box>{" "}
          </>
        );
      },
    },
    {
      field: "Discount",
      ...rightColumn,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Discount"}</Box>,
      minWidth: 80,
      flex: 1,
      renderCell: ({ row }) => {
        return <>{amountFormat(row.Discount)}</>;
      },
    },
    {
      field: "Amount",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Amount"}</Box>,
      minWidth: 120,
      flex: 1,
      ...rightColumn,
      renderCell: (params) => {
        return <Box>{amountFormat(params.row.Amount)}</Box>;
      },
    },
  ];

  const handleGetAllOrders = async () => {
    const body = {
      filterModel: {
        createdFrom: null,
        createdTo: null,
        start: 0,
        length: 1000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      storeId: "",
      orderTypeId: 0,
      fullFillmentStatusId: 0,
      paymentStatusId: 0,
      orderRequestVia: 0,
      paymentMethodId: 0,
      stationId: 0,
      carrierAssign: 0,
      salePersonIds: "",
      countryIds: "",
      regionIds: "",
      cityIds: "",
      carrierTrackingStatusIds: "",
      IncludeRegion: true,
      IncludeCity: true,
    };
    const { response } = await fetchMethod(() =>
      Axios.post(`/Order/GetAllOrders`, body, {
        headers: {
          "Content-Type": "application/json",
        },
      })
    );
    setOrders(response.result?.list);
  };

  useEffect(() => {
    handleGetAllOrders();
  }, []);
  return (
    <div>
      <DataGridProComponent
        columns={columns}
        rows={orders}
        getRowId={(row) => row.OrderNo}
      />
    </div>
  );
}
