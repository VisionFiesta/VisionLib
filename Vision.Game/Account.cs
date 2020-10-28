using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Vision.Core.Logging.Loggers;
using Vision.Game.Characters;
using Vision.Game.Structs.Char;
using Vision.Game.Structs.Common;

namespace Vision.Game
{
    public class Account
    {
        private static readonly EngineLog Logger = new EngineLog(typeof(Account));

        public ushort AccountID;

        public string AccountName;

        private readonly Dictionary<uint, Avatar> _avatarsByCharNo = new Dictionary<uint, Avatar>();
        private readonly ConcurrentDictionary<uint, Character> _charactersByCharNo = new ConcurrentDictionary<uint, Character>();

        private readonly Dictionary<string, object> _characterCreateInfos = new Dictionary<string, object>();

        public IReadOnlyCollection<Avatar> Avatars => _avatarsByCharNo.Values.ToImmutableList();
        public IReadOnlyCollection<Character> Characters => _charactersByCharNo.Values.ToImmutableList();

        public Avatar ActiveAvatar => ActiveCharacter != null ? _avatarsByCharNo.GetValueOrDefault(ActiveCharacter.CharNo) : null;
        public Character ActiveCharacter { get; private set; }

        public bool AddAvatar(Avatar avatar)
        {
            return !_avatarsByCharNo.ContainsKey(avatar.CharNo) && _avatarsByCharNo.TryAdd(avatar.CharNo, avatar);
        }

        public bool AddCharacter(ushort handle, uint charNo)
        {
            var ava = _avatarsByCharNo.GetValueOrDefault(charNo);
            if (ava != null)
            {
                var newChar = new Character(handle, _avatarsByCharNo.GetValueOrDefault(charNo));
                return _charactersByCharNo.TryAdd(charNo, newChar);
            }
            else
            {
                Logger.Error("No avatar for supplied charNo!");
                return false;
            }

            // TODO: add OptionData, ParameterData
        }

        public bool SelectCharacter(uint charNo)
        {
            if (!_charactersByCharNo.TryGetValue(charNo, out var character)) return false;
            ActiveCharacter = character;
            return true;
        }
    }
}
