import React from "react";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import { Box, InputAdornment, Stack, TextField, Tooltip } from "@mui/material";

export default function TextFieldWithInfoInputAdornment(props) {
  const {
    name,
    value,
    onChange,
    type = "text",
    required = true,
    borderRadius = "10px",
    inputAdornment,
    infoText = "",
    ...others
  } = props;

  return (
    <Stack sx={{ position: "relative" }} direction="row">
      <TextField
        value={value}
        onChange={onChange}
        id={name}
        InputProps={{
          endAdornment: inputAdornment,
        }}
        {...others}
      />
      {infoText && (
        <Box sx={{ position: "absolute", top: -10, right: 0 }}>
          <Tooltip title={infoText}>
            <InputAdornment position="end">
              <ErrorOutlineIcon
                fontSize="small"
                sx={{ color: "#442CAC", cursor: "pointer" }}
              />
            </InputAdornment>
          </Tooltip>
        </Box>
      )}
    </Stack>
  );
}
