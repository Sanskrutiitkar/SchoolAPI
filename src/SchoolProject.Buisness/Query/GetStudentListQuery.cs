
using MediatR;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Query
{
    public class GetStudentListQuery:IRequest<IEnumerable<Student>>
    {
        
    }
}