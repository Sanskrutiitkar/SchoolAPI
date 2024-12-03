using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolProject.Buisness.Models;
using SchoolProject.Buisness.Query;
using SchoolProject.Buisness.Repository;

namespace SchoolProject.Buisness.Handler
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, Student>
    {
        private readonly IStudentRepo _studentRepo;
        public GetStudentByIdHandler(IStudentRepo studentRepo)
        {
            _studentRepo=studentRepo;
        }

        public async Task<Student> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _studentRepo.GetStudentById(request.Id);
        }



    }

}
