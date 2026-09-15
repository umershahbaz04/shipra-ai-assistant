import { usePagination } from "@mui/lab";
import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import {
  ActionButtonEdit,
  centerColumn,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import { GetClientPayoutBank } from "../../../../api/AxiosInterceptors";
import CreateBankModal from "../../../../components/modals/settingsModals/CreateBankModal";

const PayoutBankList = (props) => {
  const { loading, allPayOutBank } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const calculatedHeight = windowHeight - 130;
  const [rowLoading, setRowLoading] = useState({});
  const [rowData, setRowData] = useState();
  const [updateBankModal, setUpdateBankModal] = useState();

  const handleEditBank = async (row) => {
    const rowNum = row.rowNum;
    setRowLoading((prev) => ({ ...prev, [rowNum]: true }));

    try {
      const response = await GetClientPayoutBank();
      if (response) {
        setRowData(response?.data?.result);
      }
      setUpdateBankModal(true);
    } finally {
      setRowLoading((prev) => ({ ...prev, [rowNum]: false }));
    }
  };

  const columns = [
    {
      field: "accountTitle",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_ACCOUNT_TITLE}
        </Box>
      ),
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.AccountTitle}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "BankName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_BANK_NAME}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{row.BankName}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "BranchName",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_BRANCH_NAME}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row.BranchName}</Box>;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "IBAN",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_IBAN}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row.IBAN}</Box>;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "SwiftCode",
      minWidth: 110,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_SWIFT_CODE}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row.SwiftCode}</Box>;
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
          <Box>
            <ActionButtonEdit
              onClick={() => handleEditBank(row)}
              loading={rowLoading[row.rowNum] || false}
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
          getRowId={(row) => row.rowNum}
          rows={allPayOutBank}
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
      {updateBankModal && (
        <CreateBankModal
          rowData={rowData}
          open={updateBankModal}
          onClose={() => setUpdateBankModal(false)}
        />
      )}
    </>
  );
};

export default PayoutBankList;
