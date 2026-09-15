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
import { styleSheet } from "../../../assets/styles/style";
import { fetchMethod } from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import OrderLabelsList from "./list";
import CreateOrderLabelsModal from "../../../components/modals/orderModals/CreateOrderLabelsModal";
import { GetAllClientOrderLabelLookup } from "../../../api/AxiosInterceptors";

const OrderLables = () => {
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
  const [loading, setLoading] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allClientOrderLabels, setAllClientOrderLabels] = useState([]);
  const [openCreateOrderLabelModal, setOpenCreateOrderLabelModal] =
    useState(false);
  const handleFilterClear = async () => {
    handleFilterRest();
  };

  const getAllClientOrderLabelLookup = async () => {
    setLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetAllClientOrderLabelLookup(startDateFormated, endDateFormated)
      );
      if (response.isSuccess) {
        setAllClientOrderLabels(response?.result?.list);
      }
    } catch (error) {
      console.error("Error fetching client return reasons:", error);
    }
    setLoading(false);
  };

  useEffect(() => {
    getAllClientOrderLabelLookup();
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
            tabsSmWidth={125}
            tabsMdWidth={125}
            otherBtns={
              <ButtonComponent
                title={"Create Order Label"}
                onClick={() => setOpenCreateOrderLabelModal(true)}
              />
            }
            tabData={[
              {
                label: "All",
                route: "/order-lables",
                children: (
                  <OrderLabelsList
                    isFilterOpen={isFilterOpen}
                    loading={loading}
                    allClientOrderLabels={allClientOrderLabels}
                    getAllClientOrderLabelLookup={getAllClientOrderLabelLookup}
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
                              sx={{
                                ...styleSheet.inputLabel,
                                overflow: "unset",
                              }}
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
                              {LanguageReducer?.languageType?.CLEAR_FILTER}
                            </Button>
                            <Button
                              sx={{
                                ...styleSheet.filterIcon,
                                minWidth: "100px",
                              }}
                              variant="contained"
                              onClick={() => {
                                getAllClientOrderLabelLookup();
                              }}
                            >
                              {LanguageReducer?.languageType?.FILTER}
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
          {openCreateOrderLabelModal && (
            <CreateOrderLabelsModal
              open={openCreateOrderLabelModal}
              onClose={() => setOpenCreateOrderLabelModal(false)}
              getAllClientOrderLabelLookup={getAllClientOrderLabelLookup}
            />
          )}
        </div>
      </Box>
    </>
  );
};

export default OrderLables;
