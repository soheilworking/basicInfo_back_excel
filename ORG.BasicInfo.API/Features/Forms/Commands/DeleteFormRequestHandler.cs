using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Security.Claims;

namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class DeleteFormRequestHandler(
    FormsInfoDbContext context,
    IValidator<DeleteFormRequest> validator,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteFormRequest, Result<ResponseWrite>>
{
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<DeleteFormRequest> _validator = validator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<ResponseWrite>> Handle(DeleteFormRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];

        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if (IsRoleOrg == null || (ushort)IsRoleOrg > 2 || (ushort)IsRoleOrg < 1)
        {
            return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");
        }
     
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ResponseWrite>.Invalid(validationResult.AsErrors());
        }
        var logForm = await _context.FormRawLogSyss
         .Where(item => item.IdForm == Guid.Parse(request.Id)).ToArrayAsync();
        if (logForm.Any(item => item.IdUser != userId) == true)
        {
            return Result<ResponseWrite>.Conflict("به دلیل انجام عملیات بر رویه فرم حذف نمیشود.");
        }
        _context.RemoveRange(logForm);

        var resultSCH = await _context.FormRawSyss
        .Include(f => f.UserFund)
                .ThenInclude(fu => fu.User)
        .FirstOrDefaultAsync(f => f.Id == Guid.Parse(request.Id));
        if (resultSCH == null)
        {
            return Result<ResponseWrite>.NotFound("فرم یافت نشد و یا حذف گردیده");
        }
     
        foreach (var item in resultSCH.UserFund)
        {
            if (fundsList.Contains(item.IdUser) == false)
                return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");
        }


        var listFile = await _context.FilesRawSyss.Where(item => item.IdFormRaw == Guid.Parse(request.Id)).ToArrayAsync();
        foreach (var item in listFile)
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
        return Result<ResponseWrite>.Success(new ResponseWrite("فرم مورد نظر با موفقیت حذف گردید."));
    }
}