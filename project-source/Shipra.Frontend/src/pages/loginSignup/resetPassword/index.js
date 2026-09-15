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
  Typography,
  Alert,
} from "@mui/material";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import Close from '@mui/icons-material/Close';
import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import { FormHelperText } from "@mui/material";
import { useForm } from "react-hook-form";
import OtpInput from "react-otp-input";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { styleSheet } from "../../../assets/styles/style";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { ConfirmForgotPassword } from "../../../api/AxiosInterceptors";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function ResetPasswordPage(props) {
  let navigate = useNavigate();
  const location = useLocation();
  const [values, setValues] = useState({});
  const [otp, setOtp] = useState();
  const [errorMessage, setErrorMessage] = React.useState("");

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

  const resetPassword = async (data) => {
    if (!otp) {
      setErrorMessage(LanguageReducer?.languageType?.PLEASE_ENTER_OTP_TOAST);
      return;
    }
    setIsLoading(true);
    console.log("data", data);
    const body = {
      userName: location.state.userName,
      confirmationCode: otp,
      password: data.password,
      ClientId: location.state.clientId,
    };
    ConfirmForgotPassword(body)
      .then((res) => {
        console.log("res", res);
        if (res.data.isSuccess) {
          //   successNotification(res.data.result.message);
          setIsLoading(false);
          const timer = setTimeout(() => {
            navigate("/login");
          }, 2000);
          return () => clearTimeout(timer);
        } else {
          if (res.data.errors.CodeMismatchException) {
            setErrorMessage(res.data.errors?.CodeMismatchException[0]);
          }
          if (res.data.errors.EntityNotFoundException) {
            setErrorMessage(res.data.errors?.EntityNotFoundException[0]);
          }
          setIsLoading(false);
        }
      })
      .catch((e) => {
        setErrorMessage(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST,
        );
        console.log("e", e);
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

  const handleClickShowConfirmPassword = () => {
    setValues({
      ...values,
      showConfirmPassword: !values.showConfirmPassword,
    });
  };

  const handleMouseDownConfirmPassword = (event) => {
    event.preventDefault();
  };
  const handleFocus = (event) => event.target.select();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Box
      sx={{
        height: "90vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        fontSize: "25px",
      }}
    >
      <Paper
        elevation={6}
        sx={styleSheet.cardMainClass}
        aria-describedby="alert-dialog-slide-description"
      >
        <DialogTitle sx={styleSheet.outScanHeading}>
          {LanguageReducer?.languageType?.RESET_PASSWORD_TEXT}
        </DialogTitle>
        <form onSubmit={handleSubmit(resetPassword)}>
          <DialogContent sx={styleSheet.cardContentArea}>
            {errorMessage && errorMessage != "" && (
              <Box sx={styleSheet.integrationCardText}>
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
            <OtpInput
              inputStyle={{
                width: "3rem",
                height: "3rem",
                margin: "20px .8rem",
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

            <InputLabel sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.PASSWORD_TEXT}
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
                  pattern: {
                    value:
                      /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()\-=_+[\]{}|;':",./<>?]).{8,}$/,
                    message:
                      LanguageReducer?.languageType
                        ?.PASSWORD_MUST_CONTAIN_MINIMUM_8_CHARACTERS_1_UPPERCASE_1_LOWERCASE_1_NUMBER_1_SPECIAL_CHARACTER,
                  },
                })}
              />
              <FormHelperText>{errors.password?.message}</FormHelperText>
            </FormControl>

            <InputLabel sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.CONFIRM_PASSWORD_TEXT}
            </InputLabel>
            <FormControl
              fullWidth
              variant="outlined"
              error={Boolean(errors.confirmPassword)}
            >
              <OutlinedInput
                placeholder={"●●●●●●●●●"}
                onFocus={handleFocus}
                type={values.showConfirmPassword ? "text" : "password"}
                value={values.confirmPassword}
                onChange={(e) =>
                  setValues({ ...values, confirmPassword: e.target.value })
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
                    message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
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
              <FormHelperText>{errors.confirmPassword?.message}</FormHelperText>
            </FormControl>

            <br />
            <Typography sx={styleSheet.integrationCardDesRight}>
              <Link to="/">
                {LanguageReducer?.languageType?.LOGIN_INSTEAD_TEXT}
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
                {LanguageReducer?.languageType?.SUBMIT_TEXT}
              </Button>
            )}
          </DialogActions>
        </form>
      </Paper>
    </Box>
  );
}
export default ResetPasswordPage;
