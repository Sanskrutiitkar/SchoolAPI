using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Core.Business.SharedModels
{
    public class PaymentEventMessage
    {
    public int CourseId { get; set; }
    public int StudentId { get; set; } 
    public string StudentEmail {get; set;}
    public bool PaymentSucceeded { get; set; }
    }
}