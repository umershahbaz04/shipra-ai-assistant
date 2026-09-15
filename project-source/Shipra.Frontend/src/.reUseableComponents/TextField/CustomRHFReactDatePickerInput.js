import React from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { Controller } from "react-hook-form";
import TextField from "@mui/material/TextField";
import "../../assets/styles/datePickerCustomStyles.css";

export default function CustomRHFReactDatePickerInput(props) {
  return (
    <>
      <Controller
        name={props?.name}
        control={props?.control}
        defaultValue={props?.defaultValue}
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
            <DatePicker
              wrapperClassName={"datepickerContainer"}
              customInput={
                <TextField
                  fullWidth
                  inputProps={props?.inputProps}
                  size={props?.size || "small"}
                  label={props?.label}
                  variant={props?.variant ? props?.variant : "outlined"}
                  value={value ? value : null}
                  error={error}
                  helperText={error && error?.message}
                  ref={ref}
                  placeholder={props?.placeholder}
                />
              }
              selected={value ? field.value : null}
              onChange={(date) => {
                const resolvedId = date ? date : null;
                onChange(resolvedId);
                if (props?.onChange) {
                  props?.onChange(resolvedId);
                }
              }}
              {...props}
            />
          );
        }}
      />
    </>
  );
}
