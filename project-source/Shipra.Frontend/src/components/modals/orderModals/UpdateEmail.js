import { purple } from "@mui/material/colors";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import { UpdateCustomerEmail } from "../../../api/AxiosInterceptors";
import { GridContainer, GridItem } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
const initialSmsFieldsData = {
  Email: "",
};
const UpdateEmail = (props) => {
  const { open, onClose, OrderId, generateTotalProcessPaymentLink } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [loading, setLoading] = useState(false);
  const [customerEmail, setcustomerEmail] = useState(initialSmsFieldsData);
  const handleChange = (e) => {
    const { name, value } = e.target;
    setcustomerEmail((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };
  const handleUpdateEmail = async () => {
    setLoading(true);
    const body = {
      email: customerEmail.Email,
      orderAddressId: OrderId,
    };
    console.log(body);
    try {
      const res = await UpdateCustomerEmail(body);
      if (!res?.data?.isSuccess) {
        errorNotification("Failed to Update Email.");
      } else {
        successNotification("Update successfully.");
      }
    } catch (e) {
      errorNotification("An error occurred while Updating the Email.");
    } finally {
      setcustomerEmail(initialSmsFieldsData);
      setLoading(false);
      generateTotalProcessPaymentLink();
      onClose();
    }
  };
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="sm"
        title={"Update Email"}
        actionBtn={
          <ModalButtonComponent
            title={"Update Email and Generate Link"}
            bg={purple}
            loading={loading}
            onClick={(e) => handleUpdateEmail()}
          />
        }
      >
        <GridContainer spacing={1}>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent title={"Email"} />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter url"}
              value={customerEmail.Email}
              name="Email"
              onChange={handleChange}
            />
          </GridItem>
        </GridContainer>
      </ModalComponent>
    </>
  );
};

export default UpdateEmail;
