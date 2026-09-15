import { usePagination } from "@mui/lab";
import { Box, Stack } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useSelector } from "react-redux";
import PdfExporter from "../../../../.reUseableComponents/TableDataPdfExporter/PdfExporter";
import { styleSheet } from "../../../../assets/styles/style";
import {
  CodeBox,
  DialerBox,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";

const StatusReportList = (props) => {
  const {
    loading,
    allOrderStatusReport,
    isFilterOpen,
    StatTusReportTableRef,
    startDateFormated,
  } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);

  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;

  const column = [
    {
      field: "OrderNo",
      headerName: (
        <Stack direction={"column"}>
          <Box sx={{ fontWeight: "bold" }}>{"Order No."}</Box>
        </Stack>
      ),
      exportLabel: "Order No.",
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Stack sx={{ textAlign: "" }} direction={"column"}>
              <CodeBox
                title={params.row.OrderNo}
                copyBtn
                sx={{
                  textDecoration: "underline",
                }}
              />
            </Stack>
          </Stack>
        );
      },
    },
    {
      field: "RefNo",
      headerName: (
        <Stack direction={"column"}>
          <Box sx={{ fontWeight: "bold" }}>{"Ref No."}</Box>
        </Stack>
      ),
      exportLabel: "Ref No.",
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Stack sx={{ textAlign: "" }} direction={"column"}>
              <CodeBox title={params.row.RefNo} />
            </Stack>
          </Stack>
        );
      },
    },
    {
      field: "OrderDate",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Order Date"}</Box>,
      exportLabel: "Order Date",
      minWidth: 130,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontSize: "10px" }}>{row?.OrderDate}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "CustomerName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_CUSTOMER_NAME}
        </Box>
      ),
      exportLabel: "Customer Name",
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "600" }}>{row?.CustomerName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "CustomerMobiles",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Customer Mobiles"}</Box>,
      exportLabel: "Customer Mobiles",
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>
                <DialerBox phone={row.CustomerMobiles} />
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "LastStatusWithDate",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Last Status"}</Box>,
      exportLabel: "Last Status",
      minWidth: 130,
      flex: 1,
      renderCell: ({ row }) => (
        <Box
          sx={{ fontSize: "10px", display: "flex", flexDirection: "column" }}
        >
          <span>{row?.LastStatus}</span>
          <span>{row?.LastStatusUpdatedDateTime}</span>
        </Box>
      ),
    },
    {
      field: "CurrentStatusWithDate",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Current Status"}</Box>,
      exportLabel: "Current Status",
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => (
        <Box
          sx={{ fontSize: "10px", display: "flex", flexDirection: "column" }}
        >
          <span>{row?.CurrentStatus}</span>
          <span>{row?.CurrentStatusUpdatedDateTime}</span>
        </Box>
      ),
    },
  ];
  const processedRows = allOrderStatusReport?.map((row) => ({
    ...row,
    LastStatusWithDate: `${row.LastStatus || ""}\n${
      row.LastStatusUpdatedDateTime || ""
    }`,
    CurrentStatusWithDate: `${row.CurrentStatus || ""}\n${
      row.CurrentStatusUpdatedDateTime || ""
    }`,
  }));

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
          getRowId={(row) => row.index}
          rows={allOrderStatusReport || []}
          columns={column}
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
      <PdfExporter
        ref={StatTusReportTableRef}
        columns={column}
        rows={processedRows}
        fileName="StatusReport.pdf"
        title="Status Report"
        // logoUrl={logoUrlForPDF}
        reportDate={startDateFormated}
      />
    </>
  );
};

export default StatusReportList;
