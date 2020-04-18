namespace VisionLib.Common.Networking
{
    public interface IFiestaNetCrypto
    {
        void SetSeed(ushort seed);

        ushort GetSeed();

        bool WasSeedSet();

        void XorBuffer(byte[] buffer, int offset, int length);
    }
}
