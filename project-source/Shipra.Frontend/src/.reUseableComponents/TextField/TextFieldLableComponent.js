import { InputLabel } from "@mui/material";
import React from "react";

export default function TextFieldLableComponent(props) {
  const { title, required = true } = props;
  const styles = {
    inputLabel: {
      fontSize: "12px  !important",
      fontWeight: "400  !important",
      color: "#000000  !important",
      fontFamily: "'Lato', 'Inter', 'Arial' !important",
      textTransform: "capitalize  !important",
      flexShrink: 0,
    },
  };
  return (
    <InputLabel
      required={required}
      sx={{
        ...styles.inputLabel,
        mb: !required && 0.5,
        position: "relative",
        top: !required && 4,
        "& .MuiFormLabel-asterisk": {
          color: "red !important",
          fontSize: "1.5rem !important",
          position: "relative !important",
          top: "8px !important",
          right: "3px !important",
        },
      }}
    >
      {title}
    </InputLabel>
  );
}
