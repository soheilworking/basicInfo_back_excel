using System.Net.Mime;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

[Route("api/[controller]")]
[ApiController]
public class FormUserControllerCommands(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseWrite), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseWrite>> Create([FromBody] UpsertFormUserRequest request) =>
        (await _mediator.Send(request)).ToActionResult(this);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseWrite), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseWrite>> Delete([FromQuery] string id) =>
    (await _mediator.Send(new DeleteFormUserRequest(id))).ToActionResult(this);

    [HttpDelete("File")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseWrite), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseWrite>> RemoveFile([FromQuery] string id) =>
    (await _mediator.Send(new RemoveFileUserRequest(id))).ToActionResult(this);

    [HttpPatch("accept")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseWrite), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseWrite>> ChangeAccept([FromBody] ChangeAcceptFormRequest request) =>
        (await _mediator.Send(request)).ToActionResult(this);


    [HttpPatch("reject")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseWrite), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseWrite>> ChangeReject([FromBody] ChangeRejectFormRequest request) =>
    (await _mediator.Send(request)).ToActionResult(this);
}