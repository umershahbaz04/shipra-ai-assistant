import FormControlLabel from "@mui/material/FormControlLabel";
import FormGroup from "@mui/material/FormGroup";
import Switch from "@mui/material/Switch";
import * as React from "react";
export default function SwitchMui({
  checked,
  sx = {
    mx: 1,
  },
  label = "",
  onChange = () => {},
  size = "medium",
}) {
  return (
    <FormGroup>
      <FormControlLabel
        sx={{ ...sx, "& .MuiTypography-root": { fontSize: 12 }, p: 0, m: 0 }}
        control={
          <Switch
            sx={{ mx: 0 }}
            checked={checked}
            onChange={onChange}
            size={size}
          />
        }
        label={label}
      />
    </FormGroup>
  );
}
