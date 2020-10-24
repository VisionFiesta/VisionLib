using Vision.Game.Content.GameObjects;
using Vision.Game.Enums;
using Vision.Game.Structs.BriefInfo;

namespace Vision.Game.Content
{
    public class Mob : GameObject
    {
        public ushort MobID;
        public string Name;

        public Mob(NcBriefInfoRegenMobCmd data)
        {
            Handle = data.Handle;
            Position = data.Position;
            MobID = data.MobID;

            bool hasInfo = MobInfo.GetMobInfoById(MobID, out var mobInfo);
            if (hasInfo)
            {
                var isGate = data.Flags.Flag == MobBriefFlag.MBF_GATE;

                Name = isGate ? data.Flags.GateToWhere : mobInfo.Name;
                Level = mobInfo.Level;
                Type = data.Flags.Flag == MobBriefFlag.MBF_GATE ? GameObjectType.GOT_DOOR : mobInfo.IsNPC ? GameObjectType.GOT_NPC : GameObjectType.GOT_MOB;
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case GameObjectType.GOT_DOOR: return $"{Type.ToFriendlyName()} - To: {Name}, Handle: {Handle}";
                case GameObjectType.GOT_NPC: return $"{Type.ToFriendlyName()} - Name: {Name}, Handle: {Handle}";
                case GameObjectType.GOT_MOB: return $"{Type.ToFriendlyName()} - Name: {Name}, Level: {Level}, Handle: {Handle}";
                default: return $"{Type.ToFriendlyName()} - Handle: {Handle}";
            }
        }
    }
}
