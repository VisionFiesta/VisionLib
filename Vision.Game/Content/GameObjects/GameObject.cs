﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vision.Core.Collections;
using Vision.Core.Extensions;
using Vision.Game.Characters;
using Vision.Game.Structs.Common;

namespace Vision.Game.Content.GameObjects
{
    internal class GameObjectHandleEqualityComparer : IEqualityComparer<GameObject>
    {
        public bool Equals(GameObject x, GameObject y) => x.Handle == y.Handle;
        public int GetHashCode(GameObject obj) => obj.Handle.GetHashCode();
    }

	public abstract class GameObject
    {
        private static readonly HashSet<GameObject> objects = new HashSet<GameObject>(new GameObjectHandleEqualityComparer());
        public static IReadOnlyCollection<GameObject> Objects => objects;

		public ushort Handle { get; protected set; }

		public bool IsDead { get; set; }
		public byte Level { get; set; }
		public GameObjectType Type { get; set; }
		public GameObjectState State { get; set; } = GameObjectState.GOS_NONBATTLE;
		public Stats Stats { get; set; }
		public ShineXYR Position { get; set; }
		// public MoverInstance Mount { get; set; } TODO
		// public string MapIndx => Position?.Map?.Info.MapName; TODO: Maps

		public FastList<GameObject> VisibleObjects { get; set; }
		public List<Character> VisibleCharacters => VisibleObjects.OfType<Character>().ToList();
        public List<GameObject> TouchingObjects => VisibleObjects.Filter(obj => obj.Position.GetDistance(Position) <= 10.0);

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

		public bool HasUpdatedMountSpeed { get; set; }

		public List<GameObject> SelectedBy { get; set; }

		public ushort HPChangeOrder => _hpChangeOrder++;

		private ushort _hpChangeOrder;

		// public Behavior Behavior { get; set; }

		public bool IsWalking { get; set; }
		public bool IsActive { get; set; }
		public bool IsMoving { get; set; }

		protected GameObject()
		{
			Stats = new Stats(this);
			Position = new ShineXYR();

			VisibleObjects = new FastList<GameObject>();

			// Behavior = new DefaultBehavior();

            if (!objects.Add(this))
            {
				throw new InvalidOperationException($"GameObject with handle {Handle} already exists!");
            }
		}
	}
}
