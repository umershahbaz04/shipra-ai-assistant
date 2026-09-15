import { TextField } from "@mui/material";
import React from "react";

export const TextFieldSearchComponent = (props) => {
  const { size, error, label, sx, ...others } = props;
  return (
    <TextField
      fullWidth
      size={size || "small"}
      error={error}
      label={label}
      helperText={error && error.message}
      sx={{
        "& .css-imsvlo-MuiInputBase-input-MuiOutlinedInput-input": {
          padding: "4px 5px",
        },
        ...sx,
      }}
      {...others}
    />
  );
};
