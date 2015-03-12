using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class DropMarkers : ScintillaWPFConfigItem
	{
		private string mSharedStackName;
		/// <summary>
		/// Gets/Sets a shared name associated with other Scintilla controls. 
		/// </summary>
		/// <remarks>
		/// All Scintilla controls with the same SharedStackName share a common
		/// DropMarker stack. This is useful in MDI applications where you want
		/// the DropMarker stack not to be specific to one document.
		/// </remarks>
		public string SharedStackName 
		{
			get { return mSharedStackName; }
			set { mSharedStackName = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (this.SharedStackName != null)
				scintilla.DropMarkers.SharedStackName = SharedStackName;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
			scintilla.DropMarkers.SharedStackName = null;
		}
	}
}
