#region Using Directives

using System;
using System.Diagnostics;

#endregion Using Directives


namespace ScintillaNet
{
	// See "Generic Gap Buffer" article on CodeProject by yours truly for more info:
	// http://www.codeproject.com/KB/recipes/GenericGapBuffer.aspx.
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(GapBufferDebugView<>))]
	internal sealed class GapBuffer<T> where T : struct
	{
		#region Constants

		private const int MIN_CAPACITY = 4;

		#endregion Constants


		#region Fields

		private T[] _buffer;
		private int _gapStart;
		private int _gapEnd;

		#endregion Fields


		#region Methods

		public void CopyTo(T[] array, int arrayIndex)
		{
			Debug.Assert(array != null);
			Debug.Assert(array.Rank == 1);
			Debug.Assert(arrayIndex >= 0 && arrayIndex + Count <= array.Length);

			// Copy the spans into the destination array at the offset
			Array.Copy(_buffer, 0, array, arrayIndex, _gapStart);
			Array.Copy(_buffer, _gapEnd, array, arrayIndex + _gapStart, _buffer.Length - _gapEnd);
		}

		private void EnsureGapCapacity(int required)
		{
			// Is the available space in the gap?
			if (required > (_gapEnd - _gapStart))
			{
				// Calculate a new size (double the size necessary)
				int newCapacity = (Count + required) * 2;
				if (newCapacity < MIN_CAPACITY)
					newCapacity = MIN_CAPACITY;

				// Allocate a new buffer
				T[] newBuffer = new T[newCapacity];
				int newGapEnd = newBuffer.Length - (_buffer.Length - _gapEnd);

				// Copy the spans into the front and back of the new buffer
				Array.Copy(_buffer, 0, newBuffer, 0, _gapStart);
				Array.Copy(_buffer, _gapEnd, newBuffer, newGapEnd, (newBuffer.Length - newGapEnd));
				_buffer = newBuffer;
				_gapEnd = newGapEnd;
			}
		}


		public void Insert(int index, T item)
		{
			Debug.Assert(index >= 0 && index <= Count);

			// Prepare the buffer
			PlaceGapStart(index);
			EnsureGapCapacity(1);

			_buffer[index] = item;
			_gapStart++;
		}


		private void PlaceGapStart(int index)
		{
			// Are we already there?
			if (index == _gapStart)
				return;

			// Is there even a gap?
			if ((_gapEnd - _gapStart) == 0)
			{
				_gapStart = index;
				_gapEnd = index;
				return;
			}

			// Which direction do we move the gap?
			if (index < _gapStart)
			{
				// Move the gap near (by copying the items at the beginning
				// of the gap to the end)
				int count = _gapStart - index;
				int deltaCount = ((_gapEnd - _gapStart) < count ? (_gapEnd - _gapStart) : count);
				Array.Copy(_buffer, index, _buffer, (_gapEnd - count), count);
				_gapStart -= count;
				_gapEnd -= count;
			}
			else
			{
				// Move the gap far (by copying the items at the end
				// of the gap to the beginning)
				int count = index - _gapStart;
				int deltaIndex = (index > _gapEnd ? index : _gapEnd);
				Array.Copy(_buffer, _gapEnd, _buffer, _gapStart, count);
				_gapStart += count;
				_gapEnd += count;
			}
		}


		public void RemoveAt(int index)
		{
			Debug.Assert(index >= 0 && index < Count);

			// Place the gap at the index and increase the gap size by 1
			PlaceGapStart(index);
			_gapEnd++;
		}

		#endregion Methods


		#region Properties

		public int Capacity
		{
			get { return _buffer.Length; }
		}


		public int Count
		{
			get { return _buffer.Length - (_gapEnd - _gapStart); }
		}

		#endregion Properties


		#region Indexers

		public T this[int index]
		{
			get
			{
				Debug.Assert(index >= 0 && index < Count);

				// Find the correct span and get the item
				if (index >= _gapStart)
					index += (_gapEnd - _gapStart);

				return _buffer[index];
			}
			set
			{
				Debug.Assert(index >= 0 && index < Count);

				// Find the correct span and set the item
				if (index >= _gapStart)
					index += (_gapEnd - _gapStart);

				_buffer[index] = value;
			}
		}

		#endregion Indexers


		#region Constructors

		public GapBuffer()
		{
			_buffer = new T[MIN_CAPACITY];
			_gapEnd = _buffer.Length;
		}

		#endregion Constructors
	}
}
