import React, { useState, useEffect } from "react";
import {
  Box,
  Grid,
  Card,
  CardContent,
  Typography,
  Avatar,
  CircularProgress,
  IconButton,
  TextField,
  MenuItem,
  InputAdornment,
  Tooltip,
  Divider,
  Paper,
  InputLabel,
  Button
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SearchIcon from "@mui/icons-material/Search";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import AssignmentIcon from "@mui/icons-material/Assignment";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import ReactApexChart from "react-apexcharts";
import { GetCarrierDashboardStats } from "../../api/AxiosInterceptors";
import CustomReactDatePickerInputFilter from "../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import Colors from "../../utilities/helpers/Colors";
import UtilityClass from "../../utilities/UtilityClass";

const FONT_FAMILY = "'Lato Medium', 'Inter Medium', 'Arial', sans-serif";

const CarrierSuccessGauge = ({ deliveryRatio, color }) => {
  const chartSeries = [Number(deliveryRatio)];
  const chartOptions = {
    chart: {
      type: "radialBar",
      sparkline: { enabled: true }
    },
    plotOptions: {
      radialBar: {
        hollow: { size: "65%" },
        dataLabels: {
          name: { show: false },
          value: {
            offsetY: 6,
            fontSize: "15px",
            fontFamily: FONT_FAMILY,
            fontWeight: "800",
            color: "#2C3E50",
            formatter: () => `${deliveryRatio}%`
          }
        },
        track: {
          background: "#F2F4F4"
        }
      }
    },
    colors: [color]
  };

  return (
    <ReactApexChart
      options={chartOptions}
      series={chartSeries}
      type="radialBar"
      height={130}
      width={130}
    />
  );
};

const OverallCarriersChart = ({ carriers }) => {
  if (carriers.length === 0) return null;

  const categories = carriers.map(c => c.CarrierName || c.carrierName || "Unknown");
  const series = [
    {
      name: "To Be Dispatched",
      data: carriers.map(c => Number(c.ToBeDispatched || c.TOBEDISPATCHED || c.tobetispatched || 0))
    },
    {
      name: "To Be Delivered",
      data: carriers.map(c => Number(c.ToBeDelivered || c.TOBEDELIVERED || c.tobedelivered || 0))
    },
    {
      name: "Returned",
      data: carriers.map(c => Number(c.Returned || c.RETURNED || c.returned || 0))
    },
    {
      name: "Completed",
      data: carriers.map(c => Number(c.Completed || c.COMPLETED || c.completed || 0))
    }
  ];

  const options = {
    chart: {
      type: "bar",
      stacked: true,
      toolbar: { show: false },
      zoom: { enabled: false }
    },
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: "40%",
        borderRadius: 4
      }
    },
    colors: ["#3498db", "#f39c12", "#e74c3c", "#2ecc71"],
    xaxis: {
      categories: categories,
      labels: {
        style: {
          fontFamily: FONT_FAMILY,
          fontWeight: 600,
          colors: "#7F8C8D"
        }
      }
    },
    yaxis: {
      title: {
        text: "Shipments Count",
        style: {
          fontFamily: FONT_FAMILY,
          fontWeight: 600
        }
      }
    },
    legend: {
      position: "top",
      fontFamily: FONT_FAMILY,
      fontWeight: 600
    },
    fill: {
      opacity: 1
    },
    tooltip: {
      y: {
        formatter: (val) => `${val} Shipments`
      }
    }
  };

  return (
    <ReactApexChart
      options={options}
      series={series}
      type="bar"
      height={300}
    />
  );
};

