import { Button, CircularProgress } from "@mui/material";
import { CardElement, useElements, useStripe } from '@stripe/react-stripe-js';
import { useState } from 'react';
import { useSelector } from 'react-redux';
import { styleSheet } from "../../../../assets/styles/style.js";
// const stripePromise = loadStripe('pk_test_51ND00JDjLkXxy43Vsl6exRVHL4ePCLBk5zdps5I5r6GrgidiSQZ5KmYeu4dm21UCIhVp29oiarRFxWqRv79QTyyU00c9CfrKZJ');
const PaymentForm = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const stripe = useStripe();
  const elements = useElements();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();

    setLoading(true);

    if (stripe) {
      const { error, paymentMethod } = await stripe.createPaymentMethod({
        type: 'card',
        card: elements.getElement(CardElement),
      });

      setLoading(false);

      if (!error) {
        // Send the paymentMethod.id to your backend for further processing
        console.log(paymentMethod.id);
      } else {
        console.log(error.message);
      }
    }
  };


  const cardElementOptions = {
    style: {
      base: {
        fontSize: '20px',
        color: '#424770',
        '::placeholder': {
          color: '#aab7c4',
        },
        width: '100%',
        height: '40px',
        fontFamily: 'Lato, sans-serif',
      },
      invalid: {
        color: '#9e2146',
      },
    },
  };
  return (
    <form onSubmit={handleSubmit} style={{ width: '100%' }}>
      <div style={{ margin: '20px 20px 20px 20px' }}>
        <CardElement options={cardElementOptions} />
      </div>
      {loading ? (
        <Button sx={{ ...styleSheet.saveButtonUI, marginTop: "20px" }} variant="contained" fullWidth disabled>
          <CircularProgress sx={{ color: 'white' }} />
        </Button>
      ) : (
        <Button sx={{ ...styleSheet.saveButtonUI, marginTop: "20px" }} variant="contained" fullWidth type="submit">
          {LanguageReducer?.languageType?.APPLY_TEXT}
        </Button>
      )}

    </form>
  );
};

export default PaymentForm;
