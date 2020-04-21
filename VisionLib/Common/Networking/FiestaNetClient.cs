namespace VisionLib.Common.Networking
{
    public class FiestaNetClient
    {
        public FiestaNetConnection LoginClient = new FiestaNetConnection(FiestaNetConnDest.FNCDEST_LOGIN, FiestaNetConnDir.FNCDIR_TO_SERVER);
        public FiestaNetConnection WorldClient = new FiestaNetConnection(FiestaNetConnDest.FNCDEST_WORLDMANAGER, FiestaNetConnDir.FNCDIR_TO_SERVER);
        public FiestaNetConnection ZoneClient = new FiestaNetConnection(FiestaNetConnDest.FNCDEST_ZONE, FiestaNetConnDir.FNCDIR_TO_SERVER);
    }
}
