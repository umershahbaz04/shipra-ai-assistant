import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import BorderColorIcon from "@mui/icons-material/BorderColor";
import CancelIcon from "@mui/icons-material/Cancel";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import {
  Avatar,
  Box,
  Button,
  CircularProgress,
  FormControl,
  FormControlLabel,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  Slide,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { Link, useNavigate } from "react-router-dom";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal";
import CustomRHFPhoneInput from "../../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CheckEmailAvailability,
  CheckUsernameAvailability,
  GetAllCountry,
  Signup,
  UploadClientImage,
} from "../../../api/AxiosInterceptors";
import logout from "../../../assets/images/AlisImages/logout/logout.svg";
import avatarIcon from "../../../assets/images/avatar.png";
import "../../../assets/styles/phoneInput.css";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { setUserCredential } from "../../../utilities/cookies/setCookie";
import { EnumOptions, EnumRoutesUrls } from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  CicrlesLoading,
  GridContainer,
  HeightBox,
  fetchMethod,
  getLowerCase,
  placeholders,
  purple,
  useGetWindowHeight,
  useNavigateSetState,
  useSearchMethod,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import { errorNotification } from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function SignUpPage() {
  const { height: windowHeight } = useGetWindowHeight();
  const navigate = useNavigate();
  const { setNavigateState } = useNavigateSetState();
  const {
    selectedAddressSchema,
    addressSchemaSelectData,
    addressSchemaInputData,
    handleSetSchema,
    handleChangeInputAddressSchema,
    handleChangeSelectAddressSchemaAndGetOptions,
  } = useGetAddressSchema();
  const [values, setValues] = useState({});
  const [allCountries, setAllCountries] = useState([]);
  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [buttonLoading, setButtonLoading] = useState(false);
  const [isPreverifyEmail, setIsPreverifyEmail] = useState(false);
  const [emailError, setEmailError] = useState({
    loading: true,
    hasError: true,
    message: "",
  });
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    control,
    unregister,
  } = useForm();
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
    name: "email",
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

  const handleClickShowConfirmPassword = () => {
    setValues({
      ...values,
      showConfirmPassword: !values.showConfirmPassword,
    });
  };

  const handleMouseDownConfirmPassword = (event) => {
    event.preventDefault();
  };
  const handleCheckAvailability = async () => {
    let flag = false;
    if (getValues("email")) {
      if (/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(getValues("email"))) {
        const { response } = await fetchMethod(
          () => CheckEmailAvailability(getValues("email")),
          setEmailError,
        );
        if (!response.isSuccess) {
          setEmailError({
            hasError: true,
            message: response.errors?.EmailExists[0],
          });
        } else {
          setEmailError({
            hasError: false,
            message: "",
          });
          return true;
        }
      } else {
        setEmailError({
          hasError: true,
          message: LanguageReducer?.languageType?.INVALID_EMAIL_TOAST,
        });
      }
    }
    return flag;
  };

  const signUpForm = async (data) => {
    setButtonLoading(true);
    let checkUsernameAvailability = await CheckUsernameAvailability({
      userName: data.userName,
    });
    const emailNotDuplicate = await handleCheckAvailability();
    setButtonLoading(false);
    if (emailNotDuplicate) {
      setIsLoading(true);
      if (!checkUsernameAvailability?.data?.isSuccess) {
        UtilityClass.showErrorNotificationWithDictionary(
          checkUsernameAvailability?.data?.errors,
        );
        setIsLoading(false);
        return;
      }
      let mobile = data.mobile;
      if (data.mobile) {
        mobile = "+" + UtilityClass.getFormatedNumber(data.mobile);
      }
      let mobile2 = data.phone;
      if (mobile2) {
        mobile2 = "+" + UtilityClass.getFormatedNumber(data.phone);
      }
      let body = {
        clientName: data.clientName,
        clientImage: imageURL,
        clientCompanyName: data.companyName,
        email: data.email,
        userName: data.userName,
        mobile: mobile,
        password: data.password,
        phone: mobile2,
        IsPreverifyEmail: isPreverifyEmail,
        clientAddress: {
          countryId: selectedAddressSchema.country,
          cityId: selectedAddressSchema.city,
          areaId: selectedAddressSchema.area,
          streetAddress: selectedAddressSchema.streetAddress,
          streetAddress2: selectedAddressSchema.streetAddress2,
          houseNo: null,
          buildingName: null,
          landmark: null,
          workEmail: data.email,
          provinceId: selectedAddressSchema.province,
          pinCodeId: selectedAddressSchema.pinCode,
          stateId: selectedAddressSchema.state,
          zip: null,
          addressTypeId: 0,
          latitude: null,
          longitude: null,
        },
      };
      Signup(body)
        .then((res) => {
          const response = res.data;
          setIsLoading(false);

          console.log("res:", res);
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
            // errorNotification(
            //   LanguageReducer?.languageType?.UNABLE_TO_CREATE_USER_TOAST
            // );
            // errorNotification(res?.data?.customErrorMessage);
          } else {
            const credData = {
              userName: body.userName,
              password: body.password,
            };
            let uData = UtilityClass.encryptData(credData);
            setUserCredential(uData);
            setNavigateState(EnumRoutesUrls.PRICING_PLAN, {
              client_reference_id: response.result?.client_id,
              customer_session_client_secret:
                response.result?.customerSessionClientSecret,
              pricing_table_id: response.result?.pricingTableId,
              publishable_key: response.result?.publishableKey,
            });
            localStorage.username = data.userName;
          }
        })
        .catch((e) => {
          setIsLoading(false);
          console.log("e", e);
          errorNotification(
            LanguageReducer?.languageType?.UNABLE_TO_CREATE_USER_TOAST,
          );
        });
    }
  };

  let uploadClientImage = (e) => {
    console.log("e", e.target.files[0]);
    setFile(e.target.files[0]);
    const formData = new FormData();
    formData.append("file", e.target.files[0]);
    UploadClientImage(formData)
      .then((res) => {
        console.log("res", res);
        setImageURL(res.data.result.url);
        //successNotification(res.data.result.message);
      })
      .catch((e) => console.log("e", e));
  };

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    // console.log("res", res.data.result);
    if (res.data.result != null) {
      setAllCountries(res.data.result);
    }
  };
  useEffect(() => {
    getAllCountry();
    localStorage.username = "";
  }, []);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleFocus = (event) => event.target.select();
  const schemaFieldsLength = [
    ...addressSchemaSelectData,
    ...addressSchemaInputData,
  ].length;
  useSearchMethod(getValues("email"), handleCheckAvailability);
  return (
    <Box
      display={"flex"}
      sx={{
        "& .MuiInputBase-root": {},
        margin: "0px 15px",
        height: {
          md: `calc(${windowHeight}px - 64px);`,
          sm: "auto",
          xs: "auto",
        },
      }}
    >
      {isLoading ? (
        <CicrlesLoading
          text={
            <>
              <h3 style={{ margin: 0 }}>Setting up your system</h3>
              <p style={{ margin: 0 }}>firing up service</p>
            </>
          }
        />
      ) : (
        <>
          <Box
            flexBasis={{ md: "67%", sm: "67%", xs: "100%" }}
            px={{ md: 7, sm: 2, xs: 2 }}
            sx={{
              display: "flex",
              flexDirection: "column",
              justifyContent: "center",
            }}
          >
            <form
              className="flex_between"
              style={{
                flexDirection: "column",
              }}
            >
              <Box>
                <Box sx={styleSheet.addDriverHeadingAndUpload}>
                  <Typography sx={styleSheet.addDriverHeading}>
                    {LanguageReducer?.languageType?.SETTING_SIGN_UP}
                  </Typography>
                  <Box sx={styleSheet.uploadIconArea}>
                    <IconButton component="label" sx={{ position: "relative" }}>
                      <Avatar
                        sx={{
                          width: 90,
                          height: 90,
                          border: file && "1px solid grey",
                        }}
                        src={
                          file ? file && URL.createObjectURL(file) : avatarIcon
                        }
                      ></Avatar>
                      <Box
                        width={24}
                        height={24}
                        bgcolor={purple}
                        borderRadius={"50%"}
                        position={"absolute"}
                        bottom={8}
                        right={16}
                        className={"flex_center"}
                      >
                        <BorderColorIcon
                          sx={{
                            width: 15,
                            height: 15,
                            color: "#fff",
                          }}
                        />
                      </Box>
                      <input
                        onChange={uploadClientImage}
                        hidden
                        accept="image/*"
                        type="file"
                      />
                    </IconButton>
                  </Box>
                </Box>

                <Grid container spacing={2}>
                  <Grid item md={6} sm={12} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.SETTING_PROFILE_NAME}
                    </InputLabel>
                    <TextField
                      placeholder={placeholders.name}
                      onFocus={handleFocus}
                      type="text"
                      size="small"
                      // id="clientName"
                      // name="clientName"
                      fullWidth
                      variant="outlined"
                      {...register("clientName", {
                        required: {
                          value: true,
                          message:
                            LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                        },
                        pattern: {
                          value: /^(?!\s*$).+/,
                          message:
                            LanguageReducer?.languageType
                              ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                        },
                      })}
                      error={Boolean(errors.clientName)} // set error prop
                      helperText={errors.clientName?.message}
                    />
                  </Grid>
                  <Grid item md={6} sm={12} xs={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      {
                        LanguageReducer?.languageType
                          ?.SETTING_PROFILE_COMPANY_NAME
                      }
                    </InputLabel>
                    <TextField
                      placeholder={placeholders.company_name}
                      onFocus={handleFocus}
                      type="text"
                      size="small"
                      id="companyName"
                      name="companyName"
                      fullWidth
                      variant="outlined"
                      {...register("companyName")}
                      error={Boolean(errors.companyName)} // set error prop
                      helperText={errors.companyName?.message}
                    />
                  </Grid>

                  <Grid item md={4} sm={12} xs={12}>
                    <Box sx={{ position: "relative" }}>
                      <InputLabel required sx={styleSheet.inputLabel}>
                        {LanguageReducer?.languageType?.SETTING_USER_EMAIL}
                      </InputLabel>
                      <FormControlLabel
                        sx={{
                          position: "absolute",
                          top: -4,
                          right: -12,
                          opacity: 0,
                        }}
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
                          LanguageReducer?.languageType
                            ?.SETTING_USER_CONFIRM_ACCOUNT
                        }
                      />
                    </Box>
                    <Box sx={{ position: "relative" }}>
                      <TextField
                        placeholder={placeholders.email}
                        onFocus={handleFocus}
                        type="email"
                        size="small"
                        id="email"
                        name="email"
                        fullWidth
                        variant="outlined"
                        {...register("email", {
                          required: {
                            value: true,
                            message:
                              LanguageReducer?.languageType
                                ?.FIELD_REQUIRED_TEXT,
                          },
                          pattern: {
                            value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i, // set pattern to match email format
                            message:
                              LanguageReducer?.languageType
                                ?.INVALID_EMAIL_TOAST,
                          },
                        })}
                        error={
                          getValues("email") &&
                          !emailError.loading &&
                          (emailError.hasError || Boolean(errors.email))
                        } // set error prop
                        helperText={emailError.message || errors.email?.message}
                      />
                      {getValues("email") &&
                        !emailError.loading &&
                        (Boolean(errors.email) || emailError.hasError ? (
                          <CancelIcon
                            sx={{
                              position: "absolute",
                              top: 7,
                              right: 2,
                              color: Colors.danger,
                            }}
                          />
                        ) : (
                          <CheckCircleIcon
                            sx={{
                              position: "absolute",
                              top: 7,
                              right: 2,
                              color: Colors.succes,
                            }}
                          />
                        ))}
                    </Box>
                  </Grid>
                  <Grid item md={4} sm={12} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.SETTING_PROFILE_MOBILE_NO}
                    </InputLabel>
                    <CustomRHFPhoneInput
                      error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
                      name="mobile"
                      control={control}
                      required
                    />
                  </Grid>
                  <Grid item md={4} sm={12} xs={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.SETTING_PROFILE_PHONE_NO}
                    </InputLabel>
                    <CustomRHFPhoneInput
                      name="phone"
                      control={control}
                      isContact={true}
                    />
                  </Grid>
                  <Grid
                    item
                    md={schemaFieldsLength === 0 ? 12 : 4}
                    sm={12}
                    xs={12}
                  >
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer?.languageType?.SETTING_PROFILE_COUNTRY}
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
                          unregister
                        );
                      }}
                      errors={errors}
                    />
                  </Grid>
                  {[...addressSchemaSelectData, ...addressSchemaInputData].map(
                    (input, index) => (
                      <Grid item md={4} sm={12} xs={12}>
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
                    <Grid item md={4} sm={12} xs={12}>
                      <InputLabel required sx={styleSheet.inputLabel}>
                        {
                          LanguageReducer?.languageType
                            ?.SETTING_PROFILE_USERNAME
                        }
                      </InputLabel>
                      <TextField
                        placeholder={placeholders.name}
                        onFocus={handleFocus}
                        type="text"
                        size="small"
                        id="userName"
                        name="userName"
                        fullWidth
                        variant="outlined"
                        {...register("userName", {
                          required: {
                            value: true,
                            message:
                              LanguageReducer?.languageType
                                ?.FIELD_REQUIRED_TEXT,
                          },
                          minLength: {
                            value: 5,
                            message: "Input must be at least 5 characters long",
                          },
                          pattern: {
                            value: /^[a-z0-9]+$/,
                            message:
                              "Input should not contain dots, commas, special characters, or uppercase letters",
                          },
                        })}
                        error={Boolean(errors.userName)}
                        helperText={errors.userName?.message}
                      />
                    </Grid>
                    <Grid item md={4} sm={12} xs={12}>
                      <InputLabel sx={styleSheet.inputLabel} required>
                        {LanguageReducer?.languageType?.SETTING_USER_PASSWORD}
                      </InputLabel>
                      <FormControl
                        fullWidth
                        variant="outlined"
                        error={Boolean(errors.password)}
                      >
                        <OutlinedInput
                          placeholder={"●●●●●●●●●"}
                          onFocus={handleFocus}
                          type={values.showPassword ? "text" : "password"}
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
                                LanguageReducer?.languageType
                                  ?.FIELD_REQUIRED_TEXT,
                            },
                            pattern: {
                              value:
                                /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()\-=_+[\]{}|;':",./<>?]).{8,}$/,
                              message:
                                LanguageReducer?.languageType
                                  ?.PASSWORD_MUST_CONTAIN_MINIMUM_8_CHARACTERS_1_UPPERCASE_1_LOWERCASE_1_NUMBER_1_SPECIAL_CHARACTER,
                            },
                          })}
                        />
                        <FormHelperText>
                          {errors.password?.message}
                        </FormHelperText>
                      </FormControl>
                    </Grid>
                    <Grid item md={4} sm={12} xs={12}>
                      <InputLabel sx={styleSheet.inputLabel} required>
                        {
                          LanguageReducer?.languageType
                            ?.SETTING_USER_CONFIRM_PASSWORD
                        }
                      </InputLabel>
                      <FormControl
                        fullWidth
                        variant="outlined"
                        error={Boolean(errors.confirmPassword)}
                      >
                        <OutlinedInput
                          placeholder={"●●●●●●●●●"}
                          onFocus={handleFocus}
                          type={
                            values.showConfirmPassword ? "text" : "password"
                          }
                          value={values.confirmPassword}
                          onChange={(e) =>
                            setValues({
                              ...values,
                              confirmPassword: e.target.value,
                            })
                          }
                          endAdornment={
                            <InputAdornment position="end">
                              <IconButton
                                aria-label="toggle password visibility"
                                onClick={handleClickShowConfirmPassword}
                                onMouseDown={handleMouseDownConfirmPassword}
                                edge="end"
                              >
                                {values.showConfirmPassword ? (
                                  <VisibilityOff />
                                ) : (
                                  <Visibility />
                                )}
                              </IconButton>
                            </InputAdornment>
                          }
                          size="small"
                          fullWidth
                          id="confirmPassword"
                          name="confirmPassword"
                          {...register("confirmPassword", {
                            required: {
                              value: true,
                              message:
                                LanguageReducer?.languageType
                                  ?.FIELD_REQUIRED_TEXT,
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
                        <FormHelperText>
                          {errors.confirmPassword?.message}
                        </FormHelperText>
                      </FormControl>
                    </Grid>
                  </GridContainer>
                </Grid>

                <br />
              </Box>
              <HeightBox
                length={schemaFieldsLength}
                sx={{ display: { md: "flex", sm: "none", xs: "none" } }}
              />
              <Box
                width="100%"
                marginBottom={{ xs: "15px", sm: "15px", md: 0 }}
              >
                <Typography sx={styleSheet.integrationCardDes}>
                  {
                    LanguageReducer?.languageType
                      ?.SETTING_ALREADY_HAVE_AN_ACCOUNT
                  }{" "}
                  <Link to="/">
                    {LanguageReducer?.languageType?.SETTING_LOG_IN}
                  </Link>
                </Typography>
                {buttonLoading ? (
                  <Button
                    fullWidth
                    variant="contained"
                    sx={{
                      ...styleSheet.modalSubmitButton,
                      borderRadius: "20px",
                      mt: 1,
                    }}
                    disabled
                  >
                    <CircularProgress sx={{ color: "white" }} />
                  </Button>
                ) : (
                  <Button
                    fullWidth
                    variant="contained"
                    sx={{
                      ...styleSheet.modalSubmitButton,
                      borderRadius: "20px",
                      mt: 1,
                    }}
                    onClick={handleSubmit(signUpForm)}
                  >
                    {LanguageReducer?.languageType?.SETTING_LETS_GET_STARTED}
                  </Button>
                )}
              </Box>
            </form>
          </Box>{" "}
          <Box
            flexBasis={"33%"}
            className={"flex_center"}
            bgcolor={"primary.main"}
            display={{
              md: "flex !important",
              sm: "flex !important",
              xs: "none !important",
            }}
          >
            <Box
              component={"img"}
              src={logout}
              width={"100%"}
              bgcolor={"primary.main"}
            />
          </Box>
        </>
      )}
    </Box>
  );
}
export default SignUpPage;
