import { Box, Grid, InputLabel, TextField } from "@mui/material";
import React, { useEffect } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent.js";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent.js";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent.js";
import { UpdateClientTax } from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";
import { EnumOptions } from "../../../utilities/enum/index.js";
import { purple } from "../../../utilities/helpers/Helpers.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import UtilityClass from "../../../utilities/UtilityClass.js";

function EditEmployeeModal(props) {
  let {
    open,
    setTaxInfo,
    allTaxForSelection,
    getAllClientTax,
    getAllTaxForSelection,
    taxInfo,
  } = props;
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
    // reset();
    setTaxInfo((prev) => ({ ...prev, open: false }));
  };

  useWatch({
    name: "selectTax",
    control,
  });

  const updateClientTax = async (data) => {
    console.log("vals::", data);
    const body = {
      clientTaxId: taxInfo.data.clientTaxId,
      taxId: data.selectTax.taxId,
      percentage: data.percentage,
    };
    console.log("body::", body);
    setIsLoading(true);

    UpdateClientTax(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Client tax update successfully");
          handleClose();
          getAllClientTax();
        }
      })
      .catch((e) => {
        console.log("e", e);
        if (!e?.response?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors
          );
        } else {
          errorNotification("Unable to update client tax");
        }
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  useEffect(() => {
    if (taxInfo.data) {
      let alluserRole = allTaxForSelection?.find(
        (x) => x.taxId == taxInfo?.data?.taxId
      );
      setValue("percentage", taxInfo?.data?.percentage);
      setValue("selectTax", alluserRole);
    }
  }, [taxInfo.data]);

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={LanguageReducer?.languageType?.SETTING_MANAGE_TAX_UPDATE_TAX}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.SETTING_MANAGE_TAX_UPDATE_TAX}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(updateClientTax)}
    >
      <Box sx={styleSheet.addDriverHeadingAndUpload}>
        <Grid container spacing={2}>
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
              value={getValues("selectTax")}
              optionLabel={EnumOptions.CLIENT_TEXT.LABEL}
              optionValue={EnumOptions.CLIENT_TEXT.VALUE}
              isRefesh={true}
              handleRefreshClick={getAllTaxForSelection}
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
              placeholder={"Name"}
              size="small"
              fullWidth
              variant="outlined"
              name="percentage"
              defaultValue={getValues("percentage")}
              {...register("percentage", {
                required: {
                  value: true,
                  message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                },
                pattern: {
                  value: /^(?!\s*$).+/,
                  message:
                    LanguageReducer?.languageType
                      ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                },
              })}
              error={Boolean(errors.percentage)} // set error prop
              helperText={errors.percentage?.message}
            />
          </Grid>
        </Grid>
      </Box>
    </ModalComponent>
  );
}
export default EditEmployeeModal;
