import { Avatar, Box } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import { styleSheet } from "../../../../assets/styles/style";
import PriceCalculatorDrawer from "../../../../components/modals/orderModals/PriceCalculatorDrawer";
import { EnumChangeFilterModelApiUrls } from "../../../../utilities/enum";
import {
  ActionButtonCustom,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../../utilities/toast";

const PriceCalculatorList = (props) => {
  const { carrierData, carriersCount, isLoading, orderNo } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [rowData, setRowData] = useState({});
  const [loading, setLoading] = useState({});
  const [openPriceCalculatorDrawer, setOpenPriceCalculatorDrawer] =
    useState(false);
  const handleRowClick = async (row) => {
    if (!orderNo.length) {
      errorNotification("Please Enter Order No.");
      return;
    }
    setRowData(row);
    setOpenPriceCalculatorDrawer(true);
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
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            {row?.carrierImage && (
              <Avatar
                variant="rounded"
                src={row?.carrierImage}
                alt={row?.carrierName}
                sx={{
                  width: 35,
                  height: 35,
                  bgcolor: "#fff",
                  borderRadius: "8px",
                  p: 0.5,
                  "& img": {
                    objectFit: "contain !important",
                  },
                }}
              />
            )}
            <Box
              sx={{
                fontSize: "10px",
              }}
            >
              {row?.carrierName}
            </Box>
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
        return <Box>{row?.serviceName}</Box>;
      },
    },
    {
      field: "uomName",
      minWidth: 170,
      flex: 1,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Uom Name"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{row?.uomName}</Box>;
      },
    },
    {
      field: "rateTypeName",
      minWidth: 170,
      flex: 1,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Rate TypeName"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{row?.rateTypeName}</Box>;
      },
    },
    {
      field: "calculatedRate",
      minWidth: 170,
      flex: 1,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Rate"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{row?.calculatedRate?.toFixed(2)}</Box>;
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
              loading={loading[row.carrierRateId]}
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

  const calculatedHeightTable =
    orderNo.length === 0 ? windowHeight - 457 : windowHeight - 457;

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
          getRowId={(row) => row.clientRateId}
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
          searchText={orderNo}
          rowData={rowData}
        />
      )}
    </>
  );
};

export default PriceCalculatorList;
