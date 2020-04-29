using VisionLib.Common.Game.Content.GameObjects;
using VisionLib.Core.Struct.BriefInfo;

namespace VisionLib.Common.Game.Content
{
    public class Mob : GameObject
    {
        public int MobID;

        public Mob(NcBriefInfoRegenMobCmd data)
        {
            Handle = data.Handle;
            Position = data.Position;
            MobID = data.MobID;
        }

        public string Name() => $"Mob: {MobID}";
    }
}
