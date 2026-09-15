import React from "react";
import TextField from "@mui/material/TextField";
import { useForm } from "react-hook-form";

const CustomLatLongTextField = ({
  register,
  errors,
  name,
  required,
  ...props
}) => {
  const handleKeyPress = (event) => {
    const charCode = event.charCode;
    if (
      (charCode >= 48 && charCode <= 57) || // Numbers 0-9
      charCode === 44 || // Comma
      charCode === 46 // Period
    ) {
      return true;
    } else {
      event.preventDefault();
      return false;
    }
  };

  return (
    <TextField
      type="text"
      placeholder="latitude, longitude"
      size="small"
      fullWidth
      variant="outlined"
      id="lat&long"
      name={name}
      {...register(name, {
        required: required ? "This field is required" : false,
        pattern: {
          value: /^-?\d+(\.\d+)?\s*,\s*-?\d+(\.\d+)?$/,
          message:
            "Please enter valid latitude and longitude separated by a comma",
        },
        validate: (value) => {
          if (value) {
            const [latitude, longitude] = value
              .split(",")
              .map((str) => str.trim());

            if (!latitude.includes(".") || !longitude.includes(".")) {
              return "Both latitude and longitude must contain a period.";
            }
          }
          return true;
        },
      })}
      error={!!errors["lat&long"]}
      helperText={errors["lat&long"] ? errors["lat&long"].message : ""}
      inputProps={{
        onKeyPress: handleKeyPress,
      }}
      {...props}
    />
  );
};

export default CustomLatLongTextField;
