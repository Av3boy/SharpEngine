import {
  EmbeddedCheckoutProvider,
  EmbeddedCheckout
} from '@stripe/react-stripe-js';
import {loadStripe} from '@stripe/stripe-js';
import { useCallback } from 'react';
import { useErrorEvents } from 'sharpengine-ui-shared';

const stripePromise = loadStripe("pk_test_51SbRswBuRPp1Tqbb8OFYmrC8FCKfh0DzRhcyKzkWWX0hlH62Zxid6XGmVPp8jfRDnw1GA6nAhxOSLNbshvDcXs8k009GJAtLJA");

export default function CheckoutPage() {
    const { publish } = useErrorEvents();

    const fetchClientSecret = useCallback(async () => {
        try {
            const res = await fetch("https://localhost:7215/api/v1/checkout", { method: "POST" });

            if (!res.ok) {
                const text = await res.text().catch(() => "");
                throw new Error(`Checkout request failed (${res.status}): ${text || res.statusText}`);
            }

            const data = await res.json();
            if (!data || !data.clientSecret) {
                throw new Error("Missing clientSecret in response");
            }

            return data.clientSecret as string;
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : "Unexpected error creating checkout";
            publish({ message, severity: 'error', source: 'CheckoutPage' });
            // Re-throw so Stripe knows the fetch failed
            throw err;
        }
    }, []);

    const options = { fetchClientSecret };

    return (
        <div id="checkout">
            <EmbeddedCheckoutProvider stripe={stripePromise} options={options}>
                <EmbeddedCheckout />
            </EmbeddedCheckoutProvider>
        </div>
    );
}