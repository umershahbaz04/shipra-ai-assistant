import SearchIcon from "@mui/icons-material/Search";
import {
  Box,
  Button,
  Grid,
  InputAdornment,
  InputLabel,
  Stack,
  Table,
  TableRow,
} from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import {
  GetAllActivedCarrier,
  GetAllCarrierWithServiceAndLocation,
  GetAllCountry,
  GetAllDeliveryService,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { isShowCountryInTabbarFlag } from "../../../utilities/cookies";
import {
  EnumChangeFilterModelApiUrls,
  EnumOptions,
  viewTypesEnum,
} from "../../../utilities/enum";
import CountrySchema from "../../../utilities/helpers/countryschema";
import { ToggleButtonComponent } from "../../../utilities/helpers/Helpers";
import CarriersList from "./carriersList";
export const EnumCarrierFilter = Object.freeze({
  All: "/carriers",
  Active: "/carriers/active",
  InActive: "/carriers/inActive",
});

function CarriersPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [carrierData, setCarrierData] = useState([]);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isRefreshRequired, setIsRefreshRequired] = useState(false);
  const [allCountries, setAllCountries] = useState([]);
  const [selectedCountry, setSelectedCountry] = useState("");
  const [AllDeliveryService, setAllDeliveryService] = useState();
  const [SelectedServices, setSelectedServices] = useState(null);
  const [searchText, setSearchText] = useState("");
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [viewMode, setViewMode] = useState(viewTypesEnum.GRID);
  const [activeUrl, setActiveUrl] = useState(EnumCarrierFilter.Active);
  const [isLoad, setIsLoad] = useState(false);
  const [carriersCount, setCarriersCount] = useState(0);
  const [activeCarriersCount, setActiveCarriersCount] = useState(0);
  const navigate = useNavigate();
  const { control } = useForm();

  const getAllActivedCarrier = async (countryId, deliveryServiceId) => {
    setIsLoad(true);
    setCarrierData([]);
    try {
      const response = await GetAllActivedCarrier({
        filterModel: {
          start: 0,
          length: EnumChangeFilterModelApiUrls.GET_ALL_CARRIER.length,
          search: searchText,
          sortDir: "desc",
          sortCol: 0,
        },
        countryId:
          countryId === 0 || countryId
            ? countryId
            : selectedCountry?.countryId || 0,
        deliveryServiceId:
          deliveryServiceId === 0 || deliveryServiceId
            ? deliveryServiceId
            : SelectedServices?.deliveryServiceId || 0,
      });
      if (window.location.pathname === EnumCarrierFilter.Active) {
        const filterData = response?.data?.result?.list.filter(
          (dt) => dt.Active === true,
        );
        const filterDatawithIndex = filterData.map((dt, index) => ({
          ...dt,
          index,
        }));
        setCarrierData(filterDatawithIndex);
        setActiveCarriersCount(filterDatawithIndex.length);
      } else if (window.location.pathname === EnumCarrierFilter.InActive) {
        const filterData = response?.data?.result?.list.filter(
          (dt) => dt.Active !== true,
        );
        const filterDatawithIndex = filterData.map((dt, index) => ({
          ...dt,
          index,
        }));
        setCarrierData(filterDatawithIndex);
        setActiveCarriersCount(filterDatawithIndex.length);
      }
    } catch (error) {
      console.error("Error in updating getting these carriers", error.response);
    } finally {
      setIsLoad(false);
      setIsRefreshRequired(false);
    }
  };

  const getAllCarriersData = async (countryId, deliveryServiceId) => {
    setIsLoad(true);
    try {
      const response = await GetAllCarrierWithServiceAndLocation({
        filterModel: {
          start: 0,
          length: EnumChangeFilterModelApiUrls.GET_ALL_CARRIER.length,
          search: searchText,
          sortDir: "desc",
          sortCol: 0,
        },
        countryId:
          countryId === 0 || countryId
            ? countryId
            : selectedCountry?.countryId || 0,
        deliveryServiceId:
          deliveryServiceId === 0 || deliveryServiceId
            ? deliveryServiceId
            : SelectedServices?.deliveryServiceId || 0,
      });
      setCarrierData(
        response?.data?.result?.list?.map((dt, index) => ({ ...dt, index })),
      );
      setCarriersCount(response?.data?.result?.TotalCount);
    } catch (error) {
      console.error("Error in updating getting these carriers", error.response);
    } finally {
      setIsRefreshRequired(false);
      setIsLoad(false);
    }
  };

  const handleTabChange = async (event, filterValue) => {
    if (filterValue === EnumCarrierFilter.All) {
      setActiveUrl("");
      await getAllCarriersData();
    } else if (filterValue === EnumCarrierFilter.Active) {
      setActiveUrl(EnumCarrierFilter.Active);
      await getAllActivedCarrier();
    } else if (filterValue === EnumCarrierFilter.InActive) {
      setActiveUrl(EnumCarrierFilter.InActive);
      await getAllActivedCarrier();
    }
  };

  let getAllCountry = async () => {
    let res = await GetAllCountry({});
    if (res.data.result != null) setAllCountries(res.data.result);
  };

  let getAllDeliveryService = async () => {
    let res = await GetAllDeliveryService({});
    if (res.data.result != null) setAllDeliveryService(res.data.result);
  };

  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry,
  );
  const isInitialCountryMountCarriers = useRef(true);

  useEffect(() => {
    if (!isShowCountryInTabbarFlag()) return;
    if (isInitialCountryMountCarriers.current) {
      isInitialCountryMountCarriers.current = false;
      return;
    }
    setSelectedCountry(reduxSelectedCountry || null);
    const cId = reduxSelectedCountry
      ? (reduxSelectedCountry?.countryId || reduxSelectedCountry?.id)
      : null;
    if (window.location.pathname === EnumCarrierFilter.Active) {
      getAllActivedCarrier(cId, null);
    } else {
      getAllCarriersData(cId, null);
    }
  }, [reduxSelectedCountry]);

  const handleCountryChange = async (event, _selectedCountry) => {
    setSelectedCountry(_selectedCountry);
    if (window.location.pathname === EnumCarrierFilter.Active) {
      await getAllActivedCarrier(_selectedCountry?.countryId, null);
    } else {
      await getAllCarriersData(_selectedCountry?.countryId, null);
    }
  };

  const handleServicesChange = async (event, _seletedService) => {
    setSelectedServices(_seletedService);
    if (window.location.pathname === EnumCarrierFilter.Active) {
      await getAllActivedCarrier(null, _seletedService?.deliveryServiceId);
    } else {
      await getAllCarriersData(null, _seletedService?.deliveryServiceId);
    }
  };

  const handleChangeSearch = (event) => {
    setSearchText(event.target.value);
  };

  const searchBtn = async (event, _seletedService) => {
    setSelectedServices(_seletedService);
    if (
      window.location.pathname === EnumCarrierFilter.Active ||
      window.location.pathname === EnumCarrierFilter.InActive
    ) {
      await getAllActivedCarrier(null, _seletedService?.deliveryServiceId);
    } else {
      await getAllCarriersData(null, _seletedService?.deliveryServiceId);
    }
  };

  const handle_Enter = async (e, _seletedService) => {
    if (e.key === "Enter") {
      setSelectedServices(_seletedService);
      if (
        window.location.pathname === EnumCarrierFilter.Active ||
        window.location.pathname === EnumCarrierFilter.InActive
      ) {
        await getAllActivedCarrier(null, _seletedService?.deliveryServiceId);
      } else {
        await getAllCarriersData(null, _seletedService?.deliveryServiceId);
      }
    }
  };
  const handleFilterRest = () => {
    setSelectedCountry(null);
    setSelectedServices(null);
    setSearchText("");
  };
  const handleFilterClear = () => {
    handleFilterRest();
    setIsfilterClear(true);
  };
  useEffect(() => {
    if (isfilterClear) {
      if (
        window.location.pathname === EnumCarrierFilter.Active ||
        window.location.pathname === EnumCarrierFilter.InActive
      ) {
        getAllActivedCarrier();
      } else {
        getAllCarriersData();
      }
      setIsfilterClear(false);
    }
  }, [isfilterClear]);

  useEffect(() => {
    if (isRefreshRequired) {
      getAllActivedCarrier();
    }
  }, [isRefreshRequired]);

  useEffect(() => {
    getAllCountry();
    getAllDeliveryService();
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => {
      if (
        window.location.pathname === EnumCarrierFilter.Active ||
        window.location.pathname === EnumCarrierFilter.InActive
      ) {
        getAllActivedCarrier();
      } else {
        getAllCarriersData();
      }
    }, 1000);

    return () => {
      clearTimeout(timer);
    };
  }, [searchText]);

  useEffect(() => {
    if (activeUrl === EnumCarrierFilter.Active) {
      navigate(EnumCarrierFilter.Active);
      getAllActivedCarrier();
    } else if (activeUrl === EnumCarrierFilter.InActive) {
      navigate(EnumCarrierFilter.InActive);
      getAllActivedCarrier();
    } else {
      navigate(EnumCarrierFilter.All);
      getAllCarriersData();
    }
  }, [activeUrl, navigate]);
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Grid container>
          <Grid
            item
            xl={12}
            lg={12}
            md={12}
            sm={12}
            xs={12}
            p={1}
            sx={{
              background: "#f8f8f8",
              border: "1px solid rgba(0, 0, 0, 0.12)",
              borderRadius: "10px!important",
              borderBottomLeftRadius: "0px !important",
              borderBottomRightRadius: "0px !important",
            }}
          >
            <TextFieldComponent
              sx={{
                "& fieldset": { border: "none" },
                borderRadius: "5px",
                background: "#fff",
                boxShadow: 1,
              }}
              placeholder="Search Carrier"
              name="search"
              value={searchText}
              onChange={handleChangeSearch}
              onKeyDown={handle_Enter}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <SearchIcon
                      onClick={searchBtn}
                      sx={styleSheet.searchIcon}
                    />
                  </InputAdornment>
                ),
              }}
            />
          </Grid>
          <Grid item xl={12} lg={12} md={12} sm={12} xs={12}>
            <DataGridTabs
              customBorderRadius="0px!impotant"
              handleTabChange={handleTabChange}
              tabData={[
                {
                  label:
                    LanguageReducer?.languageType
                      ?.INTEGRATION_CARRIER_ALL_CARRIERS,
                  route: EnumCarrierFilter.All,
                },
                {
                  label:
                    LanguageReducer?.languageType?.INTEGRATION_CARRIER_ACTIVE,
                  route: EnumCarrierFilter.Active,
                  tabFilter: EnumCarrierFilter.Active,
                },
                {
                  label:
                    LanguageReducer?.languageType
                      ?.INTEGRATION_CARRIER_IN_ACTIVE,
                  route: EnumCarrierFilter.InActive,
                  tabFilter: EnumCarrierFilter.InActive,
                },
              ]}
              handleFilterBtnOnClick={() => {
                setIsFilterOpen(!isFilterOpen);
              }}
              otherBtns={
                <>
                  <ToggleButtonComponent
                    value={viewMode}
                    onChange={(prevevent, nextView) => {
                      setViewMode(nextView);
                    }}
                  />
                </>
              }
            />
          </Grid>
          {isFilterOpen ? (
            <Table
              sx={{ ...styleSheet.generalFilterArea }}
              size="small"
              aria-label="a dense table"
            >
              <TableRow>
                <Grid container spacing={2} sx={{ p: "15px" }}>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.INTEGRATION_CARRIER_FILTER_BY_COUNTRY
                      }
                    </InputLabel>
                    <CountrySchema
                      name={"country"}
                      height={28}
                      control={control}
                      getOptionLabel={(option) => option?.name}
                      value={selectedCountry}
                      onChange={handleCountryChange}
                    />
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {
                        LanguageReducer?.languageType
                          ?.INTEGRATION_CARRIER_FILTER_BY_SERVICES
                      }
                    </InputLabel>
                    <SelectComponent
                      name={"Services"}
                      height={28}
                      control={control}
                      options={AllDeliveryService}
                      value={SelectedServices}
                      optionLabel={EnumOptions.SERVICES.LABEL}
                      optionValue={EnumOptions.SERVICES.VALUE}
                      getOptionLabel={(option) => option?.serviceName}
                      onChange={handleServicesChange}
                    />
                  </Grid>
                  <Grid
                    item
                    xl={2}
                    lg={2}
                    md={2}
                    sm={6}
                    xs={12}
                    sx={styleSheet.filterBtn}
                  >
                    <Stack
                      alignItems="flex-end"
                      direction="row"
                      spacing={1}
                      sx={{ ...styleSheet.filterButtonMargin, height: "100%" }}
                    >
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={handleFilterClear}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.INTEGRATION_CLEAR_FILTER
                        }
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </Table>
          ) : null}
          <Grid item xl={12} lg={12} md={12} sm={12} xs={12}>
            <CarriersList
              carrierData={carrierData}
              setIsRefreshRequired={setIsRefreshRequired}
              setActiveUrl={setActiveUrl}
              activeUrl={activeUrl}
              loading={isLoad}
              setIsLoad={setIsLoad}
              isFilterOpen={isFilterOpen}
              getAllCarriersData={getAllCarriersData}
              getAllActivedCarrier={getAllActivedCarrier}
              carriersCount={carriersCount}
              activeCarriersCount={activeCarriersCount}
              viewMode={viewMode}
            />
          </Grid>
        </Grid>
      </div>
    </Box>
  );
}

export default CarriersPage;
