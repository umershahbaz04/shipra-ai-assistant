import React, { useState, useEffect } from "react";
import {
  Box,
  Dialog,
  Divider,
  FormControl,
  InputAdornment,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
  Grid,
  InputLabel,
  TextField,
  MenuItem,
  Avatar,
  Typography,
  IconButton,
  OutlinedInput,
  Autocomplete,
} from "@mui/material";
import { styleSheet } from "../../../assets/styles/style";
import Slide from "@mui/material/Slide";
import { useSelector } from "react-redux";
import { Controller, useForm, useWatch } from "react-hook-form";

import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  CreateDriver,
  CreateExpenseCategory,
} from "../../../api/AxiosInterceptors";
import { LoadingButton } from "@mui/lab";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function AddExpenseCategoryModal(props) {
  let { open, setOpen, getAllExpenseCategoryLookup } = props;
  const [values, setValues] = useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isSubmiting, setIsSubmiting] = useState(false);

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
    reset();
    setOpen(false);
  };

  const createCategoey = async (data) => {
    const body = {
      expenseName: data.categoryName,
    };
    console.log("body::", body);
    setIsSubmiting(true);

    CreateExpenseCategory(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Category created successfully");
          getAllExpenseCategoryLookup();
          handleClose();
        }
      })
      .catch((e) => {
        console.log("e", e);
        if (!e?.response?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors
          );
        } else {
          errorNotification("Something went wrong!");
        }
      })
      .finally((e) => {
        setIsSubmiting(false);
      });
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={"Add Expense Category"}
          loading={isSubmiting}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(createCategoey)}
    >
      <Box sx={styleSheet.addDriverHeadingAndUpload}>
        <Typography sx={styleSheet.addDriverHeading} variant="h4">
          {"Add Expense Category"}
        </Typography>
      </Box>
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {"Expense category name"}
          </InputLabel>
          <TextField
            placeholder="Expense category name"
            size="small"
            fullWidth
            variant="outlined"
            name="categoryName"
            {...register("categoryName", {
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
            error={Boolean(errors.categoryName)} // set error prop
            helperText={errors.categoryName?.message}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
}
export default AddExpenseCategoryModal;
