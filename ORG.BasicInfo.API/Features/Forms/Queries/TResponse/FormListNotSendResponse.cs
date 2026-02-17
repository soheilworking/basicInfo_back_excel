namespace ORG.BasicInfo.API.Features.Forms.Queries.TResponse
{

    public class FormListNotSendResponse
    {
        public Guid IdUser { get; set; }
        public Guid IdForm { get; set; }
        public string Title { get; set; }
        public long ExpireDate { get; set; }
        public string FundName { get; set; }
        public ulong FundCode { get; set; }
    }
}
