using Microsoft.AspNetCore.Mvc;
using AssetStoreRoutes = SharpEngine.Rest.Urls.AssetStore;

using Stripe.Checkout;

namespace AssetStore.Api.v1.Controllers;

/// <summary>
///     Represents a REST API controller for handling assets.
/// </summary>
[ApiController]
[Route(AssetStoreRoutes.ChecoutRoute)]
public class CheckoutController : Controller
{
    [HttpPost]
    public ActionResult CreateCheckoutSession()
    {
        var domain = "http://localhost:3000"; // Asset store ui url
        var options = new SessionCreateOptions
        {
            UiMode = "embedded",
            LineItems =
            [
                new() {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = 2000,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Asset Store Sample Asset",
                        },
                    },
                  Quantity = 1,
                },
            ],
            Mode = "payment",
            ReturnUrl = domain + "/checkout?success=true",
        };

        var service = new SessionService();
        Session session = service.Create(options);

        return Json(new { clientSecret = session.ClientSecret });
    }
}

[Route("session-status")]
[ApiController]
public class SessionStatusController : Controller
{
    [HttpGet]
    public ActionResult SessionStatus([FromQuery] string session_id)
    {
        var sessionService = new SessionService();
        Session session = sessionService.Get(session_id);

        return Json(new { status = session.Status, customer_email = session.CustomerDetails.Email });
    }
}
