using System;
using System.Net;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Char
{
    public class NcCharLoginAck : NetPacketStruct
    {
        private const int ZoneIPLen = 16;

        public const int Size = ZoneIPLen + 2;

        public string ZoneIP { get; private set; }
        public ushort ZonePort { get; private set; }

        public IPEndPoint ZoneEndPoint { get; private set; }

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

        public override int GetSize() => Size;

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

        public override NetCommand GetCommand() => NetCommand.NC_CHAR_LOGIN_ACK;
    }
}
