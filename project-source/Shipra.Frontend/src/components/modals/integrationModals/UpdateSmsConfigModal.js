import {
  Checkbox,
  FormControlLabel,
  Grid,
  InputLabel,
  Switch,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  ActivateSmsProcess,
  GetAllSMSLookupForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function UpdateSmsConfigModal(props) {
  const [isLoading, setIsLoading] = useState(false);
  const [values, setValues] = useState({});
  const [activateData, setActivateData] = useState();
  const [allSmsService, setAllSmsService] = useState([]);

  const [isDefault, setIsDefault] = useState(false);
  const [isActiveChecked, setIsActiveChecked] = useState(false);
  const [isDeleted, setIsDeleted] = useState(false);

  let { open, setOpen, carrierId, getAll, data } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inputFields, setInputFields] = useState({});
  const [service, setService] = useState({
    smsLookupId: 0,
    serviceName: "Select Please",
  });

  const [smsConfigData, setSmsConfigData] = useState("");

  const handleClose = () => {
    setOpen(false);
  };
  let getAllSMSLookupForSelection = async () => {
    let res = await GetAllSMSLookupForSelection();
    if (res.data.result !== null) {
      let allRecords = res.data.result;
      let obj = allRecords?.find((x) => x.smsLookupId == data.smsLookupId);
      if (obj) {
        let updatedConfig = UtilityClass.updateDictionaryObjectValue(
          obj.inputRequiredConfig,
          data.config
        );

        obj.inputRequiredConfig = updatedConfig;

        let index = allRecords.findIndex(
          (x) => x.smsLookupId == data.smsLookupId
        );

        if (index !== -1) {
          allRecords[index] = obj;
        }
        setAllSmsService(allRecords);
      } else {
        setAllSmsService(allRecords);
      }
    }
  };

  useEffect(() => {
    if (data) {
      if (allSmsService && allSmsService.length > 0) {
        let defRecord = allSmsService.find(
          (x) => x?.smsLookupId == data?.smsLookupId
        );

        let updatedConfig = UtilityClass.updateDictionaryObjectValue(
          defRecord.inputRequiredConfig,
          data.config
        );

        setService(defRecord);
        setInputFields(updatedConfig);
        setSmsConfigData(updatedConfig);

        setIsActiveChecked(data?.active);
        setIsDefault(data?.isDefault);
        setIsDeleted(!data?.active);
      }
    }
  }, [allSmsService]);
  useEffect(() => {
    getAllSMSLookupForSelection();
  }, []);
  const handleConnect = async () => {
    setIsLoading(true);
    console.log("activate data function", activateData);
    try {
      if (!service?.smsLookupId || service?.smsLookupId == 0) {
        errorNotification(`Please choose sms service`);
        return false;
      }

      let isError = false;
      for (var key in activateData) {
        if (activateData[key] === "") {
          errorNotification(`The ${key}: Is required to proceed`);
          isError = true;
        }
      }
      //if any property is not filled then return with error
      if (isError) {
        return false;
      }
      activateData.SMSlookupId = service?.smsLookupId;
      let body = {
        inputParameters: activateData,
        IsActive: isActiveChecked,
        isDefault: isDefault,
      };
      console.log("body", body);
      const response = await ActivateSmsProcess(body);

      if (!response.data.isSuccess) {
        let jsonData = response.data.errors;
        UtilityClass.showErrorNotificationWithDictionary(jsonData);
      } else {
        successNotification(
          LanguageReducer?.languageType?.SUCCESSFULLY_CONNECT_TOAST
        );
        handleClose();
        getAll(true);
      }
    } catch (error) {
      console.error("Error to connect", error.response);
      errorNotification(LanguageReducer?.languageType?.UNABLE_TO_CONNECT_TOAST);
    } finally {
      setIsLoading(false);
    }
  };
  useEffect(() => {
    if (smsConfigData) {
      const parsedData = JSON.parse(smsConfigData);
      setActivateData(parsedData);
      const { PlatformLookupId, ...fields } = parsedData;
      setInputFields(fields);
    }
  }, [smsConfigData]);

  useEffect(() => {
    if (service.smsLookupId || service.smsLookupId > 0) {
      let data = allSmsService.find(
        (x) => x.smsLookupId == service.smsLookupId
      );
      setSmsConfigData(data?.inputRequiredConfig);
    } else {
      setSmsConfigData();
      setInputFields();
    }
  }, [service]);

  const handleFocus = (event) => event.target.select();

  const handelAction = (isActiveClicked) => {
    if (isActiveClicked) {
      setIsDeleted(false);
      //  handleActive();
    } else {
      setIsDeleted(true);

      //  handleDelete();
    }
  };
  const handleActiveInActive = (e) => {
    handelAction(!isActiveChecked);
    setIsActiveChecked(!isActiveChecked);
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={"Update Sms Config"}
      actionBtn={
        <ModalButtonComponent
          title={"Update Sms Config"}
          bg={purple}
          onClick={(e) => handleConnect()}
          loading={isLoading}
        />
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>Select Sms Service</InputLabel>
          <SelectComponent
            disabled
            name="sms"
            options={allSmsService}
            value={service}
            optionLabel={EnumOptions.SELECT_SMS_SERVICES.LABEL}
            optionValue={EnumOptions.SELECT_SMS_SERVICES.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllSMSLookupForSelection}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setService(resolvedId);
            }}
          />
        </Grid>
        {inputFields && Object.keys(inputFields).length > 0 ? (
          <>
            {Object.entries(inputFields).map(
              ([key, value]) =>
                key !== "PlatformLookupId" && (
                  <Grid key={key} item md={12} sm={12}>
                    <InputLabel sx={styleSheet.inputLabel}>{key}</InputLabel>
                    <TextField
                      disabled={isDeleted}
                      onFocus={handleFocus}
                      placeholder={value}
                      size="small"
                      fullWidth
                      variant="outlined"
                      value={(activateData && activateData[key]) || ""}
                      onChange={(e) =>
                        setActivateData({
                          ...activateData,
                          [key]: e.target.value,
                        })
                      }
                    />
                  </Grid>
                )
            )}
          </>
        ) : null}
      </Grid>
      <Grid item md={12} sm={12} sx={{ textAlign: "right" }}>
        <FormControlLabel
          // sx={{ margin: "3px 0px" }}
          control={
            <Checkbox
              disabled={isDeleted || data.isDefault}
              sx={{
                color: "var(--primary-color)",
                "&.Mui-checked": {
                  color: "var(--primary-color)",
                },
              }}
              checked={isDefault}
              onChange={(e) => setIsDefault(e.target.checked)}
              edge="start"
              disableRipple
              defaultChecked
            />
          }
          label={"Mark as default"}
        />
        {!isDefault && (
          <FormControlLabel
            control={
              <Switch
                checked={isActiveChecked}
                defaultChecked={isActiveChecked}
                onClick={() => {
                  handleActiveInActive();
                }}
              />
            }
            label={isActiveChecked ? "Active" : "In Active"}
          />
        )}
      </Grid>
    </ModalComponent>
  );
}
export default UpdateSmsConfigModal;
