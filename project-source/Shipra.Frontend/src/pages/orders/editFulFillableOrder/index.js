import { makeStyles } from "@material-ui/core/styles";
import { Box } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLocation, useSearchParams } from "react-router-dom";
import { GetOrderById } from "../../../api/AxiosInterceptors";
import "../../../assets/styles/hideInputArrowsStyles.css";
import { styleSheet } from "../../../assets/styles/style";
import { BackdropCustom } from "../../../utilities/helpers/Helpers";
import EditFulFillableForm from "./EditFulFillableForm";

const useStyles = makeStyles({
  customTable: {
    "& .MuiTableCell-sizeSmall": {
      padding: "6px 0px 6px 16px",
      textAlign: "center",
    },
  },
});
const EnumOrderPlaceButton = Object.freeze({
  Confirm: 1,
  ConfirmAndNew: 2,
  ConfirmAndHandleInvoice: 2,
});
function EditOrderPage(props) {
  const [searchParams] = useSearchParams();
  const newTabStateKey = searchParams.get("newTabStateKey");
  const location = useLocation();

  const { stData } = location.state || {};
  const [load, setLoad] = useState(false);
  const [orderData, setOrderData] = useState();
  const [orderLoadedValues, setOrderLoadedValues] = useState();

  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  /////////edit infor
  const getOrderData = async () => {
    try {
      setLoad(true);
      const storedState = sessionStorage.getItem(newTabStateKey);
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
          },
        }));
      }

      const orderInfo = response.data.result?.order;
      const oNoteInfo = response.data.result?.orderNote;

      let orderDate = orderInfo.orderDate && new Date(orderInfo.orderDate);
      let object = {
        orderId: orderInfo.orderId,
        description: orderInfo.description,
        remarks: orderInfo?.remarks,
        storeId: parseInt(orderInfo.storeId),
        weight: orderInfo?.weight,
        numberOfPieces: orderInfo.itemsCount,
        station: orderInfo?.stationId,
        refNo: orderInfo.refNo,
        stripeInvoiceHostURL: orderInfo?.stripeInvoiceHostURL,
        stripeInvoicePDFURL: orderInfo?.stripeInvoicePDFURL,

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
        orderDate: orderDate,
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
        <EditFulFillableForm
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
