import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import { UpdateReturnCommand } from "../../../api/AxiosInterceptors";
import { GridContainer, GridItem } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { useSelector } from "react-redux";
const initialSmsFieldsData = {
  reason: "",
  detailReason: "",
};
const UpdateReasonModal = (props) => {
  const { open, onClose, clientReturnReason, getAllClientReturnReason } = props;
  const [updateReason, setUpdateReason] = useState(initialSmsFieldsData);
  const [loading, setLoading] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setUpdateReason((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleupdateReason = async () => {
    setLoading(true);
    await UpdateReturnCommand(
      updateReason.reason,
      updateReason.detailReason,
      clientReturnReason.data.clientReturnReasonId
    )
      .then((res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Failed to update return reason.");
        } else {
          successNotification("Return reason update successfully.");
          getAllClientReturnReason();
        }
      })
      .finally(() => {
        setUpdateReason(initialSmsFieldsData);
        setLoading(false);
        setLoading(false);
        onClose();
      });
  };

  useEffect(() => {
    const currentReason = clientReturnReason.data;
    if (currentReason) {
      setUpdateReason({
        reason: currentReason.reason,
        detailReason: currentReason.reasonDetail,
      });
    }
  }, [clientReturnReason]);
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="sm"
        title={
          LanguageReducer?.languageType
            ?.SETTING_RETURN_REASON_UPDATE_RETURN_REASON
        }
        actionBtn={
          <ModalButtonComponent
            title={
              LanguageReducer?.languageType?.SETTING_RETURN_REASON_UPDATE_REASON
            }
            bg={purple}
            loading={loading}
            onClick={(e) => handleupdateReason()}
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
              value={updateReason.reason}
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
              value={updateReason.detailReason}
              name="detailReason"
              onChange={handleChange}
            />
          </GridItem>
        </GridContainer>
      </ModalComponent>
    </>
  );
};

export default UpdateReasonModal;
