import { makeStyles } from "@material-ui/core/styles";
import AddBoxIcon from "@mui/icons-material/AddBox";
import AddBoxOutlinedIcon from "@mui/icons-material/AddBoxOutlined";
import DeleteIcon from "@mui/icons-material/Delete";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import IndeterminateCheckBoxOutlinedIcon from "@mui/icons-material/IndeterminateCheckBoxOutlined";
import { LoadingButton } from "@mui/lab";
import {
  Avatar,
  Box,
  Button,
  Checkbox,
  CircularProgress,
  Divider,
  FormControl,
  FormControlLabel,
  Grid,
  InputLabel,
  OutlinedInput,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateOrder,
  CreateOrderWithInvoice,
  DeleteOrderDraft,
  GetAddressFromLatAndLong,
  GetAllActiveCarrierForCreateOrderSelection,
  GetAllClientOrderBox,
  GetAllCountry,
  GetAllStationLookup,
  GetChannelListByStoreIdForSelection,
  GetProductStocksForSelection,
  GetStoresForSelection,
  GetValidateClientPPActivate,
  UpdateOrder,
} from "../../../api/AxiosInterceptors";
import "../../../assets/styles/hideInputArrowsStyles.css";
import { styleSheet } from "../../../assets/styles/style";
import AddOrderBoxModal from "../../../components/modals/orderModals/AddOrderBoxModal";
import AddProductModal from "../../../components/modals/orderModals/AddProductModal";
import FromLinkLocationModal from "../../../components/modals/orderModals/FromLinkLocationModal";
import GoogleMapWithSearch from "../../../components/modals/resuseAbleModals/GoogleMapWithSearch";
import { handleDispatchUserProfile } from "../../../components/topNavBar";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  EnumMetaField,
  EnumOptions,
  EnumOrderType,
  EnumPaymentMethod,
  EnumPaymentStatus,
  EnumRoutesUrls,
} from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  CrossIconButton,
  CustomColorLabelledOutline,
  DataGridRenderGreyBox,
  GridContainer,
  GridItem,
  PaymentAmountBox,
  PaymentAmountTextField,
  PaymentMethodBox,
  PaymentMethodCheckbox,
  PaymentTaxAlert,
  PaymentTaxBox,
  PaymentTotalBox,
  amountFormat,
  calculateSubtotalValue,
  camelizeArray,
  decimalFormat,
  fetchMethod,
  fetchMethodResponse,
  getDefaultValueIndex,
  getLowerCase,
  handleCopyToClipBoard,
  handleDispathUpdateBadge,
  placeholders,
  truncate,
  useGetAllClientTax,
  useSetNumericInputEffect,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  checkRequiredUsingCarrierAdressSchema,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { calculateVatValue } from "../createRegularOrder";
