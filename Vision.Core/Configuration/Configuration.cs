using System;
using System.IO;
using Newtonsoft.Json;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.Configuration
{
    public abstract class Configuration<T>
    {
        private static readonly EngineLog Logger = new(typeof(Configuration<T>));

        public T Data { get; protected set; }

        public bool Initialized { get; protected set; }
        
        private readonly string _configFolderPath;
        private readonly string _shortConfigName;
        private readonly string _configFilePath;

        protected Configuration(string configFolderPath, bool useDefaults = false)
        {
            if (useDefaults)
            {
                // Actually safe as this shall never call any child members, only other-class `new()`s
                // ReSharper disable once VirtualMemberCallInConstructor
                Data = GetDataDefault();
                Initialized = true;
                return;
            }
            
            _configFolderPath = Path.GetFullPath(configFolderPath);
            // remove namespace and Configuration descriptor from typename
            _shortConfigName = GetType().Name.Replace("Vision.Core.Configuration.", "").Replace("Configuration", "");
            _configFilePath = Path.Join(_configFolderPath, $"{_shortConfigName}.json");

            Data = Initialize(out var ret);
            Initialized = ret;

        }

        protected abstract T GetDataDefault();
        
        public bool Reload()
        {
            // attempt reload - fails safe to existing data
            var result = Initialize(out var ret);
            if (ret) Data = result;
            return ret;
        }
        
        protected bool Write()
        {
            if (WriteDataToFile()) return true;
            Logger.Error(
                $"Failed to write to JSON file: {_shortConfigName}.json! Ensure destination directory \"{_configFolderPath}\" exists");
            return false;
        }
        
        private bool WriteDataToFile()
        {
            if (!Directory.Exists(_configFolderPath)) return false;
            
            var writer = new JsonSerializer();
            using var file = new StreamWriter(_configFilePath);
            writer.Formatting = Formatting.Indented;
            writer.Serialize(file, Data);

            return true;
        }

        private T LoadDataFromFile()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            };

            if (!File.Exists(_configFilePath)) return default;

            using var file = File.OpenText(_configFilePath);
            return JsonConvert.DeserializeObject<T>(file.ReadToEnd(), settings);
        }
        
        private T Initialize(out bool status)
        {
            status = false;

            try
            {
                if (!File.Exists(_configFilePath))
                {
                    Data = GetDataDefault();
                    if (!WriteDataToFile())
                    {
                        Logger.Error($"Failed to write to JSON file: {_shortConfigName}.json! Ensure destination directory \"{_configFolderPath}\" exists");
                        return Data;
                    }
                    
                    Logger.Info($"Generated missing {_shortConfigName} config. This may need to be edited for proper operation.");
                    status = true;
                    return Data;
                }

                var data = LoadDataFromFile();
                if (data != null)
                {
                    Logger.Info($"Successfully read {_shortConfigName} config.");
                    status = true;
                    return data;
                }
                
                Logger.Error($"Failed to generate {_shortConfigName} for unknown reason! Using default ctor.");
                return default;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load {_shortConfigName} config:\n {0}", ex);
                return default;
            }
        }
    }
}
