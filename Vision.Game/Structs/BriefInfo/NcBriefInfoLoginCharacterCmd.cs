using Vision.Core.Common;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Characters;
using Vision.Game.Enums;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoLoginCharacterCmd : NetPacketStruct
    {
        // struct __unaligned __declspec(align(1)) PROTO_NC_BRIEFINFO_LOGINCHARACTER_CMD
        // {
        //     unsigned __int16 handle;
        //     Name5 charid;
        //     SHINE_COORD_TYPE coord;
        //     char mode;
        //     char chrclass;
        //     PROTO_AVATAR_SHAPE_INFO shape;
        //         $7E9DF37DA92D66C9DC9760D2E5111AAE shapedata;
        //     unsigned __int16 polymorph;
        //     STOPEMOTICON_DESCRIPT emoticon;
        //     CHARTITLE_BRIEFINFO chartitle;
        //     ABNORMAL_STATE_BIT abstatebit;
        //     unsigned int myguild;
        //     char type;
        //     char isGuildAcademyMember;
        //     char IsAutoPick;
        //     char Level;
        //     char sAnimation[32];
        //     unsigned __int16 nMoverHnd;
        //     char nMoverSlot;
        //     char nKQTeamType;
        //     char IsUseItemMinimon;
        // };

        public ushort Handle;
        public string CharID; // Name5
        public ShineXYR Position = new ShineXYR();
        public CharacterState State;
        public CharacterClass Class;
        public ProtoAvatarShapeInfo Shape = new ProtoAvatarShapeInfo();
        public ProtoAvatarShapeData ShapeData;
        public ushort Polymorph;
        public StopEmoticonDescript Emoticon = new StopEmoticonDescript();
        public CharTitleBriefInfo CharTitle = new CharTitleBriefInfo();
        public AbnormalStateBit AbstateBit = new AbnormalStateBit();
        public uint MyGuild;
        public byte Type;
        public bool IsGuildAcademyMember;
        public bool IsAutoPick;
        public byte Level;
        public string Animation; // Name32Byte
        public ushort MoverHandle;
        public byte MoverSlot;
        public KQTeamType KQTeam;
        public bool IsUseItemMinimon;

        public override int GetSize() => 248;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            CharID = reader.ReadString(NameN.Name5Len);
            Position.Read(reader);
            State = (CharacterState) reader.ReadByte();
            Class = (CharacterClass) reader.ReadByte();
            Shape.Read(reader);

            ShapeData = new ProtoAvatarShapeData(State);
            ShapeData.Read(reader);

            Polymorph = reader.ReadUInt16();
            Emoticon.Read(reader);
            CharTitle.Read(reader);
            AbstateBit.Read(reader);
            MyGuild = reader.ReadUInt32();
            Type = reader.ReadByte();
            IsGuildAcademyMember = reader.ReadBoolean();
            IsAutoPick = reader.ReadBoolean();
            Level = reader.ReadByte();
            Animation = reader.ReadString(NameN.Name32ByteLen);
            MoverHandle = reader.ReadUInt16();
            MoverSlot = reader.ReadByte();
            KQTeam = (KQTeamType) reader.ReadByte();
            IsUseItemMinimon = reader.ReadBoolean();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(CharID, NameN.Name5Len);
            Position.Write(writer);
            writer.Write((byte)State);
            writer.Write((byte)Class);
            Shape.Write(writer);
            ShapeData.Write(writer);
            writer.Write(Polymorph);
            Emoticon.Write(writer);
            CharTitle.Write(writer);
            AbstateBit.Write(writer);
            writer.Write(MyGuild);
            writer.Write(Type);
            writer.Write(IsGuildAcademyMember);
            writer.Write(IsAutoPick);
            writer.Write(Level);
            writer.Write(Animation, NameN.Name32ByteLen);
            writer.Write(MoverHandle);
            writer.Write(MoverSlot);
            writer.Write((byte)KQTeam);
            writer.Write(IsUseItemMinimon);
        }

        public override FiestaNetCommand GetCommand() => FiestaNetCommand.NC_BRIEFINFO_LOGINCHARACTER_CMD;
    }

    //$7E9DF37DA92D66C9DC9760D2E5111AAE shapedata
    public class ProtoAvatarShapeData : AbstractStruct
    {
        private readonly CharacterState _state;

        public ProtoEquipment Equipment = new ProtoEquipment();
        public CharBriefInfoRideRideInfo RideInfo;
        public CharBriefInfoCamp Camp;
        public CharBriefInfoBooth Booth;


        public ProtoAvatarShapeData(CharacterState state)
        {
            _state = state;
        }

        public override int GetSize()
        {
            return 45;
        }

        public override void Read(ReaderStream reader)
        {
            switch (_state)
            {
                case CharacterState.CS_PLAYER:
                case CharacterState.CS_PLAYER2:
                case CharacterState.CS_DEAD:
                {
                    Equipment.Read(reader); // +43

                    reader.ReadBytes(GetSize() - Equipment.GetSize()); //padding
                    break;
                }
                case CharacterState.CS_CAMP:
                {
                    Camp = new CharBriefInfoCamp();
                    Camp.Read(reader); // +12

                    reader.ReadBytes(GetSize() - Camp.GetSize()); // padding
                    break;
                }
                case CharacterState.CS_VENDOR:
                {
                    Booth = new CharBriefInfoBooth();
                    Booth.Read(reader); // +43

                    reader.ReadBytes(GetSize() - Booth.GetSize()); //padding
                    break;
                }
                case CharacterState.CS_RIDE:
                    Equipment.Read(reader); // +43

                    RideInfo = new CharBriefInfoRideRideInfo();
                    RideInfo.Read(reader); // +2
                    break;
            }
        }

        public override void Write(WriterStream writer)
        {
            switch (_state)
            {
                case CharacterState.CS_PLAYER:
                case CharacterState.CS_PLAYER2:
                case CharacterState.CS_DEAD:
                {
                    Equipment.Write(writer); // +43
                    writer.Fill(GetSize() - Equipment.GetSize(), 0x00); //padding
                    break;
                }
                case CharacterState.CS_CAMP:
                {
                    Camp.Write(writer); // +12
                    writer.Fill(GetSize() - Camp.GetSize(), 0x00); // padding
                    break;
                }
                case CharacterState.CS_VENDOR:
                {
                    Booth.Write(writer); // +43
                    writer.Fill(GetSize() - Booth.GetSize(), 0x00); //padding
                    break;
                }
                case CharacterState.CS_RIDE:
                    Equipment.Write(writer); // +43
                    RideInfo.Write(writer); // +2
                    break;
            }
        }
    }

    public class CharBriefInfoCamp : AbstractStruct
    {
        public ushort Minihouse;
        public byte[] Dummy = new byte[10];

        public override int GetSize() => 12;

        public override void Read(ReaderStream reader)
        {
            Minihouse = reader.ReadUInt16();
            Dummy = reader.ReadBytes(10);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Minihouse);
            writer.Write(Dummy, 10);
        }
    }

    public class CharBriefInfoBooth : AbstractStruct
    {
        public CharBriefInfoCamp Camp;
        public bool IsSell;
        public StreetBoothSignboard Signboard;

        public override int GetSize() => 43;

        public override void Read(ReaderStream reader)
        {
            Camp = new CharBriefInfoCamp();
            Camp.Read(reader);

            IsSell = reader.ReadBoolean();

            Signboard = new StreetBoothSignboard();
            Signboard.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            Camp.Write(writer);
            writer.Write(IsSell);
            Signboard.Write(writer);
        }
    }

    public class CharBriefInfoRideRideInfo : AbstractStruct
    {
        public ushort Horse;

        public override int GetSize() => 2;
        public override void Read(ReaderStream reader) => Horse = reader.ReadUInt16();
        public override void Write(WriterStream writer) => writer.Write(Horse);
    }

}
