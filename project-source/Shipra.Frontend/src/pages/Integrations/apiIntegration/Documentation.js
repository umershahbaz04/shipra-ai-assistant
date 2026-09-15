import {
  Box,
  Typography,
  Grid,
  Stack,
  FormLabel,
  Divider,
  Dialog,
  DialogContent,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Chip,
  InputAdornment,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import JsonViewViewer from "../../../.reUseableComponents/jsonViewer";
import TextFieldWithCopyButton from "../../../.reUseableComponents/TextField/TextFieldWithCopyButton";
import { EnumRequestType } from "../../../utilities/enum";

function Documentation(props) {
  const {
    jsonData = {},
    responseData = {},
    requestType = EnumRequestType.Get,
    name = "",
    endPoint = "",
    auth = false,
    format = [],
  } = props;
  const [queryParamters, setQueryParamters] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  // Function to parse the query string into an array
  const getQueryStringParams = (queryString) => {
    const params = new URLSearchParams(queryString);
    const paramArray = [];

    for (const [key, value] of params) {
      paramArray.push({ key, value });
    }

    return paramArray;
  };

  useEffect(() => {
    if (endPoint) {
      var url = new URL(endPoint);
      var queryString = url.search;
      const queryParams = getQueryStringParams(queryString);
      setQueryParamters(queryParams);
    }
  }, []);

  const reqName = EnumRequestType.properties[requestType]?.name;

  const tableStyle = {
    td: {
      padding: "6px 0px",
      fontWeight: "bold",
      maxWidth: "0px",
    },
  };
  const tableObjecDescStyle = {
    td: {
      padding: "6px 0px",
      maxWidth: "0px",
    },
  };
  const getData = () =>{
    
  }
  return (
    <Grid>
      <Stack direction={"column"} spacing={3}>
        <Grid>
          {/* <Typography variant="h4" gutterBottom>
            Request
          </Typography> */}
          {/* <Typography variant="h5" gutterBottom>
            <Stack direction={"row"} spacing={1}>
              <Box color={"red"}>{reqName}</Box>
              <Box>{name}</Box>
            </Stack>
          </Typography> */}
          <Box>
            <TextFieldWithCopyButton
              value={endPoint}
              readOnly
              startAdornment={
                <InputAdornment
                  position="start"
                  sx={{
                    padding: "15.5px 14px",
                    backgroundColor: (theme) =>
                      EnumRequestType.Post == requestType
                        ? "#49cc90"
                        : "#61affe",
                    borderTopLeftRadius: (theme) =>
                      theme.shape.borderRadius + "px",
                    borderBottomLeftRadius: (theme) =>
                      theme.shape.borderRadius + "px",
                    width: "60px",
                  }}
                >
                  <Box sx={{ color: "#fff", fontWeight: "bold" }}>
                    {" "}
                    {reqName}
                  </Box>
                </InputAdornment>
              }
            />
          </Box>
        </Grid>
        {auth && (
          <Grid mb={2}>
            <Stack direction={"row"} sx={{ alignItems: "center" }} spacing={1}>
              <Typography variant="h5" gutterBottom>
                Authorization
              </Typography>
              <FormLabel>Bearer Token</FormLabel>
            </Stack>
            <Divider />
            <Stack
              direction={"row"}
              spacing={1}
              mt={1}
              sx={{ alignItems: "center" }}
            >
              <Typography variant="h5" gutterBottom>
                Token
              </Typography>
              <FormLabel>{"<" + "token" + ">"}</FormLabel>
            </Stack>
          </Grid>
        )}
        {queryParamters.length > 0 && (
          <Grid>
            <Stack spacing={1}>
              <Typography variant="h4" gutterBottom>
                {"Query Params"}
              </Typography>
              <Table sx={{ maxWidth: "550px" }} aria-label="simple table">
                <TableBody
                  sx={{
                    "&.MuiTableBody-root": {
                      maxWidth: "550px",
                    },
                  }}
                >
                  {queryParamters.map((row) => (
                    <TableRow
                      key={row.name}
                      sx={{
                        "&:last-child td, &:last-child th": { border: 0 },
                      }}
                    >
                      <TableCell sx={tableStyle.td}>{row.key}</TableCell>
                      <TableCell sx={tableStyle.td}>{row.value}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </Stack>
            <Grid mt={1}></Grid>
          </Grid>
        )}
        <Grid container   style={{paddingLeft:"0px"}}>
          <Grid item xs={7}>
            <Stack spacing={1}>
              <Typography variant="h4" gutterBottom>
                {"Format"}
              </Typography>
              <Table sx={{ maxWidth: "550px" }} aria-label="simple table">
                <TableBody
                  sx={{
                    "&.MuiTableBody-root": {
                      maxWidth: "550px",
                    },
                  }}
                >
                  {format?.map((row) => (
                    <TableRow
                      key={row.property}
                      sx={{
                        "&:last-child td, &:last-child th": { border: 0 },
                      }}
                    >
                      <TableCell sx={tableObjecDescStyle.td}>
                        {row.property}
                      </TableCell>
                      <TableCell sx={tableObjecDescStyle.td}>
                        {row.type}
                      </TableCell>
                      <TableCell sx={tableObjecDescStyle.td}>
                        {row.isRequired && (
                          <Box>
                            <Chip
                              sx={{
                                "&.MuiChip-root": {
                                  size: "5px",
                                  borderRadius: "8px",
                                  height: "15px",
                                  fontSize: "10px",
                                },
                              }}
                              label="REQUIRED"
                              color="warning"
                              size="small"
                            />
                          </Box>
                        )}
                        {row.description}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </Stack>
            
          </Grid>
          <Grid item xs={5}>
            <JsonViewViewer title="Body" jsonData={jsonData} />
            <JsonViewViewer title="Response" jsonData={responseData} />
          </Grid>
        </Grid>
      </Stack>
    </Grid>
  );
}
export default Documentation;
