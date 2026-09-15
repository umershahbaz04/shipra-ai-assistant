import { Box, Typography } from "@mui/material";
import React, { useState } from "react";
import { Link } from "react-router-dom";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import { styleSheet } from "../../../../assets/styles/style";
import StatusBadge from "../../../../components/shared/statudBadge";
import { EnumChangeFilterModelApiUrls } from "../../../../utilities/enum";
import Colors from "../../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  amountFormat,
  centerColumn,
  CodeBox,
  DialerBox,
  rightColumn,
} from "../../../../utilities/helpers/Helpers";
import UtilityClass from "../../../../utilities/UtilityClass";
import { useSelector } from "react-redux";

const BillingHistoryList = (props) => {
  let { invoiceHistory, isLoading } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [anchorEl, setAnchorEl] = useState(null);
  const columns = [
    {
      field: "InvoiceNumber",
      minWidth: 160,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_INVOICE_NO}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <CodeBox title={row.invoiceNumber} />;
      },
    },
    {
      field: "BillToName",
      minWidth: 160,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_TO_NAME}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>
            <Typography variant="h6">{row.billToName}</Typography>
          </Box>
        );
      },
    },
    {
      field: "BillToMobile",
      minWidth: 160,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_TO_MOBILE}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <DialerBox phone={row.billToMobile} />;
      },
    },
    // {
    //   field: "BillFromName",
    //   minWidth: 160,
    //   flex: 1,
    //   headerName: <Box sx={{ fontWeight: "600" }}>{"From Name"}</Box>,
    //   renderCell: ({ row }) => {
    //     return (
    //       <Box>
    //         <Typography variant="h6">{row.billFromName}</Typography>
    //       </Box>
    //     );
    //   },
    // },
    // {
    //   field: "BillFromMobile",
    //   minWidth: 160,
    //   flex: 1,
    //   headerName: <Box sx={{ fontWeight: "600" }}>{"From Mobile"}</Box>,
    //   renderCell: ({ row }) => {
    //     return (
    //       <Box>
    //         <Typography variant="h6">{row.billFromMobile}</Typography>
    //       </Box>
    //     );
    //   },
    // },
    {
      field: "Amount",
      minWidth: 100,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_AMOUNT}
        </Box>
      ),
      ...rightColumn,
      renderCell: (row) => {
        return (
          <Box>
            <Typography variant="h6"> {amountFormat(row?.amount)}</Typography>
          </Box>
        );
      },
    },
    {
      field: "DueDate",
      minWidth: 160,
      flex: 1,
      ...centerColumn,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_DUE_DATE}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box>
            <Typography variant="h6">
              {UtilityClass.convertUtcToLocalAndGetDate(params.row.dueDate)}
            </Typography>
          </Box>
        );
      },
    },
    {
      field: "CreateDate",
      minWidth: 160,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_CREATED_ON}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box>
            <Typography variant="h6">
              {UtilityClass.convertUtcToLocalAndGetDate(params.row.createdOn)}
            </Typography>
          </Box>
        );
      },
    },

    {
      field: "Status",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_STATUS}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: ({ row }) => {
        const isPaid = row.invoiceStatusId === 1;
        return (
          <Box
            display={"flex"}
            gap={1}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
          >
            <Box>
              <StatusBadge
                title={isPaid ? "Paid" : "Due"}
                color={isPaid ? "#fff" : "#fff"}
                bgColor={isPaid ? Colors.succes : Colors.danger}
              />
            </Box>
          </Box>
        );
      },
    },
    {
      field: "Action",
      minWidth: 200,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTINGS_BILLING_HISTORY_ACTION}
        </Box>
      ),
      ...centerColumn,
      renderCell: ({ row }) => {
        const isDue = row.invoiceStatusId === 2;
        return (
          <Box className={"flex"} sx={{ gap: 1 }}>
            {isDue && (
              <Link to={row.hostInvoiceURL} target="_blank">
                <ActionButtonCustom
                  p={0}
                  label={"Make Payment"}
                  background={Colors.succes}
                />
              </Link>
            )}

            <Link to={row.invoicePDF} target="_blank">
              <ActionButtonCustom
                p={0}
                label={
                  LanguageReducer?.languageType
                    ?.SETTINGS_BILLING_HISTORY_VIEW_INVOICE
                }
                // background={Colors.succes}
              />
            </Link>
          </Box>
        );
      },
    },
  ];

  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: "auto",
        paddingBottom: "20px",
      }}
    >
      <DataGridProComponent
        rowPadding={8}
        rows={invoiceHistory}
        columns={columns}
        loading={isLoading}
        headerHeight={40}
        getRowId={(row) => row.rowNum}
        checkboxSelection={false}
        disableSelectionOnClick
        rowsCount={invoiceHistory.length}
        paginationChangeMethod={invoiceHistory}
        paginationMethodUrl={EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.url}
        defaultRowsPerPage={EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length}
      />
    </Box>
  );
};
export default BillingHistoryList;
