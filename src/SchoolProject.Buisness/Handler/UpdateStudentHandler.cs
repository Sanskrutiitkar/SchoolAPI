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
    public class UpdateStudentHandler:IRequestHandler<UpdateStudentCommand,Student>
    {
        private readonly IStudentRepo _studentRepo;

        public UpdateStudentHandler(IStudentRepo studentRepository)
        {
            _studentRepo = studentRepository;
        }

        public async Task<Student> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var existingStudent =  await _studentRepo.GetStudentById(request.id);

            existingStudent.FirstName = request.Student.FirstName ?? existingStudent.FirstName;
            existingStudent.LastName = request.Student.LastName ?? existingStudent.LastName;
            existingStudent.StudentEmail = request.Student.StudentEmail ?? existingStudent.StudentEmail;
            existingStudent.StudentPhone = request.Student.StudentPhone ?? existingStudent.StudentPhone;
            existingStudent.StudentGender = request.Student.StudentGender;
            existingStudent.BirthDate = request.Student.BirthDate ;
            

            return await _studentRepo.UpdateStudent(existingStudent);
        }
    }
}