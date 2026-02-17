namespace ORG.BasicInfo.API.Features.Logs.Queries.TResponse;
public class LogFormResponse 
{
    public IEnumerable<LogFormListResponse> ListResponse { get; set; }
    public ulong Count { get; set; }
}
