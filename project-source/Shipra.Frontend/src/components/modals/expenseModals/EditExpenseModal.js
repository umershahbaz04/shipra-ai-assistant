import { Box, Grid, InputLabel, TextField, Typography } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";

import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAllEmployeesForSelection,
  GetAllExpenseCategoryLookup,
  UpdateExpense,
} from "../../../api/AxiosInterceptors";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import {
  fetchMethod,
  fetchMethodResponse,
  placeholders,
  purple,
  useSetNumericInputEffect,
} from "../../../utilities/helpers/Helpers";

function EditExpenseModal(props) {
  let { open, expenseForEdit, onClose, getAllDriverExpense } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isSubmiting, setIsSubmiting] = useState(false);
  const [allExpenseCategory, setAllExpenseCategory] = useState([]);
  const [allEmployees, setAllEmployees] = useState([]);

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
    onClose();
  };
  useWatch({
    name: "expenseCategory",
    control,
  });
  const getAllExpenseCategoryLookup = async () => {
    let res = await GetAllExpenseCategoryLookup();
    if (res.data.result !== null) {
      setAllExpenseCategory(res.data.result);
    }
  };
  const getAllEmployeesForSelection = async () => {
    let res = await GetAllEmployeesForSelection();
    if (res.data.result !== null) {
      setAllEmployees(res.data.result);
    }
  };
  useEffect(() => {
    getAllExpenseCategoryLookup();
    getAllEmployeesForSelection();
  }, []);

  useEffect(() => {
    if (expenseForEdit) {
      if (expenseForEdit?.expenseDate) {
        setValue("expenseDate", new Date(expenseForEdit?.expenseDate));
      }
      setValue("amount", expenseForEdit?.amount);
      setValue("detail", expenseForEdit?.details);
    }
  }, [expenseForEdit]);
  useEffect(() => {
    if (allExpenseCategory && allExpenseCategory.length > 0) {
      let defVal = allExpenseCategory.find(
        (x) => x.id == expenseForEdit?.expenseCategoryId
      );
      setValue("expenseCategory", defVal);
    }
  }, [allExpenseCategory]);

  useEffect(() => {
    //console.log("getValues", getValues("expenseCategory"));
  }, [getValues("expenseCategory")]);

  const handleUpdate = async (data) => {
    //console.log("vals::", data);
    const body = {
      ExpenseId: expenseForEdit?.expenseId,
      DriverId: expenseForEdit?.driverId,
      Amount: data?.amount,
      ExpenseDate: UtilityClass.getFormatedDateWithoutTime(data?.expenseDate),
      ExpenseCategoryId: data?.expenseCategory?.id,
      Details: data?.detail,
    };
    //console.log("body::", body);

    const { response } = await fetchMethod(
      () => UpdateExpense(body),
      setIsSubmiting,
      false
    );
    fetchMethodResponse(
      response,
      LanguageReducer?.languageType
        ?.MY_CARRIER_EXPENSES_TASKS_EXPANSE_UPDATE_SUCCESSFULLY,
      handleClose
    );
    getAllDriverExpense();
    // UpdateExpense(body)
    //   .then((res) => {
    //     if (!res?.data?.isSuccess) {
    //       UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
    //     } else {
    //       successNotification("Expense update successfully");
    //       handleClose();
    //     }
    //   })
    //   .catch((e) => {
    //     //console.log("e", e);
    //     if (!e?.response?.data?.isSuccess) {
    //       UtilityClass.showErrorNotificationWithDictionary(
    //         e?.response?.data?.errors
    //       );
    //     } else {
    //       errorNotification("Driver to create employee");
    //     }
    //   })
    //   .finally((e) => {
    //   });
  };

  useSetNumericInputEffect([open]);
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_EXPENSES_TASKS_UPDATE_EXPENSE
          }
          loading={isSubmiting}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(handleUpdate)}
    >
      <Box sx={styleSheet.addDriverHeadingAndUpload}>
        <Typography sx={styleSheet.addDriverHeading} variant="h4">
          {
            LanguageReducer?.languageType
              ?.MY_CARRIER_EXPENSES_TASKS_UPDATE_EXPENSE
          }
        </Typography>
      </Box>
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item md={12} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_EXPENSE_DATE}
          </InputLabel>
          <CustomRHFReactDatePickerInput
            name="expenseDate"
            control={control}
            defaulValue={new Date()}
            // onChange={handleOnChange}
            required
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
          />
        </Grid>

        <Grid item md={12} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {
              LanguageReducer?.languageType
                ?.MY_CARRIER_EXPENSES_EXPENSE_CATEGORY
            }
          </InputLabel>
          <SelectComponent
            name="expenseCategory"
            control={control}
            options={allExpenseCategory}
            optionLabel={EnumOptions.EXPENSE_TYPE.LABEL}
            optionValue={EnumOptions.EXPENSE_TYPE.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllExpenseCategoryLookup}
            isRHF={true}
            required={true}
            {...register("expenseCategory", {
              required: {
                value: true,
              },
            })}
            value={getValues("expenseCategory")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("expenseCategory", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        <Grid item md={12} sm={6} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_EXPENSE_AMOUNT}
          </InputLabel>
          <TextField
            placeholder={placeholders.price}
            size="small"
            fullWidth
            variant="outlined"
            name="amount"
            type="number"
            {...register("amount", {
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
              pattern: {
                // value: /^(?!\s*$).+/,
                message:
                  LanguageReducer?.languageType
                    ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
              },
            })}
            error={Boolean(errors.amount)} // set error prop
            helperText={errors.amount?.message}
          />
        </Grid>
        <Grid item sm={6} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_EXPENSE_DETAIL}
          </InputLabel>
          <TextField
            multiline
            rows={2}
            maxRows={4}
            placeholder="Detail"
            type="text"
            size="small"
            fullWidth
            variant="outlined"
            name="detail"
            {...register("detail")}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
}
export default EditExpenseModal;
