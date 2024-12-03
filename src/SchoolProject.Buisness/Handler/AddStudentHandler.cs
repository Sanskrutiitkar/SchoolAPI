using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolProject.Buisness.Commands;
using SchoolProject.Buisness.Models;
using SchoolProject.Buisness.Repository;

namespace SchoolProject.Buisness.Handler
{
    public class AddStudentHandler:IRequestHandler<AddStudentCommand,Student>
    {
        private readonly IStudentRepo _studentRepo;

        public AddStudentHandler(IStudentRepo studentRepository)
        {
            _studentRepo = studentRepository;
        }

        public async Task<Student> Handle(AddStudentCommand request, CancellationToken cancellationToken)
        {
            return await _studentRepo.AddStudent(request.Student);
        }
    }


}