#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ScintillaNet.Configuration;
using ScintillaNet.Design;
using ScintillaNet.Properties;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Represents a Scintilla text editor control.
	/// </summary>
	[Designer(typeof(ScintillaDesigner)), Docking(DockingBehavior.Ask)]
	[DefaultBindingProperty("Text"), DefaultProperty("Text"), DefaultEvent("DocumentChanged")]
	public partial class Scintilla : Control, INativeScintilla, ISupportInitialize
	{
		#region Constants

		public const string DefaultDllName = "SciLexer.dll";

		#endregion Constants

		#region Fields

		private static readonly int _modifiedState = BitVector32.CreateMask();
		private static readonly int _acceptsReturnState = BitVector32.CreateMask(_modifiedState);
		private static readonly int _acceptsTabState = BitVector32.CreateMask(_acceptsReturnState);
		private static readonly int _hideSelectionState = BitVector32.CreateMask(_acceptsTabState);
		private BitVector32 _state;

		private StyleCollection _styles;
		private LastSelection _lastSelection = new LastSelection();
		private TextAdapter _textAdapter;
		private Whitespace _whitespace;
		private Scrolling _scrolling;
		private Wrapping _wrapping;
		private MarginPadding _padding;

		private BorderStyle _borderStyle = BorderStyle.Fixed3D;
		private VisualStyleRenderer _visualStyleRenderer;

		private delegate IntPtr DirectMessageDelegate(uint msg, IntPtr wParam, IntPtr lParam);

		#endregion Fields

		#region Property Bags
		private Dictionary<string, Color> _colorBag = new Dictionary<string, Color>();
		internal Dictionary<string, Color> ColorBag { get { return _colorBag; } }

		// TODO Could there be things in here we should be disposing?
		private Hashtable _propertyBag = new Hashtable();
		internal Hashtable PropertyBag { get { return _propertyBag; } }

		#endregion

		#region Constructor / Dispose

		public Scintilla()
		{
			// It's critical that we initialize the text adapter early in construction
			// because it has to be aware of any changes to the control text.
			_textAdapter = new TextAdapter(this);

			// Create the style collection
			_styles = CreateStylesInstance();

			this._state = new BitVector32(0);
			this._state[_acceptsReturnState] = true;
			this._state[_acceptsTabState] = true;

			_ns = (INativeScintilla)this;

			// There is a strong code smell coming from this
			_textChangedTimer = new Timer();
			_textChangedTimer.Interval = 1;
			_textChangedTimer.Tick += new EventHandler(this.textChangedTimer_Tick);

			_caption = GetType().FullName;

			// Set up default encoding to UTF-8 which is the Scintilla's best supported.
			// .NET strings are UTF-16 but should be able to convert without any problems.
			DirectMessage(Constants.SCI_SETCODEPAGE, (IntPtr)Constants.SC_CP_UTF8, IntPtr.Zero);

			//	Ensure all style values have at least defaults
			_ns.StyleClearAll();

			_caret = new CaretInfo(this);
			_lines = new LinesCollection(this);
			_selection = new Selection(this);
			_indicators = new IndicatorCollection(this);
			_snippets = new SnippetManager(this);
			_margins = new MarginCollection(this);
			_whitespace = new Whitespace(this);
			_endOfLine = new EndOfLine(this);
			_dropMarkers = new DropMarkers(this);
			_hotspotStyle = new HotspotStyle(this);
			_callTip = new CallTip(this);
			_indentation = new Indentation(this);
			_markers = new MarkerCollection(this);
			_autoComplete = new AutoComplete(this);
			_documentHandler = new DocumentHandler(this);
			_lexing = new Lexing(this);
			_longLines = new LongLines(this);
			_commands = new Commands(this);
			_folding = new Folding(this);
			_configurationManager = new ConfigurationManager(this);
			_printing = new Printing(this);
			_documentNavigation = new DocumentNavigation(this);


			_helpers.AddRange(new TopLevelHelper[] 
			{ 
				_caret, 
				_lines, 
				_selection,
				_indicators, 
				_snippets,
				_margins,
				_whitespace,
				_endOfLine,
				_dropMarkers,
				_hotspotStyle,
				_indentation,
				_markers,
				_autoComplete,
				_documentHandler,
				_lexing,
				_longLines,
				_commands,
				_folding,
				_configurationManager,
				_printing,
				_documentNavigation
			});


			// Change from Scintilla's default black on white to
			// platform defaults for edit controls.
			base.BackColor = SystemColors.Window;
			base.ForeColor = SystemColors.WindowText;

			// Set the default selection colors
			this.SelectionBackColor = SystemColors.Highlight;
			this.SelectionForeColor = SystemColors.HighlightText;
			this.SelectionInactiveBackColor = SystemColors.Control;
			this.SelectionInactiveForeColor = SystemColors.ControlText;

			// Margin padding
			_padding = new MarginPadding(1); // Scintilla default
			this.Padding = DefaultPadding; // User default

			Styles[0].Font = Font;
			Styles[0].ForeColor = ForeColor;
			Styles[0].BackColor = BackColor;
			Styles.DefaultStyle.BackColor = BackColor;

			// New defaults
			DirectMessage(Constants.SCI_SETSCROLLWIDTH, (IntPtr)1, IntPtr.Zero);
			DirectMessage(Constants.SCI_SETSCROLLWIDTHTRACKING, (IntPtr)1, IntPtr.Zero);
			DirectMessage(Constants.SCI_SETLAYOUTCACHE, (IntPtr)LayoutCacheMode.Page, IntPtr.Zero);
		}

		/// <summary>
		/// Overriden. See <see cref="Control.Dispose"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			foreach (ScintillaHelperBase heler in _helpers)
			{
				heler.Dispose();
			}

			if (disposing && IsHandleCreated)
			{
				//	wi11811 2008-07-28 Chris Rickard
				//	Since we eat the destroy message in WndProc
				//	we have to manually let Scintilla know to
				//	clean up its resources.
				Message destroyMessage = new Message();
				destroyMessage.Msg = NativeMethods.WM_DESTROY;
				destroyMessage.HWnd = Handle;
				base.DefWndProc(ref destroyMessage);
			}

			base.Dispose(disposing);
		}
		#endregion

		#region Protected Control Overrides

		/// <summary>
		/// Overriden. See <see cref="Control.WndProc"/>.
		/// </summary>
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case NativeMethods.WM_DESTROY:
					{
						//	wi11811 2008-07-28 Chris Rickard
						//	If we get a destroy message we make this window a message-only window so that it doesn't actually
						//	get destroyed, causing Scintilla to wipe out all its settings associated with this window handle.
						//	We do send a WM_DESTROY message to Scintilla in the Dispose() method so that it does clean up its 
						//	resources when this control is actually done with. Credit (blame :) goes to tom103 for figuring
						//	this one out.

						if (this.IsHandleCreated)
						{
							NativeMethods.SetParent(this.Handle, NativeMethods.HWND_MESSAGE);
							return;
						}
					}
					break;

				case NativeMethods.WM_NCPAINT:
					{
						if (_borderStyle == BorderStyle.Fixed3D && Application.RenderWithVisualStyles)
						{
							// Paint a theme style border
							WmNcPaintThemeBorder(ref m);
							return;
						}
					}
					break;

				case NativeMethods.WM_PAINT:
					{
						//	I tried toggling the ControlStyles.UserPaint flag and sending the message
						//	to both base.WndProc and DefWndProc in order to get the best of both worlds,
						//	Scintilla Paints as normal and .NET fires the Paint Event with the proper
						//	clipping regions and such. This didn't work too well, I kept getting weird
						//	phantom paints, or sometimes the .NET paint events would seem to get painted
						//	over by Scintilla. This technique I use below seems to work perfectly.

						base.WndProc(ref m);

						if (_isCustomPaintingEnabled)
						{
							RECT r;
							if (!NativeMethods.GetUpdateRect(Handle, out r, false))
								r = ClientRectangle;

							Graphics g = CreateGraphics();
							g.SetClip(r);

							OnPaint(new PaintEventArgs(CreateGraphics(), r));
						}
						return;
					}

				case NativeMethods.WM_DROPFILES:
					{
						handleFileDrop(m.WParam);
						return;
					}

				case NativeMethods.WM_SETCURSOR:
					{
						base.DefWndProc(ref m);
						return;
					}

				case NativeMethods.WM_GETTEXT:
					{
						if (String.IsNullOrEmpty(Caption))
						{
							m.WParam = IntPtr.Zero;
							m.Result = IntPtr.Zero;
						}
						else
						{
							m.WParam = (IntPtr)(Caption.Length + 1);
							Marshal.Copy(Caption.ToCharArray(), 0, m.LParam, Caption.Length);
							m.Result = (IntPtr)Caption.Length;
						}
						return;
					}

				case NativeMethods.WM_GETTEXTLENGTH:
					{
						if (String.IsNullOrEmpty(Caption))
							m.Result = IntPtr.Zero;
						else
							m.Result = (IntPtr)Caption.Length;

						return;
					}

				case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFY:
					{
						// Windows Forms is designed to reflect WM_NOTIFY messages from
						// the container control back to the originating control. This
						// is unfortunately not a well documented feature, however, it allows
						// us to respond to WM_NOTIFY messages originating from ourselves.
						unsafe
						{
							IntPtr lParam = m.LParam;
							NativeMethods.NotifyHeader* nh = (NativeMethods.NotifyHeader*)lParam;
							switch (nh->code)
							{
								case Constants.SCN_MODIFIED:
									ScnModified(ref m);
									break;

								case Constants.SCN_UPDATEUI:
									ScnUpdateUI(ref m);
									break;

								case Constants.SCN_ZOOM:
									ScnZoom(ref m);
									break;
							}
						}

						// The old method of consuming notifications. This whole thing needs
						// to be taken to puppy lake and be put down.
						ReflectNotify(ref m);
						return;
					}

				case NativeMethods.WM_HSCROLL:
				case NativeMethods.WM_VSCROLL:
					{
						FireScroll(ref m);

						//	FireOnScroll calls WndProc so no need to call it again
						return;
					}

				default:
					{
						if ((int)m.Msg >= 10000)
						{
							_commands.Execute((BindableCommand)m.Msg);
							return;
						}
					}
					break;
			}

			base.WndProc(ref m);
		}


		private void ReflectNotify(ref Message m)
		{
			SCNotification scn = (SCNotification)Marshal.PtrToStructure(m.LParam, typeof(SCNotification));
			NativeScintillaEventArgs nsea = new NativeScintillaEventArgs(m, scn);

			switch (scn.nmhdr.code)
			{
				case Constants.SCN_AUTOCSELECTION:
					FireAutoCSelection(nsea);
					break;

				case Constants.SCN_CALLTIPCLICK:
					FireCallTipClick(nsea);
					break;

				case Constants.SCN_CHARADDED:
					FireCharAdded(nsea);
					break;

				case Constants.SCEN_CHANGE:
					FireChange(nsea);
					break;

				case Constants.SCN_DOUBLECLICK:
					FireDoubleClick(nsea);
					break;

				case Constants.SCN_DWELLEND:
					FireDwellEnd(nsea);
					break;

				case Constants.SCN_DWELLSTART:
					FireDwellStart(nsea);
					break;

				case Constants.SCN_HOTSPOTCLICK:
					FireHotSpotClick(nsea);
					break;

				case Constants.SCN_HOTSPOTDOUBLECLICK:
					FireHotSpotDoubleclick(nsea);
					break;

				case Constants.SCN_INDICATORCLICK:
					FireIndicatorClick(nsea);
					break;

				case Constants.SCN_INDICATORRELEASE:
					FireIndicatorRelease(nsea);
					break;

				case Constants.SCN_KEY:
					FireKey(nsea);
					break;

				case Constants.SCN_MACRORECORD:
					FireMacroRecord(nsea);
					break;

				case Constants.SCN_MARGINCLICK:
					FireMarginClick(nsea);
					break;

				case Constants.SCN_MODIFIED:
					FireModified(nsea);
					break;

				case Constants.SCN_MODIFYATTEMPTRO:
					FireModifyAttemptRO(nsea);
					break;

				case Constants.SCN_NEEDSHOWN:
					FireNeedShown(nsea);
					break;

				case Constants.SCN_PAINTED:
					FirePainted(nsea);
					break;

				case Constants.SCN_SAVEPOINTLEFT:
					FireSavePointLeft(nsea);
					break;

				case Constants.SCN_SAVEPOINTREACHED:
					FireSavePointReached(nsea);
					break;

				case Constants.SCN_STYLENEEDED:
					FireStyleNeeded(nsea);
					break;

				case Constants.SCN_URIDROPPED:
					FireUriDropped(nsea);
					break;

				case Constants.SCN_USERLISTSELECTION:
					FireUserListSelection(nsea);
					break;
			}
		}
		private static bool _sciLexerLoaded = false;

		/// <summary>
		/// Overriden. See <see cref="Control.CreateParams"/>.
		/// </summary>
		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				//	Otherwise Scintilla won't paint. When UserPaint is set to
				//	true the base Class (Control) eats the WM_PAINT message.
				//	Of course when this set to false we can't use the Paint
				//	events. This is why I'm relying on the Paint notification
				//	sent from scintilla to paint the Marker Arrows.
				SetStyle(ControlStyles.UserPaint, false);

				//	Registers the Scintilla Window Class
				//	I'm relying on the fact that a version specific renamed
				//	SciLexer exists either in the Current Dir or a global path
				//	(See LoadLibrary Windows API Search Rules)

				//	{wi15726} 2008-07-28 Chris Rickard 
				//	As milang pointed out there were some improvements to be made
				//	to this section of code. Now LoadLibrary is only called once
				//	per process (well, appdomain) and a better exception is thrown
				//	if it can't be loaded.
				//	Lastly I took out the whole concept of using an alternate name
				//	for SciLexer.dll. This is a breaking change but I don't think
				//	ANYONE has ever used this feature. If people complain I'll put
				//	it back but it completely avoids the weird behavoir described 
				//	in {wi15726}.
				//	Exception handling by jacobslusser
				if (!_sciLexerLoaded)
				{
					if (NativeMethods.LoadLibrary(DefaultDllName) == IntPtr.Zero)
					{
						int errorCode = Marshal.GetLastWin32Error();
						if (errorCode == NativeMethods.ERROR_MOD_NOT_FOUND)
						{
							// Couldn't find the SciLexer library. Provider a friendlier error message.
							string message = String.Format(
								CultureInfo.CurrentCulture,
								@"The Scintilla library could not be found. Please place the library " +
								@"in a searchable path such as the application or '{0}' directory.",
								Environment.SystemDirectory);

							throw new FileNotFoundException(message, new Win32Exception(errorCode));
						}

						throw new Win32Exception(errorCode);
					}
					else
					{
						_sciLexerLoaded = true;
					}
				}

				//	Tell Windows Forms to create a Scintilla
				//	derived Window Class for this control
				CreateParams cp = base.CreateParams;
				cp.ClassName = "Scintilla";

				// Set the window style or extended style
				// to the appropriate border type.
				switch (_borderStyle)
				{
					case BorderStyle.Fixed3D:
						cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
						break;

					case BorderStyle.FixedSingle:
						cp.Style |= NativeMethods.WS_BORDER;
						break;
				}

				return cp;
			}
		}


		/// <summary>
		/// Overriden. See <see cref="Control.IsInputKey"/>.
		/// </summary>
		protected override bool IsInputKey(Keys keyData)
		{
			if ((keyData & Keys.Shift) != Keys.None)
				keyData ^= Keys.Shift;

			switch (keyData)
			{
				case Keys.Tab:
					return _state[_acceptsTabState];
				case Keys.Enter:
					return _state[_acceptsReturnState];
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.F:

					return true;
			}

			return base.IsInputKey(keyData);
		}

		/// <summary>
		/// Overriden. See <see cref="Control.OnKeyPress"/>.
		/// </summary>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (_supressControlCharacters && (int)e.KeyChar < 32)
				e.Handled = true;

			if (_snippets.IsEnabled && _snippets.IsOneKeySelectionEmbedEnabled && _selection.Length > 0)
			{
				Snippet s;
				if (_snippets.List.TryGetValue(e.KeyChar.ToString(), out s))
				{
					if (s.IsSurroundsWith)
					{
						_snippets.InsertSnippet(s);
						e.Handled = true;
					}
				}
			}

			base.OnKeyPress(e);
		}

		/// <summary>
		/// Overriden. See <see cref="Control.OnKeyDown"/>.
		/// </summary>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled)
				e.SuppressKeyPress = _commands.ProcessKey(e);
		}

		internal void FireKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
		}

		/// <summary>
		/// Overriden. See <see cref="Control.ProcessKeyMessage"/>.
		/// </summary>
		protected override bool ProcessKeyMessage(ref Message m)
		{
			//	For some reason IsInputKey isn't working for
			//	Key.Enter. This seems to make it work as expected
			if ((int)m.WParam == (int)Keys.Enter && !AcceptsReturn)
			{
				return true;
			}
			else
			{
				return base.ProcessKeyMessage(ref m);
			}
		}

		/// <summary>
		/// Overriden. See <see cref="Control.DefaultSize"/>.
		/// </summary>
		protected override Size DefaultSize
		{
			get
			{
				return new Size(200, 100);
			}
		}

        /// <summary>Gets or sets the default cursor for the control.</summary>
        /// <returns>An object of type <see cref="T:System.Windows.Forms.Cursor"></see> representing the current default cursor.</returns>
        protected override Cursor DefaultCursor
        {
            get
            {
                return Cursors.IBeam;
            }
        }


		/// <summary>
		/// Overridden. See <see cref="Control.OnLostFocus"/>.
		/// </summary>
		protected override void OnLostFocus(EventArgs e)
		{
			if (HideSelection)
			{
				DirectMessage(Constants.SCI_HIDESELECTION, (IntPtr)1, IntPtr.Zero);
			}

			if (SelectionInactiveBackColor != SelectionBackColor)
			{
				DirectMessage(Constants.SCI_SETSELBACK, (IntPtr)1, (IntPtr)Util.ColorToRgb(SelectionInactiveBackColor));
			}

			if (SelectionInactiveForeColor != SelectionForeColor)
			{
				DirectMessage(Constants.SCI_SETSELFORE, (IntPtr)1, (IntPtr)Util.ColorToRgb(SelectionInactiveForeColor));
			}
			
			base.OnLostFocus(e);
		}


		/// <summary>
		/// Overridden. See <see cref="Control.OnGotFocus"/>.
		/// </summary>
		protected override void OnGotFocus(EventArgs e)
		{
			if (SelectionBackColor != SelectionInactiveBackColor)
			{
				DirectMessage(Constants.SCI_SETSELBACK, (IntPtr)1, (IntPtr)Util.ColorToRgb(SelectionBackColor));
			}

			if (SelectionForeColor != SelectionInactiveForeColor)
			{
				DirectMessage(Constants.SCI_SETSELFORE, (IntPtr)1, (IntPtr)Util.ColorToRgb(SelectionForeColor));
			}

			if (HideSelection)
			{
				DirectMessage(Constants.SCI_HIDESELECTION, IntPtr.Zero, IntPtr.Zero);
			}

			base.OnGotFocus(e);
		}


		/// <summary>
		/// Provides the support for code block selection
		/// </summary>
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);

			if (_isBraceMatching)
			{
				int position = CurrentPos - 1,
					   bracePosStart = -1,
					   bracePosEnd = -1;

				char character = (char)CharAtPosition(position);

				switch (character)
				{
					case '{':
					case '(':
					case '[':
						if (!this.PositionIsOnComment(position))
						{
							bracePosStart = position;
							bracePosEnd = _ns.BraceMatch(position, 0) + 1;
							_selection.Start = bracePosStart;
							_selection.End = bracePosEnd;
						}
						break;
				}
			}
		}

		/// <summary>
		/// Overriden. See <see cref="Control.OnCreateControl"/>.
		/// </summary>
		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			OnLoad(EventArgs.Empty);
		}

		/// <summary>
		/// Overriden. See <see cref="Control.OnPaint"/>.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			paintRanges(e.Graphics);
		}


		/// <summary>
		/// Raises the <see cref="BackColorChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data. </param>
		protected override void OnBackColorChanged(EventArgs e)
		{
			ResetStyles();
			base.OnBackColorChanged(e);
		}


		/// <summary>
		/// Raises the <see cref="FontChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnFontChanged(EventArgs e)
		{
			ResetStyles();
			base.OnFontChanged(e);
		}


		/// <summary>
		/// Raises the <see cref="ForeColorChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data. </param>
		protected override void OnForeColorChanged(EventArgs e)
		{
			ResetStyles();
			base.OnForeColorChanged(e);
		}

		#endregion

		#region Public Properties

		#region AcceptsReturn

		/// <summary>
		/// Gets or sets a value indicating whether pressing ENTER creates a new line of text in the
		/// control or activates the default button for the form.
		/// </summary>
		/// <returns><c>true</c> if the ENTER key creates a new line of text; <c>false</c> if the ENTER key activates
		/// the default button for the form. The default is <c>false</c>.</returns>
		[DefaultValue(true), Category("Behavior")]
		[Description("Indicates if return characters are accepted as text input.")]
		public bool AcceptsReturn
		{
			get { return _state[_acceptsReturnState]; }
			set { _state[_acceptsReturnState] = value; }
		}

		#endregion

		#region AcceptsTab

		/// <summary>
		/// Gets or sets a value indicating whether pressing the TAB key types a TAB character in the control
		/// instead of moving the focus to the next control in the tab order.
		/// </summary>
		/// <returns><c>true</c> if users can enter tabs using the TAB key; <c>false</c> if pressing the TAB key
		/// moves the focus. The default is <c>false</c>.</returns>
		[DefaultValue(true), Category("Behavior")]
		[Description("Indicates if tab characters are accepted as text input.")]
		public bool AcceptsTab
		{
			get { return _state[_acceptsTabState]; }
			set { _state[_acceptsTabState] = value; }
		}

		#endregion

		#region AllowDrop
		private bool _allowDrop;
		/// <summary>
		/// Gets or sets if .NET Drag and Drop operations are supported.
		/// </summary>
		public override bool AllowDrop
		{
			get
			{
				return _allowDrop;
			}
			set
			{
				NativeMethods.DragAcceptFiles(Handle, value);
				_allowDrop = value;
			}
		}
		#endregion

		#region AutoComplete
		private AutoComplete _autoComplete;
		/// <summary>
		/// Controls autocompletion behavior.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public AutoComplete AutoComplete
		{
			get
			{
				return _autoComplete;
			}
		}

		private bool ShouldSerializeAutoComplete()
		{
			return _autoComplete.ShouldSerialize();
		}
		#endregion

		#region BackColor

		/// <summary>
		/// Gets or sets the background color for the control.
		/// </summary>
		/// <value>
		/// A <see cref="Color"/> that represents the background color of the control.
		/// The default is <see cref="SystemColors.Window"/>.
		/// </value>
		/// <remarks>Settings this property resets any current document styling.</remarks>
		[DefaultValue(typeof(Color), "Window")]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		#endregion

		#region BackgroundImage

		/// <summary>
		/// This property is not relevant for this class.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage
		{
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}

		/// <summary>
		/// This property is not relevant for this class.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout
		{
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}

		#endregion BackgroundImage

		#region BorderStyle

		/// <summary>
		/// Gets or sets the border type of the control.
		/// </summary>
		/// <value>
		/// A <see cref="BorderStyle"/> that represents the border type of the control.
		/// The default is <c>Fixed3D</c>.
		/// </value>
		/// <exception cref="InvalidEnumArgumentException">
		/// A value that is not within the range of valid values for the enumeration was
		/// assigned to the property.
		/// </exception>
		[DefaultValue(BorderStyle.Fixed3D), Category("Appearance")]
		[Description("Indicates whether the control should have a border.")]
		public BorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set
			{
				if (!Enum.IsDefined(typeof(BorderStyle), value))
					throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));

				if (value != _borderStyle)
				{
					_borderStyle = value;

					// Get the current window style and extended style
					int style = (int)NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_STYLE);
					int exStyle = (int)NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);

					switch (_borderStyle)
					{
						case BorderStyle.Fixed3D:
							exStyle |= NativeMethods.WS_EX_CLIENTEDGE;
							style &= ~NativeMethods.WS_BORDER;
							break;

						case BorderStyle.FixedSingle:
							exStyle &= ~NativeMethods.WS_EX_CLIENTEDGE;
							style |= NativeMethods.WS_BORDER;
							break;

						default:
							style &= ~NativeMethods.WS_BORDER;
							exStyle &= ~NativeMethods.WS_EX_CLIENTEDGE;
							break;
					}

					// Update the window style and extended style
					NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_STYLE, (IntPtr)style);
					NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, (IntPtr)exStyle);

					// Force a layout and redraw of the non-client area
					NativeMethods.SetWindowPos(
						Handle, IntPtr.Zero, 0, 0, 0, 0,
						NativeMethods.SWP_NOSIZE |
						NativeMethods.SWP_NOMOVE |
						NativeMethods.SWP_NOZORDER | 
						NativeMethods.SWP_FRAMECHANGED);

					OnBorderStyleChanged(EventArgs.Empty);
				}
			}
		}

		#endregion BorderStyle

		#region CallTip
		private CallTip _callTip;
		/// <summary>
		/// Manages CallTip (Visual Studio-like code Tooltip) behaviors
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public CallTip CallTip
		{
			get
			{
				return _callTip;
			}
			set
			{
				_callTip = value;
			}
		}

		private bool ShouldSerializeCallTip()
		{
			return _callTip.ShouldSerialize();
		}
		#endregion

		#region CanCopy

		/// <summary>
		/// Gets a value indicating whether text can be copied given the current selection.
		/// </summary>
		/// <value><c>true</c> if the text can be copied; otherwise, <c>false</c>.</value>
		/// <remarks>This is equivalent to determining if there is a valid selection.</remarks>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanCopy
		{
			get
			{
				return
					DirectMessage(Constants.SCI_GETSELECTIONSTART, IntPtr.Zero, IntPtr.Zero) !=
					DirectMessage(Constants.SCI_GETSELECTIONEND, IntPtr.Zero, IntPtr.Zero);
			}
		}

		#endregion CanCopy

		#region CanCut

		/// <summary>
		/// Gets a value indicating whether text can be cut given the current selection.
		/// </summary>
		/// <value><c>true</c> if the text can be cut; otherwise, <c>false</c>.</value>
		/// <remarks>This is equivalent to determining if there is a valid selection.</remarks>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanCut
		{
			get
			{
				// Look familiar? :)
				return
					DirectMessage(Constants.SCI_GETSELECTIONSTART, IntPtr.Zero, IntPtr.Zero) !=
					DirectMessage(Constants.SCI_GETSELECTIONEND, IntPtr.Zero, IntPtr.Zero);
			}
		}

		#endregion CanCut

		#region CanDelete

		/// <summary>
		/// Gets a value indicating whether text can be removed given the current selection.
		/// </summary>
		/// <value><c>true</c> if the text can be removed; otherwise, <c>false</c>.</value>
		/// <remarks>This is equivalent to determining if there is a valid selection.</remarks>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanDelete
		{
			get
			{
				// The price of convenience...
				return
					DirectMessage(Constants.SCI_GETSELECTIONSTART, IntPtr.Zero, IntPtr.Zero) !=
					DirectMessage(Constants.SCI_GETSELECTIONEND, IntPtr.Zero, IntPtr.Zero);
			}
		}

		#endregion CanDelete

		#region CanPaste

		/// <summary>
		/// Gets a value indicating whether the document can accept text currently
		/// stored in the <c>Clipboard</c>.
		/// </summary>
		/// <value><c>true</c> if text can be pasted; otherwise, <c>false</c>.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanPaste
		{
			get
			{
				bool canPaste = DirectMessage(Constants.SCI_CANPASTE, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero;

				// It's confusing for a property to indicate that a user
				// can paste even if there is no compatible data in the clipboard,
				// so we also include that in the result.
				// TODO Do we need to do anything to support the pasting of Unicode text?
				return (canPaste && Clipboard.ContainsText(TextDataFormat.Text));
			}
		}

		#endregion CanPaste

		#region CanRedo

		/// <summary>
		/// Gets a value indicating whether a previously undone operation can be redone.
		/// </summary>
		/// <value><c>true</c> if the previously undone operation can be redone; otherwise, <c>false</c>.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanRedo
		{
			get { return DirectMessage(Constants.SCI_CANREDO, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero; }
		}

		#endregion CanRedo

		#region CanUndo

		/// <summary>
		/// Gets a value indicating whether the previous document operation can be undone.
		/// </summary>
		/// <value><c>true</c> if the previous operation performed can be undone; otherwise, <c>false</c>.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanUndo
		{
			get { return DirectMessage(Constants.SCI_CANUNDO, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero; }
		}

		#endregion CanUndo

		#region Caption

		private string _caption;
		/// <summary>
		/// Gets/Sets the Win32 Window Caption. Defaults to Type's FullName
		/// </summary>
		[Category("Behavior")]
		[Description("Win32 Window Caption")]
		public string Caption
		{
			get { return _caption; }
			set
			{
				if (_caption != value)
				{
					_caption = value;

					//	Triggers a new WM_GETTEXT query
					base.Text = value;
				}

			}
		}

		private void ResetCaption()
		{
			Caption = GetType().FullName;
		}

		private bool ShouldSerializeCaption()
		{
			return Caption != GetType().FullName;
		}
		#endregion

		#region Caret
		private CaretInfo _caret;

		/// <summary>
		/// Controls Caret Behavior
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public CaretInfo Caret
		{
			get
			{
				return _caret;
			}
		}

		private bool ShouldSerializeCaret()
		{
			return _caret.ShouldSerialize();
		}

		#endregion

		#region CurrentPos

		/// <summary>
		/// Gets or sets the character index of the current caret position.
		/// </summary>
		/// <returns>The character index of the current caret position.</returns>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CurrentPos
		{
			get
			{
				return NativeInterface.GetCurrentPos();
			}
			set
			{
				NativeInterface.GotoPos(value);
			}
		}
		#endregion

		#region Commands
		private Commands _commands;
		/// <summary>
		/// Controls behavior of keyboard bound commands.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Commands Commands
		{
			get
			{
				return _commands;
			}
			set
			{
				_commands = value;
			}
		}

		private bool ShouldSerializeCommands()
		{
			return _commands.ShouldSerialize();
		}
		#endregion

		#region ConvertLineBreaksOnPaste

		/// <summary>
		/// Gets or sets whether pasted line break characters are converted to match the document's end-of-line mode.
		/// </summary>
		/// <value>
		/// <c>true</c> if line break characters are converted; otherwise, <c>false</c>.
		/// The default is <c>true</c>.
		/// </value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ConvertLineBreaksOnPaste
		{
			// TODO Site the EOL mode property in the doc comments after the API is stabilized.
			get { return DirectMessage(Constants.SCI_GETPASTECONVERTENDINGS, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero; }
			set { DirectMessage(Constants.SCI_SETPASTECONVERTENDINGS, (value ? (IntPtr)1 : IntPtr.Zero), IntPtr.Zero); }
		}

		#endregion ConvertLineBreaksOnPaste

		#region ConfigurationManager
		private Configuration.ConfigurationManager _configurationManager;
		/// <summary>
		/// Controls behavior of loading/managing ScintillaNET configurations.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Configuration.ConfigurationManager ConfigurationManager
		{
			get
			{
				return _configurationManager;
			}
			set
			{
				_configurationManager = value;
			}
		}

		private bool ShouldSerializeConfigurationManager()
		{
			return _configurationManager.ShouldSerialize();
		}
		#endregion

		#region DefaultPadding

		/// <summary>
		/// Gets the internal spacing, in pixels, of the left and right margins of the control.
		/// </summary>
		/// <value>
		/// A <see cref="MarginPadding"/> that represents the internal spacing of the 
		/// left and right margins of the control.
		/// </value>
		protected new virtual MarginPadding DefaultPadding
		{
			get 
			{
				return new MarginPadding(1);
			}
		}

		#endregion DefaultPadding

		#region DocumentHandler
		private DocumentHandler _documentHandler;
		/// <summary>
		/// Controls behavior of Documents
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DocumentHandler DocumentHandler
		{
			get
			{
				return _documentHandler;
			}
			set
			{
				_documentHandler = value;
			}
		}
		#endregion

		#region DocumentNavigation
		private DocumentNavigation _documentNavigation;
		/// <summary>
		/// Controls behavior of automatic document navigation
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public DocumentNavigation DocumentNavigation
		{
			get
			{
				return _documentNavigation;
			}
			set
			{
				_documentNavigation = value;
			}
		}

		private bool ShouldSerializeDocumentNavigation()
		{
			return _documentNavigation.ShouldSerialize();
		}

		#endregion

		#region DropMarkers
		private DropMarkers _dropMarkers;
		/// <summary>
		/// Controls behavior of Drop Markers
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public DropMarkers DropMarkers
		{
			get
			{
				return _dropMarkers;
			}
		}

		private bool ShouldSerializeDropMarkers()
		{
			return _dropMarkers.ShouldSerialize();
		}
		#endregion

		#region EndOfLine
		private EndOfLine _endOfLine;

		/// <summary>
		/// Controls End Of Line Behavior
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public EndOfLine EndOfLine
		{
			get
			{
				return _endOfLine;
			}
			set
			{
				_endOfLine = value;
			}
		}

		private bool ShouldSerializeEndOfLine()
		{
			return _endOfLine.ShouldSerialize();
		}

		#endregion

		#region Encoding

		/// <summary>
		/// Gets or sets the type of text encoding the <see cref="Scintilla" /> control uses internally.
		/// </summary>
		/// <value>The text encoding to use. The default is <see cref="UTF8Encoding" />.</value>
		/// <remarks>The default encoding is suitable for most users.</remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Encoding Encoding
		{
			get { return _textAdapter.GetEncoding(); }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				int codePage = value.CodePage;
				switch(codePage)
				{
					case (int)Constants.SC_CP_UTF8:
					case Constants.CODEPAGE_JAPANESE:
					case Constants.CODEPAGE_CHINESE_SIMPLIFIED:
					case Constants.CODEPAGE_KOREAN_UNIFIED:
					case Constants.CODEPAGE_CHINESE_TRADITIONAL:
					case Constants.CODEPAGE_KOREAN_JOHAB:
					case Constants.CODEPAGE_ASCII:
						break;

					default:
						throw new ArgumentException("The specified encoding is not supported.");
				}

				// Update Scintilla. The adapter will be updated on the next query.
				DirectMessage(Constants.SCI_SETCODEPAGE, (IntPtr)(codePage == Constants.CODEPAGE_ASCII ? 0 : codePage), IntPtr.Zero);
			}
		}
		#endregion

		#region EnableUndo

		/// <summary>
		/// Gets or sets whether changes to the document can later be undone.
		/// </summary>
		[DefaultValue(true), Category("Behavior")]
		[Description("Indicates whether document changes can be undone.")]
		public bool EnableUndo
		{
			// TODO I wonder if there is a better name for this property....
			get { return DirectMessage(Constants.SCI_GETUNDOCOLLECTION, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero; }
			set
			{
				if (value != EnableUndo)
				{
					DirectMessage(Constants.SCI_SETUNDOCOLLECTION, (value ? (IntPtr)1 : IntPtr.Zero), IntPtr.Zero);
					
					// Clear the buffer if disabled
					if (!value)
						ClearUndo();
				}
			}
		}

		#endregion EnableUndo

		#region Folding
		private Folding _folding;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Folding Folding
		{
			get
			{
				return _folding;
			}
			set
			{
				_folding = value;
			}
		}

		private bool ShouldSerializeFolding()
		{
			return _folding.ShouldSerialize();
		}
		#endregion

		#region Font

		/// <summary>
		/// Gets or sets the font of the text displayed by the control.
		/// </summary>
		/// <value>The <see cref="Font"/> to apply to the text displayed by the control.
		/// The default is the value of the <see cref="DefaultFont"/> property.</value>
		/// <remarks>Settings this property resets any current document styling.</remarks>
		public override Font Font
		{
			get { return base.Font; }
			set { base.Font = value; }
		}

		#endregion Font

		#region ForeColor

		/// <summary>
		/// Gets or sets the foreground color of the control.
		/// </summary>
		/// <value>
		/// The foreground <see cref="Color"/> of the control.
		/// The default is <see cref="SystemColors.WindowText"/>.
		/// </value>
		/// <remarks>Settings this property resets any current document styling.</remarks>
		[DefaultValue(typeof(Color), "WindowText")]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}

		#endregion

		#region HideSelection

		/// <summary>
		/// Gets or sets a value indicating whether the selected text in the control
		/// remains highlighted when the control loses focus.
		/// </summary>
		/// <value>
		/// <c>true</c> if the selected text does not appear highlighted when the control loses focus;
		/// <c>false</c>, if the selected text remains highlighted when the control loses focus.
		/// The default is <c>false</c>.
		/// </value>
		[DefaultValue(false)]
		[Category("Behavior")]
		[Description("Indicates that the selection should be hidden when the control loses focus.")]
		public bool HideSelection
		{
			get
			{
				return _state[_hideSelectionState];
			}
			set
			{
				if (value != _state[_hideSelectionState])
				{
					_state[_hideSelectionState] = value;
					if (!Focused)
						DirectMessage(Constants.SCI_HIDESELECTION, (IntPtr)(value ? 1 : 0), IntPtr.Zero);

					// In keeping with text box tradition, we'll raise the changed event
					OnHideSelectionChanged(EventArgs.Empty);
				}
			}
		}

		#endregion HideSelection

		#region HotspotStyle
		private HotspotStyle _hotspotStyle;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public HotspotStyle HotspotStyle
		{
			get
			{
				return _hotspotStyle;
			}
			set
			{
				_hotspotStyle = value;
			}
		}

		private bool ShouldSerializeHotspotStyle()
		{
			return _hotspotStyle.ShouldSerialize();
		}
		#endregion

		#region Indicators
		private IndicatorCollection _indicators;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IndicatorCollection Indicators
		{
			get { return _indicators; }
		}
		#endregion

		#region Indentation
		private Indentation _indentation;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Indentation Indentation
		{
			get
			{
				return _indentation;
			}
			set
			{
				_indentation = value;
			}
		}

		private bool ShouldSerializeIndentation()
		{
			return _indentation.ShouldSerialize();
		}
		#endregion

		#region IsBraceMatching
		/// <summary>
		/// Enables the brace matching from current position.
		/// </summary>
		private bool _isBraceMatching = false;
		[DefaultValue(false), Category("Behavior")]
		public bool IsBraceMatching
		{
			get { return _isBraceMatching; }
			set { _isBraceMatching = value; }
		}



		/// <summary>
		/// Custom way to find the matching brace when BraceMatch() does not work
		/// </summary>
		internal int SafeBraceMatch(int position)
		{
			int match = this.CharAtPosition(position);
			int toMatch = 0;
			int length = GetLength();
			int ch;
			int sub = 0;
			LexerType lexer = _lexing.Lexer;
			_lexing.Colorize(0, -1);
			bool comment = PositionIsOnComment(position, lexer);
			switch (match)
			{
				case '{':
					toMatch = '}';
					goto down;
				case '(':
					toMatch = ')';
					goto down;
				case '[':
					toMatch = ']';
					goto down;
				case '}':
					toMatch = '{';
					goto up;
				case ')':
					toMatch = '(';
					goto up;
				case ']':
					toMatch = '[';
					goto up;
			}
			return -1;
		// search up
		up:
			while (position >= 0)
			{
				position--;
				ch = CharAtPosition(position);
				if (ch == match)
				{
					if (comment == PositionIsOnComment(position, lexer)) sub++;
				}
				else if (ch == toMatch && comment == PositionIsOnComment(position, lexer))
				{
					sub--;
					if (sub < 0) return position;
				}
			}
			return -1;
		// search down
		down:
			while (position < length)
			{
				position++;
				ch = CharAtPosition(position);
				if (ch == match)
				{
					if (comment == PositionIsOnComment(position, lexer)) sub++;
				}
				else if (ch == toMatch && comment == PositionIsOnComment(position, lexer))
				{
					sub--;
					if (sub < 0) return position;
				}
			}
			return -1;
		}
		#endregion

		#region IsCustomPaintingEnabled
		private bool _isCustomPaintingEnabled = true;
		[DefaultValue(true), Category("Behavior")]
		public bool IsCustomPaintingEnabled
		{
			get
			{
				return _isCustomPaintingEnabled;
			}
			set
			{
				_isCustomPaintingEnabled = value;
			}
		}

		#endregion

		#region LayoutCacheMode

		/// <summary>
		/// Gets or sets the line layout caching strategy in a <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>One of the <see cref="LayoutCacheMode"/> enumeration values. The default is <c>Page</c>.</value>
		/// <exception cref="InvalidEnumArgumentException">
		/// A value that is not within the range of valid values for the enumeration was
		/// assigned to the property.
		/// </exception>
		/// <remarks>Larger cache sizes increase performance at the expense of memory.</remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual LayoutCacheMode LayoutCacheMode
		{
			get { return (LayoutCacheMode)DirectMessage(Constants.SCI_GETLAYOUTCACHE, IntPtr.Zero, IntPtr.Zero); }
			set
			{
				if (!Enum.IsDefined(typeof(LayoutCacheMode), value))
					throw new InvalidEnumArgumentException("value", (int)value, typeof(LayoutCacheMode));

				if (value != LayoutCacheMode)
					DirectMessage(Constants.SCI_SETLAYOUTCACHE, (IntPtr)value, IntPtr.Zero);
			}
		}

		#endregion LayoutCacheMode

		#region Lexing
		private Lexing _lexing;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Lexing Lexing
		{
			get
			{
				return _lexing;
			}
			set
			{
				_lexing = value;
			}
		}

		private bool ShouldSerializeLexing()
		{
			return _lexing.ShouldSerialize();
		}
		#endregion

		#region LineCount

		/// <summary>
		/// Retrieves the total number of lines in the document.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LineCount
		{
			get { return (int)DirectMessage(Constants.SCI_GETLINECOUNT, IntPtr.Zero, IntPtr.Zero); }
		}

		#endregion LineCount

		#region Lines
		private LinesCollection _lines;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LinesCollection Lines
		{
			get
			{

				return _lines;
			}
		}

		#endregion

		#region LongLines
		private LongLines _longLines;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public LongLines LongLines
		{
			get
			{
				return _longLines;
			}
			set
			{
				_longLines = value;
			}
		}

		private bool ShouldSerializeLongLines()
		{
			return _longLines.ShouldSerialize();
		}
		#endregion

		#region ManagedRanges
		private List<ManagedRange> _managedRanges = new List<ManagedRange>();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<ManagedRange> ManagedRanges
		{
			get { return _managedRanges; }
		}
		#endregion

		#region Margins
		private MarginCollection _margins;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public MarginCollection Margins
		{
			get
			{
				return _margins;
			}
		}

		private bool ShouldSerializeMargins()
		{
			return _margins.ShouldSerialize();
		}

		private void ResetMargins()
		{
			_margins.Reset();
		}
		#endregion

		#region Markers
		private MarkerCollection _markers;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public MarkerCollection Markers
		{
			get
			{
				return _markers;
			}
			set
			{
				_markers = value;
			}
		}

		private bool ShouldSerializeMarkers()
		{
			return _markers.ShouldSerialize();
		}
		#endregion

		#region MatchBraces
		private bool _matchBraces = true;
		[DefaultValue(true), Category("Behavior")]
		public bool MatchBraces
		{
			get
			{
				return _matchBraces;
			}
			set
			{
				_matchBraces = value;

				//	Clear any active Brace matching that may exist
				if (!value)
					_ns.BraceHighlight(-1, -1);
			}
		}

		#endregion

		#region Modified

		/// <summary>
		/// Gets or sets a value that indicates that the control has been modified by the user since
		/// the control was created or its contents were last set.
		/// </summary>
		/// <returns><c>true</c> if the control's contents have been modified; otherwise, <c>false</c>.
		/// The default is <c>false</c>.</returns>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Modified
		{
			get { return _state[_modifiedState]; }
			set
			{
				if (_state[_modifiedState] != value)
				{
					// Update the local (and native) state
					_state[_modifiedState] = value;
					if (!value)
						_ns.SetSavePoint();

					OnModifiedChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region MouseDownCaptures
		[DefaultValue(true), Category("Behavior")]
		public bool MouseDownCaptures
		{
			get
			{
				return NativeInterface.GetMouseDownCaptures();
			}
			set
			{
				NativeInterface.SetMouseDownCaptures(value);
			}
		}
		#endregion

		#region Native Interface
		private INativeScintilla _ns;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public INativeScintilla NativeInterface
		{
			get
			{
				return this as INativeScintilla;
			}
		}
		#endregion

		#region OverType

		[DefaultValue(false), Category("Behavior")]
		public bool OverType
		{
			get
			{
				return _ns.GetOvertype();
			}
			set
			{
				_ns.SetOvertype(value);
			}
		}
		#endregion

		#region Paddding

		/// <summary>
		/// Gets or sets left and right margin padding within the control.
		/// </summary>
		/// <returns>
		/// A <see cref="MarginPadding"/> representing the control's internal left 
		/// and right margin spacing characteristics.
		/// </returns>
		[Category("Layout")]
		[Description("Specifies the interior spacing of the left and right margins of the control.")]
		public new MarginPadding Padding
		{
			get
			{
				return _padding; 
			}
			set
			{
				// Don't throw an exception, just clamp to positive values
				value = new MarginPadding(Math.Max(0, value.Left), Math.Max(0, value.Right));

				if (value != _padding)
				{
					if (value.Left != _padding.Left)
						DirectMessage(Constants.SCI_SETMARGINLEFT, IntPtr.Zero, (IntPtr)value.Left);

					if (value.Right != _padding.Right)
						DirectMessage(Constants.SCI_SETMARGINRIGHT, IntPtr.Zero, (IntPtr)value.Right);

					_padding = value;
					OnPaddingChanged(EventArgs.Empty);
				}
			}
		}

		private bool ShouldSerializePadding()
		{
			return Padding != DefaultPadding;
		}

		private void ResetPadding()
		{
			Padding = DefaultPadding;
		}

		#endregion Padding

		#region Printing
		private Printing _printing;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout")]
		public Printing Printing
		{
			get
			{
				return _printing;
			}
			set
			{
				_printing = value;
			}
		}
		private bool ShouldSerializePrinting()
		{
			return _printing.ShouldSerialize();
		}
		#endregion

		#region PositionCacheSize

		/// <summary>
		/// Gets or sets the position cache size used to layout short runs of text in a <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>The size of the position cache. The default is <c>1024</c>.</value>
		/// <remarks>Larger cache sizes increase performance at the expense of memory.</remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int PositionCacheSize
		{
			get { return (int)DirectMessage(Constants.SCI_GETPOSITIONCACHE, IntPtr.Zero, IntPtr.Zero); }
			set
			{
				// TODO Some range checking?
				if (value != PositionCacheSize)
					DirectMessage(Constants.SCI_SETPOSITIONCACHE, (IntPtr)value, IntPtr.Zero);
			}
		}

		#endregion PositionCacheSize

		#region RawText
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		unsafe public byte[] RawText
		{
			get
			{
				int length = NativeInterface.GetTextLength() + 1;

				//	May as well avoid all the crap below if we know what the outcome
				//	is going to be :)
				if (length == 1)
					return new byte[] { 0 };

				//  Allocate a buffer the size of the string + 1 for 
				//  the NULL terminator. Scintilla always sets this
				//  regardless of the encoding
				byte[] buffer = new byte[length];

				//  Get a direct pointer to the the head of the buffer
				//  to pass to the message along with the wParam. 
				//  Scintilla will fill the buffer with string data.
				fixed (byte* bp = buffer)
				{
					_ns.SendMessageDirect(Constants.SCI_GETTEXT, (IntPtr)length, (IntPtr)bp);
					return buffer;
				}
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					Clear();
				}
				else
				{
					//	This byte[] HAS to be NULL terminated or who knows how big 
					//	of an overrun we'll have on our hands
					if (value[value.Length - 1] != 0)
					{
						//	I hate to have to do this becuase it can be very inefficient.
						//	It can probably be done much better by the client app
						Array.Resize<byte>(ref value, value.Length + 1);
						value[value.Length - 1] = 0;
					}
					fixed (byte* bp = value)
						_ns.SendMessageDirect(Constants.SCI_SETTEXT, IntPtr.Zero, (IntPtr)bp);
				}
			}
		}
		#endregion

		#region ReadOnly

		/// <summary>
		/// Gets or sets a value indicating whether text in the control is read-only.
		/// </summary>
		/// <value>
		/// <c>true</c> if the control is read-only; otherwise, <c>false</c>.
		/// The default is <c>false</c>.
		/// </value>
		[DefaultValue(false)]
		[Category("Behavior")]
		[Description("Indicates whether the text in the control can be changed or not.")]
		public bool ReadOnly
		{
			get
			{
				return DirectMessage(Constants.SCI_GETREADONLY, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero;
			}
			set
			{
				if (value != ReadOnly)
				{
					DirectMessage(Constants.SCI_SETREADONLY, (value ? (IntPtr)1 : IntPtr.Zero), IntPtr.Zero);
					OnReadOnlyChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Scrolling

		/// <summary>
		/// Gets an object that controls scrolling options in the <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>A <see cref="Scrolling"/> object that manages scrolling options in a <see cref="Scintilla"/> control.</value>
		[CategoryAttribute("Layout"), Description("The control's scrolling options.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Scrolling Scrolling
		{
			get
			{
				if (_scrolling == null)
					_scrolling = CreateScrollingInstance();

				return _scrolling;
			}
		}

		#endregion

		#region Selection
		private Selection _selection;

		[Browsable(false)]
		public Selection Selection
		{
			get
			{
				return _selection;
			}
		}

		#endregion

		#region SelectionBackColor

		/// <summary>
		/// Gets or sets the background color used by the control when text is selected.
		/// </summary>
		/// <value>
		/// A <see cref="Color"/> that represents the background color of selected text.
		/// The default is <see cref="SystemColors.Highlight"/>.
		/// </value>
		/// <exception cref="ArgumentOutOfRange">
		/// The specified <paramref name="value"/> has an alpha value that is less that 255.
		/// </exception>
		[Category("Appearance")]
		[DefaultValue(typeof(Color), "Highlight")]
		[Description("The background color of selected text.")]
		public Color SelectionBackColor
		{
			get
			{
				// Scintilla doesn't provide a way to query the current selection
				// color so we have to store this one ourselves.
				return _colorBag["SelectionBackColor"];
			}
			set
			{
				if (!value.Equals(Color.Empty) && value.A < 255)
					throw new ArgumentException("Transparent colors are not supported.");

				if (value.IsEmpty)
					value = SystemColors.Highlight;

				// Set the color
				_colorBag["SelectionBackColor"] = value;
				if (Focused)
					DirectMessage(Constants.SCI_SETSELBACK, (IntPtr)1, (IntPtr)Util.ColorToRgb(value));

				// I would really love to make use of the SCI_SETSELALPHA message but I honestly
				// can't make heads or tails of what the purpose is and how to use it
			}
		}

		#endregion SelectionBackColor

		#region SelectionEnd

		/// <summary>
		/// Gets or sets the ending character index of text selected in the control.
		/// </summary>
		/// <value>The ending index of text selected in the control.</value>
		/// <remarks>
		/// The selection end always refers to the anchor end of a selection. When this property is set,
		/// the caret is not scrolled into view.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRange">The value is not a valid index within the document.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectionEnd
		{
			get
			{
				// Translate byte index to char index
				int byteIndex = (int)DirectMessage(Constants.SCI_GETANCHOR, IntPtr.Zero, IntPtr.Zero);
				return _textAdapter.ByteToCharIndex(byteIndex);
			}
			set
			{
				if (value < 0 || value > _textAdapter.TotalCharLength())
					throw new ArgumentOutOfRangeException("value", "The value must be a char index within the document.");

				int byteIndex = _textAdapter.CharToByteIndex(value);
				DirectMessage(Constants.SCI_SETANCHOR, (IntPtr)byteIndex, IntPtr.Zero);
			}
		}

		#endregion SelectionEnd

		#region SelectionForeColor

		/// <summary>
		/// Gets or sets the foreground color used by the control when text is selected.
		/// </summary>
		/// <value>
		/// A <see cref="Color"/> that represents the foreground color of selected text.
		/// The default is <see cref="SystemColors.HighlightText"/>.
		/// </value>
		/// <exception cref="ArgumentOutOfRange">
		/// The specified <paramref name="value"/> has an alpha value that is less that 255.
		/// </exception>
		[Category("Appearance")]
		[DefaultValue(typeof(Color), "HighlightText")]
		[Description("The foreground color of selected text.")]
		public Color SelectionForeColor
		{
			get
			{
				// Scintilla doesn't provide a way to query the current selection
				// text color so we have to store this one ourselves.
				return _colorBag["SelectionForeColor"];
			}
			set
			{
				if (!value.Equals(Color.Empty) && value.A < 255)
					throw new ArgumentException("Transparent colors are not supported.");

				if (value.IsEmpty)
					value = SystemColors.HighlightText;

				// Set the color
				_colorBag["SelectionForeColor"] = value;
				if (Focused)
					DirectMessage(Constants.SCI_SETSELFORE, (IntPtr)1, (IntPtr)Util.ColorToRgb(value));
			}
		}

		#endregion SelectionForeColor

		#region SelectionInactiveBackColor

		/// <summary>
		/// Gets or sets the background color used by the control when text is selected and does not have focus.
		/// </summary>
		/// <value>
		/// A <see cref="Color"/> that represents the background color of selected text when the control does not have focus.
		/// The default is <see cref="SystemColors.Control"/>.
		/// </value>
		/// <remarks>The <see cref="HideSelection"/> property must be set to <c>false</c> for this value to be visible.</remarks>
		/// <exception cref="ArgumentOutOfRange">
		/// The specified <paramref name="value"/> has an alpha value that is less that 255.
		/// </exception>
		[Category("Appearance")]
		[DefaultValue(typeof(Color), "Control")]
		[Description("The background color of selected text when the control does not have focus.")]
		public Color SelectionInactiveBackColor
		{
			get
			{
				return _colorBag["SelectionInactiveBackColor"];
			}
			set
			{
				if (!value.Equals(Color.Empty) && value.A < 255)
					throw new ArgumentException("Transparent colors are not supported.");

				if (value.IsEmpty)
					value = SystemColors.Control;

				// Set the color
				_colorBag["SelectionInactiveBackColor"] = value;
				if (!Focused && !HideSelection)
					DirectMessage(Constants.SCI_SETSELBACK, (IntPtr)1, (IntPtr)Util.ColorToRgb(value));
			}
		}

		#endregion SelectionInactiveBackColor

		#region SelectionInactiveForeColor

		/// <summary>
		/// Gets or sets the foreground color used by the control when text is selected and does not have focus.
		/// </summary>
		/// <value>
		/// A <see cref="Color"/> that represents the foreground color of selected text when the control does not have focus.
		/// The default is <see cref="SystemColors.ControlText"/>.
		/// </value>
		/// <remarks>The <see cref="HideSelection"/> property must be set to <c>false</c> for this value to be visible.</remarks>
		/// <exception cref="ArgumentOutOfRange">
		/// The specified <paramref name="value"/> has an alpha value that is less that 255.
		/// </exception>
		[Category("Appearance")]
		[DefaultValue(typeof(Color), "ControlText")]
		[Description("The foreground color of selected text when the control does not have focus.")]
		public Color SelectionInactiveForeColor
		{
			get
			{
				return _colorBag["SelectionInactiveForeColor"];
			}
			set
			{
				if (!value.Equals(Color.Empty) && value.A < 255)
					throw new ArgumentException("Transparent colors are not supported.");

				if (value.IsEmpty)
					value = SystemColors.ControlText;

				// Set the color
				_colorBag["SelectionInactiveForeColor"] = value;
				if (!Focused && !HideSelection)
					DirectMessage(Constants.SCI_SETSELFORE, (IntPtr)1, (IntPtr)Util.ColorToRgb(value));
			}
		}

		#endregion SelectionInactiveForeColor

		#region SelectionStart

		/// <summary>
		/// Gets or sets the starting character index of text selected in the control.
		/// </summary>
		/// <value>The starting index of text selected in the control.</value>
		/// <remarks>
		/// The selection start always refers to the caret end of a selection. When this property is set,
		/// the caret is not scrolled into view.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRange">The value is not a valid index within the document.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectionStart
		{
			get
			{
				// Translate byte index to char index
				int byteIndex = (int)DirectMessage(Constants.SCI_GETCURRENTPOS, IntPtr.Zero, IntPtr.Zero);
				return _textAdapter.ByteToCharIndex(byteIndex);
			}
			set
			{
				if (value < 0 || value > _textAdapter.TotalCharLength())
					throw new ArgumentOutOfRangeException("value", "The value must be a char index within the document.");

				int byteIndex = _textAdapter.CharToByteIndex(value);
				DirectMessage(Constants.SCI_SETCURRENTPOS, (IntPtr)byteIndex, IntPtr.Zero);
			}
		}

		#endregion SelectionStart

		#region SelectedText

		/// <summary>
		/// Gets or sets the currently selected text in the control.
		/// </summary>
		/// <value>A string that represents the currently selected text in the control.</value>
		/// <remarks>
		/// You can assign text to this property to change the text currently selected in the control.
		/// If no text is currently selected in the control, this property returns a zero-length string. If there
		/// is no current selection, the text assigned to this property is inserted at the caret position.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SelectedText
		{
			get
			{
				return _textAdapter.GetSelectedText();
			}
			set
			{
				_textAdapter.SetSelectedText(value);
			}
		}

		#endregion SelectedText

		#region SearchFlags
		private SearchFlags _searchFlags = SearchFlags.Empty;
		[
		DefaultValue(SearchFlags.Empty), Category("Behavior"),
		Editor(typeof(Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))
		]
		public SearchFlags SearchFlags
		{
			get
			{
				return _searchFlags;
			}
			set
			{
				_searchFlags = value;
			}
		}
		#endregion

		#region Snippets
		private SnippetManager _snippets;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public SnippetManager Snippets
		{
			get
			{
				return _snippets;
			}
		}

		private bool ShouldSerializeSnippets()
		{
			return _snippets.ShouldSerialize();
		}

		#endregion

		#region Styles

		/// <summary>
		/// Gets a collection that contains all the styles in the <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>A <see cref="StyleCollection"/> that contains all the styles in the <see cref="Scintilla"/>.</value>
		[Category("Appearance")]
		[Description("The styles used in the Scintilla control.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StyleCollection Styles
		{
			get
			{
				return _styles;
			}
		}

		#endregion

		#region SupressControlCharacters
		private bool _supressControlCharacters = true;

		/// <summary>
		/// Gets or sets a value indicating whether characters not considered alphanumeric (ASCII values 0 through 31)
		/// are prevented as text input.
		/// </summary>
		/// <returns><c>true</c> to prevent control characters as input; otherwise, <c>false</c>.
		/// The default is <c>true</c>.</returns>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupressControlCharacters
		{
			get
			{
				return _supressControlCharacters;
			}
			set
			{
				_supressControlCharacters = value;
			}
		}
		#endregion

		#region Text

		/// <summary>
		/// Gets or sets the current text in the <see cref="Scintilla" /> control.
		/// </summary>
		/// <returns>The text displayed in the control.</returns>
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design", typeof(UITypeEditor))]
		public override string Text
		{
			get { return _textAdapter.GetText(); }
			set { _textAdapter.SetText(value); }
		}
		#endregion

		#region TextLength

		/// <summary>
		/// Gets the length of text in the <see cref="Scintilla" /> control.
		/// </summary>
		/// <value>The number of characters contained in the text of the <see cref="Scintilla" /> control.</value>
		[Browsable(false)]
		public int TextLength
		{
			get
			{
				// The text adapter maintains the current char length
				return _textAdapter.TotalCharLength();
			}
		}

		#endregion TextLength

		#region UseWaitCursor
		public new bool UseWaitCursor
		{
			get
			{
				return base.UseWaitCursor;
			}
			set
			{
				base.UseWaitCursor = value;

				if (value)
					NativeInterface.SetCursor((int)Constants.SC_CURSORWAIT);
				else
					NativeInterface.SetCursor(unchecked((int)Constants.SC_CURSORNORMAL));
			}
		}
		#endregion

		#region Whitespace

		/// <summary>
		/// Gets the <see cref="Whitespace"/> display mode and style behavior associated with the <see cref="Scintilla"/> control.
		/// </summary>
		/// <returns>A <see cref="Whitespace"/> object that represents whitespace display mode and style behavior in a <see cref="Scintilla"/> control.</returns>
		[Category("Appearance"), Description("The display mode and style of whitespace characters.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Whitespace Whitespace
		{
			get { return _whitespace; }
		}

		#endregion Whitespace

		#region Wrapping

		/// <summary>
		/// Gets an object that controls text wrapping options in the <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>A <see cref="Wrapping"/> object that manages text wrapping options in a <see cref="Scintilla"/> control.</value>
		[CategoryAttribute("Behavior"), Description("The control's wrapping options.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Wrapping Wrapping
		{
			get
			{
				if (_wrapping == null)
					_wrapping = CreateWrappingInstance();

				return _wrapping;
			}
		}

		#endregion Wrapping

		#region ZoomFactor

		/// <summary>
		/// Gets or sets the current number of points added to or subtracted from the
		/// font size when text is rendered in a <see cref="Scintilla" /> control.
		/// </summary>
		/// <returns>The number of points by which the contents of the control is zoomed.</returns>
		[DefaultValue(0), Category("Behavior")]
		[Description("Defines the current number of points added to or subtracted from the font size when rendered; 0 is normal viewing.")]
		public int ZoomFactor
		{
			get
			{
				return (int)DirectMessage(Constants.SCI_GETZOOM, IntPtr.Zero, IntPtr.Zero);
			}
			set
			{
				if (value < -10 || value > 20)
					throw new ArgumentOutOfRangeException("value", "The value must be between -10 and 20.");

				DirectMessage(Constants.SCI_SETZOOM, (IntPtr)value, IntPtr.Zero);

				// Scintilla will raise the zoom event for us
			}
		}
		#endregion

		#endregion

		#region Private Methods

		private void ResetStyles()
		{
			// One of the core appearance properties has changed. When this happens
			// we restyle the document (overriding any existing styling) in the core
			// appearance properties. This behavior is consistent with the RichTextBox.
			NativeInterface.StartStyling(0, 0x7F);
			NativeInterface.SetStyling(NativeInterface.GetLength(), 0);
			Styles[0].Reset();
			Styles[0].Font = Font;
			Styles[0].ForeColor = ForeColor;
			Styles[0].BackColor = BackColor;
			Styles.DefaultStyle.BackColor = BackColor;
		}


		private void ScnModified(ref Message m)
		{
			NativeMethods.SCNotification scn = (NativeMethods.SCNotification)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.SCNotification));

			/*
			System.Diagnostics.Debug.WriteLine(String.Format("modificationType: {{{0}}}, position: {1}, length: {2}, linesAdded: {3}",
				Utilities.PrintModificationType(scn.modificationType),
				scn.position,
				scn.length,
				scn.linesAdded));
			*/

			// Keep the text adapter up-to-date or worlds will collide
			if ((scn.modificationType & Constants.SC_MOD_INSERTTEXT) == Constants.SC_MOD_INSERTTEXT || (scn.modificationType & Constants.SC_MOD_DELETETEXT) == Constants.SC_MOD_DELETETEXT)
				_textAdapter.ModifiedCallback(scn.position, scn.linesAdded);
		}


		private void ScnUpdateUI(ref Message m)
		{
			OnUpdateUI(EventArgs.Empty);

			// The SCN_UPDATEUI Notification message is sent whenever text, style or
			// selection range changes. This means that the SelectionChangeEvent could
			// potentially fire without the selection actually having changed. However
			// I feel that it's important enough that SelectionChanged gets its own
			// event.

			if (_lastSelection.Start != this.Selection.Start
			|| _lastSelection.End != this.Selection.End
			|| _lastSelection.Length != this.Selection.Length)
			{
				// TODO Validate that this is working correctly
				OnSelectionChanged(EventArgs.Empty);
			}

			//I think the selection changed event should only be fired if the selection differs
			//from the selection values in the previous call to this function
			//so here we take note of what the selection was last time 
			_lastSelection.Start = this.Selection.Start;
			_lastSelection.End = this.Selection.End;
			_lastSelection.Length = this.Selection.Length;
		}


		private void ScnZoom(ref Message m)
		{
			OnZoomFactorChanged(EventArgs.Empty);
		}


		private void WmNcPaintThemeBorder(ref Message m)
		{
			IntPtr hWnd = m.HWnd;
			IntPtr hRgn = m.WParam;
			
			// Get some graphics to paint with
			IntPtr hDc = NativeMethods.GetWindowDC(hWnd);
			if (hDc == IntPtr.Zero)
			{
				// We should treat this as a non-critical error because
				// we can still fall back to a non-themed border.
				MessageBox.Show("null dc");
				base.WndProc(ref m);
				return;
			}

			try
			{
				// Get the window and clipping dimensions
				RECT wndRect;
				NativeMethods.GetWindowRect(hWnd, out wndRect);
				Size brdrSize = SystemInformation.Border3DSize;
				RECT clipRect = new RECT(
					wndRect.Left + brdrSize.Width,
					wndRect.Top + brdrSize.Height,
					wndRect.Right - brdrSize.Width,
					wndRect.Bottom - brdrSize.Height);

				// Create a region that includes everything that needs to be painted
				// EXCEPT the border. That way we can pass it down to the default
				// proc and have it draw everything but the border, i.e. scrollbars.
				bool weOwnRgn = false;
				if (hRgn == (IntPtr)1 || hRgn == IntPtr.Zero)
				{
					// There is no region already defined so we'll create
					// our own region that the default proc can use.
					hRgn = NativeMethods.CreateRectRgnIndirect(ref clipRect);
					weOwnRgn = true;
				}
				else
				{
					// There is already a region defined. If we combine it with our clip
					// rect we'll be sure the resulting region won't overlap the border.
					IntPtr clipRgn = NativeMethods.CreateRectRgnIndirect(ref clipRect);
					NativeMethods.CombineRgn(hRgn, hRgn, clipRgn, NativeMethods.RGN_AND);
					NativeMethods.DeleteObject(clipRgn);
				}

				// Do the default painting
				m.WParam = hRgn;
				DefWndProc(ref m);

				// Don't do any further processing after we're done
				m.Result = IntPtr.Zero;

				// Cleanup any new regions
				if (weOwnRgn)
					NativeMethods.DeleteObject(hRgn);

				using (Graphics graphics = Graphics.FromHdc(hDc))
				{
					// Switching to the managed graphics object makes painting easier,
					// but it also means we have to convert to window coordinates.
					Rectangle wndRectangle = new Rectangle(0, 0, wndRect.Width, wndRect.Height);
					Rectangle clipRectangle = Rectangle.Inflate(wndRectangle, -brdrSize.Width, -brdrSize.Height);
					
					// We now want the opposite of the previous clipping operations.
					// Now we want to paint ONLY in the border and nowhere else.
					graphics.ExcludeClip(clipRectangle);

					// Determine the border state
					VisualStyleElement element = VisualStyleElement.TextBox.TextEdit.Normal;
					if (!Enabled)
						element = VisualStyleElement.TextBox.TextEdit.Disabled;
					else if (ReadOnly)
						element = VisualStyleElement.TextBox.TextEdit.ReadOnly;
					else if (Focused)
						element = VisualStyleElement.TextBox.TextEdit.Focused;

					// Configure the renderer
					if (_visualStyleRenderer == null)
						_visualStyleRenderer = new VisualStyleRenderer(element);
					else
						_visualStyleRenderer.SetParameters(element);

					// Paint the theme border
					if (_visualStyleRenderer.IsBackgroundPartiallyTransparent())
						_visualStyleRenderer.DrawParentBackground(graphics, wndRectangle, this);
					_visualStyleRenderer.DrawBackground(graphics, wndRectangle);
				}
			}
			finally
			{
				// Release the device context
				NativeMethods.ReleaseDC(hWnd, hDc);
			}
		}


		private void handleFileDrop(IntPtr hDrop)
		{
			StringBuilder buffer = null;
			uint nfiles = NativeMethods.DragQueryFile(hDrop, 0xffffffff, buffer, 0);
			List<string> files = new List<string>();
			for (uint i = 0; i < nfiles; i++)
			{
				buffer = new StringBuilder(512);

				NativeMethods.DragQueryFile(hDrop, i, buffer, 512);
				files.Add(buffer.ToString());
			}
			NativeMethods.DragFinish(hDrop);

			OnFileDrop(new FileDropEventArgs(files.ToArray()));
		}


		private List<ManagedRange> managedRangesInRange(int firstPos, int lastPos)
		{
			//	TODO: look into optimizing this so that it isn't a linear
			//	search. This is fine for a few markers per document but
			//	can be greatly improved if there are a large # of markers
			List<ManagedRange> ret = new List<ManagedRange>();
			foreach (ManagedRange mr in _managedRanges)
				if (mr.Start >= firstPos && mr.Start <= lastPos)
					ret.Add(mr);

			return ret;
		}


		private void paintRanges(Graphics g)
		{
			//	First we want to get the range (in positions) of what
			//	will be painted so that we know which markers to paint
			int firstLine = _ns.GetFirstVisibleLine();
			int lastLine = firstLine + _ns.LinesOnScreen();
			int firstPos = _ns.PositionFromLine(firstLine);
			int lastPos = _ns.PositionFromLine(lastLine + 1) - 1;

			//	If the lastLine was outside the defined document range it will
			//	contain -1, defualt it to the last doc position
			if (lastPos < 0)
				lastPos = _ns.GetLength();

			List<ManagedRange> mrs = managedRangesInRange(firstPos, lastPos);


			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			foreach (ManagedRange mr in mrs)
			{
				mr.Paint(g);
			}
		}

		#endregion

		#region Events
		private static readonly object _loadEventKey = new object();
		private static readonly object _textInsertedEventKey = new object();
		private static readonly object _textDeletedEventKey = new object();
		private static readonly object _beforeTextInsertEventKey = new object();
		private static readonly object _beforeTextDeleteEventKey = new object();
		private static readonly object _documentChangeEventKey = new object();
		private static readonly object _foldChangedEventKey = new object();
		private static readonly object _markerChangedEventKey = new object();
		private static readonly object _styleNeededEventKey = new object();
		private static readonly object _charAddedEventKey = new object();
		private static readonly object _modifiedChangedEventKey = new object();
		private static readonly object _readOnlyModifyAttemptEventKey = new object();
		private static readonly object _selectionChangedEventKey = new object();
		private static readonly object _linesNeedShownEventKey = new object();
		private static readonly object _uriDroppedEventKey = new object();
		private static readonly object _dwellStartEventKey = new object();
		private static readonly object _dwellEndEventKey = new object();
		private static readonly object _zoomFactorChangedEventKey = new object();
		private static readonly object _hotspotClickedEventKey = new object();
		private static readonly object _hotspotDoubleClickedEventKey = new object();
		private static readonly object _dropMarkerCollectEventKey = new object();
		private static readonly object _callTipClickEventKey = new object();
		private static readonly object _autoCompleteAcceptedEventKey = new object();
		private static readonly object _marginClickEventKey = new object();
		private static readonly object _indicatorClickEventKey = new object();
		private static readonly object _scrollEventKey = new object();
		private static readonly object _macroRecordEventKey = new object();
		private static readonly object _userListEventKey = new object();
		private static readonly object _fileDropEventKey = new object();
		private static readonly object _textChangedEventKey = new object();
		private static readonly object _borderStyleChangedEventKey = new object();
		private static readonly object _scrollingPropertyChangedEventKey = new object();
		private static readonly object _wrappingPropertyChangedEventKey = new object();
		private static readonly object _updateUIEventKey = new object();
		private static readonly object _hideSelectionChangedEventKey = new object();
		private static readonly object _readOnlyChangedEventKey = new object();


		#region Load

		/// <summary>
		/// Occurs when the control is first loaded.
		/// </summary>
		[Category("Behavior"), Description("Occurs when the control is first loaded.")]
		public event EventHandler Load
		{
			add { Events.AddHandler(_loadEventKey, value); }
			remove { Events.RemoveHandler(_loadEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="Load"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnLoad(EventArgs e)
		{
			EventHandler handler = Events[_loadEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region TextChanged
		/// <summary>
		/// Occurs when the text or styling of the document changes or is about to change.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the text has changed.")]
		new public event EventHandler<EventArgs> TextChanged
		{
			add { Events.AddHandler(_textChangedEventKey, value); }
			remove { Events.RemoveHandler(_textChangedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="TextChanged"/> event.
		/// </summary>
		/// <param name="e">Empty</param>
		protected virtual void OnTextChanged()
		{
			EventHandler<EventArgs> handler = Events[_textChangedEventKey] as EventHandler<EventArgs>;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion

		#region DocumentChange

		/// <summary>
		/// Occurs when the text or styling of the document changes or is about to change.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the text or styling of the document changes or is about to change.")]
		public event EventHandler<NativeScintillaEventArgs> DocumentChange
		{
			add { Events.AddHandler(_documentChangeEventKey, value); }
			remove { Events.RemoveHandler(_documentChangeEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="DocumentChange"/> event.
		/// </summary>
		/// <param name="e">An <see cref="NativeScintillaEventArgs"/> that contains the event data.</param>
		protected virtual void OnDocumentChange(NativeScintillaEventArgs e)
		{
			EventHandler<NativeScintillaEventArgs> handler = Events[_documentChangeEventKey] as EventHandler<NativeScintillaEventArgs>;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region CallTipClick

		/// <summary>
		/// Occurs when a user clicks on a call tip.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a user clicks on a call tip.")]
		public event EventHandler<CallTipClickEventArgs> CallTipClick
		{
			add { Events.AddHandler(_callTipClickEventKey, value); }
			remove { Events.RemoveHandler(_callTipClickEventKey, value); }
		}

		internal void FireCallTipClick(int arrow)
		{
			CallTipArrow a = (CallTipArrow)arrow;
			OverloadList ol = CallTip.OverloadList;
			CallTipClickEventArgs e;

			if (ol == null)
			{
				e = new CallTipClickEventArgs(a, -1, -1, null, CallTip.HighlightStart, CallTip.HighlightEnd);
			}
			else
			{
				int newIndex = ol.CurrentIndex;

				if (a == CallTipArrow.Down)
				{
					if (ol.CurrentIndex == ol.Count - 1)
						newIndex = 0;
					else
						newIndex++;
				}
				else if (a == CallTipArrow.Up)
				{
					if (ol.CurrentIndex == 0)
						newIndex = ol.Count - 1;
					else
						newIndex--;
				}

				e = new CallTipClickEventArgs(a, ol.CurrentIndex, newIndex, ol, CallTip.HighlightStart, CallTip.HighlightEnd);
			}

			OnCallTipClick(e);

			if (e.Cancel)
			{
				CallTip.Cancel();
			}
			else
			{
				if (ol != null)
				{
					//	We allow them to alse replace the list entirely or just
					//	manipulate the New Index
					CallTip.OverloadList = e.OverloadList;
					CallTip.OverloadList.CurrentIndex = e.NewIndex;
					CallTip.ShowOverloadInternal();
				}

				CallTip.HighlightStart = e.HighlightStart;
				CallTip.HighlightEnd = e.HighlightEnd;
			}
		}


		/// <summary>
		/// Raises the <see cref="CallTipClick"/> event.
		/// </summary>
		/// <param name="e">An <see cref="CallTipClickEventArgs"/> that contains the event data.</param>
		protected virtual void OnCallTipClick(CallTipClickEventArgs e)
		{
			EventHandler<CallTipClickEventArgs> handler = Events[_callTipClickEventKey] as EventHandler<CallTipClickEventArgs>;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region AutoCompleteAccepted

		/// <summary>
		/// Occurs when the user makes a selection from the auto-complete list.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the user makes a selection from the auto-complete list.")]
		public event EventHandler<AutoCompleteAcceptedEventArgs> AutoCompleteAccepted
		{
			add { Events.AddHandler(_autoCompleteAcceptedEventKey, value); }
			remove { Events.RemoveHandler(_autoCompleteAcceptedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="AutoCompleteAccepted"/> event.
		/// </summary>
		/// <param name="e">An <see cref="AutoCompleteAcceptedEventArgs"/> that contains the event data.</param>
		protected virtual void OnAutoCompleteAccepted(AutoCompleteAcceptedEventArgs e)
		{
			EventHandler<AutoCompleteAcceptedEventArgs> handler = Events[_autoCompleteAcceptedEventKey] as EventHandler<AutoCompleteAcceptedEventArgs>;
			if (handler != null)
				handler(this, e);

			if (e.Cancel)
				AutoComplete.Cancel();
		}

		#endregion

		#region TextInserted

		/// <summary>
		/// Occurs when text has been inserted into the document.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when text has been inserted into the document.")]
		public event EventHandler<TextModifiedEventArgs> TextInserted
		{
			add { Events.AddHandler(_textInsertedEventKey, value); }
			remove { Events.RemoveHandler(_textInsertedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="TextInserted"/> event.
		/// </summary>
		/// <param name="e">An <see cref="TextModifiedEventArgs"/> that contains the event data.</param>
		protected virtual void OnTextInserted(TextModifiedEventArgs e)
		{
			EventHandler<TextModifiedEventArgs> handler = Events[_textInsertedEventKey] as EventHandler<TextModifiedEventArgs>;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region TextDeleted

		/// <summary>
		/// Occurs when text has been removed from the document.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when text has been removed from the document.")]
		public event EventHandler<TextModifiedEventArgs> TextDeleted
		{
			add { Events.AddHandler(_textDeletedEventKey, value); }
			remove { Events.RemoveHandler(_textDeletedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="TextDeleted"/> event.
		/// </summary>
		/// <param name="e">An <see cref="TextModifiedEventArgs"/> that contains the event data.</param>
		protected virtual void OnTextDeleted(TextModifiedEventArgs e)
		{
			EventHandler<TextModifiedEventArgs> handler = Events[_textDeletedEventKey] as EventHandler<TextModifiedEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region BeforeTextInsert

		/// <summary>
		/// Occurs when text is about to be inserted into the document.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when text is about to be inserted into the document.")]
		public event EventHandler<TextModifiedEventArgs> BeforeTextInsert
		{
			add { Events.AddHandler(_beforeTextInsertEventKey, value); }
			remove { Events.RemoveHandler(_beforeTextInsertEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="BeforeTextInsert"/> event.
		/// </summary>
		/// <param name="e">An <see cref="TextModifiedEventArgs"/> that contains the event data.</param>
		protected virtual void OnBeforeTextInsert(TextModifiedEventArgs e)
		{
			List<ManagedRange> offsetRanges = new List<ManagedRange>();
			foreach (ManagedRange mr in _managedRanges)
			{
				if (mr.Start == e.Position && mr.PendingDeletion)
				{
					mr.PendingDeletion = false;
					ManagedRange lmr = mr;
					BeginInvoke(new MethodInvoker(delegate() { lmr.Change(e.Position, e.Position + e.Length); }));
				}

				//	If the Range is a single point we treat it slightly 
				//	differently than a spanned range
				if (mr.IsPoint)
				{
					//	Unlike a spanned range, if the insertion point of
					//	the new text == the start of the range (and thus
					//	the end as well) we offset the entire point.
					if (mr.Start >= e.Position)
						mr.Change(mr.Start + e.Length, mr.End + e.Length);
					else if (mr.End >= e.Position)
						mr.Change(mr.Start, mr.End + e.Length);
				}
				else
				{
					//	We offset a spanned range entirely only if the
					//	start occurs after the insertion point of the new
					//	text.
					if (mr.Start > e.Position)
						mr.Change(mr.Start + e.Length, mr.End + e.Length);
					else if (mr.End >= e.Position)
					{
						//	However it the start of the range == the insertion
						//	point of the new text instead of offestting the 
						//	range we expand it.
						mr.Change(mr.Start, mr.End + e.Length);
					}
				}

			}

			EventHandler<TextModifiedEventArgs> handler = Events[_beforeTextInsertEventKey] as EventHandler<TextModifiedEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region BeforeTextDelete

		/// <summary>
		/// Occurs when text is about to be removed from the document.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when text is about to be removed from the document.")]
		public event EventHandler<TextModifiedEventArgs> BeforeTextDelete
		{
			add { Events.AddHandler(_beforeTextDeleteEventKey, value); }
			remove { Events.RemoveHandler(_beforeTextDeleteEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="BeforeTextDelete"/> event.
		/// </summary>
		/// <param name="e">An <see cref="TextModifiedEventArgs"/> that contains the event data.</param>
		protected virtual void OnBeforeTextDelete(TextModifiedEventArgs e)
		{
			int firstPos = e.Position;
			int lastPos = firstPos + e.Length;

			List<ManagedRange> deletedRanges = new List<ManagedRange>();
			foreach (ManagedRange mr in _managedRanges)
			{

				//	These ranges lie within the deleted range so
				//	the ranges themselves need to be deleted
				if (mr.Start >= firstPos && mr.End <= lastPos)
				{

					//	If the entire range is being delete and NOT a superset of the range,
					//	don't delete it, only collapse it.
					if (!mr.IsPoint && e.Position == mr.Start && (e.Position + e.Length == mr.End))
					{
						mr.Change(mr.Start, mr.Start);
					}
					else
					{
						//	Notify the virtual Range that it needs to cleanup
						mr.Change(-1, -1);

						//	Mark for deletion after this foreach:
						deletedRanges.Add(mr);

					}
				}
				else if (mr.Start >= lastPos)
				{
					//	These ranges are merely offset by the deleted range
					mr.Change(mr.Start - e.Length, mr.End - e.Length);
				}
				else if (mr.Start >= firstPos && mr.Start <= lastPos)
				{
					//	The left side of the managed range is getting
					//	cut off
					mr.Change(firstPos, mr.End - e.Length);
				}
				else if (mr.Start < firstPos && mr.End >= firstPos && mr.End >= lastPos)
				{
					mr.Change(mr.Start, mr.End - e.Length);
				}
				else if (mr.Start < firstPos && mr.End >= firstPos && mr.End < lastPos)
				{
					mr.Change(mr.Start, firstPos);
				}

			}

			foreach (ManagedRange mr in deletedRanges)
				mr.Dispose();

			EventHandler<TextModifiedEventArgs> handler = Events[_beforeTextDeleteEventKey] as EventHandler<TextModifiedEventArgs>;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region FoldChanged

		/// <summary>
		/// Occurs when a folding change has occurred.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a folding change has occurred.")]
		public event EventHandler<FoldChangedEventArgs> FoldChanged
		{
			add { Events.AddHandler(_foldChangedEventKey, value); }
			remove { Events.RemoveHandler(_foldChangedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="FoldChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="FoldChangedEventArgs"/> that contains the event data.</param>
		protected virtual void OnFoldChanged(FoldChangedEventArgs e)
		{
			EventHandler<FoldChangedEventArgs> handler = Events[_foldChangedEventKey] as EventHandler<FoldChangedEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region MarkerChanged

		/// <summary>
		/// Occurs when one or more markers has changed in a line of text.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when one or more markers has changed in a line of text.")]
		public event EventHandler<MarkerChangedEventArgs> MarkerChanged
		{
			add { Events.AddHandler(_markerChangedEventKey, value); }
			remove { Events.RemoveHandler(_markerChangedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="MarkerChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="MarkerChangedEventArgs"/> that contains the event data.</param>
		protected virtual void OnMarkerChanged(MarkerChangedEventArgs e)
		{
			EventHandler<MarkerChangedEventArgs> handler = Events[_markerChangedEventKey] as EventHandler<MarkerChangedEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region IndicatorClick

		/// <summary>
		/// Occurs when the a clicks or releases the mouse on text that has an indicator.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the a clicks or releases the mouse on text that has an indicator.")]
		public event EventHandler<ScintillaMouseEventArgs> IndicatorClick
		{
			add { Events.AddHandler(_indicatorClickEventKey, value); }
			remove { Events.RemoveHandler(_indicatorClickEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="IndicatorClick"/> event.
		/// </summary>
		/// <param name="e">An <see cref="ScintillaMouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnIndicatorClick(ScintillaMouseEventArgs e)
		{
			EventHandler<ScintillaMouseEventArgs> handler = Events[_indicatorClickEventKey] as EventHandler<ScintillaMouseEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region MarginClick

		/// <summary>
		/// Occurs when the mouse was clicked inside a margin that was marked as sensitive.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the mouse was clicked inside a margin that was marked as sensitive.")]
		public event EventHandler<MarginClickEventArgs> MarginClick
		{
			add { Events.AddHandler(_marginClickEventKey, value); }
			remove { Events.RemoveHandler(_marginClickEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="MarginClick"/> event.
		/// </summary>
		/// <param name="e">An <see cref="MarginClickEventArgs"/> that contains the event data.</param>
		protected virtual void OnMarginClick(MarginClickEventArgs e)
		{
			EventHandler<MarginClickEventArgs> handler = Events[_marginClickEventKey] as EventHandler<MarginClickEventArgs>;
			if (handler != null)
				handler(this, e);

			if (e.ToggleMarkerNumber >= 0)
			{
				int mask = (int)Math.Pow(2, e.ToggleMarkerNumber);
				if ((e.Line.GetMarkerMask() & mask) == mask)
					e.Line.DeleteMarker(e.ToggleMarkerNumber);
				else
					e.Line.AddMarker(e.ToggleMarkerNumber);
			}

			if (e.ToggleFold)
				e.Line.ToggleFoldExpanded();
		}

		internal void FireMarginClick(SCNotification n)
		{
			Margin m = Margins[n.margin];
			Keys k = Keys.None;

			if ((n.modifiers & (int)KeyMod.Alt) == (int)KeyMod.Alt)
				k |= Keys.Alt;

			if ((n.modifiers & (int)KeyMod.Ctrl) == (int)KeyMod.Ctrl)
				k |= Keys.Control;

			if ((n.modifiers & (int)KeyMod.Shift) == (int)KeyMod.Shift)
				k |= Keys.Shift;

			OnMarginClick(new MarginClickEventArgs(k, n.position, Lines.FromPosition(n.position), m, m.AutoToggleMarkerNumber, m.IsFoldMargin));
		}

		#endregion

		#region StyleNeeded

		/// <summary>
		/// Occurs when the control is about to display or print text that requires styling.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the control is about to display or print text that requires styling.")]
		public event EventHandler<StyleNeededEventArgs> StyleNeeded
		{
			add { Events.AddHandler(_styleNeededEventKey, value); }
			remove { Events.RemoveHandler(_styleNeededEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="StyleNeeded"/> event.
		/// </summary>
		/// <param name="e">An <see cref="StyleNeededEventArgs"/> that contains the event data.</param>
		protected virtual void OnStyleNeeded(StyleNeededEventArgs e)
		{
			EventHandler<StyleNeededEventArgs> handler = Events[_styleNeededEventKey] as EventHandler<StyleNeededEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region CharAdded

		/// <summary>
		/// Occurs when the user types an ordinary text character (as opposed to a command character) into the text.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the user types a text character.")]
		public event EventHandler<CharAddedEventArgs> CharAdded
		{
			add { Events.AddHandler(_charAddedEventKey, value); }
			remove { Events.RemoveHandler(_charAddedEventKey, value); }
		}


		/// <summary>
		/// Raises the <see cref="CharAdded"/> event.
		/// </summary>
		/// <param name="e">An <see cref="CharAddedEventArgs"/> that contains the event data.</param>
		protected virtual void OnCharAdded(CharAddedEventArgs e)
		{
			EventHandler<CharAddedEventArgs> handler = Events[_charAddedEventKey] as EventHandler<CharAddedEventArgs>;
			if (handler != null)
				handler(this, e);

			if (_indentation.SmartIndentType != SmartIndent.None)
				_indentation.CheckSmartIndent(e.Ch);
		}
		#endregion

		#region ModifiedChanged

		/// <summary>
		/// Occurs when the value of the <see cref="Modified"> property has changed.
		/// </summary>
		[Category("Property Changed"), Description("Occurs when the value of the Modified property changes.")]
		public event EventHandler ModifiedChanged
		{
			add { Events.AddHandler(_modifiedChangedEventKey, value); }
			remove { Events.RemoveHandler(_modifiedChangedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="ModifiedChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnModifiedChanged(EventArgs e)
		{
			EventHandler handler = Events[_modifiedChangedEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion ModifiedChanged

		#region ReadOnlyChanged

		/// <summary>
		/// Occurs when the value of the <see cref="ReadOnly"/> property has changed.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the value of the ReadOnly property changes.")]
		public event EventHandler ReadOnlyChanged
		{
			add
			{
				Events.AddHandler(_readOnlyChangedEventKey, value);
			}
			remove
			{
				Events.RemoveHandler(_readOnlyChangedEventKey, value);
			}
		}


		/// <summary>
		/// Raises the <see cref="ReadOnlyChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			EventHandler handler = Events[_readOnlyChangedEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion ReadOnlyChanged

		#region ReadOnlyModifyAttempt

		/// <summary>
		/// Occurs when a user tries to modify text when in read-only mode.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a user tries to modifiy text when in read-only mode.")]
		public event EventHandler ReadOnlyModifyAttempt
		{
			add { Events.AddHandler(_readOnlyModifyAttemptEventKey, value); }
			remove { Events.RemoveHandler(_readOnlyModifyAttemptEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="ReadOnlyModifyAttempt"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnReadOnlyModifyAttempt(EventArgs e)
		{
			EventHandler handler = Events[_readOnlyModifyAttemptEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region SelectionChanged

		/// <summary>
		/// Occurs when the selection has changed.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the selection has changed.")]
		public event EventHandler SelectionChanged
		{
			add { Events.AddHandler(_selectionChangedEventKey, value); }
			remove { Events.RemoveHandler(_selectionChangedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectionChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnSelectionChanged(EventArgs e)
		{	//this is being fired in tandem with the cursor blink...
			EventHandler handler = Events[_selectionChangedEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);

			if (_isBraceMatching && (_selection.Length == 0))
			{
				int position = CurrentPos - 1,
					bracePosStart = -1,
					bracePosEnd = -1;

				char character = (char)CharAtPosition(position);

				switch (character)
				{
					case '{':
					case '}':
					case '(':
					case ')':
					case '[':
					case ']':
						if (!this.PositionIsOnComment(position))
						{
							bracePosStart = position;
							bracePosEnd = _ns.BraceMatch(position,0);

							if(bracePosEnd >= 0)
							{
								_ns.BraceHighlight(bracePosStart, bracePosEnd);
							}
							else
							{
								_ns.BraceBadLight(bracePosStart);
							}
						}
						break;
					default:
						position = CurrentPos;
						character = (char)CharAtPosition(position); //this is not being used anywhere... --Cory
						_ns.BraceHighlight(bracePosStart, bracePosEnd);
						break;
				}
			}
		}

		#endregion

		#region LinesNeedShown

		/// <summary>
		/// Occurs when a range of lines that is currently invisible should be made visible.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a range of lines that is currently invisible should be made visible.")]
		public event EventHandler<LinesNeedShownEventArgs> LinesNeedShown
		{
			add { Events.AddHandler(_linesNeedShownEventKey, value); }
			remove { Events.RemoveHandler(_linesNeedShownEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="LinesNeedShown"/> event.
		/// </summary>
		/// <param name="e">An <see cref="LinesNeedShownEventArgs"/> that contains the event data.</param>
		protected virtual void OnLinesNeedShown(LinesNeedShownEventArgs e)
		{
			EventHandler<LinesNeedShownEventArgs> handler = Events[_linesNeedShownEventKey] as EventHandler<LinesNeedShownEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region UriDropped

		//[Category("Scintilla")]
		//public event EventHandler<UriDroppedEventArgs> UriDropped
		//{
		//    add { Events.AddHandler(_uriDroppedEventKey, value); }
		//    remove { Events.RemoveHandler(_uriDroppedEventKey, value); }
		//}

		//protected virtual void OnUriDropped(UriDroppedEventArgs e)
		//{
		//    EventHandler<UriDroppedEventArgs> handler = Events[_uriDroppedEventKey] as EventHandler<UriDroppedEventArgs>;
		//    if (handler != null)
		//        handler(this, e);
		//}

		#endregion

		#region DwellStart

		/// <summary>
		/// Occurs when the user hovers the mouse (dwells) in one position for the dwell period.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the user hovers the mouse (dwells) in one position for the dwell period.")]
		public event EventHandler<ScintillaMouseEventArgs> DwellStart
		{
			add { Events.AddHandler(_dwellStartEventKey, value); }
			remove { Events.RemoveHandler(_dwellStartEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="DwellStart"/> event.
		/// </summary>
		/// <param name="e">An <see cref="ScintillaMouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnDwellStart(ScintillaMouseEventArgs e)
		{
			EventHandler<ScintillaMouseEventArgs> handler = Events[_dwellStartEventKey] as EventHandler<ScintillaMouseEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region DwellEnd

		/// <summary>
		/// Occurs when a user actions such as a mouse move or key press ends a dwell (hover) activity.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a dwell (hover) activity has ended.")]
		public event EventHandler<ScintillaMouseEventArgs> DwellEnd
		{
			add { Events.AddHandler(_dwellEndEventKey, value); }
			remove { Events.RemoveHandler(_dwellEndEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="DwellEnd"/> event.
		/// </summary>
		/// <param name="e">An <see cref="ScintillaMouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnDwellEnd(ScintillaMouseEventArgs e)
		{
			EventHandler<ScintillaMouseEventArgs> handler = Events[_dwellEndEventKey] as EventHandler<ScintillaMouseEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region ZoomFactorChanged

		/// <summary>
		/// Occurs when the value of the <see cref="ZoomFactor"/> property changes.
		/// </summary>
		[Category("Property Changed"), Description("Occurs when the value of the ZoomFactor property changes.")]
		public event EventHandler ZoomFactorChanged
		{
			add
			{
				Events.AddHandler(_zoomFactorChangedEventKey, value);
			}
			remove
			{
				Events.RemoveHandler(_zoomFactorChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the <see cref="ZoomFactorChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnZoomFactorChanged(EventArgs e)
		{
			EventHandler handler = Events[_zoomFactorChangedEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion ZoomFactorChanged

		#region HideSelectionChanged

		/// <summary>
		/// Occurs when the value of the <see cref="HideSelection"/> property has changed.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the value of the HideSelection property chagnes.")]
		public event EventHandler HideSelectionChanged
		{
			add
			{
				Events.AddHandler(_hideSelectionChangedEventKey, value);
			}
			remove
			{
				Events.RemoveHandler(_hideSelectionChangedEventKey, value);
			}
		}


		/// <summary>
		/// Raise the <see cref="HideSelectionChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data. </param>
		protected virtual void OnHideSelectionChanged(EventArgs e)
		{
			EventHandler handler = Events[_hideSelectionChangedEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion HideSelectionChanged

		#region HotspotClick

		/// <summary>
		/// Occurs when a user clicks on text that is in a style with the hotspot attribute set.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a user clicks on text with the hotspot style.")]
		public event EventHandler<ScintillaMouseEventArgs> HotspotClick
		{
			add { Events.AddHandler(_hotSpotClickEventKey, value); }
			remove { Events.RemoveHandler(_hotSpotClickEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="HotspotClick"/> event.
		/// </summary>
		/// <param name="e">An <see cref="ScintillaMouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnHotspotClick(ScintillaMouseEventArgs e)
		{
			EventHandler<ScintillaMouseEventArgs> handler = Events[_hotSpotClickEventKey] as EventHandler<ScintillaMouseEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region HotspotDoubleClick

		/// <summary>
		/// Occurs when a user double-clicks on text that is in a style with the hotspot attribute set.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a user double-clicks on text with the hotspot style.")]
		public event EventHandler<ScintillaMouseEventArgs> HotspotDoubleClick
		{
			add { Events.AddHandler(_hotSpotDoubleClickEventKey, value); }
			remove { Events.RemoveHandler(_hotSpotDoubleClickEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="HotspotDoubleClick"/> event.
		/// </summary>
		/// <param name="e">An <see cref="ScintillaMouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnHotspotDoubleClick(ScintillaMouseEventArgs e)
		{
			EventHandler<ScintillaMouseEventArgs> handler = Events[_hotSpotDoubleClickEventKey] as EventHandler<ScintillaMouseEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region DropMarkerCollect

		/// <summary>
		/// Occurs when a <see cref="DropMarker"/> is about to be collected.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a DropMarker is about to be collected.")]
		public event EventHandler<DropMarkerCollectEventArgs> DropMarkerCollect
		{
			add { Events.AddHandler(_dropMarkerCollectEventKey, value); }
			remove { Events.RemoveHandler(_dropMarkerCollectEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="DropMarkerCollect"/> event.
		/// </summary>
		/// <param name="e">An <see cref="DropMarkerCollectEventArgs"/> that contains the event data.</param>
		protected internal virtual void OnDropMarkerCollect(DropMarkerCollectEventArgs e)
		{
			EventHandler<DropMarkerCollectEventArgs> handler = Events[_dropMarkerCollectEventKey] as EventHandler<DropMarkerCollectEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region Scroll

		/// <summary>
		/// Occurs when the control is scrolled.
		/// </summary>
		[Category("Action"), Description("Occurs when the control is scrolled.")]
		public event EventHandler<ScrollEventArgs> Scroll
		{
			add { Events.AddHandler(_scrollEventKey, value); }
			remove { Events.RemoveHandler(_scrollEventKey, value); }
		}

		internal void FireScroll(ref Message m)
		{
			ScrollOrientation so = ScrollOrientation.VerticalScroll;
			int oldScroll = 0, newScroll = 0;
			ScrollEventType set = (ScrollEventType)(Util.SignedLoWord(m.WParam));
			if (m.Msg == NativeMethods.WM_HSCROLL)
			{
				so = ScrollOrientation.HorizontalScroll;
				oldScroll = _ns.GetXOffset();

				//	Let Scintilla Handle the scroll Message to actually perform scrolling
				base.WndProc(ref m);
				newScroll = _ns.GetXOffset();
			}
			else
			{
				so = ScrollOrientation.VerticalScroll;
				oldScroll = _ns.GetFirstVisibleLine();
				base.WndProc(ref m);
				newScroll = _ns.GetFirstVisibleLine();
			}

			OnScroll(new ScrollEventArgs(set, oldScroll, newScroll, so));
		}

		/// <summary>
		/// Raises the <see cref="Scroll"/> event.
		/// </summary>
		/// <param name="e">An <see cref="ScrollEventArgs"/> that contains the event data.</param>
		protected virtual void OnScroll(ScrollEventArgs e)
		{
			EventHandler<ScrollEventArgs> handler = Events[_scrollEventKey] as EventHandler<ScrollEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region MacroRecord

		/// <summary>
		/// Occurs each time a recordable change occurs.
		/// </summary>
		[Category("Scintilla"), Description("Occurs each time a recordable change occurs.")]
		public event EventHandler<MacroRecordEventArgs> MacroRecord
		{
			add { Events.AddHandler(_macroRecordEventKey, value); }
			remove { Events.RemoveHandler(_macroRecordEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="MacroRecord"/> event.
		/// </summary>
		/// <param name="e">An <see cref="MacroRecordEventArgs"/> that contains the event data.</param>
		protected virtual void OnMacroRecord(MacroRecordEventArgs e)
		{
			EventHandler<MacroRecordEventArgs> handler = Events[_macroRecordEventKey] as EventHandler<MacroRecordEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region FileDrop

		/// <summary>
		/// Occurs when a user drops a file on the <see cref="Scintilla"/> control.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when a user drops a file on the control.")]
		public event EventHandler<FileDropEventArgs> FileDrop
		{
			add { Events.AddHandler(_fileDropEventKey, value); }
			remove { Events.RemoveHandler(_fileDropEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="FileDrop"/> event.
		/// </summary>
		/// <param name="e">A <see cref="FileDropEventArgs"/> that contains the event data.</param>
		protected virtual void OnFileDrop(FileDropEventArgs e)
		{
			EventHandler<FileDropEventArgs> handler = Events[_fileDropEventKey] as EventHandler<FileDropEventArgs>;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region BorderStyle

		/// <summary>
		/// Occurs when the value of the <see cref="BorderStyle"/> property has changed.
		/// </summary>
		[Category("Property Changed"), Description("Occurs when the value of the BorderStyle property changes.")]
		public event EventHandler BorderStyleChanged
		{
			add { Events.AddHandler(_borderStyleChangedEventKey, value); }
			remove { Events.RemoveHandler(_borderStyleChangedEventKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="BorderStyleChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			EventHandler handler = Events[_borderStyleChangedEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion BorderStyle

		#region ScrollingPropertyChanged

		/// <summary>
		/// Occurs when one of <see cref="Scrolling" /> property values has changed.
		/// </summary>
		[Category("Property Changed"), Description("Occurs when one of the Scrolling property values changes.")]
		public event PropertyChangedEventHandler ScrollingPropertyChanged
		{
			add { Events.AddHandler(_scrollingPropertyChangedEventKey, value); }
			remove { Events.RemoveHandler(_scrollingPropertyChangedEventKey, value); }
		}


		/// <summary>
		/// Raises the <see cref="ScrollingPropertyChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
		protected internal void OnScrollingPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = Events[_scrollingPropertyChangedEventKey] as PropertyChangedEventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion ScrollingPropertyChanged

		#region WrappingPropertyChanged

		/// <summary>
		/// Occurs when one of <see cref="Wrapping" /> property values has changed.
		/// </summary>
		[Category("Property Changed"), Description("Occurs when one of the Wrapping property values changes.")]
		public event PropertyChangedEventHandler WrappingPropertyChanged
		{
			add { Events.AddHandler(_wrappingPropertyChangedEventKey, value); }
			remove { Events.RemoveHandler(_wrappingPropertyChangedEventKey, value); }
		}


		/// <summary>
		/// Raises the <see cref="WrappingPropertyChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
		protected internal void OnWrappingPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = Events[_wrappingPropertyChangedEventKey] as PropertyChangedEventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion WrappingPropertyChanged

		#region UpdateUI

		/// <summary>
		/// Occurs when the document text, styling, selection, or caret position has changed.
		/// </summary>
		[Category("Scintilla"), Description("Occurs when the document text, styling, selection, or caret position changes.")]
		public event EventHandler UpdateUI
		{
			add { Events.AddHandler(_updateUIEventKey, value); }
			remove { Events.RemoveHandler(_updateUIEventKey, value); }
		}


		/// <summary>
		/// Raises the <see cref="UpdateUI" /> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
		protected void OnUpdateUI(EventArgs e)
		{
			EventHandler handler = Events[_updateUIEventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}

		#endregion UpdateUI

		#endregion

		#region Public Methods

		/// <summary>
		/// Appends the specified string to the end of the control's text.
		/// </summary>
		/// <param name="text">The <see cref="String"/> to append.</param>
		/// <seealso cref="DeleteText"/>
		/// <seealso cref="GetText"/>
		/// <seealso cref="InsertText"/>
		/// <seealso cref="ReplaceText"/>
		public void AppendText(string value)
		{
			if (String.IsNullOrEmpty(value))
				return;

			byte[] buffer = Encoding.GetBytes(value);
			unsafe
			{
				fixed (byte* bp = buffer)
					DirectMessage(Constants.SCI_APPENDTEXT, (IntPtr)buffer.Length, (IntPtr)bp);
			}
		}


		/// <summary>
		/// Deletes the specified range of chars from the control's text.
		/// </summary>
		/// <param name="startIndex">The char index within the control's text at which to start deleting.</param>
		/// <param name="endIndex">The char index within the control's text at which to stop deleting.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero.
		/// -or-
		/// The <paramref name="endIndex"/> is less than <paramref name="startIndex"/> or greater
		/// than the <see cref="TextLength"/> property.
		/// </exception>
		/// <seealso cref="AppendText"/>
		/// <seealso cref="GetText"/>
		/// <seealso cref="InsertText"/>
		/// <seealso cref="ReplaceText"/>
		public void DeleteText(int startIndex, int endIndex)
		{
			int textLength = TextLength;
			Util.ValidateIndex(startIndex, "startIndex", 0, "0", textLength, "TextLength", null);
			Util.ValidateIndex(endIndex, "endIndex", startIndex, "startIndex", textLength, "TextLength", null);

			ReplaceTextInternal(startIndex, endIndex, null);
		}


		/// <summary>
		/// Deletes the specified range of chars from the control's text.
		/// </summary>
		/// <param name="range">The <see cref="Range"/> to delete.</param>
		/// <exception cref="ArgumentException">
		/// The <paramref name="range"/> <c>StartIndex</c> is less than zero.
		/// -or-
		/// The <paramref name="range"/> <c>EndIndex</c> is less than <c>StartIndex</c> or greater
		/// than the <see cref="TextLength"/> property.
		/// </exception>
		/// <seealso cref="AppendText"/>
		/// <seealso cref="GetText"/>
		/// <seealso cref="InsertText"/>
		/// <seealso cref="ReplaceText"/>
		public void DeleteText(Range range)
		{
			int textLength = TextLength;
			Util.ValidateIndex(range.StartIndex, "StartIndex", 0, "0", textLength, "TextLength", "range");
			Util.ValidateIndex(range.EndIndex, "EndIndex", range.StartIndex, "StartIndex", textLength, "TextLength", "range");

			ReplaceTextInternal(range.StartIndex, range.EndIndex, null);
		}


		/// <summary>
		/// Creates a string from the specified char range of the control's text.
		/// </summary>
		/// <param name="startIndex">The char index within the control's text at which to start copying.</param>
		/// <param name="endIndex">The char index within the control's text at which to stop copying.</param>
		/// <returns>
		/// A <see cref="String"/> equivalent to the control's text between <see cref="startIndex"/>
		/// and <see cref="endIndex"/>.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero.
		/// -or-
		/// The <paramref name="endIndex"/> is less than <paramref name="startIndex"/> or greater
		/// than the <see cref="TextLength"/> property.
		/// </exception>
		/// <seealso cref="AppendText"/>
		/// <seealso cref="DeleteText"/>
		/// <seealso cref="InsertText"/>
		/// <seealso cref="ReplaceText"/>
		public string GetText(int startIndex, int endIndex)
		{
			int textLength = TextLength;
			Util.ValidateIndex(startIndex, "startIndex", 0, "0", textLength, "TextLength", null);
			Util.ValidateIndex(endIndex, "endIndex", startIndex, "startIndex", textLength, "TextLength", null);

			return GetTextInternal(startIndex, endIndex);
		}


		/// <summary>
		/// Creates a string from the specified char range of the control's text.
		/// </summary>
		/// <param name="range">The <see cref="Range"/> to copy.</param>
		/// <returns>
		/// A <see cref="String"/> equivalent to the control's text for the <paramref name="range"/> specified.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// The <paramref name="range"/> <c>StartIndex</c> is less than zero.
		/// -or-
		/// The <paramref name="range"/> <c>EndIndex</c> is less than <c>StartIndex</c> or greater
		/// than the <see cref="TextLength"/> property.
		/// </exception>
		/// <seealso cref="AppendText"/>
		/// <seealso cref="DeleteText"/>
		/// <seealso cref="InsertText"/>
		/// <seealso cref="ReplaceText"/>
		public string GetText(Range range)
		{
			int textLength = TextLength;
			Util.ValidateIndex(range.StartIndex, "StartIndex", 0, "0", textLength, "TextLength", "range");
			Util.ValidateIndex(range.EndIndex, "EndIndex", range.StartIndex, "StartIndex", textLength, "TextLength", "range");

			return GetTextInternal(range.StartIndex, range.EndIndex);
		}


		private string GetTextInternal(int startIndex, int endIndex)
		{
			Debug.Assert(startIndex >= 0);
			Debug.Assert(endIndex >= startIndex);
			Debug.Assert(endIndex <= TextLength);

			startIndex = _textAdapter.CharToByteIndex(startIndex);
			endIndex = _textAdapter.CharToByteIndex(endIndex);

			TextRange tr = new TextRange();
			tr.chrg.cpMin = startIndex;
			tr.chrg.cpMax = endIndex;
			byte[] buffer = new byte[(endIndex - startIndex) + 1]; // +1 For the terminator
			int length;
			unsafe
			{
				fixed (byte* bp = buffer)
				{
					tr.lpstrText = (IntPtr)bp;
					TextRange* trp = &tr;
					length = (int)DirectMessage(Constants.SCI_GETTEXTRANGE, IntPtr.Zero, (IntPtr)trp);
				}
			}

			Debug.Assert(length == buffer.Length - 1);
			return Encoding.GetString(buffer, 0, length);
		}


		/// <summary>
		/// Inserts the given string at the specified char index in the control's text.
		/// </summary>
		/// <param name="index">The <see cref="String"/> to insert.</param>
		/// <param name="text">The char index within the control's text to perform the operation.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The <paramref name="index"/> argument is less than zero or greater than <see cref="TextLength"/>.
		/// </exception>
		/// <seealso cref="AppendText"/>
		/// <seealso cref="DeleteText"/>
		/// <seealso cref="GetText"/>
		/// <seealso cref="ReplaceText"/>
		public void InsertText(int index, string value)
		{
			Util.ValidateIndex(index, "index", 0, "0", TextLength, "TextLength", null);
			if (String.IsNullOrEmpty(value))
				return;

			index = _textAdapter.CharToByteIndex(index);
			byte[] buffer = Util.GetBytesZeroTerminated(value, Encoding);
			unsafe
			{
				fixed (byte* bp = buffer)
					DirectMessage(Constants.SCI_INSERTTEXT, (IntPtr)index, (IntPtr)bp);
			}
		}


		/// <summary>
		/// Replaces the specified range of the control's text with the given string.
		/// </summary>
		/// <param name="startIndex">The char index within the control's text at which to start replacing.</param>
		/// <param name="endIndex">The char index within the control's text at which to stop replacing.</param>
		/// <param name="value">The <see cref="String"/> that replaces the specified range of text, or <c>null</c>.</param>
		/// <paramref name="startIndex"/> is less than zero.
		/// -or-
		/// The <paramref name="endIndex"/> is less than <paramref name="startIndex"/> or greater
		/// than the <see cref="TextLength"/> property.
		/// </exception>
		/// <seealso cref="AppendText"/>
		/// <seealso cref="DeleteText"/>
		/// <seealso cref="GetText"/>
		/// <seealso cref="InsertText"/>
		public void ReplaceText(int startIndex, int endIndex, string value)
		{
			int textLength = TextLength;
			Util.ValidateIndex(startIndex, "startIndex", 0, "0", textLength, "TextLength", null);
			Util.ValidateIndex(endIndex, "endIndex", startIndex, "startIndex", textLength, "TextLength", null);

			ReplaceTextInternal(startIndex, endIndex, value);
		}


		/// <summary>
		/// Replaces the specified range of the control's text with the given string.
		/// </summary>
		/// <param name="range">The <see cref="Range"/> to replace.</param>
		/// <param name="value">The <see cref="String"/> that replaces the specified range of text, or <c>null</c>.</param>
		/// <exception cref="ArgumentException">
		/// The <paramref name="range"/> <c>StartIndex</c> is less than zero.
		/// -or-
		/// The <paramref name="range"/> <c>EndIndex</c> is less than <c>StartIndex</c> or greater
		/// than the <see cref="TextLength"/> property.
		/// </exception>
		/// <seealso cref="AppendText"/>
		/// <seealso cref="DeleteText"/>
		/// <seealso cref="GetText"/>
		/// <seealso cref="InsertText"/>
		public void ReplaceText(Range range, string value)
		{
			int textLength = TextLength;
			Util.ValidateIndex(range.StartIndex, "StartIndex", 0, "0", textLength, "TextLength", "range");
			Util.ValidateIndex(range.EndIndex, "EndIndex", range.StartIndex, "StartIndex", textLength, "TextLength", "range");

			ReplaceTextInternal(range.StartIndex, range.EndIndex, value);
		}


		private void ReplaceTextInternal(int startIndex, int endIndex, string value)
		{
			Debug.Assert(startIndex >= 0);
			Debug.Assert(endIndex >= startIndex);
			Debug.Assert(endIndex <= TextLength);
			Debug.Assert("Han shot first" is String);

			startIndex = _textAdapter.CharToByteIndex(startIndex);
			endIndex = _textAdapter.CharToByteIndex(endIndex);

			DirectMessage(Constants.SCI_SETTARGETSTART, (IntPtr)startIndex, IntPtr.Zero);
			DirectMessage(Constants.SCI_SETTARGETEND, (IntPtr)endIndex, IntPtr.Zero);

			byte[] buffer = Encoding.GetBytes(value ?? String.Empty);
			unsafe
			{
				fixed (byte* bp = buffer)
					DirectMessage(Constants.SCI_REPLACETARGET, (IntPtr)buffer.Length, (IntPtr)bp);
			}
		}


		[Obsolete("Use InsertText for char-based indexes.")]
		internal PositionRange InsertTextInternal(int position, string text)
        {
            NativeInterface.InsertText(position, text);
            return GetRange(position, Encoding.GetByteCount(text));
        }


		public char CharAtPosition(int position)
		{
			return _ns.GetCharAt(position);
		}

		public PositionRange GetRange(int startPosition, int endPosition)
		{
			return new PositionRange(startPosition, endPosition, this);
		}

		public PositionRange GetRange()
		{
			return new PositionRange(0, _ns.GetTextLength(), this);
		}

		public int FindColumn(int line, int column)
		{
			return _ns.FindColumn(line, column);
		}

		public int PositionFromPoint(int x, int y)
		{
			return _ns.PositionFromPoint(x, y);
		}

		public int PositionFromPointClose(int x, int y)
		{
			return _ns.PositionFromPointClose(x, y);
		}

		public int PointXFromPosition(int position)
		{
			return _ns.PointXFromPosition(position);
		}

		public int PointYFromPosition(int position)
		{
			return _ns.PointYFromPosition(position);
		}


		/// <summary>
		/// Increases the <see cref="ZoomFactor"/> property by 1 point to a maximum of 20;
		/// </summary>
		public void ZoomIn()
		{
			// No error checking necessary. Scintilla will clamp the value for us.
			DirectMessage(Constants.SCI_ZOOMIN, IntPtr.Zero, IntPtr.Zero);
		}



		/// <summary>
		/// Decreases the <see cref="ZoomFactor"/> property by 1 point to a minimum -10.
		/// </summary>
		public void ZoomOut()
		{
			// No error checking necessary. Scintilla will clamp the value for us.
			DirectMessage(Constants.SCI_ZOOMOUT, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Checks that if the specified position is on comment.
		/// </summary>
		public bool PositionIsOnComment(int position)
		{
			//this.Colorize(0, -1);
			return PositionIsOnComment(position, _lexing.Lexer);
		}

		/// <summary>
		/// Checks that if the specified position is on comment.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="lexer"></param>
		/// <returns></returns>
		public bool PositionIsOnComment(int position, LexerType lexer)
		{
			int style = _styles.GetStyleAt(position);
			if ((lexer == LexerType.Python || lexer == LexerType.Lisp)
				&& style == 1
				|| style == 12)
			{
				return true; // python or lisp
			}
			else if ((lexer == LexerType.Cpp || lexer == LexerType.Pascal || lexer == LexerType.Tcl || lexer == LexerType.Bullant)
				&& style == 1
				|| style == 2
				|| style == 3
				|| style == 15
				|| style == 17
				|| style == 18)
			{
				return true; // cpp, tcl, bullant or pascal
			}
			else if ((lexer == LexerType.Hypertext || lexer == LexerType.Xml)
				&& style == 9
				|| style == 20
				|| style == 29
				|| style == 30
				|| style == 42
				|| style == 43
				|| style == 44
				|| style == 57
				|| style == 58
				|| style == 59
				|| style == 72
				|| style == 82
				|| style == 92
				|| style == 107
				|| style == 124
				|| style == 125)
			{
				return true; // html or xml
			}
			else if ((lexer == LexerType.Perl || lexer == LexerType.Ruby || lexer == LexerType.Clw || lexer == LexerType.Bash)
				&& style == 2)
			{
				return true; // perl, bash, clarion/clw or ruby
			}
			else if ((lexer == LexerType.Sql)
				&& style == 1
				|| style == 2
				|| style == 3
				|| style == 13
				|| style == 15
				|| style == 17
				|| style == 18)
			{
				return true; // sql
			}
			else if ((lexer == LexerType.VB || lexer == LexerType.Properties || lexer == LexerType.MakeFile || lexer == LexerType.Batch || lexer == LexerType.Diff || lexer == LexerType.Conf || lexer == LexerType.Ave || lexer == LexerType.Eiffel || lexer == LexerType.EiffelKw || lexer == LexerType.Tcl || lexer == LexerType.VBScript || lexer == LexerType.MatLab || lexer == LexerType.Fortran || lexer == LexerType.F77 || lexer == LexerType.Lout || lexer == LexerType.Mmixal || lexer == LexerType.Yaml || lexer == LexerType.PowerBasic || lexer == LexerType.ErLang || lexer == LexerType.Octave || lexer == LexerType.Kix || lexer == LexerType.Asn1)
				&& style == 1)
			{
				return true; // asn1, vb, diff, batch, makefile, avenue, eiffel, eiffelkw, vbscript, matlab, crontab, fortran, f77, lout, mmixal, yaml, powerbasic, erlang, octave, kix or properties
			}
			else if ((lexer == LexerType.Latex)
				&& style == 4)
			{
				return true; // latex
			}
			else if ((lexer == LexerType.Lua || lexer == LexerType.EScript || lexer == LexerType.Verilog)
				&& style == 1
				|| style == 2
				|| style == 3)
			{
				return true; // lua, verilog or escript
			}
			else if ((lexer == LexerType.Ada)
				&& style == 10)
			{
				return true; // ada
			}
			else if ((lexer == LexerType.Baan || lexer == LexerType.Pov || lexer == LexerType.Ps || lexer == LexerType.Forth || lexer == LexerType.MsSql || lexer == LexerType.Gui4Cli || lexer == LexerType.Au3 || lexer == LexerType.Apdl || lexer == LexerType.Vhdl || lexer == LexerType.Rebol)
				&& style == 1
				|| style == 2)
			{
				return true; // au3, apdl, baan, ps, mssql, rebol, forth, gui4cli, vhdl or pov
			}
			else if ((lexer == LexerType.Asm)
				&& style == 1
				|| style == 11)
			{
				return true; // asm
			}
			else if ((lexer == LexerType.Nsis)
				&& style == 1
				|| style == 18)
			{
				return true; // nsis
			}
			else if ((lexer == LexerType.Specman)
				&& style == 2
				|| style == 3)
			{
				return true; // specman
			}
			else if ((lexer == LexerType.Tads3)
				&& style == 3
				|| style == 4)
			{
				return true; // tads3
			}
			else if ((lexer == LexerType.CSound)
				&& style == 1
				|| style == 9)
			{
				return true; // csound
			}
			else if ((lexer == LexerType.Caml)
				&& style == 12
				|| style == 13
				|| style == 14
				|| style == 15)
			{
				return true; // caml
			}
			else if ((lexer == LexerType.Haskell)
				&& style == 13
				|| style == 14
				|| style == 15
				|| style == 16)
			{
				return true; // haskell
			}
			else if ((lexer == LexerType.Flagship)
				&& style == 1
				|| style == 2
				|| style == 3
				|| style == 4
				|| style == 5
				|| style == 6)
			{
				return true; // flagship
			}
			else if ((lexer == LexerType.Smalltalk)
				&& style == 3)
			{
				return true; // smalltalk
			}
			else if ((lexer == LexerType.Css)
				&& style == 9)
			{
				return true; // css
			}
			return false;
		}


		/// <summary>
		/// Gets a word from the specified position
		/// </summary>
		public string GetWordFromPosition(int position)
		{
			//	Chris Rickard 2008-07-28
			//	Fixing implementation to actually return the word at the position...
			//	Credit goes to Stumpii for the code.
			//	As a side note: I think the previous code was implemented based off
			//	some funky code I made for the snippet keyword detection, but since
			//	it doesn't reference this method there's no reason to keep the buggy
			//	behavior. I also removed the try..catch because in theory this 
			//	shouldn't throw and we REALLY shouldn't be eating exceptions at the
			//	System.Exception level. If some start popping up I can add some
			//	conditionals or catch more specific Exceptions.
			int startPosition = NativeInterface.WordStartPosition(position, true);
			int endPosition = NativeInterface.WordEndPosition(position, true);
			return GetRange(startPosition, endPosition).Text;
		}


		/// <summary>
		/// Retrieves the zero-based byte index of the current caret location.
		/// </summary>
		/// <returns>The zero-based byte index of the caret.</returns>
		/// <dev>We need to consider how we're going to implement char (vs byte) positions.</dev>
		public int GetByteIndex()
		{
			return (int)DirectMessage(Constants.SCI_GETCURRENTPOS, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Retrieves the zero-based column (not character) index of the specified byte index.
		/// </summary>
		/// <param name="index">The byte index to search.</param>
		/// <returns>The zero-based column number of the byte index.</returns>
		/// <remarks>The tab width is included in the result.</remarks>
		/// <dev>We need to consider how we're going to implement char (vs byte) positions.</dev>
		public int GetColumnFromByteIndex(int index)
		{
			return (int)DirectMessage(Constants.SCI_GETCOLUMN, (IntPtr)index, IntPtr.Zero);
		}


		/// <summary>
		/// Retrieves the number of bytes in the document.
		/// </summary>
		/// <returns>The number of bytes contained in the text of the control.</returns>
		public int GetLength()
		{
			// TODO Implement TextLength property which truly represents the text (not byte) length
			// TODO Should this be a property?
			return (int)DirectMessage(Constants.SCI_GETLENGTH, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Retrieves the zero-based line number where the caret is currently located.
		/// </summary>
		/// <returns>The zero-based number of the current line.</returns>
		public int GetLine()
		{
			return GetLineFromByteIndex(GetByteIndex());
		}


		/// <summary>
		/// Retrieves the zero-based line number from the specified byte index within the document.
		/// </summary>
		/// <param name="index">The byte index to search.</param>
		/// <returns>The zero-based line number in which the byte index is located.</returns>
		/// <remarks>
		/// An <paramref name="index" /> value less than or equal to zero will always return zero.
		/// An <paramref name="index" /> value beyond the end of the document will always return the last line index.
		/// </remarks>
		/// <dev>We need to consider how we're going to implement char (vs byte) positions.</dev>
		public int GetLineFromByteIndex(int index)
		{
			return (int)DirectMessage(Constants.SCI_LINEFROMPOSITION, (IntPtr)index, IntPtr.Zero);
		}


		/// <summary>
		/// Exports a HTML representation of the current document.
		/// </summary>
		/// <returns>A <see cref="String"/> containing the contents of the document formatted as HTML.</returns>
		/// <remarks>Only ASCII documents are supported. Other encoding types have undefined behavior.</remarks>
		public string ExportHtml()
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
				ExportHtml(sw, "Untitled", false);

			return sb.ToString();
		}


		/// <summary>
		/// Exports a HTML representation of the current document.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/>with which to write. </param>
		/// <param name="title">The title of the HTML document.</param>
		/// <param name="allStyles"><c>true</c> to output all styles including those not
		/// used in the document; otherwise, <c>false</c>.</param>
		/// <remarks>Only ASCII documents are supported. Other encoding types have undefined behavior.</remarks>
		public void ExportHtml(TextWriter writer, string title, bool allStyles)
		{
			// Make sure the document is current
			// Lexing.Colorize();

			// Get the styles used
			int length = NativeInterface.GetLength();
			bool[] stylesUsed = new bool[(int)StylesCommon.Max + 1];
			if (allStyles)
			{
				for (int i = 0; i < stylesUsed.Length; i++)
					stylesUsed[i] = true;
			}
			else
			{
				// Record all the styles used
				for (int i = 0; i < length; i++)
					stylesUsed[Styles.GetStyleAt(i) & (int)StylesCommon.Max] = true;
			}

			// The tab width
			int tabWidth = Indentation.TabWidth;

			// Start writing
			writer.WriteLine(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">");
			writer.WriteLine("<html>");
			writer.WriteLine("<head>");
			writer.WriteLine("<title>{0}</title>", title);
			writer.WriteLine(@"<style type=""text/css"">");
			writer.WriteLine();

			// Write the body style
			writer.WriteLine("body {");
			writer.WriteLine("background-color: {0};", Util.ColorToHtml(Styles.DefaultStyle.BackColor));
			if (Wrapping.Mode == WrappingMode.None)
				writer.WriteLine("white-space: nowrap;");
			writer.WriteLine("}");
			writer.WriteLine();

			// Write the styles
			for (int i = 0; i < stylesUsed.Length; i++)
			{
				if (!stylesUsed[i])
					continue;

				Style s = Styles[i];
				writer.WriteLine("span.s{0} {{", i);
				writer.WriteLine("font-family: \"" + s.Font.Name + "\";");
				writer.WriteLine("font-size: {0}pt;", s.Font.SizeInPoints);
				if ((s.Font.Style & FontStyle.Italic) == FontStyle.Italic)
					writer.WriteLine("font-style: italic;");
				if ((s.Font.Style & FontStyle.Bold) == FontStyle.Bold)
					writer.WriteLine("font-weight: bold;");
				if (!s.ForeColor.IsEmpty && s.ForeColor != Color.Transparent)
					writer.WriteLine("color: {0};", Util.ColorToHtml(s.ForeColor));
				if (!s.BackColor.IsEmpty && s.BackColor != Color.Transparent)
					writer.WriteLine("background-color: {0};", Util.ColorToHtml(s.BackColor));

				writer.WriteLine("}");
				writer.WriteLine();
			}

			writer.WriteLine("</style>");
			writer.WriteLine("</head>");
			writer.WriteLine("<body>");

			// Write the document
			// TODO There's more to be done here to support codepages/UTF-8
			char lc;
			char c = '\0';
			int lastStyle = -1;
			for (int i = 0; i < length; i++)
			{
				lc = c;
				c = NativeInterface.GetCharAt(i);
				int style = Styles.GetStyleAt(i);
				if(style != lastStyle)
				{
					if(lastStyle != -1)
						writer.Write("</span>");

					writer.Write(@"<span class=""s{0}"">", style);
					lastStyle = style;
				}

				switch (c)
				{
					case '\0':
						continue;

					case ' ':
						if (lc == ' ')
							writer.Write("&nbsp;");
						else
							writer.Write(c);
						continue;

					case '\t':
						for (int t = 0; t < tabWidth; t++)
							writer.Write("&nbsp; ");
						continue;

					case '\r':
					case '\n':
						if (c == '\r' && i < length - 1 && NativeInterface.GetCharAt(i + 1) == '\n')
							i++;

						if (lastStyle != -1)
							writer.Write("</span>");

						writer.WriteLine("<br />");
						lastStyle = -1;
						continue;

					case '<':
						writer.Write("&lt;");
						continue;

					case '>':
						writer.Write("&gt;");
						continue;

					case '&':
						writer.Write("&amp;");
						continue;

					default:
						writer.Write(c);
						continue;
				}
			}

			if (lastStyle != -1)
				writer.Write("</span>");

			writer.WriteLine();
			writer.WriteLine("</body>");
			writer.WriteLine("</html>");
		}


		/// <summary>
		/// Sends the specified message directly to the native Scintilla control,
		/// bypassing any managed APIs.
		/// </summary>
		/// <param name="msg">The message ID.</param>
		/// <param name="wParam">The message <c>wparam</c> field.</param>
		/// <param name="lParam">The message <c>lparam</c> field.</param>
		/// <returns>An <see cref="IntPtr"/> representing the result of the message request.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IntPtr DirectMessage(uint msg, IntPtr wParam, IntPtr lParam)
		{
			// Enforce illegal cross-thread calls
			if (Form.CheckForIllegalCrossThreadCalls && InvokeRequired)
			{
				string message = Util.Format(Resources.InvalidOperation_IllegalCrossThreadCall, Name);
				throw new InvalidOperationException(message);
			}

			// NOTE: This will one day replace the current SendMessageDirect overloads
			// in the INativeScintilla interface. At that time, this will also be
			// changed to use the direct function pointer given by Scintilla instead
			// of the current implementation that calls the default proc.

			if (!IsDisposed)
			{
				Message m = Message.Create(Handle, (int)msg, wParam, lParam);
				DefWndProc(ref m);
				return m.Result;
			}

			return IntPtr.Zero;
		}


		/// <summary>
		/// Creates and returns a new <see cref="Scrolling"/> object.
		/// </summary>
		/// <returns>A new <see cref="Scrolling"/> object.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual Scrolling CreateScrollingInstance()
		{
			return new Scrolling(this);
		}


		/// <summary>
		/// Creates and returns a new <see cref="StyleCollection"/> object.
		/// </summary>
		/// <returns>A new <see cref="StyleCollection"/> object.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual StyleCollection CreateStylesInstance()
		{
			return new StyleCollection(this);
		}


		/// <summary>
		/// Creates and returns a new <see cref="Wrapping"/> object.
		/// </summary>
		/// <returns>A new <see cref="Wrapping"/> object.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual Wrapping CreateWrappingInstance()
		{
			return new Wrapping(this);
		}


		/// <summary>
		/// Joins the line range specified by removing any line breaks.
		/// </summary>
		/// <param name="startLine">The zero-based line index to start joining.</param>
		/// <param name="endLine">The zero-based line index to stop joining.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startLine"/> and <paramref name="endLine"/> do not specify a valid
		/// range of lines within the document.
		/// </exception>
		/// <remarks>Where joining would lead to no space between words, an extra space will be inserted.</remarks>
		public virtual void JoinLines(int startLine, int endLine)
		{
			if (startLine < 0)
				throw new ArgumentOutOfRangeException("startLine", "The start line must be greater than or equal to zero.");
			if (endLine < startLine)
				throw new ArgumentOutOfRangeException("endLine", "The start line and end line must specify a valid range.");

			// Convert line indexes to positions within the line
			int startPos = (int)DirectMessage(Constants.SCI_POSITIONFROMLINE, (IntPtr)startLine, IntPtr.Zero);
			int endPos = (int)DirectMessage(Constants.SCI_POSITIONFROMLINE, (IntPtr)endLine, IntPtr.Zero);

			if (startPos == -1)
				throw new ArgumentOutOfRangeException("startLine", "The start line specify a valid line within the document.");
			if (endPos == -1)
				throw new ArgumentOutOfRangeException("endLine", "The end line must specify a valid line within the document.");

			// Set the target positions (which Scintilla will convert back to line indexes)
			DirectMessage(Constants.SCI_SETTARGETSTART, (IntPtr)startPos, IntPtr.Zero);
			DirectMessage(Constants.SCI_SETTARGETEND, (IntPtr)endPos, IntPtr.Zero);

			DirectMessage(Constants.SCI_LINESJOIN, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Scrolls the contents of the control to the current caret position.
		/// </summary>
		public void ScrollToCaret()
		{
			Scrolling.ScrollToCaret();
		}


		/// <summary>
		/// Begins grouping changes until the <see cref="EndUndo"/> method is called.
		/// These changes can be undone as a single unit 
		/// </summary>
		/// <remarks>These transactions can be nested and only the top-level sequences are undone as units.</remarks>
		/// <dev>What does the above message mean?</dev>
		public void BeginUndo()
		{
			DirectMessage(Constants.SCI_BEGINUNDOACTION, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Clears all text from the control. Calling this method has no affect
		/// when the <see cref="ReadOnly"/> property is <c>true</c> 
		/// </summary>
		public void Clear()
		{
			// TODO I think clear should always clear the document regardless of the read only flag. Don't you?
			DirectMessage(Constants.SCI_CLEARALL, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Clears all information from the undo buffer of the control.
		/// </summary>
		public void ClearUndo()
		{
			DirectMessage(Constants.SCI_EMPTYUNDOBUFFER, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Copies the current selection in the document to the <c>Clipboard</c>.
		/// </summary>
		[UIPermissionAttribute(SecurityAction.Demand, Clipboard = UIPermissionClipboard.OwnClipboard)]
		public void Copy()
		{
			Copy(false);
		}


		/// <summary>
		/// Copies the current selection, or the current line if there is no selection, to the <c>Clipboard</c>.
		/// </summary>
		/// <param name="lineSelect">
		/// Indicates whether to copy the current line if there is no selection. A line copied in this mode will
		/// always be pasted to the document at the line index previous to the insertion line.
		/// </param>
		public void Copy(bool lineSelect)
		{
			if (lineSelect)
				DirectMessage(Constants.SCI_COPYALLOWLINE, IntPtr.Zero, IntPtr.Zero);
			else
				DirectMessage(Constants.SCI_COPY, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Copies the specified range of bytes in the document to the <c>Clipboard</c>.
		/// </summary>
		/// <param name="startIndex">The zero-based byte index to start copying.</param>
		/// <param name="endIndex">The zero-based byte index to stop copying.</param>
		[UIPermissionAttribute(SecurityAction.Demand, Clipboard = UIPermissionClipboard.OwnClipboard)]
		public void Copy(int startIndex, int endIndex)
		{
			DirectMessage(Constants.SCI_COPYRANGE, (IntPtr)startIndex, (IntPtr)endIndex);
		}


		/// <summary>
		/// Moves the current selection in the document to the <c>Clipboard</c>.
		/// </summary>
		[UIPermissionAttribute(SecurityAction.Demand, Clipboard = UIPermissionClipboard.OwnClipboard)]
		public void Cut()
		{
			DirectMessage(Constants.SCI_CUT, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Removes the current selection or the character following the caret from the document.
		/// </summary>
		public void Delete()
		{
			DirectMessage(Constants.SCI_CLEAR, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Groups all changes made after a call to <see cref="BeginUndo"/> as a single unit.
		/// Normal undo behavior resumes.
		/// </summary>
		public void EndUndo()
		{
			DirectMessage(Constants.SCI_ENDUNDOACTION, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Searches the text in the control for a matching string.
		/// </summary>
		/// <param name="pattern">The text to locate in the control.</param>
		/// <param name="flags">A bitwise combination of the <see cref="FindFlags"/> values.</param>
		/// <returns>
		/// A <see cref="Range"/> representing the bounds within the control where the search text was found
		/// or an empty <see cref="Range"/> if the search string is not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <c>null</c>.</exception>
		/// <remarks>A successful match selects the text but does not scroll it into view.</remarks>
		public Range Find(string pattern, FindFlags flags)
		{
			return Find(pattern, flags, 0, TextLength, true);
		}


		/// <summary>
		/// Searches the text in the control for a matching string.
		/// </summary>
		/// <param name="pattern">The text to locate in the control.</param>
		/// <param name="flags">A bitwise combination of the <see cref="FindFlags"/> values.</param>
		/// <param name="selectResult">
		/// <c>true</c> to select the result when a match is successful; otherwise, <c>false</c>.
		/// </param>
		/// <returns>
		/// A <see cref="Range"/> representing the bounds within the control where the search text was found
		/// or an empty <see cref="Range"/> if the search string is not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <c>null</c>.</exception>
		/// <remarks>A successful match does not scroll the text into view.</remarks>
		public Range Find(string pattern, FindFlags flags, bool selectResult)
		{
			return Find(pattern, flags, 0, TextLength, selectResult);
		}


		/// <summary>
		/// Searches the text in the control for a matching string.
		/// </summary>
		/// <param name="pattern">The text to locate in the control.</param>
		/// <param name="flags">A bitwise combination of the <see cref="FindFlags"/> values.</param>
		/// <param name="startIndex">The char index within the control's text at which to begin searching.</param>
		/// <param name="endIndex">The char index within the control's text at which to end searching.</param>
		/// <returns>
		/// A <see cref="Range"/> representing the bounds within the control where the search text was found
		/// or an empty <see cref="Range"/> if the search string is not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The <paramref name="startIndex"/> or <paramref name="endIndex"/> is less than zero.
		/// -or-
		/// The <paramref name="startIndex"/> or <paramref name="endIndex"/> is greater than <see cref="TextLength"/>.
		/// </exception>
		/// <remarks>
		/// It is possible to search backwards by specifiying a <paramref name="startIndex"/> greater
		/// than the <paramref name="endIndex"/>. The result, however, always specifies a forward range with the
		/// <see cref="Range.StartIndex"/> property less than the <see cref="Range.EndIndex"/> property.
		/// A successful match selects the text but does not scroll it into view.
		/// </remarks>
		public Range Find(string pattern, FindFlags flags, int startIndex, int endIndex)
		{
			return Find(pattern, flags, startIndex, endIndex, true);
		}


		/// <summary>
		/// Searches the text in the control for a matching string.
		/// </summary>
		/// <param name="pattern">The text to locate in the control.</param>
		/// <param name="flags">A bitwise combination of the <see cref="FindFlags"/> values.</param>
		/// <param name="startIndex">The char index within the control's text at which to begin searching.</param>
		/// <param name="endIndex">The char index within the control's text at which to end searching.</param>
		/// <param name="selectResult">
		/// <c>true</c> to select the result when a match is successful; otherwise, <c>false</c>.
		/// </param>
		/// <returns>
		/// A <see cref="Range"/> representing the bounds within the control where the search text was found
		/// or an empty <see cref="Range"/> if the search string is not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The <paramref name="startIndex"/> or <paramref name="endIndex"/> is less than zero.
		/// -or-
		/// The <paramref name="startIndex"/> or <paramref name="endIndex"/> is greater than <see cref="TextLength"/>.
		/// </exception>
		/// <remarks>
		/// It is possible to search backwards by specifiying a <paramref name="startIndex"/> greater
		/// than the <paramref name="endIndex"/>. The result, however, always specifies a forward range with the
		/// <see cref="Range.StartIndex"/> property less than the <see cref="Range.EndIndex"/> property.
		/// A successful match does not scroll the text into view.
		/// </remarks>
		public Range Find(string pattern, FindFlags flags, int startIndex, int endIndex, bool selectResult)
		{
			return FindReplaceChars(pattern, (int)flags, null, startIndex, endIndex, selectResult);
		}


		public Range[] FindAll(string pattern, FindFlags flags)
		{
			return FindAll(pattern, flags, 0, TextLength);
		}


		public Range[] FindAll(string pattern, FindFlags flags, int startIndex, int endIndex)
		{
			return FindReplaceAllChars(pattern, (int)flags, null, startIndex, endIndex);
		}


		public Range[] FindAllRegex(string pattern, bool caseSensitive)
		{
			return FindAllRegex(pattern, caseSensitive, 0, TextLength);
		}


		public Range[] FindAllRegex(string pattern, bool caseSensitive, int startIndex, int endIndex)
		{
			FindFlags flags = (FindFlags)(Constants.SCFIND_REGEXP | Constants.SCFIND_POSIX);
			if (caseSensitive)
				flags |= FindFlags.MatchCase;

			return FindReplaceAllChars(pattern, (int)flags, null, startIndex, endIndex);
		}


		public Range FindNext(string pattern, FindFlags flags)
		{
			return FindReplaceIncremental(pattern, flags, null, true);
		}


		public Range FindNextRegex(string pattern, bool caseSensitive)
		{
			FindFlags flags = (FindFlags)(Constants.SCFIND_REGEXP | Constants.SCFIND_POSIX);
			if (caseSensitive)
				flags |= FindFlags.MatchCase;

			return FindReplaceIncremental(pattern, flags, null, true);
		}


		public Range FindPrevious(string pattern, FindFlags flags)
		{
			return FindReplaceIncremental(pattern, flags, null, false);
		}


		/// <summary>
		/// Searches the text in the control for a string matching the specified regular expression pattern.
		/// </summary>
		/// <param name="pattern">The regular expression pattern to locate in the control.</param>
		/// <param name="caseSensitive"><c>true</c> to perform case-sensitive matching; otherwise, <c>false</c>.</param>
		/// <returns>
		/// A <see cref="Range"/> representing the bounds within the control where the search text was found
		/// or an empty <see cref="Range"/> if the search string is not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <c>null</c>.</exception>
		/// <remarks>A successful match selects the text but does not scroll it into view.</remarks>
		public Range FindRegex(string pattern, bool caseSensitive)
		{
			return FindRegex(pattern, caseSensitive, 0, TextLength, true);
		}


		/// <summary>
		/// Searches the text in the control for a string matching the specified regular expression pattern.
		/// </summary>
		/// <param name="pattern">The regular expression pattern to locate in the control.</param>
		/// <param name="caseSensitive"><c>true</c> to perform case-sensitive matching; otherwise, <c>false</c>.</param>
		/// <param name="selectResult">
		/// <c>true</c> to select the result when a match is successful; otherwise, <c>false</c>.
		/// </param>
		/// <returns>
		/// A <see cref="Range"/> representing the bounds within the control where the search text was found
		/// or an empty <see cref="Range"/> if the search string is not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <c>null</c>.</exception>
		/// <remarks>A successful match does not scroll the text into view.</remarks>
		public Range FindRegex(string pattern, bool caseSensitive, bool selectResult)
		{
			return FindRegex(pattern, caseSensitive, 0, TextLength, selectResult);
		}


		/// <summary>
		/// Searches the text in the control for a string matching the specified regular expression pattern.
		/// </summary>
		/// <param name="pattern">The regular expression pattern to locate in the control.</param>
		/// <param name="caseSensitive"><c>true</c> to perform case-sensitive matching; otherwise, <c>false</c>.</param>
		/// <param name="startIndex">The char index within the control's text at which to begin searching.</param>
		/// <param name="endIndex">The char index within the control's text at which to end searching.</param>
		/// <returns>
		/// A <see cref="Range"/> representing the bounds within the control where the search text was found
		/// or an empty <see cref="Range"/> if the search string is not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The <paramref name="startIndex"/> or <paramref name="endIndex"/> is less than zero.
		/// -or-
		/// The <paramref name="startIndex"/> or <paramref name="endIndex"/> is greater than <see cref="TextLength"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The <paramref name="endIndex"/> is less than <paramref name="startIndex"/>.
		/// </exception>
		/// <remarks>A successful match selects the text but does not scroll it into view.</remarks>
		public Range FindRegex(string pattern, bool caseSensitive, int startIndex, int endIndex)
		{
			return FindRegex(pattern, caseSensitive, startIndex, endIndex, true);
		}


		/// <summary>
		/// Searches the text in the control for a string matching the specified regular expression pattern.
		/// </summary>
		/// <param name="pattern">The regular expression pattern to locate in the control.</param>
		/// <param name="caseSensitive"><c>true</c> to perform case-sensitive matching; otherwise, <c>false</c>.</param>
		/// <param name="startIndex">The char index within the control's text at which to begin searching.</param>
		/// <param name="endIndex">The char index within the control's text at which to end searching.</param>
		/// <param name="selectResult">
		/// <c>true</c> to select the result when a match is successful; otherwise, <c>false</c>.
		/// </param>
		/// <returns>
		/// A <see cref="Range"/> representing the bounds within the control where the search text was found
		/// or an empty <see cref="Range"/> if the search string is not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The <paramref name="startIndex"/> or <paramref name="endIndex"/> is less than zero.
		/// -or-
		/// The <paramref name="startIndex"/> or <paramref name="endIndex"/> is greater than <see cref="TextLength"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The <paramref name="endIndex"/> is less than <paramref name="startIndex"/>.
		/// </exception>
		/// <remarks>A successful match does not scroll the text into view.</remarks>
		public Range FindRegex(string pattern, bool caseSensitive, int startIndex, int endIndex, bool selectResult)
		{
			if (endIndex < startIndex)
				throw new ArgumentException(Resources.Message_RegexFindReplaceDirection, "endIndex");

			FindFlags flags = (FindFlags)(Constants.SCFIND_REGEXP | Constants.SCFIND_POSIX);
			if (caseSensitive)
				flags |= FindFlags.MatchCase;

			return FindReplaceChars(pattern, (int)flags, null, startIndex, endIndex, selectResult);
		}


		private Range[] FindReplaceAllChars(string pattern, int flags, string newText, int startIndex, int endIndex)
		{
			List<Range> list = new List<Range>();

			// Convert to byte index
			int originalStartByteIndex = _textAdapter.CharToByteIndex(startIndex);
			int originalEndByteIndex = _textAdapter.CharToByteIndex(endIndex);

			int startByteIndex = originalStartByteIndex;
			int endByteIndex = originalEndByteIndex;

			while (FindReplaceBytes(pattern, flags, newText, ref startByteIndex, ref endByteIndex, false))
			{
				Range ch = new Range(_textAdapter.ByteToCharIndex(startByteIndex), _textAdapter.ByteToCharIndex(endByteIndex));
				list.Add(ch);

				// Forward searches start at the end of the last match
				if (originalStartByteIndex < originalEndByteIndex)
					startByteIndex = endByteIndex;

				endByteIndex = originalEndByteIndex; // End is always the original end
			}

			return list.ToArray();
		}


		private unsafe Range FindReplaceChars(string pattern, int flags, string text, int startIndex, int endIndex, bool selectResult)
		{
			int textLength = TextLength;
			if (startIndex < 0 || startIndex > textLength)
			{
				string message = Util.Format(Resources.Message_InvalidBoundArgument, startIndex, "startIndex", "0", "TextLength");
				throw new ArgumentOutOfRangeException("startIndex", message);
			}

			if (endIndex < 0 || endIndex > textLength)
			{
				string message = Util.Format(Resources.Message_InvalidBoundArgument, endIndex, "endIndex", "0", "TextLength");
				throw new ArgumentOutOfRangeException("endIndex", message);
			}

			int startByteIndex = _textAdapter.CharToByteIndex(startIndex);
			int endByteIndex = _textAdapter.CharToByteIndex(endIndex);

			if (!FindReplaceBytes(pattern, flags, text, ref startByteIndex, ref endByteIndex, selectResult))
				return Range.Empty;

			startIndex = _textAdapter.ByteToCharIndex(startByteIndex);
			endIndex = _textAdapter.ByteToCharIndex(endByteIndex);

			return new Range(startIndex, endIndex);
		}


		private unsafe bool FindReplaceBytes(string pattern, int flags, string text, ref int startByteIndex, ref int endByteIndex, bool selectResult)
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");

			Debug.Assert(startByteIndex >= 0 && startByteIndex <= (int)DirectMessage(Constants.SCI_GETLENGTH, IntPtr.Zero, IntPtr.Zero));
			Debug.Assert(endByteIndex >= 0 && endByteIndex <= (int)DirectMessage(Constants.SCI_GETLENGTH, IntPtr.Zero, IntPtr.Zero));

			// Set the target and search flags
			DirectMessage(Constants.SCI_SETTARGETSTART, (IntPtr)startByteIndex, IntPtr.Zero);
			DirectMessage(Constants.SCI_SETTARGETEND, (IntPtr)endByteIndex, IntPtr.Zero);
			DirectMessage(Constants.SCI_SETSEARCHFLAGS, (IntPtr)flags, IntPtr.Zero);

			byte[] buffer;

			// Search
			buffer = Encoding.GetBytes(pattern);
			int resultByteIndex;
			fixed (byte* bp = buffer)
			{
				resultByteIndex = (int)DirectMessage(Constants.SCI_SEARCHINTARGET, (IntPtr)buffer.Length, (IntPtr)bp);
			}

			if (resultByteIndex == -1)
				return false;

			// Replace
			if (text != null)
			{
				buffer = Encoding.GetBytes(text);
				fixed (byte* bp = buffer)
					DirectMessage(Constants.SCI_REPLACETARGET, (IntPtr)buffer.Length, (IntPtr)bp);
			}

			// Result (always with start index before end index)
			startByteIndex = (int)DirectMessage(Constants.SCI_GETTARGETSTART, IntPtr.Zero, IntPtr.Zero);
			endByteIndex = (int)DirectMessage(Constants.SCI_GETTARGETEND, IntPtr.Zero, IntPtr.Zero);

			// Select
			if (selectResult)
				DirectMessage(Constants.SCI_SETSEL, (IntPtr)startByteIndex, (IntPtr)endByteIndex);

			return true;
		}


		private Range FindReplaceIncremental(string pattern, FindFlags flags, string newText, bool forward)
		{
			int startByteIndex;
			int endByteIndex;

			if (forward)
			{
				startByteIndex = Math.Max(_textAdapter.CaretByteIndex(), _textAdapter.AnchorByteIndex());
				endByteIndex = _textAdapter.TotalByteLength();
			}
			else
			{
				startByteIndex = Math.Min(_textAdapter.CaretByteIndex(), _textAdapter.AnchorByteIndex());
				endByteIndex = 0;
			}

			if (FindReplaceBytes(pattern, (int)flags, newText, ref startByteIndex, ref endByteIndex, true))
				return new Range(_textAdapter.ByteToCharIndex(startByteIndex), _textAdapter.ByteToCharIndex(endByteIndex));

			return Range.Empty;
		}


		/// <summary>
		/// Moves the caret and scrolls into view the line specified.
		/// </summary>
		/// <remarks>A <see cref="lineIndex"/> value less than zero or greater than the last
		/// document line index will go to the first and last lines of the document respectively.</remarks>
		public void GoTo(int lineIndex)
		{
			DirectMessage(Constants.SCI_GOTOLINE, (IntPtr)lineIndex, IntPtr.Zero);
		}


		/// <summary>
		/// Replaces the current selection in the document with the contents of the <c>Clipboard</c>.
		/// </summary>
		[UIPermissionAttribute(SecurityAction.Demand, Clipboard = UIPermissionClipboard.OwnClipboard)]
		public void Paste()
		{
			DirectMessage(Constants.SCI_PASTE, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Redoes the last undone operation in the the control.
		/// </summary>
		public void Redo()
		{
			DirectMessage(Constants.SCI_REDO, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Selects a range of text in the control.
		/// </summary>
		/// <param name="anchorIndex">The index within the control text of the selection anchor.</param>
		/// <param name="caretIndex">The index within the control text of the selection caret.</param>
		/// <exception cref="ArgumentOutOfRange">
		/// The <paramref name="anchorIndex"/> and/or <paramref name="caretIndex"/> do not point to a valid index within the document.
		/// </exception>
		public void Select(int anchorIndex, int caretIndex)
		{
			int textLength = _textAdapter.TotalCharLength();
			if (anchorIndex < 0 || anchorIndex > textLength || caretIndex < 0 || caretIndex > textLength)
			{
				string paramName = ((anchorIndex < 0 || anchorIndex > textLength) ? "anchorIndex" : "caretIndex");
				throw new ArgumentOutOfRangeException(paramName, "The value must be an char index within the document.");
			}

			// Translate the char offsets to byte offsets
			anchorIndex = _textAdapter.CharToByteIndex(anchorIndex);
			caretIndex = _textAdapter.CharToByteIndex(caretIndex);

			DirectMessage(Constants.SCI_SETSEL, (IntPtr)anchorIndex, (IntPtr)caretIndex);
		}


		/// <summary>
		/// Selects all text in the control.
		/// </summary>
		public void SelectAll()
		{
			DirectMessage(Constants.SCI_SELECTALL, IntPtr.Zero, IntPtr.Zero);
		}


		/// <summary>
		/// Undoes the last edit operation in the control.
		/// </summary>
		public void Undo()
		{
			DirectMessage(Constants.SCI_UNDO, IntPtr.Zero, IntPtr.Zero);
		}

		#endregion

		#region other stuff
		internal bool IsDesignMode
		{
			get
			{
				return DesignMode;
			}
		}

		private List<TopLevelHelper> _helpers = new List<TopLevelHelper>();
		protected internal List<TopLevelHelper> Helpers
		{
			get
			{
				return _helpers;
			}
			set
			{
				_helpers = value;
			}
		}

		#endregion

		#region ISupportInitialize Members
		private bool _isInitializing = false;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsInitializing
		{
			get
			{
				return _isInitializing;
			}
			set
			{
				_isInitializing = value;
			}
		}

		public void BeginInit()
		{
			_isInitializing = true;
		}

		public void EndInit()
		{
			_isInitializing = false;
			foreach (ScintillaHelperBase helper in _helpers)
			{
				helper.Initialize();
			}
		}

		#endregion
	}
}
