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

namespace ORG.BasicInfo.API.Features.Users.Commands;

public class DeleteUserRequestHandler(
    FormsInfoDbContext context,
    IValidator<DeleteUserRequest> validator,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteUserRequest, Result<ResponseWrite>>
{
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<DeleteUserRequest> _validator = validator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<ResponseWrite>> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        if (IsRoleOrg == null || (ushort)IsRoleOrg != 1)
        {
            return Result<ResponseWrite>.Unauthorized("دسترسی غیر مجاز");
        }
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ResponseWrite>.Invalid(validationResult.AsErrors());
        }
        var resultSCH=await _context.Users.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
        if (resultSCH == null)
        {
            return Result<ResponseWrite>.NotFound("کاربر یافت نشد و یا حذف گردیده");
        }

        _context.Remove(resultSCH);

        var idEnc = await PermissionFund.GetEncryptDataWithThisKey(resultSCH.Id.ToString());
        var arrPFund = await _context.PermissionFunds.Where(item => item.IdUser == idEnc).ToArrayAsync();
        _context.RemoveRange(arrPFund);

        await _context.SaveChangesAsync();
        return Result<ResponseWrite>.Success(new ResponseWrite("کاربر مورد نظر با موفقیت حذف گردید."));
    }
}