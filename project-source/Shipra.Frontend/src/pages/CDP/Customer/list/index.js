import { useSelector } from "react-redux";
import {
  centerColumn,
  EyeIconLoadingButton,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  useNavigateSetState,
} from "../../../../utilities/helpers/Helpers";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import { EnumChangeFilterModelApiUrls } from "../../../../utilities/enum";
import { Box } from "@mui/material";
import { styleSheet } from "../../../../assets/styles/style";

const CDPCustomerList = (props) => {
  const { loading, isFilterOpen, allCustomers, getallCustomer } = props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { setNavigateState } = useNavigateSetState();
  const calculatedHeight = isFilterOpen
    ? windowHeight - 252
    : windowHeight - 167;

  const handleOpenCustomerDetail = (row) => {
    const url = `/cdp-customer-profile`;
    const customerData = {
      customer: row,
    };
    setNavigateState(url, {
      customerData,
    });
  };

  const columns = [
    {
      field: "CustomerName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Customer Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.CustomerName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Email",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Email"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.Email}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "mobile",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Mobile"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.Mobile}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "ltv",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"LTV"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.LTV}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "CountryCode",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Country Code"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.CountryCode}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Action"}</Box>,
      renderCell: ({ row }) => {
        return (
          <>
            <EyeIconLoadingButton
              onClick={() => handleOpenCustomerDetail(row)}
            />
          </>
        );
      },
      flex: 1,
    },
  ];

  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: calculatedHeight,
      }}
    >
      <DataGridProComponent
        rowPadding={4}
        loading={loading}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        getRowId={(row) => row.CustomerId}
        rows={allCustomers?.data || []}
        columns={columns}
        disableSelectionOnClick
        paginationChangeMethod={getallCustomer}
        paginationMethodUrl={
          EnumChangeFilterModelApiUrls.GET_ALL_CDP_CUSTOMER.url
        }
        defaultRowsPerPage={
          EnumChangeFilterModelApiUrls.GET_ALL_CDP_CUSTOMER.length
        }
        rowsCount={allCustomers.totalRecords || 0}
        height={calculatedHeight}
      />
    </Box>
  );
};

export default CDPCustomerList;
