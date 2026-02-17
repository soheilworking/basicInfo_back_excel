using System.Net.Mime;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Authentication.Response;
using ORG.BasicInfo.API.Extensions;
namespace ORG.BasicInfo.API.Features.Authentication;

[Route("api/auth")]
[ApiController]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    [HttpGet]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseFile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseFile>> GetCaptcha([FromRoute] GetCpachaCodeRequest request) =>
    (await _mediator.Send(request)).ToActionResult(this);

    [HttpPut]
    [AllowAnonymous]
    //[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
   

    [ProducesResponseType(typeof(ResponseAuth), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseAuth>> GetVerifyCode([FromBody] GetVerifyCodeRequest request) =>
    (await _mediator.Send(request)).ToActionResult(this);

    [HttpPost]

    //[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]

    [AllowAnonymous]
    //[UseCertificateFundCode]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseAuth), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseAuth>> Authenticate([FromBody] AuthenticationRequest request) =>
        (await _mediator.Send(request)).ToActionResult(this);

    [HttpPost("RefereshCode")]
    //[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]

    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ushort), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ushort>> RefereshCode([FromBody] GetAccessCodesRequest request) =>
    (await _mediator.Send(request)).ToActionResult(this);

    [HttpPatch]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> Logout([FromBody] LogoutRequest request) =>
        (await _mediator.Send(request)).ToActionResult(this);

}