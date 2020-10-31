using Vision.Game.Content.GameObjects;
using Vision.Game.Enums;
using Vision.Game.Structs.BriefInfo;

#pragma warning disable 8509

namespace Vision.Game.Content
{
    public class Mob : GameObject
    {
        public ushort MobID;
        public new string Name;

        public Mob(NcBriefInfoRegenMobCmd data) : base(data.Handle, GameObjectType.GOT_MOB)
        {
            Position = data.Position;
            MobID = data.MobID;

            var hasInfo = MobInfo.GetMobInfoById(MobID, out var mobInfo);
            if (!hasInfo) return;

            var isGate = data.Flags.Flag == MobBriefFlag.MBF_GATE;

            Name = isGate ? data.Flags.GateToWhere : mobInfo.Name;
            Level = mobInfo.Level;
            Type = data.Flags.Flag == MobBriefFlag.MBF_GATE ? GameObjectType.GOT_DOOR : mobInfo.IsNPC ? GameObjectType.GOT_NPC : GameObjectType.GOT_MOB;
        }

        public override string ToString()
        {
            var info = $"{Type.ToFriendlyName()} - ";

            info += Type switch
            {
                GameObjectType.GOT_DOOR => $"To: {Name}, Handle: {Handle}",
                GameObjectType.GOT_NPC => $"Name: {Name}",
                GameObjectType.GOT_MOB => $"Name: {Name}, Level: {Level}"
            };

            info += $", Handle: {Handle}";

            return info;
        }
    }
}
