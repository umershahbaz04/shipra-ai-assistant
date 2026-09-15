import { Box, InputLabel, TextField } from "@mui/material";
import React from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { CreateClientLeadStatus } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { purple } from "../../../utilities/helpers/Helpers";
import { errorNotification, successNotification } from "../../../utilities/toast";

function AddLeadStatusModal({ open, setOpen, onSuccess }) {
  const [isLoading, setIsLoading] = React.useState(false);
  const [statusName, setStatusName] = React.useState("");
  const [color, setColor] = React.useState("var(--primary-color)");

  const handleClose = () => {
    setStatusName("");
    setColor("var(--primary-color)");
    setOpen(false);
  };

  const handleSubmit = () => {
    if (!statusName.trim()) {
      errorNotification("Please enter a status name");
      return;
    }
    const params = {
      statusName: statusName.trim(),
      color: color,
    };
    setIsLoading(true);
    CreateClientLeadStatus(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
        } else {
          successNotification("Lead status created successfully");
          if (onSuccess) onSuccess();
          handleClose();
        }
      })
      .catch(() => {
        errorNotification("Something went wrong");
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title="Add Lead Status"
      actionBtn={
        <ModalButtonComponent
          title="Add Status"
          bg={purple}
          onClick={handleSubmit}
          loading={isLoading}
        />
      }
    >
      <Box mb={2}>
        <InputLabel required sx={styleSheet.inputLabel}>
          Status Name
        </InputLabel>
        <TextField
          size="small"
          fullWidth
          variant="outlined"
          placeholder="e.g. Interested, Not Interested..."
          value={statusName}
          onChange={(e) => setStatusName(e.target.value)}
        />
      </Box>
      <Box>
        <InputLabel sx={styleSheet.inputLabel}>Color</InputLabel>
        <Box display="flex" alignItems="center" gap={1.5} mt={0.5}>
          <input
            type="color"
            value={color}
            onChange={(e) => setColor(e.target.value)}
            style={{
              width: 40,
              height: 36,
              border: "1px solid #ccc",
              borderRadius: 4,
              cursor: "pointer",
              padding: 2,
            }}
          />
          <TextField
            size="small"
            value={color}
            onChange={(e) => setColor(e.target.value)}
            sx={{ width: 130 }}
            inputProps={{ style: { fontFamily: "monospace" } }}
          />
        </Box>
      </Box>
    </ModalComponent>
  );
}

export default AddLeadStatusModal;
