import { LoadingButton } from "@mui/lab";
import React from "react";

export default function RoundedButtonComp(props) {
  const {
    onClick,
    title,
    bg = "var(--primary-color)",
    type,
    loading,
    p,
    variant = "contained",
    sx,
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
        height: 40,
        fontSize: 16,
        borderRadius :"50px",
        background: variant === "contained" && bg,
        "&:disabled": {
          color: "gray",
          background: "rgba(30, 30, 30, 0.2) !important",
          border: "1px solid rgba(30, 30, 30, 0.2) !important",
        },
        width: "100%",
        ...sx,
      }}
      variant={variant}
      onClick={onClick}
    >
      {title}
    </LoadingButton>
  );
}
