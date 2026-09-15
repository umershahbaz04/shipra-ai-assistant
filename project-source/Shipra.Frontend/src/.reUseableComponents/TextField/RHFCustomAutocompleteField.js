import { Autocomplete, TextField, createFilterOptions } from "@mui/material";
import React, { useCallback } from "react";
import { useEffect } from "react";
import { useState } from "react";
import { Controller } from "react-hook-form";

export default function RHFCustomAutocompleteField(props) {
  const {
    options = [],
    value,
    name,
    optionProperty = "",
    setValue = () => {},
    getOptionLabel = () => {},
    onChange = () => {},
    noOptionsText,
    rules = {
      required: {
        value: true,
        message: props?.error,
      },
    },
    // defaultValue = {},
    sx = {
      "& .MuiInputBase-root": {
        padding: "1px !important",
        width: "100%",
      },
    },
    multiple = false,
  } = props;
  const [defaultValue, setDefaultValue] = useState(null);
  const [defaultOptions, setDefaultOptions] = useState([]);
  useEffect(() => {
    if (options.length > 0) {
      const data = options;

      let defaultIndex = 0;
      if (setValue) {
        const selectedValueIndex = data?.findIndex((x) =>
          x[optionProperty]?.toLowerCase().includes("please")
        );

        defaultIndex = selectedValueIndex != -1 ? selectedValueIndex + 1 : 0;
        const defaultObject = data[defaultIndex];
        setValue(name, defaultObject);
      }

      const df_value = value
        ? data.find((val) => val == val)
        : data[defaultIndex];

      // console.log(data, df_value, value);
      setDefaultValue(df_value);
      setDefaultOptions(data);
    } else {
      const data = options;
      setDefaultOptions(data);
    }
  }, [options]);
  return (
    <>
      <Controller
        value={props?.value || defaultValue}
        name={props?.name}
        control={props?.control}
        rules={props.required && rules}
        render={({ field, fieldState: { error } }) => {
          const { onChange, value, ref } = field;
          return (
            <Autocomplete
              defaultValue={defaultValue || ""}
              value={props?.value || defaultValue}
              onChange={props?.onChange}
              ListboxProps={{ style: { maxHeight: 160 } }}
              sx={props?.sx}
              getOptionLabel={getOptionLabel}
              disablePortal
              options={defaultOptions}
              disabled={props?.disabled}
              renderInput={(params) => {
                return (
                  <TextField
                    {...params}
                    size={props.size || "small"}
                    error={error}
                    helperText={error && error?.message}
                    inputRef={ref}
                  />
                );
              }}
            />
          );
        }}
      />
    </>
  );
}
