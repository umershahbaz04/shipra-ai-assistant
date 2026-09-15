import { InputLabel, TextField } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import CountrySchema from "../../../utilities/helpers/countryschema";
import {
  GetCarrierLocationByCarrier,
  UpdateValidatedOrderForCarrier,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions, inputTypesEnum } from "../../../utilities/enum";
import {
  CustomColorLabelledOutline,
  getLowerCase,
  getOptionValueObjectByValue,
  GridContainer,
  GridItem,
  purple,
} from "../../../utilities/helpers/Helpers";
import {
  addressSchemaEnum,
  checkRequiredUsingCarrierAdressSchema,
  SchemaTextField,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import UtilityClass from "../../../utilities/UtilityClass";
import { successNotification } from "../../../utilities/toast";

const UpdateAddressFromAssigntoCarrierModal = (props) => {
  const {
    open,
    onClose,
    rowData,
    CarrierID,
    allCountries,
    getValidatedOrderAddressByActiveCarrierForUpdateData,
  } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    unregister,
    getValues,
    control,
  } = useForm();

  console.log(rowData);

  const {
    selectedAddressSchema,
    addressSchemaSelectData,
    handleSetSchema,
    handleChangeInputAddressSchema,
    handleChangeSelectAddressSchemaAndGetOptions,
  } = useGetAddressSchema(setValue, false, CarrierID);
  const schemaFieldsLength = [...addressSchemaSelectData].length;

  const [CarrierLocation, setCarrierLocation] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const handleValidate = async (data) => {
    const selectedCountry = getValues("country");
    const schema = selectedCountry?.addressingScheme;
    const parsedData = JSON.parse(schema);
    const address = parsedData.keys.reduce((acc, key) => {
      acc[key] = selectedAddressSchema[key];
      return acc;
    }, {});
    const body = {
      orderId: rowData?.orderId,
      streetAddress: data?.streetAddress,
      carrierId: CarrierID,
      address,
    };
    setIsLoading(true);
    try {
      const response = await UpdateValidatedOrderForCarrier(body);
      if (response) {
        successNotification(response?.data?.result?.message);
        getValidatedOrderAddressByActiveCarrierForUpdateData();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      onClose();
      setIsLoading(false);
    }
  };

  const getCarrierLocationByCarrier = async () => {
    try {
      const response = await GetCarrierLocationByCarrier(CarrierID);
      if (response) setCarrierLocation(response?.data?.result);
    } catch (e) {
      console.error(e);
    }
  };

  const defaultCountry = getOptionValueObjectByValue(
    allCountries,
    "countryId",
    rowData?.address?.country
  );

  useEffect(() => {
    handleSetSchema("country", defaultCountry, setValue, unregister, CarrierID);
  }, []);

  useEffect(() => {
    getCarrierLocationByCarrier();
  }, []);

  useEffect(() => {
    setValue("streetAddress", rowData?.address?.streetAddress);
  }, [rowData]);

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title={"Update Address"}
      actionBtn={
        <ModalButtonComponent
          title={"Update Address"}
          bg={purple}
          onClick={handleSubmit(handleValidate)}
          loading={isLoading}
        />
      }
      component={"form"}
    >
      <GridContainer spacing={1}>
        <GridItem xs={12} md={12} lg={12}>
          <CustomColorLabelledOutline label="Address">
            <GridContainer>
              <GridItem md={4} sm={6} xs={12}>
                <InputLabel required sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.ORDERS_COUNTRY}
                </InputLabel>
                <CountrySchema
                  name="country"
                  control={control}
                  isRHF={true}
                  required={true}
                  {...register("country", {
                    required: {
                      value: true,
                    },
                  })}
                  value={getValues("country")}
                  onChange={(event, newValue) => {
                    const resolvedId = newValue ? newValue : null;
                    // setCarrierId(null);
                    handleSetSchema(
                      "country",
                      resolvedId,
                      setValue,
                      unregister
                    );
                  }}
                  errors={errors}
                />
              </GridItem>
              {addressSchemaSelectData?.map((input, index) => {
                return (
                  <GridItem md={4} sm={6} xs={12}>
                    <SchemaTextField
                      loading={input.loading}
                      disabled={input.disabled}
                      isRHF={true}
                      type={input.type}
                      name={input.key}
                      required={checkRequiredUsingCarrierAdressSchema(
                        CarrierLocation,
                        input.key,
                        input.required
                      )}
                      optionLabel={addressSchemaEnum[input.key]?.LABEL}
                      optionValue={addressSchemaEnum[input.key]?.VALUE}
                      register={register}
                      options={input.options}
                      errors={errors}
                      label={input.label}
                      value={getValues(input.key)}
                      onChange={
                        getLowerCase(input.type) === inputTypesEnum.SELECT
                          ? (name, value) => {
                              handleChangeSelectAddressSchemaAndGetOptions(
                                input.key,
                                index,
                                value,
                                setValue,
                                input.key
                              );
                            }
                          : (e) => {
                              handleChangeInputAddressSchema(
                                input.key,
                                e.target.value,
                                setValue
                              );
                            }
                      }
                    />
                  </GridItem>
                );
              })}
              {schemaFieldsLength > 0 && (
                <GridItem md={4} sm={6} xs={12}>
                  <InputLabel required sx={styleSheet.inputLabel}>
                    {"Street Address"}
                  </InputLabel>
                  <TextField
                    type="text"
                    size="small"
                    id="streetAddress"
                    name="streetAddress"
                    fullWidth
                    variant="outlined"
                    {...register("streetAddress", {
                      required: {
                        value: true,
                        message:
                          LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                      },
                      pattern: {
                        value: /^(?!\s*$).+/,
                        message:
                          LanguageReducer?.languageType
                            ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                      },
                    })}
                    error={Boolean(errors.streetAddress)}
                    helperText={errors.streetAddress?.message}
                    placeholder={"Street Address"}
                  />
                </GridItem>
              )}
            </GridContainer>
          </CustomColorLabelledOutline>
        </GridItem>
      </GridContainer>
    </ModalComponent>
  );
};

export default UpdateAddressFromAssigntoCarrierModal;
