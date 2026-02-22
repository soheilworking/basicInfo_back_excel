using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class ChangeStateFormRequestHandler(
    FormsInfoDbContext context,
    IValidator<ChangeStateFormRequest> validator,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ChangeStateFormRequest, Result<ResponseWrite>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<ChangeStateFormRequest> _validator = validator;
    public async Task<Result<ResponseWrite>> Handle(ChangeStateFormRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var IpUser = _httpContextAccessor.HttpContext.Items["IpUser"].ToString();

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
        var resultSCH = await _context.Set<FormRawSys>()
        .Include(f => f.UserFund)
        .ThenInclude(fu => fu.User)
       .FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));

        if (resultSCH == null)
        {
            return Result<ResponseWrite>.NotFound("کاربر یافت نشد و یا حذف گردیده");
        }
        if ((ushort)IsRoleOrg > 1)
            foreach (var item in resultSCH.UserFund)
            {
                if (fundsList.Contains(item.IdUser) == false)
                    return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");
            }

        resultSCH.ChangeState();
        _context.Update(resultSCH);
        
        await _context.SaveChangesAsync();
        string state_v = resultSCH.State == Domain.Abstractions.EntityState.Active ? " فعال " : " غیر فعال ";
        var log = new AddLogFormRaw(
         resultSCH.Id,
         "تغییر وضعیت به" + state_v,
          Guid.Empty,
          userId,
          IpUser,
         _context);

        await log.SaveChange();
        return Result<ResponseWrite>.Success(new ResponseWrite("کاربر مورد نظر با موفقیت حذف گردید."));
    }
}