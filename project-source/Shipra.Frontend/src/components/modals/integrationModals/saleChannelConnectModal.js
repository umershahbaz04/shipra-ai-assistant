import {
  Checkbox,
  FormControlLabel,
  FormGroup,
  Grid,
  InputLabel,
  Radio,
  TextField,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateSaleChannelConfig,
  GetStoresForSelection,
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
  s_span,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
function SaleChannelConnectModal(props) {
  const handleFocus = (event) => event.target.select();
  const [isLoading, setIsLoading] = useState(false);
  const [activateData, setActivateData] = useState();
  let {
    open,
    setOpen,
    getAll,
    selectedSaleChannel,
    settingConfig,
    inputConfig,
  } = props;
  const [allStores, setAllStores] = useState([]);
  const [inputFields, setInputFields] = useState([]);
  const [store, setStore] = useState({
    storeId: 0,
    storeName: "Select Please",
  });
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

  const handleInputChange = (sectionIndex, inputIndex, value) => {
    setInputFields((prev) => {
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
  const handleConnect = async () => {
    setIsLoading(true);
    const _submitData = inputFields.map((section) => {
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
      const updatedInputConfigData = handleConfig(inputConfig, _submitData);
      let body = {
        inputParameters: updatedInputConfigData,
        storeId: store?.storeId,
        saleChannelName: saleChannelName,
        IsAllowToDisplayInSaleChannel: isAllowSaleChannel,
        IsActive: true,
        saleChannelLookupId: selectedSaleChannel,
        settingConfig: _submitData,
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
    getAllStores();
  }, []);
  useEffect(() => {
    if (settingConfig) {
      const parsedData = JSON.parse(settingConfig);
      const _configSettings = parsedData.map((section) => {
        const _section = { ...section };
        _section.inputData = section.inputData.map((input) => {
          const _input = { ...input };
          if (
            getLowerCase(_input.type) === inputTypesEnum.SELECT &&
            _input.data
          ) {
            _input.data = _input.data.map((option) => ({
              label: option,
              value: option,
            }));
            _input.value = { label: _input.value, value: _input.value };
          }
          return _input;
        });
        return _section;
      });
      setInputFields(_configSettings);
    }
  }, [open, settingConfig]);
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={"Add Sale Channel"}
      actionBtn={
        <ModalButtonComponent
          title={"Add Sale Channel"}
          loading={isLoading}
          bg={purple}
          onClick={(e) => handleConnect()}
        />
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel} required>
            Sale Channel Name
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
        <Grid item md={12} sm={12} xs={12}>
          {" "}
          <InputLabel sx={styleSheet.inputLabel} required>
            Store({s_span("s")})
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
        {inputFields ? (
          <GridContainer>
            {inputFields.length > 0 &&
              inputFields.map((section, sectionIndex) => (
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
      </Grid>
    </ModalComponent>
  );
}
export default SaleChannelConnectModal;
