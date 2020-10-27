﻿namespace Vision.Client.Data
{
    public class StaticClientData
    {
        public string ShinePath { get; set; } = @"D:\Gamigo\Fiesta\ressystem";

        // MD5 Hash of Bin (32bytes) + some other data (32bytes)
        public byte[] VersionKey { get; protected set; } =
        {
            // 1.02.276
            // 0x35, 0x61, 0x31, 0x61, 0x37, 0x62, 0x35, 0x30, 0x34, 0x63, 0x64, 0x35, 0x63, 0x37, 0x33, 0x61,
            // 0x61, 0x36, 0x39, 0x62, 0x64, 0x38, 0x31, 0x37, 0x32, 0x61, 0x64, 0x31, 0x32, 0x63, 0x36, 0x39,
            // 0x00, 0x50, 0x27, 0x01, 0xB0, 0xAC, 0x15, 0x09, 0x30, 0xAD, 0x45, 0x75, 0x8C, 0xF9, 0xF5, 0x00,
            // 0x45, 0x1E, 0x8A, 0x00, 0x2C, 0x7F, 0x21, 0x09, 0x31, 0x7F, 0x21, 0x09, 0x00, 0x00, 0x00, 0x00

            // 1.02.277
            // 0x31, 0x65, 0x37, 0x38, 0x36, 0x32, 0x39, 0x31, 0x32, 0x34, 0x35, 0x37, 0x65, 0x37, 0x63, 0x66,
            // 0x34, 0x62, 0x36, 0x37, 0x33, 0x34, 0x30, 0x34, 0x39, 0x61, 0x34, 0x35, 0x34, 0x63, 0x36, 0x30,
            // 0x00, 0xC7, 0xB6, 0x00, 0x60, 0xBC, 0x79, 0x07, 0x90, 0xAD, 0x70, 0x75, 0x54, 0xF9, 0x93, 0x00,
            // 0x55, 0x2D, 0x1F, 0x01, 0x2C, 0x85, 0x6F, 0x09, 0x31, 0x85, 0x6F, 0x09, 0x00, 0x00, 0x00, 0x00

            // 1.02.287
            0x64, 0x38, 0x31, 0x37, 0x65, 0x36, 0x37, 0x32, 0x61, 0x62, 0x34, 0x32, 0x63, 0x30, 0x63, 0x66,
            0x62, 0x66, 0x35, 0x36, 0x66, 0x65, 0x35, 0x32, 0x32, 0x65, 0x64, 0x31, 0x31, 0x62, 0x32, 0x37,
            0x00, 0xAE, 0x0D, 0x76, 0x58, 0xAA, 0xE3, 0x02, 0x00, 0xAE, 0x0D, 0x76, 0x10, 0xFC, 0xCF, 0x00,
            0x65, 0x80, 0x5C, 0x00, 0xC4, 0x93, 0xEF, 0x02, 0xC9, 0x93, 0xEF, 0x02, 0x00, 0x00, 0x00, 0x00
        };

        // Latest as of NA 1.02.287
        public byte[] XTrapVersionHash { get; protected set; } =
        {
            0x33, 0x33, 0x42, 0x35, 0x34, 0x33, 0x42, 0x30,
            0x43, 0x41, 0x36, 0x45, 0x37, 0x43, 0x34, 0x31,
            0x45, 0x35, 0x44, 0x31, 0x44, 0x30, 0x36, 0x35,
            0x31, 0x33, 0x30, 0x37, 0x00
        };
    }
}
