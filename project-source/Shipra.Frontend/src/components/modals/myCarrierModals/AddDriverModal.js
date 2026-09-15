import BorderColorIcon from "@mui/icons-material/BorderColor";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import {
  Avatar,
  Box,
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
  Typography
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import avatarIcon from "../../../assets/images/avatar.png";
import { styleSheet } from "../../../assets/styles/style";

import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import CountrySchema from "../../../utilities/helpers/countryschema";
import TextFieldWithInfoInputAdornment from "../../../.reUseableComponents/TextField/TextFieldWithInputAdornment";
import {
  CreateDriver,
  GetAllCountry,
  GetAllRegionbyCountryId,
  GetCityByRegionId,
  UploadStoreImage,
} from "../../../api/AxiosInterceptors";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import {
  placeholders,
  purple,
  useUserNameWithPrefix
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function AddDriverModal(props) {
  let { open, setOpen, getAllDrivers, allGenderForSelection } = props;
  const [values, setValues] = useState({});
  const [allCountries, setAllCountries] = useState([]);
  const [selectedCountry, setSelectedCountry] = useState();
  const [allRegions, setAllRegions] = useState([]);
  const [selectedRegion, setSelectedRegion] = useState();
  const [allCities, setAllCities] = useState([]);
  const [selectedCity, setSelectedCity] = useState();
  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState("");
  const [isPreverifyEmail, setIsPreverifyEmail] = useState(false);
  const [isLoading, setIsLoading] = React.useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const {
    loading: userNameLoading,
    userNamePrefix,
    handleUserNamePrefix,
  } = useUserNameWithPrefix();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    reset,
    control,
  } = useForm();

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
  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    console.log("res", res.data.result);
    if (res.data.result != null) setAllCountries(res.data.result);
  };
  let getAllRegionbyCountryId = async () => {
    let res = await GetAllRegionbyCountryId(getValues("country").countryId);
    if (res.data.result != null) setAllRegions(res.data.result);
  };
  let getCityByRegionId = async () => {
    let res = await GetCityByRegionId(getValues("region").regionId);
    if (res.data.result != null) setAllCities(res.data.result);
  };
  useEffect(() => {
    getAllCountry();
  }, []);
  useEffect(() => {
    // setValue("region", null);
    // setValue("city", null);
    setAllRegions([]);
    setAllCities([]);
    if (getValues("country")) getAllRegionbyCountryId();
  }, [getValues("country")]);
  useEffect(() => {
    // setValue("city", null);
    setAllCities([]);
    if (getValues("region")) getCityByRegionId();
  }, [getValues("region")]);
  let uploadStoreImage = (e) => {
    console.log("e", e.target.files[0]);
    setFile(e.target.files[0]);
    const formData = new FormData();
    formData.append("File", e.target.files[0]);
    UploadStoreImage(formData)
      .then((res) => {
        // console.log("res", res);
        setImageURL(res.data.result.url);
        // successNotification(res.data.result.message);
      })
      .catch((e) => console.log("e", e));
  };
  const createEmployee = async (data) => {
    console.log("vals::", data);
    const body = {
      employeeImage: imageURL,
      employeeName: data.employeeName,
      countryId: data.country.countryId,
      regionId: data.region.regionId,
      cityId: data.city.cityId,
      workEmail: data.email,
      userName: userNamePrefix,
      mobile: UtilityClass.getFormatedNumber(data.mobile),
      addressLine1: data.addressLine1,
      addressLine2: data.addressLine2,
      zip: data.zip ? data.zip : "",
      password: data.password,
      latitude: 0,
      longitude: 0,
      phone: UtilityClass.getFormatedNumber(data.phone),
      genderId: data.gender.id,
      dateOfBirth: data.dateOfBirth,
      isPreverifyEmail: isPreverifyEmail,
    };
    console.log("body::", body);
    setIsLoading(true);

    CreateDriver(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Driver created successfully");
          getAllDrivers();
          handleClose();
        }
      })
      .catch((e) => {
        console.log("e", e);
        if (!e?.response?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors
          );
        } else {
          errorNotification("Driver to create employee");
        }
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={"Add Driver"}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(createEmployee)}
    >
      <Box sx={styleSheet.addDriverHeadingAndUpload}>
        <Typography sx={styleSheet.addDriverHeading} variant="h4">
          {"Add Driver"}
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
              </Grid> */}
        {/* <Grid item md={6} sm={12}>
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
        <Grid item md={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.FULL_NAME}
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
            })}
            error={Boolean(errors.employeeName)} // set error prop
            helperText={errors.employeeName?.message}
          />
        </Grid>
        <Grid item md={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.GENDER_NAME_TEXT}
          </InputLabel>
          <SelectComponent
            name="gender"
            control={control}
            options={allGenderForSelection}
            value={getValues("gender")}
            optionLabel={EnumOptions.GENDER.LABEL}
            optionValue={EnumOptions.GENDER.VALUE}
            isRHF={true}
            required={true}
            {...register("gender", {
              required: {
                value: true,
              },
            })}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("gender", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        <Grid item md={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            DOB
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
        <Grid item md={4} lg={4} sm={12}>
          <Box position={"relative"}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.EMAIL_TEXT}
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
              label="Confirm Account"
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
        <Grid item md={4} lg={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            Mobile
          </InputLabel>
          <CustomRHFPhoneInput
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            name="mobile"
            control={control}
            required
          />
        </Grid>
        <Grid item md={4} lg={4} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>Other Mobile</InputLabel>
          <CustomRHFPhoneInput
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            name="phone"
            control={control}
          />
        </Grid>
        <Grid item md={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SELECT_COUNTRY_TEXT}
          </InputLabel>
          <CountrySchema
            name="country"
            control={control}
            value={getValues("country")}
            isRHF={true}
            required={true}
            {...register("country", {
              required: {
                value: true,
              },
            })}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("country", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        <Grid item sm={6}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {"Address line 1"}
          </InputLabel>
          <TextField
            placeholder={placeholders.apartment}
            type="text"
            size="small"
            fullWidth
            variant="outlined"
            name="addressLine1"
            {...register("addressLine1")}
          />
        </Grid>
        <Grid item sm={6}>
          <InputLabel sx={styleSheet.inputLabel}>{"Address line 2"}</InputLabel>
          <TextField
            placeholder={placeholders.apartment}
            type="text"
            size="small"
            fullWidth
            variant="outlined"
            name="addressLine2"
            {...register("addressLine2")}
          />
        </Grid>
        <Grid item sm={12} sx={{ mt: "10px" }}>
          <Divider />
        </Grid>
        <Grid item sm={12}>
          <Typography sx={styleSheet.driverLoginHeading} variant="h6">
            {LanguageReducer?.languageType?.LOGIN_TEXT}
          </Typography>
        </Grid>

        <Grid item sm={4}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.USER_NAME_TEXT}
          </InputLabel>
          <TextFieldWithInfoInputAdornment
            value={userNameLoading ? placeholders.loading : userNamePrefix}
            disabled
            size={"small"}
            infoText={LanguageReducer?.languageType?.USERNAME_PREFIX_TEXT}
            fullWidth
          />
        </Grid>
        <Grid item sm={4}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.PASSWORD_TEXT}
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
    </ModalComponent>
  );
}
export default AddDriverModal;
