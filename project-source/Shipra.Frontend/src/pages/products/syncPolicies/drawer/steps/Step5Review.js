import React, { useMemo } from "react";
import { Box, Typography, Grid } from "@mui/material";
import { EnumInventorySyncMode, EnumStationSourceMode } from "../../models/InventorySyncPolicy";

export default function Step5Review({ formData }) {
  const selectedStationDetails = useMemo(() => {
    const selectedStations = (formData.stations || []).filter((s) => s.Selected);
    const names = selectedStations.map((s) => s.StationName);

    let totalUnits = 0;
    if (formData.inventorySyncModeId === EnumInventorySyncMode.AllProducts) {
      // Sum overall available units across selected stations for All Products mode
      totalUnits = selectedStations.reduce(
        (sum, s) => sum + (s.AvailableUnits || 0),
        0
      );
    } else {
      // Sum available units of ONLY selected products chosen in Step 3 for Selected Products mode
      const selectedProducts = (formData.products || []).filter((p) => p.Selected);
      totalUnits = selectedProducts.reduce(
        (sum, p) =>
          sum + Number(p.AvailableStock ?? p.QuantityAvailable ?? p.TotalQty ?? 0),
        0
      );
    }

    return {
      namesText: names.length > 0 ? names.join(" + ") : "All Product Stations",
      totalUnits,
    };
  }, [formData.stations, formData.products, formData.inventorySyncModeId]);

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
        Review & activate
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
        Confirm the policy before Shipra starts applying it to channel products.
      </Typography>

      {/* Review Card 1: Policy */}
      <Box
        sx={{
          border: "1px solid #dddde5",
          borderRadius: "7px",
          mb: 1.5,
          overflow: "hidden",
          backgroundColor: "#fff",
        }}
      >
        <Box
          sx={{
            height: "38px",
            backgroundColor: "#fafafa",
            borderBottom: "1px solid #eeeef2",
            px: 1.5,
            display: "flex",
            alignItems: "center",
            fontWeight: 700,
            fontSize: "12px",
            color: "#22242a",
          }}
        >
          Policy
        </Box>
        <Grid container>
          <Grid
            item
            xs={6}
            sx={{
              p: "11px 12px",
              borderBottom: "1px solid #f1f1f4",
              borderRight: "1px solid #f1f1f4",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Policy Name
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.policyName || "Untitled Policy"}
            </Typography>
          </Grid>

          <Grid
            item
            xs={6}
            sx={{ p: "11px 12px", borderBottom: "1px solid #f1f1f4" }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Sales Channel
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.salesChannelName || "Amazon UAE"}
            </Typography>
          </Grid>

          <Grid
            item
            xs={6}
            sx={{
              p: "11px 12px",
              borderRight: "1px solid #f1f1f4",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Scope
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.inventorySyncModeId === EnumInventorySyncMode.AllProducts
                ? "All products"
                : `${(formData.products || []).filter((p) => p.Selected).length} selected products`}
            </Typography>
          </Grid>

          <Grid item xs={6} sx={{ p: "11px 12px" }}>
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Status after save
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#1b7e4d",
              }}
            >
              Active
            </Typography>
          </Grid>
        </Grid>
      </Box>

      {/* Review Card 2: Inventory Source */}
      <Box
        sx={{
          border: "1px solid #dddde5",
          borderRadius: "7px",
          mb: 1.5,
          overflow: "hidden",
          backgroundColor: "#fff",
        }}
      >
        <Box
          sx={{
            height: "38px",
            backgroundColor: "#fafafa",
            borderBottom: "1px solid #eeeef2",
            px: 1.5,
            display: "flex",
            alignItems: "center",
            fontWeight: 700,
            fontSize: "12px",
            color: "#22242a",
          }}
        >
          Inventory Source
        </Box>
        <Grid container>
          <Grid
            item
            xs={6}
            sx={{
              p: "11px 12px",
              borderBottom: "1px solid #f1f1f4",
              borderRight: "1px solid #f1f1f4",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Source Type
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.stationSourceModeId === EnumStationSourceMode.AllStations
                ? "All Product Stations"
                : "Selected Product Stations"}
            </Typography>
          </Grid>

          <Grid
            item
            xs={6}
            sx={{ p: "11px 12px", borderBottom: "1px solid #f1f1f4" }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Selected Stations
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {selectedStationDetails.namesText}
            </Typography>
          </Grid>

          <Grid
            item
            xs={6}
            sx={{
              p: "11px 12px",
              borderRight: "1px solid #f1f1f4",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Current Combined Stock
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#BE212F",
              }}
            >
              {selectedStationDetails.totalUnits.toLocaleString()} units
            </Typography>
          </Grid>

          <Grid item xs={6} sx={{ p: "11px 12px" }}>
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Allocation Mode
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.inventorySyncModeId === 3
                ? "Percentage based"
                : formData.inventorySyncModeId === 4
                ? "Fixed or Capped"
                : formData.inventorySyncModeId === 5
                ? "Dedicated"
                : "Percentage based"}
            </Typography>
          </Grid>
        </Grid>
      </Box>

      {/* Review Card 3: Inventory Rules */}
      <Box
        sx={{
          border: "1px solid #dddde5",
          borderRadius: "7px",
          mb: 2,
          overflow: "hidden",
          backgroundColor: "#fff",
        }}
      >
        <Box
          sx={{
            height: "38px",
            backgroundColor: "#fafafa",
            borderBottom: "1px solid #eeeef2",
            px: 1.5,
            display: "flex",
            alignItems: "center",
            fontWeight: 700,
            fontSize: "12px",
            color: "#22242a",
          }}
        >
          Inventory Rules
        </Box>
        <Grid container>
          <Grid
            item
            xs={6}
            sx={{
              p: "11px 12px",
              borderBottom: "1px solid #f1f1f4",
              borderRight: "1px solid #f1f1f4",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Inventory Sync Direction
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.inventorySyncDirectionId === 1
                ? "Pull Only (Channel -> Shipra)"
                : formData.inventorySyncDirectionId === 2
                ? "Push Only (Shipra -> Channel)"
                : formData.inventorySyncDirectionId === 3
                ? "Bidirectional (Two-Way)"
                : formData.inventorySyncDirectionId === 4
                ? "No Sync (Disabled)"
                : formData.syncInventory
                ? "Push Only"
                : "Disabled"}
            </Typography>
          </Grid>

          <Grid
            item
            xs={6}
            sx={{ p: "11px 12px", borderBottom: "1px solid #f1f1f4" }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Safety Buffer
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.bufferQuantity ?? 0} units
            </Typography>
          </Grid>

          <Grid
            item
            xs={6}
            sx={{
              p: "11px 12px",
              borderRight: "1px solid #f1f1f4",
            }}
          >
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Minimum Qty
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.minPublishQuantity ?? 0}
            </Typography>
          </Grid>

          <Grid item xs={6} sx={{ p: "11px 12px" }}>
            <Typography
              sx={{
                fontSize: "11px",
                color: "#8a8c95",
                mb: "3px",
              }}
            >
              Maximum Qty
            </Typography>
            <Typography
              sx={{
                fontSize: "13px",
                fontWeight: 700,
                color: "#22242a",
              }}
            >
              {formData.maxPublishQuantity !== null &&
              formData.maxPublishQuantity !== ""
                ? `${formData.maxPublishQuantity} units`
                : "No limit"}
            </Typography>
          </Grid>
        </Grid>
      </Box>

      {/* Info / Success Box */}
      <Box
        sx={{
          backgroundColor: "#eef8f3",
          border: "1px solid #cbead9",
          borderRadius: "7px",
          p: 1.75,
          color: "#237a4c",
          fontSize: "12px",
          lineHeight: 1.45,
        }}
      >
        <strong style={{ fontWeight: 700 }}>
          What happens after activation?
        </strong>
        <br />
        Shipra calculates available quantity from the selected Product
        Stations, applies the buffer and min/max rules, then publishes the
        resulting quantity to the mapped sales-channel products.
      </Box>
    </Box>
  );
}
