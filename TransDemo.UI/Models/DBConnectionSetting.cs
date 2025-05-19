using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.UI.Models
{
    public class DbConnectionSetting
    {
        public string Key { get; set; }                  // nazwa połączenia: "CentralDB", "Branch1DB"...
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberPassword { get; set; }
        public bool Encrypt { get; set; }
        public bool TrustServerCertificate { get; set; }
        public bool KeepConnectionAlive { get; set; }
    }
}

