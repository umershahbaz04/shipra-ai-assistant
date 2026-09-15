import AllInclusiveIcon from "@mui/icons-material/AllInclusive";
import PaymentIcon from "@mui/icons-material/Payment";
import { Avatar, Box, Divider, Skeleton, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { modalTypes } from "..";
import RoundedButtonComp from "../../../../.reUseableComponents/Buttons/RoundedButtonComp";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import {
  CancelClientSubscription,
  CreateSubscription,
  StripeGetAllProducts,
} from "../../../../api/AxiosInterceptors";
import Colors from "../../../../utilities/helpers/Colors";
import {
  fetchMethod,
  handleGetClientSubscription,
  purple,
  TextLoader,
  useClientSubscriptionReducer,
} from "../../../../utilities/helpers/Helpers";
import UpgradeCancelEndTrialConfirmationModal from "./UpgradeCancelEndTrialConfirmation";
import { useSelector } from "react-redux";

const SubscriptionPlanCard = ({
  title,
  description,
  src,
  btns,
  productPriceDetails,
  loading,
  isActive,
}) => {
  return (
    <Box
      sx={{
        p: { xs: 1, md: 2 },
        bgcolor: "#fff",
        boxShadow: "0px 3px 5px rgba(0, 0, 0, 0.04)",
        border: "1px solid #e0e0e0",
        borderRadius: { xs: "5px", md: "10px" },
        width: "100%",
        height: "100%",
        maxWidth: 400,
        display: "flex",
        flexDirection: "column",
        gap: 2,
        position: "relative",
      }}
    >
      {isActive && (
        <Box
          sx={{
            position: "absolute",
            top: 3,
            right: 7,
            backgroundColor: purple,
            color: "white",
            padding: "8px 16px",
            borderRadius: "0 0 0 8px",
            fontSize: "0.75rem",
            fontWeight: "bold",
            transform: "translate(10%, -10%)",
            zIndex: 1,
            borderTopRightRadius: { xs: "5px", md: "10px" },
          }}
        >
          Active
        </Box>
      )}

      {loading ? (
        <Skeleton
          variant="rounded"
          sx={{ width: 100, height: 100, mx: "auto" }}
        />
      ) : (
        <Box sx={{ maxWidth: 100, mx: "auto" }}>
          <Avatar
            variant="rounded"
            src={src}
            sx={{ width: "100%", height: 100 }}
          >
            <PaymentIcon sx={{ width: "100%", height: 100 }} />
          </Avatar>
        </Box>
      )}
      <Typography variant="h2" sx={{ textAlign: "center" }}>
        {loading ? (
          <Box sx={{ maxWidth: 100, mx: "auto" }}>
            <TextLoader rowsNum={1} columnsNum={1} height={20} />
          </Box>
        ) : (
          title
        )}
      </Typography>
      <Typography variant="h6" sx={{ minHeight: 140 }}>
        {loading ? (
          <TextLoader rowsNum={3} columnsNum={1} height={20} />
        ) : (
          `${title} - ${description}`
        )}
      </Typography>
      <Divider />

      {loading ? (
        <Box>
          <Box>
            <TextLoader rowsNum={1} columnsNum={1} height={20} />
          </Box>
          <Box>
            <TextLoader rowsNum={1} columnsNum={1} height={20} />
          </Box>
        </Box>
      ) : (
        <Box sx={{ minHeight: 40 }}>
          {productPriceDetails.map((order_dt, order_dt_ind) => (
            <Box className={"flex_between"}>
              <Box sx={{ display: "flex", gap: 2, minWidth: 147 }}>
                <Typography variant="h6">{order_dt_ind + 1}.</Typography>
                <Typography
                  variant="h6"
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    gap: 0.5,
                  }}
                >
                  No. of Orders {order_dt?.firstUnit} -
                  <>
                    {order_dt?.lastUnit || (
                      <AllInclusiveIcon sx={{ fontSize: 16 }} />
                    )}
                  </>
                </Typography>
              </Box>
              <Typography variant="h6" sx={{ color: "rgba(0, 0, 0, 0.6)" }}>
                ${order_dt?.perUnitPrice} each
              </Typography>
              <Typography
                variant="h6"
                sx={{ color: "rgba(0, 0, 0, 0.6)", minWidth: 63 }}
              >
                {order_dt?.flatFee > 0 && `Flat fee ${order_dt?.flatFee}`}
              </Typography>
            </Box>
          ))}
        </Box>
      )}
      {btns}
    </Box>
  );
};
export default function UpgradeOrCancelPlanModal(props) {
  const { open, onClose, type, handleGetClientSubscriptionWithLoading } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const isUpgradeType = type === modalTypes.UPGRADE;
  const dispatch = useDispatch();
  const clientSubscriptionData = useClientSubscriptionReducer();
  const [plans, setPlans] = useState({
    loading: true,
    data: [],
  });
  const [subscribePlan, setSubscribePlan] = useState({
    selectedPlan: null,
    open: false,
    loading: false,
  });
  const [cancelPlan, setCancelPlan] = useState({
    open: false,
    loading: false,
  });

  const handleCancelPlanClose = () =>
    setCancelPlan((prev) => ({ ...prev, open: false }));
  const handleSubscribePlanClose = () =>
    setSubscribePlan((prev) => ({ ...prev, open: false }));
  const handleGetAllPlans = async () => {
    const { response } = await fetchMethod(StripeGetAllProducts, setPlans);
    if (response?.isSuccess) {
      const sortedData = response?.result?.sort(
        (a, b) =>
          a?.stripeProduct?.displayOrderId - b?.stripeProduct?.displayOrderId
      );
      setPlans((prev) => ({ ...prev, data: sortedData || [] }));
    }
  };

  const handleSubscribePlan = async (ProductId) => {
    const { response } = await fetchMethod(
      () => CreateSubscription(ProductId),
      setSubscribePlan
    );
    if (response?.isSuccess) {
      setSubscribePlan((prev) => ({ ...prev, loading: true }));
      setTimeout(async () => {
        await handleGetClientSubscriptionWithLoading();
        setSubscribePlan((prev) => ({ ...prev, loading: true }));
        handleSubscribePlanClose();
        onClose();
      }, 5000);
    }
  };
  const handleCancelClientSubscription = async (
    StripeSubscriptionId,
    feedback
  ) => {
    const { response } = await fetchMethod(
      () =>
        CancelClientSubscription(
          StripeSubscriptionId,
          feedback?.selectedReason?.value,
          feedback?.comment
        ),
      setCancelPlan
    );
    if (response?.isSuccess) {
      setCancelPlan((prev) => ({ ...prev, loading: true }));
      setTimeout(async () => {
        await handleGetClientSubscriptionWithLoading();
        setCancelPlan((prev) => ({ ...prev, loading: true }));
        handleCancelPlanClose();
        onClose();
      }, 5000);
    }
  };
  useEffect(() => {
    handleGetAllPlans();
  }, []);
  return (
    <>
      <ModalComponent
        maxWidth={isUpgradeType ? "xl" : "md"}
        title={
          isUpgradeType
            ? LanguageReducer?.languageType
                ?.SETTINGS_BILLING_HISTORY_SUBSCRIBE_PLAN
            : LanguageReducer?.languageType
                ?.SETTINGS_BILLING_HISTORY_ACTIVE_PLAN
        }
        open={open}
        onClose={onClose}
      >
        <Box className={"flex_center"} sx={{ gap: 2, flexWrap: "wrap" }}>
          {plans.loading
            ? (clientSubscriptionData?.data?.isSubscriptionCancel
                ? [...Array(3)]
                : isUpgradeType
                ? [...Array(2)]
                : [...Array(1)]
              ).map((_, ind) => (
                <SubscriptionPlanCard loading={true} key={ind} />
              ))
            : (clientSubscriptionData?.data?.isSubscriptionCancel
                ? [...plans.data]
                : isUpgradeType
                ? [...plans.data]
                : [
                    ...plans.data.filter(
                      (dt) =>
                        clientSubscriptionData?.data.shipraProductId ===
                        dt?.stripeProduct.productId
                    ),
                  ]
              ).map((dt, ind) => {
                const isPlanSubscribed =
                  !clientSubscriptionData?.data?.isSubscriptionCancel &&
                  clientSubscriptionData?.data.shipraProductId ===
                    dt?.stripeProduct.productId;
                const productDetails = dt?.stripeProduct || {};
                const productPriceDetails = dt?.productPriceList || [];
                const hasProductPriceList = productPriceDetails.length > 0;
                return (
                  <SubscriptionPlanCard
                    key={ind}
                    title={productDetails?.productName}
                    description={productDetails?.productDescription}
                    src={productDetails?.stripeImageURL}
                    productPriceDetails={productPriceDetails}
                    isActive={isPlanSubscribed}
                    btns={
                      type !== modalTypes.ACTIVE && (
                        <Box className={"flex_between"} gap={1}>
                          {isPlanSubscribed && (
                            <Box width={{ xs: "100%", md: "50%" }} mx={"auto"}>
                              <RoundedButtonComp
                                title={
                                  LanguageReducer?.languageType
                                    ?.SETTINGS_BILLING_HISTORY_CHANGE_PLAN
                                }
                                bg={Colors.danger}
                                onClick={() =>
                                  setCancelPlan((prev) => ({
                                    ...prev,
                                    open: true,
                                  }))
                                }
                              />
                            </Box>
                          )}
                          {!isPlanSubscribed &&
                            (hasProductPriceList ? (
                              <Box
                                width={{ xs: "100%", md: "50%" }}
                                mx={"auto"}
                              >
                                <RoundedButtonComp
                                  title={
                                    LanguageReducer?.languageType
                                      ?.SETTINGS_BILLING_HISTORY_SUBSCRIBE
                                  }
                                  onClick={() =>
                                    setSubscribePlan((prev) => ({
                                      ...prev,
                                      open: true,
                                      selectedPlan: dt.stripeProduct?.productId,
                                    }))
                                  }
                                />
                              </Box>
                            ) : (
                              <Box
                                width={{ xs: "100%", md: "50%" }}
                                mx={"auto"}
                              >
                                <RoundedButtonComp
                                  title={
                                    LanguageReducer?.languageType
                                      ?.SETTINGS_BILLING_HISTORY_CONTACT_US
                                  }
                                />
                              </Box>
                            ))}
                        </Box>
                      )
                    }
                  />
                );
              })}
        </Box>
      </ModalComponent>
      <UpgradeCancelEndTrialConfirmationModal
        open={subscribePlan.open}
        onClose={() => setSubscribePlan((prev) => ({ ...prev, open: false }))}
        loading={subscribePlan.loading}
        onConfirm={() => handleSubscribePlan(subscribePlan.selectedPlan)}
        heading={"Are you sure to subscribe this plan?"}
        message={
          "This action will make effect on your prev subscription and mark as cancel to subscribe this current plan."
        }
        buttonText={"Subscribe"}
        buttonColor={purple}
      />
      <UpgradeCancelEndTrialConfirmationModal
        open={cancelPlan.open}
        onClose={handleCancelPlanClose}
        loading={cancelPlan.loading}
        onConfirm={(feedback) =>
          handleCancelClientSubscription(
            clientSubscriptionData?.data.subscriptionId,
            feedback
          )
        }
        heading={"Are you sure cancel your subscription?"}
        message={"This action will make effect your app usage policy.."}
        buttonText={"Cancel Subscription"}
        buttonColor={purple}
        hasFeedback={true}
      />
    </>
  );
}
