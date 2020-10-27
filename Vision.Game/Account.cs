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

        private readonly ConcurrentDictionary<uint, Avatar> _avatarsByCharNo = new ConcurrentDictionary<uint, Avatar>();
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

        public bool AddCharacter(ushort handle, NcCharClientBaseCmd data)
        {
            var newChar = new Character(handle, data);

            // TODO: add OptionData, ParameterData

            return _charactersByCharNo.TryAdd(data.CharNo, newChar);
        }

        public bool SelectCharacter(uint charNo)
        {
            if (!_charactersByCharNo.TryGetValue(charNo, out var character)) return false;
            ActiveCharacter = character;
            return true;
        }
    }
}
