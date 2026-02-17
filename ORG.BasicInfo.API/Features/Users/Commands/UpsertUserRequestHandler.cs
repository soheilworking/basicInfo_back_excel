using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Extensions;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Security.Claims;

namespace ORG.BasicInfo.API.Features.Users.Commands;

public class UpsertUserRequestHandler(
    FormsInfoDbContext context,
      IHttpContextAccessor httpContextAccessor,
    IValidator<UpsertUserRequest> validator) : IRequestHandler<UpsertUserRequest, Result<ResponseWrite>>
{
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<UpsertUserRequest> _validator = validator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<ResponseWrite>> Handle(UpsertUserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ResponseWrite>.Invalid(validationResult.AsErrors());
        }
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        if (IsRoleOrg == null || (ushort)IsRoleOrg != 1)
        {
            return Result<ResponseWrite>.Unauthorized("دسترسی غیر مجاز");
        }

        if (request.Id==""||request.Id==null){
            if (await _context.Users.AsNoTracking().AnyAsync(user => user.FundCode == request.FundCode, cancellationToken))
            {
                return Result<ResponseWrite>.Conflict("شماره صندوق تکراری می باشد.");
            }
            if (await _context.Users.AsNoTracking().AnyAsync(user => user.Mobile == request.Mobile, cancellationToken))
            {
                return Result<ResponseWrite>.Conflict("شماره همراه تکراری می باشد.");
            }
            var certificate = ClientCertificateGenerator.CreateClientCertificate(request.FundCode.ToString(), null);
            var user = new User(request.NationalCodeUser, 
                request.Name,
                request.LastName,
                
                request.Mobile,
                request.IdCodeNationalFund,
                request.FundName,
                request.FundCode,
                request.UserRole=="1" || request.UserRole == "2"?true:false,
                request.IpUser,
                certificate,
                userId);
            
            if (request.Password != null && request.Password != "")
            {
                user.ChangePassword(request.Password);
            }
            else
            {
                user.ChangePassword(request.FundCode.ToString());
            }
                await user.SetUserRole(Convert.ToUInt16(request.UserRole));
            _context.Add(user);
            if(request.PermissionFunds!=null)
            foreach (var item in request.PermissionFunds)
            {
                var tmp = new PermissionFund();
                await tmp.SetData(user.Id, Guid.Parse(item));
                _context.Add(tmp);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(new ResponseWrite("کاربر با موفقیت ایجاد گردید."));
        }
        else
        {
            var resultSCH = await _context.Users.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
            if (resultSCH == null)
            {
                return Result<ResponseWrite>.NotFound("کاربر مورد نظر یافت نشد.");
            }
            if(resultSCH.FundCode!=request.FundCode)
            {
                if (await _context.Users.AsNoTracking().AnyAsync(user => user.FundCode == request.FundCode, cancellationToken))
                {
                    return Result<ResponseWrite>.Conflict("شماره جدید صندوق تکراری می باشد.");
                }
            }
            if (resultSCH.Mobile != request.Mobile)
            {
                if (await _context.Users.AsNoTracking().AnyAsync(user => user.Mobile == request.Mobile, cancellationToken))
                {
                    return Result<ResponseWrite>.Conflict("شماره جدید همراه تکراری می باشد.");
                }
            }

            var certificate = ClientCertificateGenerator.CreateClientCertificate(request.FundCode.ToString(), null);

            resultSCH.ChangeInfo(
                request.NationalCodeUser,
                request.Name,
                request.LastName,
                request.Mobile,
                request.IdCodeNationalFund,
                request.FundName,
                request.FundCode,
                request.UserRole == "1" || request.UserRole == "2" ? true : false,
                request.IpUser,
                certificate,
                userId);
            if(request.Password!=null&& request.Password != "")
            {
                resultSCH.ChangePassword(request.Password);
            }
            if((await resultSCH.GetUserRole()).ToString()!= request.UserRole)
                await resultSCH.SetUserRole(Convert.ToUInt16(request.UserRole));

            var idEnc=await PermissionFund.GetEncryptDataWithThisKey(resultSCH.Id.ToString());
           
            var arrPFund =await _context.PermissionFunds.Where(item => item.IdUser == idEnc).ToArrayAsync();
            _context.RemoveRange(arrPFund);
            if (request.PermissionFunds != null)
                foreach (var item in request.PermissionFunds)
                {
                    var tmp = new PermissionFund();
                    await tmp.SetData(resultSCH.Id, Guid.Parse(item));
                    _context.Add(tmp);
                }
            _context.Update(resultSCH);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(new ResponseWrite("کاربر با موفقیت ویرایش گردید."));
        }
    }
}