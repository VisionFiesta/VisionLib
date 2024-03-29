﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Vision.Core.Logging.Loggers;
using Vision.Game.Characters;

namespace Vision.Game
{
    public class Account
    {
        private static EngineLog Logger { get; } = new(typeof(Account));

        public ushort AccountId;

        public string AccountName;

        private readonly Dictionary<uint, WorldCharacter> _avatarsByCharNo = new();
        private readonly ConcurrentDictionary<uint, Character> _charactersByCharNo = new();

        public IEnumerable<WorldCharacter> Avatars => _avatarsByCharNo.Values.ToImmutableList();
        public IEnumerable<Character> Characters => _charactersByCharNo.Values.ToImmutableList();

        public WorldCharacter ActiveAvatar { get; private set; }
        public Character ActiveCharacter { get; private set; }

        public bool AddAvatar(WorldCharacter avatar)
        {
            return !_avatarsByCharNo.ContainsKey(avatar.CharNo) && _avatarsByCharNo.TryAdd(avatar.CharNo, avatar);
        }

        public bool AddCharacter(ushort handle, uint charNo)
        {
            var ava = _avatarsByCharNo.GetValueOrDefault(charNo);
            if (ava == null) return false;

            var newChar = new Character(handle, _avatarsByCharNo.GetValueOrDefault(charNo));
            return _charactersByCharNo.TryAdd(charNo, newChar);

            // TODO: add OptionData, ParameterData
        }

        public bool SelectAvatar(uint charNo)
        {
            if (!_avatarsByCharNo.TryGetValue(charNo, out var avatar)) return false;
            ActiveAvatar = avatar;
            return true;
        }

        public bool SelectCharacter(uint charNo)
        {
            if (ActiveAvatar == null) return false;
            if (!_charactersByCharNo.TryGetValue(charNo, out var character)) return false;
            ActiveCharacter = character;
            return true;
        }
    }
}
