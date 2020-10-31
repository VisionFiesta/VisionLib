using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Vision.Core.Enums;

namespace Vision.Game.Enums
{
    public enum ChatCommandType
    {
        CCT_WHISPER_CHAT,
        CCT_WHISPER_REPLY,
        CCT_SHOUT_CHAT,
        CCT_PARTY_CHAT,
        CCT_PARTY_INVITE,
        CCT_ACADEMY_CHAT,
        CCT_GUILD_CHAT,
        CCT_EXPEDITION_NOTICE_CHAT,
        CCT_EXPEDITION_CHAT
    }

    public static class ChatCommandTypeExtensions
    {
        public static ClientLogChatType ToClientLogChatType(this ChatCommandType type)
        {
            return type switch
            {
                ChatCommandType.CCT_WHISPER_CHAT => ClientLogChatType.CLCT_WHISPER,
                ChatCommandType.CCT_WHISPER_REPLY => ClientLogChatType.CLCT_WHISPER,
                ChatCommandType.CCT_SHOUT_CHAT => ClientLogChatType.CLCT_SHOUT,
                ChatCommandType.CCT_PARTY_CHAT => ClientLogChatType.CLCT_PARTY,
                ChatCommandType.CCT_PARTY_INVITE => ClientLogChatType.CLCT_PARTY,
                ChatCommandType.CCT_ACADEMY_CHAT => ClientLogChatType.CLCT_ACADEMY,
                ChatCommandType.CCT_GUILD_CHAT => ClientLogChatType.CLCT_GUILD,
                ChatCommandType.CCT_EXPEDITION_NOTICE_CHAT => ClientLogChatType.CLCT_EXPEDITION,
                ChatCommandType.CCT_EXPEDITION_CHAT => ClientLogChatType.CLCT_EXPEDITION,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static readonly ImmutableDictionary<ChatCommandType?, string> ChatCommandPrefixesByCommand = ImmutableDictionary.CreateRange(new Dictionary<ChatCommandType?, string>
        {
            {ChatCommandType.CCT_WHISPER_CHAT, "w"},
            {ChatCommandType.CCT_WHISPER_REPLY, "r"},
            {ChatCommandType.CCT_PARTY_CHAT, "p"},
            {ChatCommandType.CCT_PARTY_INVITE, "invite"},
            {ChatCommandType.CCT_SHOUT_CHAT, "s"},
            {ChatCommandType.CCT_ACADEMY_CHAT, "a"},
            {ChatCommandType.CCT_GUILD_CHAT, "g"},
            {ChatCommandType.CCT_EXPEDITION_NOTICE_CHAT, "n"},
            {ChatCommandType.CCT_EXPEDITION_CHAT, "f" }
        });

        public static readonly ImmutableDictionary<string, ChatCommandType?> ChatCommandsByPrefix = 
            ImmutableDictionary.CreateRange(ChatCommandPrefixesByCommand.ToDictionary(p => p.Value, p => p.Key));

        public static bool IsChatCommand(this ChatCommandType type) => type != ChatCommandType.CCT_PARTY_INVITE;

        public static bool IsOtherCommand(this ChatCommandType type) => !IsChatCommand(type);

        public static string GetCommandPrefix(this ChatCommandType type) => ChatCommandPrefixesByCommand.GetValueOrDefault(type, null);

        public static ChatCommandType? GetCommandFromPrefix(string prefix) =>
            ChatCommandsByPrefix.GetValueOrDefault(prefix, null);
    }
}
