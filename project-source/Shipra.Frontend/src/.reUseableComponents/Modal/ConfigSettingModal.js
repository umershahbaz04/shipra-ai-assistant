import { Box, InputLabel, TextField } from "@mui/material";
import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import {
  GetDynamicApiCall,
  UpdateActiveCarrierClientSettingConfig,
} from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import { EnumOptions } from "../../utilities/enum";
import {
  CustomColorLabelledOutline,
  GridContainer,
  GridItem,
  fetchMethod,
  getLowerCase,
  getTrimValue,
} from "../../utilities/helpers/Helpers";
import { errorNotification, successNotification } from "../../utilities/toast";
import ModalButtonComponent from "../Buttons/ModalButtonComponent";
import SelectComponent from "../TextField/SelectComponent";
import ModalComponent from "./ModalComponent";

export const inputTypesEnum = Object.freeze({
  SELECT: "select",
  TEXT: "text",
  NUMBER: "number",
  DATE: "date",
  CHECKBOX: "checkbox",
});

const ConfigSettingModal = (props) => {
  const { open, onClose, data } = props;
  const [configSettings, setConfigSettings] = useState([]);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [dispathExConfig, setdispathExConfig] = useState({});
  const ignoredKeys = ["DomainProdURL", "DomainTestURL"];

  const handleInputChange = (section_index, input_index, value) => {
    setConfigSettings((prev) => {
      const _configSettings = [...prev];
      _configSettings[section_index].inputData[input_index].value = value;
      return _configSettings;
    });
  };
  const handleUpdateConfigSettings = async () => {
    const _submitData = configSettings.map((section) => {
      const _section = { ...section };
      const _section_inputData = section.inputData.map((input_dt) => {
        const _input_data = { ...input_dt };
        if (!getTrimValue(input_dt.value) || input_dt.value.value === 0) {
          errorNotification(`The ${input_dt.label}: Is required to proceed`);
          return false;
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

    const { response } = await fetchMethod(
      () =>
        UpdateActiveCarrierClientSettingConfig(
          data.selectedActiveCarrierId,
          data?.data?.carrier?.carrierId,
          _submitData,
          dispathExConfig
        ),
      setSubmitLoading,
      false
    );
    if (response.isSuccess) {
      successNotification("Config Settings updated successfully");
      onClose();
    } else {
      errorNotification("Something went wrong while updating Config settings");
    }
  };

  useEffect(() => {
    const fetchData = async () => {
      if (!data?.data?.settingConfig) return;
      const parsedData = data?.data?.settingConfig;
      const _configSettings = await Promise.all(
        parsedData.map(async (section) => {
          const _section = { ...section };
          const updatedInputs = [];
          for (const input_dt of section.inputData) {
            const _input_data = { ...input_dt };
            const isSelectWithSrc =
              getLowerCase(_input_data?.type) === inputTypesEnum.SELECT &&
              _input_data?.src !== null;
            if (getLowerCase(_input_data.type) === inputTypesEnum.CHECKBOX) {
              updatedInputs.push(_input_data);
              continue;
            }
            if (getLowerCase(_input_data?.type) === inputTypesEnum.TEXT) {
              _input_data.value = _input_data.value || "";
              updatedInputs.push(_input_data);
              continue;
            }
            if (isSelectWithSrc) {
              try {
                const response = await GetDynamicApiCall(_input_data?.src?.url);
                if (response) {
                  _input_data.data = response.data?.result;
                  const selectedIds = _input_data.value
                    ?.toString()
                    .split(",")
                    .map((v) => v.trim());
                  const idKey = _input_data?.src?.obj?.id;
                  const selectedObjects = _input_data.data.filter((obj) =>
                    selectedIds.includes(obj[idKey]?.toString())
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
    if (!data) return;
    const config = JSON.parse(data?.data?.config);
    setdispathExConfig(config);
  }, [data]);

  console.log(data);

  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="md"
        title={"Config Settings"}
        actionBtn={
          <ModalButtonComponent
            loading={submitLoading}
            title={"Save"}
            bg={purple}
            onClick={handleUpdateConfigSettings}
          />
        }
      >
        <GridContainer>
          {configSettings.map((section, section_index) => (
            <GridItem lg={12} md={12} sm={12} key={section.key}>
              {section?.isHide !== true && (
                <CustomColorLabelledOutline label={section.sectionName}>
                  <GridContainer>
                    {section.inputData.map((input, input_index) => (
                      <GridItem
                        lg={6}
                        md={6}
                        sm={6}
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
                                val
                              );
                            }}
                          />
                        )}
                      </GridItem>
                    ))}
                  </GridContainer>
                </CustomColorLabelledOutline>
              )}
            </GridItem>
          ))}
          <GridItem lg={12} md={12} sm={12}>
            {data?.data?.isDispatchExCompany === true && (
            <CustomColorLabelledOutline label="Config">
              <GridContainer>
                {Object.entries(dispathExConfig)
                  .filter(([key]) => !ignoredKeys.includes(key))
                  .map(([key, value]) => (
                    <GridItem item xs={12} sm={6} md={6} key={key}>
                      <InputLabel required={true} sx={styleSheet.inputLabel}>
                        {key}
                      </InputLabel>
                      <TextField
                        fullWidth
                        value={value}
                        onChange={(e) =>
                          setdispathExConfig({
                            ...dispathExConfig,
                            [key]: e.target.value,
                          })
                        }
                        sx={{ "& .MuiInputBase-root": { height: "37px" } }}
                      />
                    </GridItem>
                  ))}
              </GridContainer>
            </CustomColorLabelledOutline>
            )}
          </GridItem>
        </GridContainer>
      </ModalComponent>
    </>
  );
};

export default ConfigSettingModal;
