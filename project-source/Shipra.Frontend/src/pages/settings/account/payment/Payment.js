import React from "react";

import { Box, Card, Typography } from "@mui/material";
import { Elements } from "@stripe/react-stripe-js";
import { loadStripe } from "@stripe/stripe-js";
import { styleSheet } from "../../../../assets/styles/style.js";
import PaymentForm from "./CheckoutForm";


const stripePromise = loadStripe('pk_test_51ND00JDjLkXxy43Vsl6exRVHL4ePCLBk5zdps5I5r6GrgidiSQZ5KmYeu4dm21UCIhVp29oiarRFxWqRv79QTyyU00c9CfrKZJ');
function Payment() {
    return (
        <>
            <Box sx={styleSheet.paymentSettingArea}>
                <Typography sx={styleSheet.paymentMethodHeading} variant="h5">
                    Payment Method
                </Typography>
                <Typography sx={styleSheet.paymentMethodText}>Add a credit card to get started with Shop Easy.</Typography>
                <Card variant="outlined" sx={styleSheet.paymentMethodCardArea}>
                    <Elements stripe={stripePromise}>
                        <PaymentForm />
                    </Elements>
                </Card>
            </Box>
        </>
    );
}

export default Payment;