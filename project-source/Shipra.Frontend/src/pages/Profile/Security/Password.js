import { Box } from "@mui/material";
import React, { useEffect, useState } from "react";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import {
  GetClientKeys,
  UpdatePassword
} from "../../../api/AxiosInterceptors";
import {
  ClipboardIcon,
  GridContainer,
  GridItem,
  PageMainBox,
  fetchMethod,
  fetchMethodResponse,
  placeholders,
  useForm,
  useLanguageReducer,
} from "../../../utilities/helpers/Helpers";
import { ProfileDetailsBox } from "../Profile/Profile";
import LoginModal from "./modals/loginModal";

export default function Password() {
  const LanguageReducer = useLanguageReducer();
  const [encryptedKeys, setEncryptedKeys] = useState({});
  const [openLoginModal, setOpenLoginModal] = useState(false);
  const initialUpdatePasswordFields = {
    open: false,
    oldPassword: "",
    newPassword: "",
    confirmNewPassword: "",
    loading: false,
  };
  const [updatePassword, setUpdatePassword] = useState(
    initialUpdatePasswordFields
  );
  const { errors, handleInvalid, handleChange, setErrors } =
    useForm(setUpdatePassword);

  const handleUpdatePasswordClose = () => {
    setUpdatePassword(initialUpdatePasswordFields);
    setErrors([]);
  };

  const [keys, setKeys] = useState({
    publicKey: "publicKey",
    secretKey: "secretKey",
    encryptedKey: "encryptedKey",
  });

  const getClientKeys = async () => {
    try {
      const response = await GetClientKeys();
      setEncryptedKeys(response?.data.result);
    } catch (error) {
      console.log(error);
    }
  };

  // update password
  const handleSubmit = async (e) => {
    e.preventDefault();
    setUpdatePassword((prev) => ({ ...prev, loading: true }));
    if (updatePassword.newPassword === updatePassword.confirmNewPassword) {
      const { response } = await fetchMethod(() =>
        UpdatePassword(updatePassword.oldPassword, updatePassword.newPassword)
      );
      fetchMethodResponse(response);
      handleUpdatePasswordClose();
    } else {
      setErrors(["confirmNewPassword"]);
    }
    setUpdatePassword((prev) => ({ ...prev, loading: true }));
  };



  useEffect(() => {
    if (
      encryptedKeys.publicKey ||
      encryptedKeys.secretKey ||
      encryptedKeys.encryptedKey
    ) {
      setKeys({
        publicKey: encryptedKeys.publicKey,
        secretKey: encryptedKeys.secretKey,
        encryptedKey: encryptedKeys.encryptedKey,
      });
    }
  }, [encryptedKeys]);

  useEffect(() => {
    getClientKeys();
  }, []);
  return (
    <>
      <PageMainBox>
        <ProfileDetailsBox title={LanguageReducer.SETING_SECURITY_PASSWORD}>
          <GridContainer
            component={"form"}
            onSubmit={handleSubmit}
            onInvalid={handleInvalid}
            xs={6}
          >
            <GridItem xs={12}>
              <TextFieldLableComponent
                title={LanguageReducer.SETING_SECURITY_CURRENT_PASSWORD}
              />
              <TextFieldComponent
                type={"password"}
                passwordType={true}
                placeholder={placeholders.password}
                value={updatePassword.oldPassword}
                name={"oldPassword"}
                onChange={handleChange}
                errors={errors}
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
                value={updatePassword.newPassword}
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
                value={updatePassword.confirmNewPassword}
                name={"confirmNewPassword"}
                onChange={handleChange}
                errors={errors}
                errorMsg={"Password does not match"}
              />
            </GridItem>
            <GridItem xs={12} textAlign={"right"}>
              <ButtonComponent
                loading={updatePassword.loading}
                title={LanguageReducer.UPDATE_TEXT}
                type={"submit"}
              />
            </GridItem>
          </GridContainer>
        </ProfileDetailsBox>

        <Box marginTop={3} marginBottom={4}>
          <ProfileDetailsBox title={LanguageReducer.API_ACCESS}>
            <GridContainer component={"form"} onInvalid={handleInvalid} xs={12}>
              <GridItem xs={12}>
                <TextFieldLableComponent
                  title={LanguageReducer.SETING_SECURITY_CLIENT_PUBLIC_KEY}
                />
                <TextFieldComponent
                  type={"text"}
                  placeholder={placeholders.clientPublicKey}
                  value={keys.publicKey}
                  name={"publicKey"}
                  onChange={(e) => {
                    setKeys({ ...keys, publicKey: e.target.value });
                  }}
                  errors={errors}
                  endAdornmentBtn={
                    <ClipboardIcon size={20} text={keys.publicKey} />
                  }
                />
              </GridItem>
              <GridItem xs={12}>
                <TextFieldLableComponent
                  title={LanguageReducer.SETING_SECURITY_CLIENT_Secret_KEY}
                />
                <Box display={"flex"} gap={2} alignItems={"center"}>
                  <TextFieldComponent
                    type={"password"}
                    passwordType={true}
                    placeholder={placeholders.password}
                    value={keys.secretKey}
                    name={"secretKey"}
                    onChange={(e) => {
                      setKeys({ ...keys, secretKey: e.target.value });
                    }}
                    errors={errors}
                    endAdornmentBtn={
                      <ClipboardIcon size={20} text={keys.secretKey} />
                    }
                  />
                  <ButtonComponent
                    loading={updatePassword.loading}
                    title={LanguageReducer.SETING_SECURITY_ROTATE}
                    onClick={() => setOpenLoginModal(true)}
                  />
                </Box>
              </GridItem>
              <GridItem xs={12}>
                <TextFieldLableComponent
                  title={LanguageReducer.SETING_SECURITY_CLIENT_ENCRYPTED_KEY}
                />
                <TextFieldComponent
                  type={"password"}
                  passwordType={true}
                  placeholder={placeholders.password}
                  value={keys.encryptedKey}
                  name={"encryptedKey"}
                  onChange={(e) => {
                    setKeys({ ...keys, encryptedKey: e.target.value });
                  }}
                  errors={errors}
                  endAdornmentBtn={
                    <ClipboardIcon size={20} text={keys.encryptedKey} />
                  }
                />
              </GridItem>
            </GridContainer>
          </ProfileDetailsBox>
        </Box>
        {openLoginModal && (
          <LoginModal
            open={openLoginModal}
            onClose={() => setOpenLoginModal(false)}
            getClientKeys={getClientKeys}
          />
        )}
      </PageMainBox>
    </>
  );
}
