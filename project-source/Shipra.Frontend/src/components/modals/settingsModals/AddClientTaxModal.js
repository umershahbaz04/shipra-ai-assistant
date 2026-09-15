import { Grid, InputLabel, TextField } from "@mui/material";
import React, { useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { CreateClientTax } from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import UtilityClass from "../../../utilities/UtilityClass.js";

import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent.js";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent.js";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent.js";
import { EnumOptions } from "../../../utilities/enum/index.js";
import { placeholders, purple } from "../../../utilities/helpers/Helpers.js";
function AddEmployeeModal(props) {
  let {
    open,
    setOpen,
    getAllClientTax,
    getAllTaxForSelection,
    allTaxForSelection,
  } = props;
  const [values, setValues] = useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = React.useState(false);
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
  useWatch({
    name: "selectTax",
    control,
  });
  const createTax = async (data) => {
    const body = {
      percentage: data.percentage,
      taxId: data.selectTax.taxId,
    };
    console.log("body::", body);
    setIsLoading(true);

    CreateClientTax(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Client tax created successfully");
          getAllClientTax();
          handleClose();
        }
      })
      .catch((e) => {
        if (!e?.response?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors
          );
        } else {
          errorNotification("Unable to create client tax");
        }
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={LanguageReducer?.languageType?.SETTING_MANAGE_TAX_ADD_CLIENT_TAX}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.SETTING_MANAGE_TAX_ADD_TAX}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(createTax)}
    >
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item sm={12} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_MANAGE_TAX_SELECT_TAX}
          </InputLabel>
          <SelectComponent
            name="selectTax"
            control={control}
            options={allTaxForSelection}
            isRHF={true}
            required={true}
            isRefesh={true}
            handleRefreshClick={getAllTaxForSelection}
            value={getValues("selectTax")}
            optionLabel={EnumOptions.CLIENT_TEXT.LABEL}
            optionValue={EnumOptions.CLIENT_TEXT.VALUE}
            {...register("selectTax", {
              required: {
                value: true,
              },
            })}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("selectTax", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        <Grid item sm={12} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_MANAGE_TAX_ADD_PERCENTAGE}
          </InputLabel>
          <TextField
            placeholder={placeholders.quantity}
            size="small"
            fullWidth
            variant="outlined"
            name="percentage"
            {...register("percentage", {
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
            })}
            error={Boolean(errors.percentage)} // set error prop
            helperText={errors.percentage?.message}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
}
export default AddEmployeeModal;
