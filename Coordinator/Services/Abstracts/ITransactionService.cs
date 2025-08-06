namespace Coordinator.Services.Abstracts
{
    public interface ITransactionService
    {
        Task<Guid> CreateTransactionAsync();
        Task PrepareServicesAsync(Guid TransactionId);

        Task<bool> CheckReadyTransactionAsync(Guid TransactionId);

        Task CommitAsync(Guid TransactionId);

        Task<bool> CheckTransactionStateServicesAsync(Guid TransactionId);

        Task RollbackAsync(Guid TransactionId);



    }
}
