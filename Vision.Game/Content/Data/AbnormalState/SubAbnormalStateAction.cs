namespace Vision.Game.Content.Data.AbnormalState
{
    public sealed class SubAbnormalStateAction
    {
        public SubAbnormalStateActionType Type { get; }
        public uint Value { get; }

        public SubAbnormalStateAction(uint type, uint value)
        {
            Type = (SubAbnormalStateActionType)type;
            Value = value;
        }
    }
}
