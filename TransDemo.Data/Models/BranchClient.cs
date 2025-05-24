// TransDemo.Data/Models/BranchClient.cs
namespace TransDemo.Data.Models
{
    public class BranchClient
    {
        public int BranchClientId { get; set; }
        public int CentralClientId { get; set; }
        public decimal Balance { get; set; }
    }
}
