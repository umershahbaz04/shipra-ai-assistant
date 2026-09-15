import AddBoxIcon from "@mui/icons-material/AddBox";
import AddBoxOutlinedIcon from "@mui/icons-material/AddBoxOutlined";
import DeleteIcon from "@mui/icons-material/Delete";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import IndeterminateCheckBoxOutlinedIcon from "@mui/icons-material/IndeterminateCheckBoxOutlined";
import {
  Avatar,
  Box,
  Button,
  CircularProgress,
  Divider,
  FormControl,
  Grid,
  InputLabel,
  OutlinedInput,
  TableCell,
  TableHead,
  TextField,
  Typography,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableRow from "@mui/material/TableRow";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAddressFromLatAndLong,
  GetAllClientOrderBox,
  GetAllCountry,
  GetAllRegionbyCountryId,
  GetAllStationLookup,
  GetChannelListByStoreIdForSelection,
  GetCityByRegionId,
  GetProductStocksForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { calculateVatValue } from "../../../pages/orders/createRegularOrder";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  EnumOptions,
  EnumPaymentMethod,
  EnumPaymentStatus,
} from "../../../utilities/enum";
import {
  ActionButtonCustom,
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
  decimalFormat,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  getSelectedValueFromStringOrObject,
  placeholders,
  purple,
  truncate,
  useGetAllClientTax,
  useMapAutocompleteSetter,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import { errorNotification } from "../../../utilities/toast";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch";
import AddProductModal from "./AddProductModal";
import AddOrderBoxModal from "./AddOrderBoxModal";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function EditFullfilableOrderDetailsModal(props) {
  let {
    open,
    setOpen,
    selectedData,
    storesForSelection,
    setSelectedData,
    orderData,
    setOrderData,
    selectedRowIndex,
    setSelectedRowIndex,
    errorsList,
    setErrorsList,
    getStoresForSelection,
  } = props;
  const [selectedProducts, setSelectedProducts] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    handleRestForm();
    setOpen(false);
  };
  const [openProductModal, setOpenProductModal] = useState(false);
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [addProductOpen, setAddProductOpen] = useState(false);
  const [openFromLinkLocationModal, setOpenFromLinkLocationModal] =
    useState(false);
  const [productStocksForSelection, setProductStocksForSelection] = useState(
    [],
  );
  const [isProductFetching, setIsProductFetching] = useState(false);
  const [openUpdateOrderBoxModal, setopenUpdateOrderBoxModal] = useState({
    open: false,
    loading: {},
  });
  const [allClientOrderBox, setAllClientOrderBox] = useState([]);
  const [selectedOrderBox, setSelectedOrderBox] = useState([]);
  const [addedOrderBoxes, setAddedOrderBoxes] = useState([]);
  const [allCountries, setAllCountries] = useState([]);
  const [allRegions, setAllRegions] = useState([]);
  const [allCities, setAllCities] = useState([]);
  const [discount, setDiscount] = useState(selectedData?.discount);
  const [shipping, setShipping] = useState(selectedData?.cShippingCharges);
  const [tax, setTax] = useState(5);
  const [note, setNote] = useState("");
  const [finalNote, setFinalNote] = useState("");
  const [remarks, setRemarks] = useState("");
  const [finalRemarks, setFinalRemarks] = useState("");
  const {
    register,
    unregister,
    handleSubmit,
    formState: { errors },
    reset,
    getValues,
    setValue,
    control,
  } = useForm();
  const handleRestForm = () => {
    setFinalNote("");
    setNote("");
    setRemarks("");
    setFinalRemarks("");
    setDiscount(0);
    setShipping(0);
    setTax(5);
    setSelectedProducts([]);
    setTotalItemPrice(0);
    reset();
    setSelectedData();
    setValue("mobile1", UtilityClass.getDefaultCountryCode());
    setValue("mobile2", UtilityClass.getDefaultCountryCode());
  };
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
  const [paymentDueLaterCheckBox, setPaymentDueLaterCheckBox] = useState(false);
  const [selectedPMOption, setselectedPMOption] = useState(1);
  const [vatValue, setVatValue] = useState(0);

  const [storesChannelForSelection, setStoresChannelForSelection] = useState(
    [],
  );
  const [isEditOrder, setIsEditOrder] = useState(false);

  const [productStations, setProductStations] = useState([]);
  const [qtySelectedStation, setQtySelectedStation] = useState({
    productStationId: 0,
    sname: "Choose station",
  });
  const [totalItemPrice, setTotalItemPrice] = useState(0);
  const [autocomplete, setAutocomplete] = useState(null);
  let getAllStationLookup = async () => {
    let res = await GetAllStationLookup();
    // console.log("getAllStationLookup", res.data);
    if (res.data.result != null) setProductStations(res.data.result);
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
    getAllStationLookup();
  }, []);
  const replaceObjectAtIndex = (updatedObject) => {
    const newArray = [...orderData];
    const modifiedObject = { ...updatedObject };
    newArray[selectedData?.index - 1] = modifiedObject;
    setOrderData(newArray);
  };
  const getGrandTotal = () => {
    return amountFormat(
      Number(totalItemPrice) +
        calculateVatValue(vatValue) +
        Number(shipping) -
        Number(discount),
    );
  };

  const updateData = (data) => {
    let amount = 0;
    let weight = 0;
    let itemValue = 0;
    let description = [];
    let orderItems = [];
    for (let index = 0; index < selectedProducts.length; index++) {
      let ditem = `${selectedProducts[index].SKU}_${selectedProducts[index].newQuantity}`;

      amount = getGrandTotal();
      weight = weight + selectedProducts[index].Weight;
      // description = `${description},${selectedProducts[index].Sku}_${
      //   selectedProducts[index].newQuantity
      //     ? selectedProducts[index].newQuantity
      //     : selectedProducts[index].QuantityAvailable
      // }`;
      description.push(ditem);
      itemValue =
        itemValue +
        (selectedProducts[index].newPrice - selectedProducts[index].discount) *
          selectedProducts[index].newQuantity;
      orderItems[index] = {
        orderItemId: selectedProducts[index]?.orderItemId || "",
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
    if (orderItems.length == 0) {
      errorNotification("Please add product to proceed");
      return false;
    }
    const actualAmmount = Number(totalItemPrice);

    const updatedObject = {
      index: selectedData?.index,
      storeId: data.store.storeId,
      SaleChannelConfigId: data.storeChannel?.id || 0,
      orderTypeId: 2,
      orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
      description: description.join(","),
      remarks: remarks,
      amount: amount,
      actualAmmount: actualAmmount,
      cShippingCharges: shipping,
      paymentStatusId:
        EnumPaymentMethod.Prepaid == selectedPMOption
          ? EnumPaymentStatus.Paid
          : EnumPaymentStatus.Unpaid,
      weight: data?.weight,
      itemValue: itemValue,
      orderRequestVia: 1,
      paymentMethodId: selectedPMOption,
      stationId: selectedData.stationId,
      discount: discount,
      vat: 0,
      refNo: data?.refNo,
      orderNote: {
        note: note,
      },
      orderAddress: {
        customerName: data.customerName,
        email: data.email,
        mobile1: UtilityClass.getFormatedNumber(data.mobile1),
        mobile2: UtilityClass.getFormatedNumber(data.mobile2),
        country: selectedAddressSchema.country,
        city: selectedAddressSchema.city,
        area: selectedAddressSchema.area,
        province: selectedAddressSchema.province,
        pinCode: selectedAddressSchema.pinCode,
        state: selectedAddressSchema.state,
        streetAddress: selectedAddressSchema.streetAddress,
        streetAddress2: selectedAddressSchema.streetAddress2,
        houseNo: selectedAddressSchema.houseNo,
        buildingName: selectedAddressSchema.buildingName,
        landmark: selectedAddressSchema.landmark,
        zip: selectedAddressSchema.zip,
        latitude: getValues("lat&long").split(",")[0] || null,
        longitude: getValues("lat&long").split(",")[1] || null,
      },
      orderTaxes: orderData?.orderTax
        ?.map((tax) => {
          return {
            orderTaxId: tax.orderTaxId,
            ClientTaxId: tax.clientTaxId,
            taxValue: Number(vatValue[tax.clientTaxId]),
          };
        })
        .filter((dt) => dt !== undefined),
      orderBoxs: addedOrderBoxes.map(
        ({ clientOrderBoxId, orderBoxId, height, length, volume, width }) => ({
          clientOrderBoxId,
          orderBoxId: orderBoxId || "",
          height,
          length,
          volume,
          width,
        }),
      ),
      orderItems: orderItems,
    };
    replaceObjectAtIndex(updatedObject);
    setErrorsList((prev) => prev.filter((x) => x.Row != selectedRowIndex));
    handleClose();
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

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    // console.log("res", res.data.result);
    if (res.data.result != null) setAllCountries(res.data.result);
  };
  let getAllRegionbyCountryId = async () => {
    let res = await GetAllRegionbyCountryId(getValues("country").countryId);
    if (res.data.result != null) setAllRegions(res.data.result);
  };
  let getCityByRegionId = async () => {
    if (getValues("region").regionId) {
      let res = await GetCityByRegionId(getValues("region").regionId);
      if (res.data.result != null) {
        const _cities = res.data.result;
        setAllCities(res.data.result);
        let defCity = selectedData.orderAddress.cityName;
        setValue(
          "city",
          getSelectedValueFromStringOrObject(
            _cities,
            EnumOptions.SElECTED_CITY.LABEL,
            defCity,
          ),
        );
      }
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
    setIsFirstLoad(false);
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
      setTotalItemPrice(selectedData?.actualAmmount);
    }
    // setDiscount(_totaldiscount);
  }, [selectedProducts]);
  const handleOnClickProduct = async () => {
    await getProductStocksForSelection();
    //open modal
    setOpenProductModal(true);
  };
  useEffect(() => {
    setValue("store", selectedData?.storeId);
    getAllCountry();
    if (selectedData?.orderItems) {
      getProductStocksForSelection();
    }
  }, []);
  const handleDelete = async (value) => {
    value.checked = false;
    let product = selectedProducts.filter(
      (item) => item.ProductStockId !== value.ProductStockId,
    );
    setSelectedProducts(product);
  };

  let getProductStocksForSelection = async () => {
    let stationId =
      qtySelectedStation?.productStationId || selectedData.stationId;
    setIsProductFetching(true);

    let res = await GetProductStocksForSelection(
      stationId,
      getValues("store")?.storeId || selectedData?.storeId,
    );

    // console.log("data:::", res.data);
    if (res.data.result != null && res.data.result?.length > 0) {
      for (let index = 0; index < res.data?.result.length; index++) {
        //get selected object against station

        let obj = selectedData?.orderItems?.find(
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
        // res.data.result[index].QuantityAvailable;
      }

      /////////
      let preloadedProducts = [];
      selectedData?.orderItems.forEach((item, i) => {
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
          preloadedProducts.push(value);
        }
      });
      setSelectedProducts(preloadedProducts);
      ///////////
      setProductStocksForSelection(res.data.result);
    } else {
      errorNotification("No product found agains't selected store.");
    }

    setIsProductFetching(false);
  };
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
  const handleFocus = (event) => event.target.select();
  const paymentMethodoptions = [
    { id: 1, label: "Prepaid" },
    { id: 2, label: "Cash On Delivery" },
  ];

  const handleCheckboxChange = (optionId) => {
    setselectedPMOption(optionId);
  };
  useEffect(() => {
    if (
      getValues("store") !== null &&
      getValues("store") !== "" &&
      getValues("store") !== undefined
    ) {
      getChannelListByStoreIdForSelection();
    }
  }, [getValues("store")]);
  const getChannelListByStoreIdForSelection = async () => {
    setValue("storeChannel", null);

    if (getValues("store").storeId) {
      let res = await GetChannelListByStoreIdForSelection(
        getValues("store").storeId,
      );
      // console.log("getStoresForSelection", res.data);
      setStoresChannelForSelection(res.data.result || []);
    }
  };
  useEffect(() => {
    if (storesForSelection?.length > 0) {
      let defData = storesForSelection.find(
        (x) => x.storeId == selectedData?.storeId,
      );
      setValue("store", defData);
    }
  }, [storesForSelection]);
  useEffect(() => {
    if (selectedData) {
      setselectedPMOption(selectedData?.paymentMethodId);
      setValue("mobile1", selectedData?.orderAddress.mobile1);
      setValue("mobile2", selectedData?.orderAddress.mobile2);
      setNote(selectedData?.orderNote?.note);
      setRemarks(selectedData?.remarks);
    }
  }, []);
  useEffect(() => {
    if (productStations && productStations.length > 0) {
      let defData = productStations.find(
        (x) => x.productStationId == selectedData.stationId,
      );
      setQtySelectedStation(defData);
    }
  }, [productStations]);
  useEffect(() => {
    if (storesChannelForSelection?.length > 0) {
      let defData = storesChannelForSelection.find(
        (x) => x.id == selectedData?.saleChannelConfigId,
      );
      setValue("storeChannel", defData);
    }
  }, [storesChannelForSelection]);
  useMapAutocompleteSetter(
    autocomplete,
    allCountries,
    allRegions,
    allCities,
    setValue,
  );
  const { allClientTax, loading } = useGetAllClientTax([]);
  const getTaxData = (taxId) => {
    const _selectedTax = allClientTax.find((tax) => tax.ClientTaxId === taxId);
    return { title: _selectedTax?.Name, value: _selectedTax?.Percentage };
  };
  useEffect(() => {
    (async () => {
      setValue("mobile1", null);
      setValue("mobile2", null);
      await handleSetSchemaValueForUpdate(selectedData.orderAddress, setValue);
      setValue("mobile1", selectedData?.orderAddress.mobile1);
      setValue("mobile2", selectedData?.orderAddress.mobile2);
      let latitude = selectedData?.orderAddress.latitude;
      let longitude = selectedData?.orderAddress.longitude;
      if (latitude !== null && longitude !== null) {
        setValue("lat&long", `${latitude},${longitude}`);
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

  useEffect(() => {
    if (selectedData?.orderBoxs?.length > 0 && allClientOrderBox.length > 0) {
      const orderBoxIds = selectedData.orderBoxs.map(
        (box) => box.clientOrderBoxId,
      );
      const matchingBoxes = allClientOrderBox.filter((x) =>
        orderBoxIds.includes(x.clientOrderBoxId),
      );
      const updatedBoxes = matchingBoxes.map((box) => {
        const orderBox = selectedData.orderBoxs.find(
          (order) => order.clientOrderBoxId === box.clientOrderBoxId,
        );
        return {
          ...box,
          orderBoxId: orderBox ? orderBox.orderBoxId : null,
        };
      });
      setAddedOrderBoxes(updatedBoxes);
    }
  }, [allClientOrderBox, selectedData]);

  useEffect(() => {
    getAllClientOrderBox();
  }, []);

  const totalVolume = addedOrderBoxes.reduce(
    (sum, box) => sum + (box.volume || 0),
    0,
  );

  useEffect(() => {
    setValue("weight", parseFloat(totalVolume.toFixed(2)));
  }, [totalVolume]);

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="xl"
      title={
        LanguageReducer?.languageType?.PRODUCT_ALL +
        " " +
        LanguageReducer?.languageType?.PRODUCTS_TEXT
      }
      actionBtn={
        <ModalButtonComponent
          title={"Update Order"}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(updateData)}
    >
      <GridContainer>
        <GridItem xs={7}>
          <GridContainer>
            <GridItem xs={12}>
              <CustomColorLabelledOutline
                isCollapse={true}
                label={LanguageReducer?.languageType?.ORDERS_STORE}
              >
                <Grid display={"flex"} gap={1}>
                  <Grid md={6} lg={6} sm={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.ORDERS_STORE}
                    </InputLabel>
                    <SelectComponent
                      disabled
                      name="store"
                      control={control}
                      options={storesForSelection}
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
                  <Grid md={6} lg={6} sm={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      Sale Channel
                    </InputLabel>
                    <SelectComponent
                      name="storeChannel"
                      control={control}
                      options={storesChannelForSelection}
                      isRHF={true}
                      optionLabel={EnumOptions.STORE_CHANNEL.LABEL}
                      optionValue={EnumOptions.STORE_CHANNEL.VALUE}
                      disabled={
                        getValues("store")
                          ? getValues("store").storeId === 0
                          : true
                      }
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
                    <Grid item sm={12} md={6} lg={6}>
                      <InputLabel required sx={styleSheet.inputLabel}>
                        {"Station"}
                      </InputLabel>
                      <SelectComponent
                        disabled
                        name="qtySelectedStation"
                        options={productStations}
                        defaulValue={qtySelectedStation}
                        value={qtySelectedStation}
                        isRHF={true}
                        required={true}
                        optionLabel={EnumOptions.SELECT_STATION.LABEL}
                        optionValue={EnumOptions.SELECT_STATION.VALUE}
                        isRefesh={true}
                        handleRefreshClick={getAllStationLookup}
                      />
                    </Grid>
                  ) : null}
                  <Grid md={6} lg={6} sm={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      Order Date
                    </InputLabel>
                    <CustomRHFReactDatePickerInput
                      name="orderDate"
                      control={control}
                      defaultValue={
                        new Date(selectedData?.orderDate) || new Date()
                      }
                      required
                      error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
                      // defaultValue={preloadedValues?.orderDate}
                    />
                  </Grid>
                </Grid>
                <Grid item md={12} sm={12}>
                  <InputLabel sx={styleSheet.inputLabel}>Ref No.</InputLabel>
                  <TextField
                    type="text"
                    placeholder={placeholders.refNo}
                    defaultValue={selectedData?.refNo || ""}
                    onFocus={handleFocus}
                    size="small"
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
                  isCollapse={true}
                  label={LanguageReducer?.languageType?.PRODUCTS_TEXT}
                >
                  <Box textAlign={"right"}>
                    <ActionButtonCustom
                      loading={isProductFetching}
                      startIcon={<AddBoxIcon fontSize="small" />}
                      label={"Update Products"}
                      onClick={() => handleOnClickProduct()}
                      // sx={styleSheet.placeOrderButton}
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
                        <TableHead sx={{ ...styleSheet.orderProductHeading }}>
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
                                  <TableCell align="center" sx={{ p: 0.5 }}>
                                    <b>{value.QuantityAvailable}</b>
                                  </TableCell>

                                  <TableCell sx={{ p: 0.5 }} align="center">
                                    {" "}
                                    <Box className={"flex_center"}>
                                      <Box sx={styleSheet.qualityIncrementBox}>
                                        <IndeterminateCheckBoxOutlinedIcon
                                          onClick={(e) =>
                                            handleOnClickDecrement(e, index)
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
                                            discount: eval(e.target.value) || 0,
                                            newPrice:
                                              newSelectedProducts[index].Price *
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
                                      value={selectedProducts[index].newPrice}
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
                                  <TableCell align="right" sx={{ p: 0.5 }}>
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
                          <TableRow sx={{ ...styleSheet.orderProductHeading }}>
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
                              <TableCell sx={{ p: 1 }}>{box.volume}</TableCell>
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
              <CustomColorLabelledOutline label={"Customer"} isCollapse={true}>
                <Grid container spacing={2}>
                  <Grid item md={6} sm={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {"Customer Name"}
                    </InputLabel>
                    <TextField
                      type="text"
                      placeholder={placeholders.name}
                      onFocus={handleFocus}
                      size="small"
                      defaultValue={selectedData?.orderAddress?.customerName}
                      fullWidth
                      variant="outlined"
                      id="customerName"
                      name="customerName"
                      {...register("customerName", {
                        required: {
                          value: true,
                          message:
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
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
                      defaultValue={selectedData?.orderAddress.email}
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
                            LanguageReducer?.languageType?.INVALID_EMAIL_TOAST,
                        },
                      })}
                      error={Boolean(errors.email)} // set error prop
                      helperText={errors.email?.message}
                    />
                  </Grid>
                  <Grid item md={6} sm={6} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.PHONE_NUMBER_TEXT}
                    </InputLabel>
                    <CustomRHFPhoneInput
                      error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
                      // defaultValue={selectedData?.orderAddress?.mobile1}
                      name="mobile1"
                      selectedCountry={getValues("country")?.mapCountryCode}
                      control={control}
                      required
                    />
                  </Grid>
                  <Grid item md={6} sm={6} xs={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.CONTACT_NUMBER_TEXT}
                    </InputLabel>
                    <CustomRHFPhoneInput
                      error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
                      defaultValue={selectedData?.orderAddress?.mobile2}
                      name="mobile2"
                      selectedCountry={getValues("country")?.mapCountryCode}
                      control={control}
                    />
                  </Grid>
                  <Grid item md={4} sm={4} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.SELECT_COUNTRY_TEXT}
                    </InputLabel>
                    <SelectComponent
                      name="country"
                      control={control}
                      options={allCountries}
                      getOptionLabel={(option) => option?.name}
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
                        handleSetSchema(
                          "country",
                          resolvedId,
                          setValue,
                          unregister,
                        );
                      }}
                      errors={errors}
                    />
                  </Grid>
                  {[...addressSchemaSelectData, ...addressSchemaInputData].map(
                    (input, index, arr) => (
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
                    ),
                  )}
                  <GridContainer>
                    <Grid item md={6} sm={6} xs={12}>
                      <InputLabel sx={styleSheet.inputLabel}>
                        latitude And longitude
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
                        label="Get Address"
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
              <CustomColorLabelledOutline label={"Payment"} isCollapse={true}>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                  <PaymentAmountBox
                    title={"Sub Total"}
                    value={decimalFormat(totalItemPrice)}
                    onChange={(e) => {
                      const _totalItemPrice = Number(e.target.value);
                      setTotalItemPrice(_totalItemPrice);
                    }}
                  />
                  <PaymentAmountBox
                    title={"Add Discount"}
                    value={discount}
                    onChange={(e) => {
                      setDiscount(Number(e.target.value));
                    }}
                  />
                  <PaymentAmountBox
                    title={"Add Shipping"}
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
                  <PaymentTotalBox value={getGrandTotal()} />
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
              </CustomColorLabelledOutline>
            </GridItem>
            <GridItem xs={12}>
              {" "}
              <CustomColorLabelledOutline label={"Note"} isCollapse={true}>
                <center>
                  <Typography
                    sx={{ fontSize: "14px !important", width: "40ch" }}
                  >
                    {finalNote ? finalNote : ""}
                  </Typography>
                </center>
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
              <CustomColorLabelledOutline label={"Remarks"} isCollapse={true}>
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
      {openProductModal && (
        <AddProductModal
          open={openProductModal}
          setOpen={setOpenProductModal}
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

      <GoogleMapWithSearch
        open={openLocationModal}
        setOpen={setOpenLocationModal}
        setValue={setValue}
        setAutocomplete={setAutocomplete}
        splitLatAndLong={splitLatAndLong}
      />
    </ModalComponent>
  );
}
export default EditFullfilableOrderDetailsModal;
