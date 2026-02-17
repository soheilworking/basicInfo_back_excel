using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class UpsertFormUserRequest : IRequest<Result<ResponseWrite>>
{
    public UpsertFormUserRequest(
        string description,
        string idFormRaw,
        string id=null)
    {
     
        Id = id;
        Description = description;
        IdFormRaw = idFormRaw;
    }

    public UpsertFormUserRequest()
    {
    }

    public string Id { get;  set; }
    public string IdFormRaw { get; set; }
    public string Description { get; set; }
    public IEnumerable<FilesType> Files { get; set; }

}
public record FilesType(string Name,string Content,ulong Size);