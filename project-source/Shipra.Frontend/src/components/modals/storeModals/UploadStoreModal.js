import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import {
  addressSchemaEnum,
  SchemaTextField,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import { getAllCountryFunc } from "../../../apiCallingFunction";
import { useForm, useWatch } from "react-hook-form";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import {
  GetAddressFromLatAndLong,
  UploadStoreImage,
} from "../../../api/AxiosInterceptors";
import { successNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import uploadIcon from "../../../assets/images/uploadIcon.png";
import { styleSheet } from "../../../assets/styles/style";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import {
  Box,
  Button,
  FormControl,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  TextField,
  Tooltip,
} from "@mui/material";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
import {
  ActionButtonCustom,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  GridContainer,
  HeightBox,
  ClipboardIcon,
  placeholders,
  purple,
} from "../../../utilities/helpers/Helpers";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import { EnumOptions, inputTypesEnum } from "../../../utilities/enum";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import TextFieldWithInfoInputAdornment from "../../../.reUseableComponents/TextField/TextFieldWithInputAdornment";

const UploadStoreModal = (props) => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { open, onClose, storeData, setUploadStoreData,allCountries } = props;
  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState();
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [autocomplete, setAutocomplete] = useState(null);
  const [values, setValues] = useState({});

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

  const handleClickShowPassword = () => {
    setValues({
      ...values,
      showPassword: !values.showPassword,
    });
  };

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  let uploadStoreImage = (e) => {
    const selectedFile = e.target.files[0];
    if (!selectedFile) return;

    const formData = new FormData();
    formData.append("File", selectedFile);

    UploadStoreImage(formData)
      .then((res) => {
        // console.log("res", res);
        setFile(selectedFile);
        setImageURL(res.data.result.url);
        successNotification(res.data.result.message);
      })
      .catch((err) => {
        UtilityClass.showErrorNotificationWithDictionary(
          err.response?.data?.errors,
        );
      })
      .finally(() => {
        e.target.value = null;
      });
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
      handleSetSchemaValueForUpdate(response.result, setValue),
    );
  };

  const createStore = async (data) => {
    setUploadStoreData((prev) =>
      prev.map((row) => {
        if (row.rowNum !== storeData.rowNum) return row;

        return {
          ...row,
          storeName: data.storeName,
          storeCompany: data.companyName || "",
          licenseNo: data.licenseNo,
          email: data.email || "",
          urls: data.Url || "",
          storeImage: imageURL ?? row.storeImage,
          userName: storeData?.userName || null,
          password: data.password || null,
          storeId: row.storeId,
          storeCode: row.storeCode,
          customerServiceNo: data.customerServiceNo,
          phone: data.phone,
          storeAddress: {
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
            country: data.country?.countryId || row.storeAddress?.country,
            city: data.city?.id || row.storeAddress?.city,
            area: data.area?.id || row.storeAddress?.area,
            province: row.storeAddress?.province || null,
            state: row.storeAddress?.state || null,
          },
          hasError: false,
          errorMsg: "",
        };
      }),
    );
    onClose();
  };

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
    }
  }, [storeData]);

  useEffect(() => {
    (async () => {
      setValue("customerServiceNo", null);
      setValue("phone", null);
      await handleSetSchemaValueForUpdate(storeData.storeAddress, setValue);
      setValue("customerServiceNo", storeData?.customerServiceNo);
      setValue("phone", storeData?.phone);
      let latitude = storeData?.storeAddress?.latitude;
      let longitude = storeData?.storeAddress?.longitude;
      if (latitude !== null && longitude !== null) {
        setValue("lat&long", `${latitude},${longitude}`);
      }
    })();
  }, [storeData]);
  
  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="lg"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.STORE_UPDATE_STORE}
          bg={purple}
          onClick={handleSubmit(createStore)}
        />
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
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_STORE_NAME}
          </InputLabel>
          <TextField
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
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            selectedCountry={getValues("country")?.mapCountryCode}
            name="phone"
            control={control}
            isContact={true}
          />
        </Grid>
        <Grid item sm={6} md={3} xs={12}>
          <InputLabel
            sx={styleSheet.inputLabel}
            required={!!storeData.userName}
          >
            {LanguageReducer?.languageType?.STORE_EMAIL}
          </InputLabel>
          <TextField
            placeholder={placeholders.email}
            size="small"
            id="email"
            name="email"
            fullWidth
            variant="outlined"
            {...register("email", {
              required: storeData.userName
                ? {
                    value: true,
                    message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                  }
                : false,
              pattern: storeData.userName
                ? {
                    value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                    message: LanguageReducer?.languageType?.INVALID_EMAIL_TOAST,
                  }
                : undefined,
            })}
            error={Boolean(errors.email)}
            helperText={errors.email?.message}
          />
        </Grid>
        <Grid item sm={6} md={3} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.STORE_URL}
          </InputLabel>
          <TextField
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
        {storeData.userName && (
          <GridContainer>
            <Grid item md={4} sm={6} xs={12}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.SETTING_USER_DOB}
              </InputLabel>
              <CustomRHFReactDatePickerInput
                name="dateOfBirth"
                defaultValue={new Date()}
                control={control}
                required={storeData.userName ? true : false}
                error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
              />
            </Grid>
            <Grid item md={4} sm={6} xs={12}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.SETTING_USER_USERNAME}
              </InputLabel>
              <TextFieldWithInfoInputAdornment
                value={storeData?.userName}
                disabled
                size="small"
                infoText={LanguageReducer?.languageType?.USERNAME_PREFIX_TEXT}
                fullWidth
                inputAdornment={
                  <InputAdornment position="end">
                    <ClipboardIcon text={storeData?.userName} />
                  </InputAdornment>
                }
              />
            </Grid>
            <Grid item md={4} sm={6} xs={12}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.SETTING_USER_PASSWORD}
              </InputLabel>
              <FormControl
                fullWidth
                variant="outlined"
                error={Boolean(errors.password)}
              >
                <OutlinedInput
                  placeholder="●●●●●●●●●"
                  type={values.showPassword ? "text" : "password"}
                  value={values.password}
                  onChange={(e) =>
                    setValues({ ...values, password: e.target.value })
                  }
                  endAdornment={
                    <InputAdornment position="end">
                      <IconButton
                        aria-label="toggle password visibility"
                        onClick={handleClickShowPassword}
                        onMouseDown={handleMouseDownPassword}
                        edge="end"
                      >
                        {values.showPassword ? (
                          <VisibilityOff />
                        ) : (
                          <Visibility />
                        )}
                      </IconButton>
                    </InputAdornment>
                  }
                  size="small"
                  id="password"
                  {...register("password", {
                    required: {
                      value: storeData.userName ? true : false,
                      message:
                        LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                    },
                    pattern: {
                      value:
                        /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()\-=_+[\]{}|;':",./<>?]).{8,}$/,
                      message:
                        LanguageReducer?.languageType
                          ?.PASSWORD_MUST_CONTAIN_MINIMUM_8_CHARACTERS_1_UPPERCASE_1_LOWERCASE_1_NUMBER_1_SPECIAL_CHARACTER,
                    },
                  })}
                />
                {errors.password && (
                  <FormHelperText>{errors.password.message}</FormHelperText>
                )}
              </FormControl>
            </Grid>
          </GridContainer>
        )}
      </Grid>
      <HeightBox length={schemaFieldsLength} />
      <GoogleMapWithSearch
        open={openLocationModal}
        setOpen={setOpenLocationModal}
        setValue={setValue}
        setAutocomplete={setAutocomplete}
        splitLatAndLong={splitLatAndLong}
      />
    </ModalComponent>
  );
};

export default UploadStoreModal;
