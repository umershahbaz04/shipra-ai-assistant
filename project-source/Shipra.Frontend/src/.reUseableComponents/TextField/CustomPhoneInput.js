import React from "react";
import {
  FormControl,
  InputLabel,
  Input,
  makeStyles,
  TextField,
} from "@mui/material";
import PhoneInput from "react-phone-input-2";
import "react-phone-input-2/lib/material.css";

import "../../assets/styles/phoneInput.css";
import { Controller } from "react-hook-form";

const styles = (theme) => ({
  field: {
    height: "10px",
    width: "100%",
    borderRadius: "4px",
    fontSize: "14px !important",
  },
});

function CustomPhoneInput(props) {
  const { value, defaultCountry, onChange, classes, name, required } = props;

  return (
    <>
      <PhoneInput
        prefix="+"
        inputStyle={styles.field}
        containerClass={styles.field}
        inputProps={{
          style: {},
          name: name,
          required: required,
        }}
        style={{ fontSize: "14px !important" }}
        specialLabel=""
        country={"ae"}
        value={value}
        onChange={(phone) => onChange({ phone })}
        defaultErrorMessage=""
      />
    </>
  );
}

export default CustomPhoneInput;
