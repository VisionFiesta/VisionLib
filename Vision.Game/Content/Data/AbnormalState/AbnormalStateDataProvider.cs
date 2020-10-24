using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Vision.Core.IO.SHN;
using Vision.Core.Logging.Loggers;

namespace Vision.Game.Content.Data.AbnormalState
{
    public class AbnormalStateDataProvider
    {
        private static readonly EngineLog Logger = new EngineLog(typeof(AbnormalStateDataProvider));

        public const ushort RestEXPID = 8817;

        private readonly SHNManager _shnManager;
        public AbnormalStateInfo RestEXP { get; private set; }

        private readonly ConcurrentDictionary<ushort, SubAbnormalStateInfo> _subAbnormalStatesDataByID = new ConcurrentDictionary<ushort, SubAbnormalStateInfo>();
        private readonly ConcurrentDictionary<ushort, List<SubAbnormalStateInfo>> _subAbnormalStatesByID = new ConcurrentDictionary<ushort, List<SubAbnormalStateInfo>>();
        private readonly ConcurrentDictionary<ushort, AbnormalStateInfo> _abnormalStatesByID = new ConcurrentDictionary<ushort, AbnormalStateInfo>();
        private readonly ConcurrentDictionary<AbnormalStateIndex, AbnormalStateInfo> _abnormalStatesByAbnormalStateIndex = new ConcurrentDictionary<AbnormalStateIndex, AbnormalStateInfo>();

        public AbnormalStateDataProvider(SHNManager shnManager)
        {
            _shnManager = shnManager;
        }

        public bool TryGetAbnormalState(ushort id, out AbnormalStateInfo info) =>
            _abnormalStatesByID.TryGetValue(id, out info);

        public bool TryGetAbnormalState(AbnormalStateIndex index, out AbnormalStateInfo info) =>
            _abnormalStatesByAbnormalStateIndex.TryGetValue(index, out info);

        public bool Initialize()
        {
            var ok = LoadSubAbnormalStates();
            if (!LoadAbnormalStates()) ok = false;
            if (!LoadAbnormalStateExtras()) ok = false;
            return ok;
        }

        private bool LoadSubAbnormalStates()
        {
            var subabstateShnLoader = _shnManager.GetSHNLoader(SHNType.SubAbState);
            subabstateShnLoader.Load((result, index) =>
            {
                var info = new SubAbnormalStateInfo(result, index);
                if (!_subAbnormalStatesDataByID.TryAdd(info.ID, info))
                {
                    subabstateShnLoader.QueueMessage(EngineLogLevel.ELL_WARNING, $"AbnormalStateDataProvider->LoadSubAbnormalStates(): Duplicate SubAbnormalState ID found: {info.ID}");
                }

                if (!_subAbnormalStatesByID.TryGetValue(info.ID, out var list))
                {
                    list = new List<SubAbnormalStateInfo>();
                    _subAbnormalStatesByID.TryAdd(info.ID, list);
                }

                list.Add(info);
            });

            return true;
        }

        private bool LoadAbnormalStates()
        {
            var abstateShnLoader = _shnManager.GetSHNLoader(SHNType.AbState);
            abstateShnLoader.Load((result, index) =>
            {
                var info = new AbnormalStateInfo(result, index);
                if (!_abnormalStatesByID.TryAdd(info.ID, info))
                {
                    abstateShnLoader.QueueMessage(EngineLogLevel.ELL_WARNING, $"AbnormalStateDataProvider->LoadAbnormalStates() : Duplicate AbnormalState ID found: {info.ID}");
                }

                if (!_abnormalStatesByAbnormalStateIndex.TryAdd(info.AbnormalStateIndex, info))
                {
                    _abnormalStatesByID.TryRemove(info.ID, out info);
                    abstateShnLoader.QueueMessage(EngineLogLevel.ELL_WARNING, $"AbnormalStateDataProvider->LoadAbnormalStates() : Duplicate AbnormalStateIndex found: {info.AbnormalStateIndex}");
                }
            });

            return true;
        }

        private bool LoadAbnormalStateExtras()
        {
            if (!TryGetAbnormalState(RestEXPID, out var abState))
            {
                Logger.Error($"AbnormalStateDataProvider->LoadAbnormalStateExtras() : Can't find 'Rest EXP' buff (ID: {RestEXPID}).");
                return false;
            }
            RestEXP = abState;
            abState.SubAbnormalStates.TryAdd(0, new SubAbnormalStateInfo(ushort.MaxValue, 0, 0, 0, TimeSpan.Zero));
            return true;
        }
    }
}
