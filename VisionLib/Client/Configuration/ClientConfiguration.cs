using VisionLib.Common.Configuration;

namespace VisionLib.Client.Configuration
{
    public class ClientConfiguration : Configuration<ClientConfiguration>
    {
        public string FiestaUsername { get; protected set; } = "";
        public string FiestaPassword { get; protected set; } = "";

        public string LoginServerIP { get; protected set; } = "35.231.44.7"; // NA Server
        public ushort LoginServerPort { get; protected set; } = 9010;

        public string BinMD5 { get; protected set; } = "5a1a7b504cd5c73aa69bd8172ad12c69";
        public byte[] ClientVersionData { get; protected set; } = new byte[]
        {
            0x00, 0xAD, 0x95, 0x74, 0x18, 0xAC, 0x9F, 0x03,
            0x30, 0xAD, 0x95, 0x74, 0x68, 0xFB, 0x5C, 0x01,
            0x45, 0x1E, 0x11, 0x01, 0x34, 0x90, 0xAB, 0x03,
            0x39, 0x90, 0xAB, 0x03, 0x00, 0x00, 0x00, 0x00
        };

        public string XTrapVersionHash { get; protected set; } = "33B543B0CA6E7C41E5D1D0651307";

        public string SHNHash { get; protected set; } =
            "68dfd55338c9c5762ccf885c2ef0af3eabd3367bf5ebf7fca1f497534c58e347" +
            "1dbd668c1378fa5bce90f332f160aaef1b8335f60910b530a7ca8f4c249f396e" +
            "74c3eef10a457b0ae2d43f7a321056ce581f6cf94e6cccfa95cc212ac5eb1716" +
            "69da17f54964650e687807d86be8d798cce2a385486eee93a1b28260a743837c" +
            "9459a6d0324686b06e041ebaf9e097149c8dffd229931a86ba93900861cb9d9f" +
            "7b64dc5f984a968a726d17e9f170262f5bad01a80756642225be26cd20c4e333" +
            "f2bfcf7dff1f4bc30dd5fa7d82efc7c27ed90b26fed027b099466341f3b8fd25" +
            "f92edd5bfc1f153634613615bbf231d016fd3fb91178f0d1a4166efdc796394f" +
            "901df81cf6b86d94a13acdd1bcafc833751776ed43ec0cc26ce2f88d107dd0ae" +
            "6e7abb51320d720b8b37ffb46ecd08c9ec52b830b80fa981f675529932c6a122" +
            "957a1e635fdba595dfe8de2d7a5eca93900e6540d7c7c09b491b2ee4dc902edd" +
            "54540d7003c3866fdc9418732f4c73ef0706113a6854cdee1b7b8ac5e9affb30" +
            "e352fdc736298031a5dce8085459e147b2b7efeb7d298cfef44388ed2d990040" +
            "25a164350457285e12f9e13ef6b55d24e39cf307ca276ffaff62b026b970b413" +
            "fcb3a29302662583582042a2c7357dd9074b0b75891a1498a4bc55da00839c8e" +
            "d60261b08f5b9120edde7cb6339145019beb7adbedee829f88475a79a989d516" +
            "4016c99b64be11562ffa1cb5bf44597b99bb58a0792134f30fdefbe38b134598" +
            "430b58ecb99a879a4ec22f4db0d3f95e1144a5e4878238c7ea5aa070ca41052f" +
            "a48670ade019c79d82e17210c64d8b33b62bc7d6045ecddaf8879996522f1efc" +
            "d5fa32266dbb1ec8b993d731e56daf2d7816d8de3d0ad310589b98324804dfea" +
            "2a65568e4d1ea997f155c0903ec4b3a483403b934a56ee21bfa90e035faf2b5f" +
            "ad2620804e0c1e92f310994bcde07566ba0983e510ca47a36345b1fc25c19976" +
            "ab4b0c86bdd1e537b88bf270cf732c5cc9e86b694291b47bae3eda93777c87d4" +
            "7992c2600142c9d322ead8bcf7f12cc6f9ff3526d5dcd01d34cd6f7d63a44e68" +
            "57591bbc75a911c84a86bf38440570b7eefe188dd030fa1b0d97a15c176fbff2" +
            "585e42b55b3fab9041e390ce204041387fb119545c12aa7802c55e50c6fdc63b" +
            "46165787e43c0b974bb877d82eabf2619bf5c8502fe76608bf84ba4e6d6b95ef" +
            "05933163a7817b402185b2c25ca422380a7aed1cf3a4acf9e7b507a35c031a5c";
    }
}
