using System;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class FormUserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}