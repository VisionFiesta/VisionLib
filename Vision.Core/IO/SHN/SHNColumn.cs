using System;

namespace Vision.Core.IO.SHN
{
    public class SHNColumn
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public uint Type { get; set; }
        public int Length { get; set; }

        public new Type GetType()
        {
            switch (Type)
            {
                default:
                    { return typeof(object); }
                case 1:
                case 12:
                    { return typeof(byte); }
                case 2:
                    { return typeof(ushort); }
                case 3:
                case 11:
                    { return typeof(uint); }
                case 0x15:
                case 13:
                    { return typeof(short); }
                case 0x10:
                    { return typeof(byte); }
                case 0x12:
                case 0x1b:
                    { return typeof(uint); }
                case 20:
                    { return typeof(sbyte); }
                case 0x16:
                    { return typeof(int); }
                case 5:
                    { return typeof(float); }
                case 0x18:
                case 0x1a:
                case 9:
                    { return typeof(string); }
                case 0x1d:
                    { return typeof(string); }
            }
        }
    }
}
