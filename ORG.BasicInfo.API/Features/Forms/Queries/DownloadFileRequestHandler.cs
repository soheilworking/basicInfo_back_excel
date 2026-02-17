using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.Forms.Queries;

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
        if (!validationResult.IsValid)
        {
            return Result<FileResponse>.Invalid(validationResult.AsErrors());
        }
        FilesRawSys resultSCH= await _context.FilesRawSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
        if (resultSCH == null)
        {
            return Result<FileResponse>.NotFound("فایل یافت نشد و یا حذف گردیده");
        }
        if ((bool)IsRoleOrg == false)
        {
     
                var formRaw = await context.Set<FormRawSys>()
                .Include(f => f.UserFund)
                  .ThenInclude(fu => fu.User)
                .FirstOrDefaultAsync(f => f.Id == resultSCH.IdFormRaw);
                if(formRaw.IsPublicForm==false){
                    var user = formRaw.UserFund.Select(fu => fu.User).FirstOrDefault(item => item.Id == userId);
                    if (user == null)
                    {
                        return Result<FileResponse>.Unauthorized("دسترسی غیر مجاز");
                    }
                }
        }
        else if(IsRoleOrg ==null)
            return Result<FileResponse>.Unauthorized("دسترسی غیر مجاز");


      
        string path = Path.Combine(AppContext.BaseDirectory, "ExcelFile", $"{resultSCH.Title.Split(".")[0]}.enc");
        var fileReal = ExcelFromBase64.SendDecryptedExcelFile(path, Convert.FromBase64String(resultSCH.KeyFile), $"{resultSCH.Title.Split(".")[0]}.xlsx");
        if (fileReal == null)
            return Result<FileResponse>.NotFound("فایل مورد نظر پیدا نشد");
        FileResponse response = new();
        response.FileName = fileReal.FileDownloadName;
        response.Content = fileReal.FileContents;
        response.ContentType = fileReal.ContentType;
        return Result<FileResponse>.Success(response);
    }
}