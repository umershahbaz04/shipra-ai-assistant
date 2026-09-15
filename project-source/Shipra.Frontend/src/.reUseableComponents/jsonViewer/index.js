import {
  Dialog,
  DialogContent,
  Grid,
  Stack,
  Typography,
  FormLabel,
  Button,
  Divider,
  Box,
} from "@mui/material";
import React, { useState } from "react";
import ReactJson from "react-json-view";

export default function JsonViewViewer(props) {
  const { maxHeight = 200, jsonData = "", title = "Body" } = props;
  const [showPopup, setShowPopup] = useState(false);

  const openPopup = () => {
    setShowPopup(true);
  };
  const closePopup = () => {
    setShowPopup(false);
  };
  const countProperties = (data) => {
    if (data && typeof data === "object") {
      return Object.keys(data).reduce((count, key) => {
        return count + 1 + countProperties(data[key]);
      }, 0);
    }
    return 0;
  };

  const propertyCount = countProperties(jsonData);
  const showMoreButton = propertyCount > 10;
  return (
    <>
      <Grid>
        <Stack direction={"row"} spacing={1} sx={{ alignItems: "center" }}>
          <Typography variant="h4" gutterBottom>
            {title}
          </Typography>
          <FormLabel>raw (json)</FormLabel>
        </Stack>
        <Divider />
        <Grid
          mt={1}
          sx={{
            maxHeight: `${maxHeight}px`,
            overflowY: "hidden",
            position: "relative",
          }}
        >
          <Grid
            sx={{
              fontSize: "11px",
              padding: "4px",
              fontFamily: "'Lucida Console', 'Courier New', monospace",
            }}
          >
            <ReactJson src={jsonData} theme="monokai" />
            {showMoreButton && (
              <Grid
                sx={{
                  position: "absolute",
                  bottom: "0px",
                  width: "100%",
                  textAlign: "center",
                  background: "#b5aaaa26",
                  padding: "5px 0px",
                }}
              >
                <Button
                  sx={{
                    borderRadius: "30px",
                    height: "27px",
                    textTransform: "capitalize",
                  }}
                  variant="contained"
                  onClick={openPopup}
                >
                  Show More
                </Button>
              </Grid>
            )}
          </Grid>
        </Grid>
        <Dialog fullWidth maxWidth={"md"} open={showPopup} onClose={closePopup}>
          <DialogContent>
            <Box
              sx={{
                fontSize: "11px",
                padding: "4px",
                fontFamily: "'Lucida Console', 'Courier New', monospace",
              }}
            >
              <ReactJson
                src={jsonData}
                name={false}
                displayDataTypes={false}
                displayObjectSize={false}
                {...props}
              />
            </Box>
          </DialogContent>
        </Dialog>
      </Grid>
    </>
  );
}
