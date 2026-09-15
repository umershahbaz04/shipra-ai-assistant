import React, { useState, useMemo, useRef, useEffect } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  Chip,
  IconButton,
  Menu,
  MenuItem,
  InputAdornment,
  Avatar,
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import DataGridComponent from "../../../../.reUseableComponents/DataGrid/DataGridComponent";
import { GetAllSaleChannelByLookupIdForSelection } from "../../../../api/AxiosInterceptors";

const ChannelLogoImage = ({ channelName, code, logoUrl }) => {
  const displayCode = code || (channelName ? channelName[0]?.toUpperCase() : "S");

  return (
    <Avatar
      variant="rounded"
      src={logoUrl || ""}
      sx={{
        width: 26,
        height: 26,
        fontSize: "12px",
        fontWeight: 700,
        border: "1px solid #e2e2e8",
        backgroundColor: "#fff",
        color: "var(--primary-color)",
        p: "2px",
        flexShrink: 0,
        "& img": {
          objectFit: "contain",
        },
      }}
    >
      {displayCode}
    </Avatar>
  );
};

const formatLastSync = (rawUpdatedOn, rawCreatedOn, createdOnStr) => {
  let dateVal = rawUpdatedOn || rawCreatedOn;
  if (!dateVal) return "Just now";

  if (typeof dateVal === "string" && !dateVal.endsWith("Z") && !dateVal.includes("+")) {
    dateVal = dateVal + "Z";
  }

  const date = new Date(dateVal);
  if (isNaN(date.getTime())) return createdOnStr || "Just now";

  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  if (diffMs < 0) return "Just now";

  const diffMins = Math.floor(diffMs / (1000 * 60));
  const diffHours = Math.floor(diffMins / 60);
  const diffDays = Math.floor(diffHours / 24);

  if (diffMins < 2) return "Just now";
  if (diffMins < 60) return `${diffMins} mins ago`;
  if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? "s" : ""} ago`;
  if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? "s" : ""} ago`;

  return date.toLocaleDateString("en-GB", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
};

