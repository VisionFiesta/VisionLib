using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Utils;

namespace VisionLib.Common.Networking.Structs.Bat
{
    public class STRUCT_NC_BAT_SKILLBASH_DAMAGE_CMD : FiestaNetPacketStruct
    {
        public readonly ushort Index;
        public readonly ushort Caster;
        public readonly byte TargetCount;
        public readonly ushort SkillID;
        public readonly ushort Target;
        public readonly SkillDamage[] SkillDamageTargets;

        public STRUCT_NC_BAT_SKILLBASH_DAMAGE_CMD(ushort index, ushort caster, ushort skillId,
            ushort target, SkillDamage[] skillDamageTargets)
        {
            Index = index;
            Caster = caster;
            TargetCount = (byte)skillDamageTargets.Length;
            SkillID = skillId;
            Target = target;
            SkillDamageTargets = skillDamageTargets;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_BAT_SKILLBASH_HIT_DAMAGE_CMD);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(Index);
            pkt.Write(Caster);
            pkt.Write(TargetCount);
            pkt.Write(SkillID);
            foreach (var sdt in SkillDamageTargets)
            {
                sdt.WriteToPacket(pkt);
            }
        }
    }

    public class SkillDamage
    {
        public ushort handle;
        public SkillDamageFlags flag;
        public uint hpchange;
        public uint resthp;
        public ushort hpchangeorder;

        public SkillDamage(FiestaNetPacket packet)
        {
            handle = packet.ReadUInt16();
            flag = new SkillDamageFlags(packet.ReadUInt16());
            hpchange = packet.ReadUInt32();
            resthp = packet.ReadUInt32();
            hpchangeorder = packet.ReadUInt16();
        }

        public void WriteToPacket(FiestaNetPacket packet)
        {
            if (packet == null) return;
            packet.Write(handle);
            packet.Write(flag.ToUShort());
            packet.Write(hpchange);
            packet.Write(resthp);
            packet.Write(hpchangeorder);
        }

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
                var bs = new BitStream();
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
                bs.Dispose();
            }

            public ushort ToUShort()
            {
                var bs = new BitStream();
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
                bs.Dispose();
                return flag;
            }
        }
    }
}

