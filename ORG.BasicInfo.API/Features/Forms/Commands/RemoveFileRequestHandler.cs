using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class RemoveFileRequestHandler(
    FormsInfoDbContext context,
    IValidator<RemoveFileRequest> validator, IHttpContextAccessor httpContextAccessor) : IRequestHandler<RemoveFileRequest, Result<ResponseWrite>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<RemoveFileRequest> _validator = validator;
    public async Task<Result<ResponseWrite>> Handle(RemoveFileRequest request, CancellationToken cancellationToken)
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
        if (IsRoleOrg == null || (ushort)IsRoleOrg > 2|| (ushort)IsRoleOrg < 1)
        {
            return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ResponseWrite>.Invalid(validationResult.AsErrors());
        }
        //var resultSCH = await _context.FilesRawSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
        var resultSCH = await _context.Set<FormRawSys>()
            .Include(f => f.UserFund)
            .ThenInclude(fu => fu.User)
           .FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));

        if (resultSCH == null)
        {
            return Result<ResponseWrite>.NotFound("فایل یافت نشد و یا حذف گردیده");
        }
        foreach (var item in resultSCH.UserFund)
        {
            if(fundsList.Contains(item.IdUser)==false)
                return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");
        }
      
        //if(fundsList.Any(resultSCH.))
  
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