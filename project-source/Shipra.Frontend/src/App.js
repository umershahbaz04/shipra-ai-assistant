import { Box } from "@mui/material";
import React, { useEffect, useLayoutEffect, useRef } from "react";
import { useDispatch } from "react-redux";
import { Route, Routes, useNavigate } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import LoadingBar from "react-top-loading-bar";
import { Axios, GetBrandingById } from "./api/AxiosInterceptors.js";
import PrivateRoutes from "./components/authRoutes";
import LoginSignupRoutes from "./components/loginSignupRoutes";
import NotFound from "./components/notFound";
import NotFound403 from "./components/notFound/NotFound403.js";
import Shopify from "./pages/Integrations/Shopify";
import ApiIntegration from "./pages/Integrations/apiIntegration";
import CarriersPage from "./pages/Integrations/carriers";
import PaymentIntegrationPage from "./pages/Integrations/paymentIntegration";
import PlatFormIntegrationPage from "./pages/Integrations/platformIntegrations";
import SmsIntegrationPage from "./pages/Integrations/smsIntegration";
import ProfilePage from "./pages/Profile/Profile/Profile.js";
import PasswordPage from "./pages/Profile/Security/Password.js";
import CODCollections from "./pages/accounts/codCollections";
import CODPending from "./pages/accounts/codPending";
import Expense from "./pages/accounts/expense";
import Profit from "./pages/accounts/profit";
import MaintenancePage from "./pages/additional/Maintenance.js";
import AnalyticsPage from "./pages/analytics";
import DashboardsPage from "./pages/dashboards";
import SaleDashboardPage from "./pages/dashboards/saleDashboard";
import CarrierDashboardPage from "./pages/dashboards/carrierDashboard";
import ReturnCarrierDashboardPage from "./pages/carrierReturn";
import UploadCarrierReturnReport from "./pages/carrierReturn/uploadReturnReport";
import CarrierSettlementPage from "./pages/carrierSettlement";
import ForgotPasswordPage from "./pages/loginSignup/fotgotPassword";
import LoginPage from "./pages/loginSignup/loginScreen";
import OtpScreen from "./pages/loginSignup/otpScreen";
import ResetPasswordPage from "./pages/loginSignup/resetPassword";
import SignUpPage from "./pages/loginSignup/signUpScreen";
import CODCollectionPending from "./pages/myCarriers/codCollectionPending";
import MyTaskCODCollections from "./pages/myCarriers/codCollections";
import DeliveryNotes from "./pages/myCarriers/deliveryNotes";
import DeliveryTasks from "./pages/myCarriers/deliveryTasks";
import Leads from "./pages/leads";
import Contacts from "./pages/contacts"; // force reload
import UploadLeads from "./pages/leads/uploadLeads";
import DriverExpensePage from "./pages/myCarriers/driverExpense";
import DriverReciveablePage from "./pages/myCarriers/driverRecievable";
import PendingForReturnPage from "./pages/myCarriers/pendingForReturn";
import ReturnsPage from "./pages/myCarriers/returns";
import OrderPage from "./pages/orders";
import CODWallet from "./pages/orders/CODWallet/index.js";
import CreateFulFillableOrderPage from "./pages/orders/createFulFillableOrder";
import CreateRegularOrderPage from "./pages/orders/createRegularOrder";
import EditFulFillableOrderPage from "./pages/orders/editFulFillableOrder";
import EditRegularOrderPage from "./pages/orders/editRegularOrder";
import PaymentLink from "./pages/orders/paymentLink/index.js";
import PriceCalculator from "./pages/orders/priceCalculator2/index.js";
import ReturnOrderPage from "./pages/orders/returnOrders/index.js";
import SaleChannelOrderPage from "./pages/orders/saleChannelOrder";
import UploadOrders from "./pages/orders/uploadOrders";
import PrincingPlanPage from "./pages/pricingPlan/PricingPlan.js";
import Inventory from "./pages/products/inventory";
import InventorySalePage from "./pages/products/inventorySale";
import LowInventoryPage from "./pages/products/lowInventory/index.js";
import ProductStationPage from "./pages/products/productStation";
import Products from "./pages/products/products";
import AddProducts from "./pages/products/products/addProducts";
import EditProducts from "./pages/products/products/editProducts";
import SaleChannelProductPage from "./pages/products/saleChannelProduct";
import SyncPoliciesPage from "./pages/products/syncPolicies";
import AccountsSettingPage from "./pages/settings/account";
import BillingHistory from "./pages/settings/billingHistory/index.js";
import TaxPage from "./pages/settings/clientTax";
import DocumentSettingsPage from "./pages/settings/documentSettings";
import DriverStatusPage from "./pages/settings/driverStatus";
import CarrierStatusPage from "./pages/settings/carrierStatus";
import MetaFieldsPage from "./pages/settings/metaFields/MetaFields.js";
import NotificationPage from "./pages/settings/notification/index.js";
import OrderBoxPage from "./pages/settings/orderBox/index.js";
import ZoneNamePage from "./pages/settings/zoneName/index.js";
import PayoutBank from "./pages/settings/payoutBank/index.js";
import PermissionsPage from "./pages/settings/permissions/Permissions.js";
import ReturnReasonPage from "./pages/settings/returnReason/index.js";
import SettingPage from "./pages/settings/settings/index.js";
import ShipmentTabs from "./pages/settings/shipmentTabs";
import LeadStatusPage from "./pages/settings/leadStatus";
import UserRolePage from "./pages/settings/userRoles";
import UsersPage from "./pages/settings/users";
import WebhookEventsPage from "./pages/settings/webhookEvents/index.js";
import ShipmentPage from "./pages/shipments";
import StorePage from "./pages/store";
import {
  EnumApiUrls,
  EnumNonCancellableApiUrls,
  EnumRoutesUrls,
} from "./utilities/enum";
import {
  handleDispatIsApiCallingFlag,
  handleGetAccessTokenByRefreshToken,
  purple,
  setupTabLogout,
  useFilterModelReducer,
  useHistory,
} from "./utilities/helpers/Helpers.js";
import WalletPage from "./pages/orders/CODWallet/index.js";
import UploadProduct from "./pages/products/products/uploadProduct/index.js";
import { useSelector } from "react-redux";
import GenericSettingPage from "./pages/settings/genericSetting/index.js";
import OrderArchivePage from "./pages/settings/OrdersArchive/index.js";
import OrderLables from "./pages/orders/orderLabels/index.js";
import Branding from "./pages/settings/Branding/index.js";
import UnauthorizedPage from "./components/unAuthorised/index.js";
import {
  getThisKeyCookie,
  setThisKeyCookie,
} from "./utilities/cookies/index.js";
import { getBranding } from "./utilities/helpers/HelpersFilter.js";
import StatusReport from "./pages/shipments/statusReport/index.js";
import WhatsappIntegration from "./pages/Integrations/whatsappIntegration/index.js";
import PickUpLocation from "./pages/shipments/pickUpLocation/index.js";
import DraftOrder from "./pages/orders/draftOrders/index.js";
import DraftRegularOrder from "./pages/orders/draftOrders/draftRegularOrder.js";
import DraftFullfillableOrder from "./pages/orders/draftOrders/draftFullfillableOrder.js";
import ServiceRateGroup from "./pages/Contract/ServiceRateGroup/index.js";
import GenerateInvoice from "./pages/Contract/GenerateInvoice/index.js";
import Invoice from "./pages/Contract/Invoice/index.js";
import ShipperRates from "./pages/Contract/ShipperRates/index.js";
import PerformanceReport from "./pages/PerformanceReport/index.js";
import InvoiceAdjustment from "./pages/Contract/InvoiceAdjustment/index.js";
import UpdatePermission from "./pages/settings/updatePermission/index.js";
import UploadStore from "./pages/store/uploadStore/index.js";
import CDPCustomer from "./pages/CDP/Customer/index.js";
import CustomerProfileInfo from "./pages/CDP/Profile/index.js";
import FactoryReset from "./pages/settings/factoryReset/index.js";

