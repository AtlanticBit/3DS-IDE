﻿#region Using Directives

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion Using Directives


namespace ScintillaNet.Dialogs
{
	/// <summary>
	/// Specifies the base class used for displaying dialog
	/// boxes with a <see cref="Scintilla control"/>.
	/// </summary>
	[DefaultProperty("Scintilla"), ToolboxItemFilter("System.Windows.Forms")]
	public abstract class CommonDialog : Component
	{
		#region Fields

		private static readonly object _closedEvent = new object();

		private Scintilla _scintilla;
		private string _messageCaption;
		private string _title;

		#endregion Fields


		#region Methods

		/// <summary>
		/// When overridden in a derived class, performs custom initialization.
		/// </summary>
		protected virtual void Initialize()
		{
		}

		///// <summary>
		///// When overridden in a derived class, specifies a common dialog box.
		///// </summary>
		///// <param name="owner">
		///// Any object that implements <see cref="IWin32Window" />
		///// that represents the top-level window that will own the dialog box.
		///// </param>
		///// <returns><c>true</c> if the dialog box was successfully run; otherwise, <c>false</c>.</returns>
		//protected abstract bool RunDialog(IWin32Window owner);


		//public void Show(IWin32Window owner)
		//{
		//    if(_scintilla == null)
		//        throw new ArgumentException("The Scintilla property must specify the control used by the dialog.");

			
		//}


		///// <summary>
		///// Runs a common dialog box with the specified owner.
		///// </summary>
		///// <param name="owner">
		///// Any object that implements <see cref="IWin32Window" />
		///// that represents the top-level window that will own the dialog box.
		///// </param>
		///// <returns>
		///// <see cref="DialogResult.OK" /> if the user clicks OK in the dialog box;
		///// otherwise, <see cref="DialogResult.Cancel" />.
		///// </returns>
		///// <exception cref="ArgumentException">The <see cref="Scintilla" /> property is <c>null</c>.</exception>
		//public DialogResult ShowDialog(IWin32Window owner)
		//{
		//    if (_scintilla == null)
		//        throw new ArgumentException("The Scintilla property must specify the control used by the dialog.");

		//    return (RunDialog(owner) ? DialogResult.OK : DialogResult.Cancel);
		//}


		/// <summary>
		/// Raises the <see cref="Closed"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnClosed(EventArgs e)
		{
			EventHandler handler = Events[_closedEvent] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion Methods


		#region Properties

		/// <summary>
		/// Gets or sets the caption used when a message box is generated by the dialog.
		/// </summary>
		/// <value>The caption text used in message boxes.</value>
		[DefaultValue(null)]
		[Category("Appearance")]
		[Description("The caption used when a message box is generated by the dialog.")]
		public string MessageCaption
		{
			get
			{
				return _messageCaption;
			}
			set
			{
				if (value != _messageCaption)
					_messageCaption = value;
			}
		}


		/// <summary>
		/// Gets or sets the <see cref="Scintilla" /> control to modify.
		/// </summary>
		/// <value>The <see cref="Scintilla" /> control to modify.</value>
		[DefaultValue(null), Category("Data")]
		[Description("The Scintilla control used by the dialog.")]
		public virtual Scintilla Scintilla
		{
			get
			{
				return _scintilla;
			}
			set
			{
				if (value != _scintilla)
					_scintilla = value;
			}
		}


		/*
		/// <summary>
		/// Gets or sets the shortcut keys associated with the dialog.
		/// </summary>
		/// <value>One of the <see cref="Keys"/> values. The default is <c>None</c>.</value>
		/// <exception cref="InvalidEnumArgumentException">
		/// The property was not set to a valid combination of the <see cref="Keys"/> enumeration.
		/// </exception>
		[DefaultValue(Keys.None)]
		[Category("Behavior")]
		[Description("The shortcut key associated with the dialog.")]
		public Keys ShortcutKeys
		{
			get
			{
				return _shortcutKeys;
			}
			set
			{
				if (value != Keys.None && !Utilities.IsValidShortcut(value))
					throw new InvalidEnumArgumentException("value", (int)value, typeof(Keys));

				if (value != _shortcutKeys)
				{
				}
			}
		}
		*/


		/// <summary>
		/// Gets or sets the dialog box title.
		/// </summary>
		/// <value>The dialog box title.</value>
		[DefaultValue(null)]
		[Category("Appearance")]
		[Description("The text to display in the title bar of the dialog.")]
		public virtual string Title
		{
			get
			{
				return _title;
			}
			set
			{
				if (value != _title)
				{
					_title = value;
				}
			}
		}

		#endregion Properties


		#region Events

		/// <summary>
		/// Occurs when the dialog is closed.
		/// </summary>
		[Description("Occurs when the user closes the dialog.")]
		public event EventHandler Closed
		{
			add
			{
				Events.AddHandler(_closedEvent, value);
			}
			remove
			{
				Events.RemoveHandler(_closedEvent, value);
			}
		}

		#endregion Events


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CommonDialog" /> class.
		/// </summary>
		public CommonDialog() : this(null)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="CommonDialog" /> class
		/// for the specified <see cref="Scintilla" /> control.
		/// </summary>
		/// <param name="scintilla">The <see cref="Scintilla" /> control that the dialog modifies.</param>
		public CommonDialog(Scintilla scintilla)
		{
			_scintilla = scintilla;
			Initialize();
		}

		#endregion Constructors
	}
}