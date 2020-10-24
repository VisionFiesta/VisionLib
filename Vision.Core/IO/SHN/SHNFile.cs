using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Vision.Core.Exceptions;
using Vision.Core.Extensions;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.IO.SHN
{
    public class SHNFile
    {
        private static readonly EngineLog Logger = new EngineLog(typeof(SHNFile));

        private string _shnFilePath;
        private readonly ISHNCrypto _shnCrypto;
        private readonly Encoding _shnEncoding;

        public readonly SHNType SHNType;
        public List<SHNColumn> SHNColumns = new List<SHNColumn>();

        public SHNResult Data { get; private set; }

        private byte[] _cryptoHeader;
        private byte[] _data;
        private int _dataLength;
        private uint _header;
        private uint _rowCount;
        // private uint _defaultRowLength;
        private uint _columnCount;

        public string MD5Hash { get; private set; }

        public static SHNFile Create(string shnFolder, SHNType shnType, ISHNCrypto shnCrypto, Encoding shnEncoding)
        {
            try
            {
                return new SHNFile(shnFolder, shnType, shnCrypto, shnEncoding);
            }
            catch (EngineException ex)
            {
                Logger.Error("Exception when creating SHNFile", ex);
                return null;
            }
        }

        internal SHNFile(string shnFolder, SHNType shnType, ISHNCrypto shnCrypto, Encoding shnEncoding)
        {
            _shnCrypto = shnCrypto;
            _shnEncoding = shnEncoding;

            if (string.IsNullOrEmpty(shnFolder) || !Directory.Exists(shnFolder))
            {
                throw new EngineException("SHN folder does not exist!");
            }

            var shnFilename = $"{shnType}.shn";
            var shnFullPath = Path.Combine(shnFolder, shnFilename);

            if (!File.Exists(shnFullPath))
            {
               throw new EngineException($"File not found for SHN Type: {shnType}");
            }

            var shnTypeOfFile = EnumExtensions.GetValueOrDefault(shnType.ToString(), SHNType.Unknown);

            if (shnType != shnTypeOfFile)
            {
                throw new EngineException($"SHN Type does not match file! Requested Type: {shnType}, File Type: {shnTypeOfFile}, File name: {shnFilename}");
            }

            SHNType = shnType;
            _shnFilePath = shnFullPath;
        }

        private static string CalculateMD5Hash(byte[] cryptoHeader, int dataLength, byte[] data)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            using var md5 = MD5.Create();

            // 32 + 4 + data.Length
            stream.Capacity = 36 + data.Length;

            writer.Write(cryptoHeader); // 32
            writer.Write(dataLength); // 4
            writer.Write(data, 0, data.Length);
            writer.Flush();

            var hash = md5.ComputeHash(stream.GetBuffer());
            return string.Concat(hash.Select(x => x.ToString("x2")));
        }

        public void Read()
        {
            Data = new SHNResult();

            SHNBinaryReader shnReader;

            using (shnReader = new SHNBinaryReader(File.OpenRead(_shnFilePath), _shnEncoding))
            {
                _cryptoHeader = shnReader.ReadBytes(32);
                _dataLength = shnReader.ReadInt32();
                _data = shnReader.ReadBytes(_dataLength - 36);
            }

            _shnCrypto.Decrypt(_data);

            MD5Hash = CalculateMD5Hash(_cryptoHeader, _dataLength, _data);

            shnReader = new SHNBinaryReader(new MemoryStream(_data), _shnEncoding);

            _header = shnReader.ReadUInt32();
            _rowCount = shnReader.ReadUInt32();
            /*_defaultRowLength =*/ shnReader.ReadUInt32();
            _columnCount = shnReader.ReadUInt32();

            var unknownCount = 0;
            var idCount = 0;

            for (uint counter = 0; counter < _columnCount; counter++)
            {
                var name = shnReader.ReadString(48);
                var type = shnReader.ReadUInt32();
                var length = shnReader.ReadInt32();

                if (name.Length == 0 || string.IsNullOrWhiteSpace(name))
                {
                    name = $"Unknown: {unknownCount}";

                    unknownCount++;
                }

                var newSHNColumn = new SHNColumn()
                {
                    ID = idCount,
                    Name = name,
                    Type = type,
                    Length = length
                };

                SHNColumns.Add(newSHNColumn);

                var newDataColumn = new DataColumn()
                {
                    ColumnName = name,
                    DataType = newSHNColumn.GetType()
                };

                Data.Columns.Add(newDataColumn);

                idCount++;
            }

            var values = new object[_columnCount];

            for (uint rowCounter = 0; rowCounter < _rowCount; rowCounter++)
            {
                shnReader.ReadUInt16();

                foreach (var column in SHNColumns)
                {
                    switch (column.Type)
                    {
                        case 1:
                            {
                                values[column.ID] = shnReader.ReadByte();
                                break;
                            }
                        case 2:
                            {
                                values[column.ID] = shnReader.ReadUInt16();
                                break;
                            }
                        case 3:
                            {
                                values[column.ID] = shnReader.ReadUInt32();
                                break;
                            }
                        case 5:
                            {
                                values[column.ID] = shnReader.ReadSingle();
                                break;
                            }
                        case 9:
                            {
                                values[column.ID] = shnReader.ReadString(column.Length);
                                break;
                            }
                        case 11:
                            {
                                values[column.ID] = shnReader.ReadUInt32();
                                break;
                            }
                        case 12:
                            {
                                values[column.ID] = shnReader.ReadByte();
                                break;
                            }
                        case 13:
                            {
                                values[column.ID] = shnReader.ReadInt16();
                                break;
                            }
                        case 0x10:
                            {
                                values[column.ID] = shnReader.ReadByte();
                                break;
                            }
                        case 0x12:
                            {
                                values[column.ID] = shnReader.ReadUInt32();
                                break;
                            }
                        case 20:
                            {
                                values[column.ID] = shnReader.ReadSByte();
                                break;
                            }
                        case 0x15:
                            {
                                values[column.ID] = shnReader.ReadInt16();
                                break;
                            }
                        case 0x16:
                            {
                                values[column.ID] = shnReader.ReadInt32();
                                break;
                            }
                        case 0x18:
                            {
                                values[column.ID] = shnReader.ReadString(column.Length);
                                break;
                            }
                        case 0x1a:
                            {
                                values[column.ID] = shnReader.ReadString();
                                break;
                            }
                        case 0x1b:
                            {
                                values[column.ID] = shnReader.ReadUInt32();
                                break;
                            }
                        case 0x1d:
                            {
                                values[column.ID] = string.Concat(shnReader.ReadUInt32().ToString(), ":", shnReader.ReadUInt32().ToString());
                                break;
                            }
                    }
                }

                Data.Rows.Add(values);
            }

            Data.Count = Data.Rows.Count;
        }

        public void Write(string writePath)
        {
            if (File.Exists(writePath))
            {
                // TODO: backup?
                // File.Delete(writePath);
                // File.Move(writePath, $"C:\\SHNBackups\\{Path.GetFileNameWithoutExtension(writePath)}{DateTime.Now:dd-MM-yyyy.hh-mm-ss-fff}.shn");
            }

            var shnStream = new MemoryStream();
            var shnWriter = new SHNBinaryWriter(shnStream, _shnEncoding);

            shnWriter.Write(_header);
            shnWriter.Write(Data.Rows.Count);
            shnWriter.Write(GetColumnLengths());
            shnWriter.Write(Data.Columns.Count);

            foreach (var column in SHNColumns)
            {
                if (column.Name.StartsWith("Unknown")) { shnWriter.Write(new byte[48]); }
                else { shnWriter.WriteString(column.Name, 48); }

                shnWriter.Write(column.Type);
                shnWriter.Write(column.Length);
            }

            foreach (DataRow row in Data.Rows)
            {
                var position = shnWriter.BaseStream.Position;

                shnWriter.Write((ushort)0);

                foreach (var column in SHNColumns)
                {
                    var columnValue = row.ItemArray[column.ID] ?? "0";

                    switch (column.Type)
                    {
                        case 1:
                            {
                                if (columnValue is string value) { columnValue = byte.Parse(value); }

                                shnWriter.Write((byte)columnValue);

                                break;
                            }
                        case 2:
                            {
                                if (columnValue is string value) { columnValue = ushort.Parse(value); }

                                shnWriter.Write((ushort)columnValue);

                                break;
                            }
                        case 3:
                            {
                                if (columnValue is string value) { columnValue = uint.Parse(value); }

                                shnWriter.Write((uint)columnValue);

                                break;
                            }
                        case 5:
                            {
                                if (columnValue is string value) { columnValue = float.Parse(value); }

                                shnWriter.Write((float)columnValue);

                                break;
                            }
                        case 9:
                            {
                                if (string.IsNullOrWhiteSpace(columnValue.ToString())) { shnWriter.WriteString(columnValue.ToString(), column.Length); }
                                else { shnWriter.WriteString((string)columnValue, column.Length); }

                                break;
                            }
                        case 11:
                            {
                                if (columnValue is string value) { columnValue = uint.Parse(value); }

                                shnWriter.Write((uint)columnValue);

                                break;
                            }
                        case 12:
                            {
                                if (columnValue is string value) { columnValue = byte.Parse(value); }

                                shnWriter.Write((byte)columnValue);

                                break;
                            }
                        case 13:
                            {
                                if (columnValue is string value) { columnValue = short.Parse(value); }

                                shnWriter.Write((short)columnValue);

                                break;
                            }
                        case 0x10:
                            {
                                if (columnValue is string value) { columnValue = byte.Parse(value); }

                                shnWriter.Write((byte)columnValue);

                                break;
                            }
                        case 0x12:
                            {
                                if (columnValue is string value) { columnValue = uint.Parse(value); }

                                shnWriter.Write((uint)columnValue);

                                break;
                            }
                        case 20:
                            {
                                if (columnValue is string value) { columnValue = sbyte.Parse(value); }

                                shnWriter.Write((sbyte)columnValue);

                                break;
                            }
                        case 0x15:
                            {
                                if (columnValue is string value) { columnValue = short.Parse(value); }

                                shnWriter.Write((short)columnValue);

                                break;
                            }
                        case 0x16:
                            {
                                if (columnValue is string value) { columnValue = int.Parse(value); }

                                shnWriter.Write((int)columnValue);

                                break;
                            }
                        case 0x18:
                            {
                                shnWriter.WriteString((string)columnValue, column.Length);

                                break;
                            }
                        case 0x1a:
                            {
                                shnWriter.WriteString((string)columnValue, -1);

                                break;
                            }
                        case 0x1b:
                            {
                                if (columnValue is string value) { columnValue = uint.Parse(value); }

                                shnWriter.Write((uint)columnValue);

                                break;
                            }
                        case 0x1d:
                            {
                                if (!columnValue.ToString().Contains(":")) { break; }

                                var combined = columnValue.ToString().Split(':');

                                shnWriter.Write(uint.Parse(combined[0]));
                                shnWriter.Write(uint.Parse(combined[1]));

                                break;
                            }
                    }

                    var mPosition = shnWriter.BaseStream.Position - position;
                    var start = shnWriter.BaseStream.Position;

                    shnWriter.BaseStream.Seek(position, SeekOrigin.Begin);
                    shnWriter.Write((ushort)mPosition);
                    shnWriter.BaseStream.Seek(start, SeekOrigin.Begin);
                }
            }

            var unencryptedData = shnStream.GetBuffer();
            var encryptedData = new byte[shnStream.Length];

            Array.Copy(unencryptedData, encryptedData, shnStream.Length);

            _shnCrypto.Decrypt(encryptedData);

            shnWriter.Close();
            shnWriter = new SHNBinaryWriter(File.Create(writePath), _shnEncoding);
            shnWriter.Write(_cryptoHeader);
            shnWriter.Write(encryptedData.Length + 36);
            shnWriter.Write(encryptedData);
            shnWriter.Close();

            _shnFilePath = writePath;
        }

        private uint GetColumnLengths()
        {
            uint start = 2;

            foreach (var column in SHNColumns) { start += (uint)column.Length; }

            return start;
        }

        public void DisallowRowChanges() { Data.RowChanged += Table_RowChanged; }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs args) { Data.RejectChanges(); }
    }
}
