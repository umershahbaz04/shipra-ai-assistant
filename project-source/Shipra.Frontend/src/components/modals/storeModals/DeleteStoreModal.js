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
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function DeleteStoreModal(props) {
  let { open, setOpen, storeId, getAllStores } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState();
  const [values, setValues] = useState({});
  const [allCountries, setAllCountries] = useState([]);
  const [selectedCountry, setSelectedCountry] = useState();
  const [allRegions, setAllRegions] = useState([]);
  const [selectedRegion, setSelectedRegion] = useState();
  const [allCities, setAllCities] = useState([]);
  const [selectedCity, setSelectedCity] = useState();

  const handleClose = () => {
    setOpen(false);
  };
  const deleteStore = async () => {
    const body = {
      storeId: storeId,
    };
    console.log("body", body);

    DeleteStoreById(body)
      .then((res) => {
        console.log("res:", res);
        if (!res?.data?.isSuccess) {
          errorNotification(
            LanguageReducer?.languageType?.UNABLE_TO_DELETE_STORE_TOAST
          );
          errorNotification(res?.data?.customErrorMessage);
        } else {
          successNotification(
            LanguageReducer?.languageType?.STORE_DELETED_SUCCESSFULLY_TOAST
          );
          getAllStores();
          handleClose();
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification(
          LanguageReducer?.languageType?.UNABLE_TO_DELETE_STORE_TOAST
        );
      });
    // setThisKeyCookie("token", data.data.token)
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
          bg={purple}
          onClick={deleteStore}
        />
      }
    >
      <Box>
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
export default DeleteStoreModal;
