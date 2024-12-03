
using Newtonsoft.Json;
using Plain.RabbitMQ;
using SchoolApi.Core.Business.SharedModels;
using SchoolProject.Api.Constants;
using SchoolProject.Buisness.Repository;


namespace SchoolProject.Api.Listener
{
    public class CourseEventListener : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IPublisher _publisher;
        private readonly IServiceScopeFactory _scopeFactory;

        public CourseEventListener(ISubscriber subscriber, IPublisher publisher, IServiceScopeFactory scopeFactory)
        {
            _subscriber = subscriber;
            _publisher = publisher;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(SubscribeCourseAssigned);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool SubscribeCourseAssigned(string message, IDictionary<string, object> headers)
        {
            using var scope = _scopeFactory.CreateScope();
            var studentRepo = scope.ServiceProvider.GetRequiredService<IStudentRepo>();
            var courseRepo = scope.ServiceProvider.GetRequiredService<ICourseRepo>();

            try
            {
                var studentCourseAssignedEvent = JsonConvert.DeserializeObject<StudentCourseMessage>(message);
                if (studentCourseAssignedEvent == null) return false;

                var student = studentRepo.GetStudentById(studentCourseAssignedEvent.StudentId).Result;
                var course = courseRepo.GetCourseById(studentCourseAssignedEvent.CourseIds.FirstOrDefault()).Result;

                if (student == null || course == null) return false;

                if (!student.Courses.Any(c => c.CourseId == course.CourseId))
                {
                    student.Courses.Add(course);
                    studentRepo.UpdateStudent(student).Wait();
                }

                var paymentSucceeded = SimulatePaymentProcessing(course.CourseId, student.StudentId);

                var paymentEventMessage = new PaymentEventMessage
                {
                    CourseId = course.CourseId,
                    StudentId = student.StudentId,
                    StudentEmail = student.StudentEmail,
                    PaymentSucceeded = paymentSucceeded
                };

                var paymentEventMessageJson = JsonConvert.SerializeObject(paymentEventMessage);

                if (paymentSucceeded)
                {
                    _publisher.Publish(paymentEventMessageJson, RabbitMQConstant.KeyPaymentSucess , null);
                }
                else
                {
                    _publisher.Publish(paymentEventMessageJson, RabbitMQConstant.KeyPaymentFail, null);
                    student.Courses.Remove(course);
                    studentRepo.UpdateStudent(student).Wait();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool SimulatePaymentProcessing(int courseId, int studentId)
        {
            return new Random().Next(0, 2) == 1;
        }
    }
}
