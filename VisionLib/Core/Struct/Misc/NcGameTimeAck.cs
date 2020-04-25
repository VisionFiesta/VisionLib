using System;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Misc
{
    public class NcGameTimeAck : NetPacketStruct
    {
        public byte Hour { get; private set; }
        public byte Minute { get; private set; }
        public byte Second { get; private set; }

        public DateTime GameTime { get; private set; }

        public NcGameTimeAck() { }

        public NcGameTimeAck(DateTime gameTime)
        {
            Hour = (byte)gameTime.Hour;
            Minute = (byte)gameTime.Minute;
            Second = (byte)gameTime.Second;

            GameTime = gameTime;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_MISC_GAMETIME_ACK);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return sizeof(byte) * 3;
        }

        public override void Read(ReaderStream reader)
        {
            Hour = reader.ReadByte();
            Minute = reader.ReadByte();
            Second = reader.ReadByte();

            GameTime = ToDateTime(this);
        }

        public override void Write(WriterStream writer)
        {
            if (writer == null) return;
            writer.Write(Hour);
            writer.Write(Minute);
            writer.Write(Second);
        }

        public static DateTime ToDateTime(NcGameTimeAck gameTime)
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, gameTime.Hour, gameTime.Minute, gameTime.Second);
        }
    }
}
