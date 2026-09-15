import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Grid,
  InputLabel,
  MenuItem,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useRef, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import {
  GetActiveCarriersForSelection,
  UplaodReturnReportFile,
  UploadFileFullfilable,
  UploadFileRegular,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { errorNotification } from "../../../utilities/toast";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function CarrierReturnReportModal(props) {
  const [xlsxFile, setXlsxFile] = useState(null);
  const fileInputRef = useRef(null);
  const [isLoading, setIsLoading] = useState(false);

  const { open, setOpen, setReportData, setErrorsList, carrierId } = props;
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    watch,
    reset,
    control,
  } = useForm({
    defaultValues: {
      carrierId: 0,
    },
  });

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    setOpen(false);
  };
  const load = (data) => {
    setErrorsList([]);
    if (!xlsxFile) {
      errorNotification(LanguageReducer?.languageType?.FILE_REQURED_TOAST);
      return;
    }
    setIsLoading(true);
    setReportData([]);
    console.log("data", data);
    const formData = new FormData();
    formData.append("File", xlsxFile);
    formData.append("CarrierId", carrierId);

    UplaodReturnReportFile(formData)
      .then((res) => {
        console.log("res", res);
        setIsLoading(false);
        if (res?.data?.result && res?.data?.result?.length > 0) {
          console.log("HELLOO");

          let updatedItems = res.data.result.map((item, index) => ({
            ...item,
            index: index + 1,
          }));

          setReportData(updatedItems);
        }
        if (res?.data?.errors?.FormatException) {
          console.log(
            "res?.data?.errors?.FormatException",
            res?.data?.errors?.FormatException
          );
          for (let j = 0; j < res?.data?.errors?.FormatException.length; j++) {
            errorNotification(res?.data?.errors?.FormatException[j]);
          }
          return;
        }
        if (res?.data?.errors?.Exception) {
          for (let j = 0; j < res?.data?.errors?.Exception.length; j++) {
            errorNotification(res?.data?.errors?.Exception[j]);
          }
          return;
        }
        if (res?.data?.errors?.InvalidParameter) {
          const errorMessages = JSON.parse(res?.data?.errors?.InvalidParameter);
          console.log("errorMessages", errorMessages);
          setErrorsList(errorMessages);
          for (let index = 0; index < errorMessages.length; index++) {
            if (!errorMessages[index].IsSuccessed) {
              errorNotification(
                `Error at row ${index}: ${errorMessages[index].Msg} `
              );
            }
          }
        }
        if (res?.data?.errors?.OrderNotFound) {
          for (let j = 0; j < res?.data?.errors?.OrderNotFound.length; j++) {
            errorNotification(res?.data?.errors?.OrderNotFound[j]);
          }
          return;
        }
        // setValue("carrierId", null);
        setXlsxFile(null);
        fileInputRef.current.value = "";

        reset();
        setOpen(false);
      })
      .catch((e) => {
        setIsLoading(false);
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
        );
        console.log("e", e);
      });
    // handleClose()
  };
  const handleFileUpload = (event) => {
    const file = event.target.files[0];
    if (
      file &&
      file.type ===
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ) {
      setXlsxFile(file);
      // File is a valid .xlsx file, you can proceed with further operations
    } else {
      errorNotification(LanguageReducer?.languageType?.INVALID_FILE_TYPE_TOAST);
      // Invalid file format, display an error message or handle accordingly
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={() => {
        setValue("carrierId", null);
        setXlsxFile(null);
        fileInputRef.current.value = "";
        handleClose();
      }}
      maxWidth="sm"
      title={"Upload carrier from excel"}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.LOAD_TEXT}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(load)}
    >
      <input
        style={{ paddingTop: "20px" }}
        type="file"
        accept=".xlsx"
        onChange={handleFileUpload}
        ref={fileInputRef}
      />
    </ModalComponent>
  );
}
export default CarrierReturnReportModal;
