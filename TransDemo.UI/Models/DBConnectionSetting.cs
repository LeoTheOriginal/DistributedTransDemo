using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.UI.Models
{
    /// <summary>
    /// Represents the settings required to establish a database connection.
    /// </summary>
    public class DbConnectionSetting
    {
        /// <summary>
        /// Gets or sets the key or name of the connection (e.g., "CentralDB", "Branch1DB").
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the server address or name where the database is hosted.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the name of the database to connect to.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the username used for database authentication.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password used for database authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the password should be remembered.
        /// </summary>
        public bool RememberPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection should be encrypted.
        /// </summary>
        public bool Encrypt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to trust the server certificate.
        /// </summary>
        public bool TrustServerCertificate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to keep the connection alive.
        /// </summary>
        public bool KeepConnectionAlive { get; set; }
    }
}

