﻿using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Core.Utils;

namespace Vision.Game.Structs.Bat
{
    public class NcBatSkillBashDamageCmd : NetPacketStruct
    {
        public readonly ushort Index;
        public readonly ushort Caster;
        public readonly byte TargetCount;
        public readonly ushort SkillID;
        public readonly ushort Target;
        public readonly SkillDamage[] SkillDamageTargets;

        public NcBatSkillBashDamageCmd(ushort index, ushort caster, ushort skillId,
            ushort target, SkillDamage[] skillDamageTargets)
        {
            Index = index;
            Caster = caster;
            TargetCount = (byte)skillDamageTargets.Length;
            SkillID = skillId;
            Target = target;
            SkillDamageTargets = skillDamageTargets;
        }

        public override int GetSize()
        {
            var sdtSize = SkillDamageTargets.Length * SkillDamage.Size;
            return sizeof(byte) + sizeof(ushort) * 4 + sdtSize;
        }

        public override void Read(ReaderStream reader)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Index);
            writer.Write(Caster);
            writer.Write(TargetCount);
            writer.Write(SkillID);
            foreach (var sdt in SkillDamageTargets)
            {
                sdt.Write(writer);
            }
        }

        protected override NetCommand GetCommand()
        {
            return NetCommand.NC_BAT_SKILLBASH_HIT_DAMAGE_CMD;
        }
    }

    public class SkillDamage : AbstractStruct
    {
        protected internal const int Size = sizeof(ushort) * 3 + sizeof(uint) * 2;

        public ushort Handle;
        public SkillDamageFlags Flags;
        public uint HPChange;
        public uint RestHP;
        public ushort HPChangeOrder;

        public class SkillDamageFlags
        {
            public bool IsDamage;
            public bool IsCritical;
            public bool IsMissed;
            public bool IsShieldBlock;
            public bool IsHeal;
            public bool IsEnchant;
            public bool IsResist;
            public bool IsCostumeWeapon;
            public bool IsDead;
            public bool IsImmune;
            public bool IsCostumeShield;

            public SkillDamageFlags(ushort flag)
            {
                using var bs = new BitStream();
                bs.Write(flag);
                bs.Read(out IsDamage);
                bs.Read(out IsCritical);
                bs.Read(out IsMissed);
                bs.Read(out IsShieldBlock);
                bs.Read(out IsHeal);
                bs.Read(out IsEnchant);
                bs.Read(out IsResist);
                bs.Read(out IsCostumeWeapon);
                bs.Read(out IsDead);
                bs.Read(out IsImmune);
                bs.Read(out IsCostumeShield);
            }

            public ushort ToUShort()
            {
                using var bs = new BitStream();
                bs.Write(new[]
                {
                    IsDamage,
                    IsCritical,
                    IsMissed,
                    IsShieldBlock,
                    IsHeal,
                    IsEnchant,
                    IsResist,
                    IsCostumeWeapon,
                    IsDead,
                    IsImmune,
                    IsCostumeShield
                });

                bs.Read(out ushort flag);
                return flag;
            }
        }

        public override int GetSize()
        {
            return Size;
        }

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            Flags = new SkillDamageFlags(reader.ReadUInt16());
            HPChange = reader.ReadUInt32();
            RestHP = reader.ReadUInt32();
            HPChangeOrder = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            if (writer == null) return;
            writer.Write(Handle);
            writer.Write(Flags.ToUShort());
            writer.Write(HPChange);
            writer.Write(RestHP);
            writer.Write(HPChangeOrder);
        }
    }
}

