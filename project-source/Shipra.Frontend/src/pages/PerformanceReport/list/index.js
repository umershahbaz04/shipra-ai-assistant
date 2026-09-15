import React, { useState, useEffect } from "react";
import {
  Box,
  Grid,
  Typography,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  CircularProgress,
  Badge,
  TextField,
  Card,
  CardContent,
  Stack,
} from "@mui/material";
import { useSelector } from "react-redux";
import ReactApexChart from "react-apexcharts";
import { styleSheet } from "../../../assets/styles/style";
import {
  centerColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  CustomColorLabelledOutline,
} from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent";
import StatusBadge from "../../../components/shared/statudBadge";
import { CodeBox } from "../../../utilities/helpers/Helpers";
import { GetRoleForPerFormanceReport, GetOrderByRole } from "../../../api/AxiosInterceptors";
import { errorNotification } from "../../../utilities/toast";
import { getThisKeyCookie } from "../../../utilities/cookies";
import moment from "moment";

const EXCLUDED_KEYS = [
  "Id",
  "id",
  "Name",
  "name",
  "OrderCount",
  "orderCount",
  "TotalCount",
  "totalCount",
  "RowNum",
  "rowNum",
  "TotalLeads",
  "totalLeads",
  "LeadCount",
  "leadCount",
  "Leads",
  "leads",
  "TotalLeadCount",
  "totalLeadCount"
];

const getPieSeries = (dt) => {
  return Object.entries(dt || {})
    .filter(([key]) => !EXCLUDED_KEYS.includes(key))
    .map(([_, value]) => Number(value) || 0);
};

const getPieOption = (dt) => {
  const entries = Object.entries(dt || {}).filter(
    ([key]) => !EXCLUDED_KEYS.includes(key)
  );

  const labels = entries.map(([key]) => key.replace(/_/g, " "));
  const pieColors = labels.map(cat => {
    const c = cat.toLowerCase();
    if (c.includes('cancel')) return '#FF3B30';
    if (c.includes('to be delivered')) return '#9C27B0';
    if (c.includes('completed') || c.includes('delivered')) return '#00BA77';
    if (c.includes('pending')) return '#FF9800';
    if (c.includes('shipment')) return '#2E93fA';
    if (c.includes('lead')) return '#66DA26';
    if (c.includes('dispatched')) return '#00BCD4';
    return '#546E7A';
  });

  return {
    colors: pieColors,
    chart: {
      type: "pie",
      sparkline: {
        enabled: false,
      },
      margins: {
        right: 0,
        left: 0,
      },
    },
    labels: labels,
    legend: {
      position: "bottom",
      offsetY: 0,
      markers: {
        width: 8,
        height: 8,
      },
    },
    stroke: {
      width: 1,
    },
    tooltip: {
      theme: "light",
    },
    dataLabels: {
      enabled: true,
      formatter: function (val, opts) {
        return opts.w.config.series[opts.seriesIndex];
      },
      dropShadow: {
        enabled: false,
      },
    },
    responsive: [
      {
        breakpoint: 480,
        options: {
          chart: {
            width: "100%",
          },
          legend: {
            position: "bottom",
            offsetY: 5,
          },
        },
      },
    ],
  };
};

const EmployeeChart = React.memo(({ dt }) => {
  const [shouldRender, setShouldRender] = React.useState(false);

  React.useEffect(() => {
    // Delay rendering of heavy ApexCharts to keep the initial page load snappy
    const timer = setTimeout(() => {
      setShouldRender(true);
    }, 150);
    return () => clearTimeout(timer);
  }, []);

  const options = React.useMemo(() => getPieOption(dt), [dt]);
  const series = React.useMemo(() => getPieSeries(dt), [dt]);

  if (!shouldRender) {
    return (
      <Box sx={{ height: 200, display: "flex", justifyContent: "center", alignItems: "center" }}>
        <CircularProgress size={24} />
      </Box>
    );
  }

  return (
    <ReactApexChart
      options={options}
      series={series}
      type="pie"
      height={200}
    />
  );
});

