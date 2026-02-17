using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Extensions;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;
using ORG.BasicInfo.Domain.FormUserAggregate;
using System.Security.Claims;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class UpsertFormUserRequestHandler(
    FormsInfoDbContext context,
    IHttpContextAccessor httpContextAccessor,
    IValidator<UpsertFormUserRequest> validator) : IRequestHandler<UpsertFormUserRequest, Result<ResponseWrite>>
{
    private readonly FormsInfoDbContext _context = context;
    private readonly IValidator<UpsertFormUserRequest> _validator = validator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<ResponseWrite>> Handle(UpsertFormUserRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var ipUser = _httpContextAccessor.HttpContext.Items["IpUser"].ToString();
        if (IsRoleOrg == null)
        {
            return Result<ResponseWrite>.Unauthorized("دسترسی غیر مجاز");
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ResponseWrite>.Invalid(validationResult.AsErrors());
        }
        
        long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var chckResultFormRaw =await _context.Set<FormRawSys>()
                    .Include(f => f.UserFund)
                  .ThenInclude(fu => fu.User)
                  .FirstOrDefaultAsync(Item => Item.Id == Guid.Parse(request.IdFormRaw)&&Item.State==Domain.Abstractions.EntityState.Active);
        if(chckResultFormRaw==null || 
           (( chckResultFormRaw.ExpireDate>0&&nowUnixTimeSeconds > chckResultFormRaw.ExpireDate) &&
                (
                    (
                    chckResultFormRaw.IsPublicForm==false&&
                    chckResultFormRaw.UserFund.Any(item=>item.Id== userId) == false
                    )|| chckResultFormRaw.IsPublicForm == true
                ))
          )
        {
            return Result<ResponseWrite>.Unauthorized("دسترسی غیر مجاز");
        }

        if (request.Files!=null&& request.Files.Count()>0)
            foreach (var item in request.Files)
            {

                var validExcelFile = ExcelValidator.ValidateBase64IsExcel(item.Content);
                if (validExcelFile == ExcelCheckResult.NotBase64 || validExcelFile == ExcelCheckResult.NotExcel)
                {
                    return Result<ResponseWrite>.Invalid(new ValidationError("فایل های مربوطه نامعتبر است"));
                }

            }
       
       
        if (request.Id==""||request.Id==null){
          

            var form = new FormUserSys(
                request.Description,
                Guid.Parse(request.IdFormRaw),
                userId,
                userId);
            form.ChangeStateAction(StateAction.NoRead);
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
            
                var file = new FilesFormsSys(form.Id, item.Name, sizeInBytes, base64Key, userId);
                    await _context.AddAsync(file);

            }
            await _context.SaveChangesAsync(cancellationToken);
            
            var log = new AddLogFormUser(
               form.Id,
               form.IdFormRaw,
               $"ثبت فرم -{chckResultFormRaw.IdCode}- -{chckResultFormRaw.Title}- ",
               userId,
               userId,
               ipUser == null ? "" : ipUser,
               Domain.FormUserAggregate.StateAction.NoRead,
               _context);

            await log.SaveChange();
            return Result.Created(new ResponseWrite("فرم با موفقیت ایجاد گردید."));
        }
        else
        {
            var resultSCH = await _context.FormUserSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
            if (resultSCH == null)
            {
                return Result<ResponseWrite>.NotFound("فرم مورد نظر یافت نشد.");
            }
            if (resultSCH.IdUserRead != Guid.Empty)
                return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");
            
            if (resultSCH.StateAction != StateAction.NoRead && 
                resultSCH.StateAction != StateAction.Reject)
                return Result<ResponseWrite>.Forbidden("دسترسی غیر مجاز");

            resultSCH.ChangeValue(
                request.Description,
                userId);

            resultSCH.ChangeStateAction(StateAction.NoRead);
            _context.Update(resultSCH);

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
                    var file = new FilesFormsSys(resultSCH.Id, item.Name, sizeInBytes,base64Key, userId);
                    await _context.AddAsync(file);

                }
            
            await _context.SaveChangesAsync(cancellationToken);
          
            var log = new AddLogFormUser(
               resultSCH.Id,
               resultSCH.IdFormRaw,
               $"ویرایش فرم -{chckResultFormRaw.IdCode}- -{chckResultFormRaw.Title}- ",
               resultSCH.LUserCreate,
               userId, 
               ipUser==null?"": ipUser,
               resultSCH.StateAction,
            _context);
            
            await log.SaveChange();
            return Result.Success(new ResponseWrite("کاربر با موفقیت ویرایش گردید."));
        }
    }
}