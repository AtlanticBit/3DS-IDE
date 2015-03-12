#region Using Directives

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion Using Directives


namespace ScintillaPad
{
	internal partial class AboutForm : Form
	{
		#region Methods

		private void graphicsPanel_Paint(object sender, PaintEventArgs e)
		{
			// I can never seem to get the title graphics to line up perfectly when
			// using labels, picture boxes, etc..., so we'll draw it ourselves. :)

			Icon ico = Properties.Resources.ApplicationIcon;
			Rectangle icoRect = new Rectangle(
				graphicsPanel.ClientRectangle.X + 12,
				graphicsPanel.ClientRectangle.Y + 16,
				32, 32);
			e.Graphics.DrawIcon(ico, icoRect);

			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			using (Font font = new Font("Tahoma", 26))
				e.Graphics.DrawString(Util.AssemblyTitle, font, Brushes.White, new PointF(49, 10));

			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			using (Font font = new Font("Tahoma", 10))
				e.Graphics.DrawString(Util.AssemblyDescription, font, Brushes.White, new PointF(54, 47));
		}


		private void websiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			// Navigate to the website
			Process.Start(websiteLinkLabel.Text);
		}

		#endregion Methods


		#region Constructors

		public AboutForm()
		{
			InitializeComponent();

			Text = Util.Format("About {0}", Util.AssemblyTitle);

			titleLabel.Text = Util.AssemblyTitle;
			versionLabel.Text = Util.Format("Version {0}", Util.AssemblyVersion);
			copyrightLabel.Text = Util.AssemblyCopyright;

			scintillaTitleLabel.Text = Util.ScintillaAssemblyTitle;
			scintillaVersionLabel.Text = Util.Format("Version {0}", Util.ScintillaAssemblyVersion);
			scintillaCopyrightLabel.Text = Util.ScintillaAssemblyCopyright;

			licenseTextBox.Text = Properties.Resources.License;
			licenseTextBox.BackColor = SystemColors.Window;

			okButton.Select();
		}

		#endregion Constructors
	}
}
