import AccountBalanceWalletIcon from "@mui/icons-material/AccountBalanceWallet";
import CardGiftcardIcon from "@mui/icons-material/CardGiftcard";
import ChatBubbleIcon from "@mui/icons-material/ChatBubble";
import CreditCardIcon from "@mui/icons-material/CreditCard";
import DevicesIcon from "@mui/icons-material/Devices";
import EmailIcon from "@mui/icons-material/Email";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import HomeIcon from "@mui/icons-material/Home";
import LabelIcon from "@mui/icons-material/Label";
import LanguageIcon from "@mui/icons-material/Language";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import NotesIcon from "@mui/icons-material/Notes";
import PersonIcon from "@mui/icons-material/Person";
import PhoneIcon from "@mui/icons-material/Phone";
import ReplayIcon from "@mui/icons-material/Replay";
import SecurityIcon from "@mui/icons-material/Security";
import StarIcon from "@mui/icons-material/Star";
import TimerIcon from "@mui/icons-material/Timer";
import AddIcon from "@mui/icons-material/Add";
import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Divider,
  Grid,
  IconButton,
  LinearProgress,
  Stack,
  Switch,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import ReactApexChart from "react-apexcharts";
import { useSelector } from "react-redux";
import { useLocation } from "react-router-dom";
import { GetCDPCustomerInfoByCustomerId } from "../../../api/AxiosInterceptors";
import CreateSegmentsAndLabels from "../Models/CreateSegmentsAndLabels";
import CreateNotesAndTasks from "../Models/CreateNotesAndTasks";

// ─── Palette ────────────────────────────────────────────────────────────────
const PRIMARY = "#1a1a2e";
const ACCENT = "#4f8ef7";
const SUCCESS = "#22c55e";
const SURFACE = "#f8fafc";
const CARD_BG = "#ffffff";
const BORDER = "#e8edf3";
const TEXT_MUTED = "#64748b";
const TEXT_DARK = "#1e293b";

const sectionTitle = (title, sub) => (
  <Box mb={1.5}>
    <Typography
      variant="h6"
      fontWeight={700}
      color={TEXT_DARK}
      fontSize="1rem"
      lineHeight={1.3}
    >
      {title}
    </Typography>
    {sub && (
      <Typography variant="caption" color={TEXT_MUTED}>
        {sub}
      </Typography>
    )}
  </Box>
);

const MiniChip = ({ label, border = true }) => (
  <Chip
    label={label}
    size="small"
    sx={{
      fontSize: "0.68rem",
      height: 22,
      backgroundColor: "#f1f5f9",
      color: TEXT_DARK,
      border: border ? `1px solid ${BORDER}` : "none",
      borderRadius: "999px",
      fontWeight: 500,
    }}
  />
);

const StatMiniCard = ({ label, value }) => (
  <Card
    variant="outlined"
    sx={{ borderRadius: 2, borderColor: BORDER, flex: 1 }}
  >
    <CardContent sx={{ p: "10px !important" }}>
      <Typography variant="caption" color={TEXT_MUTED} fontWeight={600}>
        {label}
      </Typography>
      <Typography
        variant="subtitle1"
        fontWeight={700}
        color={TEXT_DARK}
        lineHeight={1.2}
      >
        {value}
      </Typography>
    </CardContent>
  </Card>
);

// ─── Charts ──────────────────────────────────────────────────────────────────

const revenueLineOptions = {
  chart: {
    type: "line",
    toolbar: { show: false },
    sparkline: { enabled: false },
  },
  stroke: { curve: "smooth", width: 2 },
  colors: [ACCENT, SUCCESS],
  xaxis: {
    categories: [
      "Oct",
      "Nov",
      "Dec",
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
    ],
    labels: { style: { fontSize: "10px", colors: TEXT_MUTED } },
  },
  yaxis: { labels: { style: { fontSize: "10px", colors: TEXT_MUTED } } },
  legend: { position: "top", fontSize: "11px" },
  tooltip: { shared: true },
  grid: { borderColor: BORDER },
};

const revenueLineSeries = [
  {
    name: "Revenue (AED)",
    data: [320, 410, 890, 670, 480, 530, 720, 640, 590, 750, 820, 970],
  },
  { name: "Orders", data: [2, 3, 6, 4, 3, 4, 5, 4, 3, 5, 6, 7] },
];

const barOptions = {
  chart: { type: "bar", toolbar: { show: false } },
  colors: [ACCENT],
  plotOptions: { bar: { borderRadius: 4, columnWidth: "60%" } },
  xaxis: {
    categories: [
      "Oct",
      "Nov",
      "Dec",
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
    ],
    labels: { style: { fontSize: "10px", colors: TEXT_MUTED } },
  },
  yaxis: { labels: { style: { fontSize: "10px", colors: TEXT_MUTED } } },
  grid: { borderColor: BORDER },
  tooltip: {},
};

const barSeries = [
  {
    name: "AOV (AED)",
    data: [160, 137, 148, 168, 160, 133, 144, 160, 197, 150, 137, 139],
  },
];

