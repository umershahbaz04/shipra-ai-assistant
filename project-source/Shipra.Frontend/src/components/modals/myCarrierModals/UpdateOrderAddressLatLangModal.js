import React, { useEffect, useState } from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "@mui/material/colors";
import { Grid, InputLabel, TextField } from "@mui/material";
import { styleSheet } from "../../../assets/styles/style";
import { useSelector } from "react-redux";
import { useForm, useWatch } from "react-hook-form";
import {
  ActionButtonCustom,
  useMapAutocompleteSetter,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  GridContainer,
} from "../../../utilities/helpers/Helpers";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";

import { UpdateOrderLatLng, GetAllCountry, GetAddressFromLatAndLong } from "../../../api/AxiosInterceptors";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import CountrySchema from "../../../utilities/helpers/countryschema";
import { EnumOptions } from "../../../utilities/enum";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";

const UpdateOrderAddressLatLangModal = (props) => {
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [autocomplete, setAutocomplete] = useState(null);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);
  const [isAddressLoading, setIsAddressLoading] = useState(false);
  const handleFocus = (event) => event.target.select();
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
  let {
    open,
    setOpen,
    slectedAddress,
    setSelectedAddress,
    getAllDeliveryTask,
  } = props;

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

  const [allCountries, setAllCountries] = useState([]);

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    if (res.data.result != null) setAllCountries(res.data.result);
  };

  useEffect(() => {
    getAllCountry();
  }, []);

  const handleClose = () => {
    setSelectedAddress((prev) => ({
      ...prev,
      open: false,
      coordinates: {},
    }));
    setValue("lat&long", null);
  };
  useMapAutocompleteSetter(
    autocomplete
    // allCountries,
    // allRegions,
    // allCities,
    // setValue
  );

  useEffect(() => {
    if (slectedAddress.selectedItem) {
      let latitude = slectedAddress?.coordinates?.lat;
      let longitude = slectedAddress?.coordinates?.lng;
      if (latitude !== null && longitude !== null && latitude !== undefined && longitude !== undefined) {
        setValue("lat&long", `${latitude},${longitude}`);
      }
    }
  }, [slectedAddress]);

  useWatch({
    name: "lat&long",
    control,
  });

  const updateOrderAddressLatLang = (data) => {
    const latAndLong = data["lat&long"];
    const lat = latAndLong ? latAndLong.split(",")[0] : null;
    const long = latAndLong ? latAndLong.split(",")[1] : null;

    const selectedCountry = getValues("country");
    const schema = selectedCountry?.addressingScheme;
    let addressDict = {};
    if (schema) {
      const parsedData = JSON.parse(schema);
      if (parsedData?.keys) {
        parsedData.keys.forEach((key) => {
          if (
            selectedAddressSchema[key] !== null &&
            selectedAddressSchema[key] !== undefined &&
            selectedAddressSchema[key] !== ""
          ) {
            addressDict[key] = String(selectedAddressSchema[key]);
          }
        });
      }
    }

    let param = {
      OrderNo: slectedAddress?.selectedItem?.OrderNo,
      latitude: lat,
      longitude: long,
      streetAddress: getValues("streetAddress") || selectedAddressSchema?.streetAddress,
      address: addressDict,
    };
    setIsLoading(true);
    UpdateOrderLatLng(param)
      .then((res) => {
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Address updated successfully");
          getAllDeliveryTask();
          setOpen(false);
        }
      })
      .catch((e) => {
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
        );
        console.log("e", e);
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  const splitLatAndLong = async () => {
    const latAndLong = getValues("lat&long");
    if (!latAndLong || !latAndLong.includes(",")) return;

    const lat = latAndLong.split(",")[0];
    const long = latAndLong.split(",")[1];
    if (!lat || !long) return;

    const body = {
      LATITUDE: lat,
      LONGITUDE: long,
    };

    setIsAddressLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetAddressFromLatAndLong(body),
      );
      fetchMethodResponse(response, "Address Updated", () =>
        handleSetSchemaValueForUpdate(response.result, setValue),
      );
    } catch (e) {
      console.error(e);
    } finally {
      setIsAddressLoading(false);
    }
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={
        LanguageReducer?.languageType
          ?.MY_CARRIER_DELIVERY_TASKS_UPDATE_ORDER_ADDRESS_COORDINATE
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_UPDATE_ORDER_ADDRESS_COORDINATE
          }
          bg={purple}
          loading={isLoading}
          type={"submit"}
        />
      }
      component={"form"}
      onSubmit={handleSubmit(updateOrderAddressLatLang)}
    >
      <Grid container spacing={2} sx={{ p: "15px", paddingLeft: "0" }}>
        <GridContainer>
          <Grid item md={6} sm={6} xs={12}>
            <InputLabel sx={styleSheet.inputLabel}>
              Latitude And Longitude
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
              loading={isAddressLoading}
            />
            <ActionButtonCustom
              onClick={() => setOpenLocationModal(true)}
              startIcon={<FmdGoodOutlinedIcon />}
              label={LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_CHOOSE_FROM_MAP}
              height={styleSheet.fromMapButton}
            />
          </Grid>
        </GridContainer>

        <Grid item xs={12} md={4}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SELECT_COUNTRY_TEXT}
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
            value={getValues("country") || null}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              handleSetSchema("country", resolvedId, setValue, unregister);
            }}
            errors={errors}
          />
        </Grid>
        {[...addressSchemaSelectData, ...addressSchemaInputData].map(
          (input, index, arr) => (
            <Grid item xs={12} md={4} key={input.key}>
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
                value={getValues(input.key) || ""}
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
      {openLocationModal && (
        <GoogleMapWithSearch
          open={openLocationModal}
          setOpen={setOpenLocationModal}
          setValue={setValue}
          setAutocomplete={setAutocomplete}
          splitLatAndLong={splitLatAndLong}
        />
      )}
    </ModalComponent>
  );
};

export default UpdateOrderAddressLatLangModal;
