import { Box } from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { CicrlesLoading } from "../../../utilities/helpers/Helpers";

export const PaymentWidget = ({ checkoutId, integrity }) => {
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    window.wpwlOptions = {
      googlePay: {
        buttonType: "pay",
      },
    };
    const script = document.createElement("script");
    script.src = `https://eu-test.oppwa.com/v1/paymentWidgets.js?checkoutId=${checkoutId}`;
    script.integrity = integrity || "";
    script.crossOrigin = "anonymous";
    script.async = true;

    script.onload = () => setLoading(false);
    script.onerror = () => {
      setLoading(false);
      console.error("Failed to load the payment widget script.");
    };

    document.body.appendChild(script);
    return () => {
      document.body.removeChild(script);
    };
  }, [checkoutId, integrity]);

  return (
    <Box
      sx={{
        " #gpay-button-online-api-id": {
          width: "100%",
        },
        " .wpwl-form": {
          maxWidth: "40em",
        },
        " .wpwl-control": {
          height: "34px",
        },
      }}
    >
      {loading ? (
        <CicrlesLoading height="100%" />
      ) : (
        <form
          action={`${window.origin}/wallet`}
          className="paymentWidgets"
          data-brands="VISA MASTER AMEX GOOGLEPAY APPLEPAY"
        ></form>
      )}
    </Box>
  );
};
