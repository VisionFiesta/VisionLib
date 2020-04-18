namespace VisionLib.Common.Networking.Structs.User
{
    public class STRUCT_NC_USER_LOGINWORLD_ACK : FiestaNetStruct
    {
        // dw worldmanager
        // db numofavatar
        // PROTO_AVATARINFORMATION[] avatar

        public readonly ushort WorldManager;
        public byte AvatarCount;
        public PROTO_AVATARINFORMATION[] Avatars;

        public STRUCT_NC_USER_LOGINWORLD_ACK(FiestaNetPacket packet)
        {
            WorldManager = packet.ReadUInt16();
            AvatarCount = packet.ReadByte();

            if (AvatarCount > 0)
            {
                Avatars = new PROTO_AVATARINFORMATION[AvatarCount];
                for (var i = 0; i < AvatarCount; i++)
                {
                    Avatars[i] = new PROTO_AVATARINFORMATION(packet);
                }
            }
        }

        public override FiestaNetPacket ToPacket()
        {
            throw new System.NotImplementedException();
        }
    }

    public class PROTO_AVATARINFORMATION
    {
        // db byte
        // dw 2byte (uint16/ushort)
        // dd 4byte (uint32/int)
        // Name5 (string, 20 len)
        // Name3 (string, 12 len)

        public int chrregnum;
        public string name; //Name5 (5 * 4byte)
        public ushort level;
        public byte slot;
        public string loginmap; // Name3 (3 * 4byte)
        public PROTO_AVATAR_DELETE_INFO delinfo;
        public PROTO_AVATAR_SHAPE_INFO shape;
        public PROTO_EQUIPMENT equip;
        public int nKQHandle;
        public string sKQMapName; // Name3
        public SHINE_XY_TYPE nKQCoord;
        public SHINE_DATETIME dKQDate;
        public CHAR_ID_CHANGE_DATA CharIDChangeData;
        public PROTO_TUTORIAL_INFO TutorialInfo;

        public PROTO_AVATARINFORMATION(FiestaNetPacket packet)
        {
            chrregnum = packet.ReadInt32();
            name = packet.ReadString(20);
            level = packet.ReadUInt16();
            slot = packet.ReadByte();
            loginmap = packet.ReadString(12);
            delinfo = new PROTO_AVATAR_DELETE_INFO(packet);
            shape = new PROTO_AVATAR_SHAPE_INFO(packet);
            equip = new PROTO_EQUIPMENT(packet);
            nKQHandle = packet.ReadInt32();
            sKQMapName = packet.ReadString(12);
            nKQCoord = new SHINE_XY_TYPE(packet);
            dKQDate = new SHINE_DATETIME(packet);
            CharIDChangeData = new CHAR_ID_CHANGE_DATA(packet);
            TutorialInfo = new PROTO_TUTORIAL_INFO(packet);
        }
    }

    public class PROTO_AVATAR_DELETE_INFO
    {
        public byte year;
        public byte month;
        public byte day;
        public byte hour;
        public byte min;

        public PROTO_AVATAR_DELETE_INFO(FiestaNetPacket packet)
        {
            year = packet.ReadByte();
            month = packet.ReadByte();
            day = packet.ReadByte();
            hour = packet.ReadByte();
            min = packet.ReadByte();
        }
    }

    public class PROTO_AVATAR_SHAPE_INFO
    {
        public byte _bf0;
        public byte _bf1;
        public byte _bf2;
        public byte _bf3;

        public PROTO_AVATAR_SHAPE_INFO(FiestaNetPacket packet)
        {
            _bf0 = packet.ReadByte();
            _bf1 = packet.ReadByte();
            _bf2 = packet.ReadByte();
            _bf3 = packet.ReadByte();
        }
    }

    public class PROTO_EQUIPMENT
    {
        public ushort Equ_Head;
        public ushort Equ_Mouth;
        public ushort Equ_RightHand;
        public ushort Equ_Body;
        public ushort Equ_LeftHand;
        public ushort Equ_Pant;
        public ushort Equ_Boot;
        public ushort Equ_AccBoot;
        public ushort Equ_AccPant;
        public ushort Equ_AccBody;
        public ushort Equ_AccHeadA;
        public ushort Equ_MiniMon_R;
        public ushort Equ_Eye;
        public ushort Equ_AccLeftHand;
        public ushort Equ_AccRightHand;
        public ushort Equ_AccBack;
        public ushort Equ_CosEff;
        public ushort Equ_AccHip;
        public ushort Equ_Minimon;
        public ushort Equ_AccShield;
        public PROTO_EQUIPMENT_UPGRADE upgrade;

        public PROTO_EQUIPMENT(FiestaNetPacket packet)
        {
            Equ_Head = packet.ReadUInt16();
            Equ_Mouth = packet.ReadUInt16();
            Equ_RightHand = packet.ReadUInt16();
            Equ_Body = packet.ReadUInt16();
            Equ_LeftHand = packet.ReadUInt16();
            Equ_Pant = packet.ReadUInt16();
            Equ_Boot = packet.ReadUInt16();
            Equ_AccBoot = packet.ReadUInt16();
            Equ_AccPant = packet.ReadUInt16();
            Equ_AccBody = packet.ReadUInt16();
            Equ_AccHeadA = packet.ReadUInt16();
            Equ_MiniMon_R = packet.ReadUInt16();
            Equ_Eye = packet.ReadUInt16();
            Equ_AccLeftHand = packet.ReadUInt16();
            Equ_AccRightHand = packet.ReadUInt16();
            Equ_AccBack = packet.ReadUInt16();
            Equ_CosEff = packet.ReadUInt16();
            Equ_AccHip = packet.ReadUInt16();
            Equ_Minimon = packet.ReadUInt16();
            Equ_AccShield = packet.ReadUInt16();
            upgrade = new PROTO_EQUIPMENT_UPGRADE(packet);
        }
    }

    public class PROTO_EQUIPMENT_UPGRADE
    {
        public byte _bf0;
        public byte _bf1;
        public byte _bf2;

        public PROTO_EQUIPMENT_UPGRADE(FiestaNetPacket packet)
        {
            _bf0 = packet.ReadByte();
            _bf1 = packet.ReadByte();
            _bf2 = packet.ReadByte();
        }
    }

    public class SHINE_XY_TYPE
    {
        public int x;
        public int y;

        public SHINE_XY_TYPE(FiestaNetPacket packet)
        {
            x = packet.ReadByte();
            y = packet.ReadByte();
        }
    }

    public class SHINE_DATETIME
    {
        public int _bf0;

        public SHINE_DATETIME(FiestaNetPacket packet)
        {
            _bf0 = packet.ReadInt32();
        }
    }

    public class CHAR_ID_CHANGE_DATA
    {
        public bool bNeedChangeID;
        public bool bInit;
        public int nRowNo;

        public CHAR_ID_CHANGE_DATA(FiestaNetPacket packet)
        {
            bNeedChangeID = packet.ReadBoolean();
            bInit = packet.ReadBoolean();
            nRowNo = packet.ReadInt32();
        }
    }

    public class PROTO_TUTORIAL_INFO
    {
        public int nTutorialState; // enum TUTORIAL_STATE
        public byte nTutorialStep;

        public PROTO_TUTORIAL_INFO(FiestaNetPacket packet)
        {
            nTutorialState = packet.ReadInt32();
            nTutorialStep = packet.ReadByte();
        }
    }
}
