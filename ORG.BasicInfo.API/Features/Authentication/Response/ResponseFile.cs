namespace ORG.BasicInfo.API.Features.Authentication.Response
{
    public class ResponseFile
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }

        public ResponseFile(byte[] content, string contentType)
        {
            Content = content;
            ContentType = contentType;
        }
    }

}
