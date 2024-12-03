
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using SchoolApi.Core.Business.GlobalException;
using SchoolApi.Core.Business.SharedModels;
using SchoolProject.Api.Constants;
using SchoolProject.Api.DTOs;
using SchoolProject.Buisness.Commands;
using SchoolProject.Buisness.Models;
using SchoolProject.Buisness.Query;
using SchoolProject.Buisness.Repository;
using SchoolProject.Buisness.Services;

namespace SchoolProject.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentCQRSController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly IStudentRepo _studentRepo;
        private readonly Plain.RabbitMQ.IPublisher _publisher;
        private readonly IMediator _mediator;
        public StudentCQRSController(IStudentService studentService, IMapper mapper, IStudentRepo studentRepo,Plain.RabbitMQ.IPublisher publisher,IMediator mediator)
        {
            _studentService = studentService;
            _mapper = mapper;
            _studentRepo = studentRepo;
            _publisher = publisher;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentRequestDto>>> GetAllStudents()
        {
            var students = await _mediator.Send(new GetStudentListQuery());
            var dtoResponse = _mapper.Map<IEnumerable<StudentRequestDto>>(students);
            foreach(StudentRequestDto student in dtoResponse)
            {
                student.StudentAge = _studentService.CalculateAge(student.BirthDate);
            } 
            return Ok(dtoResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentRequestDto>> GetById(int id)
        {
            var student = await _mediator.Send(new GetStudentByIdQuery() { Id = id });
            if (student == null)
            {
                return NotFound(new { message = ExceptionMessages.StudentNotFound });
            }
            var dtoResponse = _mapper.Map<StudentRequestDto>(student);
            dtoResponse.StudentAge = _studentService.CalculateAge(student.BirthDate);
            return Ok(dtoResponse);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstant.Admin)]
        public async Task<ActionResult<StudentRequestDto>> Post(StudentPostDto studentDto)
        {          
            var mappedStudent = _mapper.Map<Student>(studentDto);            
            var isDuplicate =  _studentRepo.CheckDuplicate(mappedStudent.StudentEmail);
            if(isDuplicate == false){
                var returnedStudent =  await _mediator.Send(new AddStudentCommand(mappedStudent));
                var  returnMappedStudent= _mapper.Map<StudentRequestDto>(returnedStudent);
                returnMappedStudent.StudentAge = _studentService.CalculateAge(studentDto.BirthDate);
                var studentCreatedMessage = new StudentEventMessage
                {
                    EventType=RabbitMQConstant.EventTypeCreated,
                    StudentId = returnedStudent.StudentId,
                    StudentName = returnedStudent.FirstName,
                    StudentEmail = returnedStudent.StudentEmail
                };
         
                var message = JsonConvert.SerializeObject(studentCreatedMessage);
                _publisher.Publish(message, RabbitMQConstant.KeyStudentCreated, null);

                
                return Ok(returnMappedStudent);
            }
            throw new DuplicateEntryException(ExceptionMessages.DuplicateEntry);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstant.Admin)]
         public async Task<ActionResult<StudentRequestDto>> Put(int id, UpdateStudentDto studentDto)
        {

            var existingStudent = await _studentRepo.GetStudentById(id);
            if (existingStudent == null)
            {
                return NotFound(new { message = ExceptionMessages.StudentNotFound });
            }
            if(studentDto.StudentEmail != null && _studentRepo.CheckDuplicate(studentDto.StudentEmail)){
                throw new DuplicateEntryException(ExceptionMessages.DuplicateEntry);
            }
           
            var returnedStudent =  await _mediator.Send(new UpdateStudentCommand(id,_mapper.Map<Student>(studentDto)));
            var mappedStudent = _mapper.Map<StudentRequestDto>(returnedStudent);
            mappedStudent.StudentAge = _studentService.CalculateAge(returnedStudent.BirthDate);
            var studentUpdateMessage = new StudentEventMessage
                {
                    EventType=RabbitMQConstant.EventTypeUpdated,
                    StudentId = returnedStudent.StudentId,
                    StudentName = returnedStudent.FirstName,
                    StudentEmail = returnedStudent.StudentEmail
                };
         
                var message = JsonConvert.SerializeObject(studentUpdateMessage);
                _publisher.Publish(message, RabbitMQConstant.KeyStudentUpdated, null);
            return Ok(mappedStudent);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstant.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingStudent = await _studentRepo.GetStudentById(id);
            if (existingStudent == null)
            {
                  return NotFound(new { message = ExceptionMessages.StudentNotFound });
            }
            await _mediator.Send(new DeleteStudentCommand() { Id = id });
            var studentDeletedMessage = new StudentEventMessage
                {
                    EventType=RabbitMQConstant.EventTypeDeleted,
                    StudentId = existingStudent.StudentId,
                    StudentName = existingStudent.FirstName,
                    StudentEmail = existingStudent.StudentEmail
                };
         
            var message = JsonConvert.SerializeObject(studentDeletedMessage);
            _publisher.Publish(message, RabbitMQConstant.KeyStudentDeleted, null);
                  
            return Ok();
        }
    }
}