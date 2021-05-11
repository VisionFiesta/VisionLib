using System;
using System.Collections.Generic;

namespace Vision.Core
{
	/// <summary>
	/// Class for objects that can be referenced.
	/// </summary>
	public class VisionObject : IDisposable
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name => GetName();

		/// <summary>
		/// Keeps track of the number of all object types.
		/// </summary>
		private static readonly Dictionary<Type, long> TypeCounts = new();

		/// <summary>
		/// An object to lock for TypeCounts access.
		/// </summary>
		private static readonly object TypeCountsLock = new();

		/// <summary>
		/// Creates a new instance of the <see cref="VisionObject"/> class.
		/// </summary>
		public VisionObject()
		{
			var type = GetType();

			EnsureTypeExists(type);

			lock (TypeCountsLock)
			{
				TypeCounts[type]++;
			}
		}

		/// <summary>
		/// This method is called whenever an <see cref="VisionObject"/> is deconstructed.
		/// </summary>
		~VisionObject()
		{
			var type = GetType();

			EnsureTypeExists(type);

			lock (TypeCountsLock)
			{
				TypeCounts[type]--;
			}
		}

		/// <summary>
		/// Destroys the object.
		/// </summary>
		/// <param name="obj">The object to destroy.</param>
		public static void Destroy(VisionObject obj)
		{
			obj?.Dispose();
		}

		/// <summary>
		/// Determines if the object exists.
		/// </summary>
		/// <param name="obj">The object to check.</param>
		public static implicit operator bool(VisionObject obj)
		{
			return obj != null;
		}

		/// <summary>
		/// Ensures that the TypeCounts dictionary contains the type.
		/// If the type is not found, it is added to the dictionary with a count of 0.
		/// </summary>
		/// <param name="type">The type to check for.</param>
		private static void EnsureTypeExists(Type type)
		{
			lock (TypeCountsLock)
			{
				if (!TypeCounts.ContainsKey(type))
				{
					TypeCounts.Add(type, 0);
				}
			}
		}

		/// <summary>
		/// Use the static method VisionObject.Destroy() instead.
		/// </summary>
		public void Dispose()
		{
			Destroy();
		}

		/// <summary>
		/// Gets the name of the object instance. The name is based on how
		/// many instances of the object there are.
		/// </summary>
		/// <returns>The name of the object.</returns>
		public string GetName()
		{
			var type = GetType();

			EnsureTypeExists(type);

			lock (TypeCountsLock)
			{
				return $"{type.Name} {TypeCounts[type]}";
			}
		}

		/// <summary>
		/// Returns the name of the object.
		/// </summary>
		/// <returns>The name of the object.</returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// This method does nothing unless it is overidden by the top-level
		/// implementation of the <see cref="VisionObject"/> class.
		/// </summary>
		protected virtual void Destroy()
		{

		}
	}
}
