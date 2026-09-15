import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { GetAllClientRate } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { useGetAllCountries } from "../../../utilities/helpers/Helpers";
import {
  addressSchemaEnum,
  SchemaTextField,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import PriceCalculatorList from "./list";

const PriceCalculator = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { countries } = useGetAllCountries();

  const { setValue, getValues } = useForm();

  // ---------------- From Schema ----------------
  const {
    selectedAddressSchemaWithObjValue: selectedAddressSchemaWithObjValueFrom,
    addressSchemaSelectData: addressSchemaSelectDataFrom,
    handleSetSchema: handleSetSchemaFrom,
    handleChangeSelectAddressSchemaAndGetOptions:
      handleChangeSelectAddressSchemaAndGetOptionsFrom,
    handleReset: handleResetFrom,
  } = useGetAddressSchema(setValue, false, null, "from");

  // ---------------- To Schema ----------------
  const {
    selectedAddressSchemaWithObjValue: selectedAddressSchemaWithObjValueTo,
    addressSchemaSelectData: addressSchemaSelectDataTo,
    handleSetSchema: handleSetSchemaTo,
    handleChangeSelectAddressSchemaAndGetOptions:
      handleChangeSelectAddressSchemaAndGetOptionsTo,
    handleReset: handleResetTo,
  } = useGetAddressSchema(setValue, false, null, "to");

  const [isLoading, setIsLoading] = useState(false);
  const [carrierData, setCarrierData] = useState([]);
  const [carriersCount, setCarriersCount] = useState();
  const [orderNo, setOrderNo] = useState("");
  const [weight, setweight] = useState("1");
  const [priceRange, setPriceRange] = useState({ min: "", max: "" });

  const handleFilterClear = async () => {
    handleResetFrom();
    handleResetTo();
  };

  const handleFilter = () => {
    getAllClientRate();
  };
  const handleChange = (e) => {
    const { name, value } = e.target;
    setPriceRange((prev) => ({ ...prev, [name]: value }));
  };

  const getFilteredData = () => {
    const min = Number(priceRange.min) || 0;
    const max = Number(priceRange.max) || Infinity;

    return carrierData.filter((item) => {
      const rate = Number(item.calculatedRate) || 0;
      return rate >= min && rate <= max;
    });
  };

  const filteredData = getFilteredData();

  const buildDictionary = (schemaObj) => {
    const dict = {};
    Object.entries(schemaObj).forEach(([key, value]) => {
      if (value) {
        if (typeof value === "object" && value.id) {
          dict[key] = value.id;
        } else if (typeof value === "object" && value.countryId) {
          dict["countryId"] = value.countryId;
        } else {
          dict[key] = value;
        }
      }
    });
    return dict;
  };

  const getAllClientRate = async () => {
    const fromDict = buildDictionary(selectedAddressSchemaWithObjValueFrom);
    const toDict = buildDictionary(selectedAddressSchemaWithObjValueTo);
    setIsLoading(true);
    try {
      const response = await GetAllClientRate({
        filter: {
          search: orderNo,
          AddressFrom: fromDict,
          AddressTo: toDict,
          from: 1,
          to: 1,
          CarrierId: 0,
          deliveryTime: "",
          dropOfMethod: "",
          deliveryMethod: "",
          minPrice: 0,
          maxPrice: 0,
          weight: orderNo.length > 0 ? 0 : Number(weight),
        },
      });
      if (response.data.isSuccess) {
        setCarrierData(response?.data?.result?.list);
        setCarriersCount(response?.data?.result?.TotalCount);
      }
    } catch (error) {
      console.error(
        "Error in updating getting these carriers",
        error?.response
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {/* Search input */}
        <Grid
          item
          xl={12}
          lg={12}
          md={12}
          sm={12}
          xs={12}
          p={1}
          sx={{
            background: "#F8F8F8",
            border: "1px solid rgba(0, 0, 0, 0.12)",
            borderRadius: "10px!important",
            borderBottomLeftRadius: "0px !important",
            borderBottomRightRadius: "0px !important",
          }}
        >
          <Table size="small" aria-label="a dense table">
            <TableHead>
              <TableRow>
                <TableCell
                  colSpan={12}
                  sx={{ borderBottom: "none !important" }}
                >
                  <Grid container spacing={2}>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {"Order No."}
                      </InputLabel>
                      <TextField
                        type="text"
                        value={orderNo}
                        onChange={(e) => setOrderNo(e.target.value)}
                        placeholder="Enter order no"
                        variant="outlined"
                        fullWidth
                        sx={{
                          "& .MuiOutlinedInput-root": {
                            height: "28px",
                            fontSize: "14px",
                            borderRadius: "4px",
                            "& fieldset": {
                              borderColor: "hsl(0, 0%, 74.12%)",
                              borderWidth: "1px",
                            },
                            "&:hover fieldset": {
                              borderColor: "hsl(0, 0%, 50%)",
                            },
                            "&.Mui-focused fieldset": {
                              borderColor: "#563Ad5",
                            },
                          },
                          input: {
                            padding: "4px 8px",
                            background: "#fff",
                          },
                        }}
                      />
                    </Grid>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {"Weight"}
                      </InputLabel>
                      <TextField
                        type="number"
                        value={weight}
                        onChange={(e) => setweight(e.target.value)}
                        placeholder="Enter Weight"
                        variant="outlined"
                        fullWidth
                        sx={{
                          "& .MuiOutlinedInput-root": {
                            height: "28px",
                            fontSize: "14px",
                            borderRadius: "4px",
                            "& fieldset": {
                              borderColor: "hsl(0, 0%, 74.12%)",
                              borderWidth: "1px",
                            },
                            "&:hover fieldset": {
                              borderColor: "hsl(0, 0%, 50%)",
                            },
                            "&.Mui-focused fieldset": {
                              borderColor: "#563Ad5",
                            },
                          },
                          input: {
                            padding: "4px 8px",
                            background: "#fff",
                          },
                        }}
                      />
                    </Grid>
                    <Grid item xl={3} lg={3} md={3} sm={6} xs={12}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {"Price"}
                      </InputLabel>
                      <Box
                        sx={{ display: "flex", alignItems: "center", gap: 1 }}
                      >
                        <Box>
                          <TextField
                            fullWidth
                            placeholder="Min"
                            name="min"
                            value={priceRange.min}
                            onChange={handleChange}
                            type="number"
                            sx={{
                              "& .MuiOutlinedInput-root": {
                                height: "28px",
                                fontSize: "14px",
                                borderRadius: "4px",
                                "& fieldset": {
                                  borderColor: "hsl(0, 0%, 74.12%)",
                                  borderWidth: "1px",
                                },
                                "&:hover fieldset": {
                                  borderColor: "hsl(0, 0%, 50%)",
                                },
                                "&.Mui-focused fieldset": {
                                  borderColor: "#563Ad5",
                                },
                              },
                              input: {
                                padding: "4px 8px",
                                background: "#fff",
                              },
                            }}
                          />
                        </Box>
                        <Box>
                          <TextField
                            fullWidth
                            placeholder="Max"
                            name="max"
                            value={priceRange.max}
                            onChange={handleChange}
                            type="number"
                            sx={{
                              "& .MuiOutlinedInput-root": {
                                height: "28px",
                                fontSize: "14px",
                                borderRadius: "4px",
                                "& fieldset": {
                                  borderColor: "hsl(0, 0%, 74.12%)",
                                  borderWidth: "1px",
                                },
                                "&:hover fieldset": {
                                  borderColor: "hsl(0, 0%, 50%)",
                                },
                                "&.Mui-focused fieldset": {
                                  borderColor: "#563Ad5",
                                },
                              },
                              input: {
                                padding: "4px 8px",
                                background: "#fff",
                              },
                            }}
                          />
                        </Box>
                      </Box>
                    </Grid>
                  </Grid>
                </TableCell>
              </TableRow>
              <>
                {/* From Section */}
                <TableRow>
                  <TableCell
                    colSpan={12}
                    sx={{ borderBottom: "none !important" }}
                  >
                    <Grid
                      container
                      sx={{
                        border: "1px solid #ccc",
                        borderRadius: "6px",
                        overflow: "hidden",
                      }}
                    >
                      <Grid
                        item
                        xs={12}
                        sm={3}
                        md={2}
                        sx={{
                          background: "#f8f8f8",
                          borderRight: "1px solid #ccc",
                          display: "flex",
                          alignItems: "center",
                          p: 2,
                        }}
                      >
                        <Typography variant="h5" fontWeight="bold">
                          From
                        </Typography>
                      </Grid>

                      {/* Country */}
                      <Grid item xl={2} lg={2} md={2} sm={6} xs={12} p={1}>
                        <InputLabel sx={{ ...styleSheet.inputLabel }}>
                          {LanguageReducer?.languageType?.ORDER_COUNTRY}
                        </InputLabel>
                        <CountrySchema
                          name="countryFrom"
                          height={28}
                          disabled={orderNo.length > 0}
                          value={selectedAddressSchemaWithObjValueFrom.country}
                          onChange={(name, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            handleSetSchemaFrom(
                              "country",
                              resolvedId,
                              setValue
                            );
                          }}
                        />
                      </Grid>

                      {/* Dynamic Fields */}
                      {addressSchemaSelectDataFrom.map((input, index) => (
                        <Grid
                          key={index}
                          item
                          xl={2}
                          lg={2}
                          md={2}
                          sm={6}
                          xs={12}
                          p={1}
                        >
                          <SchemaTextField
                            loading={input.loading}
                            disabled={input.disabled}
                            // isRHF={false}
                            type={input.type}
                            name={`${input.key}_from`}
                            required={false}
                            height={28}
                            optionLabel={addressSchemaEnum[input.key]?.LABEL}
                            optionValue={addressSchemaEnum[input.key]?.VALUE}
                            options={input.options}
                            label={input.label}
                            value={getValues(`${input.key}_from`) || null}
                            onChange={(name, value) => {
                              handleChangeSelectAddressSchemaAndGetOptionsFrom(
                                input.key,
                                index,
                                value,
                                setValue,
                                name
                              );
                            }}
                          />
                        </Grid>
                      ))}
                    </Grid>
                  </TableCell>
                </TableRow>

                {/* To Section */}
                <TableRow>
                  <TableCell
                    colSpan={12}
                    sx={{ borderBottom: "none !important" }}
                  >
                    <Grid
                      container
                      sx={{
                        border: "1px solid #ccc",
                        borderRadius: "6px",
                        overflow: "hidden",
                      }}
                    >
                      <Grid
                        item
                        xs={12}
                        sm={3}
                        md={2}
                        sx={{
                          background: "#f8f8f8",
                          borderRight: "1px solid #ccc",
                          display: "flex",
                          alignItems: "center",
                          p: 2,
                        }}
                      >
                        <Typography variant="h5" fontWeight="bold">
                          To
                        </Typography>
                      </Grid>

                      {/* Country */}
                      <Grid item xl={2} lg={2} md={2} sm={6} xs={12} p={1}>
                        <InputLabel sx={{ ...styleSheet.inputLabel }}>
                          {LanguageReducer?.languageType?.ORDER_COUNTRY}
                        </InputLabel>
                        <CountrySchema
                          name="countryTo"
                          height={28}
                          disabled={orderNo.length > 0}
                          value={selectedAddressSchemaWithObjValueTo.country}
                          onChange={(name, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            handleSetSchemaTo("country", resolvedId, setValue);
                          }}
                        />
                      </Grid>

                      {/* Dynamic Fields */}
                      {addressSchemaSelectDataTo.map((input, index) => (
                        <Grid
                          key={index}
                          item
                          xl={2}
                          lg={2}
                          md={2}
                          sm={6}
                          xs={12}
                          p={1}
                        >
                          <SchemaTextField
                            loading={input.loading}
                            disabled={input.disabled}
                            isRHF={false}
                            type={input.type}
                            name={`${input.key}_to`}
                            required={false}
                            height={28}
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
                                name
                              );
                            }}
                          />
                        </Grid>
                      ))}
                    </Grid>
                  </TableCell>
                </TableRow>
              </>
              {/* Filter Buttons */}
              <TableRow>
                <TableCell
                  colSpan={12}
                  sx={{ borderBottom: "none !important" }}
                >
                  <Grid container justifyContent="end" pr={0.5}>
                    <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                      <Stack
                        direction="row"
                        spacing={1}
                        justifyContent="flex-end"
                        sx={{ ...styleSheet.filterButtonMargin }}
                      >
                        <Button
                          sx={{
                            ...styleSheet.filterIcon,
                            minWidth: "100px",
                          }}
                          color="inherit"
                          variant="outlined"
                          onClick={handleFilterClear}
                        >
                          {LanguageReducer?.languageType?.ORDER_CLEAR_FILTER}
                        </Button>
                        <Button
                          sx={{
                            ...styleSheet.filterIcon,
                            minWidth: "100px",
                          }}
                          variant="contained"
                          onClick={handleFilter}
                        >
                          {LanguageReducer?.languageType?.ORDERS_FILTER}
                        </Button>
                      </Stack>
                    </Grid>
                  </Grid>
                </TableCell>
              </TableRow>
            </TableHead>
          </Table>
        </Grid>

        {/* Tabs + Filters */}
        <DataGridTabs
          customBorderRadius="0px!impotant"
          tabData={[
            {
              label: LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_ALL,
              route: "/price-calculator",
              children: (
                <PriceCalculatorList
                  carrierData={filteredData}
                  isLoading={isLoading}
                  carriersCount={carriersCount}
                  orderNo={orderNo}
                />
              ),
            },
          ]}
        />
      </div>
    </Box>
  );
};

export default PriceCalculator;
