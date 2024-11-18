using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using SchoolProject.Buisness.Models;

namespace SchoolProjectTest.Api
{
    public static class FakeStudentGenerator
    {
         public static Student GenerateFakeStudent()
        {
            var faker = new Faker<Student>()
                .RuleFor(s => s.StudentId, f => f.IndexFaker + 1)  
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())  
                .RuleFor(s => s.LastName, f => f.Name.LastName())    
                .RuleFor(s => s.StudentEmail, f => f.Internet.Email())  
               .RuleFor(s => s.StudentPhone, f =>
               {
                   var firstDigit = f.Random.Int(7, 9);
                   var remainingDigits = f.Random.Number(100000000, 999999999);
                   return $"{firstDigit}{remainingDigits}";
               })
                .RuleFor(s => s.BirthDate, f => f.Date.Past(20, DateTime.Now.AddYears(-18)))
                .RuleFor(s => s.StudentGender, f => f.PickRandom<Gender>())
                .RuleFor(s => s.StudentAge, (f, s) => DateTime.Now.Year - s.BirthDate.Year);

            return faker.Generate();  
        }
    }
}