export default function SyncPolicyList({
  policies = [],
  loading = false,
  onOpenCreateDrawer,
  onEditPolicy,
  onDeletePolicy,
  onToggleStatus,
  onFilterChange,
  tabFilter = "all", // "all", "active", "in-active"
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("all");
  const [channelFilter, setChannelFilter] = useState("all");
  const [availableChannels, setAvailableChannels] = useState([]);

  // Filter Popover / Menus
  const [statusAnchorEl, setStatusAnchorEl] = useState(null);
  const [channelAnchorEl, setChannelAnchorEl] = useState(null);

  // Kebab Action Menu State
  const [menuAnchorEl, setMenuAnchorEl] = useState(null);
  const [activeRow, setActiveRow] = useState(null);

  // Refs required by project shared DataGridComponent
  const getOrdersRef = useRef([]);
  const resetRowRef = useRef(false);

  // Trigger backend filter API on search or filter dropdown change (debounced 300ms)
  useEffect(() => {
    const timer = setTimeout(() => {
      if (onFilterChange) {
        onFilterChange({
          search: searchTerm,
          statusFilter,
          channelFilter,
          tabFilter,
        });
      }
    }, 300);
    return () => clearTimeout(timer);
  }, [searchTerm, statusFilter, channelFilter, tabFilter, onFilterChange]);

  // Fetch live backend channels for channel dropdown filter
  useEffect(() => {
    GetAllSaleChannelByLookupIdForSelection(0)
      .then((res) => {
        if (res?.data?.isSuccess && Array.isArray(res?.data?.result)) {
          const list = res.data.result.filter((c) => {
            if (!c || c.id === -1 || c.id === 0) return false;
            const label = (c.text || c.SaleChannelName || "").toLowerCase();
            return (
              !label.includes("sale person") &&
              !label.includes("saleperson") &&
              c.SaleChannelLookupId !== 4 &&
              c.SaleChannelLookupId !== 100
            );
          });
          setAvailableChannels(list);
        }
      })
      .catch((err) => console.error("Error fetching sale channels for filter", err));
  }, []);

  const filteredPolicies = useMemo(() => {
    return policies.filter((policy) => {
      // Tab filter
      if (tabFilter === "active" && !policy.Active) return false;
      if (tabFilter === "in-active" && policy.Active) return false;

      // Status dropdown filter
      if (statusFilter === "active" && !policy.Active) return false;
      if (statusFilter === "paused" && policy.Active) return false;

      // Channel dropdown filter (by ID or Channel Name)
      if (channelFilter !== "all") {
        const selId = Number(channelFilter);
        const matchesId = policy.SaleChannelConfigId === selId;
        const matchesName =
          (policy.SalesChannelName || "").toLowerCase() ===
          (channelFilter || "").toLowerCase();
        if (!matchesId && !matchesName) return false;
      }

      // Search query
      if (searchTerm && searchTerm.trim() !== "") {
        const query = searchTerm.toLowerCase();
        const stationNames = (policy.ProductStationMappings || [])
          .map((s) => s.StationName || "")
          .join(" ");
        const searchStr = `${policy.PolicyName || ""} ${policy.Description || ""
          } ${policy.SalesChannelName || ""} ${stationNames}`.toLowerCase();

        if (!searchStr.includes(query)) return false;
      }

      return true;
    });
  }, [policies, tabFilter, statusFilter, channelFilter, searchTerm]);

  const selectedChannelLabel = useMemo(() => {
    if (channelFilter === "all") return "All";
    const found = availableChannels.find(
      (c) => String(c.id) === String(channelFilter)
    );
    return found ? found.text || found.SaleChannelName : channelFilter;
  }, [channelFilter, availableChannels]);

  const handleOpenStatusMenu = (e) => setStatusAnchorEl(e.currentTarget);
  const handleCloseStatusMenu = () => setStatusAnchorEl(null);

  const handleOpenChannelMenu = (e) => setChannelAnchorEl(e.currentTarget);
  const handleCloseChannelMenu = () => setChannelAnchorEl(null);

  const handleOpenMenu = (e, row) => {
    e.stopPropagation();
    setMenuAnchorEl(e.currentTarget);
    setActiveRow(row);
  };

  const handleCloseMenu = () => {
    setMenuAnchorEl(null);
    setActiveRow(null);
  };

  const columns = useMemo(
    () => [
      {
        field: "PolicyName",
        headerName: "Policy Name",
        flex: 1.5,
        minWidth: 200,
        renderCell: (params) => {
          const row = params.row;
          return (
            <Box
              sx={{
                py: 1,
                display: "flex",
                flexDirection: "column",
                justifyContent: "center",
              }}
            >
              <Typography
                sx={{
                  fontWeight: 700,
                  fontSize: "13px",
                  color: "#1c1e23",
                  lineHeight: 1.3,
                }}
              >
                {row.PolicyName}
              </Typography>
              {row.Description && (
                <Typography
                  sx={{
                    fontSize: "11px",
                    color: "#858791",
                    mt: "2px",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    whiteSpace: "nowrap",
                  }}
                >
                  {row.Description}
                </Typography>
              )}
            </Box>
          );
        },
      },
      {
        field: "SalesChannelName",
        headerName: "Sales Channel",
        flex: 1.2,
        minWidth: 160,
        renderCell: (params) => {
          const row = params.row;
          return (
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                gap: 1,
                fontWeight: 700,
                fontSize: "13px",
                color: "#22242a",
              }}
            >
              <ChannelLogoImage
                channelName={row.SalesChannelName}
                code={row.SalesChannelCode}
                logoUrl={row.ChannelLogoUrl}
              />
              <span>{row.SalesChannelName}</span>
            </Box>
          );
        },
      },
      {
        field: "StationSummary",
        headerName: "Inventory Source",
        flex: 1.5,
        minWidth: 180,
        renderCell: (params) => {
          const row = params.row;
          const stationsStr = row.StationSummary || "All Stations";
          const stationsArr = stationsStr.split(",").map((s) => s.trim());
          return (
            <Box
              sx={{
                display: "flex",
                gap: "6px",
                flexWrap: "wrap",
                alignItems: "center",
                py: 1,
              }}
            >
              {stationsArr.slice(0, 2).map((st, idx) => (
                <Box
                  key={idx}
                  component="span"
                  sx={{
                    display: "inline-flex",
                    alignItems: "center",
                    px: "10px",
                    py: "3px",
                    borderRadius: "12px",
                    border: "1px solid #dcdfe6",
                    backgroundColor: "#f1f3f7",
                    color: "#383a42",
                    fontSize: "11px",
                    fontWeight: 500,
                    lineHeight: 1.3,
                    whiteSpace: "nowrap",
                  }}
                >
                  {st}
                </Box>
              ))}
              {stationsArr.length > 2 && (
                <Box
                  component="span"
                  sx={{
                    display: "inline-flex",
                    alignItems: "center",
                    px: "8px",
                    py: "3px",
                    borderRadius: "12px",
                    border: "1px solid #dcdfe6",
                    backgroundColor: "#e8eaef",
                    color: "#525560",
                    fontSize: "11px",
                    fontWeight: 600,
                    lineHeight: 1.3,
                    whiteSpace: "nowrap",
                  }}
                >
                  +{stationsArr.length - 2}
                </Box>
              )}
            </Box>
          );
        },
      },
      {
        field: "ProductsScopeSummary",
        headerName: "Products",
        flex: 1.2,
        minWidth: 150,
        renderCell: (params) => {
          const row = params.row;
          return (
            <Typography
              sx={{
                fontSize: "12px",
                fontWeight: 600,
                color: "#383a42",
              }}
            >
              {row.ProductsScopeSummary || "All products"}
            </Typography>
          );
        },
      },
      {
        field: "InventoryRuleSummary",
        headerName: "Inventory Rule",
        flex: 1.2,
        minWidth: 150,
        renderCell: (params) => {
          const row = params.row;
          return (
            <Typography
              sx={{
                fontSize: "12px",
                fontWeight: 600,
                color: "#383a42",
              }}
            >
              {row.InventoryRuleSummary || "Buffer 5 · Max 50"}
            </Typography>
          );
        },
      },
      {
        field: "Active",
        headerName: "Status",
        flex: 0.8,
        minWidth: 100,
        renderCell: (params) => {
          const row = params.row;
          return (
            <Box sx={{ display: "flex", alignItems: "center", gap: "6px" }}>
              <Box
                sx={{
                  width: "7px",
                  height: "7px",
                  borderRadius: "50%",
                  backgroundColor: row.Active ? "#2e7d32" : "#9e9e9e",
                }}
              />
              <Typography
                sx={{
                  fontSize: "12px",
                  fontWeight: 700,
                  color: row.Active ? "#2e7d32" : "#757575",
                }}
              >
                {row.Active ? "Active" : "Paused"}
              </Typography>
            </Box>
          );
        },
      },
      {
        field: "LastSync",
        headerName: "Last Sync",
        flex: 1,
        minWidth: 120,
        renderCell: (params) => {
          const row = params.row;
          return (
            <Typography sx={{ fontSize: "12px", color: "#6e707a" }}>
              {formatLastSync(row.RawUpdatedOn, row.RawCreatedOn, row.CreatedOn)}
            </Typography>
          );
        },
      },
      {
        field: "actions",
        headerName: "Actions",
        width: 80,
        sortable: false,
        renderCell: (params) => {
          const row = params.row;
          return (
            <IconButton
              size="small"
              onClick={(e) => handleOpenMenu(e, row)}
              sx={{
                color: "#6e707a",
                "&:hover": { backgroundColor: "#f0f0f4" },
              }}
            >
              <MoreVertIcon fontSize="small" />
            </IconButton>
          );
        },
      },
    ],
    []
  );

  return (
    <Box
      sx={{
        backgroundColor: "#fff",
        borderRadius: "8px",
        border: "1px solid #e5e5eb",
        p: 2.5,
        mt: 2,
      }}
    >
      {/* Top Controls Bar */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 1.5,
          mb: 2,
          flexWrap: "wrap",
        }}
      >
        {/* Search Field */}
        <TextField
          placeholder="Search policy, channel or station"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          size="small"
          sx={{
            width: "320px",
            "& .MuiOutlinedInput-root": {
              height: "36px",
              fontSize: "13px",
              borderRadius: "6px",
              backgroundColor: "#fff",
              "& fieldset": { borderColor: "#d8d8e2" },
              "&:hover fieldset": { borderColor: "var(--primary-color)" },
            },
          }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon sx={{ color: "#9597a1", fontSize: 20 }} />
              </InputAdornment>
            ),
          }}
        />

        {/* Filter: Status */}
        <Button
          onClick={handleOpenStatusMenu}
          endIcon={<KeyboardArrowDownIcon />}
          sx={{
            height: "36px",
            borderRadius: "6px",
            border: "1px solid #d8d8e2",
            color: "#383a42",
            textTransform: "none",
            fontSize: "13px",
            fontWeight: 600,
            px: 1.5,
            backgroundColor: "#fff",
            "&:hover": {
              borderColor: "var(--primary-color)",
              backgroundColor: "#fff",
            },
          }}
        >
          Status:{" "}
          <Box
            component="span"
            sx={{ ml: 0.5, color: "#666", fontWeight: 500 }}
          >
            {statusFilter === "all"
              ? "All"
              : statusFilter === "active"
                ? "Active"
                : "Paused"}
          </Box>
        </Button>
        <Menu
          anchorEl={statusAnchorEl}
          open={Boolean(statusAnchorEl)}
          onClose={handleCloseStatusMenu}
          disableScrollLock
        >
          <MenuItem
            onClick={() => {
              setStatusFilter("all");
              handleCloseStatusMenu();
            }}
            selected={statusFilter === "all"}
            sx={{ fontSize: "13px" }}
          >
            All Statuses
          </MenuItem>
          <MenuItem
            onClick={() => {
              setStatusFilter("active");
              handleCloseStatusMenu();
            }}
            selected={statusFilter === "active"}
            sx={{ fontSize: "13px" }}
          >
            Active
          </MenuItem>
          <MenuItem
            onClick={() => {
              setStatusFilter("paused");
              handleCloseStatusMenu();
            }}
            selected={statusFilter === "paused"}
            sx={{ fontSize: "13px" }}
          >
            Paused
          </MenuItem>
        </Menu>

        {/* Filter: Sales Channel (with Logos) */}
        <Button
          onClick={handleOpenChannelMenu}
          endIcon={<KeyboardArrowDownIcon />}
          sx={{
            height: "36px",
            borderRadius: "6px",
            border: "1px solid #d8d8e2",
            color: "#383a42",
            textTransform: "none",
            fontSize: "13px",
            fontWeight: 600,
            px: 1.5,
            backgroundColor: "#fff",
            "&:hover": {
              borderColor: "var(--primary-color)",
              backgroundColor: "#fff",
            },
          }}
        >
          Channel:{" "}
          <Box
            component="span"
            sx={{ ml: 0.5, color: "#666", fontWeight: 500 }}
          >
            {selectedChannelLabel}
          </Box>
        </Button>
        <Menu
          anchorEl={channelAnchorEl}
          open={Boolean(channelAnchorEl)}
          onClose={handleCloseChannelMenu}
          disableScrollLock
          PaperProps={{
            sx: {
              maxHeight: "320px",
              boxShadow: "0 4px 16px rgba(0,0,0,0.1)",
              borderRadius: "6px",
            },
          }}
        >
          <MenuItem
            onClick={() => {
              setChannelFilter("all");
              handleCloseChannelMenu();
            }}
            selected={channelFilter === "all"}
            sx={{ fontSize: "13px" }}
          >
            All Channels
          </MenuItem>
          {availableChannels.map((c) => {
            const channelName = c.text || c.SaleChannelName || `Channel #${c.id}`;
            const logoUrl = c.ImageUrl || c.imageUrl || c.ChannelLogoUrl || "";
            return (
              <MenuItem
                key={c.id}
                onClick={() => {
                  setChannelFilter(String(c.id));
                  handleCloseChannelMenu();
                }}
                selected={channelFilter === String(c.id)}
                sx={{
                  fontSize: "13px",
                  display: "flex",
                  alignItems: "center",
                  gap: 1.25,
                }}
              >
                <ChannelLogoImage
                  channelName={channelName}
                  code={channelName[0]}
                  logoUrl={logoUrl}
                />
                <span>{channelName}</span>
              </MenuItem>
            );
          })}
        </Menu>

        {/* Reset / Active filter badge */}
        {(statusFilter !== "all" ||
          channelFilter !== "all" ||
          searchTerm.trim() !== "") && (
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
              <Button
                onClick={() => {
                  setStatusFilter("all");
                  setChannelFilter("all");
                  setSearchTerm("");
                }}
                sx={{
                  color: "var(--primary-color)",
                  fontSize: "12px",
                  fontWeight: 600,
                  textTransform: "none",
                  p: 0,
                }}
              >
                Clear
              </Button>
            </Box>
          )}

        <Box sx={{ flex: 1 }} />
      </Box>

      {/* Shared Reusable DataGridComponent */}
      <DataGridComponent
        columns={columns}
        rows={filteredPolicies}
        loading={loading}
        getRowId={(row) => row?.InventorySyncPolicyId}
        getOrdersRef={getOrdersRef}
        resetRowRef={resetRowRef}
        getRowHeight={() => "auto"}
        checkboxSelection={true}
        rowPerPage={10}
        height="calc(100vh - 220px)"
      />

      {/* Row Kebab Action Menu */}
      <Menu
        anchorEl={menuAnchorEl}
        open={Boolean(menuAnchorEl)}
        onClose={handleCloseMenu}
        disableScrollLock
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "right",
        }}
        transformOrigin={{
          vertical: "top",
          horizontal: "right",
        }}
        PaperProps={{
          sx: {
            minWidth: "150px",
            boxShadow: "0 4px 16px rgba(0,0,0,0.12)",
            borderRadius: "6px",
            py: 0.5,
          },
        }}
      >
        <MenuItem
          onClick={() => {
            const row = activeRow;
            handleCloseMenu();
            if (onEditPolicy && row) onEditPolicy(row);
          }}
          sx={{ fontSize: "13px" }}
        >
          Edit Policy
        </MenuItem>

        <MenuItem
          onClick={() => {
            const row = activeRow;
            handleCloseMenu();
            if (onToggleStatus && row) onToggleStatus(row.InventorySyncPolicyId);
          }}
          sx={{ fontSize: "13px" }}
        >
          {activeRow?.Active ? "Pause Policy" : "Resume Policy"}
        </MenuItem>

        <MenuItem
          onClick={() => {
            const row = activeRow;
            handleCloseMenu();
            if (onDeletePolicy && row)
              onDeletePolicy(row.InventorySyncPolicyId);
          }}
          sx={{ fontSize: "13px", color: "#d94747" }}
        >
          Delete Policy
        </MenuItem>
      </Menu>
    </Box>
  );
}
