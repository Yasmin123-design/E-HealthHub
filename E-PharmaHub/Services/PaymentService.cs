using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Stripe.Checkout;
using Stripe.Climate;

namespace E_PharmaHub.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDoctorService _doctorService;
        private readonly IPharmacistService _pharmacistService;
        //private readonly IOrderService _orderService;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IDoctorService doctorService,
            IPharmacistService pharmacistService
           )
        {
            _unitOfWork = unitOfWork;
            _doctorService = doctorService;
            _pharmacistService = pharmacistService;
            //_orderService = orderService;
        }

        public async Task<object> VerifySessionAsync(string sessionId)
        {
            var stripeService = new SessionService();
            var session = await stripeService.GetAsync(sessionId);

            if (session.PaymentStatus != "paid")
            {
                return new
                {
                    status = session.PaymentStatus,
                    sessionId,
                    message = "Payment not completed yet."
                };
            }

            var payment = await _unitOfWork.Payments.GetByProviderTransactionIdAsync(session.Id);
            if (payment != null && payment.Status == PaymentStatus.Paid)
            {
                return new
                {
                    status = session.PaymentStatus,
                    sessionId,
                    message = "Payment already processed."
                };
            }

            if (payment != null && payment.Status == PaymentStatus.Pending)
            {
                await _unitOfWork.Payments.MarkAsSuccess(session.Id);
                await _unitOfWork.CompleteAsync();
            }

            string clientRefId = session.Metadata.ContainsKey("ClientReferenceId")
                                 ? session.Metadata["ClientReferenceId"]
                                 : null;

            if (clientRefId != null && session.Metadata.TryGetValue("PaymentFor", out var paymentFor))
            {
                switch (paymentFor)
                {
                    case "DoctorRegistration":
                        await _doctorService.MarkAsPaid(clientRefId);
                        break;

                    case "PharmacistRegistration":
                        await _pharmacistService.MarkAsPaid(clientRefId);
                        break;

                        //case "Order":
                        //    await _orderService.MarkPaymentAsComplete(clientRefId);
                        //    break;
                }
            }

            return new
            {
                status = session.PaymentStatus,
                sessionId,
                message = "Payment verified successfully."
            };
        }

    }
}