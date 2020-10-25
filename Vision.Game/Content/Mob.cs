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

            var hasInfo = MobInfo.GetMobInfoById(MobID, out var mobInfo);
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
            string info = $"{Type.ToFriendlyName()} - ";
            switch (Type)
            {
                case GameObjectType.GOT_DOOR:
                    info += $"To: {Name}, Handle: {Handle}";
                    break;
                case GameObjectType.GOT_NPC:
                    info = $"Name: {Name}";
                    break;
                case GameObjectType.GOT_MOB:
                    info = $"Name: {Name}, Level: {Level}";
                    break;
            }

            info += $", Handle: {Handle}";

            return info;
        }
    }
}
