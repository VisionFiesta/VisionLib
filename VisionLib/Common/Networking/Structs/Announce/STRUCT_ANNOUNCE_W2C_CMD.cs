using System;
using System.Collections.Generic;
using System.Text;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.Announce
{
    public class STRUCT_ANNOUNCE_W2C_CMD : FiestaNetPacketStruct
    {
        public readonly ANNOUNCE_TYPE AnnounceType;
        public readonly byte MessageLength;
        public readonly string Message;

        public STRUCT_ANNOUNCE_W2C_CMD(FiestaNetPacket packet)
        {
            AnnounceType = (ANNOUNCE_TYPE) packet.ReadByte();
            MessageLength = packet.ReadByte();
            Message = packet.ReadString(MessageLength);
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_ANNOUNCE_W2C_CMD);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            pkt.Write((byte)AnnounceType);
            pkt.Write(MessageLength);
            pkt.Write(Message, MessageLength);
        }
    }


    public enum ANNOUNCE_TYPE : byte
    {
        AT_LV20 = 4,
        AT_PROMOTE = 5,
        AT_TITLEACQUIRE = 6,
        AT_ROAR = 11,
    }
}
