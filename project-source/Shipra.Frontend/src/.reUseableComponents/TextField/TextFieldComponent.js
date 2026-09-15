import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import { IconButton, InputAdornment, TextField } from "@mui/material";
import React from "react";
import { useShowPassword } from "../../utilities/helpers/Helpers";

export default function TextFieldComponent(props) {
  const {
    name,
    value,
    onChange,
    type = "text",
    required = true,
    errors = [],
    children,
    disabled = false,
    placeholder,
    passwordType,
    errorMsg,
    borderRadius = "4px",
    height = 38,
    fontSize = "inherit",
    endAdornmentBtn,
    isRHF,
    ...others
  } = props;

  const { showPassword, handleShowPassword } = useShowPassword();

  const hasError = isRHF
    ? Boolean(errors[name])
    : !disabled && required && errors.includes(name);

  return (
    <TextField
      sx={{
        "& .MuiInputBase-root": {
          borderRadius: borderRadius,
          background: "#fff",
          paddingRight: 0,
          height: height,
          padding: "2.5px 4px 2.5px 8px",
        },
        "& .MuiInputBase-input": {
          p: 0,
          fontSize: fontSize,
        },
      }}
      placeholder={placeholder}
      name={name}
      type={passwordType ? (showPassword ? "text" : "password") : type}
      required={required}
      disabled={disabled}
      error={hasError}
      helperText={hasError && (errorMsg || "Field Required")}
      size="small"
      fullWidth
      variant="outlined"
      value={value}
      onChange={onChange}
      InputProps={{
        endAdornment:
          type === "password" ? (
            <InputAdornment position="end">
              {endAdornmentBtn}
              <IconButton
                tabIndex={-1}
                onClick={handleShowPassword}
                onMouseDown={(event) => {
                  event.preventDefault();
                }}
              >
                {showPassword ? <Visibility /> : <VisibilityOff />}
              </IconButton>
            </InputAdornment>
          ) : (
            <InputAdornment position="end">{endAdornmentBtn}</InputAdornment>
          ),
      }}
      {...others}
    >
      {children}
    </TextField>
  );
}
