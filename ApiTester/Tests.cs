using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using TransDemo.Data.Repositories;
using TransDemo.Logic.Services;
using TransDemo.Models;

namespace ApiTester
{
    /// <summary>
    /// Główna klasa zawierająca testy jednostkowe i integracyjne dla warstw repozytoriów i usług logicznych.
    /// </summary>
    class Tests
    {
        /// <summary>
        /// Główny punkt wejścia aplikacji testującej.
        /// Ładuje konfigurację z pliku appsettings.json i uruchamia kolejne testy.
        /// </summary>
        static async Task Main(string[] args)
        {
            // 1. Ładujemy appsettings.json (musisz mieć w katalogu projektu ApiTester plik appsettings.json
            //    i w jego właściwościach Build Action=Content, Copy to Output Directory=Copy if newer)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Pobieramy trzy connection stringi:
            string connCentral = configuration.GetConnectionString("CentralDB")!;
            string connBranch1 = configuration.GetConnectionString("Branch1DB")!;
            string connBranch2 = configuration.GetConnectionString("Branch2DB")!;

            bool allTestsOk = true;

            Console.WriteLine("=== TESTY: IHistoryRepository / SqlHistoryRepository ===");
            try
            {
                TestHistoryRepository(connCentral, connBranch1, connBranch2);
                Console.WriteLine("Wszystkie testy IHistoryRepository przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach HistoryRepository: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: IDistributedTransferRepository / SqlDistributedTransferRepository ===");
            try
            {
                TestDistributedTransfer(connCentral, connBranch1, connBranch2);
                Console.WriteLine("Wszystkie testy IDistributedTransferRepository przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach DistributedTransfer: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: IAccountRepository / SqlAccountRepository ===");
            try
            {
                TestAccountRepository(connCentral);
                Console.WriteLine("Wszystkie testy IAccountRepository przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach AccountRepository: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: IBranchClientRepository / SqlBranchClientRepository ===");
            try
            {
                TestBranchClientRepository(connBranch1);
                Console.WriteLine("Wszystkie testy IBranchClientRepository przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach BranchClientRepository: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: ICentralBankRepository / SqlCentralBankRepository ===");
            try
            {
                TestCentralBankRepository(connCentral);
                Console.WriteLine("Wszystkie testy ICentralBankRepository przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach CentralBankRepository: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: ICustomerRepository / SqlCustomerRepository ===");
            try
            {
                TestCustomerRepository(connCentral);
                Console.WriteLine("Wszystkie testy ICustomerRepository przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach CustomerRepository: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: DashboardStatsService ===");
            try
            {
                await TestDashboardStatsService(configuration);
                Console.WriteLine("Wszystkie testy DashboardStatsService przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach DashboardStatsService: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: HistoryQueryService ===");
            try
            {
                await TestHistoryQueryService(configuration);
                Console.WriteLine("Wszystkie testy HistoryQueryService przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach HistoryQueryService: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: StatisticsQueryService ===");
            try
            {
                await TestStatisticsQueryService(configuration);
                Console.WriteLine("Wszystkie testy StatisticsQueryService przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach StatisticsQueryService: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: TransactionStatsService ===");
            try
            {
                await TestTransactionStatsService(configuration);
                Console.WriteLine("Wszystkie testy TransactionStatsService przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach TransactionStatsService: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine("=== TESTY: TransferService ===");
            try
            {
                TestTransferService();
                Console.WriteLine("Wszystkie testy TransferService przeszły pomyślnie.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w testach TransferService: {ex}\n");
                allTestsOk = false;
            }

            Console.WriteLine(allTestsOk
                ? "<<< WSZYSTKIE TESTY LOGIC ZALICZONE POMYŚLNIE >>>"
                : "<<< COŚ NIE WYSZŁO W TESTACH LOGIC >>>");
            Console.ReadLine();
        }

        // ============================
        // 1. Testy dla IHistoryRepository
        // ============================

        /// <summary>
        /// Wykonuje testy na repozytorium <see cref="IHistoryRepository"/>.
        /// </summary>
        /// <param name="connCentral">Connection string do bazy centralnej.</param>
        /// <param name="connA">Connection string do bazy oddziału A.</param>
        /// <param name="connB">Connection string do bazy oddziału B.</param>
        static void TestHistoryRepository(string connCentral, string connA, string connB)
        {
            var historyRepo = new SqlHistoryRepository(connCentral, connA, connB);

            // 1.1: Wyczyść tabele History w Branch1, Branch2 i BankCentral
            ExecuteNonQuery(connA, "DELETE FROM dbo.History");
            ExecuteNonQuery(connB, "DELETE FROM dbo.History");
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.History");

            // 1.2: Test AddEntryToA / AddEntryToB
            try
            {
                Console.WriteLine("  [History] 1.2: Dodaj wpis do Branch1 i Branch2…");
                historyRepo.AddEntryToA("ENTRY_A1");
                historyRepo.AddEntryToB("ENTRY_B1");

                int countA = CountHistoryRecords(connA, "ENTRY_A1");
                int countB = CountHistoryRecords(connB, "ENTRY_B1");
                Debug.Assert(countA == 1, $"Brak wpisu ENTRY_A1 w Branch1DB.dbo.History (countA={countA})");
                Debug.Assert(countB == 1, $"Brak wpisu ENTRY_B1 w Branch2DB.dbo.History (countB={countB})");
                Console.WriteLine("    > TEST HistoryRepository.AddEntryToA/B – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w HistoryRepository.AddEntryToA/B: {ex}");
                throw;
            }

            // 1.3: Test GetHistoryFromBranch
            try
            {
                Console.WriteLine("  [History] 1.3: Pobierz historię z Branch1 i Branch2…");
                historyRepo.AddEntryToA("ENTRY_A2");
                historyRepo.AddEntryToB("ENTRY_B2");

                var listA = historyRepo.GetHistoryFromBranch(1);
                var listB = historyRepo.GetHistoryFromBranch(2);
                Debug.Assert(listA.Any(e => e.Info == "ENTRY_A1"), "Nie znaleziono ENTRY_A1 w GetHistoryFromBranch(1)");
                Debug.Assert(listA.Any(e => e.Info == "ENTRY_A2"), "Nie znaleziono ENTRY_A2 w GetHistoryFromBranch(1)");
                Debug.Assert(listB.Any(e => e.Info == "ENTRY_B1"), "Nie znaleziono ENTRY_B1 w GetHistoryFromBranch(2)");
                Debug.Assert(listB.Any(e => e.Info == "ENTRY_B2"), "Nie znaleziono ENTRY_B2 w GetHistoryFromBranch(2)");
                Console.WriteLine("    > TEST HistoryRepository.GetHistoryFromBranch – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w HistoryRepository.GetHistoryFromBranch: {ex}");
                throw;
            }

            // 1.4: Test GetCentralHistory
            try
            {
                Console.WriteLine("  [History] 1.4: Pobierz historię z bazy centralnej…");
                ExecuteNonQuery(connCentral, "DELETE FROM dbo.History");
                ExecuteNonQuery(connCentral, "INSERT INTO dbo.History (Info) VALUES ('CENTRAL1'),('CENTRAL2')");

                var listC = historyRepo.GetCentralHistory();
                Debug.Assert(listC.Any(e => e.Info == "CENTRAL1"), "Nie znaleziono CENTRAL1 w GetCentralHistory()");
                Debug.Assert(listC.Any(e => e.Info == "CENTRAL2"), "Nie znaleziono CENTRAL2 w GetCentralHistory()");
                Console.WriteLine("    > TEST HistoryRepository.GetCentralHistory – OK\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w HistoryRepository.GetCentralHistory: {ex}");
                throw;
            }
        }

        // ============================
        // 2. Testy dla IDistributedTransferRepository
        // ============================

        /// <summary>
        /// Wykonuje testy dla repozytorium <see cref="IDistributedTransferRepository"/>,
        /// weryfikując scenariusze sukcesu oraz błędu (rollback) dla transferów rozproszonych.
        /// </summary>
        /// <param name="connCentral">Connection string do bazy centralnej.</param>
        /// <param name="connA">Connection string do bazy oddziału A.</param>
        /// <param name="connB">Connection string do bazy oddziału B.</param>
        static void TestDistributedTransfer(string connCentral, string connA, string connB)
        {
            var distroRepo = new SqlDistributedTransferRepository(connCentral);

            // 2.1: SCENARIUSZ SUCCESS
            PrepareDistributedScenario(connCentral, connA, connB,
                fromCentralBalance: 1000m, toCentralBalance: 500m,
                branch1InitialBalance: 1000m, branch2InitialBalance: 500m);

            try
            {
                Console.WriteLine("  [Distributed] 2.1: Wykonaj transfer (SUCCESS)…");
                distroRepo.TransferDistributed(fromAccId: 1, toAccId: 2, amount: 100m);

                // Weryfikacja centralnych sald
                decimal centralFromBal = GetCentralAccountBalance(connCentral, 1);
                decimal centralToBal = GetCentralAccountBalance(connCentral, 2);
                Debug.Assert(centralFromBal == 900.00m, "Central: saldo konta źródłowego powinno być 900");
                Debug.Assert(centralToBal == 600.00m, "Central: saldo konta docelowego powinno być 600");

                // Weryfikacja branch1 i branch2
                decimal branch1Bal = GetBranchClientBalance(connA, 1);
                decimal branch2Bal = GetBranchClientBalance(connB, 2);
                Debug.Assert(branch1Bal == 900.00m, "Branch1: saldo powinno być 900");
                Debug.Assert(branch2Bal == 600.00m, "Branch2: saldo powinno być 600");

                // Weryfikacja wpisu w centralnej Historii
                int histCentralCount = CountHistoryRecords(connCentral, "Transfer z");
                Debug.Assert(histCentralCount >= 1, "Central: w History powinien być przynajmniej 1 wpis po transferze");
                Console.WriteLine("    > TEST TransferDistributed (SUCCESS) – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w TransferDistributed (SUCCESS): {ex}");
                throw;
            }

            // 2.2: SCENARIUSZ FAIL + ROLLBACK
            PrepareDistributedScenario(connCentral, connA, connB,
                fromCentralBalance: 50m, toCentralBalance: 500m,
                branch1InitialBalance: 50m, branch2InitialBalance: 500m);

            try
            {
                Console.WriteLine("  [Distributed] 2.2: Wykonaj transfer (FAIL + ROLLBACK)…");
                distroRepo.TransferDistributed(fromAccId: 1, toAccId: 2, amount: 100m);
                throw new Exception("TransferDistributed nie rzucił wyjątku przy niewystarczających środkach");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    > Przechwycono wyjątek (oczekiwano FAIL): {ex.Message}");

                // Po wyjątku sprawdźmy, że nigdzie nie zmieniono danych
                decimal cFrom2 = GetCentralAccountBalance(connCentral, 1);
                decimal cTo2 = GetCentralAccountBalance(connCentral, 2);
                Debug.Assert(cFrom2 == 50.00m, "CentralFrom powinno pozostać 50");
                Debug.Assert(cTo2 == 500.00m, "CentralTo powinno pozostać 500");

                decimal b1_2 = GetBranchClientBalance(connA, 1);
                decimal b2_2 = GetBranchClientBalance(connB, 2);
                Debug.Assert(b1_2 == 50.00m, "Branch1: saldo nie powinno się zmienić (50)");
                Debug.Assert(b2_2 == 500.00m, "Branch2: saldo nie powinno się zmienić (500)");

                int histCnt2 = CountHistoryRecords(connCentral, "Transfer z");
                Debug.Assert(histCnt2 == 0, "Central: w History nie powinno być wpisu po nieudanym transferze");
                Console.WriteLine("    > TEST TransferDistributed (FAIL + ROLLBACK) – OK");
            }
        }

        // ============================
        // 3. Testy dla IAccountRepository
        // ============================

        /// <summary>
        /// Wykonuje testy na repozytorium <see cref="IAccountRepository"/> dla operacji GetAll, Debit, Credit.
        /// </summary>
        /// <param name="connCentral">Connection string do bazy centralnej.</param>
        static void TestAccountRepository(string connCentral)
        {
            var accountRepo = new SqlAccountRepository(connCentral);

            // 3.1: Wyczyść tabelę Accounts i Transactions
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.Transactions");
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.Accounts");

            // 3.2: Dodaj dwa konta
            try
            {
                Console.WriteLine("  [Account] 3.2: Dodaj dwa konta…");
                ExecuteNonQuery(connCentral, @"
                    INSERT INTO dbo.Accounts (CustomerId, AccountNumber, Balance, BranchId, AccountTypeId)
                    VALUES (1, 'ACC-TEST-1', 1000.00, 1, 1),
                           (2, 'ACC-TEST-2',  500.00, 2, 1);
                ");
                Console.WriteLine("    > Dodano konta w dbo.Accounts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w [Account] Insert kont: {ex}");
                throw;
            }

            // 3.3: Test GetAll()
            try
            {
                Console.WriteLine("  [Account] 3.3: Test GetAll()…");
                var allAccounts = accountRepo.GetAll().ToList();
                Debug.Assert(allAccounts.Count >= 2, "IAccountRepository.GetAll() nie zwraca poprawnej liczby kont");
                Debug.Assert(allAccounts.Any(a => a.AccountNumber == "ACC-TEST-1"), "Nie znaleziono ACC-TEST-1 w GetAll()");
                Debug.Assert(allAccounts.Any(a => a.AccountNumber == "ACC-TEST-2"), "Nie znaleziono ACC-TEST-2 w GetAll()");
                Console.WriteLine("    > TEST AccountRepository.GetAll() – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w AccountRepository.GetAll(): {ex}");
                throw;
            }

            // 3.4: Test Debit()
            try
            {
                Console.WriteLine("  [Account] 3.4: Test Debit()…");
                var first = accountRepo.GetAll().First();
                accountRepo.Debit(first.AccountId, 200m);
                decimal postDebitBal = GetCentralAccountBalance(connCentral, first.AccountId);
                Debug.Assert(postDebitBal == 800.00m, "Błąd: saldo po Debit(-200) powinno być 800");
                Console.WriteLine("    > TEST AccountRepository.Debit() – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w AccountRepository.Debit(): {ex}");
                throw;
            }

            // 3.5: Test Credit()
            try
            {
                Console.WriteLine("  [Account] 3.5: Test Credit()…");
                var first = accountRepo.GetAll().First();
                accountRepo.Credit(first.AccountId, 100m);
                decimal postCreditBal = GetCentralAccountBalance(connCentral, first.AccountId);
                Debug.Assert(postCreditBal == 900.00m, "Błąd: saldo po Credit(+100) powinno być 900");
                Console.WriteLine("    > TEST AccountRepository.Credit() – OK\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w AccountRepository.Credit(): {ex}");
                throw;
            }
        }

        // ============================
        // 4. Testy dla IBranchClientRepository
        // ============================

        /// <summary>
        /// Wykonuje testy na repozytorium <see cref="IBranchClientRepository"/> dla operacji GetBalance, Debit, Credit i obsługi braku środków.
        /// </summary>
        /// <param name="connBranch">Connection string do bazy oddziału.</param>
        static void TestBranchClientRepository(string connBranch)
        {
            var branchRepo = new SqlBranchClientRepository(connBranch);

            // 4.1: Wyczyść BranchClients i BranchTransactions
            ExecuteNonQuery(connBranch, "DELETE FROM dbo.BranchTransactions");
            ExecuteNonQuery(connBranch, "DELETE FROM dbo.BranchClients");

            // 4.2: Dodaj jeden wiersz BranchClients
            try
            {
                Console.WriteLine("  [BranchClient] 4.2: Dodaj wpis do BranchClients…");
                ExecuteNonQuery(connBranch, @"
                    INSERT INTO dbo.BranchClients (CentralClientId, LocalAccountNo, Balance)
                    VALUES (1, 'BR-CLIENT-1', 500.00);
                ");
                Console.WriteLine("    > Dodano BranchClient (CentralClientId=1, Balance=500)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w [BranchClient] Insert BranchClients: {ex}");
                throw;
            }

            // 4.3: Test GetBalance()
            try
            {
                Console.WriteLine("  [BranchClient] 4.3: Test GetBalance()…");
                decimal initialBal = branchRepo.GetBalance(1);
                Debug.Assert(initialBal == 500.00m, "Błąd: początkowe saldo BranchClients powinno być 500");
                Console.WriteLine("    > TEST BranchClientRepository.GetBalance() – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w BranchClientRepository.GetBalance(): {ex}");
                throw;
            }

            // 4.4: Test Debit() (scenariusz sukces)
            try
            {
                Console.WriteLine("  [BranchClient] 4.4: Test Debit() success…");
                branchRepo.Debit(1, 200m, "TestDEBIT");
                decimal postDebitBal = branchRepo.GetBalance(1);
                Debug.Assert(postDebitBal == 300.00m, "Błąd: saldo po Debit(200) powinno być 300");
                int txnCountAfterDebit = CountBranchTransactions(connBranch, 1, "DEBIT");
                Debug.Assert(txnCountAfterDebit == 1, "Błąd: w BranchTransactions powinien być wpis DEBIT");
                Console.WriteLine("    > TEST BranchClientRepository.Debit() – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w BranchClientRepository.Debit() success: {ex}");
                throw;
            }

            // 4.5: Test Credit()
            try
            {
                Console.WriteLine("  [BranchClient] 4.5: Test Credit()…");
                branchRepo.Credit(1, 150m, "TestCREDIT");
                decimal postCreditBal = branchRepo.GetBalance(1);
                Debug.Assert(postCreditBal == 450.00m, "Błąd: saldo po Credit(150) powinno być 450");
                int txnCountAfterCredit = CountBranchTransactions(connBranch, 1, "CREDIT");
                Debug.Assert(txnCountAfterCredit == 1, "Błąd: w BranchTransactions powinien być wpis CREDIT");
                Console.WriteLine("    > TEST BranchClientRepository.Credit() – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w BranchClientRepository.Credit(): {ex}");
                throw;
            }

            // 4.6: Test Debit() – insufficient funds
            try
            {
                Console.WriteLine("  [BranchClient] 4.6: Test Debit() insufficient funds…");
                ExecuteNonQuery(connBranch, "UPDATE dbo.BranchClients SET Balance = 100.00 WHERE CentralClientId = 1");
                bool excThrown = false;
                try
                {
                    branchRepo.Debit(1, 200m, "FailDebit");
                }
                catch (InvalidOperationException)
                {
                    excThrown = true;
                }
                Debug.Assert(excThrown, "Błąd: Debet przy niewystarczającym saldzie nie rzucił wyjątku");
                decimal afterFailBal = branchRepo.GetBalance(1);
                Debug.Assert(afterFailBal == 100.00m, "Błąd: saldo nie powinno się zmienić po nieudanym Debit");
                Console.WriteLine("    > TEST BranchClientRepository.Debit() insufficient funds – OK\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w BranchClientRepository.Debit() insufficient funds: {ex}");
                throw;
            }
        }

        // ============================
        // 5. Testy dla ICentralBankRepository
        // ============================

        /// <summary>
        /// Wykonuje testy na repozytorium <see cref="ICentralBankRepository"/> dla operacji TransferCentral.
        /// </summary>
        /// <param name="connCentral">Connection string do bazy centralnej.</param>
        static void TestCentralBankRepository(string connCentral)
        {
            var centralRepo = new SqlCentralBankRepository(connCentral);

            // 5.1: Wyczyść Accounts i Transactions
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.Transactions");

            // Tutaj dodajemy reset tożsamości, żeby nowe wiersze miały AccountId = 1 i 2
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.Accounts");
            ExecuteNonQuery(connCentral, "DBCC CHECKIDENT('dbo.Accounts', RESEED, 0)");

            // 5.2: Dodaj dwa konta
            try
            {
                Console.WriteLine("  [Central] 5.2: Dodaj dwa konta…");
                ExecuteNonQuery(connCentral, @"
            INSERT INTO dbo.Accounts (CustomerId, AccountNumber, Balance, BranchId, AccountTypeId)
            VALUES 
              (1, 'C-ACC-1', 200.00, 1, 1),
              (2, 'C-ACC-2', 100.00, 2, 1);
        ");
                Console.WriteLine("    > Dodano konta w BankCentral.dbo.Accounts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w [Central] Insert kont: {ex}");
                throw;
            }

            // 5.3: Test TransferCentral() – SUCCESS
            try
            {
                Console.WriteLine("  [Central] 5.3: Test TransferCentral() SUCCESS…");
                centralRepo.TransferCentral(fromAccId: 1, toAccId: 2, amount: 150m);

                decimal c1 = GetCentralAccountBalance(connCentral, 1);
                decimal c2 = GetCentralAccountBalance(connCentral, 2);
                Debug.Assert(c1 == 50.00m, "CentralBank: saldo konta 1 powinno być 50");
                Debug.Assert(c2 == 250.00m, "CentralBank: saldo konta 2 powinno być 250");

                int txCountDebit = CountCentralTransactions(connCentral, 1, "DEBIT");
                int txCountCredit = CountCentralTransactions(connCentral, 2, "CREDIT");
                Debug.Assert(txCountDebit == 1, "CentralBank: powinna być transakcja DEBIT dla konta 1");
                Debug.Assert(txCountCredit == 1, "CentralBank: powinna być transakcja CREDIT dla konta 2");
                Console.WriteLine("    > TEST CentralBankRepository.TransferCentral() SUCCESS – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w CentralBankRepository.TransferCentral() SUCCESS: {ex}");
                throw;
            }

            // 5.4: Test TransferCentral() – FAIL (insufficient funds)
            try
            {
                Console.WriteLine("  [Central] 5.4: Test TransferCentral() FAIL + ROLLBACK…");
                ExecuteNonQuery(connCentral, "DELETE FROM dbo.Transactions");
                ExecuteNonQuery(connCentral, "UPDATE dbo.Accounts SET Balance = 50.00 WHERE AccountId = 1");
                ExecuteNonQuery(connCentral, "UPDATE dbo.Accounts SET Balance = 100.00 WHERE AccountId = 2");

                bool centralExc = false;
                try
                {
                    centralRepo.TransferCentral(fromAccId: 1, toAccId: 2, amount: 100m);
                }
                catch
                {
                    centralExc = true;
                }
                Debug.Assert(centralExc, "CentralBank: TransferCentral nie rzucił wyjątku przy niewystarczających środkach");

                decimal c1_postFail = GetCentralAccountBalance(connCentral, 1);
                decimal c2_postFail = GetCentralAccountBalance(connCentral, 2);
                Debug.Assert(c1_postFail == 50.00m, "CentralBank: saldo konta 1 nie powinno się zmienić po nieudanym transferze");
                Debug.Assert(c2_postFail == 100.00m, "CentralBank: saldo konta 2 nie powinno się zmienić po nieudanym transferze");

                int txCountAfterFail = CountCentralTransactions(connCentral, 1, "DEBIT");
                Debug.Assert(txCountAfterFail == 0, "CentralBank: nie powinno być transakcji po nieudanym transferze");
                Console.WriteLine("    > TEST CentralBankRepository.TransferCentral() FAIL + ROLLBACK – OK\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w CentralBankRepository.TransferCentral() FAIL: {ex}");
                throw;
            }
        }

        // ============================
        // 6. Testy dla ICustomerRepository
        // ============================

        /// <summary>
        /// Wykonuje testy na repozytorium <see cref="ICustomerRepository"/> dla operacji GetAll z filtrem i bez.
        /// </summary>
        /// <param name="connCentral">Connection string do bazy centralnej.</param>
        static void TestCustomerRepository(string connCentral)
        {
            var customerRepo = new SqlCustomerRepository(connCentral);

            // 6.1: Wyczyść powiązane tabele (Accounts) zanim spróbujemy usunąć Customers
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.Accounts");
            // (opcjonalnie) zresetuj identyfikator Accounts, ale nie jest to konieczne w teście Customers
            ExecuteNonQuery(connCentral, "DBCC CHECKIDENT('dbo.Accounts', RESEED, 0)");

            // dopiero teraz można usuwać klientów
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.Customers");
            ExecuteNonQuery(connCentral, "DBCC CHECKIDENT('dbo.Customers', RESEED, 0)");

            // 6.2: Dodaj klientów
            try
            {
                Console.WriteLine("  [Customer] 6.2: Dodaj klientów…");
                ExecuteNonQuery(connCentral, @"
            INSERT INTO dbo.Customers (FirstName, LastName, Email, DateOfBirth)
            VALUES
              ('Jan', 'Kowalski', 'jan.k@example.com', '1980-01-01'),
              ('Anna', 'Nowak',    'anna.n@example.com', '1990-02-02'),
              ('Jan', 'Nowicki',  'jan.no@example.com',  '1985-03-03');
        ");
                Console.WriteLine("    > Dodano 3 klientów w BankCentral.dbo.Customers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w [Customer] Insert klientów: {ex}");
                throw;
            }

            // 6.3: Test GetAll() bez filtra
            try
            {
                Console.WriteLine("  [Customer] 6.3: Test GetAll(null)…");
                var all = customerRepo.GetAll(null).ToList();
                Debug.Assert(all.Count >= 3, "CustomerRepository.GetAll() nie zwraca wszystkich klientów");
                Console.WriteLine("    > TEST CustomerRepository.GetAll(null) – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w CustomerRepository.GetAll(null): {ex}");
                throw;
            }

            // 6.4: Test GetAll(filter)
            try
            {
                Console.WriteLine("  [Customer] 6.4: Test GetAll(\"Jan\")…");
                var filtered = customerRepo.GetAll("Jan").ToList();
                Debug.Assert(filtered.All(c => c.FullName.Contains("Jan")), "CustomerRepository.GetAll(\"Jan\") – filtr nie działa poprawnie");
                Debug.Assert(filtered.Count >= 2, "CustomerRepository.GetAll(\"Jan\") – powinno zwrócić przynajmniej 2 wiersze");
                Console.WriteLine("    > TEST CustomerRepository.GetAll(\"Jan\") – OK\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w CustomerRepository.GetAll(\"Jan\"): {ex}");
                throw;
            }
        }

        // ============================
        // Testy dla DashboardStatsService
        // ============================

        /// <summary>
        /// Testuje metody <see cref="DashboardStatsService"/>: pobieranie statystyk dziennych, sald, udziału oddziałów oraz top klientów.
        /// </summary>
        /// <param name="configuration">Instancja <see cref="IConfiguration"/> z pliku konfiguracyjnego.</param>
        static async Task TestDashboardStatsService(IConfiguration configuration)
        {
            var service = new DashboardStatsService(configuration);

            // 1. Test GetStatsAsync
            try
            {
                Console.WriteLine("  [Dashboard] 1: Test GetStatsAsync(5) …");
                var stats = await service.GetStatsAsync(5);
                DebugAssert(stats != null, "GetStatsAsync zwróciło null");
                Console.WriteLine($"    > Zwrócono {stats.Count()} wierszy z vDailyTransactionStats");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w DashboardStatsService.GetStatsAsync: {ex}");
                throw;
            }

            // 2. Test GetDailyBalanceAsync
            try
            {
                Console.WriteLine("  [Dashboard] 2: Test GetDailyBalanceAsync(5) …");
                var balances = await service.GetDailyBalanceAsync(5);
                DebugAssert(balances != null, "GetDailyBalanceAsync zwróciło null");
                Console.WriteLine($"    > Zwrócono {balances.Count()} wierszy z vDailyBalances");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w DashboardStatsService.GetDailyBalanceAsync: {ex}");
                throw;
            }

            // 3. Test GetBranchShareAsync
            try
            {
                Console.WriteLine("  [Dashboard] 3: Test GetBranchShareAsync() …");
                var shares = await service.GetBranchShareAsync();
                DebugAssert(shares != null, "GetBranchShareAsync zwróciło null");
                Console.WriteLine($"    > Zwrócono {shares.Count()} wierszy z vBranchTransactionShares");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w DashboardStatsService.GetBranchShareAsync: {ex}");
                throw;
            }

            // 4. Test GetTopCustomersAsync
            try
            {
                Console.WriteLine("  [Dashboard] 4: Test GetTopCustomersAsync(3) …");
                var topCust = await service.GetTopCustomersAsync(3);
                DebugAssert(topCust != null, "GetTopCustomersAsync zwróciło null");
                Console.WriteLine($"    > Zwrócono {topCust.Count()} wierszy z vTopCustomerTransactions");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w DashboardStatsService.GetTopCustomersAsync: {ex}");
                throw;
            }
        }

        // ============================
        // Testy dla HistoryQueryService
        // ============================

        /// <summary>
        /// Testuje metody <see cref="HistoryQueryService"/>, weryfikując pobieranie historii z centrali i oddziałów,
        /// a także obsługę nieprawidłowego numeru oddziału.
        /// </summary>
        /// <param name="configuration">Instancja <see cref="IConfiguration"/> z pliku konfiguracyjnego.</param>
        static async Task TestHistoryQueryService(IConfiguration configuration)
        {
            var service = new HistoryQueryService(configuration);

            // 1. Test GetBranchHistoryAsync(0) – centralna
            try
            {
                Console.WriteLine("  [HistoryQuery] 1: Test GetBranchHistoryAsync(0) – centralna …");
                var centralHist = await service.GetBranchHistoryAsync(0);
                DebugAssert(centralHist != null, "GetBranchHistoryAsync(0) zwróciło null");
                Console.WriteLine($"    > Zwrócono {centralHist.Count()} wierszy z BankCentral.dbo.History");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w HistoryQueryService.GetBranchHistoryAsync(0): {ex}");
                throw;
            }

            // 2. Test GetBranchHistoryAsync(1) – Branch1
            try
            {
                Console.WriteLine("  [HistoryQuery] 2: Test GetBranchHistoryAsync(1) – branch1 …");
                var b1Hist = await service.GetBranchHistoryAsync(1);
                DebugAssert(b1Hist != null, "GetBranchHistoryAsync(1) zwróciło null");
                Console.WriteLine($"    > Zwrócono {b1Hist.Count()} wierszy z Branch1DB.dbo.History");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w HistoryQueryService.GetBranchHistoryAsync(1): {ex}");
                throw;
            }

            // 3. Test GetBranchHistoryAsync(2) – Branch2
            try
            {
                Console.WriteLine("  [HistoryQuery] 3: Test GetBranchHistoryAsync(2) – branch2 …");
                var b2Hist = await service.GetBranchHistoryAsync(2);
                DebugAssert(b2Hist != null, "GetBranchHistoryAsync(2) zwróciło null");
                Console.WriteLine($"    > Zwrócono {b2Hist.Count()} wierszy z Branch2DB.dbo.History");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w HistoryQueryService.GetBranchHistoryAsync(2): {ex}");
                throw;
            }

            // 4. Test GetBranchHistoryAsync(99) – wyjątek ArgumentOutOfRangeException
            try
            {
                Console.WriteLine("  [HistoryQuery] 4: Test GetBranchHistoryAsync(99) – powinien rzucić ArgumentOutOfRangeException …");
                bool threw = false;
                try
                {
                    var _ = await service.GetBranchHistoryAsync(99);
                }
                catch (ArgumentOutOfRangeException)
                {
                    threw = true;
                }
                DebugAssert(threw, "GetBranchHistoryAsync(99) nie rzuciło ArgumentOutOfRangeException");
                Console.WriteLine("    > TEST GetBranchHistoryAsync(99) rzucił ArgumentOutOfRangeException – OK\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w HistoryQueryService.GetBranchHistoryAsync(99): {ex}");
                throw;
            }
        }

        // ============================
        // Testy dla StatisticsQueryService
        // ============================

        /// <summary>
        /// Testuje metodę <see cref="StatisticsQueryService.GetDailyStatsAsync"/>, pobierając statystyki z widoku vDailyTransactionStats.
        /// </summary>
        /// <param name="configuration">Instancja <see cref="IConfiguration"/> z pliku konfiguracyjnego.</param>
        static async Task TestStatisticsQueryService(IConfiguration configuration)
        {
            var service = new StatisticsQueryService(configuration);

            // 1. Test GetDailyStatsAsync(7)
            try
            {
                Console.WriteLine("  [StatisticsQuery] 1: Test GetDailyStatsAsync(7) …");
                var stats = await service.GetDailyStatsAsync(7);
                DebugAssert(stats != null, "GetDailyStatsAsync zwróciło null");
                Console.WriteLine($"    > Zwrócono {stats.Count()} wierszy z vDailyTransactionStats (ostatnie 7 dni)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w StatisticsQueryService.GetDailyStatsAsync: {ex}");
                throw;
            }
        }

        // ============================
        // Testy dla TransactionStatsService
        // ============================

        /// <summary>
        /// Testuje metodę <see cref="TransactionStatsService.GetDailyStatsAsync"/>, pobierając statystyki dzienne.
        /// </summary>
        /// <param name="configuration">Instancja <see cref="IConfiguration"/> z pliku konfiguracyjnego.</param>
        static async Task TestTransactionStatsService(IConfiguration configuration)
        {
            var service = new TransactionStatsService(configuration);

            // 1. Test GetDailyStatsAsync()
            try
            {
                Console.WriteLine("  [TransactionStats] 1: Test GetDailyStatsAsync() …");
                var stats = await service.GetDailyStatsAsync();
                DebugAssert(stats != null, "GetDailyStatsAsync zwróciło null");
                Console.WriteLine($"    > Zwrócono {stats.Count()} wierszy z vDailyTransactionStats");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w TransactionStatsService.GetDailyStatsAsync: {ex}");
                throw;
            }
        }

        // ============================
        // Testy dla TransferService
        // ============================

        /// <summary>
        /// Wykonuje testy na serwisie <see cref="TransferService"/>, weryfikując wywołanie metody TransferDistributed
        /// oraz obsługę flagi simulateError.
        /// </summary>
        static void TestTransferService()
        {
            // Fałszywe repozytorium do śledzenia wywołań
            var fakeRepo = new FakeDistributedRepo();
            var service = new TransferService(fakeRepo);

            // 1. Test ExecuteDistributedTransfer bez błędu
            try
            {
                Console.WriteLine("  [Transfer] 1: Test ExecuteDistributedTransfer bez symulowanego błędu …");
                service.ExecuteDistributedTransfer(
                    from: new Account { AccountId = 10 },
                    to: new Account { AccountId = 20 },
                    amount: 100m,
                    simulateError: false
                );
                DebugAssert(fakeRepo.WasCalled, "TransferService nie wywołał metody TransferDistributed na repozytorium");
                Console.WriteLine("    > TEST ExecuteDistributedTransfer (bez błędu) – OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w TransferService.ExecuteDistributedTransfer (bez błędu): {ex}");
                throw;
            }

            // 2. Test ExecuteDistributedTransfer z symulowanym błędem
            try
            {
                Console.WriteLine("  [Transfer] 2: Test ExecuteDistributedTransfer z simulateError = true …");
                bool threw = false;
                try
                {
                    service.ExecuteDistributedTransfer(
                        from: new Account { AccountId = 10 },
                        to: new Account { AccountId = 20 },
                        amount: 50m,
                        simulateError: true
                    );
                }
                catch (InvalidOperationException)
                {
                    threw = true;
                }
                DebugAssert(threw, "ExecuteDistributedTransfer(simulateError:true) nie rzuciło InvalidOperationException");
                Console.WriteLine("    > TEST ExecuteDistributedTransfer (simulateError) – OK\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ! BŁĄD w TransferService.ExecuteDistributedTransfer (simulateError): {ex}");
                throw;
            }
        }

        // ============================
        // Pomocnicze metody i klasy
        // ============================

        // ============================
        // POMOCNICZE METODY DO TESTÓW (na poziomie klasy)
        // ============================

        /// <summary>
        /// Wykonuje zapytanie SQL typu non-query (INSERT/UPDATE/DELETE).
        /// </summary>
        /// <param name="connString">Connection string do bazy danych.</param>
        /// <param name="sql">Polecenie SQL do wykonania.</param>
        /// <param name="param">Parametry do zapytania Dapper (opcjonalne).</param>
        private static void ExecuteNonQuery(string connString, string sql, object? param = null)
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            conn.Execute(sql, param);
        }

        /// <summary>
        /// Zlicza liczbę rekordów w tabeli History, których kolumna Info zawiera podany ciąg znaków.
        /// </summary>
        /// <param name="connString">Connection string do bazy danych.</param>
        /// <param name="infoContains">Ciąg znaków, który musi wystąpić w kolumnie Info.</param>
        /// <returns>Liczba rekordów spełniających warunek.</returns>
        private static int CountHistoryRecords(string connString, string infoContains)
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            return conn.QuerySingle<int>(
                "SELECT COUNT(*) FROM dbo.History WHERE Info LIKE @p",
                new { p = $"%{infoContains}%" });
        }

        /// <summary>
        /// Zlicza liczbę transakcji w BranchTransactions dla danego klienta i typu transakcji.
        /// </summary>
        /// <param name="connString">Connection string do bazy oddziału.</param>
        /// <param name="centralClientId">ID klienta w bazie centralnej, po którym łączymy tabele.</param>
        /// <param name="txnType">Typ transakcji ("DEBIT" lub "CREDIT").</param>
        /// <returns>Liczba rekordów odpowiadających filtrowi.</returns>
        private static int CountBranchTransactions(string connString, int centralClientId, string txnType)
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            return conn.QuerySingle<int>(
                @"SELECT COUNT(*) 
                  FROM dbo.BranchTransactions bt
                  JOIN dbo.BranchClients bc ON bt.BranchClientId = bc.BranchClientId
                  WHERE bc.CentralClientId = @cid AND bt.TxnType = @type",
                new { cid = centralClientId, type = txnType });
        }

        /// <summary>
        /// Zlicza liczbę transakcji w tabeli Transactions w bazie centralnej dla danego konta i typu transakcji.
        /// </summary>
        /// <param name="connString">Connection string do bazy centralnej.</param>
        /// <param name="accountId">ID konta.</param>
        /// <param name="txnType">Typ transakcji ("DEBIT" lub "CREDIT").</param>
        /// <returns>Liczba rekordów odpowiadających rodzajowi transakcji.</returns>
        private static int CountCentralTransactions(string connString, int accountId, string txnType)
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            return conn.QuerySingle<int>(
                "SELECT COUNT(*) FROM dbo.Transactions WHERE AccountId = @id AND TransactionType = @type",
                new { id = accountId, type = txnType });
        }

        /// <summary>
        /// Pobiera bieżące saldo konta w bazie centralnej.
        /// </summary>
        /// <param name="connString">Connection string do bazy centralnej.</param>
        /// <param name="accountId">ID konta.</param>
        /// <returns>Saldo konta jako decimal.</returns>
        private static decimal GetCentralAccountBalance(string connString, int accountId)
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            return conn.QuerySingle<decimal>(
                "SELECT Balance FROM dbo.Accounts WHERE AccountId = @id",
                new { id = accountId });
        }

        /// <summary>
        /// Pobiera bieżące saldo klienta (BranchClients) w bazie oddziałowej.
        /// </summary>
        /// <param name="connString">Connection string do bazy oddziału.</param>
        /// <param name="centralClientId">ID klienta w bazie centralnej, który jest kluczem powiązania.</param>
        /// <returns>Saldo klienta jako decimal.</returns>
        private static decimal GetBranchClientBalance(string connString, int centralClientId)
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            return conn.QuerySingle<decimal>(
                "SELECT Balance FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { cid = centralClientId });
        }

        /// <summary>
        /// Przygotowuje scenariusz do testów transferu rozproszonego.
        /// Czyści odpowiednie tabele w bazach i wstawia konta początkowe z zadanymi saldami.
        /// </summary>
        /// <param name="connCentral">Connection string do bazy centralnej.</param>
        /// <param name="connA">Connection string do bazy oddziału A.</param>
        /// <param name="connB">Connection string do bazy oddziału B.</param>
        /// <param name="fromCentralBalance">Początkowe saldo konta źródłowego w centrali.</param>
        /// <param name="toCentralBalance">Początkowe saldo konta docelowego w centrali.</param>
        /// <param name="branch1InitialBalance">Początkowe saldo klienta w oddziale 1.</param>
        /// <param name="branch2InitialBalance">Początkowe saldo klienta w oddziale 2.</param>
        private static void PrepareDistributedScenario(
            string connCentral, string connA, string connB,
            decimal fromCentralBalance, decimal toCentralBalance,
            decimal branch1InitialBalance, decimal branch2InitialBalance)
        {
            // Central: Wyczyść Accounts, Transactions, History
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.Transactions");
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.History");
            ExecuteNonQuery(connCentral, "DELETE FROM dbo.Accounts");

            // Branch1 i Branch2: Wyczyść BranchClients, BranchTransactions, History
            ExecuteNonQuery(connA, "DELETE FROM dbo.BranchTransactions");
            ExecuteNonQuery(connA, "DELETE FROM dbo.BranchClients");
            ExecuteNonQuery(connA, "DELETE FROM dbo.History");
            ExecuteNonQuery(connB, "DELETE FROM dbo.BranchTransactions");
            ExecuteNonQuery(connB, "DELETE FROM dbo.BranchClients");
            ExecuteNonQuery(connB, "DELETE FROM dbo.History");

            // Seed Central Accounts (AccountId = 1 i 2)
            ExecuteNonQuery(connCentral, @"
                SET IDENTITY_INSERT dbo.Accounts ON;
                INSERT INTO dbo.Accounts (AccountId, CustomerId, AccountNumber, Balance, BranchId, AccountTypeId, CreatedDate)
                VALUES 
                  (1, 1, 'CENTRAL-ACC-1', @bal1, 1, 1, SYSUTCDATETIME()),
                  (2, 2, 'CENTRAL-ACC-2', @bal2, 2, 1, SYSUTCDATETIME());
                SET IDENTITY_INSERT dbo.Accounts OFF;
            ", new { bal1 = fromCentralBalance, bal2 = toCentralBalance });

            // Seed BranchClients: CentralClientId = 1 w Branch1, = 2 w Branch2
            ExecuteNonQuery(connA, @"
                SET IDENTITY_INSERT dbo.BranchClients ON;
                INSERT INTO dbo.BranchClients (BranchClientId, CentralClientId, LocalAccountNo, Balance, LastSync)
                VALUES (1, 1, 'BR1-LOC-001', @b1bal, SYSUTCDATETIME());
                SET IDENTITY_INSERT dbo.BranchClients OFF;
            ", new { b1bal = branch1InitialBalance });

            ExecuteNonQuery(connB, @"
                SET IDENTITY_INSERT dbo.BranchClients ON;
                INSERT INTO dbo.BranchClients (BranchClientId, CentralClientId, LocalAccountNo, Balance, LastSync)
                VALUES (1, 2, 'BR2-LOC-001', @b2bal, SYSUTCDATETIME());
                SET IDENTITY_INSERT dbo.BranchClients OFF;
            ", new { b2bal = branch2InitialBalance });
        }

        /// <summary>
        /// Sprawdza warunek i w razie niepowodzenia rzuca wyjątek z podanym komunikatem.
        /// </summary>
        /// <param name="condition">Warunek, który ma być spełniony.</param>
        /// <param name="message">Komunikat wyjątku w przypadku niepowodzenia asercji.</param>
        private static void DebugAssert(bool condition, string message)
        {
            if (!condition)
                throw new Exception("Assert failed: " + message);
        }

        /// <summary>
        /// Fałszywe repozytorium implementujące <see cref="IDistributedTransferRepository"/>,
        /// służące do weryfikacji wywołania metody TransferDistributed w testach <see cref="TransferService"/>.
        /// </summary>
        private class FakeDistributedRepo : IDistributedTransferRepository
        {
            /// <summary>
            /// Flaga informująca, czy metoda <see cref="TransferDistributed"/> została wywołana.
            /// </summary>
            public bool WasCalled { get; private set; } = false;

            /// <summary>
            /// Symuluje wykonanie transferu rozproszonego, ustawiając flagę WasCalled na true.
            /// </summary>
            /// <param name="fromAccId">ID konta źródłowego.</param>
            /// <param name="toAccId">ID konta docelowego.</param>
            /// <param name="amount">Kwota do przelania.</param>
            public void TransferDistributed(int fromAccId, int toAccId, decimal amount)
            {
                WasCalled = true;
            }
        }
    }
}
