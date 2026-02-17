using Mapster;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Logs.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;
using ORG.BasicInfo.Domain.FormUserAggregate;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.Logs.Queries
{
    public class GetItems : IGetItems<LogFormListResponse, LogFormInfoResponse, FormRawSys, FormsInfoDbContext>
    {

        public override async Task<ResponseQuery<LogFormListResponse>> GetInfoAll(int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserLogSyss
                .Join(
                dbContext.Users,
                log=>log.IdUser,
                user=>user.Id,
                (log, user) =>new LogFormListResponse
                {
                    Id=log.Id,
                    IdFormRaw=log.IdFormRaw,
                    IdFormUser =log.IdFormUser,
                    IdUser=log.IdUser,
                    NamesUser=$"{user.Name} {user.LastName}",
                    NameFund=user.FundName,
                    IsOrgUser=user.IsOrgUser==true?" کاربر سازمان ":" کاربر صندوق ",
                    IdCodeFund=user.FundCode,
                    StateAction =
                                log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                log.StateAction == StateAction.Read ? "خوانده شده" :
                                log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                log.StateAction == StateAction.Reject ? "رد شده" :
                                "",
                    Timestamp=log.Timestamp
                }
                    
                ).Join(
                    dbContext.FormRawSyss,
                    formUserLog=>formUserLog.IdFormRaw,
                    formRaw=>formRaw.Id,
                    (formUserLog, formRaw) => new LogFormListResponse
                    {
                        Id = formUserLog.Id,
                        IdFormRaw = formUserLog.IdFormRaw,
                        IdFormUser = formUserLog.IdFormUser,
                        IdUser=formUserLog.IdUser,
                        NamesUser = formUserLog.NamesUser,
                        NameFund = formUserLog.NameFund,
                        IsOrgUser = formUserLog.IsOrgUser,
                        IdCodeFund = formUserLog.IdCodeFund,
                        StateAction =formUserLog.StateAction ,
                        Timestamp = formUserLog.Timestamp,
                        IdCodeForm=formRaw.IdCode,
                        TitleForm=formRaw.Title
                    }

                )
            .AsNoTracking();
           
            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken); 

        }
     
        public async Task<ResponseQuery<LogFormListResponse>> GetInfoAllForForm(
            Guid idFormUser,
            int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserLogSyss
                .Where(item=>item.IdFormUser== idFormUser)
                .Join(
                dbContext.Users,
                log => log.IdUser,
                user => user.Id,
                (log, user) => new LogFormListResponse
                {
                    Id = log.Id,
                    IdFormRaw = log.IdFormRaw,
                    IdFormUser = log.IdFormUser,
                    IdUser = log.IdUser,
                    NamesUser = $"{user.Name} {user.LastName}",
                    NameFund = user.FundName,
                    IsOrgUser = user.IsOrgUser == true ? " کاربر سازمان " : " کاربر صندوق ",
                    IdCodeFund = user.FundCode,
                    StateAction =
                                log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                log.StateAction == StateAction.Read ? "خوانده شده" :
                                log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                log.StateAction == StateAction.Reject ? "رد شده" :
                                "",
                    Timestamp = log.Timestamp
                }

                ).Join(
                    dbContext.FormRawSyss,
                    formUserLog => formUserLog.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUserLog, formRaw) => new LogFormListResponse
                    {
                        Id = formUserLog.Id,
                        IdFormRaw = formUserLog.IdFormRaw,
                        IdFormUser = formUserLog.IdFormUser,
                        NamesUser = formUserLog.NamesUser,
                        NameFund = formUserLog.NameFund,
                        IsOrgUser = formUserLog.IsOrgUser,
                        IdCodeFund = formUserLog.IdCodeFund,
                        StateAction = formUserLog.StateAction,
                        Timestamp = formUserLog.Timestamp,
                        IdCodeForm = formRaw.IdCode,
                        TitleForm = formRaw.Title,
                        IdUser = formUserLog.IdUser
                    }

                )
            .AsNoTracking();

            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }
        public async Task<IEnumerable<LogFormInfoResponseUser>> GetInfoAllForFormUser(
            Guid idFormUser,
                    FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {

            var Forms =await dbContext.FormUserLogSyss
                .Where(item => item.IdFormUser == idFormUser)
                .Join(
                dbContext.Users,
                log => log.IdUserRead,
                user => user.Id,
                (log, user) => new LogFormInfoResponseUser
                {

                    StateAction =
                                log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                log.StateAction == StateAction.Read ? "خوانده شده" :
                                log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                log.StateAction == StateAction.Reject ? "رد شده" :
                                "",
                    Timestamp = log.Timestamp,
                    DescriptionLog = log.Description,
                    NamesUserAct = user.Name + " " + user.LastName
                }

                )
            .AsNoTracking()
            .ToArrayAsync();
            return Forms;
            //return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }

        public async Task<ResponseQuery<LogFormListResponse>> GetInfoAllForUser(
          IEnumerable<Guid> idUser,
          int pageNumber,
          int pageSize,
          IEnumerable<string[]> sortFields,
          FormsInfoDbContext dbContext,
          CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserLogSyss
                .Where(item => idUser.Contains(item.IdUser))
                .Join(
                dbContext.Users,
                log => log.IdUser,
                user => user.Id,
                (log, user) => new LogFormListResponse
                {
                    Id = log.Id,
                    IdFormRaw = log.IdFormRaw,
                    IdFormUser = log.IdFormUser,
                    NamesUser = $"{user.Name} {user.LastName}",
                    NameFund = user.FundName,
                    IsOrgUser = user.IsOrgUser == true ? " کاربر سازمان " : " کاربر صندوق ",
                    IdCodeFund = user.FundCode,
                    StateAction =
                                log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                log.StateAction == StateAction.Read ? "خوانده شده" :
                                log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                log.StateAction == StateAction.Reject ? "رد شده" :
                                "",
                    Timestamp = log.Timestamp,
                    IdUser = log.IdUser
                }

                ).Join(
                    dbContext.FormRawSyss,
                    formUserLog => formUserLog.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUserLog, formRaw) => new LogFormListResponse
                    {
                        Id = formUserLog.Id,
                        IdFormRaw = formUserLog.IdFormRaw,
                        IdFormUser = formUserLog.IdFormUser,
                        NamesUser = formUserLog.NamesUser,
                        NameFund = formUserLog.NameFund,
                        IsOrgUser = formUserLog.IsOrgUser,
                        IdCodeFund = formUserLog.IdCodeFund,
                        StateAction = formUserLog.StateAction,
                        Timestamp = formUserLog.Timestamp,
                        IdCodeForm = formRaw.IdCode,
                        TitleForm = formRaw.Title,
                        IdUser = formUserLog.IdUser
                    }

                )
            .AsNoTracking();

            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }
        
  
        public override async Task<LogFormInfoResponse> GetInfoWithId(
            Guid id,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {
            var result =await dbContext.FormUserLogSyss
          .Where(item => item.Id == id)
                  .Join(
                  dbContext.Users,
                  log => log.IdUser,
                  user => user.Id,
                  (log, user) => new LogFormInfoResponse
                  {
                      Id = log.Id,
                      IdUserRead=log.IdUserRead,
                      IdFormRaw = log.IdFormRaw,
                      IdFormUser = log.IdFormUser,
                      NamesUser = $"{user.Name} {user.LastName}",
                      NameFund = user.FundName,
                      IsOrgUser = user.IsOrgUser == true ? " کاربر سازمان " : " کاربر صندوق ",
                      IdCodeFund = user.FundCode,
                      StateAction =
                                  log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                  log.StateAction == StateAction.Read ? "خوانده شده" :
                                  log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                  log.StateAction == StateAction.Reject ? "رد شده" :
                                  "",
                      Timestamp = log.Timestamp,
                      IdUser = log.IdUser,

                      DescriptionLog=log.Description
                  }

                  ).Join(
                      dbContext.FormRawSyss,
                      formUserLog => formUserLog.IdFormRaw,
                      formRaw => formRaw.Id,
                      (formUserLog, formRaw) => new LogFormInfoResponse
                      {
                          Id = formUserLog.Id,
                          IdFormRaw = formUserLog.IdFormRaw,
                          IdFormUser = formUserLog.IdFormUser,
                          NamesUser = formUserLog.NamesUser,
                          NameFund = formUserLog.NameFund,
                          IsOrgUser = formUserLog.IsOrgUser,
                          IdCodeFund = formUserLog.IdCodeFund,
                          StateAction = formUserLog.StateAction,
                          Timestamp = formUserLog.Timestamp,
                          IdCodeForm = formRaw.IdCode,
                          TitleForm = formRaw.Title,
                          IdUser = formUserLog.IdUser,
                            DescriptionLog = formUserLog.DescriptionLog,
                          IdUserRead = formUserLog.IdUserRead
                      }

                  ).Join(
                dbContext.Users,
                formUserLog => formUserLog.IdUserRead,
                user=>user.Id,
                (formUserLog, user) => new LogFormInfoResponse
                {
                    Id = formUserLog.Id,
                    IdFormRaw = formUserLog.IdFormRaw,
                    IdFormUser = formUserLog.IdFormUser,
                    NamesUser = formUserLog.NamesUser,
                    NameFund = formUserLog.NameFund,
                    IsOrgUser = formUserLog.IsOrgUser,
                    IdCodeFund = formUserLog.IdCodeFund,
                    StateAction = formUserLog.StateAction,
                    Timestamp = formUserLog.Timestamp,
                    IdCodeForm = formUserLog.IdCodeForm,
                    TitleForm = formUserLog.TitleForm,
                    IdUser = formUserLog.IdUser,
                    DescriptionLog = formUserLog.DescriptionLog,
                    IdUserRead = formUserLog.IdUserRead,
                    IdCodeUserRead=user.FundCode,
                    NamesUserRead=$"{user.Name} {user.LastName}"
                })
              .AsNoTracking().FirstOrDefaultAsync();


           
         
            return result;
        }
        public  async Task<LogFormInfoResponse> GetInfoRejectWithIdForUser(Guid id,Guid idUser, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {

            var result = await dbContext.FormUserLogSyss
          .Where(item => item.Id == id&&item.IdUser==idUser&&item.StateAction==StateAction.Reject)
                  .Join(
                  dbContext.Users,
                  log => log.IdUser,
                  user => user.Id,
                  (log, user) => new LogFormInfoResponse
                  {
                      Id = log.Id,
                      IdFormRaw = log.IdFormRaw,
                      IdFormUser = log.IdFormUser,
                      NamesUser = $"{user.Name} {user.LastName}",
                      NameFund = user.FundName,
                      IsOrgUser = user.IsOrgUser == true ? " کاربر سازمان " : " کاربر صندوق ",
                      IdCodeFund = user.FundCode,
                      StateAction =
                                  log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                  log.StateAction == StateAction.Read ? "خوانده شده" :
                                  log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                  log.StateAction == StateAction.Reject ? "رد شده" :
                                  "",
                      Timestamp = log.Timestamp,
                      IdUser = log.IdUser,
                      DescriptionLog = log.Description
                  }

                  ).Join(
                      dbContext.FormRawSyss,
                      formUserLog => formUserLog.IdFormRaw,
                      formRaw => formRaw.Id,
                      (formUserLog, formRaw) => new LogFormInfoResponse
                      {
                          Id = formUserLog.Id,
                          IdFormRaw = formUserLog.IdFormRaw,
                          IdFormUser = formUserLog.IdFormUser,
                          NamesUser = formUserLog.NamesUser,
                          NameFund = formUserLog.NameFund,
                          IsOrgUser = formUserLog.IsOrgUser,
                          IdCodeFund = formUserLog.IdCodeFund,
                          StateAction = formUserLog.StateAction,
                          Timestamp = formUserLog.Timestamp,
                          IdCodeForm = formRaw.IdCode,
                          TitleForm = formRaw.Title,
                          IdUser = formUserLog.IdUser,
                          DescriptionLog = formUserLog.DescriptionLog
                      }

                  )
              .AsNoTracking().FirstOrDefaultAsync();

            return result;
        }
        public override async Task<LogFormListResponse> GetInfoWithIdCode(ulong idCode, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var result = await dbContext.FormUserLogSyss
                  .Join(
                  dbContext.Users,
                  log => log.IdUser,
                  user => user.Id,
                  (log, user) => new LogFormInfoResponse
                  {
                      Id = log.Id,
                      IdFormRaw = log.IdFormRaw,
                      IdFormUser = log.IdFormUser,
                      NamesUser = $"{user.Name} {user.LastName}",
                      NameFund = user.FundName,
                      IsOrgUser = user.IsOrgUser == true ? " کاربر سازمان " : " کاربر صندوق ",
                      IdCodeFund = user.FundCode,
                      StateAction =
                                  log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                  log.StateAction == StateAction.Read ? "خوانده شده" :
                                  log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                  log.StateAction == StateAction.Reject ? "رد شده" :
                                  "",
                      Timestamp = log.Timestamp,
                      IdUser = log.IdUser,
                      DescriptionLog = log.Description
                  }

                  ).Join(
                      dbContext.FormRawSyss,
                      formUserLog => formUserLog.IdFormRaw,
                      formRaw => formRaw.Id,
                      (formUserLog, formRaw) => new LogFormInfoResponse
                      {
                          Id = formUserLog.Id,
                          IdFormRaw = formUserLog.IdFormRaw,
                          IdFormUser = formUserLog.IdFormUser,
                          NamesUser = formUserLog.NamesUser,
                          NameFund = formUserLog.NameFund,
                          IsOrgUser = formUserLog.IsOrgUser,
                          IdCodeFund = formUserLog.IdCodeFund,
                          StateAction = formUserLog.StateAction,
                          Timestamp = formUserLog.Timestamp,
                          IdCodeForm = formRaw.IdCode,
                          TitleForm = formRaw.Title,
                          IdUser = formUserLog.IdUser,
                          DescriptionLog = formUserLog.DescriptionLog
                      }

                  ).FirstOrDefaultAsync(item => item.IdCodeForm == idCode);



            return result.Adapt<LogFormListResponse>();
        }
        public  async Task<LogFormListResponse> GetInfoWithIdCodeForUser(
            ulong idCode,
             IEnumerable<Guid> idUser,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {
            var result = await dbContext.FormUserLogSyss
                .Where(item => idUser.Contains(item.IdUser))
                  .Join(
                  dbContext.Users,
                  log => log.IdUser,
                  user => user.Id,
                  (log, user) => new LogFormInfoResponse
                  {
                      Id = log.Id,
                      IdFormRaw = log.IdFormRaw,
                      IdFormUser = log.IdFormUser,
                      NamesUser = $"{user.Name} {user.LastName}",
                      NameFund = user.FundName,
                      IsOrgUser = user.IsOrgUser == true ? " کاربر سازمان " : " کاربر صندوق ",
                      IdCodeFund = user.FundCode,
                      StateAction =
                                  log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                  log.StateAction == StateAction.Read ? "خوانده شده" :
                                  log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                  log.StateAction == StateAction.Reject ? "رد شده" :
                                  "",
                      Timestamp = log.Timestamp,
                      IdUser = log.IdUser,
                      DescriptionLog = log.Description
                  }

                  ).Join(
                      dbContext.FormRawSyss,
                      formUserLog => formUserLog.IdFormRaw,
                      formRaw => formRaw.Id,
                      (formUserLog, formRaw) => new LogFormInfoResponse
                      {
                          Id = formUserLog.Id,
                          IdFormRaw = formUserLog.IdFormRaw,
                          IdFormUser = formUserLog.IdFormUser,
                          NamesUser = formUserLog.NamesUser,
                          NameFund = formUserLog.NameFund,
                          IsOrgUser = formUserLog.IsOrgUser,
                          IdCodeFund = formUserLog.IdCodeFund,
                          StateAction = formUserLog.StateAction,
                          Timestamp = formUserLog.Timestamp,
                          IdCodeForm = formRaw.IdCode,
                          TitleForm = formRaw.Title,
                          IdUser = formUserLog.IdUser,
                          DescriptionLog = formUserLog.DescriptionLog
                      }

                  ).FirstOrDefaultAsync(item => item.IdCodeForm == idCode);



            return result.Adapt<LogFormListResponse>();
        }

        public override async Task<ResponseQuery<LogFormListResponse>> SearchWithNameLike(string name, 
            int pageNumber, 
            int pageSize,
             IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken)
        {
            var result =  dbContext.FormUserLogSyss
           .Join(
           dbContext.Users,
           log => log.IdUser,
           user => user.Id,
           (log, user) => new LogFormInfoResponse
           {
               Id = log.Id,
               IdFormRaw = log.IdFormRaw,
               IdFormUser = log.IdFormUser,
               NamesUser = $"{user.Name} {user.LastName}",
               NameFund = user.FundName,
               IsOrgUser = user.IsOrgUser == true ? " کاربر سازمان " : " کاربر صندوق ",
               IdCodeFund = user.FundCode,
               StateAction =
                           log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                           log.StateAction == StateAction.Read ? "خوانده شده" :
                           log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                           log.StateAction == StateAction.Reject ? "رد شده" :
                           "",
               Timestamp = log.Timestamp,
               IdUser = log.IdUser,
               DescriptionLog = log.Description
           }

           ).Join(
               dbContext.FormRawSyss,
               formUserLog => formUserLog.IdFormRaw,
               formRaw => formRaw.Id,
               (formUserLog, formRaw) => new LogFormInfoResponse
               {
                   Id = formUserLog.Id,
                   IdFormRaw = formUserLog.IdFormRaw,
                   IdFormUser = formUserLog.IdFormUser,
                   NamesUser = formUserLog.NamesUser,
                   NameFund = formUserLog.NameFund,
                   IsOrgUser = formUserLog.IsOrgUser,
                   IdCodeFund = formUserLog.IdCodeFund,
                   StateAction = formUserLog.StateAction,
                   Timestamp = formUserLog.Timestamp,
                   IdCodeForm = formRaw.IdCode,
                   TitleForm = formRaw.Title,
                   IdUser = formUserLog.IdUser,
                   DescriptionLog = formUserLog.DescriptionLog
               }

           )
                  .Where(FormsItem => EF.Functions.Like(FormsItem.TitleForm, $"%{name}%")|| EF.Functions.Like(FormsItem.DescriptionLog, $"%{name}%"));
             
            return await paginationData(pageNumber, pageSize, result, sortFields, cancellationToken);

        }


        public  async Task<ResponseQuery<LogFormListResponse>> SearchWithNameLikeForUser(string name,
        IEnumerable<Guid> idUser,
            int pageNumber,
    int pageSize,
     IEnumerable<string[]> sortFields,
    FormsInfoDbContext dbContext,
    CancellationToken cancellationToken)
        {
            var result = dbContext.FormUserLogSyss
             .Where(item=>idUser.Contains(item.IdUser))

           .Join(
           dbContext.Users,
           log => log.IdUser,
           user => user.Id,
           (log, user) => new LogFormInfoResponse
           {
               Id = log.Id,
               IdFormRaw = log.IdFormRaw,
               IdFormUser = log.IdFormUser,
               NamesUser = $"{user.Name} {user.LastName}",
               NameFund = user.FundName,
               IsOrgUser = user.IsOrgUser == true ? " کاربر سازمان " : " کاربر صندوق ",
               IdCodeFund = user.FundCode,
               StateAction =
                           log.StateAction == StateAction.NoRead ? "خوانده نشده" :
                           log.StateAction == StateAction.Read ? "خوانده شده" :
                           log.StateAction == StateAction.Accept ? "پذیرفته شده" :
                           log.StateAction == StateAction.Reject ? "رد شده" :
                           "",
               Timestamp = log.Timestamp,
               IdUser = log.IdUser,
               DescriptionLog = log.Description
           }

           ).Join(
               dbContext.FormRawSyss,
               formUserLog => formUserLog.IdFormRaw,
               formRaw => formRaw.Id,
               (formUserLog, formRaw) => new LogFormInfoResponse
               {
                   Id = formUserLog.Id,
                   IdFormRaw = formUserLog.IdFormRaw,
                   IdFormUser = formUserLog.IdFormUser,
                   NamesUser = formUserLog.NamesUser,
                   NameFund = formUserLog.NameFund,
                   IsOrgUser = formUserLog.IsOrgUser,
                   IdCodeFund = formUserLog.IdCodeFund,
                   StateAction = formUserLog.StateAction,
                   Timestamp = formUserLog.Timestamp,
                   IdCodeForm = formRaw.IdCode,
                   TitleForm = formRaw.Title,
                   IdUser = formUserLog.IdUser,
                   DescriptionLog = formUserLog.DescriptionLog
               }

           )
                  .Where(FormsItem => EF.Functions.Like(FormsItem.TitleForm, $"%{name}%") || EF.Functions.Like(FormsItem.DescriptionLog, $"%{name}%"));

            return await paginationData(pageNumber, pageSize, result, sortFields, cancellationToken);

        }


    }
}
 