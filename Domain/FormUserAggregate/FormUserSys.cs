using ORG.BasicInfo.Domain.Abstractions;
using ORG.BasicInfo.Domain.UserAggregate;
using System.ComponentModel;
namespace ORG.BasicInfo.Domain.FormUserAggregate
{
    public class  FormUserSys: AggregateRoot
    {
        public Guid IdFormRaw { get; private set; }
        public string Description { get; private set; }
        public Guid IdUserRead { get; private set; }
        public Guid IdUser { get; private set; }
        public StateAction StateAction { get; private set; }
        public void ChangeValue(
            string description,
            Guid lUserEdit
          )
        {
            TEdit = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            LUserEdit = lUserEdit;
            Description = description;
        }
        public FormUserSys(
            string description,
            Guid idFormRaw,
            Guid idUser,
            Guid lUserCreate

            ) : base(lUserCreate)
        {
          
            Description = description;
            IdFormRaw = idFormRaw;
            IdUser = idUser;
            StateAction = StateAction.NoRead;
        }
        public void ChangeRead(Guid idUserRead)
        {
            if (IdUserRead == Guid.Empty)
                IdUserRead = idUserRead;
            else
                IdUserRead = Guid.Empty;
        }
        public void ChangeStateAction(StateAction state)
        {
            StateAction = state;
        }
    }
    public enum StateAction
    {
        [Description("خوانده نشده")]
        NoRead = 0,

        [Description("خوانده شده")]
        Read,

        [Description("پذیرفته شده")]
        Accept,

        [Description("رد شده")]
        Reject
    }
}
