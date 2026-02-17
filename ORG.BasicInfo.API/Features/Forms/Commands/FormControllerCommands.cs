using System.Net.Mime;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;

namespace ORG.BasicInfo.API.Features.Forms.Commands;

[Route("api/[controller]")]
[ApiController]
public class FormControllerCommands(IMediator mediator) : ControllerBase
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
    public async Task<ActionResult<ResponseWrite>> Create([FromBody] UpsertFormRequest request) =>
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
    (await _mediator.Send(new DeleteFormRequest(id))).ToActionResult(this);

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
    (await _mediator.Send(new RemoveFileRequest(id))).ToActionResult(this);

    [HttpPatch]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseWrite), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseWrite>> ChangeState([FromBody] ChangeStateFormRequest request) =>
        (await _mediator.Send(request)).ToActionResult(this);
}