import { useGetAllMetafields } from "../../../utilities/helpers/HelpersFilter";
import { handleGetDraftOrdersCount } from "../../../components/authRoutes";
import { isShowCountryInTabbarFlag } from "../../../utilities/cookies";
const useStyles = makeStyles({
  customTable: {
    "& .MuiTableCell-sizeSmall": {
      padding: "6px 0px 6px 16px",
      textAlign: "center",
    },
  },
});
const EnumOrderPlaceButton = Object.freeze({
  DUBLICATE: 1,
  SAVE: 2,
  COMPLETE: 3,
  ConfirmAndHandleInvoice: 3,
});
const EnumDraftOrder = Object.freeze({
  draft: 1,
  complete: 2,
});
function DraftFullfillableOrder(props) {
  const classes = useStyles();
  const [open, setOpen] = useState(false);
  const [openFromLinkLocationModal, setOpenFromLinkLocationModal] =
    useState(false);
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [productStocksForSelection, setProductStocksForSelection] = useState(
    [],
  );
  const [selectedProducts, setSelectedProducts] = useState([]);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [selectedPMOption, setselectedPMOption] = useState(1);
  const [storesChannelForSelection, setStoresChannelForSelection] = useState(
    [],
  );

  const [paymentLinks, setPaymentLinks] = useState({
    show: false,
    paymentUrl: "",
    invoicePdf: "",
  });
  const [allCountries, setAllCountries] = useState([]);
  const [totalItemPrice, setTotalItemPrice] = useState(0);
  const [discount, setDiscount] = useState(0);
  const [shipping, setShipping] = useState(0);
  const [formMode, setFormMode] = useState(EnumDraftOrder.draft);
  const [vatValue, setVatValue] = useState([]);
  const [note, setNote] = useState("");
  const [remarks, setRemarks] = useState("");
  const [isStripeSettingExist, setStripeSettingExist] = useState(false);
  const [isLoadingForConfirm, setIsLoadingForConfirm] = useState(false);
  const [isLoadingForConfirmAndNewOrder, setIsLoadingForConfirmAndNewOrder] =
    useState(false);
  const [dublicateOrderLoading, setdublicateOrderLoading] = useState(false);
  const [
    isLoadingForConfirmAndSendInvoiceOrder,
    setIsLoadingForConfirmAndSendInvoiceOrder,
  ] = useState(false);
  const [productStations, setProductStations] = useState([]);
  const [qtySelectedStation, setQtySelectedStation] = useState([]);
  const [selectedOrderPlaceButtom, setSelectedOrderPlaceButtom] =
    useState(false);
  const [openOrderBoxModal, setopenOrderBoxModal] = useState({
    open: false,
    loading: {},
  });
  const [allClientOrderBox, setAllClientOrderBox] = useState([]);
  const [selectedOrderBox, setSelectedOrderBox] = useState([]);
  const [addedOrderBoxes, setAddedOrderBoxes] = useState([]);
  const [allCarriersForSelection, setAllCarriersForSelection] = useState([]);
  const [isFirstLoad, setIsFirstLoad] = useState(true);
  const [carrierId, setCarrierId] = useState();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { state: orderData } = useLocation();
  let data = orderData?.result?.orderInfo;
  const order = data !== undefined ? JSON.parse(data) : data;
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    getValues,
    setValue,
    control,
    unregister,
  } = useForm();

  const {
    showMoreInfoBtn,
    selectedAddressSchema,
    addressSchemaSelectData,
    addressSchemaInputData,
    handleSetSchema,
    handleAddRemoveMoreInfoFields,
    handleChangeInputAddressSchema,
    handleChangeSelectAddressSchemaAndGetOptions,
    handleSetSchemaValueForUpdate,
  } = useGetAddressSchema(setValue, true, carrierId?.CarrierId);

  const schemaFieldsLength = [
    ...addressSchemaSelectData,
    ...addressSchemaInputData,
  ].length;

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
  useWatch({
    name: "store",
    control,
  });
  useWatch({
    name: "lat&long",
    control,
  });
  useWatch({
    name: "store",
    control,
  });
  useWatch({
    name: "Station",
    control,
  });
  const { allClientTax, loading } = useGetAllClientTax([]);
  const {
    loading: metaLoading,
    metafields,
    setMetafields,
  } = useGetAllMetafields(EnumMetaField.Order);
  const [paymentDueLaterCheckBox, setPaymentDueLaterCheckBox] = useState(true);
  const [isProductFetching, setIsProductFetching] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry,
  );
  useEffect(() => {
    if (!isShowCountryInTabbarFlag()) return;
    setCarrierId(null);
    handleSetSchema(
      "country",
      reduxSelectedCountry || null,
      setValue,
      unregister,
      carrierId?.CarrierId,
    );
  }, [reduxSelectedCountry]);
  const [autocomplete, setAutocomplete] = useState(null);

  const getAllActiveCarrierForCreateOrderSelection = async () => {
    try {
      const response = await GetAllActiveCarrierForCreateOrderSelection(
        getValues("country")?.countryId,
      );
      setAllCarriersForSelection(response?.data?.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };
  let getProductStocksForSelection = async () => {
    if (getValues("store")?.storeId && !orderData) {
      setSelectedProducts([]);
      let stationId = qtySelectedStation?.productStationId || 0;

      setIsProductFetching(true);
      let res = await GetProductStocksForSelection(
        stationId,
        getValues("store").storeId,
      );
      if (res.data.result != null && res.data.result?.length > 0) {
        for (let index = 0; index < res.data?.result.length; index++) {
          res.data.result[index].checked = false;
          res.data.result[index].discount = 0;
          res.data.result[index].newPrice = res.data.result[index].Price;
          res.data.result[index].newQuantity = 1;
          // res.data.result[index].QuantityAvailable;
        }
        setProductStocksForSelection(res.data.result);

        //open modal
        setOpen(true);
      } else {
        errorNotification("No product found agains't selected store.");
      }
      setIsProductFetching(false);
    } else {
      setIsProductFetching(true);
      let res = await GetProductStocksForSelection(
        order.StationId,
        order?.StoreId,
      );
      if (res.data.result != null && res.data.result?.length > 0) {
        for (let index = 0; index < res.data?.result.length; index++) {
          //get selected object against station
          let obj = order?.OrderItems?.find(
            (x) => x.ProductStockId == res.data.result[index].ProductStockId,
          );
          // debugger;
          //orderItemId
          res.data.result[index].checked = obj ? true : false;
          res.data.result[index].isRemove =
            obj && obj?.orderItemId ? true : false; //if edit case then remove row
          res.data.result[index].discount = 0;
          res.data.result[index].newPrice = res.data.result[index].Price;
          res.data.result[index].newQuantity = 1;
          res.data.result[index].stationId =
            qtySelectedStation?.productStationId;
        }
        let preloadedProducts = [];
        order?.OrderItems.forEach((item, i) => {
          let value = res.data.result?.find(
            (x) => x.ProductStockId == item.ProductStockId,
          );
          if (value) {
            value["orderItemId"] = item.OrderItemId;
            value["checked"] = true;
            value["Price"] = item.Price;
            value["newPrice"] = item.Price;
            value["newQuantity"] = item.Quantity;
            value["discount"] = item.Discount;
            preloadedProducts.push(value);
          }
        });
        setSelectedProducts(preloadedProducts);
        setTotalItemPrice(orderData?.order?.actualAmount);

        setProductStocksForSelection(res.data.result);
      } else {
        errorNotification("No product found agains't selected store.");
      }

      setIsProductFetching(false);
    }
  };

  let getAllStationLookup = async () => {
    let res = await GetAllStationLookup();
    if (res.data.result != null) {
      const stations = res.data.result;
      setProductStations(stations);
      const defaultStation = stations.find((station) =>
        station.sname.toLowerCase().includes("default"),
      );
      if (defaultStation && !orderData) {
        setValue("Station", defaultStation);
        setQtySelectedStation(defaultStation);
      }
    }
  };

  let getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    const _stores = res.data.result;
    setValue(
      "store",
      _stores[getDefaultValueIndex(_stores, EnumOptions.STORE.VALUE)],
    );
    setStoresForSelection(_stores);
  };
  const getChannelListByStoreIdForSelection = async () => {
    setValue("storeChannel", null);

    if (getValues("store").storeId) {
      let res = await GetChannelListByStoreIdForSelection(
        getValues("store").storeId,
      );
      setStoresChannelForSelection(res.data.result || []);
    }
  };
  let getValidateClientPPActivate = async () => {
    let res = await GetValidateClientPPActivate();
    if (res.data.isSuccess) {
      setStripeSettingExist(res.data.result.isValidate);
    } else {
      setStripeSettingExist(false);
    }
  };
  const getAllClientOrderBox = async () => {
    try {
      const { response } = await fetchMethod(() => GetAllClientOrderBox());
      if (response.isSuccess) {
        const activeOrderBoxes = response.result.filter((item) => item.active);
        setAllClientOrderBox(activeOrderBoxes);
      }
    } catch (error) {
      console.error("Error fetching client return reasons:", error);
    }
  };

  const handleDeleteOrderBox = (id) => {
    const updatedOrderBoxes = addedOrderBoxes.filter(
      (box) => box.clientOrderBoxId !== id,
    );
    const updatedSelectedOrderBoxes = selectedOrderBox.filter(
      (box) => box.clientOrderBoxId !== id,
    );
    setAddedOrderBoxes(updatedOrderBoxes);
    setSelectedOrderBox(updatedSelectedOrderBoxes);
  };

  useEffect(() => {
    getAllStationLookup();
    getStoresForSelection();
    getValidateClientPPActivate();
    getAllClientOrderBox();
  }, []);

  useEffect(() => {
    if (getValues("country") !== null && getValues("country") !== undefined) {
      getAllActiveCarrierForCreateOrderSelection();
    }
  }, [getValues("country")]);

  useEffect(() => {
    setSelectedProducts([]);
  }, [storesForSelection]);
  useEffect(() => {
    if (
      getValues("store") !== null &&
      getValues("store") !== "" &&
      getValues("store") !== undefined
    ) {
      setSelectedProducts([]);
      getChannelListByStoreIdForSelection();
    }
  }, [getValues("store")]);

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    if (res.data.result != null) setAllCountries(res.data.result);
  };

  useEffect(() => {
    getAllCountry();
    setValue("lat");
  }, []);

  const prepareCreateOrderData = (data) => {
    let amount = 0;
    let weight = 0;
    let itemValue = 0;
    let description = [];
    let orderItems = [];
    for (let index = 0; index < selectedProducts.length; index++) {
      let ditem = `${selectedProducts[index].SKU}_${selectedProducts[index].newQuantity}`;
      // amount =
      //   amount +
      //   (selectedProducts[index].newPrice - selectedProducts[index].discount);
      // weight = weight + selectedProducts[index].Weight;

      //get for main item
      description.push(ditem);
      itemValue =
        itemValue +
        (selectedProducts[index].newPrice - selectedProducts[index].discount) *
          selectedProducts[index].newQuantity;
      orderItems[index] = {
        productId: selectedProducts[index].ProductId,
        productStockId: selectedProducts[index].ProductStockId,
        productVariantId: selectedProducts[index].ProductVariantId,
        price: selectedProducts[index].newPrice,
        description: ditem,
        remarks: "",
        quantity: selectedProducts[index].newQuantity,
        discount: selectedProducts[index].discount,
        StockSku: selectedProducts[index].SKU,
      };
    }

    amount =
      Number(totalItemPrice) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);
    const param1 = {
      orderList: [
        {
          storeId: data.store.storeId,
          SaleChannelConfigId: data.storeChannel?.id || 0,
          orderTypeId: EnumOrderType.FullFillable,
          orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
          description: description.join(","),
          remarks: remarks,
          amount: amount,
          cShippingCharges: shipping,
          paymentStatusId:
            EnumPaymentMethod.Prepaid == selectedPMOption
              ? EnumPaymentStatus.Paid
              : EnumPaymentStatus.Unpaid,
          weight: data.weight,
          itemValue: itemValue,
          orderRequestVia: 1,
          paymentMethodId: selectedPMOption,
          stationId: qtySelectedStation?.productStationId,
          discount: discount,
          vat: 0,
          refNo: data?.refNo,
          orderNote: {
            note: note,
          },
          orderAddress: {
            SelectedCarrierId: carrierId?.CarrierId,
            orderAddressId: 0,
            customerName: data.customerName,
            email: data.email,
            mobile1: UtilityClass.getFormatedNumber(data.mobile1),
            mobile2: UtilityClass.getFormatedNumber(data.mobile2),
            countryId: selectedAddressSchema.country,
            cityId: selectedAddressSchema.city,
            areaId: selectedAddressSchema.area,
            streetAddress: selectedAddressSchema.streetAddress,
            streetAddress2: selectedAddressSchema.streetAddress2,
            houseNo: selectedAddressSchema.houseNo,
            buildingName: selectedAddressSchema.buildingName,
            landmark: selectedAddressSchema.landmark,
            addressTypeId: 0,
            provinceId: selectedAddressSchema.province,
            pinCodeId: selectedAddressSchema.pinCode,
            stateId: selectedAddressSchema.state,
            zip: selectedAddressSchema.zip,
            latitude: getValues("lat&long").split(",")[0] || null,
            longitude: getValues("lat&long").split(",")[1] || null,
          },
          settingConfig: metafields[0]?.settingConfig,
          orderTaxes: allClientTax
            .map((tax) => {
              if (tax.Active) {
                return {
                  orderTaxId: "",
                  ClientTaxId: tax.ClientTaxId,
                  taxValue: Number(vatValue[tax.ClientTaxId]),
                };
              }
            })
            .filter((dt) => dt !== undefined),
          orderBoxs: addedOrderBoxes.map(({ clientOrderBoxId }) => ({
            clientOrderBoxId,
          })),
          orderItems: orderItems,
        },
      ],
      IsSaleChannelOrder: false,
      OrderDraftId: 0,
    };
    if (addedOrderBoxes.length == 0) {
      errorNotification("Please Choose Order Box");
      return;
    }
    if (orderItems.length == 0) {
      errorNotification("Please add product to proceed");
      return false;
    }
    return param1;
  };

  const prepareDraftOrderData = (data) => {
    let amount = 0;
    let weight = 0;
    let itemValue = 0;
    let description = [];
    let orderItems = [];
    for (let index = 0; index < selectedProducts.length; index++) {
      let ditem = `${selectedProducts[index].SKU}_${selectedProducts[index].newQuantity}`;
      description.push(ditem);
      itemValue =
        itemValue +
        (selectedProducts[index].newPrice - selectedProducts[index].discount) *
          selectedProducts[index].newQuantity;
      orderItems[index] = {
        productId: selectedProducts[index].ProductId,
        productStockId: selectedProducts[index].ProductStockId,
        productVariantId: selectedProducts[index].ProductVariantId,
        price: selectedProducts[index].newPrice,
        description: ditem,
        remarks: "",
        quantity: selectedProducts[index].newQuantity,
        discount: selectedProducts[index].discount,
        StockSku: selectedProducts[index].SKU,
      };
    }

    amount =
      Number(totalItemPrice) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);
    const param1 = {
      orderList: [
        {
          storeId: data.store.storeId,
          SaleChannelConfigId: data.storeChannel?.id || 0,
          orderTypeId: EnumOrderType.FullFillable,
          orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
          description: description.join(","),
          remarks: remarks,
          amount: amount,
          cShippingCharges: shipping,
          paymentStatusId:
            EnumPaymentMethod.Prepaid == selectedPMOption
              ? EnumPaymentStatus.Paid
              : EnumPaymentStatus.Unpaid,
          weight: data.weight,
          itemValue: itemValue,
          orderRequestVia: 1,
          paymentMethodId: selectedPMOption,
          stationId: qtySelectedStation?.productStationId,
          discount: discount,
          vat: 0,
          refNo: data?.refNo,
          orderNote: {
            note: note,
          },
          orderAddress: {
            SelectedCarrierId: carrierId?.CarrierId,
            orderAddressId: 0,
            customerName: data.customerName,
            email: data.email,
            mobile1: UtilityClass.getFormatedNumber(data.mobile1),
            mobile2: UtilityClass.getFormatedNumber(data.mobile2),
            countryId: selectedAddressSchema.country,
            cityId: selectedAddressSchema.city,
            areaId: selectedAddressSchema.area,
            streetAddress: selectedAddressSchema.streetAddress,
            streetAddress2: selectedAddressSchema.streetAddress2,
            houseNo: selectedAddressSchema.houseNo,
            buildingName: selectedAddressSchema.buildingName,
            landmark: selectedAddressSchema.landmark,
            addressTypeId: 0,
            provinceId: selectedAddressSchema.province,
            pinCodeId: selectedAddressSchema.pinCode,
            stateId: selectedAddressSchema.state,
            zip: selectedAddressSchema.zip,
            latitude: getValues("lat&long").split(",")[0] || null,
            longitude: getValues("lat&long").split(",")[1] || null,
          },
          settingConfig: metafields[0]?.settingConfig,
          orderTaxes: allClientTax
            .map((tax) => {
              if (tax.Active) {
                return {
                  orderTaxId: "",
                  ClientTaxId: tax.ClientTaxId,
                  taxValue: Number(vatValue[tax.ClientTaxId]),
                };
              }
            })
            .filter((dt) => dt !== undefined),
          orderBoxs: addedOrderBoxes.map(({ clientOrderBoxId }) => ({
            clientOrderBoxId,
          })),
          orderItems: orderItems,
        },
      ],
      IsSaleChannelOrder: false,
      OrderDraftId:
        orderData?.result?.orderDraftId > 0
          ? orderData?.result?.orderDraftId
          : 1,
    };
    return param1;
  };
  const prepareDublicateOrderData = (data) => {
    let amount = 0;
    let weight = 0;
    let itemValue = 0;
    let description = [];
    let orderItems = [];
    for (let index = 0; index < selectedProducts.length; index++) {
      let ditem = `${selectedProducts[index].SKU}_${selectedProducts[index].newQuantity}`;
      description.push(ditem);
      itemValue =
        itemValue +
        (selectedProducts[index].newPrice - selectedProducts[index].discount) *
          selectedProducts[index].newQuantity;
      orderItems[index] = {
        productId: selectedProducts[index].ProductId,
        productStockId: selectedProducts[index].ProductStockId,
        productVariantId: selectedProducts[index].ProductVariantId,
        price: selectedProducts[index].newPrice,
        description: ditem,
        remarks: "",
        quantity: selectedProducts[index].newQuantity,
        discount: selectedProducts[index].discount,
        StockSku: selectedProducts[index].SKU,
      };
    }

    amount =
      Number(totalItemPrice) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);
    const param1 = {
      orderList: [
        {
          storeId: data.store.storeId,
          SaleChannelConfigId: data.storeChannel?.id || 0,
          orderTypeId: EnumOrderType.FullFillable,
          orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
          description: description.join(","),
          remarks: remarks,
          amount: amount,
          cShippingCharges: shipping,
          paymentStatusId:
            EnumPaymentMethod.Prepaid == selectedPMOption
              ? EnumPaymentStatus.Paid
              : EnumPaymentStatus.Unpaid,
          weight: data.weight,
          itemValue: itemValue,
          orderRequestVia: 1,
          paymentMethodId: selectedPMOption,
          stationId: qtySelectedStation?.productStationId,
          discount: discount,
          vat: 0,
          refNo: data?.refNo,
          orderNote: {
            note: note,
          },
          orderAddress: {
            SelectedCarrierId: carrierId?.CarrierId,
            orderAddressId: 0,
            customerName: data.customerName,
            email: data.email,
            mobile1: UtilityClass.getFormatedNumber(data.mobile1),
            mobile2: UtilityClass.getFormatedNumber(data.mobile2),
            countryId: selectedAddressSchema.country,
            cityId: selectedAddressSchema.city,
            areaId: selectedAddressSchema.area,
            streetAddress: selectedAddressSchema.streetAddress,
            streetAddress2: selectedAddressSchema.streetAddress2,
            houseNo: selectedAddressSchema.houseNo,
            buildingName: selectedAddressSchema.buildingName,
            landmark: selectedAddressSchema.landmark,
            addressTypeId: 0,
            provinceId: selectedAddressSchema.province,
            pinCodeId: selectedAddressSchema.pinCode,
            stateId: selectedAddressSchema.state,
            zip: selectedAddressSchema.zip,
            latitude: getValues("lat&long").split(",")[0] || null,
            longitude: getValues("lat&long").split(",")[1] || null,
          },
          settingConfig: metafields[0]?.settingConfig,
          orderTaxes: allClientTax
            .map((tax) => {
              if (tax.Active) {
                return {
                  orderTaxId: "",
                  ClientTaxId: tax.ClientTaxId,
                  taxValue: Number(vatValue[tax.ClientTaxId]),
                };
              }
            })
            .filter((dt) => dt !== undefined),
          orderBoxs: addedOrderBoxes.map(({ clientOrderBoxId }) => ({
            clientOrderBoxId,
          })),
          orderItems: orderItems,
        },
      ],
      IsSaleChannelOrder: false,
      OrderDraftId: 1,
    };
    return param1;
  };

  const handleCompleteOrder = async (data) => {
    const param1 = prepareCreateOrderData(data);
    if (param1) {
      setIsLoadingForConfirm(true);
      setSelectedOrderPlaceButtom(EnumOrderPlaceButton.COMPLETE);

      CreateOrder(param1)
        .then(async (res) => {
          if (!res?.data?.isSuccess) {
            errorNotification("Unable create order");
            errorNotification(res?.data?.customErrorMessage);
          } else {
            successNotification("Order create successfully");
            navigate("/draft-order-dashboard");
            if (orderData?.result?.orderDraftId > 0) {
              DeleteOrderDraft(orderData?.result?.orderDraftId);
            }
            const updatedBadgeCount = await handleGetDraftOrdersCount();
            handleDispathUpdateBadge(dispatch, {
              [EnumRoutesUrls.DRAFT_ORDER]: updatedBadgeCount,
            });
          }
        })
        .catch((e) => {
          errorNotification("Unable create order");
        })
        .finally(() => {
          setIsLoadingForConfirm(false);
          setSelectedOrderPlaceButtom(null);
        });
    }
  };

  function prepareData(data) {
    const { orderList, ...rest } = data;
    return {
      ...orderList[0],
      ...rest,
    };
  }

  const handleDublicateDraftOrder = async (data) => {
    const param1 = prepareDublicateOrderData(data);
    setdublicateOrderLoading(true);
    setSelectedOrderPlaceButtom(EnumOrderPlaceButton.DUBLICATE);
    CreateOrder(param1)
      .then(async (res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Unable to Draft order");
          errorNotification(res?.data?.customErrorMessage);
        } else {
          successNotification("Order Draft successfully");
          setselectedPMOption(EnumPaymentMethod.Prepaid);
          handleRestForm();
          navigate("/draft-order-dashboard");
          const updatedBadgeCount = await handleGetDraftOrdersCount();
          handleDispathUpdateBadge(dispatch, {
            [EnumRoutesUrls.DRAFT_ORDER]: updatedBadgeCount,
          });
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to draft order");
      })
      .finally(() => {
        setdublicateOrderLoading(false);
        setSelectedOrderPlaceButtom(null);
      });
  };

  const handleCreateDraftOrder = async (data) => {
    const param1 = prepareDraftOrderData(data);
    const newparam = prepareData(param1);
    setIsLoadingForConfirmAndNewOrder(true);
    setSelectedOrderPlaceButtom(EnumOrderPlaceButton.SAVE);
    if (param1) {
      setIsLoadingForConfirmAndNewOrder(true);
      setSelectedOrderPlaceButtom(EnumOrderPlaceButton.SAVE);
      if (orderData?.result?.orderDraftId > 0) {
        UpdateOrder(newparam)
          .then((res) => {
            if (!res?.data?.isSuccess) {
              UtilityClass.showErrorNotificationWithDictionary(
                res?.data?.errors,
              );
            } else {
              successNotification("Draft Order Update successfully");
              navigate("/draft-order-dashboard");
            }
          })
          .catch((e) => {
            console.log("e", e);
            errorNotification("Unable to update order");
          })
          .finally(() => {
            setIsLoadingForConfirm(false);
            setSelectedOrderPlaceButtom(null);
          });
      } else {
        CreateOrder(param1)
          .then(async (res) => {
            if (!res?.data?.isSuccess) {
              errorNotification("Unable to Draft order");
              errorNotification(res?.data?.customErrorMessage);
            } else {
              const updatedBadgeCount = await handleGetDraftOrdersCount();
              handleDispathUpdateBadge(dispatch, {
                [EnumRoutesUrls.DRAFT_ORDER]: updatedBadgeCount,
              });
              successNotification("Order Draft successfully");
              setselectedPMOption(EnumPaymentMethod.Prepaid);
              handleRestForm();
              navigate("/draft-order-dashboard");
            }
          })
          .catch((e) => {
            console.log("e", e);
            errorNotification("Unable to draft order");
          })
          .finally(() => {
            setIsLoadingForConfirmAndNewOrder(false);
            setSelectedOrderPlaceButtom(null);
          });
      }
    }
  };

  const handleSendInvoice = async (data) => {
    const param1 = prepareCreateOrderData(data);
    if (param1) {
      setIsLoadingForConfirmAndSendInvoiceOrder(true);
      setSelectedOrderPlaceButtom(EnumOrderPlaceButton.ConfirmAndHandleInvoice);

      CreateOrderWithInvoice(param1)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            errorNotification("Unable create order");
            errorNotification(res?.data?.customErrorMessage);
          } else {
            const data = res.data.result;
            setPaymentLinks((prev) => ({
              ...prev,
              show: true,
              paymentUrl: data?.hostedInvoiceUrl,
              invoicePdf: data?.invoicePdf,
            }));
            successNotification("Order create successfully");
            handleRestForm();
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Unable create order");
        })
        .finally(() => {
          setSelectedOrderPlaceButtom(null);
          setIsLoadingForConfirmAndSendInvoiceOrder(false);
        });
    }
  };

  const handleRestForm = () => {
    setNote("");
    setRemarks("");
    setDiscount(0);
    setShipping(0);
    setTotalItemPrice(0);

    reset();
    setValue("mobile1", UtilityClass.getDefaultCountryCode());
    setValue("mobile2", UtilityClass.getDefaultCountryCode());
    setSelectedProducts([]);
  };

  const handleOnchangeProduct = (e, index) => {
    let newSelectedProducts = [...selectedProducts];
    if (parseInt(e.target.value) > 0) {
      newSelectedProducts[index] = {
        ...newSelectedProducts[index],
        newQuantity: parseInt(e.target.value),
        newPrice: Number(e.target.value) * newSelectedProducts[index].Price,
        discount: 0,
      };
      setSelectedProducts(newSelectedProducts);
    }
  };
  const handleFocus = (event) => event.target.select();
  const paymentMethodoptions = [
    { id: 1, label: LanguageReducer?.languageType?.ORDERS_PREPAID },
    { id: 2, label: LanguageReducer?.languageType?.ORDERS_CASH_ON_DELIVERY },
  ];

  const handleCheckboxChange = (optionId) => {
    setselectedPMOption(optionId);
  };
  const handleOnClickDecrement = (e, index) => {
    let newSelectedProducts = [...selectedProducts];
    let newQuantity;
    if (newSelectedProducts[index].newQuantity > 1) {
      newQuantity = newSelectedProducts[index].newQuantity
        ? newSelectedProducts[index].newQuantity - 1
        : newSelectedProducts[index].QuantityAvailable - 1;
      newSelectedProducts[index] = {
        ...newSelectedProducts[index],
        newQuantity: newQuantity,
        newPrice: Number(newQuantity) * newSelectedProducts[index].Price,
        discount: 0,
      };
      setSelectedProducts(newSelectedProducts);
    }
    setIsFirstLoad(false);
  };
  const handleOnClickIncrement = (e, index) => {
    let newSelectedProducts = [...selectedProducts];
    let newQuantity;
    if (
      newSelectedProducts[index].newQuantity <=
      newSelectedProducts[index].QuantityAvailable
    ) {
      newQuantity = newSelectedProducts[index].newQuantity
        ? newSelectedProducts[index].newQuantity + 1
        : newSelectedProducts[index].QuantityAvailable + 1;
      newSelectedProducts[index] = {
        ...newSelectedProducts[index],
        newQuantity: newQuantity,
        newPrice:
          Number(newQuantity) * Number(newSelectedProducts[index].Price),
        discount: 0,
      };
      setSelectedProducts(newSelectedProducts);
    }
    setIsFirstLoad(false);
  };

  const handleChange = (value, inputIndex, metaFieldIndex) => {
    const updatedMetaFields = [...metafields];
    updatedMetaFields[metaFieldIndex].settingConfig[inputIndex].value = value;
    setMetafields(updatedMetaFields);
  };

  useEffect(() => {
    let _totalItemPrice = 0;
    let _totaldiscount = 0;
    const camelValueOrderTax = camelizeArray(order?.OrderTaxes);
    selectedProducts.forEach((data) => {
      _totalItemPrice += Number(data.newPrice);
      _totaldiscount += Number(data.discount);
    });
    if (!orderData) {
      setTotalItemPrice(_totalItemPrice);
      return;
    }
    if (!isFirstLoad) {
      setTotalItemPrice(_totalItemPrice);
    } else {
      setTotalItemPrice(
        calculateSubtotalValue(
          order?.Discount,
          order?.CShippingCharges,
          camelValueOrderTax,
          order?.Amount,
        ),
      );
    }
  }, [selectedProducts]);

  const handleOnClickProduct = () => {
    getProductStocksForSelection();
  };

  const splitLatAndLong = async () => {
    const lat = getValues("lat&long").split(",")[0],
      long = getValues("lat&long").split(",")[1];
    setValue("latitude", lat);
    setValue("longitude", long);

    const body = {
      LATITUDE: lat,
      LONGITUDE: long,
    };
    const { response } = await fetchMethod(() =>
      GetAddressFromLatAndLong(body),
    );
    fetchMethodResponse(response, "Address Updated", () =>
      handleSetSchemaValueForUpdate(response.result, setValue),
    );
  };
  useSetNumericInputEffect([selectedProducts]);

  const totalVolume = addedOrderBoxes.reduce(
    (sum, box) => sum + (box.volume || 0),
    0,
  );

  useEffect(() => {
    setValue("weight", parseFloat(totalVolume.toFixed(2)));
  }, [totalVolume]);

  useEffect(() => {
    const defaultOrderBox = allClientOrderBox.filter(
      (item) => item.isDefault === true,
    );
    setAddedOrderBoxes(defaultOrderBox);
    setSelectedOrderBox(defaultOrderBox);
  }, [allClientOrderBox]);

  useEffect(() => {
    if (productStations?.length > 0 && orderData) {
      let defData = productStations.find(
        (x) => x.productStationId == order?.StationId,
      );
      setValue("Station", defData);
      setQtySelectedStation(defData);
    }
  }, [productStations]);

  useEffect(() => {
    if (allCarriersForSelection?.length > 0 && orderData) {
      let defData = allCarriersForSelection.find(
        (x) => x.CarrierId == order?.OrderAddress?.SelectedCarrierId,
      );
      setCarrierId(defData);
    }
  }, [allCarriersForSelection]);

  useEffect(() => {
    if (storesChannelForSelection?.length > 0 && orderData) {
      let defData = storesChannelForSelection.find(
        (x) => x.id == order?.SaleChannelConfigId,
      );
      setValue("storeChannel", defData);
    }
  }, [storesChannelForSelection]);
  useEffect(() => {
    if (storesForSelection?.length > 0 && orderData) {
      let defData = storesForSelection.find((x) => x.storeId == order?.StoreId);
      setValue("store", defData);
    }
  }, [storesForSelection]);

  useEffect(() => {
    if (allClientOrderBox.length > 0 && orderData) {
      console.log(order);
      const orderBoxIds = order.OrderBoxs.map((box) => box.ClientOrderBoxId);
      const matchingBoxes = allClientOrderBox.filter((x) =>
        orderBoxIds.includes(x.clientOrderBoxId),
      );
      const updatedBoxes = matchingBoxes.map((box) => {
        const orderBox = order.OrderBoxs.find(
          (order) => order.ClientOrderBoxId === box.clientOrderBoxId,
        );
        return {
          ...box,
          orderBoxId: orderBox ? orderBox.orderBoxId : null,
        };
      });
      setAddedOrderBoxes(updatedBoxes);
      setSelectedOrderBox(updatedBoxes);
    }
  }, [allClientOrderBox, orderData]);

  useEffect(() => {
    if (getValues("Station")?.productStationId > 0) {
      if (order?.OrderItems?.length > 0) {
        getProductStocksForSelection();
      }
    }
  }, [getValues("Station"), orderData]);

  useEffect(() => {
    if (!orderData) return;

    if (orderData) {
      let data = orderData?.result?.orderInfo;
      const order = JSON.parse(data);
      setValue("numberOfPieces", order?.OrderItems?.length || 0);
      setValue("country", order?.OrderAddress?.CountryId);
      setValue("description", order.Description);
      setRemarks(order.Remarks);
      setValue("refNo", order.RefNo);
      setValue("customerName", order.OrderAddress.CustomerName);
      setValue("email", order.OrderAddress.Email);
      setValue("refNo", order.RefNo);
      setDiscount(order?.Discount);
      handleCheckboxChange(order?.PaymentMethodId);
      setNote(order?.OrderNote?.Note);
      setShipping(order?.CShippingCharges);
      if (order?.OrderItems) {
        order?.OrderItems.forEach((item, i) => {
          setValue(`descriptionOfPieces.${i}`, item.Description);
          setValue(`priceOfPieces.${i}`, item.Price);
        });
      }
      setValue("mobile1", null);
      setValue("mobile2", null);
      let updatedOrder;
      if (order.OrderAddress) {
        const {
          Country,
          City,
          Area,
          Province,
          State,
          StreetAddress,
          StreetAddress2,
          BuildingName,
          Landmark,
          HouseNo,
          ...restOrderAddress
        } = order.OrderAddress;
        if (order.OrderAddress.SelectedCarrierId > 0) {
          updatedOrder = {
            ...order,
            OrderAddress: {
              ...restOrderAddress,
              country: Country,
              city: City,
              area: Area,
              province: Province,
              state: State,
              streetAddress: StreetAddress,
              streetAddress2: StreetAddress2,
              buildingName: BuildingName,
              landmark: Landmark,
              houseNo: HouseNo,
            },
          };
        } else {
          const splitCity = City?.split("_")[0] || "";
          const splitArea = Area?.split("_")[0] || "";
          const splitprovince = Province?.split("_")[0] || "";
          const splitstate = State?.split("_")[0] || "";
          updatedOrder = {
            ...order,
            OrderAddress: {
              ...restOrderAddress,
              country: Country,
              city: Number(splitCity),
              area: Number(splitArea),
              province: Number(splitprovince),
              state: Number(splitstate),
              streetAddress: StreetAddress,
              streetAddress2: StreetAddress2,
              buildingName: BuildingName,
              landmark: Landmark,
              houseNo: HouseNo,
            },
          };
        }
      }
      if (updatedOrder) {
        handleSetSchemaValueForUpdate(
          updatedOrder.OrderAddress,
          setValue,
          order.OrderAddress?.SelectedCarrierId,
        );
      }
      setValue("mobile1", order.OrderAddress.Mobile1);
      setValue("mobile2", order.OrderAddress.Mobile2);
      let latitude = order.OrderAddress?.Latitude;
      let longitude = order.OrderAddress?.Longitude;
      if (latitude !== null && longitude !== null) {
        setValue("lat&long", `${latitude},${longitude}`);
      }
    }
  }, [orderData]);

  return (
    <Box
      sx={{
        ...styleSheet.pageRoot,
        // "& .MuiFormLabel-asterisk": {
        //   color: "red !important",
        // },
      }}
    >
      <form>
        {/* <Container maxWidth="lg" fixed sx={{ paddingLeft: "0px" }}> */}
        <div style={{ padding: "10px" }}>
          <Box
            sx={{
              display: "flex",
              justifyContent: orderData ? "space-between" : "end",
              alignItems: "center",
            }}
          >
            {orderData && (
              <Box
                sx={{
                  backgroundColor: "rgb(0, 200, 83)",
                  border: "1px solid rgb(0, 200, 83)",
                  borderRadius: "4px",
                  padding: "4px 12px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <Typography
                  variant="body1"
                  sx={{ color: "#fff", fontSize: "14px" }}
                >
                  {`Order No: ${orderData?.result?.orderNo}`}
                </Typography>
              </Box>
            )}
            <Box sx={{ display: "flex", gap: 1 }}>
              <ActionButtonCustom
                onClick={() => {
                  navigate("/draft-regular-order");
                }}
                variant="contained"
                label={"Draft Regular Order"}
              />
              <ActionButtonCustom
                onClick={() => {
                  navigate("/draft-order-dashboard");
                }}
                variant="contained"
                label={"Draft Order Dashboard"}
              />
            </Box>
          </Box>
          <GridContainer>
            <GridItem md={7} sm={12} xs={12}>
              <GridContainer>
                <GridItem xs={12}>
                  {" "}
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_STORE}
                  >
                    <Grid container spacing={1}>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel
                          required={formMode === EnumDraftOrder.complete}
                          sx={styleSheet.inputLabel}
                        >
                          {LanguageReducer?.languageType?.ORDERS_STORE}
                        </InputLabel>
                        <SelectComponent
                          name="store"
                          control={control}
                          options={storesForSelection}
                          isRHF={true}
                          required={true}
                          optionLabel={EnumOptions.STORE.LABEL}
                          optionValue={EnumOptions.STORE.VALUE}
                          isRefesh={true}
                          handleRefreshClick={getStoresForSelection}
                          {...register(
                            "store",
                            formMode === EnumDraftOrder.complete
                              ? {
                                  required: {
                                    value: true,
                                  },
                                }
                              : {},
                          )}
                          value={getValues("store")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("store", resolvedId);
                            setValue("Station", productStations[0]);
                            setQtySelectedStation(productStations[0]);
                            if (
                              selectedPMOption !== EnumPaymentMethod.Prepaid
                            ) {
                              setStripeSettingExist(true);
                            }
                          }}
                          errors={errors}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDER_SALE_CHANNEL}
                        </InputLabel>
                        <SelectComponent
                          name="storeChannel"
                          control={control}
                          isRHF={true}
                          required={false}
                          options={storesChannelForSelection}
                          getOptionLabel={(option) => option?.text}
                          optionLabel={EnumOptions.STORE_CHANNEL.LABEL}
                          optionValue={EnumOptions.STORE_CHANNEL.VALUE}
                          isRefesh={true}
                          {...register("storeChannel", {
                            // required: {
                            //   value: true,
                            // },
                          })}
                          disabled={!getValues("store")}
                          value={getValues("storeChannel")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("storeChannel", resolvedId);
                          }}
                          // errors={errors}
                        />
                      </Grid>
                      {getValues("store")?.storeId &&
                      getValues("store")?.storeId > 0 ? (
                        <Grid item md={6} sm={6} xs={12}>
                          <InputLabel
                            required={formMode === EnumDraftOrder.complete}
                            sx={styleSheet.inputLabel}
                          >
                            {LanguageReducer?.languageType?.ORDERS_STATIONS}
                          </InputLabel>
                          <SelectComponent
                            name="qtySelectedStation"
                            options={productStations}
                            isRHF={true}
                            required={true}
                            optionLabel={EnumOptions.SELECT_STATION.LABEL}
                            optionValue={EnumOptions.SELECT_STATION.VALUE}
                            isRefesh={true}
                            handleRefreshClick={getAllStationLookup}
                            {...register(
                              "Station",
                              formMode === EnumDraftOrder.complete
                                ? {
                                    required: {
                                      value: true,
                                    },
                                  }
                                : {},
                            )}
                            value={getValues("Station")}
                            onChange={(event, newValue) => {
                              const resolvedId = newValue ? newValue : null;
                              setValue("Station", resolvedId);
                              setQtySelectedStation(resolvedId);
                            }}
                            errors={errors}
                          />
                        </Grid>
                      ) : null}
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {LanguageReducer?.languageType?.ORDER_CARRIER}
                        </InputLabel>
                        <SelectComponent
                          name="carrier"
                          control={control}
                          options={allCarriersForSelection}
                          value={carrierId}
                          optionLabel={EnumOptions.ALL_CARRIER.LABEL}
                          optionValue={EnumOptions.ALL_CARRIER.VALUE}
                          isRefesh={true}
                          handleRefreshClick={
                            getAllActiveCarrierForCreateOrderSelection
                          }
                          onChange={(e, val) => {
                            setCarrierId(val);
                            handleSetSchema(
                              "country",
                              getValues("country"),
                              setValue,
                              unregister,
                              val?.CarrierId,
                              val?.AddressingScheme,
                            );
                          }}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_ORDER_DATE}
                        </InputLabel>
                        <CustomRHFReactDatePickerInput
                          name="orderDate"
                          control={control}
                          // onChange={handleOnChange}
                          required
                          error={
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT
                          }
                          defaultValue={new Date()}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_REF_NO}
                        </InputLabel>
                        <TextField
                          type="text"
                          placeholder={placeholders.refNo}
                          onFocus={handleFocus}
                          size="small"
                          fullWidth
                          variant="outlined"
                          id="refNo"
                          name="refNo"
                          {...register("refNo")}
                        />
                      </Grid>
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
                {true ? (
                  <GridItem xs={12}>
                    <CustomColorLabelledOutline
                      isCollapse={true}
                      label={LanguageReducer?.languageType?.PRODUCTS_TEXT}
                    >
                      <Box textAlign={"right"}>
                        <ActionButtonCustom
                          loading={isProductFetching}
                          startIcon={<AddBoxIcon fontSize="small" />}
                          label={"Choose Products"}
                          onClick={() => handleOnClickProduct()}
                          // sx={styleSheet.placeOrderButton}
                        />
                        {/* <Button
                      startIcon={<AddBoxIcon fontSize="small" />}
                      onClick={() => handleOnClickProduct()}
                      sx={styleSheet.placeOrderButton}
                    >
                      Choose Products
                    </Button> */}
                      </Box>
                      <Box sx={{ overflow: "auto" }}>
                        <Box
                          sx={{
                            width: "100%",
                            display: "table",
                            tableLayout: "fixed",
                          }}
                        >
                          <Table size="small">
                            <TableHead>
                              <TableRow
                                sx={{ ...styleSheet.orderProductHeading }}
                              >
                                <TableCell sx={{ fontWeight: "bold", p: 1 }}>
                                  {LanguageReducer?.languageType?.PRODUCT_TEXT}
                                </TableCell>
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 50,
                                  }}
                                  align="center"
                                >
                                  {"Comm. Qty"}
                                </TableCell>
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 50,
                                  }}
                                  align="center"
                                >
                                  {"Avl. Qty"}
                                </TableCell>
                                <TableCell
                                  sx={{ fontWeight: "bold", p: 1 }}
                                  align="center"
                                >
                                  {"Qty"}
                                </TableCell>

                                <TableCell sx={{ fontWeight: "bold", p: 1 }}>
                                  {"Discount"}
                                </TableCell>
                                <TableCell
                                  sx={{ fontWeight: "bold", p: 1 }}
                                  align="right"
                                >
                                  {"Total"}
                                </TableCell>
                                <TableCell
                                  sx={{ fontWeight: "bold", p: 1 }}
                                ></TableCell>
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {selectedProducts.map((value, index) => {
                                return (
                                  <TableRow key={index}>
                                    <TableCell sx={{ p: 0.5 }}>
                                      <Box
                                        display={"flex"}
                                        gap={1}
                                        alignItems={"center"}
                                      >
                                        <Avatar
                                          sx={{
                                            width: 60,
                                            height: 60,
                                            flexShrink: 0,
                                          }}
                                          variant="rounded"
                                          src={value.FeatureImage}
                                        />
                                        <Box flexShrink={0}>
                                          <Typography
                                            variant="h5"
                                            fontWeight={300}
                                          >
                                            {truncate(value.ProductName)}
                                          </Typography>
                                          <Typography
                                            variant="h6"
                                            fontWeight={300}
                                          >
                                            <DataGridRenderGreyBox
                                              title={truncate(
                                                value.VarientOption,
                                                25,
                                              )}
                                            />
                                          </Typography>

                                          <Box
                                            sx={styleSheet.productItemBoxPrice}
                                          >
                                            {
                                              LanguageReducer?.languageType
                                                ?.AED_TEXT
                                            }{" "}
                                            {amountFormat(value.Price)}
                                          </Box>
                                        </Box>
                                      </Box>
                                    </TableCell>
                                    <TableCell sx={{ p: 0.5 }} align="center">
                                      <b>{value.QuantityCommited}</b>
                                    </TableCell>
                                    <TableCell sx={{ p: 0.5 }} align="center">
                                      <b>{value.QuantityAvailable}</b>
                                    </TableCell>

                                    <TableCell sx={{ p: 0.5 }} align="center">
                                      <Box className={"flex_center"}>
                                        <Box
                                          sx={styleSheet.qualityIncrementBox}
                                        >
                                          <IndeterminateCheckBoxOutlinedIcon
                                            onClick={(e) =>
                                              handleOnClickDecrement(e, index)
                                            }
                                          />
                                          <Box>
                                            {/* <b>
                                    {value.newQuantity
                                      ? value.newQuantity
                                      : value.QuantityAvailable}
                                  </b> */}
                                            <TextField
                                              inputProps={{
                                                style: {
                                                  width: "30px",
                                                  padding: "0px",
                                                  textAlign: "center",
                                                },
                                              }}
                                              fullWidth
                                              min={0}
                                              type="number"
                                              placeholder={
                                                placeholders.quantity
                                              }
                                              onFocus={handleFocus}
                                              size="small"
                                              value={
                                                value.newQuantity
                                                  ? value.newQuantity
                                                  : value.QuantityAvailable
                                              }
                                              onChange={(e) =>
                                                handleOnchangeProduct(e, index)
                                              }
                                            />
                                          </Box>
                                          <AddBoxOutlinedIcon
                                            onClick={(e) =>
                                              handleOnClickIncrement(e, index)
                                            }
                                          />
                                        </Box>
                                      </Box>
                                    </TableCell>

                                    <TableCell sx={{ p: 0.5 }}>
                                      <Box sx={styleSheet.productItemBox}>
                                        <TextField
                                          size="small"
                                          type="number"
                                          onFocus={handleFocus}
                                          fullWidth
                                          InputProps={{
                                            step: "any",
                                            inputProps: {
                                              min: 0,
                                              max: selectedProducts[index]
                                                .newPrice,
                                            },
                                          }}
                                          sx={{
                                            color: "white",
                                            width: "50px",
                                            border: "none",
                                            background: "transparent",
                                            "& .MuiInputBase-root": {
                                              height: 30,
                                            },
                                          }}
                                          value={value.discount}
                                          defaultValue={value.discount}
                                          onChange={(e) => {
                                            let newSelectedProducts = [
                                              ...selectedProducts,
                                            ];
                                            newSelectedProducts[index] = {
                                              ...newSelectedProducts[index],
                                              discount:
                                                parseInt(e.target.value) || 0,
                                              newPrice:
                                                newSelectedProducts[index]
                                                  .Price *
                                                  newSelectedProducts[index]
                                                    .newQuantity -
                                                (parseInt(e.target.value) || 0),
                                            };
                                            setSelectedProducts(
                                              newSelectedProducts,
                                            );
                                            setIsFirstLoad(false);
                                          }}
                                        />
                                      </Box>
                                    </TableCell>
                                    <TableCell sx={{ p: 0.5 }} align="right">
                                      <PaymentAmountTextField
                                        value={selectedProducts[index].newPrice}
                                        onChange={(e) => {
                                          let newSelectedProducts = [
                                            ...selectedProducts,
                                          ];
                                          newSelectedProducts[index] = {
                                            ...newSelectedProducts[index],
                                            newPrice: e.target.value,
                                            discount: 0,
                                          };
                                          setSelectedProducts(
                                            newSelectedProducts,
                                          );
                                        }}
                                        width={118}
                                      />
                                    </TableCell>

                                    <TableCell sx={{ p: 0.5 }} align="right">
                                      <Button
                                        onClick={() => {
                                          value.checked = false;
                                          setSelectedProducts(
                                            selectedProducts.filter(
                                              (item) =>
                                                item.ProductStockId !==
                                                value.ProductStockId,
                                            ),
                                          );
                                        }}
                                        sx={styleSheet.deleteProductButton}
                                        variant="outlined"
                                      >
                                        <DeleteIcon />
                                      </Button>
                                    </TableCell>
                                  </TableRow>
                                );
                              })}
                            </TableBody>
                          </Table>
                        </Box>
                      </Box>
                    </CustomColorLabelledOutline>
                  </GridItem>
                ) : null}
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_ORDER_BOX}
                  >
                    <Grid item md={12} xs={12} mb={1}>
                      <InputLabel sx={styleSheet.inputLabel}>
                        {LanguageReducer?.languageType?.ORDERS_TOTAL_WEIGHT}
                      </InputLabel>
                      <TextField
                        placeholder={placeholders.quantity}
                        onFocus={handleFocus}
                        type="number"
                        size="small"
                        id="weight"
                        name="weight"
                        fullWidth
                        inputProps={{
                          step: "any",
                        }}
                        variant="outlined"
                        {...register("weight", {})}
                        error={Boolean(errors.weight)} // set error prop
                        helperText={errors.weight?.message}
                      />
                    </Grid>
                    <Box textAlign={"right"}>
                      <ActionButtonCustom
                        startIcon={<AddBoxIcon fontSize="small" />}
                        label={"Choose Order Box"}
                        onClick={() =>
                          setopenOrderBoxModal((prev) => ({
                            ...prev,
                            open: true,
                          }))
                        }
                      />
                      <Box sx={{ overflow: "auto" }}>
                        <Box
                          sx={{
                            width: "100%",
                            display: "table",
                            tableLayout: "fixed",
                          }}
                        >
                          <Table size="small">
                            <TableHead>
                              <TableRow
                                sx={{ ...styleSheet.orderProductHeading }}
                              >
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 40,
                                  }}
                                >
                                  {"Box"}
                                </TableCell>
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 40,
                                  }}
                                >
                                  {"Length"}
                                </TableCell>
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 40,
                                  }}
                                >
                                  {"Width"}
                                </TableCell>

                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 40,
                                  }}
                                >
                                  {"Height"}
                                </TableCell>
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 40,
                                  }}
                                >
                                  {"Action"}
                                </TableCell>
                                <TableCell
                                  sx={{ fontWeight: "bold", p: 1 }}
                                ></TableCell>
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {addedOrderBoxes?.map((box) => (
                                <TableRow key={box.clientOrderBoxId}>
                                  <TableCell sx={{ p: 1 }}>
                                    {box.boxName}
                                  </TableCell>
                                  <TableCell sx={{ p: 1 }}>
                                    {box.length}
                                  </TableCell>
                                  <TableCell sx={{ p: 1 }}>
                                    {box.width}
                                  </TableCell>
                                  <TableCell sx={{ p: 1 }}>
                                    {box.height}
                                  </TableCell>
                                  <TableCell sx={{ p: 1 }}>
                                    <Button
                                      sx={styleSheet.deleteProductButton}
                                      variant="outlined"
                                      onClick={() =>
                                        handleDeleteOrderBox(
                                          box.clientOrderBoxId,
                                        )
                                      }
                                      aria-label={`Delete box ${box.boxName}`}
                                    >
                                      <DeleteIcon />
                                    </Button>
                                  </TableCell>
                                </TableRow>
                              ))}
                            </TableBody>
                          </Table>
                        </Box>
                      </Box>
                    </Box>
                  </CustomColorLabelledOutline>
                </GridItem>
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_CUSTOMER}
                  >
                    <Grid container spacing={2}>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel
                          required={formMode === EnumDraftOrder.complete}
                          sx={styleSheet.inputLabel}
                        >
                          {LanguageReducer?.languageType?.ORDERS_CUSTOMER_NAME}
                        </InputLabel>
                        <TextField
                          type="text"
                          placeholder={placeholders.name}
                          onFocus={handleFocus}
                          size="small"
                          fullWidth
                          // onChange={(e) => { setOrderAddress({ ...orderAddress, customerName: e.target.value }) }}
                          variant="outlined"
                          id="customerName"
                          name="customerName"
                          {...register(
                            "customerName",
                            formMode === EnumDraftOrder.complete
                              ? {
                                  required: {
                                    value: true,
                                    message:
                                      LanguageReducer?.languageType
                                        ?.FIELD_REQUIRED_TEXT,
                                  },
                                  pattern: {
                                    value: /^(?!\s*$).+/,
                                    message:
                                      LanguageReducer?.languageType
                                        ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                                  },
                                }
                              : {},
                          )}
                          error={Boolean(errors.customerName)} // set error prop
                          helperText={errors.customerName?.message}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_EMAIL}{" "}
                        </InputLabel>
                        <TextField
                          placeholder={placeholders.email}
                          onFocus={handleFocus}
                          type="email"
                          size="small"
                          id="email"
                          name="email"
                          fullWidth
                          variant="outlined"
                          {...register("email", {
                            pattern: {
                              value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i, // set pattern to match email format
                              message:
                                LanguageReducer?.languageType
                                  ?.INVALID_EMAIL_TOAST,
                            },
                          })}
                          error={Boolean(errors.email)} // set error prop
                          helperText={errors.email?.message}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel
                          required={formMode === EnumDraftOrder.complete}
                          sx={styleSheet.inputLabel}
                        >
                          {LanguageReducer?.languageType?.ORDERS_PHONE_NO}
                        </InputLabel>
                        <CustomRHFPhoneInput
                          error={
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT
                          }
                          name="mobile1"
                          control={control}
                          required={formMode === EnumDraftOrder.complete}
                          borderRadius={"2px"}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_CONTACT_NUMBER}
                        </InputLabel>
                        <CustomRHFPhoneInput
                          error={
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT
                          }
                          name="mobile2"
                          control={control}
                          borderRadius={"2px"}
                          isContact={true}
                        />
                      </Grid>
                      <Grid
                        item
                        md={schemaFieldsLength === 0 ? 12 : 4}
                        sm={6}
                        xs={12}
                      >
                        <InputLabel
                          required={formMode === EnumDraftOrder.complete}
                          sx={styleSheet.inputLabel}
                        >
                          {LanguageReducer?.languageType?.ORDERS_COUNTRY}
                        </InputLabel>
                        <CountrySchema
                          name="country"
                          control={control}
                          isRHF={true}
                          required={true}
                          getOptionLabel={(option) => option?.name}
                          isRefesh={true}
                          handleRefreshClick={getAllCountry}
                          {...register(
                            "country",
                            formMode === EnumDraftOrder.complete
                              ? {
                                  required: {
                                    value: true,
                                  },
                                }
                              : {},
                          )}
                          value={getValues("country")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setCarrierId(null);
                            handleSetSchema(
                              "country",
                              resolvedId,
                              setValue,
                              unregister,
                              carrierId?.CarrierId,
                            );
                          }}
                          errors={errors}
                        />
                      </Grid>
                      {[
                        ...addressSchemaSelectData,
                        ...addressSchemaInputData,
                      ].map((input, index, arr) => (
                        <Grid item md={4} sm={6} xs={12}>
                          <SchemaTextField
                            loading={input.loading}
                            disabled={input.disabled}
                            isRHF={true}
                            type={input.type}
                            name={input.key}
                            required={
                              formMode === EnumDraftOrder.draft
                                ? false
                                : checkRequiredUsingCarrierAdressSchema(
                                    carrierId,
                                    input.key,
                                    input.required,
                                  )
                            }
                            optionLabel={addressSchemaEnum[input.key]?.LABEL}
                            optionValue={addressSchemaEnum[input.key]?.VALUE}
                            register={register}
                            options={input.options}
                            errors={errors}
                            label={input.label}
                            value={getValues(input.key)}
                            showMoreInfoBtn={index + 1 === arr.length}
                            btnFlag={showMoreInfoBtn}
                            handleClickShowMoreInfoBtn={(val) =>
                              handleAddRemoveMoreInfoFields(val, setValue)
                            }
                            onChange={
                              getLowerCase(input.type) === inputTypesEnum.SELECT
                                ? (name, value) => {
                                    handleChangeSelectAddressSchemaAndGetOptions(
                                      input.key,
                                      index,
                                      value,
                                      setValue,
                                      input.key,
                                    );
                                  }
                                : (e) => {
                                    handleChangeInputAddressSchema(
                                      input.key,
                                      e.target.value,
                                      setValue,
                                    );
                                  }
                            }
                          />
                        </Grid>
                      ))}
                      <GridContainer>
                        <Grid item md={6} sm={6} xs={12}>
                          <InputLabel sx={styleSheet.inputLabel}>
                            {
                              LanguageReducer?.languageType
                                ?.ORDERS_LATITUDE_AND_LONGITUDE
                            }
                          </InputLabel>
                          <CustomLatLongTextField
                            name="lat&long"
                            required={false}
                            register={register}
                            errors={errors}
                          />
                        </Grid>
                        <Grid
                          item
                          md={6}
                          sm={6}
                          xs={12}
                          alignSelf="end"
                          display="flex"
                          gap={2}
                        >
                          <ActionButtonCustom
                            onClick={splitLatAndLong}
                            startIcon={<FmdGoodOutlinedIcon />}
                            disabled={!getValues("lat&long")?.includes(",")}
                            label={
                              LanguageReducer?.languageType?.ORDERS_GET_ADDRESS
                            }
                            height={styleSheet.fromMapButton}
                          />
                          <ActionButtonCustom
                            onClick={() => setOpenLocationModal(true)}
                            startIcon={<FmdGoodOutlinedIcon />}
                            label={
                              LanguageReducer?.languageType?.ORDERS_FORM_MAP
                            }
                            height={styleSheet.fromMapButton}
                          />
                        </Grid>
                        {/* <Grid item md={3} sm={3} xs={12}>
                          <Button
                            fullWidth
                            onClick={() => setOpenFromLinkLocationModal(true)}
                            sx={{
                              ...styleSheet.currentLocationButton,
                              marginTop: "15px",
                            }}
                            variant="contained"
                          >
                            {" "}
                            <FmdGoodOutlinedIcon />
                            {LanguageReducer?.languageType?.FROM_LINK_TEXT}
                          </Button>
                        </Grid> */}
                      </GridContainer>
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
              </GridContainer>
              {/* MetaField */}
              <GridItem xs={12}>
                {metafields?.length > 0 && (
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={"MetaField"}
                  >
                    <Grid container spacing={2} sx={{ paddingTop: 2 }}>
                      {metafields.map((meta, metaFieldIndex) =>
                        meta.settingConfig.map((input, input_index) => (
                          <Grid
                            item
                            xs={12}
                            sm={6}
                            key={input.name}
                            paddingTop={"0px!important"}
                          >
                            <Box marginBottom={1}>
                              <InputLabel
                                required={input.required}
                                sx={styleSheet.inputLabel}
                              >
                                {input?.name}
                              </InputLabel>

                              {/* SELECT FIELD */}
                              {getLowerCase(input.type.label) ===
                                inputTypesEnum.SELECT && (
                                <SelectComponent
                                  height={40}
                                  name={input.name}
                                  options={input.selectOptions}
                                  optionLabel="label"
                                  optionValue="id"
                                  value={input.value}
                                  onChange={(e, newValue) =>
                                    handleChange(
                                      newValue,
                                      input_index,
                                      metaFieldIndex,
                                    )
                                  }
                                />
                              )}

                              {/* TEXT / NUMBER / DATE FIELD */}
                              {(getLowerCase(input.type.label) ===
                                inputTypesEnum.TEXT ||
                                getLowerCase(input.type.label) ===
                                  inputTypesEnum.NUMBER ||
                                getLowerCase(input.type.label) ===
                                  inputTypesEnum.DATE) && (
                                <TextField
                                  type={getLowerCase(input.type.label)}
                                  placeholder={input.description}
                                  size="small"
                                  fullWidth
                                  variant="outlined"
                                  required={input.required}
                                  value={input.value || ""}
                                  onChange={(e) =>
                                    handleChange(
                                      e.target.value,
                                      input_index,
                                      metaFieldIndex,
                                    )
                                  }
                                />
                              )}

                              {/* CHECKBOX FIELD */}
                              {getLowerCase(input.type.label) ===
                                inputTypesEnum.CHECKBOX && (
                                <FormControlLabel
                                  control={
                                    <Checkbox
                                      sx={{
                                        color: "var(--primary-color)",
                                        "&.Mui-checked": {
                                          color: "var(--primary-color)",
                                        },
                                      }}
                                      checked={
                                        input.value ??
                                        input.defaultValue ??
                                        false
                                      }
                                      onChange={(e) =>
                                        handleChange(
                                          e.target.checked,
                                          input_index,
                                          metaFieldIndex,
                                        )
                                      }
                                    />
                                  }
                                />
                              )}
                            </Box>
                          </Grid>
                        )),
                      )}
                    </Grid>
                  </CustomColorLabelledOutline>
                )}
              </GridItem>
            </GridItem>
            <GridItem md={5} sm={12} xs={12}>
              <GridContainer>
                <GridItem md={12} sm={12} xs={12}>
                  {" "}
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_PAYMENT}
                  >
                    <Box display={"flex"} flexDirection={"column"} gap={1}>
                      <PaymentAmountBox
                        title={LanguageReducer?.languageType?.ORDERS_SUBTOTAL}
                        value={decimalFormat(totalItemPrice)}
                        onChange={(e) => {
                          const _totalItemPrice = Number(e.target.value);
                          setTotalItemPrice(_totalItemPrice);
                        }}
                      />
                      <PaymentAmountBox
                        title={
                          LanguageReducer?.languageType?.ORDERS_ADD_DISCOUNT
                        }
                        value={discount}
                        onChange={(e) => {
                          setDiscount(Number(e.target.value));
                        }}
                      />
                      <PaymentAmountBox
                        title={
                          LanguageReducer?.languageType?.ORDERS_ADD_SHIPPING
                        }
                        value={shipping}
                        onChange={(e) => {
                          setShipping(Number(e.target.value));
                        }}
                      />
                      {!loading &&
                        (allClientTax.length === 0 ||
                          allClientTax.every(
                            (item) => item.Active === false,
                          )) && (
                          <PaymentTaxAlert
                            onClick={() =>
                              handleDispatchUserProfile(
                                dispatch,
                                true,
                                navigate,
                              )
                            }
                          />
                        )}
                      {loading ? (
                        <Box className={"flex_center"}>
                          <CircularProgress size={24} />
                        </Box>
                      ) : (
                        allClientTax.map((_tax) => {
                          if (_tax.Active === true) {
                            return (
                              <PaymentTaxBox
                                title={_tax.Name}
                                value={_tax.Percentage}
                                getTaxValue={(val) => {
                                  setVatValue((prev) => ({
                                    ...prev,
                                    [_tax.ClientTaxId]: val,
                                  }));
                                }}
                                subtotal={totalItemPrice}
                              />
                            );
                          }
                        })
                      )}
                      <PaymentTotalBox
                        value={(
                          Number(totalItemPrice) +
                          calculateVatValue(vatValue) +
                          Number(shipping) -
                          Number(discount)
                        ).toFixed(2)}
                      />
                      <Divider />

                      <PaymentMethodBox
                        options={
                          <Box className={"flex_between"}>
                            {paymentMethodoptions.map((option) => (
                              <PaymentMethodCheckbox
                                key={option.id}
                                checked={selectedPMOption === option.id}
                                onChange={() => handleCheckboxChange(option.id)}
                                label={option.label}
                              />
                            ))}
                          </Box>
                        }
                      />
                    </Box>

                    <Grid
                      justifyContent={"flex-end"}
                      alignItems="center"
                      sx={{ mt: 1 }}
                      container
                      spacing={1}
                    >
                      {orderData?.result?.orderDraftId > 0 && (
                        <Grid item>
                          <ActionButtonCustom
                            onClick={() => {
                              setFormMode(EnumDraftOrder.draft);
                              handleDublicateDraftOrder(getValues());
                            }}
                            variant="contained"
                            loading={dublicateOrderLoading}
                            background={Colors.linkColor}
                            loadingPosition="start"
                            disabled={
                              selectedOrderPlaceButtom &&
                              selectedOrderPlaceButtom !==
                                EnumOrderPlaceButton.DUBLICATE
                            }
                            label={"Duplicate"}
                          />
                        </Grid>
                      )}
                      <Grid item>
                        <ActionButtonCustom
                          onClick={() => {
                            setFormMode(EnumDraftOrder.draft);
                            handleCreateDraftOrder(getValues());
                          }}
                          variant="contained"
                          loading={isLoadingForConfirmAndNewOrder}
                          background={Colors.succes}
                          loadingPosition="start"
                          disabled={
                            selectedOrderPlaceButtom &&
                            selectedOrderPlaceButtom !==
                              EnumOrderPlaceButton.SAVE
                          }
                          label={"Save"}
                        />
                      </Grid>
                      <Grid item>
                        <ActionButtonCustom
                          onClick={() => {
                            setFormMode(EnumDraftOrder.complete);
                            handleSubmit(handleCompleteOrder)();
                          }}
                          variant="contained"
                          loading={isLoadingForConfirm}
                          loadingPosition="start"
                          disabled={
                            selectedOrderPlaceButtom &&
                            selectedOrderPlaceButtom !==
                              EnumOrderPlaceButton.COMPLETE
                          }
                          label={"Complete"}
                        />
                      </Grid>

                      {isStripeSettingExist &&
                        selectedPMOption !== EnumPaymentMethod.Prepaid && (
                          <Grid item>
                            {paymentLinks.show ? (
                              <Box className={"flex_center"} gap={1}>
                                <LoadingButton
                                  onClick={() =>
                                    handleCopyToClipBoard(
                                      paymentLinks.paymentUrl,
                                    )
                                  }
                                  sx={{
                                    ...styleSheet.placeOrderButton,
                                    background: Colors.succes,
                                    border: "none",
                                  }}
                                  variant="contained"
                                >
                                  {"Copy Payment Url"}
                                </LoadingButton>
                                <LoadingButton
                                  onClick={() =>
                                    handleCopyToClipBoard(
                                      paymentLinks.paymentUrl,
                                    )
                                  }
                                  sx={{
                                    ...styleSheet.placeOrderButton,
                                    background: Colors.danger,
                                    border: "none",
                                  }}
                                  variant="contained"
                                >
                                  {"Copy Invoice Pdf Url"}
                                </LoadingButton>
                                <CrossIconButton
                                  onClick={() =>
                                    setPaymentLinks((prev) => ({
                                      ...prev,
                                      show: false,
                                    }))
                                  }
                                />
                              </Box>
                            ) : (
                              <LoadingButton
                                sx={styleSheet.sendInvoiceButton}
                                variant="outlined"
                                loading={isLoadingForConfirmAndSendInvoiceOrder}
                                loadingPosition="start"
                                onClick={handleSubmit(handleSendInvoice)}
                                disabled={
                                  selectedOrderPlaceButtom &&
                                  selectedOrderPlaceButtom !==
                                    EnumOrderPlaceButton.ConfirmAndHandleInvoice
                                }
                              >
                                Confirm And Get Payment Link
                              </LoadingButton>
                            )}
                          </Grid>
                        )}
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
                <GridItem xs={12} sm={12}>
                  {" "}
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_NOTE}
                  >
                    <FormControl
                      fullWidth
                      variant="outlined"
                      sx={{ marginTop: "5px" }}
                      size="small"
                    >
                      <OutlinedInput
                        id="outlined-adornment-weight"
                        aria-describedby="outlined-weight-helper-text"
                        inputProps={{
                          "aria-label": "weight",
                        }}
                        onChange={(e) => {
                          setNote(e.target.value);
                        }}
                        value={note}
                        onFocus={handleFocus}
                        placeholder={"Describe Here"}
                      />
                    </FormControl>
                  </CustomColorLabelledOutline>
                </GridItem>
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_REMARKS}
                  >
                    <FormControl
                      fullWidth
                      variant="outlined"
                      sx={{ marginTop: "5px" }}
                      size="small"
                    >
                      <OutlinedInput
                        id="outlined-adornment-weight"
                        aria-describedby="outlined-weight-helper-text"
                        inputProps={{
                          "aria-label": "weight",
                        }}
                        onChange={(e) => {
                          setRemarks(e.target.value);
                        }}
                        value={remarks}
                        onFocus={handleFocus}
                        placeholder={"Remarks here"}
                      />
                    </FormControl>
                  </CustomColorLabelledOutline>
                </GridItem>
              </GridContainer>
            </GridItem>
          </GridContainer>
        </div>
      </form>
      {open && (
        <AddProductModal
          open={open}
          productStations={productStations}
          setOpen={setOpen}
          productStocksForSelection={productStocksForSelection}
          setSelectedProducts={setSelectedProducts}
          selectedProducts={selectedProducts}
          setProductStocksForSelection={setProductStocksForSelection}
        />
      )}
      {openOrderBoxModal.open && (
        <AddOrderBoxModal
          open={openOrderBoxModal.open}
          allClientOrderBox={allClientOrderBox}
          selectedOrderBox={selectedOrderBox}
          setSelectedOrderBox={setSelectedOrderBox}
          setAddedOrderBoxes={setAddedOrderBoxes}
          onClose={() => {
            setopenOrderBoxModal((prev) => ({ ...prev, open: false }));
          }}
        />
      )}
      <GoogleMapWithSearch
        open={openLocationModal}
        setOpen={setOpenLocationModal}
        setValue={setValue}
        setAutocomplete={setAutocomplete}
        splitLatAndLong={splitLatAndLong}
      />

      <FromLinkLocationModal
        open={openFromLinkLocationModal}
        setOpen={setOpenFromLinkLocationModal}
        setValue={setValue}
      />
    </Box>
  );
}
export default DraftFullfillableOrder;
