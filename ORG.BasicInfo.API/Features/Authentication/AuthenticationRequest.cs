using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Authentication.Response;

namespace ORG.BasicInfo.API.Features.Authentication;

public class AuthenticationRequest : IRequest<Result<ResponseAuth>>
{
    public AuthenticationRequest(
        //ulong idCodeNationalFund,
        //string nationalCodeUser,
        ulong fundCode,
        ulong verifyCode)
    {
        //IdCodeNationalFund = idCodeNationalFund;
        //NationalCodeUser = nationalCodeUser;
        FundCode = fundCode;
        VerifyCode = verifyCode;
        //CaptchaCode = captchaCode;
    }

    public AuthenticationRequest()
    {
    }

    //public ulong IdCodeNationalFund { get; init; }
    //public string NationalCodeUser { get; init; }
    public ulong FundCode { get; init; }
    public ulong VerifyCode { get; init; }
    //public string CaptchaCode { get; init; }
}