import React, { useState } from "react";
import {
  Box,
  Grid,
  TextField,
  MenuItem,
  Button,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  TablePagination,
  CircularProgress,
  Paper,
  InputLabel,
  Popover,
  Typography,
  IconButton,
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import moment from "moment";
import { AdvanceSearchOrders } from "../../../api/AxiosInterceptors";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { styleSheet } from "../../../assets/styles/style";

function TruncatedDescription({ text }) {
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);

  if (!text || !text.toString().trim()) return "-";

  const strText = text.toString();
  const isLong = strText.length > 30;
  const shortText = isLong ? strText.substring(0, 30) + "..." : strText;

  return (
    <Box sx={{ display: "flex", alignItems: "center", width: "100%", justifyContent: "space-between" }}>
      <Box sx={{ fontSize: "12px", whiteSpace: "normal", wordBreak: "break-word", flex: 1 }}>
        {shortText}
      </Box>
      {isLong && (
        <>
          <IconButton
            size="small"
            sx={{ p: 0.2, ml: 0.5 }}
            onClick={(e) => {
              e.stopPropagation();
              setAnchorEl(e.currentTarget);
            }}
          >
            {open ? (
              <ExpandLessIcon sx={{ fontSize: 16, color: "var(--primary-color)" }} />
            ) : (
              <ExpandMoreIcon sx={{ fontSize: 16, color: "var(--primary-color)" }} />
            )}
          </IconButton>
          <Popover
            open={open}
            anchorEl={anchorEl}
            onClose={(e) => {
              if (e) e.stopPropagation();
              setAnchorEl(null);
            }}
            anchorOrigin={{
              vertical: "bottom",
              horizontal: "left",
            }}
            transformOrigin={{
              vertical: "top",
              horizontal: "left",
            }}
            PaperProps={{
              sx: {
                p: 1.5,
                maxWidth: 350,
                maxHeight: 250,
                fontSize: "12px",
                whiteSpace: "normal",
                wordBreak: "break-word",
                boxShadow: "0px 4px 20px rgba(0,0,0,0.15)",
                borderRadius: "8px",
              },
            }}
          >
            <Typography variant="body2" sx={{ fontSize: "12px", color: "#333", whiteSpace: "pre-wrap" }}>
              {strText}
            </Typography>
          </Popover>
        </>
      )}
    </Box>
  );
}

const SEARCH_TYPE_OPTIONS = [
  { label: "All", value: "All" },
  { label: "Order No", value: "OrderNo" },
  { label: "Name", value: "Name" },
  { label: "Mobile", value: "Mobile" },
  { label: "Reference No", value: "RefNo" },
  { label: "Tracking No", value: "TrackingNo" },
];

