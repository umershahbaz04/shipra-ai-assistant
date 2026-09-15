import {
  Box,
  Button,
  CircularProgress,
  DialogActions,
  DialogContent,
  DialogTitle,
  InputLabel,
  Paper,
  Slide,
  TextField,
  Typography,
} from "@mui/material";
import React, { useState } from "react";
import { useSelector } from "react-redux";

import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { ForgotPassword } from "../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { placeholders } from "../../../utilities/helpers/Helpers";
const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function ForgotPasswordPage(props) {
  let navigate = useNavigate();
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

  const forgotPassword = async (values) => {
    setIsLoading(true);
    console.log("values", values);

    const body = {
      userName: values.userName,
    };
    ForgotPassword(body)
      .then((res) => {
        console.log("res", res);
        if (res.data.isSuccess) {
          successNotification(res.data.result.message);
          setIsLoading(false);
          const timer = setTimeout(() => {
            navigate("/reset-password", {
              state: {
                userName: values.userName,
                clientId: res?.data?.result?.clientId,
              },
            });
          }, 2000);
          return () => clearTimeout(timer);
        } else {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
          //   errorNotification(res.data.errors.UserNotFoundException[0]);
          setIsLoading(false);
        }
      })
      .catch((e) => {
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST,
        );
        console.log("e", e);
        setIsLoading(false);
      });
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
          {LanguageReducer?.languageType?.FORGOT_PASSWORD_TEXT}
        </DialogTitle>
        <form onSubmit={handleSubmit(forgotPassword)}>
          <DialogContent sx={styleSheet.cardContentArea}>
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
                  value: /^(?! +$)[a-z0-9]+$/,
                  message:
                    LanguageReducer?.languageType
                      ?.INPUT_SHOULD_NOT_CONTAIN_SPACES_OR_UPPERCASE,
                },
              })}
              error={Boolean(errors.userName)} // set error prop
              helperText={errors.userName?.message}
            />
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
export default ForgotPasswordPage;
