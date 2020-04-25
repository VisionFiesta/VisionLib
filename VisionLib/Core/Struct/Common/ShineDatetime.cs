using System;
using VisionLib.Common.Utils;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class ShineDatetime : AbstractStruct
    {
        // struct {
        // 	  unsigned __int32 year : 8;
        // 	  unsigned __int32 month : 5;
        // 	  unsigned __int32 date : 6;
        // 	  unsigned __int32 hour : 6;
        // 	  unsigned __int32 minute : 7;
        // };

        public int Bitfield0;

        public ShineDatetime()
        {
            Bitfield0 = ToBitField(DateTime.MinValue);
        }

        public DateTime ToDateTime() => ToDateTime(Bitfield0);

        public static DateTime ToDateTime(int bf0)
        {
            using (var bs = new BitStream())
            {
                bs.Write(bf0);

                bs.Read(out int year, 0, 8);
                bs.Read(out int month, 0, 5);
                bs.Read(out int date, 0, 6);
                bs.Read(out int hour, 0, 6);
                bs.Read(out int minute, 0, 7);
                
                return new DateTime(year, month, date, hour, minute, 0);
            }
        }

        public static int ToBitField(DateTime dt)
        {
            using (var bs = new BitStream(sizeof(int)))
            {
                bs.Write(dt.Year, 0, 8);
                bs.Write(dt.Month, 0, 5);
                bs.Write(dt.Day, 0, 6);
                bs.Write(dt.Hour, 0, 6);
                bs.Write(dt.Minute, 0, 7);

                bs.Read(out int bf0);

                return bf0;
            }
        }

        public override int GetSize()
        {
            return sizeof(int);
        }

        public override void Read(ReaderStream reader)
        {
            Bitfield0 = reader.ReadInt32();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Bitfield0);
        }
    }
}
