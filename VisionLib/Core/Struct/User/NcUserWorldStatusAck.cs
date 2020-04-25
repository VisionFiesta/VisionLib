using VisionLib.Common;
using VisionLib.Common.Enums;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.User
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
        
        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_WORLD_STATUS_ACK);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return sizeof(byte) * 3 + NameN.Name4Len;
        }

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

        public override string ToString()
        {
            if (WorldCount <= 0) return "No Worlds";

            var str = $"Count: {WorldCount}";
            foreach (var w in WorldStatuses)
            {
                str += "\n    ";
                str += $"ID: {w.WorldID}, Name: {w.WorldName}, Status: {w.WorldStatus}";
            }
            return str;
        }

        public class WorldStatusStruct : AbstractStruct
        {
            public byte WorldID { get; private set; }
            public string WorldName { get; private set; }
            public WorldServerStatus WorldStatus { get; private set; }

            internal WorldStatusStruct() { }

            internal WorldStatusStruct(byte worldId, string worldName, byte worldStatus)
            {
                WorldID = worldId;
                WorldName = worldName;
                WorldStatus = (WorldServerStatus)worldStatus;
            }

            public override int GetSize()
            {
                return sizeof(byte) * 2 + NameN.Name4Len;
            }

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
