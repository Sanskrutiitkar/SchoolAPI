using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Core.Business.SharedModels
{
    public class StudentCourseMessage
    {
    public string EventType { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentEmail { get; set; }
    public List<int> CourseIds { get; set; } = new List<int>();
    }
}