import { Box, InputLabel, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { inputTypesEnum } from "../../.reUseableComponents/Modal/ConfigSettingModal";
import { SwitchComponent } from "../../.reUseableComponents/Switch/SwitchComponent";
import SelectComponent from "../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAddressEntitiesByType,
  GetAllCountry,
} from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import { getThisKeyCookie, isShowCountryInTabbarFlag } from "../cookies";
import { EnumCookieKeys, EnumOptions } from "../enum";
import {
  ActionButtonCustom,
  LoadingTextField,
  fetchMethod,
  getLowerCase,
  useLanguageReducer,
} from "./Helpers";

export const addressSchemaEnum = Object.freeze({
  area: {
    LABEL: "name",
    VALUE: "id",
  },
  state: {
    LABEL: "name",
    VALUE: "id",
  },
  province: {
    LABEL: "name",
    VALUE: "id",
  },
  city: {
    LABEL: "name",
    VALUE: "id",
  },
  pinCode: {
    LABEL: "name",
    VALUE: "id",
  },
});

export const handleGetAllCountries = async () => {
  let data = [];
  const { response } = await fetchMethod(() => GetAllCountry({}));
  if (response.isSuccess) {
    data = response.result;
  }
  return data;
};

export const handleGetAddressEntitiesByType = async (
  SelectedEntityIds,
  SelectedEntityType,
  NextEntityType,
  selectedCarrierId,
  countryID,
) => {
  let data = [];
  const { response } = await fetchMethod(() =>
    GetAddressEntitiesByType(
      SelectedEntityIds,
      SelectedEntityType,
      NextEntityType,
      selectedCarrierId ?? 0,
      countryID,
    ),
  );
  if (response.isSuccess) {
    data = response.result;
  }
  return data;
};
export const handleGetOptionObjByOptionId = (options = [], key, id) => {
  const _selectedOption = options.find((dt) => dt[key] == id);
  return _selectedOption;
};
export const handleGetOptionIndexByOptionId = (options = [], key, value) => {
  const _selectedOptionIndex = options.findIndex((dt) => dt[key] === value);
  return _selectedOptionIndex;
};
export const checkRequiredUsingCarrierAdressSchema = (
  carrierObj,
  key,
  required,
) => {
  let parsedData = null;
  if (Boolean(carrierObj?.AddressingScheme)) {
    parsedData = JSON.parse(carrierObj.AddressingScheme);
  }
  const carrierAddressSchema = parsedData?.mappedCivilEntity || [];
  const selectedKeyInCarrierSchema = carrierAddressSchema.find((dt) =>
    Object.hasOwn(dt, key),
  );
  const isRequired =
    carrierAddressSchema?.length > 0
      ? Boolean(selectedKeyInCarrierSchema?.required)
      : required;
  return isRequired;
};
export const useGetAddressSchema = (
  setValue = () => { },
  showDefaultCountry,
  selectedCarrierId,
  postfix,
) => {
  // obj values
  const initialSelectedAddressSchemaWithObjValue = {
    country: null,
    city: null,
    state: null,
    province: null,
    pinCode: null,
    area: null,
  };
  const [
    selectedAddressSchemaWithObjValue,
    setSelectedAddressSchemaWithObjValue,
  ] = useState(initialSelectedAddressSchemaWithObjValue);
  const initialSelectedAddressSchemaWithObjValueForMultiple = {
    city: [],
    state: [],
    province: [],
    pinCode: [],
    area: [],
  };
  const [
    selectedAddressSchemaWithObjValueForMultiple,
    setSelectedAddressSchemaWithObjValueForMultiple,
  ] = useState(initialSelectedAddressSchemaWithObjValueForMultiple);
  // ids

  const initialSelectedAddressSchemaMoreInfoFields = {
    houseNo: "",
    buildingName: "",
    landmark: "",
    zip: "",
  };
  const initialSelectedAddressSchema = {
    country: null,
    city: null,
    state: null,
    province: null,
    pinCode: null,
    area: null,
    streetAddress: "",
    streetAddress2: "",
    ...initialSelectedAddressSchemaMoreInfoFields,
  };

  const [selectedAddressSchema, setSelectedAddressSchema] = useState(
    initialSelectedAddressSchema,
  );
  const initialSelectedAddressSchemaForMutiple = {
    city: [],
    state: [],
    province: [],
    pinCode: [],
    area: [],
  };
  const [selectedAddressSchemaForMutiple, setSelectedAddressSchemaForMutiple] =
    useState(initialSelectedAddressSchemaForMutiple);
  const [addressSchema, setAddressSchema] = useState({});
  const [addressSchemaSelectData, setAddressSchemaSelectData] = useState([]);
  const [
    addressSchemaSelectDataIncExcValues,
    setAddressSchemaSelectDataIncExcValues,
  ] = useState({});
  const [addressSchemaInputData, setAddressSchemaInputData] = useState([]);

  const moreInfoInputData = [
    {
      key: "houseNo",
      required: false,
      label: "House No",
      type: "text",
    },
    {
      key: "buildingName",
      required: false,
      label: "Building Name",
      type: "text",
    },
    {
      key: "landmark",
      required: false,
      label: "Landmark",
      type: "text",
    },
    {
      key: "zip",
      required: false,
      label: "Zip",
      type: "text",
    },
  ];
  const [showMoreInfoBtn, setShowMoreInfoBtn] = useState(false);

  const handleAddRemoveMoreInfoFields = (value, setValue = () => { }) => {
    setShowMoreInfoBtn(value);
    if (value) {
      setAddressSchemaInputData((prev) => [...prev, ...moreInfoInputData]);
      return;
    }
    Object.keys(initialSelectedAddressSchemaMoreInfoFields).forEach((key) => {
      setValue(key, "");
      setSelectedAddressSchema((prev) => ({
        ...prev,
        [key]: "",
      }));
    });
    setAddressSchemaInputData((prev) =>
      prev.filter(
        (item) => !moreInfoInputData.some((item2) => item2.key === item.key),
      ),
    );
  };

  const [selectedCountryCode, setSelectedCountryCode] = useState(
    getThisKeyCookie(EnumCookieKeys.COUNTRY_CODE),
  );
  // const handleCountryChange = (newCountryCode) => {
  //   setSelectedCountryCode(newCountryCode);
  //   return { prev: selectedCountryCode, next: newCountryCode };
  // };

  // for country change
  const handleSetSchema = async (
    key,
    selectedCountry = {},
    setValue = () => { },
    unregister = () => { },
    paramSelectedCarrierId,
  ) => {
    setShowMoreInfoBtn(false);
    const schema = selectedCountry?.addressingScheme;
    const _selectedAddressSchema = {};
    Object.keys(selectedAddressSchema).forEach((schema_key) => {
      _selectedAddressSchema[schema_key] = "";
      setValue(postfix ? `${schema_key}_${postfix}` : key, null);
      if (schema_key !== "country") {
        unregister(schema_key);
      }
    });
    setValue(postfix ? `${key}_${postfix}` : key, null);
    setSelectedAddressSchemaWithObjValue(
      initialSelectedAddressSchemaWithObjValue,
    );
    setSelectedAddressSchemaWithObjValueForMultiple(
      initialSelectedAddressSchemaWithObjValueForMultiple,
    );
    setSelectedAddressSchemaForMutiple(initialSelectedAddressSchemaForMutiple);
    setSelectedAddressSchema(_selectedAddressSchema);
    setAddressSchema({});
    setAddressSchemaSelectData([]);
    setAddressSchemaInputData([]);
    setAddressSchemaSelectDataIncExcValues({});
    if (schema) {
      const value = selectedCountry[EnumOptions.COUNTRY.VALUE];
      setValue(postfix ? `${key}_${postfix}` : key, selectedCountry);
      setSelectedAddressSchemaWithObjValue((prev) => ({
        ...prev,
        [key]: selectedCountry,
      }));
      setSelectedAddressSchema((prev) => ({
        ...prev,
        country: value,
      }));
      let parsedData = JSON.parse(schema);
      const customSchema = parsedData?.customAddressSchema;
      if (customSchema) {
        parsedData = {
          ...parsedData,
          ...customSchema,
        };
      }
      const _inputData = Object.entries(parsedData)
        .map(([key, data]) => {
          if (
            key !== "keys" &&
            key !== "country" &&
            key !== "customAddressSchema" &&
            !parsedData.keys.includes(key) &&
            getLowerCase(data?.type) !== inputTypesEnum.SELECT
            // data?.type !== inputTypesEnum.SELECT)
          ) {
            return { ...data, key };
          }
        })
        .filter((dt) => dt);
      setAddressSchemaInputData(_inputData);
      setAddressSchema(parsedData);
      const _selectDataInitialized = parsedData?.keys
        ?.map((dt, index) => {
          if (parsedData[dt].type === inputTypesEnum.SELECT) {
            return {
              key: dt,
              options: [],
              disabled: true,
              ...parsedData[dt],
            };
          }
        })
        .filter(Boolean);
      // const hasIncExcFilter = parsedData?.keys?.some(
      //   (dt) => parsedData[dt]?.hasIncExcFilter
      // );
      //shehzad bhai n kaha hai ye har bar jAay ga
      // if (hasIncExcFilter) {
      const _addressSchemaSelectDataIncExcValues = parsedData?.keys?.reduce(
        (acc, dt) => {
          acc[dt] = {
            id: "",
            include: parsedData[dt]?.hasIncExcFilter || true,
          };
          return acc;
        },
        {},
      );
      setAddressSchemaSelectDataIncExcValues(
        _addressSchemaSelectDataIncExcValues,
      );
      // }
      // fetch options of optional dropdowns untill get mendatory dropdown
      for (let parsedKey of parsedData?.keys) {
        if (parsedData[parsedKey].type !== inputTypesEnum.SELECT) {
          const inputInSelectKeysIndex = parsedData.keys.findIndex(
            (dt) => parsedData[dt].type !== inputTypesEnum.SELECT,
          );
          _selectDataInitialized.splice(inputInSelectKeysIndex, 0, {
            ...parsedData[parsedKey],
            key: parsedKey,
          });
          setAddressSchemaSelectData([..._selectDataInitialized]);
          break;
        }
        let _options = [];
        //  set loading and enable input
        setAddressSchemaSelectData((prev) => {
          const _selectedAddressSchema = [..._selectDataInitialized];
          const parsedKeyIndex = _selectedAddressSchema.findIndex(
            (dt) => dt.key === parsedKey,
          );
          _selectedAddressSchema[parsedKeyIndex].loading = true;
          _selectedAddressSchema[parsedKeyIndex].disabled = false;
          return _selectedAddressSchema;
        });

        _options = await handleGetAddressEntitiesByType(
          value,
          parsedData[parsedKey].parentKey,
          parsedKey,
          paramSelectedCarrierId,
          value,
        );
        setAddressSchemaSelectData((prev) => {
          const _selectedAddressSchema = [..._selectDataInitialized];
          const parsedKeyIndex = _selectedAddressSchema.findIndex(
            (dt) => dt.key === parsedKey,
          );
          _selectedAddressSchema[parsedKeyIndex].loading = false;
          _selectedAddressSchema[parsedKeyIndex].options = _options;
          return _selectedAddressSchema;
        });
        if (parsedData[parsedKey].required) {
          break;
        }
      }
    }
  };

  //reset
  const handleReset = () => {
    setSelectedAddressSchema(initialSelectedAddressSchema);
    setSelectedAddressSchemaForMutiple(initialSelectedAddressSchemaForMutiple);
    setSelectedAddressSchemaWithObjValueForMultiple(
      initialSelectedAddressSchemaWithObjValueForMultiple,
    );
    setSelectedAddressSchemaWithObjValue(
      initialSelectedAddressSchemaWithObjValue,
    );
    setAddressSchemaSelectData([]);
  };

  // for schema select field changes
  const handleChangeSelectAddressSchemaAndGetOptions = async (
    key,
    index,
    value,
    setValue,
    name,
  ) => {
    const id = value ? value[addressSchemaEnum[key]?.VALUE] : null;
    // remove next values
    const slicedKeysArrayStartIndex = addressSchema.keys?.indexOf(key) + 1;
    const slicedKeysArray = addressSchema.keys?.slice(
      slicedKeysArrayStartIndex,
    );
    if (addressSchema[key].required) {
      slicedKeysArray.forEach((schema_key) => {
        setValue(postfix ? `${schema_key}_${postfix}` : schema_key, null);
        if (addressSchemaSelectDataIncExcValues[schema_key]) {
          setAddressSchemaSelectDataIncExcValues((prev) => ({
            ...prev,
            [schema_key]: {
              ...prev[schema_key],
              id: "",
            },
          }));
        }
        setSelectedAddressSchema((prev) => ({ ...prev, [schema_key]: null }));
        setSelectedAddressSchemaWithObjValue((prev) => ({
          ...prev,
          [schema_key]: null,
        }));
        setAddressSchemaSelectData((prev) => {
          const _schemaData = [...prev];
          const _selectedIndexBykey = handleGetOptionIndexByOptionId(
            _schemaData,
            "key",
            schema_key,
          );
          _schemaData[_selectedIndexBykey].disabled = true;
          _schemaData[_selectedIndexBykey].options = [];
          return _schemaData;
        });
      });
    }
    if (id) {
      setSelectedAddressSchemaWithObjValue((prev) => ({
        ...prev,
        [key]: value,
      }));
      setSelectedAddressSchema((prev) => ({ ...prev, [key]: id }));
      if (addressSchemaSelectDataIncExcValues[key]) {
        setAddressSchemaSelectDataIncExcValues((prev) => ({
          ...prev,
          [key]: {
            ...prev[key],
            id: String(id),
          },
        }));
      }
      setValue(name, value);
      if (addressSchema[key].required) {
        // fetch options of optional dropdowns untill get mendatory dropdown
        for (let parsedKey of slicedKeysArray) {
          let _options = [];
          //  set loading and enable input
          setAddressSchemaSelectData((prev) => {
            const _selectedAddressSchema = [...prev];
            const parsedKeyIndex = _selectedAddressSchema.findIndex(
              (dt) => dt.key === parsedKey,
            );
            _selectedAddressSchema[parsedKeyIndex].loading = true;
            _selectedAddressSchema[parsedKeyIndex].disabled = false;
            return _selectedAddressSchema;
          });
          _options = await handleGetAddressEntitiesByType(
            id,
            addressSchema[parsedKey].parentKey,
            parsedKey,
            selectedCarrierId,
            selectedAddressSchema.country,
          );
          setAddressSchemaSelectData((prev) => {
            const _selectedAddressSchema = [...prev];
            const parsedKeyIndex = _selectedAddressSchema.findIndex(
              (dt) => dt.key === parsedKey,
            );
            _selectedAddressSchema[parsedKeyIndex].loading = false;
            _selectedAddressSchema[parsedKeyIndex].options = _options;
            return _selectedAddressSchema;
          });
          if (addressSchema[parsedKey].required) {
            break;
          }
        }
      }
    } else {
      setValue(postfix ? `${key}_${postfix}` : key, null);
      setSelectedAddressSchema((prev) => ({ ...prev, [key]: null }));
      setSelectedAddressSchemaWithObjValue((prev) => ({
        ...prev,
        [key]: null,
      }));
      if (addressSchemaSelectDataIncExcValues[key]) {
        setAddressSchemaSelectDataIncExcValues((prev) => ({
          ...prev,
          [key]: {
            ...prev[key],
            id: "",
          },
        }));
      }
    }
  };
  const handleChangeSelectAddressSchemaIncExcSwitch = (key) => (e) => {
    const value = e.target.checked;
    setAddressSchemaSelectDataIncExcValues((prev) => ({
      ...prev,
      [key]: {
        ...prev[key],
        include: value,
      },
    }));
  };
  // for schema select field change for multiple
  const handleChangeSelectAddressSchemaAndGetOptionsForMultiple = async (
    key,
    index,
    value,
    setValue,
    name,
  ) => {
    const ids =
      value.length > 0
        ? value.map((dt) => {
          return dt[addressSchemaEnum[key]?.VALUE];
        })
        : null;
    const slicedKeysArrayStartIndex = addressSchema.keys?.indexOf(key) + 1;
    const slicedKeysArray = addressSchema.keys?.slice(
      slicedKeysArrayStartIndex,
    );
    // for  remove next values
    if (addressSchema[key].required) {
      slicedKeysArray.forEach((schema_key) => {
        setValue(postfix ? `${schema_key}_${postfix}` : schema_key, null);
        if (addressSchemaSelectDataIncExcValues[schema_key]) {
          setAddressSchemaSelectDataIncExcValues((prev) => ({
            ...prev,
            [schema_key]: {
              ...prev[schema_key],
              id: "",
            },
          }));
        }
        setSelectedAddressSchemaForMutiple((prev) => ({
          ...prev,
          [schema_key]: [],
        }));
        setSelectedAddressSchemaWithObjValueForMultiple((prev) => ({
          ...prev,
          [schema_key]: [],
        }));
        setAddressSchemaSelectData((prev) => {
          const _schemaData = [...prev];
          const _selectedIndexBykey = handleGetOptionIndexByOptionId(
            _schemaData,
            "key",
            schema_key,
          );
          _schemaData[_selectedIndexBykey].disabled = true;
          _schemaData[_selectedIndexBykey].options = [];
          return _schemaData;
        });
      });
    }
    if (ids) {
      setSelectedAddressSchemaWithObjValueForMultiple((prev) => ({
        ...prev,
        [key]: value,
      }));
      setSelectedAddressSchemaForMutiple((prev) => ({ ...prev, [key]: ids }));
      if (addressSchemaSelectDataIncExcValues[key]) {
        setAddressSchemaSelectDataIncExcValues((prev) => ({
          ...prev,
          [key]: {
            ...prev[key],
            id: String(ids),
          },
        }));
      }
      setValue(name, value);
      if (addressSchema[key].required) {
        // fetch options of optional dropdowns untill get mendatory dropdown
        for (let parsedKey of slicedKeysArray) {
          let _options = [];
          //  set loading and enable input
          setAddressSchemaSelectData((prev) => {
            const _selectedAddressSchema = [...prev];
            const parsedKeyIndex = _selectedAddressSchema.findIndex(
              (dt) => dt.key === parsedKey,
            );
            _selectedAddressSchema[parsedKeyIndex].loading = true;
            _selectedAddressSchema[parsedKeyIndex].disabled = false;
            return _selectedAddressSchema;
          });
          _options = await handleGetAddressEntitiesByType(
            ids,
            addressSchema[parsedKey].parentKey,
            parsedKey,
            selectedCarrierId,
            selectedAddressSchema.country,
          );
          setAddressSchemaSelectData((prev) => {
            const _selectedAddressSchema = [...prev];
            const parsedKeyIndex = _selectedAddressSchema.findIndex(
              (dt) => dt.key === parsedKey,
            );
            _selectedAddressSchema[parsedKeyIndex].loading = false;
            _selectedAddressSchema[parsedKeyIndex].options = _options;
            return _selectedAddressSchema;
          });
          if (addressSchema[parsedKey].required) {
            break;
          }
        }
      }
    } else {
      setValue(postfix ? `${key}_${postfix}` : key, null);
      setSelectedAddressSchemaForMutiple((prev) => ({
        ...prev,
        [key]: [],
      }));
      setSelectedAddressSchemaWithObjValueForMultiple((prev) => ({
        ...prev,
        [key]: [],
      }));

      if (addressSchemaSelectDataIncExcValues[key]) {
        setAddressSchemaSelectDataIncExcValues((prev) => ({
          ...prev,
          [key]: {
            ...prev[key],
            id: "",
          },
        }));
      }
    }
  };

  // for schema input field changes
  const handleChangeInputAddressSchema = async (key, value, setValue) => {
    setSelectedAddressSchema((prev) => ({
      ...prev,
      [key]: value,
    }));
    setSelectedAddressSchemaWithObjValue((prev) => ({
      ...prev,
      [key]: value,
    }));
    setValue(key, value);
    const customAddressSchema = addressSchema?.customAddressSchema;
    const customAddressSchemaKeys = customAddressSchema?.keys;
    if (customAddressSchema) {
      if (
        customAddressSchemaKeys?.includes(key) &&
        customAddressSchema[key]?.type !== inputTypesEnum.SELECT
      ) {
        const childKeys = customAddressSchemaKeys.filter(
          (k) =>
            customAddressSchema[k]?.parentKey === key ||
            getLowerCase(customAddressSchema[k]?.type) ===
            inputTypesEnum.SELECT,
        );
        const strVal = String(value ?? "").trim();
        if (strVal.length >= 6) {
          for (let childKey of childKeys) {
            const parentKeyName =
              customAddressSchema[childKey]?.parentKey || key;

            setAddressSchemaSelectData((prev) => {
              const _selectedAddressSchema = [...prev];
              const parsedKeyIndex = _selectedAddressSchema.findIndex(
                (dt) => dt.key === childKey,
              );
              _selectedAddressSchema[parsedKeyIndex].loading = true;
              _selectedAddressSchema[parsedKeyIndex].disabled = false;
              return _selectedAddressSchema;
            });

            const _options = await handleGetAddressEntitiesByType(
              value,
              parentKeyName,
              childKey,
              selectedCarrierId,
              selectedAddressSchema.country,
            );

            setAddressSchemaSelectData((prev) => {
              const _selectedAddressSchema = [...prev];
              const parsedKeyIndex = _selectedAddressSchema.findIndex(
                (dt) => dt.key === childKey,
              );
              _selectedAddressSchema[parsedKeyIndex].loading = false;
              _selectedAddressSchema[parsedKeyIndex].disabled = true;
              _selectedAddressSchema[parsedKeyIndex].options = _options;
              return _selectedAddressSchema;
            });

            const firstOpt =
              _options && _options.length > 0 ? _options[0] : null;
            if (firstOpt) {
              const optVal =
                firstOpt[addressSchemaEnum[childKey]?.VALUE || "id"];
              setValue(postfix ? `${childKey}_${postfix}` : childKey, firstOpt);
              setSelectedAddressSchema((prev) => ({
                ...prev,
                [childKey]: optVal,
              }));
              setSelectedAddressSchemaWithObjValue((prev) => ({
                ...prev,
                [childKey]: firstOpt,
              }));
            } else {
              setValue(postfix ? `${childKey}_${postfix}` : childKey, null);
              setSelectedAddressSchema((prev) => ({
                ...prev,
                [childKey]: null,
              }));
              setSelectedAddressSchemaWithObjValue((prev) => ({
                ...prev,
                [childKey]: null,
              }));
            }
          }
        } else {
          for (let childKey of childKeys) {
            setValue(postfix ? `${childKey}_${postfix}` : childKey, null);
            setSelectedAddressSchema((prev) => ({
              ...prev,
              [childKey]: null,
            }));
            setSelectedAddressSchemaWithObjValue((prev) => ({
              ...prev,
              [childKey]: null,
            }));
            setAddressSchemaSelectData((prev) =>
              prev.map((item) =>
                item.key === childKey
                  ? { ...item, loading: false, disabled: true, options: [] }
                  : item,
              ),
            );
          }
        }
      }
    }
  };
  // edit mode
  const handleSetSchemaValueForUpdate = async (
    data = {},
    setValue = () => { },
    carrierID,
    countryInputName = "country",
  ) => {
    const countries = await handleGetAllCountries();
    const selectedCountryId = data["country"];
    const selectedCountryObj = handleGetOptionObjByOptionId(
      countries,
      EnumOptions.COUNTRY.VALUE,
      selectedCountryId,
    );
    setValue(countryInputName, selectedCountryObj);
    setSelectedCountryCode(selectedCountryObj?.mapCountryCode);
    setSelectedAddressSchemaWithObjValue((prev) => ({
      ...prev,
      country: selectedCountryObj,
    }));
    setSelectedAddressSchema((prev) => ({ ...prev, country: data["country"] }));
    const schema = selectedCountryObj?.addressingScheme;
    if (schema) {
      let parsedSchema = JSON.parse(schema);

      const customSchema = parsedSchema?.customAddressSchema;
      if (customSchema) {
        parsedSchema = {
          ...parsedSchema,
          ...customSchema,
        };
      }
      setAddressSchema(parsedSchema);
      const schemaKeys = parsedSchema.keys || [];
      let _selectDataInitialized = schemaKeys
        .map((dt, index) => {
          if (parsedSchema[dt].type === inputTypesEnum.SELECT) {
            return {
              key: dt,
              options: [],
              loading: index === 0,
              disabled: index !== 0,
              ...parsedSchema[dt],
            };
          }
        })
        .filter(Boolean);
      const _inputData = Object.entries(parsedSchema)
        .map(([key, input_data]) => {
          if (
            key !== "keys" &&
            key !== "country" &&
            key !== "customAddressSchema" &&
            !parsedSchema.keys.includes(key) &&
            input_data?.type !== inputTypesEnum.SELECT
          ) {
            setValue(key, data[key]);
            setSelectedAddressSchemaWithObjValue((prev) => ({
              ...prev,
              [key]: data[key],
            }));
            setSelectedAddressSchema((prev) => ({ ...prev, [key]: data[key] }));
            return { ...input_data, key };
          }
        })
        .filter(Boolean);
      let showMoreInfoFields = false;

      Object.keys(initialSelectedAddressSchemaMoreInfoFields).forEach(
        (moreInfoKey) => {
          if (data[moreInfoKey]) {
            showMoreInfoFields = true;
            setValue(moreInfoKey, data[moreInfoKey]);
            setShowMoreInfoBtn(true);
            setSelectedAddressSchema((prev) => ({
              ...prev,
              [moreInfoKey]: data[moreInfoKey],
            }));
          }
        },
      );
      if (showMoreInfoFields) {
        setAddressSchemaInputData([..._inputData, ...moreInfoInputData]);
      } else {
        setAddressSchemaInputData(_inputData);
      }

      // fetch options of optional dropdowns untill get mendatory dropdown
      for (let parsedKey of schemaKeys) {
        const isSelectKeyisTypeInput =
          parsedSchema[parsedKey].type !== inputTypesEnum.SELECT;
        if (isSelectKeyisTypeInput) {
          const parsedKey_val = data[parsedKey];
          const inputInSelectKeysIndex = parsedSchema.keys.findIndex(
            (dt) => parsedSchema[dt].type !== inputTypesEnum.SELECT,
          );
          _selectDataInitialized.splice(inputInSelectKeysIndex, 0, {
            ...parsedSchema[parsedKey],
            key: parsedKey,
          });
          setValue(parsedKey, parsedKey_val);
          setSelectedAddressSchema((prev) => ({
            ...prev,
            parsedKey: parsedKey_val,
          }));
          setSelectedAddressSchemaWithObjValue((prev) => ({
            ...prev,
            parsedKey: parsedKey_val,
          }));
          continue;
        }
        let _options = [];
        //  set loading and enable input
        setAddressSchemaSelectData((prev) => {
          const _selectedAddressSchema = [..._selectDataInitialized];
          const parsedKeyIndex = _selectedAddressSchema.findIndex(
            (dt) => dt.key === parsedKey,
          );
          _selectedAddressSchema[parsedKeyIndex].loading = true;
          _selectedAddressSchema[parsedKeyIndex].disabled = false;
          return _selectedAddressSchema;
        });
        const parentKey = parsedSchema[parsedKey].parentKey;
        _options = await handleGetAddressEntitiesByType(
          data[parentKey],
          parentKey,
          parsedKey,
          carrierID,
          selectedCountryId,
        );
        setValue(
          postfix ? `${parsedKey}_${postfix}` : parsedKey,
          handleGetOptionObjByOptionId(
            _options,
            addressSchemaEnum[parsedKey]?.VALUE,
            data[parsedKey],
          ),
        );
        setSelectedAddressSchema((prev) => ({
          ...prev,
          [parsedKey]: data[parsedKey],
        }));
        setSelectedAddressSchemaWithObjValue((prev) => ({
          ...prev,
          [parsedKey]: handleGetOptionObjByOptionId(
            _options,
            addressSchemaEnum[parsedKey]?.VALUE,
            data[parsedKey],
          ),
        }));
        setAddressSchemaSelectData((prev) => {
          const _selectedAddressSchema = [..._selectDataInitialized];
          const parsedKeyIndex = _selectedAddressSchema.findIndex(
            (dt) => dt.key === parsedKey,
          );
          _selectedAddressSchema[parsedKeyIndex].loading = false;

          const isParentKeyisTypeInput =
            parsedSchema[parentKey].type !== inputTypesEnum.SELECT;
          if (isParentKeyisTypeInput) {
            _selectedAddressSchema[parsedKeyIndex].disabled = true;
          }
          _selectedAddressSchema[parsedKeyIndex].options = _options;
          return _selectedAddressSchema;
        });
      }
    }
  };

  useEffect(() => {
    if (showDefaultCountry) {
      // default mode
      const handleSetDefaultSchemaValue = async () => {
        const countries = await handleGetAllCountries();
        const countryFlag = isShowCountryInTabbarFlag();
        const countryIdCookieKey = countryFlag
          ? EnumCookieKeys.COUNTRY_ID
          : EnumCookieKeys.LOGGEDIN_CLIENT_COUNTRY_ID;
        const rawCookieVal = getThisKeyCookie(countryIdCookieKey);
        const cookie_country_id =
          rawCookieVal && rawCookieVal !== "null" && rawCookieVal !== "undefined"
            ? Number(rawCookieVal)
            : null;
        const selectedCountryObj = cookie_country_id
          ? handleGetOptionObjByOptionId(
            countries,
            EnumOptions.COUNTRY.VALUE,
            cookie_country_id,
          )
          : null;
        const schema = selectedCountryObj?.addressingScheme;
        if (schema) {
          setValue("country", selectedCountryObj);
          setSelectedCountryCode(selectedCountryObj?.mapCountryCode);
          setSelectedAddressSchemaWithObjValue((prev) => ({
            ...prev,
            country: selectedCountryObj,
          }));
          setSelectedAddressSchema((prev) => ({
            ...prev,
            country: cookie_country_id,
          }));
          let parsedData = JSON.parse(schema);
          const customSchema = parsedData?.customAddressSchema;
          if (customSchema) {
            parsedData = {
              ...parsedData,
              ...customSchema,
            };
          }
          const _inputData = Object.entries(parsedData)
            .map(([key, data]) => {
              if (
                key !== "keys" &&
                key !== "country" &&
                key !== "customAddressSchema" &&
                !parsedData.keys.includes(key) &&
                data?.type !== inputTypesEnum.SELECT
              ) {
                return { ...data, key };
              }
            })
            .filter((dt) => dt);
          setAddressSchemaInputData(_inputData);
          setAddressSchema(parsedData);
          const _selectDataInitialized = parsedData?.keys
            ?.map((dt, index) => {
              if (parsedData[dt].type === inputTypesEnum.SELECT) {
                return {
                  key: dt,
                  options: [],
                  disabled: true,
                  ...parsedData[dt],
                };
              }
            })
            .filter(Boolean);
          // fetch options of optional dropdowns untill get mendatory dropdown
          for (let parsedKey of parsedData?.keys) {
            if (parsedData[parsedKey].type !== inputTypesEnum.SELECT) {
              const inputInSelectKeysIndex = parsedData.keys.findIndex(
                (dt) => parsedData[dt].type !== inputTypesEnum.SELECT,
              );
              _selectDataInitialized.splice(inputInSelectKeysIndex, 0, {
                ...parsedData[parsedKey],
                key: parsedKey,
              });
              setAddressSchemaSelectData([..._selectDataInitialized]);
              break;
            }
            let _options = [];
            //  set loading and enable input
            setAddressSchemaSelectData((prev) => {
              const _selectedAddressSchema = [..._selectDataInitialized];
              const parsedKeyIndex = _selectedAddressSchema.findIndex(
                (dt) => dt.key === parsedKey,
              );
              _selectedAddressSchema[parsedKeyIndex].loading = true;
              _selectedAddressSchema[parsedKeyIndex].disabled = false;
              return _selectedAddressSchema;
            });
            _options = await handleGetAddressEntitiesByType(
              cookie_country_id,
              parsedData[parsedKey].parentKey,
              parsedKey,
              selectedCarrierId,
              cookie_country_id,
            );
            setAddressSchemaSelectData((prev) => {
              const _selectedAddressSchema = [..._selectDataInitialized];
              const parsedKeyIndex = _selectedAddressSchema.findIndex(
                (dt) => dt.key === parsedKey,
              );
              _selectedAddressSchema[parsedKeyIndex].loading = false;
              _selectedAddressSchema[parsedKeyIndex].options = _options;
              return _selectedAddressSchema;
            });
            if (parsedData[parsedKey].required) {
              break;
            }
          }
        } else {
          setValue("country", null);
          setSelectedCountryCode(null);
          setSelectedAddressSchemaWithObjValue((prev) => ({
            ...prev,
            country: null,
          }));
          setSelectedAddressSchema((prev) => ({
            ...prev,
            country: null,
          }));
          setAddressSchema({});
          setAddressSchemaSelectData([]);
          setAddressSchemaInputData([]);
        }
      };
      handleSetDefaultSchemaValue();
    }
  }, []);

  return {
    selectedAddressSchema,
    selectedAddressSchemaForMutiple,
    selectedAddressSchemaWithObjValue,
    selectedAddressSchemaWithObjValueForMultiple,
    addressSchema,
    addressSchemaSelectData,
    addressSchemaSelectDataIncExcValues,
    addressSchemaInputData,
    showMoreInfoBtn,
    handleAddRemoveMoreInfoFields,
    handleSetSchema,
    handleChangeInputAddressSchema,
    handleChangeSelectAddressSchemaAndGetOptions,
    handleChangeSelectAddressSchemaIncExcSwitch,
    handleChangeSelectAddressSchemaAndGetOptionsForMultiple,
    handleSetSchemaValueForUpdate,
    handleReset,
  };
};

