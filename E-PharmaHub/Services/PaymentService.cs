using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Stripe.Checkout;

namespace E_PharmaHub.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;


        public PaymentService(
            IUnitOfWork unitOfWork
           )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Payment> GetByReferenceIdAsync(string referenceId)
        {
            return await _unitOfWork.Payments.GetByReferenceIdAsync(referenceId);
        }

        public async Task<object> VerifySessionAsync(string sessionId)
        {
            var stripeService = new SessionService();
            var session = await stripeService.GetAsync(sessionId, new SessionGetOptions
            {
                Expand = new List<string> { "payment_intent" }
            });

            var paymentIntentId = session.PaymentIntentId ?? session.PaymentIntent?.ToString();
            var paymentIntentService = new Stripe.PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

            var payment = await _unitOfWork.Payments.GetByProviderTransactionIdAsync(session.Id);
            if (payment == null)
            {
                return new
                {
                    status = "not_found",
                    sessionId,
                    message = "Payment record not found for this session."
                };
            }

            string intentStatus = paymentIntent.Status;
            string message;

            switch (intentStatus)
            {
                case "requires_capture":
                    payment.Status = PaymentStatus.Captured;
                    message = "Payment authorized successfully (awaiting pharmacist approval).";

                    var order = await _unitOfWork.Order.GetByPaymentIdEntityAsync(payment.Id);
                    if (order != null)
                    {
                        order.PaymentStatus = PaymentStatus.Captured;

                        foreach (var item in order.Items)
                        {
                            var inventoryItem = await _unitOfWork.IinventoryItem
                                .GetByPharmacyAndMedicationAsync( order.PharmacyId,item.MedicationId);

                            if (inventoryItem != null)
                            {
                                inventoryItem.Quantity -= item.Quantity;
                                if (inventoryItem.Quantity < 0)
                                    inventoryItem.Quantity = 0;

                                await _unitOfWork.IinventoryItem.Update(inventoryItem);
                            }
                        }

                        await _unitOfWork.CompleteAsync();
                    }
                    break;

                case "succeeded":
                    payment.Status = PaymentStatus.Paid; 
                    message = "Payment captured successfully.";
                    break;

                case "requires_payment_method":
                case "requires_confirmation":
                    payment.Status = PaymentStatus.Pending;
                    message = "Payment not completed yet.";
                    break;

                case "canceled":
                    payment.Status = PaymentStatus.Failed;
                    message = "Payment was canceled.";
                    break;

                default:
                    payment.Status = PaymentStatus.Pending;
                    message = $"Unhandled payment status: {intentStatus}.";
                    break;
            }

            payment.PaymentIntentId = paymentIntentId;
            await _unitOfWork.CompleteAsync();

            string clientRefId = session.Metadata.ContainsKey("ClientReferenceId")
                                 ? session.Metadata["ClientReferenceId"]
                                 : null;

            return new
            {
                status = intentStatus,
                sessionId,
                paymentIntentId,
                message
            };
        }
        public async Task DeletePaymentAsync(Payment model)
        {
                _unitOfWork.Payments.Delete(model);
                await _unitOfWork.CompleteAsync();
            
        }


    }
}