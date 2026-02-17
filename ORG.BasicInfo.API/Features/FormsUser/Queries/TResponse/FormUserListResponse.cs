using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Domain.FormUserAggregate;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse
{
    public class FormUserListResponse : AResponse
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public ulong IdCode { get; set; }
        public string Title { get; set; }
        public Guid IdFormRaw { get; set; }
        //public bool IsRead { get; set; }
        //public string StatusState { get; set; }
        public ulong IdCodeFund { get; set; }
        public string NameFund { get; set; }
        public long TCreate { get; set; }
        public long TEdit { get; set; }
        public bool IsPublicForm { get; set; }
        public long ExpireDate { get; set; }
        public bool IsStrongRow { get; set; }
        public string StateActionForm { get; set; }
        public StateAction StateAction { get; set; }
    }
}
