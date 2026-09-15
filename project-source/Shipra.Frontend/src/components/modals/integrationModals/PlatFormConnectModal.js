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
  CreateSaleChannelConfig,
  GetAllSaleChannelLookupForSelection,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import { purple, s_span } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function PlatFormConnectModal(props) {
  const [isLoading, setIsLoading] = useState(false);
  const [values, setValues] = useState({});
  const [activateData, setActivateData] = useState();
  let { open, setOpen, carrierId, getAll } = props;
  const [allPlatformForSelection, setAllPlatformForSelection] = useState([]);
  const [allStores, setAllStores] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inputFields, setInputFields] = useState({});
  const [store, setStore] = useState({
    storeId: 0,
    storeName: "Select Please",
  });
  const [selectedSaleChannel, setSelectedSaleChannel] = useState({
    saleChannelLookupId: 0,
    saleChannelName: "Select Please",
  });
  const [channelConfigData, setChannelConfigData] = useState("");
  const [saleChannelName, setSaleChannelName] = useState("");
  const [isAllowSaleChannel, setIsAllowSaleChannel] = useState(false);

  const handleClose = () => {
    setOpen(false);
  };
  let getAllStores = async () => {
    let res = await GetStoresForSelection();
    if (res.data.result !== null) {
      setAllStores(res.data.result);
    }
  };
  let gtAllSaleChannelLookupForSelection = async () => {
    let res = await GetAllSaleChannelLookupForSelection();
    if (res.data.result != null) {
      setAllPlatformForSelection(res.data.result);
    }
  };

  useEffect(() => {
    getAllStores();
    gtAllSaleChannelLookupForSelection();
  }, []);
  const handleConnect = async () => {
    setIsLoading(true);
    try {
      if (saleChannelName == "") {
        errorNotification(`Please enter sale channel name`);
        return false;
      }
      if (!store?.storeId || store?.storeId == 0) {
        errorNotification(`Please choose store`);
        return false;
      }
      if (
        !selectedSaleChannel?.saleChannelLookupId ||
        selectedSaleChannel?.saleChannelLookupId == 0
      ) {
        errorNotification(`Please choose store channel`);
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
      activateData.saleChannelLookupId =
        selectedSaleChannel?.saleChannelLookupId;
      let body = {
        inputParameters: activateData,
        storeId: store?.storeId,
        saleChannelName: saleChannelName,
        IsAllowToDisplayInSaleChannel: isAllowSaleChannel,
        IsActive: true,
        saleChannelLookupId: selectedSaleChannel?.saleChannelLookupId,
      };
      const response = await CreateSaleChannelConfig(body);

      if (!response.data.isSuccess) {
        let jsonData = response.data.errors;
        UtilityClass.showErrorNotificationWithDictionary(jsonData);
      } else {
        successNotification("Action perform successfully");
        handleClose();
        getAll(true);
      }
    } catch (error) {
      console.error("Something went wrong!", error.response);
      errorNotification("Something went wrong!");
    } finally {
      setIsLoading(false);
    }
  };
  useEffect(() => {
    if (channelConfigData) {
      const parsedData = JSON.parse(channelConfigData);
      const updatedData = Object.entries(parsedData).reduce(
        (acc, [key, value]) => {
          if (key !== "selectedSaleChannel") {
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
  }, [channelConfigData]);
  useEffect(() => {
    if (
      selectedSaleChannel?.saleChannelLookupId ||
      selectedSaleChannel?.saleChannelLookupId > 0
    ) {
      let data = allPlatformForSelection.find(
        (x) => x.saleChannelLookupId == selectedSaleChannel.saleChannelLookupId
      );
      setChannelConfigData(data?.inputRequiredConfig);
    } else {
      setChannelConfigData();
      setInputFields();
    }
  }, [selectedSaleChannel]);

  const handleFocus = (event) => event.target.select();
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={
        LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_ADD_SALE_CHANNEL
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.INTEGRATION_SALE_CHANNEL_ADD_SALE_CHANNEL
          }
          loading={isLoading}
          bg={purple}
          onClick={(e) => handleConnect()}
        />
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel} required>
            {
              LanguageReducer?.languageType
                ?.INTEGRATION_SALE_CHANNEL_SALE_CHANNEL_NAME
            }
          </InputLabel>
          <TextField
            placeholder={"Channel Name"}
            size="small"
            fullWidth
            variant="outlined"
            value={saleChannelName}
            onChange={(e) => setSaleChannelName(e.target.value)}
          />
        </Grid>
        <Grid item md={6} sm={12} xs={12}>
          {" "}
          <InputLabel sx={styleSheet.inputLabel} required>
            {LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_STORE_NAME}
          </InputLabel>
          <SelectComponent
            name="store"
            options={allStores}
            value={store}
            optionLabel={EnumOptions.STORE.LABEL}
            optionValue={EnumOptions.STORE.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllStores}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setStore(resolvedId);
            }}
            size={"md"}
          />
        </Grid>
        <Grid item md={6} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel} required>
            {
              LanguageReducer?.languageType
                ?.INTEGRATION_SALE_CHANNEL_SALE_CHANNEL_TYPE
            }
          </InputLabel>
          <SelectComponent
            name="platform"
            size="md"
            options={allPlatformForSelection}
            value={selectedSaleChannel}
            optionLabel={EnumOptions.SALE_CHANNEL.LABEL}
            optionValue={EnumOptions.SALE_CHANNEL.VALUE}
            isRefesh={true}
            handleRefreshClick={gtAllSaleChannelLookupForSelection}
            getOptionLabel={(option) => option?.saleChannelName}
            onChange={(e, val) => {
              setSelectedSaleChannel(val);
              !val && setChannelConfigData("");
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
        {" "}
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
              checked={isAllowSaleChannel}
              onChange={(e) => setIsAllowSaleChannel(e.target.checked)}
              edge="start"
              disableRipple
              defaultChecked
            />
          }
          label={
            LanguageReducer?.languageType
              ?.INTEGRATION_SALE_CHANNEL_ALLOW_TO_DISPLAY
          }
        />
      </Grid>
    </ModalComponent>
  );
}
export default PlatFormConnectModal;
