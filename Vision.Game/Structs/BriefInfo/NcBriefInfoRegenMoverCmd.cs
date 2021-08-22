using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoRegenMoverCmd : NetPacketStruct
    {
        /*
        struct __unaligned __declspec(align(1)) PROTO_NC_BRIEFINFO_REGENMOVER_CMD
        {
            unsigned __int16 nHandle;
            unsigned int nID;
            unsigned int nHP;
            SHINE_COORD_TYPE nCoord;
            ABNORMAL_STATE_BIT AbstateBit;
            char nGrade;
            unsigned __int16 nSlotHandle[10];
        };
        */

        public ushort Handle; // 2
        public uint ID; // 4
        public uint HP; // 4
        public ShineXY Position = new(); // 8
        public AbnormalStateBit AbstateBit = new(); // 112
        public byte Grade; // 1
        public ushort[] SlotHandle; // 2 * 10

        // Should be 151. 
        public override int GetSize() => 2 + 4 + 4 + ShineXY.Size + AbnormalStateBit.Size + 1 + 20;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            ID = reader.ReadUInt32();
            HP = reader.ReadUInt32();

            Position.Read(reader);
            AbstateBit.Read(reader);

            Grade = reader.ReadByte();

            SlotHandle = new ushort[10];
            for (var i = 0; i < SlotHandle.Length; i++)
            {
                SlotHandle[i] = reader.ReadUInt16();
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(ID);
            writer.Write(HP);

            Position.Write(writer);
            AbstateBit.Write(writer);

            writer.Write(Grade);

            for (var i = 0; i < SlotHandle.Length; i++)
            {
                writer.Write(SlotHandle[i]);
            }
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_REGENMOVER_CMD;

        public override string ToString()
        {
            return $"{nameof(Handle)}: {Handle}, {nameof(ID)}: {ID}, {nameof(HP)}: {HP}, {nameof(Position)}: {Position}, {nameof(Grade)}: {Grade}";
        }
    }
}
