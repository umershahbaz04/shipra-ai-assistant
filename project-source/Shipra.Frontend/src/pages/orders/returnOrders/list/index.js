import { Box, Stack } from "@mui/material";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import OrderDetailModal from "../../../../.reUseableComponents/Modal/InfoModal";
import { GetShipmentInfoByOrderNo } from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import ReturnOrderModal from "../../../../components/modals/orderModals/ReturnOrderModal";
import {
  EnumChangeFilterModelApiUrls,
  EnumDetailsModalType,
  EnumReturnStatus,
} from "../../../../utilities/enum";
import {
  ActionButtonEdit,
  centerColumn,
  CodeBox,
  DialerBox,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";

const ReturnOrderList = (props) => {
  const { isLoading, allOrderReturnData, isFilterOpen, getAllOrderReturn } =
    props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { height: windowHeight } = useGetWindowHeight();
  const [openReturnOrderModal, setOpenReturnOrderModal] = useState({
    open: false,
  });
  const [isNoteAdded, setIsNoteAdded] = useState({
    loading: false,
    orderNo: "",
    isAdded: false,
  });
  const [rowData, setrowData] = useState();
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });

  const handleGetShipmentInfoByOrderNo = async (orderNum) => {
    setInfoModal((prev) => ({ ...prev, loading: { [orderNum]: true } }));
    await GetShipmentInfoByOrderNo(orderNum)
      .then((res) => {
        const response = res.data;
        setInfoModal((prev) => ({ ...prev, open: true, data: response }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setInfoModal((prev) => ({ ...prev, loading: { [orderNum]: false } }));
      });
  };

  const column = [
    {
      field: "OrderNo",
      headerName: (
        <Stack direction={"column"}>
          <Box sx={{ fontWeight: "bold" }}>
            {LanguageReducer?.languageType?.ORDER_ORDER_NUMBER}
          </Box>
        </Stack>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Stack sx={{ textAlign: "" }} direction={"column"}>
              <CodeBox
                title={
                  infoModal.loading[params.row.OrderNo]
                    ? "loading..."
                    : params.row.OrderNo
                }
                onClick={(e) => {
                  handleGetShipmentInfoByOrderNo(params.row.OrderNo);
                }}
                copyBtn
                sx={{
                  textDecoration: infoModal.loading[params.row.OrderNo]
                    ? "none"
                    : "underline",
                }}
              />
            </Stack>
          </Stack>
        );
      },
    },
    {
      field: "returnOrderNo",
      headerName: (
        <Stack direction={"column"}>
          <Box sx={{ fontWeight: "bold" }}>
            {LanguageReducer?.languageType?.ORDER_RETURN_ORDER_NUMBER}
          </Box>
        </Stack>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Stack sx={{ textAlign: "" }} direction={"column"}>
              <CodeBox
                title={
                  infoModal.loading[params.row.ReturnOrderNo]
                    ? "loading..."
                    : params.row.ReturnOrderNo
                }
                onClick={(e) => {
                  handleGetShipmentInfoByOrderNo(params.row.ReturnOrderNo);
                }}
                copyBtn
                sx={{
                  textDecoration: infoModal.loading[params.row.ReturnOrderNo]
                    ? "none"
                    : "underline",
                }}
              />
            </Stack>
          </Stack>
        );
      },
    },
    {
      field: "Name",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_CUSTOMER_NAME}
        </Box>
      ),
      minWidth: 130,
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
      field: "Store",
      // headerAlign: "center",
      // align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_STORE_INFO}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row.StoreName}</Box>
              <Box>
                <DialerBox phone={row.Mobile1} />
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "reason",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDER_RETURN_REASON}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontSize: "10px" }}>{row?.Reason}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "ReturnStatus",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDER_RETURN_STATUS}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: ({ row }) => {
        const status = Object.values(EnumReturnStatus).find(
          (status) => status.LABEL === row?.ReturnStatus
        );

        return (
          <Box
            sx={{
              fontSize: "11px",
              padding: "4px 8px",
              borderRadius: "4px",
              backgroundColor: status?.COLOR,
              color: "#fff",
              textAlign: "center",
            }}
          >
            {status?.LABEL}
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box display={"flex"} gap={1}>
            <ActionButtonEdit
              onClick={() => {
                setrowData(row);
                setOpenReturnOrderModal((prev) => ({ ...prev, open: true }));
              }}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];
  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 59 - 62.5 - 40
    : windowHeight - 95 - 59 - 16.5;
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
          rows={allOrderReturnData}
          columns={column}
          loading={isLoading}
          headerHeight={40}
          getRowId={(row) => row.RowNum}
          checkboxSelection={false}
          disableSelectionOnClick
          rowsCount={allOrderReturnData.length}
          paginationChangeMethod={allOrderReturnData}
          paginationMethodUrl={EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.url}
          defaultRowsPerPage={
            EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length
          }
          height={calculatedHeight}
        />
      </Box>
      {infoModal.data?.result && (
        <OrderDetailModal
          data={infoModal?.data?.result}
          open={infoModal.open}
          onClose={() => setInfoModal((prev) => ({ ...prev, open: false }))}
          isSendNotification={false}
          isUpdateOrderBtn={true}
          printCarrier={infoModal?.data?.result?.order?.CarrierTrackingNo}
          modelType={EnumDetailsModalType.ORDER}
          setIsNoteAdded={setIsNoteAdded}
          isNoteAdded={isNoteAdded}
        />
      )}
      {openReturnOrderModal.open && (
        <ReturnOrderModal
          rowData={rowData}
          getAllOrderReturn={getAllOrderReturn}
          open={openReturnOrderModal.open}
          onClose={() =>
            setOpenReturnOrderModal((prev) => ({ ...prev, open: false }))
          }
        />
      )}
    </>
  );
};
export default ReturnOrderList;
