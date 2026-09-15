import { usePagination } from "@mui/lab";
import { Box, Stack } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import {
  navbarHeight,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../../utilities/toast";

const UploadLeadsList = (props) => {
  const {
    loading,
    uploadLeadsData,
    setUploadLeadsData,
    selectionModel,
    setSelectionModel,
    allCountries,
  } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const calculatedHeight = windowHeight - navbarHeight - 194;
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);

  const handleSelectedRow = (selectedRowNums) => {
    if (!selectedRowNums?.length) {
      setSelectionModel([]);
      return;
    }

    const validRows = selectedRowNums.filter((rowNum) => {
      const row = uploadLeadsData.find((r) => r.rowNum === rowNum);
      return row && !row.hasError;
    });

    const attemptedErrorRows = selectedRowNums.filter((rowNum) => {
      const row = uploadLeadsData.find((r) => r.rowNum === rowNum);
      return row && row.hasError;
    });

    setSelectionModel(validRows);

    attemptedErrorRows.forEach((rowNum) => {
      const row = uploadLeadsData.find((r) => r.rowNum === rowNum);
      errorNotification(`Row ${row.rowNum}: ${row.errorMsg}`);
    });
  };

  const columns = [
    {
      field: "PhoneNumber",
      headerName: <Box sx={{ fontWeight: "600" }}>Phone Number</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <Box disableRipple>
          <Stack direction={"row"} sx={{ alignItems: "center" }}>
            <span style={{ ...styleSheet.tableText, color: "var(--primary-color)" }}>
              {params.row.phoneNumber}
            </span>
          </Stack>
        </Box>
      ),
    },
    {
      field: "ProductName",
      headerName: <Box sx={{ fontWeight: "600" }}>Product Name</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.productName}</span>
      ),
    },
    {
      field: "CustomerName",
      headerName: <Box sx={{ fontWeight: "600" }}>Customer Name</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.customerName}</span>
      ),
    },
    {
      field: "Address",
      headerName: <Box sx={{ fontWeight: "600" }}>Address</Box>,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.address}</span>
      ),
    },
    {
      field: "Amount",
      headerName: <Box sx={{ fontWeight: "600" }}>Amount</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.amount}</span>
      ),
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
          getRowId={(row) => row.rowNum}
          rows={uploadLeadsData || []}
          columns={columns}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          checkboxSelection={true}
          selectionModel={selectionModel}
          onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
          getRowClassName={({ row }) => (row?.hasError ? "active-row" : "")}
          height={calculatedHeight}
        />
      </Box>
    </>
  );
};

export default UploadLeadsList;
