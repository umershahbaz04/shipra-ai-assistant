import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { GridContainer, GridItem } from "../../../utilities/helpers/Helpers";
import { ColorPicker } from "material-ui-color";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import {
  CreateClientOrderLabelLookup,
  UpdateClientOrderLabelLookup,
} from "../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const CreateOrderLabelsModal = (props) => {
  const { open, onClose, getAllClientOrderLabelLookup, orderLabelData } = props;
  const [loading, setLoading] = useState(false);
  const [colorName, setColorName] = useState("");
  const [color, setColor] = useState("");

  const handleCreateOrderLabel = async (data) => {
    if (!colorName) {
      errorNotification("Please Enter a Color Name");
      return;
    }
    if (!color) {
      errorNotification("Please choose a Color");
      return;
    }
    setLoading(true);
    if (!orderLabelData) {
      try {
        const response = await CreateClientOrderLabelLookup(colorName, color);
        if (response?.data?.isSuccess) {
          successNotification(response?.data?.result?.message);
          getAllClientOrderLabelLookup();
        } else {
          UtilityClass.showErrorNotificationWithDictionary(
            response?.data?.errors
          );
        }
      } catch (e) {
        console.error(e);
      } finally {
        setLoading(false);
        onClose();
      }
    } else {
      try {
        const response = await UpdateClientOrderLabelLookup(
          orderLabelData?.clientOrderLabelLookupId,
          colorName,
          color
        );
        if (response?.data?.isSuccess) {
          successNotification("Label Update successfully");
          getAllClientOrderLabelLookup();
        } else {
          UtilityClass.showErrorNotificationWithDictionary(
            response?.data?.errors
          );
        }
      } catch (e) {
        console.error(e);
      } finally {
        setLoading(false);
        onClose();
      }
    }
  };

  useEffect(() => {
    if (orderLabelData) {
      setColorName(orderLabelData?.labelName);
      setColor(orderLabelData?.colorCode);
    }
  }, [orderLabelData]);

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={!orderLabelData ? "Create Order Label" : "Update Order Label"}
      actionBtn={
        <ModalButtonComponent
          title={!orderLabelData ? "Create Label" : "Update Label"}
          bg={purple}
          loading={loading}
          onClick={handleCreateOrderLabel}
        />
      }
    >
      <GridContainer spacing={1}>
        <GridItem sm={12} md={12} lg={12}>
          <TextFieldLableComponent title="Color Name" />
          <TextFieldComponent
            sx={{
              fontSize: "14px",
              marginTop: "4px",
              "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                padding: "8px!important",
              },
            }}
            type="text"
            placeholder="Enter Color Name"
            value={colorName}
            name="colorName"
            onChange={(e) => setColorName(e.target.value)}
          />
        </GridItem>
        <GridItem sm={12} md={12} lg={12}>
          <TextFieldLableComponent title="Choose Color" />
          <ColorPicker
            defaultValue={"#fff"}
            value={color}
            // onChange={(e) => setColor(`#${e.hex}`)}
            onChange={(e) => {
              console.log(e?.value);
              if (e?.hex) {
                setColor(`#${e.hex}`);
                return;
              }
              setColor(e);
            }}
            
            // disableTextfield
          />
        </GridItem>
      </GridContainer>
    </ModalComponent>
  );
};

export default CreateOrderLabelsModal;
