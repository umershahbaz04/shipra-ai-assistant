import {
  Backdrop,
  Box,
  Button,
  CircularProgress,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Typography,
  Grid,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import ExpandMore from '@mui/icons-material/ExpandMore';
import { Link, Route, Routes, useLocation } from "react-router-dom";
import LoginPage from "../../loginSignup/loginScreen";
import RefreshTokenDocumentation from "./RefreshTokenDocumentation";
import CreateOrderDocumentation from "./CreateOrderDocumentation";
import LoginDocumentation from "./LoginDocumentation";
import TrackingHistoryDocumentation from "./TrackingHistoryDocumentation";
import { ActionButtonCustom } from "../../../utilities/helpers/Helpers";
import AddFormatModal from "../../../components/modals/documentationModals/AddFormatModal";
import {
  DownloadApiPostManCollection,
  DownloadInventoryHistorySummaryExcel,
} from "../../../api/AxiosInterceptors";
import { errorNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
function ApiIntegration(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [load, setLoad] = useState(false);
  const [expanded, setExpanded] = useState("panel1");
  const [open, setOpen] = useState(false);

  const handleChange = (panel) => (event, isExpanded) => {
    setExpanded(isExpanded ? panel : null);
  };

  const location = useLocation();

  const accordian = {
    typography: {
      "& .MuiTypography-root": {
        fontWeight: "bold",
        fontSize: "14px",
      },
    },
  };
  const handleDownload = () => {
    DownloadApiPostManCollection()
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadJson(res.data, "postmanCollection");
          // setIsLoading(false);
        } else {
          errorNotification("Unable to download");
          // setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to download");
      });
  };
  return (
    <Box sx={styleSheet.pageRoot}>
      <Box style={{ padding: "10px" }}>
        <Grid sx={{ display: "flex", justifyContent: "end" }} mb={2}>
          <ActionButtonCustom
            onClick={(event) => {
              handleDownload();
            }}
            label={"Download postman"}
          />
        </Grid>
        <>
          <Accordion
            sx={accordian.typography}
            expanded={expanded === "panel1"}
            onChange={handleChange("panel1")}
          >
            <AccordionSummary expandIcon={<ExpandMore />}>
              <Typography>
                {
                  LanguageReducer?.languageType
                    ?.INTEGRATION_API_INTEGRATION_LOGIN
                }
              </Typography>
            </AccordionSummary>
            <AccordionDetails>
              <Typography>
                <LoginDocumentation />
              </Typography>
            </AccordionDetails>
          </Accordion>
          <Accordion
            sx={accordian.typography}
            expanded={expanded === "panel2"}
            onChange={handleChange("panel2")}
          >
            <AccordionSummary
              expandIcon={<ExpandMore />}
              aria-controls="panel2a-content"
              id="panel2a-header"
            >
              <Typography>
                {
                  LanguageReducer?.languageType
                    ?.INTEGRATION_API_INTEGRATION_REFRESH_TOKEN
                }
              </Typography>
            </AccordionSummary>
            <AccordionDetails>
              <Typography>
                <RefreshTokenDocumentation />
              </Typography>
            </AccordionDetails>
          </Accordion>
          <Accordion
            sx={accordian.typography}
            expanded={expanded === "panel3"}
            onChange={handleChange("panel3")}
          >
            <AccordionSummary
              expandIcon={<ExpandMore />}
              aria-controls="panel3a-content"
              id="panel3a-header3"
            >
              <Typography>
                {
                  LanguageReducer?.languageType
                    ?.INTEGRATION_API_INTEGRATION_CREATE_ORDER
                }
              </Typography>
            </AccordionSummary>
            <AccordionDetails>
              <Typography>
                <CreateOrderDocumentation />
              </Typography>
            </AccordionDetails>
          </Accordion>
          <Accordion
            sx={accordian.typography}
            expanded={expanded === "panel4"}
            onChange={handleChange("panel4")}
          >
            <AccordionSummary
              expandIcon={<ExpandMore />}
              aria-controls="panel4a-content"
              id="panel4a-header4"
            >
              <Typography>
                {
                  LanguageReducer?.languageType
                    ?.INTEGRATION_API_INTEGRATION_ORDER_TRACKING
                }
              </Typography>
            </AccordionSummary>
            <AccordionDetails>
              <Typography>
                <TrackingHistoryDocumentation />
              </Typography>
            </AccordionDetails>
          </Accordion>
        </>
        {/* <Routes>
          <Route
            path="/"
            element={
              <>
              </>
            }
          />
        </Routes> */}
      </Box>
      <Backdrop
        sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
        open={load}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
      {open && <AddFormatModal open={open} setOpen={setOpen} />}
    </Box>
  );
}
export default ApiIntegration;
