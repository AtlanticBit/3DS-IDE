using System;
using System.Drawing;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class LongLines : ScintillaWPFConfigItem
	{
		private Color? mEdgeColor;
		public Color EdgeColor
		{
			get { return mEdgeColor.Value; }
			set { mEdgeColor = value; TryApplyConfig(); }
		}

		private int? mEdgeColumn;
		public int EdgeColumn
		{
			get { return mEdgeColumn.Value; }
			set { mEdgeColumn = value; TryApplyConfig(); }
		}

		private EdgeMode? mEdgeMode;
		public EdgeMode EdgeMode
		{
			get { return mEdgeMode.Value; }
			set { mEdgeMode = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mEdgeColor != null)
				scintilla.LongLines.EdgeColor = EdgeColor;
			if (mEdgeColumn != null)
				scintilla.LongLines.EdgeColumn = EdgeColumn;
			if (mEdgeMode != null)
				scintilla.LongLines.EdgeMode = EdgeMode;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
