import React from "react";
import { Box, Card, CardContent, Typography, CircularProgress } from "@mui/material";
import ReactApexChart from "react-apexcharts";

const getPieOption = (labels) => {
  return {
    colors: [
      "#1f77b4",
      "#ff7f0e",
      "#2ca02c",
      "#e2ce1c",
      "#9467bd",
      "#8c564b",
      "#e377c2",
    ],
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

const LeadChart = React.memo(({ leadTabData, isLoading, title }) => {
  const [shouldRender, setShouldRender] = React.useState(false);

  React.useEffect(() => {
    // Delay rendering of heavy ApexCharts to keep the initial page load snappy
    const timer = setTimeout(() => {
      setShouldRender(true);
    }, 150);
    return () => clearTimeout(timer);
  }, []);

  // Filter out the 'All' / default tab which has statusIds == "" or represents the total
  const chartData = (leadTabData || []).filter(tab => tab.statusIds !== "");
  const series = chartData.map(tab => tab.count || 0);
  const labels = chartData.map(tab => tab.label || "Unknown");
  const options = React.useMemo(() => getPieOption(labels), [labels]);

  if (isLoading || !shouldRender) {
    return (
      <Card sx={{ mb: 2, height: "100%" }}>
        <CardContent>
          <Typography variant="h6" fontWeight="bold" mb={2}>
            {title || "Leads Summary"}
          </Typography>
          <Box sx={{ height: 200, display: "flex", justifyContent: "center", alignItems: "center" }}>
            <CircularProgress size={24} />
          </Box>
        </CardContent>
      </Card>
    );
  }

  // If there's no data to show, don't render an empty chart
  if (series.length === 0 || series.every(count => count === 0)) {
    return null;
  }

  // Get total sum for the tooltip or secondary title
  const totalLeads = series.reduce((a, b) => a + b, 0);

  return (
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
              {title || "Leads Summary"}
            </Typography>
          </Box>
        </Box>
        <Box sx={{ display: "flex", justifyContent: "center" }}>
          <Box sx={{ width: "100%" }}>
            <ReactApexChart
              options={options}
              series={series}
              type="pie"
              height={200}
            />
          </Box>
        </Box>
        <Box sx={{ display: "flex", flexDirection: "column", mt: 1 }}>
          <Typography variant="subtitle2" alignSelf={"end"}>
            Total Leads: {totalLeads}
          </Typography>
        </Box>
      </CardContent>
    </Card>
  );
});

export default LeadChart;
