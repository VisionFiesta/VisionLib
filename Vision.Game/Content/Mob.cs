using Vision.Game.Content.GameObjects;
using Vision.Game.Structs.BriefInfo;

namespace Vision.Game.Content
{
    public class Mob : GameObject
    {
        public ushort MobID;

        public Mob(NcBriefInfoRegenMobCmd data)
        {
            Handle = data.Handle;
            Position = data.Position;
            MobID = data.MobID;
        }
    }
}
