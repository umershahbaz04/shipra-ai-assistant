import {
  Checkbox,
  FormControlLabel,
  Grid,
  InputLabel,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  ActivatePaymentProcess,
  GetAllPPLookupForSelection,
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
function PaymentConfigModal(props) {
  const [isLoading, setIsLoading] = useState(false);
  const [values, setValues] = useState({});
  const [activateData, setActivateData] = useState();
  let { open, setOpen, carrierId, getAll } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inputFields, setInputFields] = useState({});
  const [service, setService] = useState({
    pplookupId: 0,
    ppname: "Please Select",
  });

  const [smsConfigData, setSmsConfigData] = useState("");
  const [allPPLookupForSelection, setAllPPLookupForSelection] = useState([]);
  const [isDefault, setIsDefault] = useState(false);

  const handleClose = () => {
    setOpen(false);
  };
  let getAllSMSLookupForSelection = async () => {
    let res = await GetAllPPLookupForSelection();
    if (res.data.result !== null) {
      setAllPPLookupForSelection(res.data.result);
    }
  };
  useEffect(() => {
    getAllSMSLookupForSelection();
  }, []);
  const handleConnect = async () => {
    setIsLoading(true);
    try {
      if (!service?.pplookupId || service?.pplookupId == 0) {
        errorNotification(`Please choose payment process`);
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
      activateData.pplookupId = service?.pplookupId;
      let body = {
        inputParameters: activateData,
        IsActive: true,
        isDefault: isDefault,
      };
      const response = await ActivatePaymentProcess(body);

      if (!response.data.isSuccess) {
        let jsonData = response.data.errors;
        UtilityClass.showErrorNotificationWithDictionary(jsonData);
      } else {
        successNotification("Configured successfully");
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
      const updatedData = Object.entries(parsedData).reduce(
        (acc, [key, value]) => {
          if (key !== "pplookupId") {
            acc[key] = "";
          } else {
            acc[key] = carrierId;
          }
          return acc;
        },
        {}
      );
      setActivateData(updatedData);
      const { pplookupId, ...fields } = parsedData;
      setInputFields(fields);
    }
  }, [smsConfigData]);

  useEffect(() => {
    if (service.pplookupId || service.pplookupId > 0) {
      let data = allPPLookupForSelection.find(
        (x) => x.pplookupId == service.pplookupId
      );
      setSmsConfigData(data?.inputRequiredConfig);
    } else {
      setSmsConfigData();
      setInputFields();
    }
  }, [service]);

  const handleFocus = (event) => event.target.select();

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={
        LanguageReducer?.languageType
          ?.INTEGRATION_PAYMENT_INTEGRATION_ADD_PAYMENT_CONFIG
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.INTEGRATION_PAYMENT_INTEGRATION_ADD_PAYMENT_CONFIG
          }
          onClick={(e) => handleConnect()}
          loading={isLoading}
          bg={purple}
        />
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {
              LanguageReducer?.languageType
                ?.INTEGRATION_PAYMENT_INTEGRATION_SELECT_PAYMENT_PROCESSOR
            }
          </InputLabel>
          <SelectComponent
            name="pp"
            options={allPPLookupForSelection}
            value={service}
            optionLabel={EnumOptions.SELECT_PAYMENT_PROCESSOR.LABEL}
            optionValue={EnumOptions.SELECT_PAYMENT_PROCESSOR.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllSMSLookupForSelection}
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
                key !== "pplookupId" && (
                  <Grid key={key} item md={12} sm={12} xs={12}>
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
            // sx={{ margin: "3px 0px" }}
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
                disableRipple
              />
            }
            label={
              LanguageReducer?.languageType
                ?.INTEGRATION_PAYMENT_INTEGRATION_MARK_AS_DEFAULT
            }
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
}
export default PaymentConfigModal;
