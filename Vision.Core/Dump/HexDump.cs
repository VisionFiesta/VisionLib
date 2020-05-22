using System;
using System.IO;
using System.Text;
using Vision.Core.Extensions;

namespace Vision.Core.Dump
{
    public static class HexDump
    {
        private static readonly string NewLine = Environment.NewLine;
        private static readonly int NewLineLength = NewLine.Length;

        private static readonly char[] SpaceChars =
        {
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '
        };

        public static readonly char[] HexDigits =
        {
            '0', '1', '2', '3', '4', '5',
            '6', '7', '8', '9', 'A', 'B',
            'C', 'D', 'E', 'F'
        };

        public static void ToHexChars(uint val, char[] dst, uint dstIndex, uint size)
        {
            while (size > 0)
            {
                var i = dstIndex + size - 1;

                if (i < dst.Length)
                {
                    dst[i] = HexDigits[val & 0x000F];
                }

                if (val != 0)
                {
                    val >>= 4;
                }

                size--;
            }
        }

        public static void ToHexChars(ulong val, char[] dst, uint dstIndex, uint size)
        {
            while (size > 0)
            {
                dst[dstIndex + size - 1] = HexDigits[(uint) val & 0x000FL];

                if (val != 0)
                {
                    val >>= 4;
                }

                size--;
            }
        }

        public static void Dump(Stream stream, byte[] src, int srcIndex, int length)
        {
            switch (length)
            {
                case 0:
                    return;
                case -1:
                {
                    length = src.Length - srcIndex;

                    if (length < 1) return;
                    break;
                }
            }

            var s = length % 16;
            var r = length / 16 + (s == 0 ? 0 : 1);

            var c = new char[r * (74 + NewLineLength)];
            var d = new char[16];

            uint si = 0;
            uint ci = 0;

            do
            {
                ToHexChars(si, c, ci, 5);
                ci += 5;
                c[ci++] = ':';

                do
                {
                    if (si == length)
                    {
                        var n = 16 - s;
                        Array.Copy(SpaceChars, 0, c, ci, n * 3);
                        ci += (uint)(n * 3);
                        Array.Copy(SpaceChars, 0, d, s, n);
                        break;
                    }

                    c[ci++] = ' ';
                    var i = (uint)(src[srcIndex + si] & 0xFF);

                    ToHexChars(i, c, ci, 2);

                    ci += 2;

                    if (char.IsControl((char)i))
                    {
                        d[si % 16] = '.';
                    }
                    else
                    {
                        d[si % 16] = (char)i;
                    }
                } while ((++si % 16) != 0);

                c[ci++] = ' ';
                c[ci++] = ' ';
                c[ci++] = '|';

                Array.Copy(d, 0, c, ci, 16);

                ci += 16;
                c[ci++] = '|';



                NewLine.GetChars(0, NewLineLength, c, (int)ci);

                ci += (uint)NewLineLength;
            } while (si < length);

            var str = new string(c);
            using var sw = new StreamWriter(stream, Encoding.UTF8, str.Length, true);
            sw.Write(str);
            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        public static void Dump(Stream s, byte[] src)
        {
            Dump(s, src, 0, -1);
        }

        public static string Dump(byte[] buffer)
        {
            using var stream = new MemoryStream();
            using var reader = new StreamReader(stream);
            Dump(stream, buffer);
            var str = reader.ReadToEnd();
            return str;
        }
    }
}
