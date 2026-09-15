import React, { useEffect, useState } from "react";
import AsyncSelect from "react-select/async";
import {
  TextFieldErrorBorderColor,
  grey,
  purple,
  useLanguageReducer,
} from "../../utilities/helpers/Helpers";
import { FormHelperText } from "@mui/material";

const getStyles = ({ height, borderRadius, hasError }) => {
  return {
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
      borderRadius: borderRadius,
    }),
    dropdownIndicator: (base) => ({
      ...base,
      padding: 3.5,
    }),
    clearIndicator: (base) => ({
      ...base,
      padding: 3.5,
    }),
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
  };
};

export default function AsyncSelectComponent(props) {
  const [input, setInput] = useState("");
  const {
    value,
    onChange,
    multiple = false,
    optionLabel = "label",
    optionValue = "value",
    disabled = false,
    height = 37,
    borderRadius = "4px",
    required = false,
    name = "",
    errors = [],
    isRHF = false,
    isInput = false,
    isLoading = false,
    placeholder = "Select Please",
    loadOptions,
    length,
    searchParam = "",
  } = props;

  const languageType = useLanguageReducer();

  const hasError =
    required &&
    (isRHF ? Boolean(errors[name]) : errors.includes(name)) &&
    (value ? !value[optionValue] : isInput ? !input.trim() : true);

  const styles = getStyles({ height, borderRadius, hasError });

  const handleInputChange = (inputValue) => {
    setInput(inputValue);
  };

  useEffect(() => {
    if (!value) {
      setInput("");
    }
  }, [value]);

  return (
    <>
      <AsyncSelect
        cacheOptions
        defaultOptions
        loadOptions={(inputValue, callback) =>
          loadOptions(inputValue, length, callback)
        }
        menuPosition="fixed"
        menuPlacement="auto"
        isDisabled={disabled}
        isClearable
        isLoading={isLoading}
        getOptionLabel={(option) => option[optionLabel]}
        getOptionValue={(option) => option[optionValue]}
        isMulti={multiple}
        value={value}
        onChange={(val, triggeredAction) => {
          setInput("");
          if (val) {
            onChange(name, val, triggeredAction);
          } else {
            const zero_value_obj = {
              [optionLabel]: "Please select",
              [optionValue]: 0,
            };
            onChange(
              name,
              isRHF && required ? null : zero_value_obj,
              triggeredAction
            );
          }
        }}
        inputValue={input}
        onInputChange={handleInputChange}
        placeholder={placeholder}
        styles={styles}
        maxMenuHeight={160}
      />
      {hasError && (
        <FormHelperText error sx={{ marginLeft: "14px" }}>
          {languageType?.FIELD_REQUIRED_TEXT}
        </FormHelperText>
      )}
    </>
  );
}
