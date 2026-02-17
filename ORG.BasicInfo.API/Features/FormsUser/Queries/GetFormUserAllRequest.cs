using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using System.Xml;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries;
public class GetFormUserAllRequest :
    IRequest<Result<FormUserResponse>>
{
    public GetFormUserAllRequest( int pageNumber, int pageSize, IEnumerable<string[]> sortField,string readStatus)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortField = sortField;
        ReadStatus = readStatus;
    }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string ReadStatus { get; set; }
    public IEnumerable<string[]> SortField { get; set; }
}
