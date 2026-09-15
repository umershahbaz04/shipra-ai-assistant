import React, { useState } from "react";
import Box from "@mui/material/Box";
import { InputLabel } from "@mui/material";
import { useSelector } from "react-redux";
import { CreateClientCarrierTrackingStatus } from "../../../api/AxiosInterceptors";
import UtilityClass from "../../../utilities/UtilityClass";
import { successNotification } from "../../../utilities/toast";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";
import { styleSheet } from "../../../assets/styles/style";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";

export default function CreateCarrierStatusModal(props) {
  let { open, setOpen, getAllCarrierStatus } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const [loading, setLoading] = useState(false);
  const [trackingStatus, setTrackingStatus] = useState("");

  const handleClose = () => {
    setOpen(false);
  };

  const handleSave = async () => {
    if (!trackingStatus || trackingStatus.trim() === "") {
      UtilityClass.showErrorNotificationWithDictionary({ 1: ["Please enter a status name."] });
      return;
    }

    setLoading(true);
    try {
      const payload = {
        trackingStatus: trackingStatus.trim(),
      };
      
      const response = await CreateClientCarrierTrackingStatus(payload);
      if (response.data?.isSuccess) {
        successNotification("Carrier Tracking Status added successfully!");
        handleClose();
        if (getAllCarrierStatus) getAllCarrierStatus();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(response.data?.errors);
      }
    } catch (error) {
      console.error(error);
      UtilityClass.showErrorNotificationWithDictionary({ 1: ["Something went wrong!"] });
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title="Add Carrier Status"
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.GLOBAL_SAVE || "Save"}
          loading={loading}
          bg={purple}
          onClick={handleSave}
        />
      }
    >
      <Box sx={{ mt: 1 }}>
        <InputLabel required sx={styleSheet.inputLabel}>
          Status Name
        </InputLabel>
        <TextFieldComponent
          name="trackingStatus"
          placeholder="Enter Status Name"
          value={trackingStatus}
          onChange={(e) => setTrackingStatus(e.target.value)}
        />
      </Box>
    </ModalComponent>
  );
}
