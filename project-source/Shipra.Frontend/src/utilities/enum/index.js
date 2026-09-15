const EnumOrderType = {
  // Private Fields
  Regular: 1,
  FullFillable: 2,

  properties: {
    1: { name: "Regular", value: 1 },
    2: { name: "FullFillable", value: 2 },
  },
};
const EnumPaymentStatus = {
  // Private Fields
  Unpaid: 1,
  Paid: 2,

  properties: {
    1: { name: "Unpaid", value: 1 },
    2: { name: "Paid", value: 2 },
  },
};
const EnumFullFillmentStatus = {
  // Private Fields
  UnFulfillment: 1,
  Fulfillment: 2,
  Allocated: 3,

  properties: {
    1: { name: "UnFulfillment", value: 1 },
    2: { name: "Fulfillment", value: 2 },
    3: { name: "Allocated", value: 3 },
  },
};
const EnumNotificationTypes = {
  // Private Fields
  SendPayment: 1,
  SendShipping: 2,
  SendInvoice: 3,
  properties: {
    1: { name: "SendPayment", value: 1 },
    2: { name: "SendShipping", value: 2 },
    3: { name: "SendInvoice", value: 3 },
  },
};
const EnumUserType = {
  // Private Fields
  Admin: 1,
  Accounts: 2,
  DataEntry: 3,
  CallCenter: 4,
  Driver: 5,
  SalesCoordinator: 6,
  properties: {
    1: { name: "SuperAdmin", value: 1 },
    2: { name: "Admin", value: 2 },
    3: { name: "Employee", value: 3 },
    4: { name: "Driver", value: 4 },
    5: { name: "SalePerson", value: 5 },
    6: { name: "SalesCoordinator", value: 6 },
  },
};
const EnumAwbType = {
  // Private Fields
  Awb4x6TypeId: 1,
  CarrierAwbTypeId: 2,
  properties: {
    1: { name: "Awb4x6TypeId", value: 1 },
    2: { name: "CarrierAwbTypeId", value: 2 },
  },
};
const EnumPaymentMethod = {
  // Private Fields
  Prepaid: 1,
  CashOnDelivery: 2,
  properties: {
    1: { name: "Prepaid", value: 1 },
    2: { name: "Cash On Delivery", value: 2 },
  },
};
const EnumDeliveryTaskStatus = {
  // Private Fields
  Pending: 1,
  Started: 2,
  Completed: 3,
  Attempted: 4,
  properties: {
    1: { name: "Pending", value: 1 },
    2: { name: "Started", value: 2 },
    3: { name: "Completed", value: 3 },
    4: { name: "Attempted", value: 4 },
  },
};
const EnumDeliveryNoteDetailStatusLookup = {
  // Private Fields
  Pending: 1,
  Completed: 2,
  Attempted: 3,
  properties: {
    1: { name: "Pending", value: 1 },
    2: { name: "Completed", value: 2 },
    3: { name: "Attempted", value: 3 },
  },
};
const EnumDeliveryNoteStatus = {
  // Private Fields
  InProgress: 1,
  Completed: 2,
  properties: {
    1: { name: "InProgress", value: 1 },
    2: { name: "Completed", value: 2 },
  },
};
const EnumCarrierTrackingStatus = {
  // Private Fields
  OrderPlaced: 1,
  InOperation: 2,
  AssignInHouse: 3,
  UnAssignfromcarrier: 4,
  OutForDelivery: 5,
  Delivered: 6,
  Refunded: 7,
  Exchanged: 8,
  PendingForReturn: 9,
  Returned: 10,
  Assigntocarrier: 11,
  OutofStock: 12,
  OnHold: 13,
  MobileSwitchedOff: 14,
  WrongItem: 15,
  UnderCustomsClearance: 16,
  Cancelled: 17,
  HandovertoCourier: 18,
  MobileNotAnswered: 19,
  Delayed: 20,
  WrongAmount: 21,
  InTransit: 22,
  Lost: 23,
  Damaged: 24,
  Schedulefortomorrow: 25,
  ReturntoOrigin: 26,
  LocationChanged: 27,
  HalfDelivered: 28,

  properties: {
    1: { name: "OrderPlaced", value: 1, color: "var(--primary-color)" },
    2: { name: "InOperation", value: 2, color: "#28a745" },
    3: { name: "AssignInHouse", value: 3, color: "#FB4D31" },
    4: { name: "UnAssignfromcarrier", value: 4, color: "#F63AA0" },
    5: { name: "OutForDelivery", value: 5, color: "#1EA7FD" },
    6: { name: "Delivered", value: 6, color: "#E69D00" },
    7: { name: "Refunded", value: 7, color: "#37D5D3" },
    8: { name: "Exchanged", value: 8, color: "#6F2CAC" },
    9: { name: "PendingForReturn", value: 9, color: "#0c8e7d" },
    10: { name: "Returned", value: 10, color: "#040000" },
    11: { name: "Assigntocarrier", value: 11, color: "#889970" },
    12: { name: "OutofStock", value: 12, color: "#659962" },
    13: { name: "OnHold", value: 13, color: "#e7b3a7" },
    14: { name: "MobileSwitchedOff", value: 14, color: "#1b3461" },
    15: { name: "WrongItem", value: 15, color: "#648198" },
    16: { name: "UnderCustomsClearance", value: 16, color: "#ea5a4f" },
    17: { name: "Cancelled", value: 17, color: "#dc3545" },
    18: { name: "HandovertoCourier", value: 18, color: "#A0C2BB" },
    19: { name: "MobileNotAnswered", value: 19, color: "#926ca0" },
    20: { name: "Delayed", value: 20, color: "#95D4A3" },
    21: { name: "WrongAmount", value: 21, color: "#faa22f" },
    22: { name: "InTransit", value: 22, color: "#B9F8C0" },
    23: { name: "Lost", value: 23, color: "#61678B" },
    24: { name: "Damaged", value: 24, color: "#98BFD0" },
    25: { name: "Schedulefortomorrow", value: 25, color: "#BA667E" },
    26: { name: "ReturntoOrigin", value: 26, color: "#2E4052" },
    27: { name: "LocationChanged", value: 27, color: "#42364C" },
    28: { name: "HalfDelivered", value: 28, color: "#A3966A" },
    29: { name: "SendToCSA", value: 29, color: "#c6d2d9" },
    30: { name: "TobePickedUp", value: 30, color: "var(--primary-color)47" },
  },
};
const EnumAllProductOption = {
  // Private Fields
  Size: 1,
  Company: 2,
  Color: 3,
  Model: 4,

  properties: {
    1: { name: "Size", value: 1 },
    2: { name: "Company", value: 2 },
    3: { name: "Color", value: 3 },
    4: { name: "Model", value: 4 },
  },
};

