import React from "react";
import {
  Box,
  Typography,
  TextField,
  Radio,
  Divider,
  Grid,
} from "@mui/material";
import { EnumInventorySyncMode } from "../../models/InventorySyncPolicy";
import SelectComponent from "../../../../../.reUseableComponents/TextField/SelectComponent";

export default function Step1PolicyDetails({
  formData,
  setFormData,
  salesChannels = [],
  isLoadingChannels = false,
}) {
  const handleTextChange = (field) => (e) => {
    setFormData((prev) => ({ ...prev, [field]: e.target.value }));
  };

  const handleScopeChange = (modeId) => {
    setFormData((prev) => ({
      ...prev,
      inventorySyncModeId: modeId,
    }));
  };

  const selectedChannelValue =
    salesChannels.find((c) => c.id === formData.saleChannelConfigId) ||
    salesChannels.find(
      (c) => c.SaleChannelConfigId === formData.saleChannelConfigId
    ) ||
    null;

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
        Policy details
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
        Create a reusable rule that controls how products and inventory are
        synchronized with a sales channel.
      </Typography>

      <Grid container spacing={2}>
        <Grid item xs={12} sm={6}>
          <Box sx={{ mb: 1 }}>
            <Typography
              sx={{
                fontSize: "12px",
                color: "#555861",
                fontWeight: 700,
                mb: "6px",
              }}
            >
              Policy Name
            </Typography>
            <TextField
              fullWidth
              size="small"
              value={formData.policyName || ""}
              onChange={handleTextChange("policyName")}
              placeholder="e.g. Amazon UAE - Standard Stock"
              sx={{
                "& .MuiOutlinedInput-root": {
                  height: "36px",
                  fontSize: "13px",
                  borderRadius: "4px",
                  backgroundColor: "#fff",
                  "& fieldset": { borderColor: "#d2d2da" },
                  "&:hover fieldset": { borderColor: "var(--primary-color)" },
                  "&.Mui-focused fieldset": {
                    borderColor: "var(--primary-color)",
                    boxShadow: "0 0 0 2px #fdf2f3",
                  },
                },
              }}
            />
          </Box>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Box sx={{ mb: 1 }}>
            <Typography
              sx={{
                fontSize: "12px",
                color: "#555861",
                fontWeight: 700,
                mb: "6px",
              }}
            >
              Sales Channel
            </Typography>
            <SelectComponent
              name="saleChannelConfigId"
              options={salesChannels}
              value={selectedChannelValue}
              optionLabel="text"
              optionValue="id"
              height={36}
              isLoading={isLoadingChannels}
              placeholder="Select Sales Channel"
              onChange={(e, val) => {
                if (val) {
                  setFormData((prev) => ({
                    ...prev,
                    saleChannelConfigId: val.id || val.SaleChannelConfigId,
                    salesChannelName: val.text || val.SaleChannelName,
                    salesChannelCode: val.SaleChannelKey || "a",
                  }));
                }
              }}
            />
          </Box>
        </Grid>
      </Grid>

      <Divider sx={{ my: 2.5, borderColor: "#eeeef2" }} />

      <Typography
        variant="h6"
        sx={{
          fontSize: "15px",
          fontWeight: 700,
          mb: 0.5,
          color: "#22242a",
        }}
      >
        Apply policy to
      </Typography>
      <Typography
        variant="body2"
        sx={{
          fontSize: "13px",
          color: "#777a83",
          mb: 2,
          lineHeight: 1.45,
        }}
      >
        You can keep one default policy per channel and create exceptions only
        where needed.
      </Typography>

      <Grid container spacing={2}>
        <Grid item xs={12} sm={6}>
          <Box
            onClick={() => handleScopeChange(EnumInventorySyncMode.AllProducts)}
            sx={{
              border: "1px solid",
              borderColor:
                formData.inventorySyncModeId ===
                EnumInventorySyncMode.AllProducts
                  ? "var(--primary-color)"
                  : "#d9d9e2",
              backgroundColor:
                formData.inventorySyncModeId ===
                EnumInventorySyncMode.AllProducts
                  ? "#fdf2f3"
                  : "#fff",
              borderRadius: "7px",
              p: 1.75,
              cursor: "pointer",
              display: "flex",
              alignItems: "flex-start",
              gap: 1.25,
              boxShadow:
                formData.inventorySyncModeId ===
                EnumInventorySyncMode.AllProducts
                  ? "0 0 0 1px var(--primary-color) inset"
                  : "none",
              transition: "all 0.15s ease",
              "&:hover": { borderColor: "var(--primary-color)" },
            }}
          >
            <Radio
              checked={
                formData.inventorySyncModeId ===
                EnumInventorySyncMode.AllProducts
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
                All Products (Channel Default)
              </Typography>
              <Typography
                sx={{
                  fontSize: "12px",
                  color: "#777a84",
                  lineHeight: 1.4,
                  m: 0,
                }}
              >
                Applies to all existing and future products in this sales
                channel.
              </Typography>
            </Box>
          </Box>
        </Grid>

        <Grid item xs={12} sm={6}>
          <Box
            onClick={() =>
              handleScopeChange(EnumInventorySyncMode.SelectedProducts)
            }
            sx={{
              border: "1px solid",
              borderColor:
                formData.inventorySyncModeId ===
                EnumInventorySyncMode.SelectedProducts
                  ? "var(--primary-color)"
                  : "#d9d9e2",
              backgroundColor:
                formData.inventorySyncModeId ===
                EnumInventorySyncMode.SelectedProducts
                  ? "#fdf2f3"
                  : "#fff",
              borderRadius: "7px",
              p: 1.75,
              cursor: "pointer",
              display: "flex",
              alignItems: "flex-start",
              gap: 1.25,
              boxShadow:
                formData.inventorySyncModeId ===
                EnumInventorySyncMode.SelectedProducts
                  ? "0 0 0 1px var(--primary-color) inset"
                  : "none",
              transition: "all 0.15s ease",
              "&:hover": { borderColor: "var(--primary-color)" },
            }}
          >
            <Radio
              checked={
                formData.inventorySyncModeId ===
                EnumInventorySyncMode.SelectedProducts
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
                Specific Products (Override)
              </Typography>
              <Typography
                sx={{
                  fontSize: "12px",
                  color: "#777a84",
                  lineHeight: 1.4,
                  m: 0,
                }}
              >
                Create custom rules for high-velocity or fragile items.
              </Typography>
            </Box>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );
}
