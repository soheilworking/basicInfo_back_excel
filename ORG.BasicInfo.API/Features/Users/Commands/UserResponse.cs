using System;

namespace ORG.BasicInfo.API.Features.Users.Commands;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}