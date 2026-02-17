using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.Domain.FormAggregate
{
    public class FormRawRelatedUserSys
    {
        public Guid Id { get; private set; }
        public Guid IdForm { get; private set; }
        public Guid IdUser { get; private set; }
        public User User { get; private set; }

        public FormRawRelatedUserSys() { }
        public FormRawRelatedUserSys(
            Guid idForm,
            Guid idUser,
            User user=null
        ) 

        {
            Id = Guid.NewGuid();
            IdForm = idForm;
            IdUser = idUser;
            User = user;
        }
    }
}