const MiniPerformanceChart = React.memo(({ dt, roleLabel }) => {
  if (!dt) return null;

  let dynamicKeys = new Set();
  const excludeKeys = [
    "Id", "id", "RowNum", "TotalCount", "Name", "name", 
    "OrderCount", "orderCount", "TotalLeads", "totalLeads", 
    "LeadCount", "leadCount", "TotalLeadCount", "totalLeadCount", 
    "LeadsCount", "leadsCount", "Leads", "leads"
  ];

  Object.keys(dt).forEach(k => {
    if (!excludeKeys.includes(k)) {
      dynamicKeys.add(k);
    }
  });

  const statuses = Array.from(dynamicKeys);
  const isSalePerson = roleLabel === "Sale Person";
  
  const realSeries = [];
  const chartSeries = [];
  const categories = [];

  const addData = (val, category) => {
    if (val > 0) {
      realSeries.push(val);
      chartSeries.push(Number(Math.pow(val, 0.5).toFixed(2))); // Square root scale for better visibility
      categories.push(category);
    }
  };

  if (isSalePerson) {
    const leadCount = dt.TotalLeads !== undefined ? dt.TotalLeads
      : dt.totalLeads !== undefined ? dt.totalLeads
      : dt.LeadCount !== undefined ? dt.LeadCount
      : dt.leadCount !== undefined ? dt.leadCount
      : dt.LeadsCount !== undefined ? dt.LeadsCount
      : dt.leadsCount !== undefined ? dt.leadsCount
      : dt.Leads !== undefined ? dt.Leads
      : dt.leads !== undefined ? dt.leads
      : 0;
    addData(leadCount, 'Total Leads');
  }

  const orderCount = dt.OrderCount !== undefined ? dt.OrderCount : dt.orderCount !== undefined ? dt.orderCount : 0;
  addData(orderCount, 'Total Shipment');

  statuses.forEach(status => {
    addData(dt[status] || 0, status.replace(/_/g, ' '));
  });

    const chartColors = categories.map(cat => {
      const c = cat.toLowerCase();
      if (c.includes('cancel')) return '#FF3B30';
      if (c.includes('to be delivered')) return '#9C27B0';
      if (c.includes('completed') || c.includes('delivered')) return '#00BA77';
      if (c.includes('pending')) return '#FF9800';
      if (c.includes('shipment')) return '#2E93fA';
      if (c.includes('lead')) return '#66DA26';
      if (c.includes('dispatched')) return '#00BCD4';
      return '#546E7A';
    });

  const options = {
    chart: {
      type: 'bar',
      height: 120,
      sparkline: {
        enabled: true
      },
      animations: {
        enabled: false
      }
    },
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: '60%',
        borderRadius: 2,
        distributed: true,
        dataLabels: {
          position: 'center'
        }
      },
    },
    colors: chartColors,
    dataLabels: {
      enabled: true,
      style: {
        fontSize: '11px',
        fontWeight: 'bold',
        colors: ['#fff']
      },
      offsetY: 0,
      formatter: function (val, { dataPointIndex }) {
        return realSeries[dataPointIndex];
      }
    },
    tooltip: {
      theme: 'dark',
      x: { show: false },
      y: {
        title: {
          formatter: function (seriesName, { dataPointIndex }) {
            return categories[dataPointIndex] + ":";
          }
        },
        formatter: function (val, { dataPointIndex }) {
          return realSeries[dataPointIndex];
        }
      }
    },
    xaxis: {
      categories: categories,
      labels: { show: false },
      axisBorder: { show: false },
      axisTicks: { show: false }
    }
  };

  return (
    <Box sx={{ mt: 1, width: "100%" }}>
      <Box sx={{ height: 120 }}>
        <ReactApexChart options={options} series={[{ data: chartSeries }]} type="bar" height={120} />
      </Box>
      <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1, mt: 1 }}>
        {categories.map((cat, idx) => (
          <Box key={idx} sx={{ display: "flex", alignItems: "center", fontSize: "10px", fontWeight: 500, color: "#555" }}>
            <Box sx={{ width: 8, height: 8, borderRadius: "50%", backgroundColor: chartColors[idx], mr: 0.5 }} />
            {cat}: {realSeries[idx]}
          </Box>
        ))}
      </Box>
    </Box>
  );
});

