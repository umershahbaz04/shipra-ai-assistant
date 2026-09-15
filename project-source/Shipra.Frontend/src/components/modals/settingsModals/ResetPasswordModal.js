import { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import { AdminSetUserPassword } from "../../../api/AxiosInterceptors";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  getTrim,
  GridContainer,
  GridItem,
  placeholders,
  purple,
  useForm,
  useLanguageReducer
} from "../../../utilities/helpers/Helpers";
import { successNotification } from "../../../utilities/toast";

const ResetPasswordModal = (props) => {
  const { open, onClose, data } = props;
  const LanguageReducer = useLanguageReducer();
  const [adminUserPassWord, setAdminUserPassWord] = useState({
    username: "",
    newPassword: "",
    confirmNewPassword: "",
    loading: false,
  });
  const { errors, handleInvalid, handleChange, setErrors } =
    useForm(setAdminUserPassWord);

  const updateUserPassword = async (e) => {
    e.preventDefault();
    const body = {
      userName: getTrim(adminUserPassWord?.username),
      password: adminUserPassWord?.newPassword,
    };
    setAdminUserPassWord((prev) => ({ ...prev, loading: true }));
    try {
      const response = await AdminSetUserPassword(body);
      if (response?.data?.isSuccess) {
        successNotification(response?.data?.result?.message);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
    } finally {
      setAdminUserPassWord((prev) => ({ ...prev, loading: false }));
      onClose();
    }
  };
  useEffect(() => {
    if (data) {
      setAdminUserPassWord((prev) => ({
        ...prev,
        username: data?.EmployeeCode,
      }));
    }
  }, [data]);
  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title={"Admin User Password"}
      actionBtn={
        <ModalButtonComponent
          title={"Update Admin User Password"}
          bg={purple}
          type="submit"
          loading={adminUserPassWord.loading}
        />
      }
      onSubmit={updateUserPassword}
      onInvalid={handleInvalid}
      component={"form"}
    >
      <GridContainer spacing={1}>
        <GridItem xs={12}>
          <TextFieldLableComponent title={"userName"} />
          <TextFieldComponent
            type={"text"}
            placeholder={"demoAccount"}
            value={adminUserPassWord.username}
            name={"username"}
            onChange={handleChange}
            errors={errors}
            disabled={true}
          />
        </GridItem>
        <GridItem xs={12}>
          <TextFieldLableComponent
            title={LanguageReducer.SETING_SECURITY_NEW_PASSWORD}
          />
          <TextFieldComponent
            type={"password"}
            passwordType={true}
            placeholder={placeholders.password}
            value={adminUserPassWord.newPassword}
            name={"newPassword"}
            onChange={handleChange}
            errors={errors}
          />
        </GridItem>
        <GridItem xs={12}>
          <TextFieldLableComponent
            title={LanguageReducer.SETING_SECURITY_CONFIRM_NEW_PASSWORD}
          />
          <TextFieldComponent
            type={"password"}
            passwordType={true}
            placeholder={placeholders.password}
            value={adminUserPassWord.confirmNewPassword}
            name={"confirmNewPassword"}
            onChange={handleChange}
            errors={errors}
          />
        </GridItem>
      </GridContainer>
    </ModalComponent>
  );
};

export default ResetPasswordModal;
