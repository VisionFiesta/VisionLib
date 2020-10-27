using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Characters;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Map
{
    public class NcMapLoginAck : NetPacketStruct
    {

        public ushort Handle;
        public CharParameterData ParameterData = new CharParameterData();
        public ShineXY LoginPosition = new ShineXY();

        public override int GetSize() => 2 + CharParameter.Size + ShineXY.Size;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();

            ParameterData.Read(reader);
            LoginPosition.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);

            ParameterData.Write(writer);
            LoginPosition.Write(writer);
        }

        public override NetCommand GetCommand() => NetCommand.NC_MAP_LOGIN_ACK;

        public override string ToString() => $"{nameof(Handle)}: {Handle}";
    }
}
