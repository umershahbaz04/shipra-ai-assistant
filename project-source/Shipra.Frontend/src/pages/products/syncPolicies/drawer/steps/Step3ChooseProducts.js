import React, { useState, useEffect, useMemo, useCallback } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  Checkbox,
  Chip,
  Menu,
  MenuItem,
  InputAdornment,
  CircularProgress,
  Grid,
  TablePagination,
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";

const ProductRowItem = React.memo(
  function ProductRowItem({
    prod,
    idx,
    isLast,
    currentModeId,
    formDataBuffer,
    formDataMin,
    formDataPct,
    formDataMax,
    onToggle,
    onFieldChange,
  }) {
    const targetBuffer =
      prod.bufferQuantity !== undefined ? prod.bufferQuantity : formDataBuffer;
    const targetModeVal =
      currentModeId === 3
        ? prod.allocationPercentage !== undefined
          ? prod.allocationPercentage
          : formDataPct
        : prod.maxPublishQuantity !== undefined
        ? prod.maxPublishQuantity === null
          ? ""
          : prod.maxPublishQuantity
        : formDataMax === null
        ? ""
        : formDataMax;

    const [localBuffer, setLocalBuffer] = useState(targetBuffer);
    const [localModeVal, setLocalModeVal] = useState(targetModeVal);
    const [prevTargetBuffer, setPrevTargetBuffer] = useState(targetBuffer);
    const [prevTargetModeVal, setPrevTargetModeVal] = useState(targetModeVal);

    if (targetBuffer !== prevTargetBuffer) {
      setPrevTargetBuffer(targetBuffer);
      setLocalBuffer(targetBuffer);
    }

    if (targetModeVal !== prevTargetModeVal) {
      setPrevTargetModeVal(targetModeVal);
      setLocalModeVal(targetModeVal);
    }

    const available = Number(prod.AvailableStock || 0);
    const bufNum = Number(localBuffer ?? 0);
    const afterBuffer = Math.max(0, available - bufNum);
    let rowChannelQty = afterBuffer;

    if (currentModeId === 3) {
      const pct = Number(localModeVal || 0);
      rowChannelQty = Math.floor((afterBuffer * pct) / 100);
    } else if (currentModeId === 4) {
      const maxQ = localModeVal === "" ? null : Number(localModeVal);
      if (maxQ !== null && maxQ > 0) {
        rowChannelQty = Math.min(afterBuffer, maxQ);
      }
    }

    const minQ = Number(formDataMin || 0);
    if (minQ > 0 && rowChannelQty < minQ) {
      rowChannelQty = 0;
    }

    const handleBufferBlur = () => {
      const val = localBuffer === "" ? 0 : Math.max(0, Number(localBuffer));
      if (val !== prod.bufferQuantity) {
        onFieldChange(prod.ProductVariantId, prod.ProductId, "bufferQuantity", val);
      }
    };

    const handleModeValBlur = () => {
      const val = localModeVal;
      if (currentModeId === 3) {
        const parsedPct = val === "" ? 100 : Math.min(100, Math.max(0, Number(val)));
        if (parsedPct !== prod.allocationPercentage) {
          onFieldChange(
            prod.ProductVariantId,
            prod.ProductId,
            "allocationPercentage",
            parsedPct
          );
        }
      } else {
        const parsedMax = val === "" ? null : Math.max(0, Number(val));
        if (parsedMax !== prod.maxPublishQuantity) {
          onFieldChange(
            prod.ProductVariantId,
            prod.ProductId,
            "maxPublishQuantity",
            parsedMax
          );
        }
      }
    };

    return (
      <Box
        sx={{
          minHeight: "52px",
          display: "grid",
          gridTemplateColumns: "38px 1.5fr 0.9fr 80px 75px 75px 85px 75px",
          alignItems: "center",
          px: 1.25,
          py: 0.75,
          borderBottom: isLast ? "none" : "1px solid #eeeeF2",
          backgroundColor: prod.Selected ? "#fff" : "#fafafa",
          "&:hover": { backgroundColor: "#fff8f8" },
        }}
      >
        <Box>
          <Checkbox
            size="small"
            checked={Boolean(prod.Selected)}
            onChange={(e) =>
              onToggle(prod.ProductVariantId, prod.ProductId, e.target.checked)
            }
            sx={{
              p: 0,
              color: "#999ba4",
              "&.Mui-checked": { color: "#BE212F" },
            }}
          />
        </Box>
        <Box sx={{ pr: 1 }}>
          <Typography
            sx={{
              fontSize: "13px",
              fontWeight: 700,
              color: "#22242a",
              lineHeight: 1.2,
            }}
          >
            {prod.Title}
          </Typography>
          <Typography
            sx={{
              fontSize: "11px",
              color: "#8a8c94",
              mt: "2px",
            }}
          >
            {prod.VariantName ? `${prod.VariantName} · ` : ""}
            {prod.Category || "General"}
            {prod.Store && prod.Store !== "Shipra Store" ? ` · ${prod.Store}` : ""}
          </Typography>
          {prod.StationBreakdown &&
            prod.StationBreakdown.replace(/[: ,]/g, "").length > 0 && (
              <Typography
                sx={{
                  fontSize: "10.5px",
                  color: "#BE212F",
                  mt: "2px",
                  fontWeight: 500,
                }}
              >
                📍 {prod.StationBreakdown}
              </Typography>
            )}
        </Box>

        <Box
          sx={{
            fontSize: "12px",
            color: "#555861",
            fontFamily: "monospace",
          }}
        >
          {prod.SKU}
        </Box>

        <Box
          sx={{
            fontSize: "13px",
            fontWeight: 700,
            color: "#22242a",
          }}
        >
          {Number(prod.AvailableStock || 0).toLocaleString()}
        </Box>

        {/* Buffer Qty Input */}
        <Box sx={{ pr: 1 }}>
          <TextField
            size="small"
            type="number"
            value={localBuffer}
            onChange={(e) => setLocalBuffer(e.target.value)}
            onBlur={handleBufferBlur}
            disabled={!prod.Selected}
            inputProps={{ min: 0, max: 9999, style: { textAlign: "center", padding: "2px 4px" } }}
            sx={{
              width: "62px",
              "& .MuiOutlinedInput-root": {
                height: "28px",
                fontSize: "12px",
                borderRadius: "4px",
                backgroundColor: "#fff",
                "& fieldset": { borderColor: "#d8d8e2" },
                "&.Mui-focused fieldset": { borderColor: "#BE212F" },
              },
            }}
          />
        </Box>

        {/* Mode Specific Input (% or Max Qty) */}
        <Box sx={{ pr: 1 }}>
          <TextField
            size="small"
            type="number"
            placeholder={currentModeId === 4 ? "No max" : "%"}
            value={localModeVal}
            onChange={(e) => setLocalModeVal(e.target.value)}
            onBlur={handleModeValBlur}
            disabled={!prod.Selected}
            inputProps={{ min: 0, max: 9999, style: { textAlign: "center", padding: "2px 4px" } }}
            sx={{
              width: "62px",
              "& .MuiOutlinedInput-root": {
                height: "28px",
                fontSize: "12px",
                borderRadius: "4px",
                backgroundColor: "#fff",
                "& fieldset": { borderColor: "#d8d8e2" },
                "&.Mui-focused fieldset": { borderColor: "#BE212F" },
              },
            }}
          />
        </Box>

        {/* Resulting Channel Qty */}
        <Box
          sx={{
            fontSize: "13px",
            fontWeight: 800,
            color: "#BE212F",
          }}
        >
          {rowChannelQty.toLocaleString()}
        </Box>

        <Box>
          <Chip
            label={prod.PolicyTag || "Default"}
            size="small"
            sx={{
              height: "22px",
              fontSize: "11px",
              backgroundColor: "#f3f3f7",
              border: "1px solid #e2e2e8",
              color: "#555861",
              borderRadius: "10px",
            }}
          />
        </Box>
      </Box>
    );
  },
  (prevProps, nextProps) => {
    return (
      prevProps.prod === nextProps.prod &&
      prevProps.currentModeId === nextProps.currentModeId &&
      prevProps.formDataBuffer === nextProps.formDataBuffer &&
      prevProps.formDataMin === nextProps.formDataMin &&
      prevProps.formDataPct === nextProps.formDataPct &&
      prevProps.formDataMax === nextProps.formDataMax &&
      prevProps.isLast === nextProps.isLast
    );
  }
);

