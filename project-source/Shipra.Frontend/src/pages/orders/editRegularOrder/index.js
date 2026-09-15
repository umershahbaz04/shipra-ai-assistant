import {Box} from "@mui/material";
import React, {useEffect, useState} from "react";
import {useSelector} from "react-redux";
import {useLocation, useNavigate, useSearchParams} from "react-router-dom";
import {GetOrderById} from "../../../api/AxiosInterceptors";
import {styleSheet} from "../../../assets/styles/style";

import {BackdropCustom} from "../../../utilities/helpers/Helpers";
import EditRegularForm from "./EditRegularForm";

const EnumOrderPlaceButton = Object.freeze({
  Confirm: 1,
  ConfirmAndNew: 2,
  ConfirmAndHandleInvoice: 2,
});

function EditOrderPage(props) {
  const navigate = useNavigate();
  /////////////////
  const location = useLocation();
  const {stData} = location.state || {};
  const [searchParams] = useSearchParams();
  const newTabStateKey = searchParams.get("newTabStateKey");
  const [load, setLoad] = useState(false);
  const [orderData, setOrderData] = useState();
  const [orderLoadedValues, setOrderLoadedValues] = useState();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  /////////edit infor
  const getOrderData = async () => {
    try {
      setLoad(true);
      debugger;
      const storedState = sessionStorage.getItem(newTabStateKey);
      const activeCarrierId = sessionStorage.getItem("carrierId");
      const sessionStorageState = JSON.parse(storedState);
      const orderId = newTabStateKey
        ? sessionStorageState.order?.OrderId
        : stData?.order?.OrderId;
      const response = await GetOrderById(orderId);
      const oAddressInfo = response.data.result?.orderAddress;
      let metafields = [];
      try {
        const config = response?.data?.result?.metafields?.settingConfig;
        metafields = config ? JSON.parse(config) : [];
      } catch (e) {
        metafields = [];
      }
      const oAddressCarrierId = oAddressInfo?.selectedCarrierId;
      const splitCity = oAddressInfo?.city?.split("_")[0] || "";
      const splitArea = oAddressInfo?.area?.split("_")[0] || "";
      const splitprovince = oAddressInfo?.province?.split("_")[0] || "";
      const splitstate = oAddressInfo?.state?.split("_")[0] || "";
      const splitpinCode = oAddressInfo?.pinCode?.split("_")[0] || "";
      if (oAddressCarrierId > 0) {
        setOrderData(response.data.result);
      } else {
        setOrderData((prevState) => ({
          ...response.data.result,
          orderAddress: {
            ...response.data.result.orderAddress,
            city: Number(splitCity),
            area: Number(splitArea),
            province: Number(splitprovince),
            state: Number(splitstate),
            pinCode: Number(splitpinCode),
          },
        }));
      }
      const orderInfo = response.data.result?.order;
      const oNoteInfo = response.data.result?.orderNote;
      let object = {
        orderId: orderInfo.orderId,
        description: orderInfo.description,
        remarks: orderInfo?.remarks,
        amount: orderInfo?.actualAmount, //it will used only for eidt order
        paymentMethodId: orderInfo?.paymentMethodId,
        discount: orderInfo?.discount,
        OrderDate: orderInfo?.orderDate,
        cShippingCharges: orderInfo?.cShippingCharges,
        paymentStatusId: orderInfo?.paymentStatusId,
        stripeInvoiceHostURL: orderInfo?.stripeInvoiceHostURL,
        stripeInvoicePDFURL: orderInfo?.stripeInvoicePDFURL,

        vat: orderInfo?.vat,
        storeId: parseInt(orderInfo.storeId),
        weight: orderInfo?.weight,
        numberOfPieces: orderInfo.itemsCount,
        station: orderInfo?.stationId,
        storeChannel: orderInfo?.saleChannelConfigId,
        refNo: orderInfo.refNo,

        country: oAddressInfo?.countryId,
        region: oAddressInfo?.regionId,
        city: oAddressInfo?.cityName,
        mobile1: oAddressInfo?.mobile1,
        mobile2: oAddressInfo?.mobile2,
        streetAddress: oAddressInfo?.streetAddress,
        customerName: oAddressInfo?.customerName,
        email: oAddressInfo?.email,
        latitude: oAddressInfo?.latitude,
        longitude: oAddressInfo?.longitude,
        selectedCarrierId: oAddressInfo?.selectedCarrierId,
        CarrierIdFromAssignToCarrierModal: activeCarrierId
          ? Number(activeCarrierId)
          : null,

        orderNote: oNoteInfo?.note,
        metafields: metafields,
      };
      setOrderLoadedValues(object);
      setLoad(false);
    } catch (error) {
      console.error("Error in getting data of product:", error);
      setLoad(false);
    }
  };
  useEffect(() => {
    if (!orderData) {
      getOrderData();
    }
  }, [orderData]);

  /////////

  return (
    <Box sx={styleSheet.pageRoot}>
      {orderLoadedValues ? (
        <EditRegularForm
          preloadedValues={orderLoadedValues}
          orderData={orderData}
        />
      ) : (
        <BackdropCustom open={load} />
      )}
    </Box>
  );
}
export default EditOrderPage;
