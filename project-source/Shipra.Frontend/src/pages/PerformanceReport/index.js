import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import useDateRangeHook from "../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../.reUseableComponents/TextField/SelectComponent";
import { styleSheet } from "../../assets/styles/style";
import { EnumRoutesUrls } from "../../utilities/enum";
import UtilityClass from "../../utilities/UtilityClass";
import CountrySchema from "../../utilities/helpers/countryschema";
import PerformanceReportList from "./list";
import { isShowCountryInTabbarFlag } from "../../utilities/cookies";

const PerformanceReport = () => {
  const isShowCountryInTabbar = isShowCountryInTabbarFlag();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry,
  );
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [selectedRole, setSelectedRole] = useState(null);
  const [selectedCountry, setSelectedCountry] = useState(
    isShowCountryInTabbar ? reduxSelectedCountry : null
  );

  // States to pass down to trigger API call upon "Filter" button click
  const [activeFilters, setActiveFilters] = useState({
    startDateFormated: null,
    endDateFormated: null,
    role: "",
    country: isShowCountryInTabbar
      ? (reduxSelectedCountry?.countryId ||
          reduxSelectedCountry?.id ||
          reduxSelectedCountry ||
          "")
      : "",
  });

  useEffect(() => {
    if (isShowCountryInTabbar) {
      setSelectedCountry(reduxSelectedCountry);
      setActiveFilters((prev) => ({
        ...prev,
        country:
          reduxSelectedCountry?.countryId ||
          reduxSelectedCountry?.id ||
          reduxSelectedCountry ||
          "",
      }));
    }
  }, [reduxSelectedCountry, isShowCountryInTabbar]);

  const handleFilterApply = () => {
    setActiveFilters({
      startDateFormated: startDateFormated || null,
      endDateFormated: endDateFormated || null,
      role: selectedRole?.value || "",
      country:
        selectedCountry?.countryId ||
        selectedCountry?.id ||
        selectedCountry ||
        "",
    });
  };

  const handleFilterClear = () => {
    resetDates();
    setSelectedRole(null);
    setSelectedCountry(null);
    setActiveFilters({
      startDateFormated: null,
      endDateFormated: null,
      role: "",
      country: "",
    });
  };

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Box sx={styleSheet.topNavBar}>
          <Box
            sx={{ ...styleSheet.topNavBarLeft, fontWeight: "900 !important" }}
          ></Box>
          <Stack
            sx={styleSheet.topNavBarRight}
            direction="row"
            justifyContent="flex-end"
            alignItems="center"
            spacing={1}
          ></Stack>
        </Box>
        <DataGridTabs
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
          tabsSmWidth={150}
          tabsMdWidth={150}
          tabData={[
            {
              label: "All",
              route: EnumRoutesUrls.PERFORMANCE_REPORT,
              children: (
                <PerformanceReportList
                  isFilterOpen={isFilterOpen}
                  activeFilters={activeFilters}
                />
              ),
            },
          ]}
          filterData={
            isFilterOpen ? (
              <Table
                sx={{ ...styleSheet.generalFilterArea }}
                size="small"
                aria-label="a dense table"
              >
                <TableHead>
                  <TableRow>
                    <Grid container spacing={2} sx={{ p: "15px 10px" }}>
                      <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                        <Grid>
                          <InputLabel
                            sx={{
                              ...styleSheet.inputLabel,
                              overflow: "unset",
                            }}
                          >
                            {LanguageReducer?.languageType?.START_DATE ||
                              "Start Date"}
                          </InputLabel>
                          <CustomReactDatePickerInputFilter
                            maxDate={UtilityClass.todayDate()}
                            value={startDate}
                            onClick={(date) => setStartDate(date)}
                            size="small"
                            isClearable
                          />
                        </Grid>
                      </Grid>
                      <Grid item md={2} sm={6} xs={12}>
                        <Grid>
                          <InputLabel
                            sx={{
                              ...styleSheet.inputLabel,
                              overflow: "unset",
                            }}
                          >
                            {LanguageReducer?.languageType?.END_DATE ||
                              "End Date"}
                          </InputLabel>
                          <CustomReactDatePickerInputFilter
                            maxDate={UtilityClass.todayDate()}
                            value={endDate}
                            onClick={(date) => setEndDate(date)}
                            size="small"
                            minDate={startDate}
                            disabled={!startDate ? true : false}
                            isClearable
                          />
                        </Grid>
                      </Grid>

                      <Grid item md={2} sm={6} xs={12}>
                        <Grid>
                          <InputLabel
                            sx={{
                              ...styleSheet.inputLabel,
                              overflow: "unset",
                            }}
                          >
                            Role
                          </InputLabel>
                          <SelectComponent
                            height={28}
                            options={[
                              { label: "Driver", value: 1 },
                              { label: "Sale Person", value: 2 },
                            ]}
                            value={selectedRole}
                            optionLabel="label"
                            optionValue="value"
                            addPleaseSelectOptionOnClear={false}
                            onChange={(e, val) => {
                              setSelectedRole(val || null);
                            }}
                          />
                        </Grid>
                      </Grid>

                      <Grid item md={2} sm={6} xs={12}>
                        <Grid>
                          <InputLabel
                            sx={{
                              ...styleSheet.inputLabel,
                              overflow: "unset",
                            }}
                          >
                            Country
                          </InputLabel>
                          <CountrySchema
                            height={28}
                            value={selectedCountry}
                            onChange={(e, val) =>
                              setSelectedCountry(val || null)
                            }
                          />
                        </Grid>
                      </Grid>

                      <Grid item md={2} sm={6} xs={12} alignSelf="end">
                        <Stack
                          direction={"row"}
                          sx={{
                            ...styleSheet.filterButtonMargin,
                            display: "row",
                          }}
                          spacing={1}
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
                            {LanguageReducer?.languageType?.CLEAR_FILTER ||
                              "Clear Filter"}
                          </Button>
                          <Button
                            sx={{
                              ...styleSheet.filterIcon,
                              minWidth: "100px",
                            }}
                            variant="contained"
                            onClick={() => {
                              handleFilterApply();
                            }}
                          >
                            {LanguageReducer?.languageType?.FILTER || "Filter"}
                          </Button>
                        </Stack>
                      </Grid>
                    </Grid>
                  </TableRow>
                </TableHead>
              </Table>
            ) : null
          }
        />
      </div>
    </Box>
  );
};

export default PerformanceReport;
