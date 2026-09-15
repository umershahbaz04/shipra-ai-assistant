import { Autocomplete, TextField } from "@mui/material";
import React from "react";
import { useState } from "react";
import { useEffect } from "react";

export default function CustomAutocompleteFieldFilter(props) {
  const [defaultValue, setDefaultValue] = useState(null);
  const [defaultOptions, setDefaultOptions] = useState([]);
  let padding = props?.padding ? props?.padding : "1px !important";
  const {
    options,
    value,
    getOptionLabel = () => {},
    onChange = () => {},
    noOptionsText,
    sx = {
      "& .MuiInputBase-root": {
        padding: padding,
        width: "100%",
      },
      "& .MuiAutocomplete-input": {
        padding: "3.5px 4px 3.5px 8px !important",
        fontSize: "14px",
      },
    },
    disablePortal,
    multiple = false,
  } = props;
  useEffect(() => {
    if (options.length > 0) {
      const data = options;
      const df_value = value ? data.find((val) => val == val) : data[0];
      setDefaultValue(df_value);
      setDefaultOptions(data);
    } else {
      const data = options;
      setDefaultOptions(data);
    }
  }, [options]);

  return (
    <>
      <Autocomplete
        defaultValue={defaultValue || ""}
        value={value || defaultValue}
        fullWidth
        multiple={multiple}
        onChange={onChange}
        sx={sx}
        getOptionLabel={getOptionLabel}
        ChipProps={{
          sx: {
            height: 20,
          },
        }}
        ListboxProps={{ style: { maxHeight: 150 } }}
        disablePortal={props?.disablePortal}
        options={defaultOptions}
        renderInput={(params) => {
          return (
            <TextField
              {...params}
              size={props?.size || "small"}
              error={props?.error}
              label={props?.label}
              helperText={props?.error && props?.error?.message}
            />
          );
        }}
        {...props}
        // noOptionsText={noOptionsText || 'No options found'}
      />
    </>
  );
}
