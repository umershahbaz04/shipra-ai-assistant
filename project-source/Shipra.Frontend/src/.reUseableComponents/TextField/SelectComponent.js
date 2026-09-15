import React, { useEffect, useState } from "react";
import Select from "react-select";
import {
  TextFieldErrorBorderColor,
  grey,
  purple,
  useLanguageReducer,
} from "../../utilities/helpers/Helpers";
import { Box, FormHelperText } from "@mui/material";
import AutorenewRoundedIcon from "@mui/icons-material/AutorenewRounded";

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
export default function SelectComponent(props) {
  const [input, setInput] = useState("");
  const {
    options = [],
    value,
    onChange,
    multiple = false,
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
    isRefesh = false,
    placeholder = "Select Please",
    addPleaseSelectOptionOnClear = true,
  } = props;

  // languageType
  const languageType = useLanguageReducer();

  // hasError
  const hasError =
    required &&
    (isRHF ? Boolean(errors[name]) : errors.includes(name)) &&
    (value ? !value[optionValue] : isInput ? !input.trim() : true);

  // styles
  const styles = getStyles({ height, borderRadius, hasError });

  // handleInputChange
  const handleInputChange = (e, meta) => {
    if (meta.action === "input-change") {
      setInput(e);
      const zero_value_obj = {
        [optionLabel]: e,
      };
      onChange(name, zero_value_obj);
    }
  };

  // inputProps
  const inputProps = isInput
    ? { inputValue: input, onInputChange: handleInputChange }
    : {};

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
        <Select
          menuPosition="fixed"
          menuPlacement="auto"
          isDisabled={disabled}
          isClearable={true}
          isLoading={isLoading}
          getOptionLabel={(option) => option[optionLabel]}
          getOptionValue={(option) => option[optionValue]}
          isOptionDisabled={(option) => option[optionValue] === 0}
          options={options}
          isMulti={multiple}
          value={value}
          required={required}
          name={name}
          onChange={(val, triggeredAction) => {
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
                  triggeredAction
                );
              } else {
                onChange(name, val);
              }
            }
          }}
          {...inputProps}
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
