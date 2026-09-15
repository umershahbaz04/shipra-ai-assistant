import {
  Checkbox,
  FormControlLabel,
  Grid,
  InputLabel,
  TextField,
} from "@mui/material";
import { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  ActivateWhatsappProcess,
  GetAllWhatsappLookupForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import { useSelector } from "react-redux";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const AddWhatsAppModel = (props) => {
  const { open, onClose, carrierId, getAllWhatsappActivate } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleFocus = (event) => event.target.select();
  const [loading, setLoading] = useState(false);
  const [service, setService] = useState();
  const [allWhatsappService, setAllWhatsappService] = useState([]);
  const [isDefault, setIsDefault] = useState(false);
  const [inputFields, setInputFields] = useState({});
  const [activateData, setActivateData] = useState();
  const [whatsAppConfigData, setWhatsAppConfigData] = useState("");
  const [whatsAppConfig, setWhatsAppConfig] = useState("");

  const getAllWhatsappLookupForSelection = async () => {
    try {
      const response = await GetAllWhatsappLookupForSelection();
      if (response.data.isSuccess) {
        const filteredResult = response.data.result.slice(1);
        setAllWhatsappService(filteredResult);
      }
    } catch (e) {}
  };

  const handleConnect = async () => {
    setLoading(true);
    try {
      if (!service?.whatsAppLookupId || service?.whatsAppLookupId == 0) {
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
      const body = {
        inputParameters: {
          ...activateData,
          WhatsapplookupId: service?.whatsAppLookupId,
        },
        IsActive: true,
        isDefault: false,
      };
      // console.log("body", body);
      const response = await ActivateWhatsappProcess(body);
      if (!response.data.isSuccess) {
        let jsonData = response.data.errors;
        UtilityClass.showErrorNotificationWithDictionary(jsonData);
      } else {
        successNotification(
          LanguageReducer?.languageType?.SUCCESSFULLY_CONNECT_TOAST
        );
        onClose();
        getAllWhatsappActivate();
      }
    } catch (error) {
      console.error("Error to connect", error.response);
      errorNotification(LanguageReducer?.languageType?.UNABLE_TO_CONNECT_TOAST);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (whatsAppConfigData) {
      const parsedData = JSON.parse(whatsAppConfigData?.inputRequiredConfig);
      const updatedData = Object.entries(parsedData).reduce(
        (acc, [key, value]) => {
          if (key !== "whatsAppLookupId") {
            acc[key] = "";
          } else {
            acc[key] = carrierId;
          }
          return acc;
        },
        {}
      );
      setActivateData(updatedData);
      const { PlatformLookupId, ...fields } = parsedData;
      setInputFields(fields);
    }
  }, [whatsAppConfigData]);

  useEffect(() => {
    if (service?.whatsAppLookupId || service?.whatsAppLookupId > 0) {
      let data = allWhatsappService.find(
        (x) => x.whatsAppLookupId == service?.whatsAppLookupId
      );
      setWhatsAppConfigData(data);
    } else {
      setWhatsAppConfigData();
      setInputFields();
    }
  }, [service]);

  useEffect(() => {
    getAllWhatsappLookupForSelection();
  }, []);

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Add WhatsApp Config"}
      actionBtn={
        <ModalButtonComponent
          title={"Add WhatsApp Config"}
          bg={purple}
          onClick={(e) => handleConnect()}
          loading={loading}
        />
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {
              LanguageReducer?.languageType
                ?.INTEGRATION_SMS_INTEGRATION_SELECT_SMS_SERVICE
            }
          </InputLabel>
          <SelectComponent
            name="sms"
            options={allWhatsappService}
            value={service}
            optionLabel={EnumOptions.WHATSAPP_SERVICES.LABEL}
            optionValue={EnumOptions.WHATSAPP_SERVICES.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllWhatsappLookupForSelection}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setService(resolvedId);
            }}
            size={"md"}
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
        <Grid item md={12} sm={12} sx={{ textAlign: "right" }}>
          <FormControlLabel
            control={
              <Checkbox
                sx={{
                  color: "var(--primary-color)",
                  "&.Mui-checked": {
                    color: "var(--primary-color)",
                  },
                }}
                checked={isDefault}
                onChange={(e) => setIsDefault(e.target.checked)}
                edge="start"
              />
            }
            label={
              LanguageReducer?.languageType
                ?.INTEGRATION_SMS_INTEGRATION_MARK_AS_DEFAULT
            }
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
};

export default AddWhatsAppModel;
