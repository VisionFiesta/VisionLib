using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Map
{
    public class NcMapLoginAck : NetPacketStruct
    {

        public ushort Handle;
        // public CharParameterData ParameterData = new CharParameterData();
        public ShineXY LoginPosition = new ShineXY();

        public override int GetSize() => 2 + 232 + ShineXY.Size;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();

            // ParameterData.Read(reader);
            reader.ReadBytes(232);
            LoginPosition.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);

            writer.Fill(232, 0);
            LoginPosition.Write(writer);
        }

        public override NetCommand GetCommand() => NetCommand.NC_MAP_LOGIN_ACK;

        public override string ToString()
        {
            return $"{nameof(Handle)}: {Handle}";
        }
    }
}
