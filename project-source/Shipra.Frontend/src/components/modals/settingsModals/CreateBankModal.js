import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import { AddUpdateClientPayoutBank } from "../../../api/AxiosInterceptors";
import { GridContainer, GridItem } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { useSelector } from "react-redux";

const CreateBankModal = (props) => {
  const { open, onClose, rowData, getAllPayoutBanks } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [stringFields, setStringFields] = useState({
    bankName: "",
    accounttitle: "",
    iban: "",
    swiftcode: "",
    branchname: "",
  });
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setStringFields((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleCreateBank = async () => {
    const { bankName, accounttitle, iban, swiftcode, branchname } =
      stringFields;
    if (!bankName || !accounttitle || !iban) {
      errorNotification("Please fill in all the required fields.");
      return;
    }
    setLoading(true);
    const body = {
      clientPayoutBankId: "",
      bankName: stringFields.bankName,
      accountTitle: stringFields.accounttitle,
      iban: stringFields.iban,
      swiftCode: stringFields.swiftcode,
      branchName: stringFields.branchname,
    };
    await AddUpdateClientPayoutBank(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Failed to create Bank Account.");
        } else {
          successNotification("Created Successfully.");
          getAllPayoutBanks();
        }
      })
      .catch((e) => {})
      .finally(() => {
        setLoading(false);
        onClose();
      });
  };

  useEffect(() => {
    if (rowData) {
      setStringFields({
        bankName: rowData?.bankName,
        accounttitle: rowData?.accountTitle,
        iban: rowData?.iban,
        swiftcode: rowData?.swiftCode,
        branchname: rowData?.branchName,
      });
    }
  }, []);
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="sm"
        title={LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_CREATE_BANK}
        actionBtn={
          <ModalButtonComponent
            title={
              LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_CREATE_BANK
            }
            bg={purple}
            loading={loading}
            onClick={(e) => handleCreateBank()}
          />
        }
      >
        <GridContainer spacing={1}>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={
                LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_BANK_NAME
              }
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter Bank Name"}
              value={stringFields.bankName}
              name="bankName"
              onChange={handleChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={
                LanguageReducer?.languageType
                  ?.SETTINGS_PAYOUT_BANK_ACCOUNT_TITLE
              }
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter Title"}
              value={stringFields.accounttitle}
              name="accounttitle"
              onChange={handleChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_IBAN}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter IBAN"}
              value={stringFields.iban}
              name="iban"
              onChange={handleChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent title={"SwiftCode"} required={false} />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter SwiftCode"}
              value={stringFields.swiftcode}
              name="swiftcode"
              onChange={handleChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={
                LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_BRANCH_NAME
              }
              required={false}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter Branch Name"}
              value={stringFields.branchname}
              name="branchname"
              onChange={handleChange}
            />
          </GridItem>
        </GridContainer>
      </ModalComponent>
    </>
  );
};

export default CreateBankModal;
