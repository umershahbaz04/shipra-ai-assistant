import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import {
  Box,
  CircularProgress,
  Grid,
  InputLabel,
  TextField,
  Typography,
} from "@mui/material";
import { amber, purple } from "@mui/material/colors";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateActiveCarrierPickupLocation,
  DeletepickLocationById,
  GetActiveCarrierPickupLocationbyid,
  GetActiveCarrierPickupLocationForSelection,
  GetAddressFromLatAndLong,
} from "../../../api/AxiosInterceptors";
import { getAllCountryFunc } from "../../../apiCallingFunction";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions, inputTypesEnum } from "../../../utilities/enum";
import {
  addressSchemaEnum,
  SchemaTextField,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import {
  ActionButtonCustom,
  ActionButtonDelete,
  ActionButtonEdit,
  centerColumn,
  CodeBox,
  CustomColorLabelledOutline,
  DataGridHeaderBox,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  getOptionValueObjectByValue,
  GridContainer,
  placeholders,
  StyledTooltip,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal";

const PickupLocationModal = (props) => {
  const {
    open,
    onClose,
    pickupLocationIds,
    showTable,
    getActiveCarrierPickupLocation,
    pickupLocation,
    allActiveCarrier,
  } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [allCountries, setAllCountries] = useState([]);
  const [loading, setLoading] = useState(false);
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [tableLoading, setTableLoading] = useState(true);
  const [pickUpLocationData, setPickUpLocationData] = useState([]);
  const handleFocus = (event) => event.target.select();
  const [autocomplete, setAutocomplete] = useState(null);
  const [openDelete, setOpenDelete] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState(null);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [editLoading, setEditLoading] = useState({
    data: null,
    loading: {},
  });
  const [selectedActiveCarrier, setSeletedActiveCarrier] = useState();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
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
  } = useGetAddressSchema(
    setValue,
    true,
    pickupLocationIds?.CarrierId ?? selectedActiveCarrier?.CarrierId,
  );

  useWatch({
    name: "lat&long",
    control,
  });

  let getAllCountry = async () => {
    let data = await getAllCountryFunc();
    if (data?.length > 0) setAllCountries(data);
  };

  const handleEditPickUpLocation = async (id) => {
    setEditLoading((prev) => ({
      ...prev,
      loading: { ...prev.loading, [id]: true },
    }));

    try {
      const response = await GetActiveCarrierPickupLocationbyid(id);
      if (response.data.isSuccess) {
        setEditLoading((prev) => ({
          ...prev,
          data: response.data.result,
        }));
      }
    } catch (e) {
      console.error(e);
    } finally {
      setEditLoading((prev) => ({
        ...prev,
        loading: { ...prev.loading, [id]: false },
      }));
    }
  };

  const handleDeletePickUpLocation = async () => {
    setDeleteLoading(true);
    try {
      const response = await DeletepickLocationById(deleteItemObject);
      if (response.data.isSuccess) {
        successNotification("PickUp Location Deleted Successfully");
        getActiveCarrierPickupLocationForSelection();
      }
    } catch (e) {
      console.error(e);
    } finally {
      setDeleteLoading(false);
      setOpenDelete(false);
    }
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
      handleSetSchemaValueForUpdate(
        response.result,
        setValue,
        pickupLocationIds?.CarrierId,
      ),
    );
  };

  const getActiveCarrierPickupLocationForSelection = async (carrierId) => {
    setTableLoading(true);
    try {
      const response = await GetActiveCarrierPickupLocationForSelection(
        pickupLocationIds?.ActiveCarrierId || 0,
        pickupLocationIds?.CarrierId || 0,
      );
      if (response?.data?.isSuccess) {
        const data = response?.data?.result
          .map((item, index) => ({ ...item, index }))
          .slice(1);
        setPickUpLocationData(data);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setTableLoading(false);
    }
  };

  const create = async (data) => {
    const result =
      editLoading.data === null ? pickupLocation : editLoading.data;
    const body = {
      LocationName: data?.LocationName,
      addressTypeId: 0,
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
      customerServiceNo: data.customerServiceNo,
      phone: data.phone,
      entityAddressDataJson: "",
      carrierId: !showTable
        ? selectedActiveCarrier?.CarrierId
        : pickupLocationIds?.CarrierId,
      activeCarrierId: !showTable
        ? selectedActiveCarrier?.ActiveCarrierId
        : pickupLocationIds?.ActiveCarrierId,
      activecarrierpickuplocationid: result?.activeCarrierPickupLocationId
        ? result.activeCarrierPickupLocationId
        : 0,
    };
    setLoading(true);
    try {
      const response = await CreateActiveCarrierPickupLocation(body);
      if (response?.data?.isSuccess) {
        successNotification("Action perform successfully.");
        getActiveCarrierPickupLocationForSelection();
        getActiveCarrierPickupLocation();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch {
    } finally {
      setLoading(false);
      onClose();
    }
  };

  useEffect(() => {
    const result =
      editLoading.data === null ? pickupLocation : editLoading.data;
    const isDispatchExCompany = result?.isDispatchExCompany;
    let carrierId = 0;
    if (isDispatchExCompany && showTable) {
      carrierId = 0;
    } else if (!isDispatchExCompany && showTable) {
      carrierId = pickupLocationIds?.CarrierId;
    } else if (isDispatchExCompany && !showTable) {
      carrierId = 0;
    } else if (!isDispatchExCompany && !showTable) {
      carrierId = result?.carrierId;
    } else {
      errorNotification("Something Went Wrong while fetching pickuplocation");
    }

    if (result) {
      setValue("LocationName", result?.locationName ?? "");
      setValue("customerServiceNo", result?.customerServiceNo ?? "");
      setValue("phone", result?.phone ?? "");
      const pickPart = (value) => {
        if (!value) return result?.isDispatchExCompany ? "" : 0;
        const parts = value.split("_");
        if (result?.isDispatchExCompany) {
          return isNaN(parts[0]) ? parts[0] : parseInt(parts[0], 10);
        } else {
          return value;
        }
      };
      const city = pickPart(result?.address?.city);
      const area = pickPart(result?.address?.area);
      const province = pickPart(result?.address?.province);
      const state = pickPart(result?.address?.state, carrierId);
      const carrierObj = getOptionValueObjectByValue(
        allActiveCarrier,
        EnumOptions.ACTIVE_CARRIER.VALUE,
        result?.activeCarrierId,
      );
      // console.log({ city, area, province, state, carrierObj });
      handleSetSchemaValueForUpdate(
        {
          ...result?.address,
          city,
          area,
          province,
          state,
        },
        setValue,
        carrierId,
      );
      setSeletedActiveCarrier(carrierObj);
      let latitude = result?.address?.latitude;
      let longitude = result?.address?.longitude;
      if (latitude && longitude) {
        setValue("lat&long", `${latitude},${longitude}`);
      }
    }
  }, [editLoading.data, pickupLocation]);

  useEffect(() => {
    getAllCountry();
    getActiveCarrierPickupLocationForSelection();
  }, []);

  const pickupLocationColumn = [
    {
      field: "Address",
      headerName: <DataGridHeaderBox title={"PickUp Address"} />,
      minWidth: 300,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <CodeBox title={row.fullAddress} />
          </>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 80,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Action"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box>
            <StyledTooltip
              title={`activeCarrierPickupLocationId:${row?.activeCarrierPickupLocationId}`}
            >
              <span>
                <ActionButtonDelete
                  onClick={() => {
                    setOpenDelete(true);
                    setDeleteItemObject(row?.activeCarrierPickupLocationId);
                  }}
                />
              </span>
            </StyledTooltip>
            <StyledTooltip
              title={`activeCarrierPickupLocationId:${row?.activeCarrierPickupLocationId}`}
            >
              <span>
                <ActionButtonEdit
                  onClick={() =>
                    handleEditPickUpLocation(row?.activeCarrierPickupLocationId)
                  }
                  loading={
                    !!editLoading.loading[row?.activeCarrierPickupLocationId]
                  }
                />
              </span>
            </StyledTooltip>
          </Box>
        );
      },
      flex: 1,
    },
  ];

  return (
    <div>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="md"
        title={"Pickup Location"}
        actionBtn={
          <ModalButtonComponent
            loading={loading}
            title={"Save"}
            bg={purple}
            type="submit"
            onClick={handleSubmit(create)}
          />
        }
        component={"form"}
      >
        <Grid container spacing={1}>
          <Grid item md={12} sm={12} xs={12}>
            {tableLoading ? (
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                }}
              >
                <CircularProgress />
              </Box>
            ) : (
              <>
                {showTable ? (
                  pickUpLocationData.length === 0 ? (
                    <Box
                      sx={{
                        backgroundColor: amber[100],
                        border: `1px solid ${amber[700]}`,
                        borderRadius: "4px",
                        padding: "10px",
                        marginBottom: "12px",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                      }}
                    >
                      <Typography variant="h6" color="textPrimary">
                        There are currently no active pickup locations
                        available.
                      </Typography>
                    </Box>
                  ) : (
                    <Box mb={2}>
                      <DataGrid
                        autoHeight={false}
                        bgColor={"#fff"}
                        rowHeight={40}
                        headerHeight={40}
                        sx={{
                          fontFamily:
                            "'Lato Regular', 'Inter Regular', 'Arial' !important",
                          fontSize: "12px",
                          fontWeight: "500",
                          backgroundColor: "#fff",
                          height: "160px",
                          maxHeight: "160px",
                          overflowX: "auto",
                          overflowY: "auto",
                        }}
                        rows={pickUpLocationData}
                        getRowId={(row) => row.index}
                        columns={pickupLocationColumn}
                        checkboxSelection={false}
                        disableSelectionOnClick
                        hideFooter
                        hideFooterPagination
                      />
                    </Box>
                  )
                ) : null}
              </>
            )}
          </Grid>
        </Grid>
        <CustomColorLabelledOutline label={"Pickup Location"}>
          <Grid container spacing={1}>
            {!showTable && (
              <Grid item xl={4} lg={4} md={4} sm={6} xs={12}>
                <InputLabel
                  required
                  sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                >
                  {LanguageReducer?.languageType?.ORDER_CARRIER}
                </InputLabel>
                <SelectComponent
                  name="carrier"
                  options={allActiveCarrier}
                  disabled={pickupLocation ? true : false}
                  value={selectedActiveCarrier}
                  optionLabel={EnumOptions.ACTIVE_CARRIER.LABEL}
                  optionValue={EnumOptions.ACTIVE_CARRIER.VALUE}
                  onChange={(e, val) => {
                    setSeletedActiveCarrier(val);
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
            )}
            <Grid item md={4} sm={6} xs={12}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {"Location Name"}
              </InputLabel>
              <TextField
                type="text"
                placeholder={placeholders.name}
                onFocus={handleFocus}
                size="small"
                fullWidth
                variant="outlined"
                id="LocationName"
                name="LocationName"
                {...register("LocationName", {
                  required: {
                    value: true,
                    message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
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
            <Grid item sm={6} md={4} xs={12}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.STORE_CUSTOMER_SERVICE_NO}
              </InputLabel>
              <CustomRHFPhoneInput
                error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
                name="customerServiceNo"
                selectedCountry={getValues("country")?.mapCountryCode}
                control={control}
                required
              />
            </Grid>
            <Grid item sm={6} md={4} xs={12}>
              <InputLabel sx={styleSheet.inputLabel}>
                {"Phone No (Optional)"}
              </InputLabel>
              <CustomRHFPhoneInput
                error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
                name="phone"
                selectedCountry={getValues("country")?.mapCountryCode}
                control={control}
                isContact={true}
              />
            </Grid>
            <Grid item md={4} sm={6} xs={12}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.STORE_COUNTRY}
              </InputLabel>
              <CountrySchema
                name="country"
                control={control}
                isRHF={true}
                required={true}
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
                    pickupLocationIds?.CarrierId ??
                      selectedActiveCarrier?.CarrierId,
                  );
                }}
                errors={errors}
              />
            </Grid>
            {[...addressSchemaSelectData, ...addressSchemaInputData].map(
              (input, index, arr) => (
                <Grid item md={4} sm={6} xs={12}>
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
          </Grid>
          <GridContainer>
            <Grid item md={6} sm={6} xs={12}>
              <InputLabel sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.ORDERS_LATITUDE_AND_LONGITUDE}
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
                label={LanguageReducer?.languageType?.ORDERS_GET_ADDRESS}
                height={styleSheet.fromMapButton}
              />
              <ActionButtonCustom
                onClick={() => setOpenLocationModal(true)}
                startIcon={<FmdGoodOutlinedIcon />}
                label={LanguageReducer?.languageType?.ORDERS_FORM_MAP}
                height={styleSheet.fromMapButton}
              />
            </Grid>
          </GridContainer>
        </CustomColorLabelledOutline>

        <GoogleMapWithSearch
          open={openLocationModal}
          setOpen={setOpenLocationModal}
          setValue={setValue}
          splitLatAndLong={splitLatAndLong}
          setAutocomplete={setAutocomplete}
        />
        {openDelete && (
          <DeleteConfirmationModal
            open={openDelete}
            setOpen={setOpenDelete}
            loading={deleteLoading}
            handleDelete={handleDeletePickUpLocation}
            heading={"Confirm Deletion of Pickup Location"}
            message={
              "The selected pickup location will be permanently deleted. This action cannot be undone."
            }
            buttonText={"yes"}
          />
        )}
      </ModalComponent>
    </div>
  );
};

export default PickupLocationModal;
