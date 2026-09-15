import React from "react";
import { Box, Grid, Card, CardContent, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import DashboardIcon from "@mui/icons-material/Dashboard";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import SpeedIcon from "@mui/icons-material/Speed";
import SettingsSuggestIcon from "@mui/icons-material/SettingsSuggest";

const dashboardsList = [
  {
    title: "Sale Dashboard",
    description: "Manage sales, stores, and channels with product metrics",
    path: "/sale-dashboard",
    gradient: "linear-gradient(135deg, #7F00FF 0%, #E100FF 100%)",
    icon: <DashboardIcon sx={{ fontSize: 40, color: "#fff" }} />,
    disabled: false,
  },
  {
    title: "My Carrier",
    description: "Track deliveries, carrier performance, and outscans",
    path: "/orders-dashboard",
    gradient: "linear-gradient(135deg, #11998e 0%, #38ef7d 100%)",
    icon: <LocalShippingIcon sx={{ fontSize: 40, color: "#fff" }} />,
    disabled: false,
  },
  {
    title: "Carrier Dashboard",
    description: "Overview of carrier statistics, status reports, and SLAs",
    path: "/carrier-dashboard",
    gradient: "linear-gradient(135deg, #ff9966 0%, #ff5e62 100%)",
    icon: <SpeedIcon sx={{ fontSize: 40, color: "#fff" }} />,
    disabled: false,
  },
  {
    title: "Custom Dashboard",
    description: "Build your own views and configure custom metrics",
    path: null,
    gradient: "linear-gradient(135deg, #3A6073 0%, #3a7bd5 100%)",
    icon: <SettingsSuggestIcon sx={{ fontSize: 40, color: "#fff" }} />,
    disabled: true,
  },
];

function DashboardsPage() {
  const navigate = useNavigate();

  const handleCardClick = (dashboard) => {
    if (dashboard.disabled) return;
    if (dashboard.path) {
      navigate(dashboard.path);
    }
  };

  return (
    <Box
      sx={{
        p: 4,
        minHeight: "80vh",
        background: "linear-gradient(180deg, #F4F6F9 0%, #EBF0F6 100%)",
        borderRadius: "12px",
        display: "flex",
        flexDirection: "column",
        gap: 3,
      }}
    >
      <Box sx={{ mb: 2 }}>
        <Typography
          variant="h4"
          sx={{
            fontWeight: 700,
            color: "#1E1E1E",
            fontFamily: "'Lato Bold', 'Inter Bold', 'Arial'",
            mb: 1,
          }}
        >
          Dashboards
        </Typography>
        <Typography
          variant="body1"
          sx={{
            color: "#666",
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial'",
          }}
        >
          Choose a dashboard to monitor sales, carrier logistics, and system performance.
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {dashboardsList.map((db, index) => (
          <Grid item xs={12} sm={6} md={6} lg={3} key={index}>
            <Card
              onClick={() => handleCardClick(db)}
              sx={{
                height: "100%",
                background: db.gradient,
                borderRadius: "16px",
                color: "#fff",
                cursor: db.disabled ? "not-allowed" : "pointer",
                boxShadow: "0 10px 20px rgba(0,0,0,0.08)",
                position: "relative",
                overflow: "hidden",
                transition: "all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1)",
                opacity: db.disabled ? 0.75 : 1,
                "&:hover": {
                  transform: db.disabled ? "none" : "translateY(-6px)",
                  boxShadow: db.disabled
                    ? "0 10px 20px rgba(0,0,0,0.08)"
                    : "0 18px 30px rgba(0,0,0,0.18)",
                },
                "&::before": {
                  content: '""',
                  position: "absolute",
                  top: "-50px",
                  right: "-50px",
                  width: "150px",
                  height: "150px",
                  borderRadius: "50%",
                  background: "rgba(255, 255, 255, 0.1)",
                  pointerEvents: "none",
                },
              }}
            >
              <CardContent
                sx={{
                  p: 3,
                  height: "100%",
                  display: "flex",
                  flexDirection: "column",
                  justifyContent: "space-between",
                  gap: 3,
                }}
              >
                <Box
                  sx={{
                    width: 60,
                    height: 60,
                    borderRadius: "12px",
                    backgroundColor: "rgba(255, 255, 255, 0.2)",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    backdropFilter: "blur(4px)",
                  }}
                >
                  {db.icon}
                </Box>

                <Box>
                  <Typography
                    variant="h5"
                    sx={{
                      fontWeight: 700,
                      fontFamily: "'Lato Bold', 'Inter Bold', 'Arial'",
                      mb: 1,
                      fontSize: "1.25rem",
                    }}
                  >
                    {db.title}
                  </Typography>
                  <Typography
                    variant="body2"
                    sx={{
                      color: "rgba(255, 255, 255, 0.85)",
                      fontFamily: "'Lato Regular', 'Inter Regular', 'Arial'",
                      lineHeight: 1.4,
                    }}
                  >
                    {db.description}
                  </Typography>
                </Box>

                {db.disabled && (
                  <Box
                    sx={{
                      position: "absolute",
                      bottom: 12,
                      right: 12,
                      backgroundColor: "rgba(0,0,0,0.4)",
                      px: 1.5,
                      py: 0.5,
                      borderRadius: "10px",
                    }}
                  >
                    <Typography
                      variant="caption"
                      sx={{
                        fontWeight: 600,
                        color: "#fff",
                        textTransform: "uppercase",
                        fontSize: "10px",
                        letterSpacing: "0.5px",
                      }}
                    >
                      Coming Soon
                    </Typography>
                  </Box>
                )}
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}

export default DashboardsPage;
