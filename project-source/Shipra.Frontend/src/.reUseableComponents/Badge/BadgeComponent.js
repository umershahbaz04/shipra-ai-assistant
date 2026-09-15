import React from "react";
import { Box } from "@mui/material";

export default function BadgeComponent(props) {
  const { title, color } = props;
  return (
    <>
      <Box
        sx={{
          border: `0 solid ${color}`,
          color:"#ffffff" ,
          background:color ,
          borderRadius: "4px",
          padding: "3px 7px",
          textAlign: "center",
          fontSize:"11px"
        }}
      >
        {title}
      </Box>
    </>
  );
}
