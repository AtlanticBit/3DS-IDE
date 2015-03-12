#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

#endregion Using Directives


namespace ScintillaNet.Design
{
	internal partial class StyleBuilder : Form
	{
		#region Constants

		private const string PREVIEW_TEXT = "The quick brown fox jumps over the lazy dog.";

		#endregion Constants


		#region Fields

		private Style _style = null;

		#endregion Fields


		#region Methods

		private void _okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		#endregion Methods


		#region Properties

		public Style Style
		{
			get
			{
				return _style;
			}
		}

		#endregion Properties


		#region Constructors

		public StyleBuilder()
		{
			InitializeComponent();

			/*
			// The preview text will use this style
			_style = new Style(0, _previewScintilla);
			_propertyGrid.SelectedObject = _style;

			// Create the preview text from some assembly data
			using (StringWriter sw = new StringWriter())
			{
				sw.WriteLine(Util.Title);

				sw.WriteLine(Util.Description);

				sw.Write("Version ");
				sw.WriteLine(Util.Version);

				sw.WriteLine(Util.Copyright);

				sw.Write(PREVIEW_TEXT);

				_previewScintilla.Text = sw.ToString();
			}
			*/

			_previewScintilla.ReadOnly = true;
		}

		#endregion Constructors
	}
}
