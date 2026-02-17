using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using ORG.BasicInfo.API.Extensions;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Buffers.Text;
using System.Security.Claims;

namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class UpsertFormRequestHandler(
    FormsInfoDbContext context,
    IHttpContextAccessor httpContextAccessor,
    IValidator<UpsertFormRequest> validator) : IRequestHandler<UpsertFormRequest, Result<ResponseWrite>>
{
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<UpsertFormRequest> _validator = validator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<ResponseWrite>> Handle(UpsertFormRequest request, CancellationToken cancellationToken)
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
        foreach (var item in request.IdsFund)
        {
            if(fundsList.Contains(Guid.Parse(item))==false)
                return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");
        }
        if((ushort)IsRoleOrg==2&&request.IsPublicForm==true)
            return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ResponseWrite>.Invalid(validationResult.AsErrors());
        }
    
        if(request.Files!=null&& request.Files.Count()>0)
        foreach (var item in request.Files)
        {

            var validExcelFile = ExcelValidator.ValidateBase64IsExcel(item.Content);
            if (validExcelFile == ExcelCheckResult.NotBase64 || validExcelFile == ExcelCheckResult.NotExcel)
            {
                return Result<ResponseWrite>.Invalid(new ValidationError("فایل های مربوطه نامعتبر است"));
            }

        }
       
       
        if (request.Id==""||request.Id==null){
            if (await _context.FormRawSyss.AsNoTracking().AnyAsync(form => form.IdCode == request.IdCode, cancellationToken))
            {
                return Result<ResponseWrite>.Conflict("شماره فرم تکراری می باشد.");
            }


            var form = new FormRawSys(request.IdCode,
                request.Title,
                request.Description,
                request.ExpireDate,
                request.IsPublicForm,
                userId);

            _context.Add(form);

            if (request.Files != null && request.Files.Count() > 0)
                foreach (var item in request.Files)
            {

                string fullPath = Path.Combine(AppContext.BaseDirectory, "ExcelFile", $"{item.Name.Split(".")[0]}.enc");
                byte[] key=ExcelFromBase64.SaveBase64ExcelToFile(item.Content, fullPath);
                    string base64 = "";
                    int comma = item.Content.IndexOf(',');
                    if (comma >= 0) base64 = item.Content.Substring(comma + 1);
                    base64 = base64.Trim();

                    byte[] contentBytes = Convert.FromBase64String(base64);
                    ulong sizeInBytes =Convert.ToUInt64(contentBytes.LongLength);
                string base64Key = Convert.ToBase64String(key);
            
                var file = new FilesRawSys(form.Id, item.Name, sizeInBytes, base64Key, userId);
                    await _context.AddAsync(file);
                    
            }
            if (request.IdsFund!=null&&request.IdsFund.Count() > 0)
            {
                foreach (var item in request.IdsFund)
                {
                    var formRawRelated = new FormRawRelatedUserSys(form.Id, Guid.Parse(item));
                    await _context.AddAsync(formRawRelated);
                    form.UserFund.Add(formRawRelated);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            var log = new AddLogFormRaw(
                 form.Id,
                 $"ثبت شده",
                  Guid.Empty,
                  userId,
                  IpUser,
                 _context);

            await log.SaveChange();
            return Result.Created(new ResponseWrite("فرم با موفقیت ایجاد گردید."));
        }
        else
        {
            //var resultSCH = await _context.FormRawSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));

            var resultSCH = await _context.FormRawSyss
            .Include(f => f.UserFund)
                    .ThenInclude(fu => fu.User)
            .FirstOrDefaultAsync(f => f.Id == Guid.Parse(request.Id));

            if (resultSCH == null)
            {
                return Result<ResponseWrite>.NotFound("فرم مورد نظر یافت نشد.");
            }
            if(resultSCH.IdCode!=request.IdCode)
            {
                if (await _context.FormRawSyss.AsNoTracking().AnyAsync(form => form.IdCode == request.IdCode, cancellationToken))
                {
                    return Result<ResponseWrite>.Conflict("شماره جدید فرم تکراری می باشد.");
                }
            }

            resultSCH.ChangeValue(request.IdCode,
                request.Title,
                request.Description,
                request.ExpireDate,
                request.IsPublicForm,
                userId);
            _context.Update(resultSCH);

            if(resultSCH.UserFund.Count()>0)
                _context.RemoveRange(resultSCH.UserFund);
            

            if (request.Files != null && request.Files.Count() > 0)
                foreach (var item in request.Files)
            {

                string fullPath = Path.Combine(AppContext.BaseDirectory, "ExcelFile", $"{item.Name.Split(".")[0]}.enc");
                byte[] key = ExcelFromBase64.SaveBase64ExcelToFile(item.Content, fullPath);
                
                string base64 = "";
                int comma = item.Content.IndexOf(',');
                if (comma >= 0) base64 = item.Content.Substring(comma + 1);
                base64 = base64.Trim();

                byte[] contentBytes = Convert.FromBase64String(base64);
                ulong sizeInBytes = Convert.ToUInt64(contentBytes.LongLength);
                string base64Key = Convert.ToBase64String(key);
                var file = new FilesRawSys(resultSCH.Id, item.Name, sizeInBytes,base64Key, userId);
                await _context.AddAsync(file);

            }
           
            if (request.IdsFund!=null&&request.IdsFund.Count() > 0)
            {
                foreach (var item in request.IdsFund)
                {
                    var formRawRelated = new FormRawRelatedUserSys(resultSCH.Id, Guid.Parse(item));
                    _context.Add(formRawRelated);
                    resultSCH.UserFund.Add(formRawRelated);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            var log = new AddLogFormRaw(
          resultSCH.Id,
          $"ویرایش شده",
           Guid.Empty,
           userId,
           IpUser,
          _context);

            await log.SaveChange();
            return Result.Success(new ResponseWrite("کاربر با موفقیت ویرایش گردید."));
        }
    }
}