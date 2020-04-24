using System;
using VisionLib.Common.Enums;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_WORLD_STATUS_ACK : FiestaNetPacketStruct
    {
        public readonly byte WorldCount;
        public readonly WORLD_STATUS[] WorldStatuses;

        public STRUCT_NC_USER_WORLD_STATUS_ACK(byte worldCount, WORLD_STATUS[] worldStatuses)
        {
            WorldCount = worldCount;
            WorldStatuses = worldStatuses;
        }

        public STRUCT_NC_USER_WORLD_STATUS_ACK(FiestaNetPacket packet)
        {
            if (packet.Command != FiestaNetCommand.NC_USER_WORLD_STATUS_ACK)
            {
                throw new InvalidOperationException("Wrong command for struct!");
            } 

            WorldCount = packet.ReadByte();
            WorldStatuses = new WORLD_STATUS[WorldCount];
            for (int i = 0; i < WorldCount; i++)
            {
                var worldId = packet.ReadByte();
                var worldName = packet.ReadString(16);
                var worldStatus = packet.ReadByte();
                WorldStatuses[i] = new WORLD_STATUS(worldId, worldName, worldStatus);
            }
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_WORLD_STATUS_ACK);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(WorldCount);
            foreach (var ws in WorldStatuses)
            {
                ws.WriteToPacket(pkt);
            }
        }

        public class WORLD_STATUS
        {
            public readonly byte WorldID;
            public readonly string WorldName;
            public readonly WorldServerStatus WorldStatus;

            internal WORLD_STATUS(byte worldId, string worldName, byte worldStatus)
            {
                WorldID = worldId;
                WorldName = worldName;
                WorldStatus = (WorldServerStatus)worldStatus;
            }

            public void WriteToPacket(FiestaNetPacket pkt)
            {
                if (pkt == null) return;
                pkt.Write(WorldID);
                pkt.Write(WorldName, 16);
                pkt.Write((byte)WorldStatus);
            }
        }

        public override string ToString()
        {
            var str = $"Count: {WorldCount}";
            if (WorldCount > 0)
            {
                foreach(var w in WorldStatuses)
                {
                    str += "\n    ";
                    str += $"ID: {w.WorldID}, Name: {w.WorldName}, Status: {w.WorldStatus}";
                }
            }
            return str;
        }
    }
}
