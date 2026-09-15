import React, { useState } from "react";
import { useSelector } from "react-redux";
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
  Slide,
  TextField,
  Typography,
} from "@mui/material";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { styleSheet } from "../../../assets/styles/style";
import { useForm } from "react-hook-form";
import uploadIcon from "../../../assets/images/placeholder.jpg";
import { CreateStoreChannel } from "../../../api/AxiosInterceptors";
import { LoadingButton } from "@mui/lab";
import { placeholders, purple } from "../../../utilities/helpers/Helpers";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

export default function AddChannelModal(props) {
  let { open, setOpen, getAllStores, storeId } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState();
  const [isLoading, setIsLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    reset,
    control,
  } = useForm();

  const handleClose = () => {
    setOpen(false);
  };
  const createChannel = async (data) => {
    const body = {
      storeId: storeId,
      channelName: data.channelName,
      url: data.channelUrl,
      settingParameters: {
        name: "new config",
        stripeUrl: "dsajjsad",
      },
    };

    CreateStoreChannel(body)
      .then((res) => {
        console.log("res:", res);
        if (!res?.data?.isSuccess) {
          errorNotification(
            LanguageReducer?.languageType?.UNABLE_TO_CREATE_STORE_TOAST
          );
          errorNotification(res?.data?.customErrorMessage);
        } else {
          successNotification(
            LanguageReducer?.languageType?.STORE_CREATED_SUCCESSFULLY_TOAST
          );
          getAllStores();
          handleClose();
          reset();
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification(
          LanguageReducer?.languageType?.UNABLE_TO_CREATE_STORE_TOAST
        );
      });

    console.log("body", body);
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={"Upload carrier from excel"}
      actionBtn={
        <ModalButtonComponent
          title={"Add Channel"}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(createChannel)}
    >
      <Box sx={styleSheet.uploadStoreIconArea}>
        <img
          style={{
            borderRadius: "20px",
            width: "100px",
            height: "100px",
          }}
          src={file ? file && URL.createObjectURL(file) : uploadIcon}
          alt="uploadIcon"
        />
      </Box>
      <Box sx={{ marginBottom: "10px" }}>
        <InputLabel required sx={styleSheet.inputLabel}>
          Channel Name
        </InputLabel>
        <TextField
          placeholder=""
          size="small"
          id="channelName"
          name="channelName"
          fullWidth
          variant="outlined"
          {...register("channelName", {
            required: {
              value: true,
              message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
            },
            pattern: {
              value: /^(?!\s*$).+/,
              message:
                LanguageReducer?.languageType?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
            },
          })}
          error={Boolean(errors.channelName)} // set error prop
          helperText={errors.channelName?.message}
        />
      </Box>
      <Box>
        <InputLabel required sx={styleSheet.inputLabel}>
          Url
        </InputLabel>
        <TextField
          placeholder={placeholders.url}
          size="small"
          id="channelUrl"
          name="channelUrl"
          fullWidth
          variant="outlined"
          {...register("channelUrl", {
            required: {
              value: true,
              message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
            },
            pattern: {
              value: /^(?!\s*$).+/,
              message:
                LanguageReducer?.languageType?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
            },
          })}
          error={Boolean(errors.channelUrl)} // set error prop
          helperText={errors.channelUrl?.message}
        />
      </Box>
    </ModalComponent>
  );
}
