import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import MenuIcon from "@mui/icons-material/Menu";
import MuiAppBar from "@mui/material/AppBar";
import Backdrop from "@mui/material/Backdrop";
import Box from "@mui/material/Box";
import CssBaseline from "@mui/material/CssBaseline";
import IconButton from "@mui/material/IconButton";
import Toolbar from "@mui/material/Toolbar";
import { styled, useTheme } from "@mui/material/styles";
import PropTypes from "prop-types";
import * as React from "react";
import { useDispatch, useSelector } from "react-redux";
import { Navigate } from "react-router-dom";
import useWindowDimensions from "../../utilities/customHooks/useWindowDimensions";
import SideNavBar from "../sideNavBar";
import TopNavBar, { languagesOptions } from "../topNavBar";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import GroupsIcon from "@mui/icons-material/Groups";
import SecurityIcon from "@mui/icons-material/Security";
import SettingsIcon from "@mui/icons-material/Settings";
import Undo from "@mui/icons-material/Undo";
import AssignmentIcon from "@mui/icons-material/Assignment";
import SummarizeOutlinedIcon from "@mui/icons-material/SummarizeOutlined";
import ExpandLess from "@mui/icons-material/ExpandLess";
import { Divider } from "@mui/material";
import Drawer from "@mui/material/Drawer";
import useMediaQuery from "@mui/material/useMediaQuery";
import mbLogo from "../../assets/images/header/mbLogo.png";
import { styleSheet } from "../../assets/styles/style";
import { getSubTabData } from "../../pages/settings/settings";
import { getThisKeyCookie } from "../../utilities/cookies";
import { EnumRoutesUrls, EnumUserType } from "../../utilities/enum";
import {
  purple,
  useClientSubscriptionReducer,
  useIsUserProfileSideBarShow,
} from "../../utilities/helpers/Helpers";
import MainLoader from "../loader/mainLoader";
import {
  AccountsIcon,
  CarrierReturnsIcon,
  ContractIcon,
  IntegrationIcon,
  MyCarrierIcon,
  OrderIcon,
  ProductIcon,
  SettingIcon,
  ShipmentIcon,
  StoreIcon,
  DashboardIcon,
} from "../sideNavBar/icons";
import UnauthorizedPage from "../unAuthorised";
import {
  GetAllOrdersDraftCount,
  GetAllClientMenuByRoleId,
} from "../../api/AxiosInterceptors";
import { changeBadgeData } from "../../redux/changeFlags";
import { setMenuPermissions } from "../../redux/menuPermissions";
import { transformApiMenuToNavBarMenu } from "../../utilities/helpers/menuHelpers";
const drawerWidth = 256;

