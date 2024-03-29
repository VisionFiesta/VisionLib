﻿using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoMobCmd : NetPacketStruct
    {
        public byte MobCount;
        public NcBriefInfoRegenMobCmd[] Mobs;

        public override int GetSize() => 1 + MobCount * 167;

        public override void Read(ReaderStream reader)
        {
            MobCount = reader.ReadByte();

            Mobs = new NcBriefInfoRegenMobCmd[MobCount];

            for (var i = 0; i < Mobs.Length; i++)
            {
                Mobs[i] = new NcBriefInfoRegenMobCmd();
                Mobs[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(MobCount);

            foreach (var mob in Mobs)
            {
                mob.Write(writer);
            }
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_MOB_CMD;

        protected override bool HasMaximumSize() => false;
    }
}
