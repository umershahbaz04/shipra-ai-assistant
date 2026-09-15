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
import DocumentationOrder from "./DocumentationOrder";

function CreateOrderDocumentation(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [format, setFormat] = useState([
    {
      property: "IsSaleChannelOrder",
      type: "Boolean",
      description:
        "If the order is created from an external sales channel, the IsSaleChannelOrder value should be set to true.",
      isHeading: true,
    },
    {
      property: "orderList",
      type: "list",
      isRequired: true,
      description: "Shipra support list of orders ",
      isHeading: true,
    },
    {
      property: "storeId",
      type: "int",
      isRequired: false,
      description:
        "store is optional if you don't specify then default store will be used ",
    },
    {
      property: "channelId",
      type: "int",
      isRequired: false,
      description: "channelid  is optional ",
    },
    {
      property: "orderTypeId",
      type: "int",
      isRequired: true,
      description:
        "We support two type of order one is Regular Order if regular order then there is no product specification.\nother one is Fulfillable order when we have Fulfillable order then must choose product \nValue :  1 for Regular, 2 for fulfillable",
    },
    {
      property: "orderDate",
      type: "DateTime",
      isRequired: true,
      description: "OrderDate is required it could be off UTC format ",
    },
    {
      property: "description",
      type: "string",
      isRequired: false,
      description: "Order description ",
    },
    {
      property: "remarks",
      type: "string",
      isRequired: false,
      description: "Order remarks",
    },
    {
      property: "refNo",
      type: "string",
      isRequired: false,
      description: "Order refNo.",
    },
    {
      property: "amount",
      type: "decimal",
      isRequired: true,
      description: "Total amount of order ",
    },
    {
      property: "cShippingCharges",
      type: "decimal",
      isRequired: false,
      description: "Shipping charges for order",
    },
    {
      property: "paymentStatusId",
      type: "int",
      isRequired: false,
      description: "Payment status ",
    },
    {
      property: "weight",
      type: "decimal",
      isRequired: false,
      description: "Total weight of order",
    },
    {
      property: "itemValue",
      type: "decimal",
      isRequired: false,
      description: "order itemValue ",
    },
    {
      property: "orderRequestVia",
      type: "int",
      isRequired: true,
      description: "orderRequestVia always 1 ",
    },
    {
      property: "paymentMethodId",
      type: "int",
      isRequired: true,
      description:
        "paymentMethodId we have two type of payment method 1 for PP(Prepaid) and 2 for COD(Cash on delivery)",
    },
    {
      property: "stationId",
      type: "string",
      isRequired: true,
      description:
        "stationId is required in case of Fulfillable order otherwise it will be default.",
    },
    {
      property: "discount",
      type: "decimal",
      isRequired: false,
      description:
        "discount id optional value it will be total discount on whole amount",
    },
    {
      property: "orderNote",
      type: "object",
      isRequired: true,
      description: "orderNote is object type ",
      isHeading: true,
    },
    {
      property: "note",
      type: "string",
      isRequired: true,
      description: "note value is required ",
    },
    {
      property: "orderAddress",
      type: "object",
      isRequired: true,
      description:
        "orderAddress object is type of object which contain values of receiver information ",
      isHeading: true,
    },
    {
      property: "customerName",
      type: "string",
      isRequired: true,
      description: "receiver name is required ",
    },
    {
      property: "email",
      type: "string",
      isRequired: false,
      description: "customer email it will be used for invoice sent if exist ",
    },
    {
      property: "mobile1",
      type: "string",
      isRequired: true,
      description: "Customer mobile ",
    },
    {
      property: "mobile2",
      type: "string",
      isRequired: false,
      description: "mobile2 is optional value",
    },
    {
      property: "countryId",
      type: "int",
      isRequired: true,
      description: "countryId is receiver country ",
    },
    {
      property: "areaId",
      type: "String",
      isRequired: true,
      description: "receiver area ",
    },
    {
      property: "cityId",
      type: "String",
      isRequired: true,
      description: "receiver city ",
    },
    {
      property: "provinceId",
      type: "String",
      isRequired: true,
      description: "receiver province ",
    },
    {
      property: "stateId",
      type: "String",
      isRequired: true,
      description: "receiver state ",
    },
    {
      property: "streetAddress",
      type: "string",
      isRequired: true,
      description: "customer street address ",
    },
    {
      property: "streetAddress2",
      type: "string",
      isRequired: true,
      description: "customer street address 2",
    },
    {
      property: "houseNo",
      type: "string",
      isRequired: true,
      description: "customer houseNo",
    },
    {
      property: "buildingName",
      type: "string",
      isRequired: true,
      description: "customer buildingName",
    },
    {
      property: "landmark",
      type: "string",
      isRequired: true,
      description: "customer landmark",
    },
    {
      property: "pinCodeId",
      type: "string",
      isRequired: true,
      description: "customer pinCodeId",
    },
    {
      property: "zip",
      type: "string",
      isRequired: true,
      description: "customer zip",
    },
    {
      property: "latitude",
      type: "decimal",
      isRequired: false,
      description: "receiver latitude ",
    },
    {
      property: "longitude",
      type: "decimal",
      isRequired: false,
      description: "receiver longitude",
    },
    {
      property: "orderBoxs",
      type: "object",
      isRequired: true,
      description:
        "orderBoxs object is type of object which contain values of receiver information ",
      isHeading: true,
    },
    {
      property: "clientOrderBoxId",
      type: "int",
      isRequired: true,
      description: "receiver clientOrderBoxId",
    },
    {
      property: "orderTaxes",
      type: "List",
      isRequired: false,
      description: "orderTaxes value is optional it will be total tax on whole amount",
      isHeading: true,
    },
    {
      property: "ClientTaxId",
      type: "int",
      isRequired: false,
      description: "receiver ClientTaxId.",
    },
    {
      property: "orderTaxId",
      type: "string",
      isRequired: false,
      description: "orderTaxId is required in case of updating order box.",
    },
    {
      property: "taxValue",
      type: "int",
      isRequired: false,
      description: "receiver taxValue.",
    },
    {
      property: "orderItems",
      type: "list",
      isRequired: true,
      description: "orderItems is required it must not be empty",
      isHeading: true,
    },
    {
      property: "productId",
      type: "string",
      isRequired: false,
      description: "productId is required in case of fulfillable order type 2.",
    },
    {
      property: "productStockId",
      type: "string",
      isRequired: false,
      description:
        "productStockId is required in case of fulfillable order type 2.",
    },
    {
      property: "price",
      type: "decimal",
      isRequired: true,
      description: "price of single item",
    },
    {
      property: "description",
      type: "string",
      isRequired: true,
      description: "item description",
    },
    {
      property: "remarks",
      type: "string",
      isRequired: false,
      description: "item remarks",
    },
    {
      property: "quantity",
      type: "int",
      isRequired: true,
      description: "quantity of item minimum 1.",
    },
    {
      property: "discount",
      type: "decimal",
      isRequired: false,
      description: "discount of item",
    },
  ]);
  const [jsonData, setJsonData] = useState({
    orderList: [
      {
        storeId: 74,
        SaleChannelConfigId: 3,
        orderTypeId: 1,
        orderDate: "4/28/2025",
        description: "Create Order",
        remarks: "Create Order",
        amount: 190.85,
        cShippingCharges: "9",
        paymentStatusId: 2,
        weight: 3,
        itemValue: "155",
        orderRequestVia: 1,
        paymentMethodId: 1,
        stationId: 47,
        discount: "15",
        vat: 0,
        refNo: "PV9800MX",
        orderNote: {
          note: "",
        },
        orderAddress: {
          SelectedCarrierId: 76,
          countryId: 1,
          cityId: "2",
          areaId: "2",
          streetAddress: "Pickle Street",
          streetAddress2: "West Avenue",
          houseNo: "",
          buildingName: "",
          landmark: "",
          provinceId: "",
          pinCodeId: "",
          stateId: "",
          zip: "",
          addressTypeId: 0,
          latitude: "25.2743",
          longitude: "56.270588",
          orderAddressId: 0,
          customerName: "Technobatch",
          email: "Technobatch@gmail.com",
          mobile1: "971329032933333",
          mobile2: "971900930039900",
        },
        orderTaxes: [
          {
            orderTaxId: "",
            ClientTaxId: 12,
            taxValue: 26.35,
          },
          {
            orderTaxId: "",
            ClientTaxId: 6,
            taxValue: 15.5,
          },
        ],
        orderBoxs: [
          {
            clientOrderBoxId: 9,
          },
        ],
        orderItems: [
          {
            price: 0,
            description: "Item 1",
            remarks: "",
            quantity: 1,
            discount: 0,
          },
        ],
      },
    ],
    IsSaleChannelOrder: false,
  });
  const [responseData, setResponseData] = useState({
    result: {
      data: [
        {
          orderId: "e14f5efd-06c0-4c1a-b3ad-b3ad4522b3f9",
          orderNo: "15700952",
          refNo: "PV9800MX",
          isSuccess: true,
        },
      ],
      message: "The action perform successfully",
    },
    isSuccess: true,
    errors: {},
    configErrors: null,
    errorCombined: {},
    errorID: 0,
    statusCode: 200,
  });
  return (
    <>
      <DocumentationOrder
        requestType={EnumRequestType.Post}
        name="Create Order"
        endPoint={`${process.env.REACT_APP_Prod_BaseUrl}/api/CreateOrder`}
        jsonData={jsonData}
        responseData={responseData}
        format={format}
        auth
      />
    </>
  );
}
export default CreateOrderDocumentation;
