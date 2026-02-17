//namespace ORG.BasicInfo.Domain;

//public interface IAggregateRoot;

namespace ORG.BasicInfo.Domain.Abstractions
{
    public abstract class AggregateRoot
    {
        public Guid Id { get;}

        public long TCreate { get;}
        public Guid LUserCreate { get; private set; }
        public long TEdit { get; protected set; }
        
        public Guid LUserEdit { get; protected set;}
        public EntityState State { get; protected set;}
        
        protected AggregateRoot(Guid lUserCreate)
        {
            Id = Guid.NewGuid();
            TCreate = (DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            LUserCreate = lUserCreate;
            TEdit = 0;
            LUserEdit = default;
            State=EntityState.Active;
        }
        public void ChangeState()
        {
            State = State == EntityState.Inactive ? EntityState.Active : EntityState.Inactive;
        }

    }
    public enum EntityState
    {
        Active = 1,
        Inactive = 2
    }
}
