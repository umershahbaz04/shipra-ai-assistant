import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
  Select,
  MenuItem,
  FormControl,
  IconButton,
  Menu,
} from "@mui/material";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import RefreshIcon from "@mui/icons-material/Refresh";

import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes } from "react-router-dom";

import CountrySchema from "../../utilities/helpers/countryschema";
import { isShowCountryInTabbarFlag } from "../../utilities/cookies";
import CustomReactDatePickerInputFilter from "../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  GetAllLead,
  GetAllClientLeadStatusForSelection,
  GetAllSalePersonForSelection,
  GetAllLeadTabsCount,
  GetLeadTabsCountConfig,
  UpdateLeadTabDisplayOrder,
  CreateLeadGridColumn,
  GetAllClientLeadStatus,
  ExcelExportLeads,
  DeleteBulkLeads,
  GetSalesPersonLeadStats,
} from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import UtilityClass from "../../utilities/UtilityClass";
import { warningNotification, successNotification, errorNotification } from "../../utilities/toast";
import { useGetBreakPoint, CicrlesLoading, handleFilterAddressSchemaSelectDataIncExcValues, useGetAllCountries, CustomColorLabelledOutline } from "../../utilities/helpers/Helpers";
import initialStateFilter from "../../utilities/filterState";
import { EnumOptions } from "../../utilities/enum";
import Colors from "../../utilities/helpers/Colors";

import useDateRangeHook from "../../.reUseableComponents/CustomHooks/useDateRangeHook";
import { addressSchemaEnum } from "../../utilities/helpers/addressSchema";
import ButtonComponent from "../../.reUseableComponents/Buttons/ButtonComponent";
import SelectComponent from "../../.reUseableComponents/TextField/SelectComponent";
import SearchInputAutoCompleteMultiple from "../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";
import GeneralTabBar from "../../components/shared/shipmentTabsBar";

import AssignSalespersonModal from "../../components/modals/leadsModals/AssignSalespersonModal";
import ImportUploadLeadsModal from "../../components/modals/leadsModals/ImportUploadLeadsModal";
import CreateLeadModal from "../../components/modals/leadsModals/CreateLeadModal";
import AddLeadTabModal from "../../components/modals/leadsModals/AddLeadTabModal";
import UpdateLeadStatusModal from "../../components/modals/leadsModals/UpdateLeadStatusModal";
import DeleteConfirmationModal from "../../.reUseableComponents/Modal/DeleteConfirmationModal";
import LeadChart from "./LeadChart";

import LeadsList from "./leadsList";
const MAX_TAGS = 200;

