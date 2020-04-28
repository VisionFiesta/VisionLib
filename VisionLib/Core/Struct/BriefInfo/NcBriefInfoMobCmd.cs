using System;
using System.Collections.Generic;
using System.Text;
using VisionLib.Common.Game.Enums;
using VisionLib.Common.Networking;
using VisionLib.Core.Stream;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Core.Struct.BriefInfo
{
    public class NcBriefInfoMobCmd : NetPacketStruct
    {
        // struct __unaligned __declspec(align(1)) PROTO_NC_BRIEFINFO_REGENMOB_CMD
        // {
        //     unsigned __int16 handle;
        //     char mode;
        //     unsigned __int16 mobid;
        //     SHINE_COORD_TYPE coord;
        //     char flagstate;
        //         $BA92430D3E249705924BF5BD724F50E2 flag;
        //     char sAnimation[32];
        //     char nAnimationLevel;
        //     char nKQTeamType;
        //     char bRegenAni;
        // };

        public ushort Handle;
        public byte Mode;
        public ushort MobID;
        public ShineXYR Position;
        public byte FlagState;
        public ABSTATEINDEX AbstateBit;
        public byte[] GateToWhere = new byte[12];
        public string Animation; //Name32Byte
        public byte AnimationLevel;
        public byte KQTeamType;
        public bool RegenAni;


        public override int GetSize()
        {
            return 63;
        }

        public override void Read(ReaderStream reader)
        {
            throw new NotImplementedException();
        }

        public override void Write(WriterStream writer)
        {
            throw new NotImplementedException();
        }

        public override FiestaNetCommand GetCommand()
        {
            throw new NotImplementedException();
        }
    }
}
