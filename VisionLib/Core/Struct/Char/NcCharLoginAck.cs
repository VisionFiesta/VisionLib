using System;
using System.Net;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Char
{
    public class NcCharLoginAck : NetPacketStruct
    {
        public const int ZoneIPLen = 16;

        public string ZoneIP { get; private set; }
        public ushort ZonePort { get; private set; }

        public IPEndPoint ZoneEndPoint { get; private set; }

        public NcCharLoginAck() { }

        public NcCharLoginAck(string zoneIp, ushort zonePort)
        {
            ZoneIP = zoneIp;
            ZonePort = zonePort;

            SetEndpoint();
        }

        public NcCharLoginAck(IPEndPoint zoneEndPoint)
        {
            ZoneIP = zoneEndPoint.Address.ToString();
            ZonePort = (ushort) zoneEndPoint.Port;

            ZoneEndPoint = zoneEndPoint;
        }

        private void SetEndpoint()
        {
            if (IPAddress.TryParse(ZoneIP, out var trueZoneIP))
            {
                ZoneEndPoint = new IPEndPoint(trueZoneIP, ZonePort);
            }
            else
            {
                throw new Exception("ZoneIP failed to parse as a valid IP Address");
            }
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_CHAR_LOGIN_ACK);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return ZoneIPLen + sizeof(ushort);
        }

        public override void Read(ReaderStream reader)
        {
            ZoneIP = reader.ReadString(ZoneIPLen);
            ZonePort = reader.ReadUInt16();

            SetEndpoint();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ZoneIP, ZoneIPLen);
            writer.Write(ZonePort);
        }
    }
}
