import { Autocomplete, TextField } from "@mui/material";
import React from "react";

export default function AutoCompleteTextFieldComponent(props) {
  const {
    options = [],
    optionLabel,
    value,
    onChange,
    name,
    errors = [],
    required = true,
    disabled,
  } = props;
  return (
    <Autocomplete
      size="small"
      disabled={disabled}
      value={value}
      onChange={(e, val) => onChange(name, val)}
      disablePortal
      options={options}
      getOptionLabel={(option) => option[optionLabel]}
      sx={{
        width: "100%",
        "& .MuiInputBase-root": {
          borderRadius: "4px",
          background: "#fff",
        },
      }}
      renderInput={(params) => (
        <TextField
          {...params}
          name={name}
          error={!disabled && required && errors.includes(name)}
          required={required}
          helperText={
            !disabled &&
            required &&
            errors.indexOf(name) > -1 &&
            "Field Required"
          }
          inputProps={{
            ...params.inputProps,
            pattern: "^(?!.*[Pp][Ll][Ee][Aa][Ss][Ee]).*",
          }}
        />
      )}
    />
  );
}
