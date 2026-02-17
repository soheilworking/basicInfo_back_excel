using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Authentication.Response;

namespace ORG.BasicInfo.API.Features.Authentication;

public class GetAccessCodesRequest : IRequest<Result<ushort>>
{
    public GetAccessCodesRequest()
    {
        //Mobile = mobile;
        //VerifyCode = verifyCode;
        //CaptchaCode = captchaCode;
    }

 

    //public string Mobile { get; init; }
    //public ulong VerifyCode { get; init; }
    //public string CaptchaCode { get; init; }
}