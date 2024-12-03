
using MediatR;

namespace SchoolProject.Buisness.Commands
{
    public class DeleteStudentCommand:IRequest
    {
        public int Id { get; set; }
    }
}