function CarrierDashboardPage() {
  const navigate = useNavigate();
  const [carriersData, setCarriersData] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [sortBy, setSortBy] = useState("shipments");
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [isChartVisible, setIsChartVisible] = useState(true);

  useEffect(() => {
    fetchCarrierStats();
  }, []);

  const fetchCarrierStats = async (start = startDate, end = endDate) => {
    setIsLoading(true);
    try {
      const body = {
        FilterModel: {
          createdFrom: start ? start.toISOString() : null,
          createdTo: end ? end.toISOString() : null,
          start: 0,
          length: 1000,
          search: "",
          sortDir: "desc",
          sortCol: 0
        }
      };

      const res = await GetCarrierDashboardStats(body);
      if (res?.data?.result) {
        setCarriersData(res.data.result?.carrierGoupedList || []);
      }
    } catch (err) {
      console.error("Error fetching carrier stats:", err);
    } finally {
      setIsLoading(false);
    }
  };

  // Calculate aggregated stats across all carriers
  const totalStats = carriersData.reduce((acc, carrier) => {
    const toBeDispatched = Number(carrier.ToBeDispatched || carrier.TOBEDISPATCHED || carrier.tobetispatched || 0);
    const toBeDelivered = Number(carrier.ToBeDelivered || carrier.TOBEDELIVERED || carrier.tobedelivered || 0);
    const returned = Number(carrier.Returned || carrier.RETURNED || carrier.returned || 0);
    const completed = Number(carrier.Completed || carrier.COMPLETED || carrier.completed || 0);

    acc.toBeDispatched += toBeDispatched;
    acc.toBeDelivered += toBeDelivered;
    acc.returned += returned;
    acc.completed += completed;
    acc.total += (toBeDispatched + toBeDelivered + returned + completed);
    return acc;
  }, { toBeDispatched: 0, toBeDelivered: 0, returned: 0, completed: 0, total: 0 });

  const overallDeliveryRatio = totalStats.total > 0 ? ((totalStats.completed / totalStats.total) * 100).toFixed(1) : "0.0";

  // Filter & Sort carrier list
  const filteredCarriers = carriersData
    .filter(carrier => {
      const name = (carrier.CarrierName || carrier.carrierName || "Unknown Carrier").toLowerCase();
      return name.includes(searchTerm.toLowerCase());
    })
    .sort((a, b) => {
      const nameA = a.CarrierName || a.carrierName || "";
      const nameB = b.CarrierName || b.carrierName || "";

      const toBeDispatchedA = Number(a.ToBeDispatched || a.TOBEDISPATCHED || a.tobetispatched || 0);
      const toBeDeliveredA = Number(a.ToBeDelivered || a.TOBEDELIVERED || a.tobedelivered || 0);
      const returnedA = Number(a.Returned || a.RETURNED || a.returned || 0);
      const completedA = Number(a.Completed || a.COMPLETED || a.completed || 0);
      const totalA = toBeDispatchedA + toBeDeliveredA + returnedA + completedA;

      const toBeDispatchedB = Number(b.ToBeDispatched || b.TOBEDISPATCHED || b.tobetispatched || 0);
      const toBeDeliveredB = Number(b.ToBeDelivered || b.TOBEDELIVERED || b.tobedelivered || 0);
      const returnedB = Number(b.Returned || b.RETURNED || b.returned || 0);
      const completedB = Number(b.Completed || b.COMPLETED || b.completed || 0);
      const totalB = toBeDispatchedB + toBeDeliveredB + returnedB + completedB;

      const ratioA = totalA > 0 ? (completedA / totalA) : 0;
      const ratioB = totalB > 0 ? (completedB / totalB) : 0;

      if (sortBy === "name") {
        return nameA.localeCompare(nameB);
      } else if (sortBy === "shipments") {
        return totalB - totalA;
      } else if (sortBy === "ratio") {
        return ratioB - ratioA;
      }
      return 0;
    });

  return (
    <Box
      sx={{
        p: 3,
        minHeight: "85vh",
        background: "#F4F6F9",
        fontFamily: FONT_FAMILY,
        borderRadius: "12px"
      }}
    >
      {/* Header & Date Pickers bar */}
      <Paper
        sx={{
          p: 2,
          mb: 4,
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
          <IconButton onClick={() => navigate("/dashboards")} sx={{ color: Colors.primary }}>
            <ArrowBackIcon />
          </IconButton>
          <Typography
            variant="h5"
            sx={{
              fontWeight: 700,
              color: "#2C3E50",
              fontFamily: "'Lato Bold', 'Inter Bold', 'Arial', sans-serif"
            }}
          >
            Carriers Dashboard
          </Typography>
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
            onClick={() => fetchCarrierStats(startDate, endDate)}
            sx={{
              height: "36px",
              borderRadius: "8px",
              textTransform: "capitalize",
              fontWeight: 600,
              px: 3,
              backgroundColor: Colors.primary
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
                fetchCarrierStats(null, null);
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

      {isLoading ? (
        <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "50vh" }}>
          <CircularProgress color="primary" />
        </Box>
      ) : carriersData.length === 0 ? (
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
            minHeight: "50vh",
            bgcolor: "#fff",
            borderRadius: "12px",
            p: 4
          }}
        >
          <Typography variant="h6" sx={{ color: "#7F8C8D" }}>
            No Carrier Statistics Found
          </Typography>
          <Typography variant="body2" sx={{ color: "#BDC3C7", mt: 1 }}>
            Make sure you have active shipments within the selected date range.
          </Typography>
        </Box>
      ) : (
        <>
          {/* Overall Summary Stats */}
          <Grid container spacing={3} sx={{ mb: 4 }}>
            {/* Total Shipments */}
            <Grid item xs={12} sm={6} md={3}>
              <Paper
                sx={{
                  p: 2.5,
                  borderRadius: "16px",
                  boxShadow: "0 4px 20px rgba(0,0,0,0.01)",
                  border: "1px solid rgba(0, 0, 0, 0.03)",
                  background: "linear-gradient(135deg, #fff 0%, #F5F7FA 100%)",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between"
                }}
              >
                <Box>
                  <Typography variant="body2" sx={{ color: "#7F8C8D", fontWeight: 600, fontSize: "11px", textTransform: "uppercase" }}>
                    Total Shipments
                  </Typography>
                  <Typography variant="h4" sx={{ fontWeight: 800, mt: 0.5, color: "#2C3E50" }}>
                    {totalStats.total}
                  </Typography>
                </Box>
                <Avatar sx={{ bgcolor: "rgba(52, 152, 219, 0.1)", color: "#3498db", width: 50, height: 50 }}>
                  <AssignmentIcon sx={{ fontSize: 24 }} />
                </Avatar>
              </Paper>
            </Grid>

            {/* Completed Deliveries */}
            <Grid item xs={12} sm={6} md={3}>
              <Paper
                sx={{
                  p: 2.5,
                  borderRadius: "16px",
                  boxShadow: "0 4px 20px rgba(0,0,0,0.01)",
                  border: "1px solid rgba(0, 0, 0, 0.03)",
                  background: "linear-gradient(135deg, #fff 0%, #E8F8F5 100%)",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between"
                }}
              >
                <Box>
                  <Typography variant="body2" sx={{ color: "#7F8C8D", fontWeight: 600, fontSize: "11px", textTransform: "uppercase" }}>
                    Completed Deliveries
                  </Typography>
                  <Typography variant="h4" sx={{ fontWeight: 800, mt: 0.5, color: "#2ECC71" }}>
                    {totalStats.completed}
                  </Typography>
                </Box>
                <Avatar sx={{ bgcolor: "rgba(46, 204, 113, 0.1)", color: "#2ECC71", width: 50, height: 50 }}>
                  <CheckCircleIcon sx={{ fontSize: 24 }} />
                </Avatar>
              </Paper>
            </Grid>

            {/* Pending Deliveries */}
            <Grid item xs={12} sm={6} md={3}>
              <Paper
                sx={{
                  p: 2.5,
                  borderRadius: "16px",
                  boxShadow: "0 4px 20px rgba(0,0,0,0.01)",
                  border: "1px solid rgba(0, 0, 0, 0.03)",
                  background: "linear-gradient(135deg, #fff 0%, #FEF9E7 100%)",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between"
                }}
              >
                <Box>
                  <Typography variant="body2" sx={{ color: "#7F8C8D", fontWeight: 600, fontSize: "11px", textTransform: "uppercase" }}>
                    Pending Deliveries
                  </Typography>
                  <Typography variant="h4" sx={{ fontWeight: 800, mt: 0.5, color: "#F39C12" }}>
                    {totalStats.toBeDelivered}
                  </Typography>
                </Box>
                <Avatar sx={{ bgcolor: "rgba(243, 156, 18, 0.1)", color: "#F39C12", width: 50, height: 50 }}>
                  <LocalShippingIcon sx={{ fontSize: 24 }} />
                </Avatar>
              </Paper>
            </Grid>

            {/* Success Rate */}
            <Grid item xs={12} sm={6} md={3}>
              <Paper
                sx={{
                  p: 2.5,
                  borderRadius: "16px",
                  boxShadow: "0 4px 20px rgba(0,0,0,0.01)",
                  border: "1px solid rgba(0, 0, 0, 0.03)",
                  background: "linear-gradient(135deg, #fff 0%, #EBEDEF 100%)",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between"
                }}
              >
                <Box>
                  <Typography variant="body2" sx={{ color: "#7F8C8D", fontWeight: 600, fontSize: "11px", textTransform: "uppercase" }}>
                    Overall Success Rate
                  </Typography>
                  <Typography variant="h4" sx={{ fontWeight: 800, mt: 0.5, color: "#34495E" }}>
                    {overallDeliveryRatio}%
                  </Typography>
                </Box>
                <Avatar sx={{ bgcolor: "rgba(52, 73, 94, 0.1)", color: "#34495E", width: 50, height: 50 }}>
                  <TrendingUpIcon sx={{ fontSize: 24 }} />
                </Avatar>
              </Paper>
            </Grid>
          </Grid>

          {/* Graphical Analysis Area */}
          <Paper
            sx={{
              p: 3,
              mb: 4,
              borderRadius: "20px",
              boxShadow: "0 8px 30px rgba(0,0,0,0.02)",
              border: "1px solid rgba(0, 0, 0, 0.04)",
              bgcolor: "#fff"
            }}
          >
            <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
              <Typography variant="h6" sx={{ fontWeight: 700, color: "#2C3E50" }}>
                Carrier Shipment Distribution Comparison
              </Typography>
              <IconButton onClick={() => setIsChartVisible(!isChartVisible)}>
                {isChartVisible ? <ExpandLessIcon /> : <ExpandMoreIcon />}
              </IconButton>
            </Box>
            <Divider sx={{ mb: 2 }} />
            {isChartVisible && (
              <Box sx={{ minHeight: 300 }}>
                <OverallCarriersChart carriers={filteredCarriers.slice(0, 15)} />
                {filteredCarriers.length > 15 && (
                  <Typography variant="caption" sx={{ color: "#7F8C8D", display: "block", textAlign: "center", mt: 1 }}>
                    * Showing top 15 carriers for optimal chart layout. Filter using the search box below.
                  </Typography>
                )}
              </Box>
            )}
          </Paper>

          {/* Search and Sort Toolbar */}
          <Paper
            sx={{
              p: 2,
              mb: 4,
              borderRadius: "16px",
              boxShadow: "0 4px 20px rgba(0,0,0,0.01)",
              border: "1px solid rgba(0, 0, 0, 0.03)",
              display: "flex",
              flexWrap: "wrap",
              gap: 2,
              alignItems: "center",
              justifyContent: "space-between",
              bgcolor: "#fff"
            }}
          >
            <TextField
              placeholder="Search carrier..."
              variant="outlined"
              size="small"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              sx={{
                minWidth: 280,
                "& .MuiOutlinedInput-root": {
                  borderRadius: "12px",
                  backgroundColor: "#F8F9F9"
                }
              }}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon sx={{ color: "#BDC3C7" }} />
                  </InputAdornment>
                )
              }}
            />

            <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
              <Typography variant="body2" sx={{ color: "#7F8C8D", fontWeight: 600 }}>
                Sort By:
              </Typography>
              <TextField
                select
                size="small"
                value={sortBy}
                onChange={(e) => setSortBy(e.target.value)}
                sx={{
                  minWidth: 180,
                  "& .MuiOutlinedInput-root": {
                    borderRadius: "12px"
                  }
                }}
              >
                <MenuItem value="shipments">Total Shipments</MenuItem>
                <MenuItem value="ratio">Success Rate</MenuItem>
                <MenuItem value="name">Carrier Name</MenuItem>
              </TextField>
            </Box>
          </Paper>

          {/* Main Carriers Grid */}
          <Grid container spacing={3}>
            {filteredCarriers.map((carrier, index) => {
              const name = carrier.CarrierName || carrier.carrierName || "Unknown Carrier";
              const logo = carrier.CarrierImage || carrier.carrierImage || "";

              const toBeDispatched = Number(carrier.ToBeDispatched || carrier.TOBEDISPATCHED || carrier.tobetispatched || 0);
              const toBeDelivered = Number(carrier.ToBeDelivered || carrier.TOBEDELIVERED || carrier.tobedelivered || 0);
              const returned = Number(carrier.Returned || carrier.RETURNED || carrier.returned || 0);
              const completed = Number(carrier.Completed || carrier.COMPLETED || carrier.completed || 0);

              const totalShipments = toBeDispatched + toBeDelivered + returned + completed;
              const deliveryRatio = totalShipments > 0 ? ((completed / totalShipments) * 100).toFixed(1) : "0.0";

              return (
                <Grid item xs={12} sm={6} md={4} lg={3} key={carrier.CarrierId || carrier.carrierId || index}>
                  <Card
                    sx={{
                      height: "100%",
                      borderRadius: "20px",
                      boxShadow: "0 8px 30px rgba(0,0,0,0.03)",
                      border: "1px solid rgba(0, 0, 0, 0.04)",
                      transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
                      display: "flex",
                      flexDirection: "column",
                      position: "relative",
                      overflow: "hidden",
                      background: "#fff",
                      "&:hover": {
                        transform: "translateY(-6px)",
                        boxShadow: "0 15px 35px rgba(0,0,0,0.08)"
                      }
                    }}
                  >
                    {/* Visual indicator bar at the top of card */}
                    <Box
                      sx={{
                        height: 4,
                        width: "100%",
                        background: "linear-gradient(90deg, #3498db, #2ecc71)"
                      }}
                    />
                    <CardContent sx={{ p: 3, flexGrow: 1, display: "flex", flexDirection: "column" }}>
                      {/* Header: Avatar & Name */}
                      <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 1.5 }}>
                        <Avatar
                          src={logo}
                          alt={name}
                          sx={{
                            width: 44,
                            height: 44,
                            border: "1px solid rgba(0, 0, 0, 0.05)",
                            bgcolor: "#fff",
                            objectFit: "contain",
                            p: 0.5,
                            "& img": {
                              objectFit: "contain"
                            }
                          }}
                        >
                          {name.charAt(0)}
                        </Avatar>
                        <Box sx={{ maxWidth: 120 }}>
                          <Typography
                            variant="subtitle1"
                            sx={{
                              fontWeight: 700,
                              color: "#2C3E50",
                              fontSize: "13px",
                              lineHeight: "1.2",
                              whiteSpace: "nowrap",
                              overflow: "hidden",
                              textOverflow: "ellipsis"
                            }}
                          >
                            {name}
                          </Typography>
                          <Typography variant="caption" sx={{ color: "#BDC3C7", display: "block" }}>
                            ID: {carrier.CarrierId}
                          </Typography>
                        </Box>
                      </Box>

                      <Divider sx={{ my: 1 }} />

                      {/* Gauge Chart & Total Stats */}
                      <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", my: 1.5 }}>
                        <Box>
                          <Typography variant="caption" sx={{ color: "#95A5A6", fontWeight: 600, textTransform: "uppercase", fontSize: "10px" }}>
                            Total Shipments
                          </Typography>
                          <Typography variant="h4" sx={{ fontWeight: 800, color: "#34495E" }}>
                            {totalShipments}
                          </Typography>
                          <Typography variant="caption" sx={{ color: "#7F8C8D", display: "block", mt: 0.5 }}>
                            Success rate gauge
                          </Typography>
                        </Box>

                        <Box sx={{ width: 120, height: 120, display: "flex", justifyContent: "center", alignItems: "center", mr: -1 }}>
                          {totalShipments > 0 ? (
                            <CarrierSuccessGauge deliveryRatio={deliveryRatio} color="#2ecc71" />
                          ) : (
                            <Box
                              sx={{
                                width: 75,
                                height: 75,
                                borderRadius: "50%",
                                border: "2px dashed #BDC3C7",
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center"
                              }}
                            >
                              <Typography variant="caption" sx={{ color: "#BDC3C7", fontSize: "9px" }}>
                                Empty
                              </Typography>
                            </Box>
                          )}
                        </Box>
                      </Box>

                      {/* Detail listing */}
                      <Box sx={{ mt: "auto", pt: 1.5, display: "flex", flexDirection: "column", gap: 1 }}>
                        <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                            <Box sx={{ width: 8, height: 8, borderRadius: "50%", bgcolor: "#3498db" }} />
                            <Typography variant="body2" sx={{ fontSize: "11px", color: "#7F8C8D" }}>To Be Dispatched</Typography>
                          </Box>
                          <Typography variant="body2" sx={{ fontSize: "11px", fontWeight: 700, color: "#2C3E50" }}>{toBeDispatched}</Typography>
                        </Box>

                        <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                            <Box sx={{ width: 8, height: 8, borderRadius: "50%", bgcolor: "#f39c12" }} />
                            <Typography variant="body2" sx={{ fontSize: "11px", color: "#7F8C8D" }}>To Be Delivered</Typography>
                          </Box>
                          <Typography variant="body2" sx={{ fontSize: "11px", fontWeight: 700, color: "#2C3E50" }}>{toBeDelivered}</Typography>
                        </Box>

                        <Divider sx={{ my: 0.5 }} />

                        <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                            <Box sx={{ width: 8, height: 8, borderRadius: "50%", bgcolor: "#2ecc71" }} />
                            <Typography variant="body2" sx={{ fontSize: "11px", color: "#7F8C8D" }}>Completed</Typography>
                          </Box>
                          <Typography variant="body2" sx={{ fontSize: "11px", fontWeight: 700, color: "#2C3E50" }}>{completed}</Typography>
                        </Box>

                        <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                            <Box sx={{ width: 8, height: 8, borderRadius: "50%", bgcolor: "#e74c3c" }} />
                            <Typography variant="body2" sx={{ fontSize: "11px", color: "#7F8C8D" }}>Returned</Typography>
                          </Box>
                          <Typography variant="body2" sx={{ fontSize: "11px", fontWeight: 700, color: "#2C3E50" }}>{returned}</Typography>
                        </Box>
                      </Box>
                    </CardContent>
                  </Card>
                </Grid>
              );
            })}
          </Grid>
        </>
      )}
    </Box>
  );
}

export default CarrierDashboardPage;
