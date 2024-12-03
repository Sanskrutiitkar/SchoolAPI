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
    public class GetStudentListHandler : IRequestHandler<GetStudentListQuery, IEnumerable<Student>>
    {
        private readonly IStudentRepo _studentRepo;
        public GetStudentListHandler(IStudentRepo studentRepo)
        {
            _studentRepo=studentRepo;
        }
        public async Task<IEnumerable<Student>> Handle(GetStudentListQuery request, CancellationToken cancellationToken)
        {
            return await _studentRepo.GetAllStudents();
        }
    }
}