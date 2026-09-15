import { LoadingButton } from "@mui/lab";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from "@mui/material";
import { styleSheet } from "../../assets/styles/style";
import React from "react";
import { useLanguageReducer } from "../../utilities/helpers/Helpers";

export default function ModalComponent(props) {
  const {
    open,
    onClose,
    title,
    maxWidth = "xl",
    fullScreen = false,
    children,
    loading,
    actionBtn,
    component,
    onSubmit,
    onInvalid,
    height = "auto",
    paddingBottom,
    paddingTop,
    disabledDismissBtn,
  } = props;

  const LanguageReducer = useLanguageReducer();
  return (
    <Dialog
      open={open}
      onClose={onClose}
      fullWidth={true}
      fullScreen={fullScreen}
      maxWidth={maxWidth}
      PaperProps={{
        sx: {
          borderRadius: "20px",
          height: height,
          position: "unset !important",
        },
        component: component,
        onSubmit: onSubmit,
        onInvalid: onInvalid,
      }}
    >
      <DialogTitle
        textAlign={"center"}
        paddingBottom={paddingBottom}
        paddingTop={paddingTop}
      >
        <Typography fontSize={25} fontWeight={600}>
          {title}
        </Typography>
      </DialogTitle>
      <DialogContent>{children}</DialogContent>
      <DialogActions sx={{ p: 0 }}>
        <Button
          fullWidth
          variant="contained"
          disabled={disabledDismissBtn}
          sx={{
            ...styleSheet.modalDismissButton,
            borderBottomLeftRadius: "20px !important",
          }}
          onClick={onClose}
        >
          {LanguageReducer.DISMISS_TEXT}
        </Button>
        {actionBtn}
      </DialogActions>
    </Dialog>
  );
}
