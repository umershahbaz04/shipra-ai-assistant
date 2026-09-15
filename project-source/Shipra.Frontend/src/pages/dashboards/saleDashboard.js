import React, { useState, useEffect, useCallback } from "react";
import {
  Box,
  Grid,
  Card,
  CardContent,
  Typography,
  Avatar,
  CircularProgress,
  InputBase,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Pagination,
  Chip,
  IconButton,
  InputLabel,
  Button
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import SearchIcon from "@mui/icons-material/Search";
import PersonIcon from "@mui/icons-material/Person";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import InventoryIcon from "@mui/icons-material/Inventory";
import StorefrontIcon from "@mui/icons-material/Storefront";
import CustomReactDatePickerInputFilter from "../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import Colors from "../../utilities/helpers/Colors";
import UtilityClass from "../../utilities/UtilityClass";
import {
  GetSaleDashboardStores,
  GetSaleDashboardChannels,
  GetSaleDashboardProducts
} from "../../api/AxiosInterceptors";

const FONT_FAMILY = "'Lato Medium', 'Inter Medium', 'Arial', sans-serif";

function SaleDashboardPage() {
  const navigate = useNavigate();

  // Stores states
  const [stores, setStores] = useState([]);
  const [selectedStore, setSelectedStore] = useState(null);
  const [isStoresLoading, setIsStoresLoading] = useState(false);
  const [storeSearchQuery, setStoreSearchQuery] = useState("");

  // Channels states
  const [channels, setChannels] = useState([]);
  const [selectedChannel, setSelectedChannel] = useState(null);
  const [isChannelsLoading, setIsChannelsLoading] = useState(false);

  // Products states
  const [products, setProducts] = useState([]);
  const [totalProducts, setTotalProducts] = useState(0);
  const [isProductsLoading, setIsProductsLoading] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [page, setPage] = useState(1);
  const rowsPerPage = 50;
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [appliedStartDate, setAppliedStartDate] = useState(null);
  const [appliedEndDate, setAppliedEndDate] = useState(null);

  // Fetch all stores on load
  const fetchStores = async () => {
    setIsStoresLoading(true);
    try {
      const res = await GetSaleDashboardStores({});
      if (res.data && res.data.result) {
        const list = res.data.result || [];
        setStores(list);
        if (list.length > 0) {
          // Auto select first store
          setSelectedStore(list[0]);
        }
      }
    } catch (e) {
      console.error("Error fetching stores:", e);
    } finally {
      setIsStoresLoading(false);
    }
  };

  useEffect(() => {
    fetchStores();
  }, []);

  // Fetch channels when store or dates change
  const fetchChannels = async (storeId) => {
    setIsChannelsLoading(true);
    try {
      const payload = { storeId };
      if (appliedStartDate) payload.createdFrom = appliedStartDate.toISOString();
      if (appliedEndDate) payload.createdTo = appliedEndDate.toISOString();
      
      const res = await GetSaleDashboardChannels(payload);
      if (res.data && res.data.result) {
        const list = res.data.result.filter(ch => ch.Id !== 0 && ch.text !== "Select Please");
        setChannels(list);
        setSelectedChannel(null); // Reset channel selection on store change
      }
    } catch (e) {
      console.error("Error fetching channels:", e);
    } finally {
      setIsChannelsLoading(false);
    }
  };

  useEffect(() => {
    if (selectedStore) {
      fetchChannels(selectedStore.StoreId);
    }
  }, [selectedStore, appliedStartDate, appliedEndDate]);

  // Fetch products when store, channel, page, search or date changes
  const fetchProducts = useCallback(async () => {
    if (!selectedStore) return;
    setIsProductsLoading(true);
    try {
      const res = await GetSaleDashboardProducts({
        storeId: selectedStore.StoreId,
        saleChannelConfigId: selectedChannel ? selectedChannel.Id : null,
        filterModel: {
          start: (page - 1) * rowsPerPage,
          length: rowsPerPage,
          search: searchQuery,
          sortDir: "desc",
          sortCol: 0,
          createdFrom: appliedStartDate ? appliedStartDate.toISOString() : null,
          createdTo: appliedEndDate ? appliedEndDate.toISOString() : null
        }
      });
      if (res.data && res.data.result) {
        setProducts(res.data.result.list || []);
        setTotalProducts(res.data.result.TotalCount || 0);
      }
    } catch (e) {
      console.error("Error fetching products:", e);
    } finally {
      setIsProductsLoading(false);
    }
  }, [selectedStore, selectedChannel, page, searchQuery, appliedStartDate, appliedEndDate]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  // Handlers
  const handleStoreSelect = (store) => {
    setSelectedStore(store);
    setPage(1);
    setSearchQuery("");
  };

  const handleChannelSelect = (channel) => {
    if (selectedChannel && selectedChannel.Id === channel.Id) {
      setSelectedChannel(null); // Deselect
    } else {
      setSelectedChannel(channel);
    }
    setPage(1);
  };

  const handleSearchChange = (e) => {
    setSearchQuery(e.target.value);
    setPage(1);
  };

  const handlePageChange = (event, value) => {
    setPage(value);
  };

  // Filter stores list by search input
  const filteredStores = stores.filter(store =>
    store.StoreName?.toLowerCase().includes(storeSearchQuery.toLowerCase())
  );

  return (
    <Box
      sx={{
        p: 3,
        minHeight: "85vh",
        background: "#F4F6F9",
        borderRadius: "16px",
        display: "flex",
        flexDirection: "column",
        gap: 3,
        fontFamily: FONT_FAMILY
      }}
    >
      {/* Header & Date Pickers bar */}
      <Paper
        sx={{
          p: 2,
          mb: 1,
          borderRadius: "16px",
          boxShadow: "0 2px 12px rgba(0,0,0,0.03)",
          border: "1px solid rgba(0,0,0,0.03)",
          display: "flex",
          flexWrap: "wrap",
          alignItems: "center",
          justifyContent: "space-between",
          gap: 2,
          bgcolor: "#fff"
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
          <IconButton onClick={() => navigate("/dashboards")} sx={{ color: "var(--primary-color)" }}>
            <ArrowBackIcon />
          </IconButton>
          <Box>
            <Typography
              variant="h5"
              sx={{
                fontWeight: 700,
                color: "#2C3E50",
                fontFamily: "'Lato Bold', 'Inter Bold', 'Arial', sans-serif"
              }}
            >
              Sale Dashboard
            </Typography>
            <Typography variant="body2" sx={{ color: "#7F8C8D", fontFamily: FONT_FAMILY }}>
              Monitor sales channels, product stocks, and salesperson metrics.
            </Typography>
          </Box>
        </Box>

        {/* Date Filter Inputs */}
        <Box sx={{ display: "flex", alignItems: "flex-end", gap: 2, flexWrap: "wrap" }}>
          <Box>
            <InputLabel sx={{ fontSize: "11px", fontWeight: 600, color: "#7F8C8D", mb: 0.5 }}>Created From</InputLabel>
            <CustomReactDatePickerInputFilter
              value={startDate}
              onClick={(date) => setStartDate(date)}
              size="small"
              isClearable
              maxDate={UtilityClass.todayDate()}
            />
          </Box>
          <Box>
            <InputLabel sx={{ fontSize: "11px", fontWeight: 600, color: "#7F8C8D", mb: 0.5 }}>Created To</InputLabel>
            <CustomReactDatePickerInputFilter
              value={endDate}
              onClick={(date) => setEndDate(date)}
              size="small"
              minDate={startDate}
              disabled={!startDate}
              isClearable
              maxDate={UtilityClass.todayDate()}
            />
          </Box>
          <Button
            variant="contained"
            color="primary"
            onClick={() => {
              setAppliedStartDate(startDate);
              setAppliedEndDate(endDate);
            }}
            sx={{
              height: "36px",
              borderRadius: "8px",
              textTransform: "capitalize",
              fontWeight: 600,
              px: 3,
              backgroundColor: "var(--primary-color)"
            }}
          >
            Filter
          </Button>
          {(startDate || endDate) && (
            <Button
              variant="outlined"
              color="secondary"
              onClick={() => {
                setStartDate(null);
                setEndDate(null);
                setAppliedStartDate(null);
                setAppliedEndDate(null);
              }}
              sx={{
                height: "36px",
                borderRadius: "8px",
                textTransform: "capitalize",
                fontWeight: 600,
                px: 3
              }}
            >
              Clear
            </Button>
          )}
        </Box>
      </Paper>

      {/* Main Layout Grid */}
      <Grid container spacing={3}>
        {/* Left Sidebar - Stores List (Only shown if more than 1 store) */}
        {stores.length > 1 && (
          <Grid item xs={12} md={3} sx={{ position: { md: "sticky" }, top: { md: "88px" }, height: { md: "calc(100vh - 140px)" } }}>
            <Paper
              sx={{
                p: 2,
                borderRadius: "16px",
                boxShadow: "0 4px 20px rgba(0,0,0,0.04)",
                height: "100%",
                display: "flex",
                flexDirection: "column",
                border: "1px solid #E5EAEE"
              }}
            >
              <Typography variant="subtitle1" sx={{ fontWeight: 700, mb: 1.5, color: "#1E1E1E", px: 1, fontFamily: FONT_FAMILY }}>
                Stores
              </Typography>

              {/* Store Search Input */}
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  backgroundColor: "#F4F6F9",
                  borderRadius: "10px",
                  px: 1.5,
                  py: 0.5,
                  mb: 2,
                  border: "1px solid #E5EAEE"
                }}
              >
                <SearchIcon sx={{ color: "#888", mr: 1, fontSize: 18 }} />
                <InputBase
                  placeholder="Search store..."
                  value={storeSearchQuery}
                  onChange={(e) => setStoreSearchQuery(e.target.value)}
                  sx={{ width: "100%", fontSize: "13px", fontFamily: FONT_FAMILY }}
                />
              </Box>

              {/* Scrollable list of stores */}
              <Box
                sx={{
                  flexGrow: 1,
                  overflowY: "auto",
                  display: "flex",
                  flexDirection: "column",
                  gap: 1,
                  pr: 0.5,
                  "&::-webkit-scrollbar": {
                    width: "5px",
                  },
                  "&::-webkit-scrollbar-thumb": {
                    backgroundColor: "#E5EAEE",
                    borderRadius: "4px",
                  },
                  "&::-webkit-scrollbar-thumb:hover": {
                    backgroundColor: "#D1D8E0",
                  }
                }}
              >
                {isStoresLoading ? (
                  <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
                    <CircularProgress size={24} sx={{ color: "var(--primary-color)" }} />
                  </Box>
                ) : filteredStores.length === 0 ? (
                  <Box sx={{ textAlign: "center", py: 2 }}>
                    <Typography variant="body2" color="textSecondary" sx={{ fontFamily: FONT_FAMILY }}>
                      No stores found
                    </Typography>
                  </Box>
                ) : (
                  filteredStores.map((store) => {
                    const isSelected = selectedStore && selectedStore.StoreId === store.StoreId;
                    return (
                      <Box
                        key={store.StoreId}
                        onClick={() => handleStoreSelect(store)}
                        sx={{
                          display: "flex",
                          alignItems: "center",
                          justifyContent: "space-between",
                          p: 1.5,
                          borderRadius: "12px",
                          cursor: "pointer",
                          transition: "all 0.2s",
                          backgroundColor: isSelected ? "rgba(86, 58, 213, 0.08)" : "transparent",
                          border: isSelected ? "1px solid var(--primary-color)" : "1px solid transparent",
                          borderLeft: isSelected ? "4px solid var(--primary-color)" : "1px solid transparent",
                          "&:hover": {
                            backgroundColor: isSelected ? "rgba(86, 58, 213, 0.12)" : "#F4F6F9"
                          }
                        }}
                      >
                        <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, minWidth: 0 }}>
                          <Avatar
                            src={store.StoreImage && store.StoreImage !== "null" ? store.StoreImage : undefined}
                            variant="rounded"
                            sx={{
                              width: 32,
                              height: 32,
                              backgroundColor: isSelected ? "rgba(86, 58, 213, 0.15)" : "#F4F6F9",
                              color: isSelected ? "var(--primary-color)" : "#666",
                            }}
                          >
                            {!store.StoreImage || store.StoreImage === "null" ? (
                              <StorefrontIcon sx={{ fontSize: 18 }} />
                            ) : null}
                          </Avatar>
                          <Typography
                            variant="body2"
                            noWrap
                            sx={{
                              fontWeight: isSelected ? 600 : 500,
                              color: isSelected ? "var(--primary-color)" : "#333",
                              fontFamily: FONT_FAMILY
                            }}
                          >
                            {store.StoreName}
                          </Typography>
                        </Box>
                        <Chip
                          label={`${store.SaleChannelConfigCount || 0} Ch`}
                          size="small"
                          sx={{
                            backgroundColor: isSelected ? "var(--primary-color)" : "#E5EAEE",
                            color: isSelected ? "#fff" : "#666",
                            fontWeight: 600,
                            fontSize: "11px",
                            height: "20px",
                            fontFamily: FONT_FAMILY
                          }}
                        />
                      </Box>
                    );
                  })
                )}
              </Box>
            </Paper>
          </Grid>
        )}

        {/* Right Main Content Panel */}
        <Grid item xs={12} md={stores.length > 1 ? 9 : 12}>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
            {/* Sale Channels Cards Section */}
            <Box>
              <Typography variant="subtitle1" sx={{ fontWeight: 700, mb: 2, color: "#1E1E1E", fontFamily: FONT_FAMILY }}>
                Sale Channels
              </Typography>
              {isChannelsLoading ? (
                <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
                  <CircularProgress size={28} sx={{ color: "var(--primary-color)" }} />
                </Box>
              ) : channels.length === 0 ? (
                <Card sx={{ borderRadius: "12px", border: "1px dashed #BDBDBD", p: 3, textAlign: "center" }}>
                  <Typography variant="body2" color="textSecondary" sx={{ fontFamily: FONT_FAMILY }}>
                    No active sale channels connected to this store.
                  </Typography>
                </Card>
              ) : (
                <Grid container spacing={2}>
                  {channels.map((ch) => {
                    const isSelected = selectedChannel && selectedChannel.Id === ch.Id;
                    return (
                      <Grid item xs={12} sm={6} md={4} key={ch.Id}>
                        <Card
                          onClick={() => handleChannelSelect(ch)}
                          sx={{
                            borderRadius: "16px",
                            boxShadow: isSelected ? "0 8px 24px rgba(86, 58, 213, 0.15)" : "0 4px 12px rgba(0,0,0,0.03)",
                            border: isSelected ? "2px solid var(--primary-color)" : "1px solid #E5EAEE",
                            cursor: "pointer",
                            transition: "all 0.2s",
                            "&:hover": {
                              transform: "translateY(-4px)",
                              boxShadow: "0 8px 20px rgba(0,0,0,0.08)"
                            }
                          }}
                        >
                          <CardContent sx={{ display: "flex", alignItems: "center", gap: 2, p: "16px !important" }}>
                            <Avatar
                              src={ch.ImageUrl}
                              variant="rounded"
                              sx={{ width: 44, height: 44, backgroundColor: "#F4F6F9" }}
                            />
                            <Box sx={{ flexGrow: 1, minWidth: 0 }}>
                              <Typography variant="body1" noWrap sx={{ fontWeight: 700, color: "#333", fontFamily: FONT_FAMILY }}>
                                {ch.Name}
                              </Typography>
                              <Typography variant="caption" sx={{ color: "#888", display: "flex", alignItems: "center", gap: 0.5, fontFamily: FONT_FAMILY }}>
                                <InventoryIcon sx={{ fontSize: 12 }} />
                                {ch.OrderCount || 0} Orders
                              </Typography>
                            </Box>
                            {isSelected && (
                              <Chip
                                label="Active Filter"
                                size="small"
                                sx={{ backgroundColor: "var(--primary-color)", color: "#fff", fontWeight: 600, fontSize: "10px", fontFamily: FONT_FAMILY }}
                              />
                            )}
                          </CardContent>
                        </Card>
                      </Grid>
                    );
                  })}
                </Grid>
              )}
            </Box>

            {/* Products Table Section */}
            <Paper
              sx={{
                borderRadius: "16px",
                boxShadow: "0 4px 20px rgba(0,0,0,0.04)",
                border: "1px solid #E5EAEE",
                overflow: "hidden"
              }}
            >
              {/* Table Header Filter controls */}
              <Box sx={{ p: 2.5, display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: 2, borderBottom: "1px solid #E5EAEE" }}>
                <Box>
                  <Typography variant="subtitle1" sx={{ fontWeight: 700, color: "#1E1E1E", fontFamily: FONT_FAMILY }}>
                    Products {selectedChannel ? `in ${selectedChannel.Name}` : ""}
                  </Typography>
                  <Typography variant="caption" color="textSecondary" sx={{ fontFamily: FONT_FAMILY }}>
                    Showing {products.length} of {totalProducts} items
                  </Typography>
                </Box>

                {/* Search Bar */}
                <Box
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    backgroundColor: "#F4F6F9",
                    borderRadius: "10px",
                    px: 1.5,
                    py: 0.5,
                    width: "300px",
                    border: "1px solid #E5EAEE"
                  }}
                >
                  <SearchIcon sx={{ color: "#888", mr: 1, fontSize: 20 }} />
                  <InputBase
                    placeholder="Search product name or SKU..."
                    value={searchQuery}
                    onChange={handleSearchChange}
                    sx={{ width: "100%", fontSize: "14px", fontFamily: FONT_FAMILY }}
                  />
                </Box>
              </Box>

              {/* Table Content */}
              {isProductsLoading ? (
                <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", py: 8, flexDirection: "column", gap: 2 }}>
                  <CircularProgress size={36} sx={{ color: "var(--primary-color)" }} />
                  <Typography variant="body2" color="textSecondary" sx={{ fontFamily: FONT_FAMILY }}>Loading products catalog...</Typography>
                </Box>
              ) : products.length === 0 ? (
                <Box sx={{ p: 6, textAlign: "center" }}>
                  <Typography variant="body1" color="textSecondary" sx={{ fontFamily: FONT_FAMILY }}>No products found matching the criteria.</Typography>
                </Box>
              ) : (
                <TableContainer
                  sx={{
                    maxHeight: "480px",
                    overflowY: "auto",
                    "&::-webkit-scrollbar": {
                      width: "5px",
                      height: "5px"
                    },
                    "&::-webkit-scrollbar-thumb": {
                      backgroundColor: "#E5EAEE",
                      borderRadius: "4px"
                    },
                    "&::-webkit-scrollbar-thumb:hover": {
                      backgroundColor: "#D1D8E0"
                    }
                  }}
                >
                  <Table sx={{ minWidth: 650 }}>
                    <TableHead sx={{ backgroundColor: "#F9FAFC", position: "sticky", top: 0, zIndex: 1 }}>
                      <TableRow>
                        <TableCell sx={{ fontWeight: 700, color: "#555", fontFamily: FONT_FAMILY, backgroundColor: "#F9FAFC" }}>Product Title / Name</TableCell>
                        <TableCell sx={{ fontWeight: 700, color: "#555", fontFamily: FONT_FAMILY, backgroundColor: "#F9FAFC" }}>SKU</TableCell>
                        <TableCell sx={{ fontWeight: 700, color: "#555", fontFamily: FONT_FAMILY, backgroundColor: "#F9FAFC" }}>Price</TableCell>
                        <TableCell sx={{ fontWeight: 700, color: "#555", fontFamily: FONT_FAMILY, backgroundColor: "#F9FAFC" }}>Available Qty</TableCell>
                        <TableCell sx={{ fontWeight: 700, color: "#555", fontFamily: FONT_FAMILY, backgroundColor: "#F9FAFC" }}>Salesperson</TableCell>
                        <TableCell sx={{ fontWeight: 700, color: "#555", fontFamily: FONT_FAMILY, backgroundColor: "#F9FAFC" }}>Sale Channel</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {products.map((row) => (
                        <TableRow
                          key={row.ProductId}
                          sx={{
                            "&:hover": { backgroundColor: "#F9FAFC" },
                            transition: "background-color 0.2s"
                          }}
                        >
                          {/* Image + Title */}
                          <TableCell>
                            <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
                              <Avatar
                                src={row.FeatureImage}
                                variant="rounded"
                                sx={{ width: 36, height: 36, border: "1px solid #E5EAEE", backgroundColor: "#fff" }}
                              />
                              <Typography variant="body2" sx={{ fontWeight: 600, color: "#333", fontFamily: FONT_FAMILY }}>
                                {row.ProductName}
                              </Typography>
                            </Box>
                          </TableCell>

                          {/* SKU */}
                          <TableCell>
                            <Typography variant="body2" sx={{ fontFamily: "monospace", color: "#666" }}>
                              {row.SKU || "-"}
                            </Typography>
                          </TableCell>

                          {/* Price */}
                          <TableCell>
                            <Typography variant="body2" sx={{ fontWeight: 600, color: "#111", fontFamily: FONT_FAMILY }}>
                              {row.Price ? `${row.Price.toFixed(2)} AED` : "0.00 AED"}
                            </Typography>
                          </TableCell>

                          {/* Quantity Available */}
                          <TableCell>
                            <Typography variant="body2" sx={{ fontWeight: 600, color: row.QuantityAvailable > 5 ? "#11998e" : "#ff5e62", fontFamily: FONT_FAMILY }}>
                              {row.QuantityAvailable} units
                            </Typography>
                          </TableCell>

                          {/* Salesperson Name */}
                          <TableCell>
                            <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                              <PersonIcon sx={{ fontSize: 16, color: "#888" }} />
                              <Typography variant="body2" sx={{ color: "#333", fontFamily: FONT_FAMILY }}>
                                {row.SalespersonName || "-"}
                              </Typography>
                            </Box>
                          </TableCell>

                          {/* Sale Channel */}
                          <TableCell>
                            {row.SaleChannelName ? (
                              <Chip
                                label={row.SaleChannelName}
                                size="small"
                                sx={{ backgroundColor: "rgba(86, 58, 213, 0.08)", color: "var(--primary-color)", fontWeight: 600, fontSize: "11px", fontFamily: FONT_FAMILY }}
                              />
                            ) : (
                              <Typography variant="body2" color="textSecondary" sx={{ fontFamily: FONT_FAMILY }}>-</Typography>
                            )}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              )}

              {/* Table Pagination */}
              {totalProducts > rowsPerPage && (
                <Box sx={{ p: 2, display: "flex", justifyContent: "flex-end", borderTop: "1px solid #E5EAEE" }}>
                  <Pagination
                    count={Math.ceil(totalProducts / rowsPerPage)}
                    page={page}
                    onChange={handlePageChange}
                    color="primary"
                    shape="rounded"
                    size="medium"
                  />
                </Box>
              )}
            </Paper>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );
}

export default SaleDashboardPage;
