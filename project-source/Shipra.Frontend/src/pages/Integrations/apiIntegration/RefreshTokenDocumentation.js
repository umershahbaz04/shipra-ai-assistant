import {
  Box,
  Typography,
  Grid,
  Stack,
  FormLabel,
  Divider,
  Dialog,
  DialogContent,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import JsonViewViewer from "../../../.reUseableComponents/jsonViewer";
import TextFieldWithCopyButton from "../../../.reUseableComponents/TextField/TextFieldWithCopyButton";
import Documentation from "./Documentation";
import { EnumOrderType, EnumRequestType } from "../../../utilities/enum";

function RefreshTokenDocumentation(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [format, setFormat] = useState([
    {
      property: "userName",
      type: "string",
      isRequired: true,
      description: "For get access by refresh token userName is required.",
    },
    {
      property: "refreshToken",
      type: "string",
      isRequired: true,
      description:
        "Refresh token is required for get new access token which already provided in login request response ",
    },
  ]);
  const [jsonData, setJsonData] = useState({
    userName: "",
    refreshToken: "",
  });
  const [responseData, setResponseData] = useState({
    result: {
      access_token: "",
      token_type: "Bearer",
      expires_in: 3600,
      user_name: "",
      refresh_token: "",
      id_token: "",
      email: "",
      companyImage: "",
    },
    isSuccess: true,
    errors: {},
    errorCombined: {},
    errorID: 0,
    statusCode: 200,
  });
  return (
    <>
      <Documentation
        requestType={EnumRequestType.Post}
        name="Get Access Token with refresh token"
        endPoint={`${process.env.REACT_APP_Prod_BaseUrl}/api/GetAccessTokenWithRefreshToken`}
        jsonData={jsonData}
        responseData={responseData}
        format={format}
        auth
      />
    </>
  );
}
export default RefreshTokenDocumentation;
