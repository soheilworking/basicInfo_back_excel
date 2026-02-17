using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Users.Queries;
public class GetUserOptionRequest :
    IRequest<Result<IEnumerable<UserListResponse>>>
{
    public GetUserOptionRequest( string text, IEnumerable<string> idsFund=null)
    {
        if (ulong.TryParse(text, out _) == true&& ulong.Parse(text)>0)
            FundCode = ulong.Parse(text);
        else
            FundName = text;
        IdsFund = idsFund;
    }
    public string FundName { get; set; } = null;
    public ulong FundCode { get; set; }
    public IEnumerable<string> IdsFund { get; set; }
}
