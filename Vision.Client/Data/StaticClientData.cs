using System.Linq;
using System.Text;

namespace Vision.Client.Data
{
    public class StaticClientData
    {
        public string ShinePath { get; protected set; } = @"F:\Fiesta\Content\GamigoNA\ressystem";

        public string BinMD5 { get; protected set; } = "554b53b1d16c280d071a86961d3bf425";

        public byte[] BinMD5Bytes => Encoding.ASCII.GetBytes(BinMD5);

        // latest as of 1.02.320
        public byte[] VersionData { get; protected set; } =
        {
            0x00, 0x34, 0xF1, 0x74, 0xA8, 0xAA, 0x08, 0x09,
            0x50, 0x34, 0xF1, 0x74, 0xD4, 0xFA, 0x8F, 0x00,
            0x25, 0xC1, 0xDA, 0x00, 0x34, 0xED, 0x14, 0x09,
            0x39, 0xED, 0x14, 0x09, 0x00, 0x00, 0x00, 0x00
        };

        public byte[] FullVersionBytes => BinMD5Bytes.Concat(VersionData).ToArray();

        // Latest as of NA 1.02.287 (unchanged as of 4/27/2021)
        public byte[] XTrapVersionHash { get; protected set; } =
        {
            0x33, 0x33, 0x42, 0x35, 0x34, 0x33, 0x42, 0x30,
            0x43, 0x41, 0x36, 0x45, 0x37, 0x43, 0x34, 0x31,
            0x45, 0x35, 0x44, 0x31, 0x44, 0x30, 0x36, 0x35,
            0x31, 0x33, 0x30, 0x37, 0x00
        };

        public string SHNHash { get; protected set; } = "32df49f86ccb938df38049c13877543fbc051811314b68120b4e9fa16e9ae2088b56e16ba1ee3012ac76506886f328304bf71bd36e23b57c96e69af3d51c694174c3eef10a457b0ae2d43f7a321056ce581f6cf94e6cccfa95cc212ac5eb171620caac524008ee4e3a157d3eedd86e15cce2a385486eee93a1b28260a743837c2d548b6b6c26a78aaf52564580f0f0c7aa076045d80c2f4b60e15ccc8f9aa8ae440137e86351832a80516b0038b96fda5bad01a80756642225be26cd20c4e333f2bfcf7dff1f4bc30dd5fa7d82efc7c24bbf6af91542a1c284ad20a976589e76f92edd5bfc1f153634613615bbf231d016fd3fb91178f0d1a4166efdc796394f9f2759e43a9afe3c04aaef931a78d465751776ed43ec0cc26ce2f88d107dd0ae6e7abb51320d720b8b37ffb46ecd08c9ec52b830b80fa981f675529932c6a122957a1e635fdba595dfe8de2d7a5eca93900e6540d7c7c09b491b2ee4dc902edd54540d7003c3866fdc9418732f4c73ef0706113a6854cdee1b7b8ac5e9affb30ee3257567634378f2c55bf399f17e997b7c06a571aa583dbf05158290dbac5f299a61880d73cfe72f9b609edf3569f306bdb63793a9346fcc6947ac9a337fa74905de16e344b1a09fd5b3202bf70c17336292f2f6f16fd4f27c17b70e8670447a10b4b9ade3738ab4fce3ee3f524ff35b7faeff26491c7df053fc3f55884297d430b58ecb99a879a4ec22f4db0d3f95e1144a5e4878238c7ea5aa070ca41052fa48670ade019c79d82e17210c64d8b33b62bc7d6045ecddaf8879996522f1efcd5fa32266dbb1ec8b993d731e56daf2d7816d8de3d0ad310589b98324804dfea0d0a95156feefb4b47a69ec3bbe0edeb83403b934a56ee21bfa90e035faf2b5ffe51f3eb246eeae538fcfa4d68c625d6ba0983e510ca47a36345b1fc25c19976ab4b0c86bdd1e537b88bf270cf732c5cc9e86b694291b47bae3eda93777c87d4a5a223b3a54ecdf31d849840c1060bf39e257da1d15c8dc93e46db4efcf186d157591bbc75a911c84a86bf38440570b7eefe188dd030fa1b0d97a15c176fbff2585e42b55b3fab9041e390ce2040413881f9b49d0e59185b5d185f416a09d6c2975c2e9055d741e944ced85eff480bdc10d1e853d555e2c4656128fd9aa12b9992844cd25d663b7203c421e916363d2f6ac533e6af4442a51ebb92d29f02ffa1";
    }
}
