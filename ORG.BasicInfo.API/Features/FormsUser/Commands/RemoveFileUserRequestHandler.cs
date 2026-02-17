using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class RemoveFileUserRequestHandler(
    FormsInfoDbContext context,
    IValidator<RemoveFileUserRequest> validator,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<RemoveFileUserRequest, Result<ResponseWrite>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<RemoveFileUserRequest> _validator = validator;
    public async Task<Result<ResponseWrite>> Handle(RemoveFileUserRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        if (IsRoleOrg == null)
        {
            return Result<ResponseWrite>.Unauthorized("دسترسی غیر مجاز");
        }
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ResponseWrite>.Invalid(validationResult.AsErrors());
        }
        var resultSCH = await _context.FilesFormsSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
        if (resultSCH == null)
        {
            return Result<ResponseWrite>.NotFound("فایل یافت نشد و یا حذف گردیده");
        }
        var isReadForm = await _context.FormUserSyss.FirstOrDefaultAsync(item => (item.Id == resultSCH.IdFormUser));
       if(isReadForm==null)
            return Result<ResponseWrite>.NotFound("فرم مربوطه یافت نشد.");

        if (isReadForm.IdUser != userId)
            return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");

        if (isReadForm.IdUserRead != Guid.Empty)
            return Result<ResponseWrite>.Forbidden("عملیات غیر مجاز");


        string path = Path.Combine(AppContext.BaseDirectory, "ExcelFile", $"{resultSCH.Title.Split(".")[0]}.enc");
        if (File.Exists(path))
        {
            File.Delete(path);
            _context.Remove(resultSCH);
            await _context.SaveChangesAsync();
        }
        else
        {
            Result<ResponseWrite>.NotFound("فایل مورد نظر وجود ندارد.");
        }
       
        return Result<ResponseWrite>.Success(new ResponseWrite("فایل مورد نظر با موفقیت حذف گردید."));
    }
}