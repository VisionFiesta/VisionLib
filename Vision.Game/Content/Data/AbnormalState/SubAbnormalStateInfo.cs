using System;
using System.Collections.Generic;
using Vision.Core.IO.SHN;
using Vision.Core.Utils;

namespace Vision.Game.Content.Data.AbnormalState
{
    public class SubAbnormalStateInfo
    {
        public ushort ID { get; }

        public uint Strength { get; }
        public byte Type { get; }
        public byte SubType { get; }
        public TimeSpan KeepTime { get; }
        public List<SubAbnormalStateAction> Actions { get; }

        public SubAbnormalStateInfo(ushort id, uint strength, byte type, byte subType, TimeSpan keepTime, params SubAbnormalStateAction[] actions)
        {
            ID = id;
            Strength = strength;
            Type = type;
            SubType = subType;
            KeepTime = keepTime;
            Actions = new List<SubAbnormalStateAction>(actions);
        }

        public SubAbnormalStateInfo(SHNResult result, int rIndex)
        {
            ID = result.Read<ushort>(rIndex, "ID");
            Strength = result.Read<uint>(rIndex, "Strength");
            Type = result.Read<byte>(rIndex, "Type");
            SubType = result.Read<byte>(rIndex, "SubType");
            KeepTime = TimeSpan.FromMilliseconds(result.Read<uint>(rIndex, "KeepTime"));
            Actions = new List<SubAbnormalStateAction>();

            for (var i = 0; i < 4; i++)
            {
                var letter = StringUtils.CharactersUpper[i];
                uint actionIndex = result.Read<uint>(rIndex, "ActionIndex" + letter),
                    actionValue = result.Read<uint>(rIndex, "ActionArg" + letter);
                if (actionIndex == 0)
                    continue;
                Actions.Add(new SubAbnormalStateAction(actionIndex, actionValue));
            }
        }
    }
}
