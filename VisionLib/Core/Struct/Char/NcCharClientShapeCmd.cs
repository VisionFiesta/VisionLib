using VisionLib.Common.Game.Characters.Shape;
using VisionLib.Common.Networking;
using VisionLib.Core.Stream;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Core.Struct.Char
{
    public class NcCharClientShapeCmd : NetPacketStruct
    {
        public CharacterShape Shape;

        public override int GetSize()
        {
            return 4;
        }

        public override void Read(ReaderStream reader)
        {
            var raw = new ProtoAvatarShapeInfo();
            raw.Read(reader);
            Shape = new CharacterShape(raw);
        }

        public override void Write(WriterStream writer)
        {
            var raw = new ProtoAvatarShapeInfo()
            {
                JobGender = Shape.GetJobGenderByte(),
                Hair = Shape.Hair.OrigData,
                HairColor = Shape.HairColor.OrigData,
                Face = Shape.Face.OrigData
            };
            raw.Write(writer);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_CHAR_CLIENT_SHAPE_CMD;
        }
    }
}
