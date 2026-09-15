import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import React, { useEffect, useState, useRef } from "react";
import { useSelector } from "react-redux";
import { Route, Routes } from "react-router-dom";
import { styleSheet } from "../../../assets/styles/style";
import GeneralTabBar from "../../../components/shared/tabsBar";
import { useForm, useWatch } from "react-hook-form";
import {
  GetAllPlatformConfig,
  GetAllPlatformForSelection,
  GetAllSaleChannelConfig,
  GetAllSaleChannelLookupForSelection,
  GetProductStations,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import PlatFormIntegrationList from "./list";
import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader";
import {
  ActionButton,
  ActionButtonCustom,
  ActionFilterButton,
} from "../../../utilities/helpers/Helpers";
import KeyboardArrowDown from '@mui/icons-material/KeyboardArrowDown';
import PlatFormConnectModal from "../../../components/modals/integrationModals/PlatFormConnectModal";
import ProductStationList from "./list";
import AddProductStationModal from "../../../components/modals/productStationModals/AddProductStationModal";
import UpdateProductStationModal from "../../../components/modals/productStationModals/UpdateProductStationModal";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";

const EnumOrderDashboardAction = {
  EXCELEXPORT: "ExcelExport",
};
function ProductStationPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [storeId, setStoreId] = useState(0);
  const [orderTypeId, setOrderTypeId] = useState(0);
  const [carrierIds, setCarrierIds] = useState("");
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [allProductStations, setAllProductStations] = useState([]);
  const [isGridLoading, setIsGridLoading] = useState(false);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const [inputFields, setInputFields] = useState([]);
  const [openConnectModal, setOpenConnectModal] = useState(false);

  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  const getFiltersFromState = () => {
    let search = inputFields.join();
    let filters = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 10,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
    };

    return filters;
  };
  // getAllStations
  const getAllStations = async () => {
    let params = getFiltersFromState();

    setIsGridLoading(true);
    await GetProductStations(params)
      .then((res) => {
        const response = res.data;
        if (response.result) {
          setAllProductStations(response.result);
        }
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setIsGridLoading(false);
      });
  };
  useEffect(() => {
    getAllStations();
  }, []);

  useEffect(() => {
    if (isFilterReset) {
      getAllStations();
    }
  }, [isFilterReset]);
  const handleFilterRest = () => {
    setStartDate(null);
    setEndDate(null);
  };
  const getAllActive = () => {
    let obj = {
      TotalCount: 0,
      list: [],
    };
    obj.list = allProductStations?.list?.filter((item) => {
      return item?.Active;
    });
    obj.TotalCount = allProductStations.TotalCount;
    return obj;
  };
  const getAllInActive = () => {
    let obj = {
      TotalCount: 0,
      list: [],
    };
    obj.list = allProductStations?.list?.filter((item) => {
      return !item?.Active;
    });
    obj.TotalCount = allProductStations.TotalCount;
    return obj;
  };
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <DataGridHeader
          tabs={true}
          tabData={[
            {
              label: LanguageReducer?.languageType?.PRODUCT_ALL,
              route: "/product-station",
            },
            {
              label: LanguageReducer?.languageType?.PRODUCT_ACTIVE,
              route: "/product-station/active",
            },
            {
              label: LanguageReducer?.languageType?.PRODUCT_IN_ACTIVE,
              route: "/product-station/in-active",
            },
          ]}
        >
          <Box className={"flex_center"} gap={1}>
            <ActionButtonCustom
              width={{ xs: "150px !important" }}
              onClick={(event) => {
                setOpenConnectModal(true);
              }}
              label={
                LanguageReducer?.languageType
                  ?.PRODUCT_PRODUCT_STATIONS_ADD_PRODUCT_STATIONS
              }
            />
            {/* <ActionFilterButton
                onClick={() =>
                  !isFilterOpen ? setIsFilterOpen(true) : setIsFilterOpen(false)
                }
              /> */}
          </Box>
        </DataGridHeader>
        {isFilterOpen ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <Stack direction={"column"} m={1}>
                <Stack direction={{ xs: "column", sm: "row" }} spacing={1}>
                  <Grid>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {LanguageReducer?.languageType?.PRODUCTS_START_DATE}
                    </InputLabel>

                    <CustomReactDatePickerInput
                      value={startDate}
                      onClick={(date) => setStartDate(date)}
                      size="small"
                      isClearable
                      inputProps={{ style: { padding: "4px 5px" } }}
                    />
                  </Grid>
                  <Grid>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {LanguageReducer?.languageType?.PRODUCTS_END_DATE}
                    </InputLabel>
                    <CustomReactDatePickerInput
                      value={endDate}
                      onClick={(date) => setEndDate(date)}
                      width="180px"
                      size="small"
                      minDate={startDate}
                      disabled={!startDate ? true : false}
                      isClearable
                      inputProps={{ style: { padding: "4px 5px" } }}
                    />
                  </Grid>

                  <Grid>
                    <Stack mt={2} direction={"row"}>
                      <Button
                        sx={{
                          ...styleSheet.filterIcon,
                          minWidth: "100px",
                          marginLeft: "5px",
                        }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          setIsFilterReset(true);
                          handleFilterRest();
                        }}
                      >
                        {LanguageReducer?.languageType?.PRODUCTS_CLEAR_FILTER}
                      </Button>

                      <Button
                        sx={{
                          ...styleSheet.filterIcon,
                          minWidth: "100px",
                          marginLeft: "5px",
                        }}
                        variant="contained"
                        onClick={() => {
                          getAllStations();
                        }}
                      >
                        {LanguageReducer?.languageType?.PRODUCT_FILTER}
                      </Button>
                    </Stack>
                  </Grid>
                </Stack>
              </Stack>
            </TableHead>
          </Table>
        ) : null}
        <Routes>
          <Route
            path="/"
            element={
              <ProductStationList
                rows={allProductStations}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                loading={isGridLoading}
                getAllStations={getAllStations}
              />
            }
          />
          <Route
            path="/active"
            element={
              <ProductStationList
                rows={getAllActive()}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                loading={isGridLoading}
                getAllStations={getAllStations}
              />
            }
          />
          <Route
            path="/in-active"
            element={
              <ProductStationList
                rows={getAllInActive()}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                loading={isGridLoading}
                getAllStations={getAllStations}
              />
            }
          />
        </Routes>
      </div>
      {openConnectModal && (
        <AddProductStationModal
          open={openConnectModal}
          setOpen={setOpenConnectModal}
          getAll={getAllStations}
        />
      )}
    </Box>
  );
}
export default ProductStationPage;
