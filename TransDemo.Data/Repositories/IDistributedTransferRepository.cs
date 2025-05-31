using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.Data.Repositories
{
    public interface IDistributedTransferRepository
    {
        void TransferDistributed(int fromAccId, int toAccId, decimal amount);
    }
}

