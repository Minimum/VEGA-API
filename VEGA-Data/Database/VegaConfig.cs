using Npgsql;
using System;

namespace VEGA_Data.Database
{
    public class VegaConfig
    {
        public const String DataPath = "/vega";

        public static VegaConfig Instance { get; private set; }

        public String DataHostname { get; set; }
        public int DataPort { get; set; }
        public String DataUsername { get; set; }
        public String DataPassword { get; set; }
        public String DataDatabase { get; set; }
        public String DataConnectionString { get; private set; }

        public String UploadPath { get; set; }

        public VegaConfig()
        {
            DataHostname = "localhost";
            DataPort = 5432;
            DataUsername = "";
            DataPassword = "";
            DataDatabase = "vega";
            UploadPath = "uploads";
        }

        public static void Initialize(VegaConfig config)
        {
            if (Instance == null)
            {
                Instance = config;
            }

            Instance.DataConnectionString = BuildConnectionString(Instance);

            return;
        }

        public String GetFullUploadPath()
        {
            return DataPath + "/" + UploadPath;
        }

        private static String BuildConnectionString(VegaConfig config)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder
            {
                Host = config.DataHostname,
                Port = config.DataPort,
                Username = config.DataUsername,
                Password = config.DataPassword,
                Database = config.DataDatabase,
                Pooling = true,
                MaxPoolSize = 500
            };

            return builder.ToString();
        }
    }
}
