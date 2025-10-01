using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace SportProgramm.Scripts
{
    public class DatabaseConfig
    {
        public string Server { get; set; } = "(local)";
        public string Database { get; set; } = "SportProgrammProject";
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseWindowsAuth { get; set; } = true;

        public static DatabaseConfig Load()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.config");
                if (File.Exists(configPath))
                {
                    string[] lines = File.ReadAllLines(configPath);
                    var config = new DatabaseConfig();

                    foreach (string line in lines)
                    {
                        if (line.Contains("="))
                        {
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                string key = parts[0].Trim();
                                string value = parts[1].Trim();

                                switch (key)
                                {
                                    case "Server": config.Server = value; break;
                                    case "Database": config.Database = value; break;
                                    case "Username": config.Username = value; break;
                                    case "Password": config.Password = value; break;
                                    case "UseWindowsAuth": config.UseWindowsAuth = bool.Parse(value); break;
                                }
                            }
                        }
                    }
                    return config;
                }
            }
            catch
            {
                // Игнорируем ошибки
            }

            return new DatabaseConfig();
        }

        public void Save()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.config");
                var sb = new StringBuilder();

                sb.AppendLine($"Server={Server}");
                sb.AppendLine($"Database={Database}");
                sb.AppendLine($"Username={Username}");
                sb.AppendLine($"Password={Password}");
                sb.AppendLine($"UseWindowsAuth={UseWindowsAuth}");

                File.WriteAllText(configPath, sb.ToString());
            }
            catch
            {
                // Игнорируем ошибки
            }
        }
    }
}
