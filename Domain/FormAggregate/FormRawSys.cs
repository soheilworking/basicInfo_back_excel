using ORG.BasicInfo.Domain.Abstractions;
using ORG.BasicInfo.Domain.UserAggregate;
namespace ORG.BasicInfo.Domain.FormAggregate
{
    public class  FormRawSys: AggregateRoot
    {
        public ulong IdCode { get; private set; }
       
        public string Title { get; private set; }
        public string Description { get; private set; }
        public long ExpireDate { get; private set; }
        public bool IsPublicForm { get; private set; }
        public List<FormRawRelatedUserSys> UserFund = new();
        public List<FilesRawSys> FilesRawOrgSys = new();
        
        public void ChangeValue(
            ulong idCode,
            string title,
            string description,
            long expireDate,
            bool isPublicForm,
            Guid lUserEdit
          )
        {
            TEdit = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
       
            LUserEdit = lUserEdit;
            IdCode = idCode;
            Title = title;
            Description = description;
            ExpireDate = expireDate;
            IsPublicForm = isPublicForm;
        }

        public FormRawSys(
            ulong idCode,
            string title,
            string description,
            long expireDate,
            bool isPublicForm,
            Guid lUserCreate
            ) : base(lUserCreate)
        {
          
            IdCode = idCode;
            Title = title;
            Description = description;
            ExpireDate = expireDate;
            IsPublicForm = isPublicForm;
        }

    }

}
