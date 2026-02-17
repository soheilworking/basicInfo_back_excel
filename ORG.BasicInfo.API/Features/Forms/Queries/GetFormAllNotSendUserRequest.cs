using System;
using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Forms.Queries;
public class GetFormAllNotSendUserRequest :
    IRequest<Result<FormResponse>>
{
    public GetFormAllNotSendUserRequest(int pageNumber, int pageSize, IEnumerable<string[]> sortField)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortField = sortField;
    }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<string[]> SortField { get; set; }

}
