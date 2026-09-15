import React, { useState, useEffect } from "react";
import { Box, InputLabel } from "@mui/material";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import Colors from "../../../utilities/helpers/Colors";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { UpdateLeadStatus, GetAllClientLeadStatusForSelection } from "../../../api/AxiosInterceptors";
import { fetchMethod } from "../../../utilities/helpers/Helpers";
import { warningNotification, successNotification } from "../../../utilities/toast";
import { styleSheet } from "../../../assets/styles/style";

const UpdateLeadStatusModal = ({ open, handleClose, lead, onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [selectedStatus, setSelectedStatus] = useState(null);
  const [allStatus, setAllStatus] = useState([]);

  useEffect(() => {
    if (open) {
      fetchStatuses();
    } else {
      setSelectedStatus(null);
    }
  }, [open]);

  useEffect(() => {
    if (open && allStatus.length > 0 && lead?.leadStatusId) {
      // Use == to ignore potential string/number type differences
      const currentStatus = allStatus.find(s => s.value == lead.leadStatusId);
      setSelectedStatus(currentStatus || null);
    }
  }, [open, lead, allStatus]);

  const fetchStatuses = async () => {
    try {
      const res = await fetchMethod(GetAllClientLeadStatusForSelection);
      if (res?.response?.result) {
        setAllStatus(
          res.response.result
            .filter((x) => x.description?.toLowerCase() !== "completed")
            .map((x) => ({
              label: x.description,
              value: x.clientLeadStatusId,
            }))
        );
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleSave = async () => {
    if (!selectedStatus) {
      warningNotification("Please select a status");
      return;
    }

    if (selectedStatus.value == lead?.leadStatusId) {
      warningNotification("Status is already set to the selected value");
      return;
    }

    setLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        UpdateLeadStatus({
          leadId: lead.leadId,
          leadStatusId: selectedStatus.value
        })
      );

      if (response?.isSuccess) {
        successNotification("Status updated successfully");
        onSuccess();
        handleClose();
      }
    } catch (error) {
      console.error("Error updating lead status:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title="Update Lead Status"
      actionBtn={
        <ModalButtonComponent
          title="Update"
          bg={Colors.primary}
          onClick={handleSave}
          loading={loading}
        />
      }
    >
      <Box mb={2}>
        <InputLabel required sx={styleSheet.inputLabel}>
          Lead Status
        </InputLabel>
        <SelectComponent
          name="leadStatus"
          options={allStatus}
          value={selectedStatus}
          optionLabel="label"
          optionValue="value"
          onChange={(name, val) => setSelectedStatus(val)}
        />
      </Box>
    </ModalComponent>
  );
};

export default UpdateLeadStatusModal;

