import {
  Box,
  Button,
  Divider,
  Grid,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import StepperComp, {
  fetchMethod,
  purple,
  useStepper,
} from "../../../utilities/helpers/Helpers";
import {
  AddUpdateWallet,
  PrepareTotalProcessingCheckout,
} from "../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import Colors from "../../../utilities/helpers/Colors";
import { PaymentWidget } from "./PaymentWidget";
const steps = ["Select or Enter Amonut", "Select Payment Method And Continue"];
const amounts = [100, 500, 1000, 2000, 4000];

const ChargeAccountModal = (props) => {
  const { open, onClose } = props;
  const { activeStep, showBackBtn, showNextBtn, handleBack, handleNext } =
    useStepper(steps);
  const [selectedAmount, setSelectedAmount] = useState(0);
  const [isCustom, setIsCustom] = useState(false);
  const [loading, setLoading] = useState(false);
  const [checkoutAmountData, setCheckoutAmountData] = useState({});

  const handleAmountSelect = (amount) => {
    setSelectedAmount(amount);
    setIsCustom(false);
  };

  const handleCustomSelect = () => {
    setSelectedAmount(0);
    setIsCustom(true);
  };

  const handleCustomAmountChange = (e) => {
    setSelectedAmount(e.target.value ? parseFloat(e.target.value) : 0);
  };

  const handlePrepareTotalProcessingCheckout = async () => {
    const { response } = await fetchMethod(
      () => PrepareTotalProcessingCheckout(selectedAmount),
      setLoading,
      false
    );
    setCheckoutAmountData(response.result);
  };

  const handleNextStep = async () => {
    if (activeStep === 0) {
      if (selectedAmount <= 0) {
        errorNotification("Invalid amount!");
        return;
      }
      await handlePrepareTotalProcessingCheckout();
    } else if (activeStep === 1) {
    }
    handleNext();
  };

  useEffect(() => {
    const handleBeforeUnload = (event) => {
      event.preventDefault();
      event.returnValue = "this is";
      return event.returnValue; // Required for modern browsers to show the confirmation dialog.
    };

    window.addEventListener("beforeunload", handleBeforeUnload);
    if (!showNextBtn) {
      window.removeEventListener("beforeunload", handleBeforeUnload);
    }
    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
    };
  }, [showNextBtn]);

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title="Charge Account"
      actionBtn={
        <>
          {showBackBtn && (
            <ModalButtonComponent
              title="Back"
              bg={Colors.danger}
              loading={loading}
              onClick={handleBack}
              showBorderBottomRightRadius={false}
            />
          )}
          {showNextBtn && (
            <ModalButtonComponent
              title="Next"
              bg={purple}
              loading={loading}
              onClick={handleNextStep}
            />
          )}
        </>
      }
    >
      <StepperComp steps={steps} activeStep={activeStep} />
      {activeStep === 0 && (
        <Box p={3} display="flex" flexDirection={{ xs: "column", md: "row" }}>
          {/* Left Section - Select Amount */}
          <Box flex={1} mr={{ md: 2 }}>
            <Typography variant="h6" fontWeight="bold">
              Select Amount
            </Typography>
            <Typography color="textSecondary" variant="body2">
              Please select your desired charge amount or enter it manually.
            </Typography>

            {/* Current Credit Display */}
            <Box mt={2} textAlign="center">
              <TextField
                fullWidth
                value={selectedAmount}
                onChange={handleCustomAmountChange}
                type="number"
                disabled={!isCustom}
                placeholder="0"
                InputProps={{
                  disableUnderline: true,
                  readOnly: !isCustom,
                  style: {
                    fontSize: "2rem",
                    textAlign: "center",
                    cursor: isCustom ? "text" : "default",
                    padding: 0,
                    backgroundColor: "transparent",
                    color: "inherit",
                  },
                }}
                sx={{
                  "& .MuiInputBase-input": {
                    textAlign: "center",
                  },
                  "& .MuiOutlinedInput-root": {
                    "& fieldset": {
                      borderColor: isCustom ? "white" : "transparent",
                    },
                    "&:hover fieldset": {
                      borderColor: "transparent",
                    },
                    "&.Mui-focused fieldset": {
                      borderColor: isCustom ? "white" : "transparent",
                    },
                    "&.Mui-disabled .MuiOutlinedInput-notchedOutline": {
                      borderColor: "transparent",
                    },
                  },
                }}
              />
              <Typography color="textSecondary">
                Current credit: {selectedAmount} AED
              </Typography>
            </Box>

            <Divider sx={{ my: 2 }}>Or</Divider>

            {/* Amount Options */}
            <Grid container spacing={2}>
              {amounts.map((amount) => (
                <Grid item xs={4} sm={4} md={4} key={amount}>
                  <Button
                    variant="outlined"
                    fullWidth
                    onClick={() => handleAmountSelect(amount)}
                    sx={{
                      backgroundColor:
                        selectedAmount === amount && !isCustom
                          ? purple
                          : "inherit",
                      color:
                        selectedAmount === amount && !isCustom
                          ? "white"
                          : "inherit",
                      "&:hover": {
                        backgroundColor:
                          selectedAmount === amount && !isCustom
                            ? purple
                            : "inherit",
                      },
                    }}
                  >
                    {amount}
                  </Button>
                </Grid>
              ))}
              <Grid item xs={4} sm={4} md={4}>
                <Button
                  variant="outlined"
                  fullWidth
                  onClick={handleCustomSelect}
                  sx={{
                    backgroundColor: isCustom ? purple : "inherit",
                    color: isCustom ? "white" : "inherit",
                    "&:hover": {
                      backgroundColor: isCustom ? purple : "inherit",
                    },
                  }}
                >
                  Custom
                </Button>
              </Grid>
            </Grid>
          </Box>

          {/* Right Section - Order Summary */}
          <Box flex={1} mt={{ xs: 4, md: 0 }}>
            <Typography variant="h6" fontWeight="bold">
              Order Summary
            </Typography>
            <Box display="flex" justifyContent="space-between" mt={2}>
              <Typography>Subtotal</Typography>
              <Typography>{selectedAmount} AED</Typography>
            </Box>
            <Box display="flex" justifyContent="space-between" mt={1}>
              <Typography>VAT Amount</Typography>
              <Typography>0 AED</Typography>
            </Box>
            <Divider sx={{ my: 2 }} />

            <Box
              display="flex"
              justifyContent="space-between"
              fontWeight="bold"
            >
              <Typography variant="body1">Total</Typography>
              <Typography variant="body1">{selectedAmount} AED</Typography>
            </Box>

            {/* COD Wallet Switch */}
            <Box display="flex" alignItems="center" mt={2}>
              <Switch color="primary" />
              <Box ml={1}>
                <Typography variant="body2">COD Wallet</Typography>
                <Typography variant="caption" color="textSecondary">
                  Available balance: 0 AED
                </Typography>
              </Box>
            </Box>

            {/* Promo Code */}
            <Typography variant="body2" mt={2}>
              Promo Code (optional)
            </Typography>
            <Box display="flex" alignItems="center" mt={1}>
              <TextField
                variant="outlined"
                size="small"
                placeholder="Promo Code"
                fullWidth
              />
              <Button
                variant="contained"
                color="primary"
                sx={{ ml: 1, textTransform: "capitalize" }}
              >
                Apply
              </Button>
            </Box>
          </Box>
        </Box>
      )}
      {activeStep === 1 &&
        checkoutAmountData?.id &&
        checkoutAmountData?.integrity && (
          <Box p={3}>
            <PaymentWidget
              checkoutId={checkoutAmountData?.id}
              integrity={checkoutAmountData?.integrity}
            />
          </Box>
        )}
    </ModalComponent>
  );
};

export default ChargeAccountModal;
