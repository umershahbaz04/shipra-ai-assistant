import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import {
  Box,
  Button,
  CircularProgress,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  Paper,
  Slide,
  TextField,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { Link, useLocation, useNavigate } from "react-router-dom";
import {
  GetAllClientMenuByRoleId,
  Login,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { handleDispatchUserProfile } from "../../../components/topNavBar";
import { updateUserRole } from "../../../redux/userRole";
import { setMenuPermissions } from "../../../redux/menuPermissions";
import UtilityClass from "../../../utilities/UtilityClass";
import { getThisKeyCookie } from "../../../utilities/cookies";
import { setUserCredential } from "../../../utilities/cookies/setCookie";
import { EnumRoutesUrls, EnumUserType } from "../../../utilities/enum";
import {
  cookie_isUserProfileSideBarShow,
  handleGetAndSetMapApiKey,
  placeholders,
  useNavigateSetState,
} from "../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../utilities/toast";
import setLoginCookies from "./setLoginCookies";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function LoginPage(props) {
  let navigate = useNavigate();
  const { setNavigateState } = useNavigateSetState();
  const dispatch = useDispatch();
  const location = useLocation();
  const tokenExists = getThisKeyCookie("access_token");

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [values, setValues] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    reset,
    control,
  } = useForm();
  useEffect(() => {
    if (tokenExists) {
      handleDispatchUserProfile(dispatch, false, navigate);
      navigate("/analytics");
    }
  }, []);
  const loginForm = async (data) => {
    setIsLoading(true);
    const body = {
      userName: data.userName,
      password: data.password,
    };
    Login(body)
      .then(async (res) => {
        const response = res.data;

        if (!res.data.isSuccess) {
          if (
            res?.data?.errors?.UsernameNotConfirm &&
            res?.data?.errors?.UsernameNotConfirm[0]
          ) {
            localStorage.username = data.userName;
            // if plan active
            if (response.result?.isPlannedSubscribed) {
              let uData = UtilityClass.encryptData(body);
              setUserCredential(uData);
              const timer = setTimeout(() => {
                navigate("/verify-otp", {
                  state: { userName: body.userName },
                });
              }, 2000);
              return () => clearTimeout(timer);
            } else {
              setNavigateState(EnumRoutesUrls.PRICING_PLAN, {
                client_reference_id: response.result?.client_id,
                customer_session_client_secret:
                  response.result?.customerSessionClientSecret,
                pricing_table_id: response.result?.pricingTableId,
                publishable_key: response.result?.publishableKey,
              });
            }
          } else {
            UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
          }
          setIsLoading(false);
        } else {
          if (res?.data?.result?.userRoleId === EnumUserType.SalesCoordinator) {
            window.location.href = "https://portal.shipra.io/";
            setIsLoading(false);
            return;
          }
          if (
            !res?.data?.result?.userRoleId ||
            (res?.data?.result?.userRoleId != EnumUserType.Driver &&
              res?.data?.result?.userRoleId != EnumUserType.SalesCoordinator)
          ) {
            // if plan active
            if (response.result?.isPlannedSubscribed) {
              dispatch(updateUserRole(res?.data?.result?.userRoleId));
              await setLoginCookies(res);
              handleGetAndSetMapApiKey(dispatch);
              try {
                const menuRes = await GetAllClientMenuByRoleId(
                  res?.data?.result?.userRoleId
                );
                if (menuRes?.data?.result) {
                  dispatch(setMenuPermissions(menuRes.data.result));
                } else {
                  dispatch(setMenuPermissions([]));
                }
              } catch (e) {
                console.error("Failed to fetch menu permissions", e);
                dispatch(setMenuPermissions([]));
              }
              setIsLoading(false);
              navigate(EnumRoutesUrls.ORDERS);
              if (cookie_isUserProfileSideBarShow) {
                handleDispatchUserProfile(dispatch, false, navigate);
              }
            } else {
              if (response.result?.customerSessionClientSecret) {
                localStorage.username = data.userName;
                setNavigateState(EnumRoutesUrls.PRICING_PLAN, {
                  client_reference_id: response.result?.client_id,
                  customer_session_client_secret:
                    response.result?.customerSessionClientSecret,
                  pricing_table_id: response.result?.pricingTableId,
                  publishable_key: response.result?.publishableKey,
                });
              } else {
                errorNotification(
                  "Something went wrong.Please contact your administrator",
                );
              }
            }
          } else {
            errorNotification("You are not authorized");
          }
        }
      })
      .catch((e) => {
        errorNotification(
          LanguageReducer?.languageType?.INVALID_USER_NAME_PASSWORD_TOAST,
        );
        setIsLoading(false);
        console.log("e", e);
      })
      .finally((e) => {
        setIsLoading(false);
      });
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
  const handleFocus = (event) => event.target.select();

  useEffect(() => {
    localStorage.username = "";
  }, []);

  return (
    <Box
      sx={{
        width: "100%",
        maxWidth: "500px",
        margin: "auto",
        marginTop: { md: "0px", sm: "50px", xs: "50px" },
        height: { md: "90vh", sm: "auto", sm: "auto" },
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        fontSize: "25px",
        p: { md: "10px", sm: "15px", xs: "20px" },
      }}
    >
      {tokenExists ? null : (
        <Paper
          elevation={6}
          sx={{ ...styleSheet.cardMainClass, width: "100%" }}
          aria-describedby="alert-dialog-slide-description"
        >
          <DialogTitle sx={{ ...styleSheet.outScanHeading, width: "100%" }}>
            {LanguageReducer?.languageType?.LOGIN_TEXT}
          </DialogTitle>
          <form onSubmit={handleSubmit(loginForm)} width={"100%"}>
            <DialogContent
              sx={{ ...styleSheet.cardContentArea, width: "100%" }}
            >
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.USER_NAME_TEXT}
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
                    message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                  },
                  pattern: {
                    value: /^[A-Za-z0-9]+$/,
                    message: "Input should not contain spaces.",
                  },
                })}
                error={Boolean(errors.userName)} // set error prop
                helperText={errors.userName?.message}
              />
              <br />
              <br />
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.PASSWORD_TEXT}
              </InputLabel>
              <FormControl fullWidth variant="outlined">
                <OutlinedInput
                  placeholder={"●●●●●●●●●"}
                  onFocus={handleFocus}
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
              <Typography sx={styleSheet.forgotPasswordText}>
                <Link to="/forgot-password">
                  {LanguageReducer?.languageType?.FORGOT_PASSWORD_TEXT}?{" "}
                </Link>
              </Typography>
              <Typography sx={{ ...styleSheet.integrationCardDes, display: "none" }}>
                {LanguageReducer?.languageType?.DONT_HAVE_AN_ACCOUNT_TEXT}{" "}
                <Link to="/signup">
                  {LanguageReducer?.languageType?.SIGN_UP_TEXT}
                </Link>
              </Typography>
            </DialogContent>
            <DialogActions>
              {isLoading ? (
                <Button
                  fullWidth
                  variant="contained"
                  sx={styleSheet.modalSubmitButton}
                  disabled
                >
                  <CircularProgress sx={{ color: "white" }} />
                </Button>
              ) : (
                <Button
                  fullWidth
                  variant="contained"
                  sx={styleSheet.modalSubmitButton}
                  type="submit"
                >
                  {LanguageReducer?.languageType?.LOGIN_TEXT}
                </Button>
              )}
            </DialogActions>
          </form>
        </Paper>
      )}
    </Box>
  );
}
export default LoginPage;
