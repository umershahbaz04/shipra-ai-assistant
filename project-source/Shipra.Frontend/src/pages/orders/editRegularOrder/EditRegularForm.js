import DeleteIcon from "@mui/icons-material/Delete";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import { LoadingButton } from "@mui/lab";
import {
  Box,
  Button,
  Checkbox,
  CircularProgress,
  Divider,
  FormControlLabel,
  Grid,
  InputLabel,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateOrder,
  GetAddressFromLatAndLong,
  GetAllActiveCarrierForCreateOrderSelection,
  GetAllCarrierWithServiceAndLocation,
  GetAllClientOrderBox,
  GetAllCountry,
  GetAllStationLookup,
  GetChannelListByStoreIdForSelection,
  GetStoresForSelection,
  GetValidateClientPPActivate,
  UpdateOrder,
  UpdateOrderWithInvoice,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import AddOrderBoxModal from "../../../components/modals/orderModals/AddOrderBoxModal";
import GoogleMapWithSearch from "../../../components/modals/resuseAbleModals/GoogleMapWithSearch";
import {
  EnumChangeFilterModelApiUrls,
  EnumNavigateState,
  EnumOptions,
  EnumPaymentMethod,
  EnumPaymentStatus,
} from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  CrossIconButton,
  CustomColorLabelledOutline,
  GridContainer,
  GridItem,
  PaymentAmountBox,
  PaymentMethodBox,
  PaymentMethodCheckbox,
  PaymentTaxBox,
  PaymentTotalBox,
  calculateSubtotalValue,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  handleCopyToClipBoard,
  placeholders,
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
import useCurrentCountryLocation from "../../../utilities/helpers/currentCountryLocation";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { calculateVatValue } from "../createRegularOrder";
import UtilityClass from "../../../utilities/UtilityClass";
import { getThisKeyCookie } from "../../../utilities/cookies";

const EnumOrderPlaceButton = Object.freeze({
  Confirm: 1,
  ConfirmAndNew: 2,
  ConfirmAndHandleInvoice: 2,
});

function EditRegularForm(props) {
  const { preloadedValues, orderData } = props;
  const navigate = useNavigate();
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allCountries, setAllCountries] = useState([]);
  const [discount, setDiscount] = useState(0);
  const [shipping, setShipping] = useState(0);
  const [vatValue, setVatValue] = useState([]);
  const [subtotal, setSubtotal] = useState(0);
  const [note, setNote] = useState("");
  const [productStations, setProductStations] = useState([]);
  const [selectedPMOption, setselectedPMOption] = useState(
    EnumPaymentMethod.Prepaid,
  );
  const [selectedPSOption, setSelectedPSOption] = useState(
    EnumPaymentStatus.Unpaid,
  );
  const [storesChannelForSelection, setStoresChannelForSelection] = useState(
    [],
  );
  const [isStripeSettingExist, setStripeSettingExist] = useState(false);
  const [isLoadingForConfirm, setIsLoadingForConfirm] = useState(false);
  const [
    isLoadingForConfirmAndSendInvoiceOrder,
    setIsLoadingForConfirmAndSendInvoiceOrder,
  ] = useState(false);
  const [paymentLinks, setPaymentLinks] = useState({
    show: false,
    paymentUrl: "",
    invoicePdf: "",
    showCrossIcon: true,
  });
  const [selectedOrderPlaceButtom, setSelectedOrderPlaceButtom] =
    useState(false);

  const [openUpdateOrderBoxModal, setopenUpdateOrderBoxModal] = useState({
    open: false,
    loading: {},
  });
  const [metafields, setMetafields] = useState([]);
  const [allClientOrderBox, setAllClientOrderBox] = useState([]);
  const [selectedOrderBox, setSelectedOrderBox] = useState([]);
  const [addedOrderBoxes, setAddedOrderBoxes] = useState([]);
  const [allCarriersForSelection, setAllCarriersForSelection] = useState([]);
  const [weightCount, setweightCount] = useState(0);
  const [carrierId, setCarrierId] = useState();
  const [dublicateOrderLoading, setDublicateOrderLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
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
  const hasProcessedPrices = useRef(false);

  const [autocomplete, setAutocomplete] = useState(null);
  const { allClientTax, loading } = useGetAllClientTax([]);

  const cookieData = getThisKeyCookie("genericSetting");
  const parsedData = cookieData ? JSON.parse(cookieData) : null;
  const { allGenericSetting } = useGetAllGenericSetting([], !parsedData);
  const finalGenericSetting = parsedData || allGenericSetting;
  const genericSettingFlag =
    finalGenericSetting?.[0]?.inputData?.[2]?.value === "true";

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
  const watchedPrices = useWatch({ control, name: "priceOfPieces" });

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    setValue("store", null);
    setStoresForSelection(res.data.result);
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
  let getAllStationLookup = async () => {
    let res = await GetAllStationLookup();
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
    if (getValues("store")) {
      getChannelListByStoreIdForSelection();
    }
  }, [getValues("store")]);

  useEffect(() => {
    if (orderData) {
      let order = orderData?.order;
      setSubtotal(
        calculateSubtotalValue(
          order?.discount,
          order?.cShippingCharges,
          orderData?.orderTax,
          order.amount,
        ),
      );
      setDiscount(order?.discount);

      if (preloadedValues) {
        if (order?.paymentMethodId) {
          handleCheckboxChange(order?.paymentMethodId);
        }
        if (order?.paymentStatusId) {
          handlePaymentStatusCheckboxChange(order?.paymentStatusId);
        }
      }
      setNote(orderData?.orderNote?.note);
      setShipping(order?.cShippingCharges);

      if (orderData?.orderItems) {
        orderData?.orderItems.forEach((item, i) => {
          setValue(`descriptionOfPieces.${i}`, item.description);
          setValue(`priceOfPieces.${i}`, item.price);
          setValue(`hsCode.${i}`, item.hsCode);
          setValue(`originCountryCode.${i}`, item.originCountryCode);
          setValue(`unitRate.${i}`, item.unitRate);
          setValue(`itemWeight.${i}`, item.weight);
        });
      }
    }
  }, [orderData]);

  useEffect(() => {
    getAllStationLookup();
    getValidateClientPPActivate();
  }, []);

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

  useEffect(() => {
    if (storesForSelection && storesForSelection.length > 0) {
      // Set the default value once options are available.
      setValue("store", storesForSelection[0]);
    }
  }, [storesForSelection]);
  useEffect(() => {
    getAllCountry();
  }, []);

  useEffect(() => {
    if (storesChannelForSelection?.length > 0) {
      let defData = storesChannelForSelection.find(
        (x) => x.id == preloadedValues?.storeChannel,
      );
      setValue("storeChannel", defData);
    }
  }, [storesChannelForSelection]);
  useEffect(() => {
    if (allCarriersForSelection?.length > 0) {
      if (preloadedValues?.CarrierIdFromAssignToCarrierModal !== null) {
        let defData = allCarriersForSelection.find(
          (x) =>
            x.CarrierId == preloadedValues?.CarrierIdFromAssignToCarrierModal,
        );
        setCarrierId(defData);
      } else {
        let defData = allCarriersForSelection.find(
          (x) => x.CarrierId == preloadedValues?.selectedCarrierId,
        );
        setCarrierId(defData);
      }
    }
  }, [allCarriersForSelection]);
  useEffect(() => {
    if (productStations?.length > 0) {
      let defData = productStations.find(
        (x) => x.productStationId == preloadedValues?.station,
      );
      setValue("station", defData);
    }
  }, [productStations]);

  const prepareCreateOrderData = (data) => {
    const amount =
      Number(subtotal) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);

    let orderItems = [];
    for (let index = 0; index < data.numberOfPieces; index++) {
      orderItems[index] = {
        price: data.priceOfPieces.length > 0 ? data.priceOfPieces[index] : "",
        description:
          data.descriptionOfPieces.length > 0
            ? data.descriptionOfPieces[index]
            : "",
        ...(genericSettingFlag && {
          HsCode: data?.hsCode?.length > 0 ? data?.hsCode[index] : "",
          OriginCountryCode:
            data?.originCountryCode?.length > 0
              ? data?.originCountryCode[index]
              : "",
          UnitRate: data?.unitRate?.length > 0 ? data?.unitRate[index] : "",
          Weight: data?.itemWeight?.length > 0 ? data?.itemWeight[index] : "",
        }),
        remarks: "",
        quantity: 1,
        discount: 0,
      };
      // }
    }

    const param1 = {
      orderId: orderData?.order?.orderId,
      SaleChannelConfigId: data.storeChannel?.id,
      storeId: data.store.storeId,
      orderTypeId: 1,
      orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
      description: data.description,
      remarks: data.remarks,
      amount: amount,
      cShippingCharges: shipping,
      paymentStatusId: selectedPSOption,
      weight: data.weight,
      itemValue: subtotal,
      orderRequestVia: 1,
      paymentMethodId: selectedPMOption,
      stationId: data.station?.productStationId,
      discount: discount,
      vat: 0,
      refNo: data?.refNo,
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
        addressTypeId: 0,
        latitude: getValues("lat&long").split(",")[0] || null,
        longitude: getValues("lat&long").split(",")[1] || null,
        orderAddressId: orderData.orderAddress.orderAddressId,
        customerName: data.customerName,
        email: data.email,
        mobile1: data.mobile1,
        mobile2: data.mobile2,
      },
      settingConfig: metafields,
      orderTaxes: orderData?.orderTax
        .map((tax) => {
          return {
            orderTaxId: tax.orderTaxId,
            clientTaxId: tax.clientTaxId,
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
    // console.log("params", param1);

    return param1;
  };
  const handleConfirmOrder = async (data) => {
    const param1 = prepareCreateOrderData(data);
    // console.log("params", param1);
    if (addedOrderBoxes.length == 0) {
      errorNotification("Please Choose Order Box");
      return;
    }
    setIsLoadingForConfirm(true);
    setSelectedOrderPlaceButtom(EnumOrderPlaceButton.Confirm);
    UpdateOrder(param1)
      .then((res) => {
        // console.log("res:::", res);
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
        } else {
          successNotification("Order Update successfully");
          navigate("/orders-dashboard");
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable create order");
      })
      .finally(() => {
        setIsLoadingForConfirm(false);
        setSelectedOrderPlaceButtom(null);
      });
  };

  const handleDublicateOrder = async (data) => {
    const param1 = prepareCreateOrderData(data);
    if (addedOrderBoxes.length == 0) {
      errorNotification("Please Choose Order Box");
      return;
    }
    setDublicateOrderLoading(true);
    const { orderId, ...orderListWithoutId } = param1;
    try {
      const response = await CreateOrder({
        orderList: [orderListWithoutId],
        IsSaleChannelOrder: false,
      });
      if (response?.data?.isSuccess) {
        successNotification("Order Create Successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {
    } finally {
      setDublicateOrderLoading(false);
    }
  };

  const handleSendInvoice = async (data) => {
    const param1 = prepareCreateOrderData(data);
    // console.log("params", param1);
    setIsLoadingForConfirmAndSendInvoiceOrder(true);
    setSelectedOrderPlaceButtom(EnumOrderPlaceButton.ConfirmAndHandleInvoice);
    UpdateOrderWithInvoice(param1)
      .then((res) => {
        // console.log("res:::", res);
        if (!res?.data?.isSuccess) {
          errorNotification("Unable to update order");
          errorNotification(res?.data?.customErrorMessage);
        } else {
          const data = res.data.result;
          setPaymentLinks((prev) => ({
            ...prev,
            show: true,
            paymentUrl: data?.hostedInvoiceUrl,
            invoicePdf: data?.invoicePdf,
          }));
          successNotification("Order updated successfully");
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
  };

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

  useEffect(() => {
    if (preloadedValues) {
      getStoresForSelection();
    }
  }, []);
  useEffect(() => {
    if (storesForSelection?.length > 0) {
      let defData = storesForSelection.find(
        (x) => x.storeId == preloadedValues?.storeId,
      );
      setValue("store", defData);
    }
  }, [storesForSelection]);
  const handleFocus = (event) => event.target.select();

  const { navigateStateData } = useGetNavigateState(
    EnumNavigateState.EDIT_ORDER.pageName,
    false,
  );
  const disabledInput = navigateStateData
    ? navigateStateData.disabledInput
    : false;

  useSetNumericInputEffect([]);
  useMapAutocompleteSetter(autocomplete, allCountries, [], [], setValue);

  useEffect(() => {
    (async () => {
      setValue("mobile1", null);
      setValue("mobile2", null);
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

  useEffect(() => {
    if (preloadedValues?.OrderDate) {
      const parsedDate = new Date(preloadedValues.OrderDate);
      if (!isNaN(parsedDate)) {
        setValue("orderDate", parsedDate);
      }
    }
  }, [preloadedValues]);

  useEffect(() => {
    if (!watchedPrices || !Array.isArray(watchedPrices)) return;

    if (!hasProcessedPrices.current) {
      hasProcessedPrices.current = true;
      return;
    }
    const total = watchedPrices.reduce((acc, curr) => {
      const price = parseFloat(curr);
      return acc + (isNaN(price) ? 0 : price);
    }, 0);
    setSubtotal(total);
  }, [watchedPrices]);

  useEffect(() => {
    if (preloadedValues?.metafields) {
      setMetafields(preloadedValues.metafields);
    }
  }, [preloadedValues]);

  const handleChange = (value, inputIndex) => {
    const updatedMetafields = [...metafields];
    updatedMetafields[inputIndex].value = value;
    setMetafields(updatedMetafields);
  };

  return (
    <Box sx={styleSheet.pageRoot}>
      <form>
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
                  {" "}
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_STORE}
                  >
                    <Grid container spacing={2}>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_STORE}
                        </InputLabel>
                        <SelectComponent
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
                          disabled={disabledInput}
                          value={getValues("store")}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("store", resolvedId);
                          }}
                          errors={errors}
                        />
                      </Grid>
                      <Grid item md={6} lg={6} sm={6} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDER_SALE_CHANNEL}
                        </InputLabel>
                        <SelectComponent
                          name="storeChannel"
                          control={control}
                          options={storesChannelForSelection}
                          optionLabel={EnumOptions.STORE_CHANNEL.LABEL}
                          optionValue={EnumOptions.STORE_CHANNEL.VALUE}
                          disabled={!getValues("store") || disabledInput}
                          value={getValues("storeChannel")}
                          {...register("storeChannel", {
                            // required: {
                            //   value: true,
                            // },
                          })}
                          onChange={(event, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            setValue("storeChannel", resolvedId);
                          }}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_STATIONS}
                        </InputLabel>
                        <SelectComponent
                          disabled={disabledInput}
                          name="station"
                          control={control}
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
                          }}
                          errors={errors}
                        />
                      </Grid>
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
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_ORDER_DATE}
                        </InputLabel>
                        <CustomRHFReactDatePickerInput
                          name="orderDate"
                          control={control}
                          disabled={disabledInput}
                          // onChange={handleOnChange}
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
                        disabled={disabledInput}
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
                  {" "}
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
                          variant="outlined"
                          id="customerName"
                          name="customerName"
                          disabled={disabledInput}
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
                      <Grid item md={12} sm={12} xs={12}>
                        <Typography sx={{ ...styleSheet.pageSubHeading }}>
                          {" "}
                          {LanguageReducer?.languageType?.ORDERS_ADD_SHIPPING}
                        </Typography>
                      </Grid>
                      <Grid item md={4} sm={4} xs={12}>
                        <InputLabel required sx={styleSheet.inputLabel}>
                          {LanguageReducer?.languageType?.ORDERS_COUNTRY}
                        </InputLabel>
                        <SelectComponent
                          name="country"
                          disabled={disabledInput}
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
                            value={getValues(input.key)}
                            viewMode={disabledInput}
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
                                        input.value ??
                                        input.defaultValue ??
                                        false
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
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDER_DETAILS_TEXT}
                  >
                    <Grid container spacing={2}>
                      <Grid item md={6} sm={6} xs={12}>
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
                          disabled={disabledInput}
                          {...register("description", {
                            // required: {
                            //   value: true,
                            //   message:
                            //     LanguageReducer?.languageType
                            //       ?.FIELD_REQUIRED_TEXT,
                            // },
                            pattern: {
                              value: /^(?!\s*$).+/,
                              message:
                                LanguageReducer?.languageType
                                  ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                            },
                          })}
                          error={Boolean(errors.description)} // set error prop
                          helperText={errors.description?.message}
                        />
                      </Grid>
                      <Grid item md={6} sm={6} xs={12}>
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
                          disabled={disabledInput}
                          {...register("remarks", {
                            // required: {
                            //   value: true,
                            //   message:
                            //     LanguageReducer?.languageType
                            //       ?.FIELD_REQUIRED_TEXT,
                            // },
                            pattern: {
                              value: /^(?!\s*$).+/,
                              message:
                                LanguageReducer?.languageType
                                  ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                            },
                          })}
                          error={Boolean(errors.remarks)} // set error prop
                          helperText={errors.remarks?.message}
                        />
                      </Grid>
                      <Grid item md={12} sm={12} xs={12}>
                        <InputLabel sx={styleSheet.inputLabel}>
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
                          disabled={disabledInput}
                          variant="outlined"
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
                      {Array.from(
                        { length: getValues("numberOfPieces") },
                        (_, index) => index,
                      ).map((i) => (
                        <React.Fragment key={i}>
                          <Grid
                            item
                            md={genericSettingFlag ? 2 : 6}
                            sm={2}
                            xs={2}
                          >
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
                                //   message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                                // },
                              })}
                              error={Boolean(errors.descriptionOfPieces?.[i])}
                              helperText={
                                errors.descriptionOfPieces?.[i]?.message
                              }
                            />
                          </Grid>

                          <Grid
                            item
                            md={genericSettingFlag ? 2 : 6}
                            sm={2}
                            xs={2}
                          >
                            <InputLabel sx={styleSheet.inputLabel}>
                              {
                                LanguageReducer?.languageType
                                  ?.ORDERS_PRICE_OF_PIECE_1
                              }{" "}
                              {i + 1}
                            </InputLabel>
                            <TextField
                              placeholder={`${
                                LanguageReducer?.languageType
                                  ?.ORDERS_PRICE_OF_PIECE_1
                              } ${i + 1}`}
                              type="number"
                              size="small"
                              onFocus={handleFocus}
                              id={`priceOfPieces.${i}`}
                              name={`priceOfPieces.${i}`}
                              fullWidth
                              variant="outlined"
                              {...register(`priceOfPieces.${i}`, {
                                // required: {
                                //   value: true,
                                //   message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                                // },
                              })}
                              error={Boolean(errors.priceOfPieces?.[i])}
                              helperText={errors.priceOfPieces?.[i]?.message}
                            />
                          </Grid>
                          {genericSettingFlag && (
                            <>
                              <Grid item md={2} sm={2} xs={2} required>
                                <InputLabel sx={styleSheet.inputLabel} required>
                                  {"HsCode "} {i + 1}
                                </InputLabel>
                                <TextField
                                  placeholder={`${"HsCode"} ${i + 1}`}
                                  type="text"
                                  size="small"
                                  onFocus={handleFocus}
                                  id={`hsCode.${i}`}
                                  name={`hsCode.${i}`}
                                  fullWidth
                                  variant="outlined"
                                  {...register(`hsCode.${i}`, {
                                    required: {
                                      value: true,
                                      message:
                                        LanguageReducer?.languageType
                                          ?.FIELD_REQUIRED_TEXT,
                                    },
                                  })}
                                  error={Boolean(errors.hsCode?.[i])}
                                  helperText={errors.hsCode?.[i]?.message}
                                />
                              </Grid>
                              <Grid item md={2} sm={2} xs={2}>
                                <InputLabel sx={styleSheet.inputLabel} required>
                                  {"Origin Country Code"} {i + 1}
                                </InputLabel>
                                <TextField
                                  placeholder={`${"originCountryCode"} ${i + 1}`}
                                  type="text"
                                  size="small"
                                  onFocus={handleFocus}
                                  id={`originCountryCode.${i}`}
                                  name={`originCountryCode.${i}`}
                                  fullWidth
                                  variant="outlined"
                                  {...register(`originCountryCode.${i}`, {
                                    required: {
                                      value: true,
                                      message:
                                        LanguageReducer?.languageType
                                          ?.FIELD_REQUIRED_TEXT,
                                    },
                                  })}
                                  error={Boolean(errors.originCountryCode?.[i])}
                                  helperText={
                                    errors.originCountryCode?.[i]?.message
                                  }
                                />
                              </Grid>
                              <Grid item md={2} sm={2} xs={2}>
                                <InputLabel sx={styleSheet.inputLabel} required>
                                  {"Unit Rate"} {i + 1}
                                </InputLabel>
                                <TextField
                                  placeholder={`${"unitRate"} ${i + 1}`}
                                  type="number"
                                  size="small"
                                  onFocus={handleFocus}
                                  id={`unitRate.${i}`}
                                  name={`unitRate.${i}`}
                                  fullWidth
                                  variant="outlined"
                                  {...register(`unitRate.${i}`, {
                                    required: {
                                      value: true,
                                      message:
                                        LanguageReducer?.languageType
                                          ?.FIELD_REQUIRED_TEXT,
                                    },
                                  })}
                                  error={Boolean(errors.unitRate?.[i])}
                                  helperText={errors.unitRate?.[i]?.message}
                                />
                              </Grid>
                              <Grid item md={2} sm={2} xs={2}>
                                <InputLabel sx={styleSheet.inputLabel} required>
                                  {"Weight"} {i + 1}
                                </InputLabel>
                                <TextField
                                  placeholder={`${"weight"} ${i + 1}`}
                                  type="number"
                                  size="small"
                                  onFocus={handleFocus}
                                  id={`itemWeight.${i}`}
                                  name={`itemWeight.${i}`}
                                  fullWidth
                                  variant="outlined"
                                  {...register(`itemWeight.${i}`, {
                                    required: {
                                      value: true,
                                      message:
                                        LanguageReducer?.languageType
                                          ?.FIELD_REQUIRED_TEXT,
                                    },
                                  })}
                                  error={Boolean(errors.itemWeight?.[i])}
                                  helperText={errors.itemWeight?.[i]?.message}
                                />
                              </Grid>
                            </>
                          )}
                        </React.Fragment>
                      ))}
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
              </GridContainer>
            </GridItem>
            <GridItem md={5} sm={12} xs={12}>
              <GridContainer>
                <GridItem md={12} sm={12} xs={12}>
                  {" "}
                  <CustomColorLabelledOutline
                    label={"Payment"}
                    isCollapse={true}
                  >
                    <Box display={"flex"} flexDirection={"column"} gap={1}>
                      <PaymentAmountBox
                        title={LanguageReducer?.languageType?.ORDERS_SUBTOTAL}
                        value={subtotal}
                        disabled={disabledInput}
                        onChange={(e) => {
                          setSubtotal(e.target.value);
                        }}
                      />
                      <PaymentAmountBox
                        title={
                          LanguageReducer?.languageType?.ORDERS_ADD_DISCOUNT
                        }
                        value={discount}
                        disabled={disabledInput}
                        onChange={(e) => {
                          setDiscount(e.target.value);
                        }}
                      />
                      <PaymentAmountBox
                        title={
                          LanguageReducer?.languageType?.ORDERS_ADD_SHIPPING
                        }
                        value={shipping}
                        disabled={disabledInput}
                        onChange={(e) => {
                          setShipping(e.target.value);
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
                      sx={{ mt: "20px" }}
                      container
                      spacing={1}
                    >
                      <Grid item>
                        <ActionButtonCustom
                          onClick={handleSubmit(handleDublicateOrder)}
                          variant="contained"
                          loading={dublicateOrderLoading}
                          background={Colors.linkColor}
                          label={"Duplicate"}
                        />
                      </Grid>
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
                        selectedPMOption !== EnumPaymentMethod.Prepaid &&
                        (!orderData.order.stripeInvoiceHostURL ||
                          !orderData.order.stripeInvoicePDFURL) && (
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
              </GridContainer>
            </GridItem>
          </GridContainer>
        </div>
      </form>
      {/* {openLocationModal && ( */}
      <GoogleMapWithSearch
        open={openLocationModal}
        setOpen={setOpenLocationModal}
        setValue={setValue}
        // isEditMode={true}
        // handleMapValues={handleMapValues}
        setAutocomplete={setAutocomplete}
        splitLatAndLong={splitLatAndLong}
      />
      {/* //)} */}
      {/* <FromLinkLocationModal
        open={openFromLinkLocationModal}
        setOpen={setOpenFromLinkLocationModal}
        setValue={setValue}
      /> */}
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
    </Box>
  );
}
export default EditRegularForm;
