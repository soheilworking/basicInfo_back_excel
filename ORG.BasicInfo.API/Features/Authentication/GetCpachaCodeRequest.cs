using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Authentication.Response;

namespace ORG.BasicInfo.API.Features.Authentication;

public class GetCpachaCodeRequest : IRequest<Result<ResponseFile>>
{
    public GetCpachaCodeRequest(ulong mobile)
    {
        Mobile = mobile;
    }

    public GetCpachaCodeRequest()
    {
    }

    public ulong Mobile { get; init; }
}