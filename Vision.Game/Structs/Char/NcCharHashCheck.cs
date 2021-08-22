using System;
using System.Collections;
using System.Collections.Generic;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Char
{
    public class NcCharHashCheck : NetPacketStruct
    {
        public const int Size = 720;

        private byte[] _data = new byte[Size];
        
        public NcCharHashCheck() {}
        
        public NcCharHashCheck(IEnumerable<byte> prefix)
        {
            var random = new Random();
            var i = 0;
            foreach (var b in prefix)
            {
                _data[i] = b;
                i++;
            }

            for (var j = i; j < _data.Length; j++)
            {
                _data[j] = (byte) random.Next(0, 255);
            }
        }
        
        public override int GetSize() => Size;

        public override void Read(ReaderStream reader) => _data = reader.ReadBytes(Size);

        public override void Write(WriterStream writer) => writer.Write(_data);

        protected override NetCommand GetCommand() => NetCommand.NC_CHAR_HASHCHECK;
    }
}