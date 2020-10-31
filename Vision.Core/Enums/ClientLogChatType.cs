using System.Drawing;

namespace Vision.Core.Enums
{
    public enum ClientLogChatType
    {
        CLCT_NORMAL,
        CLCT_SHOUT,
        CLCT_WHISPER,
        CLCT_PARTY,
        CLCT_ACADEMY,
        CLCT_GUILD,
        CLCT_ROAR,
        CLCT_EXPEDITION,
        CLCT_ANNOUNCE // TODO: move announce out of Chat
    }

    public static class ClientChatTypeExtensions
    {
        public static Color ToColor(this ClientLogChatType level)
        {
            return level switch
            {
                ClientLogChatType.CLCT_NORMAL => Color.White,
                ClientLogChatType.CLCT_SHOUT => FiestaColors.ChatShoutColor,
                ClientLogChatType.CLCT_WHISPER => FiestaColors.ChatWhisperColor,
                ClientLogChatType.CLCT_PARTY => FiestaColors.ChatPartyColor,
                ClientLogChatType.CLCT_ACADEMY => FiestaColors.ChatAcademyColor,
                ClientLogChatType.CLCT_GUILD => FiestaColors.ChatGuildColor,
                ClientLogChatType.CLCT_ROAR => FiestaColors.ChatRoarColor,
                ClientLogChatType.CLCT_ANNOUNCE => FiestaColors.ChatGuildColor,
                _ => Color.White
            };
        }

        public static string ToName(this ClientLogChatType level)
        {
            return level switch
            {
                ClientLogChatType.CLCT_NORMAL => "Normal",
                ClientLogChatType.CLCT_SHOUT => "Shout",
                ClientLogChatType.CLCT_WHISPER => "Whisper",
                ClientLogChatType.CLCT_PARTY => "Party",
                ClientLogChatType.CLCT_ACADEMY => "Academy",
                ClientLogChatType.CLCT_GUILD => "Guild",
                ClientLogChatType.CLCT_ROAR => "Roar",
                ClientLogChatType.CLCT_ANNOUNCE => "Announce",
                _ => "Unk"
            };
        }
    }
}
