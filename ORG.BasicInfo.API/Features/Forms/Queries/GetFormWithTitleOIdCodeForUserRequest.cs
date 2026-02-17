using System;
using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Forms.Queries;
public class GetFormWithTitleOIdCodeForUserRequest :
    IRequest<Result<IEnumerable<FormListResponse>>>
{
    public GetFormWithTitleOIdCodeForUserRequest(string text, string id)
    {


        if (ulong.TryParse(text, out _) == true)
            IdCode = ulong.Parse(text);
        else
            Title = text;
        Id = id;

    }
    public string Title { get; set; }
    public ulong IdCode { get; set; }
    public string Id { get; set; }
}
