namespace WebAPI.Domain.Models;

public class RefreshToken : Entity
{
    public override string? PartitionKey => id;
    public string? UserId { get; set; }
    public DateTime ExpirationDateTime { get; set; }
    public bool IsValid { get; set; }
    public UserType? UserType { get; set; }
}