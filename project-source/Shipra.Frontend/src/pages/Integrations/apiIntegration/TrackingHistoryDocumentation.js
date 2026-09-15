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

function TrackingHistoryDocumentation(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [jsonData, setJsonData] = useState({
    userName: "",
    refreshToken: "",
  });
  const [format, setFormat] = useState([
    {
      property: "OrderNo",
      type: "string",
      isRequired: true,
      description: "Order No is required for get order history.",
    },
  ]);
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
        requestType={EnumRequestType.Get}
        name="Order Tracking"
        endPoint={`${process.env.REACT_APP_Prod_BaseUrl}/api/GetOrderTrackingHistoryByOrderNo?OrderNo=10000001`}
        jsonData={jsonData}
        responseData={responseData}
        format={format}
        auth
      />
    </>
  );
}
export default TrackingHistoryDocumentation;
