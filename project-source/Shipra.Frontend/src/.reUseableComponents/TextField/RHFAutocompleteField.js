import { Autocomplete, TextField, createFilterOptions } from "@mui/material";
import React from "react";
import { Controller } from "react-hook-form";

export default function RHFAutocompleteField(props) {
  const { options, getOptionLabel, defaultValue } = props;
  //   const OPTIONS_LIMIT = 10;
  // const defaultFilterOptions = createFilterOptions();
  // const filterOptions = (options, state) => {
  //   return defaultFilterOptions(options, state).slice(0, OPTIONS_LIMIT);
  // };
  const defaultVal = defaultValue
    ? defaultValue
    : {
        id: 0,
        text: "Please select",
      };
      // debugger
  return (
    <>
      <Controller
        defaultValue={defaultVal}
        name={props?.name}
        control={props?.control}
        rules={
          props?.required
            ? {
                required: {
                  value: true,
                  message: props?.error,
                },
              }
            : {}
        }
        render={({ field, fieldState: { error } }) => {
          const { onChange, value, ref } = field;
          return (
            <Autocomplete
              value={
                value
                  ? options?.find((option) => {
                      return value === option?.id;
                    }) ?? defaultVal
                  : defaultVal
              }
              onChange={(event, newValue) => {
                const resolvedId = newValue ? newValue.id : null;
                onChange(resolvedId);
                props.onChange?.(resolvedId);
              }}
              ListboxProps={{ style: { maxHeight: 150 } }}
              sx={props?.sx}
              getOptionLabel={getOptionLabel}
              disablePortal
              options={options}
              renderInput={(params) => (
                <TextField
                  {...params}
                  size={props.size || "small"}
                  error={error}
                  helperText={error && error?.message}
                  inputRef={ref}
                />
              )}
            />
          );
        }}
      />
    </>
  );
}
