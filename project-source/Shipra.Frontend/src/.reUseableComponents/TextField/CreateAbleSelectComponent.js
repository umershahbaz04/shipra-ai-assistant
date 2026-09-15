import React, { useEffect, useState } from "react";
import CreatableSelect from "react-select/creatable";
import { Box, FormHelperText } from "@mui/material";
import AutorenewRoundedIcon from "@mui/icons-material/AutorenewRounded";
import {
  TextFieldErrorBorderColor,
  grey,
  purple,
  useLanguageReducer,
} from "../../utilities/helpers/Helpers";

const getStyles = ({ height, borderRadius, hasError }) => ({
  control: (base) => ({
    ...base,
    boxShadow: "none",
    borderColor: hasError ? TextFieldErrorBorderColor : "hsl(0deg 0% 74.12%)",
    ":hover": {
      border: `1px solid ${hasError ? TextFieldErrorBorderColor : "black"}`,
    },
    "&:focus-within": {
      border: `1px solid ${hasError ? TextFieldErrorBorderColor : purple}`,
      boxShadow: `0 0 0 1px ${hasError ? TextFieldErrorBorderColor : purple}`,
    },
    minHeight: height,
    borderRadius,
  }),
  dropdownIndicator: (base) => ({ ...base, padding: 3.5 }),
  clearIndicator: (base) => ({ ...base, padding: 3.5 }),
  valueContainer: (base) => ({
    ...base,
    padding: "0px 6px",
    maxHeight: 83,
    overflowY: "auto",
    fontSize: "12px",
  }),
  input: (base) => ({
    ...base,
    margin: 0,
    padding: 0,
    fontSize: "12px",
  }),
  menu: (base) => ({
    ...base,
    zIndex: 9999,
    fontSize: "12px",
  }),
  multiValueLabel: (styles) => ({
    ...styles,
    padding: 1,
  }),
  option: (base, { isSelected }) => ({
    ...base,
    color: isSelected ? purple : "black",
    backgroundColor: isSelected ? "rgba(86, 58, 213, 0.08)" : "transparent",
    padding: 4,
    fontSize: 12,
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    "&:hover": {
      backgroundColor: !isSelected && grey,
      color: !isSelected && "black",
      cursor: "pointer",
    },
  }),
});

export default function CreateAbleSelectComponent(props) {
  const [input, setInput] = useState("");
  const [createOptionLoading, setCreateOptionLoading] = useState(false);
  const languageType = useLanguageReducer();

  const {
    options = [],
    value,
    onChange,
    optionLabel = "",
    optionValue = "",
    disabled = false,
    height = 37,
    borderRadius = "4px",
    required = false,
    name = "",
    errors = [],
    isRHF = false,
    isInput = false,
    isLoading = false,
    handleRefreshClick = () => {},
    onCreateOption = () => {},
    isRefesh = false,
    placeholder = "Select Please",
    addPleaseSelectOptionOnClear = true,
    isMulti = false,
  } = props;

  const hasError =
    required &&
    (isRHF ? Boolean(errors[name]) : errors.includes(name)) &&
    (value ? !value[optionValue] : isInput ? !input.trim() : true);

  const styles = getStyles({ height, borderRadius, hasError });

  const handleInputChange = (e, meta) => {
    if (meta.action === "input-change") {
      setInput(e);
    }
  };

  const handleCreateOption = async (inputValue) => {
    setCreateOptionLoading(true);
    await onCreateOption(inputValue);
    setCreateOptionLoading(false);
    setInput("");
  };

  const handleChange = (val, triggeredAction) => {
    if (
      triggeredAction.action === "clear" ||
      triggeredAction.action === "select-option"
    ) {
      setInput("");
    }
    if (val) {
      onChange(name, val, triggeredAction);
    } else {
      if (addPleaseSelectOptionOnClear) {
        const zero_value_obj = {
          [optionLabel]: "Please select",
          [optionValue]: 0,
        };
        onChange(
          name,
          isRHF && required ? null : zero_value_obj,
          triggeredAction,
        );
      } else {
        onChange(name, val);
      }
    }
  };

  useEffect(() => {
    if (!value) {
      setInput("");
    }
  }, [value]);

  return (
    <>
      <Box sx={{ position: "relative" }}>
        {isRefesh && options.length === 0 && (
          <AutorenewRoundedIcon
            sx={{
              color: "blue",
              cursor: "pointer",
              position: "absolute",
              top: -25,
              right: 0,
            }}
            onClick={handleRefreshClick}
          />
        )}
        <CreatableSelect
          menuPosition="fixed"
          menuPlacement="auto"
          isDisabled={disabled}
          isClearable
          isLoading={isLoading || createOptionLoading}
          isMulti={isMulti}
          inputValue={input}
          onInputChange={handleInputChange}
          onCreateOption={handleCreateOption}
          getOptionLabel={(option) =>
            option.__isNew__ ? option.label : option[optionLabel]
          }
          getOptionValue={(option) => option[optionValue]}
          isOptionDisabled={(option) => option[optionValue] === 0}
          options={options}
          value={value}
          required={required}
          name={name}
          onChange={handleChange}
          placeholder={placeholder}
          styles={styles}
          maxMenuHeight={160}
        />
      </Box>
      {hasError && (
        <FormHelperText error sx={{ marginLeft: "14px" }}>
          {languageType?.FIELD_REQUIRED_TEXT}
        </FormHelperText>
      )}
    </>
  );
}
