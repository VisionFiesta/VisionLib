using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.IO.SHN
{
    public class SimpleSHNLoader
    {
        private static readonly EngineLog Logger = new EngineLog(typeof(SimpleSHNLoader));

        private static readonly Type[] AllFileDefinitions = VisionAssembly.VisionTypes.Where(type => type.GetCustomAttributes(typeof(Definition), true).Length > 0).ToArray();
        private static readonly Type ObjectCollectionType = typeof(ObjectCollection<>);
        private static readonly List<SHNType> Types = new List<SHNType>();
        private static readonly Dictionary<SHNType, dynamic> FileObjects = new Dictionary<SHNType, dynamic>();

        private static string _shnFolder;
        private static ISHNCrypto _crypto;
        private static int _count;

        protected internal static bool Initialize(string shnFolder, ISHNCrypto crypto)
        {
            _shnFolder = shnFolder;
            _crypto = crypto;

            if (AllFileDefinitions.Length <= 0)
            {
                Logger.Error("SimpleSHNLoader->SimpleSHNLoader() : No file definitions were found!");
                return false;
            }

            Logger.Info($"SimpleSHNLoader->Initialize() : Loaded {AllFileDefinitions.Length} SHN definitions");
            return true;
        }

        protected internal static void Load(bool useParallel, params SHNType[] types)
        {
            var newTypes = types.Where(shnType => !Types.Contains(shnType)).ToList();
            var parallelTypes = newTypes.Where(shnType => shnType.IsLarge());
            var nonParallelTypes = newTypes.Where(shnType => !shnType.IsLarge());

            if (useParallel)
            {
                Parallel.Invoke(new ParallelOptions(), new Action[]
                {
                    () => Parallel.ForEach(parallelTypes, async shnType =>
                    {
                        if (await SingleLoadAsync(shnType, true))
                        {
                            Types.Add(shnType);
                        }
                    }),
                    async () =>
                    {
                        foreach (var shnType in nonParallelTypes)
                        {
                            if (await SingleLoadAsync(shnType, false))
                            {
                                Types.Add(shnType);
                            }
                        }
                    }

                });
            }
            else
            {
                foreach (var shnType in newTypes)
                {
                    if (SingleLoadAsync(shnType, false).Result)
                    {
                        Types.Add(shnType);
                    }
                }
            }
        }

        private static async Task<bool> SingleLoadAsync(SHNType type, bool parallel)
        {
            var definition = AllFileDefinitions.FirstOrDefault(def => def.Name == type.ToString());
            if (definition == null)
            {
                Logger.Warning($"SimpleSHNLoader->SingleLoad() : No definition for SHN: {type}");
                return false;
            }

            var collection = ObjectCollectionType.MakeGenericType(definition);
            var created = (dynamic)Activator.CreateInstance(collection);

            if (parallel)
            {
                var mutex = new Mutex(false, type.ToString());

                mutex.WaitOne();

                FileObjects.Remove(type);
                FileObjects.Add(type, created);

                mutex.ReleaseMutex();
            }
            else
            {
                FileObjects.Remove(type);
                FileObjects.Add(type, created);
            }

            using (var file = new SimpleSHNFile(Path.Combine(_shnFolder, type.ToFilename()), _crypto))
            using (var reader = new DataTableReader(file._table))
            {
                var res = true;
                while (res)
                {
                    created.Add(reader);
                    res = await reader.ReadAsync();
                }
            }

            return true;
        }

        protected internal static bool TryGetObjects<T>(SHNType type, out ObjectCollection<T> objects)
        {
            objects = new ObjectCollection<T>();

            if (_count == 0) return false;
            if (!Types.Contains(type)) return false;
            if (!FileObjects.ContainsKey(type)) return false;

            objects = FileObjects[type] as ObjectCollection<T>;
            _count++;
            return true;
        }
    }
}