export default function AdvanceSearchModal({ open, onClose, onSelectOrder }) {
  const [searchType, setSearchType] = useState("All");
  const [searchQuery, setSearchQuery] = useState("");
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [hasSearched, setHasSearched] = useState(false);
  const [selectedRowLoading, setSelectedRowLoading] = useState("");

  const fetchSearchData = async (typeVal, queryVal, pageVal, sizeVal) => {
    setLoading(true);
    setHasSearched(true);
    try {
      const payload = {
        searchType: typeVal,
        searchQuery: queryVal,
        start: pageVal * sizeVal,
        length: sizeVal,
      };
      const res = await AdvanceSearchOrders(payload);
      if (res?.data?.isSuccess || res?.data?.result) {
        const dataObj = res?.data?.result || {};
        const listData = dataObj.list || dataObj.List || [];
        const countData = dataObj.totalCount || dataObj.TotalCount || 0;
        setResults(listData);
        setTotalCount(countData);
      } else {
        setResults([]);
        setTotalCount(0);
      }
    } catch (err) {
      console.error("Advance search error:", err);
      setResults([]);
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchClick = () => {
    setPage(0);
    fetchSearchData(searchType, searchQuery, 0, pageSize);
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSearchClick();
    }
  };

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
    fetchSearchData(searchType, searchQuery, newPage, pageSize);
  };

  const handleChangeRowsPerPage = (event) => {
    const newSize = parseInt(event.target.value, 10);
    setPageSize(newSize);
    setPage(0);
    fetchSearchData(searchType, searchQuery, 0, newSize);
  };

  const handleRowClick = async (orderNo) => {
    if (selectedRowLoading) return;
    setSelectedRowLoading(orderNo);
    try {
      if (onSelectOrder) {
        await onSelectOrder(orderNo);
      }
    } catch (err) {
      console.error("Select order error:", err);
    } finally {
      setSelectedRowLoading("");
      if (onClose) {
        onClose();
      }
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      title={"Advance Search"}
      maxWidth="lg"
    >
      <Box sx={{ p: 1 }}>
        {/* Controls Bar */}
        <Grid container spacing={2} alignItems="center" mb={2}>
          <Grid item xs={12} sm={4} md={3}>
            <InputLabel sx={styleSheet.inputLabel}>{"Search By"}</InputLabel>
            <TextField
              select
              fullWidth
              size="small"
              value={searchType}
              onChange={(e) => setSearchType(e.target.value)}
            >
              {SEARCH_TYPE_OPTIONS.map((opt) => (
                <MenuItem key={opt.value} value={opt.value}>
                  {opt.label}
                </MenuItem>
              ))}
            </TextField>
          </Grid>
          <Grid item xs={12} sm={5} md={6}>
            <InputLabel sx={styleSheet.inputLabel}>{"Search Query"}</InputLabel>
            <TextField
              fullWidth
              size="small"
              placeholder="Type order no, name, mobile, etc..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              onKeyDown={handleKeyDown}
            />
          </Grid>
          <Grid item xs={12} sm={3} md={3} sx={{ mt: 2.5 }}>
            <Button
              fullWidth
              variant="contained"
              sx={{
                background:
                  "var(--primary-color)",
                height: "40px",
                borderRadius: "8px",
                textTransform: "none",
                fontWeight: 600,
              }}
              startIcon={<SearchIcon />}
              onClick={handleSearchClick}
            >
              Search
            </Button>
          </Grid>
        </Grid>

        {/* Results Table */}
        <Paper
          variant="outlined"
          sx={{
            overflow: "hidden",
            minHeight: 250,
            position: "relative",
            borderRadius: "12px",
          }}
        >
          {loading && (
            <Box
              sx={{
                position: "absolute",
                top: 0,
                left: 0,
                right: 0,
                bottom: 0,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                bgcolor: "rgba(255, 255, 255, 0.7)",
                zIndex: 10,
              }}
            >
              <CircularProgress size={32} />
            </Box>
          )}

          <Table size="small">
            <TableHead sx={{ bgcolor: "#f8f9fa" }}>
              <TableRow>
                <TableCell sx={{ fontWeight: "bold" }}>Order No</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>Customer Name</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>Mobile</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>Ref No</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>Tracking No</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>Description</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>Remarks</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>Order Date</TableCell>
                <TableCell sx={{ fontWeight: "bold" }}>Amount</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {results.length > 0 ? (
                results.map((row) => {
                  const oNo = row.OrderNo || row.orderNo || "";
                  const isRowLoading = selectedRowLoading === oNo;
                  return (
                    <TableRow
                      key={row.OrderId || row.orderId || oNo}
                      hover
                      sx={{ cursor: "pointer", opacity: isRowLoading ? 0.6 : 1 }}
                      onClick={() => handleRowClick(oNo)}
                    >
                      <TableCell>
                        <Box
                          sx={{
                            color: "var(--primary-color)",
                            fontWeight: "bold",
                            textDecoration: "underline",
                            display: "flex",
                            alignItems: "center",
                            gap: 1,
                          }}
                        >
                          {isRowLoading && <CircularProgress size={16} sx={{ color: "var(--primary-color)" }} />}
                          {oNo}
                        </Box>
                      </TableCell>
                      <TableCell>
                        {row.CustomerName || row.customerName || "-"}
                      </TableCell>
                      <TableCell>
                        {row.Mobile1 || row.mobile1 || "-"}
                      </TableCell>
                      <TableCell>{row.RefNo || row.refNo || "-"}</TableCell>
                      <TableCell>
                        {row.CarrierTrackingNo || row.carrierTrackingNo || "-"}
                      </TableCell>
                      <TableCell>
                        <TruncatedDescription text={row.Description || row.description} />
                      </TableCell>
                      <TableCell>
                        <TruncatedDescription text={row.Remarks || row.remarks} />
                      </TableCell>
                      <TableCell>
                        {row.OrderDate || row.orderDate
                          ? moment(row.OrderDate || row.orderDate).format(
                              "DD/MM/YYYY"
                            )
                          : "-"}
                      </TableCell>
                      <TableCell>
                        {row.Amount !== undefined && row.Amount !== null
                          ? Number(row.Amount).toFixed(2)
                          : "0.00"}
                      </TableCell>
                    </TableRow>
                  );
                })
              ) : (
                <TableRow>
                  <TableCell
                    colSpan={9}
                    align="center"
                    sx={{ py: 4, color: "text.secondary" }}
                  >
                    {hasSearched
                      ? "No matching orders found."
                      : "Choose search type, enter a query and click Search."}
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>

          {totalCount > 0 && (
            <TablePagination
              component="div"
              count={totalCount}
              page={page}
              onPageChange={handleChangePage}
              rowsPerPage={pageSize}
              onRowsPerPageChange={handleChangeRowsPerPage}
              rowsPerPageOptions={[5, 10, 25, 50]}
            />
          )}
        </Paper>
      </Box>
    </ModalComponent>
  );
}
