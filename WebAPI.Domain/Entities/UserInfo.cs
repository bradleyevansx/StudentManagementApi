using Microsoft.Azure.Cosmos;

namespace WebAPI.Domain.Models;
public class UserInfo : Entity
{
    public UserType UserType { get; set; }
    public UserStatus UserStatus { get; set; }
    public Person Person { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public override string? PartitionKey => id;
    public List<string> TeacherIds { get; set; }
    public List<string> ParentIds { get; set; }
    public List<string> StudentIds { get; set; }
    
    public string Email { get; set; }
}