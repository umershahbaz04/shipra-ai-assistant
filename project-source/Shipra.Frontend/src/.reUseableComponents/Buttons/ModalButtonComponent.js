import React from "react";
import { styleSheet } from "../../assets/styles/style";
import { LoadingButton } from "@mui/lab";

export default function ModalButtonComponent(props) {
  const {
    title,
    onClick,
    loading,
    type,
    bg,
    disabled = false,
    showBorderBottomRightRadius = true,
  } = props;
  return (
    <LoadingButton
      type={type}
      fullWidth
      loading={loading}
      disabled={disabled}
      variant="contained"
      sx={{
        ...styleSheet.modalCarrierSubmitButton,
        borderBottomRightRadius:
          showBorderBottomRightRadius && "20px !important",
        marginLeft: "0px !important",
        background: disabled
          ? "#e0e0e0 !important"
          : (bg ||
            "var(--primary-color)"),
        color: disabled ? "#9e9e9e !important" : "#fff !important",
      }}
      onClick={onClick}
    >
      {title}
    </LoadingButton>
  );
}
