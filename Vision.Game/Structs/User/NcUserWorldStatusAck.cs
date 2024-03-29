﻿using System.Linq;
using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Enums;

namespace Vision.Game.Structs.User
{
    public class NcUserWorldStatusAck : NetPacketStruct
    {
        public byte WorldCount { get; private set; }
        public WorldStatusStruct[] WorldStatuses { get; private set; }

        public NcUserWorldStatusAck() { }

        public NcUserWorldStatusAck(byte worldCount, WorldStatusStruct[] worldStatuses)
        {
            WorldCount = worldCount;
            WorldStatuses = worldStatuses;
        }

        public override int GetSize() => WorldCount + 3 + NameN.Name4Len;

        public override void Read(ReaderStream reader)
        {
            WorldCount = reader.ReadByte();
            WorldStatuses = new WorldStatusStruct[WorldCount];
            for (var i = 0; i < WorldCount; i++)
            {
                WorldStatuses[i] = new WorldStatusStruct();
                WorldStatuses[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(WorldCount);
            WorldStatuses = new WorldStatusStruct[WorldCount];
            foreach (var ws in WorldStatuses)
            {
                ws.Write(writer);
            }
        }

        protected override NetCommand GetCommand() => NetCommand.NC_USER_WORLD_STATUS_ACK;

        protected override bool HasMaximumSize() => false;

        public override string ToString()
        {
            if (WorldCount <= 0) return "No Worlds";

            var str = $"Count: {WorldCount}";
            return WorldStatuses.Aggregate(str, (current, w) => current + $"\tID: {w.WorldID}, Name: {w.WorldName}, Status: {w.WorldStatus}");
        }

        public class WorldStatusStruct : AbstractStruct
        {
            public byte WorldID { get; private set; }
            public string WorldName { get; private set; }
            public WorldServerStatus WorldStatus { get; private set; }

            public WorldStatusStruct() { }

            public WorldStatusStruct(byte worldId, string worldName, byte worldStatus)
            {
                WorldID = worldId;
                WorldName = worldName;
                WorldStatus = (WorldServerStatus)worldStatus;
            }

            public override int GetSize() => 2 + NameN.Name4Len;

            public override void Read(ReaderStream reader)
            {
                WorldID = reader.ReadByte();
                WorldName = reader.ReadString(NameN.Name4Len);
                WorldStatus = (WorldServerStatus) reader.ReadByte();
            }

            public override void Write(WriterStream writer)
            {
                writer.Write(WorldID);
                writer.Write(WorldName, NameN.Name4Len);
                writer.Write((byte)WorldStatus);
            }
        }
    }
}
