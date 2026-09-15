import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import { UpdateClientOrderBox } from "../../../api/AxiosInterceptors";
import {
  GridContainer,
  GridItem,
  handleCalculateVolume,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { Checkbox, FormControlLabel } from "@mui/material";
import { useSelector } from "react-redux";

const UpdateOrderBoxModal = (props) => {
  const { open, onClose, orderBoxData, getAllClientOrderBox } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [stringFields, setStringFields] = useState({
    boxName: "",
  });
  const [numericFields, setNumericFields] = useState({
    length: null,
    width: null,
    height: null,
  });
  const [loading, setLoading] = useState(false);
  const [isDefault, setIsDefault] = useState(false);

  const handleStringChange = (e) => {
    const { name, value } = e.target;
    setStringFields((prev) => ({
      ...prev,
      [name]: value,
    }));
  };
  // Handle change for numeric fields
  const handleNumericChange = (e) => {
    const { name, value } = e.target;
    setNumericFields((prev) => ({
      ...prev,
      [name]: parseFloat(value),
    }));
  };
  const handleUpdateOrderBox = async () => {
    setLoading(true);
    const body = {
      clientOrderBoxId: orderBoxData.data.clientOrderBoxId,
      boxName: stringFields.boxName,
      length: numericFields.length,
      width: numericFields.width,
      height: numericFields.height,
      isDefault: isDefault,
    };
    await UpdateClientOrderBox(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Failed to update order box.");
        } else {
          successNotification("Update Successfully");
          getAllClientOrderBox();
        }
      })
      .catch((e) => {
        errorNotification("An error occurred while updating the order box.");
      })
      .finally(() => {
        setLoading(false);
        onClose();
      });
  };

  useEffect(() => {
    const currentBoxData = orderBoxData.data;
    if (currentBoxData) {
      setStringFields({
        boxName: currentBoxData.boxName,
      });
      setNumericFields({
        length: currentBoxData.length,
        width: currentBoxData.width,
        height: currentBoxData.height,
      });
      setIsDefault(currentBoxData.isDefault);
    }
  }, [orderBoxData]);

  useEffect(() => {
    if (numericFields.length && numericFields.width && numericFields.height) {
      const calculatedVolume = handleCalculateVolume(
        numericFields.length,
        numericFields.width,
        numericFields.height
      );
      setNumericFields((prev) => ({
        ...prev,
        volume: calculatedVolume,
      }));
    }
  }, [
    numericFields.length,
    numericFields.width,
    numericFields.height,
    orderBoxData,
  ]);

  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="sm"
        title={
          LanguageReducer?.languageType?.SETTING_ORDER_BOX_UPDATE_ORDER_BOX
        }
        actionBtn={
          <ModalButtonComponent
            title={LanguageReducer?.languageType?.SETTING_ORDER_BOX_UPDATE_BOX}
            bg={purple}
            loading={loading}
            onClick={(e) => handleUpdateOrderBox()}
          />
        }
      >
        <GridContainer spacing={1}>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={LanguageReducer?.languageType?.SETTING_ORDER_BOX_BOX_NAME}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter Box Name"}
              value={stringFields.boxName}
              name="boxName"
              onChange={handleStringChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={LanguageReducer?.languageType?.SETTING_ORDER_BOX_LENGTH}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="number"
              placeholder={"Enter Length"}
              value={numericFields.length}
              name="length"
              onChange={handleNumericChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={LanguageReducer?.languageType?.SETTING_ORDER_BOX_WIDTH}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="number"
              placeholder={"Enter Width"}
              value={numericFields.width}
              name="width"
              onChange={handleNumericChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={LanguageReducer?.languageType?.SETTING_ORDER_BOX_HEIGHT}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="number"
              placeholder={"Enter Height"}
              value={numericFields.height}
              name="height"
              onChange={handleNumericChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={LanguageReducer?.languageType?.SETTING_ORDER_BOX_VOLUME}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="number"
              disabled
              placeholder={"Enter Volume"}
              value={numericFields.volume}
              name="volume"
              onChange={handleNumericChange}
            />
          </GridItem>
          <GridItem md={12} sm={12} sx={{ textAlign: "right" }}>
            <FormControlLabel
              control={
                <Checkbox
                  sx={{
                    color: "var(--primary-color)",
                    "&.Mui-checked": {
                      color: "var(--primary-color)",
                    },
                  }}
                  checked={isDefault}
                  onChange={(e) => setIsDefault(e.target.checked)}
                  edge="start"
                  disableRipple
                  defaultChecked={isDefault}
                />
              }
              label={
                LanguageReducer?.languageType?.SETTING_ORDER_BOX_MARK_AS_DEFAULT
              }
            />
          </GridItem>
        </GridContainer>
      </ModalComponent>
    </>
  );
};

export default UpdateOrderBoxModal;
