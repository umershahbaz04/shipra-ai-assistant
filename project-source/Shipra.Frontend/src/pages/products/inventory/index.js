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
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes, useLocation } from "react-router-dom";
import * as XLSX from "xlsx";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import SwitchMui from "../../../.reUseableComponents/Switch/SwitchMui";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SearchInputAutoCompleteMultiple from "../../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import {
  ExcelExportProductInventory,
  ExcelExportProductInventorySummary,
  GetAllLowQuantityProductStock,
  GetAllProductInventory,
  GetAllStationLookup,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import SyncInventoryModal from "../../../components/modals/productModals/SyncInventoryModal";
import AssignStationModal from "../../../components/modals/productModals/AssignStationModal";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import {
  MAX_TAGS,
  handleFocus,
  useGetBreakPoint,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import InventoryList from "./inventoryList";
import MenuIconComponent from "../../../.reUseableComponents/Buttons/MenuIconComponent";

function Inventory(props) {
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
  const [QtyAvailable, setQtyAvailable] = useState("");
  const [searchParams, setSearchParams] = useState([]);
  const [isFilterOpen, setIsFilterOpen] = useState(false);

  const [isGridLoading, setIsGridLoading] = useState(null);
  const [openSyncModal, setOpenSyncModal] = useState(false);
  const [openAssignStation, setOpenAssignStation] = useState(false);
  const [isAvailable, setIsAvailable] = useState(true);
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const [selectedItems, setSelectedItems] = useState([]);
  const belowMdScreen = useGetBreakPoint("md");
  const belowSmScreen = useGetBreakPoint("sm");

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
  const getFiltersFromState = (_searchParams) => {
    let filters = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 10000,
        search: _searchParams || String(searchParams),
        sortDir: "desc",
        sortCol: 0,
      },
      ProductStationId: selectedStation?.productStationId,
      // productStockStatusId: 0,
      isAvailable: isAvailable,
      StoreId:
        selectedStore.length > 0
          ? selectedStore?.map((data) => data.storeId).toString()
          : "",
      availableQty: Number(QtyAvailable),
    };
    return filters;
  };
  const downloadExcel = () => {
    let params = getFiltersFromState();
    ExcelExportProductInventory(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadExcel(res.data, "inventory");
          // setIsLoading(false);
        } else {
          successNotification("data not found");
          // setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  const downloadExcelSummary = () => {
    let params = getFiltersFromState();
    ExcelExportProductInventorySummary(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadExcel(res.data, "inventory summary");
          // setIsLoading(false);
        } else {
          successNotification("data not found");
          // setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  let getAllProductInventory = async (searchParams) => {
    let filter = getFiltersFromState(searchParams);

    setIsGridLoading(true);
    let res = await GetAllProductInventory(filter);
    setIsGridLoading(false);

    //console.log("getAllProductInventory:::", res.data);
    if (res.data.result != null) setInventory(res.data.result);
  };

  useEffect(() => {
    getAllStationLookup();
    getAllStores();
  }, []);
  // useEffect(() => {
  //   getAllProductInventory();
  // }, []);
  const handleFilterRest = () => {
    setStartDate(null);
    setEndDate(null);
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
      resetDates();
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
  useEffect(() => {
    getAllProductInventory();
  }, [isAvailable]);

  const handleTabChange = async (event, filterValue) => {};

  const menuItems = [
    {
      label: "Assign Station",
      onClick: () => setOpenAssignStation(true),
    },
    {
      label: LanguageReducer?.languageType?.PRODUCT_INVENTORY_SYNC_INVENTORY,
      onClick: () => syncInventory,
    },
    {
      label: "Stock Report Excel",
      onClick: () => {
        downloadExcel();
      },
    },
    {
      label: "Stock Summary Report",
      onClick: () => {
        downloadExcelSummary();
      },
    },
  ];

  const actionBtnMenuData = !belowMdScreen
    ? [
        {
          title: "Stock Report Excel",
          onClick: () => {
            downloadExcel();
          },
        },
        {
          title: "Stock Summary Report",
          onClick: () => {
            downloadExcelSummary();
          },
        },
      ]
    : null;

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
              onChange={(e, value) => {
                const _searchParams = value.slice(0, MAX_TAGS);
                getAllProductInventory(String(_searchParams));
                setSearchParams(_searchParams);
              }}
              inputFields={searchParams}
              MAX_TAGS={MAX_TAGS}
            />
          </Grid>
        </Grid>
        <DataGridTabs
          customBorderRadius={"0px !important"}
          handleTabChange={handleTabChange}
          tabData={[
            {
              label: LanguageReducer?.languageType?.PRODUCT_ALL,
              route: "/inventory",
            },
            {
              label: LanguageReducer?.languageType?.PRODUCT_ACTIVE,
              route: "/inventory/active",
            },
            {
              label: LanguageReducer?.languageType?.PRODUCT_IN_ACTIVE,
              route: "/inventory/in-active",
            },
          ]}
          actionBtnMenuData={actionBtnMenuData}
          otherBtns={
            <>
              {!belowSmScreen && (
                <SwitchMui
                  checked={isAvailable}
                  onChange={() => setIsAvailable((prev) => !prev)}
                  label={
                    LanguageReducer?.languageType
                      ?.PRODUCT_INVENTORY_AVAILABLE_ONLY
                  }
                />
              )}
              {!belowMdScreen && (
                <ButtonComponent
                  title={
                    LanguageReducer?.languageType
                      ?.PRODUCT_INVENTORY_SYNC_INVENTORY
                  }
                  onClick={syncInventory}
                />
              )}
              {!belowMdScreen && (
                <ButtonComponent
                  title={"Assign Station"}
                  onClick={() => setOpenAssignStation(true)}
                />
              )}
            </>
          }
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
          responsiveButton={
            <>
              {belowMdScreen && menuItems.length > 0 ? (
                <MenuIconComponent menuItems={menuItems} />
              ) : null}
            </>
          }
        />
        {isFilterOpen ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <TableRow>
                <Grid container spacing={1} sx={{ p: "15px" }}>
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
                        options={stores}
                        multiple={true}
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
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.PRODUCT_INVENTORY_QTY_AVAILABLE
                        }
                      </InputLabel>
                      <TextFieldComponent
                        type={"number"}
                        borderRadius={"4px"}
                        height={29}
                        value={QtyAvailable}
                        onChange={(e) => {
                          setQtyAvailable(e.target.value);
                        }}
                      />
                    </Grid>
                  </Grid>
                  {belowSmScreen && (
                    <Grid
                      item
                      xl={2}
                      lg={2}
                      md={2}
                      sm={6}
                      xs={5}
                      alignSelf={"end"}
                    >
                      <Grid>
                        <SwitchMui
                          checked={isAvailable}
                          onChange={() => setIsAvailable((prev) => !prev)}
                          label={
                            LanguageReducer?.languageType
                              ?.PRODUCT_INVENTORY_AVAILABLE_ONLY
                          }
                        />
                      </Grid>
                    </Grid>
                  )}
                  <Grid
                    item
                    xl={2}
                    lg={2}
                    md={2}
                    sm={6}
                    xs={6}
                    alignSelf={"end"}
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
        <Routes>
          <Route
            path="/"
            element={
              <InventoryList
                inventory={inventory?.list ? inventory?.list : []}
                getAllProductInventory={getAllProductInventory}
                isGridLoading={isGridLoading}
                isFilterOpen={isFilterOpen}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                setSelectedItems={setSelectedItems}
              />
            }
          />
          <Route
            path="/active"
            element={
              <InventoryList
                getAllProductInventory={getAllProductInventory}
                inventory={
                  inventory?.list
                    ? inventory?.list.filter((product) => {
                        return product.Active === true;
                      })
                    : []
                }
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                setSelectedItems={setSelectedItems}
                isGridLoading={isGridLoading}
              />
            }
          />
          <Route
            path="/in-active"
            element={
              <InventoryList
                getAllProductInventory={getAllProductInventory}
                inventory={
                  inventory?.list
                    ? inventory?.list.filter((product) => {
                        return product.Active !== true;
                      })
                    : []
                }
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                setSelectedItems={setSelectedItems}
                isGridLoading={isGridLoading}
              />
            }
          />
        </Routes>
      </div>
      
      {openAssignStation && (
        <AssignStationModal
          open={openAssignStation}
          handleClose={() => setOpenAssignStation(false)}
          handleRefresh={getAllProductInventory}
        />
      )}

      {openSyncModal && (
        <SyncInventoryModal
          open={openSyncModal}
          setOpen={setOpenSyncModal}
          getAll={getAllProductInventory}
          selectedItems={getOrdersRef.current}
        />
      )}
    </Box>
  );
}
export default Inventory;