const DrawerHeader = styled("div")(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  paddingLeft: "0px !important",
  padding: theme.spacing(0, 1),
  // necessary for content to be below app bar
  ...theme.mixins.toolbar,
}));
const AppBar = styled(MuiAppBar, {
  shouldForwardProp: (prop) => prop !== "open",
})(({ theme, open }) => ({
  zIndex: theme.zIndex.drawer + 1,
  transition: theme.transitions.create(["width", "margin"], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  background: "#f8f8f8",
  ...(open && {
    marginLeft: drawerWidth,
    width: `calc(100% - ${drawerWidth}px)`,
    transition: theme.transitions.create(["width", "margin"], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  }),
}));

export const MuiIcon = ({ isSelected, icon }) => {
  return React.cloneElement(icon, {
    sx: {
      color: isSelected ? "var(--primary-color)" : "#1E1E1E",
    },
  });
};

export const handleGetDraftOrdersCount = async () => {
  try {
    const response = await GetAllOrdersDraftCount();
    if (response?.data?.isSuccess) {
      return response.data.result;
    }
  } catch (e) { }
};

function PrivateRoutes({ children, type, ...rest }) {
  const { window } = rest;
  const [mobileOpen, setMobileOpen] = React.useState(false);
  const [pcOpen, setPcOpen] = React.useState(true);
  const { width } = useWindowDimensions();
  const theme = useTheme();
  const matches = useMediaQuery(theme.breakpoints.down("xl"));
  const brandingData = sessionStorage.getItem("Branding");
  const decoded = brandingData ? decodeURIComponent(brandingData) : null;
  const Branding = decoded ? JSON?.parse(decoded) : decoded;

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const isUserProfileSideBarShow = useIsUserProfileSideBarShow();

  const dispatch = useDispatch();

  const menuPermissionsState = useSelector(
    (state) => state.menuPermissionsReducer || {},
  );
  const menuPermissionsData = menuPermissionsState.menuPermissions || [];

  React.useEffect(() => {
    if (!menuPermissionsState.hasFetched) {
      GetAllClientMenuByRoleId()
        .then((res) => {
          if (res?.data?.result) {
            dispatch(setMenuPermissions(res.data.result));
          } else {
            dispatch(setMenuPermissions([]));
          }
        })
        .catch((e) => {
          console.error("Failed to fetch menu permissions in PrivateRoutes", e);
          dispatch(setMenuPermissions([]));
        });
    }
  }, [dispatch, menuPermissionsState.hasFetched]);

  const allowPersonalCarrierContract =
    getThisKeyCookie("allowPersonalCarrierContract") &&
    JSON.parse(getThisKeyCookie("allowPersonalCarrierContract"));

  const allowShipperInvocie =
    getThisKeyCookie("allowShipperInvocie") &&
    JSON.parse(getThisKeyCookie("allowShipperInvocie"));

  const userProfileSideBarMenu = [
    {
      isCollapse: false,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.SETTING_PROFILE_TEXT,
        path: EnumRoutesUrls.PROFILE,
      },
      icon: <MuiIcon icon={<AccountCircleIcon />} />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.SETING_SECURITY_TEXT,
        path: EnumRoutesUrls.PASSWORD,
      },
      subTabData: [
        {
          sideTitle: LanguageReducer?.languageType?.SETING_SECURITY_PASSWORD,
          topTitle: LanguageReducer?.languageType?.SETING_SECURITY_TEXT,
          path: EnumRoutesUrls.PASSWORD,
        },
      ],
      icon: <MuiIcon icon={<SecurityIcon />} />,
    },
    {
      isCollapse: false,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.SETTING_USER_TEXT,
        topTitle: LanguageReducer?.languageType?.SETTING_USER_USERS,
        path: EnumRoutesUrls.USERS,
      },
      icon: <MuiIcon icon={<GroupsIcon />} />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.SETTINGS,
        topTitle: LanguageReducer?.languageType?.SETTING_MANAGE_TAX_TEXT,
        path: EnumRoutesUrls.TAX,
      },
      subTabData: [
        // {
        //   sideTitle: LanguageReducer?.languageType?.DRIVER_STATUSES_TEXT,
        //   topTitle: LanguageReducer?.languageType?.DRIVER_STATUSE_S__TEXT,
        //   path: EnumRoutesUrls.DRIVER_STATUSES,
        // },
        {
          sideTitle: LanguageReducer?.languageType?.SETTING_MANAGE_TAX_TEXT,
          topTitle: LanguageReducer?.languageType?.SETTING_MANAGE_TAX_TEXT,
          path: EnumRoutesUrls.TAX,
        },
        {
          sideTitle: LanguageReducer?.languageType?.BILLING,
          topTitle: LanguageReducer?.languageType?.BILLING,
          path: EnumRoutesUrls.BILLING,
        },
      ],
      otherRoutes: [
        // {
        //   topTitle:
        //     LanguageReducer?.languageType?.CREATE_FULFILLABLE_ORDER_TEXT,
        //   path: "/create-fulfillable-order",
        // },
      ],
      icon: <MuiIcon icon={<SettingsIcon />} />,
    },
    // {
    //   isCollapse: false,
    //   tabData: {
    //     sideTitle: LanguageReducer?.languageType?.CONTACT_US_TEXT,
    //     path: EnumRoutesUrls.CONSTACT_US,
    //   },
    //   icon: <MuiIcon icon={<CallIcon />} />,
    // },
  ];

  const integrationSubData = [
    {
      sideTitle: "API" + " " + LanguageReducer?.languageType?.INTEGRATION,
      path: "/integration",
    },
    {
      sideTitle: LanguageReducer?.languageType?.INTEGRATION_CARRIER_TEXT,
      topTitle: LanguageReducer?.languageType?.INTEGRATION_CARRIER_TEXT_S,
      path: "/carriers",
    },
    {
      sideTitle: LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_TEXT,
      topTitle: LanguageReducer?.languageType?.INTEGRATION_SALE_CHANNEL_S_TEXT,
      path: "/sale-channels",
    },
    {
      sideTitle:
        LanguageReducer?.languageType?.INTEGRATION_SMS_INTEGRATION_TEXT,
      path: "/sms",
    },
    {
      sideTitle: "Whatsapp Integration",
      path: "/whatsapp",
    },
    {
      sideTitle:
        LanguageReducer?.languageType?.INTEGRATION_PAYMENT_INTEGRATION_TEXT,
      path: "/payment",
    },
  ];

  const contractMenu = [
    allowShipperInvocie && {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.CONTRACT_CONTRACT_TEXT,
        topTitle:
          LanguageReducer?.languageType
            ?.SERVICE_RATE_GROUP_SERVICE_RATE_GROUP_TEXT,
        path: EnumRoutesUrls.SERIVCE_RATE_GROUP,
      },
      subTabData: [
        {
          sideTitle:
            LanguageReducer?.languageType
              ?.SERVICE_RATE_GROUP_SERVICE_RATE_GROUP_TEXT,
          topTitle:
            LanguageReducer?.languageType
              ?.SERVICE_RATE_GROUP_SERVICE_RATE_GROUP_TEXT,
          path: EnumRoutesUrls.SERIVCE_RATE_GROUP,
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.SHIPPER_RATES_SHIPPER_RATES_TEXT,
          topTitle:
            LanguageReducer?.languageType?.SHIPPER_RATES_SHIPPER_RATES_TEXT,
          path: EnumRoutesUrls.SHIPPER_RATES,
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.GENERATE_INVOICE_GENERATE_TEXT,
          topTitle:
            LanguageReducer?.languageType?.GENERATE_INVOICE_GENERATE_TEXT,
          path: EnumRoutesUrls.GENERATE_INVOICE,
        },
        {
          sideTitle: LanguageReducer?.languageType?.INVOICE_INVOICE_TEXT,
          topTitle: LanguageReducer?.languageType?.INVOICE_INVOICE_TEXT,
          path: EnumRoutesUrls.INVOICE,
        },
        {
          sideTitle: "Invoice Adjustment",
          topTitle: "Invoice Adjustment",
          path: EnumRoutesUrls.INVOICE_ADJUSTMENT,
        },
      ],
      otherRoutes: [],
      icon: <ContractIcon />,
    },
  ].filter(Boolean);

  const navBarMenu = [
    {
      isCollapse: false,
      tabData: {
        sideTitle: "Dashboards",
        path: EnumRoutesUrls.DASHBOARDS,
      },
      otherRoutes: [
        {
          path: EnumRoutesUrls.SALE_DASHBOARD,
        },
        {
          path: EnumRoutesUrls.CARRIER_DASHBOARD,
        },
      ],
      icon: <DashboardIcon />,
    },
    {
      isCollapse: false,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.ANALYTICS_TEXT,
        path: "/analytics",
      },
      icon: <AccountsIcon />,
    },
    {
      isCollapse: false,
      tabData: {
        sideTitle: allowShipperInvocie
          ? "Stores / Shipper"
          : LanguageReducer?.languageType?.STORES_TEXT,
        topTitle: allowShipperInvocie
          ? "Stores / Shipper"
          : LanguageReducer?.languageType?.STORE_S_TEXT,
        path: "/store",
      },
      otherRoutes: [
        {
          topTitle: "Upload Store",
          path: "/upload-stores",
        },
      ],
      icon: <StoreIcon />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.PRODUCTS_TEXT,
        topTitle: LanguageReducer?.languageType?.PRODUCTS_TEXT_S,
        path: "/products",
      },
      subTabData: [
        {
          sideTitle: LanguageReducer?.languageType?.PRODUCT_INVENTORY_TEXT,
          topTitle: LanguageReducer?.languageType?.PRODUCT_INVENTORY_TEXT,
          path: "/inventory",
        },
        {
          sideTitle: LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_TEXT,
          path: "/low-inventory",
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.PRODUCT_INVENTORY_SALES_TEXT,
          topTitle: LanguageReducer?.languageType?.INVENTORY_SALE_S_TEXT,
          path: "/inventory-sales",
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.PRODUCT_SALE_CHANNEL_PRODUCT_TEXT,
          topTitle:
            LanguageReducer?.languageType?.PRODUCT_SALE_CHANNEL_PRODUCT_S_TEXT,
          path: "/sale-channel-product",
        },
        {
          sideTitle: "Sync Policies",
          topTitle: "Sync Policies",
          path: "/sync-policies",
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.PRODUCT_PRODUCT_STATIONS_TEXT,
          topTitle:
            LanguageReducer?.languageType?.PRODUCT_PRODUCT_STATION_S_TEXT,
          path: "/product-station",
        },
      ],
      otherRoutes: [
        {
          topTitle: "Sync Policies",
          path: "/sync-policies",
        },
        {
          topTitle: LanguageReducer?.languageType?.ADD_PRODUCTS_TEXT,
          path: "/add-products",
        },
        {
          topTitle: "Upload Product",
          path: "/upload-product",
        },
        {
          topTitle: "Edit Product",
          path: "/edit-products",
        },
      ],
      icon: <ProductIcon />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: "Leads",
        topTitle: "Leads",
        path: "/leads",
      },
      subTabData: [
        {
          sideTitle: "Leads",
          topTitle: "Leads",
          path: "/leads",
        },
        {
          sideTitle: "Contacts",
          topTitle: "Contacts",
          path: "/contacts",
        },
      ],
      otherRoutes: [
        {
          topTitle: "Upload Leads",
          path: "/upload-leads",
        },
      ],
      icon: <MuiIcon icon={<AssignmentIcon />} />,
    },
    {
      isCollapse: false,
      tabData: {
        sideTitle: "Performance Report",
        topTitle: "Performance Report",
        path: EnumRoutesUrls.PERFORMANCE_REPORT,
      },
      icon: <MuiIcon icon={<SummarizeOutlinedIcon />} />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.ORDERS_TEXT,
        topTitle: LanguageReducer?.languageType?.ORDERS_ORDER_DASHBOARD,
        path: "/orders-dashboard",
      },
      subTabData: [
        {
          sideTitle: "Draft Orders",
          topTitle: "Draft Orders",
          path: "/draft-order-dashboard",
          hasBadge: true,
        },
        {
          sideTitle: LanguageReducer?.languageType?.ORDER_RETURN_ORDER,
          topTitle: LanguageReducer?.languageType?.ORDER_RETURN_ORDER,
          path: "/return-order",
        },
        {
          sideTitle: LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR,
          topTitle: LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR,
          path: "/price-calculator",
        },
        {
          sideTitle: LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK,
          topTitle: LanguageReducer?.languageType?.ORDERS_PAYMENT_LINK,
          path: "/payment-link",
        },
        {
          sideTitle: "Order Labels",
          topTitle: "Order Labels",
          path: "/order-lables",
        },
        {
          sideTitle: LanguageReducer?.languageType?.ORDERS_WALLET,
          topTitle: LanguageReducer?.languageType?.ORDERS_WALLET,
          path: "/wallet",
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_TEXT,
          topTitle:
            LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_S_TEXT,
          path: "/sale-channel-orders",
        },
      ],
      otherRoutes: [
        {
          topTitle:
            LanguageReducer?.languageType?.ORDERS_CREATE_FULFILLABLE_ORDER,
          path: "/create-fulfillable-order",
        },
        {
          topTitle: LanguageReducer?.languageType?.ORDERS_CREATE_REGULAR_ORDER,
          path: "/create-regular-order",
        },
        {
          topTitle: "Draft Fullfillable Order",
          path: "/draft-fullfillable-order",
        },
        {
          topTitle: "Draft Regular Order",
          path: "/draft-regular-order",
        },
        {
          topTitle: LanguageReducer?.languageType?.ORDERS_UPLOAD_ORDER,
          path: "/upload-orders",
        },
        {
          topTitle: LanguageReducer?.languageType?.ORDERS_UPDATE_REGULAR_ORDER,
          path: "/edit-order-regular",
        },
        {
          topTitle:
            LanguageReducer?.languageType?.ORDERS_UPDATE_FULFILLABLE_ORDER,
          path: "/edit-order-fulfillable",
        },
      ],
      icon: <OrderIcon />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.SHIPMENTS_TEXT,
        topTitle: LanguageReducer?.languageType?.SHIPMENT_S_TEXT,
        path: "/shipments",
      },
      subTabData: [
        {
          sideTitle: "Status Report",
          topTitle: "Status Report",
          path: EnumRoutesUrls.STATUS_REPORT,
        },
        {
          sideTitle: "Pickup Location",
          topTitle: "Pickup Location",
          path: EnumRoutesUrls.PICKUP_LOCATION,
        },
      ],
      icon: <ShipmentIcon />,
    },
    ...contractMenu,
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.CARRIER_RETURNS_TEXT,
        topTitle: LanguageReducer?.languageType?.CARRIER_RETURN_S_TEXT,
        path: "/carrier-returns",
      },
      subTabData: [
        {
          sideTitle:
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURNS,
          topTitle:
            LanguageReducer?.languageType
              ?.CARRIER_RETURN_CREATE_CARRIER_RETURN_S,
          path: "/create-carrier-returns",
        },
        // {
        //   sideTitle: LanguageReducer?.languageType?.PENDING_CARRIER_RETURNS,
        //   topTitle: LanguageReducer?.languageType?.PENDING_CARRIER_RETURN_S,
        //   path: "/pending-carrier-returns",
        // },
      ],
      otherRoutes: [
        // {
        //   topTitle:
        //     LanguageReducer?.languageType?.CREATE_FULFILLABLE_ORDER_TEXT,
        //   path: "/create-fulfillable-order",
        // },
      ],
      icon: <CarrierReturnsIcon />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.My_CARRIER,
        topTitle:
          LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASK_S_TEXT,
        path: "/delivery-tasks",
      },
      subTabData: [
        {
          sideTitle:
            LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_TEXT,
          topTitle:
            LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASK_S_TEXT,
          path: "/delivery-tasks",
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTES_TEXT,
          topTitle:
            LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_NOTE_S_TEXT,
          path: "/delivery-notes",
        },
        {
          sideTitle: LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_TEXT,
          topTitle: LanguageReducer?.languageType?.MY_CARRIER_EXPENSE_S_TEXT,
          path: "/driver-expenses",
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.MY_CARRIER_DRIVER_RECIEVEABLE_TEXT,
          path: "/driver-recievable",
        },
        {
          sideTitle: LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_TEXT,
          path: "/cod-collection-pending",
        },
        // {
        //  sideTitle: LanguageReducer?.languageType?.COD_COLLECTION,
        //   path: "/cod-collections",
        // },

        // {
        //   sideTitle: LanguageReducer?.languageType?.DRIVERS,
        //   topTitle: LanguageReducer?.languageType?.DRIVER_S,
        //   path: "/drivers",
        // },
        {
          sideTitle:
            LanguageReducer?.languageType?.MY_CARRIER_PENDING_FOR_RETURN_TEXT,
          path: "/pending-for-return",
        },
        {
          sideTitle: LanguageReducer?.languageType?.MY_CARRIER_RETURN,
          topTitle: LanguageReducer?.languageType?.MY_CARRIER_RETURN_S,
          path: "/returns",
        },
      ],
      otherRoutes: [
        {
          topTitle:
            LanguageReducer?.languageType?.ORDERS_CREATE_FULFILLABLE_ORDER,
          path: "/create-fulfillable-order",
        },
        {
          topTitle: LanguageReducer?.languageType?.ORDERS_CREATE_REGULAR_ORDER,
          path: "/create-regular-order",
        },
        {
          topTitle: LanguageReducer?.languageType?.ORDERS_UPLOAD_ORDER,
          path: "/upload-orders",
        },
      ],
      icon: <MyCarrierIcon />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.ACCOUNTS,
        topTitle: LanguageReducer?.languageType?.COD_PENDING,
        path: "/cod-pending",
      },
      subTabData: [
        {
          sideTitle: LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_TEXT,
          path: "/cod-pending",
        },
        {
          sideTitle:
            LanguageReducer?.languageType?.ACCOUNTS_CARRIER_SETTLEMENT_TEXT,
          path: "/carrier-settlement",
        },
        // {
        //   sideTitle: LanguageReducer?.languageType?.PROFIT,
        //   path: "/profit",
        // },
        // {
        //   sideTitle: LanguageReducer?.languageType?.EXPENSE,
        //   path: "/expense",
        // },
      ],
      otherRoutes: [
        {
          topTitle:
            LanguageReducer?.languageType?.ORDERS_CREATE_FULFILLABLE_ORDER,
          path: "/create-fulfillable-order",
        },
        {
          topTitle: LanguageReducer?.languageType?.ORDERS_CREATE_REGULAR_ORDER,
          path: "/create-regular-order",
        },
        {
          topTitle: LanguageReducer?.languageType?.ORDERS_UPLOAD_ORDER,
          path: "/upload-orders",
        },
      ],
      icon: <AccountsIcon />,
    },
    {
      isCollapse: true,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.INTEGRATION,
        topTitle:
          LanguageReducer?.languageType?.INTEGRATION_API_INTEGRATION_TEXT,
        path: "/integration",
      },
      subTabData: allowPersonalCarrierContract
        ? integrationSubData
        : integrationSubData.filter((dt) => dt.path !== "/carriers"),
      otherRoutes: [
        // {
        //   topTitle:
        //     LanguageReducer?.languageType?.CREATE_FULFILLABLE_ORDER_TEXT,
        //   path: "/create-fulfillable-order",
        // },
      ],
      icon: <IntegrationIcon />,
    },
    // {
    //   isCollapse: true,
    //   tabData: {
    //     sideTitle: "CDP",
    //     topTitle: "CDP Customer",
    //     path: "/cdp-customer",
    //   },
    //   subTabData: [
    //     {
    //       sideTitle: "Customer",
    //       topTitle: "Customer",
    //       path: "/cdp-customer",
    //     },
    //   ],
    //   otherRoutes: [
    //     {
    //       topTitle: "Customer Profile",
    //       path: "/cdp-customer-profile",
    //     },
    //   ],
    //   icon: <IntegrationIcon />,
    // },
    {
      isCollapse: false,
      tabData: {
        sideTitle: LanguageReducer?.languageType?.APP_SETTING,
        topTitle: LanguageReducer?.languageType?.APP_SETTING,
        path: EnumRoutesUrls.APP_SETTINGS,
      },
      // subTabData: [
      //   {
      //     sideTitle:
      //       LanguageReducer?.languageType?.SETTING_DRIVER_STATUSES_TEXT,
      //     topTitle:
      //       LanguageReducer?.languageType?.SETTING_DRIVER_STATUSES_TEXT_S,
      //     path: "/driver-statuses",
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.ACCOUNTS_RETURN_REASON_TEXT,
      //     topTitle: LanguageReducer?.languageType?.ACCOUNTS_RETURN_REASON_TEXT,
      //     path: "/return-reason",
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_TEXT,
      //     topTitle: LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_TEXT,
      //     path: "/shipment-tab",
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.SETTING_PERMISSION_TEXT,
      //     topTitle: LanguageReducer?.languageType?.SETTING_PERMISSION_TEXT,
      //     path: EnumRoutesUrls.PERRMISSIONS,
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.SETTING_META_FIELD_TEXT,
      //     topTitle: LanguageReducer?.languageType?.SETTING_META_FIELD_TEXT,
      //     path: EnumRoutesUrls.META_FIELDS,
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.SETTING_CHOOSE_ROLES_TEXT,
      //     topTitle: LanguageReducer?.languageType?.SETTING_CHOOSE_ROLES_TEXT,
      //     path: EnumRoutesUrls.USER_ROLES,
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.NOTIFICATION,
      //     topTitle: LanguageReducer?.languageType?.NOTIFICATION,
      //     path: EnumRoutesUrls.Notification,
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.SETTING_ORDER_BOX,
      //     topTitle: LanguageReducer?.languageType?.SETTING_ORDER_BOX,
      //     path: EnumRoutesUrls.ORDER_BOX,
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.SETTING_WEBHOOK_EVENT,
      //     topTitle: LanguageReducer?.languageType?.SETTING_WEBHOOK_EVENT,
      //     path: EnumRoutesUrls.WEBHOOK_EVENTS,
      //   },
      //   {
      //     sideTitle: LanguageReducer?.languageType?.PAYOUT_BANK,
      //     topTitle: LanguageReducer?.languageType?.PAYOUT_BANK,
      //     path: EnumRoutesUrls.PAYOUT_BANK,
      //   },
      // ],
      otherRoutes: [
        ...getSubTabData(LanguageReducer),
        // {
        //   topTitle:
        //     LanguageReducer?.languageType?.CREATE_FULFILLABLE_ORDER_TEXT,
        //   path: "/create-fulfillable-order",
        // },
      ],
      icon: <SettingIcon />,
    },
  ];

  const dynamicApiNavBarMenu = React.useMemo(() => {
    return transformApiMenuToNavBarMenu(
      menuPermissionsData,
      LanguageReducer,
      navBarMenu
    );
  }, [menuPermissionsData, LanguageReducer, navBarMenu]);

  const effectiveNavBarMenu = menuPermissionsState.hasFetched
    ? dynamicApiNavBarMenu
    : [];

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleDrawerOpenPc = () => {
    setPcOpen(true);
  };

  React.useEffect(() => {
    if (matches) {
      setPcOpen(false);
    }
  }, [matches]);

  React.useEffect(() => {
    const fetchDraftCount = async () => {
      const count = await handleGetDraftOrdersCount();
      dispatch(changeBadgeData({ "/draft-order-dashboard": count || 0 }));
    };
    fetchDraftCount();
  }, [dispatch]);

  const handleDrawerClosePc = () => {
    setPcOpen(false);
  };

  const adminAuth = {
    isLoading: false,
    isPermitted: true,
  }; /* useSelector((state) => state.adminAuth); */
  const container =
    window !== undefined ? () => window().document.body : undefined;
  let access_token = getThisKeyCookie("access_token");
  const languageDirection =
    languagesOptions.find((lang) => lang.name === localStorage.language)?.dir ||
    "ltr";

  const clientSubscriptionData = useClientSubscriptionReducer();
  const isPlanNavbarShow =
    clientSubscriptionData.isTrialMode ||
    clientSubscriptionData?.data?.isSubscriptionCancel;
  if (access_token) {
    return (
      <Box
        sx={{
          ...styleSheet["@global"],
          display: "flex",
          flexDirection: languageDirection === "ltr" ? "row" : "row-reverse",
        }}
      >
        <CssBaseline />
        <AppBar
          position="fixed"
          color="default"
          enableColorOnDark
          evaluation={0}
          sx={{
            width: { sm: `calc(100% - ${pcOpen ? drawerWidth : 60}px)` },
            ml: {
              sm:
                languageDirection === "ltr"
                  ? `${pcOpen ? drawerWidth : 60}px`
                  : "",
            },
            boxShadow: "none",
            zIndex: 5,
            right: languageDirection === "ltr" ? 0 : "",
            left: languageDirection === "ltr" ? "" : 0,
          }}
        >
          <Toolbar sx={{ p: "0px !important" }}>
            <IconButton
              color="inherit"
              aria-label="open drawer"
              edge="start"
              onClick={handleDrawerToggle}
              sx={{ ml: 0.5, pr: 0, display: { sm: "none" } }}
            >
              <MenuIcon />
            </IconButton>
            <TopNavBar
              navBarMenu={
                isUserProfileSideBarShow
                  ? userProfileSideBarMenu
                  : effectiveNavBarMenu
              }
            />
          </Toolbar>
        </AppBar>

        <Box
          component="nav"
          sx={{
            width: { sm: pcOpen ? drawerWidth : 60 },
            flexShrink: { sm: 0 },
          }}
          aria-label="mailbox folders"
        >
          {/* <SideNavBar /> */}
          <Drawer
            anchor={languageDirection === "ltr" ? "left" : "right"}
            container={container}
            variant="temporary"
            open={mobileOpen}
            onClose={handleDrawerToggle}
            ModalProps={{
              keepMounted: true, // Better open performance on mobile.
            }}
            sx={{
              display: { xs: "block", sm: "none" },
              "& .MuiDrawer-paper": {
                boxSizing: "border-box",
                overflow: "visible",
                width: drawerWidth,
                zIndex: "1000",
                background: "#EEEEEE",
              },
            }}
          >
            <DrawerHeader>
              <Toolbar
                sx={{
                  justifyContent: "flex-start !important",
                  paddingLeft: "4px !important",
                }}
              >
                <img
                  style={{
                    width: "auto",
                    maxWidth: "180px",
                    height: "60px",
                    maxHeight: "60px",
                    objectFit: "contain",
                    marginTop: "10px",
                  }}
                  src={(!Branding?.logoUrl || Branding?.logoUrl === "/logo.svg") ? "/logo.svg" : Branding?.logoUrl}
                  alt="Nav logo"
                />
              </Toolbar>
              <Divider />
            </DrawerHeader>
            <SideNavBar
              mobileOpen={mobileOpen}
              navBarMenu={
                isUserProfileSideBarShow
                  ? userProfileSideBarMenu
                  : effectiveNavBarMenu
              }
            />
          </Drawer>
          <Drawer
            anchor={languageDirection === "ltr" ? "left" : "right"}
            variant="permanent"
            open={pcOpen}
            sx={{
              display: { xs: "none", sm: "block" },
              "& .MuiDrawer-paper": {
                boxSizing: "border-box",
                width: pcOpen ? drawerWidth : 60,
                boxShadow: "0px 3px 3px -2px rgba(0, 0, 0, 0.2)",
                filter:
                  "drop-shadow(0px 3px 4px rgba(0, 0, 0, 0.14)) drop-shadow(0px 1px 8px rgba(0, 0, 0, 0.12))",
                borderRight: "none",
                zIndex: "1000",
                overflow: "visible",
              },
            }}
          >
            <DrawerHeader
              style={{
                paddingRight: "17px",
                background: "#EEEEEE",
                height: "100px",
                justifyContent: "start",
              }}
            >
              <Toolbar
                style={{
                  background: "#EEEEEE",
                  padding: 0,
                }}
              >
                {pcOpen || width < 600 ? (
                  <img
                    style={{
                      width: "auto",
                      maxWidth: "180px",
                      height: "60px",
                      maxHeight: "60px",
                      objectFit: "contain",
                      marginRight: "-10px",
                    }}
                    src={(!Branding?.logoUrl || Branding?.logoUrl === "/logo.svg") ? "/logo.svg" : Branding?.logoUrl}
                    alt="Nav logo"
                  />
                ) : (
                  <img
                    onClick={handleDrawerOpenPc}
                    style={{
                      width: "35px",
                      height: "35px",
                      objectFit: "contain",
                      cursor: "pointer",
                      marginLeft: "5px",
                    }}
                    src={(!Branding?.logoUrl || Branding?.logoUrl === "/logo.svg") ? "/logo.svg" : Branding?.logoUrl}
                    alt="Nav logo"
                  />
                )}
              </Toolbar>
              {pcOpen ? (
                <IconButton
                  sx={{
                    background: "white",
                    padding: "0px",
                    border: "1px solid white",
                    position: "absolute",
                    right: languageDirection === "ltr" ? "-11px" : "240px",
                    zIndex: 1,
                    top: isPlanNavbarShow ? 74 : 47,
                    "&:hover": {
                      border: `1px solid ${purple}`,
                      background: "rgb(86, 58, 213, .2)",
                    },
                  }}
                  onClick={handleDrawerClosePc}
                >
                  {languageDirection === "rtl" ? (
                    <ChevronRightIcon />
                  ) : (
                    <ChevronLeftIcon
                      sx={{
                        fontSize: "22px",
                        "&:hover": {
                          color: purple,
                        },
                      }}
                    />
                  )}
                </IconButton>
              ) : (
                <IconButton
                  sx={{
                    background: "white",
                    padding: "0px",
                    border: "1px solid white",
                    position: "absolute",
                    zIndex: 1,
                    top: isPlanNavbarShow ? 74 : 47,
                    right: languageDirection === "ltr" ? "-13px" : "46px",
                    "&:hover": {
                      border: `1px solid ${purple}`,
                      background: "rgb(86, 58, 213, .2)",
                    },
                  }}
                  onClick={handleDrawerOpenPc}
                >
                  <ChevronRightIcon
                    sx={{
                      fontSize: "19px",
                      "&:hover": {
                        color: purple,
                      },
                    }}
                  />
                </IconButton>
              )}
            </DrawerHeader>
            <SideNavBar
              pcOpen={pcOpen}
              navBarMenu={
                isUserProfileSideBarShow
                  ? userProfileSideBarMenu
                  : effectiveNavBarMenu
              }
            />
          </Drawer>
        </Box>
        <Box
          component="main"
          sx={{
            flexGrow: 1,
            position: "relative",
            width: { xs: "100%", sm: `calc(100% - ${drawerWidth}px)` },
            backgroundColor: "white",
            padding: "0 0 !important",
            paddingLeft: "0px",
            paddingRight: "0px",
            marginTop: isPlanNavbarShow ? "100px" : "64px",
          }}
        >
          {adminAuth.isLoading || !menuPermissionsState.hasFetched ? (
            <Box
              sx={{
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                minHeight: "80vh",
                width: "100%",
                backgroundColor: "#ffffff",
              }}
            >
              <MainLoader />
            </Box>
          ) : menuPermissionsState.hasFetched && menuPermissionsData.length === 0 ? (
            <UnauthorizedPage
              title="Unauthorized Access"
              message="You are unauthorized to access any menu items. Please contact your system administrator."
            />
          ) : adminAuth.isPermitted ? (
            children
          ) : (
            <UnauthorizedPage />
          )}
        </Box>

        <Backdrop
          sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
          open={adminAuth.isLoading}
        >
          <MainLoader />
        </Backdrop>
      </Box>
    );
  } else {
    return <Navigate to={`/login`} />;
  }
}

PrivateRoutes.propTypes = {
  /**
   * Injected by the documentation to work in an iframe.
   * You won't need it on your project.
   */
  window: PropTypes.func,
};

export default PrivateRoutes;
