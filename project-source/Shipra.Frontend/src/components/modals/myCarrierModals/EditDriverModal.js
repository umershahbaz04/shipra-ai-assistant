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
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent.js";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput.js";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput.js";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent.js";
import {
  GetAllCountry,
  GetAllRegionbyCountryId,
  GetCityByRegionId,
  GetEmployeeById,
  UpdateEmployee,
  UploadEmployeeImage,
} from "../../../api/AxiosInterceptors.js";
import avatarIcon from "../../../assets/images/avatar.png";
import { styleSheet } from "../../../assets/styles/style.js";
import { placeholders, purple } from "../../../utilities/helpers/Helpers.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import UtilityClass from "../../../utilities/UtilityClass.js";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function EditDriverModal(props) {
  let {
    open,
    setOpen,
    getAllEmployees,
    allGenderForSelection,
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
    setValue("region", null);
    setValue("city", null);
    setAllRegions([]);
    setAllCities([]);
    if (getValues("country")?.countryId) {
      getAllRegionbyCountryId();
    }
  }, [getValues("country")]);
  useEffect(() => {
    setValue("city", null);
    setAllCities([]);
    if (getValues("region")) getCityByRegionId();
  }, [getValues("region")]);
  let uploadStoreImage = (e) => {
    console.log("e", e.target.files[0]);
    setFile(e.target.files[0]);
    const formData = new FormData();
    formData.append("File", e.target.files[0]);
    UploadEmployeeImage(formData)
      .then((res) => {
        // console.log("res", res);
        setImageURL(res.data.result.url);
        // successNotification(res.data.result.message);
      })
      .catch((e) => console.log("e", e));
  };
  console.log("imageURL", imageURL);
  const updateEmployee = async (data) => {
    console.log("vals::", data);

    const body = {
      EmployeeId: selectedRowData?.EmployeeId,
      employeeImage: imageURL,
      employeeName: data.employeeName,
      countryId: data.country.countryId,
      regionId: data.region.regionId,
      cityId: data.city.cityId,
      workEmail: data.email,
      userName: data.userName,
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
    };
    console.log("body::", body);
    setIsLoading(true);

    UpdateEmployee(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Driver update successfully");
          getAllEmployees();
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
          errorNotification("Unable to update employee");
        }
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  useEffect(() => {
    if (userData) {
      let defCountry = allCountries.find(
        (x) => x.countryId == userData?.countryId
      );
      let defgender = allGenderForSelection?.find(
        (x) => x.id == userData?.genderId
      );
      setValue("country", defCountry);
      setValue("email", userData?.workEmail);
      setValue("employeeCode", userData?.employeeCode);
      setValue("gender", defgender);
      let dob = userData?.dateOfBirth && new Date(userData?.dateOfBirth);
      setValue("dateOfBirth", dob);
      setValue("mobile", userData?.mobileNo);
      if (userData?.phoneNo) {
        setValue("phone", userData?.phoneNo);
      } else {
        setValue("phone", UtilityClass.getDefaultCountryCode());
      }
      setValue("addressLine1", userData?.addressLine1);
      setValue("addressLine2", userData?.addressLine2);
      setValue("employeeName", userData?.employeeName);
      setValue("userName", userData?.employeeName);
      setImageURL(userData?.employeeImage);
    }
  }, [userData]);
  useEffect(() => {
    if (allRegions?.length > 0) {
      let defRegion = allRegions.find((x) => x.regionId == userData?.regionId);
      setValue("region", defRegion);
    }
  }, [allRegions]);
  useEffect(() => {
    if (allCities?.length > 0) {
      let defData = allCities.find((x) => x.cityId == userData?.cityId);
      setValue("city", defData);
    }
  }, [allCities]);
  useEffect(() => {
    if (selectedRowData) {
      GetEmployeeById(selectedRowData.EmployeeId)
        .then((res) => {
          console.log("res:::", res);
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.errors);
          } else {
            setUserData(res?.data?.result);
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
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={"Update Driver"}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(updateEmployee)}
    >
      <Box sx={styleSheet.addDriverHeadingAndUpload}>
        <Typography sx={styleSheet.addDriverHeading} variant="h4">
          {"Update Driver"}
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
      <Grid container spacing={2}>
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
        <Grid item md={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.FULL_NAME}
          </InputLabel>
          <TextField
            placeholder={"Name"}
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
            getOptionLabel={(option) => option?.text}
            requiredError={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            value={getValues("gender")}
            errors={errors}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("gender", resolvedId);
            }}
            rules={{
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
              validate: {
                positive: (v) => parseInt(v?.id) > 0,
              },
            }}
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
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.EMAIL_TEXT}
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
        <Grid item md={4} lg={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.PHONE_NO_TEXT}
          </InputLabel>
          <CustomRHFPhoneInput
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            name="mobile"
            control={control}
            required
          />
        </Grid>
        <Grid item md={4} lg={4} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.PHONE_NO_TEXT}
          </InputLabel>
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
          <SelectComponent
            name="country"
            control={control}
            options={allCountries}
            getOptionLabel={(option) => option?.name}
            requiredError={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            value={getValues("country")}
            errors={errors}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("country", resolvedId);
            }}
            rules={{
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
              validate: {
                positive: (v) => parseInt(v?.countryId) > 0,
              },
            }}
            required
          />
        </Grid>
        <Grid item md={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SELECT_REGION_TEXT}
          </InputLabel>
          <SelectComponent
            name="region"
            control={control}
            options={allRegions}
            getOptionLabel={(option) => option?.name}
            requiredError={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            value={getValues("region")}
            errors={errors}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("region", resolvedId);
            }}
            rules={{
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
              validate: {
                positive: (v) => parseInt(v?.regionId) > 0,
              },
            }}
            required
          />
        </Grid>
        <Grid item md={4} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SELECT_CITY_TEXT}
          </InputLabel>
          <SelectComponent
            name="city"
            control={control}
            options={allCities}
            getOptionLabel={(option) => option?.name}
            requiredError={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
            value={getValues("city")}
            errors={errors}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("city", resolvedId);
            }}
            rules={{
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
              validate: {
                positive: (v) => parseInt(v?.cityId) > 0,
              },
            }}
            required
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
        {/* <Grid item sm={12} sx={{ mt: "10px" }}>
                <Divider />
              </Grid>
              <Grid item sm={12}>
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
    </ModalComponent>
  );
}
export default EditDriverModal;