const CompanyVIPChart = React.memo(({ data, roleLabel, companyName }) => {
  if (!data) return null;

  const isSalePerson = roleLabel === "Sale Person";

  const entries = Object.entries(data).filter(
    ([key]) =>
      ![
        "totalOrders",
        "totalLeads",
        "totalEmployees",
        "Id",
        "id",
        "Name",
        "name",
        "OrderCount",
        "orderCount",
        "TotalCount",
        "totalCount",
        "RowNum",
        "rowNum",
        "TotalLeads",
        "totalLeads",
        "LeadCount",
        "leadCount",
        "TotalLeadCount",
        "totalLeadCount",
        "LeadsCount",
        "leadsCount",
        "Leads",
        "leads",
      ].includes(key),
  );

  const labels = entries.map(([key]) => key.replace(/_/g, " "));
  const series = entries.map(([_, val]) => Number(val) || 0);

  const chartColors = labels.map((cat) => {
    const c = cat.toLowerCase();
    if (c.includes("cancel")) return "#FF3B30";
    if (c.includes("to be delivered")) return "#9C27B0";
    if (c.includes("completed") || c.includes("delivered")) return "#00BA77";
    if (c.includes("pending")) return "#FF9800";
    if (c.includes("shipment")) return "#2E93fA";
    if (c.includes("lead")) return "#66DA26";
    if (c.includes("dispatched")) return "#00BCD4";
    return "#546E7A";
  });

  const options = {
    chart: {
      type: "donut",
      sparkline: { enabled: false },
    },
    colors: chartColors.length > 0 ? chartColors : ["var(--primary-color)", "#00BA77", "#FF9800", "#FF3B30"],
    labels: labels.length > 0 ? labels : ["Total Shipments"],
    legend: {
      show: true,
      position: "bottom",
      fontSize: "12px",
      fontWeight: 600,
    },
    dataLabels: {
      enabled: false, // Disable slice numbers to prevent overlapping with the center labels
    },
    plotOptions: {
      pie: {
        donut: {
          size: "78%", // Sleeker, modern thin donut style
          labels: {
            show: true,
            name: {
              show: true,
              fontSize: "11px",
              fontWeight: 600,
              color: "#777",
              offsetY: -8,
            },
            value: {
              show: true,
              fontSize: "24px",
              fontWeight: 800,
              color: "#1a1a24",
              offsetY: 6,
              formatter: () => data.totalOrders,
            },
            total: {
              show: true,
              label: "Total Shipments",
              color: "#777",
              fontSize: "11px",
              fontWeight: 600,
              formatter: () => data.totalOrders,
            },
          },
        },
      },
    },
    stroke: { width: 2, colors: ["#fff"] },
    tooltip: { theme: "dark" },
  };

  const donutSeries = series.length > 0 ? series : [data.totalOrders];

  return (
    <Card
      sx={{
        mb: 2.5,
        background: "linear-gradient(135deg, #ffffff 0%, #f8f9fe 100%)",
        border: "1.5px solid var(--primary-color)",
        boxShadow: "0 4px 18px rgba(86, 58, 213, 0.12)",
        borderRadius: 2.5,
        p: 2,
      }}
    >
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        flexWrap="wrap"
        gap={1}
        mb={2}
        pb={1.5}
        borderBottom="1px solid #e0e0e0"
      >
        <Box display="flex" alignItems="center" gap={1}>
          <Box
            sx={{
              width: 10,
              height: 24,
              borderRadius: 1,
              backgroundColor: "var(--primary-color)",
            }}
          />
          <Typography
            variant="h6"
            sx={{ fontWeight: 800, color: "#1a1a24", fontSize: "1.1rem" }}
          >
            🏢 {companyName || "Company"} Overall Performance Overview
          </Typography>
        </Box>
        <Stack direction="row" spacing={1.5} flexWrap="wrap" gap={1}>
          <Box
            sx={{
              bgcolor: "rgba(86, 58, 213, 0.1)",
              color: "var(--primary-color)",
              px: 2,
              py: 0.75,
              borderRadius: 2,
              fontWeight: 700,
              fontSize: "0.85rem",
            }}
          >
            📦 Total Shipments: {data.totalOrders}
          </Box>
          {isSalePerson && (
            <Box
              sx={{
                bgcolor: "rgba(0, 186, 119, 0.1)",
                color: "#00BA77",
                px: 2,
                py: 0.75,
                borderRadius: 2,
                fontWeight: 700,
                fontSize: "0.85rem",
              }}
            >
              🎯 Total Leads: {data.totalLeads}
            </Box>
          )}
          <Box
            sx={{
              bgcolor: "rgba(255, 152, 0, 0.1)",
              color: "#FF9800",
              px: 2,
              py: 0.75,
              borderRadius: 2,
              fontWeight: 700,
              fontSize: "0.85rem",
            }}
          >
            👥 Active {roleLabel}s: {data.totalEmployees}
          </Box>
        </Stack>
      </Box>

      <Grid container spacing={2} alignItems="center">
        <Grid item xs={12} md={5}>
          <Box sx={{ height: 230, display: "flex", justifyContent: "center" }}>
            <ReactApexChart
              options={options}
              series={donutSeries}
              type="donut"
              height={230}
              width="100%"
            />
          </Box>
        </Grid>
        <Grid item xs={12} md={7}>
          <Grid container spacing={1.5}>
            {labels.map((lbl, idx) => (
              <Grid item xs={6} sm={4} key={idx}>
                <Box
                  sx={{
                    p: 1.5,
                    borderRadius: 2,
                    bgcolor: "#fff",
                    border: "1px solid #eef0f6",
                    boxShadow: "0 2px 6px rgba(0,0,0,0.03)",
                    display: "flex",
                    alignItems: "center",
                    gap: 1,
                  }}
                >
                  <Box
                    sx={{
                      width: 12,
                      height: 12,
                      borderRadius: "50%",
                      backgroundColor: chartColors[idx] || "#546E7A",
                    }}
                  />
                  <Box>
                    <Typography
                      variant="caption"
                      sx={{ color: "#666", display: "block", fontSize: "0.75rem" }}
                    >
                      {lbl}
                    </Typography>
                    <Typography
                      variant="subtitle2"
                      sx={{ fontWeight: 700, color: "#1a1a24" }}
                    >
                      {series[idx]}
                    </Typography>
                  </Box>
                </Box>
              </Grid>
            ))}
          </Grid>
        </Grid>
      </Grid>
    </Card>
  );
});

