import AddBoxIcon from "@mui/icons-material/AddBox";
import DeleteIcon from "@mui/icons-material/Delete";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import { LoadingButton } from "@mui/lab";
import {
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
} from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
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
  GetStoresForSelection,
  GetValidateClientPPActivate,
  UpdateOrder,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import AddOrderBoxModal from "../../../components/modals/orderModals/AddOrderBoxModal";
import GoogleMapWithSearch from "../../../components/modals/resuseAbleModals/GoogleMapWithSearch";
import { handleDispatchUserProfile } from "../../../components/topNavBar";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  EnumMetaField,
  EnumOptions,
  EnumPaymentMethod,
  EnumPaymentStatus,
  EnumRoutesUrls,
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
  PaymentTaxAlert,
  PaymentTaxBox,
  PaymentTotalBox,
  calculateSubtotalValue,
  camelizeArray,
  fetchMethod,
  fetchMethodResponse,
  getDefaultValueIndex,
  getLowerCase,
  handleCopyToClipBoard,
  handleDispathUpdateBadge,
  placeholders,
  useGetAllClientTax,
  useMapAutocompleteSetter,
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
import { useGetAllMetafields } from "../../../utilities/helpers/HelpersFilter";
import { Typography } from "@material-ui/core";
import { handleGetDraftOrdersCount } from "../../../components/authRoutes";
import { isShowCountryInTabbarFlag } from "../../../utilities/cookies";
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
export const calculateVatValue = (data) => {
  let value = 0;
  Object.values(data).forEach((val) => {
    value += Number(val);
  });
  return isNaN(value) ? 0 : value;
};
function DraftRegularOrder(props) {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { state: orderData } = useLocation();
  let data = orderData?.result?.orderInfo;
  const order = data !== undefined ? JSON.parse(data) : data;
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allCountries, setAllCountries] = useState([]);
  const [discount, setDiscount] = useState(0);
  const [shipping, setShipping] = useState(0);
  const [subtotal, setSubtotal] = useState(0);
  const [vatValue, setVatValue] = useState({});
  const [note, setNote] = useState("");
  const [productStations, setProductStations] = useState([]);
  const [selectedPMOption, setselectedPMOption] = useState(1);
  const [formMode, setFormMode] = useState(EnumDraftOrder.draft);
  const [dublicateOrderLoading, setdublicateOrderLoading] = useState(false);
  const [openOrderBoxModal, setopenOrderBoxModal] = useState({
    open: false,
    loading: {},
  });
  const [allClientOrderBox, setAllClientOrderBox] = useState([]);
  const [selectedOrderBox, setSelectedOrderBox] = useState([]);
  const [addedOrderBoxes, setAddedOrderBoxes] = useState([]);
  const [storesChannelForSelection, setStoresChannelForSelection] = useState(
    [],
  );
  const [paymentLinks, setPaymentLinks] = useState({
    show: false,
    paymentUrl: "",
    invoicePdf: "",
  });
  const [isStripeSettingExist, setStripeSettingExist] = useState(false);
  const [isLoadingForConfirm, setIsLoadingForConfirm] = useState(false);
  const [isLoadingForConfirmAndNewOrder, setIsLoadingForConfirmAndNewOrder] =
    useState(false);
  const [
    isLoadingForConfirmAndSendInvoiceOrder,
    setIsLoadingForConfirmAndSendInvoiceOrder,
  ] = useState(false);
  const [selectedOrderPlaceButtom, setSelectedOrderPlaceButtom] =
    useState(false);
  const [allCarriersForSelection, setAllCarriersForSelection] = useState([]);
  const [carrierId, setCarrierId] = useState();
  const initialState = {
    country: {
      countryId: 0,
      name: "Select Please",
    },

    region: {
      regionId: 0,
      name: "Select Please",
    },
    city: {
      cityId: 0,
      name: "Select Please",
    },
  };
  const { allClientTax, loading } = useGetAllClientTax([]);
  const {
    loading: metaLoading,
    metafields,
    setMetafields,
  } = useGetAllMetafields(EnumMetaField.Order);

  const [isMetafieldsMapped, setIsMetafieldsMapped] = useState(false);

  useEffect(() => {
    if (orderData && metafields?.length > 0 && !isMetafieldsMapped) {
      const draftStr = orderData.result?.orderInfo;
      if (draftStr) {
        const order = JSON.parse(draftStr);
        if (order.settingConfig && order.settingConfig.length > 0) {
          const updatedMetaFields = JSON.parse(JSON.stringify(metafields));
          if (updatedMetaFields[0]?.settingConfig) {
            let hasChanges = false;
            order.settingConfig.forEach((savedInput) => {
              const index = updatedMetaFields[0].settingConfig.findIndex(
                (i) => i.name === savedInput.name,
              );
              if (
                index !== -1 &&
                updatedMetaFields[0].settingConfig[index].value !==
                  savedInput.value
              ) {
                updatedMetaFields[0].settingConfig[index].value =
                  savedInput.value;
                hasChanges = true;
              }
            });
            if (hasChanges) {
              setMetafields(updatedMetaFields);
            }
          }
        }
      }
      setIsMetafieldsMapped(true);
    }
  }, [orderData, metafields, isMetafieldsMapped]);
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
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
  const [autocomplete, setAutocomplete] = useState(null);
  useWatch({
    name: "country",
    control,
  });
  useWatch({
    name: "lat&long",
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
  const watchedPrices = useWatch({ control, name: "priceOfPieces" });

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

  const [paymentDueLaterCheckBox, setPaymentDueLaterCheckBox] = useState(false);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const getStoresForSelection = async () => {
    let res = await GetStoresForSelection();
    const _stores = res.data.result;
    if (!orderData) {
      setValue(
        "store",
        _stores[getDefaultValueIndex(_stores, EnumOptions.STORE.VALUE)],
      );
    }
    setStoresForSelection(_stores);
  };
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
  let getAllStationLookup = async () => {
    let res = await GetAllStationLookup();
    if (res.data.result != null) {
      const stations = res.data.result;
      setProductStations(stations);
      const defaultStation = stations.find((station) =>
        station.sname.toLowerCase().includes("default"),
      );
      if (!orderData && defaultStation) {
        setValue("station", defaultStation);
      }
    }
  };
  let getValidateClientPPActivate = async () => {
    let res = await GetValidateClientPPActivate();
    // console.log("getValidateClientPPActivate", res.data);
    if (res.data.isSuccess) {
      setStripeSettingExist(res.data.result.isValidate);
    } else {
      setStripeSettingExist(false);
    }
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
    if (
      getValues("store") !== null &&
      getValues("store") !== "" &&
      getValues("store") !== undefined
    ) {
      getChannelListByStoreIdForSelection();
    }
  }, [getValues("store")]);
  useEffect(() => {
    getStoresForSelection();
    getAllStationLookup();
    getValidateClientPPActivate();
    getAllClientOrderBox();
  }, []);
  useEffect(() => {
    if (getValues("country") !== null && getValues("country") !== undefined) {
      getAllActiveCarrierForCreateOrderSelection();
    }
  }, [getValues("country")]);

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    if (res.data.result != null) setAllCountries(res.data.result);
  };
  useEffect(() => {
    getAllCountry();
  }, []);

  const createOrder = async (data) => {
    const param1 = prepareCreateOrderData(data);

    console.log("params", param1);
    CreateOrder(param1)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
        } else {
          successNotification(
            LanguageReducer?.languageType?.PRODUCT_CREATED_SUCCESSFULLY_TOAST,
          );
          navigate("/orders-dashboard");
        }
      })
      .catch((e) => {
        console.log("e", e);
        UtilityClass.showErrorNotificationWithDictionary(
          e?.response?.data?.errors,
        );
      });
  };

  const prepareCreateOrderData = (data) => {
    const amount =
      Number(subtotal) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);
    const itemValue = subtotal;
    let orderItems = [];
    for (let index = 0; index < data.numberOfPieces; index++) {
      orderItems[index] = {
        price: data.priceOfPieces.length > 0 ? data.priceOfPieces[index] : "",
        description:
          data.descriptionOfPieces.length > 0
            ? data.descriptionOfPieces[index]
            : "",
        remarks: "",
        quantity: 1,
        discount: 0,
      };
    }
    const param1 = {
      orderList: [
        {
          storeId: data.store.storeId,
          SaleChannelConfigId: data.storeChannel?.id,
          orderTypeId: 1,
          orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
          description: data.description,
          remarks: data.remarks,
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
            orderAddressId: 0,
            customerName: data.customerName,
            email: data.email,
            mobile1: data.mobile1,
            mobile2: data.mobile2,
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
      settingConfig: metafields[0]?.settingConfig,
      IsSaleChannelOrder: false,
      OrderDraftId: 0,
    };

    return param1;
  };

  const prepareDraftOrderData = (data) => {
    const amount =
      Number(subtotal) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);
    const itemValue = subtotal;
    let orderItems = [];
    for (let index = 0; index < data.numberOfPieces; index++) {
      orderItems[index] = {
        price: data.priceOfPieces.length > 0 ? data.priceOfPieces[index] : "",
        description:
          data.descriptionOfPieces.length > 0
            ? data.descriptionOfPieces[index]
            : "",
        remarks: "",
        quantity: 1,
        discount: 0,
      };
    }
    const param1 = {
      orderList: [
        {
          storeId: data.store?.storeId,
          SaleChannelConfigId: data?.storeChannel?.id,
          orderTypeId: 1,
          orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
          description: data?.description,
          remarks: data?.remarks,
          amount: amount,
          cShippingCharges: shipping,
          paymentStatusId:
            EnumPaymentMethod.Prepaid == selectedPMOption
              ? EnumPaymentStatus.Paid
              : EnumPaymentStatus.Unpaid,
          weight: data?.weight,
          itemValue: itemValue,
          orderRequestVia: 1,
          paymentMethodId: selectedPMOption,
          stationId: data?.station?.productStationId,
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
            latitude: getValues("lat&long")?.split(",")[0] || null,
            longitude: getValues("lat&long")?.split(",")[1] || null,
            orderAddressId: 0,
            customerName: data.customerName,
            email: data?.email,
            mobile1: data?.mobile1,
            mobile2: data?.mobile2,
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
      settingConfig: metafields[0]?.settingConfig,
      IsSaleChannelOrder: false,
      OrderDraftId:
        orderData?.result?.orderDraftId > 0
          ? orderData?.result?.orderDraftId
          : 1,
    };

    return param1;
  };
  const prepareDublicateOrderData = (data) => {
    const amount =
      Number(subtotal) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);
    const itemValue = subtotal;
    let orderItems = [];
    for (let index = 0; index < data.numberOfPieces; index++) {
      orderItems[index] = {
        price: data.priceOfPieces.length > 0 ? data.priceOfPieces[index] : "",
        description:
          data.descriptionOfPieces.length > 0
            ? data.descriptionOfPieces[index]
            : "",
        remarks: "",
        quantity: 1,
        discount: 0,
      };
    }
    const param1 = {
      orderList: [
        {
          storeId: data.store?.storeId,
          SaleChannelConfigId: data?.storeChannel?.id,
          orderTypeId: 1,
          orderDate: UtilityClass.getFormatedDateWithoutTime(data?.orderDate),
          description: data?.description,
          remarks: data?.remarks,
          amount: amount,
          cShippingCharges: shipping,
          paymentStatusId:
            EnumPaymentMethod.Prepaid == selectedPMOption
              ? EnumPaymentStatus.Paid
              : EnumPaymentStatus.Unpaid,
          weight: data?.weight,
          itemValue: itemValue,
          orderRequestVia: 1,
          paymentMethodId: selectedPMOption,
          stationId: data?.station?.productStationId,
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
            latitude: getValues("lat&long")?.split(",")[0] || null,
            longitude: getValues("lat&long")?.split(",")[1] || null,
            orderAddressId: 0,
            customerName: data.customerName,
            email: data?.email,
            mobile1: data?.mobile1,
            mobile2: data?.mobile2,
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
      settingConfig: metafields[0]?.settingConfig,
      IsSaleChannelOrder: false,
      OrderDraftId: 1,
    };

    return param1;
  };

  const handleCompleteOrder = async (data) => {
    const param1 = prepareCreateOrderData(data);
    if (addedOrderBoxes.length == 0) {
      errorNotification("Please Choose Order Box");
      return;
    }
    // console.log("params", param1);
    setIsLoadingForConfirm(true);
    setSelectedOrderPlaceButtom(EnumOrderPlaceButton.COMPLETE);
    CreateOrder(param1)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
        } else {
          successNotification("Order create successfully");
          navigate("/draft-order-dashboard");
          if (orderData?.result?.orderDraftId > 0) {
            DeleteOrderDraft(orderData?.result?.orderDraftId);
          }
        }
      })
      .catch((e) => {
        console.log("e", e);
        UtilityClass.showErrorNotificationWithDictionary(
          e?.response?.data?.errors,
        );
      })
      .finally(() => {
        setIsLoadingForConfirm(false);
        setSelectedOrderPlaceButtom(null);
      });
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
      .then((res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Unable to Draft order");
          errorNotification(res?.data?.customErrorMessage);
        } else {
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
        setdublicateOrderLoading(false);
        setSelectedOrderPlaceButtom(null);
      });
  };

  const handleDraftOrder = async (data) => {
    const param1 = prepareDraftOrderData(data);
    const newparam = prepareData(param1);
    setIsLoadingForConfirmAndNewOrder(true);
    setSelectedOrderPlaceButtom(EnumOrderPlaceButton.SAVE);
    if (orderData?.result?.orderDraftId > 0) {
      UpdateOrder(newparam)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
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
          // console.log("res:::", res);
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
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
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors,
          );
        })
        .finally(() => {
          setIsLoadingForConfirmAndNewOrder(false);
          setSelectedOrderPlaceButtom(null);
        });
    }
  };

  const handleSendInvoice = async (data) => {
    const param1 = prepareCreateOrderData(data);
    setIsLoadingForConfirmAndSendInvoiceOrder(true);
    setSelectedOrderPlaceButtom(EnumOrderPlaceButton.ConfirmAndHandleInvoice);
    CreateOrderWithInvoice(param1)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
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
  };

  const handleRestForm = () => {
    setNote("");
    setDiscount(0);
    setShipping(0);
    setSubtotal(0);
    setValue("numberOfPieces", "");

    reset();
    setValue("mobile1", UtilityClass.getDefaultCountryCode());
    setValue("mobile2", UtilityClass.getDefaultCountryCode());
  };
  const paymentMethodoptions = [
    { id: 1, label: LanguageReducer?.languageType?.ORDERS_PREPAID },
    { id: 2, label: LanguageReducer?.languageType?.ORDERS_CASH_ON_DELIVERY },
  ];
  const handleCheckboxChange = (optionId) => {
    setselectedPMOption(optionId);
  };
  const handleFocus = (event) => event.target.select();

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

  useSetNumericInputEffect([]);
  useMapAutocompleteSetter(autocomplete, allCountries, setValue);
  const totalVolume = addedOrderBoxes.reduce(
    (sum, box) => sum + (box.volume || 0),
    0,
  );

  const handleChange = (value, inputIndex, metaFieldIndex) => {
    const updatedMetaFields = [...metafields];
    updatedMetaFields[metaFieldIndex].settingConfig[inputIndex].value = value;
    setMetafields(updatedMetaFields);
  };

  useEffect(() => {
    setValue("weight", parseFloat(totalVolume.toFixed(2)));
  }, [totalVolume]);
  useEffect(() => {
    if (!orderData) {
      const defaultOrderBox = allClientOrderBox.filter(
        (item) => item.isDefault === true,
      );
      setAddedOrderBoxes(defaultOrderBox);
      setSelectedOrderBox(defaultOrderBox);
    }
  }, [allClientOrderBox]);

  useEffect(() => {
    if (productStations?.length > 0 && orderData) {
      let defData = productStations.find(
        (x) => x.productStationId == order?.StationId,
      );
      setValue("station", defData);
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
    if (!orderData) {
      if (watchedPrices && Array.isArray(watchedPrices)) {
        const total = watchedPrices.reduce((acc, curr) => {
          const price = parseFloat(curr);
          return acc + (isNaN(price) ? 0 : price);
        }, 0);
        setSubtotal(total);
      }
    }
  }, [watchedPrices]);

  useEffect(() => {
    if (!orderData) return;

    if (orderData) {
      let data = orderData?.result?.orderInfo;
      const order = JSON.parse(data);
      const camelValueOrderTax = camelizeArray(order?.OrderTaxes);
      setValue("numberOfPieces", order?.OrderItems?.length || 0);
      setValue("country", order?.OrderAddress?.CountryId);
      setSubtotal(
        calculateSubtotalValue(
          order?.Discount,
          order?.CShippingCharges,
          camelValueOrderTax,
          order.Amount,
        ),
      );

      setValue("description", order.Description);
      setValue("remarks", order.Remarks);
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

  console.log(orderData);

  return (
    <Box sx={styleSheet.pageRoot}>
      <form onSubmit={handleSubmit(createOrder)}>
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
                <span
                  style={{
                    color: "#fff",
                    fontSize: "14px",
                  }}
                >
                  {`Order No: ${orderData?.result?.orderNo}`}
                </span>
              </Box>
            )}
            <Box sx={{ display: "flex", gap: 1 }}>
              <ActionButtonCustom
                onClick={() => {
                  navigate("/draft-fullfillable-order");
                }}
                variant="contained"
                label={"Draft Fullfillable Order"}
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
                {/* store */}
                <GridItem xs={12}>
                  <CustomColorLabelledOutline
                    isCollapse={true}
                    label={LanguageReducer?.languageType?.ORDERS_STORE}
                  >
                    <Grid container spacing={2}>
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
                            if (
                              selectedPMOption === EnumPaymentMethod.Prepaid
                            ) {
                              setStripeSettingExist(false);
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
                      <Grid item md={6} sm={6} xs={12}>
                        <InputLabel
                          required={formMode === EnumDraftOrder.complete}
                          sx={styleSheet.inputLabel}
                        >
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
                          {...register(
                            "station",
                            formMode === EnumDraftOrder.complete
                              ? {
                                  required: {
                                    value: true,
                                  },
                                }
                              : {},
                          )}
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
                            setAllCarriersForSelection([]);
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
                {/* customer */}
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
                          borderRadius={"3px"}
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
                          borderRadius={"3px"}
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
                          getOptionLabel={(option) => option?.name}
                          isRHF={true}
                          required={true}
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
                      </GridContainer>
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
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
                {/* order details */}
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
                        <InputLabel
                          required={formMode === EnumDraftOrder.complete}
                          sx={styleSheet.inputLabel}
                        >
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
                          defaultValue={1}
                          fullWidth
                          variant="outlined"
                          {...register(
                            "numberOfPieces",
                            formMode === EnumDraftOrder.complete
                              ? {
                                  required: {
                                    value: true,
                                    message:
                                      LanguageReducer?.languageType
                                        ?.FIELD_REQUIRED_TEXT,
                                  },
                                }
                              : {},
                          )}
                          error={Boolean(errors.numberOfPieces)} // set error prop
                          helperText={errors.numberOfPieces?.message}
                        />
                      </Grid>
                      {Array.from(
                        { length: getValues("numberOfPieces") },
                        (_, index) => index,
                      ).map((i) => (
                        <React.Fragment key={i}>
                          <Grid item md={6} sm={6} xs={12}>
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

                          <Grid item md={6} sm={6} xs={12}>
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
                        </React.Fragment>
                      ))}
                    </Grid>
                  </CustomColorLabelledOutline>
                </GridItem>
              </GridContainer>
            </GridItem>
            <GridItem md={5} sm={12} xs={12}>
              <GridContainer>
                {/* payment */}
                <GridItem md={12} sm={12} xs={12}>
                  <CustomColorLabelledOutline
                    isCollapse={true}
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
                            handleDraftOrder(getValues());
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
                  </CustomColorLabelledOutline>{" "}
                </GridItem>
                {/* Note */}
                <GridItem xs={12} sm={12}>
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
        </div>
      </form>
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
        splitLatAndLong={splitLatAndLong}
        setAutocomplete={setAutocomplete}
      />
    </Box>
  );
}
export default DraftRegularOrder;
