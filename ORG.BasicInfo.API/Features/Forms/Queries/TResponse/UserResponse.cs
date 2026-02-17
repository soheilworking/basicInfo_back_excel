using ORG.BasicInfo.API.Features.Users.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
public class FormResponse 
{
    public IEnumerable<FormListResponse> ListResponse { get; set; }
    public ulong Count { get; set; }
}
