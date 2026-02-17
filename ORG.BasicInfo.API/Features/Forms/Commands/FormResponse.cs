using System;

namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class FormResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}