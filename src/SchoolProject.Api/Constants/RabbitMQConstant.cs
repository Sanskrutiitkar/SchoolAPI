using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Api.Constants
{
    public class RabbitMQConstant
    {
        public const string ConnectionString="amqp://guest:guest@localhost:5672";
        public const string EventTypeCreated = "created";
        public const string EventTypeUpdated = "updated";
        public const string EventTypeDeleted = "deleted";
        public const string EventTypeCourseAssigned ="course_assigned";
        public const string KeyCourseAssigned = "course.assigned";
        public const string KeyStudentDeleted ="student.deleted";
        public const string KeyStudentUpdated ="student.updated";
        public const string KeyStudentCreated ="student.created";
        public const string KeyPaymentSucess ="payment.succeeded";
        public const string KeyPaymentFail="payment.failed";
        public const string StudentExchange ="student_exchange";
        public const string StudentEventQueue ="student_event_queue";
        public const string PaymentExchange ="payment_exchange";
        public const string PaymentEventQueue="payment_event_queue";
        public const string CourseExchange = "course_exchange";
        public const string CourseEventQueue ="course_event_queue";
        public const string StudentKey ="student.*";
        public const string PaymentKey = "payment.*";

    
    }
}