const EnumMetaField = {
  Order: 1,
  Product: 2,
};

const EnumRequestType = {
  // Private Fields
  Get: 1,
  Post: 2,
  properties: {
    1: { name: "GET", value: 1 },
    2: { name: "POST", value: 2 },
  },
};
const EnumCarrierAssign = {
  // Private Fields
  UnAssigne: 1,
  Assign: 2,
  properties: {
    1: { name: "UnAssigne", value: 1 },
    2: { name: "Assign", value: 2 },
  },
};
const EnumDeliveryNoteStatusId = {
  // Private Fields
  Pending: 1,
  Completed: 2,
  properties: {
    1: { name: "Pending", value: 1 },
    2: { name: "Completed", value: 2 },
  },
};
const EnumEmployeeType = {
  EMPLOYEE: 1,
  SHIPPER: 2,
};
const EnumRoutesUrls = {
  PROFILE: "/profile",
  SECURITY: "/security",
  USERS: "/users",
  SETTINGS: "/settings",
  ACCOUNTS: "/accounts",
  SESSIONS: "/sessions",
  CONSTACT_US: "/contact-us",
  PASSWORD: "/password",
  SIGN_IN_HISTORY: "/sign-in-history",
  DRIVER_STATUSES: "/driver-statuses",
  SHIPMENT_TAB: "/shipment-tab",
  LEAD_STATUS: "/lead-status",
  DASHBOARDS: "/dashboards",
  SALE_DASHBOARD: "/sale-dashboard",
  CARRIER_DASHBOARD: "/carrier-dashboard",
  CARRIER_STATUS: "/carrier-status",
  ANALYTICS: "/analytics",
  ORDERS: "/orders-dashboard",
  APP_SETTINGS: "/app-settings",
  PERRMISSIONS: "/permissions",
  UPDATE_PERRMISSION: "/update-permission",
  NOTFOUND_403: "/NotFound-403",
  META_FIELDS: "/meta-fields",
  TAX: "/tax",
  USER_ROLES: "/user-roles",
  MIX_SETTINGS: "/document-settings",
  MAINTENACE: "/maintenance",
  Notification: "/notification",
  ORDER_BOX: "/order-box",
  WEBHOOK_EVENTS: "/webhook-events",
  WEBHOOK_EVENTS_STARIC: "/webhook-events/*",
  BILLING: "/billing",
  PRICING_PLAN: "/pricing-plan",
  VERIFY_OTP: "/verify-otp",
  LOG_IN: "/login",
  PAYOUT_BANK: "/payout-bank",
  GENERIC_SETTING: "/generic-setting",
  BRANDING: "/branding",
  UNAUTHORIZED: "/unauthorized",
  STATUS_REPORT: "/status-report",
  PICKUP_LOCATION: "/pickup-location",
  ORDERS_ARCHIVE: "/archive-orders",
  DRAFT_ORDER: "/draft-order-dashboard",
  GENERATE_INVOICE: "/generate-invoice",
  SERIVCE_RATE_GROUP: "/service-rate-group",
  SHIPPER_RATES: "/shipper-rates",
  INVOICE: "/invoice",
  INVOICE_ADJUSTMENT: "/invoice-adjustment",
  PERFORMANCE_REPORT: "/performance-report",
  FACTORY_RESET: "/factory-reset",
  ZONE_NAME: "/zone",
  SYNC_POLICIES: "/sync-policies",
};
const EnumNavigateState = {
  DELIVERY_TASKS: {
    pageName: "deliveryTasks",
    state: {
      selectedOrderNo: "selectedOrderNo",
    },
  },

  EDIT_ORDER: {
    pageName: "editOrder",
    state: {
      disabledInput: "disabledInput",
    },
  },
};
const EnumCancelPlanFeedbackOptions = [
  {
    label: "To Expensive",
    value: "too_expensive",
  },
  {
    label: "Missing Features",
    value: "missing_features",
  },
  {
    label: "Switched Service",
    value: "switched_service",
  },
  {
    label: "Unused",
    value: "unused",
  },
  {
    label: "Customer Service",
    value: "customer_service",
  },
  {
    label: "Too Complex",
    value: "too_complex",
  },
  {
    label: "Low Quality",
    value: "low_quality",
  },
  {
    label: "Other",
    value: "other",
  },
];
const EnumOptions = {
  CLIENT_LEAD_STATUS: {
    LABEL: "description",
    VALUE: "clientLeadStatusId",
  },
  CARRIER_TRACKING_STATUS: {
    LABEL: "trackingStatus",
    VALUE: "carrierTrackingStatusId",
  },
  STORE: {
    LABEL: "storeName",
    VALUE: "storeId",
  },
  EMPLOYEE_STORE: {
    LABEL: "StoreName",
    VALUE: "StoreId",
  },
  ROLE: {
    LABEL: "roleName",
    VALUE: "clientUserRoleId",
  },
  PRODUCTS: {
    LABEL: "SKUOption",
    VALUE: "ProductId",
  },
  SELECT_STATION: {
    LABEL: "sname",
    VALUE: "productStationId",
  },
  REGIONS: {
    LABEL: "text",
    VALUE: "id",
  },
  ORDER_TYPE: {
    LABEL: "orderTypeName",
    VALUE: "orderTypeId",
  },
  CARRIER: {
    LABEL: "name",
    VALUE: "activeCarrierId",
  },
  ACTIVE_CARRIER: {
    LABEL: "Name",
    VALUE: "ActiveCarrierId",
  },
  ALL_CARRIER: {
    LABEL: "Name",
    VALUE: "CarrierId",
  },
  COUNTRY: {
    LABEL: "name",
    VALUE: "countryId",
  },
  SERVICES: {
    LABEL: "serviceName",
    VALUE: "deliveryServiceId",
  },
  FULL_FILLMENT_STATUS: {
    LABEL: "text",
    VALUE: "id",
  },
  SELECTED_REASON: {
    LABEL: "text",
    VALUE: "id",
  },
  PICKUP_CARRIER_LOCATION: {
    LABEL: "countryName",
    VALUE: "carrierLocationId",
  },
  CARRIER_PICKUP_LOCATION: {
    LABEL: "fullAddress",
    VALUE: "activeCarrierPickupLocationId",
  },
  PAYMENT_STATUS: {
    LABEL: "text",
    VALUE: "id",
  },
  PAYMENT_METHOD: {
    LABEL: "pmName",
    VALUE: "paymentMethodId",
  },
  CARRIER_ASSIGN: {
    LABEL: "",
    VALUE: "",
  },
  SALES_PERSON: {
    LABEL: "text",
    VALUE: "id",
  },
  CITY: {
    LABEL: "text",
    VALUE: "id",
  },
  DRIVER: {
    LABEL: "DriverName",
    VALUE: "DriverId",
  },
  EXPENSE_TYPE: {
    LABEL: "text",
    VALUE: "id",
  },
  METAFIELD_TYPES: {
    LABEL: "label",
    VALUE: "id",
  },
  USER_ROLE: {
    LABEL: "roleName",
    VALUE: "clientUserRoleId",
  },
  DELIVRRY_TASK_STATUS: {
    LABEL: "deliveryTaskStatus",
    VALUE: "deliveryTaskStatusId",
  },
  STORE_CHANNEL: {
    LABEL: "text",
    VALUE: "id",
  },
  SELECTED_REGION: {
    LABEL: "name",
    VALUE: "regionId",
  },
  SElECTED_CITY: {
    LABEL: "name",
    VALUE: "cityId",
  },
  CATAGORY: {
    LABEL: "categoryName",
    VALUE: "productCategoryId",
  },
  SALE_CHANNEL: {
    LABEL: "saleChannelName",
    VALUE: "saleChannelLookupId",
  },
  CHOOSE_STATUS: {
    LABEL: "trackingStatus",
    VALUE: "carrierTrackingStatusId",
  },
  GENDER: {
    LABEL: "text",
    VALUE: "id",
  },
  ALL_SALE_CHANNEL: {
    LABEL: "text",
    VALUE: "id",
  },
  SELECT_SMS_SERVICES: {
    LABEL: "serviceName",
    VALUE: "smsLookupId",
  },
  WHATSAPP_SERVICES: {
    LABEL: "serviceName",
    VALUE: "whatsAppLookupId",
  },
  SELECT_PAYMENT_PROCESSOR: {
    LABEL: "ppname",
    VALUE: "pplookupId",
  },
  CLIENT_TEXT: {
    LABEL: "name",
    VALUE: "taxId",
  },
  CONFIG_SETTING_INPUTDATA_OPTIONS: {
    LABEL: "label",
    VALUE: "value",
  },
  TIME_ZONE: {
    LABEL: "text",
    VALUE: "id",
  },
  NOTIFICATION_EVENTS: {
    LABEL: "eventName",
    VALUE: "notificationEventId",
  },
  NOTIFICATION_SERVICE: {
    LABEL: "ServiceName",
    VALUE: "SMSActivateId",
  },
  WHATSAPP_NOTIFICATION_SERVICE: {
    LABEL: "ServiceName",
    VALUE: "WhatsAppActivateId",
  },
  CANCEL_PLAN_FEEDBACK: {
    LABEL: "label",
    VALUE: "value",
  },
  RETURN_STATUS: {
    LABEL: "returnStatus",
    VALUE: "returnStatusLookupId",
  },
  WEBHOOK_EVENTS: {
    LABEL: "eventName",
    VALUE: "webhookEventLookupId",
  },
  ORDER: {
    LABEL: "OrderNo",
    VALUE: "OrderId",
  },
  LINK_STATUS: {
    LABEL: "statusName",
    VALUE: "paymentLinkStatusId",
  },
  TRANSCTION: {
    LABEL: "transactionName",
    VALUE: "transactionTypeId",
  },
  RETURN_REASON: {
    LABEL: "reason",
    VALUE: "clientReturnReasonId",
  },
  ORDER_LABELS: {
    LABEL: "labelName",
    VALUE: "clientOrderLabelLookupId",
  },
  CONTAINER_CODE: {
    LABEL: "text",
    VALUE: "id",
  },
  PICKUP_SERVICE_CODE: {
    LABEL: "text",
    VALUE: "id",
  },
  ORIGIN_TYPE: {
    LABEL: "typeName",
    VALUE: "civilEntityTypeId",
  },
  SERVICE_TYPE: {
    LABEL: "serviceName",
    VALUE: "deliveryServiceId",
  },
  EMPLOYEE_SALE_PERSON: {
    LABEL: "EmployeeDisplay",
    VALUE: "SaleChannelConfigId",
  },
  INVOICE_STATUS: {
    LABEL: "statusName",
    VALUE: "invoiceStatusId",
  },
  TRANSACTION_TYPE: {
    LABEL: "name",
    VALUE: "transactionTypeId",
  },
  CONTROLLER_NAME: {
    LABEL: "controllerName",
    VALUE: "controllerName",
  },
  PERMISSION_ACTION: {
    LABEL: "actionName",
    VALUE: "permissionActionId",
  },
  PERMISSION_GROUP: {
    LABEL: "permissionGroup",
    VALUE: "permissionGroupId",
  },
  EMPLOYEE_TYPE: {
    LABEL: "employeeTypeName",
    VALUE: "employeeTypeId",
  },
};

