using System.Net.Mime;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Logs.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.Logs.Queries;

[Route("api/[controller]")]
[ApiController]
public class LogFormUserControllerQueries(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(LogFormInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LogFormInfoResponse>> GetById([FromQuery] string id) =>
        (await _mediator.Send(new GetLogFormByIdRequest(id))).ToActionResult(this);


    [HttpGet("GetAll")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(LogFormResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LogFormResponse>> GetAll(

        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] IEnumerable<string[]> sortField
        ) =>
    (await _mediator.Send(new GetLogsFormAllRequest(pageNumber,pageSize,sortField))).ToActionResult(this);


    [HttpGet("GetAllOneForm/{id}")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(LogFormResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LogFormResponse>> GetGetAllOneFormAll(
        [FromRoute] string id,
    [FromQuery] int pageNumber,
    [FromQuery] int pageSize,
    [FromQuery] IEnumerable<string[]> sortField
    ) =>
        (await _mediator.Send(new GetLogsOneFormAllRequest(id,pageNumber, pageSize, sortField))).ToActionResult(this);

    [HttpGet("GetAllOneFormUser")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<LogFormInfoResponseUser>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<LogFormInfoResponseUser>>> GetGetAllOneFormUserAll(
    [FromQuery] string id
    ) =>
    (await _mediator.Send(new GetLogsOneFormUserAllRequest(id))).ToActionResult(this);

    [HttpGet("SearchWithNames")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(LogFormResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LogFormResponse>> SearchWithName(
        [FromQuery] string names,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] IEnumerable<string[]> sortField) =>
    (await _mediator.Send(new GetLogFormWithNameTextRequest(pageNumber, pageSize, sortField, names))).ToActionResult(this);


}