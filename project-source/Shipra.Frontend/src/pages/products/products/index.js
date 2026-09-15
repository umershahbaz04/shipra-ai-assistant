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
import { useEffect, useState, useRef } from "react";
import { useSelector } from "react-redux";
import { Route, Routes, useLocation, useNavigate } from "react-router-dom";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import MenuIconComponent from "../../../.reUseableComponents/Buttons/MenuIconComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { GetAllProducts } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import AssignStoreToProductModal from "../../../components/modals/productModals/AssignStoreToProductModal";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions, viewTypesEnum } from "../../../utilities/enum";
import {
  fetchMethod,
  ToggleButtonComponent,
  useGetBreakPoint,
} from "../../../utilities/helpers/Helpers";
import { useGetAllStores } from "../../../utilities/helpers/HelpersFilter";
import ProducList from "./productList";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import SearchIcon from "@mui/icons-material/Search";

function Products() {
  const belowMdScreen = useGetBreakPoint("md");
  const navigate = useNavigate();
  const [products, setProducts] = useState([]);
  const [productsCount, setProductsCount] = useState(0);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isGridLoading, setIsGridLoading] = useState(false);
  const [viewMode, setViewMode] = useState(viewTypesEnum.TABLE);
  const [searchValue, setSearchValue] = useState("");
  const [openAssignAndUnAssignModal, setOpenAssignAndnAssignModal] = useState({
    open: false,
    data: [],
  });
  const {
    allStores,
    selectedStore,
    setSelctedStore,
    storeId,
    resetSelctedStore,
  } = useGetAllStores();
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  let getAllProducts = async (searchText) => {
    const { response } = await fetchMethod(
      () =>
        GetAllProducts(
          startDateFormated ? startDateFormated : null,
          endDateFormated ? endDateFormated : null,
          selectedStore
            ? selectedStore?.map((data) => data.storeId).toString()
            : "",
          null,
          searchText
        ),
      setIsGridLoading,
      false
    );
    if (response.result !== null) {
      setProducts(response?.result?.list);
      setProductsCount(response?.result?.TotalCount);
    }
  };
  const [isfilterClear, setIsfilterClear] = useState(false);
  const isFirstRender = useRef(true);

  const handleFilterRest = () => {
    resetDates();
    resetSelctedStore();
    setIsfilterClear(true);
  };
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const location = useLocation();

  useEffect(() => {
    if (location.pathname === "/products") {
      handleFilterRest();
    }
  }, [location.pathname]);

  useEffect(() => {
    if (isfilterClear) {
      getAllProducts();
      setIsfilterClear(false);
    }
  }, [isfilterClear]);

  const handleTabChange = (event, filterValue) => {
    navigate(filterValue);
  };

  const handleChangeSearch = (event) => {
    setSearchValue(event.target.value);
  };

  const handle_Enter = async (e) => {
    if (e.key === "Enter") {
      await getAllProducts(searchValue);
    }
  };

  const menuItems = [
    {
      label: LanguageReducer?.languageType?.PRODUCTS_ADD_PRODUCTS,
      onClick: () => {
        navigate("/add-products");
      },
    },
    {
      label: "Upload Product",
      onClick: () => navigate("/upload-product"),
    },
  ];

  useEffect(() => {
    if (isFirstRender.current) {
      isFirstRender.current = false;
      return;
    }

    const timer = setTimeout(() => {
      getAllProducts(searchValue);
    }, 600);

    return () => {
      clearTimeout(timer);
    };
  }, [searchValue]);

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
              placeholder="Search Product"
              name="search"
              value={searchValue}
              onChange={handleChangeSearch}
              onKeyDown={handle_Enter}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <SearchIcon
                      onClick={handle_Enter}
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
                  label: LanguageReducer?.languageType?.PRODUCT_ALL,
                  route: "/products",
                },
                {
                  label: LanguageReducer?.languageType?.PRODUCT_ACTIVE,
                  route: "/products/active",
                },
                {
                  label: LanguageReducer?.languageType?.PRODUCT_IN_ACTIVE,
                  route: "/products/in-active",
                },
              ]}
              handleFilterBtnOnClick={() => {
                setIsFilterOpen(!isFilterOpen);
              }}
              otherBtns={
                <>
                  <ToggleButtonComponent
                    reverse
                    value={viewMode}
                    onChange={(prevevent, nextView) => {
                      setViewMode(nextView);
                    }}
                  />
                  {!belowMdScreen && (
                    <>
                      <ButtonComponent
                        p={1}
                        btnLgWidth={"110px"}
                        title={
                          LanguageReducer?.languageType?.PRODUCTS_ADD_PRODUCTS
                        }
                        onClick={() => {
                          navigate("/add-products");
                        }}
                      />
                      <ButtonComponent
                        p={1}
                        btnLgWidth={"110px"}
                        title={"Upload Product"}
                        onClick={() => {
                          navigate("/upload-product");
                        }}
                      />
                      <ButtonComponent
                        p={1}
                        btnLgWidth={"140px"}
                        title={"Assign/unassign Store"}
                        onClick={() =>
                          setOpenAssignAndnAssignModal((prev) => ({
                            ...prev,
                            open: true,
                          }))
                        }
                      />
                    </>
                  )}
                </>
              }
              responsiveButton={
                <>
                  {belowMdScreen && menuItems.length > 0 ? (
                    <MenuIconComponent menuItems={menuItems} />
                  ) : null}
                </>
              }
            />
          </Grid>
        </Grid>
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
                        value={startDate}
                        maxDate={UtilityClass.todayDate()}
                        onClick={(date) => setStartDate(date)}
                        size="small"
                        isClearable

                        // inputProps={{ style: { padding: "2px 5px" } }}
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
                        value={endDate}
                        onClick={(date) => setEndDate(date)}
                        size="small"
                        minDate={startDate}
                        maxDate={UtilityClass.todayDate()}
                        disabled={!startDate ? true : false}
                        isClearable
                        // inputProps={{ style: { padding: "2px 5px" } }}
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
                        multiple={true}
                        height={28}
                        options={allStores}
                        value={selectedStore}
                        optionLabel={EnumOptions.STORE.LABEL}
                        optionValue={EnumOptions.STORE.VALUE}
                        getOptionLabel={(option) => option?.storeName}
                        onChange={(e, val) => {
                          setSelctedStore(val);
                        }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
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
                        onClick={() => {
                          handleFilterRest();
                        }}
                      >
                        {LanguageReducer?.languageType?.PRODUCTS_CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllProducts();
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
              <ProducList
                isGridLoading={isGridLoading}
                products={products}
                productsCount={productsCount}
                getAllProducts={getAllProducts}
                isFilterOpen={isFilterOpen}
                viewMode={viewMode}
                setIsGridLoading={setIsGridLoading}
              />
            }
          />
          <Route
            path="/active"
            element={
              <ProducList
                isGridLoading={isGridLoading}
                getAllProducts={getAllProducts}
                viewMode={viewMode}
                products={products.filter((product) => {
                  return product.Active === true;
                })}
                productsCount={productsCount}
                setIsGridLoading={setIsGridLoading}
              />
            }
          />
          <Route
            path="/in-active"
            element={
              <ProducList
                getAllProducts={getAllProducts}
                isGridLoading={isGridLoading}
                viewMode={viewMode}
                products={products.filter((product) => {
                  return product.Active === false;
                })}
                productsCount={productsCount}
                setIsGridLoading={setIsGridLoading}
              />
            }
          />
        </Routes>
        {openAssignAndUnAssignModal.open && (
          <AssignStoreToProductModal
            open={openAssignAndUnAssignModal.open}
            onClose={() =>
              setOpenAssignAndnAssignModal((prev) => ({
                ...prev,
                open: false,
              }))
            }
            getAllProductslist={getAllProducts}
          />
        )}
      </div>
    </Box>
  );
}
export default Products;
