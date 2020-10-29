using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Core.Utils;

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

        public bool Login;
        public uint Year;
        public uint Month;
        public uint Day;
        public uint Hour;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            using var bs = new BitStream();
            bs.Write(reader.ReadUInt32());

            bs.Read(out uint login, 0, 1);
            Login = login == 1;

            bs.Read(out Year, 0, 11);
            bs.Read(out Month, 0, 4);
            bs.Read(out Day, 0, 5);
            bs.Read(out Hour, 0, 5);
        }

        public override void Write(WriterStream writer)
        {
            using var bs = new BitStream();
            bs.Write(Login ? 1 : 0, 0, 1);
            bs.Write(Year, 0, 11);
            bs.Write(Month, 0, 4);
            bs.Write(Day, 0, 5);
            bs.Write(Hour, 0, 5);

            bs.Read(out uint bf0);
            writer.Write(bf0);
        }

        public override string ToString() => $"{(Login ? "Online" : "Offline")} - Last seen date: {Hour}:00, {Day}/{Month}/{Year}";
    }
}
