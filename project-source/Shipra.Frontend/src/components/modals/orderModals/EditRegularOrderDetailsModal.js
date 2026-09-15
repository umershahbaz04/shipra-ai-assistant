import DeleteIcon from "@mui/icons-material/Delete";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import SendIcon from "@mui/icons-material/Send";
import {
  Box,
  Button,
  CircularProgress,
  Divider,
  FormControl,
  Grid,
  InputAdornment,
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
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
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
  GetAllStationLookup,
  GetChannelListByStoreIdForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { calculateVatValue } from "../../../pages/orders/createRegularOrder";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  EnumOptions,
  EnumOrderType,
  EnumPaymentMethod,
  EnumPaymentStatus,
} from "../../../utilities/enum";
import {
  ActionButtonCustom,
  CustomColorLabelledOutline,
  GridContainer,
  GridItem,
  PaymentAmountBox,
  PaymentMethodBox,
  PaymentMethodCheckbox,
  PaymentTaxBox,
  PaymentTotalBox,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  placeholders,
  purple,
  useGetAllClientTax,
  useMapAutocompleteSetter,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch";
import AddOrderBoxModal from "./AddOrderBoxModal";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function EditRegularOrderDetailsModal(props) {
  const navigate = useNavigate();
  const dispatch = useDispatch();
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
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    handleRestForm();
    setSelectedData();
    setOpen(false);
  };
  const handleRestForm = () => {
    setFinalNote("");
    setNote("");
    setDiscount(0);
    setShipping(0);
    setTax(5);
    reset();
    setSelectedData();
    setValue("mobile1", UtilityClass.getDefaultCountryCode());
    setValue("mobile2", UtilityClass.getDefaultCountryCode());
  };
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [addProductOpen, setAddProductOpen] = useState(false);
  const [openFromLinkLocationModal, setOpenFromLinkLocationModal] =
    useState(false);
  const [allCountries, setAllCountries] = useState([]);
  const [allRegions, setAllRegions] = useState([]);
  const [allCities, setAllCities] = useState([]);
  const [discount, setDiscount] = useState(selectedData?.discount);
  const [shipping, setShipping] = useState(selectedData?.cShippingCharges);
  const [tax, setTax] = useState(5);
  const [note, setNote] = useState("");
  const [finalNote, setFinalNote] = useState("");
  const [subtotal, setSubtotal] = useState(selectedData?.amount);
  const [storesChannelForSelection, setStoresChannelForSelection] = useState(
    [],
  );
  const [openUpdateOrderBoxModal, setopenUpdateOrderBoxModal] = useState({
    open: false,
    loading: {},
  });
  const [allClientOrderBox, setAllClientOrderBox] = useState([]);
  const [selectedOrderBox, setSelectedOrderBox] = useState([]);
  const [addedOrderBoxes, setAddedOrderBoxes] = useState([]);
  const [autocomplete, setAutocomplete] = useState(null);
  const {
    register,
    unregister,
    handleSubmit,
    formState: { errors },
    reset,
    getValues,
    setValue,
    control,
  } = useForm({
    defaultValues: {
      numberOfPieces: selectedData?.itemsCount,
      descriptionOfPieces: [],
      mobile1: selectedData?.orderAddress?.mobile1,
    },
  });
  const { allClientTax, loading } = useGetAllClientTax([]);

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
    name: "numberOfPieces",
    control,
  });
  useWatch({
    name: "lat&long",
    control,
  });

  const [paymentDueLaterCheckBox, setPaymentDueLaterCheckBox] = useState(false);
  const [selectedPMOption, setselectedPMOption] = useState(1);
  const [productStations, setProductStations] = useState([]);
  const [vatValue, setVatValue] = useState(0);

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
  useEffect(() => {
    getAllStationLookup();
  }, []);
  const replaceObjectAtIndex = (updatedObject) => {
    const newArray = [...orderData];
    const modifiedObject = { ...updatedObject };
    newArray[selectedData?.index - 1] = modifiedObject;
    setOrderData(newArray);
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
  

  const updateData = (data) => {
    const amount =
      Number(subtotal) +
      calculateVatValue(vatValue) +
      Number(shipping) -
      Number(discount);
    const actualAmmount = Number(subtotal);

    const itemValue = subtotal;
    let orderItems = [];

    for (let index = 0; index < data.numberOfPieces; index++) {
      // if (data.descriptionOfPieces[index].length != 0) {
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
      // }
    }

    const updatedObject = {
      index: selectedData?.index,
      storeId: data.store.storeId,
      SaleChannelConfigId: data.storeChannel?.id,
      orderTypeId: EnumOrderType.Regular,
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
      itemValue: itemValue,
      orderRequestVia: 1,
      paymentMethodId: selectedPMOption,
      stationId: selectedData?.stationId,
      discount: discount,
      vat: 0,
      refNo: data?.refNo,
      orderNote: {
        note: finalNote,
      },
      orderAddress: {
        customerName: data.customerName,
        email: data.email,
        mobile1: data.mobile1,
        mobile2: data.mobile2,
        country: selectedAddressSchema.country,
        city: selectedAddressSchema.city,
        area: selectedAddressSchema.area,
        province: selectedAddressSchema.province,
        pinCode: selectedAddressSchema.pinCode,
        state: selectedAddressSchema.state,
        streetAddress: selectedAddressSchema.streetAddress,
        streetAddress2: selectedAddressSchema.streetAddress2,
        streetAddress2: selectedAddressSchema.streetAddress2,
        houseNo: selectedAddressSchema.houseNo,
        buildingName: selectedAddressSchema.buildingName,
        landmark: selectedAddressSchema.landmark,
        zip: selectedAddressSchema.zip,
        latitude: getValues("lat&long").split(",")[0] || null,
        longitude: getValues("lat&long").split(",")[1] || null,
      },
      orderTaxes: selectedData?.orderTaxes
        .map((tax) => {
          return {
            orderTaxId: tax.orderTaxId,
            clientTaxId: tax.clientTaxId,
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
    const filtersData = errorsList.filter((x) => x.Row != selectedRowIndex);
    setErrorsList(filtersData);
    handleClose();
  };

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    // console.log("res", res.data.result);
    if (res.data.result != null) setAllCountries(res.data.result);
  };
  useEffect(() => {
    // setValue("numberOfPieces", selectedData?.orderItems?.length);
    setValue("store", selectedData?.storeId);
    getAllCountry();
  }, []);

  const handleFocus = (event) => event.target.select();
  const paymentMethodoptions = [
    { id: 1, label: "Prepaid" },
    { id: 2, label: "Cash On Delivery" },
  ];

  const handleCheckboxChange = (optionId) => {
    setselectedPMOption(optionId);
  };

  useEffect(() => {
    if (selectedData) {
      setselectedPMOption(selectedData?.paymentMethodId);
      setValue("mobile1", selectedData?.orderAddress.mobile1);
      setValue("mobile2", selectedData?.orderAddress.mobile2);
    }
  }, []);

  const getChannelListByStoreIdForSelection = async () => {
    setValue("storeChannel", null);

    if (getValues("store").storeId) {
      let res = await GetChannelListByStoreIdForSelection(
        getValues("store").storeId,
      );
      if (res.data.result != null) {
        setStoresChannelForSelection(res.data.result);
      }
    }
  };

  useEffect(() => {
    if (storesForSelection && storesForSelection.length > 0 && selectedData) {
      const defvalue = storesForSelection.find(
        (x) => x.storeId === selectedData.storeId,
      );
      setValue("store", defvalue);
    }
  }, [storesForSelection, selectedData]);
  useEffect(() => {
    if (productStations && productStations.length > 0 && selectedData) {
      const defvalue = productStations.find(
        (x) => x.productStationId === selectedData.stationId,
      );
      setValue("station", defvalue);
    }
  }, [productStations]);
  useEffect(() => {
    if (
      storesChannelForSelection &&
      storesChannelForSelection.length > 0 &&
      selectedData
    ) {
      const defvalue = storesChannelForSelection.find(
        (x) => x.id === selectedData.saleChannelConfigId,
      );
      setValue("storeChannel", defvalue);
    }
  }, [storesChannelForSelection]);

  useEffect(() => {
    if (
      getValues("store") !== null &&
      getValues("store") !== "" &&
      getValues("store") !== undefined
    ) {
      getChannelListByStoreIdForSelection();
    }
  }, [getValues("store")]);
  useMapAutocompleteSetter(
    autocomplete,
    allCountries,
    allRegions,
    allCities,
    setValue,
  );
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
      setValue("mobile2", selectedData?.orderAddress.mobile1);
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
      title={"Update Order" + " "}
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
      <Box sx={styleSheet.pageRoot}>
        {/* <Container maxWidth="lg" fixed sx={{ paddingLeft: "0px" }}> */}
        <div>
          <Grid container spacing={2}>
            <Grid item sm={12} md={12} lg={7}>
              <CustomColorLabelledOutline
                isCollapse={true}
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
                      value={getValues("store")}
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
                      onChange={(event, newValue) => {
                        const resolvedId = newValue ? newValue : null;
                        setValue("store", resolvedId);
                      }}
                      errors={errors}
                    />
                  </Grid>
                  <Grid item md={6} lg={6} sm={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      Select Sale Channel
                    </InputLabel>
                    <SelectComponent
                      name="storeChannel"
                      control={control}
                      options={storesChannelForSelection}
                      disabled={
                        getValues("store")
                          ? getValues("store").storeId === 0
                          : true
                      }
                      isRHF={true}
                      optionLabel={EnumOptions.STORE_CHANNEL.LABEL}
                      optionValue={EnumOptions.STORE_CHANNEL.VALUE}
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
                      {LanguageReducer?.languageType?.SELECT_STATION_TEXT}
                    </InputLabel>
                    <SelectComponent
                      name="station"
                      control={control}
                      options={productStations}
                      value={getValues("station")}
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
                      onChange={(event, newValue) => {
                        const resolvedId = newValue ? newValue : null;
                        setValue("station", resolvedId);
                      }}
                      errors={errors}
                    />
                  </Grid>
                  <Grid item md={6} lg={6} sm={12}>
                    <InputLabel sx={styleSheet.inputLabel} required>
                      Order Date
                    </InputLabel>
                    <CustomRHFReactDatePickerInput
                      name="orderDate"
                      control={control}
                      // onChange={handleOnChange}
                      required
                      error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
                      defaultValue={new Date()}
                    />
                  </Grid>
                  <Grid item md={12} sm={12}>
                    <InputLabel sx={styleSheet.inputLabel}>Ref No.</InputLabel>
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
                      defaultValue={selectedData?.refNo}
                    />
                  </Grid>
                </Grid>
              </CustomColorLabelledOutline>
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
                      selectedCountry={getValues("country")?.mapCountryCode}
                      name="mobile1"
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
              <CustomColorLabelledOutline
                isCollapse={true}
                label={LanguageReducer?.languageType?.ORDER_DETAILS_TEXT}
              >
                <Grid container spacing={2}>
                  <Grid item md={6} sm={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.ENTER_DESCRIPTION_TEXT}
                    </InputLabel>
                    <TextField
                      placeholder="Description"
                      onFocus={handleFocus}
                      size="small"
                      multiline
                      fullWidth
                      defaultValue={selectedData?.description}
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
                  <Grid item md={6} sm={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.ENTER_REMARKS_TEXT}
                    </InputLabel>
                    <TextField
                      placeholder="Remarks"
                      onFocus={handleFocus}
                      size="small"
                      multiline
                      fullWidth
                      defaultValue={selectedData?.remarks}
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
                  <Grid item md={6} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.NUMBER_OF_PIECES_TEXT}
                    </InputLabel>
                    <TextField
                      placeholder={placeholders.quantity}
                      onFocus={handleFocus}
                      type="number"
                      size="small"
                      id="numberOfPieces"
                      name="numberOfPieces"
                      fullWidth
                      defaultValues={
                        (selectedData?.orderItems.length &&
                          selectedData?.orderItems.length > 0) ||
                        1
                      }
                      variant="outlined"
                      {...register("numberOfPieces", {
                        required: {
                          value: true,
                          message:
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                        },
                      })}
                      error={Boolean(errors.numberOfPieces)} // set error prop
                      helperText={errors.numberOfPieces?.message}
                    />
                  </Grid>
                  {Array.from(
                    { length: getValues("numberOfPieces") },
                    (_, index) => index,
                  ).map((i, index) => (
                    <Grid item md={6} xs={12}>
                      <InputLabel sx={styleSheet.inputLabel}>
                        {
                          LanguageReducer?.languageType
                            ?.DESCRIPTION_OF_PIECE_TEXT
                        }{" "}
                        {i + 1}
                      </InputLabel>
                      <TextField
                        placeholder={`${
                          LanguageReducer?.languageType
                            ?.DESCRIPTION_OF_PIECE_TEXT
                        } ${i + 1}`}
                        onFocus={handleFocus}
                        type="text"
                        size="small"
                        defaultValue={selectedData?.orderItems[i]?.description}
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
                        helperText={errors.descriptionOfPieces?.[i]?.message}
                      />
                    </Grid>
                  ))}
                </Grid>
              </CustomColorLabelledOutline>
            </Grid>
            <Grid item sm={12} md={12} lg={5}>
              <CustomColorLabelledOutline label={"Payment"} isCollapse={true}>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                  <PaymentAmountBox
                    title={"Subtotal"}
                    required
                    value={subtotal}
                    onChange={(e) => {
                      setSubtotal(e.target.value);
                    }}
                  />
                  <PaymentAmountBox
                    title={"Add Discount"}
                    value={discount}
                    onChange={(e) => {
                      setDiscount(e.target.value);
                    }}
                  />
                  <PaymentAmountBox
                    title={"Add Shipping"}
                    value={shipping}
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
                    endAdornment={
                      <InputAdornment position="end">
                        <Button
                          onClick={(e) => {
                            setFinalNote(note);
                          }}
                          sx={styleSheet.sendNotButton}
                          variant="contained"
                        >
                          <SendIcon />
                        </Button>
                      </InputAdornment>
                    }
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
            </Grid>
          </Grid>
        </div>
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

        {/* <FromLinkLocationModal
                open={openFromLinkLocationModal}
                setOpen={setOpenFromLinkLocationModal}
                setValue={setValue}
              /> */}
      </Box>
    </ModalComponent>
  );
}
export default EditRegularOrderDetailsModal;
