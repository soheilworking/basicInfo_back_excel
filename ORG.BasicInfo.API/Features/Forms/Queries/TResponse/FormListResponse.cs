using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Domain.Abstractions;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.Forms.Queries.TResponse
{
    public class FormListResponse : AResponse
    {
        public Guid Id { get; set; }
        public ulong IdCode { get; set; }
        public string Title { get; set; }
        public long ExpireDate { get; set; }
        public bool IsPublicForm { get; set; }
        public EntityState State { get; set; }
        public long TCreate { get; set; }
        public long TEdit { get; set; }
        public bool IsRead { get; set; }
        public bool IsStrongRow { get; set; }

    }
}
