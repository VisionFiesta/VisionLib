﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Vision.Core;
using Vision.Core.Collections;
using Vision.Core.Extensions;
using Vision.Game.Characters;
using Vision.Game.Structs.Common;

namespace Vision.Game.Content.GameObjects
{
    internal class GameObjectHandleEqualityComparer : IEqualityComparer<GameObject>
    {
        public bool Equals(GameObject x, GameObject y) => x?.Handle == y?.Handle;

        public int GetHashCode(GameObject obj) => obj.Handle.GetHashCode();
    }

	public abstract class GameObject : VisionObject
    {

		public ushort Handle { get; }
        public byte Level { get; set; }
		public GameObjectType Type { get; protected set; }
        public GameObjectState State { get; set; } = GameObjectState.GOS_NONBATTLE;
		public GameObjectStats Stats { get; set; }
        public string MapName { get; set; }
		public ShineXYR Position { get; set; }

        public HashSet<GameObject> VisibleObjects { get; private set; } = new(new GameObjectHandleEqualityComparer());
        public IReadOnlyCollection<Character> VisibleCharacters => VisibleObjects.OfType<Character>().ToImmutableList();
        public IReadOnlyCollection<GameObject> TouchingObjects => VisibleObjects.Filter(obj => obj.Position.GetDistance(Position) <= 10.0).ToImmutableList();

		private GameObject _target;
		public GameObject Target
		{
			get => _target;
			set
			{
				if (value != null && !value.SelectedBy.Contains(this))
					value.SelectedBy.Add(this);
				_target = value;
			}
		}

		public void ToSelectedBy(Action<GameObject> action, bool onlyCharacters = false)
		{
			for (var upperBound = SelectedBy.GetUpperBound(); upperBound >= 0; --upperBound)
			{
				try
				{
					var gameObject = SelectedBy[upperBound];
					if (gameObject.Target != this) continue;
					if (onlyCharacters)
					{
						if (!(gameObject is Character))
							continue;
					}
					action(gameObject);
				}
				catch
				{
					// ignored
				}
			}
		}

        public List<GameObject> SelectedBy { get; private set; } = new();
        public ushort HPChangeOrder => _hpChangeOrder++;
        private ushort _hpChangeOrder;

        protected GameObject(ushort handle, GameObjectType type)
        {
            Handle = handle;
            Type = type;

			Stats = new GameObjectStats(this);
			Stats.Update();

			Position = new ShineXYR();
		}

        public override string ToString()
        {
            return this switch
            {
                Character chr => chr.ToString(),
                Mob mob => mob.ToString(),
                Mover mover => mover.ToString(),
                _ => $"Handle: {Handle}"
            };
        }

        protected override void Destroy()
        {
            foreach (var go in VisibleObjects) go.Dispose();
			VisibleObjects.Clear();
            VisibleObjects = null;
            foreach (var go in SelectedBy) go.Dispose();
			SelectedBy.Clear();
            SelectedBy = null;
        }
    }
}
