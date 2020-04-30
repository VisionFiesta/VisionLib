using System;
using System.IO;
using Newtonsoft.Json;
using Vision.Core.Common.Configuration;
using Vision.Core.Common.Logging.Loggers;

namespace Vision.Core.Configuration
{
    public class Configuration<T>
    {
        private const string ConfigFolder = "Config";

        public static T Instance { get; set; }

        public static bool Load(out string message)
        {
            Instance = Initialize(out message);
            return message == string.Empty;
        }

        private static void CreateDefaultFolder()
        {
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }
        }

        private void WriteJson(string configName = null)
        {
            CreateDefaultFolder();
            var path = Path.Combine(ConfigFolder, $"{configName ?? typeof(T).Name}.json");

            var writer = new JsonSerializer();
            var file = new StreamWriter(path);
            writer.Formatting = Formatting.Indented;
            writer.Serialize(file, this);
            file.Close();
        }

        private static T ReadJson(string configName = null)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            };

            var path = Path.Combine(ConfigFolder, $"{configName ?? typeof(T).Name}.json");
            if (!File.Exists(path)) return default;

            using (var file = File.OpenText(path))
            {
                var text = file.ReadToEnd();
                var obj = JsonConvert.DeserializeObject<T>(text, settings);
                return obj;
            }
        }

        private static T Initialize(out string message)
        {
            var fullTypeName = typeof(T).FullName?.Replace("Vision.Client.Configuration.", "");
            var shortTypeName = fullTypeName?.Replace("Configuration", "");

            try
            {
                var instance = ReadJson();
                if (instance != null)
                {
                    EngineLog.Info($"Successfully read {shortTypeName} config.");
                    message = "";
                    return instance;
                }

                if (!Write(out var pConfig))
                {
                    message = $"Failed to create default {fullTypeName}.";
                    return default;
                }
                pConfig.WriteJson();

                EngineLog.Info($"Successfully generated {shortTypeName} config.");
                message = $"No {fullTypeName} found! Please edit generated config.";
                return default;
            }
            catch (Exception ex)
            {
                EngineLog.Error($"Failed to load {shortTypeName} config:\n {0}", ex);
                message = $"Failed to load {fullTypeName}:\n {ex.StackTrace}";
                return default;
            }
        }

        private static bool Write(out dynamic pConfig)
        {
            pConfig = default(T);
            try
            {
                pConfig = (T)Activator.CreateInstance(typeof(T));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
