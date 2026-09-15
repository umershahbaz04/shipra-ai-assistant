import { Box } from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent.js";
import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader.js";
import {
  CreateOrder,
  GetAllOrderTypeLookup,
  GetAllStationLookup,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";
import SaleChannelOrderModal from "../../../components/modals/orderModals/SaleChannelOrderModal.js";
import SaleChannelSyncModel from "../../../components/modals/orderModals/SaleChannelSyncModel.js";
import UtilityClass from "../../../utilities/UtilityClass.js";
import {
  ActionButtonCustom,
  ActionButtonEdit,
  DataGridHeaderBox,
  amountFormat,
  centerColumn,
  navbarHeight,
  pagePadding,
  useClientSubscriptionReducer,
  useGetWindowHeight,
} from "../../../utilities/helpers/Helpers.js";
import { successNotification } from "../../../utilities/toast/index.js";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";

export default function SaleChannelOrderPage() {
  const [saleChannerlOrderModal, setSaleChannerlOrderModal] = useState({
    open: false,
    data: {},
  });
  const [selectedRowIndex, setSelectedRowIndex] = useState();
  const [allOrders, setAllOrders] = useState([]);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [stores, setStores] = React.useState([]);
  const [allOrderTypeLookup, setAllOrderTypeLookup] = useState([]);
  const [productStations, setProductStations] = useState([]);
  const [orderTypeId, setorderTypeId] = useState();
  const [isLoadingForConfirm, setIsLoadingForConfirm] = useState(false);
  const [openFetch, setOpenFetch] = useState();
  const [errorsList, setErrorsList] = useState([]);
  const [selectionModel, setSelectionModel] = useState();
  const [filteredOrders, setFilteredOrders] = useState([]);
  const resetRowRef = useRef(false);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const getAllOrderTypeLookup = async () => {
    try {
      const response = await GetAllOrderTypeLookup();
      setAllOrderTypeLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };

  let getAllStores = async () => {
    let res = await GetStoresForSelection();
    if (res.data.result !== null) {
      setStores(res.data.result);
    }
  };
  let getAllStationLookup = async () => {
    let res = await GetAllStationLookup();
    if (res.data.result != null) setProductStations(res.data.result);
  };
  // columns
  const columns = [
    {
      field: "orderRef",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_NO}
        />
      ),
      flex: 1,
      minWidth: 100,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center", cursor: "pointer" }} disableRipple>
            <>
              <Box>{params.row?.refNo}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "name",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_NAME}
        />
      ),
      minWidth: 150,
      flex: 1,

      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "start", cursor: "pointer" }} disableRipple>
            <>
              <Box>{params.row?.orderAddress?.customerName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "orderType",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDER_TYPE}
        />
      ),
      flex: 2,
      minWidth: 150,
      renderCell: (params) => {
        return (
          <Box sx={{ textAlign: "center" }}>
            <>
              <Box>{params.row?.orderTypeName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "store",
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_STORE
          }
        />
      ),
      flex: 2,
      minWidth: 150,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <Box>{params.row?.storeName}</Box>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row?.customerServiceNo}
              </Box>
            </>
          </Box>
        );
      },
    },

    {
      field: "address",
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_ADDRESS
          }
        />
      ),
      flex: 2,
      minWidth: 150,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <Box
                noWrap={false}
              >{`${params.row?.orderAddress?.streetAddress}`}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "phone",
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_PHONE
          }
        />
      ),
      flex: 2,
      renderCell: (params) => {
        return params.row?.orderAddress?.mobile1;
      },
    },
    {
      ...centerColumn,
      field: "vat",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_VAT}
        />
      ),
      flex: 2,
      renderCell: ({ row }) => {
        return amountFormat(row.vat);
      },
    },
    {
      ...centerColumn,
      field: "amount",
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_AMOUNT
          }
        />
      ),
      flex: 2,
      renderCell: ({ row }) => {
        return amountFormat(row.amount);
      },
    },
    {
      ...centerColumn,
      field: "Action",
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_ACTION
          }
        />
      ),
      flex: 2,
      renderCell: ({ row }) => {
        return (
          <Box>
            <ActionButtonEdit
              onClick={(e) => {
                setSelectedRowIndex(row.index);
                setorderTypeId(row?.orderTypeId);
                setSaleChannerlOrderModal((prev) => ({
                  ...prev,
                  data: row,
                  open: true,
                }));
              }}
            />
          </Box>
        );
      },
    },
  ];
  const getRowClassName = (params) => {
    for (let i = 0; i < errorsList.length; i++) {
      if (
        params.row.index + 1 == errorsList[i].Row &&
        errorsList[i].IsSuccessed === false
      )
        return "active-row"; // CSS class name for active rows
    }
    return "";
  };
  useEffect(() => {
    getAllStores();
    getAllOrderTypeLookup();
    getAllStationLookup();
  }, []);
  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
  };
  const handleUploadOrder = () => {
    const selectedOrders = selectionModel.map((index) => allOrders[index]);
    setFilteredOrders(selectedOrders);

    const mappedData = {
      orderList: selectedOrders.map((data) => ({
        ...data,
        orderAddress: {
          countryId: data.orderAddress.country,
          cityId: data.orderAddress.city,
          areaId: data.orderAddress.area,
          pinCodeId: data.orderAddress.pinCode,
          stateId: data.orderAddress.state,
          provinceId: data.orderAddress.province,
          buildingName: data.orderAddress.buildingName,
          customerFullAddress: data.orderAddress.customerFullAddress,
          customerName: data.orderAddress.customerName,
          email: data.orderAddress.email,
          houseNo: data.orderAddress.houseNo,
          landmark: data.orderAddress.landmark,
          latitude: data.orderAddress.latitude,
          longitude: data.orderAddress.longitude,
          mobile1: data.orderAddress.mobile1,
          mobile2: data.orderAddress.mobile2,
          orderAddressId: data.orderAddress.orderAddressId,
          streetAddress: data.orderAddress.streetAddress,
          streetAddress2: data.orderAddress.streetAddress2,
        },
      })),
      IsSaleChannelOrder: true,
    };
    setIsLoadingForConfirm(true);
    CreateOrder(mappedData)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
        } else {
          successNotification("Order created successfully");
          const remainingOrders = allOrders.filter(
            (order, index) => !selectionModel.includes(index)
          );
          setAllOrders(remainingOrders);
          setSelectionModel([]);
          setFilteredOrders([]);
          if (selectionModel.length === allOrders.length) {
            navigate("/orders-dashboard");
          }
        }
      })
      .catch((e) => {
        console.log("e", e);
        UtilityClass.showErrorNotificationWithDictionary(
          e?.response?.data?.errors
        );
      })
      .finally(() => {
        setIsLoadingForConfirm(false);
      });
  };
  const calculatedHeight = windowHeight - navbarHeight - 65;
  return (
    <Box sx={styleSheet.pageRoot}>
      <Box sx={{ padding: pagePadding }}>
        <DataGridHeader tabs={false}>
          <Box className={"flex_center"} gap={1}>
            {selectionModel?.length > 0 && (
              <ActionButtonCustom
                onClick={handleUploadOrder}
                label={
                  LanguageReducer?.languageType
                    ?.ORDERS_SALE_CHANNEL_ORDERS_UPLOAD_ORDER
                }
                loading={isLoadingForConfirm}
                width="120px"
              />
            )}
            <ActionButtonCustom
              onClick={(event) => {
                setOpenFetch(true);
              }}
              label={
                LanguageReducer?.languageType
                  ?.ORDERS_SALE_CHANNEL_ORDERS_SYNC_ORDER
              }
              width="120px"
            />
          </Box>
        </DataGridHeader>
        <DataGridComponent
          columns={columns}
          rows={allOrders}
          getRowId={(allOrders) => allOrders.index}
          resetRowRef={resetRowRef}
          getRowHeight={() => "auto"}
          checkboxSelection={true}
          selectionModel={selectionModel}
          onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
          getRowClassName={getRowClassName}
          height={calculatedHeight}
        />
      </Box>

      {openFetch && (
        <SaleChannelSyncModel
          resetRowRef={resetRowRef}
          setAllOrders={setAllOrders}
          allStores={stores}
          setErrorsList={setErrorsList}
          allOrderTypeLookup={allOrderTypeLookup}
          open={openFetch}
          productStations={productStations}
          setOpen={setOpenFetch}
          getAllStationLookup={getAllStationLookup}
          getAllStores={getAllStores}
          getAllOrderTypeLookup={getAllOrderTypeLookup}
        />
      )}
      {saleChannerlOrderModal.open && (
        <SaleChannelOrderModal
          open={saleChannerlOrderModal.open}
          selectedRowIndex={selectedRowIndex}
          onClose={() =>
            setSaleChannerlOrderModal((prev) => ({ ...prev, open: false }))
          }
          orderData={saleChannerlOrderModal.data}
          orderTypeId={orderTypeId}
          setAllOrders={setAllOrders}
          errorsList={errorsList}
          setErrorsList={setErrorsList}
          allOrders={allOrders}
        />
      )}
    </Box>
  );
}
