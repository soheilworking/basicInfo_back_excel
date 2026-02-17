using System.Net.Mime;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.City.Queries;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.Users.Queries;

[Route("api/[controller]")]
[ApiController]
public class UsersControllerQueries(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserInfoResponse>> GetById([FromQuery] string id) =>
        (await _mediator.Send(new GetUserByIdRequest(id))).ToActionResult(this);


    [HttpGet("GetAllOptions")]
        [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<UserListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserListResponse>>> GetAllOptions([FromQuery] string text, [FromQuery]  IEnumerable<string> idsSelected) =>
        (await _mediator.Send(new GetUserOptionRequest(text, idsSelected))).ToActionResult(this);

    [HttpGet("SearchWithFundCode")]
           [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<UserListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserListResponse>>> GetByFundCode([FromQuery] ulong idCode) =>
        (await _mediator.Send(new GetUserWithFundCodeRequest(idCode))).ToActionResult(this);

    [HttpGet("SearchWithMobile")]
           [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<UserListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserListResponse>>> GetByMobile([FromQuery] ulong mobile) =>
    (await _mediator.Send(new GetUserWithMobileRequest(mobile))).ToActionResult(this);



    [HttpGet("SearchWithNames")]
           [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> SearchWithName([FromQuery] string names, 
        [FromQuery] int pageNumber, 
        [FromQuery] int pageSize, 
        [FromQuery] IEnumerable<string[]> sortField) =>
    (await _mediator.Send(new GetUserWithNamesRequest(pageNumber,pageSize, sortField, names))).ToActionResult(this);

    [HttpGet("SearchWithNameFund")]
           [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> SearchWithNameFund([FromQuery] string nameFund,
    [FromQuery] int pageNumber,
    [FromQuery] int pageSize,
    [FromQuery] IEnumerable<string[]> sortField) =>
        (await _mediator.Send(new GetUserWithNameFundRequest(pageNumber, pageSize, sortField, nameFund))).ToActionResult(this);

    [HttpGet("GetAll")]
        [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> GetAll([FromQuery]  int pageNumber, 
        [FromQuery] int pageSize, 
        [FromQuery] IEnumerable<string[]> sortField) =>
    (await _mediator.Send(new GetUserAllRequest(pageNumber, pageSize,sortField))).ToActionResult(this);

    [HttpGet("RepeatedFundCode")]
           [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RepeatedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RepeatedResult>> RepeatedFundCode([FromQuery] string id, [FromQuery] ulong fundCode) =>
        (await _mediator.Send(new GetRepeatedUserWithFundCodeRequest( fundCode,id))).ToActionResult(this);


    [HttpGet("RepeatedMobile")]
           [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RepeatedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RepeatedResult>> RepeatedMobile([FromQuery] string id, [FromQuery] ulong mobile) =>
      (await _mediator.Send(new GetRepeatedUserWithMobileRequest(mobile, id))).ToActionResult(this);

    [HttpGet("GetCertificateUser")]
    [Authorize(AuthenticationSchemes = "JwtAccessScheme")]
    [RequireSessionValidation]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCertificateUser([FromQuery] string id)
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


    //public async Task<ActionResult<FileResponse>> GetCertificateUser([FromQuery] string id) =>
    //    (await _mediator.Send(new DownloadFileRequest(id))).ToActionResult(this);


}