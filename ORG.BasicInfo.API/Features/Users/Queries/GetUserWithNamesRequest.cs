using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Users.Queries;
public class GetUserWithNamesRequest :
    IRequest<Result<UserResponse>>
{
    public GetUserWithNamesRequest( int pageNumber, int pageSize, IEnumerable<string[]> sortField, string name)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortField = sortField;
        Name = name;

    }
    public string Name { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<string[]> SortField { get; set; }
}
