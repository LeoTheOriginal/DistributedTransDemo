using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.Json;

namespace TransDemo.UI.Models
{
    /// <summary>
    /// Represents application settings, including connection strings.
    /// Provides methods for loading from and saving to a JSON file.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the collection of connection strings.
        /// The key is the name of the connection, and the value is the connection string.
        /// </summary>
        public required Dictionary<string, string> ConnectionStrings { get; set; }

        /// <summary>
        /// Loads the <see cref="AppSettings"/> from a JSON file at the specified path.
        /// </summary>
        /// <param name="path">The file path to load the settings from.</param>
        /// <returns>An instance of <see cref="AppSettings"/> deserialized from the file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <exception cref="JsonException">Thrown if the file content is not valid JSON.</exception>
        public static AppSettings Load(string path)
            => JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path))!;

        /// <summary>
        /// Saves the current <see cref="AppSettings"/> instance to a JSON file at the specified path.
        /// </summary>
        /// <param name="path">The file path to save the settings to.</param>
        public void Save(string path)
            => File.WriteAllText(path,
                JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}