using System;
using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.API.Features.Logs.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Logs.Queries;
public class GetLogsFormAllRequest :
    IRequest<Result<LogFormResponse>>
{
    public GetLogsFormAllRequest(
        //Guid id,
        int pageNumber,
        int pageSize,
        IEnumerable<string[]> sortField
        
        )
    {
        //Id = id;
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortField = sortField;
    }
    public Guid Id { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<string[]> SortField { get; set; }

}
