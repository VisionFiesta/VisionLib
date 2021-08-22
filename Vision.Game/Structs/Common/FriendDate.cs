using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Common
{
    public class FriendDate : AbstractStruct
    {
        public const int Size = 4;

        /*
           struct PROTO_FRIEND_DATE
           {
           unsigned __int32 login : 1;
           unsigned __int32 year : 11;
           unsigned __int32 month : 4;
           unsigned __int32 day : 5;
           unsigned __int32 hour : 5;
           };
         */

        public bool Online;
        public uint Year;
        public uint Month;
        public uint Day;
        public uint Hour;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            var bitfieldData = reader.ReadUInt32();
            Online = ((bitfieldData >> 0) & 0x0001) == 1;
            Year = (bitfieldData >> 1) & 0x07FF;
            Month = (bitfieldData >> 12) & 0x000F;
            Day = (bitfieldData >> 16) & 0x001F;
            Hour = (bitfieldData >> 21) & 0x001F;
        }

        public override void Write(WriterStream writer)
        {
            uint bitfieldData = 0;

            bitfieldData |= ((Online ? 1u : 0u) & 0x1) << 0;
            bitfieldData |= (Year & 0x7FF) << 1;
            bitfieldData |= (Month & 0xF) << 12;
            bitfieldData |= (Day & 0x1F) << 16;
            bitfieldData |= (Hour & 0x1F) << 16;

            writer.Write(bitfieldData);
        }

        public override string ToString() => Online ? "Online" : $"Offline - Last seen date: {Hour}:00, {Month}/{Day}/{Year}";
    }
}