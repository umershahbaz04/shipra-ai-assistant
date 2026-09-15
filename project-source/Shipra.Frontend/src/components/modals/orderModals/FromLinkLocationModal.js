import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  InputLabel,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import PushPinIcon from "@mui/icons-material/PushPin";
import { useForm } from "react-hook-form";
import { errorNotification } from "../../../utilities/toast";
import { placeholders, purple } from "../../../utilities/helpers/Helpers";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function FromLinkLocationModal(props) {
  const { open, setOpen, setValue } = props;
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm();

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    setOpen(false);
  };
  const getLatLng = (data) => {
    const regex = /@([-+]?\d+\.\d+),([-+]?\d+\.\d+)/;
    const matches = data.link.match(regex);

    if (matches && matches.length >= 3) {
      const latitude = parseFloat(matches[1]);
      const longitude = parseFloat(matches[2]);

      console.log("Latitude:", latitude);
      console.log("Longitude:", longitude);
      setValue("latitude", latitude);
      setValue("longitude", longitude);
      reset();
      handleClose();
    } else {
      console.log("Latitude and longitude not found in the URL.");
      errorNotification(
        LanguageReducer?.languageType
          ?.LATITUDE_AND_LONGITUDE_NOT_FOUND_IN_THE_URL_TOAST
      );
    }
  };
  const handleFocus = (event) => event.target.select();
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={LanguageReducer?.languageType?.LOCATION_FROM_LINK_TEXT}
      actionBtn={
        <ModalButtonComponent title={"Add"} bg={purple} type="submit" />
      }
      component={"form"}
      onSubmit={handleSubmit(getLatLng)}
    >
      <InputLabel sx={styleSheet.inputLabel}>
        {LanguageReducer?.languageType?.FROM_LINK_TEXT}
      </InputLabel>
      <TextField
        placeholder={placeholders.url}
        onFocus={handleFocus}
        type="text"
        size="small"
        id="link"
        name="link"
        fullWidth
        variant="outlined"
        {...register("link", {
          required: {
            value: true,
            message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
          },
          pattern: {
            value:
              /^(?! *$)(?:[a-zA-Z]+:)?\/\/(?:\w+:{0,1}\w*@)?(?:[\w.+-]+|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(?::\d+)?(?:\/[^\s]*)?$/,
            message:
              LanguageReducer?.languageType?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
          },
        })}
        error={Boolean(errors.link)} // set error prop
        helperText={errors.link?.message}
      />
    </ModalComponent>
  );
}
export default FromLinkLocationModal;
