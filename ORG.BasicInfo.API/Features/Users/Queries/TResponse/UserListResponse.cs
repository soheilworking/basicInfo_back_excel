using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.Users.Queries.TResponse
{
    public class UserListResponse : AResponse
    {
        public Guid Id { get; set; }
        public string Name { get;  set; }
        public string LastName { get;  set; }
        public ulong Mobile { get;  set; }
        public string FundName { get;  set; }
        public ulong FundCode { get;  set; }
        public UserState State { get;  set; }
        public long TCreate { get;  set; }
        public long TEdit { get;  set; }

    }
}
