namespace Vision.Core.IO.SHN
{
    public class SHNCrypto : ISHNCrypto
    {
        public void Calculate(byte[] data)
        {
            var dataLen = (byte)data.Length;
            for (var i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(data[i] ^ dataLen);

                var dl = (byte) i;

                dl = (byte) (dl & 15);
                dl = (byte) (dl + 85);
                dl = (byte) (dl ^ (byte) ((byte) i * 11));
                dl = (byte) (dl ^ dataLen);
                dl = (byte) (dl ^ 170);

                dataLen = dl;
            }
        }
    }
}
