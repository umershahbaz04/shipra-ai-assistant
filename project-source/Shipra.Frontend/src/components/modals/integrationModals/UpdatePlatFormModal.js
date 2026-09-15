import {
  Checkbox,
  FormControlLabel,
  FormGroup,
  Grid,
  InputLabel,
  Radio,
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
  GetAllSaleChannelLookupForSelection,
  GetStoresForSelection,
  UpdateSaleChannelConfig,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions, inputTypesEnum } from "../../../utilities/enum";
import {
  CustomColorLabelledOutline,
  getLowerCase,
  getTrimValue,
  GridContainer,
  GridItem,
  purple,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { da } from "date-fns/locale";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function UpdatePlatFormModal(props) {
  let { open, setOpen, carrierId, getAll, data, allPlatformForSelection } =
    props;

  const [isLoading, setIsLoading] = useState(false);
  const [activateData, setActivateData] = useState();
  const [allStores, setAllStores] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inputFields, setInputFields] = useState([]);
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
  const [isActiveChecked, setIsActiveChecked] = useState(false);
  const [isDeleted, setIsDeleted] = useState(false);
  const [settingConfig, setSettingConfig] = useState();
  const [config, setconfig] = useState();

  const handleClose = () => {
    setOpen(false);
  };
  const handelAction = (isActiveClicked) => {
    if (isActiveClicked) {
      setIsDeleted(false);
    } else {
      setIsDeleted(true);
    }
  };
  const handleActiveInActive = (e) => {
    handelAction(!isActiveChecked);
    setIsActiveChecked(!isActiveChecked);
  };
  const handleInputChange = (sectionIndex, inputIndex, value) => {
    setSettingConfig((prev) => {
      const _configSettings = [...prev];
      _configSettings[sectionIndex].inputData[inputIndex].value = value;
      return _configSettings;
    });
  };
  const findValueByKey = (data, keyToFind) => {
    for (const section of data) {
      for (const item of section.inputData) {
        if (item.key === keyToFind) {
          return { [keyToFind]: item.value };
        }
      }
    }
    return null;
  };
  const handleConfig = (config, data) => {
    const updateData = JSON.parse(config);
    const result = {};
    for (const key in updateData) {
      if (updateData.hasOwnProperty(key)) {
        const value = findValueByKey(data, `${key}`);
        if (value) {
          result[key] = value[`${key}`];
        }
      }
    }
    return result;
  };
  let getAllStores = async () => {
    let res = await GetStoresForSelection();
    if (res.data.result !== null) {
      setAllStores(res.data.result);
    }
  };

  const handleConnect = async () => {
    setIsLoading(true);
    const _submitData = settingConfig.map((section) => {
      const _section = { ...section };
      const _section_inputData = section.inputData.map((input_dt) => {
        const _input_data = { ...input_dt };
        if (!getTrimValue(input_dt.value) || input_dt.value.value === 0) {
          errorNotification(`The ${input_dt.label}: Is required to proceed`);
          return false;
        }
        if (getLowerCase(_input_data.type) === inputTypesEnum.SELECT) {
          const manipulated_data = _input_data.data.map((option) => {
            return option.value;
          });

          _input_data.data = manipulated_data;
          _input_data.value = input_dt.value.value;
        }
        return _input_data;
      });
      _section.inputData = _section_inputData;
      return _section;
    });
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

      const updatedInputConfigData = handleConfig(config, _submitData);
      let body = {
        inputParameters: updatedInputConfigData,
        storeId: store?.storeId,
        saleChannelName: saleChannelName,
        IsAllowToDisplayInSaleChannel: isAllowSaleChannel,
        IsActive: isActiveChecked,
        saleChannelLookupId: selectedSaleChannel?.saleChannelLookupId,
        saleChannelConfigId: data?.saleChannelConfigId,
        settingConfig: _submitData,
      };
      console.log(body);
      const response = await UpdateSaleChannelConfig(body);

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
    if (data) {
      let settingupdatedConfig = JSON.parse(data.settingConfig);
      let updatedConfig = data.config;

      const _configSettings = settingupdatedConfig?.map((section) => {
        const _section = { ...section };
        _section.inputData = section.inputData?.map((input) => {
          const _input = { ...input };
          if (
            getLowerCase(_input.type) === inputTypesEnum.SELECT &&
            _input.data
          ) {
            _input.data = _input.data?.map((option) => ({
              label: option,
              value: option,
            }));
            _input.value = { label: _input.value, value: _input.value };
          }
          return _input;
        });
        return _section;
      });
      setconfig(updatedConfig);
      setInputFields(settingupdatedConfig);
      setSettingConfig(_configSettings);
    }
  }, [allPlatformForSelection, data]);
  console.log(settingConfig);
  useEffect(() => {
    if (data) {
      if (allStores && allStores.length > 0) {
        let defRecord = allStores.find((x) => x?.storeId == data?.storeId);
        setStore(defRecord);
        setIsActiveChecked(data?.active);
        setIsDeleted(!data?.active);
        setSaleChannelName(data?.saleChannelName);
        setIsAllowSaleChannel(data?.isAllowToDisplayInSaleChannel);
      }
    }
  }, [allStores]);
  useEffect(() => {
    if (data) {
      if (allPlatformForSelection && allPlatformForSelection.length > 0) {
        let defRecord = allPlatformForSelection.find(
          (x) => x?.saleChannelLookupId == data?.saleChannelLookupId
        );
        setSelectedSaleChannel(defRecord);
      }
    }
  }, [allPlatformForSelection]);

  useEffect(() => {
    getAllStores();
  }, []);
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={"Update Sale Channel"}
      actionBtn={
        <ModalButtonComponent
          title={"Update Sale Channel"}
          bg={purple}
          onClick={(e) => handleConnect()}
          loading={isLoading}
        />
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>Channel Name</InputLabel>
          <TextField
            disabled={!isActiveChecked}
            placeholder={"Channel Name"}
            size="small"
            fullWidth
            variant="outlined"
            value={saleChannelName}
            onChange={(e) => setSaleChannelName(e.target.value)}
          />
        </Grid>
        <Grid item md={12} sm={12} xs={12}>
          {" "}
          <InputLabel sx={styleSheet.inputLabel}>Select Store</InputLabel>
          <SelectComponent
            disabled
            name="store"
            options={allStores}
            value={store}
            optionLabel={EnumOptions.STORE.LABEL}
            optionValue={EnumOptions.STORE.VALUE}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setStore(resolvedId);
            }}
          />
        </Grid>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            Select Sale Channel
          </InputLabel>
          <SelectComponent
            disabled
            name="platform"
            size="md"
            options={allPlatformForSelection}
            value={selectedSaleChannel}
            optionLabel={EnumOptions.SALE_CHANNEL.LABEL}
            optionValue={EnumOptions.SALE_CHANNEL.VALUE}
            onChange={(e, val) => {
              setSelectedSaleChannel(val);
              !val && setChannelConfigData("");
            }}
          />
        </Grid>
        {settingConfig ? (
          <GridContainer>
            {settingConfig.length > 0 &&
              settingConfig?.map((section, sectionIndex) => (
                <GridItem lg={12} md={12} sm={12} xs={12} key={section.key}>
                  <CustomColorLabelledOutline label={section.sectionName}>
                    <GridContainer>
                      {section.inputData.map((input, inputIndex) => (
                        <GridItem
                          lg={6}
                          md={6}
                          sm={6}
                          xs={12}
                          key={input.key}
                          sx={{
                            paddingTop: "8px !important",
                            marginBottom: "2px !important",
                          }}
                        >
                          <InputLabel
                            required={input.required}
                            sx={styleSheet.inputLabel}
                          >
                            {input.label}
                          </InputLabel>

                          {getLowerCase(input.type) ===
                            inputTypesEnum.SELECT && (
                            <SelectComponent
                              height={40}
                              name={input.key}
                              options={input.data}
                              optionLabel={
                                EnumOptions.CONFIG_SETTING_INPUTDATA_OPTIONS
                                  .LABEL
                              }
                              optionValue={
                                EnumOptions.CONFIG_SETTING_INPUTDATA_OPTIONS
                                  .VALUE
                              }
                              value={input.value}
                              onChange={(e, val) => {
                                handleInputChange(
                                  sectionIndex,
                                  inputIndex,
                                  val
                                );
                              }}
                            />
                          )}

                          {(getLowerCase(input.type) === inputTypesEnum.TEXT ||
                            getLowerCase(input.type) ===
                              inputTypesEnum.NUMBER) && (
                            <TextField
                              type={getLowerCase(input.type)}
                              placeholder={
                                input.value !== ""
                                  ? input.value
                                  : "Please Enter a value"
                              }
                              size="small"
                              fullWidth
                              variant="outlined"
                              required={input.required}
                              value={input.value}
                              onChange={(e) => {
                                const val = e.target.value;
                                handleInputChange(
                                  sectionIndex,
                                  inputIndex,
                                  val
                                );
                              }}
                            />
                          )}
                          {getLowerCase(input.type) ===
                            inputTypesEnum.RADIO && (
                            <FormGroup
                              sx={{ display: "flex", flexDirection: "row" }}
                            >
                              <FormControlLabel
                                control={
                                  <Radio
                                    checked={input.value === true}
                                    onChange={() =>
                                      handleInputChange(
                                        sectionIndex,
                                        inputIndex,
                                        true
                                      )
                                    }
                                  />
                                }
                                label="Yes"
                              />
                              <FormControlLabel
                                control={
                                  <Radio
                                    checked={input.value === false}
                                    onChange={() =>
                                      handleInputChange(
                                        sectionIndex,
                                        inputIndex,
                                        false
                                      )
                                    }
                                  />
                                }
                                label="No"
                              />
                            </FormGroup>
                          )}
                        </GridItem>
                      ))}
                    </GridContainer>
                  </CustomColorLabelledOutline>
                </GridItem>
              ))}
          </GridContainer>
        ) : (
          ""
        )}
      </Grid>
      <Grid item md={12} sm={12} sx={{ textAlign: "right" }}>
        {" "}
        <FormControlLabel
          // sx={{ margin: "3px 0px" }}
          control={
            <Checkbox
              disabled={!isActiveChecked}
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
          label={"Allow to display in Sale Channel"}
        />
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
      </Grid>
    </ModalComponent>
  );
}
export default UpdatePlatFormModal;
