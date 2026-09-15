import EditIcon from "@mui/icons-material/Edit";
import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import {
  ExportShipperInvoiceDetailsToExcel,
  GetShipperInvoiceReportPdf,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UpdateShipperInvoiceStatusModal from "../../../../components/modals/ContractModals/UpdateShipperInvoiceStatusModal";
import UtilityClass from "../../../../utilities/UtilityClass";
import {
  ActionButtonCustom,
  centerColumn,
  ExcelButton,
  PDFButton,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../../utilities/toast";

const DInoviceList = (props) => {
  const { loading, isFilterOpen, allDInvoice, getAllShipperInvoice } = props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const [excelLoading, setExcelLoading] = useState({});
  const [pdfLoading, setPdfLoading] = useState({});
  const [editInoviceStatusData, setEditInoviceStatusData] = useState({
    open: false,
    data: {},
  });

  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;

  const downloadPDF = (id) => {
    const body = {
      ShipperInvoiceId: id,
    };

    setPdfLoading((prev) => ({
      ...prev,
      [id]: true,
    }));

    GetShipperInvoiceReportPdf(body)
      .then((res) => {
        UtilityClass.downloadPdf(res.data, "Invoice");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable To Download Pdf");
      })
      .finally(() => {
        setPdfLoading((prev) => ({
          ...prev,
          [id]: false,
        }));
      });
  };

  const downloadExcel = (id) => {
    setExcelLoading((prev) => ({ ...prev, [id]: true }));
    ExportShipperInvoiceDetailsToExcel(id)
      .then((res) => {
        UtilityClass.downloadExcel(res.data, "InvoiceDetails");
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      })
      .finally(() => {
        setExcelLoading((prev) => ({ ...prev, [id]: false }));
      });
  };

  const handleEditInvoiceStats = (row) => {
    setEditInoviceStatusData({
      open: true,
      data: row,
    });
  };

  const columns = [
    {
      field: "invoiceNo",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Invoice No"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.invoiceNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "statusName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Status Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.statusName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "refNo",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Ref No"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.refNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "totalOrder",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Total Order"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{(row?.totalOrder).toFixed(2)}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "amount",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Amount"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{(row?.amount).toFixed(2)}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "createdOn",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Created On"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>
                {UtilityClass.convertUtcToLocalAndGetDate(row?.createdOn)}
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "action",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Action"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box display={"flex"} alignItems={"center"} gap={1}>
                <PDFButton
                  loading={pdfLoading[row.shipperInvoiceId]}
                  onClick={() => downloadPDF(row?.shipperInvoiceId)}
                />
                <ExcelButton
                  loading={excelLoading[row?.shipperInvoiceId] || false}
                  onClick={() => {
                    downloadExcel(row.shipperInvoiceId);
                  }}
                />
                <ActionButtonCustom
                  onClick={() => handleEditInvoiceStats(row)}
                  sx={styleSheet.editProductButton}
                  label={<EditIcon />}
                />
              </Box>
            </>
          </Box>
        );
      },
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
          rows={allDInvoice || []}
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
      {editInoviceStatusData.open && (
        <UpdateShipperInvoiceStatusModal
          open={editInoviceStatusData.open}
          rowData={editInoviceStatusData.data}
          getAllShipperInvoice={getAllShipperInvoice}
          onClose={() =>
            setEditInoviceStatusData({
              open: false,
              data: {},
            })
          }
        />
      )}
    </>
  );
};

export default DInoviceList;
