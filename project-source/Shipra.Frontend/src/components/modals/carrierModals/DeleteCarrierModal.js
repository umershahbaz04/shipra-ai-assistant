import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import {
  Avatar,
  Box,
  Button,
  CardHeader,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  FormControl,
  Grid,
  InputAdornment,
  InputLabel,
  MenuItem,
  OutlinedInput,
  TextField,
  Typography,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import {
  CreateStore,
  DeleteStoreById,
  GetAllCountry,
  GetAllRegionbyCountryId,
  GetCityByRegionId,
  UploadStoreImage,
} from "../../../api/AxiosInterceptors";
import ConfirmationIcon from "../../../assets/images/confirmationIcon.png";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { styleSheet } from "../../../assets/styles/style";
import { LoadingButton } from "@mui/lab";
import { red } from "../../../utilities/helpers/Helpers";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function DeleteCarrierModal(props) {
  let { open, setOpen, setIsDeletedConfirm, loading } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const handleClose = () => {
    setOpen(false);
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.DELETE_TEXT}
          loading={loading}
          bg={red}
          onClick={() => setIsDeletedConfirm(true)}
        />
      }
    >
      <Box sx={{width: "500px" }}>
        <CardHeader
          avatar={
            <Avatar
              sx={{
                bgcolor: "transparent",
                fontSize: "11px",
                fontWeight: "500",
                width: 100,
                height: 100,
              }}
              variant="square"
            >
              <img width="100%" height="100%" src={ConfirmationIcon}></img>
            </Avatar>
          }
          title={
            <Typography sx={styleSheet.cardTitleOrder}>
              {LanguageReducer?.languageType?.CONFIRMATION_STORE_DELETE_TOAST}
            </Typography>
          }
          subheader={
            <Typography sx={styleSheet.cardDesOrder}>
              {
                LanguageReducer?.languageType
                  ?.SUPPORT_CONFIRMATION_STORE_DELETE_TOAST
              }
            </Typography>
          }
        />
      </Box>
    </ModalComponent>
  );
}
export default DeleteCarrierModal;
