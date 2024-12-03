using Newtonsoft.Json;
using Plain.RabbitMQ;
using SchoolApi.Core.Business.SharedModels;
using SchoolProject.Buisness.Services;
using SchoolProject.Api.Constants;

namespace SchoolProject.Api.Listener
{
    public class StudentEventEmailListener : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IServiceScopeFactory _scopeFactory;

        public StudentEventEmailListener(ISubscriber subscriber, IServiceScopeFactory scopeFactory)
        {
            _subscriber = subscriber;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            using var scope = _scopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                var studentEventMessage = JsonConvert.DeserializeObject<StudentEventMessage>(message);

                if (studentEventMessage == null)
                {
                    return false;
                }

                string subject = "Student Event Notification";
                string body = string.Empty;

                switch (studentEventMessage.EventType)
                {
                    case RabbitMQConstant.EventTypeCreated:
                        subject = "Welcome to Our School!";
                        body = $"Dear {studentEventMessage.StudentName},\n\n" +
                               $"Welcome to our school! Your student ID is {studentEventMessage.StudentId}.\n\n" +
                               $"Best Regards,\nSchool Team";
                        break;

                    case RabbitMQConstant.EventTypeUpdated:
                        subject = "Your Student Information Was Updated";
                        body = $"Dear {studentEventMessage.StudentName},\n\n" +
                               $"Your student information has been updated. Your student ID is {studentEventMessage.StudentId}.\n\n" +
                               $"Best Regards,\nSchool Team";
                        break;

                    case RabbitMQConstant.EventTypeDeleted:
                        subject = "Your Student Profile Was Deleted";
                        body = $"Dear {studentEventMessage.StudentName},\n\n" +
                               $"We're sorry to inform you that your student profile has been deleted. Your student ID was {studentEventMessage.StudentId}.\n\n" +
                               $"Best Regards,\nSchool Team";
                        break;
                }

                emailService.SendEmail(studentEventMessage.StudentEmail, subject, body);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
