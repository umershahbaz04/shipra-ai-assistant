import CloseIcon from "@mui/icons-material/Close";
import { LoadingButton } from "@mui/lab";
import {
  Box,
  Grid,
  InputLabel,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import { useEffect, useMemo, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAllCivilEntityType,
  GetAllCountry,
  GetAllDeliveryService,
  GetAllServiceRateGroupForSelection,
  UpsertServiceRateGroup,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import {
  addressSchemaEnum,
  SchemaTextField,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import { successNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import InfoIcon from "@mui/icons-material/Info";
import CreateAbleSelectComponent from "../../../.reUseableComponents/TextField/CreateAbleSelectComponent";
import { findAllByDisplayValue } from "@testing-library/react";

const ORIGIN_TYPE_TO_SCHEMA_KEY = {
  country: "country",
  province: "province",
  city: "city",
  area: "area",
  state: "state",
  pinCode: "pinCode",
};

const CreateServiceRateModal = (props) => {
  let { open, onClose, getAllServiceRateGroup, rowdata } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
    getValues,
    setValue,
    control,
    unregister,
  } = useForm({});
  // ---------------- From Schema ----------------
  const {
    selectedAddressSchemaWithObjValue: selectedAddressSchemaWithObjValueFrom,
    addressSchemaSelectData: addressSchemaSelectDataFrom,
    handleSetSchema: handleSetSchemaFrom,
    handleChangeSelectAddressSchemaAndGetOptionsForMultiple:
      handleChangeSelectAddressSchemaAndGetOptionsFrom,
    handleSetSchemaValueForUpdate: handleSetSchemaValueForUpdateFrom,
    handleReset: handleResetFrom,
  } = useGetAddressSchema(setValue, false, null, "from");

  // ---------------- To Schema ----------------
  const {
    selectedAddressSchemaWithObjValue: selectedAddressSchemaWithObjValueTo,
    addressSchemaSelectData: addressSchemaSelectDataTo,
    handleSetSchema: handleSetSchemaTo,
    handleChangeSelectAddressSchemaAndGetOptionsForMultiple:
      handleChangeSelectAddressSchemaAndGetOptionsTo,
    handleSetSchemaValueForUpdate: handleSetSchemaValueForUpdateTo,
    handleReset: handleResetTo,
  } = useGetAddressSchema(setValue, false, null, "to");
  const selectedOriginType = watch("OriginType")?.typeName;
  const countryFromValue = watch("countryFrom");
  const countryToValue = watch("countryTo");
  const slabsWatch = watch("slabs");
  const fromCountry = watch("country_from");
  const toCountry = watch("country_to");

  useWatch({
    name: "country_from",
    control,
  });
  useWatch({
    name: "country_to",
    control,
  });
  useWatch({
    name: "OriginType",
    control,
  });
  useWatch({
    name: "serviceType",
    control,
  });
  useWatch({
    name: "code",
    control,
  });

  const [isLoading, setisLoading] = useState(false);
  const [rows, setRows] = useState([
    { weightFrom: "", weightTo: "", rate: "", serviceRateGroupSlabId: 0 },
  ]);
  const [originType, setOriginType] = useState([]);
  const [serviceType, setServiceType] = useState([]);
  const [countries, setCountries] = useState([]);
  const [allServiceRateGroup, setAllServiceRateGroup] = useState([]);

  let getAllCountry = async () => {
    if (countries.length === 0) {
      let res = await GetAllCountry({});
      if (res.data.result) {
        const countryResult = res.data.result;
        setCountries(countryResult);
        if (rowdata?.originTypeId === 1) {
          const countryfrom = countryResult.find(
            (dt) => dt.countryId === rowdata.from,
          );
          const countryto = countryResult.find(
            (dt) => dt.countryId === rowdata.to,
          );

          setValue("country_from", countryfrom || null);
          setValue("country_to", countryto || null);
        }
      }
    }
  };

  const getAllServiceRateGroupForSelection = async () => {
    try {
      const response = await GetAllServiceRateGroupForSelection();
      if (response?.data?.isSuccess) {
        setAllServiceRateGroup(response?.data?.result);
        const data = response?.data?.result;
        const defaultCode = data.find((dt) => dt.code === rowdata.code);
        setValue("code", defaultCode || null);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {}
  };

  const handleCreateServiceGroup = (inputValue) => {
    const newOption = {
      id: inputValue,
      code: inputValue,
    };
    setAllServiceRateGroup((prev) => [...prev, newOption]);
    setValue("code", newOption || null);
  };

  const getAllDeliveryService = async () => {
    try {
      const response = await GetAllDeliveryService();
      if (response?.data?.isSuccess) {
        const serviceTypeResult = response?.data?.result;
        setServiceType(serviceTypeResult);
        const defaultServiceType = serviceTypeResult.find(
          (dt) => dt.deliveryServiceId === rowdata.serviceTypeId,
        );

        setValue("serviceType", defaultServiceType || null);
      }
    } catch (e) {}
  };
  const getAllCivilEntityType = async () => {
    try {
      const response = await GetAllCivilEntityType();
      if (response?.data?.isSuccess) {
        const originTypeResult = response?.data?.result;
        setOriginType(originTypeResult);
        const defaultOriginType = originTypeResult.find(
          (dt) => dt.civilEntityTypeId === rowdata.originTypeId,
        );

        setValue("OriginType", defaultOriginType || null);
      }
    } catch (e) {}
  };

  const getAllowedSchemaKeys = (schemaKeys = [], originType) => {
    if (originType === "country") {
      return [];
    }

    const targetKey = ORIGIN_TYPE_TO_SCHEMA_KEY[originType];
    if (!targetKey) return [];
    const index = schemaKeys.indexOf(targetKey);

    if (index !== -1) {
      return schemaKeys.slice(0, index + 1);
    }
    return [];
  };

  const filteredAddressSchemaFrom = useMemo(() => {
    if (
      !fromCountry?.addressingScheme ||
      !selectedOriginType ||
      !addressSchemaSelectDataFrom.length
    ) {
      return [];
    }
    const schemaKeys = JSON.parse(fromCountry.addressingScheme)?.keys || [];
    const allowedKeys = getAllowedSchemaKeys(schemaKeys, selectedOriginType);
    return addressSchemaSelectDataFrom.filter((item) =>
      allowedKeys.includes(item.key),
    );
  }, [fromCountry, selectedOriginType, addressSchemaSelectDataFrom]);

  const filteredAddressSchemaTo = useMemo(() => {
    if (
      !toCountry?.addressingScheme ||
      !selectedOriginType ||
      !addressSchemaSelectDataTo.length
    ) {
      return [];
    }
    const schemaKeys = JSON.parse(toCountry.addressingScheme)?.keys || [];
    const allowedKeys = getAllowedSchemaKeys(schemaKeys, selectedOriginType);
    return addressSchemaSelectDataTo.filter((item) =>
      allowedKeys.includes(item.key),
    );
  }, [toCountry, selectedOriginType, addressSchemaSelectDataTo]);

  const handleChange = (index, field, value) => {
    const updatedRows = [...rows];
    updatedRows[index][field] = value;
    setRows(updatedRows);
  };

  const handleChangeCountryFrom = (name, newValue) => {
    if (selectedOriginType === "country") {
      setValue(name, newValue);
    } else {
      const resolvedId = newValue ? newValue : null;
      handleSetSchemaFrom("country", resolvedId, setValue, unregister);
    }
  };

  const handleChangeCountryTo = (name, newValue) => {
    if (selectedOriginType === "country") {
      setValue(name, newValue);
    } else {
      const resolvedId = newValue ? newValue : null;
      handleSetSchemaTo("country", resolvedId, setValue, unregister);
    }
  };

  const addRow = () => {
    setRows([...rows, { weightFrom: "", weightTo: "", rate: "" }]);
  };

  const deleteRow = (index) => {
    if (rows.length === 1) return;
    setRows(rows.filter((_, i) => i !== index));
  };

  console.log(getValues());

  const hanldeCreateCarrierContract = async (data) => {
    setisLoading(true);
    const originKey = ORIGIN_TYPE_TO_SCHEMA_KEY[data?.OriginType?.typeName];

    const extractIds = (value) => {
      if (!value) return "";
      if (Array.isArray(value)) {
        return value.map((v) => v.id).join(",");
      }
      return value?.id ? String(value.id) : "";
    };

    const extractCountryIds = (countries) => {
      if (!countries) return "";
      if (Array.isArray(countries)) {
        return countries.map((c) => c.countryId).join(",");
      }
      if (typeof countries === "object") {
        return countries.countryId ?? "";
      }
      return "";
    };

    const buildAddressObject = (suffix) => {
      const address = {
        countryId: data[`country_${suffix}`]?.countryId
          ? String(data[`country_${suffix}`].countryId)
          : "",
      };

      Object.values(ORIGIN_TYPE_TO_SCHEMA_KEY).forEach((key) => {
        const field = `${key}_${suffix}`;
        if (data[field]) {
          address[key] = extractIds(data[field]);
        }
      });

      return address;
    };

    const params = {
      ServiceRateGroupId: rowdata?.serviceRateGroupId ?? 0,
      OriginTypeId: String(data?.OriginType?.civilEntityTypeId),

      From:
        data?.OriginType?.civilEntityTypeId === 1
          ? extractCountryIds(data?.country_from)
          : extractIds(data[`${originKey}_from`]),

      To:
        data?.OriginType?.civilEntityTypeId === 1
          ? extractCountryIds(data?.country_to)
          : extractIds(data[`${originKey}_to`]),

      AddressFrom:
        data?.OriginType?.civilEntityTypeId === 1
          ? { countryId: extractCountryIds(data?.country_from) }
          : buildAddressObject("from"),
      AddressTo:
        data?.OriginType?.civilEntityTypeId === 1
          ? { countryId: extractCountryIds(data?.country_to) }
          : buildAddressObject("to"),

      AdditionalRate: String(data.additionalRate),
      ServiceTypeId: String(data?.serviceType?.deliveryServiceId),
      Code: data?.code?.code,
      Unit: String(data.unit),
      CalculationMethodId: "2",

      Slabs: (rows || []).map((slab) => ({
        ServiceRateGroupSlabId: slab?.serviceRateGroupSlabId ?? 0,
        WeightFrom: slab.weightFrom ? Number(slab.weightFrom) : null,
        WeightTo: slab.weightTo ? Number(slab.weightTo) : null,
        Rate: String(slab.rate),
      })),
    };
    try {
      const response = await UpsertServiceRateGroup(params);
      if (response?.data?.isSuccess) {
        successNotification("Service Successfully Created");
        getAllServiceRateGroup();
        onClose();
      }
    } catch (e) {
    } finally {
      setisLoading(false);
    }
  };

  useEffect(() => {
    if (selectedOriginType === "country") {
      addressSchemaSelectDataFrom.forEach((item) => {
        unregister(`${item.key}_from`);
        setValue(`${item.key}_from`, null);
      });

      addressSchemaSelectDataTo.forEach((item) => {
        unregister(`${item.key}_to`);
        setValue(`${item.key}_to`, null);
      });
    }
  }, [selectedOriginType]);

  useEffect(() => {
    if (countryFromValue) {
      setValue("country", countryFromValue, { shouldValidate: true });
    }
    if (countryToValue) {
      setValue("country", countryToValue, { shouldValidate: true });
    }
  }, [countryFromValue, countryToValue]);

  useEffect(() => {
    if (slabsWatch?.length > 1) {
      slabsWatch.forEach((_, index) => {
        setValue(`slabs.${index}.rate`, slabsWatch[index]?.rate, {
          shouldValidate: true,
        });
      });
    }
  }, [slabsWatch]);

  useEffect(() => {
    if (rowdata) {
      setValue("code", rowdata?.code);
      setValue("unit", rowdata?.unit);
      setValue("additionalRate", rowdata?.additionalRate);
    }
    if (rowdata?.slabs?.length) {
      const formattedSlabs = rowdata.slabs.map((slab) => ({
        weightFrom: slab.weightFrom ?? "",
        weightTo: slab.weightTo ?? "",
        rate: slab.rate ?? "",
        serviceRateGroupSlabId: slab?.serviceRateGroupSlabId,
      }));

      setRows(formattedSlabs);

      formattedSlabs.forEach((slab, index) => {
        setValue(`slabs.${index}.weightFrom`, slab.weightFrom);
        setValue(`slabs.${index}.weightTo`, slab.weightTo);
        setValue(`slabs.${index}.rate`, slab.rate);
      });
    } else {
      setRows([{ weightFrom: "", weightTo: "", rate: "" }]);
    }
    if (rowdata?.originTypeId !== 1 && selectedOriginType) {
      handleSetSchemaValueForUpdateFrom(
        rowdata?.entityFromAddress,
        setValue,
        null,
        "country_from",
      );
      handleSetSchemaValueForUpdateTo(
        rowdata?.entityToAddress,
        setValue,
        null,
        "country_to",
      );
    }
  }, [rowdata, selectedOriginType]);

  useEffect(() => {
    getAllDeliveryService();
    getAllCivilEntityType();
    getAllCountry();
    getAllServiceRateGroupForSelection();
  }, []);

  return (
    <div>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="md"
        title={"Service Contract"}
        actionBtn={
          <ModalButtonComponent
            title={"Create Service Contract"}
            loading={isLoading}
            bg={purple}
            onClick={handleSubmit(hanldeCreateCarrierContract)}
          />
        }
        component={"form"}
      >
        <Grid container spacing={2}>
          <Grid item md={12} sm={12} xs={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {"Origin Type"}
            </InputLabel>
            <SelectComponent
              name="OriginType"
              control={control}
              options={originType}
              isRHF={true}
              required={true}
              optionLabel={EnumOptions.ORIGIN_TYPE.LABEL}
              optionValue={EnumOptions.ORIGIN_TYPE.VALUE}
              isRefesh={true}
              disabled={rowdata}
              handleRefreshClick={getAllCivilEntityType}
              {...register("OriginType", {
                required: {
                  value: true,
                },
              })}
              value={getValues("OriginType")}
              onChange={(event, newValue) => {
                const resolvedId = newValue ? newValue : null;
                setValue("OriginType", resolvedId);
              }}
              errors={errors}
            />
          </Grid>
          <Grid item md={6} sm={12} xs={12}>
            <Box
              sx={{
                backgroundColor: "#f8f8f8",
                border: "1px solid #e0e0e0",
                borderTopLeftRadius: "4px",
                borderTopRightRadius: "4px",
                height: "50px",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              <Typography variant="h6" fontWeight={600}>
                From
              </Typography>
            </Box>

            <Box
              sx={{
                backgroundColor: "#fff",
                borderLeft: "1px solid #e0e0e0",
                borderRight: "1px solid #e0e0e0",
                borderBottom: "1px solid #e0e0e0",
                borderBottomLeftRadius: "4px",
                borderBottomRightRadius: "4px",
                padding: "16px",
              }}
            >
              <Grid container>
                <Grid item xs={12} p={1}>
                  <InputLabel required sx={{ ...styleSheet.inputLabel }}>
                    {LanguageReducer?.languageType?.ORDER_COUNTRY}
                  </InputLabel>
                  <CountrySchema
                    name="country_from"
                    isRHF={true}
                    required={true}
                    control={control}
                    multiple={selectedOriginType === "country"}
                    handleRefreshClick={getAllCountry}
                    errors={errors}
                    {...register("country_from", {
                      required: {
                        value: true,
                      },
                    })}
                    value={getValues("country_from")}
                    onChange={(name, newValue) =>
                      handleChangeCountryFrom(name, newValue)
                    }
                  />
                </Grid>
                {filteredAddressSchemaFrom.map((input, index) => (
                  <Grid key={index} item xs={12} p={1}>
                    <SchemaTextField
                      loading={input.loading}
                      disabled={input.disabled}
                      isRHF={true}
                      type={input.type}
                      name={`${input.key}_from`}
                      required={input.required}
                      register={register}
                      multiple={true}
                      optionLabel={addressSchemaEnum[input.key]?.LABEL}
                      optionValue={addressSchemaEnum[input.key]?.VALUE}
                      options={input.options}
                      label={input.label}
                      errors={errors}
                      value={getValues(`${input.key}_from`) || null}
                      onChange={(name, value) => {
                        handleChangeSelectAddressSchemaAndGetOptionsFrom(
                          input.key,
                          index,
                          value,
                          setValue,
                          name,
                        );
                      }}
                    />
                  </Grid>
                ))}
              </Grid>
            </Box>
          </Grid>

          <Grid item md={6} sm={12} xs={12}>
            <Box
              sx={{
                backgroundColor: "#f8f8f8",
                border: "1px solid #e0e0e0",
                borderTopLeftRadius: "4px",
                borderTopRightRadius: "4px",
                height: "50px",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              <Typography variant="h6" fontWeight={600}>
                To
              </Typography>
            </Box>

            <Box
              sx={{
                backgroundColor: "#fff",
                borderLeft: "1px solid #e0e0e0",
                borderRight: "1px solid #e0e0e0",
                borderBottom: "1px solid #e0e0e0",
                borderBottomLeftRadius: "4px",
                borderBottomRightRadius: "4px",
                padding: "16px",
              }}
            >
              <Grid container>
                <Grid item xs={12} p={1}>
                  <InputLabel required sx={{ ...styleSheet.inputLabel }}>
                    {LanguageReducer?.languageType?.ORDER_COUNTRY}
                  </InputLabel>
                  <CountrySchema
                    name="country_to"
                    isRHF={true}
                    required={true}
                    control={control}
                    multiple={selectedOriginType === "country"}
                    {...register("country_to", {
                      required: {
                        value: true,
                      },
                    })}
                    value={getValues("country_to")}
                    errors={errors}
                    onChange={(name, newValue) =>
                      handleChangeCountryTo(name, newValue)
                    }
                  />
                </Grid>
                {filteredAddressSchemaTo.map((input, index) => (
                  <Grid key={index} item xs={12} p={1}>
                    <SchemaTextField
                      loading={input.loading}
                      disabled={input.disabled}
                      isRHF={true}
                      type={input.type}
                      name={`${input.key}_to`}
                      required={input.required}
                      register={register}
                      errors={errors}
                      multiple={true}
                      optionLabel={addressSchemaEnum[input.key]?.LABEL}
                      optionValue={addressSchemaEnum[input.key]?.VALUE}
                      options={input.options}
                      label={input.label}
                      value={getValues(`${input.key}_to`) || null}
                      onChange={(name, value) => {
                        handleChangeSelectAddressSchemaAndGetOptionsTo(
                          input.key,
                          index,
                          value,
                          setValue,
                          name,
                        );
                      }}
                    />
                  </Grid>
                ))}
              </Grid>
            </Box>
          </Grid>
          <Grid item xs={12}>
            <Box
              sx={{
                border: "1px solid #e0e0e0",
                borderRadius: "6px",
                padding: 2,
              }}
            >
              {/* Heading */}
              <Typography variant="h6" sx={{ mb: 2, fontWeight: 600 }}>
                Slab Detail
              </Typography>

              {rows.map((row, index) => (
                <Grid
                  container
                  spacing={2}
                  alignItems="center"
                  key={index}
                  sx={{ mb: 1 }}
                >
                  <Grid item xs={3}>
                    <InputLabel sx={{ ...styleSheet.inputLabel }}>
                      {"Weight From"}
                    </InputLabel>
                    <TextField
                      type="number"
                      fullWidth
                      value={row.weightFrom}
                      {...register(`slabs.${index}.weightFrom`)}
                      onChange={(e) => {
                        handleChange(index, "weightFrom", e.target.value);
                        setValue(`slabs.${index}.weightFrom`, e.target.value);
                      }}
                      error={!!errors?.slabs?.[index]?.weightFrom}
                      sx={{ "& .MuiInputBase-root": { height: "37px" } }}
                    />
                  </Grid>

                  <Grid item xs={3}>
                    <InputLabel sx={{ ...styleSheet.inputLabel }}>
                      {"Weight To"}
                    </InputLabel>
                    <TextField
                      type="number"
                      fullWidth
                      value={row.weightTo}
                      {...register(`slabs.${index}.weightTo`)}
                      onChange={(e) => {
                        handleChange(index, "weightTo", e.target.value);
                        setValue(`slabs.${index}.weightTo`, e.target.value);
                      }}
                      error={!!errors?.slabs?.[index]?.weightTo}
                      sx={{ "& .MuiInputBase-root": { height: "37px" } }}
                    />
                  </Grid>

                  <Grid item xs={3}>
                    <InputLabel required sx={{ ...styleSheet.inputLabel }}>
                      {"Rate"}
                    </InputLabel>
                    <TextField
                      type="number"
                      fullWidth
                      {...register(`slabs.${index}.rate`, {
                        required: "Rate is required",
                      })}
                      sx={{ "& .MuiInputBase-root": { height: "37px" } }}
                      value={row.rate}
                      onChange={(e) => {
                        const value = e.target.value;
                        handleChange(index, "rate", value);
                        setValue(`slabs.${index}.rate`, value, {
                          shouldValidate: true,
                          shouldDirty: true,
                        });
                      }}
                      error={!!errors?.slabs?.[index]?.rate}
                      helperText={errors?.slabs?.[index]?.rate?.message}
                    />
                  </Grid>

                  <Grid
                    item
                    xs={3}
                    sx={{
                      display: "flex",
                      gap: 1,
                      alignSelf: "end",
                    }}
                  >
                    <LoadingButton
                      sx={styleSheet.LoadingButtonDelete}
                      disabled={rows.length === 1}
                      onClick={() => deleteRow(index)}
                      variant="outlined"
                    >
                      <CloseIcon fontSize="10px" />
                    </LoadingButton>
                    {index === rows.length - 1 && (
                      <ButtonComponent title={"+ Add Row"} onClick={addRow} />
                    )}
                  </Grid>
                </Grid>
              ))}
            </Box>
          </Grid>
          <Grid item xs={6}>
            <InputLabel required sx={{ ...styleSheet.inputLabel }}>
              {"Unit"}
            </InputLabel>
            <TextField
              type="number"
              fullWidth
              {...register("unit", {
                required: "Unit is required",
              })}
              defaultValue={0}
              error={!!errors.unit}
              helperText={errors.unit?.message}
              sx={{ "& .MuiInputBase-root": { height: "37px" } }}
            />
          </Grid>
          <Grid item xs={6}>
            <InputLabel required sx={{ ...styleSheet.inputLabel }}>
              {"Additional Rate"}
            </InputLabel>
            <TextField
              type="number"
              fullWidth
              {...register("additionalRate", {
                required: "Additional Rate is required",
              })}
              defaultValue={0}
              sx={{ "& .MuiInputBase-root": { height: "37px" } }}
              error={!!errors.additionalRate}
              helperText={errors.additionalRate?.message}
            />
          </Grid>
          <Grid item xs={6}>
            <InputLabel required sx={{ ...styleSheet.inputLabel }}>
              {"Service Type"}
            </InputLabel>
            <SelectComponent
              name="serviceType"
              control={control}
              options={serviceType}
              isRHF={true}
              required={true}
              optionLabel={EnumOptions.SERVICE_TYPE.LABEL}
              optionValue={EnumOptions.SERVICE_TYPE.VALUE}
              isRefesh={false}
              handleRefreshClick={getAllDeliveryService}
              {...register("serviceType", {
                required: {
                  value: true,
                },
              })}
              value={getValues("serviceType")}
              onChange={(event, newValue) => {
                const resolvedId = newValue ? newValue : null;
                setValue("serviceType", resolvedId);
              }}
              errors={errors}
            />
          </Grid>
          <Grid item xs={6}>
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <InputLabel required sx={{ ...styleSheet.inputLabel }}>
                Code
              </InputLabel>

              <Tooltip
                title="Type a code and press Enter to save it as a record"
                arrow
              >
                <InfoIcon sx={{ fontSize: "20px", color: "red" }} />
              </Tooltip>
            </Box>
            <CreateAbleSelectComponent
              name="code"
              control={control}
              options={allServiceRateGroup}
              isRHF={true}
              required={true}
              optionLabel={"id"}
              optionValue={"code"}
              isMulti={false}
              isRefesh={false}
              {...register("code", {
                required: {
                  value: true,
                },
              })}
              onCreateOption={handleCreateServiceGroup}
              value={getValues("code")}
              onChange={(event, newValue) => {
                setValue("code", newValue || null);
              }}
              errors={errors}
            />
          </Grid>
        </Grid>
      </ModalComponent>
    </div>
  );
};

export default CreateServiceRateModal;
