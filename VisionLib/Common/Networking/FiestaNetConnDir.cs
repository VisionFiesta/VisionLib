namespace VisionLib.Common.Networking
{
    public enum FiestaNetConnDir
    {
        FNCDIR_TO_SERVER,
        FNCDIR_TO_CLIENT
    }

    public static class FiestaNetworkConnectionDirectionExtensions
    {
        public static bool IsFromServer(this FiestaNetConnDir dir)
        {
            return dir != FiestaNetConnDir.FNCDIR_TO_CLIENT;
        }

        public static bool IsFromClient(this FiestaNetConnDir dir)
        {
            return dir == FiestaNetConnDir.FNCDIR_TO_SERVER;
        }
    }
}