function App() {
  const ref = useRef();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPath, previousPath, currentControllers, previousControllers } =
    useHistory();
  useLayoutEffect(() => {
    document.title = `${window.location.host}${window.location.pathname}`;
  }, [currentPath]);
  const filterModelReducer = useFilterModelReducer();
  const filterModelRef = useRef(filterModelReducer);

  useEffect(() => {
    filterModelRef.current = filterModelReducer;
  }, [filterModelReducer]);

  useEffect(() => {
    let url = "";
    const requestInterceptor = Axios.interceptors.request.use((config) => {
      url = config.url;
      if (EnumApiUrls[url]?.changeFilterModel) {
        let filterModel = config.data?.filterModel;
        const hasfilterModelRefData = filterModelRef.current[url];
        if (filterModel && hasfilterModelRefData) {
          filterModel.start = filterModelRef.current[url].start;
          filterModel.length = filterModelRef.current[url].length;
          config.data.filterModel = filterModel;
        }
      }
      if (EnumApiUrls[url]?.showLoading) {
        handleDispatIsApiCallingFlag(dispatch, {
          apiName: url,
          loading: true,
          msg: EnumApiUrls[url]?.loadingText,
        });
      }
      ref.current?.continuousStart();
      if (!EnumNonCancellableApiUrls.includes(url)) {
        const controller = new AbortController();
        currentControllers.current.push(controller);
        config.signal = controller.signal;
      }
      return config;
    });

    const responseInterceptor = Axios.interceptors.response.use(
      (response) => {
        const url = response.config.url;
        ref.current?.complete();
        handleDispatIsApiCallingFlag(dispatch, {
          apiName: url,
          loading: false,
          msg: "",
        });
        return response;
      },
      (error) => {
        ref.current?.complete();
        handleDispatIsApiCallingFlag(dispatch, {
          apiName: url,
          loading: false,
          msg: "",
        });
        return Promise.reject(error);
      },
    );

    return () => {
      Axios.interceptors.request.eject(requestInterceptor);
      Axios.interceptors.response.eject(responseInterceptor);
    };
  }, []);

  useEffect(() => {
    if (previousPath === "/analytics") {
      if (previousControllers.current.length > 0) {
        previousControllers.current.forEach((controller) => controller.abort());
      }
    }
  }, [currentPath]);

  useEffect(() => {
    let timer;
    const token = getThisKeyCookie("access_token");
    if (token) {
      timer = setInterval(
        () => {
          handleGetAccessTokenByRefreshToken(navigate, LanguageReducer);
        },
        14 * 60 * 1000,
      ); // 14 minutes
    }

    return () => {
      if (timer) {
        clearInterval(timer);
      }
    };
  }, [currentPath, handleGetAccessTokenByRefreshToken]);

  useEffect(() => {
    getBranding();
  }, []);

  // useEffect(() => {
  //   const cleanup = setupTabLogout();
  //   return cleanup;
  // }, []);

  return (
    <Box>
      {/* <Box className={"flex_center active-row"} sx={{ px: 1, py: 2 }}>
        <Typography variant="h5">
          You are using shipra trial account.You trial period will be ended in
          next 20 days
        </Typography>
      </Box> */}
      <Routes>
        <Route
          path="/"
          element={
            <LoginSignupRoutes>
              <LoginPage />
            </LoginSignupRoutes>
          }
        />
        <Route
          path="/login"
          element={
            <LoginSignupRoutes>
              <LoginPage />
            </LoginSignupRoutes>
          }
        />
        <Route
          path="/signup"
          element={
            <LoginSignupRoutes>
              <SignUpPage />
            </LoginSignupRoutes>
          }
        />
        <Route
          path="/verify-otp"
          element={
            <LoginSignupRoutes>
              <OtpScreen />
            </LoginSignupRoutes>
          }
        />
        <Route
          path="/forgot-password"
          element={
            <LoginSignupRoutes>
              <ForgotPasswordPage />
            </LoginSignupRoutes>
          }
        />
        <Route
          path="/reset-password"
          element={
            <LoginSignupRoutes>
              <ResetPasswordPage />
            </LoginSignupRoutes>
          }
        />
        <Route
          path="/dashboards"
          element={
            <PrivateRoutes>
              <DashboardsPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/sale-dashboard"
          element={
            <PrivateRoutes>
              <SaleDashboardPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/carrier-dashboard"
          element={
            <PrivateRoutes>
              <CarrierDashboardPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/analytics"
          element={
            <PrivateRoutes>
              <AnalyticsPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/orders-dashboard/*"
          element={
            <PrivateRoutes>
              <OrderPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/draft-order-dashboard/*"
          element={
            <PrivateRoutes>
              <DraftOrder />
            </PrivateRoutes>
          }
        />
        <Route
          path="/return-order/*"
          element={
            <PrivateRoutes>
              <ReturnOrderPage />
            </PrivateRoutes>
          }
        />
        {/* <Route
          path="/price-calculator/*"
          element={
            <PrivateRoutes>
              <PriceCalculator />
            </PrivateRoutes>
          }
        /> */}
        <Route
          path="/price-calculator/*"
          element={
            <PrivateRoutes>
              <PriceCalculator />
            </PrivateRoutes>
          }
        />
        <Route
          path="/payment-link/*"
          element={
            <PrivateRoutes>
              <PaymentLink />
            </PrivateRoutes>
          }
        />
        <Route
          path="/order-lables/*"
          element={
            <PrivateRoutes>
              <OrderLables />
            </PrivateRoutes>
          }
        />
        <Route
          path="/wallet/*"
          element={
            <PrivateRoutes>
              <WalletPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/sale-channel-orders/*"
          element={
            <PrivateRoutes>
              <SaleChannelOrderPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="sale-channel-product"
          element={
            <PrivateRoutes>
              <SaleChannelProductPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/create-fulfillable-order"
          element={
            <PrivateRoutes>
              <CreateFulFillableOrderPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/create-regular-order"
          element={
            <PrivateRoutes>
              <CreateRegularOrderPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/draft-regular-order"
          element={
            <PrivateRoutes>
              <DraftRegularOrder />
            </PrivateRoutes>
          }
        />
        <Route
          path="/draft-fullfillable-order"
          element={
            <PrivateRoutes>
              <DraftFullfillableOrder />
            </PrivateRoutes>
          }
        />
        <Route
          path="/upload-orders"
          element={
            <PrivateRoutes>
              <UploadOrders />
            </PrivateRoutes>
          }
        />
        <Route
          path="/edit-order-regular/*"
          element={
            <PrivateRoutes>
              <EditRegularOrderPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/edit-order-fulfillable/*"
          element={
            <PrivateRoutes>
              <EditFulFillableOrderPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/store/*"
          element={
            <PrivateRoutes>
              <StorePage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/upload-stores"
          element={
            <PrivateRoutes>
              <UploadStore />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.SERIVCE_RATE_GROUP}
          element={
            <PrivateRoutes>
              <ServiceRateGroup />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.SHIPPER_RATES}
          element={
            <PrivateRoutes>
              <ShipperRates />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.PERFORMANCE_REPORT}
          element={
            <PrivateRoutes>
              <PerformanceReport />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.GENERATE_INVOICE}
          element={
            <PrivateRoutes>
              <GenerateInvoice />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.INVOICE}
          element={
            <PrivateRoutes>
              <Invoice />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.INVOICE_ADJUSTMENT}
          element={
            <PrivateRoutes>
              <InvoiceAdjustment />
            </PrivateRoutes>
          }
        />
        <Route
          path="/carrier-settlement/*"
          element={
            <PrivateRoutes>
              <CarrierSettlementPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/carriers/*"
          element={
            <PrivateRoutes>
              <CarriersPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/sale-channels/*"
          element={
            <PrivateRoutes>
              <PlatFormIntegrationPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/sms/*"
          element={
            <PrivateRoutes>
              <SmsIntegrationPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/whatsapp/*"
          element={
            <PrivateRoutes>
              <WhatsappIntegration />
            </PrivateRoutes>
          }
        />
        <Route
          path="/payment/*"
          element={
            <PrivateRoutes>
              <PaymentIntegrationPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/shopify"
          element={
            <PrivateRoutes>
              <Shopify />
            </PrivateRoutes>
          }
        />
        <Route
          path="/integration/*"
          element={
            <PrivateRoutes>
              <ApiIntegration />
            </PrivateRoutes>
          }
        />
        <Route
          path="/cdp-customer/*"
          element={
            <PrivateRoutes>
              <CDPCustomer />
            </PrivateRoutes>
          }
        />
        <Route
          path="/cdp-customer-profile"
          element={
            <PrivateRoutes>
              <CustomerProfileInfo />
            </PrivateRoutes>
          }
        />
        <Route
          path="/leads/*"
          element={
            <PrivateRoutes>
              <Leads />
            </PrivateRoutes>
          }
        />
        <Route
          path="/contacts/*"
          element={
            <PrivateRoutes>
              <Contacts />
            </PrivateRoutes>
          }
        />
        <Route
          path="/upload-leads"
          element={
            <PrivateRoutes>
              <UploadLeads />
            </PrivateRoutes>
          }
        />
        <Route
          path="/delivery-tasks/*"
          element={
            <PrivateRoutes>
              <DeliveryTasks />
            </PrivateRoutes>
          }
        />
        <Route
          path="/delivery-notes/*"
          element={
            <PrivateRoutes>
              <DeliveryNotes />
            </PrivateRoutes>
          }
        />
        <Route
          path="/shipments/*"
          element={
            <PrivateRoutes>
              <ShipmentPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.STATUS_REPORT}
          element={
            <PrivateRoutes>
              <StatusReport />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.PICKUP_LOCATION}
          element={
            <PrivateRoutes>
              <PickUpLocation />
            </PrivateRoutes>
          }
        />
        <Route
          path="/create-carrier-returns"
          element={
            <PrivateRoutes>
              <UploadCarrierReturnReport />
            </PrivateRoutes>
          }
        />
        {/* <Route
            path="/pending-carrier-returns"
            element={
              <PrivateRoutes>
                <PendingCarrierReturn />
              </PrivateRoutes>
            }
          /> */}
        <Route
          path="/carrier-returns"
          element={
            <PrivateRoutes>
              <ReturnCarrierDashboardPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/account/*"
          element={
            <PrivateRoutes>
              <AccountsSettingPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/profit/*"
          element={
            <PrivateRoutes>
              <Profit />
            </PrivateRoutes>
          }
        />
        <Route
          path="/cod-pending/*"
          element={
            <PrivateRoutes>
              <CODPending />
            </PrivateRoutes>
          }
        />
        <Route
          path="/accounts-cod-collections/*"
          element={
            <PrivateRoutes>
              <CODCollections />
            </PrivateRoutes>
          }
        />
        <Route
          path="/expense/*"
          element={
            <PrivateRoutes>
              <Expense />
            </PrivateRoutes>
          }
        />
        <Route
          path="/products/*"
          element={
            <PrivateRoutes>
              <Products />
            </PrivateRoutes>
          }
        />
        <Route
          path="/inventory/*"
          element={
            <PrivateRoutes>
              <Inventory />
            </PrivateRoutes>
          }
        />
        <Route
          path="/low-inventory/*"
          element={
            <PrivateRoutes>
              <LowInventoryPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/add-products/*"
          element={
            <PrivateRoutes>
              <AddProducts />
            </PrivateRoutes>
          }
        />
        <Route
          path="/upload-product/*"
          element={
            <PrivateRoutes>
              <UploadProduct />
            </PrivateRoutes>
          }
        />
        <Route
          path="/edit-products/*"
          element={
            <PrivateRoutes>
              <EditProducts />
            </PrivateRoutes>
          }
        />
        <Route
          path="/sync-policies/*"
          element={
            <PrivateRoutes>
              <SyncPoliciesPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/product-station/*"
          element={
            <PrivateRoutes>
              <ProductStationPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/inventory-sales/*"
          element={
            <PrivateRoutes>
              <InventorySalePage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/cod-collection-pending/*"
          element={
            <PrivateRoutes>
              <CODCollectionPending />
            </PrivateRoutes>
          }
        />
        <Route
          path="/cod-collections"
          element={
            <PrivateRoutes>
              <MyTaskCODCollections />
            </PrivateRoutes>
          }
        />
        <Route
          path="/driver-recievable"
          element={
            <PrivateRoutes>
              <DriverReciveablePage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/pending-for-return"
          element={
            <PrivateRoutes>
              <PendingForReturnPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/returns"
          element={
            <PrivateRoutes>
              <ReturnsPage />
            </PrivateRoutes>
          }
        />
        {/* <Route
            path="/drivers"
            element={
              <PrivateRoutes>
                <DriversPage />
              </PrivateRoutes>
            }
          /> */}
        <Route
          path="/driver-expenses"
          element={
            <PrivateRoutes>
              <DriverExpensePage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/users"
          element={
            <PrivateRoutes>
              <UsersPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/user-roles"
          element={
            <PrivateRoutes>
              <UserRolePage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/driver-status"
          element={
            <PrivateRoutes>
              <DriverStatusPage />
            </PrivateRoutes>
          }
        />
        <Route
          path="/return-reason"
          element={
            <PrivateRoutes>
              <ReturnReasonPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={"/tax"}
          element={
            <PrivateRoutes>
              <TaxPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.MIX_SETTINGS}
          element={
            <PrivateRoutes>
              <DocumentSettingsPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.APP_SETTINGS}
          element={
            <PrivateRoutes>
              <SettingPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.DRIVER_STATUSES}
          element={
            <PrivateRoutes>
              <DriverStatusPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.SHIPMENT_TAB}
          element={
            <PrivateRoutes>
              <ShipmentTabs />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.LEAD_STATUS}
          element={
            <PrivateRoutes>
              <LeadStatusPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.WEBHOOK_EVENTS_STARIC}
          element={
            <PrivateRoutes>
              <WebhookEventsPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.PROFILE}
          element={
            <PrivateRoutes>
              <ProfilePage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.PASSWORD}
          element={
            <PrivateRoutes>
              <PasswordPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.PERRMISSIONS}
          element={
            <PrivateRoutes>
              <PermissionsPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.UPDATE_PERRMISSION}
          element={
            <PrivateRoutes>
              <UpdatePermission />
            </PrivateRoutes>
          }
        />
        <Route
          path="*"
          element={
            <PrivateRoutes type="404">
              <NotFound />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.NOTFOUND_403}
          element={
            <PrivateRoutes type="403">
              <NotFound403 />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.META_FIELDS}
          element={
            <PrivateRoutes>
              <MetaFieldsPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.GENERIC_SETTING}
          element={
            <PrivateRoutes>
              <GenericSettingPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.ORDERS_ARCHIVE}
          element={
            <PrivateRoutes>
              <OrderArchivePage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.BRANDING}
          element={
            <PrivateRoutes>
              <Branding />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.FACTORY_RESET}
          element={
            <PrivateRoutes>
              <FactoryReset />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.Notification}
          element={
            <PrivateRoutes>
              <NotificationPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.ORDER_BOX}
          element={
            <PrivateRoutes>
              <OrderBoxPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.ZONE_NAME}
          element={
            <PrivateRoutes>
              <ZoneNamePage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.PAYOUT_BANK}
          element={
            <PrivateRoutes>
              <PayoutBank />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.BILLING}
          element={
            <PrivateRoutes>
              <BillingHistory />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.UNAUTHORIZED}
          element={
            <PrivateRoutes>
              <UnauthorizedPage />
            </PrivateRoutes>
          }
        />
        <Route
          path={EnumRoutesUrls.CARRIER_STATUS}
          element={
            <PrivateRoutes>
              <CarrierStatusPage />
            </PrivateRoutes>
          }
        />
        <Route path={EnumRoutesUrls.MAINTENACE} element={<MaintenancePage />} />
        <Route
          path={EnumRoutesUrls.PRICING_PLAN}
          element={
            <LoginSignupRoutes>
              <PrincingPlanPage />
            </LoginSignupRoutes>
          }
        />
      </Routes>
      <LoadingBar ref={ref} color={purple} height={4} shadow={false} />

      <ToastContainer />
    </Box>
  );
}

export default App;
