using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Enums;

namespace Vision.Game.Structs.User
{
    public class NcUserWorldSelectAck : NetPacketStruct
    {
        public const int WorldIPv4Len = 16;
        public const int ConnectionHashLen = 64;

        public WorldServerStatus WorldStatus { get; private set; }
        public string WorldIPv4 { get; private set; }
        public ushort WorldPort { get; private set; }
        public byte[] ConnectionHash { get; private set; }

        public NcUserWorldSelectAck() { }

        public NcUserWorldSelectAck(byte worldStatus, string worldIPv4, ushort worldPort, byte[] connectionHash)
        {
            WorldStatus = (WorldServerStatus) worldStatus;
            WorldIPv4 = worldIPv4;
            WorldPort = worldPort;
            ConnectionHash = connectionHash;
        }

        public override string ToString() => $"\tStatus: {WorldStatus.ToMessage()}, IP: {WorldIPv4}, Port: {WorldPort}";

        public override int GetSize()
        {
            return sizeof(ushort) + sizeof(byte) + WorldIPv4Len + ConnectionHashLen;
        }

        public override void Read(ReaderStream reader)
        {
            WorldStatus = (WorldServerStatus)reader.ReadByte();
            WorldIPv4 = reader.ReadString(WorldIPv4Len);
            WorldPort = reader.ReadUInt16();
            ConnectionHash = reader.ReadBytes(ConnectionHashLen);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write((byte)WorldStatus);
            writer.Write(WorldIPv4, WorldIPv4Len);
            writer.Write(WorldPort);
            writer.Write(ConnectionHash, ConnectionHashLen);
        }

        protected override NetCommand GetCommand()
        {
            return NetCommand.NC_USER_WORLDSELECT_ACK;
        }
    }
}
