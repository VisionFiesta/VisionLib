using System;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.Misc
{
    public class STRUCT_GAMETIME_ACK : FiestaNetPacketStruct
    {
        public readonly byte Hour;
        public readonly byte Minute;
        public readonly byte Second;

        public readonly DateTime GameTime;

        public STRUCT_GAMETIME_ACK(DateTime gameTime)
        {
            Hour = (byte)gameTime.Hour;
            Minute = (byte)gameTime.Minute;
            Second = (byte)gameTime.Second;

            GameTime = gameTime;
        }

        public STRUCT_GAMETIME_ACK(FiestaNetPacket packet)
        {
            Hour = packet.ReadByte();
            Minute = packet.ReadByte();
            Second = packet.ReadByte();

            GameTime = ToDateTime(this);
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_MISC_GAMETIME_ACK);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(Hour);
            pkt.Write(Minute);
            pkt.Write(Second);
        }

        public static DateTime ToDateTime(STRUCT_GAMETIME_ACK gametime)
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, gametime.Hour, gametime.Minute, gametime.Second);
        }
    }
}
