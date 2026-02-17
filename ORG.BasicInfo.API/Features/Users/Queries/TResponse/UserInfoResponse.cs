using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.Users.Queries.TResponse
{
    public class UserInfoResponse : UserListResponse
    {

        public string UserRole { get;  set; }
        public string IpUser { get; set; }
        public ulong IdCodeNationalFund { get; set; }
        public ulong NationalCodeUser { get; set; }
        public Guid LUserCreate { get; set; }
        public Guid LUserEdit { get; set; }
        public IEnumerable<Guid> PermissionFunds { get; set; }
    }
}
