import {
  Divider,
  Grid,
  InputLabel,
  Slide,
  TextField,
  Typography
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAllReasons,
  UpdateProductStockQuantityByReason,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { placeholders, purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function EditInventoryModal({
  open,
  setOpen,
  inventoryItem,
  getAllProductInventory,
}) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [checkError, setCheckError] = useState(false);
  const [allReasons, setAllReasons] = useState([]);
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
  const handleClose = () => {
    resetForm();
    setOpen(false);
  };
  const reasonValue = useWatch({
    name: "reason",
    control,
  });
  const quantityValue = useWatch({
    name: "quantity",
    control,
  });
  const [quantity, setQuantity] = useState(0);
  const updateData = async (data) => {
    // console.log(data);
    if (checkError) {
      errorNotification(
        LanguageReducer?.languageType
          .ENTER_QUANTITY_GREATER_THAN_AVAILABLE_QUANTITY_OR_DAMAGE_REPLACE_QUANTITY_TOAST
      );
    } else {
      let body = {
        productStockId: inventoryItem?.ProductStockId,
        transactionTypeId: data?.reason?.id,
        quantity: parseFloat(data?.quantity),
        comment: data?.comment,
      };
      //console.log("body", body);
      try {
        const response = await UpdateProductStockQuantityByReason(body);
        // console.log("Update Quantity", response);
        successNotification(
          LanguageReducer?.languageType?.QUANTITY_SUCCESSFULLY_UPDATED_TOAST
        );

        resetForm();
        getAllProductInventory();
        handleClose();
      } catch (error) {
        console.error("Error in updating quantity:", error.response);
      }
    }
  };
  const resetForm = () => {
    setValue("quantity", "");
    setValue("comment", "");
    setValue("reason", "");
    setQuantity(0);
  };
  const handleGetAllReasons = async () => {
    try {
      const response = await GetAllReasons();
      // console.log("response of api of reasons", response);
      const filterResult = response.data.result.filter((dt) => dt.id !== 0);
      setAllReasons(filterResult);
    } catch (error) {
      console.error("Error fetching reasons:", error.response);
    }
  };
  useEffect(() => {
    const reasonId = reasonValue?.id;
    const qtyVal = parseFloat(quantityValue);
    const availableQuantity = parseFloat(inventoryItem?.QuantityAvailable || 0);
    const damageQuantity = parseFloat(inventoryItem?.QuantityDamage || 0);

    if (reasonId === 1) {
      setQuantity(availableQuantity + (isNaN(qtyVal) ? 0 : qtyVal));
      setCheckError(false);
    } else if (reasonId === 8) {
      setQuantity(isNaN(qtyVal) ? 0 : qtyVal);
      setCheckError(false);
    } else if (reasonId === 9) {
      if (!isNaN(qtyVal)) {
        if (qtyVal <= availableQuantity) {
          setQuantity(availableQuantity - qtyVal);
          setCheckError(false);
        } else {
          errorNotification(
            LanguageReducer?.languageType
              ?.ENTER_QUANTITY_GREATER_THAN_AVAILABLE_QUANTITY_TOAST
          );
          setCheckError(true);
        }
      }
    } else if (reasonId === 7) {
      if (!isNaN(qtyVal)) {
        if (qtyVal <= damageQuantity) {
          setQuantity(availableQuantity + qtyVal);
          setCheckError(false);
        } else {
          errorNotification(
            LanguageReducer?.languageType
              ?.ENTER_QUANTITY_GREATER_THAN_DAMAGE_REPLACE_QUANTITY_TOAST
          );
          setCheckError(true);
        }
      }
    }
    if (isNaN(qtyVal)) {
      setQuantity(availableQuantity);
    }
  }, [reasonValue, quantityValue, inventoryItem]);
  useEffect(() => {
    handleGetAllReasons();
  }, []);
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={LanguageReducer?.languageType?.UPDATE_QUANTITY_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={"Update Inventory"}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(updateData)}
    >
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item sm={12} sx={{ mt: "10px" }}>
          <Divider />
        </Grid>
        <Grid item sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.QUANTITY_TEXT}
          </InputLabel>
          <TextField
            type="number"
            id="quantity"
            name="quantity"
            placeholder={placeholders.quantity}
            size="small"
            fullWidth
            variant="outlined"
            {...register("quantity", {
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
            })}
            error={Boolean(errors.quantity)}
            helperText={errors.quantity?.message}
          />
        </Grid>
        <Grid item xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.REASON_TEXT}
          </InputLabel>
          <SelectComponent
            name="reason"
            control={control}
            options={allReasons}
            isRHF={true}
            required={true}
            optionLabel={EnumOptions.SELECTED_REASON.LABEL}
            optionValue={EnumOptions.SELECTED_REASON.VALUE}
            isRefesh={true}
            handleRefreshClick={handleGetAllReasons}
            {...register("reason", {
              required: {
                value: true,
              },
            })}
            value={getValues("reason")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("reason", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        {checkError !== true && (
          <>
            <Grid item sm={12}>
              <Typography
                sx={{ fontWeight: "bold", color: "black" }}
                variant="body1"
              >
                Previous Quantity:{" "}
                <span style={{ color: "blue", fontWeight: "bold" }}>
                  {inventoryItem?.QuantityAvailable}
                </span>
              </Typography>
            </Grid>
            <Grid item sm={12}>
              <Typography
                sx={{ fontWeight: "bold", color: "black" }}
                variant="body1"
              >
                New Quantity:{" "}
                <span style={{ color: "green", fontWeight: "bold" }}>
                  {quantity}
                </span>
              </Typography>
            </Grid>
          </>
        )}

        <Grid item sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.COMMENT_TEXT}
          </InputLabel>
          <TextField
            {...register("comment", {
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
            })}
            error={Boolean(errors.comment)}
            helperText={errors.comment?.message}
            type="text"
            name="comment"
            id="comment"
            variant="outlined"
            size="small"
            fullWidth
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
}

export default EditInventoryModal;
