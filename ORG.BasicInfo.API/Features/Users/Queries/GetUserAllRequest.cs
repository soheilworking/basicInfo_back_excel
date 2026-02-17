using System;
using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.City.Queries;
public class GetUserAllRequest :
    IRequest<Result<UserResponse>>
{
    public GetUserAllRequest( int pageNumber, int pageSize, IEnumerable<string[]> sortField)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortField = sortField;
    }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<string[]> SortField { get; set; }
}
