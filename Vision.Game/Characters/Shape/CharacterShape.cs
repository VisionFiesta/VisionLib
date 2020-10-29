using Vision.Core.Utils;
using Vision.Game.Structs.Common;

namespace Vision.Game.Characters.Shape
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
            CharacterFace.GetFaceByID(rawInfo.Face, out Face);
        }

        public byte GetJobGenderByte()
        {
            using var bs = new BitStream();
            bs.Write((byte)Class, 0, 7);
            bs.Write((byte)Gender, 0, 1);

            return (byte) bs.ReadByte();
        }

        public override string ToString() => $"Shape - Class:{Class}, Gender: {Gender}, Hair: {Hair}, HairColor: {HairColor}";
    }
}
