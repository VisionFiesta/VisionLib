using System;
using System.Diagnostics.CodeAnalysis;
using Vision.Core;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Core.Utils;
using Vision.Game.Characters;
using Vision.Game.Characters.Shape;

namespace Vision.Game.Structs.Common
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ProtoAvatarInformation : AbstractStruct
    {
        public const int Size = sizeof(int) * 2
                                + NameN.Name5Len
                                + NameN.Name3Len * 2
                                + sizeof(ushort)
                                + sizeof(byte)
                                + ProtoAvatarDeleteInfo.Size
                                + ProtoAvatarShapeInfo.Size
                                + ProtoEquipment.Size
                                + ShineXY.Size
                                + ShineDatetime.Size
                                + CharIdChangeData.Size
                                + ProtoTutorialInfo.Size;

        // db byte
        // dw 2byte (uint16/ushort)
        // dd 4byte (uint32/int)
        // Name5 (string, 20 len)
        // Name3 (string, 12 len)

        public uint CharNo; // +4
        public string CharName; //Name5 (5 * 4byte), +20
        public ushort Level; // +2
        public byte CharSlot; // +1
        public string LoginMap; // Name3 (3 * 4byte), +12
        public ProtoAvatarDeleteInfo DeleteInfo = new ProtoAvatarDeleteInfo(); // +5
        public ProtoAvatarShapeInfo CharShape = new ProtoAvatarShapeInfo(); // +4
        public ProtoEquipment Equipment = new ProtoEquipment(); // +43
        public int KQHandle; // +4
        public string KQMapName; // Name3, +12
        public ShineXY nKQCoord = new ShineXY(); // +8
        public ShineDatetime KQDate = new ShineDatetime(); // +4
        public CharIdChangeData CharIDChangeData = new CharIdChangeData(); // +6
        public ProtoTutorialInfo TutorialInfo = new ProtoTutorialInfo(); // +5

        public override string ToString() => $"Name: {CharName}, Level:, {Level}, Class: {CharShape.JobGender}, Map:{LoginMap}, CharNo: {CharNo}";

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            CharNo = reader.ReadUInt32();
            CharName = reader.ReadString(20);
            Level = reader.ReadUInt16();
            CharSlot = reader.ReadByte();
            LoginMap = reader.ReadString(12);
            DeleteInfo.Read(reader);
            CharShape.Read(reader);
            Equipment.Read(reader);
            KQHandle = reader.ReadInt32();
            KQMapName = reader.ReadString(12);
            nKQCoord.Read(reader);
            KQDate.Read(reader);
            CharIDChangeData.Read(reader);
            TutorialInfo.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharNo);
            writer.Write(CharName, NameN.Name5Len);
            writer.Write(Level);
            writer.Write(CharSlot);
            writer.Write(LoginMap, NameN.Name3Len);
            DeleteInfo.Write(writer);
            CharShape.Write(writer);
            Equipment.Write(writer);
            writer.Write(KQHandle);
            writer.Write(KQMapName, NameN.Name3Len);
        }
    }

    public class ProtoAvatarDeleteInfo : AbstractStruct
    {
        public const int Size = 5;

        public byte Year;
        public byte Month;
        public byte Day;
        public byte Hour;
        public byte Min;

        public DateTime Time;

        public bool IsDeleted;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Year = reader.ReadByte();
            Month = reader.ReadByte();
            Day = reader.ReadByte();
            Hour = reader.ReadByte();
            Min = reader.ReadByte();

            if (Year == 0)
            {
                Time = new DateTime();
                IsDeleted = false;
            }
            else
            {
                Time = new DateTime(Year, Month, Day, Hour, Min, 0);
                IsDeleted = true;
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Year);
            writer.Write(Month);
            writer.Write(Day);
            writer.Write(Hour);
            writer.Write(Min);
        }
    }

    public class ProtoAvatarShapeInfo : AbstractStruct
    {
        public const int Size = 4;

        public byte JobGender;
        public byte Hair;
        public byte HairColor;
        public byte Face;

        public CharacterClass Job;
        public CharacterGender Gender;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            JobGender = reader.ReadByte();
            Hair = reader.ReadByte();
            HairColor = reader.ReadByte();
            Face = reader.ReadByte();

            using var bs = new BitStream();
            bs.Write(JobGender);

            var jobByte = byte.MaxValue;
            var genderByte = byte.MaxValue;

            bs.Read(out jobByte, 0, 7);
            bs.Read(out genderByte, 0, 1);

            Job = (CharacterClass)jobByte;
            Gender = (CharacterGender)genderByte;
        }

        public override void Write(WriterStream writer)
        {
            using (var bs = new BitStream())
            {
                bs.Write((byte)Job, 0, 7);
                bs.Write((byte)Gender, 0, 1);

                JobGender = (byte)bs.ReadByte();

                writer.Write(JobGender);
            }

            writer.Write(Hair);
            writer.Write(HairColor);
            writer.Write(Face);
        }
    }

    public class ProtoEquipment : AbstractStruct
    {
        public const int Size = 43;

        // 20 ushort (40 bytes)
        public ushort EquHead;
        public ushort EquMouth;
        public ushort EquRightHand;
        public ushort EquBody;
        public ushort EquLeftHand;
        public ushort EquPant;
        public ushort EquBoot;
        public ushort EquAccBoot;
        public ushort EquAccPant;
        public ushort EquAccBody;
        public ushort EquAccHeadA;
        public ushort EquMiniMonR;
        public ushort EquEye;
        public ushort EquAccLeftHand;
        public ushort EquAccRightHand;
        public ushort EquAccBack;
        public ushort EquCosEff;
        public ushort EquAccHip;
        public ushort EquMinimon;
        public ushort EquAccShield;

        // 3 bytes
        public ProtoEquipmentUpgrade Upgrade = new ProtoEquipmentUpgrade();

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            EquHead = reader.ReadUInt16();
            EquMouth = reader.ReadUInt16();
            EquRightHand = reader.ReadUInt16();
            EquBody = reader.ReadUInt16();
            EquLeftHand = reader.ReadUInt16();
            EquPant = reader.ReadUInt16();
            EquBoot = reader.ReadUInt16();
            EquAccBoot = reader.ReadUInt16();
            EquAccPant = reader.ReadUInt16();
            EquAccBody = reader.ReadUInt16();
            EquAccHeadA = reader.ReadUInt16();
            EquMiniMonR = reader.ReadUInt16();
            EquEye = reader.ReadUInt16();
            EquAccLeftHand = reader.ReadUInt16();
            EquAccRightHand = reader.ReadUInt16();
            EquAccBack = reader.ReadUInt16();
            EquCosEff = reader.ReadUInt16();
            EquAccHip = reader.ReadUInt16();
            EquMinimon = reader.ReadUInt16();
            EquAccShield = reader.ReadUInt16();

            Upgrade.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(EquHead);
            writer.Write(EquMouth);
            writer.Write(EquRightHand);
            writer.Write(EquBody);
            writer.Write(EquLeftHand);
            writer.Write(EquPant);
            writer.Write(EquBoot);
            writer.Write(EquAccBoot);
            writer.Write(EquAccPant);
            writer.Write(EquAccBody);
            writer.Write(EquAccHeadA);
            writer.Write(EquMiniMonR);
            writer.Write(EquEye);
            writer.Write(EquAccLeftHand);
            writer.Write(EquAccRightHand);
            writer.Write(EquAccBack);
            writer.Write(EquCosEff);
            writer.Write(EquAccHip);
            writer.Write(EquMinimon);
            writer.Write(EquAccShield);

            Upgrade.Write(writer);
        }
    }

    public class ProtoEquipmentUpgrade : AbstractStruct
    {
        public const int Size = 3;

        public byte Bitfield0;
        public byte Bitfield1;
        public byte Bitfield2;

        public override int GetSize() => 3;

        public override void Read(ReaderStream reader)
        {
            Bitfield0 = reader.ReadByte();
            Bitfield1 = reader.ReadByte();
            Bitfield2 = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Bitfield0);
            writer.Write(Bitfield1);
            writer.Write(Bitfield2);
        }
    }

    public class CharIdChangeData : AbstractStruct
    {
        public const int Size = 6;

        public bool NeedChangeId;
        public bool Init;
        public int RowNo;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            NeedChangeId = reader.ReadBoolean();
            Init = reader.ReadBoolean();
            RowNo = reader.ReadInt32();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(NeedChangeId);
            writer.Write(Init);
            writer.Write(RowNo);
        }
    }

    public class ProtoTutorialInfo : AbstractStruct
    {
        public const int Size = 5;

        public int TutorialState; // TODO: enum TUTORIAL_STATE
        public byte TutorialStep;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            TutorialState = reader.ReadInt32();
            TutorialStep = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(TutorialState);
            writer.Write(TutorialStep);
        }
    }
}
