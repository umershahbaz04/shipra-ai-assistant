import Close from '@mui/icons-material/Close';
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import OtpInput from "react-otp-input";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  CheckIsUserConfirmed,
  ConfirmSignUpUser,
  Login,
  ResendConfirmationCode,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { getThisKeyCookie } from "../../../utilities/cookies";
import { EnumRoutesUrls } from "../../../utilities/enum";
import {
  CicrlesLoading,
  fetchMethod,
  useNavigateSetState,
} from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import setLoginCookies from "../loginScreen/setLoginCookies";

function OTPPage(props) {
  const [otp, setOtp] = useState();
  const [minutes, setMinutes] = useState(1);
  const [seconds, setSeconds] = useState(0);
  const [isSubmit, setIsSubmit] = useState(false);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const navigate = useNavigate();
  const { setNavigateState } = useNavigateSetState();
  const handleLogin = async () => {
    let cookieVal = getThisKeyCookie("U2FsdGVkX1");
    const body = UtilityClass.decryptData(cookieVal);

    await Login(body)
      .then(async (res) => {
        const response = res.data;
        if (res.data.isSuccess) {
          await setLoginCookies(res);
          // if plan active
          if (response.result?.isPlannedSubscribed) {
            if (getThisKeyCookie("access_token")) navigate("/analytics");
          } else {
            setNavigateState(EnumRoutesUrls.PRICING_PLAN, {
              client_reference_id: response.result?.client_id,
              customer_session_client_secret:
                response.result?.customerSessionClientSecret,
            });
          }
        }
      })
      .catch((e) => {
        setErrorMessage(
          LanguageReducer?.languageType?.INVALID_USER_NAME_PASSWORD_TOAST
        );
        console.log("e", e);
      });
  };
  const resendOTP = () => {
    ResendConfirmationCode({ userName: localStorage.username })
      .then((res) => {
        setMinutes(1);
        setSeconds(0);
        const response = res.data;
        if (response.isSuccess) {
        } else {
          if (
            response.errors?.InvalidParameterException[0] ===
            "User is already confirmed."
          ) {
            handleLogin();
          }
          setErrorMessage(response.errors?.InvalidParameterException[0]);
        }
      })
      .catch((e) => {
        setErrorMessage(
          LanguageReducer?.languageType?.UNABLE_TO_SEND_OTP_USER_TOAST
        );
        console.log("e", e);
      });
  };

  const confirmSignUpUser = async () => {
    if (!otp) {
      setErrorMessage(LanguageReducer?.languageType?.PLEASE_ENTER_OTP_TOAST);
      return;
    }
    const body = {
      userName: localStorage.username,
      code: otp,
    };
    setIsSubmit(true);
    ConfirmSignUpUser(body)
      .then((res) => {
        if (res.data.isSuccess) {
          handleLogin();
        } else {
          if (res.errors?.Error[0] === "User is already confirmed.") {
            handleLogin();
          }
          setErrorMessage(res.data.errors.Error[0]);
        }
      })
      .catch((e) => {
        setErrorMessage(
          LanguageReducer?.languageType?.UNABLE_TO_VERIFY_USER_TOAST
        );
        console.log("e", e);
      })
      .finally((e) => {
        setIsSubmit(false);
      });
  };

  useEffect(() => {
    const interval = setInterval(() => {
      if (seconds > 0) {
        setSeconds(seconds - 1);
      }
      if (seconds === 0) {
        if (minutes === 0) {
          clearInterval(interval);
        } else {
          setSeconds(59);
          setMinutes(minutes - 1);
        }
      }
    }, 1000);

    return () => {
      clearInterval(interval);
    };
  }, [seconds]);

  const handleNavigate = async () => {
    const { response } = await fetchMethod(
      () => CheckIsUserConfirmed(localStorage.username),
      setLoading,
      false
    );
    if (response.isSuccess) {
      navigate(EnumRoutesUrls.ANALYTICS);
      localStorage.username = "";
    }
  };

  useEffect(() => {
    handleNavigate();
  }, []);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Box
      sx={{
        width: "100%",
        maxWidth: "500px",
        margin: "auto",
        height: "90vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        fontSize: "25px",
        p: { md: "10px", sm: "15px", xs: "20px" },
      }}
    >
      {loading ? (
        <CicrlesLoading />
      ) : (
        <Paper
          elevation={6}
          sx={{ ...styleSheet.cardMainClass, width: "100%" }}
          aria-describedby="alert-dialog-slide-description"
        >
          <DialogTitle sx={{ ...styleSheet.outScanHeading, width: "100%" }}>
            {LanguageReducer?.languageType?.VERIFY_OTP_TEXT}
          </DialogTitle>
          <DialogContent
            sx={{
              ...styleSheet.cardContentArea,
              width: "100%",
              padding: { md: "20px 24px", sm: "20px 24px", xs: "7px" },
            }}
          >
            {errorMessage && errorMessage != "" && (
              <Box sx={{ ...styleSheet.integrationCardText, width: "100%" }}>
                <Alert
                  variant="outlined"
                  severity="error"
                  action={
                    <IconButton
                      aria-label="close"
                      color="inherit"
                      size="small"
                      onClick={() => {
                        setErrorMessage("");
                      }}
                    >
                      <Close fontSize="inherit" />
                    </IconButton>
                  }
                >
                  {errorMessage}
                </Alert>
              </Box>
            )}
            <Box
              sx={{
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                "& input": {
                  margin: { md: "20px .8rem", sm: "20px .8rem", xs: "5px" },
                  textAlign: "center",
                },
              }}
            >
              <OtpInput
                inputStyle={{
                  width: "100%",
                  height: "3rem",
                  fontSize: "1rem",
                  borderRadius: 4,
                  border: "2px solid rgba(0,0,0,0.3)",
                }}
                value={otp}
                onChange={setOtp}
                numInputs={6}
                separator={<span>-</span>}
                renderInput={(props) => <input {...props} />}
              />
            </Box>
            <Typography sx={styleSheet.integrationCardText}>
              <b>A verification code has been sent to your email. </b>
            </Typography>
            {seconds > 0 || minutes > 0 ? (
              <Typography sx={styleSheet.integrationCardDes}>
                {LanguageReducer?.languageType?.TIME_REMAINING_TEXT}
                {minutes < 10 ? `0${minutes}` : minutes} :{" "}
                {seconds < 10 ? `0${seconds}` : seconds}
                <Button
                  disabled={seconds > 0 || minutes > 0}
                  onClick={resendOTP}
                  variant="text"
                >
                  {LanguageReducer?.languageType?.RESEND_OTP_TEXT}
                </Button>
              </Typography>
            ) : (
              <Typography sx={styleSheet.integrationCardDes}>
                {LanguageReducer?.languageType?.DID_NOT_RECEIVE_CODE_TEXT}
                <Button
                  disabled={seconds > 0 || minutes > 0}
                  onClick={resendOTP}
                  variant="text"
                >
                  {LanguageReducer?.languageType?.RESEND_OTP_TEXT}
                </Button>
              </Typography>
            )}
          </DialogContent>
          <DialogActions>
            <Button
              fullWidth
              variant="contained"
              sx={styleSheet.modalSubmitButton}
              onClick={() => {
                confirmSignUpUser();
              }}
              loading={isSubmit}
            >
              {isSubmit ? (
                <CircularProgress sx={{ color: "white" }} />
              ) : (
                "Confirm"
              )}
            </Button>
          </DialogActions>
        </Paper>
      )}
    </Box>
  );
}
export default OTPPage;
