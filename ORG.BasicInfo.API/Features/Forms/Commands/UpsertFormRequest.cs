using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;

namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class UpsertFormRequest : IRequest<Result<ResponseWrite>>
{
    public UpsertFormRequest(
        string title,
        string description,
        ulong idCode,
        long expireDate,
        bool isPublicForm,
        IEnumerable<string> idsFund,
        string id=null)
    {
     
        Id = id;
        Title = title;
        Description = description;
        IdCode = idCode;
        ExpireDate = expireDate;
        IsPublicForm = isPublicForm;
        IdsFund = idsFund;

    }

    public UpsertFormRequest()
    {
    }

    public string Id { get;  set; }
    public ulong IdCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public long ExpireDate { get; set; }
    public bool IsPublicForm { get; set; }
    public IEnumerable<FilesType> Files { get; set; }
    public IEnumerable<string> IdsFund { get; set; }

}
public record FilesType(string Name,string Content,ulong Size);