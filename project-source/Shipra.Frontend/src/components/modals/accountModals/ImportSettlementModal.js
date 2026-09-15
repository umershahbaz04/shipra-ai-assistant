import { Grid, InputLabel } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useRef, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { UplaodCarrierSettlementFile } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function ImportSettlementModal(props) {
  let {
    open,
    setOpen,
    setSettlementData,
    setErrorsList,
    carrierData,
    setCarrierId,
  } = props;
  const [xlsxFile, setXlsxFile] = useState(null);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const fileInputRef = useRef(null);
  const [isLoading, setIsLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    watch,
    reset,
    control,
  } = useForm();
  useWatch({
    name: "carrier",
    control,
  });
  const loadFile = (data) => {
    setIsLoading(true);
    console.log("data", data);
    console.log("xlsxFile", xlsxFile);
    if (!xlsxFile) {
      errorNotification(LanguageReducer?.languageType?.FILE_REQURED_TOAST);
    }
    const formData = new FormData();
    formData.append("File", xlsxFile);
    formData.append("CarrierId", data.carrier?.carrierId);

    UplaodCarrierSettlementFile(formData)
      .then((res) => {
        setIsLoading(false);
        console.log("res", res);
        if (res?.data?.result && res?.data?.result?.length > 0) {
          console.log("HELLOO");
          let updatedItems = res.data.result.map((item, index) => ({
            ...item,
            index: index + 1,
          }));
          setSettlementData(updatedItems);
          handleClose();
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
        if (!res?.data?.errorMessages) {
          UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
        }
      })
      .catch((e) => {
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
        );
        console.log("e", e);
        setIsLoading(false);
      });
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
  const handleClose = () => {
    setOpen(false);
  };

  return (
    <ModalComponent
      open={open}
      onClose={() => {
        setXlsxFile(null);
        fileInputRef.current.value = "";
        handleClose();
      }}
      maxWidth="sm"
      title={LanguageReducer?.languageType?.CREATE_SETTLEMENT_FROM_FILE_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.LOAD_TEXT}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(loadFile)}
    >
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SELECT_CARRIER_TEXT}
          </InputLabel>
          <SelectComponent
            name="carrier"
            control={control}
            options={carrierData}
            optionLabel={EnumOptions.CARRIER.LABEL}
            optionValue={EnumOptions.CARRIER.VALUE}
            isRHF={true}
            required={true}
            {...register("carrier", {
              required: {
                value: true,
              },
            })}
            value={getValues("carrier")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("carrier", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
      </Grid>
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
export default ImportSettlementModal;
