import { purple } from "@mui/material/colors";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { useEffect, useState } from "react";
import { DataGrid } from "@mui/x-data-grid";
import { styleSheet } from "../../../assets/styles/style";
import { usePagination } from "@mui/lab";
import { Box, InputLabel, TextField, Typography } from "@mui/material";
import {
  ActionButtonCustom,
  centerColumn,
  GreyBox,
} from "../../../utilities/helpers/Helpers";
import {
  CreateInvoice,
  GetShipperOrderForInvoices,
} from "../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const GenerateInvoiceModal = (props) => {
  let {
    open,
    onClose,
    generateInvoiceData,
    invoiceAdjustmentLoading,
    saleChannelConfigId,
  } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const [rates, setRates] = useState("");
  const [invoiceSelectionModel, setInvoiceSelectionModel] = useState([]);
  const [orderInvoiceLoading, setOrderInvoiceLoading] = useState([]);
  const [allOrderShipperInvoice, setAllOrderShipperInvoice] = useState([]);
  const [orderInvoiceSelectionModel, setOrderInvoiceSelectionModel] = useState(
    []
  );

  const handleInvoiceSelectedRow = (oNos) => {
    setInvoiceSelectionModel(oNos);
  };
  const handleOrderInvoiceSelectedRow = (oNos) => {
    setOrderInvoiceSelectionModel(oNos);
  };

  const handleRateChange = (orderNo, value) => {
    setAllOrderShipperInvoice((prev) =>
      prev.map((item) =>
        item.orderNo === orderNo
          ? {
              ...item,
              rate: value === "" ? null : Number(value),
            }
          : item
      )
    );
  };

  const hanldeCreateInovice = async () => {
    setIsLoading(true);

    try {
      const selectedOrders = allOrderShipperInvoice?.filter((order) =>
        orderInvoiceSelectionModel.includes(order.orderNo)
      );

      const invoiceAdjustmentdata = generateInvoiceData?.filter((invoice) =>
        invoiceSelectionModel.includes(invoice?.shipperInvoiceAdjustmentId)
      );

      if (!selectedOrders.length) {
        errorNotification("Please Select atleast 1 order");
        return;
      }

      const details = selectedOrders?.map((order) => ({
        OrderId: order?.orderId,
        OrderNo: order?.orderNo,
        Rate:
          order.deliveryCharges !== null && order?.deliveryCharges !== undefined
            ? order?.deliveryCharges
            : order?.rate ?? null,
      }));

      const invoiceAdjustmentIds = invoiceAdjustmentdata
        .map((invoice) => invoice?.shipperInvoiceAdjustmentId)
        .join(",");

      const body = {
        SaleChannelConfigId: selectedOrders[0]?.saleChannelConfigId,
        ClientId: selectedOrders[0]?.clientId,
        ShipperInvoiceAdjustmentIds: invoiceAdjustmentIds,
        Details: details,
      };
      console.log(body);
      const response = await CreateInvoice(body);

      if (response?.data?.isSuccess) {
        successNotification("Invoice Create Successfully");
        onClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setIsLoading(false);
    }
  };

  const getShipperOrderForInvoices = async () => {
    setOrderInvoiceLoading(true);
    try {
      const response = await GetShipperOrderForInvoices(
        null,
        null,
        saleChannelConfigId
      );
      if (response?.data?.isSuccess) {
        setAllOrderShipperInvoice(response?.data?.result);
      }
    } catch (e) {
    } finally {
      setOrderInvoiceLoading(false);
    }
  };

  const handleAddRates = () => {
    if (!rates || Number(rates) <= 0) {
      errorNotification("Please enter rate you want to set for order invoice");
      return;
    }

    const hasAnyRate = allOrderShipperInvoice.some(
      (item) =>
        item.deliveryCharges > 0 ||
        (item.rate !== null && item.rate !== undefined && item.rate !== "")
    );

    if (hasAnyRate) return;

    setAllOrderShipperInvoice((prev) =>
      prev.map((item) => ({
        ...item,
        rate: Number(rates),
      }))
    );
  };

  useEffect(() => {
    getShipperOrderForInvoices();
  }, []);

  const columns = [
    {
      field: "name",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Transaction Type"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        let transType = row?.transactionTypeId == 1 ? "Debit" : "Credit";
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{transType}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Amount",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Amount"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.amount}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "Commnet",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Comment"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.comment}</Box>
            </>
          </Box>
        );
      },
    },
  ];

  const orderInvoiceColumns = [
    {
      field: "orderNo",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Order No"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.orderNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "customerName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Customer Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.customerName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "storeName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Store Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.storeName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "invoiceStatus",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Invoice Status"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.invoiceStatus}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "carrierPayment",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Carrier Payment"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.carrierPayment}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Weight",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Weight"}</Box>,
      minWidth: 110,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.weight}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "rate",
      headerName: <Box sx={{ fontWeight: "bold" }}>Rate</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        const displayValue =
          row.deliveryCharges > 0 ? row.deliveryCharges : row.rate ?? "";

        return (
          <Box sx={{ padding: "5px" }}>
            <TextField
              size="small"
              type="number"
              value={displayValue}
              sx={{
                "& .MuiInputBase-root": { height: "30px", background: "#fff" },
              }}
              onChange={(e) => handleRateChange(row.orderNo, e.target.value)}
            />
          </Box>
        );
      },
    },
  ];

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title={"Generate Invoice"}
      actionBtn={
        <ModalButtonComponent
          title={"Create invoice"}
          loading={isLoading}
          bg={purple}
          onClick={hanldeCreateInovice}
        />
      }
    >
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: 200,
        }}
      >
        <DataGrid
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row?.shipperInvoiceAdjustmentId}
          rows={generateInvoiceData || []}
          columns={columns}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 15, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          checkboxSelection
          selectionModel={invoiceSelectionModel}
          onSelectionModelChange={(oNo) => handleInvoiceSelectedRow(oNo)}
        />
      </Box>
      <Box sx={{ my: 2 }}>
        <GreyBox>
          <Typography variant="h4">Add Rates</Typography>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 1,
            }}
          >
            <Box sx={{ width: "100%", minWidth: "600px" }}>
              <InputLabel sx={{ ...styleSheet.inputLabel }}>
                {"Rates"}
              </InputLabel>
              <TextField
                fullWidth
                size="small"
                placeholder="10"
                value={rates}
                onChange={(e) => setRates(e.target.value)}
                sx={{
                  "& .MuiInputBase-root": {
                    height: "28px",
                    background: "#fff",
                  },
                }}
              />
            </Box>
            <Box sx={{ alignSelf: "end" }}>
              <ActionButtonCustom
                label={"Add Rates"}
                onClick={handleAddRates}
              />
            </Box>
          </Box>
        </GreyBox>
      </Box>
      <Box>
        <Typography sx={{ py: "8px" }} variant="h3">
          Orders Invoice
        </Typography>
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            height: 500,
          }}
        >
          <DataGrid
            rowHeight={40}
            headerHeight={40}
            loading={orderInvoiceLoading}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row.orderNo}
            rows={allOrderShipperInvoice || []}
            columns={orderInvoiceColumns}
            disableSelectionOnClick
            pagination
            page={currentPage}
            pageSize={pageSize}
            rowsPerPageOptions={[5, 10, 15, 25]}
            paginationMode="client"
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
            checkboxSelection
            selectionModel={orderInvoiceSelectionModel}
            onSelectionModelChange={(oNo) => handleOrderInvoiceSelectedRow(oNo)}
          />
        </Box>
      </Box>
    </ModalComponent>
  );
};

export default GenerateInvoiceModal;
