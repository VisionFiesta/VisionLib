using VisionLib.Common.Game.Content.GameObjects;

namespace VisionLib.Common.Game.Characters
{
    public class Character : GameObject
    {
        public static CharacterCommon Global { get; set; }

        public int UserNo { get; set; }
        public byte AdminLevel { get; set; }
        public ushort PrisonMinutes { get; set; }

        public long EXP { get; set; }
        public long Cen { get; set; }
        public long UserCen { get; set; }
        public int Fame { get; set; }

        public int Flags { get; set; }
        public byte PKYellowTime { get; set; }

        public byte StatPoints { get; set; }
        public int KillPoints { get; set; }

        public CharacterParameters Parameters { get; set; }

        public long NextMHHPTick { get; set; }
        public long NextMHSPTick { get; set; }

        public long LastLPUpdate { get; set; }

        public bool IsLoggingOut { get; set; }
        public bool IsLoggedOut { get; set; }
        public bool IsTrading { get; set; }

        public Character(int charNo)
        {
            Type = GameObjectType.GOT_CHARACTER;
        }
    }
}
