
using MediatR;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Commands
{
    public class AddStudentCommand:IRequest<Student>
    {
        public Student Student { get; set; }

        public AddStudentCommand(Student student)
        {
            this.Student = student;
        }
    }
}