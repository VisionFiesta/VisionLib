using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Char
{
    public class NcCharClientBaseCmd : NetPacketStruct
    {
        public uint CharNo;
        public string CharName;
        public byte Slot;
        public byte Level;
        public ulong EXP;
        public byte[] Unk00 = new byte[4];
        public ushort HPStones;
        public ushort SPStones;
        public uint HP;
        public uint SP;
        public uint LP;
        public uint Fame;
        public uint Money;
        public byte Unk01;
        public string MapName;
        public ShineXYR Position = new ShineXYR();
        public byte STRBonus;
        public byte ENDBonus;
        public byte DEXBonus;
        public byte INTBonus;
        public byte SPRBonus;
        public byte[] Unk02 = new byte[2];
        public uint KillPoints;
        public byte[] Unk03 = new byte[8];

        public override int GetSize() => 106; // as of 1.02.276, NA server

        public override void Read(ReaderStream reader)
        {
            CharNo = reader.ReadUInt32();
            CharName = reader.ReadString(NameN.Name5Len);
            Slot = reader.ReadByte();
            Level = reader.ReadByte();
            EXP = reader.ReadUInt64();
            Unk00 = reader.ReadBytes(4);
            HPStones = reader.ReadUInt16();
            SPStones = reader.ReadUInt16();
            HP = reader.ReadUInt32();
            SP = reader.ReadUInt32();
            LP = reader.ReadUInt32();
            Fame = reader.ReadUInt32();
            Money = reader.ReadUInt32();
            Unk01 = reader.ReadByte();
            MapName = reader.ReadString(NameN.Name3Len);

            Position.Read(reader);

            STRBonus = reader.ReadByte();
            ENDBonus = reader.ReadByte();
            DEXBonus = reader.ReadByte();
            INTBonus = reader.ReadByte();
            SPRBonus = reader.ReadByte();
            Unk02 = reader.ReadBytes(2);
            KillPoints = reader.ReadUInt32();
            Unk03 = reader.ReadBytes(8);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharNo);
            writer.Write(CharName);
            writer.Write(Slot);
            writer.Write(Level);
            writer.Write(EXP);
            writer.Fill(4, 0x00); // Unk00 from NA capture
            writer.Write(HPStones);
            writer.Write(SPStones);
            writer.Write(HP);
            writer.Write(SP);
            writer.Write(LP);
            writer.Write(Fame);
            writer.Write(Money);
            writer.Fill(1, 0x00); // Unk01 from NA capture
            writer.Write(MapName);

            Position.Write(writer);

            writer.Write(STRBonus);
            writer.Write(ENDBonus);
            writer.Write(DEXBonus);
            writer.Write(INTBonus);
            writer.Write(SPRBonus);
            writer.Write(new byte[] { 0x0A, 0x00 }); // Unk02 from NA capture
            writer.Write(KillPoints);
            writer.Fill(8, 0x00); // Unk03 from NA capture
        }

        public override NetCommand GetCommand() => NetCommand.NC_CHAR_CLIENT_BASE_CMD;
    }
}
