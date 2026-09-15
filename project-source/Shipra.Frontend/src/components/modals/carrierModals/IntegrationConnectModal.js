import { Box, Button, Grid, InputLabel, TextField } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  activateCarrier,
  CheckDuplicationCarrierAlias,
  GetAllCarrierLocationsByCarrier,
  GetDynamicApiCall,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
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
import UtilityClass from "../../../utilities/UtilityClass";

function IntegrationConnectModal(props) {
  const [isLoading, setIsLoading] = useState(false);
  const [fieldsDisabled, setFieldsDisabled] = useState(false);
  const [configSettings, setConfigSettings] = useState([]);
  const [activateData, setActivateData] = useState();
  const [carrierLocationData, setCarrierLocationData] = useState();
  const [selectedCarrierLocationData, setSelectedCarrierLocationData] =
    useState();
  let {
    open,
    setOpen,
    carrierId,
    setIsRefreshRequired,
    getAllCarriersData,
    setActiveUrl,
    carrierData,
  } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [inputFields, setInputFields] = useState(
    props.carrierData?.inputRequiredConfig || {}
  );

  const getAllCarrierLocationsByCarrier = async () => {
    try {
      const response = await GetAllCarrierLocationsByCarrier(carrierId);
      if (response.data.isSuccess) {
        setCarrierLocationData(response.data.result);
      }
    } catch (e) {}
  };

  const handleClose = () => {
    setOpen(false);
  };

  const handleInputChange = (section_index, input_index, value) => {
    setConfigSettings((prev) => {
      const _configSettings = [...prev];
      _configSettings[section_index].inputData[input_index].value = value;
      return _configSettings;
    });
  };

  const handleConnect = async () => {
    if (
      !selectedCarrierLocationData ||
      selectedCarrierLocationData?.carrierLocationId === 0
    ) {
      errorNotification("Please Select Carrier Location");
      return;
    }
    try {
      setIsLoading(true);
      setFieldsDisabled(true);
      let isError = false;

      for (var key in activateData) {
        if (activateData[key] === "") {
          errorNotification(`The ${key}: Is required to proceed`);
          isError = true;
        }
      }
      const _submitData = configSettings.map((section) => {
        const _section = { ...section };
        const _section_inputData = section.inputData.map((input_dt) => {
          const _input_data = { ...input_dt };
          if (
            (input_dt.required !== false && !getTrimValue(input_dt.value)) ||
            (input_dt.value === 0 && input_dt.required !== false)
          ) {
            errorNotification(`The ${input_dt.label}: Is required to proceed`);
            isError = true;
          }
          if (getLowerCase(_input_data.type) === inputTypesEnum.SELECT) {
            const manipulated_data = _input_data.data.map(
              (option) => option.value
            );
            _input_data.data = _input_data?.src ? [] : manipulated_data;
            if (Array.isArray(input_dt.value)) {
              const dynamicKey = Object.keys(input_dt.value[0] || {})[0];
              _input_data.value = input_dt.value
                .map((item) => item[dynamicKey])
                .join(",");
            } else {
              const dynamicKey = Object.keys(input_dt.value || {})[0];
              _input_data.value = input_dt.value?.[dynamicKey];
            }
          }
          return _input_data;
        });
        _section.inputData = _section_inputData;
        return _section;
      });
      console.log(_submitData, activateData);
      if (isError) {
        return false;
      }
      let body = {
        inputParameters: activateData,
        settingConfig: _submitData,
        CarrierId: carrierId,
        CarrierAlias: carrierAllias,
        CarrierLocationId: selectedCarrierLocationData?.carrierLocationId,
      };
      // console.log("body", body);
      const response = await activateCarrier(body);
      if (!response.data.isSuccess) {
        let jsonData = response.data.errors;
        for (const key in jsonData) {
          if (jsonData.hasOwnProperty(key)) {
            const messagesArray = jsonData[key];
            for (const message of messagesArray) {
              errorNotification(message);
            }
          }
        }
      } else {
        successNotification(
          LanguageReducer?.languageType?.SUCCESSFULLY_CONNECT_TOAST
        );
        handleClose();
        setActiveUrl("/carriers/active");
      }
    } catch (error) {
      console.error("Error to connect", error.response);
      errorNotification(LanguageReducer?.languageType?.UNABLE_TO_CONNECT_TOAST);
    } finally {
      setIsLoading(false);
      setFieldsDisabled(false);
    }
  };
  const [carrierAllias, setCarrierAllias] = useState("");

  useEffect(() => {
    if (props.carrierData) {
      if (props.carrierData?.inputRequiredConfig) {
        const parsedData = JSON.parse(props.carrierData?.inputRequiredConfig);
        if (parsedData !== null && parsedData !== "undefined") {
          const updatedData = Object.entries(parsedData).reduce(
            (acc, [key, value]) => {
              if (key !== "CarrierId") {
                acc[key] = "";
              } else {
                acc[key] = carrierId;
              }
              return acc;
            },
            {}
          );
          setActivateData(updatedData);
          const { CarrierId, ...fields } = parsedData;
          setInputFields(fields);
        }
      }

      setCarrierAllias(props?.carrierData?.nextAliasName || 0);
    }
  }, [props.carrierData]);

  const handleFocus = (event) => event.target.select();

  useEffect(() => {
    const fetchData = async () => {
      if (!carrierData?.settingConfig) return;
      const parsedData = JSON.parse(carrierData?.settingConfig);
      const _configSettings = await Promise.all(
        parsedData.map(async (section) => {
          const _section = { ...section };
          const updatedInputs = [];
          for (const input_dt of section.inputData) {
            const _input_data = { ...input_dt };
            const isSelectWithSrc =
              getLowerCase(_input_data.type) === inputTypesEnum.SELECT &&
              _input_data?.src !== null;
            if (getLowerCase(_input_data.type) === inputTypesEnum.CHECKBOX) {
              updatedInputs.push(_input_data);
              continue;
            }
            if (getLowerCase(_input_data.type) === inputTypesEnum.TEXT) {
              _input_data.value = _input_data.value || "";
              updatedInputs.push(_input_data);
              continue;
            }
            if (isSelectWithSrc) {
              try {
                const response = await GetDynamicApiCall(_input_data?.src?.url);
                if (response) {
                  _input_data.data = response.data?.result;
                  const selectedIds = _input_data.value.split(",").map(Number);
                  const selectedObjects = _input_data.data.filter((obj) =>
                    selectedIds.includes(obj.carrierTrackingStatusId)
                  );
                  _input_data.value = selectedObjects;
                }
              } catch (e) {
                console.error(e);
              }
            } else {
              const manipulated_data = _input_data?.data?.map((option) => ({
                label: option,
                value: option,
              }));
              _input_data.data = manipulated_data;
              _input_data.value = {
                label: input_dt.value,
                value: input_dt.value,
              };
            }
            updatedInputs.push(_input_data);
          }
          return { ..._section, inputData: updatedInputs };
        })
      );
      setConfigSettings(_configSettings);
    };
    fetchData();
  }, [open]);

  useEffect(() => {
    if (carrierLocationData) {
      setSelectedCarrierLocationData(carrierLocationData[0]);
    }
  }, [carrierLocationData]);

  useEffect(() => {
    getAllCarrierLocationsByCarrier();
  }, []);

  console.log(configSettings);

  return (
    <>
      <ModalComponent
        open={open}
        onClose={handleClose}
        maxWidth="md"
        title={""}
        actionBtn={
          <ModalButtonComponent
            title={"Activate"}
            loading={isLoading}
            bg={purple}
            onClick={handleConnect}
          />
        }
      >
        <Box sx={styleSheet.ModalLogoArea}>
          <Button sx={{ mt: "8px", outlineColor: "white", mb: "7px" }}>
            <img src={props?.carrierImage} alt="uploadIcon" />
          </Button>
        </Box>
        <CustomColorLabelledOutline label={"Carrier Settings"}>
          <GridContainer>
            <GridItem lg={6} md={6} sm={12} xs={12} paddingBottom={2}>
              <Box>
                <InputLabel sx={styleSheet.inputLabel}>Nick Name</InputLabel>
                <TextField
                  onFocus={handleFocus}
                  placeholder={"Nick Name"}
                  disabled={fieldsDisabled}
                  size="small"
                  fullWidth
                  variant="outlined"
                  value={carrierAllias}
                  onChange={(e) => setCarrierAllias(e.target.value)}
                />
              </Box>
            </GridItem>
            <GridItem lg={6} md={6} sm={12} xs={12} paddingBottom={2}>
              <InputLabel required sx={styleSheet.inputLabel}>
                Carrier Location
              </InputLabel>
              <SelectComponent
                name="carrierLocationData"
                options={carrierLocationData}
                value={selectedCarrierLocationData}
                optionLabel={EnumOptions.PICKUP_CARRIER_LOCATION.LABEL}
                optionValue={EnumOptions.PICKUP_CARRIER_LOCATION.VALUE}
                onChange={(e, val) => {
                  setSelectedCarrierLocationData(val);
                }}
              />
            </GridItem>
          </GridContainer>
          <GridContainer>
            <GridItem lg={12} md={12} sm={12} xs={12}>
              <Grid container spacing={2}>
                {inputFields &&
                  Object.keys(inputFields).length > 0 &&
                  Object.entries(inputFields).map(
                    ([key, value]) =>
                      key !== "CarrierId" && (
                        <Grid
                          item
                          sm={6}
                          md={6}
                          lg={6}
                          xs={12}
                          key={key}
                          py={"0px!important"}
                          paddingTop={"8px!important"}
                        >
                          <InputLabel required sx={styleSheet.inputLabel}>
                            {key}
                          </InputLabel>
                          <TextField
                            onFocus={handleFocus}
                            placeholder={value}
                            disabled={fieldsDisabled}
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
              </Grid>
            </GridItem>
          </GridContainer>
        </CustomColorLabelledOutline>
        <GridContainer>
          {configSettings.length > 0 &&
            configSettings?.map((section, section_index) => (
              <GridItem lg={12} md={12} sm={12} xs={12} key={section.key}>
                {section?.isHide !== true && (
                  <CustomColorLabelledOutline label={section.sectionName}>
                    <Grid container spacing={2} sx={{ paddingTop: 2 }}>
                      {section.inputData.map((input, input_index) => (
                        <Grid
                          item
                          xs={12}
                          sm={6}
                          key={input.key}
                          paddingTop={"0px!important"}
                        >
                          <Box marginBottom={1}>
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
                                disabled={fieldsDisabled}
                                options={input.data}
                                optionLabel={
                                  input.src
                                    ? input.src.obj.label
                                    : EnumOptions
                                        .CONFIG_SETTING_INPUTDATA_OPTIONS.LABEL
                                }
                                optionValue={
                                  input.src
                                    ? input.src.obj.id
                                    : EnumOptions
                                        .CONFIG_SETTING_INPUTDATA_OPTIONS.VALUE
                                }
                                value={input.value}
                                multiple={input.multiple}
                                onChange={(e, val) => {
                                  handleInputChange(
                                    section_index,
                                    input_index,
                                    val
                                  );
                                }}
                              />
                            )}
                            {(getLowerCase(input.type) ===
                              inputTypesEnum.TEXT ||
                              getLowerCase(input.type) ===
                                inputTypesEnum.NUMBER) && (
                              <TextField
                                type={getLowerCase(input.type)}
                                placeholder={input.value}
                                size="small"
                                fullWidth
                                variant="outlined"
                                disabled={fieldsDisabled}
                                required={input.required}
                                value={input.value}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  handleInputChange(
                                    section_index,
                                    input_index,
                                    val
                                  );
                                }}
                              />
                            )}
                          </Box>
                        </Grid>
                      ))}
                    </Grid>
                  </CustomColorLabelledOutline>
                )}
              </GridItem>
            ))}
        </GridContainer>
      </ModalComponent>
    </>
  );
}
export default IntegrationConnectModal;
