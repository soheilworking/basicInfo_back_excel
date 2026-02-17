namespace ORG.BasicInfo.Domain;

public interface IEntity<TKey>
    where TKey : notnull
{
    TKey Id { get; }
}