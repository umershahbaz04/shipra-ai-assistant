import { LoadingButton } from "@mui/lab";
import React from "react";

export default function ButtonComponent(props) {
  const {
    onClick,
    title,
    bg = "var(--primary-color)",
    type,
    loading,
    p,
    variant = "contained",
    sx,
    btnMdWidth,
    btnLgWidth,
  } = props;
  return (
    <LoadingButton
      loading={loading}
      type={type}
      sx={{
        p: p,
        color: variant === "contained" && "#fff",
        border: "1px solid rgba(30, 30, 30, 0.2) !important",
        textTransform: "capitalize !important",
        height: "29px",
        fontSize: { md: "10px", lg: "12px" },
        background: variant === "contained" && bg,
        width: { md: btnMdWidth || "85px", lg: btnLgWidth || "auto" },
        "&:disabled": {
          color: "gray",
          background: "rgba(30, 30, 30, 0.2) !important",
          border: "1px solid rgba(30, 30, 30, 0.2) !important",
        },
        ...sx,
      }}
      variant={variant}
      onClick={onClick}
    >
      {title}
    </LoadingButton>
  );
}
