import FileUploadOutlined from '@mui/icons-material/FileUploadOutlined';
import DisplaySettingsIcon from "@mui/icons-material/DisplaySettings";
import FileDownloadOutlinedIcon from "@mui/icons-material/FileDownloadOutlined";
import { LoadingButton } from "@mui/lab";
import {
  Box,
  Card,
  Grid,
  InputLabel,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import DataGridProComponent from "../../../.reUseableComponents/DataGrid/DataGridProComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateOrder,
  ExcelExportAddressEntitiesByType,
  GetOrderFileByOrderTypeId,
  GetProductStocksForSelection,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { getAllCountryFunc } from "../../../apiCallingFunction";
import { styleSheet } from "../../../assets/styles/style";
import EditFullfilableOrderDetailsModal from "../../../components/modals/orderModals/EditFullfilableOrderDetailsModal";
import EditRegularOrderDetailsModal from "../../../components/modals/orderModals/EditRegularOrderDetailsModal";
import PlaceOrderModal from "../../../components/modals/orderModals/PlaceOrderModal";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions, EnumOrderType } from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  ActionButtonEdit,
  DescriptionBoxWithChild,
  GridItem,
  StyledTooltip,
  amountFormat,
  centerColumn,
  dataGridProFooterHeight,
  fetchMethod,
  navbarHeight,
  placeholders,
  truncate,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import { useGetAddressSchema } from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { DataGrid } from "@mui/x-data-grid";

