using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Azure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.Users.Queries;

public class DownloadFileRequestHandler(
    FormsInfoDbContext context,
    IValidator<DownloadFileRequest> validator, IHttpContextAccessor httpContextAccessor) : IRequestHandler<DownloadFileRequest, Result<FileResponse>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<DownloadFileRequest> _validator = validator;
    public async Task<Result<FileResponse>> Handle(DownloadFileRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (IsRoleOrg == null||(ushort)IsRoleOrg!=1)
        {
            return Result<FileResponse>.Forbidden("دسترسی غیر مجاز");
        }
            if (!validationResult.IsValid)
        {
            return Result<FileResponse>.Invalid(validationResult.AsErrors());
        }
        var resultSCH= await _context.Users.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
        if (resultSCH == null)
        {
            return Result<FileResponse>.NotFound("فایل یافت نشد و یا حذف گردیده");
        }


        //var stream = new MemoryStream(resultSCH.Certificate);
        FileResponse response = new();
        response.FileName = resultSCH.FundName.Replace(" ","_");
        response.Content = resultSCH.Certificate;
        response.ContentType = "application/x-pkcs12";
        return Result<FileResponse>.Success(response);
    }
}