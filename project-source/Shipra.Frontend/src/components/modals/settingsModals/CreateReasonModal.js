import React, { useState } from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import {
  fetchMethod,
  GridContainer,
  GridItem,
} from "../../../utilities/helpers/Helpers";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import { purple } from "@mui/material/colors";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { CreateReturnCommand } from "../../../api/AxiosInterceptors";
import { useSelector } from "react-redux";
const initialSmsFieldsData = {
  reason: "",
  detailReason: "",
};
const CreateReasonModal = (props) => {
  const { open, onClose, getAllClientReturnReason } = props;
  const [createReason, setCreateReason] = useState(initialSmsFieldsData);
  const [loading, setLoading] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleChange = (e) => {
    const { name, value } = e.target;
    setCreateReason((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleCreateReason = async () => {
    setLoading(true);
    await CreateReturnCommand(createReason.reason, createReason.detailReason)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Failed to create return reason.");
        } else {
          successNotification("Return reason created successfully.");
          getAllClientReturnReason();
        }
      })
      .catch((e) => {
        errorNotification(
          "An error occurred while creating the return reason."
        );
      })
      .finally(() => {
        setCreateReason(initialSmsFieldsData);
        setLoading(false);
        onClose();
      });
  };

  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="sm"
        title={
          LanguageReducer?.languageType
            ?.SETTING_RETURN_REASON_CREATE_RETURN_REASON
        }
        actionBtn={
          <ModalButtonComponent
            title={
              LanguageReducer?.languageType?.SETTING_RETURN_REASON_CREATE_REASON
            }
            bg={purple}
            loading={loading}
            onClick={(e) => handleCreateReason()}
          />
        }
      >
        <GridContainer spacing={1}>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={
                LanguageReducer?.languageType?.SETTING_RETURN_REASON_REASON
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
              placeholder={"Enter Reason"}
              value={createReason.reason}
              name="reason"
              onChange={handleChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={
                LanguageReducer?.languageType
                  ?.SETTING_RETURN_REASON_DETAIL_REASON
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
              placeholder={"Enter Detail Reason"}
              value={createReason.detailReason}
              name="detailReason"
              onChange={handleChange}
            />
          </GridItem>
        </GridContainer>
      </ModalComponent>
    </>
  );
};

export default CreateReasonModal;
