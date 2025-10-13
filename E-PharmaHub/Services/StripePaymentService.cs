using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Stripe.Checkout;

namespace E_PharmaHub.Services
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public StripePaymentService(IConfiguration config , IUnitOfWork unitOfWork)
        {
            _config = config;
            Stripe.StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
            _unitOfWork = unitOfWork;
        }

        public async Task<StripeSessionResponseDto> CreateCheckoutSessionAsync(PaymentRequestDto dto)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new()
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = dto.Amount * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = dto.PaymentFor.ToString()
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",

                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    CaptureMethod = "manual",
                    Metadata = new Dictionary<string, string>
            {
                { "PaymentFor", dto.PaymentFor.ToString() },
                { "ClientReferenceId", dto.ReferenceId },
                { "UserId", dto.ReferenceId ?? "unknown" }
            }
                },

                SuccessUrl = "http://localhost:3000/payment-success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:3000/payment-cancel",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            session = await service.GetAsync(session.Id, new SessionGetOptions
            {
                Expand = new List<string> { "payment_intent" }
            });

            var paymentIntentId = session.PaymentIntentId ?? session.PaymentIntent?.ToString();

            var payment = new Payment
            {
                ReferenceId = dto.ReferenceId,
                PaymentFor = dto.PaymentFor,
                ProviderTransactionId = session.Id,
                PaymentIntentId = paymentIntentId,  
                Status = PaymentStatus.Pending,
                Amount = dto.Amount,
                PayerUserId = dto.ReferenceId
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            if (dto.PaymentFor == PaymentForType.Order && dto.OrderId.HasValue)
            {
                var order = await _unitOfWork.Order.GetByIdAsync(dto.OrderId.Value);
                if (order != null)
                {
                    order.PaymentId = payment.Id;
                    await _unitOfWork.CompleteAsync();
                }
            }

            return new StripeSessionResponseDto
            {
                CheckoutUrl = session.Url,
                SessionId = session.Id
            };
        }


    }
}
