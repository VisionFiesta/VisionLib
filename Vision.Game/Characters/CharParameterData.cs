using System;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Characters
{
    public class CharParameterData : AbstractStruct
    {
        public const int Size = 232;

        /*
         * struct CHAR_PARAMETER_DATA
           {
           unsigned __int64 PrevExp;
           unsigned __int64 NextExp;
           SHINE_CHAR_STATVAR Strength;
           SHINE_CHAR_STATVAR Constitute;
           SHINE_CHAR_STATVAR Dexterity;
           SHINE_CHAR_STATVAR Intelligence;
           SHINE_CHAR_STATVAR Wizdom;
           SHINE_CHAR_STATVAR MentalPower;
           SHINE_CHAR_STATVAR WClow;
           SHINE_CHAR_STATVAR WChigh;
           SHINE_CHAR_STATVAR AC;
           SHINE_CHAR_STATVAR TH;
           SHINE_CHAR_STATVAR TB;
           SHINE_CHAR_STATVAR MAlow;
           SHINE_CHAR_STATVAR MAhigh;
           SHINE_CHAR_STATVAR MR;
           SHINE_CHAR_STATVAR MH;
           SHINE_CHAR_STATVAR MB;
           unsigned int MaxHp;
           unsigned int MaxSp;
           unsigned int MaxLp;
           unsigned int MaxAp;
           unsigned int MaxHPStone;
           unsigned int MaxSPStone;
           // $A0609CE5C39123E94A9A13D20640EA5B PwrStone;
           // $A0609CE5C39123E94A9A13D20640EA5B GrdStone;
           SHINE_CHAR_STATVAR PainRes;
           SHINE_CHAR_STATVAR RestraintRes;
           SHINE_CHAR_STATVAR CurseRes;
           SHINE_CHAR_STATVAR ShockRes;
           };
           
         */
        public ulong PrevExp { get; set; }
        public ulong NextExp { get; set; }

        public CharParameter Strength { get; } = new();
        public CharParameter Constitution { get; } = new();
        public CharParameter Dexterity { get; } = new();
        public CharParameter Intelligence { get; } = new();
        public CharParameter Wisdom { get; } = new();
        public CharParameter MentalPower { get; } = new();

        public CharParameter WcLow { get; } = new();
        public CharParameter WcHigh { get; } = new();

        public CharParameter Ac { get; } = new();
        public CharParameter Th { get; } = new();
        public CharParameter Tb { get; } = new();

        public CharParameter MaLow { get; } = new();
        public CharParameter MaHigh { get; } = new();

        public CharParameter Mr { get; } = new();
        public CharParameter Mh { get; } = new();
        public CharParameter Mb { get; } = new();

        public uint HPStoneHealth { get; set; }
        public uint SPStoneSpirit { get; set; }
        public uint MaxHPStones { get; set; }
        public uint MaxSPStones { get; set; }

        // $A0609CE5C39123E94A9A13D20640EA5B PwrStone;
        // private CharParameterStone HPStone;
        // $A0609CE5C39123E94A9A13D20640EA5B GrdStone;
        // private CharParameterStone SPStone;

        public uint HPStonePrice { get; set; }
        public uint SPStonePrice { get; set; }
        public CharParameter IllnessResistance { get; } = new();
        public CharParameter DiseaseResistance { get; } = new();
        public CharParameter CurseResistance { get; } = new();
        public CharParameter StunResistance { get; } = new();
        public uint MaxHP { get; set; }
        public uint MaxSP { get; set; }
        public uint MaxLP { get; set; } = 100;
        public uint SkillPoints { get; set; }

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            PrevExp = reader.ReadUInt64();
            NextExp = reader.ReadUInt64();

            Strength.Read(reader);
            Constitution.Read(reader);
            Intelligence.Read(reader);
            Wisdom.Read(reader);
            MentalPower.Read(reader);
            WcLow.Read(reader);
            WcHigh.Read(reader);
            Ac.Read(reader);
            Th.Read(reader);
            Tb.Read(reader);
            MaLow.Read(reader);
            MaHigh.Read(reader);
            Mr.Read(reader);
            Mh.Read(reader);
            Mb.Read(reader);

            MaxHP = reader.ReadUInt32();
            MaxSP = reader.ReadUInt32();
            MaxLP = reader.ReadUInt32();
            /*MaxAP =*/ reader.ReadUInt32();
            MaxHPStones = reader.ReadUInt32();
            MaxSPStones = reader.ReadUInt32();

            // todo: stone fix
            var _ = reader.ReadBytes(40);
           
            IllnessResistance.Read(reader);
            DiseaseResistance.Read(reader);
            CurseResistance.Read(reader);
            StunResistance.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            // todo: never lmfao
        }
    }

    public class CharParameter : AbstractStruct
    {
        public const int Size = 8;

        public uint Base { get; private set; }
        public uint Bonus { get; private set; }

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Base = reader.ReadUInt32();
            Bonus = reader.ReadUInt32();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Base);
            writer.Write(Bonus);
        }
    }

    public class CharParameterStone : AbstractStruct
    {
        public const int Size = 16;

        public uint Flag;
        public uint EPPhysic;
        public uint EPMagic;
        public uint MaxStone;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Flag = reader.ReadUInt32();
            EPPhysic = reader.ReadUInt32();
            EPMagic = reader.ReadUInt32();
            MaxStone = reader.ReadUInt32();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Flag);
            writer.Write(EPPhysic);
            writer.Write(EPMagic);
            writer.Write(MaxStone);
        }
    }
}
