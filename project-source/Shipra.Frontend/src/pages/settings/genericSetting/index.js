import { Box } from "@material-ui/core";
import { Checkbox, Grid, InputLabel, TextField } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateUpdateClientGenericSetting,
  GetDynamicApiCall,
  GetGenericSetting,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumCookieKeys, EnumOptions, inputTypesEnum } from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  CicrlesLoading,
  CustomColorLabelledOutline,
  fetchMethod,
  getLowerCase,
  getTrimValue,
  GridContainer,
  GridItem,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { setThisKeyCookie, setGenericCheckboxCookies, removeThisKeyCookie, isShowCountryInTabbarFlag } from "../../../utilities/cookies";
import { updateSelectedCountry } from "../../../redux/country";

const GenericSettingPage = () => {
  const dispatch = useDispatch();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [pageLoading, setPageLoading] = useState(false);
  const [genericSettingData, setGenericSettingData] = useState([]);
  const [parseGenericData, setParseGenericData] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const handleInputChange = (section_index, input_index, value) => {
    setParseGenericData((prev) => {
      const _configSettings = [...prev];
      _configSettings[section_index].inputData[input_index].value = value;
      return _configSettings;
    });
  };

  const getGenericSetting = async () => {
    setPageLoading(true);
    try {
      const response = await GetGenericSetting();
      if (response) {
        const ParseData = JSON.parse(response?.data?.result);
        setGenericSettingData(ParseData);
      }
    } catch (e) {
    } finally {
      setPageLoading(false);
    }
  };

  const createUpdateClientGenericSetting = async () => {
    for (const section of parseGenericData) {
      for (const input_dt of section.inputData) {
        const val = input_dt.value;
        const isRequired = input_dt.required;
        if (isRequired) {
          if (val === null || val === undefined || (typeof val === "string" && !getTrimValue(val)) || (typeof val === "object" && val.value === 0)) {
            errorNotification(`The ${input_dt.label}: Is required to proceed`);
            return;
          }
        }
      }
    }
    const _submitData = parseGenericData.map((section) => {
      const _section = { ...section };
      const _section_inputData = section.inputData.map((input_dt) => {
        const _input_data = { ...input_dt };
        if (getLowerCase(_input_data.type) === inputTypesEnum.SELECT) {
          const manipulated_data = _input_data.data.map(
            (option) => option.value,
          );
          _input_data.data = _input_data?.src ? [] : manipulated_data;
          if (input_dt.value && typeof input_dt.value === "object") {
            if (Array.isArray(input_dt.value)) {
              if (input_dt.value.length > 0) {
                const dynamicKey = Object.keys(input_dt.value[0] || {})[0];
                _input_data.value = input_dt.value
                  .map((item) => item[dynamicKey])
                  .join(",");
              } else {
                _input_data.value = "";
              }
            } else {
              const dynamicKey = Object.keys(input_dt.value || {})[0];
              _input_data.value = input_dt.value?.[dynamicKey];
            }
          } else {
            _input_data.value = input_dt.value;
          }
        }

        return _input_data;
      });
      _section.inputData = _section_inputData;
      return _section;
    });
    const previousFlagValue = isShowCountryInTabbarFlag();
    let showCountryInTabbarItemFound = false;
    let newFlagValue = previousFlagValue;
    for (const section of parseGenericData) {
      if (section?.inputData) {
        const item = section.inputData.find((dt) => dt?.key === "showCountryInTabbar");
        if (item) {
          showCountryInTabbarItemFound = true;
          newFlagValue = item.value === true || item.value === "true";
          break;
        }
      }
    }

    const { response } = await fetchMethod(
      () => CreateUpdateClientGenericSetting(_submitData),
      setIsLoading,
      false,
    );
    if (response.isSuccess) {
      setThisKeyCookie("genericSetting", null);
      setGenericCheckboxCookies(parseGenericData);

      if (showCountryInTabbarItemFound && previousFlagValue !== newFlagValue) {
        dispatch(updateSelectedCountry(null));
        removeThisKeyCookie(EnumCookieKeys.COUNTRY_ID);
      }

      successNotification("Config Settings updated successfully");
      getGenericSetting();
    } else {
      errorNotification("Something went wrong while updating Config settings");
    }
  };

  useEffect(() => {
    const fetchData = async () => {
      if (!genericSettingData) return;
      const _configSettings = await Promise.all(
        genericSettingData.map(async (section) => {
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
             if (isSelectWithSrc) {
              try {
                const response = await GetDynamicApiCall(_input_data?.src?.url);
                if (response && response.data?.result) {
                  _input_data.data = response.data?.result;
                  const selectedIds = _input_data.value ? _input_data.value.split(",").map(Number) : [];
                  const selectedObjects = _input_data.data.filter((obj) =>
                    selectedIds.includes(obj.carrierTrackingStatusId),
                  );
                  _input_data.value = selectedObjects;
                } else {
                  _input_data.data = [];
                  _input_data.value = [];
                }
              } catch (e) {
                console.error(e);
                _input_data.data = [];
                _input_data.value = [];
              }
            } else {
              if (getLowerCase(_input_data.type) === inputTypesEnum.SELECT) {
                const manipulated_data = Array.isArray(_input_data.data) 
                  ? _input_data.data.map((option) => ({
                      label: option,
                      value: option,
                    }))
                  : [];
                _input_data.data = manipulated_data;
                _input_data.value = {
                  label: input_dt.value,
                  value: input_dt.value,
                };
              } else {
                // Keep text, number, checkbox values plain as they are
                _input_data.value = input_dt.value;
              }
            }
            updatedInputs.push(_input_data);
          }
          return { ..._section, inputData: updatedInputs };
        }),
      );
      console.log(_configSettings);
      setGenericCheckboxCookies(_configSettings);
      setParseGenericData(_configSettings);
    };

    fetchData();
  }, [genericSettingData]);

  useEffect(() => {
    getGenericSetting();
  }, []);

  return (
    <>
      {pageLoading ? (
        <CicrlesLoading />
      ) : (
        <Box sx={{}}>
          <GridContainer spacing={2} padding="20px">
            {parseGenericData.map((section, section_index) => (
              <GridItem lg={8} md={8} sm={8} key={section.key}>
                <CustomColorLabelledOutline label={section.sectionName}>
                  <GridContainer>
                    {section.inputData.map((input, input_index) => (
                      <GridItem
                        lg={8}
                        md={8}
                        sm={8}
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
                        {getLowerCase(input.type) === inputTypesEnum.SELECT && (
                          <SelectComponent
                            height={40}
                            name={input.key}
                            options={input.data}
                            optionLabel={
                              input.src
                                ? input.src.obj.label
                                : EnumOptions.CONFIG_SETTING_INPUTDATA_OPTIONS
                                    .LABEL
                            }
                            optionValue={
                              input.src
                                ? input.src.obj.id
                                : EnumOptions.CONFIG_SETTING_INPUTDATA_OPTIONS
                                    .VALUE
                            }
                            value={input.value}
                            multiple={input.multiple}
                            onChange={(e, val) => {
                              handleInputChange(
                                section_index,
                                input_index,
                                val,
                              );
                            }}
                          />
                        )}
                        {(getLowerCase(input.type) === inputTypesEnum.TEXT ||
                          getLowerCase(input.type) ===
                            inputTypesEnum.NUMBER) && (
                          <TextField
                            type={getLowerCase(input.type)}
                            placeholder={input.value}
                            size="small"
                            fullWidth
                            variant="outlined"
                            required={input.required}
                            value={input.value}
                            onChange={(e) => {
                              const val = e.target.value;
                              handleInputChange(
                                section_index,
                                input_index,
                                val,
                              );
                            }}
                          />
                        )}
                        {getLowerCase(input.type) ===
                          inputTypesEnum.CHECKBOX && (
                          <label key={input.key}>
                            <Checkbox
                              checked={input.value === "true"}
                              onChange={(e) =>
                                handleInputChange(
                                  section_index,
                                  input_index,
                                  e.target.checked.toString(),
                                )
                              }
                            />
                            {input.label}{" "}
                          </label>
                        )}
                      </GridItem>
                    ))}
                  </GridContainer>
                </CustomColorLabelledOutline>
              </GridItem>
            ))}
            {genericSettingData.length > 0 && parseGenericData.length > 0 && (
              <Grid item xs={8}>
                <Box display="flex" justifyContent="end" marginTop="20px">
                  <ActionButtonCustom
                    label={
                      LanguageReducer?.languageType
                        ?.SETTINGS_DOCUMENT_SETTINGS_SAVE
                    }
                    background={Colors.primary}
                    loading={isLoading}
                    onClick={createUpdateClientGenericSetting}
                  />
                </Box>
              </Grid>
            )}
          </GridContainer>
        </Box>
      )}
    </>
  );
};

export default GenericSettingPage;