const pieOptions = {
  chart: { type: "donut" },
  labels: ["Email", "Facebook", "Google", "TikTok", "Direct"],
  colors: [ACCENT, "#f59e0b", SUCCESS, "#ec4899", "#a855f7"],
  legend: { position: "bottom", fontSize: "11px" },
  dataLabels: { enabled: false },
  plotOptions: { pie: { donut: { size: "65%" } } },
};

const pieSeries = [35, 28, 18, 12, 7];

const sparklineOpts = (values) => ({
  chart: { type: "area", sparkline: { enabled: true } },
  stroke: { curve: "smooth", width: 2 },
  fill: { opacity: 0.15 },
  colors: [ACCENT],
  tooltip: { enabled: false },
});

const TIMELINE = [
  {
    icon: <ChatBubbleIcon fontSize="small" />,
    text: "WhatsApp chat: 'Is size L available?'",
    time: "2025-09-04 10:08",
  },
  {
    icon: <LocalShippingIcon fontSize="small" />,
    text: "Order ORD-78570 placed (AED 219) via TikTok Shop.",
    time: "2025-09-03 19:22",
  },
  {
    icon: <CreditCardIcon fontSize="small" />,
    text: "Payment captured (Prepaid).",
    time: "2025-09-03 19:25",
  },
  {
    icon: <LocalShippingIcon fontSize="small" />,
    text: "Order ORD-78502 RTO initiated (address issue).",
    time: "2025-09-01 12:06",
  },
  {
    icon: <StarIcon fontSize="small" />,
    text: "Rated 5★ for previous delivery experience.",
    time: "2025-08-22 16:41",
  },
  {
    icon: <ReplayIcon fontSize="small" />,
    text: "Return requested for size exchange.",
    time: "2025-08-19 14:05",
  },
];

const ORDERS = [
  {
    id: "ORD-78421",
    awb: "ECO0006096481",
    date: "2025-08-19",
    channel: "Shopify",
    source: "Facebook Ads",
    payment: "COD",
    carrier: "Eco Express",
    status: "Delivered",
    value: "289.00",
  },
  {
    id: "ORD-78422",
    awb: "ECO0006096482",
    date: "2025-08-19",
    channel: "Shopify",
    source: "Facebook Ads",
    payment: "COD",
    carrier: "Eco Express",
    status: "Delivered",
    value: "289.00",
  },
  {
    id: "ORD-78423",
    awb: "ECO0006096483",
    date: "2025-08-19",
    channel: "WooCommerce",
    source: "Facebook Ads",
    payment: "Prepaid",
    carrier: "Eco Express",
    status: "RTO Initiated",
    value: "289.00",
  },
  {
    id: "ORD-78424",
    awb: "ECO0006096484",
    date: "2025-08-19",
    channel: "WooCommerce",
    source: "Facebook Ads",
    payment: "Prepaid",
    carrier: "Eco Express",
    status: "RTO Initiated",
    value: "289.00",
  },
];

const statusColor = (status) => {
  if (status === "Delivered") return { bg: "#dcfce7", color: "#15803d" };
  if (status === "RTO Initiated") return { bg: "#fee2e2", color: "#b91c1c" };
  return { bg: "#f1f5f9", color: TEXT_DARK };
};

const paymentColor = (p) =>
  p === "COD"
    ? { bg: "#fee2e2", color: "#b91c1c" }
    : { bg: "#f1f5f9", color: TEXT_DARK };

// ─── Main Component ──────────────────────────────────────────────────────────

