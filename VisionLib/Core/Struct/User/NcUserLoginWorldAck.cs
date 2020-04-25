using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Core.Struct.User
{
    public class NcUserLoginWorldAck : NetPacketStruct
    {
        private static readonly int Size = sizeof(ushort) + sizeof(byte) + new ProtoAvatarInformation().GetSize();

        public ushort WorldManagerHandle { get; private set; }
        public byte AvatarCount { get; private set; }
        public ProtoAvatarInformation[] Avatars { get; private set; }

        public NcUserLoginWorldAck() { }

        public NcUserLoginWorldAck(ushort worldManagerHandle, byte avatarCount,
            ProtoAvatarInformation[] avatars)
        {
            WorldManagerHandle = worldManagerHandle;
            AvatarCount = avatarCount;
            Avatars = avatars;
        }

        public override int GetSize()
        {
            return Size;
        }

        public override void Read(ReaderStream reader)
        {
            WorldManagerHandle = reader.ReadUInt16();
            AvatarCount = reader.ReadByte();

            if (AvatarCount <= 0) return;
            Avatars = new ProtoAvatarInformation[AvatarCount];
            for (var i = 0; i < AvatarCount; i++)
            {
                Avatars[i] = new ProtoAvatarInformation();
                Avatars[i].Read(reader);
            }
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_LOGINWORLD_ACK);
            Write(pkt.Writer);
            // TODO: ToPacket for remaining struct parts
            return pkt;
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(WorldManagerHandle);
            writer.Write(AvatarCount);
        }
    }
}
