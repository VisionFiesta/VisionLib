namespace Vision.Core.Networking.Crypto
{
    public interface INetCrypto
    {
        void SetSeed(ushort seed);

        ushort GetSeed();

        bool WasSeedSet();

        void XorBuffer(byte[] buffer, int offset, int length);
    }
}
