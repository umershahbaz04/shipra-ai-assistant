import { Box } from "@mui/material";
import React, { useState } from "react";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import { styleSheet } from "../../../../assets/styles/style";
import { EnumChangeFilterModelApiUrls } from "../../../../utilities/enum";
import {
  ActionButtonCustom,
  DataGridAvatar,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import PriceCalculatorDrawer from "../../../../components/modals/orderModals/PriceCalculatorDrawer";
import { GetShipraContractByShipraContractCarrierId } from "../../../../api/AxiosInterceptors";
import { errorNotification } from "../../../../utilities/toast";
import { useSelector } from "react-redux";

const PriceCalculatorList = (props) => {
  const { carrierData, carriersCount, isFilterOpen, isLoading, searchText } =
    props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [rowData, setRowData] = useState({});
  const [loading, setLoading] = useState({});
  const [openPriceCalculatorDrawer, setOpenPriceCalculatorDrawer] =
    useState(false);
  const handleRowClick = async (row) => {
    if (!searchText.length) {
      errorNotification("Please Enter Order No.");
      return;
    }
    try {
      setLoading((prev) => ({ ...prev, [row.ShipraContractCarrierId]: true }));
      const response = await GetShipraContractByShipraContractCarrierId(
        row.ShipraContractCarrierId
      );
      if (response) {
        setRowData(response?.data?.result);
      } else {
        errorNotification("An Error Occurred While Fetching Carrier Data");
      }
    } catch (error) {
      console.error("Error:", error);
    } finally {
      setOpenPriceCalculatorDrawer(true);
      setLoading((prev) => ({ ...prev, [row.ShipraContractCarrierId]: false }));
    }
  };
  const columns = [
    {
      field: "carrier",
      minWidth: 190,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_Carrier}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>
            <DataGridAvatar src={row.CarrierImage} name={row.CarrierName} />
          </Box>
        );
      },
    },
    {
      field: "ServiceType",
      minWidth: 170,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_SERVICE_TYPE}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.ServiceNames}</Box>;
      },
    },
    {
      field: "DeliveryTime",
      minWidth: 170,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_DELIVERY_TIME}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.DeliveryTime}</Box>;
      },
    },
    {
      field: "Pickup/Dropoff",
      minWidth: 170,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.ORDERS_PRICE_CALCULATOR_PICKUP_DROFOFF
          }
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.DropOffMethod}</Box>;
      },
    },
    {
      field: "DeliveryMethod",
      minWidth: 170,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.ORDERS_PRICE_CALCULATOR_DELIVERY_METHOD
          }
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.DeliveryMethod}</Box>;
      },
    },
    {
      field: "Price",
      minWidth: 170,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_PRICE}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{(row?.FlatRate).toFixed(2)}</Box>;
      },
    },
    {
      field: "Action",
      minWidth: 140,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <>
            <ActionButtonCustom
              onClick={() => handleRowClick(row)}
              sx={{
                ...styleSheet.integrationactivatedButton,
                width: "16%",
                height: "28px",
                borderRadius: "4px",
              }}
              loading={loading[row.ShipraContractCarrierId]}
              variant="contained"
              label={
                LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_SELECT
              }
            />
          </>
        );
      },
    },
  ];
  const calculatedHeightTable = isFilterOpen
    ? windowHeight - 169 - 88.5
    : windowHeight - 230;
  return (
    <>
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: calculatedHeightTable,
          paddingBottom: "20px",
        }}
      >
        <DataGridProComponent
          rowPadding={8}
          rows={carrierData}
          columns={columns}
          loading={isLoading}
          headerHeight={40}
          getRowId={(row) => row.RowNum}
          checkboxSelection={false}
          disableSelectionOnClick
          rowsCount={carriersCount}
          paginationChangeMethod={carrierData}
          paginationMethodUrl={EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.url}
          defaultRowsPerPage={
            EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length
          }
          height={calculatedHeightTable}
        />
      </Box>
      {openPriceCalculatorDrawer && (
        <PriceCalculatorDrawer
          open={openPriceCalculatorDrawer}
          onClose={() => setOpenPriceCalculatorDrawer(false)}
          searchText={searchText}
          rowData={rowData}
        />
      )}
    </>
  );
};

export default PriceCalculatorList;
