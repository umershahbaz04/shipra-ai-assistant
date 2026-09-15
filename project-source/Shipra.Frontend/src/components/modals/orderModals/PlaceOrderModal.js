import { Box, CircularProgress, Grid, InputLabel } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useRef, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAllOrderTypeLookup,
  UploadFileFullfilable,
  UploadFileRegular,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { LoadingTextField, purple } from "../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../utilities/toast";
import AutorenewRoundedIcon from "@mui/icons-material/AutorenewRounded";
const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function PlaceOrderModal(props) {
  const [allOrderTypeLookup, setAllOrderTypeLookup] = useState([]);
  const [xlsxFile, setXlsxFile] = useState(null);
  const fileInputRef = useRef(null);
  const [isLoading, setIsLoading] = useState(false);
  const [orderTypeLoading, setOrderTypeLoading] = useState(false);
  const {
    open,
    setOpen,
    setOrderData,
    storesForSelection,
    getStoresForSelection,
    setErrorsList,
    countryId,
    setLoading,
  } = props;
  const {
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    control,
    reset,
    register,
  } = useForm();
  useWatch({
    name: "orderType",
    control,
  });
  useWatch({
    name: "store",
    control,
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
    setLoading(true);
    setIsLoading(true);
    setOrderData([]);
    // console.log("data", data);
    const formData = new FormData();
    formData.append("File", xlsxFile);
    formData.append("CountryId", countryId);
    if (data.orderType.orderTypeId === 1) {
      UploadFileRegular(formData)
        .then((res) => {
          setIsLoading(false);
          if (res?.data?.result && res?.data?.result?.length > 0) {
            let updatedItems = res.data.result.map((item, index) => ({
              ...item,
              actualAmmount: item?.amount,
              index: index + 1,
            }));
            setOrderData(updatedItems);
          }
          if (res?.data?.errors?.FormatException) {
            console.log(
              "res?.data?.errors?.FormatException",
              res?.data?.errors?.FormatException
            );
            for (
              let j = 0;
              j < res?.data?.errors?.FormatException.length;
              j++
            ) {
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
            const errorMessages = JSON.parse(
              res?.data?.errors?.InvalidParameter
            );
            console.log("errorMessages", errorMessages);
            setErrorsList(errorMessages);
            for (let index = 0; index < errorMessages.length; index++) {
              if (!errorMessages[index].IsSuccessed) {
                errorNotification(
                  `Error at row ${index + 1}: ${errorMessages[index].Msg} `
                );
              }
            }
          }
          setValue("orderType", null);
          setValue("store", null);
          setXlsxFile(null);
          // fileInputRef.current.value = "";
          reset();
          setOpen(false);
        })
        .catch((e) => {
          errorNotification(
            LanguageReducer?.languageType
              ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
          );
          console.log("e", e);
        })
        .finally(() => {
          setIsLoading(false);
          setLoading(false);
        });
    } else {
      formData.append("StoreId", data.store?.storeId);
      UploadFileFullfilable(formData)
        .then((res) => {
          setIsLoading(false);
          if (res?.data?.result && res?.data?.result?.length > 0) {
            let updatedItems = res.data.result.map((item, index) => ({
              ...item,
              actualAmmount: item?.amount,
              index: index + 1,
            }));
            setOrderData(updatedItems);
          }

          if (res?.data?.errors?.FormatException) {
            console.log(
              "res?.data?.errors?.FormatException",
              res?.data?.errors?.FormatException
            );
            for (
              let j = 0;
              j < res?.data?.errors?.FormatException.length;
              j++
            ) {
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
            const errorMessages = JSON.parse(
              res?.data?.errors?.InvalidParameter
            );
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
          setValue("orderType", null);
          setValue("store", null);
          setXlsxFile(null);
          // fileInputRef.current.value = "";
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
        })
        .finally(() => {
          setIsLoading(false);
          setLoading(false);
        });
    }
    handleClose();
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
  const getAllOrderTypeLookup = async () => {
    setOrderTypeLoading(true);
    try {
      const response = await GetAllOrderTypeLookup();
      setAllOrderTypeLookup(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    } finally {
      setOrderTypeLoading(false);
    }
  };

  useEffect(() => {
    getAllOrderTypeLookup();
  }, []);
  return (
    <ModalComponent
      open={open}
      onClose={() => {
        setValue("orderType", null);
        setValue("store", null);
        setXlsxFile(null);
        fileInputRef.current.value = "";
        handleClose();
      }}
      maxWidth="sm"
      title={LanguageReducer?.languageType?.PLACE_ORDER_FROM_EXCEL_FILE_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={"Create Order"}
          bg={purple}
          type="submit"
          loading={isLoading}
        />
      }
      component={"form"}
      onSubmit={handleSubmit(load)}
    >
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item md={12} sm={12} xs={12}>
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.SELECT_ORDER_TYPE_TEXT}
            </InputLabel>
          </Box>
          <SelectComponent
            name="orderType"
            control={control}
            options={allOrderTypeLookup}
            optionLabel={EnumOptions.ORDER_TYPE.LABEL}
            optionValue={EnumOptions.ORDER_TYPE.VALUE}
            isRHF={true}
            required={true}
            isLoading={orderTypeLoading}
            disabled={orderTypeLoading}
            isRefesh={true}
            handleRefreshClick={getAllOrderTypeLookup}
            value={getValues("orderType")}
            {...register("orderType", {
              required: {
                value: true,
              },
            })}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("orderType", resolvedId);
            }}
            errors={errors}
          />
        </Grid>

        {getValues("orderType")?.orderTypeId === 2 ? (
          <Grid item md={12} sm={12} xs={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.SELECT_STORE_TEXT}
            </InputLabel>
            <SelectComponent
              name="store"
              control={control}
              options={storesForSelection}
              value={getValues("store")}
              isRHF={true}
              required={true}
              isRefesh={true}
              handleRefreshClick={getStoresForSelection}
              optionLabel={EnumOptions.STORE.LABEL}
              optionValue={EnumOptions.STORE.VALUE}
              {...register("store", {
                required: {
                  value: true,
                },
              })}
              onChange={(event, newValue) => {
                const resolvedId = newValue ? newValue : null;
                setValue("store", resolvedId);
              }}
              errors={errors}
            />
          </Grid>
        ) : null}
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
export default PlaceOrderModal;
