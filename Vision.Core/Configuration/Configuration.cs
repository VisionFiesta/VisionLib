using System;
using System.IO;
using Newtonsoft.Json;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.Configuration
{
    public class Configuration<T>
    {
        // private const string ConfigFolder = "Config";

        protected static readonly EngineLog Logger = new(typeof(Configuration<T>));

        public T ConfigurationData { get; private set; }
        private readonly string configFolderPath;

        public Configuration(string configFolderPath)
        {
            this.configFolderPath = configFolderPath;
        }

        protected bool Load(out string message)
        {
            ConfigurationData = Initialize(out message);
            return message == string.Empty;
        }

        private bool WriteJson(string configName = null)
        {
            if (!Directory.Exists(configFolderPath)) return false;

            var path = Path.Combine(configFolderPath, $"{configName ?? typeof(T).Name}.json");

            var writer = new JsonSerializer();
            using var file = new StreamWriter(path);
            writer.Formatting = Formatting.Indented;
            writer.Serialize(file, ConfigurationData);

            return true;
        }

        private T ReadJson(string configName = null)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            };

            var path = Path.Combine(configFolderPath, $"{configName ?? typeof(T).Name}.json");
            if (!File.Exists(path)) return default;

            using var file = File.OpenText(path);
            var text = file.ReadToEnd();
            var obj = JsonConvert.DeserializeObject<T>(text, settings);
            return obj;
        }

        private T Initialize(out string message)
        {
            var fullTypeName = typeof(T).FullName?.Replace("Vision.Core.Configuration.", "");
            var shortTypeName = fullTypeName?.Replace("Configuration", "");

            try
            {
                var instance = ReadJson(shortTypeName);
                if (instance != null)
                {
                    Logger.Info($"Successfully read {shortTypeName} config.");
                    message = "";
                    return instance;
                }

                if (!WriteJson())
                {
                    message= $"Failed to write to JSON file: {shortTypeName}.json - Ensure destination directory exists";
                    return default;
                }

                Logger.Info($"Successfully generated {shortTypeName} config.");
                message = $"No {fullTypeName} found! Please edit generated config.";
                return default;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load {shortTypeName} config:\n {0}", ex);
                message = $"Failed to load {fullTypeName}:\n {ex.StackTrace}";
                return default;
            }
        }
    }
}
