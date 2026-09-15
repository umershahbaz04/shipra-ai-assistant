import SearchIcon from "@mui/icons-material/Search";
import {
  Box,
  Button,
  Grid,
  InputAdornment,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import {
  GetActiveCarriersForSelection,
  GetAllShipraContractCarrierWithServiceAndLocation,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import { useGetAllCountries } from "../../../utilities/helpers/Helpers";
import {
  addressSchemaEnum,
  SchemaTextField,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import PriceCalculatorList from "./list";
import { warningNotification } from "../../../utilities/toast";
import SearchInputAutoCompleteMultiple from "../../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";

const PriceCalculatorBackUp = () => {
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const {
    loading: countryLoading,
    countries,
    selectedCountryIds,
    setSelectedCountryIds,
  } = useGetAllCountries();
  const {
    selectedAddressSchema,
    selectedAddressSchemaForMutiple,
    selectedAddressSchemaWithObjValue,
    selectedAddressSchemaWithObjValueForMultiple,
    addressSchemaSelectData,
    handleSetSchema,
    handleChangeSelectAddressSchemaAndGetOptionsForMultiple,
    handleReset,
  } = useGetAddressSchema();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [carrierData, setCarrierData] = useState([]);
  const [carriersCount, setCarriersCount] = useState();
  const [inputFields, setInputFields] = useState([]);
  const [allActiveCarriersForSelection, setAllActiveCarriersForSelection] =
    useState([]);
  const [carrierId, setCarrierId] = useState();

  const handleFocus = (event) => event.target.select();

  const handleFilterClear = async () => {
    setIsfilterClear(true);
    resetDates();
    handleReset();
    setCarrierId("");
  };
  const handleFilter = () => {};
  const MAX_TAGS = 200;
  const getAllShipraContractCarrierWithServiceAndLocation = async () => {
    setIsLoading(true);
    try {
      const response = await GetAllShipraContractCarrierWithServiceAndLocation({
        filterModel: {
          createdFrom: null,
          createdTo: null,
          start: 0,
          length: 100,
          search: "",
          sortDir: "asc",
          sortCol: 0,
        },
        countryId: 0,
        deliveryServiceId: 0,
        deliveryTypeId: 0,
      });
      setCarrierData(response?.data?.result?.list);
      setCarriersCount(response?.data?.result?.TotalCount);
    } catch (error) {
      console.error(
        "Error in updating getting these carriers",
        error?.response
      );
    } finally {
      setIsLoading(false);
    }
  };

  const getAllActiveCarriersForSelection = async () => {
    try {
      const response = await GetActiveCarriersForSelection();
      setAllActiveCarriersForSelection(response.data.result);
    } catch (error) {
      console.error("Error fetching GetAllOrderTypeLookup:", error.response);
    }
  };

  useEffect(() => {
    getAllActiveCarriersForSelection();
    getAllShipraContractCarrierWithServiceAndLocation();
  }, []);
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Grid
          sx={{
            background: "#F8F8F8",
            borderRadius: "10px!important",
            border: "1px solid rgba(0, 0, 0, 0.12)",
            borderBottom: "none",
            borderBottomLeftRadius: "0px !important",
            borderBottomRightRadius: "0px !important",
            paddingTop: "6px",
          }}
        >
          <Grid item xl={12} lg={12} md={12} sm={12} xs={12}>
            <SearchInputAutoCompleteMultiple
              handleFocus={handleFocus}
              placeholder="Enter Order No."
              onChange={(e, value) => {
                if (value.length <= MAX_TAGS) {
                  setInputFields(value.slice(0, MAX_TAGS));
                } else {
                  warningNotification(
                    LanguageReducer?.languageType?.MAXMIUM_NUMBER_REACHED
                  );
                }
              }}
              inputFields={inputFields}
              MAX_TAGS={MAX_TAGS}
            />
          </Grid>
        </Grid>
        <DataGridTabs
          customBorderRadius="0px!impotant"
          filterData={
            isFilterOpen ? (
              <Table
                sx={{ ...styleSheet.generalFilterArea }}
                size="small"
                aria-label="a dense table"
              >
                <TableHead>
                  <TableRow>
                    <Grid container spacing={2} sx={{ p: "15px" }}>
                      <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                        <Grid>
                          <InputLabel
                            sx={{
                              ...styleSheet.inputLabel,
                              overflow: "unset",
                            }}
                          >
                            {LanguageReducer?.languageType?.ORDER_START_DATE}
                          </InputLabel>
                          <CustomReactDatePickerInputFilter
                            value={startDate}
                            onClick={(date) => setStartDate(date)}
                            size="small"
                            isClearable
                            maxDate={UtilityClass.todayDate()}
                          />
                        </Grid>
                      </Grid>
                      <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                        <Grid>
                          <InputLabel
                            sx={{
                              ...styleSheet.inputLabel,
                              overflow: "unset",
                            }}
                          >
                            {LanguageReducer?.languageType?.ORDER_END_DATE}
                          </InputLabel>
                          <CustomReactDatePickerInputFilter
                            value={endDate}
                            onClick={(date) => setEndDate(date)}
                            size="small"
                            minDate={startDate}
                            disabled={!startDate ? true : false}
                            isClearable
                            maxDate={UtilityClass.todayDate()}
                          />
                        </Grid>
                      </Grid>
                      <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {LanguageReducer?.languageType?.ORDER_CARRIER}
                        </InputLabel>
                        <SelectComponent
                          name="reason"
                          height={28}
                          options={allActiveCarriersForSelection}
                          value={carrierId}
                          optionLabel={EnumOptions.CARRIER.LABEL}
                          optionValue={EnumOptions.CARRIER.VALUE}
                          getOptionLabel={(option) => option.Name}
                          onChange={(e, val) => {
                            setCarrierId(val);
                          }}
                        />
                      </Grid>
                      <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {LanguageReducer?.languageType?.ORDER_COUNTRY}
                        </InputLabel>
                        <CountrySchema
                          name="country"
                          required={true}
                          height={28}
                          value={selectedAddressSchemaWithObjValue.country}
                          onChange={(name, newValue) => {
                            const resolvedId = newValue ? newValue : null;
                            handleSetSchema("country", resolvedId);
                          }}
                        />
                      </Grid>

                      {[...addressSchemaSelectData].map((input, index) => (
                        <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                          <SchemaTextField
                            loading={input.loading}
                            disabled={input.disabled}
                            multiple={true}
                            height={28}
                            type={input.type}
                            name={input.key}
                            required={false}
                            optionLabel={addressSchemaEnum[input.key]?.LABEL}
                            optionValue={addressSchemaEnum[input.key]?.VALUE}
                            options={input.options}
                            label={input.label}
                            value={
                              selectedAddressSchemaWithObjValueForMultiple[
                                input.key
                              ]
                            }
                            onChange={(name, value) => {
                              handleChangeSelectAddressSchemaAndGetOptionsForMultiple(
                                input.key,
                                index,
                                value,
                                () => {},
                                input.key
                              );
                            }}
                          />
                        </Grid>
                      ))}
                      <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                        <Stack
                          alignItems="flex-end"
                          direction="row"
                          spacing={1}
                          sx={{
                            ...styleSheet.filterButtonMargin,
                            height: "100%",
                          }}
                        >
                          <Button
                            sx={{
                              ...styleSheet.filterIcon,
                              minWidth: "100px",
                            }}
                            color="inherit"
                            variant="outlined"
                            onClick={() => {
                              handleFilterClear();
                            }}
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
                  </TableRow>
                </TableHead>
              </Table>
            ) : null
          }
          tabData={[
            {
              label: LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_ALL,
              route: "/price-calculator",
              children: (
                <PriceCalculatorList
                  carrierData={carrierData}
                  isLoading={isLoading}
                  isFilterOpen={isFilterOpen}
                  carriersCount={carriersCount}
                  searchText={inputFields}
                />
              ),
            },
          ]}
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
        />
      </div>
    </Box>
  );
};

export default PriceCalculatorBackUp;