const PerformanceReportList = (props) => {
  const { isFilterOpen, activeFilters } = props;
  const [loadingRoles, setLoadingRoles] = useState(false);
  const [loadingOrders, setLoadingOrders] = useState(false);
  const [rolesList, setRolesList] = useState([]);
  const [selectedRoleId, setSelectedRoleId] = useState(null);
  const [orders, setOrders] = useState([]);
  const [search, setSearch] = useState("");

  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const companyAggregatedData = React.useMemo(() => {
    if (!rolesList || rolesList.length === 0) return null;

    const totalOrders = rolesList.reduce(
      (sum, item) =>
        sum +
        (Number(item.OrderCount !== undefined ? item.OrderCount : item.orderCount) ||
          0),
      0,
    );
    const totalLeads = rolesList.reduce(
      (sum, item) =>
        sum +
        (Number(
          item.TotalLeads !== undefined
            ? item.TotalLeads
            : item.totalLeads !== undefined
            ? item.totalLeads
            : item.LeadCount !== undefined
            ? item.LeadCount
            : item.leadCount !== undefined
            ? item.leadCount
            : item.LeadsCount !== undefined
            ? item.LeadsCount
            : item.leadsCount !== undefined
            ? item.leadsCount
            : item.Leads !== undefined
            ? item.Leads
            : item.leads !== undefined
            ? item.leads
            : 0,
        ) || 0),
      0,
    );

    const statusTotals = {};
    const excludeKeys = [
      "Id",
      "id",
      "RowNum",
      "rowNum",
      "TotalCount",
      "totalCount",
      "Name",
      "name",
      "OrderCount",
      "orderCount",
      "TotalLeads",
      "totalLeads",
      "LeadCount",
      "leadCount",
      "TotalLeadCount",
      "totalLeadCount",
      "LeadsCount",
      "leadsCount",
      "Leads",
      "leads",
    ];

    rolesList.forEach((dt) => {
      Object.keys(dt || {}).forEach((key) => {
        if (!excludeKeys.includes(key)) {
          const val = Number(dt[key]) || 0;
          statusTotals[key] = (statusTotals[key] || 0) + val;
        }
      });
    });

    return {
      totalOrders,
      totalLeads,
      totalEmployees: rolesList.length,
      ...statusTotals,
    };
  }, [rolesList]);

  useEffect(() => {
    const fetchRolesAndFirstOrders = async () => {
      if (!activeFilters?.role) {
        setRolesList([]);
        setSelectedRoleId(null);
        setOrders([]);
        return;
      }

      setLoadingRoles(true);
      setLoadingOrders(true);
      try {
        const roleVal =
          activeFilters.role === 1 || activeFilters.role === "1" || activeFilters.role === "Driver"
            ? 1
            : activeFilters.role === 2 || activeFilters.role === "2" || activeFilters.role === "Sale Person"
            ? 2
            : 0;
        const start = activeFilters.startDateFormated || null;
        const end = activeFilters.endDateFormated || null;

        const countryVal = activeFilters?.country || null;

        const res = await GetRoleForPerFormanceReport(roleVal, start, end, countryVal);
        const list = res?.data?.result?.List || res?.data?.result?.list || [];
        setRolesList(list);

        if (list.length > 0) {
          const firstItem = list[0];
          const firstId = firstItem.Id || firstItem.id;
          setSelectedRoleId(firstId);

          const ordersRes = await GetOrderByRole(firstId, roleVal, start, end, countryVal);
          setOrders(ordersRes?.data?.result?.List || ordersRes?.data?.result?.list || []);
        } else {
          setSelectedRoleId(null);
          setOrders([]);
        }
      } catch (err) {
        console.error("Error fetching performance roles:", err);
        errorNotification("Failed to fetch roles.");
      } finally {
        setLoadingRoles(false);
        setLoadingOrders(false);
      }
    };

    fetchRolesAndFirstOrders();
  }, [activeFilters]);

  const handleRoleClick = async (id) => {
    setSelectedRoleId(id);
    setLoadingOrders(true);
    try {
      const roleVal =
        activeFilters?.role === 1 || activeFilters?.role === "1" || activeFilters?.role === "Driver"
          ? 1
          : activeFilters?.role === 2 || activeFilters?.role === "2" || activeFilters?.role === "Sale Person"
          ? 2
          : 0;
      const start = activeFilters?.startDateFormated || null;
      const end = activeFilters?.endDateFormated || null;
      const countryVal = activeFilters?.country || null;
      const res = await GetOrderByRole(id, roleVal, start, end, countryVal);
      setOrders(res?.data?.result?.List || res?.data?.result?.list || []);
    } catch (err) {
      console.error("Error fetching orders by role:", err);
      errorNotification("Failed to fetch orders.");
    } finally {
      setLoadingOrders(false);
    }
  };

  const roleLabel = activeFilters?.role === 1 || activeFilters?.role === "1" || activeFilters?.role === "Driver"
    ? "Driver"
    : activeFilters?.role === 2 || activeFilters?.role === "2" || activeFilters?.role === "Sale Person"
    ? "Sale Person"
    : "Role";

  const isSalePerson = roleLabel === "Sale Person";

  const columns = [
    {
      field: "TrackingNo",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Tracking No"}</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <CodeBox title={row?.CarrierTrackingNo || row?.carrierTrackingNo || row?.TrackingNo || row?.OrderNo || row?.orderNo || ""} />
          </Box>
        );
      },
    },
    {
      field: "DriverName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{roleLabel}</Box>,
      minWidth: 130,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{row?.EmployeeName || row?.employeeName || row?.DriverName || row?.driverName || "Unassigned"}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "OrderDate",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Order Date"}</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        const date = row?.OrderDate || row?.orderDate || "";
        return (
          <Box>
            {date ? moment.utc(date).local().format("DD/MM/YYYY") : "N/A"}
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Created On"}</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        const date = row?.CreatedOn || row?.createdOn || "";
        return (
          <Box>
            {date ? moment.utc(date).local().format("DD/MM/YYYY") : "N/A"}
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Mobile",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Mobile"}</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{row?.Mobile1 || row?.mobile1 || "-"}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "Amount",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Amount"}</Box>,
      minWidth: 100,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{row?.Amount || row?.amount || "0.00"}</Box>;
      },
    },
    {
      field: "Address",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Address"}</Box>,
      minWidth: 200,
      flex: 2,
      renderCell: ({ row }) => {
        return <Box sx={{ whiteSpace: "nowrap", overflow: "hidden", textOverflow: "ellipsis" }} title={row?.CustomerFullAddress || row?.customerFullAddress || "-"}>{row?.CustomerFullAddress || row?.customerFullAddress || "-"}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "DeliveryTaskStatus",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Status"}</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        const statusText = row?.CarrierTrackingStatus || row?.carrierTrackingStatus || row?.DeliveryTaskStatus || row?.deliveryTaskStatus || "Pending";
        
        let bgClr = "#00BA77"; // Success
        if (statusText?.toLowerCase().includes("pending") || statusText?.toLowerCase().includes("cancelled")) {
          bgClr = "#FF3B30"; // Red
        } else if (statusText?.toLowerCase().includes("out for delivery")) {
          bgClr = "var(--primary-color)"; // Purple
        } else if (statusText?.toLowerCase().includes("mobile not answered")) {
          bgClr = "var(--primary-color)"; // Purple
        }

        return (
          <StatusBadge
            title={statusText}
            bgColor={bgClr}
            color="#fff"
          />
        );
      },
    },
  ];

  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;

  return (
    <>
      {rolesList.length > 0 && (
        <CustomColorLabelledOutline
          isCollapse={true}
          label={`${roleLabel} & Company Performance Overview`}
          defaultCollapseState={false}
        >
          <CompanyVIPChart
            data={companyAggregatedData}
            roleLabel={roleLabel}
            companyName={
              getThisKeyCookie("user_name") || getThisKeyCookie("clIdentifier")
            }
          />
          <Grid
            container
            spacing={2}
            mb={2}
          >
            {rolesList.map((dt) => {
              const itemName = dt.Name || dt.name || "Unknown";
              const itemOrderCount = dt.OrderCount !== undefined ? dt.OrderCount : dt.orderCount;
              const itemLeadCount = dt.LeadCount !== undefined ? dt.LeadCount
                : dt.leadCount !== undefined ? dt.leadCount
                : dt.TotalLeads !== undefined ? dt.TotalLeads
                : dt.totalLeads !== undefined ? dt.totalLeads
                : dt.TotalLeadCount !== undefined ? dt.TotalLeadCount
                : dt.totalLeadCount !== undefined ? dt.totalLeadCount
                : dt.LeadsCount !== undefined ? dt.LeadsCount
                : dt.leadsCount !== undefined ? dt.leadsCount
                : dt.Leads !== undefined ? dt.Leads
                : dt.leads !== undefined ? dt.leads
                : 0;

              return (
                <Grid item xs={12} sm={6} md={3} key={dt.Id || dt.id}>
                  <Card
                    sx={{
                      height: "100%",
                      display: "flex",
                      flexDirection: "column",
                      bgcolor: "#f5f5f5",
                      border: "1px solid #e8e8e8",
                      boxShadow: "none",
                      transition: "all 0.2s ease",
                      "&:hover": {
                        boxShadow: "0 2px 8px rgba(0, 0, 0, 0.08)",
                        borderColor: "#ddd",
                      },
                      borderRadius: 1.5,
                    }}
                  >
                    <CardContent sx={{ p: 1.5, flexGrow: 1, pb: "12px !important" }}>
                      <Box display="flex" alignItems="center" mb={1}>
                        <Box minWidth={0}>
                          <Typography
                            variant="subtitle2"
                            sx={{
                              fontWeight: 600,
                              color: "#212121",
                              fontSize: "0.95rem",
                              wordBreak: "break-word",
                            }}
                          >
                            {itemName}
                          </Typography>
                        </Box>
                      </Box>
                      <Stack spacing={1.2}>
                        <EmployeeChart dt={dt} />
                      </Stack>
                      <Box sx={{ display: "flex", flexDirection: "column" }}>
                        <Typography variant="subtitle2" alignSelf={"end"}>
                          Total Shipment : {itemOrderCount}
                        </Typography>
                        {isSalePerson && (
                          <Typography variant="subtitle2" alignSelf={"end"}>
                            Total Leads : {itemLeadCount}
                          </Typography>
                        )}
                      </Box>
                    </CardContent>
                  </Card>
                </Grid>
              );
            })}
          </Grid>
        </CustomColorLabelledOutline>
      )}

      <Grid container spacing={1} mt={rolesList.length > 0 ? 1 : 0}>
        <Grid item md={3} sm={12} xs={12}>
          <Box
            sx={{
              border: "1px solid #ced4da",
              borderRight: "0px !important",
              backgroundColor: "rgb(247, 248, 248) !important",
              p: 1.5,
              height: calculatedHeight,
              display: "flex",
              flexDirection: "column",
              overflow: "hidden",
            }}
          >
            <Typography
              variant="h6"
              fontWeight="600"
              mb={1}
              sx={{ color: "#333", display: "flex", justifyContent: "space-between", fontSize: "14px", px: 1, pb: 1, borderBottom: "1px solid #ccc" }}
            >
              <Box>{roleLabel} Name</Box>
            </Typography>
            <Box px={1} mb={1}>
              <TextField
                fullWidth
                size="small"
                placeholder={`Search ${roleLabel}`}
                value={search}
                onChange={(e) => setSearch(e.target.value)}
              />
            </Box>

            <Box sx={{ overflowY: "auto", flexGrow: 1, p: 0 }}>
              {loadingRoles ? (
                <Box display="flex" justifyContent="center" p={2}>
                  <CircularProgress size={24} />
                </Box>
              ) : rolesList.length > 0 ? (
                rolesList.filter(item => {
                  const itemName = item.Name || item.name || "";
                  return itemName.toLowerCase().includes(search.toLowerCase());
                }).map((item) => {
                  const itemId = item.Id || item.id;
                  const itemName = item.Name || item.name;
                  const itemOrderCount = item.OrderCount !== undefined ? item.OrderCount : item.orderCount;
                  const itemLeadCount = item.LeadCount !== undefined ? item.LeadCount
                    : item.leadCount !== undefined ? item.leadCount
                    : item.TotalLeads !== undefined ? item.TotalLeads
                    : item.totalLeads !== undefined ? item.totalLeads
                    : item.TotalLeadCount !== undefined ? item.TotalLeadCount
                    : item.totalLeadCount !== undefined ? item.totalLeadCount
                    : item.LeadsCount !== undefined ? item.LeadsCount
                    : item.leadsCount !== undefined ? item.leadsCount
                    : item.Leads !== undefined ? item.Leads
                    : item.leads !== undefined ? item.leads
                    : 0;

                  return (
                    <Box
                      key={itemId}
                      onClick={() => handleRoleClick(itemId)}
                      sx={{
                        display: "flex",
                        flexDirection: "column",
                        px: 1.5,
                        py: 1,
                        cursor: "pointer",
                        borderLeft: selectedRoleId === itemId
                          ? "5px solid var(--primary-color)"
                          : "5px solid transparent",
                        backgroundColor: selectedRoleId === itemId
                          ? "rgba(86, 58, 213, 0.08)"
                          : "transparent",
                        color: selectedRoleId === itemId ? "var(--primary-color)" : "#444",
                        transition: "0.2s",
                        "&:hover": {
                          backgroundColor: "#f5f7ff",
                        },
                      }}
                    >
                      <Box sx={{ fontWeight: selectedRoleId === itemId ? 700 : 500, fontSize: "13px" }}>
                        <span style={{ fontWeight: "bold" }}>{roleLabel} Name:</span> {itemName || "Unknown"}
                      </Box>
                      <Box sx={{ fontWeight: 500, fontSize: "12px", mt: 0.5 }}>
                        Total Shipment: {itemOrderCount}
                      </Box>
                      {isSalePerson && (
                        <Box sx={{ fontWeight: 500, fontSize: "12px", mt: 0.5 }}>
                          Total Leads: {itemLeadCount}
                        </Box>
                      )}
                      <MiniPerformanceChart dt={item} roleLabel={roleLabel} />
                    </Box>
                  );
                })
              ) : (
                <Typography variant="body2" color="textSecondary" align="center" mt={2}>
                  No data available. Please select a role and click filter.
                </Typography>
              )}
            </Box>
          </Box>
        </Grid>

        <Grid item md={9} sm={12} xs={12} px={"0px !important"}>
          <Box
            sx={{
              ...styleSheet.allOrderTable,
              height: calculatedHeight,
              "& .MuiDataGrid-root": {
                borderRadius: "0px 0px 8px 0px !important",
              },
            }}
          >
            <DataGridComponent
              loading={loadingOrders}
              rowHeight={40}
              headerHeight={40}
              sx={{
                fontFamily:
                  "'Lato Regular', 'Inter Regular', 'Arial' !important",
                fontSize: "12px",
                fontWeight: "500",
              }}
              getRowId={(row) => row.OrderId || row.orderId || row.id || Math.random()}
              rows={orders}
              columns={columns}
              disableSelectionOnClick
              pagination
              rowPerPage={25}
              height={calculatedHeight}
            />
          </Box>
        </Grid>
      </Grid>
    </>
  );
};

export default PerformanceReportList;
