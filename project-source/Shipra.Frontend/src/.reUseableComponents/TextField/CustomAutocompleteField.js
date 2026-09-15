import { Autocomplete, TextField } from "@mui/material";
import React from "react";
import { LoadingTextField } from "../../utilities/helpers/Helpers";

export default function CustomAutocompleteField(props) {
  const {
    options = [],
    value,
    getOptionLabel = () => {},
    onChange = () => {},
    disablePortal,
    multiple = false,
    loading,
  } = props;
  return (
    <>
      {loading ? (
        <LoadingTextField height={35}/>
      ) : (
        <Autocomplete
          value={value}
          fullWidth
          multiple={multiple}
          onChange={onChange}
          sx={{
            "& .MuiInputBase-root": {
              padding: "1px !important",
              width: "100%",
            },
            "& .MuiAutocomplete-input": {
              fontSize: "12px",
              cursor: "pointer",
            },
          }}
          getOptionLabel={getOptionLabel}
          ChipProps={{
            sx: {
              height: 20,
            },
          }}
          ListboxProps={{
            sx: {
              fontSize: "12px !important",
              "& .MuiAutocomplete-option": {
                fontSize: "12px !important",
              },
            },
            style: { maxHeight: 150 },
          }}
          disablePortal={disablePortal}
          options={options}
          renderInput={(params) => {
            return (
              <TextField
                {...params}
                size={props?.size || "small"}
                placeholder="Select Please"
                error={props?.error}
                label={props?.label}
                helperText={props?.error && props?.error?.message}
                onKeyDown={(event) => {
                  if (event.key === " ") {
                    event.stopPropagation();
                  }
                }}
              />
            );
          }}
          {...props}
        />
      )}
    </>
  );
}
