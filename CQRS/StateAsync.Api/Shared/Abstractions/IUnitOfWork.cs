namespace StateAsync.Api.Shared.Abstractions;

public interface IUnitOfWork
{
    Task<int> Commit(CancellationToken cancellationToken);
}
