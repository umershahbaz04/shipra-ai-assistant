import SyncIcon from "@mui/icons-material/Sync";
import { LoadingButton } from "@mui/lab";
import { Box, Button, CircularProgress, Link, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Tooltip } from "@mui/material";
import React, { useState } from "react";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import {
  DeleteOrderDraft,
  FetchPaybylinkbyServiceUUId,
  GetOrderDraftByDraftId,
  GetProductNameByOrderDraftId,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import StatusBadge from "../../../../components/shared/statudBadge";
import { EnumChangeFilterModelApiUrls } from "../../../../utilities/enum";
import {
  ActionButtonCustom,
  amountFormat,
  centerColumn,
  ClipboardIcon,
  CodeBox,
  DescriptionBoxWithChild,
  DialerBox,
  getTrimValue,
  useGetWindowHeight,
  useNavigateSetState,
} from "../../../../utilities/helpers/Helpers";
import { successNotification } from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";
import { useSelector } from "react-redux";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import OrderDrafItemDetailModal from "../../../../components/modals/orderModals/OrderDrafItemDetailModal";
import Colors from "../../../../utilities/helpers/Colors";

const DraftOrderList = (props) => {
  const { isFilterOpen, loading, allOrders, getAllOrderDrafts } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [orderData, setOrderData] = useState({
    data: [],
    loading,
  });
  const [orderIteminfoModal, setOrderIteminfoModal] = useState({
    open: false,
    loading: {},
    data: {},
    rowData: [],
  });
  const [openDelete, setOpenDelete] = useState(false);
  const [loadingStates, setLoadingStates] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const { setNavigateState } = useNavigateSetState();

  const handleDeleteConfirmation = (data) => {
    setOpenDelete(true);
    setDeleteItemObject(data);
  };

  const handleEditOrderDraft = async (data) => {
    const orderDraftId = data?.orderDraftId;
    try {
      setOrderData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [orderDraftId]: true },
      }));

      const response = await GetOrderDraftByDraftId(orderDraftId);
      if (response?.data?.isSuccess) {
        const result = response?.data.result;
        setOrderData((prev) => ({
          ...prev,
          data: result,
          loading: { ...prev.loading, [orderDraftId]: false },
        }));
        let address;
        if (result.orderTypeId === 1) {
          address = "/draft-regular-order";
        } else {
          address = "/draft-fullfillable-order";
        }
        setNavigateState(address, {
          result,
        });
      } else {
        setOrderData((prev) => ({
          ...prev,
          loading: { ...prev.loading, [orderDraftId]: false },
        }));
      }
    } catch (error) {
      console.error("Error fetching client order label:", error);
    } finally {
      setOrderData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [orderDraftId]: false },
      }));
    }
  };

  const handleDeleteOrderDraft = async () => {
    try {
      setLoadingStates(true);
      const response = await DeleteOrderDraft(deleteItemObject);
      console.log(response);
      if (response.data?.isSuccess) {
        getAllOrderDrafts();
        successNotification("Labels Delete successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoadingStates(false);
      setOpenDelete(false);
    }
  };

  const handleGetShipmentItemInfoByOrderNo = async (data) => {
    if (data?.orderTypeId === 1) {
      const orderItemsWithRowNum = data?.orderInfo?.OrderItems?.map(
        (item, index) => ({
          ...item,
          rowNum: index + 1,
        })
      );

      setOrderIteminfoModal((prev) => ({
        ...prev,
        open: true,
        data: data,
        rowData: orderItemsWithRowNum,
      }));
    } else {
      try {
        setOrderIteminfoModal((prev) => ({
          ...prev,
          loading: { [data?.orderDraftId]: true },
        }));
        const response = await GetProductNameByOrderDraftId(data?.orderDraftId);
        if (response?.data?.isSuccess) {
          const parsedata = JSON.parse(response?.data?.result?.orderInfo);
          const orderItemsWithRowNum = parsedata.OrderItems?.map(
            (item, index) => ({
              ...item,
              rowNum: index + 1,
            })
          );
          setOrderIteminfoModal((prev) => ({
            ...prev,
            open: true,
            data: data,
            rowData: orderItemsWithRowNum,
          }));
        }
      } catch (e) {
      } finally {
        setOrderIteminfoModal((prev) => ({
          ...prev,
          loading: { [data?.orderDraftId]: false },
        }));
      }
    }
  };

  const columns = [
    {
      field: "orderNo",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_ORDER_NO_REF_NO}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box sx={{ fontWeight: "bold" }}>
            {row.orderNo}
            {row?.orderInfo?.RefNo && (
              <CodeBox title={row?.orderInfo?.RefNo} color={Colors.purple} />
            )}
          </Box>
        );
      },
    },
    {
      field: "Store",
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
              <Box>{row?.storeName}</Box>
              <Box>
                <DialerBox phone={row?.orderInfo?.OrderAddress?.Mobile1} />
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "DropOfAddress",
      ...centerColumn,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_CUSTOMER_INFO}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box display={"flex"} flexDirection={"column"} disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row.orderInfo?.OrderAddress?.CustomerName}
                <DescriptionBoxWithChild>
                  <TableContainer>
                    <Table sx={{ minWidth: 275 }} aria-label="simple table">
                      <TableHead>
                        <TableRow>
                          <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                              // width: "110px",
                            }}
                            align="left"
                          >
                            Name
                          </TableCell>
                          <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                            }}
                          >
                            Mobile
                          </TableCell>
                          <TableCell
                            sx={{
                              fontWeight: "bold",
                              fontSize: "11px",
                              padding: "5px",
                              // width: "150px",
                            }}
                          >
                            Address
                          </TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        <TableRow
                          key={params.row.orderInfo?.CustomerName}
                          sx={{
                            "&:last-child td, &:last-child th": { border: 0 },
                          }}
                        >
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                            }}
                            align="left"
                          >
                            {params.row.orderInfo?.OrderAddress?.CustomerName}
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="right"
                          >
                            <DialerBox
                              phone={
                                params.row.orderInfo?.OrderAddress?.Mobile1
                              }
                            />
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              // width: "150px",
                            }}
                          >
                            {
                              params.row.orderInfo?.OrderAddress
                                ?.CustomerFullAddress
                            }
                          </TableCell>
                        </TableRow>
                      </TableBody>
                    </Table>
                  </TableContainer>
                </DescriptionBoxWithChild>
              </Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "CustomerAdress",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Customer Adress"}</Box>,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>{params.row.orderInfo?.OrderAddress?.CustomerFullAddress}</Box>
        );
      },
    },
    {
      field: "Payment",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_PAYMENT_STATUS}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row.orderInfo?.PaymentMethod}
              </Box>
              <StatusBadge
                title={
                  params.row.orderInfo?.PaymentStatusId === 1
                    ? "Unpaid"
                    : "Paid"
                }
                color={
                  params.row.orderInfo?.PaymentStatusId === 1
                    ? "#fff;"
                    : "#fff;"
                }
                bgColor={
                  params.row.orderInfo?.PaymentStatusId === 1
                    ? "#dc3545;"
                    : "#28a745;"
                }
              />
            </>
          </Box>
        );
      },
    },
    {
      field: "OrderTypeName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_ORDER_TYPE}
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      renderCell: (params) => {
        return (
          <Stack direction={"column"}>
            <StatusBadge
              title={params.row.orderTypeId === 1 ? "Regular" : "Fullfillable"}
              color="#1E1E1E;"
              bgColor="#EAEAEA"
            />
            <Box>
              {UtilityClass.convertUtcToLocalAndGetDate(params.row.createdOn)}
            </Box>
          </Stack>
        );
      },
    },
    {
      field: "ItemsCount",
      headerAlign: "center",
      align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_ITEM_COUNT}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <Box className={"flex_center"} flexDirection={"column"}>
              {orderIteminfoModal?.loading[row.orderDraftId] ? (
                <CircularProgress size={20} />
              ) : (
                <CodeBox
                  title={row.orderInfo?.OrderItems?.length ?? 0}
                  onClick={(e) => {
                    handleGetShipmentItemInfoByOrderNo(row);
                  }}
                  eyeBtn={true}
                />
              )}
            </Box>
          </>
        );
      },
    },
    {
      field: "Amount",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_AMOUNT}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      ...centerColumn,
      renderCell: ({ row }) => {
        return <Box>{amountFormat(row?.orderInfo?.Amount)}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 150,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Action"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box display={"flex"} gap={1} width="100%" justifyContent={"center"}>
            <Button
              sx={styleSheet.deleteProductButton}
              variant="outlined"
              onClick={() => handleDeleteConfirmation(row.orderDraftId)}
              aria-label={`Delete lable ${row.LabelName}`}
            >
              <DeleteIcon />
            </Button>
            <ActionButtonCustom
              onClick={() => handleEditOrderDraft(row)}
              loading={orderData.loading[row.orderDraftId]}
              sx={styleSheet.editProductButton}
              label={<EditIcon />}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];
  const calculatedHeightTable = isFilterOpen
    ? windowHeight - 95 - 59 - 62.5 - 40
    : windowHeight - 95 - 59 - 16.5;

  return (
    <>
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: calculatedHeightTable,
          paddingBottom: "20px",
        }}
      >
        <DataGridProComponent
          rowPadding={8}
          rows={allOrders}
          columns={columns}
          loading={loading}
          headerHeight={40}
          getRowId={(row) => row.orderDraftId}
          checkboxSelection={false}
          disableSelectionOnClick
          rowsCount={allOrders.length || 0}
          paginationChangeMethod={allOrders}
          paginationMethodUrl={
            EnumChangeFilterModelApiUrls.GET_ALL_ORDER_PAYMENT_LINK.url
          }
          defaultRowsPerPage={
            EnumChangeFilterModelApiUrls.GET_ALL_ORDER_PAYMENT_LINK.length
          }
          height={calculatedHeightTable}
        />
      </Box>
      <DeleteConfirmationModal
        open={openDelete}
        setOpen={setOpenDelete}
        loading={loadingStates}
        handleDelete={handleDeleteOrderDraft}
        heading={"Are you sure you want to delete this draft order"}
        message={
          "The selected order label will be permanently deleted. This action cannot be undone."
        }
        buttonText={"Delete"}
      />
      {orderIteminfoModal.data && (
        <OrderDrafItemDetailModal
          onClose={() =>
            setOrderIteminfoModal((prev) => ({ ...prev, open: false }))
          }
          data={orderIteminfoModal?.data}
          rowData={orderIteminfoModal?.rowData}
          open={orderIteminfoModal.open}
        />
      )}
    </>
  );
};

export default DraftOrderList;
