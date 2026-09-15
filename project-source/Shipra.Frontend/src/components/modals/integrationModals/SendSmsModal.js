import { useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import {
    GridContainer,
    GridItem,
    purple,
} from "../../../utilities/helpers/Helpers";

const SmsModal = (props) => {
  const { open, onClose } = props;
  const initialSmsFieldsData = {
    smsText: "",
    SmsNumber: "",
    loading: false,
  };

  const [smsData, setSmsData] = useState(initialSmsFieldsData);
  console.log(smsData);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setSmsData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Send Sms Modal"}
      actionBtn={
        <ModalButtonComponent
          title={"Send Sms"}
          bg={purple}
          //   onClick={(e) => handleConnect()}
        />
      }
    >
      <GridContainer spacing={1}>
        <GridItem sm={12} md={12} lg={12}>
          <TextFieldLableComponent title={"Enter Sms"} />
          <TextFieldComponent
            sx={{
              fontsize: "14px",
              marginTop: "4px",
              "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                padding: "8px!important",
              },
            }}
            type="text"
            placeholder={"Enter Sms"}
            value={smsData.smsText}
            name="smsText"
            onChange={handleChange}
          />
        </GridItem>
        <GridItem sm={12} md={12} lg={12}>
          <TextFieldLableComponent title={"Enter Number"} />
          <TextFieldComponent
            sx={{
              fontsize: "14px",
              marginTop: "4px",
              "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                padding: "8px!important",
              },
            }}
            type="number"
            placeholder={"Enter Number"}
            value={smsData.SmsNumber}
            name="SmsNumber"
            onChange={handleChange}
          />
        </GridItem>
      </GridContainer>
    </ModalComponent>
  );
};

export default SmsModal;
