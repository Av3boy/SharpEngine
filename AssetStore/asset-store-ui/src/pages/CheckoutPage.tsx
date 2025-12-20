import {
  EmbeddedCheckoutProvider,
  EmbeddedCheckout
} from '@stripe/react-stripe-js';

export default function CheckoutPage() {

    const options = { "": "" };

    return (
        <div id="checkout">
            <EmbeddedCheckoutProvider stripe={stripePromise} options={options}>
            <EmbeddedCheckout />
            </EmbeddedCheckoutProvider>
        </div>
    );
}