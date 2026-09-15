import React, { useState } from "react";
import {
  Box,
  Typography,
  InputLabel,
  TextField,
  FormControl,
  OutlinedInput,
  InputAdornment,
  IconButton,
  MenuItem,
  Button,
} from "@mui/material";
import { styleSheet } from "../../../../assets/styles/style.js";
import { useSelector } from "react-redux";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import { placeholders } from "../../../../utilities/helpers/Helpers.js";

function SettingProfile(props) {
  const [values, setValues] = useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClickShowPassword = () => {
    setValues({
      ...values,
      showPassword: !values.showPassword,
    });
  };

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };
  return (
    <Box sx={styleSheet.profileSettingArea}>
      <Typography sx={styleSheet.profileSettingHeading} variant="h5">
        {"Profit"}
      </Typography>
      <Box sx={{ mb: "15px" }}>
        <InputLabel sx={styleSheet.inputLabel}>{"Name"}</InputLabel>
        <TextField
          placeholder={"Enter Name"}
          size="small"
          fullWidth
          variant="outlined"
        />
      </Box>
      <Box sx={{ mb: "15px" }}>
        <InputLabel sx={styleSheet.inputLabel}>{"Email"}</InputLabel>
        <TextField
          placeholder={placeholders.price}
          size="small"
          fullWidth
          variant="outlined"
        />
      </Box>
      <Box sx={{ mb: "15px" }}>
        <InputLabel sx={styleSheet.inputLabel}>{"Role"}</InputLabel>
        <TextField
          placeholder={"Account Owner"}
          size="small"
          fullWidth
          variant="outlined"
        />
      </Box>
      <Box sx={{ mb: "15px" }}>
        <InputLabel sx={styleSheet.inputLabel}>{"Password"}</InputLabel>
        <FormControl fullWidth variant="outlined">
          <OutlinedInput
            id="outlined-adornment-password"
            type={values.showPassword ? "text" : "password"}
            value={values.password}
            onChange={(e) => setValues({ ...values, password: e.target.value })}
            endAdornment={
              <InputAdornment position="end">
                <IconButton
                  aria-label="toggle password visibility"
                  onClick={handleClickShowPassword}
                  onMouseDown={handleMouseDownPassword}
                  edge="end"
                >
                  {values.showPassword ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              </InputAdornment>
            }
            size="small"
            fullWidth
          />
        </FormControl>
        <InputLabel sx={styleSheet.inputLabelBottom}>
          {"Change Password"}
        </InputLabel>
      </Box>
      <Box sx={{ mb: "25px" }}>
        <InputLabel sx={styleSheet.inputLabel}>{"Language"}</InputLabel>
        <TextField
          defaultValue={"English"}
          select
          size="small"
          fullWidth
          variant="outlined"
        >
          <MenuItem value="" selected disabled>
            Select
          </MenuItem>
          <MenuItem value="English">English</MenuItem>
          <MenuItem value="Arabic">Arabic</MenuItem>
        </TextField>
      </Box>
      <Button sx={styleSheet.saveButtonUI} variant="contained" fullWidth>
        {"Save"}
      </Button>
    </Box>
  );
}
export default SettingProfile;
