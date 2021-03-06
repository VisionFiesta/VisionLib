﻿using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Core.Utils;

namespace Vision.Game.Structs.Common
{
    public class PlayerQuestData : AbstractStruct
    {
        public const int Size = 27;

        public long StartTime;
        public long EndTime;
        public uint RepeatCount;
        public byte ProgressStep;
        public byte[] EndNPCMobCount { get; } = new byte[5];
        public byte EndLocationScenario { get; private set; } // bitpacked, 1 ea
        public ushort EndRunningTimeSec;

        // packed/unpacked from EndLocationScenario
        public byte EndLocation;
        public byte EndScenario;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            StartTime = reader.ReadInt64();
            EndTime = reader.ReadInt64();
            RepeatCount = reader.ReadUInt32();
            ProgressStep = reader.ReadByte();
            //EndNPCMobCount = reader.ReadBytes(5);

            EndLocationScenario = reader.ReadByte();
            using (var bs = new BitStream())
            {
                bs.Write(EndLocationScenario);
                bs.Read(out EndLocation, 0, 1);
                bs.Read(out EndScenario, 0, 1);
            }

            EndRunningTimeSec = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(StartTime);
            writer.Write(EndTime);
            writer.Write(RepeatCount);
            writer.Write(ProgressStep);
            writer.Write(EndNPCMobCount, 5);

            using (var bs = new BitStream())
            {
                bs.Write(EndLocation, 0, 1);
                bs.Write(EndScenario, 0, 1);
                bs.Read(out byte endLocationScenario);
                writer.Write(endLocationScenario);
            }

            writer.Write(EndRunningTimeSec);
        }
    }
}
