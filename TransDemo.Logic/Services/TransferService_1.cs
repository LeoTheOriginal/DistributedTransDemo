//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Transactions;
//using TransDemo.Data.Repositories;

//namespace TransDemo.Logic.Services
//{
//    public class TransferService(IHistoryRepository repo)
//    {
//        private readonly IHistoryRepository _repo = repo;

//        public void RunDemoTransaction(bool simulateError = false)
//        {
//            using (var scope = new TransactionScope(
//                TransactionScopeOption.Required,
//                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
//            {
//                _repo.AddEntryToA("Start demo");
//                if (simulateError)
//                    throw new InvalidOperationException("Symulowany błąd!");

//                _repo.AddEntryToB("Finish demo");
//                scope.Complete();
//            }
//        }
//    }
//}

