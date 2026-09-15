import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import {
  FormControl,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  TextField,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../../.reUseableComponents/Buttons/ModalButtonComponent";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import { RotateKeys } from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import { placeholders } from "../../../../utilities/helpers/Helpers";
import { successNotification } from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";

const LoginModal = ({ open, onClose, getClientKeys }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [values, setValues] = useState({});
  const [userData, setUserData] = useState({});
  const [openDelete, setOpenDelete] = useState(false);
  const [loading, setLoading] = useState(false);
  const handleClickShowPassword = () => {
    setValues((prev) => ({
      ...prev,
      showPassword: !prev.showPassword,
    }));
  };

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  const handleFocus = (event) => event.target.select();

  const loginForm = (data) => {
    setUserData(data);
    setOpenDelete(true);
  };
  const handleApproved = async () => {
    setLoading(true);
    try {
      const body = {
        userName: userData?.userName,
        password: userData?.password,
      };

      const response = await RotateKeys(body);
      if (response?.data?.isSuccess) {
        successNotification(response?.data?.result?.message);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (error) {
    } finally {
      setOpenDelete(false);
      onClose();
      setLoading(false);
      getClientKeys();
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth={"sm"}
      title={"Client Keys"}
      actionBtn={
        <ModalButtonComponent title={"Rotate"} bg={purple} type="submit" />
      }
      component={"form"}
      onSubmit={handleSubmit(loginForm)}
    >
      <Grid container spacing={2}>
        {/* Username Field */}
        <Grid item xs={12}>
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
            error={Boolean(errors.userName)}
            helperText={errors.userName?.message}
          />
        </Grid>

        {/* Password Field */}
        <Grid item xs={12} sx={{ paddingTop: "6px !important" }}>
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
      {openDelete && (
        <DeleteConfirmationModal
          open={openDelete}
          setOpen={setOpenDelete}
          handleDelete={handleApproved}
          heading={"Are you sure you want to rotate the key"}
          message={
            "Your connection will be lost. Please save your work before proceeding"
          }
          buttonText={"yes"}
          loading={loading}
        />
      )}
    </ModalComponent>
  );
};

export default LoginModal;
