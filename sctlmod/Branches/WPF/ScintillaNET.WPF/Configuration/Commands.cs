using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class Commands : ScintillaWPFConfigItem
	{
		private bool? mAllowDuplicateBindings;
		/// <summary>
		/// Gets/Sets if a key combination can be bound to more than one command. (default is true)
		/// </summary>
		/// <remarks>
		/// When set to false only the first command bound to a key combination is kept.
		/// Subsequent requests are ignored. 
		/// </remarks>
		[DefaultValue(true)]
		public bool AllowDuplicateBindings
		{
			get { return mAllowDuplicateBindings.Value; }
			set { mAllowDuplicateBindings = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mAllowDuplicateBindings != null)
				scintilla.Commands.AllowDuplicateBindings = AllowDuplicateBindings;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
			scintilla.Commands.RemoveAllBindings();
			scintilla.Commands.AllowDuplicateBindings = true;
		}
	}
}
