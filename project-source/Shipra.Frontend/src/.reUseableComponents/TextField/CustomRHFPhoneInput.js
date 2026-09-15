import { Box, FormHelperText, Stack } from "@mui/material";
import React from "react";
import PhoneInput from "react-phone-input-2";
import "react-phone-input-2/lib/material.css";

import { Controller } from "react-hook-form";
import "../../assets/styles/phoneInput.css";
import { EnumCookieKeys } from "../../utilities/enum";
import { getThisKeyCookie } from "../../utilities/cookies";

function CustomRHFPhoneInput(props) {
  const {
    control,
    name,
    inputAdornment,
    borderRadius = "4px",
    selectedCountry = getThisKeyCookie(EnumCookieKeys.COUNTRY_CODE),
    style,
    isContact,
    required,
    error,
  } = props;
  return (
    <Box
      sx={{
        "& .react-tel-input": {
          width: "100%",
        },
        "& .react-tel-input input": {
          backgroundColor: style?.bg ? style?.bg : "inherit !important",
          fontSize: style?.fs,
        },
        "& .react-tel-input .form-control": {
          width: "100% !important",
          height: "20px",
          borderRadius: borderRadius,
        },
        "& .react-tel-input .form-control:focus": {
          borderColor: "#563adf",
          boxShadow: "0 0 0 1px #563adf",
        },
        "& .error-border": {
          borderColor: "#d32f2f !important",
        },
      }}
    >
      <Controller
        name={name}
        control={control}
        rules={
          isContact
            ? {
                minLength: {
                  value: 9,
                  message: "Phone number must be at least 6 digits",
                },
              }
            : required
            ? {
                required: {
                  value: true,
                  message: error,
                },
                minLength: {
                  value: 9,
                  message: "Phone number must be at least 6 digits",
                },
              }
            : {}
        }
        render={({ field: { onChange, value }, fieldState: { error } }) => {
          return (
            <>
              <Stack sx={{ position: "relative" }} direction={"row"}>
                <PhoneInput
                  inputClass={error?.message ? "error-border" : ""}
                  value={value}
                  onChange={onChange}
                  defaultCountry={"ae"}
                  id={name}
                  autoFormat={true}
                  specialLabel=""
                  enableSearch={true}
                  defaultErrorMessage=""
                  prefix="+"
                  country={selectedCountry}
                  {...props}
                />
                {inputAdornment && inputAdornment}
              </Stack>
              {error?.message && (
                <FormHelperText sx={{ color: "#d32f2f", marginLeft: "14px" }}>
                  {error.message}
                </FormHelperText>
              )}
            </>
          );
        }}
      />
    </Box>
  );
}

export default CustomRHFPhoneInput;
