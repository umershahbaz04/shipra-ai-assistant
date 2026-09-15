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
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useRef, useState } from "react";
import { styleSheet } from "../../../assets/styles/style";
import { useSelector } from "react-redux";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { useForm } from "react-hook-form";
import {
  UpdateOrderAmount,
  UplaodCarrierSettlementFile,
} from "../../../api/AxiosInterceptors";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function UpdateOrderAmountModal(props) {
  let {
    open,
    setOpen,
    selectedSettlement,
    setSettlementData,
    settlementData,
    setSelectedSettlement,
  } = props;
  console.log("selectedSettlement", selectedSettlement);
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
      orderNo: selectedSettlement.orderNo,
      amount: data.cod,
    };

    console.log("params::", params);
    UpdateOrderAmount(params)
      .then((res) => {
        console.log("res", res);
        let newArray = [...settlementData];
        let modifiedObject = newArray[selectedSettlement.index - 1];
        modifiedObject.diffrence = 0;
        modifiedObject.fileAmount = data.cod;
        newArray[selectedSettlement.index - 1] = modifiedObject;
        console.log("newArray", newArray);
        setIsLoading(false);
        successNotification(res.data.result.message);
        setSettlementData(newArray);
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
    setSelectedSettlement(null);
    setOpen(false);
  };
  const handleFocus = (event) => event.target.select();

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={LanguageReducer?.languageType?.UPDATE_ORDER_AMOUNT_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={"Update Amount"}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={ handleSubmit(updateAmount)}
    >
      <Grid container spacing={2}>
        <Grid item md={12} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            COD{" "}
          </InputLabel>
          <TextField
            onFocus={handleFocus}
            type="number"
            size="small"
            id="cod"
            name="cod"
            defaultValue={selectedSettlement?.fileAmount}
            fullWidth
            variant="outlined"
            {...register("cod", {
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
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
