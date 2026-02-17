using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse
{
    public class FormUserInfoResponse : FormUserListResponse
    {
        public string Description { get; set; }
        public IEnumerable<FormFile> FilesList { get; set; }
    }
    public record FormFile(string Id,string Name,long UploadedAt,ulong size);

}
