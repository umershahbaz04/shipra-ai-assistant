import { Box, InputLabel, TextField } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateLeadGridColumn,
  GetAllClientLeadStatusForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import { errorNotification, successNotification } from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function AddLeadTabModal(props) {
  const { open, setOpen, setStatusMoved, onSuccess } = props;
  const [isLoading, setIsLoading] = useState(false);
  const [allLeadStatuses, setAllLeadStatuses] = useState([]);
  const [tabName, setTabName] = useState("");
  const [selectedStatusIds, setSelectedStatusIds] = useState([]);

  const getAllClientLeadStatusForSelection = async () => {
    try {
      const res = await GetAllClientLeadStatusForSelection();
      if (res?.data?.result) {
        setAllLeadStatuses(res.data.result);
      }
    } catch (e) {
      console.error("Error fetching lead statuses:", e);
    }
  };

  useEffect(() => {
    getAllClientLeadStatusForSelection();
  }, []);

  const handleClose = () => setOpen(false);

  const handleSubmit = () => {
    if (!tabName) {
      errorNotification("Please enter tab name");
      return;
    }
    if (selectedStatusIds.length === 0) {
      errorNotification("Please select statuses for this tab");
      return;
    }

    const dashboardStatusIdValues = selectedStatusIds
      .map((s) => s.clientLeadStatusId)
      .join(",");

    const params = {
      columnName: tabName.trim().toUpperCase(),
      dashboardStatusIdValues,
    };

    setIsLoading(true);
    CreateLeadGridColumn(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
        } else {
          successNotification("Tab created successfully");
          if (setStatusMoved) setStatusMoved(true);
          if (onSuccess) onSuccess();
          handleClose();
        }
      })
      .catch(() => errorNotification("Something went wrong"))
      .finally(() => setIsLoading(false));
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title="Add Tab"
      actionBtn={
        <ModalButtonComponent
          title="Add Tab"
          bg={purple}
          onClick={handleSubmit}
          loading={isLoading}
        />
      }
    >
      <Box mb={1}>
        <InputLabel required sx={styleSheet.inputLabel}>
          Tab Name
        </InputLabel>
        <TextField
          onChange={(e) => setTabName(e.target.value)}
          size="small"
          fullWidth
          variant="outlined"
          value={tabName}
        />
      </Box>
      <Box>
        <InputLabel required sx={styleSheet.inputLabel}>
          Select Status
        </InputLabel>
        <SelectComponent
          multiple={true}
          name="leadStatus"
          options={allLeadStatuses}
          value={selectedStatusIds}
          optionLabel={EnumOptions.CLIENT_LEAD_STATUS.LABEL}
          optionValue={EnumOptions.CLIENT_LEAD_STATUS.VALUE}
          isRefesh={true}
          handleRefreshClick={getAllClientLeadStatusForSelection}
          onChange={(e, val) => setSelectedStatusIds(val)}
          padding={"5px"}
        />
      </Box>
    </ModalComponent>
  );
}

export default AddLeadTabModal;
