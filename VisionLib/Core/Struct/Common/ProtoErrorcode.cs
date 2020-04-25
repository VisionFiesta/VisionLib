using System;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class ProtoErrorcode : AbstractStruct
    {
        public PROTO_ERRORCODE ErrorCode;

        public ProtoErrorcode()
        {
            ErrorCode = PROTO_ERRORCODE.EC_NONE;
        }

        public ProtoErrorcode(PROTO_ERRORCODE errorCode)
        {
            ErrorCode = errorCode;
        }

        public enum PROTO_ERRORCODE : ushort
        {
            EC_NONE = 0,                        // NONE
            EC_UNKNOWN_ERROR = 66,                // An unknown error has occurred
            EC_DB_ERROR_1 = 67,                    // DB error
            EC_AUTHENTICATION_FAILED = 68,        // Authentication failed.
            EC_CHECK_ID_OR_PASS = 69,            // Please check ID or Password.
            EC_DB_ERROR_2 = 70,                    // DB error
            EC_ID_BLOCKED = 71,                    // The ID has been blocked.
            EC_WORLD_IS_MAINTENANCE = 72,        // The World Servers are down for maintenance.
            EC_AUTH_TIMED_OUT = 73,                // Authentication timed out. Please try to log in again.
            EC_LOGIN_FAILED = 74,                // Login failed
            EC_AGREEMENT_MISSING = 75,            // Please accept the agreement to continue.
            EC_WRONG_REGION = 81,                // You are not in our service area.
            EC_FAILED_TO_CREATE = 130,
            EC_WRONG_CLASS = 131,
            EC_NAME_TAKEN = 132,
            EC_ERROR_IN_MAX_SLOT = 133,
            EC_NAME_IN_USE = 385,
            EC_FAILED_WORLDSERVER = 321,
            EC_FAILED_ZONESERVER = 322,
            EC_NOCHAR_INSLOT = 324,
            EC_CLIENT_MANIPULATED = 327,
            EC_ERROR_CLIENTDATA = 328,
            EC_ERROR_CHAR_INFO = 1410,
            EC_ERROR_APPEARANCE = 1411,
            EC_ERROR_OPTIONS = 1412,
            EC_MAXVALUE = 1413
        }

        public override int GetSize()
        {
            return sizeof(PROTO_ERRORCODE);
        }

        public override void Read(ReaderStream reader)
        {
            var raw = reader.ReadUInt16();
            if (Enum.IsDefined(typeof(PROTO_ERRORCODE), raw))
            {
                ErrorCode = (PROTO_ERRORCODE) raw;
            }
            else
            {
                ErrorCode = PROTO_ERRORCODE.EC_UNKNOWN_ERROR;
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write((ushort)ErrorCode);
        }
    }
}
