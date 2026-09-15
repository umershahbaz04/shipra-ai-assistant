import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import {
  Box,
  Button,
  Checkbox,
  FormControl,
  FormControlLabel,
  FormGroup,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  Switch,
  TextField,
} from "@mui/material";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import Tooltip from "@mui/material/Tooltip";
import React, { useEffect, useRef, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateEmployee,
  CreateStore,
  GetAddressFromLatAndLong,
  GetNextEmployeeUserName,
  UploadStoreImage,
} from "../../../api/AxiosInterceptors";
import { getAllCountryFunc } from "../../../apiCallingFunction";
import uploadIcon from "../../../assets/images/uploadIcon.png";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumEmployeeType, EnumOptions } from "../../../utilities/enum";
import {
  ActionButtonCustom,
  ClipboardIcon,
  GridContainer,
  HeightBox,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  placeholders,
  purple,
  useGetAllClientUserRole,
  useMapAutocompleteSetter,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import FromLinkLocationModal from "../orderModals/FromLinkLocationModal";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField ";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import TextFieldWithInfoInputAdornment from "../../../.reUseableComponents/TextField/TextFieldWithInputAdornment";

function AddStoreModal(props) {
  let { open, setOpen, getAllStores } = props;
  const hasFetched = useRef(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [openFromLinkLocationModal, setOpenFromLinkLocationModal] =
    useState(false);
  const [isLoading, setIsLoading] = React.useState(false);

  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState();
  const [allCountries, setAllCountries] = useState([]);
  const [autocomplete, setAutocomplete] = useState(null);
  const [isSalesPersonCreated, setIsSalesPersonCreated] = useState(false);
  const [values, setValues] = useState({});
  const [userNameLoading, setUserNameLoading] = useState(false);
  const [userNamePrefix, setUserNamePrefix] = useState("");
  const [isPreverifyEmail, setIsPreverifyEmail] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    reset,
    control,
    unregister,
  } = useForm();

  console.log(errors);

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

  const schemaFieldsLength = [
    ...addressSchemaSelectData,
    ...addressSchemaInputData,
  ].length;

  useWatch({
    name: "country",
    control,
  });
  useWatch({
    name: "lat&long",
    control,
  });
  useWatch({
    name: "storeName",
    control,
  });
  const handleClose = () => {
    reset();
    setFile();
    setOpen(false);
  };

  const { clientUserRole } = useGetAllClientUserRole();
  const salePersonId = clientUserRole.find(
    (r) => r.roleName === "Sale Person",
  )?.clientUserRoleId;
  console.log(clientUserRole);

  const getNextEmployeeUserName = async () => {
    setUserNameLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetNextEmployeeUserName(getValues("storeName")),
      );
      if (response.isSuccess) {
        setUserNamePrefix(response.result.data);
      }
    } catch (e) {
      console.log(e);
    } finally {
      setUserNameLoading(false);
    }
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
          err.response?.data?.errors,
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

  const handleClickShowPassword = () => {
    setValues({
      ...values,
      showPassword: !values.showPassword,
    });
  };

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  useEffect(() => {
    getAllCountry();
  }, []);

  const createStore = async (data) => {
    const body = {
      storeName: data.storeName,
      storeCompany: data.companyName,
      customerServiceNo: UtilityClass.getFormatedNumber(data.customerServiceNo),
      phone: UtilityClass.getFormatedNumber(data.phone),
      email: data.email,
      urls: data.Url,
      licenseNo: data.licenseNo,
      storeImage: imageURL,
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
      },
    };
    setIsLoading(true);
    CreateStore(body)
      .then((res) => {
        // console.log("res:", res);
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
        } else {
          const salePersonBody = {
            employeeName: data.storeName,
            genderId: 1,
            dateOfBirth: data.dateOfBirth,
            employeeImage: imageURL,
            userName: userNamePrefix,
            StoreId: res?.data?.result?.data,
            mobile: UtilityClass.getFormatedNumber(data.customerServiceNo),
            workEmail: data.email,
            password: data.password,
            phone: UtilityClass.getFormatedNumber(data.phone),
            isPreverifyEmail: isPreverifyEmail,
            userRoleId: salePersonId,
            employeeTypeId: EnumEmployeeType.SHIPPER,
            employeeAddress: {
              countryId: selectedAddressSchema.country,
              cityId: selectedAddressSchema.city,
              areaId: selectedAddressSchema.area,
              streetAddress: selectedAddressSchema.streetAddress,
              streetAddress2: selectedAddressSchema.streetAddress2,
              houseNo: null,
              buildingName: null,
              landmark: null,
              provinceId: selectedAddressSchema.province,
              pinCodeId: selectedAddressSchema.pinCode,
              stateId: selectedAddressSchema.state,
              zip: data.zip ? data.zip : "",
              addressTypeId: 0,
              latitude: getValues("lat&long").split(",")[0] || null,
              longitude: getValues("lat&long").split(",")[1] || null,
            },
          };
          if (isSalesPersonCreated) {
            CreateEmployee(salePersonBody)
              .then((res) => {
                if (!res?.data?.isSuccess) {
                  UtilityClass.showErrorNotificationWithDictionary(
                    res.data.errors,
                  );
                } else {
                  successNotification("Employee created successfully");
                  getAllStores();
                }
              })
              .catch((e) => {
                console.error("Error:", e);
              });
          }
          successNotification(
            LanguageReducer?.languageType?.STORE_CREATED_SUCCESSFULLY_TOAST,
          );
          setImageURL();
          handleClose();
        }
      })
      .catch((e) => {
        errorNotification(
          LanguageReducer?.languageType?.UNABLE_TO_CREATE_STORE_TOAST,
        );
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  const handleFocus = (event) => event.target.select();
  useMapAutocompleteSetter(autocomplete, setValue);

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
    if (isSalesPersonCreated) {
      const employeeName = getValues("storeName");
      if (employeeName?.length < 3) {
        hasFetched.current = false;
      }

      if (employeeName?.length >= 3 && !hasFetched.current) {
        getNextEmployeeUserName();
        hasFetched.current = true;
      }
    }
  }, [getValues("storeName"), isSalesPersonCreated]);

  return (
    <Box borderRadius={"10px"}>
      <ModalComponent
        open={open}
        onClose={handleClose}
        maxWidth="lg"
        title={""}
        actionBtn={
          <ModalButtonComponent
            title={LanguageReducer?.languageType?.STORE_ADD}
            loading={isLoading}
            bg={purple}
            type="submit"
            onClick={handleSubmit(createStore)}
          />
        }
        component={"form"}
      >
        <Box sx={styleSheet.uploadStoreIconArea}>
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
              // {...register("companyName", {
              // required: {
              //   value: true,
              //   message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              // },
              // pattern: {
              //   value: /^(?!\s*$).+/,
              //   message:
              //     LanguageReducer?.languageType
              //       ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
              // },
              // })}
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
                //   value: false,
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
              name="phone"
              selectedCountry={getValues("country")?.mapCountryCode}
              control={control}
              isContact={true}
            />
          </Grid>
          <Grid item sm={6} md={3} xs={12}>
            {isSalesPersonCreated ? (
              <Box position={"relative"}>
                <InputLabel
                  sx={styleSheet.inputLabel}
                  required={isSalesPersonCreated}
                >
                  {LanguageReducer?.languageType?.STORE_EMAIL}
                </InputLabel>
                <FormControlLabel
                  sx={{ position: "absolute", top: -4, right: -12 }}
                  labelPlacement="left"
                  control={
                    <Switch
                      size="small"
                      value={isPreverifyEmail}
                      checked={isPreverifyEmail}
                      onChange={(e) => {
                        setIsPreverifyEmail(e.target.checked);
                      }}
                    />
                  }
                  componentsProps={{
                    typography: {
                      fontSize: 12,
                      fontWeight: 400,
                      color: "#000000",
                    },
                  }}
                  label={
                    LanguageReducer?.languageType?.SETTING_USER_CONFIRM_ACCOUNT
                  }
                />
              </Box>
            ) : (
              <InputLabel
                sx={styleSheet.inputLabel}
                required={isSalesPersonCreated}
              >
                {LanguageReducer?.languageType?.STORE_EMAIL}
              </InputLabel>
            )}
            <TextField
              placeholder={placeholders.email}
              size="small"
              id="email"
              name="email"
              fullWidth
              variant="outlined"
              {...register("email", {
                required: {
                  value: isSalesPersonCreated,
                  message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                },
                pattern: {
                  value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i, // set pattern to match email format
                  message: LanguageReducer?.languageType?.INVALID_EMAIL_TOAST,
                },
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
              placeholder={placeholders.url}
              size="small"
              id="Url"
              name="Url"
              fullWidth
              variant="outlined"
              // {...register("Url", {
              // required: {
              //   value: true,
              //   message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              // },
              // pattern: {
              //   value: /^(?!\s*$).+/,
              //   message:
              //     LanguageReducer?.languageType
              //       ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
              // },
              // })}
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
                handleSetSchema("country", resolvedId, setValue, unregister);
              }}
              errors={errors}
            />
          </Grid>
          {console.log([...addressSchemaSelectData, ...addressSchemaInputData])}
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
              <FormGroup>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={isSalesPersonCreated}
                      onChange={(e) =>
                        setIsSalesPersonCreated(e.target.checked)
                      }
                    />
                  }
                  label="Create SalesPerson"
                />
              </FormGroup>
            </Grid>
          </GridContainer>
          {isSalesPersonCreated && (
            <GridContainer>
              <Grid item md={4} sm={6} xs={12}>
                <InputLabel required sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.SETTING_USER_DOB}
                </InputLabel>
                <CustomRHFReactDatePickerInput
                  name="dateOfBirth"
                  defaultValue={new Date()}
                  control={control}
                  required
                  error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
                />
              </Grid>
              <Grid item md={4} sm={6} xs={12}>
                <InputLabel required sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.SETTING_USER_USERNAME}
                </InputLabel>
                <TextFieldWithInfoInputAdornment
                  value={
                    userNameLoading ? placeholders.loading : userNamePrefix
                  }
                  disabled
                  size="small"
                  infoText={LanguageReducer?.languageType?.USERNAME_PREFIX_TEXT}
                  fullWidth
                  inputAdornment={
                    <InputAdornment position="end">
                      <ClipboardIcon text={userNamePrefix} />
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
                        value: true,
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
        <HeightBox length={schemaFieldsLength} />
        {/* <ToastContainer /> */}
        <GoogleMapWithSearch
          open={openLocationModal}
          setOpen={setOpenLocationModal}
          setValue={setValue}
          setAutocomplete={setAutocomplete}
          splitLatAndLong={splitLatAndLong}
        />
        <FromLinkLocationModal
          open={openFromLinkLocationModal}
          setOpen={setOpenFromLinkLocationModal}
          setValue={setValue}
        />
      </ModalComponent>
    </Box>
  );
}
export default AddStoreModal;
