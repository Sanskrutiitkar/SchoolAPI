
using Newtonsoft.Json;
using Plain.RabbitMQ;
using SchoolApi.Core.Business.SharedModels;
using SchoolProject.Buisness.Services;


namespace SchoolProject.Api.Listener
{
    public class PaymentEventListener : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentEventListener(ISubscriber subscriber, IServiceScopeFactory scopeFactory)
        {
            _subscriber = subscriber;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(SubscribePaymentEvent);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool SubscribePaymentEvent(string message, IDictionary<string, object> headers)
        {
            using var scope = _scopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                var paymentEvent = JsonConvert.DeserializeObject<PaymentEventMessage>(message);

                if (paymentEvent == null)
                {
                    return false;
                }

                if (paymentEvent.PaymentSucceeded)
                {
                    SendPaymentSucessEmail(emailService, paymentEvent);
                }
                else
                {
                    SendPaymentFailureEmail(emailService, paymentEvent);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SendPaymentFailureEmail(IEmailService emailService, PaymentEventMessage paymentEvent)
        {        
                string subject = "Payment Failed for Your Course Registration";
                string body = $"Dear Student,\n\n" +
                              $"Unfortunately, your payment for the course has failed.\n\n" +
                              $"Please check your payment details and try again.\n\n" +
                              $"Best regards,\nSchool Team";

                emailService.SendEmail(paymentEvent.StudentEmail, subject, body);
        }
         private void SendPaymentSucessEmail(IEmailService emailService, PaymentEventMessage paymentEvent)
        {        
                string subject = "Payment Sucessful for Your Course Registration";
                string body = $"Dear Student,\n\n" +
                              $"Uour payment for the course is sucessful.\n\n" +
                              $"Please check your payment details.\n\n" +
                              $"Best regards,\nSchool Team";

                emailService.SendEmail(paymentEvent.StudentEmail, subject, body);
        }
    }
}
