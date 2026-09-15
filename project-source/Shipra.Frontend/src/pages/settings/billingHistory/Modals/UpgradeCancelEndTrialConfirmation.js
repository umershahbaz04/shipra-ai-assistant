import { Box, TextField, Typography } from "@mui/material";
import React, { useState } from "react";
import ModalButtonComponent from "../../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../../.reUseableComponents/TextField/SelectComponent";
import ConfirmationIcon from "../../../../assets/images/confirmationIcon.png";
import {
  EnumCancelPlanFeedbackOptions,
  EnumOptions,
} from "../../../../utilities/enum";
import Colors from "../../../../utilities/helpers/Colors";
import { errorNotification } from "../../../../utilities/toast";

function UpgradeCancelEndTrialConfirmationModal(props) {
  let {
    open,
    setOpen = () => {},
    onClose = () => {},
    loading = false,
    onConfirm = () => {},
    buttonColor = Colors.primary,
    heading = "",
    message = "",
    messageDetails = "",
    buttonText = "Deactivate",
    hasFeedback,
  } = props;
  const initialFeedbackFields = {
    selectedReason: null,
    comment: "",
  };
  const [feedback, setFeedback] = useState(initialFeedbackFields);
  const handleClose = () => {
    onClose();
    setFeedback(initialFeedbackFields);
    setOpen(false);
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      actionBtn={
        <ModalButtonComponent
          title={buttonText}
          loading={loading}
          bg={buttonColor}
          onClick={() => {
            if (!hasFeedback) {
              onConfirm(feedback);
              return;
            }
            if (hasFeedback && feedback.selectedReason && feedback.comment) {
              onConfirm(feedback);
              return;
            }
            errorNotification(
              !feedback.selectedReason && !feedback.comment
                ? "Please select reason and enter feedback"
                : !feedback.selectedReason
                ? "Please select reason"
                : "Please enter feedback"
            );
          }}
        />
      }
    >
      <Box display={"flex"} gap={1}>
        <Box width={100} height={100}>
          <Box
            component={"img"}
            width={"100%"}
            height={"100%"}
            sx={{ objectFit: "contain" }}
            src={ConfirmationIcon}
          />
        </Box>
        <Box alignSelf={"center"}>
          <Typography variant="h3">{heading}</Typography>
          <Typography variant="h5" fontWeight={300} mt={0.25}>
            {message + messageDetails}
          </Typography>
          {hasFeedback && (
            <Box className={"flex_col"} sx={{ gap: 1, mt: 1 }}>
              <SelectComponent
                addPleaseSelectOptionOnClear={false}
                placeholder={"Please select the reason"}
                height={40}
                options={EnumCancelPlanFeedbackOptions}
                optionLabel={EnumOptions.CANCEL_PLAN_FEEDBACK.LABEL}
                optionValue={EnumOptions.CANCEL_PLAN_FEEDBACK.VALUE}
                value={feedback.selectedReason}
                onChange={(e, val) => {
                  setFeedback((prev) => ({ ...prev, selectedReason: val }));
                }}
              />
              <TextField
                multiline
                minRows={4}
                placeholder={"Please enter feedback"}
                type="text"
                size="small"
                fullWidth
                value={feedback.comment}
                onChange={(e) =>
                  setFeedback((prev) => ({ ...prev, comment: e.target.value }))
                }
              />
            </Box>
          )}
        </Box>
      </Box>
    </ModalComponent>
  );
}
export default UpgradeCancelEndTrialConfirmationModal;
