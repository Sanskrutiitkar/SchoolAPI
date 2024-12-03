
using MediatR;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Query
{
    public class GetStudentByIdQuery: IRequest<Student>
    {
        public int Id {get; set;}
    }
}