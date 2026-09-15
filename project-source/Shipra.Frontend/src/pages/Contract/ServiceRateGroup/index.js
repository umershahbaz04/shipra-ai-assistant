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
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import { GetAllServiceRateGroup } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import CreateServiceRateModal from "../../../components/modals/ContractModals/CreateServiceRateModal";
import { EnumRoutesUrls } from "../../../utilities/enum";
import { successNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import ServiceRateGroupList from "./list";

const ServiceRateGroup = () => {
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
  const [allServiceRateGroup, setAllServiceRateGroup] = useState([]);
  const [openCreateCarrierRateModal, setOpenCreateCarrierRateModal] =
    useState(false);

  const handleFilterClear = async () => {
    handleFilterRest();
  };

  const getAllServiceRateGroup = async () => {
    const body = {
      FilterModel: {
        createdFrom: startDateFormated || null,
        createdTo: endDateFormated || null,
        start: 0,
        length: 100,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
    };
    setLoading(true);
    try {
      const response = await GetAllServiceRateGroup(body);
      if (response?.data?.isSuccess) {
        setAllServiceRateGroup(response?.data?.result?.list);
      }
    } catch (e) {
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getAllServiceRateGroup();
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
              <>
                {/* <ButtonComponent
                  title={"Create Carrier Contract Rate"}
                  onClick={() => setOpenCreateOrderLabelModal(true)}
                /> */}
                <ButtonComponent
                  title={"Create Service Rate"}
                  onClick={() => setOpenCreateCarrierRateModal(true)}
                />
              </>
            }
            tabData={[
              {
                label: "All",
                route: EnumRoutesUrls.SERIVCE_RATE_GROUP,
                children: (
                  <ServiceRateGroupList
                    isFilterOpen={isFilterOpen}
                    loading={loading}
                    allServiceRateGroup={allServiceRateGroup}
                    getAllServiceRateGroup={getAllServiceRateGroup}
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
                              // onClick={() => {
                              //   getAllClientOrderLabelLookup();
                              // }}
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
          {openCreateCarrierRateModal && (
            <CreateServiceRateModal
              open={openCreateCarrierRateModal}
              onClose={() => setOpenCreateCarrierRateModal(false)}
              getAllServiceRateGroup={getAllServiceRateGroup}
            />
          )}
        </div>
      </Box>
    </>
  );
};

export default ServiceRateGroup;
