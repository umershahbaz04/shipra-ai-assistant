import DeleteIcon from "@mui/icons-material/Delete";
import {
  Autocomplete,
  Box,
  Button,
  Chip,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Divider,
  Grid,
  IconButton,
  InputLabel,
  MenuItem,
  Paper,
  Slide,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  TextField,
} from "@mui/material";
import React, { Fragment, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import {
  AddProductOption,
  CheckUniqueProductStockSkus,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { ActionButtonDelete, purple } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function CreateVariantModal({
  open,
  handleClose,
  inputFields,
  setInputFields,
  allProductOptions,
  productId,
  getProductData,
  getProductOptionNameById,
  productStations,
}) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);
  const [selectedOptions, setSelectedOptions] = useState(null);
  const [inputFieldsData, setInputFieldsData] = useState(null);
  const [variantText, setVariantText] = useState("");
  const [addOptions, setAddOptions] = useState([]);
  const [productOptions, setProductOptions] = useState([]);
    const [checkError, setCheckError] = useState(false);
  const [selectedStations, setSelectedStations] = useState([]);
  const [stationQuantities, setStationQuantities] = useState({});

  useEffect(() => {
    if (productStations && productStations.length > 0) {
      let initial = {};
      productStations.forEach((s) => {
        if (s.productStationId > 0) {
          initial[s.productStationId] = { quantity: 0, lowQuantity: 0 };
        }
      });
      setStationQuantities(initial);
      setSelectedStations([]);
    }
  }, [productStations, open]);

  const setGridLength = inputFieldsData?.length + addOptions?.length >= 3;

  const handleDismiss = () => {
    setVariantText("");
    setAddOptions(null);
    reset();
    handleClose();
  };
  const createCombination = () => {
    let text = "";

    selectedOptions.forEach((option, index) => {
      if (option.options.length > 0) {
        text += option.options;

        if (index !== selectedOptions.length - 1) {
          text += "/";
        }
      }
    });
    console.log("add options here", addOptions);
    if (addOptions !== null && addOptions !== undefined) {
      if (addOptions.length > 0) {
        if (text.length > 0) {
          text += "/";
        }

        text += addOptions[0].options;
      }
    }
    setVariantText(text);
  };
  const addInputField = () => {
    if (addOptions !== null && addOptions !== undefined) {
      if (inputFieldsData?.length + addOptions?.length >= 3) {
        errorNotification(
          LanguageReducer?.languageType?.CANNOT_ADD_MORE_OPTIONS_TEXT
        );
        return;
      }
      setAddOptions([
        ...addOptions,
        {
          selectedOption: "",
          options: [],
        },
      ]);
    } else {
      setAddOptions([{ selectedOption: "", options: [] }]);
    }
  };
  const removeInputFields = (index) => {
    const rows = [...addOptions];
    rows.splice(index, 1);
    setAddOptions(rows);
  };
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    watch,
    reset,
    control,
  } = useForm();
  const handleCreateVariant = async (data) => {
    setIsLoading(true);
    let res = await CheckUniqueProductStockSkus(data.sku);
    if (res?.data?.result?.isExist) {
      errorNotification(
        LanguageReducer?.languageType?.SKU_NOT_AVAILBE_EMPTY_TOAST
      );
      setIsLoading(false);
      return;
    }
    let selectedProductOptions = selectedOptions.map((option, index) => ({
      productOptionsId: "",
      optionId: option.selectedOption,
      optionValue: option.options,
      displayOrder: index,
      isDeleted: false,
    }));
    let productOptions = [...selectedProductOptions];
    if (addOptions !== null && addOptions !== undefined) {
      let addProductOptions = addOptions.map((option, index) => ({
        productOptionsId: "",
        optionId: option.selectedOption,
        optionValue: option.options,
        displayOrder: index,
        isDeleted: false,
      }));
      productOptions = [...productOptions, ...addProductOptions];
    }
    setProductOptions(productOptions);
    const stationQuantitiesPayload = selectedStations.map((st) => ({
      stationId: st.productStationId,
      quantity: parseInt(stationQuantities[st.productStationId]?.quantity) || 0,
      lowQuantityLimit: parseInt(stationQuantities[st.productStationId]?.lowQuantity) || 0,
    }));

    let body = {
      productStock: {
        productId: productId,
        sku: data.sku,
        price: parseFloat(data.price),
        quantityAvailable: 0,
        lowQuantityLimit: stationQuantitiesPayload[0]?.lowQuantityLimit || 0,
        productStationId: stationQuantitiesPayload[0]?.stationId || null,
        productStationIds: selectedStations.map((s) => s.productStationId),
        stationQuantities: stationQuantitiesPayload,
        varientOption: variantText,
        productStockStatusId: 0,
        active: true,
        productOptions: productOptions,
      },
    };
    console.log("body data", body);
    try {
      const response = await AddProductOption(body);
      console.log("Add Product Option Response", response);
      successNotification(
        LanguageReducer?.languageType?.VARIANT_ADDED_SUCCESSFULLY_TOAST
      );
      getProductData();
      setVariantText("");
      setAddOptions(null);
      reset();
      handleDismiss();
    } catch (error) {
      console.error("Error in updating quantity:", error.response);
      errorNotification(
        LanguageReducer?.languageType?.UNABLE_TO_ADD_VARIANT_TOAST
      );
    } finally {
      setIsLoading(false);
    }
  };
  const checkNameExists = (index, name) => {
    const options = inputFieldsData[index].options;
    for (let i = 0; i < options.length; i++) {
      if (options[i] === name) {
        return true;
      }
    }

    return false;
  };
  const handleChangeSelectedOptions = (e, index) => {
    if (checkNameExists(index, e.target.value)) {
      errorNotification(LanguageReducer?.languageType?.NAME_ALREADY_EXIST_TEXT);
    }

    const list = [...selectedOptions];
    list[index].options = e.target.value;
    setSelectedOptions(list);
    createCombination();
  };
  const [disabledSelectedOption, setDisabledSelectedOption] = useState();
  const handleDisabledSelectedOption = (index) => {
    setDisabledSelectedOption((prevStates) => ({
      ...prevStates,
      [index]: true,
    }));
  };
  const handleChangeNewOptions = (e, index) => {
    if (!checkNameExists(index, e.target.value)) {
      const list = [...addOptions];
      list[index].options = e.target.value;
      createCombination();
      setAddOptions(list);

      //set disabled if option
      if (e.target.value != "") {
        handleDisabledSelectedOption(index);
      } else {
        delete disabledSelectedOption[index];
      }
    } else {
      errorNotification(LanguageReducer?.languageType?.NAME_ALREADY_EXIST_TEXT);
    }
  };
  const handleChange = (index, evnt) => {
    const { name, value } = evnt.target;
    const list = [...addOptions];
    list[index]["selectedOption"] = value;
    setAddOptions(list);
  };
  useEffect(() => {
    const selectedOptions = JSON.parse(
      JSON.stringify(inputFields, (key, value) => {
        if (key === "options") {
          return [];
        }
        return value;
      })
    );

    setSelectedOptions(selectedOptions);
    setInputFieldsData(JSON.parse(JSON.stringify(inputFields)));
  }, [inputFields]);

  return (
    <ModalComponent
      open={open}
      onClose={handleDismiss}
      maxWidth="md"
      title={LanguageReducer?.languageType?.CREATE_VARIANT_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={"Create variant"}
          loading={isLoading}
          bg={purple}
          type="submit"
        />
      }
      component={"form"}
      onSubmit={handleSubmit(handleCreateVariant)}
    >
      {inputFieldsData?.map((data, index) => {
        {
        }
        let disabled = false;

        if (selectedOptions.length > 0) {
          if (
            selectedOptions.find(
              (x) => x.selectedOption == data?.selectedOption
            )
          ) {
            disabled = true;
          }
        }
        return (
          <Fragment key={index}>
            <Grid container spacing={2}>
              <Grid
                item
                sm={setGridLength ? 5 : 6}
                xs={6}
                sx={{ textAlign: "center" }}
              >
                <InputLabel sx={styleSheet.inputLabel}>
                  {LanguageReducer?.languageType?.OPTION_NAME_TEXT}
                </InputLabel>
                <TextField
                  style={{ marginTop: "4px" }}
                  defaultValue={data.selectedOption}
                  select
                  size="small"
                  id={`optionName[${index}]`}
                  name={`optionName[${index}]`}
                  fullWidth
                  variant="outlined"
                  value={data.selectedOption}
                  disabled
                >
                  <MenuItem
                    value={data.selectedOption}
                    id={`optionName[${index}]`}
                    name={`optionName[${index}]`}
                    disabled
                  >
                    {getProductOptionNameById(data.selectedOption)}
                  </MenuItem>
                </TextField>
              </Grid>
              <Grid item xs={6} sm={setGridLength ? 5 : 6}>
                <InputLabel sx={styleSheet.inputLabel}>{"Name"}</InputLabel>
                <TextField
                  style={{ marginTop: "4px" }}
                  type="text"
                  id="selectTag"
                  name="selectTag"
                  onChange={(e) => handleChangeSelectedOptions(e, index)}
                  required
                  variant="outlined"
                  size="small"
                  fullWidth
                />
              </Grid>
            </Grid>
          </Fragment>
        );
      })}

      {addOptions?.map((data, index) => {
        return (
          <Fragment key={index}>
            <Grid container spacing={2}>
              <Grid item xs={5} sx={{ textAlign: "center" }}>
                <InputLabel sx={{ ...styleSheet.inputLabel }}>
                  {LanguageReducer?.languageType?.OPTION_NAME_TEXT}
                </InputLabel>
                <TextField
                  disabled={
                    disabledSelectedOption && disabledSelectedOption[index]
                  }
                  onChange={(evnt) => handleChange(index, evnt)}
                  defaultValue=""
                  select
                  size="small"
                  required
                  fullWidth
                  variant="outlined"
                >
                  <MenuItem value="" disabled></MenuItem>
                  {allProductOptions.map((allProductOption) => {
                    let disabled = false;

                    if (inputFields.length > 0) {
                      if (
                        inputFields.find(
                          (x) => x.selectedOption == allProductOption?.id
                        )
                      ) {
                        disabled = true;
                      }
                    }

                    return (
                      <MenuItem
                        key={allProductOption.id}
                        value={allProductOption.id}
                        disabled={disabled}
                      >
                        {allProductOption.text}
                      </MenuItem>
                    );
                  })}
                </TextField>
              </Grid>
              <Grid item xs={5}>
                <InputLabel sx={{ ...styleSheet.inputLabel }}>
                  {"Name"}
                </InputLabel>
                <TextField
                  type="text"
                  required
                  name={`name[${index}]`}
                  onChange={(e) => handleChangeNewOptions(e, index)}
                  id={`name[${index}]`}
                  variant="outlined"
                  size="small"
                  fullWidth
                  error={Boolean(errors.name?.[index])}
                  helperText={errors.name?.[index]?.message}
                />
              </Grid>
              <Grid item xs={2} alignSelf={"center"} marginTop={"20px"}>
                {addOptions.length !== 0 && (
                  <ActionButtonDelete
                    label=""
                    onClick={() => removeInputFields(index)}
                  />
                )}
              </Grid>
            </Grid>
          </Fragment>
        );
      })}
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item xs={12}>
          <Button
            sx={{
              ...styleSheet.modalCarrierSubmitButton,
            }}
            fullWidth
            variant="contained"
            onClick={addInputField}
          >
            + {LanguageReducer?.languageType?.ADD_OPTION_TEXT}
          </Button>
        </Grid>
        <Grid item sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.OPTION_NAME_TEXT}
          </InputLabel>
          <TextField
            value={variantText}
            type="text"
            name="option"
            id="option"
            variant="outlined"
            size="small"
            fullWidth
          />
        </Grid>
        <Grid item sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.PRICE_TEXT}
          </InputLabel>
          <TextField
            {...register("price", {
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
            })}
            error={Boolean(errors.price)}
            helperText={errors.price?.message}
            type="number"
            name="price"
            id="price"
            variant="outlined"
            size="small"
            fullWidth
          />
        </Grid>
        <Grid item sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.ENTER_SKU_TEXT}
          </InputLabel>
          <TextField
            {...register("sku", {
              required: {
                value: true,
                message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
              },
            })}
            error={Boolean(errors.sku)}
            helperText={errors.sku?.message}
            type="text"
            name="sku"
            id="sku"
            variant="outlined"
            size="small"
            fullWidth
          />
        </Grid>
        <Grid item sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            Select Stations
          </InputLabel>
          <Autocomplete
            multiple
            size="small"
            id="create-variant-stations-autocomplete"
            options={productStations?.filter((x) => x.productStationId > 0) || []}
            getOptionLabel={(option) => option.sname || ""}
            isOptionEqualToValue={(option, value) =>
              option.productStationId === value.productStationId
            }
            value={selectedStations}
            onChange={(event, newValue) => {
              setSelectedStations(newValue);
            }}
            renderTags={(tagValue, getTagProps) =>
              tagValue.map((option, index) => (
                <Chip
                  label={option.sname}
                  size="small"
                  {...getTagProps({ index })}
                />
              ))
            }
            renderInput={(params) => (
              <TextField
                {...params}
                variant="outlined"
                size="small"
                placeholder={
                  selectedStations.length === 0 ? "Select stations..." : ""
                }
              />
            )}
          />
        </Grid>
        
        {selectedStations.length > 0 && (
          <Grid item sm={12}>
            <TableContainer component={Paper} sx={{ maxHeight: 200, mt: 1 }}>
              <Table size="small" aria-label="variant stations table">
                <TableBody>
                  {selectedStations.map((station) => (
                    <TableRow key={station.productStationId}>
                      <TableCell component="th" scope="row" sx={{ fontWeight: 500, fontSize: "14px", width: "35%" }}>
                        {station.sname}
                      </TableCell>
                      <TableCell align="right">
                        <Box display={"flex"} gap={1}>
                          <TextField
                            type="number"
                            size="small"
                            label="Quantity"
                            value={stationQuantities[station.productStationId]?.quantity ?? 0}
                            onChange={(e) => {
                              const val = parseInt(e.target.value) || 0;
                              setStationQuantities((prev) => ({
                                ...prev,
                                [station.productStationId]: {
                                  ...prev[station.productStationId],
                                  quantity: val,
                                },
                              }));
                            }}
                            inputProps={{ min: 0 }}
                            fullWidth
                          />
                          <TextField
                            type="number"
                            size="small"
                            label="Low Quantity Amount"
                            value={stationQuantities[station.productStationId]?.lowQuantity ?? 0}
                            onChange={(e) => {
                              const val = parseInt(e.target.value) || 0;
                              setStationQuantities((prev) => ({
                                ...prev,
                                [station.productStationId]: {
                                  ...prev[station.productStationId],
                                  lowQuantity: val,
                                },
                              }));
                            }}
                            inputProps={{ min: 0 }}
                            fullWidth
                          />
                        </Box>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Grid>
        )}


      </Grid>
    </ModalComponent>
  );
}

export default CreateVariantModal;
