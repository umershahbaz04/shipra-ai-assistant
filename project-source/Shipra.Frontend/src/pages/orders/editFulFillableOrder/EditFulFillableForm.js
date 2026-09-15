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
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  DeleteOrderItemById,
  GetAddressFromLatAndLong,
  GetAllActiveCarrierForCreateOrderSelection,
  GetAllClientOrderBox,
  GetAllCountry,
  GetChannelListByStoreIdForSelection,
  GetProductStocksForSelection,
  GetStoresForSelection,
  GetValidateClientPPActivate,
  UpdateOrder,
  UpdateOrderWithInvoice,
} from "../../../api/AxiosInterceptors";
import { getAllStationLookupFunc } from "../../../apiCallingFunction";
import "../../../assets/styles/hideInputArrowsStyles.css";
import { styleSheet } from "../../../assets/styles/style";
import AddOrderBoxModal from "../../../components/modals/orderModals/AddOrderBoxModal";
import AddProductModal from "../../../components/modals/orderModals/AddProductModal";
import GoogleMapWithSearch from "../../../components/modals/resuseAbleModals/GoogleMapWithSearch";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  EnumNavigateState,
  EnumOptions,
  EnumOrderType,
  EnumPaymentMethod,
  EnumPaymentStatus,
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
  PaymentTaxBox,
  PaymentTotalBox,
  amountFormat,
  calculateSubtotalValue,
  decimalFormat,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  handleCopyToClipBoard,
  placeholders,
  truncate,
  useGetAllClientTax,
  useGetAllGenericSetting,
  useGetNavigateState,
  useMapAutocompleteSetter,
  useSetNumericInputEffect,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  checkRequiredUsingCarrierAdressSchema,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { calculateVatValue } from "../createRegularOrder";
