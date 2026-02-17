using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.Logs.Queries.TResponse
{
    public class LogFormListResponse
    {
        public Guid Id { get; set; }
        public Guid IdFormUser { get; set; }
        public Guid IdFormRaw { get; set; }
        public Guid IdUser { get; set; }
        public ulong IdCodeForm { get; set; }
        public string TitleForm { get; set; }
        public string StateAction { get; set; }
        public string NamesUser { get; set; }
        public ulong IdCodeFund { get; set; }
        public string NameFund { get; set; }
        public string IsOrgUser { get; set; }
        public long Timestamp { get; set; }

    }
}