const EnumTableName = {
  ORDERTABLE: "orderTable",
  DELIVERY_TASK_TABLE: "deliveryTaskTable",
};

const EnumChangeFilterModelApiUrls = {
  GET_ALL_ORDERS: { url: "/Order/GetAllOrders", length: 100 },
  GET_ALL_SHIPMENTS: { url: "/Shipment/GetShipments", length: 100 },
  GET_ALL_PRODUCTS: { url: "/Product/GetAllProducts", length: 100 },
  GET_ALL_SALE_CHANNEL: {
    url: "/SaleChannel/GetAllSaleChannelLookupForSelection",
    length: 20,
  },
  GET_ALL_CDP_CUSTOMER: {
    url: "/CDP/GetAllCustomers",
    length: 100,
  },
  GET_ALL_SALE_CHANNEL_ACTIVE: {
    url: "/SaleChannel/GetAllSaleChannelConfig",
    length: 20,
  },
  GET_ALL_CARRIER: {
    url: "/Carrier/GetAllCarrierWithServiceAndLocation",
    length: 100,
  },
  GET_ALL_ORDER_PAYMENT_LINK: {
    url: "Order/GetAllOrderPaymentLinks",
    length: 100,
  },
};
const EnumApiUrls = {
  "/Order/GetWayBillsByOrderNos": {
    showLoading: true,
    loadingText: "Printing Labels",
  },
  "/Order/ExcelExportOrders": {
    showLoading: true,
    loadingText: "Exporting Excel",
  },
  "/Order/AssignToCarrier": {
    showLoading: true,
    loadingText: "Assigning Carrier",
  },
  "/Order/GetOrderInvoiceByOrderNos": {
    showLoading: true,
    loadingText: "Printing Invoice",
  },
  "/Order/GetCarrierWayBillsByOrderNos": {
    showLoading: true,
    loadingText: "Printing Labels",
  },
  "/Delivery/CreateDeliveryTask": {
    showLoading: true,
    loadingText: "Assign In House",
  },
  "/Order/BatchUpdateOrderStatus": {
    showLoading: true,
    loadingText: "Update Status",
  },
  "/Order/RefreshCarrierStatus": {
    showLoading: true,
    loadingText: "Refreshing Status",
  },
  "/Order/GetAllOrders": {
    changeFilterModel: true,
  },
  "/Shipment/GetShipments": {
    changeFilterModel: true,
  },
  "/Product/GetAllProducts": {
    changeFilterModel: true,
  },
  "/Carrier/GetAllCarrierWithServiceAndLocation": {
    changeFilterModel: true,
  },
  "/Order/ExcelExportOrders": {
    changeFilterModel: true,
    showLoading: true,
    loadingText: "Exporting Excel",
  },
  "/Order/DeleteOrders": {
    changeFilterModel: true,
    showLoading: true,
    loadingText: "Deleting Order",
  },
};
const EnumNonCancellableApiUrls = [
  "/UserManagement/GetAccessTokenWithRefreshToken",
];
const EnumCookieKeys = {
  TIME_ZONE: "timeZone",
  COUNTRY: "country",
  COUNTRY_ID: "countryId",
  LOGGEDIN_CLIENT_COUNTRY_ID: "loggedinClientCountryId",
  COUNTRY_CODE: "countryCode",
  RESTRICTED_COUNTRY: "mapCountry",
};

