﻿using System;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Misc
{
    public class NcGameTimeAck : NetPacketStruct
    {
        public byte Hour { get; private set; }
        public byte Minute { get; private set; }
        public byte Second { get; private set; }

        public DateTime GameTime { get; private set; }

        public NcGameTimeAck() { }

        public NcGameTimeAck(DateTime gameTime)
        {
            Hour = (byte)gameTime.Hour;
            Minute = (byte)gameTime.Minute;
            Second = (byte)gameTime.Second;

            GameTime = gameTime;
        }

        public override int GetSize()
        {
            return sizeof(byte) * 3;
        }

        public override void Read(ReaderStream reader)
        {
            Hour = reader.ReadByte();
            Minute = reader.ReadByte();
            Second = reader.ReadByte();

            GameTime = ToDateTime(this);
        }

        public override void Write(WriterStream writer)
        {
            if (writer == null) return;
            writer.Write(Hour);
            writer.Write(Minute);
            writer.Write(Second);
        }

        public static DateTime ToDateTime(NcGameTimeAck gameTime)
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, gameTime.Hour, gameTime.Minute, gameTime.Second);
        }

        protected override NetCommand GetCommand()
        {
            return NetCommand.NC_MISC_GAMETIME_ACK;
        }
    }
}
