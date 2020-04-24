using VisionLib.Common.Networking.Structs.Common;

namespace VisionLib.Common.Game.Characters.Shape
{
    public class CharacterShape
    {
        public CharacterClass Class;
        public CharacterGender Gender;
        public CharacterHair Hair;
        public CharacterHairColor HairColor;
        public CharacterFace Face;

        public CharacterShape() { }

        public CharacterShape(ProtoAvatarShapeInfo rawInfo)
        {
            Class = rawInfo.Job;
            Gender = rawInfo.Gender;
            Hair = new CharacterHair(rawInfo.Hair);
            HairColor = new CharacterHairColor(rawInfo.HairColor);
            Face = new CharacterFace(rawInfo.Face);
        }
    }
}
