import React, { useEffect, useState } from "react";
import {
  Box,
  CircularProgress,
  Divider,
  Grid,
  InputLabel,
  TextField,
  Checkbox,
  FormControlLabel,
} from "@mui/material";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { useForm, useWatch } from "react-hook-form";
import {
  CreateOrder,
  GetAddressFromLatAndLong,
  GetAllCountry,
  GetStoresForSelection,
  UpdateOrderIds,
  GetOrderDraftByDraftId,
  GetOrdersByContactMobile,
  UpdateLeadStatus,
  GetAllClientLeadStatusForSelection,
} from "../../../api/AxiosInterceptors";
import { DataGrid } from "@mui/x-data-grid";
import { styleSheet } from "../../../assets/styles/style";
import { EnumPaymentMethod, EnumPaymentStatus, EnumOptions, EnumLeadStatus } from "../../../utilities/enum";
import { errorNotification, successNotification, warningNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  PaymentAmountBox,
  PaymentMethodBox,
  PaymentMethodCheckbox,
  PaymentTotalBox,
  getLowerCase,
  useGetAllClientTax,
  CustomColorLabelledOutline,
  centerColumn,
  ActionButtonCustom,
  fetchMethod,
  fetchMethodResponse,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import Colors from "../../../utilities/helpers/Colors";
import { useGetAllMetafields } from "../../../utilities/helpers/HelpersFilter";
import { EnumMetaField } from "../../../utilities/enum";

const paymentMethodOptions = [
  { id: EnumPaymentMethod.Prepaid, label: "Prepaid" },
  { id: EnumPaymentMethod.CashOnDelivery, label: "Cash On Delivery" },
];

const EditLeadModal = ({ open, onClose, lead, onSuccess }) => {
  const {
    register,
    handleSubmit,
    control,
    setValue,
    getValues,
    unregister,
    reset,
    formState: { errors },
  } = useForm();

  useWatch({ name: "country", control });
  useWatch({ name: "lat&long", control });

  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allCountries, setAllCountries] = useState([]);
  const [selectedStore, setSelectedStore] = useState(null);
  const [selectedPM, setSelectedPM] = useState(EnumPaymentMethod.CashOnDelivery);
  const [subtotal, setSubtotal] = useState(0);
  const [discount, setDiscount] = useState(0);
  const [shipping, setShipping] = useState(0);
  const [vatValue, setVatValue] = useState({});
  const [saveDraftLoading, setSaveDraftLoading] = useState(false);
  const [completeLoading, setCompleteLoading] = useState(false);
  const [draftData, setDraftData] = useState(null);
  const [isMetafieldsMapped, setIsMetafieldsMapped] = useState(false);
  const [orders, setOrders] = useState([]);
  const [ordersLoading, setOrdersLoading] = useState(false);
  const [isGettingAddress, setIsGettingAddress] = useState(false);

  // Lead Status
  const [allLeadStatuses, setAllLeadStatuses] = useState([]);
  const [selectedLeadStatus, setSelectedLeadStatus] = useState(null);
  const [statusUpdateLoading, setStatusUpdateLoading] = useState(false);
  const [completedStatusId, setCompletedStatusId] = useState(null);

  const { allClientTax, loading: taxLoading } = useGetAllClientTax([]);

  const {
    loading: metaLoading,
    metafields,
    setMetafields,
  } = useGetAllMetafields(EnumMetaField.Order);

  const handleChangeMeta = (value, inputIndex, metaFieldIndex) => {
    const updatedMetaFields = [...metafields];
    updatedMetaFields[metaFieldIndex].settingConfig[inputIndex].value = value;
    setMetafields(updatedMetaFields);
  };

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
  } = useGetAddressSchema(setValue, true);

  useEffect(() => {
    if (open) {
      setIsMetafieldsMapped(false);
      setDraftData(null);
      // Reset metafield values to prevent leakage from previous leads
      if (metafields?.length > 0 && metafields[0]?.settingConfig) {
        const resetMetaFields = JSON.parse(JSON.stringify(metafields));
        resetMetaFields[0].settingConfig.forEach(i => i.value = null);
        setMetafields(resetMetaFields);
      }
      loadStores();
      loadCountries();
      loadLeadStatuses();
      if (lead) {
        console.log("lead", lead);
        const phone = lead.PhoneNumber || lead.phoneNumber;
        if (phone) {
          fetchOrders(phone);
        } else {
          setOrders([]);
        }
        const draftId = lead.OrderDraftId || lead.orderDraftId;
        console.log("draftId extracted:", draftId);
        if (draftId) {
          // If draft ID exists, fetch draft data and populate
          console.log("Calling GetOrderDraftByDraftId for:", draftId);
          GetOrderDraftByDraftId(draftId).then(res => {
            console.log("GetOrderDraftByDraftId response:", res);
            if (res?.data?.isSuccess) {
              const draftStr = res.data.result.orderInfo;
              if (draftStr) {
                const draft = JSON.parse(draftStr);
                setDraftData(draft);
                const address = draft.OrderAddress || {};

                setValue("customerName", address.CustomerName || "");
                setValue("mobile1", address.Mobile1 || lead.PhoneNumber || "");
                setValue("mobile2", address.Mobile2 || "");
                setValue("refNo", draft.RefNo || "");
                setValue("streetAddress", address.StreetAddress || "");
                setValue("streetAddress2", address.StreetAddress2 || "");
                setValue("remarks", draft.Remarks || "");
                setValue("description", draft.Description || lead.ProductName || "");

                setSubtotal(draft.ItemValue || 0);
                setShipping(draft.CShippingCharges || 0);
                setDiscount(draft.Discount || 0);

                if (draft.PaymentMethodId) setSelectedPM(draft.PaymentMethodId);

                // Set country and trigger update for nested schema (city, area, etc.)
                if (address.Country) {
                  let updatedAddress = {};
                  if (address) {
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
                    } = address;

                    if (address.SelectedCarrierId > 0) {
                      updatedAddress = {
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
                      };
                    } else {
                      const splitCity = City?.split("_")[0] || "";
                      const splitArea = Area?.split("_")[0] || "";
                      const splitprovince = Province?.split("_")[0] || "";
                      const splitstate = State?.split("_")[0] || "";
                      updatedAddress = {
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
                      };
                    }
                  }
                  handleSetSchemaValueForUpdate(updatedAddress, setValue, null);
                }
              }
            }
          }).catch(console.error);
        } else {
          // Fallback to basic lead info if no draft exists
          setValue("customerName", "");
          setValue("mobile1", lead.PhoneNumber || "");
          setValue("description", lead.ProductName || "");
          setSubtotal(0);
        }
      }
    } else {
      reset();
      setDraftData(null);
      setIsMetafieldsMapped(false);
      setSubtotal(0);
      setDiscount(0);
      setShipping(0);
      setVatValue({});
      setSelectedStore(null);
      setSelectedPM(EnumPaymentMethod.CashOnDelivery);
      setOrders([]);
      setSelectedLeadStatus(null);
      if (metafields?.length > 0 && metafields[0]?.settingConfig) {
        const resetMetaFields = JSON.parse(JSON.stringify(metafields));
        resetMetaFields[0].settingConfig.forEach(i => i.value = null);
        setMetafields(resetMetaFields);
      }
    }
  }, [open, lead, reset]);

  useEffect(() => {
    if (draftData && metafields?.length > 0 && !isMetafieldsMapped) {
      if (draftData.settingConfig && draftData.settingConfig.length > 0) {
        const updatedMetaFields = [...metafields];
        if (updatedMetaFields[0]?.settingConfig) {
          let hasChanges = false;
          draftData.settingConfig.forEach((savedInput) => {
            const index = updatedMetaFields[0].settingConfig.findIndex((i) => i.name === savedInput.name);
            if (index !== -1 && updatedMetaFields[0].settingConfig[index].value !== savedInput.value) {
              updatedMetaFields[0].settingConfig[index].value = savedInput.value;
              hasChanges = true;
            }
          });
          if (hasChanges) {
            setMetafields(updatedMetaFields);
          }
        }
      }
      setIsMetafieldsMapped(true);
    }
  }, [draftData, metafields, isMetafieldsMapped]);

  const fetchOrders = async (phone) => {
    setOrdersLoading(true);
    try {
      const response = await GetOrdersByContactMobile({ MobileNumber: phone });
      if (response?.data?.result?.list) {
        setOrders(response.data.result.list);
      } else {
        setOrders([]);
      }
    } catch (e) {
      console.error(e);
      setOrders([]);
    } finally {
      setOrdersLoading(false);
    }
  };

  const loadStores = async () => {
    try {
      const res = await GetStoresForSelection();
      const stores = res?.data?.result || [];
      setStoresForSelection(stores);
      if (stores.length > 0) setSelectedStore(stores[0]);
    } catch (e) {
      console.error(e);
    }
  };

  const loadLeadStatuses = async () => {
    try {
      const res = await fetchMethod(GetAllClientLeadStatusForSelection);
      if (res?.response?.result) {
        const statuses = res.response.result
          .filter((x) => x.description?.toLowerCase() !== "completed")
          .map((x) => ({
            label: x.description,
            value: x.clientLeadStatusId,
          }));
        setAllLeadStatuses(statuses);
        const compStatus = res.response.result.find(
          (x) => x.description?.toLowerCase() === "completed"
        );
        if (compStatus) {
          setCompletedStatusId(compStatus.clientLeadStatusId);
        }
        // Set current lead status
        if (lead?.LeadStatusId || lead?.leadStatusId) {
          const statusId = lead.LeadStatusId || lead.leadStatusId;
          const current = statuses.find((s) => s.value == statusId);
          setSelectedLeadStatus(current || null);
        }
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleUpdateStatus = async () => {
    if (!selectedLeadStatus) {
      warningNotification("Please select a status");
      return;
    }
    const currentId = lead?.LeadStatusId || lead?.leadStatusId;
    if (selectedLeadStatus.value == currentId) {
      warningNotification("Status is already set to this value");
      return;
    }
    setStatusUpdateLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        UpdateLeadStatus({
          leadId: lead.LeadId || lead.leadId,
          leadStatusId: selectedLeadStatus.value,
        })
      );
      if (response?.isSuccess) {
        successNotification("Status updated successfully");
        if (onSuccess) onSuccess();
      }
    } catch (e) {
      console.error(e);
    } finally {
      setStatusUpdateLoading(false);
    }
  };

  const loadCountries = async () => {
    try {
      const res = await GetAllCountry({});
      setAllCountries(res?.data?.result || []);
    } catch (e) {
      console.error(e);
    }
  };

  const splitLatAndLong = async () => {
    const latLongValue = getValues("lat&long") || "";
    const lat = latLongValue.split(",")[0]?.trim();
    const long = latLongValue.split(",")[1]?.trim();
    if (!lat || !long) return;

    setIsGettingAddress(true);
    try {
      const body = { LATITUDE: lat, LONGITUDE: long };
      const { response } = await fetchMethod(() => GetAddressFromLatAndLong(body));
      fetchMethodResponse(response, "Address Fetched Successfully", () =>
        handleSetSchemaValueForUpdate(response.result, setValue)
      );
    } finally {
      setIsGettingAddress(false);
    }
  };

  const calculateVatValue = (vatObj) => {
    let value = 0;
    Object.values(vatObj).forEach((val) => { value += Number(val); });
    return isNaN(value) ? 0 : value;
  };

  const total = (Number(subtotal) + calculateVatValue(vatValue) + Number(shipping) - Number(discount)).toFixed(2);

  const buildPayload = (data, isDraft = false) => {
    const address = selectedAddressSchema || {};
    return {
      orderList: [
        {
          storeId: selectedStore?.storeId,
          orderTypeId: 1,
          orderDate: UtilityClass.getFormatedDateWithoutTime(new Date()),
          description: data.description || "",
          remarks: data.remarks || "",
          amount: Number(total),
          cShippingCharges: Number(shipping),
          paymentStatusId:
            selectedPM === EnumPaymentMethod.Prepaid
              ? EnumPaymentStatus.Paid
              : EnumPaymentStatus.Unpaid,
          weight: 0,
          itemValue: Number(subtotal),
          orderRequestVia: 1,
          paymentMethodId: selectedPM,
          discount: Number(discount),
          refNo: data.refNo || "",
          orderNote: { note: "" },
          orderAddress: {
            countryId: getValues("country")?.countryId || 0,
            cityId: getValues("city")?.id || 0,
            areaId: getValues("area")?.id || 0,
            streetAddress: data.streetAddress || "",
            streetAddress2: data.streetAddress2 || "",
            addressTypeId: 0,
            orderAddressId: 0,
            customerName: data.customerName || "",
            email: "",
            mobile1: data.mobile1 || "",
            mobile2: data.mobile2 || "",
          },
          orderTaxes: [],
          orderBoxs: [],
          orderItems: [
            {
              productStockId: 0,
              quantity: 1,
              price: Number(subtotal) || 0,
              discount: 0,
              stationId: 0,
              description: data.description || "",
            },
          ],
          settingConfig: metafields?.[0]?.settingConfig || [],
        },
      ],
      settingConfig: metafields?.[0]?.settingConfig || [],
      IsSaleChannelOrder: false,
      OrderDraftId: isDraft ? 1 : 0,
    };
  };

  const handleSaveDraft = handleSubmit(async (data) => {
    setSaveDraftLoading(true);
    try {
      const res = await CreateOrder(buildPayload(data, true));
      if (res?.data?.isSuccess) {
        successNotification("Draft saved successfully");
        // Update Lead with draft ID if leadId exists
        const draftId = res?.data?.result?.data?.orderDraftId;
        if (lead?.LeadId && draftId) {
          await UpdateOrderIds({
            LeadId: lead.LeadId,
            OrderDraftId: draftId,
          }).catch(console.error);
        }
        // Update lead status if changed
        if (selectedLeadStatus) {
          await fetchMethod(() =>
            UpdateLeadStatus({
              leadId: lead?.LeadId || lead?.leadId,
              leadStatusId: selectedLeadStatus.value,
            })
          ).catch(console.error);
        }
        if (onSuccess) onSuccess();
        onClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
      }
    } catch (e) {
      errorNotification("Failed to save draft");
    } finally {
      setSaveDraftLoading(false);
    }
  });

  const handleComplete = handleSubmit(async (data) => {
    setCompleteLoading(true);
    try {
      const res = await CreateOrder(buildPayload(data, false));
      if (res?.data?.isSuccess) {
        successNotification("Order created successfully");
        // Update Lead with order ID if leadId exists
        const orderId = res?.data?.result?.data?.[0]?.orderId;
        if (lead?.LeadId && orderId) {
          await UpdateOrderIds({
            LeadId: lead.LeadId,
            OrderId: orderId,
          }).catch(console.error);
        }
        // Update lead status to Completed
        if (completedStatusId) {
          await fetchMethod(() =>
            UpdateLeadStatus({
              leadId: lead?.LeadId || lead?.leadId,
              leadStatusId: completedStatusId,
            })
          ).catch(console.error);
        }
        if (onSuccess) onSuccess();
        onClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
      }
    } catch (e) {
      errorNotification("Failed to create order");
    } finally {
      setCompleteLoading(false);
    }
  });

  const inputSx = {
    "& .MuiOutlinedInput-root": {
      fontSize: "12px",
      fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    },
  };
  const lSx = styleSheet.inputLabel;

  const orderColumns = [
    {
      field: "OrderNo",
      headerName: <Box sx={{ fontWeight: "600" }}>Order Number</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.OrderNo}</span>
      ),
    },
    {
      field: "CustomerName",
      headerName: <Box sx={{ fontWeight: "600" }}>Name</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.CustomerName || "-"}</span>
      ),
    },
    {
      field: "Description",
      headerName: <Box sx={{ fontWeight: "600" }}>Description</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.Description || "-"}</span>
      ),
    },
    {
      field: "CustomerFullAddress",
      headerName: <Box sx={{ fontWeight: "600" }}>Full Address</Box>,
      minWidth: 250,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.CustomerFullAddress || "-"}</span>
      ),
    },
    {
      ...centerColumn,
      field: "OrderDate",
      headerName: <Box sx={{ fontWeight: "600" }}>Order Date</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{UtilityClass.convertUtcToLocalAndGetDate(params.row.OrderDate)}</span>
      ),
    },
  ];

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      title="Create Order"
      maxWidth="lg"
      actionBtn={null}
    >
      <Box sx={{ mt: 1 }}>

        <CustomColorLabelledOutline
          isCollapse={true}
          defaultCollapseState={false}
          label={`Orders (${orders.length})`}
        >
          <Box
            sx={{
              ...styleSheet.allOrderTable,
              height: 250,
              width: "100%",
              mt: 1,
              mb: 2,
            }}
          >
            <DataGrid
              loading={ordersLoading}
              rows={orders}
              columns={orderColumns}
              getRowId={(row) => row.OrderNo}
              disableRowSelectionOnClick
              headerHeight={40}
              sx={{
                fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
                fontSize: "12px",
                fontWeight: "500",
              }}
            />
          </Box>
        </CustomColorLabelledOutline>

        <Grid container spacing={2} mt={1}>

          {/* ─── LEFT COLUMN ─── */}
          <Grid item xs={12} md={8}>
            <Grid container spacing={2}>

              {/* Customer Name */}
              <Grid item xs={12} md={6}>
                <InputLabel sx={lSx} required>Customer Name</InputLabel>
                <TextField
                  fullWidth size="small" sx={inputSx}
                  placeholder="e.g. John Doe"
                  {...register("customerName", { required: "Required" })}
                  error={!!errors.customerName}
                  helperText={errors.customerName?.message}
                />
              </Grid>

              {/* Phone No */}
              <Grid item xs={12} md={6}>
                <InputLabel sx={lSx} required>Phone No</InputLabel>
                <CustomRHFPhoneInput name="mobile1" control={control} required error={!!errors.mobile1} />
              </Grid>

              {/* Mobile 2 */}
              <Grid item xs={12} md={6}>
                <InputLabel sx={lSx}>Mobile 2</InputLabel>
                <CustomRHFPhoneInput name="mobile2" control={control} />
              </Grid>

              {/* Ref No */}
              <Grid item xs={12} md={6}>
                <InputLabel sx={lSx}>Ref No</InputLabel>
                <TextField fullWidth size="small" sx={inputSx} placeholder="SH12321" {...register("refNo")} />
              </Grid>

              {/* Country */}
              <Grid item xs={12} md={4}>
                <InputLabel sx={lSx} required>Country</InputLabel>
                <SelectComponent
                  name="country"
                  control={control}
                  options={allCountries}
                  isRHF={true}
                  getOptionLabel={(option) => option?.name}
                  optionLabel={EnumOptions.COUNTRY.LABEL}
                  optionValue={EnumOptions.COUNTRY.VALUE}
                  value={getValues("country")}
                  onChange={(event, newValue) => {
                    const resolvedId = newValue ? newValue : null;
                    handleSetSchema("country", resolvedId, setValue, unregister);
                  }}
                  errors={errors}
                />
              </Grid>

              {/* Dynamic Address Schema Fields (City, Province, Area etc.) */}
              {[...addressSchemaSelectData, ...addressSchemaInputData].map(
                (input, index, arr) => (
                  <Grid item md={4} sm={6} xs={12} key={input.key}>
                    <SchemaTextField
                      loading={input.loading}
                      disabled={input.disabled}
                      isRHF={true}
                      type={input.type}
                      name={input.key}
                      required={false}
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
                )
              )}



              {/* Latitude & Longitude */}
              <Grid item xs={12} md={6}>
                <InputLabel sx={lSx}>Latitude & Longitude</InputLabel>
                <CustomLatLongTextField
                  name="lat&long"
                  required={false}
                  register={register}
                  errors={errors}
                />
              </Grid>
              <Grid item xs={12} md={6} alignSelf="end" display="flex" gap={1}>
                <ActionButtonCustom
                  onClick={splitLatAndLong}
                  startIcon={<FmdGoodOutlinedIcon />}
                  disabled={!getValues("lat&long")?.includes(",")}
                  label="Get Address"
                  loading={isGettingAddress}
                  height={40}
                />
              </Grid>

              {/* Remarks */}
              <Grid item xs={12} md={6}>
                <InputLabel sx={lSx}>Remarks</InputLabel>
                <TextField fullWidth size="small" sx={inputSx} placeholder="Remarks" {...register("remarks")} />
              </Grid>

              {/* Description */}
              <Grid item xs={12} md={6}>
                <InputLabel sx={lSx}>Description</InputLabel>
                <TextField fullWidth size="small" sx={inputSx} placeholder="Description" {...register("description")} />
              </Grid>

              {/* MetaField Section */}
              {metafields?.length > 0 && (
                <Grid item xs={12} mt={2}>
                  <CustomColorLabelledOutline isCollapse={true} label={"MetaField"}>
                    <Grid container spacing={2} sx={{ paddingTop: 2 }}>
                      {metafields.map((meta, metaFieldIndex) =>
                        meta.settingConfig.map((input, input_index) => (
                          <Grid item xs={12} sm={6} key={input.name} paddingTop={"0px!important"}>
                            <Box marginBottom={1}>
                              <InputLabel required={input.required} sx={styleSheet.inputLabel}>
                                {input?.name}
                              </InputLabel>

                              {/* SELECT FIELD */}
                              {getLowerCase(input.type.label) === inputTypesEnum.SELECT && (
                                <SelectComponent
                                  height={40}
                                  name={input.name}
                                  options={input.selectOptions}
                                  optionLabel="label"
                                  optionValue="id"
                                  value={input.value}
                                  onChange={(e, newValue) => handleChangeMeta(newValue, input_index, metaFieldIndex)}
                                />
                              )}

                              {/* TEXT / NUMBER / DATE FIELD */}
                              {(getLowerCase(input.type.label) === inputTypesEnum.TEXT ||
                                getLowerCase(input.type.label) === inputTypesEnum.NUMBER ||
                                getLowerCase(input.type.label) === inputTypesEnum.DATE) && (
                                  <TextField
                                    type={getLowerCase(input.type.label)}
                                    placeholder={input.description}
                                    size="small"
                                    fullWidth
                                    variant="outlined"
                                    required={input.required}
                                    value={input.value || ""}
                                    onChange={(e) => handleChangeMeta(e.target.value, input_index, metaFieldIndex)}
                                  />
                                )}

                              {/* CHECKBOX FIELD */}
                              {getLowerCase(input.type.label) === inputTypesEnum.CHECKBOX && (
                                <FormControlLabel
                                  control={
                                    <Checkbox
                                      sx={{
                                        color: "var(--primary-color)",
                                        "&.Mui-checked": { color: "var(--primary-color)" },
                                      }}
                                      checked={input.value ?? input.defaultValue ?? false}
                                      onChange={(e) => handleChangeMeta(e.target.checked, input_index, metaFieldIndex)}
                                    />
                                  }
                                />
                              )}
                            </Box>
                          </Grid>
                        ))
                      )}
                    </Grid>
                  </CustomColorLabelledOutline>
                </Grid>
              )}

            </Grid>
          </Grid>

          {/* ─── RIGHT COLUMN (Payment Panel) ─── */}
          <Grid item xs={12} md={4}>
            <Box display="flex" flexDirection="column" gap={1} sx={{ borderLeft: "1px dashed #ddd", pl: 2, height: "100%" }}>

              <PaymentAmountBox
                title="Sub total"
                required
                value={subtotal}
                onChange={(e) => setSubtotal(e.target.value)}
              />

              {taxLoading ? (
                <Box className="flex_center"><CircularProgress size={20} /></Box>
              ) : (
                allClientTax.map((_tax) =>
                  _tax.Active && (
                    <PaymentAmountBox
                      key={_tax.ClientTaxId}
                      title={_tax.Name}
                      value={_tax.Percentage}
                      onChange={(e) => {
                        setVatValue((prev) => ({
                          ...prev,
                          [_tax.ClientTaxId]: e.target.value,
                        }));
                      }}
                    />
                  )
                )
              )}

              <PaymentTotalBox value={total} />

              <Divider />

              <PaymentMethodBox
                options={
                  <Box className="flex_between">
                    {paymentMethodOptions.map((option) => (
                      <PaymentMethodCheckbox
                        key={option.id}
                        checked={selectedPM === option.id}
                        onChange={() => setSelectedPM(option.id)}
                        label={option.label}
                      />
                    ))}
                  </Box>
                }
              />

              <Divider />

              {/* Lead Status */}
              <Box>
                <InputLabel sx={{ ...styleSheet.inputLabel, mb: 0.5 }}>Lead Status</InputLabel>
                <SelectComponent
                  name="leadStatus"
                  options={allLeadStatuses}
                  value={selectedLeadStatus}
                  optionLabel="label"
                  optionValue="value"
                  onChange={(name, val) => setSelectedLeadStatus(val)}
                />
              </Box>

              {/* Action Buttons */}
              <Box sx={{ display: "flex", flexDirection: "column", gap: 1, }}>
                <ModalButtonComponent
                  title="Save Draft"
                  loading={saveDraftLoading}
                  onClick={handleSaveDraft}
                  bg={Colors.succes}
                />
                <ModalButtonComponent
                  title="Complete"
                  loading={completeLoading}
                  onClick={handleComplete}
                  bg={Colors.linkColor}
                />
              </Box>

            </Box>
          </Grid>

        </Grid>
      </Box>
    </ModalComponent>
  );
};

export default EditLeadModal;