function Leads(props) {
  const [inputFields, setInputFields] = useState([]);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allLead, setAllLead] = useState([]);
  const [isAllListLoading, setIsAllListLoading] = useState(false);
  const [selectedLeads, setSelectedLeads] = useState([]);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);

  // Salesperson filter
  const [allSalesPerson, setAllSalesPerson] = useState([]);
  const [selectedSalesPerson, setSelectedSalesPerson] = useState(initialStateFilter.multiple);

  const [allLeadStatus, setAllLeadStatus] = useState([]);

  // Assignment filter
  const [assignmentFilter, setAssignmentFilter] = useState("all");

  const isShowCountryInTabbar = isShowCountryInTabbarFlag();
  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry
  );
  const [selectedCountry, setSelectedCountry] = useState(
    isShowCountryInTabbar ? (reduxSelectedCountry || null) : null
  );

  useEffect(() => {
    if (isShowCountryInTabbar) {
      setSelectedCountry(reduxSelectedCountry || null);
    }
  }, [reduxSelectedCountry, isShowCountryInTabbar]);

  const { countries } = useGetAllCountries();

  // Tab state — mirrors shipment tab pattern
  const [leadTabData, setLeadTabData] = useState([]);
  const [activeTabStatusIds, setActiveTabStatusIds] = useState(""); // "" = All (no filter)

  // Drag reorder state — mirrors updateShipmentTabs in shipments
  const [updateLeadTabs, setUpdateLeadTabs] = useState([]);
  const [isMovingStatus, setIsMovingStatus] = useState(false);
  const [statusMoved, setStatusMoved] = useState(false);
  const [isTabsCountLoading, setIsTabsCountLoading] = useState(false);

  // Add Tab modal
  const [openAddTabModal, setOpenAddTabModal] = useState(false);
  const handleAddTab = () => {
    setOpenAddTabModal(true);
  };

  // Assign Salesperson modal
  const [openAssignSalesperson, setOpenAssignSalesperson] = useState(false);

  // Upload Leads modal
  const [openImportLeadsModal, setOpenImportLeadsModal] = useState(false);

  // Create Lead modal
  const [openCreateLeadModal, setOpenCreateLeadModal] = useState(false);
  const [anchorElAction, setAnchorElAction] = useState(null);

  // Update Lead Status modal
  const [openUpdateStatusModal, setOpenUpdateStatusModal] = useState(false);
  const [selectedLeadForStatus, setSelectedLeadForStatus] = useState(null);

  // Delete Leads Confirm modal
  const [openDeleteConfirm, setOpenDeleteConfirm] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  const [isfilterClear, setIsfilterClear] = useState(false);
  const belowMdScreen = useGetBreakPoint("md");
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

  const getFiltersFromState = (statusIds) => {
    const search = inputFields.join();
    // statusIds passed directly from tab click; fallback to activeTabStatusIds
    const tabIds = statusIds !== undefined ? statusIds : activeTabStatusIds;
    // We are removing LeadStatus dropdown filter as requested, so only tabIds are used
    const filterStatusIds = tabIds || "";
    return {
      Start: 0,
      Length: 1000,
      Search: search,
      SortDir: "desc",
      SortCol: 0,
      SalespersonIds: selectedSalesPerson?.map((d) => d.id).toString(),
      LeadStatusIds: filterStatusIds,
      AssignmentFilter: assignmentFilter === "all" ? "" : assignmentFilter,
      CountryId: selectedCountry?.countryId || selectedCountry?.id || null,
      StartDate: startDateFormated,
      EndDate: endDateFormated,
    };
  };

  const getAllLead = async (statusIds) => {
    setIsAllListLoading(true);
    try {
      const params = getFiltersFromState(statusIds);
      const res = await GetAllLead(params);
      if (res?.data?.result !== null) {
        setAllLead(res.data.result);
      }
    } catch (e) {
      console.error("Error fetching leads:", e);
    } finally {
      setIsAllListLoading(false);
      resetRowRef.current = false;
    }
  };

  const downloadExcel = () => {
    let params = getFiltersFromState();
    if (selectedLeads.length > 0) {
      params.Search = selectedLeads.join(",");
    }

    ExcelExportLeads(params)
      .then((res) => {
        if (!res?.data?.isSuccess && res.data) {
          UtilityClass.downloadExcel(res.data, "Leads");
        } else {
          successNotification("No leads found or unable to export.");
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download Excel");
      });
  };

  const handleDeleteLeads = () => {
    if (selectedLeads.length === 0) return;
    setIsDeleting(true);
    DeleteBulkLeads({ LeadIds: selectedLeads })
      .then((res) => {
        if (res?.data?.isSuccess) {
          successNotification(res?.data?.message || "Leads deleted successfully.");
          getAllLead();
          buildTabData();
          setSelectedLeads([]);
          if (resetRowRef) resetRowRef.current = true;
          setOpenDeleteConfirm(false);
        } else {
          errorNotification(res?.data?.message || "Error deleting leads.");
        }
      })
      .catch((e) => {
        console.error("e", e);
        errorNotification("Unable to delete leads.");
      })
      .finally(() => {
        setIsDeleting(false);
      });
  };

  // ──────────────────────────────────────────────
  // Lead Tabs — mirrors shipment tab pattern
  // ──────────────────────────────────────────────
  const getLeadTabsConfig = async () => {
    try {
      const res = await GetLeadTabsCountConfig();
      return res?.data?.result || [];
    } catch (e) {
      console.error("Error fetching lead tab config:", e);
      return [];
    }
  };

  const [salesPersonLeadStats, setSalesPersonLeadStats] = useState([]);
  const [tabConfig, setTabConfig] = useState([]);
  const [isChartLoading, setIsChartLoading] = useState(false);
  const [isChartDataFetched, setIsChartDataFetched] = useState(false);

  const buildTabsHelper = (dataObj, config) => {
    const tabs = config.map((tab) => {
      const key = tab.dashboardStatusNameForKey || tab.DashboardStatusNameForKey;
      const name = tab.dashboardStatusName || tab.DashboardStatusName;
      const isDefault = tab.isDefaultStatusTab ?? tab.IsDefaultStatusTab ?? false;
      const statusIds = tab.dashboardStatusValue || tab.DashboardStatusValue || "";

      let count = 0;
      if (isDefault) {
        count = dataObj["TotalLeads"] || dataObj["TotalCount"] || dataObj["totalCount"] || 0;
      } else {
        count = dataObj[key] || 0;
      }

      return {
        leadGridColumnId: tab.leadGridColumnId || tab.LeadGridColumnId,
        label: name,
        statusIds: isDefault ? "" : statusIds,
        count: count,
        displayOrder: tab.displayOrder || tab.DisplayOrder,
      };
    });
    tabs.sort((a, b) => a.displayOrder - b.displayOrder);
    return tabs;
  };

  const getLeadTabsCount = async (config) => {
    try {
      const body = {
        SalespersonIds: selectedSalesPerson?.map((d) => d.id).toString(),
        Search: inputFields.join(),
        AssignmentFilter: assignmentFilter === "all" ? "" : assignmentFilter,
        CountryId: selectedCountry?.countryId || selectedCountry?.id || null,
        StartDate: startDateFormated,
        EndDate: endDateFormated,
      };
      
      // Fetch total counts
      const countsRes = await GetAllLeadTabsCount(body);
      
      return {
        counts: countsRes?.data?.result?.[0] || {}
      };
    } catch (e) {
      console.error("Error fetching lead tab counts:", e);
      return { counts: {} };
    }
  };

  const fetchChartData = async (forceRefetch = false) => {
    if (isChartDataFetched && !forceRefetch) return;
    setIsChartLoading(true);
    try {
      const body = {
        SalespersonIds: selectedSalesPerson?.map((d) => d.id).toString(),
        Search: inputFields.join(),
        AssignmentFilter: assignmentFilter === "all" ? "" : assignmentFilter,
        CountryId: selectedCountry?.countryId || selectedCountry?.id || null,
        StartDate: startDateFormated,
        EndDate: endDateFormated,
      };
      
      const statsRes = await GetSalesPersonLeadStats(body);
      const stats = statsRes?.data?.result || [];
      
      const currentTabConfig = tabConfig.length > 0 ? tabConfig : await getLeadTabsConfig();

      const spChartsData = stats.map(spRaw => {
        return {
          name: spRaw.Name || spRaw.name,
          id: spRaw.Id || spRaw.id,
          tabs: buildTabsHelper(spRaw, currentTabConfig)
        };
      });
      setSalesPersonLeadStats(spChartsData);
      setIsChartDataFetched(true);
    } catch (e) {
      console.error("Error fetching chart data:", e);
    } finally {
      setIsChartLoading(false);
    }
  };

  const buildTabData = async () => {
    setIsTabsCountLoading(true);
    try {
      let config = await getLeadTabsConfig();

      if (!config || config.length === 0) {
        try {
          await GetAllClientLeadStatus({
            filterModel: { start: 0, length: 1000, search: "", sortDir: "desc", sortCol: 0 },
          });
          config = await getLeadTabsConfig();
        } catch (e) {
          console.error("Auto-sync lead statuses failed:", e);
        }
      }

      if (!config || config.length === 0) return;
      
      setTabConfig(config);

      const { counts } = await getLeadTabsCount(config);

      setLeadTabData(buildTabsHelper(counts, config));

      // If chart was previously fetched, we should update it with new filters
      if (isChartDataFetched) {
        fetchChartData(true);
      }
      
    } catch (e) {
      console.error("Error building tab data:", e);
    } finally {
      setIsTabsCountLoading(false);
    }
  };

  const getAllSalePersonForSelection = async () => {
    try {
      const res = await GetAllSalePersonForSelection();
      setAllSalesPerson(res?.data?.result || []);
    } catch (e) {
      console.error("Error fetching salespersons:", e);
    }
  };

  const getAllLeadStatusForSelection = async () => {
    try {
      const res = await GetAllClientLeadStatusForSelection();
      const statuses = res?.data?.result?.filter(
        (x) => x.description?.toLowerCase() !== "completed"
      ) || [];
      setAllLeadStatus(statuses);
    } catch (e) {
      console.error("Error fetching lead statuses:", e);
    }
  };

  // Drag reorder — mirrors shipments useEffect([updateShipmentTabs])
  useEffect(() => {
    if (updateLeadTabs.length > 0) {
      const reordered = updateLeadTabs.map((tab) => ({
        leadGridColumnId: tab?.leadGridColumnId || tab?.LeadGridColumnId,
        displayOrder: tab?.displayOrder,
      }));
      setIsMovingStatus(true);
      UpdateLeadTabDisplayOrder({ list: reordered })
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
          } else {
            setStatusMoved(true);
          }
        })
        .catch((e) => console.error("UpdateLeadTabDisplayOrder error", e))
        .finally(() => setIsMovingStatus(false));
    }
  }, [updateLeadTabs]);

  // Reload tabs after drag reorder or new tab added
  useEffect(() => {
    if (statusMoved) {
      buildTabData();
      getAllLead();
      setStatusMoved(false);
    }
  }, [statusMoved]);

  useEffect(() => {
    getAllSalePersonForSelection();
    getAllLeadStatusForSelection();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      getAllLead();
      buildTabData();
    }, 800);
    return () => clearTimeout(timeoutId);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [inputFields, startDateFormated, endDateFormated, selectedSalesPerson, assignmentFilter, selectedCountry]);

  useEffect(() => {
    if (isfilterClear) {
      resetDates();
      setIsfilterClear(false);
      buildTabData();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isfilterClear]);

  const handleFilterReset = () => {
    setStartDate(null);
    setEndDate(null);
    setSelectedSalesPerson(initialStateFilter.multiple);
    setSelectedCountry(null);
    setInputFields([]);
    setActiveTabStatusIds(""); // reset to All tab
  };

  const handleFilterClear = () => {
    handleFilterReset();
    setIsfilterClear(true);
  };

  // Called by GeneralTabBar when a tab is clicked — mirrors shipments
  const handleTabStatusChange = (statusIds) => {
    setActiveTabStatusIds(statusIds);
    getAllLead(statusIds);
  };

  const getRoutComponent = (
    <LeadsList
      loading={isAllListLoading}
      allLead={allLead}
      getOrdersRef={getOrdersRef}
      resetRowRef={resetRowRef}
      getAllLead={getAllLead}
      setSelectedLeads={setSelectedLeads}
      isFilterOpen={isFilterOpen}
      setOpenUpdateStatusModal={setOpenUpdateStatusModal}
      setSelectedLeadForStatus={setSelectedLeadForStatus}
    />
  );

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Box sx={{ mb: 1 }}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <Box flexGrow={1}>
              <SearchInputAutoCompleteMultiple
                onChange={(e, value) => {
                  if (value.length <= MAX_TAGS) {
                    setInputFields(value.slice(0, MAX_TAGS));
                  } else {
                    warningNotification(
                      LanguageReducer?.languageType?.MAXMIUM_NUMBER_REACHED ||
                      "Maximum tag limit reached"
                    );
                  }
                }}
                inputFields={inputFields}
                MAX_TAGS={MAX_TAGS}
              />
            </Box>
          </Box>
        </Box>

        <CustomColorLabelledOutline
          isCollapse={true}
          label="Salesperson Lead Summary"
          defaultCollapseState={false}
          onClick={() => {
            if (!isChartDataFetched) {
              fetchChartData();
            }
          }}
        >
          <Box display="flex" justifyContent="flex-end" mb={1}>
            <Button 
              variant="outlined" 
              size="small" 
              onClick={(e) => { e.stopPropagation(); fetchChartData(true); }} 
              startIcon={<RefreshIcon />}
              disabled={isChartLoading}
            >
              Refresh
            </Button>
          </Box>
          <Box sx={{ height: "350px", overflowY: "auto", overflowX: "hidden", p: 1 }}>
            {isChartLoading ? (
              <CicrlesLoading height="100%" />
            ) : (
              <Grid container spacing={2} sx={{ mb: 2 }}>
                {salesPersonLeadStats.map((sp, idx) => (
                  <Grid item xs={12} sm={6} md={3} lg={3} key={sp.id || idx}>
                    <LeadChart leadTabData={sp.tabs} isLoading={false} title={sp.name} />
                  </Grid>
                ))}
                {salesPersonLeadStats.length === 0 && (
                  <Grid item xs={12}>
                    <Box p={2} textAlign="center" color="text.secondary">No salesperson stats available</Box>
                  </Grid>
                )}
              </Grid>
            )}
          </Box>
        </CustomColorLabelledOutline>

        {/* Lead Status Tabs — same as GeneralTabBar in shipments */}
        <GeneralTabBar
          tabData={leadTabData}
          tabGridWidth={{ md: 7, lg: 7, sm: 12 }}
          btnGridWidth={{ md: 5, lg: 5, sm: 12 }}
          value={activeTabStatusIds}
          setTrackingStatusIds={setActiveTabStatusIds}
          getShipments={handleTabStatusChange}
          isFilterOpen={isFilterOpen}
          setIsFilterOpen={setIsFilterOpen}
          // Required by GeneralTabBar — mirrors shipments exactly
          setUpdateShipmentTabs={setUpdateLeadTabs}
          loading={isTabsCountLoading || isMovingStatus}
          placement={"bottom"}
          options={[
            { title: "Add Tab", value: "ADD_TAB" },
            { title: "Export Excel", value: "EXPORT_EXCEL" },
            { title: "Delete Leads", value: "DELETE_LEADS" }
          ]}
          onChangeMenu={(value) => {
            if (value === "ADD_TAB") {
              handleAddTab();
            }
            if (value === "EXPORT_EXCEL") {
              downloadExcel();
            }
            if (value === "DELETE_LEADS") {
              if (selectedLeads.length > 0) {
                setOpenDeleteConfirm(true);
              } else {
                warningNotification("Please select at least one lead to delete.");
              }
            }
          }}
          copybtn={
            !belowMdScreen ? (
              <Stack direction="row" spacing={1} alignItems="center">
                <ButtonComponent
                  btnMdWidth={"150px"}
                  btnLgWidth={"150px"}
                  bg={Colors.primary}
                  title={"Create Lead"}
                  onClick={() => setOpenCreateLeadModal(true)}
                />
                <ButtonComponent
                  btnMdWidth={"150px"}
                  btnLgWidth={"150px"}
                  bg={Colors.succes}
                  title={"Upload / Paste"}
                  onClick={() => setOpenImportLeadsModal(true)}
                />
                {selectedLeads.length > 0 && (
                  <ButtonComponent
                    btnMdWidth={"150px"}
                    btnLgWidth={"160px"}
                    bg={Colors.primary}
                    title={"Assign Salesperson"}
                    onClick={() => setOpenAssignSalesperson(true)}
                  />
                )}
              </Stack>
            ) : null
          }
        />

        {isFilterOpen ? (
          <Table size="small" sx={{ mt: 0.5 }}>
            <TableHead>
              <TableRow>
                <Grid container spacing={1} sx={{ p: 1 }}>
                  {/* Start Date */}
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <InputLabel sx={{ ...styleSheet.inputLabel, overflow: "unset" }}>
                        Start Date
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

                  {/* End Date */}
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <InputLabel sx={{ ...styleSheet.inputLabel, overflow: "unset" }}>
                        End Date
                      </InputLabel>
                      <CustomReactDatePickerInputFilter
                        value={endDate}
                        onClick={(date) => setEndDate(date)}
                        size="small"
                        minDate={startDate}
                        disabled={!startDate}
                        isClearable
                        maxDate={UtilityClass.todayDate()}
                      />
                    </Grid>
                  </Grid>

                  {/* Sales Person */}
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <InputLabel sx={{ ...styleSheet.inputLabel, overflow: "unset" }}>
                        Sales Person
                      </InputLabel>
                      <SelectComponent
                        multiple={true}
                        name="salesperson"
                        height={28}
                        options={allSalesPerson}
                        value={selectedSalesPerson}
                        optionLabel={"text"}
                        optionValue={"id"}
                        getOptionLabel={(option) => option?.text}
                        onChange={(e, val) => {
                          setSelectedSalesPerson(val);
                        }}
                      />
                    </Grid>
                  </Grid>

                  {/* Assignment Filter */}
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={6}>
                    <Grid>
                      <InputLabel sx={{ ...styleSheet.inputLabel, overflow: "unset" }}>
                        Assignment
                      </InputLabel>
                      <FormControl fullWidth size="small">
                        <Select
                          value={assignmentFilter}
                          onChange={(e) => setAssignmentFilter(e.target.value)}
                          sx={{ height: 28, fontSize: "14px", backgroundColor: "#fff" }}
                        >
                          <MenuItem value="all">All</MenuItem>
                          <MenuItem value="assigned">Assigned</MenuItem>
                          <MenuItem value="unassigned">Unassigned</MenuItem>
                        </Select>
                      </FormControl>
                    </Grid>
                  </Grid>

                  {/* Address Schema */}
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel sx={{ ...styleSheet.inputLabel, overflow: "unset" }}>
                        {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_COUNTRY || "Country"}
                      </InputLabel>
                      <CountrySchema
                        name="country"
                        required={false}
                        height={28}
                        value={selectedCountry}
                        onChange={(e, val) => {
                          setSelectedCountry(val);
                        }}
                      />
                    </Grid>
                  </Grid>

                  {/* Buttons */}
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Stack
                      alignItems="flex-end"
                      direction="row"
                      spacing={1}
                      sx={{ mt: 2.5 }}
                    >
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="outlined"
                        onClick={() => {
                          handleFilterClear();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER || "Clear Filter"}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllLead();
                          buildTabData();
                        }}
                      >
                        {LanguageReducer?.languageType?.FILTER || "Filter"}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}

        <Routes>
          <Route path="/" element={getRoutComponent} />
        </Routes>
      </div>

      <ImportUploadLeadsModal
        open={openImportLeadsModal}
        onClose={() => setOpenImportLeadsModal(false)}
        onSuccess={() => setStatusMoved(true)}
      />

      <CreateLeadModal
        open={openCreateLeadModal}
        onClose={() => setOpenCreateLeadModal(false)}
        onSuccess={() => setStatusMoved(true)}
      />

      <AssignSalespersonModal
        open={openAssignSalesperson}
        onClose={() => setOpenAssignSalesperson(false)}
        selectedLeads={selectedLeads}
        onSuccess={() => {
          getAllLead();
          setSelectedLeads([]);
          resetRowRef.current = true;
        }}
      />

      <AddLeadTabModal
        open={openAddTabModal}
        setOpen={setOpenAddTabModal}
        onSuccess={() => setStatusMoved(true)}
      />

      <UpdateLeadStatusModal
        open={openUpdateStatusModal}
        handleClose={() => {
          setOpenUpdateStatusModal(false);
          setSelectedLeadForStatus(null);
        }}
        lead={selectedLeadForStatus}
        onSuccess={() => {
          getAllLead(activeTabStatusIds);
          buildTabData();
        }}
      />

      {openDeleteConfirm && (
        <DeleteConfirmationModal
          open={openDeleteConfirm}
          setOpen={setOpenDeleteConfirm}
          loading={isDeleting}
          handleDelete={handleDeleteLeads}
          heading="Confirm Deletion"
          message={`Are you sure you want to delete ${selectedLeads.length} selected lead(s)? This will also delete their associated draft orders (actual orders will not be deleted).`}
          messageDetails=" This action cannot be undone."
          buttonText="Delete"
        />
      )}
    </Box>
  );
}

export default Leads;