export default function Step3ChooseProducts({
  formData,
  setFormData,
  isLoadingProducts = false,
  onRefreshProducts,
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("All");
  const [selectedStore, setSelectedStore] = useState("All");

  const [categoryAnchorEl, setCategoryAnchorEl] = useState(null);
  const [storeAnchorEl, setStoreAnchorEl] = useState(null);

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(25);

  // Dynamically extract categories from live products
  const categories = useMemo(() => {
    const cats = new Set();
    (formData.products || []).forEach((p) => {
      if (p.Category) cats.add(p.Category);
    });
    return ["All", ...Array.from(cats)];
  }, [formData.products]);

  // Dynamically extract stores from live products
  const stores = useMemo(() => {
    const storeList = new Set();
    (formData.products || []).forEach((p) => {
      if (p.Store) storeList.add(p.Store);
    });
    return ["All", ...Array.from(storeList)];
  }, [formData.products]);

  // Reactive in-memory filtering (Search, Category, Store/Brand)
  const filteredProducts = useMemo(() => {
    return (formData.products || []).filter((prod) => {
      const q = searchTerm.trim().toLowerCase();
      const matchesSearch =
        !q ||
        prod.Title?.toLowerCase().includes(q) ||
        prod.SKU?.toLowerCase().includes(q) ||
        prod.Barcode?.toLowerCase().includes(q) ||
        prod.VariantName?.toLowerCase().includes(q);

      const matchesCat =
        selectedCategory === "All" || prod.Category === selectedCategory;
      const matchesStore =
        selectedStore === "All" || prod.Store === selectedStore;

      return matchesSearch && matchesCat && matchesStore;
    });
  }, [formData.products, searchTerm, selectedCategory, selectedStore]);

  // Reset page to 0 when filters or search change
  useEffect(() => {
    setPage(0);
  }, [searchTerm, selectedCategory, selectedStore]);

  const paginatedProducts = useMemo(() => {
    const start = page * rowsPerPage;
    return filteredProducts.slice(start, start + rowsPerPage);
  }, [filteredProducts, page, rowsPerPage]);

  // Stats calculation
  const stats = useMemo(() => {
    const totalCount = (formData.products || []).length;
    const selectedList = (formData.products || []).filter((p) => p.Selected);
    const selectedCount = selectedList.length;
    const totalAvailable = (formData.products || []).reduce(
      (sum, p) => sum + (Number(p.AvailableStock) || 0),
      0
    );
    const selectedAvailable = selectedList.reduce(
      (sum, p) => sum + (Number(p.AvailableStock) || 0),
      0
    );

    return {
      totalCount,
      selectedCount,
      totalAvailable,
      selectedAvailable,
      filteredCount: filteredProducts.length,
    };
  }, [formData.products, filteredProducts]);

  const allFilteredSelected =
    filteredProducts.length > 0 &&
    filteredProducts.every((p) => p.Selected);

  const isSomeFilteredSelected =
    filteredProducts.some((p) => p.Selected) && !allFilteredSelected;

  const handleSelectAll = (e) => {
    const isChecked = e.target.checked;
    const filteredVariantIds = new Set(
      filteredProducts.map((p) => p.ProductVariantId || p.ProductId)
    );
    setFormData((prev) => ({
      ...prev,
      products: (prev.products || []).map((p) =>
        filteredVariantIds.has(p.ProductVariantId || p.ProductId)
          ? { ...p, Selected: isChecked }
          : p
      ),
    }));
  };

  const handleProductToggle = useCallback((productVariantId, productId, isChecked) => {
    setFormData((prev) => ({
      ...prev,
      products: (prev.products || []).map((p) =>
        (p.ProductVariantId === productVariantId && p.ProductId === productId) ||
        (productVariantId && p.ProductVariantId === productVariantId) ||
        (!productVariantId && p.ProductId === productId)
          ? { ...p, Selected: isChecked }
          : p
      ),
    }));
  }, [setFormData]);

  const currentModeId = Number(
    formData.inventorySyncModeId && [3, 4, 5].includes(Number(formData.inventorySyncModeId))
      ? formData.inventorySyncModeId
      : 3
  );

  const handleProductFieldChange = useCallback((productVariantId, productId, field, value) => {
    setFormData((prev) => ({
      ...prev,
      products: (prev.products || []).map((p) => {
        const match =
          (p.ProductVariantId === productVariantId && p.ProductId === productId) ||
          (productVariantId && p.ProductVariantId === productVariantId) ||
          (!productVariantId && p.ProductId === productId);
        if (match) {
          return { ...p, [field]: value };
        }
        return p;
      }),
    }));
  }, [setFormData]);

  const getProductPublishedStock = (prod) => {
    const available = Number(prod.AvailableStock || 0);
    const buffer =
      prod.bufferQuantity !== undefined
        ? Number(prod.bufferQuantity)
        : Number(formData.bufferQuantity ?? 5);
    const minQty = Number(formData.minPublishQuantity ?? 0);

    const afterBuffer = Math.max(0, available - buffer);
    let published = afterBuffer;

    if (currentModeId === 3) {
      const pct =
        prod.allocationPercentage !== undefined
          ? Number(prod.allocationPercentage)
          : Number(formData.allocationPercentage ?? 5);
      published = Math.floor((afterBuffer * pct) / 100);
    } else if (currentModeId === 4) {
      const maxQ =
        prod.maxPublishQuantity !== undefined && prod.maxPublishQuantity !== null
          ? Number(prod.maxPublishQuantity)
          : formData.maxPublishQuantity !== null && formData.maxPublishQuantity !== ""
          ? Number(formData.maxPublishQuantity)
          : null;
      if (maxQ !== null && maxQ > 0) {
        published = Math.min(afterBuffer, maxQ);
      }
    }

    if (minQty > 0 && published < minQty) {
      published = 0;
    }

    return published;
  };

  const handleRuleChange = (field, val) => {
    setFormData((prev) => {
      const updatedProducts = (prev.products || []).map((p) => ({
        ...p,
        [field]: val,
      }));
      return {
        ...prev,
        [field]: val,
        products: updatedProducts,
      };
    });
  };

  const sampleProductLabel = useMemo(() => {
    const selected = (formData.products || []).find((p) => p.Selected && p.SKU);
    return selected?.SKU || selected?.Title || "TS-BLK-M";
  }, [formData.products]);

  const calculationResult = useMemo(() => {
    const available = stats.selectedAvailable || stats.totalAvailable || 6328;
    const buffer = Number(formData.bufferQuantity ?? 5);
    const minQty = Number(formData.minPublishQuantity ?? 0);
    const maxQty =
      formData.maxPublishQuantity === "" || formData.maxPublishQuantity === null
        ? null
        : Number(formData.maxPublishQuantity);
    const percentage = Number(formData.allocationPercentage ?? 5);

    const afterBuffer = Math.max(0, available - buffer);
    let published = afterBuffer;

    if (currentModeId === 3) {
      published = Math.floor((afterBuffer * percentage) / 100);
    } else if (currentModeId === 4) {
      if (maxQty !== null && maxQty > 0) {
        published = Math.min(afterBuffer, maxQty);
      }
    }

    if (minQty > 0 && published < minQty) {
      published = 0;
    }

    return {
      available,
      buffer,
      afterBuffer,
      maxQty,
      percentage,
      published,
    };
  }, [
    stats.selectedAvailable,
    stats.totalAvailable,
    formData.bufferQuantity,
    formData.minPublishQuantity,
    formData.maxPublishQuantity,
    formData.allocationPercentage,
    currentModeId,
  ]);

  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "560px" }}>
      <Typography
        variant="h6"
        sx={{
          fontSize: "16px",
          fontWeight: 700,
          mb: 0.5,
          color: "#22242a",
        }}
      >
        Choose products
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
        The policy can be used for all channel products or only selected items.
        Stock reflects available inventory across the selected station source.
      </Typography>

      {/* Filter toolbar */}
      <Box sx={{ display: "flex", gap: 1, mb: 1.5 }}>
        <TextField
          size="small"
          placeholder="Search SKU, product or barcode"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          sx={{
            flex: 1,
            "& .MuiOutlinedInput-root": {
              height: "32px",
              fontSize: "13px",
              borderRadius: "4px",
              backgroundColor: "#fff",
              "& fieldset": { borderColor: "#d7d7de" },
              "&:hover fieldset": { borderColor: "var(--primary-color)" },
              "&.Mui-focused fieldset": { borderColor: "var(--primary-color)" },
            },
          }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon sx={{ fontSize: 18, color: "#9a9ca5" }} />
              </InputAdornment>
            ),
          }}
        />

        {/* Category Filter */}
        <Button
          variant="outlined"
          size="small"
          endIcon={<KeyboardArrowDownIcon sx={{ fontSize: 16 }} />}
          onClick={(e) => setCategoryAnchorEl(e.currentTarget)}
          sx={{
            height: "32px",
            borderColor: "#cfcfd8",
            color: "#333",
            fontSize: "12px",
            fontWeight: 600,
            textTransform: "none",
            backgroundColor: "#fff",
            "&:hover": { borderColor: "#BE212F", backgroundColor: "#fff8f8" },
          }}
        >
          {selectedCategory === "All" ? "Category" : selectedCategory}
        </Button>
        <Menu
          anchorEl={categoryAnchorEl}
          open={Boolean(categoryAnchorEl)}
          onClose={() => setCategoryAnchorEl(null)}
          PaperProps={{ sx: { maxHeight: 260, width: 180 } }}
          anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
          transformOrigin={{ vertical: "top", horizontal: "right" }}
        >
          {categories.map((cat) => (
            <MenuItem
              key={cat}
              selected={selectedCategory === cat}
              onClick={() => {
                setSelectedCategory(cat);
                setCategoryAnchorEl(null);
              }}
              sx={{ fontSize: "13px" }}
            >
              {cat}
            </MenuItem>
          ))}
        </Menu>

        {/* Store / Brand Filter */}
        <Button
          variant="outlined"
          size="small"
          endIcon={<KeyboardArrowDownIcon sx={{ fontSize: 16 }} />}
          onClick={(e) => setStoreAnchorEl(e.currentTarget)}
          sx={{
            height: "32px",
            borderColor: "#cfcfd8",
            color: "#333",
            fontSize: "12px",
            fontWeight: 600,
            textTransform: "none",
            backgroundColor: "#fff",
            "&:hover": { borderColor: "#BE212F", backgroundColor: "#fff8f8" },
          }}
        >
          {selectedStore === "All" ? "Store / Brand" : selectedStore}
        </Button>
        <Menu
          anchorEl={storeAnchorEl}
          open={Boolean(storeAnchorEl)}
          onClose={() => setStoreAnchorEl(null)}
          PaperProps={{ sx: { maxHeight: 260, width: 200 } }}
          anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
          transformOrigin={{ vertical: "top", horizontal: "right" }}
        >
          {stores.map((st) => (
            <MenuItem
              key={st}
              selected={selectedStore === st}
              onClick={() => {
                setSelectedStore(st);
                setStoreAnchorEl(null);
              }}
              sx={{ fontSize: "13px" }}
            >
              {st}
            </MenuItem>
          ))}
        </Menu>
      </Box>

      {/* Mode-specific rule inputs grid */}
      <Grid container spacing={1.5} sx={{ mb: 1.5 }}>
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
              value={formData.bufferQuantity ?? 5}
              onChange={(e) =>
                handleRuleChange(
                  "bufferQuantity",
                  e.target.value === "" ? 0 : Math.max(0, Number(e.target.value))
                )
              }
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
              onChange={(e) =>
                handleRuleChange(
                  "minPublishQuantity",
                  e.target.value === "" ? 0 : Math.max(0, Number(e.target.value))
                )
              }
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

        {currentModeId === 3 ? (
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
                Allocation Percentage %
              </Typography>
              <TextField
                fullWidth
                size="small"
                type="number"
                value={formData.allocationPercentage ?? 5}
                onChange={(e) =>
                  handleRuleChange(
                    "allocationPercentage",
                    e.target.value === ""
                      ? 5
                      : Math.min(100, Math.max(0, Number(e.target.value)))
                  )
                }
                inputProps={{ min: 0, max: 100 }}
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
        ) : (
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
                onChange={(e) =>
                  handleRuleChange(
                    "maxPublishQuantity",
                    e.target.value === "" ? null : Math.max(0, Number(e.target.value))
                  )
                }
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
        )}
      </Grid>

      {/* Live Example Calculation Preview Card */}
      <Box
        sx={{
          backgroundColor: "#fdf2f3",
          border: "1px solid #f8d7da",
          borderRadius: "7px",
          p: 1.75,
          mb: 2,
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
          Example calculation for {sampleProductLabel}
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
            {calculationResult.available}
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

        {currentModeId === 3 ? (
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              my: "6px",
              fontSize: "12px",
              color: "#666975",
            }}
          >
            <span>Allocation percentage</span>
            <strong style={{ fontWeight: 700, color: "#22242a" }}>
              {calculationResult.percentage}%
            </strong>
          </Box>
        ) : (
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
        )}

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
          <span>{calculationResult.published}</span>
        </Box>
      </Box>

      {/* Products count and selection summary */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          mb: 1.25,
          px: 0.5,
        }}
      >
        <Typography sx={{ fontSize: "12px", color: "#555861", fontWeight: 600 }}>
          Products:{" "}
          <Box component="span" sx={{ color: "#22242a", fontWeight: 700 }}>
            {stats.filteredCount !== stats.totalCount
              ? `${stats.filteredCount} matching (${stats.totalCount} total)`
              : `${stats.totalCount} total`}
          </Box>
        </Typography>

        <Typography sx={{ fontSize: "12px", color: "#666973" }}>
          <Box component="span" sx={{ color: "#BE212F", fontWeight: 700 }}>
            {stats.selectedCount}
          </Box>{" "}
          selected ·{" "}
          <Box component="span" sx={{ color: "#22242a", fontWeight: 700 }}>
            {stats.selectedAvailable.toLocaleString()}
          </Box>{" "}
          units available
        </Typography>
      </Box>

      {/* Products table with internal scroll */}
      <Box
        sx={{
          border: "1px solid #dedee5",
          borderRadius: "6px",
          overflow: "hidden",
          backgroundColor: "#fff",
          display: "flex",
          flexDirection: "column",
          height: "460px",
          boxShadow: "0 1px 3px rgba(0,0,0,0.03)",
        }}
      >
        {/* Sticky Table header */}
        <Box
          sx={{
            minHeight: "36px",
            backgroundColor: "#fafafa",
            fontSize: "11px",
            fontWeight: 700,
            color: "#666973",
            display: "grid",
            gridTemplateColumns: "38px 1.5fr 0.9fr 80px 75px 75px 85px 75px",
            alignItems: "center",
            px: 1.25,
            borderBottom: "1px solid #dedee5",
            position: "sticky",
            top: 0,
            zIndex: 2,
            flexShrink: 0,
          }}
        >
          <Box>
            <Checkbox
              size="small"
              checked={allFilteredSelected}
              indeterminate={isSomeFilteredSelected}
              onChange={handleSelectAll}
              disabled={isLoadingProducts || filteredProducts.length === 0}
              sx={{
                p: 0,
                color: "#999ba4",
                "&.Mui-checked, &.MuiCheckbox-indeterminate": {
                  color: "#BE212F",
                },
              }}
            />
          </Box>
          <Box>Product</Box>
          <Box>SKU</Box>
          <Box>Available</Box>
          <Box>Buffer Qty</Box>
          <Box>{currentModeId === 3 ? "Allocation %" : "Max Qty"}</Box>
          <Box>Channel Qty</Box>
          <Box>Policy</Box>
        </Box>

        {/* Scrollable Product rows container */}
        <Box
          sx={{
            overflowY: "auto",
            flex: 1,
            "&::-webkit-scrollbar": { width: "6px" },
            "&::-webkit-scrollbar-track": { backgroundColor: "#f5f5f8" },
            "&::-webkit-scrollbar-thumb": {
              backgroundColor: "#d5d5df",
              borderRadius: "3px",
              "&:hover": { backgroundColor: "#b5b5c2" },
            },
          }}
        >
          {isLoadingProducts ? (
            <Box
              sx={{
                height: "100%",
                minHeight: "360px",
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "center",
                gap: 1.5,
                color: "#858791",
              }}
            >
              <CircularProgress size={30} sx={{ color: "#BE212F" }} />
              <Typography sx={{ fontSize: "13px", fontWeight: 600, color: "#555861" }}>
                Loading products for selected inventory source...
              </Typography>
            </Box>
          ) : filteredProducts.length === 0 ? (
            <Box
              sx={{
                p: 4,
                textAlign: "center",
                color: "#858791",
                fontSize: "13px",
              }}
            >
              No products found matching the criteria.
            </Box>
          ) : (
            paginatedProducts.map((prod, idx) => (
              <ProductRowItem
                key={`${prod.ProductId}-${prod.ProductVariantId || idx}`}
                prod={prod}
                idx={idx}
                isLast={idx === paginatedProducts.length - 1}
                currentModeId={currentModeId}
                formDataBuffer={formData.bufferQuantity ?? 5}
                formDataMin={formData.minPublishQuantity ?? 0}
                formDataPct={formData.allocationPercentage ?? 5}
                formDataMax={formData.maxPublishQuantity}
                onToggle={handleProductToggle}
                onFieldChange={handleProductFieldChange}
              />
            ))
          )}
        </Box>

        {!isLoadingProducts && filteredProducts.length > 0 && (
          <TablePagination
            component="div"
            count={filteredProducts.length}
            page={page}
            onPageChange={(e, newPage) => setPage(newPage)}
            rowsPerPage={rowsPerPage}
            onRowsPerPageChange={(e) => {
              setRowsPerPage(parseInt(e.target.value, 10));
              setPage(0);
            }}
            rowsPerPageOptions={[25, 50, 100]}
            sx={{
              borderTop: "1px solid #eeeeF2",
              backgroundColor: "#fafafa",
              ".MuiTablePagination-selectLabel, .MuiTablePagination-displayedRows": {
                fontSize: "12px",
                color: "#555861",
              },
              ".MuiTablePagination-select": {
                fontSize: "12px",
              },
            }}
          />
        )}
      </Box>

      {/* Summary Footer */}
      <Box
        sx={{
          mt: 1.5,
          p: 1.25,
          backgroundColor: "#fdf2f3",
          borderRadius: "6px",
          border: "1px solid #e2ddf8",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
        }}
      >
        <Typography sx={{ fontSize: "12px", color: "#4f515a" }}>
          Selected Items:{" "}
          <Box component="span" sx={{ fontWeight: 700, color: "#22242a" }}>
            {stats.selectedCount} of {stats.totalCount} products
          </Box>
        </Typography>
        <Typography sx={{ fontSize: "12px", color: "#4f515a" }}>
          Total Available Stock:{" "}
          <Box component="span" sx={{ fontWeight: 700, color: "#BE212F" }}>
            {stats.selectedAvailable.toLocaleString()} units
          </Box>
        </Typography>
      </Box>
    </Box>
  );
}
