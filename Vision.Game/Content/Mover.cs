using Vision.Game.Content.GameObjects;

namespace Vision.Game.Content
{
    public class Mover : GameObject
    {
        public uint ID { get; }

        public Mover(ushort handle, uint id)
        {
            Handle = handle;
            ID = id;
        }

    }
}
