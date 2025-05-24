// TransDemo.Data/Repositories/IBranchClientRepository.cs
namespace TransDemo.Data.Repositories
{
    public interface IBranchClientRepository
    {
        /// <summary>
        /// Pobiera stan lokalnego konta danego klienta (wg CentralClientId).
        /// </summary>
        decimal GetBalance(int centralClientId);

        /// <summary>
        /// Obciąża lokalne konto klienta i zapisuje rekord w BranchTransactions.
        /// Rzuca, jeśli niewystarczające środki.
        /// </summary>
        void Debit(int centralClientId, decimal amount, string description);

        /// <summary>
        /// Zasila lokalne konto klienta i zapisuje rekord w BranchTransactions.
        /// </summary>
        void Credit(int centralClientId, decimal amount, string description);
    }
}
