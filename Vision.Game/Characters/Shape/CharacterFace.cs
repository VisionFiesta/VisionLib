using System.Collections.Generic;
using Vision.Core.Common.Collections;
using Vision.Core.Common.Extensions;
using Vision.Core.Common.IO.SHN;
using Vision.Game.Enums;

namespace Vision.Game.Characters.Shape
{
    public class CharacterFace
    {
        public static readonly Dictionary<byte, CharacterFace> AllFacesByID = new FastDictionary<byte, CharacterFace>();

        public byte ID;
        public string Name;
        public BodyShopGrade Grade;

        public CharacterFace(SHNResult shnResult, int index)
        {
            ID = shnResult.Read<byte>(index, "ID");
            Name = shnResult.Read<string>(index, "FaceName");
            var grade = shnResult.Read<byte>(index, "Grade");
            Grade = EnumExtensions.GetValueOrDefault(grade, BodyShopGrade.None);
        }

        public static bool GetFaceByID(byte id, out CharacterFace face) => AllFacesByID.TryGetValue(id, out face);
    }
}
