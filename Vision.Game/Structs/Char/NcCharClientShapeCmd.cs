using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Characters.Shape;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Char
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
                Face = Shape.Face.ID
            };
            raw.Write(writer);
        }

        public override NetCommand GetCommand()
        {
            return NetCommand.NC_CHAR_CLIENT_SHAPE_CMD;
        }
    }
}
