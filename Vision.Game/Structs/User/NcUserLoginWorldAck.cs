using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.User
{
    public class NcUserLoginWorldAck : NetPacketStruct
    {
        private static readonly int Size = sizeof(ushort) + sizeof(byte) + new ProtoAvatarInformation().GetSize();

        public ushort AccountID { get; private set; }
        public byte AvatarCount { get; private set; }
        public ProtoAvatarInformation[] Avatars { get; private set; }

        public NcUserLoginWorldAck() { }

        public NcUserLoginWorldAck(ushort worldManagerHandle, byte avatarCount,
            ProtoAvatarInformation[] avatars)
        {
            AccountID = worldManagerHandle;
            AvatarCount = avatarCount;
            Avatars = avatars;
        }

        public override int GetSize()
        {
            return Size;
        }

        public override void Read(ReaderStream reader)
        {

            AccountID = reader.ReadUInt16();
            AvatarCount = reader.ReadByte();

            if (AvatarCount <= 0) return;
            Avatars = new ProtoAvatarInformation[AvatarCount];
            for (var i = 0; i < AvatarCount; i++)
            {
                Avatars[i] = new ProtoAvatarInformation();
                Avatars[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(AccountID);
            writer.Write(AvatarCount);
        }

        public override NetCommand GetCommand()
        {
            return NetCommand.NC_USER_LOGINWORLD_ACK;
        }
    }
}