export const SchemaTextField = (props) => {
  const {
    bgcolor,
    isRHF = true,
    disabled,
    loading,
    type = inputTypesEnum.SELECT,
    required = true,
    options,
    optionLabel,
    optionValue,
    name,
    value,
    onChange = () => { },
    errors = [],
    label,
    register = () => { },
    multiple,
    height = "38px",
    showMoreInfoBtn,
    btnFlag,
    viewMode,
    handleClickShowMoreInfoBtn = () => { },
    hasIncExcFilter,
    incExcValue,
    onChangeIncExcSwitch = () => { },
  } = props;
  const handleFocus = (event) => event.target.select();
  const LanguageReducer = useLanguageReducer();
  return (
    <>
      <Box className={"flex_between"}>
        <InputLabel required={required} sx={styleSheet.inputLabel}>
          {label}
        </InputLabel>
        {hasIncExcFilter && (
          <SwitchComponent
            size="small"
            r_side="Include"
            checked={incExcValue}
            onChange={onChangeIncExcSwitch}
          />
        )}
      </Box>
      {loading ? (
        <LoadingTextField height={height} />
      ) : getLowerCase(type) === inputTypesEnum.SELECT ? (
        <SelectComponent
          {...register(name, {
            required: {
              value: required,
              message: LanguageReducer?.FIELD_REQUIRED_TEXT,
            },
          })}
          isRHF={isRHF}
          errors={errors}
          height={height}
          required={required}
          name={name}
          options={options}
          optionLabel={optionLabel}
          optionValue={optionValue}
          value={value || null}
          onChange={onChange}
          disabled={disabled}
          multiple={multiple}
        />
      ) : (
        <Box className={"flex_between"} gap={1}>
          <TextField
            sx={{
              "& .MuiInputBase-root": {
                bgcolor: bgcolor,
              },
              "& .MuiInputBase-root input": {
                fontSize: "12px",
              },
            }}
            required={required}
            disabled={disabled}
            placeholder={name}
            onFocus={handleFocus}
            type={type}
            size="small"
            name={name}
            fullWidth
            {...register(name, {
              required: {
                value: required,
                message: LanguageReducer?.FIELD_REQUIRED_TEXT,
              },
            })}
            value={value}
            onChange={onChange}
            variant="outlined"
            error={isRHF ? Boolean(errors[name]) : errors.includes(name)}
            helperText={
              isRHF
                ? errors[name]?.message
                : errors.includes(name) && LanguageReducer?.FIELD_REQUIRED_TEXT
            }
          />
          {!viewMode && showMoreInfoBtn && (
            <ActionButtonCustom
              label={
                btnFlag
                  ? LanguageReducer?.MORE_INFO_MINUS
                  : LanguageReducer?.MORE_INFO_PLUS
              }
              p={0}
              onClick={() => {
                handleClickShowMoreInfoBtn(!btnFlag);
              }}
            />
          )}
        </Box>
      )}
    </>
  );
};
