import React, { useEffect, useState } from "react";
import { Box, InputLabel, TextField } from "@mui/material";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import Colors from "../../../utilities/helpers/Colors";
import { styleSheet } from "../../../assets/styles/style";
import { fetchMethod } from "../../../utilities/helpers/Helpers";
import { CreateClientLeadStatus, UpdateClientLeadStatus } from "../../../api/AxiosInterceptors";
import { errorNotification, successNotification, warningNotification } from "../../../utilities/toast";

export default function AddClientLeadStatusModal({ open, setOpen, editingStatus, onSuccess }) {
  const [loading, setLoading] = useState(false);
  const [description, setDescription] = useState("");
  const [displayOrder, setDisplayOrder] = useState("");

  useEffect(() => {
    if (editingStatus) {
      setDescription(editingStatus.Description || editingStatus.description || "");
      setDisplayOrder(editingStatus.DisplayOrder || editingStatus.displayOrder || "");
    } else {
      setDescription("");
      setDisplayOrder("");
    }
  }, [editingStatus]);

  const handleClose = () => {
    setOpen(false);
  };

  const handleSave = async () => {
    if (!description.trim()) {
      warningNotification("Description is required");
      return;
    }
    if (!displayOrder) {
      warningNotification("Display Order is required");
      return;
    }

    setLoading(true);
    try {
      const payload = {
        Description: description,
        DisplayOrder: parseInt(displayOrder)
      };

      let apiCall;
      if (editingStatus) {
        payload.ClientLeadStatusId = editingStatus.ClientLeadStatusId || editingStatus.clientLeadStatusId || editingStatus.Id || editingStatus.id;
        apiCall = UpdateClientLeadStatus;
      } else {
        apiCall = CreateClientLeadStatus;
      }

      const res = await fetchMethod(() => apiCall(payload));
      
      if (res?.response?.isSuccess) {
        successNotification(editingStatus ? "Status updated successfully" : "Status created successfully");
        onSuccess();
        handleClose();
      } else {
        errorNotification("Error while saving");
      }
    } catch (error) {
      console.error(error);
      errorNotification("Failed to save status");
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      title={editingStatus ? "Edit Lead Status" : "Add Lead Status"}
      maxWidth="sm"
      actionBtn={
        <ModalButtonComponent
          title={editingStatus ? "Update" : "Save"}
          bg={Colors.primary}
          loading={loading}
          onClick={handleSave}
        />
      }
    >
      <Box display="flex" flexDirection="column" gap={2}>
        <Box>
          <InputLabel required sx={styleSheet.inputLabel}>
            Description
          </InputLabel>
          <TextField
            fullWidth
            size="small"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Enter status description"
          />
        </Box>
        <Box>
          <InputLabel required sx={styleSheet.inputLabel}>
            Display Order
          </InputLabel>
          <TextField
            fullWidth
            type="number"
            size="small"
            value={displayOrder}
            onChange={(e) => setDisplayOrder(e.target.value)}
            placeholder="Enter display order (e.g. 1)"
          />
        </Box>
      </Box>
    </ModalComponent>
  );
}
