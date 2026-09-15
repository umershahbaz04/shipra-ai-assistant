import { TextField } from "@mui/material";
import React from "react";
import DatePicker from "react-datepicker";

export default function DatePickerTextFieldComponent(props) {
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
    ...others
  } = props;

  return (
    <DatePicker
      selected={value}
      onChange={onChange}
      customInput={
        <TextField
          sx={{
            width: "100%",
            "& .MuiInputBase-root": {
              borderRadius: "10px",
              background: "#fff",
            },
          }}
          name={name}
          error={!disabled && required && errors.includes(name)}
          required={required}
          helperText={
            !disabled &&
            required &&
            errors.indexOf(name) > -1 &&
            "Field Required"
          }
        />
      }
    />
  );
}
