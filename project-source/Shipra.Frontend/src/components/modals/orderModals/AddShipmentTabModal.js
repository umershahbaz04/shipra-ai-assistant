import { Box, InputLabel, TextField } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateShipmentGridColumn,
  GetAllCarrierTrackingStatusForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function AddShipmentTabModal(props) {
  let { open, setOpen, setStatusMoved } = props;
  const [isLoading, setIsLoading] = React.useState(false);
  const [allCarrierTrackingStatus, setAllCarrierTrackingStatus] =
    React.useState([]);
  const [tabName, setTabName] = React.useState("");
  const [selectedStatusIds, setSelectedStatusIds] = useState([]);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const MAX_TAGS = 1000;

  let getAllCarrierTrackingStatusForSelection = async () => {
    let res = await GetAllCarrierTrackingStatusForSelection();

    if (res.data.result !== null) {
      if (res.data.result.find((x) => x?.carrierTrackingStatusId == 0)) {
        res.data.result = res.data.result?.filter(
          (x) => x?.carrierTrackingStatusId != 0
        );
      }

      setAllCarrierTrackingStatus(res.data.result);
    }
  };
  useEffect(() => {
    getAllCarrierTrackingStatusForSelection();
  }, []);

  const handleClose = () => {
    setOpen(false);
  };
  const handleSubmit = () => {
    if (tabName == "") {
      errorNotification("Please Enter tab name");
      return false;
    }
    if (selectedStatusIds.length == 0) {
      errorNotification("Please select statuses for this tab");
      return false;
    }
    var dashboardStatusIdValues = selectedStatusIds
      .map((status) => status.carrierTrackingStatusId)
      .join(",");
    let params = {
      columnName: tabName?.trim().toUpperCase(),
      dashboardStatusIdValues: dashboardStatusIdValues,
    };
    setIsLoading(true);
    CreateShipmentGridColumn(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
          // setIsLoading(false);
        } else {
          successNotification("Tab Created successfully");
          setStatusMoved(true);
          handleClose();
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_ADD_TAB}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_ADD_TAB}
          bg={purple}
          onClick={(e) => handleSubmit()}
          loading={isLoading}
        />
      }
    >
      <Box mb={1}>
        <InputLabel required sx={styleSheet.inputLabel}>
          {LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_TAB_NAME}
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
          {LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_SELECT_STATUS}
        </InputLabel>
        <SelectComponent
          multiple={true}
          name="reason"
          options={allCarrierTrackingStatus}
          value={selectedStatusIds}
          optionLabel={EnumOptions.CHOOSE_STATUS.LABEL}
          optionValue={EnumOptions.CHOOSE_STATUS.VALUE}
          isRefesh={true}
          handleRefreshClick={getAllCarrierTrackingStatusForSelection}
          onChange={(e, val) => {
            setSelectedStatusIds(val);
          }}
          padding={"5px "}
        />
      </Box>
    </ModalComponent>
  );
}
export default AddShipmentTabModal;
