using System;
using System.IO;
using Newtonsoft.Json;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.Configuration
{
    public class Config<T> where T : Config<T>
    {
        private const string ConfigFolder = "conf";

        private static T _instance;
        public static T Instance => _instance ??= Initialize();

        private static T ReadJson(string configName = null)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            };

            var path = Path.Combine(ConfigFolder, $"{configName ?? typeof(T).Name}.json");
            if (!File.Exists(path)) return default;

            using var file = File.OpenText(path);
            var text = file.ReadToEnd();
            var obj = JsonConvert.DeserializeObject<T>(text, settings);
            return obj;
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

        private static T Initialize()
        {
            var fullTypeName = typeof(T).FullName?.Replace("Vision.Core.Configuration.", "");
            var shortTypeName = fullTypeName?.Replace("Configuration", "");

            try
            {
                var inst = ReadJson();
                if (inst != null)
                {
                    EngineLog.Info($"Successfully read {shortTypeName} config.");
                    return inst;
                }

                if (!Write(out var pConfig))
                {
                    EngineLog.Error($"Failed to create default {fullTypeName}.");
                    return default;
                }

                pConfig.WriteJson();

                EngineLog.Info($"Successfully generated {shortTypeName} config. Config may need to be edited.");
                return default;
            }
            catch (Exception ex)
            {
                EngineLog.Error($"Failed to load {shortTypeName} config:\n {0}", ex);
                return default;
            }
        }

        private static bool Write(out T pConfig)
        {
            pConfig = default(T);
            try
            {
                pConfig = (T)Activator.CreateInstance(typeof(T));
                return true;
            }
            catch { return false; }
        }
    }
}
