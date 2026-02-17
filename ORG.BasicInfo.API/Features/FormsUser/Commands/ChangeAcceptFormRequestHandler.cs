using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using System.Security.Claims;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class ChangeAcceptFormRequestHandler(
    FormsInfoDbContext context,
    IValidator<ChangeAcceptFormRequest> validator,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ChangeAcceptFormRequest, Result<ResponseWrite>>
{
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<ChangeAcceptFormRequest> _validator = validator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<ResponseWrite>> Handle(ChangeAcceptFormRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var ipUser = _httpContextAccessor.HttpContext.Items["IpUser"].ToString();
        if (IsRoleOrg == null||(bool)IsRoleOrg==false)
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

        resultSCH.ChangeRead(userId);
        resultSCH.ChangeStateAction(Domain.FormUserAggregate.StateAction.Accept);
        _context.Update(resultSCH);
        await _context.SaveChangesAsync();

        var log = new AddLogFormUser(
            resultSCH.Id,
            resultSCH.IdFormRaw,
            $"تغییر وضعیت به تایید شده",
            resultSCH.LUserCreate,
            userId,
            ipUser==null?"": ipUser,
            Domain.FormUserAggregate.StateAction.Accept,
            _context);

        await log.SaveChange();
        return Result<ResponseWrite>.Success(new ResponseWrite("فرم مورد نظر با موفقیت تایید گردید."));
    }
}