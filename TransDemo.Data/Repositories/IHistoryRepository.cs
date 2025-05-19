using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.Data.Repositories
{
    public interface IHistoryRepository
    {
        void AddEntryToA(string info);
        void AddEntryToB(string info);
    }
}
