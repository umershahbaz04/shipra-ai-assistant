import AccountBalanceOutlinedIcon from "@mui/icons-material/AccountBalanceOutlined";
import AdminPanelSettingsIcon from "@mui/icons-material/AdminPanelSettings";
import AttributionRoundedIcon from "@mui/icons-material/AttributionRounded";
import SourceIcon from "@mui/icons-material/Source";
import EventIcon from "@mui/icons-material/Event";
import AssignmentReturnIcon from "@mui/icons-material/AssignmentReturn";
import Inventory2OutlinedIcon from "@mui/icons-material/Inventory2Outlined";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import ManageAccountsIcon from "@mui/icons-material/ManageAccounts";
import NotificationsNoneOutlinedIcon from "@mui/icons-material/NotificationsNoneOutlined";
import PersonOutlinedIcon from "@mui/icons-material/PersonOutlined";
import SettingsSuggestIcon from "@mui/icons-material/SettingsSuggest";
import TextFieldsIcon from "@mui/icons-material/TextFields";
import {
  Box,
  Card,
  CardContent,
  Grid,
  Tooltip,
  Typography,
} from "@mui/material";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { EnumRoutesUrls } from "../../../utilities/enum";
import { greyBorder } from "../../../utilities/helpers/Helpers";
import ArchiveIcon from "@mui/icons-material/Archive";
import InfoIcon from "@mui/icons-material/Info";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import AddModeratorIcon from "@mui/icons-material/AddModerator";
import LabelIcon from "@mui/icons-material/Label";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import MapIcon from "@mui/icons-material/Map";

export const getSubTabData = (LanguageReducer) => {
  return [
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_DRIVER_STATUSES_TEXT,
      topTitle: LanguageReducer?.languageType?.SETTING_DRIVER_STATUSES_TEXT_S,
      icon: <AttributionRoundedIcon style={{ fontSize: 40 }} />,
      path: "/driver-statuses",
    },
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_RETURN_REASON_TEXT,
      topTitle: LanguageReducer?.languageType?.SETTING_RETURN_REASON_TEXT,
      icon: <AssignmentReturnIcon style={{ fontSize: 40 }} />,
      path: "/return-reason",
    },
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_TEXT,
      topTitle: LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_TEXT,
      icon: <LocalShippingIcon style={{ fontSize: 40 }} />,
      path: "/shipment-tab",
    },
    {
      sideTitle: "Lead Status",
      topTitle: "Lead Status",
      icon: <LabelIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.LEAD_STATUS,
    },
    {
      sideTitle: "Carrier Statuses",
      topTitle: "Carrier Statuses",
      icon: <LocalShippingIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.CARRIER_STATUS,
    },
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_PERMISSION_TEXT,
      topTitle: LanguageReducer?.languageType?.SETTING_PERMISSION_TEXT,
      icon: <AdminPanelSettingsIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.PERRMISSIONS,
    },
    // {
    //   sideTitle: "Update Permission",
    //   topTitle: "Update Permission",
    //   icon: <AddModeratorIcon style={{ fontSize: 40 }} />,
    //   path: EnumRoutesUrls.UPDATE_PERRMISSION,
    // },
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_META_FIELD_TEXT,
      topTitle: LanguageReducer?.languageType?.SETTING_META_FIELD_TEXT,
      icon: <TextFieldsIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.META_FIELDS,
    },
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_CHOOSE_ROLES_TEXT,
      topTitle: LanguageReducer?.languageType?.SETTING_CHOOSE_ROLES_TEXT,
      icon: <PersonOutlinedIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.USER_ROLES,
    },
    {
      sideTitle: LanguageReducer?.languageType?.NOTIFICATION,
      topTitle: LanguageReducer?.languageType?.NOTIFICATION,
      icon: <NotificationsNoneOutlinedIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.Notification,
    },
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_ORDER_BOX,
      topTitle: LanguageReducer?.languageType?.SETTING_ORDER_BOX,
      icon: <Inventory2OutlinedIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.ORDER_BOX,
    },
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_WEBHOOK_EVENT,
      topTitle: LanguageReducer?.languageType?.SETTING_WEBHOOK_EVENT,
      icon: <EventIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.WEBHOOK_EVENTS,
    },
    {
      sideTitle: LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK,
      topTitle: LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK,
      icon: <AccountBalanceOutlinedIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.PAYOUT_BANK,
    },
    {
      sideTitle: LanguageReducer?.languageType?.DOCUMENT_SETTINGS,
      topTitle: LanguageReducer?.languageType?.DOCUMENT_SETTINGS,
      icon: <SettingsSuggestIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.MIX_SETTINGS,
    },
    {
      sideTitle: "Generic Setting",
      topTitle: "Generic Setting",
      icon: <ManageAccountsIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.GENERIC_SETTING,
    },
    {
      sideTitle: "Archive Orders",
      topTitle: "Archive Orders",
      icon: <ArchiveIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.ORDERS_ARCHIVE,
    },
    {
      sideTitle: "Invoice Module",
      topTitle: "Invoice Module",
      icon: <InfoIcon style={{ fontSize: 40 }} />,
      showWarning: true,
      subText:
        "To activate this feature, please contact the support team.",
    },
    {
      sideTitle: "Factory Reset",
      topTitle: "Factory Reset",
      icon: <DeleteForeverIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.FACTORY_RESET,
    },
    {
      sideTitle: LanguageReducer?.languageType?.SETTING_ZONE || "Zone",
      topTitle: LanguageReducer?.languageType?.SETTING_ZONE || "Zone",
      icon: <MapIcon style={{ fontSize: 40 }} />,
      path: EnumRoutesUrls.ZONE_NAME,
    },
    // {
    //   sideTitle: "Branding",
    //   topTitle: "Branding",
    //   icon: <SourceIcon style={{ fontSize: 40 }} />,
    //   path: EnumRoutesUrls.BRANDING,
    // },
  ];
};
const SettingPage = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const navigate = useNavigate();

  return (
    <Box p={1.5}>
      <Grid container spacing={2}>
        {getSubTabData(LanguageReducer).map((data, index) => (
          <Grid item xs={12} sm={6} md={3} key={index}>
            <Card
              sx={{
                bgcolor: "#f5f5f5",
                border: greyBorder,
                boxShadow: "none",
                borderRadius: 2,
                cursor: "pointer",
                "&:hover": {
                  boxShadow: 3,
                },
              }}
              onClick={() => navigate(data.path)}
            >
              <CardContent sx={{ padding: "16px !important" }}>
                <Box
                  display="flex"
                  alignItems="center"
                  justifyContent="space-between"
                >
                  <Box display="flex" alignItems="center" gap={1.5}>
                    {data.icon}

                    <Typography variant="body2" color="textSecondary">
                      {data.sideTitle}
                    </Typography>
                  </Box>

                  {data.showWarning && (
                    <Tooltip title={data.subText} arrow>
                      <ErrorOutlineIcon
                        color="warning"
                        fontSize="small"
                        sx={{ cursor: "pointer" }}
                      />
                    </Tooltip>
                  )}
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};

export default SettingPage;
