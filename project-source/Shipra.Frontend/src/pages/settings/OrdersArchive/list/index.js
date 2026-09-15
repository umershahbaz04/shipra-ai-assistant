import FileDownload from '@mui/icons-material/FileDownload';
import { usePagination } from "@mui/lab";
import { Box, Tooltip } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import {
  ExcelOrdersArchive,
  RestoreOrderArchive,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import {
  ActionButtonCustom,
  centerColumn,
  ExcelButton,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import UtilityClass from "../../../../utilities/UtilityClass";
import RotateLeftIcon from "@mui/icons-material/RotateLeft";
import { successNotification } from "../../../../utilities/toast";

const OrderArchiveList = (props) => {
  const { loading, archiveOrder, isFilterOpen, getAllArchiveOrders } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;
  const [rowData, setRowData] = useState({
    Data: {},
    loading: {},
  });
  const [restoreOrder, setRestoreOrder] = useState({
    Data: {},
    loading: {},
  });
  const handleDownloadOrderArchive = async (data) => {
    const rowNum = data?.rowNum;
    try {
      setRowData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [rowNum]: true },
      }));

      const response = await ExcelOrdersArchive(data?.ArchiveDate);
      if (response) {
        const result = response?.data;
        UtilityClass.downloadExcel(result, "OrdersArchive");
        setRowData((prev) => ({
          ...prev,
          loading: { ...prev.loading, [rowNum]: false },
        }));
      } else {
        setRowData((prev) => ({
          ...prev,
          loading: { ...prev.loading, [rowNum]: false },
        }));
      }
    } catch (error) {
      console.error("Error fetching order archive:", error);
    } finally {
      setRowData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [rowNum]: false },
      }));
    }
  };

  const handleRestoreOrderArchive = async (data) => {
    const rowNum = data?.rowNum;
    try {
      setRestoreOrder((prev) => ({
        ...prev,
        loading: { ...prev.loading, [rowNum]: true },
      }));

      const response = await RestoreOrderArchive(data?.ArchiveNo);
      if (response?.data?.isSuccess) {
        successNotification("Order restore to orderDashboard Successfully");
        getAllArchiveOrders();
        setRestoreOrder((prev) => ({
          ...prev,
          loading: { ...prev.loading, [rowNum]: false },
        }));
      } else {
        setRestoreOrder((prev) => ({
          ...prev,
          loading: { ...prev.loading, [rowNum]: false },
        }));
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (error) {
      console.error("Error Restoring order archive:", error);
    } finally {
      setRestoreOrder((prev) => ({
        ...prev,
        loading: { ...prev.loading, [rowNum]: false },
      }));
    }
  };

  const columns = [
    {
      field: "PaidOn",
      minWidth: 150,
      headerName: <Box sx={{ fontWeight: "600" }}>{"ArchiveDate"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box>
            {UtilityClass.convertUtcToLocalAndGetDate(row?.ArchiveDate)}
          </Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box sx={{ display: "flex", gap: 1 }}>
            <Tooltip title="Download Excel" placement="top">
              <span>
                <ExcelButton
                  onClick={() => handleDownloadOrderArchive(row)}
                  loading={rowData.loading[row.rowNum]}
                />
              </span>
            </Tooltip>

            <Tooltip title="Restore Order Archive" placement="top">
              <span>
                <ActionButtonCustom
                  onClick={() => handleRestoreOrderArchive(row)}
                  loading={restoreOrder.loading[row.rowNum]}
                  sx={styleSheet.restoreButton}
                  label={<RotateLeftIcon />}
                />
              </span>
            </Tooltip>
          </Box>
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
        rows={archiveOrder || []}
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
  );
};

export default OrderArchiveList;
