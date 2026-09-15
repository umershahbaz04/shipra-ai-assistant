import React, { useMemo } from "react";
import {
  Box,
  Typography,
  Radio,
  Checkbox,
  FormControl,
  Select,
  MenuItem,
  Divider,
  Grid,
  TextField,
} from "@mui/material";
import { EnumStationSourceMode } from "../../models/InventorySyncPolicy";

export default function Step2InventorySource({
  formData,
  setFormData,
  syncModes = [],
}) {
  const handleModeChange = (modeId) => {
    setFormData((prev) => {
      const updatedStations = prev.stations.map((st) => ({
        ...st,
        Selected: modeId === EnumStationSourceMode.AllStations ? true : st.Selected,
      }));
      return {
        ...prev,
        stationSourceModeId: modeId,
        stations: updatedStations,
      };
    });
  };

  const handleStationToggle = (stationId) => (e) => {
    const isChecked = e.target.checked;
    setFormData((prev) => {
      const updatedStations = (prev.stations || []).map((st) =>
        st.ProductStationId === stationId ? { ...st, Selected: isChecked } : st
      );
      const allSelected =
        updatedStations.length > 0 && updatedStations.every((s) => s.Selected);
      return {
        ...prev,
        stationSourceModeId: allSelected
          ? EnumStationSourceMode.AllStations
          : EnumStationSourceMode.SelectedStations,
        stations: updatedStations,
      };
    });
  };

  const handlePriorityChange = (stationId) => (e) => {
    const newPriority = Number(e.target.value);
    setFormData((prev) => ({
      ...prev,
      stations: prev.stations.map((st) =>
        st.ProductStationId === stationId ? { ...st, Priority: newPriority } : st
      ),
    }));
  };

  const selectedStats = useMemo(() => {
    const selected = (formData.stations || []).filter((s) => s.Selected);
    const totalUnits = selected.reduce((sum, s) => sum + (s.AvailableUnits || 0), 0);
    return {
      count: selected.length,
      totalUnits,
    };
  }, [formData.stations]);

  const currentModeId = Number(
    formData.inventorySyncModeId && [3, 4, 5].includes(Number(formData.inventorySyncModeId))
      ? formData.inventorySyncModeId
      : (syncModes && syncModes[0]
          ? syncModes[0].inventorySyncModeId || syncModes[0].InventorySyncModeId
          : 3)
  );

  return (
    <Box>
      <Typography
        variant="h6"
        sx={{
          fontSize: "16px",
          fontWeight: 700,
          mb: 0.5,
          color: "#22242a",
        }}
      >
        Choose inventory source
      </Typography>
      <Typography
        variant="body2"
        sx={{
          fontSize: "13px",
          color: "#777a83",
          mb: 2.5,
          lineHeight: 1.45,
        }}
      >
        Select the Shipra Product Stations whose available inventory should be
        published to this sales channel.
      </Typography>

      <Grid container spacing={2}>
        <Grid item xs={12} sm={6}>
          <Box
            onClick={() => handleModeChange(EnumStationSourceMode.AllStations)}
            sx={{
              border: "1px solid",
              borderColor:
                formData.stationSourceModeId === EnumStationSourceMode.AllStations
                  ? "var(--primary-color)"
                  : "#d9d9e2",
              backgroundColor:
                formData.stationSourceModeId === EnumStationSourceMode.AllStations
                  ? "#fdf2f3"
                  : "#fff",
              borderRadius: "7px",
              p: 1.75,
              cursor: "pointer",
              display: "flex",
              alignItems: "flex-start",
              gap: 1.25,
              boxShadow:
                formData.stationSourceModeId === EnumStationSourceMode.AllStations
                  ? "0 0 0 1px var(--primary-color) inset"
                  : "none",
              transition: "all 0.15s ease",
              "&:hover": { borderColor: "var(--primary-color)" },
            }}
          >
            <Radio
              checked={
                formData.stationSourceModeId === EnumStationSourceMode.AllStations
              }
              size="small"
              sx={{
                p: 0,
                mt: "2px",
                color: "#999ba4",
                "&.Mui-checked": { color: "var(--primary-color)" },
              }}
            />
            <Box>
              <Typography
                sx={{
                  fontSize: "13px",
                  fontWeight: 700,
                  color: "#22242a",
                  mb: 0.5,
                }}
              >
                All Product Stations
              </Typography>
              <Typography
                sx={{
                  fontSize: "12px",
                  color: "#777a84",
                  lineHeight: 1.4,
                  m: 0,
                }}
              >
                Combine available stock from every active inventory location.
              </Typography>
            </Box>
          </Box>
        </Grid>

        <Grid item xs={12} sm={6}>
          <Box
            onClick={() =>
              handleModeChange(EnumStationSourceMode.SelectedStations)
            }
            sx={{
              border: "1px solid",
              borderColor:
                formData.stationSourceModeId ===
                  EnumStationSourceMode.SelectedStations
                  ? "var(--primary-color)"
                  : "#d9d9e2",
              backgroundColor:
                formData.stationSourceModeId ===
                  EnumStationSourceMode.SelectedStations
                  ? "#fdf2f3"
                  : "#fff",
              borderRadius: "7px",
              p: 1.75,
              cursor: "pointer",
              display: "flex",
              alignItems: "flex-start",
              gap: 1.25,
              boxShadow:
                formData.stationSourceModeId ===
                  EnumStationSourceMode.SelectedStations
                  ? "0 0 0 1px var(--primary-color) inset"
                  : "none",
              transition: "all 0.15s ease",
              "&:hover": { borderColor: "var(--primary-color)" },
            }}
          >
            <Radio
              checked={
                formData.stationSourceModeId ===
                EnumStationSourceMode.SelectedStations
              }
              size="small"
              sx={{
                p: 0,
                mt: "2px",
                color: "#999ba4",
                "&.Mui-checked": { color: "var(--primary-color)" },
              }}
            />
            <Box>
              <Typography
                sx={{
                  fontSize: "13px",
                  fontWeight: 700,
                  color: "#22242a",
                  mb: 0.5,
                }}
              >
                Selected Product Stations
              </Typography>
              <Typography
                sx={{
                  fontSize: "12px",
                  color: "#777a84",
                  lineHeight: 1.4,
                  m: 0,
                }}
              >
                Choose exactly which warehouses or stores can contribute stock.
              </Typography>
            </Box>
          </Box>
        </Grid>
      </Grid>

      {/* Dynamic Inventory Sync Mode / Allocation Strategy Dropdown */}
      <Box sx={{ mt: 2.5 }}>
        <Typography
          sx={{
            fontSize: "12.5px",
            fontWeight: 700,
            color: "#22242a",
            mb: 0.75,
          }}
        >
          Inventory Sync Mode (Allocation Strategy)
        </Typography>
        <FormControl fullWidth size="small">
          <Select
            value={currentModeId}
            onChange={(e) =>
              setFormData((prev) => ({
                ...prev,
                inventorySyncModeId: Number(e.target.value),
              }))
            }
            sx={{
              height: "38px",
              fontSize: "13px",
              borderRadius: "5px",
              backgroundColor: "#fff",
              "& fieldset": { borderColor: "#d8d8e2" },
              "&:hover fieldset": { borderColor: "var(--primary-color)" },
            }}
          >
            {(syncModes && syncModes.length > 0
              ? syncModes.filter((m) => {
                  const id = m.inventorySyncModeId ?? m.InventorySyncModeId;
                  const name = (m.name ?? m.Name ?? "").toLowerCase();
                  return id === 3 || id === 4 || id === 5 || ["percentage", "fixedorcapped", "dedicated"].includes(name);
                })
              : [
                  { inventorySyncModeId: 3, name: "Percentage" },
                  { inventorySyncModeId: 4, name: "FixedOrCapped" },
                  { inventorySyncModeId: 5, name: "Dedicated" },
                ]
            ).map((mode) => {
              const modeId =
                mode.inventorySyncModeId ?? mode.InventorySyncModeId;
              const modeName = mode.name ?? mode.Name;
              return (
                <MenuItem
                  key={modeId}
                  value={modeId}
                  sx={{ fontSize: "13px" }}
                >
                  {modeName}
                </MenuItem>
              );
            })}
          </Select>
        </FormControl>
      </Box>

      <Divider sx={{ my: 2.5, borderColor: "#eeeef2" }} />

      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          mb: 1.25,
        }}
      >
        <Typography
          sx={{
            fontSize: "13px",
            fontWeight: 700,
            color: "#22242a",
          }}
        >
          Product Stations
        </Typography>
        <Typography
          sx={{
            fontSize: "12px",
            color: "#858791",
          }}
        >
          {selectedStats.count} selected
        </Typography>
      </Box>

      {/* Station List */}
      <Box
        sx={{
          border: "1px solid #dcdce4",
          borderRadius: "6px",
          overflowY: "auto",
          maxHeight: "280px",
          backgroundColor: "#fff",
          "&::-webkit-scrollbar": { width: "6px" },
          "&::-webkit-scrollbar-track": { backgroundColor: "#f5f5f8" },
          "&::-webkit-scrollbar-thumb": {
            backgroundColor: "#d5d5df",
            borderRadius: "3px",
            "&:hover": { backgroundColor: "#b5b5c2" },
          },
        }}
      >
        {(formData.stations || []).map((station, idx) => (
          <Box
            key={station.ProductStationId}
            sx={{
              minHeight: "50px",
              borderBottom:
                idx === formData.stations.length - 1
                  ? "none"
                  : "1px solid #eeeef2",
              display: "grid",
              gridTemplateColumns: "34px 1fr 110px",
              alignItems: "center",
              px: 1.5,
              py: 0.75,
              backgroundColor: station.Selected ? "#fff" : "#fafafa",
              "&:hover": { backgroundColor: "#fff8f8" },
            }}
          >
            <Checkbox
              checked={Boolean(station.Selected)}
              onChange={handleStationToggle(station.ProductStationId)}
              size="small"
              sx={{
                p: 0,
                color: "#999ba4",
                "&.Mui-checked": { color: "#BE212F" },
              }}
            />
            <Box>
              <Typography
                sx={{
                  fontSize: "13px",
                  fontWeight: 700,
                  color: "#22242a",
                  lineHeight: 1.2,
                }}
              >
                {station.StationName}
              </Typography>
              <Typography
                sx={{
                  fontSize: "11px",
                  color: "#898b94",
                  mt: "2px",
                }}
              >
                {station.StationCode}{" "}
                {station.StationTypeName ? `· ${station.StationTypeName}` : ""}
              </Typography>
            </Box>

            <Box sx={{ textAlign: "right" }}>
              <Typography
                sx={{
                  fontSize: "13px",
                  fontWeight: 700,
                  color: "#22242a",
                  lineHeight: 1.2,
                }}
              >
                {Number(station.AvailableUnits).toLocaleString()}
              </Typography>
              <Typography
                sx={{
                  fontSize: "10px",
                  color: "#91939b",
                  fontWeight: 500,
                  display: "block",
                }}
              >
                available units
              </Typography>
            </Box>
          </Box>
        ))}
      </Box>

      {/* Summary bar */}
      <Box
        sx={{
          mt: 1.75,
          backgroundColor: "#f7f7fa",
          border: "1px solid #e1e1e8",
          borderRadius: "6px",
          p: "12px 14px",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
        }}
      >
        <Box>
          <Typography
            sx={{
              fontSize: "13.5px",
              fontWeight: 700,
              color: "#22242a",
            }}
          >
            Combined inventory source
          </Typography>
          <Typography
            sx={{
              fontSize: "11px",
              color: "#858791",
              mt: "3px",
            }}
          >
            Calculated from the selected stations
          </Typography>
        </Box>
        <Typography
          sx={{
            color: "var(--primary-color)",
            fontSize: "16px",
            fontWeight: 800,
          }}
        >
          {selectedStats.totalUnits.toLocaleString()} units
        </Typography>
      </Box>
    </Box>
  );
}
