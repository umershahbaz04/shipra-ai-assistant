import BorderColorIcon from "@mui/icons-material/BorderColor";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import {
  Avatar,
  Box,
  Button,
  Divider,
  FormControl,
  FormControlLabel,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useRef, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent.js";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent.js";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput.js";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput.js";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent.js";
import TextFieldWithInfoInputAdornment from "../../../.reUseableComponents/TextField/TextFieldWithInputAdornment.js";
import {
  CreateEmployee,
  GetAddressFromLatAndLong,
  GetAllClientUserRole,
  GetAllCountry,
  GetAllEmployeeType,
  GetAllStores,
  GetNextEmployeeUserName,
  UploadStoreImage,
} from "../../../api/AxiosInterceptors.js";
import avatarIcon from "../../../assets/images/avatar.png";
import { styleSheet } from "../../../assets/styles/style.js";
import UtilityClass from "../../../utilities/UtilityClass.js";
import { EnumOptions } from "../../../utilities/enum/index.js";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import {
  ActionButtonCustom,
  ClipboardIcon,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  GridContainer,
  placeholders,
  purple,
  useMapAutocompleteSetter,
  useUserNameWithPrefix,
} from "../../../utilities/helpers/Helpers.js";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch.js";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField .js";
import WhatsappSmsModal from "./WhatsappSmsModal.js";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function AddEmployeeModal(props) {
  let {
    open,
    setOpen,
    getAllEmployees,
    allGenderForSelection,
    getGenderForSelection,
    setdataForWhatsappSms,
    setOpenWhatsappSmsModal,
  } = props;
  const hasFetched = useRef(false);
  const [values, setValues] = useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [allCountries, setAllCountries] = useState([]);
  const [allRegions, setAllRegions] = useState([]);
  const [allCities, setAllCities] = useState([]);
  const [allStores, setAllStores] = useState([]);
  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState("");
  const [isLoading, setIsLoading] = React.useState(false);
  const [isPreverifyEmail, setIsPreverifyEmail] = useState(false);
  const [clientUserRole, setClientUserRole] = useState([]);
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [autocomplete, setAutocomplete] = useState(null);
  const [userNameLoading, setUserNameLoading] = useState(false);
  const [userNamePrefix, setUserNamePrefix] = useState("");
  const [employeeTypes, setEmployeeTypes] = useState([]);

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

  const handleClose = () => {
    // reset();
    setOpen(false);
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
    name: "gender",
    control,
  });
  useWatch({
    name: "userRole",
    control,
  });
  useWatch({
    name: "employeeType",
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
  useWatch({
    name: "employeeName",
    control,
  });
  let getAllStores = async () => {
    let res = await GetAllStores({
      filterModel: {
        createdFrom: null,
        createdTo: null,
        start: 0,
        length: 10000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
    });
    if (res.data.result !== null) setAllStores(res.data.result.list);
  };
  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    if (res.data.result != null) setAllCountries(res.data.result);
  };
  let getGetAllClientUserRole = async () => {
    let res = await GetAllClientUserRole({});
    if (res.data.result != null) setClientUserRole(res.data.result);
  };

  const getAllEmployeeType = async () => {
    try {
      const response = await GetAllEmployeeType();
      if (response?.data?.isSuccess) {
        setEmployeeTypes(response?.data?.result);
      }
    } catch (e) {}
  };

  const getNextEmployeeUserName = async () => {
    setUserNameLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetNextEmployeeUserName(getValues("employeeName")),
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
  useEffect(() => {
    getAllCountry();
    getGetAllClientUserRole();
    getAllStores();
    getAllEmployeeType();
  }, []);
  let uploadStoreImage = (e) => {
    setFile(e.target.files[0]);
    const formData = new FormData();
    formData.append("File", e.target.files[0]);
    UploadStoreImage(formData)
      .then((res) => {
        setImageURL(res.data.result.url);
      })
      .catch((e) => console.log("e", e));
  };
  const createEmployee = async (data) => {
    console.log(data);
    let mobile = data.mobile;
    if (data.mobile) {
      mobile = "+" + UtilityClass.getFormatedNumber(data.mobile);
    }
    let mobile2 = data.phone;
    if (mobile2) {
      mobile2 = "+" + UtilityClass.getFormatedNumber(data.phone);
    }

    const body = {
      employeeName: data.employeeName,
      genderId: data.gender.id,
      dateOfBirth: data.dateOfBirth,
      employeeImage: imageURL,
      userName: userNamePrefix,
      StoreId: data?.store?.StoreId,
      mobile: mobile,
      workEmail: data.email,
      password: data.password,
      phone: mobile2,
      isPreverifyEmail: isPreverifyEmail,
      userRoleId: data.userRole.clientUserRoleId,
      employeeTypeId: data?.employeeType?.employeeTypeId,
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
    console.log("body::", body);
    setdataForWhatsappSms(body);
    setIsLoading(true);

    CreateEmployee(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Employee created successfully");
          getAllEmployees();
          handleClose();
          setOpenWhatsappSmsModal(true);
        }
      })
      .catch((e) => {
        if (!e?.response?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors,
          );
        } else {
          errorNotification("Unable to create employee");
        }
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  useMapAutocompleteSetter(
    autocomplete,
    allCountries,
    allRegions,
    allCities,
    setValue,
  );

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
    const employeeName = getValues("employeeName");
    if (employeeName?.length < 3) {
      hasFetched.current = false;
    }

    if (employeeName?.length >= 3 && !hasFetched.current) {
      getNextEmployeeUserName();
      hasFetched.current = true;
    }
  }, [getValues("employeeName")]);

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.SETTING_USER_ADD_EMPLOYEE}
          loading={isLoading}
          bg={purple}
          onClick={handleSubmit(createEmployee)}
        />
      }
      component={"form"}
    >
      <Box sx={styleSheet.addDriverHeadingAndUpload}>
        <Typography sx={styleSheet.addDriverHeading} variant="h4">
          {"Add User"}
        </Typography>
        <Box sx={styleSheet.uploadIconArea}>
          <Avatar
            sx={{ width: "100%", height: "100%" }}
            src={file ? file && URL.createObjectURL(file) : avatarIcon}
          ></Avatar>
          <IconButton sx={styleSheet.editProfileIcon} component="label">
            <BorderColorIcon sx={{ width: "14px", height: "14px" }} />

            <input
              hidden
              accept="image/*"
              multiple
              type="file"
              onChange={uploadStoreImage}
            />
          </IconButton>
        </Box>
      </Box>
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        {/* <Grid item md={6} sm={12}>
                <InputLabel sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.EMPLOYEE_CODE_TEXT}
                </InputLabel>
                <TextField
                  placeholder="SP00001"
                  size="small"
                  fullWidth
                  variant="outlined"
                  name="employeeCode"
                  {...register("employeeCode")}
                />
              </Grid>
              <Grid item md={6} sm={12}>
                <InputLabel sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.STATUS_TEXT}
                </InputLabel>
                <TextField
                  defaultValue={"Active"}
                  select
                  size="small"
                  fullWidth
                  variant="outlined"
                >
                  <MenuItem value="Active">Active</MenuItem>
                  <MenuItem value="Pending">Pending</MenuItem>
                  <MenuItem value="Completed">Completed</MenuItem>
                  <MenuItem value="Expire">Expire</MenuItem>
                  <MenuItem value="Closed">Closed</MenuItem>
                </TextField>
              </Grid> */}
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_FULL_NAME}
          </InputLabel>
          <TextField
            placeholder={placeholders.name}
            size="small"
            fullWidth
            variant="outlined"
            name="employeeName"
            {...register("employeeName", {
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
              minLength: {
                value: 3,
                message: "Name must be at least 3 characters long.",
              },
            })}
            error={Boolean(errors.employeeName)} // set error prop
            helperText={errors.employeeName?.message}
          />
        </Grid>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_GENDER}
          </InputLabel>
          <SelectComponent
            name="gender"
            control={control}
            options={allGenderForSelection}
            isRHF={true}
            required={true}
            optionLabel={EnumOptions.GENDER.LABEL}
            optionValue={EnumOptions.GENDER.VALUE}
            {...register("gender", {
              required: {
                value: true,
              },
            })}
            value={getValues("gender")}
            isRefesh={true}
            handleRefreshClick={getGenderForSelection}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("gender", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_DOB}
          </InputLabel>
          <CustomRHFReactDatePickerInput
            name="dateOfBirth"
            defaultValue={new Date()}
            control={control}
            // onChange={handleOnChange}
            required
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
          />
        </Grid>
        <Grid item md={4} lg={4} sm={6} xs={12}>
          <Box position={"relative"}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.SETTING_USER_EMAIL}
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
          <TextField
            placeholder={placeholders.email}
            type="text"
            size="small"
            fullWidth
            variant="outlined"
            name="email"
            {...register("email", {
              required: {
                value: true,
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
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_MOBILE}
          </InputLabel>
          <CustomRHFPhoneInput
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            name="mobile"
            selectedCountry={getValues("country")?.mapCountryCode}
            control={control}
            required
          />
        </Grid>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_PHONE_NO}
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
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.SETTING_USER_COUNTRY}
            </InputLabel>
          </Box>
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
              {LanguageReducer?.languageType?.SETTING_LATITUDE_AND_LONGITUDE}
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
              label={LanguageReducer?.languageType?.SETTING_GET_ADDRESS}
              height={styleSheet.fromMapButton}
            />
            <ActionButtonCustom
              onClick={() => setOpenLocationModal(true)}
              startIcon={<FmdGoodOutlinedIcon />}
              label={LanguageReducer?.languageType?.SETTING_FORM_MAP}
              height={styleSheet.fromMapButton}
            />
          </Grid>
        </GridContainer>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {"Employee Type"}
          </InputLabel>
          <SelectComponent
            name="employeeType"
            control={control}
            options={employeeTypes}
            isRHF={true}
            required={true}
            isRefesh={true}
            handleRefreshClick={getAllEmployeeType}
            optionLabel={EnumOptions.EMPLOYEE_TYPE.LABEL}
            optionValue={EnumOptions.EMPLOYEE_TYPE.VALUE}
            {...register("employeeType", {
              required: {
                value: true,
              },
            })}
            value={getValues("employeeType")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("employeeType", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_USER_ROLE}
          </InputLabel>
          <SelectComponent
            name="userRole"
            control={control}
            options={clientUserRole}
            isRHF={true}
            required={true}
            isRefesh={true}
            handleRefreshClick={getGetAllClientUserRole}
            optionLabel={EnumOptions.USER_ROLE.LABEL}
            optionValue={EnumOptions.USER_ROLE.VALUE}
            {...register("userRole", {
              required: {
                value: true,
              },
            })}
            value={getValues("userRole")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("userRole", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        {getValues("userRole")?.roleName === "Sale Person" ? (
          <Grid item md={4} sm={6} xs={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.SETTING_STORE}
            </InputLabel>
            <SelectComponent
              name="store"
              control={control}
              options={allStores}
              isRHF={true}
              required={true}
              isRefesh={true}
              handleRefreshClick={getAllStores}
              optionLabel={EnumOptions.EMPLOYEE_STORE.LABEL}
              optionValue={EnumOptions.EMPLOYEE_STORE.VALUE}
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
        ) : (
          ""
        )}
        <Grid item sm={12} sx={{ mt: "10px" }}>
          <Divider />
        </Grid>
        <Grid item sm={12} xs={12}>
          <Typography sx={styleSheet.driverLoginHeading} variant="h6">
            {LanguageReducer?.languageType?.LOGIN_TEXT}
          </Typography>
        </Grid>
        <Grid item sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_USERNAME}
          </InputLabel>
          <TextFieldWithInfoInputAdornment
            value={userNameLoading ? placeholders.loading : userNamePrefix}
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
        <Grid item sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_PASSWORD}
          </InputLabel>
          <FormControl fullWidth variant="outlined">
            <OutlinedInput
              placeholder={"●●●●●●●●●"}
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
                    {values.showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              }
              size="small"
              fullWidth
              id="password"
              name="password"
              {...register("password", {
                required: {
                  value: true,
                  message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                },
              })}
              error={Boolean(errors.password)} // set error prop
              helperText={errors.password?.message}
            />
          </FormControl>
        </Grid>
      </Grid>
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
export default AddEmployeeModal;
