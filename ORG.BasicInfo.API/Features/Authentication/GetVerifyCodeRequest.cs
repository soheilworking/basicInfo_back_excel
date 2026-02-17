using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Authentication.Response;

namespace ORG.BasicInfo.API.Features.Authentication;

public class GetVerifyCodeRequest : IRequest<Result<ResponseAuth>>
{
    public GetVerifyCodeRequest(
        //ulong idCodeNationalFund,
        //string nationalCodeUser,
        ulong fundCode,
        string password,
        string captchaCode)
    {
        //IdCodeNationalFund = idCodeNationalFund;
        //NationalCodeUser = nationalCodeUser;
        FundCode = fundCode;
        CaptchaCode = captchaCode;
        Password = password;
    }

    public GetVerifyCodeRequest()
    {
    }

    //public ulong IdCodeNationalFund { get; init; }
    //public string NationalCodeUser { get; init; }
    public ulong FundCode { get; init; }
    public string Password { get; init; }
    public string CaptchaCode { get; init; }
}