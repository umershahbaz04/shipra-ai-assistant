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
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import { GetAllClientOrderBox } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import CreateOrderBoxModal from "../../../components/modals/settingsModals/CreateOrderBoxModal";
import { fetchMethod } from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import OrderBoxList from "./list";
const OrderBoxPage = (props) => {
  const [loading, setLoading] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [openCreateOrderBox, setOpenCreateOrderBox] = useState({
    open: false,
    loading: {},
  });
  const [allClientOrderBox, setAllClientOrderBox] = useState([]);
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
  const handleFilterRest = () => {
    resetDates();
  };
  const [isfilterClear, setIsfilterClear] = useState(false);
  const handleFilterClear = async () => {
    handleFilterRest();
    setIsfilterClear(true);
  };

  const getAllClientOrderBox = async () => {
    setLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetAllClientOrderBox(startDateFormated, endDateFormated)
      );
      if (response.isSuccess) {
        setAllClientOrderBox(response?.result);
      }
    } catch (error) {
      console.error("Error fetching client return reasons:", error);
    }
    setLoading(false);
  };

  useEffect(() => {
    getAllClientOrderBox();
  }, []);

  return (
    <>
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
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
            tabsSmWidth="10px"
            tabsMdWidth="10px"
            otherBtns={
              <ButtonComponent
                title={
                  LanguageReducer?.languageType
                    ?.SETTING_ORDER_BOX_CREATE_ORDER_BOX
                }
                onClick={() =>
                  setOpenCreateOrderBox((prev) => ({ ...prev, open: true }))
                }
              />
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
                          {LanguageReducer?.languageType?.START_DATE}
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
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {LanguageReducer?.languageType?.END_DATE}
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
                          sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                          color="inherit"
                          variant="outlined"
                          onClick={() => {
                            handleFilterClear();
                          }}
                        >
                          {LanguageReducer?.languageType?.CLEAR_FILTER}
                        </Button>
                        <Button
                          sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                          variant="contained"
                          //   onClick={() => {
                          //     ();
                          //   }}
                        >
                          {LanguageReducer?.languageType?.FILTER}
                        </Button>
                      </Stack>
                    </Grid>
                  </Grid>
                </TableRow>
              </TableHead>
            </Table>
          ) : null}
          {openCreateOrderBox.open && (
            <CreateOrderBoxModal
              open={openCreateOrderBox.open}
              getAllClientOrderBox={getAllClientOrderBox}
              onClose={() =>
                setOpenCreateOrderBox((prev) => ({ ...prev, open: false }))
              }
            />
          )}
          <OrderBoxList
            isFilterOpen={isFilterOpen}
            loading={loading}
            allClientOrderBox={allClientOrderBox}
            getAllClientOrderBox={getAllClientOrderBox}
          />
        </div>
      </Box>
    </>
  );
};

export default OrderBoxPage;
