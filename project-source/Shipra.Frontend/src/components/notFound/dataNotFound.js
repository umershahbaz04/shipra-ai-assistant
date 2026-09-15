import React from "react";
import ErrorOutlineRoundedIcon from "@mui/icons-material/ErrorOutlineRounded";
import { Box } from "@mui/material";
import { styleSheet } from "../../assets/styles/style";
function DataNotFound({ icon, title, fontSize, color,component }) {
  return (
    <Box sx={styleSheet.NotFoundRoot} component={component?component:""}>
      <Box sx={styleSheet.NotFoundContent}>
        {icon ? icon : <ErrorOutlineRoundedIcon sx={{ fontSize: "60px", color: "#BDBDBD" }} fontSize="large" />}
        <br />
        <Box sx={{ color: color ? color : "", fontSize: fontSize ? fontSize : "" }}> {title ? title : "Not Exists"}</Box>
      </Box>
    </Box>
  );
}
export default (DataNotFound);
