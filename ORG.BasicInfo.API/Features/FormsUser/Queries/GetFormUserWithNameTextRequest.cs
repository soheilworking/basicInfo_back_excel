using System;
using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries;
public class GetFormUserWithNameTextRequest :
    IRequest<Result<FormUserResponse>>
{
    public GetFormUserWithNameTextRequest( int pageNumber, int pageSize, IEnumerable<string[]> sortField, string name,string readStatus)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortField = sortField;
        Name = name;
        ReadStatus = readStatus;
    }
    public string Name { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string ReadStatus { get; set; }
    public IEnumerable<string[]> SortField { get; set; }
}
