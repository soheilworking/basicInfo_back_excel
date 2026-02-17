namespace ORG.BasicInfo.API.Features.Users.Queries.TResponse;
public class UserResponse 
{
    public IEnumerable<UserListResponse> ListResponse { get; set; }
    public ulong Count { get; set; }
}
