import { Grid, InputLabel } from "@mui/material";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "@mui/material/colors";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import {
  CreateThirdPartyPickupLocation,
  GetActiveCarrierPickupLocationForSelection,
  GetAllUpsSettingSelectionByType,
} from "../../../api/AxiosInterceptors";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { styleSheet } from "../../../assets/styles/style";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import { successNotification } from "../../../utilities/toast";

const UPSCarrierPickUpLocationModal = (props) => {
  const { open, onClose, carrierId, activeCarrierId } = props;
  const [loading, setLoading] = useState(false);
  const [pickUpLocationData, setPickUpLocationData] = useState([]);
  const [selectedPickUpLocation, setSelectedPickUpLocation] = useState();
  const [numericFields, setNumericFields] = useState({
    weight: null,
    Quantity: null,
  });
  const [dynamicDataSelect, setdynamicDataSelect] = useState({
    ContainerCode: [],
    PickupserviceCode: [],
    selectedContainerCode: null,
    selectedServiceCode: null,
  });

  const handleNumericChange = (e) => {
    const { name, value } = e.target;
    setNumericFields((prev) => ({
      ...prev,
      [name]: parseFloat(value),
    }));
  };

  const getActiveCarrierPickupLocationForSelection = async () => {
    try {
      const response = await GetActiveCarrierPickupLocationForSelection(
        activeCarrierId,
        carrierId
      );
      if (response?.data?.isSuccess) {
        const data = response?.data?.result
          .map((item, index) => ({ ...item, index }))
          .slice(1);
        setPickUpLocationData(data);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
    }
  };

  const fetchContainerCode = async () => {
    try {
      const response = await GetAllUpsSettingSelectionByType(
        "ContainerCode",
        carrierId
      );

      if (response?.data?.isSuccess) {
        setdynamicDataSelect((prev) => ({
          ...prev,
          ContainerCode: response?.data?.result || [],
        }));
      }
    } catch {}
  };
  const fetchPickupserviceCode = async () => {
    try {
      const response = await GetAllUpsSettingSelectionByType(
        "PickupserviceCode",
        carrierId
      );

      if (response?.data?.isSuccess) {
        setdynamicDataSelect((prev) => ({
          ...prev,
          PickupserviceCode: response?.data?.result || [],
        }));
      }
    } catch {}
  };

  const handleCreateThirdPartyPickupLocation = async () => {
    const body = {
      weight: numericFields?.weight,
      Quantity: numericFields?.Quantity,
      ServiceCode: dynamicDataSelect?.selectedServiceCode?.id,
      ContainerCode: dynamicDataSelect?.selectedContainerCode?.id,
      ActiveCarrierId: activeCarrierId,
      carrierid: carrierId,
      ActiveCarrierPickupLocationId:
        selectedPickUpLocation?.activeCarrierPickupLocationId,
    };
    setLoading(true);
    try {
      const response = await CreateThirdPartyPickupLocation(body);
      console.log(response);
      if (response?.data?.isSuccess) {
        successNotification("Third Party Location Create Successfully");
        onClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getActiveCarrierPickupLocationForSelection();
    fetchContainerCode();
    fetchPickupserviceCode();
  }, []);

  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="md"
        title={"Pickup Location"}
        actionBtn={
          <ModalButtonComponent
            loading={loading}
            title={"Save"}
            bg={purple}
            onClick={handleCreateThirdPartyPickupLocation}
          />
        }
      >
        <Grid container spacing={1}>
          <Grid item md={6} sm={6} xs={6}>
            <InputLabel sx={styleSheet.inputLabel}>
              Select Pickup Location
            </InputLabel>
            <SelectComponent
              name="pickUpLocation"
              options={pickUpLocationData}
              value={selectedPickUpLocation}
              optionLabel={EnumOptions.CARRIER_PICKUP_LOCATION.LABEL}
              optionValue={EnumOptions.CARRIER_PICKUP_LOCATION.VALUE}
              onChange={(e, newValue) => {
                const resolvedId = newValue ? newValue : null;
                setSelectedPickUpLocation(resolvedId);
              }}
            />
          </Grid>
          <Grid item md={6} sm={6} xs={6}>
            <InputLabel sx={styleSheet.inputLabel}>
              Select Container Code
            </InputLabel>
            <SelectComponent
              name="ContainerCode"
              options={dynamicDataSelect.ContainerCode}
              value={dynamicDataSelect.selectedContainerCode}
              optionLabel={EnumOptions.CONTAINER_CODE.LABEL}
              optionValue={EnumOptions.CONTAINER_CODE.VALUE}
              onChange={(e, newValue) => {
                setdynamicDataSelect((prev) => ({
                  ...prev,
                  selectedContainerCode: newValue,
                }));
              }}
            />
          </Grid>
          <Grid item md={6} sm={6} xs={6}>
            <InputLabel sx={styleSheet.inputLabel}>
              Select Pickup serviceCode
            </InputLabel>
            <SelectComponent
              name="PickupserviceCode"
              options={dynamicDataSelect.PickupserviceCode}
              value={dynamicDataSelect.selectedServiceCode}
              optionLabel={EnumOptions.PICKUP_SERVICE_CODE.LABEL}
              optionValue={EnumOptions.PICKUP_SERVICE_CODE.VALUE}
              onChange={(e, newValue) => {
                setdynamicDataSelect((prev) => ({
                  ...prev,
                  selectedServiceCode: newValue,
                }));
              }}
            />
          </Grid>
          <Grid item md={6} sm={6} xs={6}>
            <InputLabel sx={styleSheet.inputLabel}>Enter Weight</InputLabel>
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="number"
              placeholder={"Enter weight"}
              value={numericFields.weight}
              name="weight"
              onChange={handleNumericChange}
            />
          </Grid>
          <Grid item md={6} sm={6} xs={6}>
            <InputLabel sx={styleSheet.inputLabel}>Enter Quantity</InputLabel>
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="number"
              placeholder={"Enter Quantity"}
              value={numericFields.Quantity}
              name="Quantity"
              onChange={handleNumericChange}
            />
          </Grid>
        </Grid>
      </ModalComponent>
    </>
  );
};

export default UPSCarrierPickUpLocationModal;
