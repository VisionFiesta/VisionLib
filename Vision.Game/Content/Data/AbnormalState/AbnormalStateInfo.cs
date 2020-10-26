using System.Collections.Concurrent;
using System.Collections.Generic;
using Vision.Core.IO.SHN;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Content.Data.AbnormalState
{
    public class AbnormalStateInfo : AbstractStruct
    {
        public const int Size = 12;

        public ushort ID { get; private set; }
        public string Name { get; }
        public AbnormalStateIndex AbnormalStateIndex { get; }
        public ushort KeepTimeRatio { get; private set; }
        public ushort KeepTimePower { get; private set; }
        public List<AbnormalStateInfo> PartyStates { get; }
        public byte Grade { get; }
        public ushort PartyRange { get; }
        public ConcurrentDictionary<uint, SubAbnormalStateInfo> SubAbnormalStates { get; }
        public byte AbstateSaveType { get; }

        public AbnormalStateInfo MainAbState { get; set; }

        public AbnormalStateInfo() {}

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

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            ID = reader.ReadUInt16();
            KeepTimeRatio = reader.ReadUInt16();
            KeepTimePower = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write((uint)ID);
            writer.Write((uint)KeepTimeRatio);
            writer.Write((uint)KeepTimePower);
        }

        public override string ToString() => $"{nameof(ID)}: {ID}, {nameof(Name)}: {Name}";
    }
}
