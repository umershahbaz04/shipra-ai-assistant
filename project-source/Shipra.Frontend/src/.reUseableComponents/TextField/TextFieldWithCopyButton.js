import ContentCopyOutlined from '@mui/icons-material/ContentCopyOutlined';
import Visibility from '@mui/icons-material/Visibility';
import {
  Box,
  FormControl,
  IconButton,
  InputAdornment,
  OutlinedInput,
  TextField,
  Tooltip,
} from "@mui/material";
import React from "react";
import UtilityClass from "../../utilities/UtilityClass";
import { placeholders } from "../../utilities/helpers/Helpers";

export default function TextFieldWithCopyButton(props) {
  const { value = "", width, size, startAdornment } = props;
  return (
    <>
      <FormControl
        {...props}
        sx={{
          width: width || "100%",
          "& .MuiOutlinedInput-root": {
            paddingLeft: "0px",
            fontSize:"14px"
          },
        }}
        variant="outlined"
      >
        <OutlinedInput
         placeholder={placeholders.url}
          type={"text"}
          value={value}
          sx={{
            "& .MuiInputBase-input": {
              overflow: "hidden",
              textOverflow: "ellipsis",
              padding: "0px",
            },
          }}
          endAdornment={
            <InputAdornment position="end">
              <IconButton
                aria-label="toggle password visibility"
                // onClick={handleClickShowPassword}
                // onMouseDown={handleMouseDownPassword}
                edge="end"
              >
                <Tooltip title="Copy to Clipboard">
                  <ContentCopyOutlined
                    sx={{ cursor: "pointer" }}
                    onClick={(e) => UtilityClass.copyToClipboard(value)}
                  />
                </Tooltip>
              </IconButton>
            </InputAdornment>
          }
          startAdornment={startAdornment}
          size={size || "small"}
        />
      </FormControl>
      {/* <TextField
        placeholder="Password"
        InputProps={{
          startAdornment: (
            <InputAdornment
              position="start"
              sx={{
                padding: "27.5px 14px",
                backgroundColor: (theme) => "red",
                borderTopLeftRadius: (theme) => theme.shape.borderRadius + "px",
                borderBottomLeftRadius: (theme) =>
                  theme.shape.borderRadius + "px",
              }}
            >
              POST
            </InputAdornment>
          ),
        }} 
      />*/}
    </>
  );
}
