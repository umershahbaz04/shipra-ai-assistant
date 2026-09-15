import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
} from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes } from "react-router-dom";
import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import {
  GetAllSaleChannelConfig,
  GetAllSaleChannelLookupForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import PlatFormConnectModal from "../../../components/modals/integrationModals/PlatFormConnectModal";
import {
  ActionButtonCustom,
  ToggleButtonComponent,
} from "../../../utilities/helpers/Helpers";
import PlatFormIntegrationList from "./list";
import AllSaleChannelList from "./allChannelList";
import { viewTypesEnum } from "../../../utilities/enum";

const EnumOrderDashboardAction = {
  EXCELEXPORT: "ExcelExport",
};
function PlatFormIntegrationPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [storeId, setStoreId] = useState(0);
  const [orderTypeId, setOrderTypeId] = useState(0);
  const [carrierIds, setCarrierIds] = useState("");
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [allPlatformForSelection, setAllPlatformForSelection] = useState([]);
  const [allPlatformConfig, setAllPlatformConfig] = useState([]);
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [isGridLoading, setIsGridLoading] = useState(false);
  const resetRowRef = useRef(false);
  const [viewMode, setViewMode] = useState(viewTypesEnum.GRID);
  const getOrdersRef = useRef([]);
  const [inputFields, setInputFields] = useState([]);
  const [openConnectModal, setOpenConnectModal] = useState(false);
  const [currentPage, setCurrentPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const getFiltersFromState = (page = currentPage, size = pageSize) => {
    let search = inputFields.join();
    let filters = {
      filterModel: {
        createdFrom: startDate,
        createdTo: endDate,
        start: page * size,
        length: size,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      storeId: storeId,
      carrierIds: carrierIds,
      orderTypeId: orderTypeId,
    };

    return filters;
  };
  // getAllSaleChannelConfig
  const getAllSaleChannelConfig = async (page = currentPage, size = pageSize) => {
    setCurrentPage(page);
    setPageSize(size);
    let params = getFiltersFromState(page, size);

    setIsGridLoading(true);
    await GetAllSaleChannelConfig(params)
      .then((res) => {
        const response = res.data;
        if (response.result) {
          setAllPlatformConfig(response.result);
        }
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setIsGridLoading(false);
      });
  };
  useEffect(() => {
    getAllSaleChannelConfig();
  }, []);

  useEffect(() => {
    if (isFilterReset) {
      getAllSaleChannelConfig();
    }
  }, [isFilterReset]);
  const handleFilterRest = () => {
    setStartDate(null);
    setEndDate(null);
  };
  const gtAllSaleChannelLookupForSelection = async () => {
    setIsGridLoading(true);
    let res = await GetAllSaleChannelLookupForSelection();
    if (res.data.result != null) {
      const filterResult = res.data.result.filter((_, index) => index !== 0);
      setAllPlatformForSelection(filterResult);
    }
    setIsGridLoading(false);
  };
  const getAllActive = () => {
    let obj = {
      TotalCount: 0,
      list: [],
    };
    obj.list = allPlatformConfig?.list?.filter((item) => {
      return item?.Active;
    });
    obj.TotalCount = allPlatformConfig.TotalCount;
    return obj;
  };
  const getAllInActive = () => {
    let obj = {
      TotalCount: 0,
      list: [],
    };
    obj.list = allPlatformConfig?.list?.filter((item) => !item?.Active) || [];
    obj.TotalCount = obj.list.length;
    return obj;
  };

  useEffect(() => {
    gtAllSaleChannelLookupForSelection();
  }, []);

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <DataGridHeader
          tabs={true}
          tabData={[
            {
              label:
                LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_ALL,
              route: "/sale-channels",
            },
            {
              label:
                LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_ACTIVE,
              route: "/sale-channels/active",
              onclick: getAllSaleChannelConfig,
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.INTEGRATION_SALE_CHANNEL_INACTIVE,
              route: "/sale-channels/in-active",
            },
          ]}
        >
          <Box className={"flex_center"} gap={1}>
            <ToggleButtonComponent
              value={viewMode}
              onChange={(prevevent, nextView) => {
                setViewMode(nextView);
              }}
            />
            <ActionButtonCustom
              onClick={(event) => {
                setOpenConnectModal(true);
              }}
              label={
                LanguageReducer?.languageType
                  ?.INTEGRATION_SALE_CHANNEL_ADD_SALE_CHANNEL
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
                      {
                        LanguageReducer?.languageType
                          ?.INTEGRATION_SALE_CHANNEL_START_DATE
                      }
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
                      {
                        LanguageReducer?.languageType
                          ?.INTEGRATION_SALE_CHANNEL_END_DATE
                      }
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
                        {LanguageReducer?.languageType?.ORDER_CLEAR_FILTER}
                      </Button>

                      <Button
                        sx={{
                          ...styleSheet.filterIcon,
                          minWidth: "100px",
                          marginLeft: "5px",
                        }}
                        variant="contained"
                        onClick={() => {
                          getAllSaleChannelConfig();
                        }}
                      >
                        {LanguageReducer?.languageType?.ORDERS_FILTER}
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
              <AllSaleChannelList
                allPlatformForSelection={allPlatformForSelection}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                GridLoading={isGridLoading}
                viewMode={viewMode}
                getAllSaleChannelConfig={getAllSaleChannelConfig}
              />
            }
          />
           <Route
            path="/active"
            element={
              <PlatFormIntegrationList
                rows={getAllActive()}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                GridLoading={isGridLoading}
                viewMode={viewMode}
                currentPage={currentPage}
                pageSize={pageSize}
                getAllSaleChannelConfig={getAllSaleChannelConfig}
              />
            }
          />
          <Route
            path="/in-active"
            element={
              <PlatFormIntegrationList
                rows={getAllInActive()}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                GridLoading={isGridLoading}
                viewMode={viewMode}
                currentPage={currentPage}
                pageSize={pageSize}
                getAllSaleChannelConfig={getAllSaleChannelConfig}
              />
            }
          />
        </Routes>
      </div>
      {openConnectModal && (
        <PlatFormConnectModal
          open={openConnectModal}
          setOpen={setOpenConnectModal}
          getAll={getAllSaleChannelConfig}
        />
      )}
    </Box>
  );
}
export default PlatFormIntegrationPage;
