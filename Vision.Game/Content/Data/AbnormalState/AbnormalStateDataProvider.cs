using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Vision.Core.IO.SHN;
using Vision.Core.Logging.Loggers;

namespace Vision.Game.Content.Data.AbnormalState
{
    public class AbnormalStateDataProvider
    {
        public const ushort RestEXPID = 8817;
        public static AbnormalStateInfo RestEXP { get; private set; }

        private static ConcurrentDictionary<ushort, SubAbnormalStateInfo> _subAbnormalStatesDataByID;
        private static ConcurrentDictionary<ushort, List<SubAbnormalStateInfo>> _subAbnormalStatesByID;
        private static ConcurrentDictionary<ushort, AbnormalStateInfo> _abnormalStatesByID;
        private static ConcurrentDictionary<AbnormalStateIndex, AbnormalStateInfo> _abnormalStatesByAbnormalStateIndex;

        public static bool TryGetAbnormalState(ushort id, out AbnormalStateInfo info) =>
            _abnormalStatesByID.TryGetValue(id, out info);

        public static bool TryGetAbnormalState(AbnormalStateIndex index, out AbnormalStateInfo info) =>
            _abnormalStatesByAbnormalStateIndex.TryGetValue(index, out info);

        public static bool Initialize()
        {
            var ok = LoadSubAbnormalStates();
            if (!LoadAbnormalStates()) ok = false;
            if (!LoadAbnormalStateExtras()) ok = false;
            return ok;
        }

        private static bool LoadSubAbnormalStates()
        {
            _subAbnormalStatesByID = new ConcurrentDictionary<ushort, List<SubAbnormalStateInfo>>();
            _subAbnormalStatesDataByID = new ConcurrentDictionary<ushort, SubAbnormalStateInfo>();

            var subabstateShnLoader = new SHNLoader(SHNType.SubAbState);
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

        private static bool LoadAbnormalStates()
        {
            _abnormalStatesByID = new ConcurrentDictionary<ushort, AbnormalStateInfo>();
            _abnormalStatesByAbnormalStateIndex = new ConcurrentDictionary<AbnormalStateIndex, AbnormalStateInfo>();

            var abstateShnLoader = new SHNLoader(SHNType.AbState);
            abstateShnLoader.Load((result, index) =>
            {
                var info = new AbnormalStateInfo(result, index);
                if (!_abnormalStatesByID.TryAdd(info.ID, info))
                {
                    abstateShnLoader.QueueMessage(EngineLogLevel.ELL_WARNING, $"AbnormalStateDataProvider->LoadAbnormalStates(): Duplicate AbnormalState ID found: {info.ID}");
                }

                if (!_abnormalStatesByAbnormalStateIndex.TryAdd(info.AbnormalStateIndex, info))
                {
                    _abnormalStatesByID.TryRemove(info.ID, out info);
                    abstateShnLoader.QueueMessage(EngineLogLevel.ELL_WARNING, $"AbnormalStateDataProvider->LoadAbnormalStates(): Duplicate AbnormalStateIndex found: {info.AbnormalStateIndex}");
                }
            });

            return true;
        }

        private static bool LoadAbnormalStateExtras()
        {
            if (!TryGetAbnormalState(RestEXPID, out AbnormalStateInfo abState))
                throw new InvalidOperationException($"Can't find 'Rest EXP' buff (ID: {RestEXPID}).");
            RestEXP = abState;
            abState.SubAbnormalStates.TryAdd(0, new SubAbnormalStateInfo(ushort.MaxValue, 0, 0, 0, TimeSpan.Zero));
            return true;
        }
    }
}
