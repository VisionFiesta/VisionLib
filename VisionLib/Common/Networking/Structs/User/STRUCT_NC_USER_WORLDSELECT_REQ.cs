using VisionLib.Common.Enums;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_WORLDSELECT_REQ : FiestaNetPacketStruct
    {
        public readonly byte WorldID;

        public STRUCT_NC_USER_WORLDSELECT_REQ(byte worldID)
        {
            WorldID = worldID;
        }

        public STRUCT_NC_USER_WORLDSELECT_REQ(FiestaNetPacket packet)
        {
            WorldID = packet.ReadByte();
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_WORLDSELECT_REQ);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            pkt?.Write(WorldID);
        }
    }
}
