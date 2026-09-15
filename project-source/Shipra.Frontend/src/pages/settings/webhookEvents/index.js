import AddBoxIcon from "@mui/icons-material/AddBox";
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
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  GetAllClientWebhookEvents,
  GetAllWebhookEventLog,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import CreateWebhookModal from "../../../components/modals/settingsModals/CreateWebhookModal";
import {
  ActionButtonCustom,
  fetchMethod,
} from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import WebhookEventList from "./list";
import WebhookLogs from "./logs";
const EnumTabFilter = Object.freeze({
  webhook: "/webhook-events",
  Logs: "/webhook-events/Logs",
});
const WebhookEventsPage = () => {
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [openCreateWebhookModal, setOpenCreateWebhookModal] = useState({
    open: false,
    loading: {},
  });
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [allClientWebhookEvents, setAllClientWebhookEvents] = useState([]);
  const [loading, setLoading] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [logLoading, setLogLoading] = useState(true);
  const [allWebhookLogs, setallWebhookLogs] = useState([]);

  const getAllClientWebhookEvents = async () => {
    setLoading(true);
    try {
      const { response } = await fetchMethod(() => GetAllClientWebhookEvents());
      if (response.isSuccess) {
        setAllClientWebhookEvents(response?.result?.list);
      }
    } catch (error) {
      console.error("Error fetching:", error);
    }
    setLoading(false);
  };

  const getAllWebhookEventLog = async () => {
    setLogLoading(true);
    try {
      const { response } = await fetchMethod(() =>
        GetAllWebhookEventLog(startDate, endDate)
      );
      if (response.isSuccess) {
        setallWebhookLogs(response?.result?.list);
      }
    } catch (error) {
      console.error("Error fetching:", error);
    }
    setLogLoading(false);
  };

  const handleFilterClear = async () => {
    setIsfilterClear(true);
    resetDates();
    if (window.location.pathname === "/webhook-events") {
      getAllClientWebhookEvents();
    } else {
      getAllWebhookEventLog();
    }
  };

  const handleFilter = () => {
    if (window.location.pathname === "/webhook-events") {
      getAllClientWebhookEvents();
    } else {
      getAllWebhookEventLog();
    }
  };

  useEffect(() => {
    getAllClientWebhookEvents();
    getAllWebhookEventLog();
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
            tabsSmWidth="150px"
            tabsMdWidth="250px"
            filterData={
              isFilterOpen ? (
                <Table
                  sx={{ ...styleSheet.generalFilterArea }}
                  size="small"
                  aria-label="a dense table"
                >
                  <TableHead>
                    <TableRow>
                      <Grid container spacing={2} sx={{ p: "15px" }}>
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
                              value={startDate}
                              onClick={(date) => setStartDate(date)}
                              size="small"
                              isClearable
                              maxDate={UtilityClass.todayDate()}
                            />
                          </Grid>
                        </Grid>
                        <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
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
                              value={endDate}
                              onClick={(date) => setEndDate(date)}
                              size="small"
                              minDate={startDate}
                              disabled={!startDate ? true : false}
                              isClearable
                              maxDate={UtilityClass.todayDate()}
                            />
                          </Grid>
                        </Grid>

                        <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                          <Stack
                            alignItems="flex-end"
                            direction="row"
                            spacing={1}
                            sx={{
                              ...styleSheet.filterButtonMargin,
                              height: "100%",
                            }}
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
                              onClick={handleFilter}
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
            tabData={[
              {
                label: LanguageReducer?.languageType?.SETTING_WEBHOOK,
                route: EnumTabFilter.webhook,
                children: (
                  <WebhookEventList
                    loading={loading}
                    allClientWebhookEvents={allClientWebhookEvents}
                    getAllClientWebhookEvents={getAllClientWebhookEvents}
                    isFilterOpen={isFilterOpen}
                  />
                ),
              },
              {
                label: LanguageReducer?.languageType?.SETTING_WEBHOOK_LOGS,
                route: EnumTabFilter.Logs,
                children: (
                  <WebhookLogs
                    logLoading={logLoading}
                    allWebhookLogs={allWebhookLogs}
                    isFilterOpen={isFilterOpen}
                  />
                ),
              },
            ]}
            otherBtns={
              window.location.pathname === "/webhook-events" ? (
                <ActionButtonCustom
                  label={
                    LanguageReducer?.languageType?.SETTING_WEBHOOK_ADD_END_POINT
                  }
                  startIcon={<AddBoxIcon fontSize="small" />}
                  onClick={() =>
                    setOpenCreateWebhookModal((prev) => ({
                      ...prev,
                      open: true,
                    }))
                  }
                />
              ) : null
            }
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
          />
          {openCreateWebhookModal.open && (
            <CreateWebhookModal
              open={openCreateWebhookModal.open}
              allClientWebhookEvents={allClientWebhookEvents}
              getAllClientWebhookEvents={getAllClientWebhookEvents}
              onClose={() =>
                setOpenCreateWebhookModal((prev) => ({ ...prev, open: false }))
              }
            />
          )}
        </div>
      </Box>
    </>
  );
};

export default WebhookEventsPage;
