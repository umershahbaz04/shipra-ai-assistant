import React, { useState, useEffect, useMemo } from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { GridContainer, GridItem } from "../../../utilities/helpers/Helpers";
import { useSelector } from "react-redux";

const ZoneModal = ({
  open,
  onClose,
  onSave,
  cities = [],
  initialData = null,
  isEdit = false,
}) => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [zoneName, setZoneName] = useState("");
  const [cityId, setCityId] = useState("");
  const [loading, setLoading] = useState(false);

  const cityOptions = useMemo(() => {
    return (cities || []).map((city) => {
      const idVal =
        city.cityID ??
        city.CityID ??
        city.cityId ??
        city.CityId ??
        city.id ??
        city.Id ??
        city.value;
      const labelVal =
        city.name ||
        city.Name ||
        city.cityName ||
        city.CityName ||
        city.text ||
        city.label ||
        "Unnamed City";
      return {
        label: labelVal,
        value: idVal !== undefined && idVal !== null ? String(idVal) : "",
      };
    });
  }, [cities]);

  useEffect(() => {
    if (initialData) {
      setZoneName(initialData.name || initialData.Name || "");
      const rawCityId =
        initialData.cityID ??
        initialData.CityID ??
        initialData.cityId ??
        initialData.CityId ??
        "";
      setCityId(
        rawCityId !== undefined && rawCityId !== null ? String(rawCityId) : "",
      );
    } else {
      setZoneName("");
      setCityId("");
    }
  }, [initialData, open]);

  const selectedCity = useMemo(() => {
    if (!cityId) return null;
    return cityOptions.find((c) => String(c.value) === String(cityId)) || null;
  }, [cityOptions, cityId]);

  const handleFormSubmit = async () => {
    if (!zoneName.trim()) {
      return;
    }
    setLoading(true);
    try {
      await onSave({
        zoneName: zoneName.trim(),
        cityId: cityId ? parseInt(cityId) : 0,
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={isEdit ? "Edit Zone" : "Create New Zone"}
      actionBtn={
        <ModalButtonComponent
          loading={loading}
          disabled={!zoneName.trim() || loading}
          onClick={handleFormSubmit}
          handleOnClick={handleFormSubmit}
          title={isEdit ? "Update Zone" : "Save Zone"}
        />
      }
    >
      <GridContainer spacing={2}>
        <GridItem xs={12}>
          <TextFieldLableComponent title="Zone Name" required={true} />
          <TextFieldComponent
            fullWidth
            size="small"
            placeholder="Enter Zone Name"
            name="zoneName"
            value={zoneName}
            onChange={(e) => setZoneName(e.target.value)}
          />
        </GridItem>

        <GridItem xs={12}>
          <TextFieldLableComponent title="Select City" required={false} />
          <SelectComponent
            options={cityOptions}
            optionLabel="label"
            optionValue="value"
            value={selectedCity}
            onChange={(name, selectedOption) => {
              setCityId(
                selectedOption && selectedOption.value
                  ? selectedOption.value
                  : "",
              );
            }}
            placeholder="Select City"
            height={38}
          />
        </GridItem>
      </GridContainer>
    </ModalComponent>
  );
};

export default ZoneModal;
