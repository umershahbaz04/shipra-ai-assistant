import React from "react";
import BlockIcon from "@mui/icons-material/Block";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";

function UnauthorizedPage({
  title = "Unauthorized Access",
  message = "You are unauthorized to access any menu items. Please contact your system administrator.",
}) {
  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
        minHeight: "75vh",
        width: "100%",
        padding: "40px 20px",
        textAlign: "center",
      }}
    >
      <BlockIcon sx={{ fontSize: 120, color: "#d32f2f", mb: 2 }} />
      <Typography
        variant="h4"
        component="h1"
        sx={{
          color: "#d32f2f",
          fontWeight: 700,
          fontSize: { xs: "22px", md: "28px" },
          mb: 2,
          fontFamily: "'Lato', 'Inter', sans-serif",
        }}
      >
        {title}
      </Typography>
      <Typography
        variant="body1"
        sx={{
          color: "#d32f2f",
          fontSize: { xs: "15px", md: "18px" },
          fontWeight: 500,
          maxWidth: "650px",
          lineHeight: 1.6,
          fontFamily: "'Lato', 'Inter', sans-serif",
        }}
      >
        {message}
      </Typography>
    </Box>
  );
}

export default UnauthorizedPage;
