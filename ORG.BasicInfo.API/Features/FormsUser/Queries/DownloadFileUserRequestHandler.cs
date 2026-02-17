using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries;

public class DownloadFileUserRequestHandler(
    FormsInfoDbContext context,
    IValidator<DownloadFileUserRequest> validator,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<DownloadFileUserRequest, Result<FileUserResponse>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<DownloadFileUserRequest> _validator = validator;
    public async Task<Result<FileUserResponse>> Handle(DownloadFileUserRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if (!validationResult.IsValid)
        {
            return Result<FileUserResponse>.Invalid(validationResult.AsErrors());
        }
        if (IsRoleOrg == null)
        {
            return Result<FileUserResponse>.Unauthorized("دسترسی غیر مجاز");
        }
        if ((ushort)IsRoleOrg == 3)
        {
          
            var resultSCH = await _context.FilesFormsSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
         
            if (resultSCH == null)
            {
                return Result<FileUserResponse >.NotFound("فایل یافت نشد و یا حذف گردیده");
            }
            var resultFormSCH = await _context.FormUserSyss.FirstOrDefaultAsync(item => item.Id == resultSCH.IdFormUser);
            if(resultFormSCH==null||resultFormSCH.IdUser!= userId)
            {
                return Result<FileUserResponse>.NotFound("فایل یافت نشد و یا حذف گردیده");

            }
            string path = Path.Combine(AppContext.BaseDirectory, "ExcelFile", $"{resultSCH.Title.Split(".")[0]}.enc");
            var fileReal=ExcelFromBase64.SendDecryptedExcelFile(path, Convert.FromBase64String(resultSCH.KeyFile), $"{resultSCH.Title.Split(".")[0]}.xlsx");
            if (fileReal == null)
                return Result<FileUserResponse>.NotFound("فایل مورد نظر پیدا نشد");
            FileUserResponse response = new();
            response.FileName = fileReal.FileDownloadName;
            response.Content = fileReal.FileContents;
            response.ContentType = fileReal.ContentType;
            return Result<FileUserResponse>.Success(response);
        }
        else if ((ushort)IsRoleOrg == 1)
        {

            var resultSCH = await _context.FilesFormsSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));

            if (resultSCH == null)
            {
                return Result<FileUserResponse>.NotFound("فایل یافت نشد و یا حذف گردیده");
            }
            var resultFormSCH = await _context.FormUserSyss.FirstOrDefaultAsync(item => item.Id == resultSCH.IdFormUser);
            if (resultFormSCH == null)
            {
                return Result<FileUserResponse>.NotFound("فایل یافت نشد و یا حذف گردیده");

            }
            string path = Path.Combine(AppContext.BaseDirectory, "ExcelFile", $"{resultSCH.Title.Split(".")[0]}.enc");
            var fileReal = ExcelFromBase64.SendDecryptedExcelFile(path, Convert.FromBase64String(resultSCH.KeyFile), $"{resultSCH.Title.Split(".")[0]}.xlsx");
            if (fileReal == null)
                return Result<FileUserResponse>.NotFound("فایل مورد نظر پیدا نشد");
            FileUserResponse response = new();
            response.FileName = fileReal.FileDownloadName;
            response.Content = fileReal.FileContents;
            response.ContentType = fileReal.ContentType;
            return Result<FileUserResponse>.Success(response);
        }
        else if ((ushort)IsRoleOrg == 2)
        {

            var resultSCH = await _context.FilesFormsSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));

            if (resultSCH == null)
            {
                return Result<FileUserResponse>.NotFound("فایل یافت نشد و یا حذف گردیده");
            }
            var resultFormSCH = await _context.FormUserSyss.FirstOrDefaultAsync(item => item.Id == resultSCH.IdFormUser);
            
            if (resultFormSCH == null || fundsList?.FirstOrDefault(resultFormSCH.IdUser)==null)
            {
                return Result<FileUserResponse>.NotFound("فایل یافت نشد و یا حذف گردیده");

            }

            string path = Path.Combine(AppContext.BaseDirectory, "ExcelFile", $"{resultSCH.Title.Split(".")[0]}.enc");
            var fileReal = ExcelFromBase64.SendDecryptedExcelFile(path, Convert.FromBase64String(resultSCH.KeyFile), $"{resultSCH.Title.Split(".")[0]}.xlsx");
            if (fileReal == null)
                return Result<FileUserResponse>.NotFound("فایل مورد نظر پیدا نشد");
            FileUserResponse response = new();
            response.FileName = fileReal.FileDownloadName;
            response.Content = fileReal.FileContents;
            response.ContentType = fileReal.ContentType;
            return Result<FileUserResponse>.Success(response);
        }
        else
            return Result<FileUserResponse>.Unauthorized("دسترسی غیر مجاز");
    }
}