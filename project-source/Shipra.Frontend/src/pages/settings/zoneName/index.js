import React, { useEffect, useState, useCallback, useRef } from "react";
import {
  Box,
  Card,
  CardContent,
  Stack,
  Typography,
  Chip,
  CircularProgress,
  Alert,
  IconButton,
  Tooltip,
  Table,
  TableHead,
  TableRow,
  Grid,
  InputLabel,
  Button,
} from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import RefreshIcon from "@mui/icons-material/Refresh";
import GestureIcon from "@mui/icons-material/Gesture";
import ClearIcon from "@mui/icons-material/Clear";
import SaveIcon from "@mui/icons-material/Save";

import {
  GoogleMap,
  Polygon,
  Marker,
  OverlayView,
  useJsApiLoader,
} from "@react-google-maps/api";

import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import useWindowDimensions from "../../../utilities/customHooks/useWindowDimensions";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  GetAllZonesWithCoords,
  SaveNewZone,
  UpdateZoneFromZoneBoundries,
  DeleteZoneByID,
  GetAllCities,
} from "../../../api/AxiosInterceptors";
import {
  fetchMethod,
  handleGetAndSetMapApiKey,
  useGetMapApiKeyReducer,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import ZoneModal from "./ZoneModal";
import ZoneMapComponent from "../../../.reUseableComponents/Map/ZoneMapComponent";

const DEFAULT_CENTER = { lat: 25.276987, lng: 55.296249 }; // UAE default coordinates

const getPolygonCentroid = (coords) => {
  if (!coords || coords.length === 0) return DEFAULT_CENTER;
  let latSum = 0;
  let lngSum = 0;
  coords.forEach((pt) => {
    latSum += pt.lat;
    lngSum += pt.lng;
  });
  return {
    lat: latSum / coords.length,
    lng: lngSum / coords.length,
  };
};

const ZoneNamePage = () => {
  const dispatch = useDispatch();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const mapApiKey = useGetMapApiKeyReducer();
  const { height: windowHeight } = useWindowDimensions();

  // Dynamic Date Range Hook
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  const [loading, setLoading] = useState(false);
  const [zones, setZones] = useState([]);
  const [cities, setCities] = useState([]);
  const [selectedZone, setSelectedZone] = useState(null);
  const [isDrawingMode, setIsDrawingMode] = useState(false);
  const [drawnCoords, setDrawnCoords] = useState(null);
  const [isFilterOpen, setIsFilterOpen] = useState(false);

  // Modal State
  const [modalState, setModalState] = useState({
    open: false,
    isEdit: false,
    initialData: null,
  });

  const mapRef = useRef(null);

  // Compute dynamic map height so page does not scroll
  const computedMapHeight = Math.max(
    350,
    windowHeight - (isFilterOpen ? 285 : 155) - (isDrawingMode ? 45 : 0),
  );

  const mapContainerStyle = {
    width: "100%",
    height: `${computedMapHeight}px`,
  };

  // Fetch API Key if missing
  useEffect(() => {
    if (!mapApiKey) {
      handleGetAndSetMapApiKey(dispatch);
    }
  }, [dispatch, mapApiKey]);

  // Fetch Cities lookup
  const fetchCities = useCallback(async () => {
    try {
      const res = await GetAllCities();
      const list =
        res?.data?.result?.list ||
        res?.data?.result?.List ||
        res?.data?.List ||
        res?.data?.list ||
        (Array.isArray(res?.data?.result) ? res.data.result : []);
      setCities(list);
    } catch (err) {
      console.error("Error fetching cities:", err);
    }
  }, []);

  // Fetch Zones with Polygon Coords
  const fetchZones = useCallback(async () => {
    setLoading(true);
    try {
      const { response } = await fetchMethod(() => GetAllZonesWithCoords());
      console.log(response);
      if (response?.isSuccess || response?.result || response?.Result) {
        const rawZones =
          response?.result?.zones ||
          response?.result?.Zones ||
          response?.zones ||
          response?.Zones ||
          (Array.isArray(response?.result) ? response.result : []);
        const zoneList = rawZones.map((z) => {
          let parsedCoords = [];
          try {
            parsedCoords =
              typeof z.coords === "string"
                ? JSON.parse(z.coords)
                : z.coords || z.Coords || [];
          } catch (e) {
            console.error("Failed to parse zone coords:", e);
          }
          return {
            ...z,
            parsedCoords,
          };
        });
        setZones(zoneList);
      }
    } catch (err) {
      console.error("Error fetching zones:", err);
      errorNotification("Failed to load zone boundaries.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchZones();
    fetchCities();
  }, [fetchZones, fetchCities]);

  const handleFilterClear = () => {
    resetDates();
  };

  // Handle map load
  const onMapLoad = useCallback((map) => {
    mapRef.current = map;
  }, []);

  // Handle direct map click during drawing mode
  const handleMapClick = useCallback(
    (e) => {
      if (!isDrawingMode || !e.latLng) return;
      const newPoint = { lat: e.latLng.lat(), lng: e.latLng.lng() };
      setDrawnCoords((prev) => [...(prev || []), newPoint]);
    },
    [isDrawingMode],
  );

  // Polygon Drawing Complete Callback
  const handlePolygonComplete = useCallback((polygon) => {
    try {
      const path = polygon.getPath();
      const coords = [];
      for (let i = 0; i < path.getLength(); i++) {
        const pt = path.getAt(i);
        coords.push({ lat: pt.lat(), lng: pt.lng() });
      }
      polygon.setMap(null);
      setDrawnCoords(coords);
      setIsDrawingMode(false);
      setModalState({
        open: true,
        isEdit: false,
        initialData: null,
      });
    } catch (e) {
      console.error("Error in handlePolygonComplete:", e);
    }
  }, []);

  // Save New Zone / Update Zone
  const handleSaveZone = async ({ zoneName, cityId }) => {
    setLoading(true);
    try {
      if (selectedZone) {
        // Update existing zone (include updated coords if edited on map)
        const currentCoords =
          selectedZone.editedCoords || selectedZone.parsedCoords || [];
        const zoneId = selectedZone.zoneId || selectedZone.ZoneId;
        const name = zoneName || selectedZone.name || selectedZone.Name || "";
        const city = cityId ?? selectedZone.cityID ?? selectedZone.CityID ?? selectedZone.cityId ?? 0;
        const payload = {
          ZoneId: zoneId,
          Name: name,
          CityID: city,
          Coords:
            currentCoords.length > 0
              ? JSON.stringify(currentCoords)
              : typeof selectedZone.coords === "string"
              ? selectedZone.coords
              : JSON.stringify(selectedZone.coords || selectedZone.Coords || []),
        };
        const res = await SaveNewZone(payload);
        const isSuccess =
          res?.status === 200 &&
          (res?.data?.isSuccess ||
            res?.data?.Result ||
            res?.data?.result?.result ||
            res?.data?.result === true ||
            res?.data?.statusCode === 200);
        if (isSuccess) {
          successNotification("Zone updated successfully.");
          fetchZones();
          setSelectedZone(null);
        } else {
          errorNotification(
            res?.data?.result?.errorMsg ||
              res?.data?.ErrorMsg ||
              res?.data?.errMsg ||
              "Failed to update zone.",
          );
        }
      } else {
        // Create new zone
        if (!drawnCoords || drawnCoords.length === 0) {
          errorNotification("No zone boundaries drawn on map.");
          return;
        }
        const payload = {
          CityID: cityId,
          Name: zoneName,
          Coords: JSON.stringify(drawnCoords),
        };
        const res = await SaveNewZone(payload);
        const isSuccess =
          res?.status === 200 &&
          (res?.data?.isSuccess ||
            res?.data?.Result ||
            res?.data?.result?.result ||
            res?.data?.result === true ||
            res?.data?.statusCode === 200);
        if (isSuccess) {
          successNotification("Zone created successfully.");
          fetchZones();
          setDrawnCoords(null);
        } else {
          errorNotification(
            res?.data?.result?.errorMsg ||
              res?.data?.ErrorMsg ||
              "Failed to save new zone.",
          );
        }
      }
    } catch (err) {
      console.error("Error saving/updating zone:", err);
      errorNotification("Error saving zone.");
    } finally {
      setLoading(false);
      setModalState({ open: false, isEdit: false, initialData: null });
    }
  };

  // Delete Zone
  const handleDeleteZone = async () => {
    if (!selectedZone) return;
    const zoneId = selectedZone.zoneId || selectedZone.ZoneId;
    if (
      window.confirm(
        `Are you sure you want to delete zone "${selectedZone.name || selectedZone.Name}"?`,
      )
    ) {
      setLoading(true);
      try {
        const res = await DeleteZoneByID(zoneId);
        if (res?.data?.result || res?.data?.Result || res?.data?.isSuccess) {
          successNotification("Zone deleted successfully.");
          setSelectedZone(null);
          fetchZones();
        } else {
          errorNotification(res?.data?.errMsg || "Failed to delete zone.");
        }
      } catch (err) {
        errorNotification("Error deleting zone.");
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {/* DataGridTabs Component with Zone Management Tab & Inner Buttons */}
        <DataGridTabs
          handleFilterBtnOnClick={() => setIsFilterOpen(!isFilterOpen)}
          isRouteChangeable={false}
          selectedTab="/zone"
          tabsSmWidth="10px"
          tabsMdWidth="10px"
          tabData={[
            {
              label: LanguageReducer?.languageType?.SETTING_ZONE || "Zone",
              value: "/zone",
              route: "/zone",
            },
          ]}
          otherBtns={
            <Stack direction="row" spacing={1} alignItems="center">
              {/* Display Selected Zone Info & Icon Buttons inside Tab Bar */}
              {selectedZone && (
                <Stack direction="row" alignItems="center" spacing={0.5}>
                  <Chip
                    label={`Selected: ${selectedZone.name || selectedZone.Name}`}
                    color="primary"
                    variant="outlined"
                    size="small"
                    sx={{ fontWeight: "bold" }}
                  />
                  <Tooltip title="Save Boundary Changes">
                    <IconButton
                      size="small"
                      color="success"
                      onClick={() => {
                        const name = selectedZone.name || selectedZone.Name || "";
                        const cityId = selectedZone.cityID || selectedZone.CityID || selectedZone.cityId || 0;
                        handleSaveZone({ zoneName: name, cityId });
                      }}
                    >
                      <SaveIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Edit Zone">
                    <IconButton
                      size="small"
                      color="primary"
                      onClick={() =>
                        setModalState({
                          open: true,
                          isEdit: true,
                          initialData: selectedZone,
                        })
                      }
                    >
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Delete Zone">
                    <IconButton
                      size="small"
                      color="error"
                      onClick={handleDeleteZone}
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Clear Selection">
                    <IconButton
                      size="small"
                      onClick={() => setSelectedZone(null)}
                    >
                      <ClearIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                </Stack>
              )}
              <ButtonComponent
                title={isDrawingMode ? "Cancel Drawing" : "Draw New Zone"}
                startIcon={isDrawingMode ? <ClearIcon /> : <GestureIcon />}
                variant={isDrawingMode ? "outlined" : "contained"}
                color={isDrawingMode ? "error" : "primary"}
                onClick={() => {
                  setIsDrawingMode((prev) => !prev);
                  if (selectedZone) setSelectedZone(null);
                }}
                handleOnClick={() => {
                  setIsDrawingMode((prev) => !prev);
                  if (selectedZone) setSelectedZone(null);
                }}
              />
            </Stack>
          }
          filterData={
            isFilterOpen ? (
              <Table
                sx={{ ...styleSheet.generalFilterArea }}
                size="small"
                aria-label="zone-filter-table"
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
                            {"Create From"}
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
                            {"Create To"}
                          </InputLabel>
                          <CustomReactDatePickerInputFilter
                            maxDate={UtilityClass.todayDate()}
                            value={endDate}
                            onClick={(date) => setEndDate(date)}
                            size="small"
                            minDate={startDate}
                            disabled={!startDate}
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
                            onClick={handleFilterClear}
                          >
                            {LanguageReducer?.languageType?.CLEAR_FILTER ||
                              "Clear"}
                          </Button>
                          <Button
                            sx={{
                              ...styleSheet.filterIcon,
                              minWidth: "100px",
                            }}
                            variant="contained"
                            onClick={fetchZones}
                          >
                            {LanguageReducer?.languageType?.FILTER || "Filter"}
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

        {/* Drawing Mode Instruction Alert */}
        {isDrawingMode && (
          <Alert
            severity="info"
            sx={{ my: 0.5, py: 0.2, display: "flex", alignItems: "center" }}
            action={
              <Stack direction="row" spacing={1} alignItems="center">
                {drawnCoords && drawnCoords.length >= 3 && (
                  <Button
                    size="small"
                    color="success"
                    variant="contained"
                    onClick={() => {
                      setIsDrawingMode(false);
                      setModalState({
                        open: true,
                        isEdit: false,
                        initialData: null,
                      });
                    }}
                  >
                    Complete Shape ({drawnCoords.length} points)
                  </Button>
                )}
                {drawnCoords && drawnCoords.length > 0 && (
                  <Button
                    size="small"
                    color="inherit"
                    variant="outlined"
                    onClick={() => setDrawnCoords([])}
                  >
                    Reset Points
                  </Button>
                )}
              </Stack>
            }
          >
            Click on the map to add polygon boundary points.
            {drawnCoords?.length > 0
              ? ` (${drawnCoords.length} points added)`
              : ""}
          </Alert>
        )}

        {/* Interactive Google Map Container - Connected seamlessly with DataGridTabs */}
        <Card
          sx={{
            border: "1px solid rgba(224, 224, 224, 1)",
            borderTop: "none",
            borderRadius: "0px 0px 8px 8px",
            boxShadow: "none",
            overflow: "hidden",
          }}
        >
          <CardContent
            sx={{ p: 0, position: "relative", "&:last-child": { pb: 0 } }}
          >
            <ZoneMapComponent
              zones={zones}
              selectedZone={selectedZone}
              setSelectedZone={setSelectedZone}
              isDrawingMode={isDrawingMode}
              drawnCoords={drawnCoords}
              handleMapClick={handleMapClick}
              mapContainerStyle={mapContainerStyle}
              zoom={6}
              onMapLoad={onMapLoad}
              loading={loading}
            />
          </CardContent>
        </Card>

        {/* Modal Dialog for Zone Creation / Edit */}
        <ZoneModal
          open={modalState.open}
          onClose={() =>
            setModalState({ open: false, isEdit: false, initialData: null })
          }
          onSave={handleSaveZone}
          cities={cities}
          initialData={modalState.initialData}
          isEdit={modalState.isEdit}
        />
      </div>
    </Box>
  );
};

export default ZoneNamePage;