import { getThisKeyCookie } from "../../../utilities/cookies";

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
function EditFulFillableForm(props) {
  const { preloadedValues, orderData } = props;
  const classes = useStyles();
  const location = useLocation();

  const { data } = location.state || {};
  const [open, setOpen] = useState(false);
  const [isProductFetching, setIsProductFetching] = useState(false);

  const [openFromLinkLocationModal, setOpenFromLinkLocationModal] =
    useState(false);
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [productStocksForSelection, setProductStocksForSelection] = useState(
    [],
  );
  const [selectedProducts, setSelectedProducts] = useState([]);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [selectedPMOption, setselectedPMOption] = useState(
    EnumPaymentMethod.Prepaid,
  );
  const [selectedPSOption, setSelectedPSOption] = useState(
    EnumPaymentStatus.Unpaid,
  );
  const [storesChannelForSelection, setStoresChannelForSelection] = useState(
    [],
  );
  const [load, setLoad] = useState(false);
  const [productStations, setProductStations] = useState([]);
  const [qtySelectedStation, setQtySelectedStation] = useState();
  const [allCountries, setAllCountries] = useState([]);
  const [allRegions, setAllRegions] = useState([]);
  const [allCities, setAllCities] = useState([]);
  const [totalItemPrice, setTotalItemPrice] = useState(0);
  const [vatValue, setVatValue] = useState([]);
  const [subtotal, setSubtotal] = useState(0);
  const [allCarriersForSelection, setAllCarriersForSelection] = useState([]);
  const [carrierId, setCarrierId] = useState();
  const [discount, setDiscount] = useState(0);
  const [shipping, setShipping] = useState(0);
  const [tax, setTax] = useState(5);
  const [note, setNote] = useState("");
  const [finalNote, setFinalNote] = useState("");
  const [remarks, setRemarks] = useState("");
  const [finalRemarks, setFinalRemarks] = useState("");
  const [isStripeSettingExist, setStripeSettingExist] = useState(false);
  const [isLoadingForConfirm, setIsLoadingForConfirm] = useState(false);
  const [metafields, setMetafields] = useState([]);
  const [isLoadingForConfirmAndNewOrder, setIsLoadingForConfirmAndNewOrder] =
    useState(false);
  const [isEditOrder, setIsEditOrder] = useState(false);
  const [
    isLoadingForConfirmAndSendInvoiceOrder,
    setIsLoadingForConfirmAndSendInvoiceOrder,
  ] = useState(false);
  const [selectedOrderPlaceButtom, setSelectedOrderPlaceButtom] =
    useState(false);
  const [paymentLinks, setPaymentLinks] = useState({
    show: false,
    paymentUrl: "",
    invoicePdf: "",
    showCrossIcon: true,
  });
  const [openUpdateOrderBoxModal, setopenUpdateOrderBoxModal] = useState({
    open: false,
    loading: {},
  });
  const [allClientOrderBox, setAllClientOrderBox] = useState([]);
  const [BoxLoading, setBoxLoading] = useState(false);
  const [selectedOrderBox, setSelectedOrderBox] = useState([]);
  const [addedOrderBoxes, setAddedOrderBoxes] = useState([]);
  const [weightCount, setweightCount] = useState(0);

  const navigate = useNavigate();
  const cookieData = getThisKeyCookie("genericSetting");
  const parsedData = cookieData ? JSON.parse(cookieData) : null;
  const { allGenericSetting } = useGetAllGenericSetting([], !parsedData);
  const finalGenericSetting = parsedData || allGenericSetting;
  const genericSettingFlag =
    finalGenericSetting?.[0]?.inputData?.[2]?.value === "true";

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    getValues,
    setValue,
    control,
    unregister,
  } = useForm({ defaultValues: preloadedValues });

  const {
    selectedAddressSchema,
    addressSchemaSelectData,
    addressSchemaInputData,
    showMoreInfoBtn,
    handleAddRemoveMoreInfoFields,
    handleSetSchema,
    handleChangeInputAddressSchema,
    handleChangeSelectAddressSchemaAndGetOptions,
    handleSetSchemaValueForUpdate,
  } = useGetAddressSchema(setValue, true, carrierId?.CarrierId);

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
  const { allClientTax, loading } = useGetAllClientTax([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  let getAllStationLookup = async () => {
    let data = await getAllStationLookupFunc();
    if (data.length > 0) {
      setProductStations(data);
    }
  };
  useEffect(() => {
    getAllStationLookup();
  }, []);
  let getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    //console.log("getStoresForSelection", res.data);
    setProductStocksForSelection([]);
    setValue("store", null);

    setStoresForSelection(res.data.result);
  };

  useEffect(() => {
    if (productStations.length > 0) {
      let selectedStation = productStations[0];
      if (orderData) {
        selectedStation = productStations?.find(
          (x) => x.productStationId == orderData?.order?.stationId,
        );
        selectedStation = selectedStation;
      }
      setQtySelectedStation(selectedStation);
    }
  }, [productStations]);
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

  const handleDeleteOrderBox = (data) => {
    const id = data?.clientOrderBoxId;
    if (id) {
      const updatedOrderBoxes = addedOrderBoxes.filter(
        (box) => box.clientOrderBoxId !== id,
      );
      const updatedSelectedOrderBoxes = selectedOrderBox.filter(
        (box) => box.clientOrderBoxId !== id,
      );
      setAddedOrderBoxes(updatedOrderBoxes);
      setSelectedOrderBox(updatedSelectedOrderBoxes);
    } else {
      const id = data?.orderBoxId;
      const updatedOrderBoxes = addedOrderBoxes.filter(
        (box) => box.orderBoxId !== id,
      );
      setAddedOrderBoxes(updatedOrderBoxes);
    }
  };

  useEffect(() => {
    getStoresForSelection();
    getValidateClientPPActivate();
  }, []);
  useEffect(() => {
    if (
      getValues("store") !== null &&
      getValues("store") !== "" &&
      getValues("store") !== undefined
    ) {
      getChannelListByStoreIdForSelection();
    }
  }, [getValues("store")]);

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    const _countries = res.data.result;
    setAllCountries(_countries);
  };
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

  useEffect(() => {
    getAllCountry();
  }, []);
  const [autocomplete, setAutocomplete] = useState(null);
  const getTotalAmount = () => {
    let total = 0;
    for (let index = 0; index < selectedProducts.length; index++) {
      total =
        total +
        ((selectedProducts[index].newPrice / 1 -
          selectedProducts[index].discount / 1) *
          selectedProducts[index].newQuantity) /
          1;
    }
    return total;
  };
  const prepareCreateOrderData = (data) => {
    let amount = 0;
    let weight = 0;
    let itemValue = 0;
    let description = [];
    let orderItems = [];
    for (let index = 0; index < selectedProducts.length; index++) {
      let ditem = `${selectedProducts[index].SKU}_${selectedProducts[index].newQuantity}`;
      weight = weight + selectedProducts[index].Weight;
      description.push(ditem);
      itemValue =
        itemValue +
        (selectedProducts[index].newPrice - selectedProducts[index].discount) *
          selectedProducts[index].newQuantity;
      orderItems[index] = {
        orderItemId: selectedProducts[index]?.orderItemId || "",
        productId: selectedProducts[index].ProductId,
        productStockId: selectedProducts[index].ProductStockId,
        productVariantId: selectedProducts[index].ProductVariantId,
        price: selectedProducts[index].newPrice,
        description: ditem,
        remarks: "",
        quantity: selectedProducts[index].newQuantity,
        discount: selectedProducts[index].discount,
        ...(genericSettingFlag && {
          HsCode: selectedProducts[index]?.hsCode,
          OriginCountryCode: selectedProducts[index]?.originCountryCode,
          UnitRate: Number(selectedProducts[index]?.unitRate),
          Weight: Number(selectedProducts[index]?.Weight),
        }),
      };
    }
    amount =
      Number(totalItemPrice) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);
    if (addedOrderBoxes.length == 0) {
      errorNotification("Please Choose Order Box");
      return;
    }
    if (orderItems.length == 0) {
      errorNotification("Please add product to proceed");
      return false;
    } else {
      const param1 = {
        orderId: orderData?.order?.orderId,
        storeId: data.store.storeId,
        orderTypeId: EnumOrderType.FullFillable,
        description: description.join(","),
        orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
        remarks: remarks,
        amount: amount,
        cShippingCharges: shipping,
        paymentStatusId: selectedPSOption,
        weight: data.weight,
        itemValue: itemValue,
        orderRequestVia: 1,
        paymentMethodId: selectedPMOption,
        stationId: orderData?.order?.stationId,
        discount: discount,
        vat: 0,
        refNo: data?.refNo,
        SaleChannelConfigId: data.storeChannel?.id || 0,
        orderNote: {
          note: note,
        },
        orderAddress: {
          SelectedCarrierId: carrierId?.CarrierId || null,
          countryId: selectedAddressSchema.country,
          cityId: selectedAddressSchema.city,
          areaId: selectedAddressSchema.area,
          streetAddress: selectedAddressSchema.streetAddress,
          streetAddress2: selectedAddressSchema.streetAddress2,
          houseNo: selectedAddressSchema.houseNo,
          buildingName: selectedAddressSchema.buildingName,
          landmark: selectedAddressSchema.landmark,
          provinceId: selectedAddressSchema.province,
          pinCodeId: selectedAddressSchema.pinCode,
          stateId: selectedAddressSchema.state,
          zip: selectedAddressSchema.zip,
          latitude: getValues("lat&long").split(",")[0] || null,
          longitude: getValues("lat&long").split(",")[1] || null,
          orderAddressId: orderData?.orderAddress?.orderAddressId,
          customerName: data.customerName,
          email: data.email,
          mobile1: UtilityClass.getFormatedNumber(data.mobile1),
          mobile2: UtilityClass.getFormatedNumber(data.mobile2),
        },
        settingConfig: metafields,
        orderTaxes: orderData?.orderTax
          ?.map((tax) => {
            return {
              orderTaxId: tax.orderTaxId,
              ClientTaxId: tax.clientTaxId,
              taxValue: Number(vatValue[tax.clientTaxId]),
            };
          })
          .filter((dt) => dt !== undefined),
        orderBoxs: addedOrderBoxes.map((box) => ({
          orderBoxId: box?.orderBoxId ?? null,
          clientOrderBoxId: box?.clientOrderBoxId,
          length: Number(box.length),
          width: Number(box.width),
          height: Number(box.height),
        })),
        orderItems: orderItems,
      };
      return param1;
    }
  };
  const handleConfirmOrder = async (data) => {
    const param1 = prepareCreateOrderData(data);
    if (param1) {
      setIsLoadingForConfirm(true);
      setSelectedOrderPlaceButtom(EnumOrderPlaceButton.Confirm);

      UpdateOrder(param1)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
          } else {
            successNotification("Order updated successfully");
            navigate("/orders-dashboard");
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Unable updated order");
        })
        .finally(() => {
          setIsLoadingForConfirm(false);
          setSelectedOrderPlaceButtom(null);
        });
    }
  };
  const handleSendInvoice = async (data) => {
    const param1 = prepareCreateOrderData(data);
    if (param1) {
      setIsLoadingForConfirmAndSendInvoiceOrder(true);
      setSelectedOrderPlaceButtom(EnumOrderPlaceButton.ConfirmAndHandleInvoice);

      UpdateOrderWithInvoice(param1)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            errorNotification("Unable to update order");
            errorNotification(res?.data?.customErrorMessage);
          } else {
            successNotification("Order updated successfully");
            const data = res.data.result;
            setPaymentLinks((prev) => ({
              ...prev,
              show: true,
              paymentUrl: data?.hostedInvoiceUrl,
              invoicePdf: data?.invoicePdf,
            }));
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Unable to update order");
        })
        .finally(() => {
          setSelectedOrderPlaceButtom(null);
          setIsLoadingForConfirmAndSendInvoiceOrder(false);
        });
    }
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

  const paymentStatusOptions = [
    { id: EnumPaymentStatus.Unpaid, label: "Unpaid" },
    { id: EnumPaymentStatus.Paid, label: "Paid" },
  ];

  const handleCheckboxChange = (optionId) => {
    setselectedPMOption(optionId);
  };

  const handlePaymentStatusCheckboxChange = (optionId) => {
    setSelectedPSOption(optionId);
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
  const [isFirstLoad, setIsFirstLoad] = useState(true);
  useEffect(() => {
    let _totalItemPrice = 0;
    let _totaldiscount = 0;
    selectedProducts.forEach((data) => {
      _totalItemPrice += Number(data.newPrice);
      _totaldiscount += Number(data.discount);
    });
    if (!isFirstLoad) {
      setTotalItemPrice(_totalItemPrice);
    } else {
      setTotalItemPrice(
        calculateSubtotalValue(
          orderData?.order?.discount,
          orderData?.order?.cShippingCharges,
          orderData?.orderTax,
          orderData?.order?.amount,
        ),
      );
    }
  }, [selectedProducts]);
  useEffect(() => {
    if (storesForSelection?.length > 0) {
      let defData = storesForSelection.find(
        (x) => x.storeId == preloadedValues?.storeId,
      );
      setValue("store", defData);
    }
  }, [storesForSelection]);
  useEffect(() => {
    if (allCarriersForSelection?.length > 0) {
      let defData = allCarriersForSelection.find(
        (x) => x.CarrierId == preloadedValues?.selectedCarrierId,
      );
      setCarrierId(defData);
    }
  }, [allCarriersForSelection]);
  useEffect(() => {
    if (storesChannelForSelection?.length > 0) {
      let defData = storesChannelForSelection.find(
        (x) => x.id == orderData?.order?.saleChannelConfigId,
      );
      setValue("storeChannel", defData);
    }
  }, [storesChannelForSelection]);

  useEffect(() => {
    if (orderData) {
      let order = orderData?.order;
      setDiscount(order?.discount);
      setShipping(order?.cShippingCharges);
      setFinalRemarks(order?.remarks);
      setRemarks(order?.remarks);
      if (preloadedValues) {
        if (order?.paymentMethodId) {
          handleCheckboxChange(order?.paymentMethodId);
        }
        if (order?.paymentStatusId) {
          handlePaymentStatusCheckboxChange(order?.paymentStatusId);
        }
      }
      setNote(orderData?.orderNote?.note);
      setFinalNote(orderData?.orderNote?.note);
      setSubtotal(
        calculateSubtotalValue(
          orderData?.order?.discount,
          orderData?.order?.cShippingCharges,
          orderData?.orderTax,
          orderData?.order?.amount,
        ),
      );

      if (orderData) {
        if (orderData?.orderItems) {
          getProductStocksForSelection();
        }
      }

      setIsEditOrder(true);
    }
  }, [orderData]);
  let getProductStocksForSelection = async () => {
    if (isEditOrder) {
      //let do somework regarding edit order
    } else {
      setSelectedProducts([]);
    }
    let stationId =
      qtySelectedStation?.productStationId || orderData?.order.stationId;
    setIsProductFetching(true);

    let res = await GetProductStocksForSelection(
      stationId,
      getValues("store")?.storeId || orderData?.order?.storeId,
    );

    if (res.data.result != null && res.data.result?.length > 0) {
      for (let index = 0; index < res.data?.result.length; index++) {
        //get selected object against station

        let obj = orderData?.orderItems?.find(
          (x) => x.productStockId == res.data.result[index].ProductStockId,
        );
        //orderItemId

        res.data.result[index].checked = obj ? true : false;
        res.data.result[index].isRemove =
          obj && obj?.orderItemId ? true : false; //if edit case then remove row
        res.data.result[index].discount = 0;
        res.data.result[index].newPrice = res.data.result[index].Price;
        res.data.result[index].newQuantity = 1;
        res.data.result[index].stationId = qtySelectedStation?.productStationId;
      }

      let preloadedProducts = [];
      orderData?.orderItems.forEach((item, i) => {
        let value = res.data.result?.find(
          (x) => x.ProductStockId == item.productStockId,
        );
        if (value) {
          value["orderItemId"] = item.orderItemId;
          value["checked"] = true;
          value["Price"] = item.price;
          value["newPrice"] = item.price;
          value["newQuantity"] = item.quantity;
          value["discount"] = item.discount;
          value["hsCode"] = item.hsCode;
          value["originCountryCode"] = item.originCountryCode;
          value["unitRate"] = item.unitRate;
          value["weight"] = item.weight;
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
  };
  const handleOnClickProduct = async () => {
    await getProductStocksForSelection();
    //open modal
    setOpen(true);
  };
  const handleDelete = async (value) => {
    value.checked = false;
    if (value?.orderItemId) {
      await handleDeleteItemFromDb(value);
    }
    let product = selectedProducts.filter(
      (item) => item.ProductStockId !== value.ProductStockId,
    );
    setSelectedProducts(product);
  };
  const [isDeleting, setIsDeleting] = useState(false);
  const handleDeleteItemFromDb = async (item) => {
    let param = {
      orderId: orderData?.order?.orderId,
      orderItemId: item?.orderItemId,
    };
    setIsDeleting(true);
    DeleteOrderItemById(param)
      .then((res) => {
        if (!res.data.isSuccess) {
          let jsonData = res.data.errors;

          UtilityClass.showErrorNotificationWithDictionary(jsonData);
        } else {
          successNotification("Item delete successfully");
        }
      })
      .catch((e) => {
        setIsDeleting(false);
        let res = e.response;
        if (!res.data.isSuccess) {
          let jsonData = res.data.errors;

          UtilityClass.showErrorNotificationWithDictionary(jsonData);
        } else {
          errorNotification(
            LanguageReducer?.languageType
              ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST,
          );
        }

        console.log("e", e);
      });
  };

  const handleBoxDimensionChange = (data, field, value) => {
    const clientOrderBoxId = data?.clientOrderBoxId;
    if (clientOrderBoxId) {
      setAddedOrderBoxes((prev) =>
        prev.map((box) => {
          if (box.clientOrderBoxId !== clientOrderBoxId) return box;

          const updatedBox = {
            ...box,
            [field]: Number(value),
          };

          const { length, width, height } = updatedBox;
          updatedBox.volume =
            length && width && height ? (length * width * height) / 5000 : 0;

          return updatedBox;
        }),
      );
    } else {
      setAddedOrderBoxes((prev) =>
        prev.map((box) => {
          if (box.orderBoxId !== data.orderBoxId) return box;

          const updatedBox = {
            ...box,
            [field]: Number(value),
          };

          const { length, width, height } = updatedBox;
          updatedBox.volume =
            length && width && height ? (length * width * height) / 5000 : 0;

          return updatedBox;
        }),
      );
    }
  };

  const { navigateStateData } = useGetNavigateState(
    EnumNavigateState.EDIT_ORDER.pageName,
    false,
  );
  const disabledInput = navigateStateData
    ? navigateStateData.disabledInput
    : false;

  useSetNumericInputEffect([selectedProducts]);
  useMapAutocompleteSetter(
    autocomplete,
    allCountries,
    allRegions,
    allCities,
    setValue,
  );

  useEffect(() => {
    (async () => {
      await handleSetSchemaValueForUpdate(
        orderData.orderAddress,
        setValue,
        preloadedValues?.selectedCarrierId,
      );
      setValue("mobile1", preloadedValues.mobile1);
      setValue("mobile2", preloadedValues.mobile2);
      let latitude = preloadedValues?.latitude;
      let longitude = preloadedValues?.longitude;
      if (latitude !== null && longitude !== null) {
        setValue("lat&long", `${latitude},${longitude}`);
      }
      if (weightCount === 0) {
        setValue("weight", preloadedValues?.weight);
        setweightCount(weightCount + 1);
      }
    })();
  }, []);

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

  const handleChange = (value, inputIndex) => {
    const updatedMetafields = [...metafields];
    updatedMetafields[inputIndex].value = value;
    setMetafields(updatedMetafields);
  };

  useEffect(() => {
    if (!orderData?.orderBoxes?.length) return;

    const updatedBoxes = orderData.orderBoxes.map((orderBox) => {
      const clientBox = orderBox.clientOrderBoxId
        ? allClientOrderBox.find(
            (x) => x.clientOrderBoxId === orderBox.clientOrderBoxId,
          )
        : null;

      if (clientBox) {
        return {
          clientOrderBoxId: clientBox.clientOrderBoxId,
          orderBoxId: orderBox.orderBoxId,
          length: clientBox.length,
          width: clientBox.width,
          height: clientBox.height,
          volume: clientBox.volume,
        };
      }

      return {
        clientOrderBoxId: null,
        orderBoxId: orderBox.orderBoxId,
        length: orderBox.length,
        width: orderBox.width,
        height: orderBox.height,
        volume: orderBox.volume,
      };
    });

    setAddedOrderBoxes(updatedBoxes);
  }, [orderData, allClientOrderBox]);

  useEffect(() => {
    if (preloadedValues?.metafields) {
      setMetafields(preloadedValues.metafields);
    }
  }, [preloadedValues]);

  useEffect(() => {
    getAllClientOrderBox();
  }, []);

  useEffect(() => {
    if (getValues("country") !== null && getValues("country") !== undefined) {
      getAllActiveCarrierForCreateOrderSelection();
    }
  }, [getValues("country")]);

  const totalVolume = addedOrderBoxes.reduce(
    (sum, box) => sum + (box.volume || 0),
    0,
  );

  useEffect(() => {
    if (weightCount > 0) {
      setValue("weight", parseFloat(totalVolume.toFixed(2)));
    }
  }, [totalVolume]);

  // useEffect(() => {
  //   if (preloadedValues?.OrderDate) {
  //     const parsedDate = new Date(preloadedValues.OrderDate);
  //     if (!isNaN(parsedDate)) {
  //       setValue("orderDate", parsedDate);
  //     }
  //   }
  // }, [preloadedValues]);

  return (
    <Box
      sx={{
        ...styleSheet.pageRoot,
        "& .MuiFormLabel-asterisk": {
          color: "red !important",
        },
      }}
    >
      <form>
        {/* <Container maxWidth="lg" fixed sx={{ paddingLeft: "0px" }}> */}
        <div style={{ padding: "10px" }}>
          <GridItem textAlign={"right"}>
            <ActionButtonCustom
              onClick={() => {
                navigate("/orders-dashboard");
              }}
              variant="contained"
              label={LanguageReducer?.languageType?.ORDERS_ORDER_DASHBOARD}
            />
          </GridItem>
          <GridContainer>
            <GridItem md={7} sm={12} xs={12}>
              <GridContainer>
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_STORE}
                  >
                    <Grid container spacing={1}>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_STORE}
                        </InputLabel>
                        <SelectComponent
                          disabled
                          name="store"
                          control={control}
                          options={storesForSelection}
                          isRHF={true}
                          required={true}
                          getOptionLabel={(option) => option?.storeName}
                          optionLabel={EnumOptions.STORE.LABEL}
                          optionValue={EnumOptions.STORE.VALUE}
                          isRefesh={true}
                          handleRefreshClick={getStoresForSelection}
                          {...register("store", {
                            required: {
                              value: true,
                            },
                          })}
                          value={getValues("store")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("store", resolvedId);
                          }}
                          errors={errors}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDER_SALE_CHANNEL}
                        </InputLabel>
                        <SelectComponent
                          disabled
                          name="storeChannel"
                          control={control}
                          options={storesChannelForSelection}
                          isRHF={false}
                          required={false}
                          optionLabel={EnumOptions.STORE_CHANNEL.LABEL}
                          optionValue={EnumOptions.STORE_CHANNEL.VALUE}
                          {...register("storeChannel")}
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
                          <InputLabel required sx={styleSheet.inputLabel}>
                            {LanguageReducer?.languageType?.ORDERS_STATIONS}
                          </InputLabel>
                          <SelectComponent
                            disabled
                            name="qtySelectedStation"
                            options={productStations}
                            defaulValue={qtySelectedStation}
                            value={qtySelectedStation}
                            optionLabel={EnumOptions.SELECT_STATION.LABEL}
                            optionValue={EnumOptions.SELECT_STATION.VALUE}
                            isRefesh={true}
                            handleRefreshClick={getAllStationLookup}
                            onChange={(e, newValue) => {
                              const resolvedId = newValue ? newValue : null;
                              setQtySelectedStation(resolvedId);
                            }}
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
                          isRHF={true}
                          required={true}
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
                          disabled={disabledInput}
                          control={control}
                          required
                          error={
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT
                          }
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
                {!genericSettingFlag ? (
                  <GridItem xs={12}>
                    <CustomColorLabelledOutline
                      isCollapse={true}
                      label={LanguageReducer?.languageType?.PRODUCTS_TEXT}
                    >
                      {!disabledInput && (
                        <Box textAlign={"right"}>
                          <ActionButtonCustom
                            loading={isProductFetching}
                            startIcon={<AddBoxIcon fontSize="small" />}
                            label={"Update Products"}
                            onClick={() => handleOnClickProduct()}
                            // sx={styleSheet.placeOrderButton}
                          />
                        </Box>
                      )}
                      <Box sx={{ overflow: "auto" }}>
                        <Box
                          sx={{
                            width: "100%",
                            display: "table",
                            tableLayout: "fixed",
                          }}
                        >
                          <Table size="small">
                            <TableHead
                              sx={{ ...styleSheet.orderProductHeading }}
                            >
                              {" "}
                              <TableRow>
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
                              {selectedProducts &&
                                selectedProducts.length > 0 &&
                                selectedProducts?.map((value, index) => {
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
                                              sx={
                                                styleSheet.productItemBoxPrice
                                              }
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
                                      <TableCell align="center" sx={{ p: 0.5 }}>
                                        <b>{value.QuantityAvailable}</b>
                                      </TableCell>

                                      <TableCell sx={{ p: 0.5 }} align="center">
                                        {" "}
                                        {disabledInput ? (
                                          <TextField
                                            disabled={disabledInput}
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
                                            value={
                                              value.newQuantity
                                                ? value.newQuantity
                                                : value.QuantityAvailable
                                            }
                                          />
                                        ) : (
                                          <Box className={"flex_center"}>
                                            <Box
                                              sx={
                                                styleSheet.qualityIncrementBox
                                              }
                                            >
                                              <IndeterminateCheckBoxOutlinedIcon
                                                onClick={(e) =>
                                                  handleOnClickDecrement(
                                                    e,
                                                    index,
                                                  )
                                                }
                                              />
                                              <Box sx={{ fontSize: "17px" }}>
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
                                                    handleOnchangeProduct(
                                                      e,
                                                      index,
                                                    )
                                                  }
                                                />
                                              </Box>
                                              <AddBoxOutlinedIcon
                                                onClick={(e) =>
                                                  handleOnClickIncrement(
                                                    e,
                                                    index,
                                                  )
                                                }
                                              />
                                            </Box>
                                          </Box>
                                        )}
                                      </TableCell>

                                      <TableCell sx={{ p: 0.5 }}>
                                        <Box sx={styleSheet.productItemBox}>
                                          <TextField
                                            disabled={disabledInput}
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
                                                  eval(e.target.value) || 0,
                                                newPrice:
                                                  newSelectedProducts[index]
                                                    .Price *
                                                    newSelectedProducts[index]
                                                      .newQuantity -
                                                  (eval(e.target.value) || 0),
                                              };
                                              setSelectedProducts(
                                                newSelectedProducts,
                                              );
                                              setIsFirstLoad(false);
                                            }}
                                          />
                                        </Box>
                                      </TableCell>
                                      <TableCell align="right" sx={{ p: 0.5 }}>
                                        <PaymentAmountTextField
                                          disabled={disabledInput}
                                          value={
                                            selectedProducts[index].newPrice
                                          }
                                          onChange={(e) => {
                                            let newSelectedProducts = [
                                              ...selectedProducts,
                                            ];
                                            newSelectedProducts[index] = {
                                              ...newSelectedProducts[index],
                                              newPrice: e.target.value,
                                            };
                                            setSelectedProducts(
                                              newSelectedProducts,
                                            );
                                          }}
                                          width={118}
                                        />
                                      </TableCell>
                                      {!disabledInput && (
                                        <TableCell
                                          align="right"
                                          sx={{ p: 0.5 }}
                                        >
                                          <Button
                                            onClick={() => {
                                              handleDelete(value);
                                            }}
                                            sx={styleSheet.deleteProductButton}
                                            variant="outlined"
                                          >
                                            <DeleteIcon />
                                          </Button>
                                        </TableCell>
                                      )}
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
                        {...register("weight", {
                          // required: {
                          //   value: true,
                          //   message:
                          //     LanguageReducer?.languageType
                          //       ?.FIELD_REQUIRED_TEXT,
                          // },
                        })}
                        error={Boolean(errors.weight)} // set error prop
                        helperText={errors.weight?.message}
                      />
                    </Grid>
                    <Box textAlign={"right"}>
                      <ActionButtonCustom
                        label={"Update Order Box"}
                        onClick={() =>
                          setopenUpdateOrderBoxModal((prev) => ({
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
                                    minWidth: 25,
                                  }}
                                >
                                  {"Length"}
                                </TableCell>
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 25,
                                  }}
                                >
                                  {"Width"}
                                </TableCell>

                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 25,
                                  }}
                                >
                                  {"Height"}
                                </TableCell>
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    p: 1,
                                    minWidth: 20,
                                  }}
                                >
                                  {"Volume"}
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
                                    <TextField
                                      size="small"
                                      type="number"
                                      value={box.length}
                                      onChange={(e) =>
                                        handleBoxDimensionChange(
                                          box,
                                          "length",
                                          e.target.value,
                                        )
                                      }
                                      sx={{
                                        "& .MuiInputBase-root": {
                                          height: "30px",
                                        },
                                        width: 110,
                                      }}
                                      inputProps={{ min: 0 }}
                                    />
                                  </TableCell>
                                  <TableCell sx={{ p: 1 }}>
                                    <TextField
                                      size="small"
                                      type="number"
                                      value={box.width}
                                      onChange={(e) =>
                                        handleBoxDimensionChange(
                                          box,
                                          "width",
                                          e.target.value,
                                        )
                                      }
                                      sx={{
                                        "& .MuiInputBase-root": {
                                          height: "30px",
                                        },
                                        width: 110,
                                      }}
                                      inputProps={{ min: 0 }}
                                    />
                                  </TableCell>
                                  <TableCell sx={{ p: 1 }}>
                                    <TextField
                                      size="small"
                                      type="number"
                                      value={box.height}
                                      onChange={(e) =>
                                        handleBoxDimensionChange(
                                          box,
                                          "height",
                                          e.target.value,
                                        )
                                      }
                                      sx={{
                                        "& .MuiInputBase-root": {
                                          height: "30px",
                                        },
                                        width: 110,
                                      }}
                                      inputProps={{ min: 0 }}
                                    />
                                  </TableCell>
                                  <TableCell sx={{ p: 1 }}>
                                    {box.volume}
                                  </TableCell>
                                  <TableCell sx={{ p: 1 }}>
                                    <Button
                                      sx={styleSheet.deleteProductButton}
                                      variant="outlined"
                                      onClick={() => handleDeleteOrderBox(box)}
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
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_CUSTOMER_NAME}
                        </InputLabel>
                        <TextField
                          type="text"
                          placeholder={placeholders.name}
                          onFocus={handleFocus}
                          size="small"
                          fullWidth
                          disabled={disabledInput}
                          // onChange={(e) => { setOrderAddress({ ...orderAddress, customerName: e.target.value }) }}
                          variant="outlined"
                          id="customerName"
                          name="customerName"
                          {...register("customerName", {
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
                          })}
                          error={Boolean(errors.customerName)} // set error prop
                          helperText={errors.customerName?.message}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_EMAIL}{" "}
                        </InputLabel>
                        <TextField
                          disabled={disabledInput}
                          placeholder={placeholders.email}
                          onFocus={handleFocus}
                          type="email"
                          size="small"
                          id="email"
                          name="email"
                          fullWidth
                          variant="outlined"
                          {...register("email", {
                            // required: {
                            //   value: true,
                            //   message:
                            //     LanguageReducer?.languageType
                            //       ?.FIELD_REQUIRED_TEXT,
                            // },
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
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_PHONE_NO}
                        </InputLabel>
                        <CustomRHFPhoneInput
                          error={
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT
                          }
                          name="mobile1"
                          control={control}
                          required
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
                      <Grid item md={4} sm={4} xs={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_COUNTRY}
                        </InputLabel>
                        <SelectComponent
                          disabled={disabledInput}
                          name="country"
                          control={control}
                          options={allCountries}
                          isRHF={true}
                          required={true}
                          optionLabel={EnumOptions.COUNTRY.LABEL}
                          optionValue={EnumOptions.COUNTRY.VALUE}
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
                            setCarrierId(null);
                            handleSetSchema(
                              "country",
                              resolvedId,
                              setValue,
                              unregister,
                              carrierId?.CarrierId ||
                                preloadedValues?.selectedCarrierId,
                            );
                          }}
                          errors={errors}
                        />
                      </Grid>
                      {[
                        ...addressSchemaSelectData,
                        ...addressSchemaInputData,
                      ].map((input, index, arr) => (
                        <Grid item md={4} sm={4} xs={12}>
                          <SchemaTextField
                            loading={input.loading}
                            disabled={disabledInput || input.disabled}
                            isRHF={true}
                            type={input.type}
                            name={input.key}
                            required={checkRequiredUsingCarrierAdressSchema(
                              carrierId,
                              input.key,
                              input.required,
                            )}
                            optionLabel={addressSchemaEnum[input.key]?.LABEL}
                            optionValue={addressSchemaEnum[input.key]?.VALUE}
                            register={register}
                            options={input.options}
                            errors={errors}
                            label={input.label}
                            viewMode={disabledInput}
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
                        {!disabledInput && (
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
                                LanguageReducer?.languageType
                                  ?.ORDERS_GET_ADDRESS
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
                        )}
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
              <Grid item xs={12}>
                {metafields.length > 0 && (
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={"MetaField"}
                  >
                    <Grid container spacing={2} sx={{ paddingTop: 2 }}>
                      {metafields.map((input, input_index) => (
                        <Grid
                          item
                          xs={12}
                          sm={6}
                          key={input.name}
                          sx={{ paddingTop: "0px!important" }}
                        >
                          <Box marginBottom={1}>
                            <InputLabel
                              sx={styleSheet.inputLabel}
                              required={input.required}
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
                                  handleChange(newValue, input_index)
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
                                  handleChange(e.target.value, input_index)
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
                                      input.value ?? input.defaultValue ?? false
                                    }
                                    onChange={(e) =>
                                      handleChange(
                                        e.target.checked,
                                        input_index,
                                      )
                                    }
                                  />
                                }
                                label={input.description || ""}
                              />
                            )}
                          </Box>
                        </Grid>
                      ))}
                    </Grid>
                  </CustomColorLabelledOutline>
                )}
              </Grid>
            </GridItem>
            <GridItem md={5} sm={12} xs={12}>
              <GridContainer>
                <GridItem md={12} sm={12} xs={12}>
                  <CustomColorLabelledOutline
                    label={"Payment"}
                    isCollapse={true}
                  >
                    <Box display={"flex"} flexDirection={"column"} gap={1}>
                      <PaymentAmountBox
                        disabled={disabledInput}
                        title={LanguageReducer?.languageType?.ORDERS_SUBTOTAL}
                        value={decimalFormat(totalItemPrice)}
                        onChange={(e) => {
                          const _totalItemPrice = Number(e.target.value);
                          setTotalItemPrice(_totalItemPrice);
                        }}
                      />
                      <PaymentAmountBox
                        disabled={disabledInput}
                        title={
                          LanguageReducer?.languageType?.ORDERS_ADD_DISCOUNT
                        }
                        value={discount}
                        onChange={(e) => {
                          setDiscount(Number(e.target.value));
                        }}
                      />
                      <PaymentAmountBox
                        disabled={disabledInput}
                        title={
                          LanguageReducer?.languageType?.ORDERS_ADD_SHIPPING
                        }
                        value={shipping}
                        onChange={(e) => {
                          setShipping(Number(e.target.value));
                        }}
                      />
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
                                disabled={disabledInput}
                                key={option.id}
                                checked={selectedPMOption === option.id}
                                onChange={() => handleCheckboxChange(option.id)}
                                label={option.label}
                              />
                            ))}
                          </Box>
                        }
                      />
                      <Box textAlign={"center"} mt={2}>
                        <Typography variant="h5" fontWeight={700}>
                          Payment Status
                        </Typography>
                      </Box>
                      <Box className={"flex_between"} mb={2}>
                        {paymentStatusOptions.map((option) => (
                          <PaymentMethodCheckbox
                            disabled={disabledInput}
                            key={option.id}
                            checked={selectedPSOption === option.id}
                            onChange={() => handlePaymentStatusCheckboxChange(option.id)}
                            label={option.label}
                          />
                        ))}
                      </Box>
                    </Box>
                    <Grid
                      justifyContent={"flex-end"}
                      alignItems="center"
                      sx={{ mt: 1 }}
                      container
                      spacing={1}
                    >
                      {!disabledInput && (
                        <Grid item>
                          <ActionButtonCustom
                            onClick={handleSubmit(handleConfirmOrder)}
                            variant="contained"
                            loading={isLoadingForConfirm}
                            loadingPosition="start"
                            disabled={
                              selectedOrderPlaceButtom &&
                              selectedOrderPlaceButtom !==
                                EnumOrderPlaceButton.Confirm
                            }
                            label={"Update"}
                          />
                        </Grid>
                      )}
                      {!disabledInput &&
                        isStripeSettingExist &&
                        (!orderData.order.stripeInvoiceHostURL ||
                          !orderData.order.stripeInvoicePDFURL) &&
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
                                {paymentLinks.showCrossIcon && (
                                  <CrossIconButton
                                    onClick={() =>
                                      setPaymentLinks((prev) => ({
                                        ...prev,
                                        show: false,
                                      }))
                                    }
                                  />
                                )}
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
                                Update And Get Payment Link
                              </LoadingButton>
                            )}
                          </Grid>
                        )}
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
                <GridItem xs={12}>
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
                        // endAdornment={
                        //   <InputAdornment position="end">
                        //     <Button
                        //       onClick={(e) => {
                        //         setFinalNote(note);
                        //       }}
                        //       sx={styleSheet.sendNotButton}
                        //       variant="contained"
                        //     >
                        //       <SendIcon />
                        //     </Button>
                        //   </InputAdornment>
                        // }
                        aria-describedby="outlined-weight-helper-text"
                        inputProps={{
                          "aria-label": "weight",
                        }}
                        disabled={disabledInput}
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
                        // endAdornment={
                        //   <InputAdornment position="end">
                        //     <Button
                        //       onClick={(e) => {
                        //         setFinalRemarks(remarks);
                        //       }}
                        //       style={{ color: "white" }}
                        //       sx={styleSheet.sendNotButton}
                        //     >
                        //       <SendIcon />
                        //     </Button>
                        //   </InputAdornment>
                        // }
                        aria-describedby="outlined-weight-helper-text"
                        inputProps={{
                          "aria-label": "weight",
                        }}
                        disabled={disabledInput}
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
            {genericSettingFlag ? (
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
                    />
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
                          <TableRow sx={{ ...styleSheet.orderProductHeading }}>
                            <TableCell sx={{ fontWeight: "bold", p: 1 }}>
                              {"Product"}
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
                            <TableCell sx={{ fontWeight: "bold", p: 1 }}>
                              {"HsCode"}
                            </TableCell>
                            <TableCell sx={{ fontWeight: "bold", p: 1 }}>
                              {"Origin Country Code"}
                            </TableCell>
                            <TableCell sx={{ fontWeight: "bold", p: 1 }}>
                              {"Unit Rate"}
                            </TableCell>
                            <TableCell sx={{ fontWeight: "bold", p: 1 }}>
                              {"Weight"}
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
                                    gap={0.5}
                                    alignItems={"center"}
                                    width={"90px !important"}
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
                                      <Typography variant="h5" fontWeight={300}>
                                        {truncate(value.ProductName)}
                                      </Typography>
                                      <Typography variant="h6" fontWeight={300}>
                                        <DataGridRenderGreyBox
                                          title={truncate(
                                            value.VarientOption,
                                            25,
                                          )}
                                        />
                                      </Typography>

                                      <Box sx={styleSheet.productItemBoxPrice}>
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
                                    <Box sx={styleSheet.qualityIncrementBox}>
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
                                          placeholder={placeholders.quantity}
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
                                          max: selectedProducts[index].newPrice,
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
                                            newSelectedProducts[index].Price *
                                              newSelectedProducts[index]
                                                .newQuantity -
                                            (parseInt(e.target.value) || 0),
                                        };
                                        setSelectedProducts(
                                          newSelectedProducts,
                                        );
                                      }}
                                    />
                                  </Box>
                                </TableCell>
                                <TableCell sx={{ p: 0.5 }}>
                                  <TextField
                                    size="small"
                                    type="text"
                                    onFocus={handleFocus}
                                    fullWidth
                                    sx={{
                                      color: "white",
                                      width: "75px",
                                      border: "none",
                                      background: "transparent",
                                      "& .MuiInputBase-root": {
                                        height: 30,
                                      },
                                    }}
                                    value={value.hsCode || ""}
                                    onChange={(e) => {
                                      let newSelectedProducts = [
                                        ...selectedProducts,
                                      ];
                                      newSelectedProducts[index] = {
                                        ...newSelectedProducts[index],
                                        hsCode: e.target.value,
                                      };
                                      setSelectedProducts(newSelectedProducts);
                                    }}
                                  />
                                </TableCell>
                                <TableCell sx={{ p: 0.5 }}>
                                  <TextField
                                    size="small"
                                    type="text"
                                    onFocus={handleFocus}
                                    fullWidth
                                    sx={{
                                      color: "white",
                                      width: "100px",
                                      border: "none",
                                      background: "transparent",
                                      "& .MuiInputBase-root": {
                                        height: 30,
                                      },
                                    }}
                                    value={value.originCountryCode || ""}
                                    onChange={(e) => {
                                      let newSelectedProducts = [
                                        ...selectedProducts,
                                      ];
                                      newSelectedProducts[index] = {
                                        ...newSelectedProducts[index],
                                        originCountryCode: e.target.value,
                                      };
                                      setSelectedProducts(newSelectedProducts);
                                    }}
                                  />
                                </TableCell>
                                <TableCell sx={{ p: 0.5 }}>
                                  <TextField
                                    size="small"
                                    type="number"
                                    onFocus={handleFocus}
                                    fullWidth
                                    sx={{
                                      color: "white",
                                      width: "75px",
                                      border: "none",
                                      background: "transparent",
                                      "& .MuiInputBase-root": {
                                        height: 30,
                                      },
                                    }}
                                    value={value.unitRate || 0}
                                    onChange={(e) => {
                                      let newSelectedProducts = [
                                        ...selectedProducts,
                                      ];
                                      newSelectedProducts[index] = {
                                        ...newSelectedProducts[index],
                                        unitRate: e.target.value,
                                      };
                                      setSelectedProducts(newSelectedProducts);
                                    }}
                                  />
                                </TableCell>
                                <TableCell sx={{ p: 0.5 }}>
                                  <TextField
                                    size="small"
                                    type="number"
                                    onFocus={handleFocus}
                                    fullWidth
                                    sx={{
                                      color: "white",
                                      width: "75px",
                                      border: "none",
                                      background: "transparent",
                                      "& .MuiInputBase-root": {
                                        height: 30,
                                      },
                                    }}
                                    defaultValue={value.Weight}
                                    value={value.Weight || 0}
                                    onChange={(e) => {
                                      let newSelectedProducts = [
                                        ...selectedProducts,
                                      ];
                                      newSelectedProducts[index] = {
                                        ...newSelectedProducts[index],
                                        Weight: e.target.value,
                                      };
                                      setSelectedProducts(newSelectedProducts);
                                    }}
                                  />
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
                                      setSelectedProducts(newSelectedProducts);
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
          </GridContainer>
        </div>
      </form>
      {open && (
        <AddProductModal
          open={open}
          setOpen={setOpen}
          productStocksForSelection={productStocksForSelection}
          setSelectedProducts={setSelectedProducts}
          selectedProducts={selectedProducts}
          productStations={productStations}
          setIsEditOrder={setIsEditOrder}
          isEditOrder={isEditOrder}
          orderData={orderData}
          setProductStocksForSelection={setProductStocksForSelection}
        />
      )}
      {openUpdateOrderBoxModal.open && (
        <AddOrderBoxModal
          open={openUpdateOrderBoxModal.open}
          allClientOrderBox={allClientOrderBox}
          selectedOrderBox={selectedOrderBox}
          setSelectedOrderBox={setSelectedOrderBox}
          setAddedOrderBoxes={setAddedOrderBoxes}
          addedOrderBoxes={addedOrderBoxes}
          onClose={() =>
            setopenUpdateOrderBoxModal((prev) => ({ ...prev, open: false }))
          }
        />
      )}
      {/* {openLocationModal && ( */}
      <GoogleMapWithSearch
        open={openLocationModal}
        setOpen={setOpenLocationModal}
        setValue={setValue}
        setAutocomplete={setAutocomplete}
        splitLatAndLong={splitLatAndLong}
      />
      {/* )} */}
      {/* <FromLinkLocationModal
        open={openFromLinkLocationModal}
        setOpen={setOpenFromLinkLocationModal}
        setValue={setValue}
      /> */}
    </Box>
  );
}
export default EditFulFillableForm;
