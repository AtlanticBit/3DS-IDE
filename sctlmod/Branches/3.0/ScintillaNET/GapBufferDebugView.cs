#region Using Directives

using System;
using System.Diagnostics;

#endregion Using Directives


namespace ScintillaNet
{
	internal sealed class GapBufferDebugView<T> where T : struct
	{
		#region Fields

		private GapBuffer<T> _buffer;

		#endregion Fields


		#region Properties

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] array = new T[_buffer.Count];
				_buffer.CopyTo(array, 0);
				return array;
			}
		}

		#endregion Properties


		#region Constructors

		public GapBufferDebugView(GapBuffer<T> buffer)
		{
			Debug.Assert(buffer != null);
			_buffer = buffer;
		}

		#endregion Constructors
	}
}
