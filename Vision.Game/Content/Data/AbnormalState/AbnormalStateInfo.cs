using System.Collections.Concurrent;
using System.Collections.Generic;
using Vision.Core.IO.SHN;

namespace Vision.Game.Content.Data.AbnormalState
{
    public class AbnormalStateInfo
    {
        public ushort ID { get; }
        public string Name { get; }
        public AbnormalStateIndex AbnormalStateIndex { get; }
        public ushort KeepTimeRatio { get; }
        public ushort KeepTimePower { get; }
        public List<AbnormalStateInfo> PartyStates { get; }
        public byte Grade { get; }
        public ushort PartyRange { get; }
        public ConcurrentDictionary<uint, SubAbnormalStateInfo> SubAbnormalStates { get; }
        public byte AbstateSaveType { get; }

        public AbnormalStateInfo MainAbState { get; set; }

        public AbnormalStateInfo(SHNResult result, int rIndex)
        {
            ID = result.Read<ushort>(rIndex, "ID");
            Name = result.Read<string>(rIndex, "InxName");
            AbnormalStateIndex = (AbnormalStateIndex) result.Read<ushort>(rIndex, "AbStataIndex");
            KeepTimeRatio = result.Read<ushort>(rIndex, "KeepTimeRatio");
            KeepTimePower = result.Read<ushort>(rIndex, "KeepTimePower");
            Grade = result.Read<byte>(rIndex, "StateGrade");

            PartyStates = new List<AbnormalStateInfo>();


            string[] partyStateNames = new string[5];
            for (var i = 0; i < 5; i++)
            {
                var psn = result.Read<string>(rIndex, $"PartyState{i + 1}");
                if (psn == "-") break;
                partyStateNames[i] = psn;
            }

            if (partyStateNames[0] != null)
            {

            }


            

            SubAbnormalStates = new ConcurrentDictionary<uint, SubAbnormalStateInfo>();

            AbstateSaveType = result.Read<byte>(rIndex, "AbstateSaveType");


        }
    }
}
