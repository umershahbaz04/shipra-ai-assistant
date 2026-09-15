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
import { Route, Routes, useNavigate } from "react-router-dom";

import ButtonComponent from "../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import { GetAllStores } from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import AddChannelModal from "../../components/modals/channelModals/AddChannelModal";
import AddStoreModal from "../../components/modals/storeModals/AddStoreModal";
import DeleteStoreModal from "../../components/modals/storeModals/DeleteStoreModal";
import { useSetNumericInputEffect } from "../../utilities/helpers/Helpers";
import UtilityClass from "../../utilities/UtilityClass";
import StoreList from "./storeList/index";

function StorePage(props) {
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const navigate = useNavigate();
  const [openAddStore, setOpenAddStore] = useState(false);
  const [openDeleteStore, setOpenDeleteStore] = useState(false);

  const [stores, setStores] = useState([]);
  const [storeId, setStoreId] = useState("");
  const [isFilterOpen, setIsFilterOpen] = useState(false);

  const [isGridLoading, setIsGridLoading] = useState(null);

  let getAllStores = async () => {
    setIsGridLoading(true);
    let res = await GetAllStores({
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 10000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
    });
    setIsGridLoading(false);
    if (res.data.result !== null) setStores(res.data.result.list);
  };
  useEffect(() => {
    getAllStores();
  }, []);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const [openAddChannel, setOpenAddChannel] = useState(false);
  const handleFilterRest = () => {
    resetDates();
  };
  const [isfilterClear, setIsfilterClear] = useState(false);

  const handleFilterClear = async () => {
    handleFilterRest();
    setIsfilterClear(true);
  };

  const handleTabChange = (event, filterValue) => {
    navigate(filterValue);
  };

  useEffect(() => {
    if (isfilterClear) getAllStores();
  }, [isfilterClear]);

  useSetNumericInputEffect([openAddStore]);
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <DataGridTabs
          handleTabChange={handleTabChange}
          tabData={[
            {
              label: LanguageReducer?.languageType?.STORE_ALL,
              route: "/store",
            },
            {
              label: LanguageReducer?.languageType?.STORE_ACTIVE,
              route: "/store/active",
            },
            {
              label: LanguageReducer?.languageType?.STORE_INACTIVE,
              route: "/store/in-active",
            },
          ]}
          disableSearch
          {...props}
          handleFilterBtnOnClick={() => {
            setIsFilterOpen(!isFilterOpen);
          }}
          otherBtns={
            <>
              <ButtonComponent
                p={1}
                btnLgWidth={"110px"}
                title={"Upload Store"}
                onClick={() => navigate("/upload-stores")}
              />
              <ButtonComponent
                p={1}
                btnLgWidth={"110px"}
                title={"Add Store"}
                onClick={() => setOpenAddStore(true)}
              />
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
                <Grid container spacing={2} sx={{ p: "15px 10px" }}>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.STORE_START_DATE}
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
                        {LanguageReducer?.languageType?.STORE_END_DATE}
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
                    <Stack
                      direction={"row"}
                      alignItems={"flex-end"}
                      sx={{ ...styleSheet.filterButtonMargin, height: "100%" }}
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
                        {LanguageReducer?.languageType?.STORE_CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllStores();
                        }}
                      >
                        {LanguageReducer?.languageType?.STORE_FILTER}
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
              <StoreList
                isFilterOpen={isFilterOpen}
                stores={stores}
                setStoreId={setStoreId}
                setOpenAddStore={setOpenAddStore}
                setOpenDeleteStore={setOpenDeleteStore}
                setOpenAddChannel={setOpenAddChannel}
                getAllStores={getAllStores}
                isGridLoading={isGridLoading}
              />
            }
          />
          <Route
            path="/active"
            element={
              <StoreList
                stores={stores.filter((store) => {
                  return store.Active === true;
                })}
                setStoreId={setStoreId}
                setOpenAddStore={setOpenAddStore}
                setOpenDeleteStore={setOpenDeleteStore}
                setOpenAddChannel={setOpenAddChannel}
                getAllStores={getAllStores}
                isGridLoading={isGridLoading}
              />
            }
          />
          <Route
            path="/in-active"
            element={
              <StoreList
                stores={stores.filter((store) => {
                  return store.Active === false;
                })}
                setStoreId={setStoreId}
                setOpenAddStore={setOpenAddStore}
                setOpenDeleteStore={setOpenDeleteStore}
                setOpenAddChannel={setOpenAddChannel}
                getAllStores={getAllStores}
                isGridLoading={isGridLoading}
              />
            }
          />
        </Routes>
      </div>
      {openAddStore && (
        <AddStoreModal
          open={openAddStore}
          setOpen={setOpenAddStore}
          getAllStores={getAllStores}
          {...props}
        />
      )}

      <DeleteStoreModal
        open={openDeleteStore}
        setOpen={setOpenDeleteStore}
        getAllStores={getAllStores}
        storeId={storeId}
        {...props}
      />
      <AddChannelModal
        open={openAddChannel}
        setOpen={setOpenAddChannel}
        storeId={storeId}
        {...props}
        getAllStores={getAllStores}
      />
    </Box>
  );
}
export default StorePage;
