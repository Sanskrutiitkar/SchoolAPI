

namespace SchoolProject.Api.Constants
{
    public class ExceptionMessages
    {
        public const string StudentNotFound= "Student with this id not found";
        public const string AlreadyInactive= "Student with this id is already inactive";
        public const string PaginationPageNumer = "Please enter a valid page number";
        public const string PaginationPageSize = "Please enter a valid page size";
        public const string DuplicateEntry = "This email is already used";
        public const string CourseNotFound = "Course not found";
        public const string CoursePaymentInitialized="Course assignment initiated, waiting for payment confirmation";
    }
}