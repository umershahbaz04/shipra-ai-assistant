import React, { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useForm } from "react-hook-form";
import { Box, Grid, InputLabel, TextField } from "@mui/material";
import {
  purple,
  useGetAllClientUserRole,
} from "../../../utilities/helpers/Helpers";
import { createClientUserRole } from "../../../api/AxiosInterceptors";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import UtilityClass from "../../../utilities/UtilityClass";
import { styleSheet } from "../../../assets/styles/style";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

function AddUserRoleModal(props) {
  const { open, setOpen, setIsRoleAdded } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const dispatch = useDispatch();
  const [isLoading, setIsLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm();

  const handleClose = () => {
    reset();
    setOpen(false);
  };

  const onSubmit = async (data) => {
    setIsLoading(true);
    await createClientUserRole(data)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("User role created successfully");
          handleClose();
          setIsRoleAdded(true);
        }
      })
      .catch((e) => {
        if (!e?.response?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(
            e?.response?.data?.errors
          );
        } else {
          errorNotification("Unable to create user role");
        }
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };

  return (
    <Box borderRadius={"10px"}>
      <ModalComponent
        open={open}
        onClose={handleClose}
        maxWidth="sm"
        title={LanguageReducer?.languageType?.SETTING_PERMISSION_ADD_USER_ROLES}
        actionBtn={
          <ModalButtonComponent
            onClick={handleSubmit(onSubmit)}
            disabled={isLoading}
            title={LanguageReducer?.languageType?.SETTING_PERMISSION_ADD_ROLE}
            loading={isLoading}
            bg={purple}
            type="submit"
          />
        }
        component={"form"}
      >
        <Grid item sm={12} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.SETTING_PERMISSION_ROLE_NAME}
          </InputLabel>
          <TextField
            size="small"
            fullWidth
            variant="outlined"
            placeholder="Please select an option"
            {...register("RoleName", {
              required: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              pattern: {
                value: /^(?!\s*$).+/,
                message:
                  LanguageReducer?.languageType
                    ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
              },
            })}
            error={Boolean(errors.RoleName)}
            helperText={errors.RoleName?.message}
          />
          <InputLabel sx={styleSheet.inputLabel} style={{ marginTop: "20px" }}>
            {LanguageReducer?.languageType?.SETTING_PERMISSION_ROLE_DESCRIPTION}
          </InputLabel>
          <TextField
            size="small"
            fullWidth
            variant="outlined"
            placeholder="Please select an option"
            {...register("RoleDescription", {})}
            error={Boolean(errors.RoleDescription)}
            helperText={errors.RoleDescription?.message}
          />
        </Grid>
      </ModalComponent>
    </Box>
  );
}

export default AddUserRoleModal;
