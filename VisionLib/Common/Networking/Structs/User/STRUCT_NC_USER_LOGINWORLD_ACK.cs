using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Networking.Structs.Common;

namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_LOGINWORLD_ACK : FiestaNetPacketStruct
    {
        public readonly ushort WorldManagerHandle;
        public byte AvatarCount;
        public ProtoAvatarInformation[] Avatars;

        public STRUCT_NC_USER_LOGINWORLD_ACK(ushort worldManagerHandle, byte avatarCount,
            ProtoAvatarInformation[] avatars)
        {
            WorldManagerHandle = worldManagerHandle;
            AvatarCount = avatarCount;
            Avatars = avatars;
        }

        public STRUCT_NC_USER_LOGINWORLD_ACK(FiestaNetPacket packet)
        {
            WorldManagerHandle = packet.ReadUInt16();
            AvatarCount = packet.ReadByte();

            if (AvatarCount <= 0) return;
            Avatars = new ProtoAvatarInformation[AvatarCount];
            for (var i = 0; i < AvatarCount; i++)
            {
                Avatars[i] = new ProtoAvatarInformation(packet);
            }
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_LOGINWORLD_ACK);
           WriteToPacket(pkt);
            // TODO: ToPacket for remaining struct parts
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(WorldManagerHandle);
            pkt.Write(AvatarCount);
        }
    }
}
