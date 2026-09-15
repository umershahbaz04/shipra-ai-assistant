import EditIcon from "@mui/icons-material/Edit";
import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import GenerateInvoiceModal from "../../../../components/modals/ContractModals/GenerateInvoiceModal";
import {
  ActionButtonCustom,
  centerColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import {
  GetShipperInvoiceAdjustmentById,
  GetShipperInvoiceAdjustmentBySCId,
} from "../../../../api/AxiosInterceptors";
const GenerateInvoiceList = (props) => {
  const { loading, isFilterOpen, allGenerateInvoice } = props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;
  const [saleChannelConfigId, SetSaleChannelConfigId] = useState();
  const [generateInvoiceData, setGenerateInvoiceData] = useState({
    open: false,
    data: [],
    loading: {},
  });

  const handleEditGenerateInvoice = async (data) => {
    setGenerateInvoiceData((prev) => ({
      ...prev,
      loading: { ...prev.loading, [data?.SaleChannelConfigId]: true },
    }));
    try {
      const response = await GetShipperInvoiceAdjustmentBySCId(
        data?.SaleChannelConfigId
      );
      if (response?.data?.isSuccess) {
        SetSaleChannelConfigId(data?.SaleChannelConfigId);
        setGenerateInvoiceData((prev) => ({
          ...prev,
          data: response?.data?.result,
          open: true,
        }));
      }
    } catch (e) {
    } finally {
      setGenerateInvoiceData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data?.SaleChannelConfigId]: false },
      }));
    }
  };

  const columns = [
    {
      field: "EmployeeName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Employee Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.EmployeeName}</Box>
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
          <Box>
            <ActionButtonCustom
              onClick={() => handleEditGenerateInvoice(row)}
              loading={generateInvoiceData.loading[row.SaleChannelConfigId]}
              sx={styleSheet.editProductButton}
              label={<EditIcon />}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];
  return (
    <>
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: calculatedHeight,
        }}
      >
        <DataGrid
          loading={loading}
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row?.SaleChannelConfigId}
          rows={allGenerateInvoice || []}
          columns={columns}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 15, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
        />
      </Box>
      {generateInvoiceData.open && (
        <GenerateInvoiceModal
          open={generateInvoiceData.open}
          onClose={() =>
            setGenerateInvoiceData((prev) => ({ ...prev, open: false }))
          }
          generateInvoiceData={generateInvoiceData?.data}
          saleChannelConfigId={saleChannelConfigId}
        />
      )}
    </>
  );
};

export default GenerateInvoiceList;
