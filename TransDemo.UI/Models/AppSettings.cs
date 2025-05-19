using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.Json;

public class AppSettings
{
    // od teraz jedynie Dictionary<string,string>
    public Dictionary<string, string> ConnectionStrings { get; set; }

    public static AppSettings Load(string path)
        => JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path))!;

    public void Save(string path)
        => File.WriteAllText(path,
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
}

