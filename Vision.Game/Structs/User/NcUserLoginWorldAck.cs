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
        public byte CharacterCount { get; private set; }
        public ProtoAvatarInformation[] Avatars { get; private set; }

        public NcUserLoginWorldAck() { }

        public NcUserLoginWorldAck(ushort worldManagerHandle, byte characterCount,
            ProtoAvatarInformation[] avatars)
        {
            AccountID = worldManagerHandle;
            CharacterCount = characterCount;
            Avatars = avatars;
        }

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            AccountID = reader.ReadUInt16();
            CharacterCount = reader.ReadByte();

            if (CharacterCount <= 0) return;
            Avatars = new ProtoAvatarInformation[CharacterCount];
            for (var i = 0; i < CharacterCount; i++)
            {
                Avatars[i] = new ProtoAvatarInformation();
                Avatars[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(AccountID);
            writer.Write(CharacterCount);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_USER_LOGINWORLD_ACK;

        protected override bool HasMaximumSize() => false;
    }
}
