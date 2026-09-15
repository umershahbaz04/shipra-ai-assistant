import {
  Box,
  Grid,
  InputLabel,
  TextField,
  Typography
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";

import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomRHFReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomRHFReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateDriver,
  GetAllEmployeesForSelection,
  GetAllExpenseCategoryLookup,
  GetExpenseById,
} from "../../../api/AxiosInterceptors";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function AddExpenseModal(props) {
  let { open, setOpen, selectedRowData } = props;
  const [values, setValues] = useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [allCountries, setAllCountries] = useState([]);
  const [selectedCountry, setSelectedCountry] = useState();
  const [allRegions, setAllRegions] = useState([]);
  const [selectedRegion, setSelectedRegion] = useState();
  const [allCities, setAllCities] = useState([]);
  const [selectedCity, setSelectedCity] = useState();
  const [file, setFile] = useState();
  const [imageURL, setImageURL] = useState("");
  const [isSubmiting, setIsSubmiting] = useState(false);
  const [allExpenseCategory, setAllExpenseCategory] = useState([]);
  const [allEmployees, setAllEmployees] = useState([]);
  const [expenseForEdit, setExpenseForEdit] = useState({});

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
    setOpen(false);
  };

  const getExpenseById = async () => {
    let res = await GetExpenseById(selectedRowData?.ExpenseId);
    console.log("GetExpenseById", res.data.result);
    if (res.data.result !== null) {
      setExpenseForEdit(res.data.result);
    }
  };

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
    getExpenseById();
  }, []);
  useEffect(() => {
    if (selectedRowData) {
      getExpenseById();
    }
  }, []);

  const createEmployee = async (data) => {
    console.log("vals::", data);
    const body = {
      countryId: data.country.countryId,
    };
    console.log("body::", body);
    setIsSubmiting(true);

    CreateDriver(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Driver created successfully");
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
          errorNotification("Driver to create employee");
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
      maxWidth="md"
      title={""}
      actionBtn={
        <ModalButtonComponent
          title={"Add Expense"}
          loading={isSubmiting}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(createEmployee)}
    >
      <Box sx={styleSheet.addDriverHeadingAndUpload}>
        <Typography sx={styleSheet.addDriverHeading} variant="h4">
          {"Add Expense"}
        </Typography>
      </Box>
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>Expense Date</InputLabel>
          <CustomRHFReactDatePickerInput
            name="expenseDate"
            control={control}
            defaultValue={new Date()}
            // onChange={handleOnChange}
            required
            error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
          />
        </Grid>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>{"Employee"}</InputLabel>
          <SelectComponent
            name="employee"
            control={control}
            options={allEmployees}
            optionLabel={EnumOptions.EMPLOYEE.LABEL}
            optionValue={EnumOptions.EMPLOYEE.VALUE}
            isRHF={true}
            required={true}
            {...register("employee", {
              required: {
                value: true,
              },
            })}
            value={getValues("employee")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("employee", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {"Expense Categoey"}
          </InputLabel>
          <SelectComponent
            name="expenseCategoey"
            control={control}
            optionLabel={EnumOptions.EXPENSE_TYPE.LABEL}
            optionValue={EnumOptions.EXPENSE_TYPE.VALUE}
            isRHF={true}
            required={true}
            {...register("expenseCategoey", {
              required: {
                value: true,
              },
            })}
            value={getValues("expenseCategoey")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("expenseCategoey", resolvedId);
            }}
            errors={errors}
          />
        </Grid>
        <Grid item sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>{"Detail"}</InputLabel>
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
export default AddExpenseModal;