const EnumReturnStatus = Object.freeze({
  CREATED: { VALUE: 1, LABEL: "Created", COLOR: "warning.main" },
  IN_PROGRESS: { VALUE: 2, LABEL: "In Progress", COLOR: "warning.main" },
  APPROVED: { VALUE: 3, LABEL: "Approved", COLOR: "success.main" },
  REJECTED: { VALUE: 4, LABEL: "Rejected", COLOR: "error.main" },
});

const EnumDetailsModalType = {
  ORDER: "order",
  SHIPMENT: "shipment",
};

const inputTypesEnum = Object.freeze({
  SELECT: "select",
  TEXT: "text",
  NUMBER: "number",
  RADIO: "radio",
  CHECKBOX: "checkbox",
});
const viewTypesEnum = Object.freeze({
  GRID: 1,
  TABLE: 2,
});
const EnumCarrierContractType = Object.freeze({
  OwnContractType: 1,
  ShipraContactType: 2,
});

const EnumLeadStatus = Object.freeze({
  Open: 1,
  InProgress: 2,
  Called: 3,
  Completed: 4,

  properties: {
    1: { name: "Open", value: 1 },
    2: { name: "In progress", value: 2 },
    3: { name: "Called", value: 3 },
    4: { name: "Completed", value: 4 },
  },
});

export {
  EnumOrderType,
  EnumPaymentStatus,
  EnumFullFillmentStatus,
  EnumNotificationTypes,
  EnumAwbType,
  EnumPaymentMethod,
  EnumDeliveryTaskStatus,
  EnumDeliveryNoteDetailStatusLookup,
  EnumDeliveryNoteStatus,
  EnumCarrierTrackingStatus,
  EnumAllProductOption,
  EnumRequestType,
  EnumCarrierAssign,
  EnumRoutesUrls,
  EnumDeliveryNoteStatusId,
  EnumUserType,
  EnumNavigateState,
  EnumOptions,
  EnumApiUrls,
  EnumCookieKeys,
  EnumDetailsModalType,
  EnumChangeFilterModelApiUrls,
  EnumNonCancellableApiUrls,
  inputTypesEnum,
  viewTypesEnum,
  EnumCancelPlanFeedbackOptions,
  EnumCarrierContractType,
  EnumReturnStatus,
  EnumTableName,
  EnumMetaField,
  EnumEmployeeType,
  EnumLeadStatus,
};
