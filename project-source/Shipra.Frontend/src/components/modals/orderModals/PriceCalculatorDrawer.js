import { Box, Card, Divider, Grid, Typography } from "@mui/material";
import React, { useState } from "react";
import DrawerComponent from "../../../.reUseableComponents/Modal/DrawerComponent";
import {
  AssignToCarrier,
  GetCarrierWayBillsByOrderNos,
} from "../../../api/AxiosInterceptors";
import { EnumAwbType, EnumCarrierContractType } from "../../../utilities/enum";
import { successNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent";
import {
  CodeBox,
  DataGridHeaderBox,
  fetchMethod,
} from "../../../utilities/helpers/Helpers";

const PriceCalculatorDrawer = (props) => {debugger
  const { rowData, open, onClose, searchText } = props;
  const [loading, setLoading] = useState(false);
  const [configErrors, setConfigErrors] = useState({
    rows: [],
    data: {},
  });
  const [errors, setErrors] = useState({
    rows: [],
    data: {},
  });
  const handlePayoutRequest = async () => {
    const orderList = [{
      OrderNo:searchText,
      ActiveCarrierPickupLocationId: 0,
    }]
    const param = {
      carrierId: rowData?.carrierId,
      ActiveCarrierId: 0,
      orderNos: searchText,
      orderList: orderList,
      ShipraContactType: rowData?.shipraContractCarrierId,
      CarrierContractTypeId: EnumCarrierContractType?.ShipraContactType,
      DeliveryCharges :rowData?.calculatedRate
    };
    console.log(param);
    try {
      setLoading(true);
      const res = await AssignToCarrier(param);
      if (!res?.data?.isSuccess) {
        if (!res.data.isSuccess && res.data.errors) {
          const _erros = res.data.errors;
          const _rows = Object.keys(_erros).map((data) => {
            return { orderNo: data };
          });
          setErrors((prev) => ({ ...prev, data: _erros, rows: _rows }));
        }
        if (!res.data.isSuccess && res.data.configErrors) {
          const _configError = res.data.configErrors;
          const _configRow = Object.keys(_configError).map((data) => {
            return { orderNo: data };
          });
          setConfigErrors((prev) => ({
            ...prev,
            data: _configError,
            rows: _configRow,
          }));
        }
        UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
      } else {
        const { response } = await fetchMethod(() =>
          GetCarrierWayBillsByOrderNos({
            orderNos: searchText.join(","),
            awbTypeId: EnumAwbType.CarrierAwbTypeId,
          })
        );
        if (response) {
          UtilityClass.downloadPdf(response, "Awb");
        }
        successNotification("Carrier assigned successfully!");
        onClose();
      }
    } catch (error) {
    } finally {
      setLoading(false);
    }
  };

  const configColumns = [
    {
      field: "OrderNo",
      headerName: <DataGridHeaderBox title={"Title"} />,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.orderNo} />
          </>
        );
      },
    },
    {
      field: "Errors",
      headerName: <DataGridHeaderBox title={"Errors"} />,
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        const errorsForOrder = configErrors.data[row.orderNo] || [];
        return (
          <>
            {errorsForOrder.map((dt, index) => (
              <Box
                key={index}
                style={{
                  padding: "4px 0",
                  whiteSpace: "normal",
                  wordBreak: "break-word",
                }}
              >
                {index + 1}. {dt}
              </Box>
            ))}
          </>
        );
      },
    },
  ];
  const columns = [
    {
      field: "OrderNo",
      headerName: <DataGridHeaderBox title={"Title"} />,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.orderNo} />
          </>
        );
      },
    },
    {
      field: "Errors",
      headerName: <DataGridHeaderBox title={"Errors"} />,
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        const errorsForOrder = errors.data[row.orderNo] || [];
        return (
          <>
            {errorsForOrder.map((dt, index) => (
              <Box
                key={index}
                style={{
                  padding: "4px 0",
                  whiteSpace: "normal",
                  wordBreak: "break-word",
                }}
              >
                {index + 1}. {dt}
              </Box>
            ))}
          </>
        );
      },
    },
  ];
  return (
    <DrawerComponent
      open={open}
      onClose={onClose}
      anchor="right"
      title={"Shipping Detail"}
      onClick={handlePayoutRequest}
      loading={loading}
    >
      <Card
        sx={{
          backgroundColor: "#f8f8f8",
          color: "#fff",
          borderRadius: "12px",
          padding: 2,
        }}
      >
        <Grid container spacing={2} alignItems="center">
          <Grid item>
            <Box
              sx={{
                width: 150,
                height: 100,
                borderRadius: "10px",
                overflow: "hidden",
              }}
              className={"flex_center"}
            >
              <Box
                component={"img"}
                src={rowData?.carrierImage}
                sx={{ maxWidth: "100%", maxHeight: "100%" }}
              />
            </Box>
          </Grid>
          <Grid item xs>
            <Typography variant="h6" fontWeight="bold" color="gray">
              {rowData?.carrierName}
            </Typography>
            <Typography variant="body2" color="gray">
              {rowData?.CarrierWebsite}
            </Typography>
          </Grid>
          <Grid item>
            <Typography variant="h6" fontWeight="bold" color="gray">
              {rowData?.rate?.toFixed(2)}
            </Typography>
          </Grid>
        </Grid>
      </Card>
      {/* Dynamic section rendering */}

      {errors.rows.length > 0 && (
        <Box marginTop={"10px"}>
          <DataGridComponent
            autoHeight
            bgColor={"#fff"}
            getRowHeight={() => "auto"}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            rows={errors.rows}
            getRowId={(row) => row.orderNo}
            columns={columns}
            checkboxSelection={false}
            disableSelectionOnClick
            pageSize={10}
            rowsPerPageOptions={[5, 10, 15, 25]}
          />
        </Box>
      )}
      {configErrors.rows.length > 0 && (
        <Box marginTop={"10px"}>
          <DataGridComponent
            autoHeight
            bgColor={"#fff"}
            getRowHeight={() => "auto"}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            rows={configErrors.rows}
            getRowId={(row) => row.orderNo}
            columns={configColumns}
            checkboxSelection={false}
            disableSelectionOnClick
            pageSize={10}
            rowsPerPageOptions={[5, 10, 15, 25]}
          />
        </Box>
      )}
    </DrawerComponent>
  );
};

export default PriceCalculatorDrawer;
