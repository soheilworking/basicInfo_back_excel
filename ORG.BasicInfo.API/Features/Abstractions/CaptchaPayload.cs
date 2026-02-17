namespace ORG.BasicInfo.API.Features.Abstractions
{
    public record CaptchaPayload(string Code, string Ip, long CreatedAtUnix);
}
