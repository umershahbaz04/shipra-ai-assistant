import React from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import TextField from "@mui/material/TextField";
import "../../assets/styles/datePickerCustomStyles.css";
import "./CustomReactDatePickerInput.css";

const CustomReactDatePickerInputFilter = React.forwardRef((props, ref) => {
  const { ...rest } = props;
  return (
    <DatePicker
      placeholderText="Select date"
      customInput={
        <TextField
          dateFormat="dd/MM/yyyy"
          fullWidth
          inputProps={
            props?.inputProps || {
              style: { padding: "5px", fontSize: "13px" },
            }
          }
          size={props?.size || "small"}
          label={props?.label}
          variant={props?.variant ? variant : "outlined"}
          value={props?.value}
          onClick={props?.onClick}
          ref={ref}
        />
      }
      selected={props?.value}
      onChange={props?.onClick}
      {...rest} // Spread all other props to DatePicker
    />
  );
});

export default CustomReactDatePickerInputFilter;
