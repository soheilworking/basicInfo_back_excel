namespace ORG.BasicInfo.API.Features.Logs.Queries.TResponse
{
    public class LogFormInfoResponse : LogFormListResponse
    {
        public Guid IdUserRead { get; set; }
        public ulong IdCodeUserRead { get; set; }
        public string NamesUserRead { get; set; }
        public string DescriptionLog { get; set; }
    }

}
