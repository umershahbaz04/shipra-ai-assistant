import { Box, InputLabel } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateDriverStatues as CreateDriverCtssetting,
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
function AddDriverStatusModal(props) {
  let { open, setOpen, exsitingStatues, getAllDriverStatus } = props;
  const [isLoading, setIsLoading] = React.useState(false);
  const [allCarrierTrackingStatus, setAllCarrierTrackingStatus] =
    React.useState([]);
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
  useEffect(() => {
    if (exsitingStatues && exsitingStatues.length > 0) {
      const filteredArray = allCarrierTrackingStatus?.filter((obj1) =>
        exsitingStatues.some(
          (obj2) =>
            obj1.carrierTrackingStatusId === obj2.CarrierTrackingStatusId
        )
      );

      setSelectedStatusIds(filteredArray);
    }
  }, [allCarrierTrackingStatus]);
  const handleClose = () => {
    setOpen(false);
  };
  const handleSubmit = () => {
    if (selectedStatusIds.length == 0) {
      errorNotification("Please choose statuses.");
      return false;
    }
    var dashboardStatusIdValues = selectedStatusIds
      .map((status) => status.carrierTrackingStatusId)
      .join(",");
    let params = {
      carrierTrackingStatusIds: dashboardStatusIdValues,
    };
    setIsLoading(true);
    CreateDriverCtssetting(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
          // setIsLoading(false);
        } else {
          successNotification("Status Created successfully");
          handleClose();
          getAllDriverStatus();
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to Create");
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  console.log(selectedStatusIds);
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={
        LanguageReducer?.languageType?.SETTING_DRIVER_STATUSES_ADD_DRIVER_STATUS
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.SETTING_DRIVER_STATUSES_ADD_DRIVER_STATUS
          }
          loading={isLoading}
          bg={purple}
          onClick={(e) => handleSubmit()}
        />
      }
    >
      <Box>
        <InputLabel required sx={styleSheet.inputLabel}>
          {LanguageReducer?.languageType?.SETTING_DRIVER_STATUSES_CHOOSE_STATUS}
        </InputLabel>
        <SelectComponent
          multiple={true}
          name="driverStatus"
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
export default AddDriverStatusModal;
