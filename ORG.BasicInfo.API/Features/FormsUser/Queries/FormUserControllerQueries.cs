using System.Net.Mime;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.FormsUser.Queries;

[Route("api/[controller]")]
[ApiController]
public class FormUserControllerQueries(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FormUserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FormUserInfoResponse>> GetById([FromQuery] string id) =>
        (await _mediator.Send(new GetFormUserByIdRequest(id))).ToActionResult(this);

    [HttpGet("File")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFileById([FromQuery] string id)
    {
        var result = await _mediator.Send(new DownloadFileUserRequest(id));

        if (result.Status == Ardalis.Result.ResultStatus.Unauthorized) return Unauthorized(result.Errors);
        if (result.Status == Ardalis.Result.ResultStatus.Invalid) return BadRequest(result.Errors);
        if (result.Status == Ardalis.Result.ResultStatus.NotFound) return NotFound(result.Errors);
        if (result.Status != Ardalis.Result.ResultStatus.Ok || result.Value == null) return StatusCode(500);

        var file = result.Value;
        if (file.Content == null || file.Content.Length == 0) return NotFound();

        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        var fileName = string.IsNullOrWhiteSpace(file.FileName) ? "file" : file.FileName;

        return File(file.Content, contentType, fileName);
    }

    [HttpGet("SearchWithIdCode/{readStatus}")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<FormUserListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<FormUserListResponse>>> GetByFundCode(
        [FromQuery] ulong idCode,
        [FromRoute] string readStatus) =>
        (await _mediator.Send(new GetFormUserWithIdCodeRequest(idCode, readStatus))).ToActionResult(this);


    [HttpGet("SearchWithNames/{readStatus}")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FormUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FormUserResponse>> SearchWithName(
        [FromQuery] string names,
        [FromRoute] string readStatus,
        [FromQuery] int pageNumber, 
        [FromQuery] int pageSize, 
        [FromQuery] IEnumerable<string[]> sortField) =>
    (await _mediator.Send(new GetFormUserWithNameTextRequest(pageNumber,pageSize, sortField, names, readStatus))).ToActionResult(this);


   
    [HttpGet("GetAll/{readStatus}")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FormUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FormUserResponse>> GetAll([FromQuery]  int pageNumber, 
        [FromQuery] int pageSize,
        [FromRoute] string readStatus,
        [FromQuery] IEnumerable<string[]> sortField) =>
    (await _mediator.Send(new GetFormUserAllRequest(pageNumber, pageSize,sortField, readStatus))).ToActionResult(this);

    [HttpGet("GetUserNotSend")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FormUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FormUserResponse>> GetUserNotSend(
    [FromQuery] int pageNumber,
    [FromQuery] int pageSize,
    [FromQuery] IEnumerable<string[]> sortField) =>
    (await _mediator.Send(new GetFormUserNotSendRequest(pageNumber, pageSize, sortField))).ToActionResult(this);


    [HttpGet("RepeatedIdCode")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RepeatedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RepeatedResult>> RepeatedIdCode([FromQuery] string id, [FromQuery] ulong fundCode) =>
        (await _mediator.Send(new GetRepeatedFormUserWithIdCodeRequest( fundCode,id))).ToActionResult(this);

    //GetFormUserNotSendRequest
}