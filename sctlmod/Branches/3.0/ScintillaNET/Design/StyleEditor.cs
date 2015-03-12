#region Using Directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

#endregion Using Directives


namespace ScintillaNet.Design
{
	internal class StyleEditor : UITypeEditor
	{
		#region Methods

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (editorService != null)
				{
					// Display the dialog editor
					using (StyleBuilder styleBuilder = new StyleBuilder())
					{
						IUIService uiService = (IUIService)provider.GetService(typeof(IUIService));
						if (uiService != null)
							styleBuilder.Font = (Font)uiService.Styles["DialogFont"];

						// We create a new style with the same properties as the style we're editing
						// so it can be used on the Scintilla control in the style builder. Then we
						// copy the properties back to the source style.
						Style style = value as Style;
						/*
						style.CopyTo(styleBuilder.Style);
						if (styleBuilder.ShowDialog() == DialogResult.OK)
							styleBuilder.Style.CopyTo(style);
						*/
					}
				}
			}

			return value;
		}


		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		#endregion Methods
	}
}
