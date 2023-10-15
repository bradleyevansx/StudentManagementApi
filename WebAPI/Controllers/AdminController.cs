using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Domain.Interfaces;
using WebAPI.Domain.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IRepository<UserInfo> _repositoryConnection;

    public AdminController(IRepository<UserInfo> repositoryConnection)
    {
        _repositoryConnection = repositoryConnection;
    }

    [Authorize(Roles = "Admin"), HttpPut("teacher/students-and-parents")]
    public async Task<IActionResult> CreateNewStudentAndParentAndAssignToTeacherAsync(
        [FromBody] StudentParentCreationRequest request)
    {
        var teacher = await _repositoryConnection.GetAsync(request.teacherId);
        if (teacher is null) return Problem("Must use valid teacher id");
        var newStudent = await _repositoryConnection.CreateAsync(new UserInfo
        {
            Person = new Person() { FirstName = request.studentFirstName, LastName = request.studentLastName },
            UserType = UserType.Student, ParentIds = new List<string>(), StudentIds = new List<string>(),
            TeacherIds = new List<string> { request.teacherId },
            UserStatus = UserStatus.Active
        });
        var newParent = await _repositoryConnection.CreateAsync(new UserInfo
        {
            Person = new Person() { FirstName = request.parentFirstName, LastName = request.parentLastName },
            UserType = UserType.Parent, TeacherIds = new List<string> { request.teacherId }, StudentIds =
                new List<string>
                {
                    newStudent.Resource!.id!
                },
            UserStatus = UserStatus.Active,
            ParentIds = new List<string>()
        });
        teacher!.StudentIds.Add(newStudent.Resource.id!);
        teacher.ParentIds.Add(newParent.Resource.id!);
        newStudent.Resource.ParentIds = new List<string>() { newParent.Resource.id! };
        var updateNewStudent = await _repositoryConnection.UpdateAsync(newStudent.Resource);
        var result = await _repositoryConnection.UpdateAsync(teacher);
        var response = new
        {
            newStudentId = newStudent.Resource.id,
            newParentId = newParent.Resource.id
        };


        if (result.StatusCode is HttpStatusCode.OK) return Ok(response);
        else return BadRequest("Error In Updating the Entity");
    }

    [Authorize(Roles = "Admin"), HttpPost("teacher")]
    public async Task<IActionResult> CreateNewTeacherAsync([FromBody] string[] person)
    {
        var newTeacher = await _repositoryConnection.CreateAsync(new UserInfo()
        {
            UserStatus = UserStatus.Active, UserType = UserType.Teacher,
            Person = new Person() { FirstName = person[0], LastName = person[1] }, TeacherIds = new List<string>(),
            StudentIds = new List<string>(), ParentIds = new List<string>()
        });

        if (newTeacher.StatusCode is HttpStatusCode.Created) return Ok(newTeacher.Resource.id);
        else return BadRequest("Error In creating the teacher.");
    }
    [Authorize(Roles = "Admin"), HttpPost("parent")]
    public async Task<IActionResult> CreateNewParentAsync([FromBody] string[] person)
    {
        var newParent = await _repositoryConnection.CreateAsync(new UserInfo()
        {
            UserStatus = UserStatus.Active, UserType = UserType.Parent,
            Person = new Person() { FirstName = person[0], LastName = person[1] }, TeacherIds = new List<string>(),
            StudentIds = new List<string>(), ParentIds = new List<string>()
        });

        if (newParent.StatusCode is HttpStatusCode.Created) return Ok(newParent.Resource.id);
        else return BadRequest("Error in creating the parent.");
    }

    [Authorize(Roles = "Admin"), HttpPost("student")]
    public async Task<IActionResult> CreateNewStudentAsync([FromBody] string[] person)
    {
        var newStudent = await _repositoryConnection.CreateAsync(new UserInfo()
        {
            UserStatus = UserStatus.Active, UserType = UserType.Student,
            Person = new Person() { FirstName = person[0], LastName = person[1] },
            TeacherIds = new List<string>(), StudentIds = new List<string>(), ParentIds = new List<string>()
        });

        if (newStudent.StatusCode is HttpStatusCode.Created) return Ok(newStudent.Resource.id);
        else return BadRequest("Error In creating the student.");
    }

    [Authorize(Roles = "Admin"), HttpGet("teacher")]
    public async Task<IActionResult> GetTeachersAsync()
    {
        var teachers = await _repositoryConnection.Query().Where(x => x.UserType == UserType.Teacher).ToListAsync();

        if (teachers.Count > 0) return Ok(teachers);
        else return BadRequest("No teachers found.");
    }

    [Authorize(Roles = "Admin"), HttpGet("parent")]
    public async Task<IActionResult> GetParentsAsync()
    {
        var parents = await _repositoryConnection.Query().Where(x => x.UserType == UserType.Parent).ToListAsync();

        if (parents.Count > 0) return Ok(parents);
        else return BadRequest("No parents found.");
    }

    [Authorize(Roles = "Admin"), HttpGet("student")]
    public async Task<IActionResult> GetStudentsAsync()
    {
        var students = await _repositoryConnection.Query().Where(x => x.UserType == UserType.Student).ToListAsync();

        if (students.Count > 0) return Ok(students);
        else return BadRequest("No students found.");
    }
    [Authorize(Roles = "Admin"), HttpGet("{userId}")]
    public async Task<IActionResult> GetUserAsync(string userId)
    {
        var user = await _repositoryConnection.GetAsync(userId);

        if (user is not null) return Ok(user);
        else return BadRequest("No users found.");
    }
    [Authorize(Roles = "Admin"), HttpPut]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UserInfo user)
    {
        var response = await _repositoryConnection.UpdateAsync(user);

        if (response.StatusCode is HttpStatusCode.OK) return Ok(response.Resource);
        else return BadRequest("Error In Updating the Entity");
    }
}