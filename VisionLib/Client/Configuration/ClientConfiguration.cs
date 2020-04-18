using VisionLib.Common.Configuration;

namespace VisionLib.Client.Configuration
{
    public class ClientConfiguration : Configuration<ClientConfiguration>
    {
        public string FiestaUsername { get; protected set; } = "";
        public string FiestaPassword { get; protected set; } = "";

        public string LoginServerIP { get; protected set; } = "35.231.44.7"; // NA Server
        public ushort LoginServerPort { get; protected set; } = 9010;

        public string WMServerIP { get; protected set; } = "35.231.44.7"; // NA Server
        public ushort WMServerPort { get; protected set; } = 9110;

        public int VersionYear { get; protected set; } = 2020;
        public int Version { get; protected set; } = 6916;

        public string BinMD5 { get; protected set; } = "5a1a7b504cd5c73aa69bd8172ad12c69";
        public byte[] ClientVersionData { get; protected set; } = new byte[]
        {
            0x00, 0xAD, 0x95, 0x74, 0x18, 0xAC, 0x9F, 0x03,
            0x30, 0xAD, 0x95, 0x74, 0x68, 0xFB, 0x5C, 0x01,
            0x45, 0x1E, 0x11, 0x01, 0x34, 0x90, 0xAB, 0x03,
            0x39, 0x90, 0xAB, 0x03, 0x00, 0x00, 0x00, 0x00
        };

        public string XTrapVersionHash { get; protected set; } = "33B543B0CA6E7C41E5D1D0651307";
    }
}
