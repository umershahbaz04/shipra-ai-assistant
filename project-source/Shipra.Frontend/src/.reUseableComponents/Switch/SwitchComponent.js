import { Box, InputLabel, Switch } from "@mui/material";
import { styleSheet } from "../../assets/styles/style";

export const SwitchComponent = ({
  l_side,
  r_side,
  checked,
  onChange,
  disabled,
  size = "medium",
}) => {
  return (
    <Box className={"flex_center"}>
      <InputLabel required={false} sx={styleSheet.inputLabel}>
        {l_side}
      </InputLabel>
      <Switch
        size={size}
        value={checked}
        checked={checked}
        onChange={onChange}
        disabled={disabled}
      />
      <InputLabel required={false} sx={styleSheet.inputLabel}>
        {r_side}
      </InputLabel>
    </Box>
  );
};
