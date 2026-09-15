import React, { useMemo } from "react";
import {
  Box,
  Typography,
  Switch,
  TextField,
  Divider,
  Grid,
  FormControl,
  Select,
  MenuItem,
} from "@mui/material";
import { InventorySyncPolicy } from "../../models/InventorySyncPolicy";

export default function Step4SyncRules({
  formData,
  setFormData,
  syncDirections = [],
}) {
  const handleToggleChange = (field) => (e) => {
    setFormData((prev) => ({ ...prev, [field]: e.target.checked }));
  };

  const handleNumberChange = (field) => (e) => {
    const val = e.target.value === "" ? "" : Math.max(0, Number(e.target.value));
    setFormData((prev) => ({ ...prev, [field]: val }));
  };

  // Sample product label (picks first selected product SKU or TS-BLK-M)
  const sampleProductLabel = useMemo(() => {
    const selected = (formData.products || []).find(
      (p) => p.Selected && p.SKU && !/^[0-9a-fA-F-]{20,}$/.test(p.SKU)
    );
    return (
      selected?.SKU ||
      selected?.VariantSKU ||
      selected?.Title ||
      selected?.VariantName ||
      "TS-BLK-M"
    );
  }, [formData.products]);

  // Dynamic Combined Available Inventory across all selected items/stations
  const combinedAvailableInventory = useMemo(() => {
    const selectedProducts = (formData.products || []).filter((p) => p.Selected);
    const prodSum = selectedProducts.reduce(
      (sum, p) =>
        sum + Number(p.AvailableStock ?? p.QuantityAvailable ?? p.TotalQty ?? 0),
      0
    );

    if (prodSum > 0) return prodSum;

    // Fallback to selected stations available units
    const stationSum = (formData.stations || [])
      .filter((s) => s.Selected)
      .reduce((sum, s) => sum + Number(s.AvailableUnits || 0), 0);

    return stationSum > 0 ? stationSum : 78;
  }, [formData.products, formData.stations]);

  // Dynamic Live Calculation Preview
  const calculationResult = useMemo(() => {
    const available = combinedAvailableInventory;
    const buffer = Number(formData.bufferQuantity || 0);
    const minQty = Number(formData.minPublishQuantity || 0);
    const maxQty =
      formData.maxPublishQuantity === "" || formData.maxPublishQuantity === null
        ? null
        : Number(formData.maxPublishQuantity);

    const afterBuffer = Math.max(0, available - buffer);

    let published = afterBuffer;
    if (minQty > 0 && afterBuffer < minQty) {
      published = 0;
    }
    if (maxQty !== null && maxQty > 0 && published > maxQty) {
      published = maxQty;
    }

    return {
      availableStock: available,
      buffer: buffer,
      afterBuffer: afterBuffer,
      maxQty: maxQty,
      publishedQuantity: published,
    };
  }, [
    combinedAvailableInventory,
    formData.bufferQuantity,
    formData.minPublishQuantity,
    formData.maxPublishQuantity,
  ]);

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
        Inventory sync rules
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
        Control how Shipra converts available inventory into the quantity
        published on the selected sales channel.
      </Typography>

      {/* Main sync switches */}
      <Box sx={{ borderTop: "1px solid #eeeef2" }}>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            minHeight: "54px",
            borderBottom: "1px solid #eeeef2",
            py: 1,
          }}
        >
          <Box sx={{ flex: 1 }}>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
                mb: "3px",
              }}
            >
              Sync inventory
            </Typography>
            <Typography
              sx={{
                fontSize: "11.5px",
                color: "#7a7c85",
              }}
            >
              Push available stock from Shipra to the sales channel.
            </Typography>
          </Box>
          <Switch
            checked={Boolean(formData.syncInventory)}
            onChange={handleToggleChange("syncInventory")}
            sx={{
              "& .MuiSwitch-switchBase.Mui-checked": {
                color: "#BE212F",
                "& + .MuiSwitch-track": { backgroundColor: "#BE212F" },
              },
            }}
          />
        </Box>

        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            minHeight: "54px",
            borderBottom: "1px solid #eeeef2",
            py: 1,
          }}
        >
          <Box sx={{ flex: 1 }}>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
                mb: "3px",
              }}
            >
              Set unavailable inventory to zero
            </Typography>
            <Typography
              sx={{
                fontSize: "11.5px",
                color: "#7a7c85",
              }}
            >
              When available stock reaches the threshold, publish zero instead
              of leaving old stock on the channel.
            </Typography>
          </Box>
          <Switch
            checked={Boolean(formData.setUnavailableToZero)}
            onChange={handleToggleChange("setUnavailableToZero")}
            sx={{
              "& .MuiSwitch-switchBase.Mui-checked": {
                color: "#BE212F",
                "& + .MuiSwitch-track": { backgroundColor: "#BE212F" },
              },
            }}
          />
        </Box>
      </Box>

      {/* Rules inputs 3-column grid */}
      <Grid container spacing={1.5} sx={{ mt: 1 }}>
        <Grid item xs={12} sm={4}>
          <Box
            sx={{
              border: "1px solid #dfdfe6",
              borderRadius: "6px",
              p: 1.25,
              backgroundColor: "#fff",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#747781",
                mb: "5px",
                fontWeight: 700,
              }}
            >
              Safety Buffer Qty
            </Typography>
            <TextField
              fullWidth
              size="small"
              type="number"
              value={formData.bufferQuantity ?? 0}
              onChange={handleNumberChange("bufferQuantity")}
              inputProps={{ min: 0 }}
              sx={{
                "& .MuiOutlinedInput-root": {
                  height: "34px",
                  fontSize: "13px",
                  borderRadius: "4px",
                  "& fieldset": { borderColor: "#d4d4dc" },
                  "&.Mui-focused fieldset": { borderColor: "#BE212F" },
                },
              }}
            />
          </Box>
        </Grid>

        <Grid item xs={12} sm={4}>
          <Box
            sx={{
              border: "1px solid #dfdfe6",
              borderRadius: "6px",
              p: 1.25,
              backgroundColor: "#fff",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#747781",
                mb: "5px",
                fontWeight: 700,
              }}
            >
              Minimum Threshold Qty
            </Typography>
            <TextField
              fullWidth
              size="small"
              type="number"
              value={formData.minPublishQuantity ?? 0}
              onChange={handleNumberChange("minPublishQuantity")}
              inputProps={{ min: 0 }}
              sx={{
                "& .MuiOutlinedInput-root": {
                  height: "34px",
                  fontSize: "13px",
                  borderRadius: "4px",
                  "& fieldset": { borderColor: "#d4d4dc" },
                  "&.Mui-focused fieldset": { borderColor: "#BE212F" },
                },
              }}
            />
          </Box>
        </Grid>

        <Grid item xs={12} sm={4}>
          <Box
            sx={{
              border: "1px solid #dfdfe6",
              borderRadius: "6px",
              p: 1.25,
              backgroundColor: "#fff",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#747781",
                mb: "5px",
                fontWeight: 700,
              }}
            >
              Maximum Publish Qty
            </Typography>
            <TextField
              fullWidth
              size="small"
              type="number"
              placeholder="No limit"
              value={
                formData.maxPublishQuantity === null
                  ? ""
                  : formData.maxPublishQuantity
              }
              onChange={(e) => {
                const val = e.target.value;
                setFormData((prev) => ({
                  ...prev,
                  maxPublishQuantity: val === "" ? null : Math.max(0, Number(val)),
                }));
              }}
              inputProps={{ min: 0 }}
              sx={{
                "& .MuiOutlinedInput-root": {
                  height: "34px",
                  fontSize: "13px",
                  borderRadius: "4px",
                  "& fieldset": { borderColor: "#d4d4dc" },
                  "&.Mui-focused fieldset": { borderColor: "#BE212F" },
                },
              }}
            />
          </Box>
        </Grid>
      </Grid>

      {/* Live Calculation Preview Card */}
      <Box
        sx={{
          backgroundColor: "#fdf2f3",
          border: "1px solid #f8d7da",
          borderRadius: "7px",
          p: 1.75,
          mt: 2,
        }}
      >
        <Typography
          sx={{
            fontWeight: 700,
            fontSize: "12.5px",
            mb: 1.25,
            color: "#22242a",
          }}
        >
          Example calculation for TS-BLK-M
        </Typography>

        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            my: "6px",
            fontSize: "12px",
            color: "#666975",
          }}
        >
          <span>Combined available inventory</span>
          <strong style={{ fontWeight: 700, color: "#22242a" }}>
            {calculationResult.availableStock}
          </strong>
        </Box>

        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            my: "6px",
            fontSize: "12px",
            color: "#666975",
          }}
        >
          <span>Safety buffer</span>
          <strong style={{ fontWeight: 700, color: "#22242a" }}>
            − {calculationResult.buffer}
          </strong>
        </Box>

        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            my: "6px",
            fontSize: "12px",
            color: "#666975",
          }}
        >
          <span>After buffer</span>
          <strong style={{ fontWeight: 700, color: "#22242a" }}>
            {calculationResult.afterBuffer}
          </strong>
        </Box>

        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            my: "6px",
            fontSize: "12px",
            color: "#666975",
          }}
        >
          <span>Maximum published quantity</span>
          <strong style={{ fontWeight: 700, color: "#22242a" }}>
            {calculationResult.maxQty !== null && calculationResult.maxQty > 0
              ? calculationResult.maxQty
              : "No limit"}
          </strong>
        </Box>

        <Box
          sx={{
            borderTop: "1px solid #f5c6cb",
            pt: 1.25,
            mt: 1.25,
            display: "flex",
            justifyContent: "space-between",
            fontSize: "13.5px",
            color: "#BE212F",
            fontWeight: 800,
          }}
        >
          <span>
            Quantity sent to {formData.salesChannelName || "Amazon UAE"}
          </span>
          <span>{calculationResult.publishedQuantity}</span>
        </Box>
      </Box>

      <Divider sx={{ my: 2.5, borderColor: "#eeeef2" }} />

      {/* Dynamic Inventory Sync Direction Dropdown */}
      <Box sx={{ mb: 2.5 }}>
        <Typography
          sx={{
            fontSize: "13px",
            fontWeight: 700,
            color: "#22242a",
            mb: 0.75,
          }}
        >
          Inventory Sync Direction
        </Typography>
        <FormControl fullWidth size="small">
          <Select
            value={
              formData.inventorySyncDirectionId ||
              (syncDirections && syncDirections[0]
                ? syncDirections[0].inventorySyncDirectionId || syncDirections[0].InventorySyncDirectionId
                : 2)
            }
            onChange={(e) =>
              setFormData((prev) => ({
                ...prev,
                inventorySyncDirectionId: Number(e.target.value),
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
            {(syncDirections || []).map((dir) => {
              const dirId =
                dir.inventorySyncDirectionId ?? dir.InventorySyncDirectionId;
              const dirName = dir.name ?? dir.Name;
              return (
                <MenuItem
                  key={dirId}
                  value={dirId}
                  sx={{ fontSize: "13px" }}
                >
                  {dirName}
                </MenuItem>
              );
            })}
          </Select>
        </FormControl>
      </Box>

      {/* Other sync settings */}
      <Typography
        variant="h6"
        sx={{
          fontSize: "15px",
          fontWeight: 700,
          mb: 1,
          color: "#22242a",
        }}
      >
        Other sync settings
      </Typography>

      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          minHeight: "54px",
          borderBottom: "1px solid #eeeef2",
          py: 1,
        }}
      >
        <Box sx={{ flex: 1 }}>
          <Typography
            sx={{
              fontSize: "13px",
              fontWeight: 700,
              color: "#22242a",
              mb: "3px",
            }}
          >
            Sync product price
          </Typography>
          <Typography
            sx={{
              fontSize: "11.5px",
              color: "#7a7c85",
            }}
          >
            Shipra becomes the source for channel price updates.
          </Typography>
        </Box>
        <Switch
          checked={Boolean(formData.syncPrice)}
          onChange={handleToggleChange("syncPrice")}
          sx={{
            "& .MuiSwitch-switchBase.Mui-checked": {
              color: "#BE212F",
              "& + .MuiSwitch-track": { backgroundColor: "#BE212F" },
            },
          }}
        />
      </Box>

      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          minHeight: "54px",
          borderBottom: "1px solid #eeeef2",
          py: 1,
        }}
      >
        <Box sx={{ flex: 1 }}>
          <Typography
            sx={{
              fontSize: "13px",
              fontWeight: 700,
              color: "#22242a",
              mb: "3px",
            }}
          >
            Sync product information
          </Typography>
          <Typography
            sx={{
              fontSize: "11.5px",
              color: "#7a7c85",
            }}
          >
            Push title, description and product attributes when changed.
          </Typography>
        </Box>
        <Switch
          checked={Boolean(formData.syncProductInfo)}
          onChange={handleToggleChange("syncProductInfo")}
          sx={{
            "& .MuiSwitch-switchBase.Mui-checked": {
              color: "#BE212F",
              "& + .MuiSwitch-track": { backgroundColor: "#BE212F" },
            },
          }}
        />
      </Box>

      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          minHeight: "54px",
          py: 1,
        }}
      >
        <Box sx={{ flex: 1 }}>
          <Typography
            sx={{
              fontSize: "13px",
              fontWeight: 700,
              color: "#22242a",
              mb: "3px",
            }}
          >
            Sync product media
          </Typography>
          <Typography
            sx={{
              fontSize: "11.5px",
              color: "#7a7c85",
            }}
          >
            Keep images and media aligned with the Shipra catalog.
          </Typography>
        </Box>
        <Switch
          checked={Boolean(formData.syncMedia)}
          onChange={handleToggleChange("syncMedia")}
          sx={{
            "& .MuiSwitch-switchBase.Mui-checked": {
              color: "#BE212F",
              "& + .MuiSwitch-track": { backgroundColor: "#BE212F" },
            },
          }}
        />
      </Box>
    </Box>
  );
}