const CustomerProfileInfo = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const location = useLocation();
  const { customerData } = location.state || {};
  const [isOn, setIsOn] = useState(false);
  const [customerInfo, setCustomerInfo] = useState({});
  const [openSegmentsAndLabelsModal, setOpenSegmentsAndLabelsModal] =
    useState(false);
  const [openNotesAndTasksModal, setOpenNotesAndTasksModal] = useState(false);

  const handleSwitchChange = (event) => {
    const checked = event.target.checked;
    setIsOn(checked);
  };

  const getCDPCustomerInfoByCustomerId = () => {
    try {
      const response = GetCDPCustomerInfoByCustomerId(
        customerData?.customer?.CustomerId,
      );
      if (response?.data?.isSuccess) {
        const customerInfo = response?.data?.result || {};
        setCustomerInfo(customerInfo);
      }
    } catch (e) {}
  };

  useEffect(() => {
    if (customerData?.customer?.CustomerId) {
      getCDPCustomerInfoByCustomerId();
    }
  }, [customerData]);

  return (
    <Box sx={{ backgroundColor: SURFACE, minHeight: "100vh" }}>
      <Box sx={{ p: "10px" }}>
        <Box
          sx={{
            backgroundColor: "#cfe0fd",
            m: 1,
            ml: "auto",
            width: "fit-content",
            display: "flex",
            alignItems: "center",
            gap: 1.5,
            padding: "2px 8px",
            borderRadius: "8px",
            boxShadow: "0 2px 8px rgba(0, 0, 0, 0.08)",
          }}
        >
          <Typography
            variant="body2"
            sx={{
              fontWeight: 600,
              color: "text.primary",
              whiteSpace: "nowrap",
              letterSpacing: "0.02em",
            }}
          >
            {isOn ? "Mock Data" : "Original"}
          </Typography>

          <Switch
            checked={isOn}
            onChange={handleSwitchChange}
            color="primary"
            sx={{
              "& .MuiSwitch-thumb": {
                boxShadow: "0 2px 4px rgba(0, 0, 0, 0.15)",
              },
            }}
          />
        </Box>
        {/* ── Row 1: Profile + RFM ──────────────────────────────────── */}
        <Grid container spacing={2} mb={2}>
          {/* Profile Card */}
          <Grid item xs={12} lg={8}>
            <Card
              sx={{
                borderRadius: 3,
                boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                border: `1px solid ${BORDER}`,
                height: "100%",
              }}
            >
              <CardContent sx={{ p: 3 }}>
                <Grid container spacing={2} alignItems="center">
                  <Grid item xs={12} xl={8}>
                    <Stack
                      direction={{ xs: "column", md: "row" }}
                      spacing={2}
                      alignItems={{ md: "center" }}
                    >
                      <Avatar
                        sx={{
                          width: 64,
                          height: 64,
                          backgroundColor: ACCENT,
                          fontSize: "1.3rem",
                          fontWeight: 800,
                          flexShrink: 0,
                        }}
                      >
                        DC
                      </Avatar>
                      <Box>
                        <Stack
                          direction={{ xs: "column", sm: "row" }}
                          spacing={1}
                          alignItems={{ sm: "center" }}
                          mb={1}
                        >
                          <Typography
                            variant="h5"
                            fontWeight={800}
                            color={TEXT_DARK}
                          >
                            Dummy Customer
                          </Typography>
                          <Stack direction="row" spacing={0.5} flexWrap="wrap">
                            <MiniChip label="VIP" />
                            <MiniChip label="Upsell Eligible" />
                            <MiniChip label="Low NPS Risk" />
                          </Stack>
                        </Stack>
                        <Stack
                          direction="row"
                          spacing={2}
                          flexWrap="wrap"
                          sx={{
                            "& .contact-item": {
                              display: "flex",
                              alignItems: "center",
                              gap: 0.5,
                              fontSize: "0.8rem",
                              color: TEXT_MUTED,
                            },
                          }}
                        >
                          <Box className="contact-item">
                            <EmailIcon sx={{ fontSize: 14 }} />
                            ahsan@shipra.io
                          </Box>
                          <Box className="contact-item">
                            <PhoneIcon sx={{ fontSize: 14 }} />
                            +971 56 379 8893
                          </Box>
                          <Box className="contact-item">
                            <LocationOnIcon sx={{ fontSize: 14 }} />
                            Dubai, UAE
                          </Box>
                          <Box className="contact-item">
                            <LanguageIcon sx={{ fontSize: 14 }} />
                            shipra.io
                          </Box>
                          <Box className="contact-item">
                            <PersonIcon sx={{ fontSize: 14 }} />
                            CustomerID: C-00192
                          </Box>
                        </Stack>
                      </Box>
                    </Stack>
                  </Grid>

                  <Grid item xs={12} xl={4}>
                    <Stack direction="row" justifyContent="space-around">
                      <Box textAlign="center">
                        <Typography
                          variant="caption"
                          color={TEXT_MUTED}
                          fontWeight={600}
                          display="block"
                        >
                          Lifetime Value
                        </Typography>
                        <Typography
                          variant="h5"
                          fontWeight={800}
                          color={TEXT_DARK}
                        >
                          AED 14,320
                        </Typography>
                      </Box>
                      <Box textAlign="center">
                        <Typography
                          variant="caption"
                          color={TEXT_MUTED}
                          fontWeight={600}
                          display="block"
                        >
                          Total Orders
                        </Typography>
                        <Typography
                          variant="h5"
                          fontWeight={800}
                          color={TEXT_DARK}
                        >
                          27
                        </Typography>
                      </Box>
                      <Box textAlign="center">
                        <Typography
                          variant="caption"
                          color={TEXT_MUTED}
                          fontWeight={600}
                          display="block"
                        >
                          AOV
                        </Typography>
                        <Typography
                          variant="h5"
                          fontWeight={800}
                          color={TEXT_DARK}
                        >
                          AED 530
                        </Typography>
                      </Box>
                    </Stack>
                  </Grid>
                </Grid>
              </CardContent>
            </Card>
          </Grid>

          {/* RFM Card */}
          <Grid item xs={12} lg={4}>
            <Card
              sx={{
                borderRadius: 3,
                boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                border: `1px solid ${BORDER}`,
                height: "100%",
              }}
            >
              <CardContent sx={{ p: 3 }}>
                {sectionTitle("RFM Snapshot")}
                <Stack spacing={2} mt={1.5}>
                  {[
                    { label: "Recency", pct: 92 },
                    { label: "Frequency", pct: 78 },
                    { label: "Monetary", pct: 88 },
                  ].map((r) => (
                    <Box key={r.label}>
                      <Stack
                        direction="row"
                        justifyContent="space-between"
                        mb={0.5}
                      >
                        <Typography
                          variant="body2"
                          fontWeight={600}
                          color={TEXT_DARK}
                        >
                          {r.label}
                        </Typography>
                        <Typography variant="body2" color={TEXT_MUTED}>
                          {r.pct}th pct
                        </Typography>
                      </Stack>
                      <LinearProgress
                        variant="determinate"
                        value={r.pct}
                        sx={{
                          height: 8,
                          borderRadius: 4,
                          backgroundColor: "#e2e8f0",
                          "& .MuiLinearProgress-bar": {
                            backgroundColor: SUCCESS,
                            borderRadius: 4,
                          },
                        }}
                      />
                    </Box>
                  ))}
                </Stack>
                <Box mt={2}>
                  <Typography
                    variant="body2"
                    fontWeight={600}
                    color={TEXT_DARK}
                  >
                    Segment:{" "}
                    <Chip
                      label="Loyalist | High LTV"
                      size="small"
                      sx={{
                        fontSize: "0.65rem",
                        height: 20,
                        backgroundColor: "#f1f5f9",
                        border: `1px solid ${BORDER}`,
                        borderRadius: "999px",
                        ml: 0.5,
                      }}
                    />
                  </Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        {/* ── Row 2: KPI Cards ──────────────────────────────────────── */}
        <Grid container spacing={2} mb={2}>
          {[
            {
              label: "DELIVERY ON-TIME",
              value: "96%",
              sub: "↑ 2.1% this month",
              vals: [130, 140, 135, 145],
            },
            {
              label: "RTO RATE",
              value: "3.9%",
              sub: "↓ 0.8% this month",
              vals: [145, 135, 140, 130],
            },
            {
              label: "RETURN RATE",
              value: "5.2%",
              sub: "≈ stable",
              vals: [132, 134, 133, 135],
            },
            {
              label: "ENGAGEMENT",
              value: "High",
              sub: "+24 events / 7d",
              vals: [100, 120, 130, 145],
            },
          ].map((k) => (
            <Grid item xs={12} sm={6} lg={3} key={k.label}>
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 2 }}>
                  <Typography
                    variant="caption"
                    fontWeight={700}
                    color={TEXT_MUTED}
                    letterSpacing={0.5}
                  >
                    {k.label}
                  </Typography>
                  <Typography
                    variant="h4"
                    fontWeight={800}
                    color={TEXT_DARK}
                    mt={0.5}
                  >
                    {k.value}
                  </Typography>
                  <Stack
                    direction="row"
                    alignItems="flex-end"
                    justifyContent="space-between"
                    mt={1}
                  >
                    <Typography
                      variant="caption"
                      color={TEXT_MUTED}
                      sx={{ maxWidth: "50%" }}
                    >
                      {k.sub}
                    </Typography>
                    <Box sx={{ width: "50%", height: 50 }}>
                      <ReactApexChart
                        options={sparklineOpts(k.vals)}
                        series={[{ data: k.vals }]}
                        type="area"
                        height={50}
                      />
                    </Box>
                  </Stack>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>

        {/* ── Row 3: Charts + Sidebar ───────────────────────────────── */}
        <Grid container spacing={2}>
          <Grid item xs={12} lg={8}>
            <Stack spacing={2}>
              {/* Revenue & Orders */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  {sectionTitle("Revenue & Orders", "Last 12 months")}
                  <Grid container spacing={1} mt={1}>
                    <Grid item xs={12} md={6}>
                      <ReactApexChart
                        options={revenueLineOptions}
                        series={revenueLineSeries}
                        type="line"
                        height={220}
                      />
                    </Grid>
                    <Grid item xs={12} md={6}>
                      <ReactApexChart
                        options={barOptions}
                        series={barSeries}
                        type="bar"
                        height={220}
                      />
                    </Grid>
                  </Grid>
                </CardContent>
              </Card>

              {/* Marketing Attribution */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  {sectionTitle(
                    "Marketing Attribution",
                    "Last-touch vs First-touch",
                  )}
                  <Grid container spacing={2} mt={0.5}>
                    <Grid item xs={12} md={6}>
                      <ReactApexChart
                        options={pieOptions}
                        series={pieSeries}
                        type="donut"
                        height={240}
                      />
                    </Grid>
                    <Grid item xs={12} md={6}>
                      <Box mb={3}>
                        <Typography
                          variant="subtitle2"
                          fontWeight={700}
                          color={TEXT_DARK}
                          mb={1}
                        >
                          Latest UTM (Last click)
                        </Typography>
                        <Stack direction="row" flexWrap="wrap" gap={0.8}>
                          {[
                            "source=facebook",
                            "medium=cpc",
                            "campaign=sept_clearance",
                            "term=oversized+tee",
                            "content=video_ad_02",
                          ].map((t) => (
                            <MiniChip key={t} label={t} />
                          ))}
                        </Stack>
                      </Box>
                      <Box>
                        <Typography
                          variant="subtitle2"
                          fontWeight={700}
                          color={TEXT_DARK}
                          mb={1}
                        >
                          First UTM (First touch)
                        </Typography>
                        <Stack direction="row" flexWrap="wrap" gap={0.8}>
                          {[
                            "source=google",
                            "medium=search",
                            "campaign=summer_sale",
                          ].map((t) => (
                            <MiniChip key={t} label={t} />
                          ))}
                        </Stack>
                      </Box>
                    </Grid>
                  </Grid>
                </CardContent>
              </Card>

              {/* Activity Timeline */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  {sectionTitle("Activity Timeline", "Cross-channel events")}
                  <Stack spacing={0} mt={1}>
                    {TIMELINE.map((item, i) => (
                      <Box
                        key={i}
                        sx={{
                          display: "flex",
                          gap: 2,
                          pb: i < TIMELINE.length - 1 ? 2 : 0,
                          position: "relative",
                        }}
                      >
                        {i < TIMELINE.length - 1 && (
                          <Box
                            sx={{
                              position: "absolute",
                              left: 19,
                              top: 36,
                              bottom: 0,
                              width: 2,
                              backgroundColor: BORDER,
                            }}
                          />
                        )}
                        <Box
                          sx={{
                            width: 40,
                            height: 40,
                            borderRadius: "50%",
                            backgroundColor: "#f1f5f9",
                            border: `2px solid ${BORDER}`,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            color: ACCENT,
                            flexShrink: 0,
                            zIndex: 1,
                          }}
                        >
                          {item.icon}
                        </Box>
                        <Box>
                          <Typography
                            variant="body2"
                            fontWeight={600}
                            color={TEXT_DARK}
                          >
                            {item.text}
                          </Typography>
                          <Typography variant="caption" color={TEXT_MUTED}>
                            {item.time}
                          </Typography>
                        </Box>
                      </Box>
                    ))}
                  </Stack>
                </CardContent>
              </Card>

              {/* Orders Table */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    alignItems="center"
                    mb={2}
                  >
                    {sectionTitle("Orders", "Most recent")}
                    <Button
                      variant="outlined"
                      size="small"
                      startIcon={<FileDownloadIcon />}
                      sx={{
                        borderColor: BORDER,
                        color: TEXT_DARK,
                        textTransform: "none",
                        fontSize: "0.78rem",
                        borderRadius: 2,
                      }}
                    >
                      Export
                    </Button>
                  </Stack>
                  <TableContainer>
                    <Table size="small">
                      <TableHead>
                        <TableRow
                          sx={{
                            "& th": {
                              fontWeight: 700,
                              fontSize: "0.75rem",
                              color: TEXT_MUTED,
                              borderBottom: `1px solid ${BORDER}`,
                              whiteSpace: "nowrap",
                            },
                          }}
                        >
                          {[
                            "Order ID",
                            "AWB",
                            "Date",
                            "Channel",
                            "Source",
                            "Payment",
                            "Carrier",
                            "Status",
                            "Value (AED)",
                          ].map((h) => (
                            <TableCell key={h}>{h}</TableCell>
                          ))}
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {ORDERS.map((o, i) => {
                          const sc = statusColor(o.status);
                          const pc = paymentColor(o.payment);
                          return (
                            <TableRow
                              key={i}
                              sx={{
                                "&:last-child td": { border: 0 },
                                "& td,th": {
                                  fontSize: "0.78rem",
                                  color: TEXT_DARK,
                                  borderBottom: `1px solid ${BORDER}`,
                                  py: 1.2,
                                },
                              }}
                            >
                              <TableCell sx={{ fontWeight: 700 }}>
                                {o.id}
                              </TableCell>
                              <TableCell>{o.awb}</TableCell>
                              <TableCell>{o.date}</TableCell>
                              <TableCell>{o.channel}</TableCell>
                              <TableCell>{o.source}</TableCell>
                              <TableCell>
                                <Chip
                                  label={o.payment}
                                  size="small"
                                  sx={{
                                    fontSize: "0.65rem",
                                    height: 20,
                                    backgroundColor: pc.bg,
                                    color: pc.color,
                                    fontWeight: 700,
                                    borderRadius: "999px",
                                  }}
                                />
                              </TableCell>
                              <TableCell>{o.carrier}</TableCell>
                              <TableCell>
                                <Chip
                                  label={o.status}
                                  size="small"
                                  sx={{
                                    fontSize: "0.65rem",
                                    height: 20,
                                    backgroundColor: sc.bg,
                                    color: sc.color,
                                    fontWeight: 600,
                                    borderRadius: "999px",
                                  }}
                                />
                              </TableCell>
                              <TableCell sx={{ fontWeight: 600 }}>
                                {o.value}
                              </TableCell>
                            </TableRow>
                          );
                        })}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </CardContent>
              </Card>
            </Stack>
          </Grid>

          {/* ── Sidebar ─────────────────────────────────────────────── */}
          <Grid item xs={12} lg={4}>
            <Stack spacing={2}>
              {/* Segments & Labels */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    alignItems="flex-start"
                    mb={0.5}
                  >
                    <Stack>
                      {/* Title + Icon */}
                      <Stack direction="row" alignItems="center" gap={1}>
                        {sectionTitle("Segments & Labels", null)}

                        <Tooltip title="Add Segments & Labels" arrow>
                          <IconButton
                            size="small"
                            onClick={() => {
                              setOpenSegmentsAndLabelsModal(true);
                            }}
                            sx={{
                              border: `1px solid ${BORDER}`,
                              borderRadius: 2,
                              marginBottom: 1,
                              color: TEXT_MUTED,
                              "&:hover": {
                                backgroundColor: "#f5f5f5",
                                color: "primary.main",
                                transform: "scale(1.1)",
                              },
                            }}
                          >
                            <AddIcon sx={{ fontSize: 16 }} />
                          </IconButton>
                        </Tooltip>
                      </Stack>

                      {/* Subtitle */}
                      {sectionTitle(null, "Dynamic audience membership")}
                    </Stack>

                    <LabelIcon sx={{ color: TEXT_MUTED, fontSize: 18 }} />
                  </Stack>
                  <Stack direction="row" flexWrap="wrap" gap={0.8} mt={1}>
                    {[
                      "High LTV",
                      "WhatsApp Engagers",
                      "UAE > Dubai",
                      "Fashion/Clothing",
                      "COD Sensitive",
                    ].map((t) => (
                      <MiniChip key={t} label={t} />
                    ))}
                  </Stack>
                  <Divider sx={{ my: 1.5 }} />
                  <Stack direction="row" flexWrap="wrap" gap={0.8}>
                    {[
                      "WhatsApp Engagers",
                      "Upsell Eligible",
                      "Low NPS Risk",
                      "Prefers Cash",
                      "No-Call",
                    ].map((t) => (
                      <MiniChip key={t} label={t} border={false} />
                    ))}
                  </Stack>
                </CardContent>
              </Card>

              {/* Consent */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    alignItems="center"
                    mb={0.5}
                  >
                    {sectionTitle(
                      "Consent & Preferences",
                      "Privacy & communication",
                    )}
                    <SecurityIcon sx={{ color: TEXT_MUTED, fontSize: 18 }} />
                  </Stack>
                  <Stack spacing={1} mt={1}>
                    {[
                      { label: "Email Marketing", status: "Opted-in" },
                      { label: "WhatsApp", status: "Opted-in" },
                      { label: "SMS", status: "Opted-out" },
                      { label: "Personalization", status: "Allowed" },
                    ].map((c) => (
                      <Stack
                        key={c.label}
                        direction="row"
                        justifyContent="space-between"
                        alignItems="center"
                      >
                        <Typography variant="body2" color={TEXT_DARK}>
                          {c.label}
                        </Typography>
                        <Chip
                          label={c.status}
                          size="small"
                          sx={{
                            fontSize: "0.65rem",
                            height: 20,
                            borderRadius: "999px",
                            backgroundColor:
                              c.status === "Opted-out" ? "#fee2e2" : "#dcfce7",
                            color:
                              c.status === "Opted-out" ? "#b91c1c" : "#15803d",
                            fontWeight: 600,
                          }}
                        />
                      </Stack>
                    ))}
                  </Stack>
                  <Divider sx={{ my: 1.5 }} />
                  <Typography variant="caption" color={TEXT_MUTED}>
                    ℹ️ GDPR / PDPL ready, purpose-based consents tracked
                  </Typography>
                </CardContent>
              </Card>

              {/* Wallet & Loyalty */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    alignItems="center"
                    mb={0.5}
                  >
                    {sectionTitle("Wallet & Loyalty", "Balances & perks")}
                    <AccountBalanceWalletIcon
                      sx={{ color: TEXT_MUTED, fontSize: 18 }}
                    />
                  </Stack>
                  <Grid container spacing={1} mt={0.5}>
                    {[
                      { label: "Wallet Balance", value: "AED 128.50" },
                      { label: "Points", value: "1,920 pts" },
                      { label: "Tier", value: "Gold" },
                      { label: "Coupons", value: "3 Active" },
                    ].map((w) => (
                      <Grid item xs={6} key={w.label}>
                        <StatMiniCard label={w.label} value={w.value} />
                      </Grid>
                    ))}
                  </Grid>
                </CardContent>
              </Card>

              {/* Identities & Devices */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    alignItems="center"
                    mb={0.5}
                  >
                    {sectionTitle("Identities & Devices", "Known identifiers")}
                    <DevicesIcon sx={{ color: TEXT_MUTED, fontSize: 18 }} />
                  </Stack>
                  <Box mt={1}>
                    <Typography
                      variant="caption"
                      fontWeight={700}
                      color={TEXT_MUTED}
                    >
                      EMAILS
                    </Typography>
                    <Typography variant="body2" color={TEXT_DARK}>
                      ahsan@shipra.io
                    </Typography>
                    <Typography variant="body2" color={TEXT_DARK}>
                      billing@shipra.io
                    </Typography>
                  </Box>
                  <Box mt={1.5}>
                    <Typography
                      variant="caption"
                      fontWeight={700}
                      color={TEXT_MUTED}
                    >
                      PHONES
                    </Typography>
                    <Typography variant="body2" color={TEXT_DARK}>
                      +971563798893
                    </Typography>
                  </Box>
                  <Divider sx={{ my: 1.5 }} />
                  <Typography
                    variant="caption"
                    fontWeight={700}
                    color={TEXT_MUTED}
                  >
                    DEVICES
                  </Typography>
                  <Stack spacing={1} mt={1}>
                    {[
                      {
                        name: "iPhone 14 Pro",
                        detail:
                          "iOS 18 • Safari • Last seen 2025-09-04 • IP 2.51.***",
                      },
                      {
                        name: "MacBook Air",
                        detail:
                          "macOS 15 • Chrome • Last seen 2025-08-29 • IP 176.***.12",
                      },
                    ].map((d) => (
                      <Card
                        key={d.name}
                        variant="outlined"
                        sx={{ borderColor: BORDER, borderRadius: 2 }}
                      >
                        <CardContent sx={{ p: "10px !important" }}>
                          <Typography
                            variant="body2"
                            fontWeight={700}
                            color={TEXT_DARK}
                          >
                            {d.name}
                          </Typography>
                          <Typography variant="caption" color={TEXT_MUTED}>
                            {d.detail}
                          </Typography>
                        </CardContent>
                      </Card>
                    ))}
                  </Stack>
                </CardContent>
              </Card>

              {/* Risk & Service */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    alignItems="center"
                    mb={0.5}
                  >
                    {sectionTitle("Risk & Service", "Signals & tickets")}
                    <TimerIcon sx={{ color: TEXT_MUTED, fontSize: 18 }} />
                  </Stack>
                  <Stack spacing={1} mt={1}>
                    {[
                      { label: "Churn Risk", value: "Low" },
                      { label: "Fraud Score", value: "0.12" },
                      { label: "Open Tickets", value: "1" },
                      { label: "Avg. Resolution", value: "6h 32m" },
                    ].map((r) => (
                      <Stack
                        key={r.label}
                        direction="row"
                        justifyContent="space-between"
                        alignItems="center"
                      >
                        <Typography variant="body2" color={TEXT_DARK}>
                          {r.label}
                        </Typography>
                        <Chip
                          label={r.value}
                          size="small"
                          sx={{
                            fontSize: "0.7rem",
                            height: 22,
                            borderRadius: "999px",
                            backgroundColor: "#f1f5f9",
                            color: TEXT_DARK,
                            fontWeight: 600,
                          }}
                        />
                      </Stack>
                    ))}
                  </Stack>
                </CardContent>
              </Card>

              {/* Next Best Actions */}
              <Card
                sx={{
                  borderRadius: 3,
                  boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                  border: `1px solid ${BORDER}`,
                }}
              >
                <CardContent sx={{ p: 3 }}>
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    alignItems="center"
                    mb={0.5}
                  >
                    {sectionTitle("Next Best Actions", "Personalized offers")}
                    <CardGiftcardIcon
                      sx={{ color: TEXT_MUTED, fontSize: 18 }}
                    />
                  </Stack>
                  <Stack spacing={1} mt={1}>
                    {[
                      {
                        title: "Offer free express upgrade",
                        sub: "Predicted uplift: +7.4% conversion",
                      },
                      {
                        title: "Bundle tees + joggers (10% off)",
                        sub: "Predicted AOV increase: +AED 46",
                      },
                      {
                        title: "Re-engage via WhatsApp broadcast",
                        sub: "Best time: Thu 6–8pm",
                      },
                    ].map((a) => (
                      <Card
                        key={a.title}
                        variant="outlined"
                        sx={{ borderColor: BORDER, borderRadius: 2 }}
                      >
                        <CardContent sx={{ p: "10px !important" }}>
                          <Typography
                            variant="body2"
                            fontWeight={700}
                            color={TEXT_DARK}
                          >
                            {a.title}
                          </Typography>
                          <Typography variant="caption" color={TEXT_MUTED}>
                            {a.sub}
                          </Typography>
                        </CardContent>
                      </Card>
                    ))}
                  </Stack>
                </CardContent>
              </Card>
            </Stack>
          </Grid>
        </Grid>

        {/* ── Row 4: Addresses + Notes ──────────────────────────────── */}
        <Grid container spacing={2} mt={0}>
          <Grid item xs={12} md={6}>
            <Card
              sx={{
                borderRadius: 3,
                boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                border: `1px solid ${BORDER}`,
                height: "100%",
              }}
            >
              <CardContent sx={{ p: 3 }}>
                <Stack
                  direction="row"
                  justifyContent="space-between"
                  alignItems="center"
                  mb={0.5}
                >
                  {sectionTitle("Addresses", "Primary & shipping")}
                  <HomeIcon sx={{ color: TEXT_MUTED, fontSize: 18 }} />
                </Stack>
                <Stack spacing={1} mt={1}>
                  {[
                    {
                      label: "Home (Default)",
                      lines: [
                        "Apartment 1204, Marina View Tower",
                        "Dubai Marina, Dubai, United Arab Emirates",
                      ],
                    },
                    {
                      label: "Office",
                      lines: [
                        "Shipra HQ, Al Qayid Al Abqari Information Technology LLC",
                        "Business Bay, Dubai, United Arab Emirates",
                      ],
                    },
                  ].map((a) => (
                    <Card
                      key={a.label}
                      variant="outlined"
                      sx={{ borderColor: BORDER, borderRadius: 2 }}
                    >
                      <CardContent sx={{ p: "10px !important" }}>
                        <Typography
                          variant="body2"
                          fontWeight={700}
                          color={TEXT_DARK}
                        >
                          {a.label}
                        </Typography>
                        {a.lines.map((l) => (
                          <Typography
                            key={l}
                            variant="caption"
                            color={TEXT_MUTED}
                            display="block"
                          >
                            {l}
                          </Typography>
                        ))}
                      </CardContent>
                    </Card>
                  ))}
                </Stack>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={12} md={6}>
            <Card
              sx={{
                borderRadius: 3,
                boxShadow: "0 1px 6px rgba(0,0,0,0.07)",
                border: `1px solid ${BORDER}`,
                height: "100%",
              }}
            >
              <CardContent sx={{ p: 3 }}>
                <Stack
                  direction="row"
                  justifyContent="space-between"
                  alignItems="flex-start"
                  mb={0.5}
                >
                  <Stack>
                    {/* Title + Icon */}
                    <Stack direction="row" alignItems="center" gap={1}>
                      {sectionTitle("Notes & Tasks", null)}

                      <Tooltip title="Add Notes & Tasks" arrow>
                        <IconButton
                          size="small"
                          onClick={() => {
                            setOpenNotesAndTasksModal(true);
                          }}
                          sx={{
                            border: `1px solid ${BORDER}`,
                            borderRadius: 2,
                            marginBottom: 1,
                            color: TEXT_MUTED,
                            "&:hover": {
                              backgroundColor: "#f5f5f5",
                              color: "primary.main",
                              transform: "scale(1.1)",
                            },
                          }}
                        >
                          <AddIcon sx={{ fontSize: 16 }} />
                        </IconButton>
                      </Tooltip>
                    </Stack>

                    {/* Subtitle */}
                    {sectionTitle(null, "Internal only")}
                  </Stack>

                  <NotesIcon sx={{ color: TEXT_MUTED, fontSize: 18 }} />
                </Stack>
                <Stack spacing={1} mt={1}>
                  {[
                    {
                      type: "Note",
                      text: "Prefers WhatsApp over calls; avoids daytime deliveries on Fridays.",
                    },
                    {
                      type: "Task",
                      text: "Follow up on size exchange for ORD-78502 by Sep 6.",
                    },
                    {
                      type: "Task",
                      text: "Add to segment: Dubai—High LTV—WhatsApp",
                    },
                  ].map((n, i) => (
                    <Card
                      key={i}
                      variant="outlined"
                      sx={{ borderColor: BORDER, borderRadius: 2 }}
                    >
                      <CardContent sx={{ p: "10px !important" }}>
                        <Chip
                          label={n.type}
                          size="small"
                          sx={{
                            fontSize: "0.6rem",
                            height: 18,
                            borderRadius: "999px",
                            backgroundColor:
                              n.type === "Note" ? "#dbeafe" : "#f3e8ff",
                            color: n.type === "Note" ? "#1d4ed8" : "#7e22ce",
                            fontWeight: 700,
                            mb: 0.5,
                          }}
                        />
                        <Typography
                          variant="caption"
                          color={TEXT_MUTED}
                          display="block"
                        >
                          {n.text}
                        </Typography>
                      </CardContent>
                    </Card>
                  ))}
                </Stack>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
        {openSegmentsAndLabelsModal && (
          <CreateSegmentsAndLabels
            open={openSegmentsAndLabelsModal}
            onClose={() => setOpenSegmentsAndLabelsModal(false)}
            customerId={customerData?.customer?.CustomerId}
          />
        )}
        {openNotesAndTasksModal && (
          <CreateNotesAndTasks
            open={openNotesAndTasksModal}
            onClose={() => setOpenNotesAndTasksModal(false)}
            customerId={customerData?.customer?.CustomerId}
          />
        )}
      </Box>
    </Box>
  );
};

export default CustomerProfileInfo;
