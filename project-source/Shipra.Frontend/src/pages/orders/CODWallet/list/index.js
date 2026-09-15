import { Box, Link, Typography } from "@mui/material";
import { default as React, useState } from "react";
import { useSelector } from "react-redux";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import { styleSheet } from "../../../../assets/styles/style";
import StatusBadge from "../../../../components/shared/statudBadge";
import { EnumChangeFilterModelApiUrls } from "../../../../utilities/enum";
import Colors from "../../../../utilities/helpers/Colors";
import {
  centerColumn,
  getTrimValue,
  ImageButton,
  navbarHeight,
  PDFButton,
  useGetWindowHeight,
  ViewButton,
} from "../../../../utilities/helpers/Helpers";
import {
  GetAllOrdersByPayoutId,
  GetAllPayoutFileByPayoutId,
  GetAllPayoutStatusHistory,
  GetPDFAllOrdersByPayoutId,
} from "../../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import PayoutHistoryModal from "../modals/PayoutHistoryModal";
import UtilityClass from "../../../../utilities/UtilityClass";
import OpenPayoutFileModal from "../modals/openPayoutFileModal";
import PayoutOrderModal from "../modals/payoutOrderModal";

const CODWalletList = (props) => {
  const { isFilterOpen, isLoading, allPayoutStatus } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [openHistoryModal, setOpenHistoryModal] = useState(false);
  const [openPayoutFileModal, setOpenPayoutFileModal] = useState(false);
  const [payoutStatusHistory, setPayoutStatusHistory] = useState([]);
  const [loadingMap, setLoadingMap] = useState({});
  const [loading, setLoading] = useState({});
  const [payoutFileData, setpayoutFileData] = useState([]);
  const [pdfLoading, setPdfLoading] = useState({});
  const [viewLoading, setViewLoading] = useState({});
  const [openPayoutOrderModal, setOpenPayoutOrderModal] = useState(false);
  const [payoutOrderData, setPayoutOrderData] = useState([]);
  const [payoutId, setpayoutId] = useState();
  const calculatedHeight = isFilterOpen
    ? windowHeight - 346
    : windowHeight - 260;

  const handlePayoutClick = async (data) => {
    const payoutId = data?.PayoutId;
    setLoadingMap((prev) => ({ ...prev, [payoutId]: true }));

    try {
      const response = await GetAllPayoutStatusHistory(payoutId);
      if (response) {
        setPayoutStatusHistory(response?.data?.result?.list);
        setOpenHistoryModal(true);
      }
    } catch {
      errorNotification("An error occurred while Fetching Payment Status");
    } finally {
      setLoadingMap((prev) => ({ ...prev, [payoutId]: false }));
    }
  };
  const statusColors = {
    pending: Colors.warning,
    completed: "#28a745",
    failed: "#dc3545",
    processing: Colors.warning,
  };

  const handleFileModal = async (data) => {
    setLoading((prev) => ({ ...prev, [data.PayoutId]: true }));
    try {
      const response = await GetAllPayoutFileByPayoutId(data.PayoutId);
      if (response?.data?.result.length > 0) {
        setpayoutFileData(response?.data?.result);
        setOpenPayoutFileModal(true);
      } else {
        errorNotification("No FIle Exist");
      }
    } catch (e) {
    } finally {
      setLoading((prev) => ({ ...prev, [data.PayoutId]: false }));
    }
  };
  const handleViewModal = async (id) => {
    setViewLoading((prev) => ({ ...prev, [id]: true }));
    try {
      const response = await GetAllOrdersByPayoutId(null, null, id);
      if (response?.data?.result?.list.length > 0) {
        setPayoutOrderData(response?.data?.result);
        setOpenPayoutOrderModal(true);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
    } finally {
      setViewLoading((prev) => ({ ...prev, [id]: false }));
    }
  };

  const hanldePDFDWonload = async (id) => {
    const body = {
      payoutId: id,
    };
    setPdfLoading((prev) => ({ ...prev, [id]: true }));
    await GetPDFAllOrdersByPayoutId(body)
      .then((res) => {
        if (res) {
          UtilityClass.downloadPdf(res.data, "PayoutOrderDetail");
        }
      })
      .catch((e) => {
        errorNotification("No FIle Exist");
      })
      .finally(() => {
        successNotification("Successfully Download");
        setPdfLoading((prev) => ({ ...prev, [id]: false }));
      });
  };

  const columns = [
    {
      field: "PayoutRef",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_PAYOUT_REF}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box sx={{ fontWeight: "bold" }}>
            {loadingMap[row.PayoutId] ? (
              <Typography variant="p" color="var(--primary-color)">
                Loading...
              </Typography>
            ) : (
              <Link
                component="button"
                onClick={() => handlePayoutClick(row)}
                underline="none"
                sx={{
                  color: "primary.main",
                  cursor: "pointer",
                  textDecoration: "underline",
                }}
              >
                {row.PayoutRef}
              </Link>
            )}
          </Box>
        );
      },
    },
    {
      field: "TransactionRef",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_TRANSACTION_REF}
        </Box>
      ),
      minWidth: 160,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box sx={{ fontWeight: "bold" }}>{row.TransactionRef}</Box>;
      },
    },
    {
      field: "CreatedOn",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_CREATED_ON}
        </Box>
      ),
      minWidth: 160,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box sx={{ fontWeight: "bold" }}>
            {UtilityClass.convertUtcToLocalAndGetDate(row.CreatedOn)}
          </Box>
        );
      },
    },
    {
      field: "AccountTitle",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_ACCOUNT_TITLE}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box sx={{ fontWeight: "bold" }}>{row.AccountTitle}</Box>;
      },
    },
    {
      field: "Bank Name",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_BANK_NAME}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box sx={{ fontWeight: "bold" }}>{row.BankName}</Box>;
      },
    },

    {
      field: "Paymentstatus",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_PAYMENT_STATUS}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        const bgColor =
          statusColors[getTrimValue(params.row.StatusName)] || "#f8f8f8";
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <StatusBadge
                title={params.row.StatusName}
                color={
                  getTrimValue(params.row.StatusName) === "pending"
                    ? "#fff;"
                    : "#fff;"
                }
                bgColor={bgColor}
              />
            </>
          </Box>
        );
      },
    },
    {
      field: "Currency",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_CURRENCY}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.Currency}</Box>;
      },
    },
    {
      field: "amount",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_AMOUNT}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.Amount?.toFixed(2)}</Box>;
      },
    },
    {
      field: "ServiceCharges",
      minWidth: 140,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_SERVICE_CHARGES}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.ServiceCharges?.toFixed(2)}</Box>;
      },
    },
    {
      field: "TransactionCharges",
      minWidth: 140,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_TRANSCTION_CHARGES}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.TransactionCharges?.toFixed(2)}</Box>;
      },
    },
    {
      field: "Outstanding",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_OUTSTANDING}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.Outstanding?.toFixed(2)}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "action",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_ACTION}
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box sx={{ display: "flex", gap: 0.5, fontWeight: "bold" }}>
            <ViewButton
              loading={viewLoading[row.PayoutId]}
              onClick={() => {
                handleViewModal(row.PayoutId);
                setpayoutId(row.PayoutId);
              }}
            />
            <ImageButton
              loading={loading[row.PayoutId]}
              onClick={() => handleFileModal(row)}
            />
            <PDFButton
              onClick={(e) => hanldePDFDWonload(row.PayoutId)}
              loading={pdfLoading[row.PayoutId]}
            />
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
          paddingBottom: "20px",
        }}
      >
        <DataGridProComponent
          rowPadding={8}
          rows={allPayoutStatus?.list ? allPayoutStatus?.list : []}
          columns={columns}
          loading={isLoading}
          headerHeight={40}
          getRowId={(row) => row.RowNum}
          checkboxSelection={false}
          disableSelectionOnClick
          rowsCount={allPayoutStatus?.TotalCount}
          paginationChangeMethod={allPayoutStatus}
          paginationMethodUrl={
            EnumChangeFilterModelApiUrls.GET_ALL_ORDER_PAYMENT_LINK.url
          }
          defaultRowsPerPage={
            EnumChangeFilterModelApiUrls.GET_ALL_ORDER_PAYMENT_LINK.length
          }
          height={calculatedHeight}
        />
      </Box>
      {openHistoryModal && (
        <PayoutHistoryModal
          open={openHistoryModal}
          onClose={() => setOpenHistoryModal(false)}
          payoutStatusHistory={payoutStatusHistory}
        />
      )}
      {openPayoutFileModal && (
        <OpenPayoutFileModal
          open={openPayoutFileModal}
          onClose={() => setOpenPayoutFileModal(false)}
          payoutFileData={payoutFileData}
        />
      )}
      {openPayoutOrderModal && (
        <PayoutOrderModal
          open={openPayoutOrderModal}
          onClose={() => setOpenPayoutOrderModal(false)}
          payoutOrderData={payoutOrderData}
          payoutId={payoutId}
        />
      )}
    </>
  );
};

export default CODWalletList;
