namespace Vision.Game.Enums
{
    public enum LoginResponse : byte
    {
        DatabaseError = 67, //DB Error.
        AuthFailed = 68, //Authentication Failed.
        CheckIdPw = 69, //Please check ID or Password.
        IdBlocked = 71, //The ID has been blocked.
        WorldMaintenance = 72, //The World Servers are down for maintenance.
        TimeOut = 73, //Authentication timed out. Please try to log in again.
        LoginFailed = 74, //Login failed.
        Agreement = 75, //Please accept the agreement to continue.
    }

	public static class LoginResponseExtensions
	{
		public static string ToMessage(this LoginResponse response)
		{
			switch ((byte)response)
			{
				case 67:
					return "Database Error";
				case 68:
					return "Authentication Failed";
				case 69:
					return "Please check ID or Password";
				case 71:
					return "The ID has been blocked";
				case 72:
					return "The World Servers are down for maintenance";
				case 73:
					return "Authentication timed out";
				case 74:
					return "Login failed";
				case 75:
					return "Please accept the agreement to continue";
				default:
					return "Unknown login error";
			}
		}
	}
}