function UploadOrders(props) {
  const [anchorEl, setAnchorEl] = useState(null);
  const [open, setOpen] = useState(false);
  const [editOpen, setEditOpen] = useState(false);
  const [selectedData, setSelectedData] = useState();
  const [orderData, setOrderData] = useState([]);
  const [orderType, setOrderType] = useState();
  const [selectedRowIndex, setSelectedRowIndex] = useState();
  const [placingOrder, setPlacingOrder] = useState(false);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [errorsList, setErrorsList] = useState([]);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [allCountries, setAllCountries] = useState([]);
  const [loading, setLoading] = useState();
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  const {
    register,
    formState: { errors },
    setValue,
    getValues,
    control,
    unregister,
  } = useForm();

  useWatch({
    name: "store",
    control,
  });
  useWatch({
    name: "station",
    control,
  });
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  let getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    setStoresForSelection(res.data.result);
  };

  let getAllCountry = async () => {
    let data = await getAllCountryFunc();
    if (data?.length > 0) setAllCountries(data);
  };

  useEffect(() => {
    getStoresForSelection();
    getAllCountry();
  }, []);

  useEffect(() => {
    if (selectedData) {
      setEditOpen(true);
    }
  }, [selectedData]);

  const downloadSample = async (id, c_id) => {
    let res = await GetOrderFileByOrderTypeId(id, c_id);
    const link = document.createElement("a");
    link.href = res.data.result.url;
    link.download = id === 1 ? "Regular.xlsx" : "Fulfillable.xlsx";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };
  const handleCreateOrder = async () => {
    const hasErrorInList = errorsList.some((dt) => dt.IsSuccessed === false);
    if (errorsList.length > 0 && hasErrorInList) {
      errorNotification("Please fix errors before proceed");
    } else {
      await createOrder();
    }
  };
  console.log(orderData);
  const createOrder = async () => {
    let params = [];
    setErrorsList([]);

    for (let i = 0; i < orderData.length; i++) {
      let orderBox = [];
      for (let j = 0; j < orderData[i].orderBoxs.length; j++) {
        orderBox.push({
          clientOrderBoxId: orderData[i].orderBoxs[j].clientOrderBoxId,
          orderBoxId: orderData[i].orderBoxs[j].orderBoxId,
          width: orderData[i].orderBoxs[j].width,
          volume: orderData[i].orderBoxs[j].volume,
          length: orderData[i].orderBoxs[j].length,
          height: orderData[i].orderBoxs[j].height,
        });
      }
      let orderItems = [];
      for (let j = 0; j < orderData[i].orderItems.length; j++) {
        orderItems.push({
          productId: orderData[i].orderItems[j].productId,
          productStockId: orderData[i].orderItems[j].productStockId,
          price: orderData[i].orderItems[j].price,
          description: orderData[i].orderItems[j].description,
          remarks: orderData[i].orderItems[j].remarks,
          quantity: orderData[i].orderItems[j].quantity,
          discount: orderData[i].orderItems[j].discount,
        });
      }
      params.push({
        SaleChannelConfigId: !orderData[i].saleChannelConfigId
          ? orderData[i].SaleChannelConfigId
          : orderData[i].saleChannelConfigId,
        storeId: orderData[i].storeId,
        orderTypeId: orderData[i].orderTypeId,
        orderDate: orderData[i].orderDate,
        description: orderData[i].description,
        remarks: orderData[i].remarks,
        amount: orderData[i].amount,
        cShippingCharges: orderData[i].cShippingCharges,
        paymentStatusId: orderData[i].paymentStatusId,
        weight: orderData[i].weight,
        itemValue: orderData[i].itemValue,
        orderRequestVia: orderData[i].orderRequestVia,
        paymentMethodId: orderData[i].paymentMethodId,
        stationId: orderData[i].stationId,
        discount: orderData[i].discount,
        vat: orderData[i].vat,
        refNo: orderData[i].refNo,
        orderNote: orderData[i].orderNote,
        orderAddress: {
          customerName: orderData[i].orderAddress.customerName,
          email: orderData[i].orderAddress.email,
          mobile1: orderData[i].orderAddress.mobile1,
          mobile2: orderData[i].orderAddress.mobile2,
          countryId: orderData[i].orderAddress.country,
          provinceId: orderData[i].orderAddress.province,
          pinCodeId: orderData[i].orderAddress.pinCode,
          stateId: orderData[i].orderAddress.state,
          cityId: orderData[i].orderAddress.city,
          areaId: orderData[i].orderAddress.area,
          streetAddress: orderData[i].orderAddress.streetAddress,
          latitude: orderData[i].orderAddress.latitude,
          longitude: orderData[i].orderAddress.longitude,
        },
        orderItems: orderItems,
        orderBoxs: orderBox,
      });
    }
    let param1 = { orderList: params };
    console.log(param1);
    setPlacingOrder(true);
    CreateOrder(param1)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          for (let i = 0; i < res?.data?.errorCombined.length; i++) {
            errorNotification(res?.data?.errorCombined[i]);
          }

          errorNotification(res?.data?.customErrorMessage);
        } else {
          successNotification("Orders created successfully");
          navigate("/orders-dashboard");
        }
      })
      .catch((e) => {
        console.log("e", e);
        let msg = [];
        let count = 0;
        for (const key in e.response?.data?.errorCombined) {
          if (e.response?.data?.errorCombined.hasOwnProperty(key)) {
            const errorMessage = e.response?.data?.errorCombined[key];
            errorNotification(errorMessage);
            msg.push({
              Row: parseInt(key) + 1,
              Msg: errorMessage,
              IsSuccessed: false,
            });
            // Do something with the error message
          } else {
            msg.push({ Row: count, Msg: "errorMessage", IsSuccessed: true });
          }
          count++;
        }
        console.log("msgmsg", msg);
        setErrorsList(msg);
        errorNotification("Something went wrong");
      })
      .finally((e) => {
        setPlacingOrder(false);
      });
  };
  const [stockLoading, setStockLoading] = useState(false);
  const [filterdProductStock, setFilterdProductStock] = useState([]);

  const getProductStockByStationAndStore = async (data) => {
    setStockLoading(true);
    let res = await GetProductStocksForSelection(data.stationId, data?.storeId);
    let preloadedProducts = [];
    setStockLoading(false);

    if (res.data.result && res.data.result.length > 0) {
      data?.orderItems.forEach((item, i) => {
        let value = res.data.result?.find(
          (x) => x.ProductStockId == item.productStockId
        );
        if (value) {
          value["orderItemId"] = item.orderItemId;
          value["Price"] = item.price;
          value["newQuantity"] = item.quantity;
          value["discount"] = item.discount;
          preloadedProducts.push(value);
        }
      });
      setFilterdProductStock(preloadedProducts);
    }
  };
  const handleEdit = (row) => {
    row.amount = row.actualAmmount;
    setSelectedData(row);
  };
  const columns = [
    {
      field: "refNo",
      headerName: (
        <Box sx={{ fontWeight: "600", paddingLeft: "25px" }}>
          {LanguageReducer?.languageType?.ORDERS_REF_NO}
        </Box>
      ),
      renderCell: (params) => {
        return <Box sx={{ paddingLeft: "25px" }}>{params.row.refNo}</Box>;
      },
      flex: 1,
    },
    {
      field: "orderTypeId",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_ORDER_TYPE}
        </Box>
      ),
      renderCell: (params) => {
        return params.row.orderTypeId == 1 ? "Regular" : "Fulfillable";
      },
      flex: 1,
    },
    {
      field: "customerName",
      renderCell: (params) => {
        return params?.row?.orderAddress?.customerName;
      },
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_CUSTOMER}
        </Box>
      ),
      flex: 1,
    },

    {
      field: "mobile1",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_CONTACT_NUMBER}
        </Box>
      ),
      renderCell: (params) => {
        return params?.row?.orderAddress?.mobile1;
      },
      flex: 1,
    },
    {
      field: "Product",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDER_PRODUCT}
        </Box>
      ),
      flex: 1,
      ...centerColumn,
      renderCell: (params) => {
        return (
          <Box>
            {params?.row?.orderItems?.length}

            <DescriptionBoxWithChild
              onClick={() => getProductStockByStationAndStore(params.row)}
            >
              <TableContainer>
                <Table sx={{ minWidth: 300 }} aria-label="simple table">
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
                        Sku
                      </TableCell>
                      <TableCell
                        sx={{
                          fontWeight: "bold",
                          fontSize: "11px",
                          padding: "5px",
                        }}
                      >
                        Qty
                      </TableCell>
                      <TableCell
                        sx={{
                          fontWeight: "bold",
                          fontSize: "11px",
                          padding: "5px",
                          // width: "150px",
                        }}
                        align="right"
                      >
                        Discount
                      </TableCell>
                      <TableCell
                        sx={{
                          fontWeight: "bold",
                          fontSize: "11px",
                          padding: "5px",
                          // width: "150px",
                        }}
                        align="right"
                      >
                        Price
                      </TableCell>
                    </TableRow>
                  </TableHead>
                  {stockLoading ? (
                    <Box>{placeholders.loading}</Box>
                  ) : (
                    <TableBody>
                      {filterdProductStock?.map((item) => (
                        <TableRow
                          key={1}
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
                            {item?.SKU}
                          </TableCell>
                          <TableCell
                            sx={{ padding: "7px", fontSize: "11px" }}
                            align="right"
                          >
                            {item?.newQuantity}
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              // width: "150px",
                            }}
                            align="right"
                          >
                            {item?.discount}
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "7px",
                              fontSize: "11px",
                              // width: "150px",
                            }}
                            align="right"
                          >
                            {amountFormat(item?.Price)}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  )}
                </Table>
              </TableContainer>
            </DescriptionBoxWithChild>
          </Box>
        );
      },
    },
    {
      field: "remarks",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_REMARKS}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "amount",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_AMOUNT}
        </Box>
      ),
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <Box sx={{ textAlign: "center" }}>{row.amount}</Box>{" "}
          </>
        );
      },
    },
    {
      field: "description",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_DESCRIPTION}
        </Box>
      ),
      valueGetter: (params) => `${params?.row?.description || ""}`,
      flex: 1,
    },
    {
      field: "orderAddress",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDER_ADDRESS}
        </Box>
      ),
      sortable: false,
      renderCell: (params) => {
        return truncate(params.row.orderAddress.streetAddress);
      },
      flex: 1,
    },

    {
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_ACTION}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        const index = errorsList.findIndex((error) => {
          return error.Row == params.row.index && error.IsSuccessed == false;
        });
        return (
          <Box>
            <ActionButtonEdit
              onClick={(e) => {
                // console.log("params:::", params);
                setSelectedRowIndex(params.row.index);
                setOrderType(params.row.orderTypeId);
                handleEdit(params.row);
                // setAnchorEl(e.currentTarget);
              }}
            />
            {index != -1 && (
              <StyledTooltip title={errorsList[index].Msg}></StyledTooltip>
            )}
          </Box>
        );
      },
    },
  ];
  const getRowClassName = (params) => {
    for (let i = 0; i < errorsList.length; i++) {
      if (
        params.row.index == errorsList[i].Row &&
        errorsList[i].IsSuccessed === false
      )
        return "active-row"; // CSS class name for active rows
    }
    return "";
  };
  const { addressSchemaSelectData, handleSetSchema } = useGetAddressSchema();

  useWatch({
    name: "country",
    control,
  });
  useWatch({
    name: "region",
    control,
  });
  useWatch({
    name: "city",
    control,
  });

  const handleCreateOrderFiltered = async () => {
    // Filter out rows that have "Duplicate Mobile" warning message
    const duplicateRowIndexes = errorsList
      .filter((err) => !err.IsSuccessed && err.Msg.includes("Duplicate Mobile!"))
      .map((err) => err.Row);

    if (duplicateRowIndexes.length === 0) {
      // No duplicates found, execute normal create path
      await handleCreateOrder();
      return;
    }

    const filteredOrderData = orderData.filter((item) => !duplicateRowIndexes.includes(item.index));
    if (filteredOrderData.length === 0) {
      errorNotification("All uploaded orders are duplicates. Nothing to create.");
      return;
    }

    // Temporarily replace orderData to trigger creation on filtered list
    const originalOrderData = [...orderData];
    setOrderData(filteredOrderData);
    setTimeout(async () => {
      await createOrder();
    }, 100);
  };

  return (
    <Box sx={styleSheet.pageRoot}>
      {/* <Container maxWidth="xl" fixed sx={{ paddingLeft: "0px" }}> */}
      <div style={{ padding: "10px" }}>
        <GridItem mb={1} textAlign={"right"}>
          <ActionButtonCustom
            onClick={() => {
              navigate("/orders-dashboard");
            }}
            label={LanguageReducer?.languageType?.ORDERS_ORDER_DASHBOARD}
          />
        </GridItem>
        <Card sx={styleSheet.uploadOrderCard} variant="outlined">
          <Grid container spacing={1.5} paddingTop={"0px"}>
            <Box
              width={"100%"}
              display={"flex"}
              justifyContent={"space-between"}
            >
              <Grid item md={3} sm={12} xs={12} paddingLeft={"12px"}>
                <InputLabel required sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.ORDERS_COUNTRY}
                </InputLabel>
                <CountrySchema
                  name="country"
                  control={control}
                  isRHF={true}
                  required={true}
                  isRefesh={true}
                  handleRefreshClick={getAllCountry}
                  {...register("country", {
                    required: {
                      value: true,
                    },
                  })}
                  value={getValues("country")}
                  onChange={(event, newValue) => {
                    const resolvedId = newValue ? newValue : null;
                    handleSetSchema("country", resolvedId, setValue, unregister);
                  }}
                  errors={errors}
                />
              </Grid>
              <Box display={"flex"} flexDirection={"row"} alignSelf={"end"} gap={1.5}>
                {orderData.length > 0 && (
                  <>
                    <ActionButtonCustom
                      loading={placingOrder}
                      disabled={orderData.length === 0}
                      onClick={() => {
                        handleCreateOrder();
                      }}
                      label={LanguageReducer?.languageType?.ORDERS_UPLOAD_ORDER}
                    />
                    <ActionButtonCustom
                      loading={placingOrder}
                      disabled={orderData.length === 0}
                      onClick={() => {
                        handleCreateOrderFiltered();
                      }}
                      background={Colors.orange || "#ff9800"}
                      label="Remove Duplicate and Create"
                    />
                  </>
                )}
              </Box>
            </Box>
            <Grid item md={12} sm={6} sx={{ display: "flex" }} gap={2}>
              <ActionButtonCustom
                label={LanguageReducer?.languageType?.ORDER_IMPORT}
                onClick={() => {
                  setOpen(true);
                }}
                startIcon={<FileUploadOutlined />}
                disabled={getValues("country")?.countryId ? false : true}
              />
              <ActionButtonCustom
                label={LanguageReducer?.languageType?.ORDER_SAMPLE_REGULAR}
                onClick={() => {
                  downloadSample(1, getValues("country")?.countryId);
                }}
                startIcon={<FileDownloadOutlinedIcon />}
                disabled={getValues("country")?.countryId ? false : true}
              />
              <ActionButtonCustom
                label={LanguageReducer?.languageType?.ORDER_SAMPLE_FULLFILLABLE}
                onClick={() => {
                  downloadSample(2, getValues("country")?.countryId);
                }}
                startIcon={<FileDownloadOutlinedIcon />}
                disabled={getValues("country")?.countryId ? false : true}
              />
              {addressSchemaSelectData?.map((entity, entity_i) => {
                const prevKey =
                  entity_i === 0
                    ? "country"
                    : addressSchemaSelectData[entity_i - 1].key;
                return (
                  <ActionButtonCustom
                    background={Colors.succes}
                    label={`Download ${entity.label}`}
                    onClick={async () => {
                      const { response } = await fetchMethod(() =>
                        ExcelExportAddressEntitiesByType(
                          getValues("country")?.countryId,
                          entity.key
                        )
                      );
                      UtilityClass.downloadExcel(
                        response,
                        `${entity.label} File`
                      );
                    }}
                    startIcon={<FileDownloadOutlinedIcon />}
                  />
                );
              })}
            </Grid>
          </Grid>
        </Card>
        <br />
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            height:
              windowHeight -
              navbarHeight -
              dataGridProFooterHeight -
              118 -
              40 -
              19 -
              12,
          }}
        >
          <DataGridProComponent
            rows={orderData}
            columns={columns}
            loading={loading}
            getRowId={(row) => `${row.orderDate} ${row.description} `}
            defaultRowsPerPage={200}
            getRowClassName={getRowClassName}
            height={
              windowHeight -
              navbarHeight -
              dataGridProFooterHeight -
              118 -
              40 -
              19 -
              12
            }
            rowsCount={orderData.length}
          />
          <Menu
            anchorEl={anchorEl}
            id="power-search-menu"
            open={Boolean(anchorEl)}
            onClose={() => {
              setAnchorEl(null);
            }}
            PaperProps={{
              elevation: 0,
              sx: {
                overflow: "visible",
                filter: "drop-shadow(0px 2px 8px rgba(0,0,0,0.32))",
                mt: 1.5,
                "& .MuiAvatar-root": {
                  width: 32,
                  height: 32,
                  ml: -0.5,
                  mr: 1,
                },
                "&:before": {
                  content: '""',
                  display: "block",
                  position: "absolute",
                  top: 0,
                  right: 14,
                  width: 10,
                  height: 10,
                  bgcolor: "background.paper",
                  transform: "translateY(-50%) rotate(45deg)",
                  zIndex: 0,
                },
              },
            }}
            transformOrigin={{ horizontal: "right", vertical: "top" }}
            anchorOrigin={{ horizontal: "right", vertical: "bottom" }}
          >
            <Box sx={{ width: "200px" }}>
              <List disablePadding>
                <ListItem
                  onClick={() => {
                    setEditOpen(true);
                    setAnchorEl(null);
                  }}
                  disablePadding
                >
                  <ListItemButton>
                    <ListItemIcon sx={{ minWidth: "30px" }}>
                      <DisplaySettingsIcon />
                    </ListItemIcon>
                    <ListItemText primary="Edit Order" />
                  </ListItemButton>
                </ListItem>
              </List>
            </Box>
          </Menu>
          {/* <OrderDetailModal open={open} setOpen={setOpen} /> */}
          <PlaceOrderModal
            setLoading={setLoading}
            open={open}
            setOpen={setOpen}
            setOrderData={setOrderData}
            storesForSelection={storesForSelection}
            getStoresForSelection={getStoresForSelection}
            setErrorsList={setErrorsList}
            errorsList={errorsList}
            countryId={getValues("country")?.countryId}
          />
          {selectedData ? (
            orderType === 2 ? (
              <EditFullfilableOrderDetailsModal
                selectedData={selectedData}
                setSelectedData={setSelectedData}
                open={editOpen}
                setOpen={setEditOpen}
                storesForSelection={storesForSelection}
                getStoresForSelection={getStoresForSelection}
                orderData={orderData}
                setOrderData={setOrderData}
                selectedRowIndex={selectedRowIndex}
                setSelectedRowIndex={setSelectedRowIndex}
                errorsList={errorsList}
                setErrorsList={setErrorsList}
              />
            ) : (
              <EditRegularOrderDetailsModal
                selectedData={selectedData}
                setSelectedData={setSelectedData}
                open={editOpen}
                setOpen={setEditOpen}
                storesForSelection={storesForSelection}
                getStoresForSelection={getStoresForSelection}
                orderData={orderData}
                setOrderData={setOrderData}
                selectedRowIndex={selectedRowIndex}
                setSelectedRowIndex={setSelectedRowIndex}
                errorsList={errorsList}
                setErrorsList={setErrorsList}
              />
            )
          ) : null}
        </Box>
      </div>
    </Box>
  );
}
export default UploadOrders;
