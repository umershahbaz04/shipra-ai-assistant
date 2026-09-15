import {
  Box,
  Button,
  ButtonGroup,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import { saveAs } from "file-saver";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes, useLocation } from "react-router-dom";
import * as XLSX from "xlsx";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAllLowQuantityProductStock,
  GetAllStationLookup,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import LowInventoryList from "./lowInventoryList";

function LowInventoryPage(props) {
  const [inventory, setInventory] = useState([]);
  const [productStations, setProductStations] = useState([]);
  const [stores, setStores] = useState([]);
  const initialState = {
    store: {
      storeId: 0,
      storeName: "Select Please",
    },
    station: {
      productStationId: 0,
      sname: "Select Please",
    },
  };
  const [selectedStore, setSelectedStore] = useState([]);
  const [selectedStation, setSelectedStation] = useState(initialState.station);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isGridLoading, setIsGridLoading] = useState(null);
  const [openSyncModal, setOpenSyncModal] = useState(false);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  //#region
  const syncInventory = () => {
    setOpenSyncModal(true);
    //console.log("syncInventory");
  };

  //#endregion

  const exportToXLSX = () => {
    const worksheet = XLSX.utils.json_to_sheet(inventory?.list);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Data");
    const buffer = XLSX.write(workbook, { type: "array", bookType: "xlsx" });
    const filename = "data.xlsx";
    saveAs(new Blob([buffer]), filename);
  };

  let getAllStationLookup = async () => {
    let res = await GetAllStationLookup();
    //console.log("getAllStationLookup", res.data);
    setProductStations(res.data.result);
  };
  let getAllStores = async () => {
    let res = await GetStoresForSelection();
    //console.log("GetAllStores", res.data);
    if (res.data.result !== null) setStores(res.data.result);
  };
  const getFiltersFromState = () => {
    let filters = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 10000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      ProductStationId: selectedStation?.productStationId,
      // productStockStatusId: 0,
      StoreId: selectedStore
        ? selectedStore?.map((data) => data.storeId).toString()
        : "",
    };
    return filters;
  };
  let getAllProductInventory = async () => {
    let filter = getFiltersFromState();

    setIsGridLoading(true);
    let res = await GetAllLowQuantityProductStock(filter);
    setIsGridLoading(false);

    //console.log("getAllProductInventory:::", res.data);
    if (res.data.result != null) setInventory(res.data.result);
  };

  useEffect(() => {
    getAllStationLookup();
    getAllStores();
  }, []);
  useEffect(() => {
    getAllProductInventory();
  }, []);
  const handleFilterRest = () => {
    resetDates();
    setSelectedStation(initialState.station);
    setSelectedStore([]);
  };
  const [isfilterClear, setIsfilterClear] = useState(false);
  const handleFilterClear = async () => {
    handleFilterRest();
    setIsfilterClear(true);
  };
  const location = useLocation();
  useEffect(() => {
    //console.log(location.pathname);
    if (location.pathname === "/inventory") {
      handleFilterClear();
    }
  }, [location.pathname]);
  useEffect(() => {
    if (isfilterClear) {
      getAllProductInventory();
      setIsfilterClear(false);
    }
  }, [isfilterClear]);
  const downloadLowinventory = () => {
    let params = getFiltersFromState();
    GetAllLowQuantityProductStock(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadExcel(res.data, "inventory");
          // setIsLoading(false);
        } else {
          successNotification("Data not found");
          // setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };

  const handleTabChange = async (event, filterValue) => {};
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
          >
            <ButtonGroup
              variant="outlined"
              aria-label="split button"
            ></ButtonGroup>
          </Stack>
        </Box>
        <DataGridTabs
          handleTabChange={handleTabChange}
          tabData={[
            {
              label: LanguageReducer?.languageType?.PRODUCT_ALL,
              route: "/low-inventory",
            },
            {
              label: LanguageReducer?.languageType?.PRODUCT_ACTIVE,
              route: "/low-inventory/active",
            },
            {
              label: LanguageReducer?.languageType?.PRODUCT_IN_ACTIVE,
              route: "/low-inventory/in-active",
            },
          ]}
          // actionBtnMenuData={[
          //   {
          //     title: "Export to Excel",
          //     onClick: () => {
          //       exportToXLSX();
          //     },
          //   },
          //   {
          //     title: "Sync Inventory",
          //     onClick: () => {
          //       syncInventory();
          //     },
          //   },
          //   {
          //     title: "Low inventory",
          //     onClick: () => {
          //       downloadLowinventory();
          //     },
          //   },
          // ]}
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
        />

        {isFilterOpen ? (
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
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.PRODUCTS_START_DATE}
                      </InputLabel>

                      <CustomReactDatePickerInputFilter
                        maxDate={UtilityClass.todayDate()}
                        value={startDate}
                        onClick={(date) => setStartDate(date)}
                        size="small"
                        isClearable

                        // inputProps={{ style: { padding: "4px 5px" } }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.PRODUCTS_END_DATE}
                      </InputLabel>
                      <CustomReactDatePickerInputFilter
                        maxDate={UtilityClass.todayDate()}
                        value={endDate}
                        onClick={(date) => setEndDate(date)}
                        size="small"
                        minDate={startDate}
                        disabled={!startDate ? true : false}
                        isClearable

                        // inputProps={{ style: { padding: "4px 5px" } }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.PRODUCTS_STORE}
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        multiple={true}
                        options={stores}
                        value={selectedStore}
                        optionLabel={EnumOptions.STORE.LABEL}
                        optionValue={EnumOptions.STORE.VALUE}
                        getOptionLabel={(option) => option?.storeName}
                        onChange={(e, val) => {
                          setSelectedStore(val);
                        }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.PRODUCT_INVENTORY_SELECT_STATION
                        }
                      </InputLabel>
                      <SelectComponent
                        name="reason"
                        height={28}
                        options={productStations}
                        value={selectedStation}
                        optionLabel={EnumOptions.SELECT_STATION.LABEL}
                        optionValue={EnumOptions.SELECT_STATION.VALUE}
                        getOptionLabel={(option) => option?.sname}
                        onChange={(e, val) => {
                          setSelectedStation(val);
                        }}
                      />
                    </Grid>
                  </Grid>

                  <Grid
                    item
                    xl={2}
                    lg={2}
                    md={2}
                    sm={6}
                    xs={12}
                    alignSelf="end"
                  >
                    <Stack
                      direction={"row"}
                      sx={{ ...styleSheet.filterButtonMargin, display: "row" }}
                      spacing={1}
                    >
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          handleFilterClear();
                        }}
                      >
                        {LanguageReducer?.languageType?.PRODUCTS_CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllProductInventory();
                        }}
                      >
                        {LanguageReducer?.languageType?.PRODUCT_FILTER}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}
        {/* <LowInventoryList
          inventory={inventory?.list ? inventory?.list : []}
          getAllProductInventory={getAllProductInventory}
          isGridLoading={isGridLoading}
        /> */}
        <Routes>
          <Route
            path="/"
            element={
              <LowInventoryList
                inventory={inventory?.list ? inventory?.list : []}
                getAllProductInventory={getAllProductInventory}
                isGridLoading={isGridLoading}
                isFilterOpen={isFilterOpen}
              />
            }
          />
          <Route
            path="/active"
            element={
              <LowInventoryList
                getAllProductInventory={getAllProductInventory}
                inventory={
                  inventory?.list
                    ? inventory?.list.filter((product) => {
                        return product.Active === true;
                      })
                    : []
                }
                isGridLoading={isGridLoading}
              />
            }
          />
          <Route
            path="/in-active"
            element={
              <LowInventoryList
                getAllProductInventory={getAllProductInventory}
                inventory={
                  inventory?.list
                    ? inventory?.list.filter((product) => {
                        return product.Active !== true;
                      })
                    : []
                }
                isGridLoading={isGridLoading}
              />
            }
          />
        </Routes>
      </div>
    </Box>
  );
}
export default LowInventoryPage;
