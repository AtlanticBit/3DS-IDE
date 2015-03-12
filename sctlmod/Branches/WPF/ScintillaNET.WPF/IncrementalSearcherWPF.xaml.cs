using System;
using System.Drawing;
using System.Windows.Controls;
using System.ComponentModel;

namespace ScintillaNET.WPF
{
	public partial class IncrementalSearcherWPF : UserControl
	{
		private readonly IncrementalSearcher mInnerSearcher = new IncrementalSearcher(true);

		public IncrementalSearcherWPF()
		{
			InitializeComponent();
			formsHost.Child = mInnerSearcher;
		}


        public void MoveFormAwayFromSelection() { mInnerSearcher.MoveFormAwayFromSelection(); }

        /// <summary>
        /// Gets or sets whether the control should automatically move away from the current
        /// selection to prevent obscuring it.
        /// </summary>
        /// <returns>true to automatically move away from the current selection; otherwise, false.</returns>
		[Category("Behavior")]
		[Description("Whether the control should automatically move away from the current selection to prevent obscuring it.")]
		[DefaultValue(true)]
        public bool AutoPosition
		{
			get { return mInnerSearcher.AutoPosition; }
			set { mInnerSearcher.AutoPosition = value; }
		}
		private ScintillaWPF mScintilla;
		[EditorBrowsable(EditorBrowsableState.Never)]
        public ScintillaWPF Scintilla
        {
            get { return mScintilla; }
			set 
			{
				mScintilla = value;
				if (mScintilla != null)
					mInnerSearcher.Scintilla = mScintilla.Scintilla;
				else
					mInnerSearcher.Scintilla = null;
			}
        }
	}
}
