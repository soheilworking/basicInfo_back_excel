using System.Net.Mime;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.Forms.Queries;

[Route("api/[controller]")]
[ApiController]
public class FormControllerQueries(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FormInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FormInfoResponse>> GetById([FromQuery] string id) =>
        (await _mediator.Send(new GetFormByIdRequest(id))).ToActionResult(this);

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
        var result = await _mediator.Send(new DownloadFileRequest(id));

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

    [HttpGet("SearchWithIdCode")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<FormListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<FormListResponse>>> GetByFundCode([FromQuery] ulong idCode) =>
        (await _mediator.Send(new GetFormWithIdCodeRequest(idCode))).ToActionResult(this);


    [HttpGet("SearchWithNames")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FormResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FormResponse>> SearchWithName([FromQuery] string names, 
        [FromQuery] int pageNumber, 
        [FromQuery] int pageSize, 
        [FromQuery] IEnumerable<string[]> sortField) =>
    (await _mediator.Send(new GetFormWithNameTextRequest(pageNumber,pageSize, sortField, names))).ToActionResult(this);


  
    [HttpGet("GetAll")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FormResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FormResponse>> GetAll([FromQuery]  int pageNumber, 
        [FromQuery] int pageSize, 
        [FromQuery] IEnumerable<string[]> sortField) =>
    (await _mediator.Send(new GetFormAllRequest(pageNumber, pageSize,sortField))).ToActionResult(this);

    [HttpGet("GetAllNotSendForUser")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FormResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FormResponse>> GetAllNotSend([FromQuery] int pageNumber,
    [FromQuery] int pageSize,
    [FromQuery] IEnumerable<string[]> sortField) =>
        (await _mediator.Send(new GetFormAllNotSendUserRequest(pageNumber, pageSize, sortField))).ToActionResult(this);


    [HttpGet("RepeatedIdCode")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RepeatedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RepeatedResult>> RepeatedIdCode([FromQuery] string id, [FromQuery] ulong fundCode) =>
        (await _mediator.Send(new GetRepeatedFormWithIdCodeRequest( fundCode,id))).ToActionResult(this);


    [HttpGet("GetFormWithTitleOIdCode")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<FormListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<FormListResponse>>> GetFormWithTitleOIdCode([FromQuery] string text, [FromQuery] string id) =>
    (await _mediator.Send(new GetFormWithTitleOIdCodeForUserRequest(text,id))).ToActionResult(this);

    
}