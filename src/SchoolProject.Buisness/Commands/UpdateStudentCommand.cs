
using MediatR;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Commands
{
    public class UpdateStudentCommand:IRequest<Student>
    {
        public int id {get; set;}
        public Student Student { get; set; }

        public UpdateStudentCommand(int id, Student student)
        {
            this.id = id;
            this.Student = student;
        }
    }
}