using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolProject.Buisness.Commands;
using SchoolProject.Buisness.Repository;

namespace SchoolProject.Buisness.Handler
{
    public class DeleteStudentHandler :IRequestHandler<DeleteStudentCommand>
    {
        private readonly IStudentRepo _studentRepo;

        public DeleteStudentHandler(IStudentRepo studentRepository)
        {
            _studentRepo = studentRepository;
        }

        public async Task<Unit> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
             await _studentRepo.DeleteStudent(request.Id);
             return Unit.Value;
        }


    }
}