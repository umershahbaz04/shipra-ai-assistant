import { Box, FormControlLabel, InputLabel, Switch } from "@mui/material";
import React from "react";
import { styleSheet } from "../../assets/styles/style";

export default function LabelWithCheckBox({
  onChange = () => {},
  checked = false,
  inputLabel = "",
  required = false,
  checkedLabel = "Active",
  unCheckedLabel = "Deactive",
  isShowSwitch = true,
  disabled = false,
}) {
  return (
    <>
      <Box position={"relative"}>
        <InputLabel required={required} sx={styleSheet.inputLabel}>
          {inputLabel}
        </InputLabel>
        {isShowSwitch && (
          <FormControlLabel
            sx={{ position: "absolute", top: -4, right: -12 }}
            labelPlacement="left"
            control={
              <Switch
                size="small"
                value={checked}
                checked={checked}
                onChange={onChange}
                disabled={disabled}
              />
            }
            componentsProps={{
              typography: {
                fontSize: 12,
                fontWeight: 400,
                color: "#000000",
              },
            }}
            label={checked ? checkedLabel : unCheckedLabel}
          />
        )}
      </Box>
    </>
  );
}
