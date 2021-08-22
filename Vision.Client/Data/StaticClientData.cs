using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Vision.Core.Configuration;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Vision.Client.Data
{
    public class StaticClientData
    {
        public static StaticClientData DefaultStaticClientData { get; } = new(); 
        
        public string ShinePath { get; protected set; } = @"F:\Gamigo\FiestaNA\ressystem";

        // latest as of 1.02.311
        public string BinMd5 { get; protected set; } = "d73a0cf2b4296d28a91d070dabfe72d4";

        [JsonIgnore]
        public byte[] BinMd5Bytes => Encoding.ASCII.GetBytes(BinMd5);

        // latest as of 1.02.311
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] VersionData { get; protected set; } =
        {
            0x00, 0xFA, 0x2F, 0x01, 0xD0, 0xD6, 0xFE, 0x70,
            0xB1, 0xCC, 0xE9, 0x2F, 0xFE, 0xFF, 0xFF, 0xFF,
            0x2C, 0xFA, 0x2F, 0x01, 0x65, 0x21, 0x50, 0x77,
            0x2C, 0x09, 0x00, 0x00, 0x18, 0xFA, 0x2F, 0x01
        };

        [JsonIgnore]
        public byte[] FullVersionBytes => BinMd5Bytes.Concat(VersionData).ToArray();

        // Latest as of NA 1.02.287 (unchanged as of 8/17/2021, 1.02.310)
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] XTrapVersionHash { get; protected set; } =
        {
            0x33, 0x33, 0x42, 0x35, 0x34, 0x33, 0x42, 0x30,
            0x43, 0x41, 0x36, 0x45, 0x37, 0x43, 0x34, 0x31,
            0x45, 0x35, 0x44, 0x31, 0x44, 0x30, 0x36, 0x35,
            0x31, 0x33, 0x30, 0x37, 0x00
        };

        // latest as of 1.02.310
        // public string ShnHash { get; protected set; } = "b888d285d03b21555a3023ea25c5f1dc2f7ff1ef6e7fb803e5fe75a6e988c3a7226922463b9c895259e35a71213ff46a0a24233ecd7449d9e6708bb225535b6c3501b5d079164a889be2e73e4bc401e5270a830aeeca819d3a797acfd2838178edf53df73d70b8fa0088727afaf527cad8f24717dc2ceecb60291ada4b8143865c0c6cd7cd658380bd2989cfb58796f528aadb9ea809ca4dcfe4960af52f9ed79928be776ef1ddc6f1f70efdaf2ba2a5abc124553d79c44aaa9ddb475c0a87ecafa45cbe0e8fc518331d088fffbde2d1a70c9be806d0acbc726b28137aaee3cbb3bb4e6518f73768fbb971d9ecae506ad758ca9bef279f153b6e4aa67b0de76694505fc51714df3baa361686fccbadec1cc4ba41ab2b6d0e75dbfb253de13abddcba4f7b8384bfbc8334f7ba581d09f22c67d15caf210a4cedc2dca419229f6c6dd88ba2d0fd7dac994faed308f505a6c626ab6650c5ee0474c51d4965f1046c48edaa6fc4c09332431ce31c0ad1ac7e9d703583cb797009e83b9093369828d66ed90d1348250a96b5c6d5d11eff182a40683c8f295e9877a03b224025c33e7b2412b031375ba9d877abd58efa45fe4057fbb649ebc8da86cd7d64cd88d39ab3692a947059eb81169c7342c394b6eda8a3aeed845f1740fe5e08a35f9c5eea67ed7a259b5b3b263be7e43a89dd2cd35492981935c93df6f595b2579f53b3662deabfc48d79247de32e537511fda940bb0784cd1ccfa6aae18e3889397e11852b6adb711a22f8515eac76747518e2dd33054a795cfbdf0fac17435fb758cbfd73d72815d34896398714dfff3f6a9c81ed4cf0f51c5cc38aefded2f4fb8ca2313f71909257dc7cdf59423ac7671683336db710d49469a9a744bd531e0df4010d135bc86e37472127a65ab065dc3a7f66c26e1c98f2152bfbf1ffe4446ce26a9776e7b2cf5b5e91d120ddffe476699b7f48b9b443801ac8620a02099c92516a3f37c3bc0562a875f30f991950b094359363c12005120aeb20b1349b7465e858165b9c95e2d6e080967c60ed0f1e7e93d5d393168b32ad5874ae6f8c774bd1122ca35a56859c825c841d44fceb530f990b76a81e76f23e15806ec68e06222b49757b2e0c70434ab7872728f9f4e63a1d048f721aa6439f70ad7c2fb5c4bcfe2cac17fde365888a01373b0bc71df833d25a231df12414eb6ab0b4b41ca1f17321f174";
        
        // 1.02.311
        public string ShnHash { get; protected set; } =
            "b888d285d03b21555a3023ea25c5f1dc2f7ff1ef6e7fb803e5fe75a6e988c3a7226922463b9c895259e35a71213ff46a0a24233ecd7449d9e6708bb225535b6c3501b5d079164a889be2e73e4bc401e5270a830aeeca819d3a797acfd2838178edf53df73d70b8fa0088727afaf527cad8f24717dc2ceecb60291ada4b8143865c0c6cd7cd658380bd2989cfb58796f528aadb9ea809ca4dcfe4960af52f9ed79928be776ef1ddc6f1f70efdaf2ba2a5abc124553d79c44aaa9ddb475c0a87ecafa45cbe0e8fc518331d088fffbde2d1a70c9be806d0acbc726b28137aaee3cbb3bb4e6518f73768fbb971d9ecae506ad758ca9bef279f153b6e4aa67b0de76694505fc51714df3baa361686fccbadec1cc4ba41ab2b6d0e75dbfb253de13abddcba4f7b8384bfbc8334f7ba581d09f22c67d15caf210a4cedc2dca419229f6c6dd88ba2d0fd7dac994faed308f505a6c626ab6650c5ee0474c51d4965f1046c48edaa6fc4c09332431ce31c0ad1ac7e9d703583cb797009e83b9093369828d66ed90d1348250a96b5c6d5d11eff182a40683c8f295e9877a03b224025c33e7b2412b031375ba9d877abd58efa45fe4057fbb649ebc8da86cd7d64cd88d39ab3692a947059eb81169c7342c394b6eda8a3aeed845f1740fe5e08a35f9c5eea67ed7a259b5b3b263be7e43a89dd2cd35492981935c93df6f595b2579f53b3662deabfc48d79247de32e537511fda940bb0784cd1ccfa6aae18e3889397e11852b6adb711a22f8515eac76747518e2dd33054a795cfbdf0fac17435fb758cbfd73d72815d34896398714dfff3f6a9c81ed4cf0f51c5cc38aefded2f4fb8ca2313f71909257dc7cdf59423ac7671683336db710d49469a9a744bd531e0df4010d135bc86e37472127a65ab065dc3a7f66c26e1c98f2152bfbf1ffe4446ce26a9776e7b2cf5b5e91d120ddffe476699b7f48b9b443801ac8620a02099c92516a3f37c3bc0562a875f30f991950b094359363c12005120aeb20b1349b7465e858165b9c95e2d6e080967c60ed0f1e7e93d5d393168b32ad5874ae6f8c774bd1122ca35a56859c825c841d44fceb530f990b763464e6b33548b80d83443795435b67eac93e4381771398c8ca91228b7ae8627144b29c4f631d6cc0e708db4afb48cae0f70d89eb91cd1b9e6c969d191cdd305f7089d08cb641018d04b51924b652d0f2";
    }
}
