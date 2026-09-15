import React from "react";
import { Autocomplete, Chip, TextField } from "@mui/material";
import { getTrimValue } from "../../utilities/helpers/Helpers";

export default function SearchInputAutoCompleteMultiple(props) {
  const {
    onChange = () => {},
    onInputChange = () => {},
    handleFocus = () => {},
    inputFields = [],
    MAX_TAGS = 150,
    placeholder,
    padding,
  } = props;

  // Store the current input value in a ref to manage it
  const [inputValue, setInputValue] = React.useState("");

  return (
    <Autocomplete
      sx={{ padding: padding || "5px 7px" }}
      multiple
      id="tags-filled"
      variant="outlined"
      size="small"
      onFocus={handleFocus}
      inputValue={inputValue}
      onInputChange={(e, val, reason) => {
        setInputValue(val);

        const trimmedValue = String(val.trim());
        const values = trimmedValue.split(" ").filter(Boolean);
        if (values.length > 1) {
          onChange(e, [...inputFields, ...values]);
          setInputValue("");
        } else {
          onInputChange(e, trimmedValue);
        }
      }}
      onChange={(e, val) => {
        const trimmedValue = val.map((dt) => getTrimValue(dt));
        onChange(e, trimmedValue);
      }}
      value={inputFields}
      options={[]}
      freeSolo
      renderTags={(value, getTagProps) =>
        value
          .slice(0, MAX_TAGS)
          .map((option, index) => (
            <Chip
              key={index}
              variant="outlined"
              size="small"
              label={option}
              {...getTagProps({ index })}
            />
          ))
      }
      renderInput={(params) => (
        <TextField
          {...params}
          sx={{
            "& fieldset": { border: "none" },
            borderRadius: "5px",
            background: "#fff",
            boxShadow: 1,
          }}
          variant="outlined"
          size="small"
          placeholder={placeholder || "Search"}
          inputProps={{
            ...params.inputProps,
            value: params.inputProps.value,
          }}
        />
      )}
    />
  );
}
