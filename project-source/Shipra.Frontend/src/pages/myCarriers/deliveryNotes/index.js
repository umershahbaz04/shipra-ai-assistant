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
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes } from "react-router-dom";
import { styleSheet } from "../../../assets/styles/style";
import GeneralTabBar from "../../../components/shared/tabsBar";
import NotesList from "./notesList";
import {
  GetAllDeliveryNote,
  GetDriversForSelection,
} from "../../../api/AxiosInterceptors";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SearchInputAutoCompleteMultiple from "../../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { EnumOptions } from "../../../utilities/enum";
import UtilityClass from "../../../utilities/UtilityClass";
import { warningNotification } from "../../../utilities/toast";

import MergeDeliveryNotesModal from "../../../components/modals/myCarrierModals/MergeDeliveryNotesModal";

const MAX_TAGS = 200;

const EnumTabFilter = Object.freeze({
  All: "/delivery-notes",
  InProgress: "/delivery-notes/inprogress",
  Completed: "/delivery-notes/completed",
});

function DeliveryNotes(props) {
  let defaultValues = {
    startDate: null,
    endDate: null,
    deliveryNoteStatusId: 0,
  };
  const [inputFields, setInputFields] = useState([]);
  const [allDrivers, setAllDrivers] = useState([]);
  const [driverId, setDriverId] = useState([]);
  const [selectedNoteIds, setSelectedNoteIds] = useState([]);
  const [openMergeModal, setOpenMergeModal] = useState(false);

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
  const [isTabFilter, setIsTabFilter] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allDeliveryNote, setAllDeliveryNote] = useState([]);
  const [isAllListLoading, setIsAllListLoading] = useState(false);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const [deliveryNoteStatusId, setDeliveryNoteStatusId] = useState(
    defaultValues.deliveryNoteStatusId
  );

  const selectedNotesData = (allDeliveryNote?.list || []).filter((note) =>
    selectedNoteIds.includes(note.DeliveryNoteId)
  );

  const handleMergeClick = () => {
    if (selectedNotesData.length < 2) {
      warningNotification("Please select at least 2 delivery notes to merge.");
      return;
    }

    const uniqueDrivers = new Set(
      selectedNotesData.map((note) => note.DriverId || note.DriverName)
    );
    if (uniqueDrivers.size > 1) {
      warningNotification("Selected delivery notes must belong to the same driver.");
      return;
    }

    setOpenMergeModal(true);
  };

  const getDriversForSelection = async () => {
    try {
      let res = await GetDriversForSelection();
      if (res?.data?.result != null) {
        setAllDrivers(res.data.result);
      }
    } catch (e) {
      console.error(e);
    }
  };

  useEffect(() => {
    getDriversForSelection();
  }, []);

  const resetFilter = () => {
    setDeliveryNoteStatusId(defaultValues.deliveryNoteStatusId);
    setStartDate(defaultValues.startDate);
    setEndDate(defaultValues.endDate);
    setDriverId([]);
    setInputFields([]);
    resetDates();
  };

  const getFiltersFromState = () => {
    let search = inputFields.join(",");
    let filters = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 1000,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
      deliveryNoteStatusId: deliveryNoteStatusId,
      DriverIds: Array.isArray(driverId)
        ? driverId.map((data) => data?.DriverId).toString()
        : "",
    };
    return filters;
  };

  let getAllDeliveryNote = async () => {
    setIsAllListLoading(true);
    let params = getFiltersFromState();
    let res = await GetAllDeliveryNote(params);
    if (res.data.result !== null) {
      setAllDeliveryNote(res.data.result);
    }
    setIsAllListLoading(false);
    resetRowRef.current = false;
  };

  useEffect(() => {
    getAllDeliveryNote();
  }, []);

  useEffect(() => {
    let timeoutId;
    if (inputFields.length > 0) {
      setIsAllListLoading(true);
      timeoutId = setTimeout(() => {
        getAllDeliveryNote();
      }, 500);
    } else {
      getAllDeliveryNote();
    }
    return () => {
      clearTimeout(timeoutId);
    };
  }, [inputFields, startDateFormated, endDateFormated, driverId]);

  const handleTabChange = (event, filterValue) => {
    resetFilter();
    setIsFilterOpen(false);
    if (filterValue === EnumTabFilter.All) {
      setDeliveryNoteStatusId(0);
    } else if (filterValue == EnumTabFilter.InProgress) {
      setDeliveryNoteStatusId(1);
    } else if (filterValue == EnumTabFilter.Completed) {
      setDeliveryNoteStatusId(2);
    }
    resetRowRef.current = true;
    setIsTabFilter(true);
  };

  useEffect(() => {
    if (isTabFilter) {
      getAllDeliveryNote();
      setIsTabFilter(false);
    }
  }, [deliveryNoteStatusId]);

  const getlNaviagiotnComponent = () => {
    return (
      <NotesList
        loading={isAllListLoading}
        allDeliveryNote={allDeliveryNote}
        getOrdersRef={getOrdersRef}
        resetRowRef={resetRowRef}
        getAllDeliveryNote={getAllDeliveryNote}
        selectedNoteIds={selectedNoteIds}
        setSelectedNoteIds={setSelectedNoteIds}
      />
    );
  };

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Box
          sx={{
            background: "#F8F8F8",
            border: "1px solid rgba(224, 224, 224, 1)",
            borderBottom: "none",
            borderRadius: "8px 8px 0px 0px!important",
            paddingTop: "5px",
          }}
        >
          <Box
            sx={{ display: "flex", alignItems: "center", gap: 1, pr: "7px" }}
          >
            <Box flexGrow={1}>
              <SearchInputAutoCompleteMultiple
                onChange={(e, value) => {
                  if (value.length <= MAX_TAGS) {
                    setInputFields(value.slice(0, MAX_TAGS));
                  } else {
                    warningNotification(
                      LanguageReducer?.languageType?.MAXMIUM_NUMBER_REACHED
                    );
                  }
                }}
                inputFields={inputFields}
                MAX_TAGS={MAX_TAGS}
              />
            </Box>
          </Box>
        </Box>
        <GeneralTabBar
          handleTabChange={handleTabChange}
          isFilterOpen={isFilterOpen}
          setIsFilterOpen={setIsFilterOpen}
          handleFilterBtnOnClick={() => setIsFilterOpen(!isFilterOpen)}
          extraBtn={
            selectedNoteIds?.length >= 2 ? (
              <Button
                sx={{
                  ...styleSheet.filterIconColord,
                  minWidth: "110px",
                  bgcolor: "var(--primary-color) !important",
                  color: "#ffffff !important",
                  borderColor: "var(--primary-color) !important",
                  "&:hover": { bgcolor: "#452bb5 !important" },
                }}
                variant="contained"
                onClick={handleMergeClick}
              >
                Merge Notes
              </Button>
            ) : null
          }
          tabData={[
            {
              label:
                LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_All,
              route: EnumTabFilter.All,
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.MY_CARRIER_DELIVERY_NOTES_IN_PROGRESS,
              route: EnumTabFilter.InProgress,
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.MY_CARRIER_DELIVERY_NOTES_COMPLETED,
              route: EnumTabFilter.Completed,
            },
          ]}
          {...props}
          disableFilter={false}
          disableSearch
        />
        {openMergeModal && (
          <MergeDeliveryNotesModal
            open={openMergeModal}
            setOpen={setOpenMergeModal}
            selectedNotesData={selectedNotesData}
            getAllDeliveryNote={getAllDeliveryNote}
            resetRowRef={resetRowRef}
          />
        )}
        {isFilterOpen ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <TableRow>
                <Grid container spacing={2} sx={{ p: "15px" }}>
                  <Grid item xl={3} lg={3} md={4} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_START_DATE
                        }
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
                  <Grid item xl={3} lg={3} md={4} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_END_DATE
                        }
                      </InputLabel>
                      <CustomReactDatePickerInputFilter
                        value={endDate}
                        onClick={(date) => setEndDate(date)}
                        size="small"
                        isClearable
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={3} lg={3} md={4} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.MY_CARRIER_DELIVERY_TASKS_DRIVER || "Driver"
                        }
                      </InputLabel>
                      <SelectComponent
                        multiple={true}
                        name="driver"
                        options={allDrivers}
                        height={28}
                        value={driverId}
                        optionLabel={EnumOptions.DRIVER.LABEL}
                        optionValue={EnumOptions.DRIVER.VALUE}
                        getOptionLabel={(option) => option?.DriverName}
                        onChange={(e, val) => {
                          setDriverId(val || []);
                        }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={3} lg={3} md={4} sm={6} xs={12}>
                    <Stack
                      alignItems="flex-end"
                      justifyContent="flex-end"
                      direction="row"
                      spacing={1}
                      sx={{ height: "100%", pt: 2 }}
                    >
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          resetFilter();
                          getAllDeliveryNote();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER || "Clear Filter"}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllDeliveryNote();
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
          <Route path="/" element={getlNaviagiotnComponent()} />
          <Route path="/inprogress" element={getlNaviagiotnComponent()} />
          <Route path="/completed" element={getlNaviagiotnComponent()} />
        </Routes>
      </div>
    </Box>
  );
}
export default DeliveryNotes;
