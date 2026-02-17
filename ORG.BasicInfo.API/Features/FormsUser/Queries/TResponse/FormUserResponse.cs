using ORG.BasicInfo.API.Features.Users.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
public class FormUserResponse 
{
    public IEnumerable<FormUserListResponse> ListResponse { get; set; }
    public ulong Count { get; set; }
}
