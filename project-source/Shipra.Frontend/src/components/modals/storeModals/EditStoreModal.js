import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import {
  Box,
  Button,
  FormControlLabel,
  Grid,
  InputAdornment,
  InputLabel,
  Switch,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import Tooltip from "@mui/material/Tooltip";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  ActivateStoreById,
  DeleteStoreById,
  GetAddressFromLatAndLong,
  GetStoreById,
  UpdateStore,
  UploadStoreImage,
} from "../../../api/AxiosInterceptors";
import { getAllCountryFunc } from "../../../apiCallingFunction";
import uploadIcon from "../../../assets/images/uploadIcon.png";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import {
  ActionButtonCustom,
  GridContainer,
  HeightBox,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  placeholders,
  purple,
  useMapAutocompleteSetter,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import FromLinkLocationModal from "../orderModals/FromLinkLocationModal";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function EditStoreModal(props) {
  let { open, setOpen, getAllStores, selectedRow, storeData, setStoreData } =
    props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [openFromLinkLocationModal, setOpenFromLinkLocationModal] =
    useState(false);
  const [isLoading, setIsLoading] = React.useState(false);

  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState();
  const [allCountries, setAllCountries] = useState([]);
  const [allRegions, setAllRegions] = useState([]);
  const [allCities, setAllCities] = useState([]);
  const [openDeleteConfirmation, setOpenDeleteConfirmation] = useState(false);
  const [isActiveChecked, setIsActiveChecked] = useState(false);
  const [autocomplete, setAutocomplete] = useState(null);

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
  } = useGetAddressSchema();

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
    name: "lat&long",
    control,
  });
  const handleClose = () => {
    setStoreData();
    setOpen(false);
  };

  let uploadStoreImage = (e) => {
    const selectedFile = e.target.files[0];
    if (!selectedFile) return;

    const formData = new FormData();
    formData.append("File", selectedFile);

    UploadStoreImage(formData)
      .then((res) => {
        console.log("res", res);
        setFile(selectedFile);
        setImageURL(res.data.result.url);
        successNotification(res.data.result.message);
      })
      .catch((err) => {
        UtilityClass.showErrorNotificationWithDictionary(
          err.response?.data?.errors
        );
      })
      .finally(() => {
        e.target.value = null;
      });
  };

  let getAllCountry = async () => {
    let data = await getAllCountryFunc();
    if (data?.length > 0) setAllCountries(data);
  };

  useEffect(() => {
    getAllCountry();
  }, []);
  useEffect(() => {
    if (storeData) {
      setImageURL(storeData?.storeImage);
      setValue("storeName", storeData?.storeName);
      setValue("companyName", storeData?.storeCompany);
      setValue("licenseNo", storeData?.licenseNo);
      setValue("customerServiceNo", storeData?.customerServiceNo);
      if (!storeData?.phone) {
        setValue("phone", UtilityClass.getDefaultCountryCode());
      } else {
        setValue("phone", storeData?.phone);
      }
      setValue("email", storeData?.email);
      setValue("zip", storeData?.address?.zip);
      setValue("Url", storeData?.urls);
      setValue("longitude", storeData?.address?.longitude);
      setValue("latitude", storeData?.address?.latitude);
      setIsActiveChecked(storeData?.active);
    }
  }, [storeData]);
  const createStore = async (data) => {
    // console.log("vals::", data);
    const body = {
      storeId: storeData.storeId,
      storeName: data.storeName,
      storeCompany: data.companyName,
      customerServiceNo: UtilityClass.getFormatedNumber(data.customerServiceNo),
      phone: UtilityClass.getFormatedNumber(data.phone),
      email: data.email,
      urls: data.Url,
      storeImage: imageURL,
      licenseNo: data.licenseNo,
      address: {
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
        zip: data.zip,
        addressTypeId: 0,
        latitude: getValues("lat&long").split(",")[0] || null,
        longitude: getValues("lat&long").split(",")[1] || null,
        storeAddressId: storeData.address.addressId,
      },
    };
    // console.log("body", body);
    setIsLoading(true);
    UpdateStore(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(!res?.data?.errors);
        } else {
          successNotification("Store Update successfully");
          getAllStores();
          handleClose();
        }
      })
      .catch((e) => {
        console.log("e", e);
        if (e.response?.data?.errors) {
          UtilityClass.showErrorNotificationWithDictionary(
            e.response?.data?.errors
          );
        } else {
          errorNotification("Somthing went wrong!!!");
        }
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  const getStoreById = () => {
    if (storeData) {
      GetStoreById(storeData?.storeId)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.errors);
          } else {
            setStoreData(res?.data?.result);
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Something went wrong");
        })
        .finally(() => {});
    }
  };
  const handleFocus = (event) => event.target.select();

  const label = { inputProps: { "aria-label": "Color switch demo" } };
  const handleDelete = async () => {
    try {
      let param = {
        storeId: storeData.storeId,
      };
      const response = await DeleteStoreById(param);
      if (response.data?.isSuccess) {
        getAllStores();
        getStoreById();
        setOpenDeleteConfirmation(false);
        successNotification("Store  activated successfully");
      } else {
        errorNotification("Store deactivated unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
    }
  };
  const [isActivating, setIsActivating] = useState(false);
  const handleActivateStoreById = async () => {
    try {
      let param = {
        storeId: storeData.storeId,
      };
      setIsActivating(true);
      const response = await ActivateStoreById(param);
      if (response.data?.isSuccess) {
        getAllStores();
        getStoreById();
        setOpenDeleteConfirmation(false);
        successNotification("Store Activated successfully");
      } else {
        errorNotification("Store Activated unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
      setIsActivating(false);
    }
  };
  const handleActiveInActive = (e) => {
    handelAction(!isActiveChecked);
    setIsActiveChecked(!isActiveChecked);
  };
  const handelAction = (isActiveClicked) => {
    if (isActiveClicked) {
      handleActivateStoreById();
    } else {
      handleDelete();
    }
  };
  useMapAutocompleteSetter(
    autocomplete,
    allCountries,
    allRegions,
    allCities,
    setValue
  );

  useEffect(() => {
    (async () => {
      setValue("customerServiceNo", null);
      setValue("phone", null);
      await handleSetSchemaValueForUpdate(storeData.address, setValue);
      setValue("customerServiceNo", storeData?.customerServiceNo);
      setValue("phone", storeData?.phone);
      let latitude = storeData?.address?.latitude;
      let longitude = storeData?.address?.longitude;
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
      GetAddressFromLatAndLong(body)
    );
    fetchMethodResponse(response, "Address Updated", () =>
      handleSetSchemaValueForUpdate(response.result, setValue)
    );
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={""}
      actionBtn={
        storeData?.active && (
          <ModalButtonComponent
            title={LanguageReducer?.languageType?.STORE_UPDATE_STORE}
            loading={isLoading}
            bg={purple}
            onClick={handleSubmit(createStore)}
          />
        )
      }
      component={"form"}
    >
      <Box sx={{ ...styleSheet.uploadStoreIconArea }}>
        <Button
          sx={{ mt: "8px", outlineColor: "white", mb: "7px" }}
          component="label"
        >
          <img
            style={{
              borderRadius: "20px",
              width: "100px",
              height: "100px",
            }}
            src={imageURL || uploadIcon}
            alt="uploadIcon"
          />
          <input
            onChange={uploadStoreImage}
            hidden
            accept=".jpg,.png,.jpeg"
            multiple={false}
            type="file"
            id="imageInput"
          />
        </Button>
      </Box>
      {/* <Box>
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.STORE_CODE_TEXT}
              </InputLabel>
              <TextField
                placeholder="SH 001"
                size="small"
                id="storeCode"
                name="storeCode"
                fullWidth
                variant="outlined"
                {...register("storeCode", {
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
                error={Boolean(errors.storeCode)} // set error prop
                helperText={errors.storeCode?.message}
              />
            </Box> */}
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_STORE_NAME}
          </InputLabel>
          <TextField
            disabled={!storeData?.active}
            placeholder={placeholders.store_name}
            size="small"
            id="storeName"
            name="storeName"
            fullWidth
            variant="outlined"
            {...register("storeName", {
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
            error={Boolean(errors.storeName)} // set error prop
            helperText={errors.storeName?.message}
          />
        </Grid>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_COMPANY_NAME}
          </InputLabel>
          <TextField
            disabled={!storeData?.active}
            placeholder={placeholders.company_name}
            size="small"
            id="companyName"
            name="companyName"
            fullWidth
            variant="outlined"
            {...register("companyName", {
              required: {
                value: false,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
            })}
            error={Boolean(errors.companyName)} // set error prop
            helperText={errors.companyName?.message}
          />
        </Grid>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_LICENSE_NUMBER}
          </InputLabel>
          <TextField
            disabled={!storeData?.active}
            placeholder={placeholders.license_num}
            size="small"
            id="licenseNo"
            name="licenseNo"
            fullWidth
            variant="outlined"
            {...register("licenseNo", {
              // required: {
              //   value: true,
              //   message:
              //     LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              // },
              pattern: {
                value: /^(?!\s*$).+/,
                message:
                  LanguageReducer?.languageType
                    ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
              },
            })}
            error={Boolean(errors.licenseNo)} // set error prop
            helperText={errors.licenseNo?.message}
          />
        </Grid>
        <Grid item sm={6} md={3} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_CUSTOMER_SERVICE_NO}
          </InputLabel>
          <CustomRHFPhoneInput
            disabled={!storeData?.active}
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            name="customerServiceNo"
            selectedCountry={getValues("country")?.mapCountryCode}
            control={control}
            required
            inputAdornment={
              <Box sx={{ position: "absolute", right: 27, top: 18 }}>
                <Tooltip title="This Number will reflect on Airway Bill">
                  <InputAdornment position="end">
                    <ErrorOutlineIcon
                      fontSize="small"
                      sx={{ color: "black", cursor: "pointer" }}
                    />
                  </InputAdornment>
                </Tooltip>
              </Box>
            }
          />
        </Grid>
        <Grid item sm={6} md={3} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_PHONE_NO}
          </InputLabel>
          <CustomRHFPhoneInput
            disabled={!storeData?.active}
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            selectedCountry={getValues("country")?.mapCountryCode}
            name="phone"
            control={control}
            isContact={true}
          />
        </Grid>
        <Grid item sm={6} md={3} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_EMAIL}
          </InputLabel>
          <TextField
            disabled={!storeData?.active}
            placeholder={placeholders.email}
            size="small"
            id="email"
            name="email"
            fullWidth
            variant="outlined"
            {...register("email", {
              required: {
                value: false,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
              // pattern: {
              //   value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i, // set pattern to match email format
              //   message: LanguageReducer?.languageType?.INVALID_EMAIL_TOAST,
              // },
            })}
            error={Boolean(errors.email)} // set error prop
            helperText={errors.email?.message}
          />
        </Grid>
        <Grid item sm={6} md={3} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_URL}
          </InputLabel>
          <TextField
            disabled={!storeData?.active}
            placeholder={placeholders.url}
            size="small"
            id="Url"
            name="Url"
            fullWidth
            variant="outlined"
            {...register("Url", {
              required: {
                value: false,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
              //   pattern: {
              //     value: /^(?!\s*$).+/,
              //     message:
              //       LanguageReducer?.languageType
              //         ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
              //   },
            })}
            error={Boolean(errors.Url)} // set error prop
            helperText={errors.Url?.message}
          />
        </Grid>
        <Grid item md={schemaFieldsLength === 0 ? 12 : 4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_COUNTRY}
          </InputLabel>
          <SelectComponent
            name="country"
            control={control}
            options={allCountries}
            isRHF={true}
            required={true}
            optionLabel={EnumOptions.COUNTRY.LABEL}
            optionValue={EnumOptions.COUNTRY.VALUE}
            {...register("country", {
              required: {
                value: true,
              },
            })}
            value={getValues("country")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              handleSetSchema("country", resolvedId, setValue, unregister);
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
        <GridContainer>
          <Grid item md={6} sm={6} xs={12}>
            <InputLabel sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.STORE_LATITUDE_AND_LONGITUDE}
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
              label={LanguageReducer?.languageType?.STORE_GET_ADDRESS}
              height={styleSheet.fromMapButton}
            />
            <ActionButtonCustom
              onClick={() => setOpenLocationModal(true)}
              startIcon={<FmdGoodOutlinedIcon />}
              label={LanguageReducer?.languageType?.STORE_FORM_MAP}
              height={styleSheet.fromMapButton}
            />
          </Grid>
        </GridContainer>
        {storeData && !storeData?.isDefault && (
          <Grid item md={3} sm={3} xs={12} alignSelf={"end"}>
            <FormControlLabel
              sx={{ marginTop: "15px" }}
              control={
                <Switch
                  defaultChecked={storeData?.active}
                  onClick={() => {
                    handleActiveInActive();
                  }}
                />
              }
              label={
                isActiveChecked
                  ? LanguageReducer?.languageType?.ACTIVE
                  : LanguageReducer?.languageType?.IN_ACTIVE
              }
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
      </Grid>
      {/* <ToastContainer /> */}
      <HeightBox length={schemaFieldsLength} />

      <GoogleMapWithSearch
        open={openLocationModal}
        setOpen={setOpenLocationModal}
        setValue={setValue}
        setAutocomplete={setAutocomplete}
        splitLatAndLong={splitLatAndLong}
      />

      {openDeleteConfirmation && (
        <DeleteConfirmationModal
          open={openDeleteConfirmation}
          setOpen={setOpenDeleteConfirmation}
          handleDelete={handleDelete}
        />
      )}
      <FromLinkLocationModal
        open={openFromLinkLocationModal}
        setOpen={setOpenFromLinkLocationModal}
        setValue={setValue}
      />
    </ModalComponent>
  );
}
export default EditStoreModal;
