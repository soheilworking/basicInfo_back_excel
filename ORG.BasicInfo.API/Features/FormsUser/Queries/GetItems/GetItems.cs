using Mapster;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;
using ORG.BasicInfo.Domain.FormUserAggregate;

namespace ORG.BasicInfo.API.Features.FormsUser.Queries
{
    public class GetItems : IGetItems<FormUserListResponse, FormUserInfoResponse, FormUserSys, FormsInfoDbContext>
    {

        public override async Task<ResponseQuery<FormUserListResponse>> GetInfoAll(int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserSyss
                .AsNoTracking()
                .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateActionForm = formUser.StateAction ==
                            StateAction.NoRead ? "خوانده نشده" :
                            formUser.StateAction == StateAction.Read ? "خوانده شده" :
                            formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                            formUser.StateAction == StateAction.Reject ? "رد شده" : "",
                        IdUser =formUser.IdUser,
                        IsStrongRow = formUser.StateAction ==
                            StateAction.NoRead ? true :false
                    }
                ).Join(dbContext.Users,
                    res => res.IdUser,
                    user => user.Id,
                    (res, user) => new FormUserListResponse
                    {
                        Id = res.Id,
                        IdCode = res.IdCode,
                        Title = res.Title,
                        TCreate = res.TCreate,
                        TEdit = res.TEdit,
                        StateActionForm = res.StateActionForm,
                        NameFund =user.FundName,
                        IdCodeFund=user.FundCode,
                          IsStrongRow = res.IsStrongRow
                    });



            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken); 

        }

        public async Task<ResponseQuery<FormUserListResponse>> GetInfoAllStateAction(
            int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext,
            StateAction stateAcion,
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserSyss
                .AsNoTracking()
                .Where(item=>item.StateAction == stateAcion)
                .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateActionForm = formUser.StateAction ==
                            StateAction.NoRead ? "خوانده نشده" :
                            formUser.StateAction == StateAction.Read ? "خوانده شده" :
                            formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                            formUser.StateAction == StateAction.Reject ? "رد شده" : "",
                        IdUser = formUser.IdUser
                    }
                ).Join(dbContext.Users,
                    res => res.IdUser,
                    user => user.Id,
                    (res, user) => new FormUserListResponse
                    {
                        Id = res.Id,
                        IdCode = res.IdCode,
                        Title = res.Title,
                        TCreate = res.TCreate,
                        TEdit = res.TEdit,
                        StateActionForm = res.StateActionForm,
                        NameFund = user.FundName,
                        IdCodeFund = user.FundCode
                    });



            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }

        public override async Task<FormUserInfoResponse> GetInfoWithId(Guid id, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var Forms = await dbContext.FormUserSyss
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);
    
            return Forms.Adapt<FormUserInfoResponse>();
        }

        public override async Task<FormUserListResponse> GetInfoWithIdCode(ulong idCode, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var Forms = await dbContext.FormUserSyss
                       .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateActionForm = formUser.StateAction ==
                            StateAction.NoRead ? "خوانده نشده" :
                            formUser.StateAction == StateAction.Read ? "خوانده شده" :
                            formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                            formUser.StateAction == StateAction.Reject ? "رد شده" : "",
                        IdUser = formUser.IdUser,
                        IsStrongRow = formUser.StateAction ==
                            StateAction.NoRead ? true : false
                    }
                ).Join(dbContext.Users,
                    res => res.IdUser,
                    user => user.Id,
                    (res, user) => new FormUserListResponse
                    {
                        Id = res.Id,
                        IdCode = res.IdCode,
                        Title = res.Title,
                        TCreate = res.TCreate,
                        TEdit = res.TEdit,
                        StateActionForm = res.StateActionForm,
                        NameFund = user.FundName,
                        IdCodeFund = user.FundCode,
                        IsStrongRow=res.IsStrongRow
                        
                    })

                        .AsNoTracking()
           .FirstOrDefaultAsync(item => item.IdCode == idCode);
    
              

            return Forms.Adapt<FormUserListResponse>();
        }
        public  async Task<FormUserListResponse> GetInfoWithIdCodeStateAction(ulong idCode, StateAction stateAcion, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var Forms = await dbContext.FormUserSyss
                        .Where(item=>item.StateAction==stateAcion)
                       .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateActionForm = formUser.StateAction ==
                            StateAction.NoRead ? "خوانده نشده" :
                            formUser.StateAction == StateAction.Read ? "خوانده شده" :
                            formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                            formUser.StateAction == StateAction.Reject ? "رد شده" : "",
                        IdUser = formUser.IdUser
                    }
                ).Join(dbContext.Users,
                    res => res.IdUser,
                    user => user.Id,
                    (res, user) => new FormUserListResponse
                    {
                        Id = res.Id,
                        IdCode = res.IdCode,
                        Title = res.Title,
                        TCreate = res.TCreate,
                        TEdit = res.TEdit,
                        StateActionForm = res.StateActionForm,
                        NameFund = user.FundName,
                        IdCodeFund = user.FundCode
                    })

                        .AsNoTracking()
           .FirstOrDefaultAsync(item => item.IdCode == idCode);



            return Forms.Adapt<FormUserListResponse>();
        }


        public override async Task<ResponseQuery<FormUserListResponse>> SearchWithNameLike(string name, 
            int pageNumber, 
            int pageSize,
             IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserSyss
                         .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateActionForm =
                           formUser.StateAction == StateAction.NoRead ? "خوانده نشده" :
                           formUser.StateAction == StateAction.Read ? "خوانده شده" :
                           formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                           formUser.StateAction == StateAction.Reject ? "رد شده" :
                           "",
                        
                        //IsRead = formUser.IdUserRead != Guid.Empty ? true : false,
                        IdUser = formUser.IdUser,
                        IsStrongRow = formUser.StateAction ==
                            StateAction.NoRead ? true : false
                    }
                ).Join(dbContext.Users,
                    res => res.IdUser,
                    user => user.Id,
                    (res, user) => new FormUserListResponse
                    {
                        Id = res.Id,
                        IdCode = res.IdCode,
                        Title = res.Title,
                        TCreate = res.TCreate,
                        TEdit = res.TEdit,
                        StateActionForm = res.StateActionForm,
                        NameFund = user.FundName,
                        IdCodeFund = user.FundCode,
                        IsStrongRow=res.IsStrongRow
                    })
                  .AsNoTracking()
                  .Where(
                FormsItem => 
                  EF.Functions.Like(FormsItem.Title, $"%{name}%")|| 
                  EF.Functions.Like(FormsItem.NameFund, $"%{name}%") ||
                  EF.Functions.Like(FormsItem.IdCodeFund.ToString(), $"%{name}%")
                  );
             
            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }
        public  async Task<ResponseQuery<FormUserListResponse>> SearchWithNameLikeStateAction(
            string name,
            int pageNumber,
            int pageSize,
            StateAction stateAcion,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserSyss
                .Where(item=>item.StateAction==stateAcion)
                .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateActionForm =
                           formUser.StateAction == StateAction.NoRead ? "خوانده نشده" :
                           formUser.StateAction == StateAction.Read ? "خوانده شده" :
                           formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                           formUser.StateAction == StateAction.Reject ? "رد شده" :
                           "",
                        IdUser = formUser.IdUser
                    }
                ).Join(dbContext.Users,
                    res => res.IdUser,
                    user => user.Id,
                    (res, user) => new FormUserListResponse
                    {
                        Id = res.Id,
                        IdCode = res.IdCode,
                        Title = res.Title,
                        TCreate = res.TCreate,
                        TEdit = res.TEdit,
                        StateActionForm = res.StateActionForm,
                        NameFund = user.FundName,
                        IdCodeFund = user.FundCode,
                        
                    })
                  .AsNoTracking()
                  .Where(
                FormsItem =>
                  EF.Functions.Like(FormsItem.Title, $"%{name}%") ||
                  EF.Functions.Like(FormsItem.NameFund, $"%{name}%") ||
                  EF.Functions.Like(FormsItem.IdCodeFund.ToString(), $"%{name}%")
                  );

            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }
  
        public async Task<RepeatedResult> GetIsRpeatedIdCode(Guid id, ulong idCode, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var result = await ValidateUniqueAsync(id, "IdCode", idCode, "شماره فرم", dbContext, cancellationToken);
            return result;

        }

        // user data


        public async Task<ResponseQuery<FormUserListResponse>> GetInfoAllForUser(int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            IEnumerable<Guid> idUser,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserSyss
               .Where(item => idUser.Contains(item.IdUser))
               .Join(dbContext.Users,
                    formUser=> formUser.IdUser,
                    user=>user.Id,
                    (formUser, user) => new
                    {
                        formUser.Id,
                        formUser.IdFormRaw,
                        formUser.IdUser,
                        formUser.IdUserRead,
                        formUser.LUserCreate,
                        formUser.LUserEdit,
                        formUser.TCreate,
                        formUser.TEdit,
                        formUser.StateAction,
                    }
               )
            .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                    
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateAction= formUser.StateAction,
                        StateActionForm =
                           formUser.StateAction == StateAction.NoRead ? "خوانده نشده" :
                           formUser.StateAction == StateAction.Read ? "خوانده شده" :
                           formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                           formUser.StateAction == StateAction.Reject ? "رد شده" :
                           "",
                        IdUser = formUser.IdUser,
                        IsStrongRow = formUser.StateAction ==
                            StateAction.Reject ? true : false
                    }
            )

            .AsNoTracking();

            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }
        public async Task<ResponseQuery<FormUserListResponse>> GetInfoAllForUserStateAction(int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            IEnumerable<Guid> idUser,
             StateAction stateAcion,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {
            
            var Forms = dbContext.FormUserSyss
                .Where(item => idUser.Contains(item.IdUser) && item.StateAction==stateAcion)
            .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateAction = formUser.StateAction,
                        StateActionForm =
                           formUser.StateAction == StateAction.NoRead ? "خوانده نشده" :
                           formUser.StateAction == StateAction.Read ? "خوانده شده" :
                           formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                           formUser.StateAction == StateAction.Reject ? "رد شده" :
                           "",
                        IdUser = formUser.IdUser
                    }
            )

            .AsNoTracking();

            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }
        public async Task<FormUserInfoResponse> GetInfoWithIdForUser(Guid id,IEnumerable<Guid> idUser, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {

            var Forms = await dbContext.FormUserSyss
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id && idUser.Contains(item.IdUser));

            return Forms.Adapt<FormUserInfoResponse>();
        }

        public async Task<FormUserListResponse> GetInfoWithIdCodeForUser(ulong idCode,
           IEnumerable<Guid> idUser,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {
            var Forms = await dbContext.FormUserSyss
                       .Join(dbContext.FormRawSyss,
                                formUser => formUser.IdFormRaw,
                                formRaw => formRaw.Id,
                                (formUser, formRaw) => new FormUserListResponse
                                {
                                    Id = formUser.Id,
                                    IdCode = formRaw.IdCode,
                                    Title = formRaw.Title,
                                    TCreate = formUser.TCreate,
                                    TEdit = formUser.TEdit,
                                    StateAction = formUser.StateAction,
                                    StateActionForm =
                                       formUser.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                       formUser.StateAction == StateAction.Read ? "خوانده شده" :
                                       formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                       formUser.StateAction == StateAction.Reject ? "رد شده" :
                                       "",
                                    IdUser = formUser.IdUser,
                                    IsStrongRow = formUser.StateAction ==
                                            StateAction.Reject ? true : false
                                }
                        )

                        .AsNoTracking()
           .FirstOrDefaultAsync(item => item.IdCode == idCode && idUser.Contains(item.IdUser));



            return Forms.Adapt<FormUserListResponse>();
        }
        public async Task<FormUserListResponse> GetInfoWithIdCodeForUserStateAction(ulong idCode,
           IEnumerable<Guid> idUser,
            StateAction stateAcion,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {
            var Forms = await dbContext.FormUserSyss
                        
                       .Join(dbContext.FormRawSyss,
                                formUser => formUser.IdFormRaw,

                                formRaw => formRaw.Id,
                                (formUser, formRaw) => new FormUserListResponse
                                {
                                    Id = formUser.Id,
                                    IdCode = formRaw.IdCode,
                                    Title = formRaw.Title,
                                    TCreate = formUser.TCreate,

                                    TEdit = formUser.TEdit,
                                    StateActionForm =
                                           formUser.StateAction == StateAction.NoRead ? "خوانده نشده" :
                                           formUser.StateAction == StateAction.Read ? "خوانده شده" :
                                           formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                                           formUser.StateAction == StateAction.Reject ? "رد شده" :
                                           "",
                                    IdUser = formUser.IdUser,
                                    StateAction=formUser.StateAction
                                }
                        )

                        .AsNoTracking()
                           .FirstOrDefaultAsync(item => item.IdCode == idCode &&
                           idUser.Contains(item.IdUser) &&
                           item.StateAction==stateAcion);



            return Forms.Adapt<FormUserListResponse>();
        }

        public async Task<ResponseQuery<FormUserListResponse>> SearchWithNameLikeForUser(
            string name,
            IEnumerable<Guid> idUser,
            int pageNumber,
            int pageSize,
             IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserSyss
                         .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        
                        TCreate = formUser.TCreate,
                        StateAction = formUser.StateAction,
                        TEdit = formUser.TEdit,
                        StateActionForm = formUser.StateAction == 
                            StateAction.NoRead ? "خوانده نشده" : 
                            formUser.StateAction == StateAction.Read ? "خوانده شده" : 
                            formUser.StateAction == StateAction.Accept ? "پذیرفته شده" : 
                            formUser.StateAction == StateAction.Reject ? "رد شده" : "",

                        IdUser = formUser.IdUser,
                        IsStrongRow = formUser.StateAction ==
                                            StateAction.Reject ? true : false
                    }
                ).Where(item => idUser.Contains(item.IdUser))
                .Where(
                FormsItem => EF.Functions.Like(FormsItem.Title, $"%{name}%")
                  );

            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }

        public async Task<ResponseQuery<FormUserListResponse>> SearchWithNameLikeForUserStateAction(
            string name,
            IEnumerable<Guid> idUser,
            int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            StateAction stateAcion,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormUserSyss
                         .Join(dbContext.FormRawSyss,
                    formUser => formUser.IdFormRaw,
                    formRaw => formRaw.Id,
                    (formUser, formRaw) => new FormUserListResponse
                    {
                        Id = formUser.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        TCreate = formUser.TCreate,
                        TEdit = formUser.TEdit,
                        StateActionForm = formUser.StateAction ==
                            StateAction.NoRead ? "خوانده نشده" :
                            formUser.StateAction == StateAction.Read ? "خوانده شده" :
                            formUser.StateAction == StateAction.Accept ? "پذیرفته شده" :
                            formUser.StateAction == StateAction.Reject ? "رد شده" : "",

                        IdUser = formUser.IdUser,
                        StateAction=formUser.StateAction
                    }
                ).Where(item => idUser.Contains(item.IdUser) && item.StateAction == stateAcion)
                .Where(
                FormsItem => EF.Functions.Like(FormsItem.Title, $"%{name}%")
                  );

            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }


 

        public async Task<ResponseQuery<FormUserListResponse>> GetInfoAllUserNotSend(
           int pageNumber,
           int pageSize,
           IEnumerable<string[]> sortFields,
           FormsInfoDbContext dbContext,
           CancellationToken cancellationToken)
        {
            // فرم‌های عمومی که کاربرها ارسال نکرده‌اند
            var formsPublic = from formRaw in dbContext.FormRawSyss 
                              where formRaw.State == Domain.Abstractions.EntityState.Active && 
                              formRaw.IsPublicForm from user in dbContext.Users 
                              where user.IsOrgUser == false && 
                              !dbContext.FormUserSyss.Any(fu => fu.IdFormRaw == formRaw.Id && fu.IdUser == user.Id) 
                              select new FormUserListResponse 
                              { Id = Guid.NewGuid(), IdFormRaw = formRaw.Id
                              , IdCode = formRaw.IdCode, Title = formRaw.Title
                              , IdUser = user.Id, IdCodeFund = user.FundCode
                              , NameFund = user.FundName, IsPublicForm = formRaw.IsPublicForm
                              , ExpireDate = formRaw.ExpireDate };

            // فرم‌های خصوصی که کاربرها ارسال نکرده‌اند
            var formsPrivate = from form in dbContext.FormRawSyss 
                               where form.State == Domain.Abstractions.EntityState.Active && 
                               form.IsPublicForm == false from uf in form.UserFund 
                               let user = uf.User 
                               where !dbContext.FormUserSyss.Any(fu => fu.IdFormRaw == uf.IdForm && fu.IdUser == uf.IdUser) 
                               select new FormUserListResponse { Id = Guid.NewGuid(),
                                   IdFormRaw = uf.IdForm,
                                   Title = form.Title,
                                   IdCode = form.IdCode,
                                   ExpireDate = form.ExpireDate,
                                   IdUser = uf.IdUser,
                                   NameFund = user.FundName,
                                   IdCodeFund = user.FundCode,
                                   IsPublicForm = form.IsPublicForm };

            // ترکیب فرم‌های عمومی و خصوصی
            var unionQuery = formsPrivate.Union(formsPublic);

            return await paginationData(pageNumber, pageSize, unionQuery, sortFields, cancellationToken);
        }

        public async Task<ResponseQuery<FormUserListResponse>> GetInfoAllUserNotSendForUserOrg(
           int pageNumber,
           int pageSize,
           IEnumerable<Guid> idUser,
           IEnumerable<string[]> sortFields,
           FormsInfoDbContext dbContext,
           CancellationToken cancellationToken)
        {
            // فرم‌های عمومی که کاربرها ارسال نکرده‌اند
            var formsPublic = from formRaw in dbContext.FormRawSyss
                              where formRaw.State == Domain.Abstractions.EntityState.Active &&
                              formRaw.IsPublicForm
                              from user in dbContext.Users
                              where user.IsOrgUser == false &&
                              idUser.Contains(user.Id)&&
                              !dbContext.FormUserSyss.Any(fu => fu.IdFormRaw == formRaw.Id && fu.IdUser == user.Id)
                              select new FormUserListResponse
                              {
                                  Id = Guid.NewGuid(),
                                  IdFormRaw = formRaw.Id
                              ,
                                  IdCode = formRaw.IdCode,
                                  Title = formRaw.Title
                              ,
                                  IdUser = user.Id,
                                  IdCodeFund = user.FundCode
                              ,
                                  NameFund = user.FundName,
                                  IsPublicForm = formRaw.IsPublicForm
                              ,
                                  ExpireDate = formRaw.ExpireDate
                              };

            // فرم‌های خصوصی که کاربرها ارسال نکرده‌اند
            var formsPrivate = from form in dbContext.FormRawSyss
                               where form.State == Domain.Abstractions.EntityState.Active &&
                               form.IsPublicForm == false
                               from uf in form.UserFund
                               let user = uf.User
                               where idUser.Contains(user.Id)
                               where !dbContext.FormUserSyss.Any(fu => fu.IdFormRaw == uf.IdForm && fu.IdUser == uf.IdUser)
                               select new FormUserListResponse
                               {
                                   Id = Guid.NewGuid(),
                                   IdFormRaw = uf.IdForm,
                                   Title = form.Title,
                                   IdCode = form.IdCode,
                                   ExpireDate = form.ExpireDate,
                                   IdUser = uf.IdUser,
                                   NameFund = user.FundName,
                                   IdCodeFund = user.FundCode,
                                   IsPublicForm = form.IsPublicForm
                               };

            // ترکیب فرم‌های عمومی و خصوصی
            var unionQuery = formsPrivate.Union(formsPublic);

            return await paginationData(pageNumber, pageSize, unionQuery, sortFields, cancellationToken);
        }

    }
}
 