using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoMinihouseBuildCmd : NetPacketStruct
    {
        public const int Size = 2 + CharBriefInfoCamp.Size + NameN.Name5Len + ShineXYR.Size + 21;

        public ushort Handle;
        public CharBriefInfoCamp Camp = new();
        public string CharID;
        public ShineXYR Coordinates = new();
        public byte[] Title; // 21

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();

            Camp.Read(reader);

            CharID = reader.ReadString(NameN.Name5Len);

            Coordinates.Read(reader);

            Title = reader.ReadBytes(21);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);

            Camp.Write(writer);

            writer.Write(CharID);

            Coordinates.Write(writer);

            writer.Write(Title, 21);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_MINIHOUSEBUILD_CMD;

        public override string ToString() => $"{nameof(Handle)}: {Handle}, {nameof(Camp)}: {Camp}, {nameof(CharID)}: {CharID}, {nameof(Coordinates)}: {Coordinates}, {nameof(Title)}: {Title}";
    }
}
