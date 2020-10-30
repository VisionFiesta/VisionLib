using Vision.Game.Content.GameObjects;

namespace Vision.Game.Content
{
    public class Mover : GameObject
    {
        public uint ID { get; }

        public Mover(ushort handle, uint id) : base(handle, GameObjectType.GOT_MOVER)
        {
            ID = id;
        }

        public override string ToString() => $"Mover - {nameof(ID)}: {ID}, Handle: {Handle}";
    }
}
