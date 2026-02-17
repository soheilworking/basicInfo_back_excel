using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using BCrypt.Net;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.Domain.UserAggregate;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Extensions;
using ORG.BasicInfo.API.Features.Authentication.Response;

namespace ORG.BasicInfo.API.Features.Authentication;

public class GetVerifyCodeRequestHandler(
    FormsInfoDbContext dbContext,
    IValidator<GetVerifyCodeRequest> validator,
    IRedisCacheService redisService,
    IHttpContextAccessor httpContextAccessor
    //,ExternalDbSingletonService externalDb
    ) : IRequestHandler<GetVerifyCodeRequest, Result<ResponseAuth>>
{
    //private readonly ExternalDbSingletonService _externalDb = externalDb;
    private readonly FormsInfoDbContext _dbContext = dbContext;
    private readonly IValidator<GetVerifyCodeRequest> _validator = validator;
    private readonly IRedisCacheService _redisService= redisService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<ResponseAuth>> Handle(GetVerifyCodeRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ResponseAuth>.Invalid(validationResult.AsErrors());
        }
        var resultCaptcha=CheckCaptcha.CheckCapchaCode(request.CaptchaCode, _httpContextAccessor.HttpContext);
        if (resultCaptcha != "")
            return Result<ResponseAuth>.Error(resultCaptcha);

        

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => 
            user.FundCode == request.FundCode && 
            user.State == UserState.Active, cancellationToken);

        if (user == null
            //||user.ComparePassword(request.Password)==false
            )
        {
            return Result<ResponseAuth>.NotFound("کاربر پیدا نشد و یا حساب کاربری غیر فعال گردیده");
        }
        string ipSystem = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (user.IpUser != "::" && user.IpUser != ipSystem)
        {
            return Result<ResponseAuth>.Error( "ip نامعتبر" );
        }
        var mobile = user.Mobile;
        var rnd = new Random();
        int value = rnd.Next(0, 1_000_000); // 0 .. 999999
        string plainCode = value.ToString("D6");
        //await _externalDb.ExecuteNonQueryAsync(@$"
        //                INSERT INTO tblsms (
        //                    fldMes, fldMobileNumber, fldOK, flddate, fldcode,
        //                    fldMarhale, fldKargozar, fldSMS, fldSaveDate
        //                )
        //                VALUES (
        //                    N'کد ورود به سامانه فرم ها(به هیچ عنوان این کد را در اختیار دیگران قرار تدهید.): {plainCode}',
        //                    '0{mobile.ToString()}',
        //                    0,
        //                    bankdb.dbo.MiladiTOShamsi(GETDATE()),
        //                    3360387, 106, 1, 1,
        //                    bankdb.dbo.MiladiTOShamsi(GETDATE())
        //                )

        //            ");
        plainCode = "123456";
        //Console.WriteLine(plainCode);

        var verifyCodeHash = BCrypt.Net.BCrypt.EnhancedHashPassword(plainCode, HashType.SHA512);
        await _redisService.SetAsync<string>(mobile.ToString(), verifyCodeHash, TimeSpan.FromMinutes(2));
        


        return Result.Success(new ResponseAuth("کد تایید ارسال گردید.", mobile.ToString().Substring(6,4)));
    }


}
