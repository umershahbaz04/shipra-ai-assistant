import AddBoxIcon from "@mui/icons-material/AddBox";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import AddProductModal from "../../../components/modals/orderModals/AddProductModal";
import { LoadingButton } from "@mui/lab";
import {
  Box,
  Button,
  CircularProgress,
  Divider,
  FormControl,
  Grid,
  InputLabel,
  OutlinedInput,
  Table,
  TableCell,
  TableBody,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
  Avatar,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import React, { useCallback, useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import IndeterminateCheckBoxOutlinedIcon from "@mui/icons-material/IndeterminateCheckBoxOutlined";
import { useNavigate } from "react-router-dom";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import AddBoxOutlinedIcon from "@mui/icons-material/AddBoxOutlined";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAllCountry,
  GetAllRegionbyCountryId,
  GetAllStationLookup,
  GetChannelListByStoreIdForSelection,
  GetCityByRegionId,
  GetStoresForSelection,
  GetProductStocksForSelection,
  GetValidateClientPPActivate,
  GetAllOrderTypeLookup,
  GetAddressFromLatAndLong,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { handleDispatchUserProfile } from "../../../components/topNavBar";
import DeleteIcon from "@mui/icons-material/Delete";
import {
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
  PaymentTaxAlert,
  PaymentTaxBox,
  PaymentTotalBox,
  amountFormat,
  calculateSubtotalValue,
  decimalFormat,
  fetchMethod,
  fetchMethodResponse,
  getDefaultValueIndex,
  getLowerCase,
  getOptionValueObjectByValue,
  getSelectedOrInputValueForApi,
  handleCopyToClipBoard,
  placeholders,
  truncate,
  useGetAllClientTax,
  useMapAutocompleteSetter,
  usePhoneInput,
} from "../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../utilities/toast";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
export const calculateVatValue = (data) => {
  let value = 0;
  Object.values(data).forEach((val) => {
    value += Number(val);
  });
  return isNaN(value) ? 0 : value;
};
const SaleChannelOrderModal = (props) => {
  const {
    open,
    onClose,
    orderData,
    selectedRowIndex,
    allOrders,
    setAllOrders,
    setErrorsList,
    errorsList,
  } = props;
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allCountries, setAllCountries] = useState([]);
  const [allRegions, setAllRegions] = useState([]);
  const [totalItemPrice, setTotalItemPrice] = useState();
  const [allCities, setAllCities] = useState([]);
  const [discount, setDiscount] = useState(0);
  const [finalNote, setFinalNote] = useState("");
  const [shipping, setShipping] = useState(0);
  const [subtotal, setSubtotal] = useState();
  const [finalRemarks, setFinalRemarks] = useState("");
  const [vatValue, setVatValue] = useState({});
  const [note, setNote] = useState(orderData.orderNote.note);
  const [productStations, setProductStations] = useState([]);
  const [storesChannelForSelection, setStoresChannelForSelection] = useState(
    []
  );
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [paymentLinks, setPaymentLinks] = useState({
    show: false,
    paymentUrl: "",
    invoicePdf: "",
  });
  const [autocomplete, setAutocomplete] = useState(null);
  const [isStripeSettingExist, setStripeSettingExist] = useState(false);
  const [isLoadingForConfirm, setIsLoadingForConfirm] = useState(false);
  const [selectedProducts, setSelectedProducts] = useState([]);
  const [isProductFetching, setIsProductFetching] = useState(false);
  const [Open, setOpen] = useState(false);
  const [remarks, setRemarks] = useState("");
  const [selectedPMOption, setSelectedPMOption] = useState(1);
  const [qtySelectedStation, setQtySelectedStation] = useState([]);
  const [isEditOrder, setIsEditOrder] = useState(false);
  const [isLoadingForConfirmAndNewOrder, setIsLoadingForConfirmAndNewOrder] =
    useState(false);
  const [productStocksForSelection, setProductStocksForSelection] = useState(
    []
  );
  const [
    isLoadingForConfirmAndSendInvoiceOrder,
    setIsLoadingForConfirmAndSendInvoiceOrder,
  ] = useState(false);
  const [selectedOrderPlaceButtom, setSelectedOrderPlaceButtom] =
    useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
    getValues,
    setValue,
    control,
    unregister,
  } = useForm({
    defaultValues: {
      descriptionOfPieces: [],
    },
  });

  useMapAutocompleteSetter(
    autocomplete,
    allCountries,
    allRegions,
    allCities,
    setValue
  );

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
  } = useGetAddressSchema();

  const LanguageReducer = useSelector((state) => state.LanguageReducer);

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

  const paymentMethodoptions = [
    { id: 1, label: "Prepaid" },
    { id: 2, label: "Cash On Delivery" },
  ];
  useEffect(() => {
    if (orderData) {
      let order = orderData?.order;
      setFinalRemarks(order?.remarks);
      setRemarks(order?.remarks);
      handleCheckboxChange(order?.paymentMethodId || EnumPaymentMethod.Prepaid);
      setNote(orderData?.orderNote?.note);
      setFinalNote(orderData?.orderNote?.note);

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
      qtySelectedStation?.productStationId || orderData?.stationId;
    setIsProductFetching(true);

    let res = await GetProductStocksForSelection(
      0,
      getValues("store")?.storeId || orderData?.order?.storeId
    );

    // console.log("data:::", res.data);
    if (res.data.result != null && res.data.result?.length > 0) {
      for (let index = 0; index < res.data?.result.length; index++) {
        //get selected object against station

        let obj = orderData?.orderItems?.find(
          (x) => x.productStockId == res.data.result[index].ProductStockId
        );
        //orderItemId

        res.data.result[index].checked = obj ? true : false;
        res.data.result[index].isRemove =
          obj && obj?.orderItemId ? true : false; //if edit case then remove row
        res.data.result[index].discount = 0;
        res.data.result[index].newPrice = res.data.result[index].Price;
        res.data.result[index].newQuantity = 1;
        res.data.result[index].stationId = qtySelectedStation?.productStationId;
        // res.data.result[index].QuantityAvailable;
      }

      /////////
      let preloadedProducts = [];
      orderData?.orderItems.forEach((item, i) => {
        let value = res.data.result?.find(
          (x) => x.ProductStockId == item.productStockId
        );
        if (value) {
          value["orderItemId"] = item.orderItemId;
          value["checked"] = true;
          value["Price"] = item.price;
          value["newPrice"] = item.price;
          value["newQuantity"] = item.quantity;
          value["discount"] = item.discount;
          preloadedProducts.push(value);
        }
      });
      setSelectedProducts(preloadedProducts);
      setTotalItemPrice(orderData?.amount);
      ///////////
      setProductStocksForSelection(res.data.result);
    } else {
      errorNotification("No product found agains't selected store.");
    }

    setIsProductFetching(false);
  };

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
    name: "station",
    control,
  });
  useWatch({
    name: "numberOfPieces",
    control,
  });
  useWatch({
    name: "lat&long",
    control,
  });
  const { allClientTax, loading } = useGetAllClientTax([]);

  const getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    const _stores = res.data.result;
    setValue(
      "store",
      getOptionValueObjectByValue(
        _stores,
        EnumOptions.STORE.VALUE,
        orderData?.storeId
      )
    );
    setStoresForSelection(_stores);
  };
  const getChannelListByStoreIdForSelection = async () => {
    setValue("storeChannel", null);

    if (getValues("store").storeId) {
      let res = await GetChannelListByStoreIdForSelection(
        getValues("store").storeId
      );
      const storeChannel = res.data.result;
      if (storeChannel) {
        setValue(
          "storeChannel",
          getOptionValueObjectByValue(
            storeChannel,
            EnumOptions.STORE_CHANNEL.VALUE,
            orderData?.saleChannelConfigId
          )
        );
      }
      setStoresChannelForSelection(res.data.result || []);
    }
  };
  let getAllStationLookup = async () => {
    let res = await GetAllStationLookup();
    const _station = res.data.result;
    setValue(
      "station",
      getOptionValueObjectByValue(
        _station,
        EnumOptions.SELECT_STATION.VALUE,
        orderData?.stationId
      )
    );
    if (res.data.result != null) setProductStations(res.data.result);
  };

  let getValidateClientPPActivate = async () => {
    let res = await GetValidateClientPPActivate();
    if (res.data.isSuccess) {
      setStripeSettingExist(res.data.result.isValidate);
    } else {
      setStripeSettingExist(false);
    }
  };

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    const _country = res.data.result;
    setValue(
      "country",
      getOptionValueObjectByValue(
        _country,
        EnumOptions.COUNTRY.VALUE,
        orderData?.orderAddress.countryId
      )
    );
    if (res.data.result != null) setAllCountries(res.data.result);
  };

  const handleFocus = (event) => event.target.select();
  const handleCheckboxChange = (optionId) => {
    setSelectedPMOption(optionId);
  };

  const handleOnClickProduct = () => {
    getProductStocksForSelection();
    setOpen(true);
  };
  const updateData = (data) => {
    const actualAmmount = Number(subtotal);
    let amount = 0;
    let itemValues = subtotal;
    let itemValue = 0;
    let orderItems = [];
    let description = [];
    if (orderData.orderTypeId === EnumOrderType.Regular) {
      amount =
        Number(subtotal) +
        calculateVatValue(vatValue) +
        Number(shipping) -
        Number(discount);
      for (let index = 0; index < data.numberOfPieces; index++) {
        orderItems[index] = {
          price: 0,
          description:
            data.descriptionOfPieces.length > 0
              ? data.descriptionOfPieces[index]
              : "",
          remarks: "",
          quantity: 1,
          discount: 0,
        };
      }
    }
    if (orderData.orderTypeId === EnumOrderType.FullFillable) {
      amount =
        Number(totalItemPrice) +
        calculateVatValue(vatValue) +
        Number(shipping) -
        Number(discount);
      for (let index = 0; index < selectedProducts.length; index++) {
        let ditem = `${selectedProducts[index].SKU}_${selectedProducts[index].newQuantity}`;
        description.push(ditem);
        itemValue =
          itemValue +
          (selectedProducts[index].newPrice -
            selectedProducts[index].discount) *
            selectedProducts[index].newQuantity;
        orderItems[index] = {
          productId: selectedProducts[index].ProductId,
          productStockId: selectedProducts[index].ProductStockId,
          price: selectedProducts[index].newPrice,
          description: ditem,
          remarks: "",
          quantity: selectedProducts[index].newQuantity,
          discount: selectedProducts[index].discount,
          StockSku: selectedProducts[index].SKU,
        };
      }
    }
    const updatedObject = {
      index: selectedRowIndex,
      storeId: data.store.storeId,
      SaleChannelConfigId: data.storeChannel?.id,
      orderTypeId: orderData.orderTypeId,
      orderDate: data?.orderDate,
      description: data.description,
      remarks: data.remarks,
      amount: amount,
      actualAmmount: actualAmmount,
      cShippingCharges: shipping,
      paymentStatusId:
        EnumPaymentMethod.Prepaid == selectedPMOption
          ? EnumPaymentStatus.Paid
          : EnumPaymentStatus.Unpaid,
      weight: data.weight,
      ItemsCount: data?.numberOfPieces,
      itemValue:
        orderData.orderTypeId === EnumOrderType.FullFillable
          ? itemValue
          : itemValues,
      orderRequestVia: 1,
      paymentMethodId: selectedPMOption,
      discount: discount,
      vat: 0,
      refNo: data?.refNo,
      orderNote: {
        note: finalNote,
      },
      orderAddress: {
        area: selectedAddressSchema.area,
        city: selectedAddressSchema.city,
        country: selectedAddressSchema.country,
        customerFullAddress: null,
        customerName: data.customerName,
        email: data.email,
        houseNo: selectedAddressSchema.houseNo,
        buildingName: selectedAddressSchema.buildingName,
        landmark: selectedAddressSchema.landmark,
        zip: selectedAddressSchema.zip,
        latitude: getValues("lat&long").split(",")[0] || null,
        longitude: getValues("lat&long").split(",")[1] || null,
        mobile1: data.mobile1,
        mobile2: data.mobile2,
        orderAddressId: 0,
        pinCode: selectedAddressSchema.pinCode,
        province: selectedAddressSchema.province,
        state: selectedAddressSchema.state,
        streetAddress: selectedAddressSchema.streetAddress,
        streetAddress2: selectedAddressSchema.streetAddress2,
      },
      orderTaxes: orderData?.orderTaxes
        .map((tax) => {
          return {
            orderTaxId: tax.orderTaxId,
            clientTaxId: tax.clientTaxId,
            taxValue: Number(vatValue[tax.clientTaxId]),
          };
        })
        .filter((dt) => dt !== undefined),
      orderItems: orderItems,
    };
    setAllOrders((prev) => {
      const _allOrders = [...prev];
      _allOrders[selectedRowIndex] = {
        ..._allOrders[selectedRowIndex],
        stationId: getValues("station")?.productStationId,
        storeName: data.store?.storeName,
        ...updatedObject,
      };
      return _allOrders;
    });
    const filtersData = errorsList.filter((x) => x.Row != selectedRowIndex + 1);
    setErrorsList(filtersData);
    onClose();
  };
  useEffect(() => {
    if (productStations.length > 0) {
      let selectedStation = productStations[0];
      if (orderData) {
        selectedStation = productStations?.find(
          (x) => x.productStationId == orderData?.stationId
        );
        selectedStation = selectedStation;
      }
      setQtySelectedStation(selectedStation);
    }
  }, [productStations]);
  useEffect(() => {
    getAllCountry();
    getStoresForSelection();
    getValidateClientPPActivate();
    getStoresForSelection();
    getAllStationLookup();
    getValidateClientPPActivate();
    // getAllOrderTypeLookup();
    setTotalItemPrice(orderData?.amount);
    setSubtotal(orderData?.amount);
    setDiscount(orderData?.discount);
    setShipping(orderData?.cShippingCharges);
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

  useEffect(() => {
    if (orderData) {
      setValue("mobile1", orderData?.orderAddress.mobile1);
      setValue("mobile2", orderData?.orderAddress.mobile2);
    }
  }, []);
  useEffect(() => {
    (async () => {
      setValue("mobile1", null);
      setValue("mobile2", null);
      await handleSetSchemaValueForUpdate(orderData?.orderAddress, setValue);
      setValue("mobile1", orderData?.orderAddress.mobile1);
      setValue("mobile2", orderData?.orderAddress.mobile2);
      let latitude = orderData?.orderAddress?.latitude;
      let longitude = orderData?.orderAddress?.longitude;
      if (latitude !== null && longitude !== null) {
        setValue("lat&long", `${latitude},${longitude}`);
      }
    })();
  }, []);

  const PhoneInput1 = usePhoneInput("mobile1", control, getValues, true);
  const PhoneInput2 = usePhoneInput("mobile2", control, getValues, false, true);

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
      GetAddressFromLatAndLong(body)
    );
    fetchMethodResponse(response, "Address Updated", () =>
      handleSetSchemaValueForUpdate(response.result, setValue)
    );
  };

  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="xl"
        title={"Update Order"}
        actionBtn={
          <ModalButtonComponent
            title={"Update Order"}
            bg={purple}
            onClick={handleSubmit(updateData)}
          />
        }
      >
        {/* <Box width="50%" margin="auto">
          <SelectComponent
            name="orderType"
            control={control}
            options={allOrderTypeLookup}
            getOptionLabel={(option) => option?.name}
            optionLabel={EnumOptions.ORDER_TYPE.LABEL}
            optionValue={EnumOptions.ORDER_TYPE.VALUE}
            value={getValues("orderType")}
            onChange={(event, newValue) => {
              let resolvedId = newValue ? newValue : null;
              setValue("orderType", resolvedId);
              setOrderTypeValue(resolvedId);
            }}
          />
        </Box> */}
        {orderData.orderTypeId === EnumOrderType.Regular ? (
          <GridContainer>
            <GridItem xs={7}>
              <GridContainer>
                {/* store */}
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    label={LanguageReducer?.languageType?.ORDERS_STORE}
                  >
                    <Grid container spacing={2}>
                      <Grid item md={6} sm={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
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
                          {...register("store", {
                            required: {
                              value: true,
                            },
                          })}
                          value={getValues("store")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("store", resolvedId);
                            if (
                              selectedPMOption === EnumPaymentMethod.Prepaid
                            ) {
                              setStripeSettingExist(false);
                            }
                          }}
                          errors={errors}
                        />
                      </Grid>
                      <Grid item md={6} lg={6} sm={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          Sale Channel
                        </InputLabel>
                        <SelectComponent
                          name="storeChannel"
                          control={control}
                          options={storesChannelForSelection}
                          getOptionLabel={(option) => option?.text}
                          isRHF={true}
                          optionLabel={EnumOptions.STORE_CHANNEL.LABEL}
                          optionValue={EnumOptions.STORE_CHANNEL.VALUE}
                          disabled={
                            getValues("store")
                              ? getValues("store").storeId === 0
                              : true
                          }
                          {...register("storeChannel", {
                            // required: {
                            //   value: true,
                            // },
                          })}
                          value={getValues("storeChannel")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("storeChannel", resolvedId);
                          }}
                          // errors={errors}
                        />
                      </Grid>
                      <Grid item md={6} sm={6}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_STATIONS}
                        </InputLabel>
                        <SelectComponent
                          name="station"
                          control={control}
                          options={productStations}
                          getOptionLabel={(option) => option?.sname}
                          isRHF={true}
                          required={true}
                          optionLabel={EnumOptions.SELECT_STATION.LABEL}
                          optionValue={EnumOptions.SELECT_STATION.VALUE}
                          isRefesh={true}
                          handleRefreshClick={getAllStationLookup}
                          {...register("station", {
                            required: {
                              value: true,
                            },
                          })}
                          value={getValues("station")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("station", resolvedId);
                          }}
                          errors={errors}
                        />
                      </Grid>
                      <Grid item md={6} lg={6} sm={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
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
                      <Grid item md={12} sm={12}>
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
                          defaultValue={orderData?.refNo}
                          {...register("refNo")}
                        />
                      </Grid>
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
                {/* customer */}
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    label={LanguageReducer?.languageType?.ORDERS_CUSTOMER}
                  >
                    <Grid container spacing={2}>
                      <Grid item md={6} sm={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_CUSTOMER_NAME}
                        </InputLabel>
                        <TextField
                          type="text"
                          placeholder={placeholders.name}
                          onFocus={handleFocus}
                          size="small"
                          fullWidth
                          variant="outlined"
                          id="customerName"
                          name="customerName"
                          defaultValue={orderData?.orderAddress?.customerName}
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
                      <Grid item md={6} sm={12}>
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
                        <PhoneInput1 />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_CONTACT_NUMBER}
                        </InputLabel>
                        <PhoneInput2 />
                      </Grid>
                      <Grid item md={4} sm={4} xs={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_COUNTRY}
                        </InputLabel>
                        <CountrySchema
                          name="country"
                          control={control}
                          getOptionLabel={(option) => option?.name}
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
                            handleSetSchema(
                              "country",
                              resolvedId,
                              setValue,
                              unregister
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
                            disabled={input.disabled}
                            isRHF={true}
                            type={input.type}
                            name={input.key}
                            required={input.required}
                            optionLabel={addressSchemaEnum[input.key]?.LABEL}
                            optionValue={addressSchemaEnum[input.key]?.VALUE}
                            register={register}
                            options={input.options}
                            errors={errors}
                            label={input.label}
                            value={getValues(input.key)}
                            viewMode={true}
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
                                      input.key
                                    );
                                  }
                                : (e) => {
                                    handleChangeInputAddressSchema(
                                      input.key,
                                      e.target.value,
                                      setValue
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
                            label={LanguageReducer?.languageType?.FROM_MAP_TEXT}
                            height={styleSheet.fromMapButton}
                          />
                        </Grid>
                      </GridContainer>
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
                {/* order details */}
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    label={LanguageReducer?.languageType?.ORDER_DETAILS_TEXT}
                  >
                    <Grid container spacing={2}>
                      <Grid item md={6} sm={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_DESCRIPTION}
                        </InputLabel>
                        <TextField
                          placeholder="Description"
                          onFocus={handleFocus}
                          size="small"
                          multiline
                          fullWidth
                          rows={4}
                          variant="outlined"
                          id="description"
                          name="description"
                          defaultValue={orderData?.description}
                          {...register("description", {
                            pattern: {
                              value: /^(?!\s*$).+/,
                              message:
                                LanguageReducer?.languageType
                                  ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                            },
                          })}
                          error={Boolean(errors.description)}
                          helperText={errors.description?.message}
                        />
                      </Grid>
                      <Grid item md={6} sm={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_REMARKS}
                        </InputLabel>
                        <TextField
                          placeholder="Remarks"
                          onFocus={handleFocus}
                          size="small"
                          multiline
                          fullWidth
                          rows={4}
                          variant="outlined"
                          id="remarks"
                          name="remarks"
                          defaultValue={orderData.remarks}
                          {...register("remarks", {
                            pattern: {
                              value: /^(?!\s*$).+/,
                              message:
                                LanguageReducer?.languageType
                                  ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                            },
                          })}
                          error={Boolean(errors.remarks)}
                          helperText={errors.remarks?.message}
                        />
                      </Grid>
                      <Grid item md={6} xs={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {
                            LanguageReducer?.languageType
                              ?.ORDERS_NUMBER_OF_PIECES
                          }
                        </InputLabel>
                        <TextField
                          placeholder={placeholders.quantity}
                          onFocus={handleFocus}
                          type="number"
                          size="small"
                          id="numberOfPieces"
                          name="numberOfPieces"
                          fullWidth
                          variant="outlined"
                          defaultValue={1}
                          {...register("numberOfPieces", {
                            required: {
                              value: true,
                              message:
                                LanguageReducer?.languageType
                                  ?.FIELD_REQUIRED_TEXT,
                            },
                          })}
                          error={Boolean(errors.numberOfPieces)} // set error prop
                          helperText={errors.numberOfPieces?.message}
                        />
                      </Grid>
                      <Grid item md={6} xs={12}>
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
                          defaultValue={orderData?.weight}
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
                      {Array.from(
                        { length: getValues("numberOfPieces") },
                        (_, index) => index
                      ).map((i, index) => (
                        <Grid item md={6} xs={12}>
                          <InputLabel sx={styleSheet.inputLabel}>
                            {
                              LanguageReducer?.languageType
                                ?.ORDERS_DESCRIPTION_OF_PIECE_1
                            }{" "}
                            {i + 1}
                          </InputLabel>
                          <TextField
                            placeholder={`${
                              LanguageReducer?.languageType
                                ?.ORDERS_DESCRIPTION_OF_PIECE_1
                            } ${i + 1}`}
                            type="text"
                            size="small"
                            onFocus={handleFocus}
                            id={`descriptionOfPieces.${i}`}
                            name={`descriptionOfPieces.${i}`}
                            fullWidth
                            variant="outlined"
                            {...register(`descriptionOfPieces.${i}`, {
                              // required: {
                              //   value: true,
                              //   message:
                              //     LanguageReducer?.languageType
                              //       ?.FIELD_REQUIRED_TEXT,
                              // },
                            })}
                            error={Boolean(errors.descriptionOfPieces?.[i])} // set error prop
                            helperText={
                              errors.descriptionOfPieces?.[i]?.message
                            }
                          />
                        </Grid>
                      ))}
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
              </GridContainer>
            </GridItem>
            <GridItem xs={5}>
              <GridContainer>
                {/* payment */}
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    label={LanguageReducer?.languageType?.ORDERS_PAYMENT}
                  >
                    <Box display={"flex"} flexDirection={"column"} gap={1}>
                      <PaymentAmountBox
                        title={LanguageReducer?.languageType?.ORDERS_SUBTOTAL}
                        required
                        value={subtotal}
                        onChange={(e) => {
                          setSubtotal(e.target.value);
                        }}
                      />
                      <PaymentAmountBox
                        title={
                          LanguageReducer?.languageType?.ORDERS_ADD_DISCOUNT
                        }
                        value={discount}
                        onChange={(e) => {
                          setDiscount(e.target.value);
                        }}
                      />
                      <PaymentAmountBox
                        title={
                          LanguageReducer?.languageType?.ORDERS_ADD_SHIPPING
                        }
                        value={shipping}
                        onChange={(e) => {
                          setShipping(e.target.value);
                        }}
                      />
                      {!loading &&
                        (allClientTax.length === 0 ||
                          allClientTax.every(
                            (item) => item.Active === false
                          )) && (
                          <PaymentTaxAlert
                            onClick={() =>
                              handleDispatchUserProfile(
                                dispatch,
                                true,
                                navigate
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
                                subtotal={subtotal}
                              />
                            );
                          }
                        })
                      )}
                      <PaymentTotalBox
                        value={(
                          Number(subtotal) +
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
                      {isStripeSettingExist &&
                        selectedPMOption !== EnumPaymentMethod.Prepaid && (
                          <Grid item>
                            {paymentLinks.show ? (
                              <Box className={"flex_center"} gap={1}>
                                <LoadingButton
                                  onClick={() =>
                                    handleCopyToClipBoard(
                                      paymentLinks.paymentUrl
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
                                      paymentLinks.paymentUrl
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
                                // loading={isLoadingForConfirmAndSendInvoiceOrder}
                                // loadingPosition="start"
                                // onClick={handleSubmit(handleSendInvoice)}
                                // disabled={
                                //   selectedOrderPlaceButtom &&
                                //   selectedOrderPlaceButtom !==
                                //     EnumOrderPlaceButton.ConfirmAndHandleInvoice
                                // }
                              >
                                Confirm And Get Payment Link
                              </LoadingButton>
                            )}
                          </Grid>
                        )}
                    </Grid>
                  </CustomColorLabelledOutline>{" "}
                </GridItem>
                {/* Note */}
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    label={LanguageReducer?.languageType?.ORDERS_NOTE}
                  >
                    <center>
                      <Typography
                        sx={{ fontSize: "14px !important", width: "40ch" }}
                      >
                        {finalNote
                          ? finalNote
                          : LanguageReducer?.languageType?.ORDERS_NOTE}
                      </Typography>
                    </center>
                    <FormControl
                      fullWidth
                      variant="outlined"
                      sx={{ marginTop: "22px" }}
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
                        placeholder={"Describe Here"}
                      />
                    </FormControl>
                  </CustomColorLabelledOutline>
                </GridItem>
              </GridContainer>
            </GridItem>
          </GridContainer>
        ) : orderData.orderTypeId === EnumOrderType.FullFillable ? (
          <GridContainer>
            <GridItem xs={7}>
              <GridContainer>
                <GridItem xs={12}>
                  {" "}
                  <CustomColorLabelledOutline
                    label={LanguageReducer?.languageType?.ORDERS_STORE}
                  >
                    <Grid display={"flex"} gap={1}>
                      <Grid md={6} lg={6} sm={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
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
                          {...register("store", {
                            required: {
                              value: true,
                            },
                          })}
                          value={getValues("store")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("store", resolvedId);
                            if (
                              selectedPMOption !== EnumPaymentMethod.Prepaid
                            ) {
                              setStripeSettingExist(true);
                            }
                          }}
                          errors={errors}
                        />
                      </Grid>
                      <Grid md={6} lg={6} sm={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          Sale Channel
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
                    </Grid>
                    <Grid display={"flex"} gap={1} mt={1}>
                      {getValues("store")?.storeId &&
                      getValues("store")?.storeId > 0 ? (
                        <Grid item sm={12} md={6} lg={6} mb={2}>
                          <InputLabel required sx={styleSheet.inputLabel}>
                            {LanguageReducer?.languageType?.ORDERS_STATIONS}
                          </InputLabel>
                          <SelectComponent
                            name="station"
                            options={productStations}
                            isRHF={true}
                            required={true}
                            optionLabel={EnumOptions.SELECT_STATION.LABEL}
                            optionValue={EnumOptions.SELECT_STATION.VALUE}
                            isRefesh={true}
                            handleRefreshClick={getAllStationLookup}
                            {...register("station", {
                              required: {
                                value: true,
                              },
                            })}
                            value={getValues("station")}
                            onChange={(event, newValue) => {
                              const resolvedId = newValue ? newValue : null;
                              setValue("station", resolvedId);
                              setQtySelectedStation(resolvedId);
                            }}
                            errors={errors}
                          />
                        </Grid>
                      ) : null}
                      <Grid
                        md={
                          getValues("store")?.storeId &&
                          getValues("store")?.storeId > 0
                            ? 6
                            : 12
                        }
                        sm={12}
                      >
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
                    </Grid>
                    <Grid item md={12} sm={12}>
                      <InputLabel sx={styleSheet.inputLabel}>
                        {LanguageReducer?.languageType?.ORDERS_REF_NO}
                      </InputLabel>
                      <TextField
                        type="text"
                        placeholder={placeholders.refNo}
                        onFocus={handleFocus}
                        size="small"
                        defaultValue={orderData?.refNo}
                        fullWidth
                        variant="outlined"
                        id="refNo"
                        name="refNo"
                        {...register("refNo")}
                      />
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
                {true ? (
                  <GridItem xs={12}>
                    <CustomColorLabelledOutline
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
                                                25
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
                                              newSelectedProducts
                                            );
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
                                            newSelectedProducts
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
                                                value.ProductStockId
                                            )
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
                    label={LanguageReducer?.languageType?.ORDERS_CUSTOMER}
                  >
                    <Grid container spacing={2}>
                      <Grid item md={6} sm={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
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
                          defaultValue={orderData?.orderAddress?.customerName}
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
                      <Grid item md={6} sm={12}>
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
                        <PhoneInput1 />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_CONTACT_NUMBER}
                        </InputLabel>
                        <PhoneInput2 />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
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
                          {...register("country", {
                            required: {
                              value: true,
                            },
                          })}
                          value={getValues("country")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            handleSetSchema(
                              "country",
                              resolvedId,
                              setValue,
                              unregister
                            );
                          }}
                          errors={errors}
                        />
                      </Grid>
                      {[
                        ...addressSchemaSelectData,
                        ...addressSchemaInputData,
                      ].map((input, index, arr) => (
                        <Grid item md={6} sm={12}>
                          <SchemaTextField
                            loading={input.loading}
                            disabled={input.disabled}
                            isRHF={true}
                            type={input.type}
                            name={input.key}
                            required={input.required}
                            optionLabel={addressSchemaEnum[input.key]?.LABEL}
                            optionValue={addressSchemaEnum[input.key]?.VALUE}
                            register={register}
                            options={input.options}
                            errors={errors}
                            label={input.label}
                            value={getValues(input.key)}
                            viewMode={true}
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
                                      input.key
                                    );
                                  }
                                : (e) => {
                                    handleChangeInputAddressSchema(
                                      input.key,
                                      e.target.value,
                                      setValue
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
                            label={LanguageReducer?.languageType?.FROM_MAP_TEXT}
                            height={styleSheet.fromMapButton}
                          />
                        </Grid>
                      </GridContainer>
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
              </GridContainer>
            </GridItem>
            <GridItem xs={5}>
              <GridContainer>
                <GridItem xs={12}>
                  {" "}
                  <CustomColorLabelledOutline
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
                            (item) => item.Active === false
                          )) && (
                          <PaymentTaxAlert
                            onClick={() =>
                              handleDispatchUserProfile(
                                dispatch,
                                true,
                                navigate
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
                      {isStripeSettingExist &&
                        selectedPMOption !== EnumPaymentMethod.Prepaid && (
                          <Grid item>
                            {paymentLinks.show ? (
                              <Box className={"flex_center"} gap={1}>
                                <LoadingButton
                                  onClick={() =>
                                    handleCopyToClipBoard(
                                      paymentLinks.paymentUrl
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
                                      paymentLinks.paymentUrl
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
                                //   loading={isLoadingForConfirmAndSendInvoiceOrder}
                                //   loadingPosition="start"
                                //   onClick={handleSubmit(handleSendInvoice)}
                                //   disabled={
                                //     selectedOrderPlaceButtom &&
                                //     selectedOrderPlaceButtom !==
                                //       EnumOrderPlaceButton.ConfirmAndHandleInvoice
                                //   }
                              >
                                Confirm And Get Payment Link
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
                    label={LanguageReducer?.languageType?.ORDERS_NOTE}
                  >
                    <center>
                      <Typography
                        sx={{ fontSize: "14px !important", width: "40ch" }}
                      >
                        {finalNote
                          ? finalNote
                          : LanguageReducer?.languageType?.ORDERS_NOTE}
                      </Typography>
                    </center>
                    <FormControl
                      fullWidth
                      variant="outlined"
                      sx={{ marginTop: "22px" }}
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
                        defaultValue={orderData.orderNote.note}
                        placeholder={"Describe Here"}
                      />
                    </FormControl>
                  </CustomColorLabelledOutline>
                </GridItem>
                <GridItem xs={12}>
                  <CustomColorLabelledOutline label={"Remarks"}>
                    <center>
                      <Typography
                        sx={{ fontSize: "14px !important", width: "40ch" }}
                      >
                        {finalRemarks ? finalRemarks : null}
                      </Typography>
                    </center>
                    <FormControl
                      fullWidth
                      variant="outlined"
                      sx={{ marginTop: "22px" }}
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
                        placeholder={"Remarks Here"}
                      />
                    </FormControl>
                  </CustomColorLabelledOutline>
                </GridItem>
              </GridContainer>
            </GridItem>
          </GridContainer>
        ) : null}
      </ModalComponent>
      {open && (
        <AddProductModal
          open={Open}
          productStations={productStations}
          setOpen={setOpen}
          productStocksForSelection={productStocksForSelection}
          setSelectedProducts={setSelectedProducts}
          selectedProducts={selectedProducts}
          setProductStocksForSelection={setProductStocksForSelection}
        />
      )}
      <GoogleMapWithSearch
        open={openLocationModal}
        setOpen={setOpenLocationModal}
        setValue={setValue}
        setAutocomplete={setAutocomplete}
        splitLatAndLong={splitLatAndLong}
      />
    </>
  );
};

export default SaleChannelOrderModal;
