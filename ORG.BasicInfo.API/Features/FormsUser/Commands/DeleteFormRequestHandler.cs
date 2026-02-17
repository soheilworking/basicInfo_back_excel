using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using System.Security.Claims;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class DeleteFormUserRequestHandler(
    FormsInfoDbContext context,
    IValidator<DeleteFormUserRequest> validator,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteFormUserRequest, Result<ResponseWrite>>
{
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<DeleteFormUserRequest> _validator = validator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<ResponseWrite>> Handle(DeleteFormUserRequest request, CancellationToken cancellationToken)
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
        var resultSCH=await _context.FormUserSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
        if (resultSCH == null)
        {
            return Result<ResponseWrite>.NotFound("فرم یافت نشد و یا حذف گردیده");
        }
        if (resultSCH.IdUser != userId)
            return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");

        if (resultSCH.IdUserRead != Guid.Empty)
            return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");
        if(resultSCH.StateAction!=Domain.FormUserAggregate.StateAction.NoRead)
            return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");

        var resultSCHFiles = await _context.FilesFormsSyss.Where(item => item.IdFormUser == Guid.Parse(request.Id)).ToArrayAsync();
        foreach (var item in resultSCHFiles)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "ExcelFile", $"{item.Title.Split(".")[0]}.enc");
            if (File.Exists(path))
            {
                File.Delete(path);
                _context.Remove(item);
            }
        }

        _context.Remove(resultSCH);
        await _context.SaveChangesAsync();
        return Result<ResponseWrite>.Success(new ResponseWrite("کاربر مورد نظر با موفقیت حذف گردید."));
    }
}