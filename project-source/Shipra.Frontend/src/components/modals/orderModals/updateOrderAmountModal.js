import { Box, FormLabel, Grid, InputLabel, TextField } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { UpdateOrderAmount } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function UpdateOrderAmountModal(props) {
  let {
    open,
    setOpen,
    setSelectedRowData,
    selectedRowData,
    getAll,
    codText = "COD",
  } = props;
  const [isLoading, setIsLoading] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
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
  const updateAmount = (data) => {
    setIsLoading(true);
    let params = {
      orderNo: selectedRowData.OrderNo,
      amount: data.cod,
    };

    UpdateOrderAmount(params)
      .then((res) => {
        getAll();
        setIsLoading(false);
        successNotification(res?.data?.result?.message);
        reset();
        handleClose();
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

  const handleClose = () => {
    setSelectedRowData(null);
    setOpen(false);
  };
  useEffect(() => {
    setValue("cod", selectedRowData?.Amount);
  }, [selectedRowData]);
  const handleFocus = (event) => event.target.select();
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={LanguageReducer?.languageType?.UPDATE_ORDER_AMOUNT_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_UPDATE_AMOUNT
          }
          bg={purple}
          type="submit"
          loading={isLoading}
        />
      }
      component={"form"}
      onSubmit={handleSubmit(updateAmount)}
    >
      <Grid container spacing={2}>
        <Grid item md={12} sm={12}>
          <Box>
            <FormLabel sx={{ fontSize: "13px" }}>
              {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_NOTE} :{" "}
              {selectedRowData?.Amount}
            </FormLabel>
          </Box>
          <InputLabel required sx={styleSheet.inputLabel}>
            {codText}
          </InputLabel>
          <TextField
            onFocus={handleFocus}
            type="text"
            size="small"
            id="cod"
            name="cod"
            fullWidth
            variant="outlined"
            {...register("cod", {
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
              pattern: {
                value: /^\d+(\.\d+)?$/,
                message: "Only numeric values are allowed",
              },
            })}
            error={Boolean(errors.cod)} // set error prop
            helperText={errors.cod?.message}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
}
export default UpdateOrderAmountModal;
