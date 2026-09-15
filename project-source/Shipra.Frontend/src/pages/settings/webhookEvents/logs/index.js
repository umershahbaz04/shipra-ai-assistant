import {
  Box,
  Card,
  CardContent,
  Divider,
  Grid,
  List,
  ListItem,
  ListItemText,
  Tooltip,
  Typography,
} from "@mui/material";
import React, { useState } from "react";
import { styleSheet } from "../../../../assets/styles/style";
import {
  CicrlesLoading,
  greyBorder,
  navbarHeight,
  pagePadding,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import UtilityClass from "../../../../utilities/UtilityClass";
const WebhookLogs = (props) => {
  const { logLoading, allWebhookLogs, isFilterOpen } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const [responseData, setResponseData] = useState({});
  const [selectedIndex, setSelectedIndex] = useState();

  let parsedData = null;
  const handleResponse = (data, index) => {
    const parsedRequest = data?.Request ? JSON.parse(data.Request) : {};
    const parsedResponse = data?.Response ? JSON.parse(data.Response) : {};
    parsedData = {
      ...data,
      Request: parsedRequest,
      Response: parsedResponse,
    };
    setResponseData(parsedData);
    setSelectedIndex(index);
  };
  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 152.5
    : windowHeight - 95 - 36;
  return (
    <Box sx={styleSheet.pageRoot}>
      {logLoading ? (
        <CicrlesLoading />
      ) : (
        <Grid
          container
          spacing={2}
          sx={{
            padding: pagePadding,
            border: greyBorder,
            borderRadius: "8px !important",
            borderTopLeftRadius: "0px !important",
            borderTopRightRadius: "0px !important",
            height: "100%",
            width: "100%!important",
            minHeight: calculatedHeight,
            overflowY: "auto",
            margin: "0px !important",
          }}
        >
          {/* Webhook Logs Column */}
          <Grid
            item
            xs={12}
            sm={12}
            md={8}
            sx={{
              paddingLeft: {
                md: "14px!important",
                sm: "8px !important",
                xs: "4px !important",
              },
              bgcolor: "background.paper",
            }}
          >
            <Typography component="h2" variant="h3">
              Webhook Logs
            </Typography>
            <List>
              {allWebhookLogs?.map((req, index) => (
                <Box
                  key={index}
                  onClick={() => handleResponse(req, index)}
                  sx={{
                    backgroundColor:
                      selectedIndex === index
                        ? "rgba(98, 0, 234, 0.1)"
                        : "transparent",
                  }}
                >
                  <ListItem
                    sx={{
                      paddingTop: "3px !important",
                      paddingBottom: "3px !important",
                      paddingLeft: { md: "14px", sm: "8px", xs: "4px" },
                      paddingRight: { md: "14px", sm: "8px", xs: "4px" },
                      borderLeft:
                        selectedIndex === index ? "3px solid #6200EA" : "none",
                    }}
                  >
                    <ListItemText
                      primary={
                        <Box
                          sx={{
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                            flexDirection: {
                              md: "row",
                              sm: "column",
                              xs: "column",
                            },
                          }}
                        >
                          <Box sx={{ display: "flex", alignItems: "center" }}>
                            <Typography
                              variant="body2"
                              sx={{
                                color: req.StatusCode === 200 ? "green" : "red",
                                display: "inline",
                                marginRight: "8px",
                              }}
                            >
                              {req.StatusCode}
                            </Typography>
                            <Typography
                              variant="body2"
                              sx={{ marginRight: "8px" }}
                            >
                              POST
                            </Typography>
                            <Tooltip title={req?.WebhookUrl} arrow>
                              <Typography
                                variant="body2"
                                sx={{
                                  display: "inline-block",
                                  overflow: "hidden",
                                  textOverflow: "ellipsis",
                                  whiteSpace: "nowrap",
                                  width: {
                                    xs: "250px",
                                    sm: "auto",
                                  },
                                }}
                              >
                                {req?.WebhookUrl}
                              </Typography>
                            </Tooltip>
                          </Box>
                          <Typography variant="body2">
                            {UtilityClass.convertUtcToLocalAndGetDate(
                              req.CreatedOn
                            )}
                          </Typography>
                        </Box>
                      }
                    />
                  </ListItem>
                  <Divider />
                </Box>
              ))}
            </List>
          </Grid>

          {/* Request and Response Cards */}
          <Grid
            item
            xs={12}
            sm={12}
            md={4}
            sx={{
              width: "28% !important",
              position: { md: "fixed", xs: "static" },
              top: isFilterOpen ? 205 : 160,
              right: { md: 20, xs: "auto" },
              height: calculatedHeight - 40,
              overflowY: "auto",
            }}
          >
            <Card variant="outlined" sx={{ mb: 2 }}>
              <CardContent>
                <Typography component="h2" variant="subtitle2">
                  <strong>Request Body:</strong>
                </Typography>
                <Divider sx={{ my: 2 }} />
                {Object.keys(responseData).length === 0 ||
                Object.keys(responseData.Request).length === 0 ? (
                  <pre
                    style={{
                      backgroundColor: "#f4f4f4",
                      padding: "10px",
                      borderRadius: "4px",
                    }}
                  >
                    No Data Available
                  </pre>
                ) : (
                  Object.entries(responseData.Request).map(([key, value]) => (
                    <Box key={key}>
                      {typeof value === "object" && value !== null ? (
                        <>
                          <Typography
                            variant="h6"
                            sx={{ fontWeight: "bold", mt: 2 }}
                          >
                            {key}
                          </Typography>
                          {Object.entries(value).map(([subKey, subValue]) => (
                            <Typography key={subKey} variant="body1">
                              <strong>
                                {subKey.replace(/([A-Z])/g, " $1")}:{" "}
                              </strong>
                              {typeof subValue === "object"
                                ? JSON.stringify(subValue)
                                : subValue}
                            </Typography>
                          ))}
                        </>
                      ) : (
                        <Typography variant="body1">
                          <strong>{key.replace(/([A-Z])/g, " $1")}: </strong>
                          {value}
                        </Typography>
                      )}
                    </Box>
                  ))
                )}
                <Divider sx={{ my: 2 }} />
              </CardContent>
            </Card>

            <Card variant="outlined">
              <CardContent>
                <Typography component="h2" variant="subtitle2">
                  <strong>Response Body:</strong>
                </Typography>
                <Divider sx={{ my: 2 }} />
                {Object.keys(responseData).length === 0 ||
                Object.keys(responseData.Response).length === 0 ? (
                  <pre
                    style={{
                      backgroundColor: "#f4f4f4",
                      padding: "10px",
                      borderRadius: "4px",
                    }}
                  >
                    No Data Available
                  </pre>
                ) : (
                  Object.entries(responseData.Response).map(([key, value]) => (
                    <Box key={key}>
                      {typeof value === "object" && value !== null ? (
                        <>
                          <Typography
                            variant="h6"
                            sx={{ fontWeight: "bold", mt: 2 }}
                          >
                            {key}
                          </Typography>
                          {Object.entries(value).map(([subKey, subValue]) => (
                            <Typography key={subKey} variant="body1">
                              <strong>
                                {subKey.replace(/([A-Z])/g, " $1")}:{" "}
                              </strong>
                              {typeof subValue === "object"
                                ? JSON.stringify(subValue)
                                : subValue}
                            </Typography>
                          ))}
                        </>
                      ) : (
                        <Typography variant="body1">
                          <strong>{key.replace(/([A-Z])/g, " $1")}: </strong>
                          {value}
                        </Typography>
                      )}
                    </Box>
                  ))
                )}
                <Divider sx={{ my: 2 }} />
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}
    </Box>
  );
};

export default WebhookLogs;
