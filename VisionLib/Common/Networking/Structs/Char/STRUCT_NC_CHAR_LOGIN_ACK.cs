using System;
using System.Net;

using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.Char
{
    public class STRUCT_NC_CHAR_LOGIN_ACK : FiestaNetPacketStruct
    {
        public const int ZoneIPLen = 16;

        public readonly string ZoneIP;
        public readonly ushort ZonePort;

        public readonly IPEndPoint ZoneEndPoint;

        public STRUCT_NC_CHAR_LOGIN_ACK(string zoneIp, ushort zonePort)
        {
            ZoneIP = zoneIp;
            ZonePort = zonePort;

            if (IPAddress.TryParse(ZoneIP, out var trueZoneIP))
            {
                ZoneEndPoint = new IPEndPoint(trueZoneIP, ZonePort);
            }
            else
            {
                throw new Exception("ZoneIP failed to parse as a valid IP Address");
            }
        }

        public STRUCT_NC_CHAR_LOGIN_ACK(FiestaNetPacket packet)
        {
            ZoneIP = packet.ReadString(ZoneIPLen);
            ZonePort = packet.ReadUInt16();
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_CHAR_LOGIN_ACK);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(ZoneIP, ZoneIPLen);
            pkt.Write(ZonePort);
        }
    }
}
