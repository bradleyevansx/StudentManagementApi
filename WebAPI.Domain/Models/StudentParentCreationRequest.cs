namespace WebAPI.Domain.Models;

public class StudentParentCreationRequest
{
    public string teacherId { get; set; }
    public string studentFirstName { get; set; }
    public string studentLastName { get; set; }
    public string parentFirstName { get; set; }
    public string parentLastName { get; set; }
    
}