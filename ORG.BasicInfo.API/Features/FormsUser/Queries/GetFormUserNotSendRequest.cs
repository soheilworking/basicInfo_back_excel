using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using System.Xml;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries;
public class GetFormUserNotSendRequest :
    IRequest<Result<FormUserResponse>>
{
    public GetFormUserNotSendRequest( int pageNumber, int pageSize, IEnumerable<string[]> sortField)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortField = sortField;
    }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<string[]> SortField { get; set; }
}
