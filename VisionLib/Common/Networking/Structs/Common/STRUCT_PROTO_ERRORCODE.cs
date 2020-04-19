namespace VisionLib.Common.Networking.Structs.Common
{
    public class STRUCT_PROTO_ERRORCODE
    {
        public readonly PROTO_ERRORCODE ErrorCode;

        public STRUCT_PROTO_ERRORCODE(PROTO_ERRORCODE errorCode)
        {
            ErrorCode = errorCode;
        }

        public STRUCT_PROTO_ERRORCODE(FiestaNetPacket packet) : this((PROTO_ERRORCODE)packet.ReadUInt16()) {}

        public enum PROTO_ERRORCODE : ushort
        {
            NONE = 0,                        // NONE
            UNKNOWN_ERROR = 66,                // An unknown error has occurred
            DB_ERROR_1 = 67,                    // DB error
            AUTHENTICATION_FAILED = 68,        // Authentication failed.
            CHECK_ID_OR_PASS = 69,            // Please check ID or Password.
            DB_ERROR_2 = 70,                    // DB error
            ID_BLOCKED = 71,                    // The ID has been blocked.
            WORLD_IS_MAINTENANCE = 72,        // The World Servers are down for maintenance.
            AUTH_TIMED_OUT = 73,                // Authentication timed out. Please try to log in again.
            LOGIN_FAILED = 74,                // Login failed
            AGREEMENT_MISSING = 75,            // Please accept the agreement to continue.
            WRONG_REGION = 81,                // You are not in our service area.

            FAILED_TO_CREATE = 130,
            WRONG_CLASS = 131,
            NAME_TAKEN = 132,
            ERROR_IN_MAX_SLOT = 133,
            
            NAME_IN_USE = 385,

            FAILED_WORLDSERVER = 321,
            FAILED_ZONESERVER = 322,
            NOCHAR_INSLOT = 324,
            CLIENT_MANIPULATED = 327,
            ERROR_CLIENTDATA = 328,
            ERROR_CHAR_INFO = 1410,
            ERROR_APPEARANCE = 1411,
            ERROR_OPTIONS = 1412,
        }
    }
}
