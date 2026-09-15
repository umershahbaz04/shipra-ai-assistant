import { Box, FormHelperText } from "@mui/material";
import React from "react";
import PhoneInput from "react-phone-input-2";
import "react-phone-input-2/lib/material.css";

export default function PhoneTextFieldComponent(props) {
  const {
    value,
    onChange,
    name,
    required = true,
    country,
    errors = [],
    isRHF,
  } = props;
  const hasError = isRHF
    ? Boolean(errors[name])
    : required && errors.includes(name);

  return (
    <Box
      sx={{
        "& .form-control": {
          padding: " 15px 14px 15px 58px",
        },
        "& .form-control:focus": {
          borderColor: hasError ? `#d32f2f !important` : `var(--primary-color) !important`,
          boxShadow: hasError
            ? "0 0 0 1px #d32f2f !important"
            : "0 0 0 1px var(--primary-color) !important",
        },
      }}
    >
      <PhoneInput
        country={country}
        value={value}
        onChange={(phone) => onChange(name, phone)}
        specialLabel={""}
        inputStyle={{
          height: "38px",
          width: "100%",
          borderRadius: "4px",
          border: hasError
            ? "1px solid  #d32f2f"
            : "1px solid rgb(188 188 188)",
          fontSize: "14px !important",
          // background: "transparent",
        }}
        inputProps={{
          name: name,
          required: required,
          pattern: required && ".{6,}",
        }}
      />
      {hasError && (
        <FormHelperText error sx={{ marginLeft: "14px" }}>
          Field Required
        </FormHelperText>
      )}
    </Box>
  );
}
