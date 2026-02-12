namespace Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        ITokenRepository TokenRepository { get; }
        IAdminRepository AdminRepository { get; }
        ISupplyRepository SupplyRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IBillRepository BillRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IAccountRepository AccountRepository { get; }
        IAccountTransactionRepository AccountTransactionRepository { get; }

        void DiscardChanges();

        Task<bool> SaveChangesAsync(bool overwriteDbChangesInCaseOfConcurrentUpdates = true);
    }
}
