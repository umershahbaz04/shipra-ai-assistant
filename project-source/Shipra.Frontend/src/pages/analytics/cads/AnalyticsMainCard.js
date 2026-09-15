import { Box, Card, Paper, Typography } from "@mui/material";
import React from "react";

export default function AnalyticsMainCard(props) {
  const {
    children,
    title,
    bgColor,
    height,
    width = "100%",
    mt = 1.25,
    titleRightSide,
  } = props;
  return (
    <Box
      sx={{
        width: width,
        borderRadius: { xs: "5px", md: "20px" },
        bgcolor: bgColor,
        padding: 1.25,
        height: height,
        boxShadow: "0px 0px 9px lightgray",
        display: "flex",
        flexDirection: "column",
      }}
    >
      <Box className={"flex_between"}>
        <Typography variant="h4" fontSize={"1.7vh"}>
          {title}
        </Typography>
        {titleRightSide}
      </Box>
      <Box
        sx={{
          height: height,
          flexGrow: 1,
          display: "flex",
          flexDirection: "column",
          gap: { xs: "5px", md: "3%" },
          mt: mt,
        }}
      >
        {children}
      </Box>
    </Box>
  );
}
