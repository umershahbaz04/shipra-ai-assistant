import BorderColorIcon from "@mui/icons-material/BorderColor";
import {
  Avatar,
  Box,
  Grid,
  IconButton,
  InputLabel,
  TextField,
  Typography,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent.js";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal.js";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent.js";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput.js";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput.js";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent.js";
import {
  GetAddressFromLatAndLong,
  GetAllClientUserRole,
  GetAllCountry,
  GetAllEmployeeType,
  GetAllStores,
  GetEmployeeById,
  UpdateEmployee,
  UploadStoreImage,
} from "../../../api/AxiosInterceptors.js";
import avatarIcon from "../../../assets/images/avatar.png";
import { styleSheet } from "../../../assets/styles/style.js";
import UtilityClass from "../../../utilities/UtilityClass.js";
import { EnumOptions } from "../../../utilities/enum/index.js";
import FmdGoodOutlinedIcon from "@mui/icons-material/FmdGoodOutlined";
import {
  ActionButtonCustom,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  getOptionValueObjectByValue,
  GridContainer,
  placeholders,
  purple,
  useMapAutocompleteSetter,
} from "../../../utilities/helpers/Helpers.js";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import GoogleMapWithSearch from "../resuseAbleModals/GoogleMapWithSearch.js";
import CustomLatLongTextField from "../../../.reUseableComponents/TextField/CustomLatLongTextField .js";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function EditEmployeeModal(props) {
  let {
    open,
    setOpen,
    getAllEmployees,
    allGenderForSelection,
    getGenderForSelection,
    selectedRowData,
  } = props;
  const [values, setValues] = useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [allCountries, setAllCountries] = useState([]);
  const [selectedCountry, setSelectedCountry] = useState();
  const [allRegions, setAllRegions] = useState([]);
  const [selectedRegion, setSelectedRegion] = useState();
  const [allCities, setAllCities] = useState([]);
  const [selectedCity, setSelectedCity] = useState();
  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState("");
  const [isLoading, setIsLoading] = React.useState(false);
  const [userData, setUserData] = useState();
  const [allStores, setAllStores] = useState([]);
  const [clientUserRole, setClientUserRole] = useState([]);
  const [openLocationModal, setOpenLocationModal] = useState(false);
  const [employeeTypes, setEmployeeTypes] = useState([]);
  const [autocomplete, setAutocomplete] = useState(null);
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
    addressSchema,
    selectedAddressSchema,
    addressSchemaSelectData,
    addressSchemaInputData,
    handleSetSchema,
    handleChangeInputAddressSchema,
    handleChangeSelectAddressSchemaAndGetOptions,
    handleSetSchemaValueForUpdate,
  } = useGetAddressSchema();

  let defUserRole = clientUserRole?.find(
    (x) => x.clientUserRoleId == userData?.clientUserRoleId,
  );

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
  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    if (res.data.result != null) setAllCountries(res.data.result);
  };

  const getAllEmployeeType = async () => {
    try {
      const response = await GetAllEmployeeType();
      if (response?.data?.isSuccess) {
        setEmployeeTypes(response?.data?.result);
      }
    } catch (e) {}
  };

  useEffect(() => {
    getAllCountry();
    getAllEmployeeType();
  }, []);
  let uploadStoreImage = (e) => {
    setFile(e.target.files[0]);
    const formData = new FormData();
    formData.append("File", e.target.files[0]);
    UploadStoreImage(formData)
      .then((res) => {
        setImageURL(res.data.result.url);
        // successNotification(res.data.result.message);
      })
      .catch((e) => console.log("e", e));
  };
  const updateEmployee = async (data) => {
    let mobile = data.mobile;
    if (data.mobile) {
      mobile = "+" + UtilityClass.getFormatedNumber(data.mobile);
    }
    let mobile2 = data.phone;
    if (mobile2) {
      mobile2 = "+" + UtilityClass.getFormatedNumber(data.phone);
    }

    const body = {
      EmployeeId: selectedRowData?.EmployeeId,
      employeeName: data.employeeName,
      employeeImage: imageURL,
      workEmail: data.email,
      userName: data.userName,
      StoreId: data?.store?.StoreId,
      mobile: mobile,
      password: data.password,
      phone: mobile2,
      genderId: data.gender.id,
      userRoleId: data.userRole.clientUserRoleId,
      dateOfBirth: data.dateOfBirth,
      employeeTypeId: data?.employeeType?.employeeTypeId,
      address: {
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
        addressId: "",
      },
    };
    setIsLoading(true);

    UpdateEmployee(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Employee update successfully");
          getAllEmployees();
          handleClose();
        }
      })
      .catch((e) => {
        console.log("e", e);
        if (!e?.response?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors,
          );
        } else {
          errorNotification("Unable to update employee");
        }
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  useEffect(() => {
    if (userData) {
      let defgender = allGenderForSelection?.find(
        (x) => x.id == userData?.genderId,
      );
      let defEmployee = employeeTypes?.find(
        (x) => x.employeeTypeId == userData?.employeeTypeId,
      );
      setValue("email", userData?.workEmail);
      setValue("employeeType", defEmployee);
      setValue("employeeCode", userData?.employeeCode);
      setValue("gender", defgender);
      let dob = userData?.dateOfBirth && new Date(userData?.dateOfBirth);
      setValue("dateOfBirth", dob);
      setValue("mobile", userData?.mobileNo);
      setValue("phone", userData?.phoneNo);
      setValue("employeeName", userData?.employeeName);
      setValue("userName", userData?.employeeName);
      setImageURL(userData?.employeeImage);
    }
  }, [userData]);

  useEffect(() => {
    if (selectedRowData) {
      GetEmployeeById(selectedRowData.EmployeeId)
        .then(async (res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.errors);
          } else {
            const _data = res?.data?.result;

            const { response: userRoleResponse } = await fetchMethod(() =>
              GetAllClientUserRole({}),
            );
            const { response: storeResponse } = await fetchMethod(() =>
              GetAllStores({
                filterModel: {
                  createdFrom: null,
                  createdTo: null,
                  start: 0,
                  length: 10000,
                  search: "",
                  sortDir: "desc",
                  sortCol: 0,
                },
              }),
            );
            const _clientUserRole = userRoleResponse.result;
            const _allStores = storeResponse.result.list;
            setUserData(_data);
            if (userRoleResponse.isSuccess) {
              setClientUserRole(_clientUserRole);
            }
            if (storeResponse.isSuccess) {
              setAllStores(_allStores);
            }
            setValue(
              "userRole",
              getOptionValueObjectByValue(
                _clientUserRole,
                EnumOptions.USER_ROLE.VALUE,
                _data?.clientUserRoleId,
              ),
            );
            setValue(
              "store",
              getOptionValueObjectByValue(
                _allStores,
                EnumOptions.EMPLOYEE_STORE.VALUE,
                _data?.storeId,
              ),
            );
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Something went wrong");
        })
        .finally(() => {});
    }
  }, []);
  const [scroll, setScroll] = React.useState("paper");
  useMapAutocompleteSetter(
    autocomplete,
    allCountries,
    allRegions,
    allCities,
    setValue,
  );

  useEffect(() => {
    (async () => {
      setValue("mobile", null);
      setValue("phone", null);
      await handleSetSchemaValueForUpdate(userData?.address, setValue);
      setValue("mobile", userData?.mobileNo);
      setValue("phone", userData?.phoneNo);
      let latitude = userData?.address?.latitude;
      let longitude = userData?.address?.longitude;
      if (
        latitude !== null &&
        longitude !== null &&
        latitude !== undefined &&
        longitude !== undefined
      ) {
        setValue("lat&long", `${latitude},${longitude}`);
      }
    })();
  }, [userData]);

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

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.SETTING_USER_UPDATE_EMPLOYEE}
          loading={isLoading}
          bg={purple}
          onClick={handleSubmit(updateEmployee)}
        />
      }
      component={"form"}
    >
      <Box sx={styleSheet.addDriverHeadingAndUpload}>
        <Typography sx={styleSheet.addDriverHeading} variant="h4">
          {LanguageReducer?.languageType?.SETTING_USER_UPDATE_EMPLOYEE}
        </Typography>
        <Box sx={styleSheet.uploadIconArea}>
          <Avatar
            sx={{ width: "100%", height: "100%" }}
            src={
              file
                ? file && URL.createObjectURL(file)
                : imageURL
                  ? imageURL
                  : avatarIcon
            }
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
        {/* <Grid item md={6} sm={6} xs={12}>
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
              <Grid item md={6} sm={6} xs={12}>
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
            placeholder={LanguageReducer?.languageType?.SETTING_USER_FULL_NAME}
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
            isRefesh={true}
            handleRefreshClick={getGenderForSelection}
            {...register("gender", {
              required: {
                value: true,
              },
            })}
            value={getValues("gender")}
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
            control={control}
            defaultValue={new Date()}
            // onChange={handleOnChange}
            required
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
          />
        </Grid>
        <Grid item md={4} lg={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_EMAIL}
          </InputLabel>
          <TextField
            disabled
            placeholder={placeholders.email}
            type="email"
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
        <Grid item md={4} lg={4} sm={6} xs={12}>
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
        <Grid item md={4} lg={4} sm={6} xs={12}>
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
          <InputLabel required sx={styleSheet.inputLabel}>
            {"Employee Type"}
          </InputLabel>
          <SelectComponent
            name="employeeType"
            control={control}
            options={employeeTypes}
            disabled={defUserRole?.roleName === "Sale Person"}
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
            optionLabel={EnumOptions.USER_ROLE.LABEL}
            optionValue={EnumOptions.USER_ROLE.VALUE}
            disabled={getValues("userRole")?.roleName === "Sale Person" && true}
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
        <Grid item md={12} sm={12} xs={12}>
          <Typography sx={{ ...styleSheet.pageSubHeading }}>
            {" "}
            {LanguageReducer?.languageType?.SHIPPING_ADDRESS_TEXT}
          </Typography>
        </Grid>
        <Grid item md={4} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_USER_COUNTRY}
          </InputLabel>
          <SelectComponent
            name="country"
            // disabled={disabledInput}
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
              handleSetSchema("country", resolvedId, setValue, unregister);
            }}
            errors={errors}
          />
        </Grid>
        {[...addressSchemaSelectData, ...addressSchemaInputData].map(
          (input, index) => (
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
        {/* <Grid item sm={6} xs={12} sx={{ mt: "10px" }}>
                <Divider />
              </Grid>
              <Grid item sm={6} xs={12}>
                <Typography sx={styleSheet.driverLoginHeading} variant="h6">
                  {LanguageReducer?.languageType?.LOGIN_TEXT}
                </Typography>
              </Grid>
              <Grid item sm={6}>
                <InputLabel required sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.USER_NAME_TEXT}
                </InputLabel>
                <TextField
                  disabled
                  placeholder={placeholders.company_name}
                  fullWidth
                  variant="outlined"
                  size="small"
                  {...register("userName", {
                    required: {
                      value: true,
                      message:
                        LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                    },
                    pattern: {
                      value: /^(?! +$)[a-z0-9]+$/,
                      message:
                        LanguageReducer?.languageType
                          ?.INPUT_SHOULD_NOT_CONTAIN_SPACES_OR_UPPERCASE,
                    },
                  })}
                  error={Boolean(errors.userName)} // set error prop
                  helperText={errors.userName?.message}
                />
              </Grid>
              <Grid item sm={6}>
                <InputLabel required sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.PASSWORD_TEXT}
                </InputLabel>
                <FormControl fullWidth variant="outlined">
                  <OutlinedInput
                    disabled
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
                          {values.showPassword ? (
                            <VisibilityOff />
                          ) : (
                            <Visibility />
                          )}
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
                        message:
                          LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                      },
                    })}
                    error={Boolean(errors.password)} // set error prop
                    helperText={errors.password?.message}
                  />
                </FormControl>
              </Grid> */}
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
export default EditEmployeeModal;
