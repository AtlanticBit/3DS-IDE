using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using ScintillaNET.WPF.Configuration;

using Forms = System.Windows.Forms;
using SN = ScintillaNET;

namespace ScintillaNET.WPF
{
	[DefaultProperty("Text")]
	[DefaultEvent("DocumentChanged")]
	[ContentProperty("WPFConfig")]
	public partial class ScintillaWPF : UserControl, SN.INativeScintilla
	{
		private readonly SN.Scintilla mInnerScintilla;
		public SN.Scintilla Scintilla { get { return mInnerScintilla; } }

		public ScintillaWPF()
		{
			InitializeComponent();
			this.mInnerScintilla = new SN.Scintilla();
			this.winFormsHost.Child = this.mInnerScintilla;
			this.mWPFConfig = new ScintillaWPFConfigItemCollection(this);
		}

		private readonly ScintillaWPFConfigItemCollection mWPFConfig;
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ScintillaWPFConfigItemCollection WPFConfig
		{
			get { return mWPFConfig; }
		}

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether pressing ENTER creates a new line of text in the
		/// control or activates the default button for the form.
		/// </summary>
		[Category("Behavior")]
		[Description("Indicates if return characters are accepted as text input.")]
		[DefaultValue(true)]
		public bool AcceptsReturn
		{
			get { return mInnerScintilla.AcceptsReturn; }
			set { mInnerScintilla.AcceptsReturn = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether pressing the TAB key types a TAB character in the control
		/// instead of moving the focus to the next control in the tab order.
		/// </summary>
		[Category("Behavior")]
		[Description("Indicates if tab characters are accepted as text input.")]
		[DefaultValue(true)]
		public bool AcceptsTab
		{
			get { return mInnerScintilla.AcceptsTab; }
			set { mInnerScintilla.AcceptsTab = value; }
		}
		
		/// <summary>
		/// Gets a collection containing all annotations in the control.
		/// </summary>
		[Category("Appearance")]
		[Description("The annotations and options.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.AnnotationCollection Annotations
		{
			get { return mInnerScintilla.Annotations; }
		}

		

		/// <summary>
		/// Controls autocompletion behavior.
		/// </summary>
		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.AutoComplete AutoComplete
		{
			get { return mInnerScintilla.AutoComplete; }
		}


		// NO SUPPORT: BackColor
		// NO SUPPORT: BackgroundImage
		// NO SUPPORT: BackgroundImageLayout


		/// <summary>
		/// Gets or sets the border style of the control.
		/// </summary>
		/// <exception cref="InvalidEnumArgumentException">The value assigned is not one of the <see cref="BorderStyle" /> values.</exception>
		[Category("Appearance")]
		[Description("Indicates whether the control should have a border.")]
		[DefaultValue(Forms.BorderStyle.Fixed3D)]
		public Forms.BorderStyle BorderStyle
		{
			get { return mInnerScintilla.BorderStyle; }
			set { mInnerScintilla.BorderStyle = value; }
		}

		/// <summary>
		/// Manages CallTip (Visual Studio-like code Tooltip) behaviors
		/// </summary>
		public SN.CallTip CallTip
		{
			get { return mInnerScintilla.CallTip; }
			set { mInnerScintilla.CallTip = value; }
		}

		
		// NO SUPPORT: Caption
		/*
		/// <summary>
		/// Gets/Sets the Win32 Window Caption. Defaults to Type's FullName
		/// </summary>
		[Category("Behavior")]
		[Description("Win32 Window Caption")]
		public string Caption
		{
			get { return mInnerScintilla.Caption; }
			set { mInnerScintilla.Caption =  value; }
		}
		*/


		/// <summary>
		/// Controls Caret Behavior
		/// </summary>
		[Category("Appearance")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.CaretInfo Caret
		{
			get { return mInnerScintilla.Caret; }
		}

		/// <summary>
		/// Gets Clipboard access for the control.
		/// </summary>
		[Category("Behavior")]
		[Description("Clipboard (cut, copy, paste) options.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Clipboard Clipboard
		{
			get { return mInnerScintilla.Clipboard; }
		}

		/// <summary>
		/// Controls behavior of keyboard bound commands.
		/// </summary>
		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Commands Commands
		{
			get { return mInnerScintilla.Commands; }
		}

		/// <summary>
		/// Controls behavior of loading/managing ScintillaNET configurations.
		/// </summary>
		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Configuration.ConfigurationManager ConfigurationManager
		{
			get { return mInnerScintilla.ConfigurationManager; }
		}


		// NO SUPPORT: CreateParams


		/// <summary>
		/// Gets or sets the character index of the current caret position.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CurrentPos
		{
			get { return mInnerScintilla.CurrentPos; }
			set { mInnerScintilla.CurrentPos = value; }
		}


		// NO SUPPORT: DefaultCursor
		// NO SUPPORT: DefaultSize


		/// <summary>
		/// Controls behavior of Documents
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SN.DocumentHandler DocumentHandler
		{
			get { return mInnerScintilla.DocumentHandler; }
		}

		/// <summary>
		/// Controls behavior of automatic document navigation
		/// </summary>
		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.DocumentNavigation DocumentNavigation
		{
			get { return mInnerScintilla.DocumentNavigation; }
		}

		/// <summary>
		/// Controls behavior of Drop Markers
		/// </summary>
		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.DropMarkers DropMarkers
		{
			get { return mInnerScintilla.DropMarkers; }
		}

		/// <summary>
		/// Gets or sets the type of text encoding the <see cref="Scintilla" /> control uses internally.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="value" /> is null.</exception>
		/// <exception cref="ArgumentException">The specified encoding is not supported.</exception>
		/// <remarks>
		/// The default encoding is suitable for most users.
		/// Changing it can have unintended consequences.
		/// </remarks>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		// NEED CAT
		// NEED DESC
		// NEED DEFAULT
		public Encoding Encoding
		{
			get { return mInnerScintilla.Encoding; }
			set { mInnerScintilla.Encoding = value; }
		}

		/// <summary>
		/// Controls End Of Line Behavior
		/// </summary>
		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.EndOfLine EndOfLine
		{
			get { return mInnerScintilla.EndOfLine; }
		}

		// NEED DESC
		[Category("Behavior")]
		// TODO: Should this be marked as content serialized?
		public SN.FindReplace FindReplace
		{
			get { return mInnerScintilla.FindReplace; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Folding Folding
		{
			get { return mInnerScintilla.Folding; }
		}


		// NO SUPPORT: Font
		// NO SUPPORT: ForeColor


		// NEED DESC
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SN.GoTo GoTo
		{
			get { return mInnerScintilla.GoTo; }
		}

		[Category("Appearance")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.HotspotStyle HotspotStyle
		{
			get { return mInnerScintilla.HotspotStyle; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Indentation Indentation
		{
			get { return mInnerScintilla.Indentation; }
		}

		/// <summary>
		/// Gets an object used by the <see cref="Scintilla" /> control to manage indicators.
		/// </summary>
		/// <remarks>
		/// Indicators are used to display additional information over the top of styling.
		/// They can be used to show, for example, syntax errors, deprecated names, and bad indentation by drawing underlines under text or boxes around text.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SN.Indicators Indicators
		{
			get { return mInnerScintilla.Indicators; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DefaultValue(false)]
		public bool IsBraceMatching
		{
			get { return mInnerScintilla.IsBraceMatching; }
			set { mInnerScintilla.IsBraceMatching = value; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DefaultValue(true)]
		public bool IsCustomPaintingEnabled
		{
			get { return mInnerScintilla.IsCustomPaintingEnabled; }
			set { mInnerScintilla.IsCustomPaintingEnabled = value; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DefaultValue(false)]
		public bool IsReadOnly
		{
			get { return mInnerScintilla.IsReadOnly; }
			set { mInnerScintilla.IsReadOnly = value; }
		}

		/// <summary>
		/// Gets or sets the line layout caching strategy in a <see cref="Scintilla" /> control.
		/// </summary>
		/// <returns>
		/// One of the <see cref="LayoutCacheMode"/> enumeration values.
		/// The default is <see cref="LayoutCacheMode.Caret" />.
		/// </returns>
		/// <exception cref="InvalidEnumArgumentException">
		/// The value assigned is not one of the <see cref="LayoutCacheMode" /> values.
		/// </exception>
		/// <remarks>Larger cache sizes increase performance at the expense of memory.</remarks>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		// TODO: Allow modification of this via the designer.
		// NEED CAT
		// NEED DESC
		[DefaultValue(typeof(LayoutCacheMode), "Caret")]
		public SN.LayoutCacheMode LayoutCacheMode
		{
			get { return mInnerScintilla.LayoutCacheMode; }
			set { mInnerScintilla.LayoutCacheMode = value; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Lexing Lexing
		{
			get { return mInnerScintilla.Lexing; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SN.LineCollection Lines
		{
			get { return mInnerScintilla.Lines; }
		}

		/// <summary>
		/// Gets an object that controls line wrapping options in the <see cref="Scintilla"/> control.
		/// </summary>
		/// <returns>A <see cref="LineWrapping"/> object that manages line wrapping options in a <see cref="Scintilla"/> control.</returns>
		[Category("Behavior")]
		[Description("The control's line wrapping options.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.LineWrapping LineWrapping
		{
			get { return mInnerScintilla.LineWrapping; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.LongLines LongLines
		{
			get { return mInnerScintilla.LongLines; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<SN.ManagedRange> ManagedRanges
		{
			get { return mInnerScintilla.ManagedRanges; }
		}

		[Browsable(true)]
		[Category("Appearance")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.MarginCollection Margins
		{
			get { return mInnerScintilla.Margins; }
		}

		/// <summary>
		/// Gets a collection representing the marker objects and options within the control.
		/// </summary>
		/// <returns>A <see cref="MarkerCollection" /> representing the marker objects and options within the control.</returns>
		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.MarkerCollection Markers
		{
			get { return mInnerScintilla.Markers; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DefaultValue(true)]
		public bool MatchBraces
		{
			get { return mInnerScintilla.MatchBraces; }
			set { mInnerScintilla.MatchBraces = value; }
		}

		/// <summary>
		/// Gets or sets a value that indicates that the control has been modified by the user since
		/// the control was created or its contents were last set.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the control's contents have been modified; otherwise, <c>false</c>.
		/// The default is <c>false</c>.
		/// </returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Modified
		{
			get { return mInnerScintilla.Modified; }
			set { mInnerScintilla.Modified = value; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DefaultValue(true)]
		public bool MouseDownCaptures
		{
			get { return mInnerScintilla.MouseDownCaptures; }
			set { mInnerScintilla.MouseDownCaptures = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SN.INativeScintilla NativeInterface
		{
			get { return mInnerScintilla.NativeInterface; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DefaultValue(false)]
		public bool OverType
		{
			get { return mInnerScintilla.OverType; }
			set { mInnerScintilla.OverType = value; }
		}

		/// <summary>
		/// Gets the Scintilla direct object pointer that this control represents.
		/// </summary>
		/// <returns>An <see cref="IntPtr" /> that represents the Scintilla direct object pointer of the control.</returns>
		/// <remarks>
		/// Warning: The Surgeon General Has Determined that Calling the Underlying Scintilla
		/// Window Directly May Result in Unexpected Behavior!
		/// </remarks>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr Pointer
		{
			get { return mInnerScintilla.Pointer; }
		}

		/// <summary>
		/// Gets or sets the position cache size used to layout short runs of text in a <see cref="Scintilla" /> control.
		/// </summary>
		/// <returns>The size of the position cache in bytes. The default is 1024.</returns>
		/// <remarks>Larger cache sizes increase performance at the expense of memory.</remarks>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(1024)]
		public int PositionCacheSize
		{
			get { return mInnerScintilla.PositionCacheSize; }
			set { mInnerScintilla.PositionCacheSize = value; }
		}

		[Category("Layout")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		// TODO: Support design-time definition of this with xaml.
		public SN.Printing Printing
		{
			get { return mInnerScintilla.Printing; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public byte[] RawText
		{
			get { return mInnerScintilla.RawText; }
			set { mInnerScintilla.RawText = value; }
		}

		/// <summary>
		/// Gets an object that manages scrolling options in a <see cref="Scintilla" /> control.
		/// </summary>
		/// <returns>A <see cref="Scrolling"/> object that manages scrolling options in a <see cref="Scintilla" /> control.</returns>
		[CategoryAttribute("Layout")]
		[Description("The control's scrolling options.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Scrolling Scrolling
		{
			get { return mInnerScintilla.Scrolling; }
		}

		[Category("Behavior")]
		[DefaultValue(SearchFlags.Empty)]
		[Editor(typeof(Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public SN.SearchFlags SearchFlags
		{
			get { return mInnerScintilla.SearchFlags; }
			set { mInnerScintilla.SearchFlags = value; }
		}

		[Category("Appearance")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Selection Selection
		{
			get { return mInnerScintilla.Selection; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.SnippetManager Snippets
		{
			get { return mInnerScintilla.Snippets; }
		}

		[Category("Appearance")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.StyleCollection Styles
		{
			get { return mInnerScintilla.Styles; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether characters not considered alphanumeric (ASCII values 0 through 31)
		/// are prevented as text input.
		/// </summary>
		/// <returns>
		/// <c>true</c> to prevent control characters as input; otherwise, <c>false</c>.
		/// The default is <c>true</c>.
		/// </returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		// TODO: Why is this hidden from the designer O_o....
		public bool SupressControlCharacters
		{
			get { return mInnerScintilla.SupressControlCharacters; }
			set { mInnerScintilla.SupressControlCharacters = value; }
		}

		/// <summary>
		/// Gets or sets the current text in the <see cref="Scintilla" /> control.
		/// </summary>
		/// <returns>The text displayed in the control.</returns>
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public string Text
		{
			get { return mInnerScintilla.Text; }
			set { mInnerScintilla.Text = value; }
		}

		/// <summary>
		/// Gets the _length of text in the control.
		/// </summary>
		/// <returns>The number of characters contained in the text of the control.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TextLength
		{
			get { return mInnerScintilla.TextLength; }
		}

		[Category("Behavior")]
		// NEED DESC
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.UndoRedo UndoRedo
		{
			get { return mInnerScintilla.UndoRedo; }
		}

		// NEED CAT
		// NEED DESC
		// NEED DEFAULT
		public bool UseWaitCursor
		{
			get { return mInnerScintilla.UseWaitCursor; }
			set { mInnerScintilla.UseWaitCursor = value; }
		}

		/// <summary>
		/// Gets the <see cref="Whitespace"/> display mode and style behavior associated with the <see cref="Scintilla"/> control.
		/// </summary>
		/// <returns>A <see cref="Whitespace"/> object that represents whitespace display mode and style behavior in a <see cref="Scintilla"/> control.</returns>
		[Category("Appearance")]
		[Description("The display mode and style of whitespace characters.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SN.Whitespace Whitespace
		{
			get { return mInnerScintilla.Whitespace; }
		}

		/// <summary>
		/// Gets or sets the current number of points added to or subtracted from the
		/// font size when text is rendered in a <see cref="Scintilla" /> control.
		/// </summary>
		/// <returns>The number of points by which the contents of the control is zoomed.</returns>
		[Category("Behavior")]
		[Description("Defines the current number of points added to or subtracted from the font size when rendered; 0 is normal viewing.")]
		[DefaultValue(0)]
		public int ZoomFactor
		{
			get { return mInnerScintilla.ZoomFactor; }
			set { mInnerScintilla.ZoomFactor = value; }
		}
		#endregion

		#region Events

		/// <summary>
		/// Occurs when an annotation has changed.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when an annotation has changed.")]
		public event EventHandler<SN.AnnotationChangedEventArgs> AnnotationChanged
		{
			add { mInnerScintilla.AnnotationChanged += value; }
			remove { mInnerScintilla.AnnotationChanged -= value; }
		}

		/// <summary>
		/// Occurs when the user makes a selection from the auto-complete list.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when the user makes a selection from the auto-complete list.")]
		public event EventHandler<SN.AutoCompleteAcceptedEventArgs> AutoCompleteAccepted
		{
			add { mInnerScintilla.AutoCompleteAccepted += value; }
			remove { mInnerScintilla.AutoCompleteAccepted -= value; }
		}

		/// <summary>
		/// Occurs when text is about to be removed from the document.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when text is about to be removed from the document.")]
		public event EventHandler<SN.TextModifiedEventArgs> BeforeTextDelete
		{
			add { mInnerScintilla.BeforeTextDelete += value; }
			remove { mInnerScintilla.BeforeTextDelete -= value; }
		}

		/// <summary>
		/// Occurs when text is about to be inserted into the document.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when text is about to be inserted into the document.")]
		public event EventHandler<SN.TextModifiedEventArgs> BeforeTextInsert
		{
			add { mInnerScintilla.BeforeTextInsert += value; }
			remove { mInnerScintilla.BeforeTextInsert -= value; }
		}

		/// <summary>
		/// Occurs when the value of the <see cref="BorderStyle" /> property has changed.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the value of the BorderStyle property changes.")]
		public event EventHandler BorderStyleChanged
		{
			add { mInnerScintilla.BorderStyleChanged += value; }
			remove { mInnerScintilla.BorderStyleChanged -= value; }
		}

		/// <summary>
		/// Occurs when a user clicks on a call tip.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a user clicks on a call tip.")]
		public event EventHandler<SN.CallTipClickEventArgs> CallTipClick
		{
			add { mInnerScintilla.CallTipClick += value; }
			remove { mInnerScintilla.CallTipClick -= value; }
		}

		/// <summary>
		/// Occurs when the user types an ordinary text character (as opposed to a command character) into the text.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when the user types a text character.")]
		public event EventHandler<SN.CharAddedEventArgs> CharAdded
		{
			add { mInnerScintilla.CharAdded += value; }
			remove { mInnerScintilla.CharAdded -= value; }
		}

		/// <summary>
		/// Occurs when the text or styling of the document changes or is about to change.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when the text or styling of the document changes or is about to change.")]
		public event EventHandler<SN.NativeScintillaEventArgs> DocumentChange
		{
			add { mInnerScintilla.DocumentChange += value; }
			remove { mInnerScintilla.DocumentChange -= value; }
		}

		/// <summary>
		/// Occurs when a <see cref="DropMarker"/> is about to be collected.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a DropMarker is about to be collected.")]
		public event EventHandler<SN.DropMarkerCollectEventArgs> DropMarkerCollect
		{
			add { mInnerScintilla.DropMarkerCollect += value; }
			remove { mInnerScintilla.DropMarkerCollect -= value; }
		}

		/// <summary>
		/// Occurs when a user actions such as a mouse move or key press ends a dwell (hover) activity.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a dwell (hover) activity has ended.")]
		public event EventHandler<SN.ScintillaMouseEventArgs> DwellEnd
		{
			add { mInnerScintilla.DwellEnd += value; }
			remove { mInnerScintilla.DwellEnd -= value; }
		}

		/// <summary>
		/// Occurs when the user hovers the mouse (dwells) in one position for the dwell period.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when the user hovers the mouse (dwells) in one position for the dwell period.")]
		public event EventHandler<SN.ScintillaMouseEventArgs> DwellStart
		{
			add { mInnerScintilla.DwellStart += value; }
			remove { mInnerScintilla.DwellStart -= value; }
		}

		/// <summary>
		/// Occurs when a folding change has occurred.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a folding change has occurred.")]
		public event EventHandler<SN.FoldChangedEventArgs> FoldChanged
		{
			add { mInnerScintilla.FoldChanged += value; }
			remove { mInnerScintilla.FoldChanged -= value; }
		}

		/// <summary>
		/// Occurs when a user clicks on text that is in a style with the hotspot attribute set.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a user clicks on text with the hotspot style.")]
		public event EventHandler<SN.HotspotClickEventArgs> HotspotClick
		{
			add { mInnerScintilla.HotspotClick += value; }
			remove { mInnerScintilla.HotspotClick -= value; }
		}

		/// <summary>
		/// Occurs when a user double-clicks on text that is in a style with the hotspot attribute set.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a user double-clicks on text with the hotspot style.")]
		public event EventHandler<SN.HotspotClickEventArgs> HotspotDoubleClick
		{
			add { mInnerScintilla.HotspotDoubleClick += value; }
			remove { mInnerScintilla.HotspotDoubleClick -= value; }
		}

		/// <summary>
		/// Occurs when a user releases a click on text that is in a style with the hotspot attribute set.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a user releases a click on text with the hotspot style.")]
		public event EventHandler<SN.HotspotClickEventArgs> HotspotReleaseClick
		{
			add { mInnerScintilla.HotspotReleaseClick += value; }
			remove { mInnerScintilla.HotspotReleaseClick -= value; }
		}

		/// <summary>
		/// Occurs when the a clicks or releases the mouse on text that has an indicator.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when the a clicks or releases the mouse on text that has an indicator.")]
		public event EventHandler<SN.ScintillaMouseEventArgs> IndicatorClick
		{
			add { mInnerScintilla.IndicatorClick += value; }
			remove { mInnerScintilla.IndicatorClick -= value; }
		}

		/// <summary>
		/// Occurs when a range of lines that is currently invisible should be made visible.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a range of lines that is currently invisible should be made visible.")]
		public event EventHandler<SN.LinesNeedShownEventArgs> LinesNeedShown
		{
			add { mInnerScintilla.LinesNeedShown += value; }
			remove { mInnerScintilla.LinesNeedShown -= value; }
		}

		/// <summary>
		/// Occurs when the control is first loaded.
		/// </summary>
		[Category("Behavior")]
		[Description("Occurs when the control is first loaded.")]
		public event EventHandler Load
		{
			add { mInnerScintilla.Load += value; }
			remove { mInnerScintilla.Load -= value; }
		}

		/// <summary>
		/// Occurs each time a recordable change occurs.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs each time a recordable change occurs.")]
		public event EventHandler<SN.MacroRecordEventArgs> MacroRecord
		{
			add { mInnerScintilla.MacroRecord += value; }
			remove { mInnerScintilla.MacroRecord -= value; }
		}

		/// <summary>
		/// Occurs when the mouse was clicked inside a margin that was marked as sensitive.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when the mouse was clicked inside a margin that was marked as sensitive.")]
		public event EventHandler<SN.MarginClickEventArgs> MarginClick
		{
			add { mInnerScintilla.MarginClick += value; }
			remove { mInnerScintilla.MarginClick -= value; }
		}

		/// <summary>
		/// Occurs when one or more markers has changed in a line of text.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when one or more markers has changed in a line of text.")]
		public event EventHandler<SN.MarkerChangedEventArgs> MarkerChanged
		{
			add { mInnerScintilla.MarkerChanged += value; }
			remove { mInnerScintilla.MarkerChanged -= value; }
		}

		/// <summary>
		/// Occurs when the value of the <see cref="Modified"> property has changed.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the value of the Modified property changes.")]
		public event EventHandler ModifiedChanged
		{
			add { mInnerScintilla.ModifiedChanged += value; }
			remove { mInnerScintilla.ModifiedChanged -= value; }
		}

		/// <summary>
		/// Occurs when a user tries to modify text when in read-only mode.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when a user tries to modify text when in read-only mode.")]
		public event EventHandler ReadOnlyModifyAttempt
		{
			add { mInnerScintilla.ReadOnlyModifyAttempt += value; }
			remove { mInnerScintilla.ReadOnlyModifyAttempt -= value; }
		}

		/// <summary>
		/// Occurs when the control is scrolled.
		/// </summary>
		[Category("Action")]
		[Description("Occurs when the control is scrolled.")]
		// TODO: Make this use the WPF scroll event args rather than the winforms one.
		public event EventHandler<Forms.ScrollEventArgs> Scroll
		{
			add { mInnerScintilla.Scroll += value; }
			remove { mInnerScintilla.Scroll -= value; }
		}

		/// <summary>
		/// Occurs when the selection has changed.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when the selection has changed.")]
		public event EventHandler SelectionChanged
		{
			add { mInnerScintilla.SelectionChanged += value; }
			remove { mInnerScintilla.SelectionChanged -= value; }
		}

		/// <summary>
		/// Occurs when the control is about to display or print text that requires styling.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when the control is about to display or print text that requires styling.")]
		public event EventHandler<SN.StyleNeededEventArgs> StyleNeeded
		{
			add { mInnerScintilla.StyleNeeded += value; }
			remove { mInnerScintilla.StyleNeeded -= value; }
		}

		/// <summary>
		/// Occurs when text has been modified in the document.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when text has been modified in the document.")]
		public event EventHandler TextChanged
		{
			add { mInnerScintilla.TextChanged += value; }
			remove { mInnerScintilla.TextChanged -= value; }
		}

		/// <summary>
		/// Occurs when text has been removed from the document.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when text has been removed from the document.")]
		public event EventHandler<SN.TextModifiedEventArgs> TextDeleted
		{
			add { mInnerScintilla.TextDeleted += value; }
			remove { mInnerScintilla.TextDeleted -= value; }
		}

		/// <summary>
		/// Occurs when text has been inserted into the document.
		/// </summary>
		[Category("Scintilla")]
		[Description("Occurs when text has been inserted into the document.")]
		public event EventHandler<SN.TextModifiedEventArgs> TextInserted
		{
			add { mInnerScintilla.TextInserted += value; }
			remove { mInnerScintilla.TextInserted -= value; }
		}

		/// <summary>
		/// Occurs when the value of the <see cref="ZoomFactor"/> property changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the value of the ZoomFactor property changes.")]
		public event EventHandler ZoomFactorChanged
		{
			add { mInnerScintilla.ZoomFactorChanged += value; }
			remove { mInnerScintilla.ZoomFactorChanged -= value; }
		}
		#endregion

		#region Methods

		/// <summary>
		/// Adds a line _end marker to the _end of the document
		/// </summary>
		public void AddLastLineEnd() { mInnerScintilla.AddLastLineEnd(); }
		/// <summary>
		/// Appends a copy of the specified string to the _end of this instance.
		/// </summary>
		/// <param name="text">The <see cref="String"/> to append.</param>
		/// <returns>A <see cref="Range"/> representing the appended text.</returns>
		public SN.Range AppendText(string text) { return mInnerScintilla.AppendText(text); }
		public char CharAt(int position) { return mInnerScintilla.CharAt(position); }
        /// <summary>
        /// Sends the specified message directly to the native Scintilla window,
        /// bypassing any managed APIs.
        /// </summary>
        /// <param name="msg">The message ID.</param>
        /// <param name="wParam">The message <c>wparam</c> field.</param>
        /// <param name="lParam">The message <c>lparam</c> field.</param>
        /// <returns>An <see cref="IntPtr"/> representing the result of the message request.</returns>
        /// <remarks>
        /// Warning: The Surgeon General Has Determined that Calling the Underlying Scintilla
        /// Window Directly May Result in Unexpected Behavior!
        /// </remarks>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual IntPtr DirectMessage(int msg, IntPtr wParam, IntPtr lParam) { return mInnerScintilla.DirectMessage(msg, wParam, lParam); }
		/// <summary>
		/// Exports a HTML representation of the current document.
		/// </summary>
		/// <returns>A <see cref="String"/> containing the contents of the document formatted as HTML.</returns>
		/// <remarks>Only ASCII documents are supported. Other encoding types have undefined behavior.</remarks>
		public string ExportHtml() { return mInnerScintilla.ExportHtml(); }
		/// <summary>
		/// Exports a HTML representation of the current document.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/>with which to write. </param>
		/// <param name="title">The title of the HTML document.</param>
		/// <param name="allStyles">
		/// <c>true</c> to output all styles including those not
		/// used in the document; otherwise, <c>false</c>.
		/// </param>
		/// <remarks>Only ASCII documents are supported. Other encoding types have undefined behavior.</remarks>
		public void ExportHtml(TextWriter writer, string title, bool allStyles) { mInnerScintilla.ExportHtml(writer, title, allStyles); }
		public int FindColumn(int line, int column) { return mInnerScintilla.FindColumn(line, column); }
		public int GetColumn(int position) { return mInnerScintilla.GetColumn(position); }
		/// <summary>
		/// Gets the text of the line containing the caret.
		/// </summary>
		/// <returns>A <see cref="String" /> representing the text of the line containing the caret.</returns>
		public string GetCurrentLine() { return mInnerScintilla.GetCurrentLine(); }
		/// <summary>
		/// Gets the text of the line containing the caret and the current caret position within that line.
		/// </summary>
		/// <param name="caretPosition">When this method returns, contains the byte offset of the current caret position with the line.</param>
		/// <returns>A <see cref="String" /> representing the text of the line containing the caret.</returns>
		public string GetCurrentLine(out int caretPosition) { return mInnerScintilla.GetCurrentLine(out caretPosition); }
		public SN.Range GetRange() { return mInnerScintilla.GetRange(); }
		public SN.Range GetRange(int position) { return mInnerScintilla.GetRange(position); }
		public SN.Range GetRange(int startPosition, int endPosition) { return mInnerScintilla.GetRange(startPosition, endPosition); }
		/// <summary>
		/// Gets a word from the specified position
		/// </summary>
		public string GetWordFromPosition(int position) { return mInnerScintilla.GetWordFromPosition(position); }
		/// <summary>
		/// Inserts text at the current cursor position
		/// </summary>
		/// <param name="text">Text to insert</param>
		/// <returns>The range inserted</returns>
		public SN.Range InsertText(string text) { return mInnerScintilla.InsertText(text); }
		/// <summary>
		/// Inserts text at the given position
		/// </summary>
		/// <param name="position">The position to insert text in</param>
		/// <param name="text">Text to insert</param>
		/// <returns>The text range inserted</returns>
		public SN.Range InsertText(int position, string text) { return mInnerScintilla.InsertText(position, text); }
		public int PointXFromPosition(int position) { return mInnerScintilla.PointXFromPosition(position); }
		public int PointYFromPosition(int position) { return mInnerScintilla.PointYFromPosition(position); }
		public int PositionFromPoint(int x, int y) { return mInnerScintilla.PositionFromPoint(x, y); }
		public int PositionFromPointClose(int x, int y) { return mInnerScintilla.PositionFromPointClose(x, y); }
		/// <summary>
		/// Checks that if the specified position is on comment.
		/// </summary>
		public bool PositionIsOnComment(int position) { return mInnerScintilla.PositionIsOnComment(position); }
		/// <summary>
		/// Checks that if the specified position is on comment.
		/// </summary>
		public bool PositionIsOnComment(int position, SN.Lexer lexer) { return mInnerScintilla.PositionIsOnComment(position, lexer); }
		/// <summary>
		/// Increases the <see cref="ZoomFactor"/> property by 1 point to a maximum of 20;
		/// </summary>
		public void ZoomIn() { mInnerScintilla.ZoomIn(); }
		/// <summary>
		/// Decreases the <see cref="ZoomFactor"/> property by 1 point to a minimum -10.
		/// </summary>
		public void ZoomOut() { mInnerScintilla.ZoomOut(); }

		#endregion

		#region INativeScintilla

#pragma warning disable 618

		#region Methods

		void INativeScintilla.AddRefDocument(IntPtr pDoc) { ((INativeScintilla)mInnerScintilla).AddRefDocument(pDoc); }
		void INativeScintilla.AddStyledText(int length, byte[] s) { ((INativeScintilla)mInnerScintilla).AddStyledText(length, s); }
		void INativeScintilla.AddText(int length, string s) { ((INativeScintilla)mInnerScintilla).AddText(length, s); }
		void INativeScintilla.Allocate(int bytes) { ((INativeScintilla)mInnerScintilla).Allocate(bytes); }
		void INativeScintilla.AppendText(int length, string s) { ((INativeScintilla)mInnerScintilla).AppendText(length, s); }
		void INativeScintilla.AssignCmdKey(int keyDefinition, int sciCommand) { ((INativeScintilla)mInnerScintilla).AssignCmdKey(keyDefinition, sciCommand); }
		bool INativeScintilla.AutoCActive() { return ((INativeScintilla)mInnerScintilla).AutoCActive(); }
		void INativeScintilla.AutoCCancel() { ((INativeScintilla)mInnerScintilla).AutoCCancel(); }
		void INativeScintilla.AutoCComplete() { ((INativeScintilla)mInnerScintilla).AutoCComplete(); }
		bool INativeScintilla.AutoCGetAutoHide() { return ((INativeScintilla)mInnerScintilla).AutoCGetAutoHide(); }
		bool INativeScintilla.AutoCGetCancelAtStart() { return ((INativeScintilla)mInnerScintilla).AutoCGetCancelAtStart(); }
		bool INativeScintilla.AutoCGetChooseSingle() { return ((INativeScintilla)mInnerScintilla).AutoCGetChooseSingle(); }
		int INativeScintilla.AutoCGetCurrent() { return ((INativeScintilla)mInnerScintilla).AutoCGetCurrent(); }
		bool INativeScintilla.AutoCGetDropRestOfWord() { return ((INativeScintilla)mInnerScintilla).AutoCGetDropRestOfWord(); }
		bool INativeScintilla.AutoCGetIgnoreCase() { return ((INativeScintilla)mInnerScintilla).AutoCGetIgnoreCase(); }
		int INativeScintilla.AutoCGetMaxHeight() { return ((INativeScintilla)mInnerScintilla).AutoCGetMaxHeight(); }
		int INativeScintilla.AutoCGetMaxWidth() { return ((INativeScintilla)mInnerScintilla).AutoCGetMaxWidth(); }
		char INativeScintilla.AutoCGetSeparator() { return ((INativeScintilla)mInnerScintilla).AutoCGetSeparator(); }
		char INativeScintilla.AutoCGetTypeSeparator() { return ((INativeScintilla)mInnerScintilla).AutoCGetTypeSeparator(); }
		int INativeScintilla.AutoCPosStart() { return ((INativeScintilla)mInnerScintilla).AutoCPosStart(); }
		void INativeScintilla.AutoCSelect(string select) { ((INativeScintilla)mInnerScintilla).AutoCSelect(select); }
		void INativeScintilla.AutoCSetAutoHide(bool autoHide) { ((INativeScintilla)mInnerScintilla).AutoCSetAutoHide(autoHide); }
		void INativeScintilla.AutoCSetCancelAtStart(bool cancel) { ((INativeScintilla)mInnerScintilla).AutoCSetCancelAtStart(cancel); }
		void INativeScintilla.AutoCSetChooseSingle(bool chooseSingle) { ((INativeScintilla)mInnerScintilla).AutoCSetChooseSingle(chooseSingle); }
		void INativeScintilla.AutoCSetDropRestOfWord(bool dropRestOfWord) { ((INativeScintilla)mInnerScintilla).AutoCSetDropRestOfWord(dropRestOfWord); }
		void INativeScintilla.AutoCSetFillUps(string chars) { ((INativeScintilla)mInnerScintilla).AutoCSetFillUps(chars); }
		void INativeScintilla.AutoCSetIgnoreCase(bool ignoreCase) { ((INativeScintilla)mInnerScintilla).AutoCSetIgnoreCase(ignoreCase); }
		void INativeScintilla.AutoCSetMaxHeight(int rowCount) { ((INativeScintilla)mInnerScintilla).AutoCSetMaxHeight(rowCount); }
		void INativeScintilla.AutoCSetMaxWidth(int characterCount) { ((INativeScintilla)mInnerScintilla).AutoCSetMaxWidth(characterCount); }
		void INativeScintilla.AutoCSetSeparator(char separator) { ((INativeScintilla)mInnerScintilla).AutoCSetSeparator(separator); }
		void INativeScintilla.AutoCSetTypeSeparator(char separatorCharacter) { ((INativeScintilla)mInnerScintilla).AutoCSetTypeSeparator(separatorCharacter); }
		void INativeScintilla.AutoCShow(int lenEntered, string list) { ((INativeScintilla)mInnerScintilla).AutoCShow(lenEntered, list); }
		void INativeScintilla.AutoCStops(string chars) { ((INativeScintilla)mInnerScintilla).AutoCStops(chars); }
		void INativeScintilla.BackTab() { ((INativeScintilla)mInnerScintilla).BackTab(); }
		void INativeScintilla.BeginUndoAction() { ((INativeScintilla)mInnerScintilla).BeginUndoAction(); }
		void INativeScintilla.BraceBadLight(int pos1) { ((INativeScintilla)mInnerScintilla).BraceBadLight(pos1); }
		void INativeScintilla.BraceHighlight(int pos1, int pos2) { ((INativeScintilla)mInnerScintilla).BraceHighlight(pos1, pos2); }
		int INativeScintilla.BraceMatch(int pos, int maxReStyle) { return ((INativeScintilla)mInnerScintilla).BraceMatch(pos, maxReStyle); }
		bool INativeScintilla.CallTipActive() { return ((INativeScintilla)mInnerScintilla).CallTipActive(); }
		void INativeScintilla.CallTipCancel() { ((INativeScintilla)mInnerScintilla).CallTipCancel(); }
		int INativeScintilla.CallTipGetPosStart() { return ((INativeScintilla)mInnerScintilla).CallTipGetPosStart(); }
		void INativeScintilla.CallTipSetBack(int colour) { ((INativeScintilla)mInnerScintilla).CallTipSetBack(colour); }
		void INativeScintilla.CallTipSetFore(int colour) { ((INativeScintilla)mInnerScintilla).CallTipSetFore(colour); }
		void INativeScintilla.CallTipSetForeHlt(int colour) { ((INativeScintilla)mInnerScintilla).CallTipSetForeHlt(colour); }
		void INativeScintilla.CallTipSetHlt(int hlStart, int hlEnd) { ((INativeScintilla)mInnerScintilla).CallTipSetHlt(hlStart, hlEnd); }
		void INativeScintilla.CallTipShow(int posStart, string definition) { ((INativeScintilla)mInnerScintilla).CallTipShow(posStart, definition); }
		void INativeScintilla.CallTipUseStyle(int tabsize) { ((INativeScintilla)mInnerScintilla).CallTipUseStyle(tabsize); }
		void INativeScintilla.Cancel() { ((INativeScintilla)mInnerScintilla).Cancel(); }
		bool INativeScintilla.CanRedo() { return ((INativeScintilla)mInnerScintilla).CanRedo(); }
		bool INativeScintilla.CanUndo() { return ((INativeScintilla)mInnerScintilla).CanUndo(); }
		void INativeScintilla.CharLeft() { ((INativeScintilla)mInnerScintilla).CharLeft(); }
		void INativeScintilla.CharLeftExtend() { ((INativeScintilla)mInnerScintilla).CharLeftExtend(); }
		void INativeScintilla.CharLeftRectExtend() { ((INativeScintilla)mInnerScintilla).CharLeftRectExtend(); }
		void INativeScintilla.CharRight() { ((INativeScintilla)mInnerScintilla).CharRight(); }
		void INativeScintilla.CharRightExtend() { ((INativeScintilla)mInnerScintilla).CharRightExtend(); }
		void INativeScintilla.CharRightRectExtend() { ((INativeScintilla)mInnerScintilla).CharRightRectExtend(); }
		void INativeScintilla.ChooseCaretX() { ((INativeScintilla)mInnerScintilla).ChooseCaretX(); }
		void INativeScintilla.Clear() { ((INativeScintilla)mInnerScintilla).Clear(); }
		void INativeScintilla.ClearAll() { ((INativeScintilla)mInnerScintilla).ClearAll(); }
		void INativeScintilla.ClearAllCmdKeys() { ((INativeScintilla)mInnerScintilla).ClearAllCmdKeys(); }
		void INativeScintilla.ClearCmdKey(int keyDefinition) { ((INativeScintilla)mInnerScintilla).ClearCmdKey(keyDefinition); }
		void INativeScintilla.ClearDocumentStyle() { ((INativeScintilla)mInnerScintilla).ClearDocumentStyle(); }
		void INativeScintilla.ClearRegisteredImages() { ((INativeScintilla)mInnerScintilla).ClearRegisteredImages(); }
		void INativeScintilla.Colourise(int start, int end) { ((INativeScintilla)mInnerScintilla).Colourise(start, end); }
		void INativeScintilla.ConvertEols(int eolMode) { ((INativeScintilla)mInnerScintilla).ConvertEols(eolMode); }
		IntPtr INativeScintilla.CreateDocument() { return ((INativeScintilla)mInnerScintilla).CreateDocument(); }
		void INativeScintilla.DeleteBack() { ((INativeScintilla)mInnerScintilla).DeleteBack(); }
		void INativeScintilla.DeleteBackNotLine() { ((INativeScintilla)mInnerScintilla).DeleteBackNotLine(); }
		void INativeScintilla.DelLineLeft() { ((INativeScintilla)mInnerScintilla).DelLineLeft(); }
		void INativeScintilla.DelLineRight() { ((INativeScintilla)mInnerScintilla).DelLineRight(); }
		void INativeScintilla.DelWordLeft() { ((INativeScintilla)mInnerScintilla).DelWordLeft(); }
		void INativeScintilla.DelWordRight() { ((INativeScintilla)mInnerScintilla).DelWordRight(); }
		int INativeScintilla.DocLineFromVisible(int displayLine) { return ((INativeScintilla)mInnerScintilla).DocLineFromVisible(displayLine); }
		void INativeScintilla.DocumentEnd() { ((INativeScintilla)mInnerScintilla).DocumentEnd(); }
		void INativeScintilla.DocumentEndExtend() { ((INativeScintilla)mInnerScintilla).DocumentEndExtend(); }
		void INativeScintilla.DocumentStart() { ((INativeScintilla)mInnerScintilla).DocumentStart(); }
		void INativeScintilla.DocumentStartExtend() { ((INativeScintilla)mInnerScintilla).DocumentStartExtend(); }
		void INativeScintilla.EditToggleOvertype() { ((INativeScintilla)mInnerScintilla).EditToggleOvertype(); }
		void INativeScintilla.EmptyUndoBuffer() { ((INativeScintilla)mInnerScintilla).EmptyUndoBuffer(); }
		int INativeScintilla.EncodeFromUtf8(string utf8, out string encoded) { return ((INativeScintilla)mInnerScintilla).EncodeFromUtf8(utf8, out encoded); }
		void INativeScintilla.EndUndoAction() { ((INativeScintilla)mInnerScintilla).EndUndoAction(); }
		void INativeScintilla.EnsureVisible(int line) { ((INativeScintilla)mInnerScintilla).EnsureVisible(line); }
		void INativeScintilla.EnsureVisibleEnforcePolicy(int line) { ((INativeScintilla)mInnerScintilla).EnsureVisibleEnforcePolicy(line); }
		int INativeScintilla.FindColumn(int line, int column) { return ((INativeScintilla)mInnerScintilla).FindColumn(line, column); }
		int INativeScintilla.FindText(int searchFlags, ref TextToFind ttf) { return ((INativeScintilla)mInnerScintilla).FindText(searchFlags, ref ttf); }
		int INativeScintilla.FormatRange(bool bDraw, ref RangeToFormat pfr) { return ((INativeScintilla)mInnerScintilla).FormatRange(bDraw, ref pfr); }
		void INativeScintilla.FormFeed() { ((INativeScintilla)mInnerScintilla).FormFeed(); }
		int INativeScintilla.GetAnchor() { return ((INativeScintilla)mInnerScintilla).GetAnchor(); }
		bool INativeScintilla.GetBackSpaceUnIndents() { return ((INativeScintilla)mInnerScintilla).GetBackSpaceUnIndents(); }
		bool INativeScintilla.GetBufferedDraw() { return ((INativeScintilla)mInnerScintilla).GetBufferedDraw(); }
		int INativeScintilla.GetCaretFore() { return ((INativeScintilla)mInnerScintilla).GetCaretFore(); }
		int INativeScintilla.GetCaretLineBack() { return ((INativeScintilla)mInnerScintilla).GetCaretLineBack(); }
		int INativeScintilla.GetCaretLineBackAlpha() { return ((INativeScintilla)mInnerScintilla).GetCaretLineBackAlpha(); }
		bool INativeScintilla.GetCaretLineVisible() { return ((INativeScintilla)mInnerScintilla).GetCaretLineVisible(); }
		int INativeScintilla.GetCaretPeriod() { return ((INativeScintilla)mInnerScintilla).GetCaretPeriod(); }
		bool INativeScintilla.GetCaretSticky() { return ((INativeScintilla)mInnerScintilla).GetCaretSticky(); }
		int INativeScintilla.GetCaretStyle() { return ((INativeScintilla)mInnerScintilla).GetCaretStyle(); }
		int INativeScintilla.GetCaretWidth() { return ((INativeScintilla)mInnerScintilla).GetCaretWidth(); }
		char INativeScintilla.GetCharAt(int position) { return ((INativeScintilla)mInnerScintilla).GetCharAt(position); }
		int INativeScintilla.GetColumn(int position) { return ((INativeScintilla)mInnerScintilla).GetColumn(position); }
		int INativeScintilla.GetControlCharSymbol() { return ((INativeScintilla)mInnerScintilla).GetControlCharSymbol(); }
		int INativeScintilla.GetCurrentPos() { return ((INativeScintilla)mInnerScintilla).GetCurrentPos(); }
		int INativeScintilla.GetCursor() { return ((INativeScintilla)mInnerScintilla).GetCursor(); }
		IntPtr INativeScintilla.GetDocPointer() { return ((INativeScintilla)mInnerScintilla).GetDocPointer(); }
		int INativeScintilla.GetEdgeColour() { return ((INativeScintilla)mInnerScintilla).GetEdgeColour(); }
		int INativeScintilla.GetEdgeColumn() { return ((INativeScintilla)mInnerScintilla).GetEdgeColumn(); }
		int INativeScintilla.GetEdgeMode() { return ((INativeScintilla)mInnerScintilla).GetEdgeMode(); }
		int INativeScintilla.GetEndStyled() { return ((INativeScintilla)mInnerScintilla).GetEndStyled(); }
		int INativeScintilla.GetEolMode() { return ((INativeScintilla)mInnerScintilla).GetEolMode(); }
		bool INativeScintilla.GetFocus() { return ((INativeScintilla)mInnerScintilla).GetFocus(); }
		bool INativeScintilla.GetFoldExpanded(int line) { return ((INativeScintilla)mInnerScintilla).GetFoldExpanded(line); }
		uint INativeScintilla.GetFoldLevel(int line) { return ((INativeScintilla)mInnerScintilla).GetFoldLevel(line); }
		int INativeScintilla.GetFoldParent(int line) { return ((INativeScintilla)mInnerScintilla).GetFoldParent(line); }
		int INativeScintilla.GetHighlightGuide() { return ((INativeScintilla)mInnerScintilla).GetHighlightGuide(); }
		int INativeScintilla.GetHotspotActiveBack() { return ((INativeScintilla)mInnerScintilla).GetHotspotActiveBack(); }
		int INativeScintilla.GetHotspotActiveFore() { return ((INativeScintilla)mInnerScintilla).GetHotspotActiveFore(); }
		bool INativeScintilla.GetHotspotActiveUnderline() { return ((INativeScintilla)mInnerScintilla).GetHotspotActiveUnderline(); }
		bool INativeScintilla.GetHotspotSingleLine() { return ((INativeScintilla)mInnerScintilla).GetHotspotSingleLine(); }
		int INativeScintilla.GetIndicatorCurrent() { return ((INativeScintilla)mInnerScintilla).GetIndicatorCurrent(); }
		int INativeScintilla.GetIndicatorValue() { return ((INativeScintilla)mInnerScintilla).GetIndicatorValue(); }
		int INativeScintilla.GetIndent() { return ((INativeScintilla)mInnerScintilla).GetIndent(); }
		bool INativeScintilla.GetIndentationGuides() { return ((INativeScintilla)mInnerScintilla).GetIndentationGuides(); }
		int INativeScintilla.GetLastChild(int line, int level) { return ((INativeScintilla)mInnerScintilla).GetLastChild(line, level); }
		int INativeScintilla.GetLength() { return ((INativeScintilla)mInnerScintilla).GetLength(); }
		int INativeScintilla.GetLexer() { return ((INativeScintilla)mInnerScintilla).GetLexer(); }
		int INativeScintilla.GetLine(int line, out string text) { return ((INativeScintilla)mInnerScintilla).GetLine(line, out text); }
		int INativeScintilla.GetLineEndPosition(int line) { return ((INativeScintilla)mInnerScintilla).GetLineEndPosition(line); }
		int INativeScintilla.GetLineIndentation(int line) { return ((INativeScintilla)mInnerScintilla).GetLineIndentation(line); }
		int INativeScintilla.GetLineIndentPosition(int line) { return ((INativeScintilla)mInnerScintilla).GetLineIndentPosition(line); }
		int INativeScintilla.GetLineSelEndPosition(int line) { return ((INativeScintilla)mInnerScintilla).GetLineSelEndPosition(line); }
		int INativeScintilla.GetLineSelStartPosition(int line) { return ((INativeScintilla)mInnerScintilla).GetLineSelStartPosition(line); }
		int INativeScintilla.GetLineState(int line) { return ((INativeScintilla)mInnerScintilla).GetLineState(line); }
		bool INativeScintilla.GetLineVisible(int line) { return ((INativeScintilla)mInnerScintilla).GetLineVisible(line); }
		int INativeScintilla.GetMarginLeft() { return ((INativeScintilla)mInnerScintilla).GetMarginLeft(); }
		int INativeScintilla.GetMarginMaskN(int margin) { return ((INativeScintilla)mInnerScintilla).GetMarginMaskN(margin); }
		int INativeScintilla.GetMarginRight() { return ((INativeScintilla)mInnerScintilla).GetMarginRight(); }
		bool INativeScintilla.GetMarginSensitiveN(int margin) { return ((INativeScintilla)mInnerScintilla).GetMarginSensitiveN(margin); }
		int INativeScintilla.GetMarginTypeN(int margin) { return ((INativeScintilla)mInnerScintilla).GetMarginTypeN(margin); }
		int INativeScintilla.GetMarginWidthN(int margin) { return ((INativeScintilla)mInnerScintilla).GetMarginWidthN(margin); }
		int INativeScintilla.GetMaxLineState() { return ((INativeScintilla)mInnerScintilla).GetMaxLineState(); }
		int INativeScintilla.GetModEventMask() { return ((INativeScintilla)mInnerScintilla).GetModEventMask(); }
		bool INativeScintilla.GetModify() { return ((INativeScintilla)mInnerScintilla).GetModify(); }
		bool INativeScintilla.GetMouseDownCaptures() { return ((INativeScintilla)mInnerScintilla).GetMouseDownCaptures(); }
		int INativeScintilla.GetMouseDwellTime() { return ((INativeScintilla)mInnerScintilla).GetMouseDwellTime(); }
		bool INativeScintilla.GetOvertype() { return ((INativeScintilla)mInnerScintilla).GetOvertype(); }
		int INativeScintilla.GetPrintColourMode() { return ((INativeScintilla)mInnerScintilla).GetPrintColourMode(); }
		int INativeScintilla.GetPrintMagnification() { return ((INativeScintilla)mInnerScintilla).GetPrintMagnification(); }
		int INativeScintilla.GetPrintWrapMode() { return ((INativeScintilla)mInnerScintilla).GetPrintWrapMode(); }
		void INativeScintilla.GetProperty(string key, out string value) { ((INativeScintilla)mInnerScintilla).GetProperty(key, out value); }
		void INativeScintilla.GetPropertyExpanded(string key, out string value) { ((INativeScintilla)mInnerScintilla).GetPropertyExpanded(key, out value); }
		int INativeScintilla.GetPropertyInt(string key, int @default) { return ((INativeScintilla)mInnerScintilla).GetPropertyInt(key, @default); }
		bool INativeScintilla.GetReadOnly() { return ((INativeScintilla)mInnerScintilla).GetReadOnly(); }
		int INativeScintilla.GetSearchFlags() { return ((INativeScintilla)mInnerScintilla).GetSearchFlags(); }
		int INativeScintilla.GetSelectionEnd() { return ((INativeScintilla)mInnerScintilla).GetSelectionEnd(); }
		int INativeScintilla.GetSelectionMode() { return ((INativeScintilla)mInnerScintilla).GetSelectionMode(); }
		int INativeScintilla.GetSelectionStart() { return ((INativeScintilla)mInnerScintilla).GetSelectionStart(); }
		void INativeScintilla.GetSelText(out string text) { ((INativeScintilla)mInnerScintilla).GetSelText(out text); }
		int INativeScintilla.GetStatus() { return ((INativeScintilla)mInnerScintilla).GetStatus(); }
		byte INativeScintilla.GetStyleAt(int position) { return ((INativeScintilla)mInnerScintilla).GetStyleAt(position); }
		[Obsolete("The modern style indicators make this obsolete, this should always be 7")]
		int INativeScintilla.GetStyleBits() { return ((INativeScintilla)mInnerScintilla).GetStyleBits(); }
		int INativeScintilla.GetStyleBitsNeeded() { return ((INativeScintilla)mInnerScintilla).GetStyleBitsNeeded(); }
		void INativeScintilla.GetStyledText(ref TextRange tr) { ((INativeScintilla)mInnerScintilla).GetStyledText(ref tr); }
		bool INativeScintilla.GetTabIndents() { return ((INativeScintilla)mInnerScintilla).GetTabIndents(); }
		int INativeScintilla.GetTabWidth() { return ((INativeScintilla)mInnerScintilla).GetTabWidth(); }
		int INativeScintilla.GetTargetEnd() { return ((INativeScintilla)mInnerScintilla).GetTargetEnd(); }
		int INativeScintilla.GetTargetStart() { return ((INativeScintilla)mInnerScintilla).GetTargetStart(); }
		int INativeScintilla.GetText(int length, out string text) { return ((INativeScintilla)mInnerScintilla).GetText(length, out text); }
		int INativeScintilla.GetTextLength() { return ((INativeScintilla)mInnerScintilla).GetTextLength(); }
		int INativeScintilla.GetTextRange(ref TextRange tr) { return ((INativeScintilla)mInnerScintilla).GetTextRange(ref tr); }
		bool INativeScintilla.GetTwoPhaseDraw() { return ((INativeScintilla)mInnerScintilla).GetTwoPhaseDraw(); }
		bool INativeScintilla.GetUndoCollection() { return ((INativeScintilla)mInnerScintilla).GetUndoCollection(); }
		bool INativeScintilla.GetUsePalette() { return ((INativeScintilla)mInnerScintilla).GetUsePalette(); }
		bool INativeScintilla.GetUseTabs() { return ((INativeScintilla)mInnerScintilla).GetUseTabs(); }
		bool INativeScintilla.GetViewEol() { return ((INativeScintilla)mInnerScintilla).GetViewEol(); }
		int INativeScintilla.GetViewWs() { return ((INativeScintilla)mInnerScintilla).GetViewWs(); }
		void INativeScintilla.GotoLine(int line) { ((INativeScintilla)mInnerScintilla).GotoLine(line); }
		void INativeScintilla.GotoPos(int position) { ((INativeScintilla)mInnerScintilla).GotoPos(position); }
		void INativeScintilla.GrabFocus() { ((INativeScintilla)mInnerScintilla).GrabFocus(); }
		void INativeScintilla.HideLines(int lineStart, int lineEnd) { ((INativeScintilla)mInnerScintilla).HideLines(lineStart, lineEnd); }
		void INativeScintilla.HideSelection(bool hide) { ((INativeScintilla)mInnerScintilla).HideSelection(hide); }
		void INativeScintilla.Home() { ((INativeScintilla)mInnerScintilla).Home(); }
		void INativeScintilla.HomeDisplay() { ((INativeScintilla)mInnerScintilla).HomeDisplay(); }
		void INativeScintilla.HomeDisplayExtend() { ((INativeScintilla)mInnerScintilla).HomeDisplayExtend(); }
		void INativeScintilla.HomeExtend() { ((INativeScintilla)mInnerScintilla).HomeExtend(); }
		void INativeScintilla.HomeRectExtend() { ((INativeScintilla)mInnerScintilla).HomeRectExtend(); }
		void INativeScintilla.HomeWrap() { ((INativeScintilla)mInnerScintilla).HomeWrap(); }
		void INativeScintilla.HomeWrapExtend() { ((INativeScintilla)mInnerScintilla).HomeWrapExtend(); }
		uint INativeScintilla.IndicatorAllOnFor(int position) { return ((INativeScintilla)mInnerScintilla).IndicatorAllOnFor(position); }
		void INativeScintilla.IndicatorClearRange(int position, int fillLength) { ((INativeScintilla)mInnerScintilla).IndicatorClearRange(position, fillLength); }
		void INativeScintilla.IndicatorFillRange(int position, int fillLength) { ((INativeScintilla)mInnerScintilla).IndicatorFillRange(position, fillLength); }
		int INativeScintilla.IndicatorEnd(int indicator, int position) { return ((INativeScintilla)mInnerScintilla).IndicatorEnd(indicator, position); }
		int INativeScintilla.IndicatorStart(int indicator, int position) { return ((INativeScintilla)mInnerScintilla).IndicatorStart(indicator, position); }
		int INativeScintilla.IndicatorValueAt(int indicator, int position) { return ((INativeScintilla)mInnerScintilla).IndicatorValueAt(indicator, position); }
		void INativeScintilla.InsertText(int pos, string text) { ((INativeScintilla)mInnerScintilla).InsertText(pos, text); }
		void INativeScintilla.LineCopy() { ((INativeScintilla)mInnerScintilla).LineCopy(); }
		void INativeScintilla.LineCut() { ((INativeScintilla)mInnerScintilla).LineCut(); }
		void INativeScintilla.LineDelete() { ((INativeScintilla)mInnerScintilla).LineDelete(); }
		void INativeScintilla.LineDown() { ((INativeScintilla)mInnerScintilla).LineDown(); }
		void INativeScintilla.LineDownExtend() { ((INativeScintilla)mInnerScintilla).LineDownExtend(); }
		void INativeScintilla.LineDownRectExtend() { ((INativeScintilla)mInnerScintilla).LineDownRectExtend(); }
		void INativeScintilla.LineDuplicate() { ((INativeScintilla)mInnerScintilla).LineDuplicate(); }
		void INativeScintilla.LineEnd() { ((INativeScintilla)mInnerScintilla).LineEnd(); }
		void INativeScintilla.LineEndDisplay() { ((INativeScintilla)mInnerScintilla).LineEndDisplay(); }
		void INativeScintilla.LineEndDisplayExtend() { ((INativeScintilla)mInnerScintilla).LineEndDisplayExtend(); }
		void INativeScintilla.LineEndExtend() { ((INativeScintilla)mInnerScintilla).LineEndExtend(); }
		void INativeScintilla.LineEndRectExtend() { ((INativeScintilla)mInnerScintilla).LineEndRectExtend(); }
		void INativeScintilla.LineEndWrap() { ((INativeScintilla)mInnerScintilla).LineEndWrap(); }
		void INativeScintilla.LineEndWrapExtend() { ((INativeScintilla)mInnerScintilla).LineEndWrapExtend(); }
		int INativeScintilla.LineFromPosition(int pos) { return ((INativeScintilla)mInnerScintilla).LineFromPosition(pos); }
		int INativeScintilla.LineLength(int line) { return ((INativeScintilla)mInnerScintilla).LineLength(line); }
		void INativeScintilla.LineScrollDown() { ((INativeScintilla)mInnerScintilla).LineScrollDown(); }
		void INativeScintilla.LineScrollUp() { ((INativeScintilla)mInnerScintilla).LineScrollUp(); }
		int INativeScintilla.LinesOnScreen() { return ((INativeScintilla)mInnerScintilla).LinesOnScreen(); }
		void INativeScintilla.LineTranspose() { ((INativeScintilla)mInnerScintilla).LineTranspose(); }
		void INativeScintilla.LineUp() { ((INativeScintilla)mInnerScintilla).LineUp(); }
		void INativeScintilla.LineUpExtend() { ((INativeScintilla)mInnerScintilla).LineUpExtend(); }
		void INativeScintilla.LineUpRectExtend() { ((INativeScintilla)mInnerScintilla).LineUpRectExtend(); }
		void INativeScintilla.LoadLexerLibrary(string path) { ((INativeScintilla)mInnerScintilla).LoadLexerLibrary(path); }
		void INativeScintilla.LowerCase() { ((INativeScintilla)mInnerScintilla).LowerCase(); }
		int INativeScintilla.MarkerAdd(int line, int markerNumber) { return ((INativeScintilla)mInnerScintilla).MarkerAdd(line, markerNumber); }
		void INativeScintilla.MarkerAddSet(int line, uint markerMask) { ((INativeScintilla)mInnerScintilla).MarkerAddSet(line, markerMask); }
		void INativeScintilla.MarkerDefine(int markerNumber, int markerSymbol) { ((INativeScintilla)mInnerScintilla).MarkerDefine(markerNumber, markerSymbol); }
		void INativeScintilla.MarkerDefinePixmap(int markerNumber, string xpm) { ((INativeScintilla)mInnerScintilla).MarkerDefinePixmap(markerNumber, xpm); }
		void INativeScintilla.MarkerDelete(int line, int markerNumber) { ((INativeScintilla)mInnerScintilla).MarkerDelete(line, markerNumber); }
		void INativeScintilla.MarkerDeleteAll(int markerNumber) { ((INativeScintilla)mInnerScintilla).MarkerDeleteAll(markerNumber); }
		void INativeScintilla.MarkerDeleteHandle(int handle) { ((INativeScintilla)mInnerScintilla).MarkerDeleteHandle(handle); }
		int INativeScintilla.MarkerGet(int line) { return ((INativeScintilla)mInnerScintilla).MarkerGet(line); }
		int INativeScintilla.MarkerLineFromHandle(int handle) { return ((INativeScintilla)mInnerScintilla).MarkerLineFromHandle(handle); }
		int INativeScintilla.MarkerNext(int lineStart, uint markerMask) { return ((INativeScintilla)mInnerScintilla).MarkerNext(lineStart, markerMask); }
		int INativeScintilla.MarkerPrevious(int lineStart, uint markerMask) { return ((INativeScintilla)mInnerScintilla).MarkerPrevious(lineStart, markerMask); }
		void INativeScintilla.MarkerSetAlpha(int markerNumber, int alpha) { ((INativeScintilla)mInnerScintilla).MarkerSetAlpha(markerNumber, alpha); }
		void INativeScintilla.MarkerSetBack(int markerNumber, int colour) { ((INativeScintilla)mInnerScintilla).MarkerSetBack(markerNumber, colour); }
		void INativeScintilla.MarkerSetFore(int markerNumber, int colour) { ((INativeScintilla)mInnerScintilla).MarkerSetFore(markerNumber, colour); }
		void INativeScintilla.MoveCaretInsideView() { ((INativeScintilla)mInnerScintilla).MoveCaretInsideView(); }
		void INativeScintilla.NewLine() { ((INativeScintilla)mInnerScintilla).NewLine(); }
		void INativeScintilla.Null() { ((INativeScintilla)mInnerScintilla).Null(); }
		void INativeScintilla.PageDown() { ((INativeScintilla)mInnerScintilla).PageDown(); }
		void INativeScintilla.PageDownExtend() { ((INativeScintilla)mInnerScintilla).PageDownExtend(); }
		void INativeScintilla.PageDownRectExtend() { ((INativeScintilla)mInnerScintilla).PageDownRectExtend(); }
		void INativeScintilla.PageUp() { ((INativeScintilla)mInnerScintilla).PageUp(); }
		void INativeScintilla.PageUpExtend() { ((INativeScintilla)mInnerScintilla).PageUpExtend(); }
		void INativeScintilla.PageUpRectExtend() { ((INativeScintilla)mInnerScintilla).PageUpRectExtend(); }
		void INativeScintilla.ParaDown() { ((INativeScintilla)mInnerScintilla).ParaDown(); }
		void INativeScintilla.ParaDownExtend() { ((INativeScintilla)mInnerScintilla).ParaDownExtend(); }
		void INativeScintilla.ParaUp() { ((INativeScintilla)mInnerScintilla).ParaUp(); }
		void INativeScintilla.ParaUpExtend() { ((INativeScintilla)mInnerScintilla).ParaUpExtend(); }
		int INativeScintilla.PointXFromPosition(int position) { return ((INativeScintilla)mInnerScintilla).PointXFromPosition(position); }
		int INativeScintilla.PointYFromPosition(int position) { return ((INativeScintilla)mInnerScintilla).PointYFromPosition(position); }
		int INativeScintilla.PositionAfter(int position) { return ((INativeScintilla)mInnerScintilla).PositionAfter(position); }
		int INativeScintilla.PositionBefore(int position) { return ((INativeScintilla)mInnerScintilla).PositionBefore(position); }
		int INativeScintilla.PositionFromLine(int line) { return ((INativeScintilla)mInnerScintilla).PositionFromLine(line); }
		int INativeScintilla.PositionFromPoint(int x, int y) { return ((INativeScintilla)mInnerScintilla).PositionFromPoint(x, y); }
		int INativeScintilla.PositionFromPointClose(int x, int y) { return ((INativeScintilla)mInnerScintilla).PositionFromPointClose(x, y); }
		void INativeScintilla.Redo() { ((INativeScintilla)mInnerScintilla).Redo(); }
		void INativeScintilla.RegisterImage(int type, string xpmData) { ((INativeScintilla)mInnerScintilla).RegisterImage(type, xpmData); }
		void INativeScintilla.ReleaseDocument(IntPtr pDoc) { ((INativeScintilla)mInnerScintilla).ReleaseDocument(pDoc); }
		void INativeScintilla.ReplaceSel(string text) { ((INativeScintilla)mInnerScintilla).ReplaceSel(text); }
		int INativeScintilla.ReplaceTarget(int length, string text) { return ((INativeScintilla)mInnerScintilla).ReplaceTarget(length, text); }
		int INativeScintilla.ReplaceTargetRE(int length, string text) { return ((INativeScintilla)mInnerScintilla).ReplaceTargetRE(length, text); }
		void INativeScintilla.SearchAnchor() { ((INativeScintilla)mInnerScintilla).SearchAnchor(); }
		int INativeScintilla.SearchInTarget(int length, string text) { return ((INativeScintilla)mInnerScintilla).SearchInTarget(length, text); }
		int INativeScintilla.SearchNext(int searchFlags, string text) { return ((INativeScintilla)mInnerScintilla).SearchNext(searchFlags, text); }
		int INativeScintilla.SearchPrev(int searchFlags, string text) { return ((INativeScintilla)mInnerScintilla).SearchPrev(searchFlags, text); }
		void INativeScintilla.SelectAll() { ((INativeScintilla)mInnerScintilla).SelectAll(); }
		void INativeScintilla.SelectionDuplicate() { ((INativeScintilla)mInnerScintilla).SelectionDuplicate(); }
		bool INativeScintilla.SelectionIsRectangle() { return ((INativeScintilla)mInnerScintilla).SelectionIsRectangle(); }
		IntPtr INativeScintilla.SendMessageDirect(uint msg, IntPtr wParam, IntPtr lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg); }
		int INativeScintilla.SendMessageDirect(uint msg, int wParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam); }
		int INativeScintilla.SendMessageDirect(uint msg, VOID wParam, int lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, VOID wParam, string lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, int lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, uint lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, bool wParam, int lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, bool lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, string wParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam); }
		int INativeScintilla.SendMessageDirect(uint msg, bool wParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam); }
		int INativeScintilla.SendMessageDirect(uint msg, string wParam, int lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, string lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, out string text) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, out text); }
		int INativeScintilla.SendMessageDirect(uint msg, out string text) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, out text); }
		int INativeScintilla.SendMessageDirect(uint msg, IntPtr wParam, out string lParam, int length) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, out lParam, length); }
		int INativeScintilla.SendMessageDirect(uint msg, string wParam, out string lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, out lParam); }
		int INativeScintilla.SendMessageDirect(uint msg, string wParam, string lParam) { return ((INativeScintilla)mInnerScintilla).SendMessageDirect(msg, wParam, lParam); }
		void INativeScintilla.SetAnchor(int position) { ((INativeScintilla)mInnerScintilla).SetAnchor(position); }
		void INativeScintilla.SetBackSpaceUnIndents(bool bsUnIndents) { ((INativeScintilla)mInnerScintilla).SetBackSpaceUnIndents(bsUnIndents); }
		void INativeScintilla.SetBufferedDraw(bool isBuffered) { ((INativeScintilla)mInnerScintilla).SetBufferedDraw(isBuffered); }
		void INativeScintilla.SetCaretFore(int alpha) { ((INativeScintilla)mInnerScintilla).SetCaretFore(alpha); }
		void INativeScintilla.SetCaretLineBack(int show) { ((INativeScintilla)mInnerScintilla).SetCaretLineBack(show); }
		void INativeScintilla.SetCaretLineBackAlpha(int alpha) { ((INativeScintilla)mInnerScintilla).SetCaretLineBackAlpha(alpha); }
		void INativeScintilla.SetCaretLineVisible(bool colour) { ((INativeScintilla)mInnerScintilla).SetCaretLineVisible(colour); }
		void INativeScintilla.SetCaretPeriod(int milliseconds) { ((INativeScintilla)mInnerScintilla).SetCaretPeriod(milliseconds); }
		void INativeScintilla.SetCaretSticky(bool useCaretStickyBehaviour) { ((INativeScintilla)mInnerScintilla).SetCaretSticky(useCaretStickyBehaviour); }
		void INativeScintilla.SetCaretStyle(int style) { ((INativeScintilla)mInnerScintilla).SetCaretStyle(style); }
		void INativeScintilla.SetCaretWidth(int pixels) { ((INativeScintilla)mInnerScintilla).SetCaretWidth(pixels); }
		void INativeScintilla.SetControlCharSymbol(int symbol) { ((INativeScintilla)mInnerScintilla).SetControlCharSymbol(symbol); }
		void INativeScintilla.SetCurrentPos(int position) { ((INativeScintilla)mInnerScintilla).SetCurrentPos(position); }
		void INativeScintilla.SetCursor(int curType) { ((INativeScintilla)mInnerScintilla).SetCursor(curType); }
		void INativeScintilla.SetDocPointer(IntPtr pDoc) { ((INativeScintilla)mInnerScintilla).SetDocPointer(pDoc); }
		void INativeScintilla.SetEdgeColour(int colour) { ((INativeScintilla)mInnerScintilla).SetEdgeColour(colour); }
		void INativeScintilla.SetEdgeColumn(int column) { ((INativeScintilla)mInnerScintilla).SetEdgeColumn(column); }
		void INativeScintilla.SetEdgeMode(int mode) { ((INativeScintilla)mInnerScintilla).SetEdgeMode(mode); }
		void INativeScintilla.SetEolMode(int eolMode) { ((INativeScintilla)mInnerScintilla).SetEolMode(eolMode); }
		void INativeScintilla.SetFocus(bool focus) { ((INativeScintilla)mInnerScintilla).SetFocus(focus); }
		void INativeScintilla.SetFoldExpanded(int line, bool expanded) { ((INativeScintilla)mInnerScintilla).SetFoldExpanded(line, expanded); }
		void INativeScintilla.SetFoldFlags(int flags) { ((INativeScintilla)mInnerScintilla).SetFoldFlags(flags); }
		void INativeScintilla.SetFoldLevel(int line, uint level) { ((INativeScintilla)mInnerScintilla).SetFoldLevel(line, level); }
		void INativeScintilla.SetFoldMarginColour(bool useSetting, int colour) { ((INativeScintilla)mInnerScintilla).SetFoldMarginColour(useSetting, colour); }
		void INativeScintilla.SetFoldMarginHiColour(bool useSetting, int colour) { ((INativeScintilla)mInnerScintilla).SetFoldMarginHiColour(useSetting, colour); }
		void INativeScintilla.SetHighlightGuide(int column) { ((INativeScintilla)mInnerScintilla).SetHighlightGuide(column); }
		void INativeScintilla.SetHotspotActiveBack(bool useHotspotBackColour, int colour) { ((INativeScintilla)mInnerScintilla).SetHotspotActiveBack(useHotspotBackColour, colour); }
		void INativeScintilla.SetHotspotActiveFore(bool useHotspotForeColour, int colour) { ((INativeScintilla)mInnerScintilla).SetHotspotActiveFore(useHotspotForeColour, colour); }
		void INativeScintilla.SetHotspotActiveUnderline(bool underline) { ((INativeScintilla)mInnerScintilla).SetHotspotActiveUnderline(underline); }
		void INativeScintilla.SetHotspotSingleLine(bool singleLine) { ((INativeScintilla)mInnerScintilla).SetHotspotSingleLine(singleLine); }
		void INativeScintilla.SetIndent(int widthInChars) { ((INativeScintilla)mInnerScintilla).SetIndent(widthInChars); }
		void INativeScintilla.SetIndentationGuides(bool view) { ((INativeScintilla)mInnerScintilla).SetIndentationGuides(view); }
		void INativeScintilla.SetIndicatorCurrent(int indicator) { ((INativeScintilla)mInnerScintilla).SetIndicatorCurrent(indicator); }
		void INativeScintilla.SetIndicatorValue(int value) { ((INativeScintilla)mInnerScintilla).SetIndicatorValue(value); }
		void INativeScintilla.SetKeywords(int keywordSet, string keyWordList) { ((INativeScintilla)mInnerScintilla).SetKeywords(keywordSet, keyWordList); }
		int INativeScintilla.SetLengthForEncode(int bytes) { return ((INativeScintilla)mInnerScintilla).SetLengthForEncode(bytes); }
		void INativeScintilla.SetLexer(int lexer) { ((INativeScintilla)mInnerScintilla).SetLexer(lexer); }
		void INativeScintilla.SetLexerLanguage(string name) { ((INativeScintilla)mInnerScintilla).SetLexerLanguage(name); }
		void INativeScintilla.SetLineIndentation(int line, int indentation) { ((INativeScintilla)mInnerScintilla).SetLineIndentation(line, indentation); }
		void INativeScintilla.SetLineState(int line, int value) { ((INativeScintilla)mInnerScintilla).SetLineState(line, value); }
		void INativeScintilla.SetMarginLeft(int pixels) { ((INativeScintilla)mInnerScintilla).SetMarginLeft(pixels); }
		void INativeScintilla.SetMarginMaskN(int margin, int mask) { ((INativeScintilla)mInnerScintilla).SetMarginMaskN(margin, mask); }
		void INativeScintilla.SetMarginRight(int pixels) { ((INativeScintilla)mInnerScintilla).SetMarginRight(pixels); }
		void INativeScintilla.SetMarginSensitiveN(int margin, bool sensitive) { ((INativeScintilla)mInnerScintilla).SetMarginSensitiveN(margin, sensitive); }
		void INativeScintilla.SetMarginTypeN(int margin, int type) { ((INativeScintilla)mInnerScintilla).SetMarginTypeN(margin, type); }
		void INativeScintilla.SetMarginWidthN(int margin, int pixelWidth) { ((INativeScintilla)mInnerScintilla).SetMarginWidthN(margin, pixelWidth); }
		void INativeScintilla.SetModEventMask(int modEventMask) { ((INativeScintilla)mInnerScintilla).SetModEventMask(modEventMask); }
		void INativeScintilla.SetMouseDownCaptures(bool captures) { ((INativeScintilla)mInnerScintilla).SetMouseDownCaptures(captures); }
		void INativeScintilla.SetMouseDwellTime(int mouseDwellTime) { ((INativeScintilla)mInnerScintilla).SetMouseDwellTime(mouseDwellTime); }
		void INativeScintilla.SetOvertype(bool overType) { ((INativeScintilla)mInnerScintilla).SetOvertype(overType); }
		void INativeScintilla.SetPrintColourMode(int mode) { ((INativeScintilla)mInnerScintilla).SetPrintColourMode(mode); }
		void INativeScintilla.SetPrintMagnification(int magnification) { ((INativeScintilla)mInnerScintilla).SetPrintMagnification(magnification); }
		void INativeScintilla.SetPrintWrapMode(int wrapMode) { ((INativeScintilla)mInnerScintilla).SetPrintWrapMode(wrapMode); }
		void INativeScintilla.SetProperty(string key, string value) { ((INativeScintilla)mInnerScintilla).SetProperty(key, value); }
		void INativeScintilla.SetReadOnly(bool readOnly) { ((INativeScintilla)mInnerScintilla).SetReadOnly(readOnly); }
		void INativeScintilla.SetSavePoint() { ((INativeScintilla)mInnerScintilla).SetSavePoint(); }
		void INativeScintilla.SetSearchFlags(int searchFlags) { ((INativeScintilla)mInnerScintilla).SetSearchFlags(searchFlags); }
		void INativeScintilla.SetSel(int anchorPos, int currentPos) { ((INativeScintilla)mInnerScintilla).SetSel(anchorPos, currentPos); }
		void INativeScintilla.SetSelBack(bool useSelectionBackColour, int colour) { ((INativeScintilla)mInnerScintilla).SetSelBack(useSelectionBackColour, colour); }
		void INativeScintilla.SetSelectionEnd(int position) { ((INativeScintilla)mInnerScintilla).SetSelectionEnd(position); }
		void INativeScintilla.SetSelectionMode(int mode) { ((INativeScintilla)mInnerScintilla).SetSelectionMode(mode); }
		void INativeScintilla.SetSelectionStart(int position) { ((INativeScintilla)mInnerScintilla).SetSelectionStart(position); }
		void INativeScintilla.SetSelFore(bool useSelectionForeColour, int colour) { ((INativeScintilla)mInnerScintilla).SetSelFore(useSelectionForeColour, colour); }
		void INativeScintilla.SetStatus(int status) { ((INativeScintilla)mInnerScintilla).SetStatus(status); }
		[Obsolete("The modern style indicators make this obsolete, this should always be 7")]
		void INativeScintilla.SetStyleBits(int bits) { ((INativeScintilla)mInnerScintilla).SetStyleBits(bits); }
		void INativeScintilla.SetStyling(int length, int style) { ((INativeScintilla)mInnerScintilla).SetStyling(length, style); }
		void INativeScintilla.SetStylingEx(int length, string styles) { ((INativeScintilla)mInnerScintilla).SetStylingEx(length, styles); }
		void INativeScintilla.SetTabIndents(bool tabIndents) { ((INativeScintilla)mInnerScintilla).SetTabIndents(tabIndents); }
		void INativeScintilla.SetTabWidth(int widthInChars) { ((INativeScintilla)mInnerScintilla).SetTabWidth(widthInChars); }
		void INativeScintilla.SetTargetEnd(int pos) { ((INativeScintilla)mInnerScintilla).SetTargetEnd(pos); }
		void INativeScintilla.SetTargetStart(int pos) { ((INativeScintilla)mInnerScintilla).SetTargetStart(pos); }
		void INativeScintilla.SetText(string text) { ((INativeScintilla)mInnerScintilla).SetText(text); }
		void INativeScintilla.SetTwoPhaseDraw(bool twoPhase) { ((INativeScintilla)mInnerScintilla).SetTwoPhaseDraw(twoPhase); }
		void INativeScintilla.SetUndoCollection(bool collectUndo) { ((INativeScintilla)mInnerScintilla).SetUndoCollection(collectUndo); }
		void INativeScintilla.SetUsePalette(bool allowPaletteUse) { ((INativeScintilla)mInnerScintilla).SetUsePalette(allowPaletteUse); }
		void INativeScintilla.SetUseTabs(bool useTabs) { ((INativeScintilla)mInnerScintilla).SetUseTabs(useTabs); }
		void INativeScintilla.SetViewEol(bool visible) { ((INativeScintilla)mInnerScintilla).SetViewEol(visible); }
		void INativeScintilla.SetViewWs(int wsMode) { ((INativeScintilla)mInnerScintilla).SetViewWs(wsMode); }
		void INativeScintilla.SetVisiblePolicy(int visiblePolicy, int visibleSlop) { ((INativeScintilla)mInnerScintilla).SetVisiblePolicy(visiblePolicy, visibleSlop); }
		void INativeScintilla.SetWhitespaceBack(bool useWhitespaceBackColour, int colour) { ((INativeScintilla)mInnerScintilla).SetWhitespaceBack(useWhitespaceBackColour, colour); }
		void INativeScintilla.SetWhitespaceFore(bool useWhitespaceForeColour, int colour) { ((INativeScintilla)mInnerScintilla).SetWhitespaceFore(useWhitespaceForeColour, colour); }
		void INativeScintilla.SetXCaretPolicy(int caretPolicy, int caretSlop) { ((INativeScintilla)mInnerScintilla).SetXCaretPolicy(caretPolicy, caretSlop); }
		void INativeScintilla.SetYCaretPolicy(int caretPolicy, int caretSlop) { ((INativeScintilla)mInnerScintilla).SetYCaretPolicy(caretPolicy, caretSlop); }
		void INativeScintilla.ShowLines(int lineStart, int lineEnd) { ((INativeScintilla)mInnerScintilla).ShowLines(lineStart, lineEnd); }
		void INativeScintilla.StartRecord() { ((INativeScintilla)mInnerScintilla).StartRecord(); }
		void INativeScintilla.StartStyling(int position, int mask) { ((INativeScintilla)mInnerScintilla).StartStyling(position, mask); }
		void INativeScintilla.StopRecord() { ((INativeScintilla)mInnerScintilla).StopRecord(); }
		void INativeScintilla.StutteredPageDown() { ((INativeScintilla)mInnerScintilla).StutteredPageDown(); }
		void INativeScintilla.StutteredPageDownExtend() { ((INativeScintilla)mInnerScintilla).StutteredPageDownExtend(); }
		void INativeScintilla.StutteredPageUp() { ((INativeScintilla)mInnerScintilla).StutteredPageUp(); }
		void INativeScintilla.StutteredPageUpExtend() { ((INativeScintilla)mInnerScintilla).StutteredPageUpExtend(); }
		void INativeScintilla.StyleClearAll() { ((INativeScintilla)mInnerScintilla).StyleClearAll(); }
		void INativeScintilla.StyleResetDefault() { ((INativeScintilla)mInnerScintilla).StyleResetDefault(); }
		int INativeScintilla.StyleGetBack(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetBack(styleNumber); }
		bool INativeScintilla.StyleGetBold(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetBold(styleNumber); }
		int INativeScintilla.StyleGetCase(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetCase(styleNumber); }
		bool INativeScintilla.StyleGetChangeable(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetChangeable(styleNumber); }
		int INativeScintilla.StyleGetCharacterSet(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetCharacterSet(styleNumber); }
		bool INativeScintilla.StyleGetEOLFilled(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetEOLFilled(styleNumber); }
		int INativeScintilla.StyleGetFore(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetFore(styleNumber); }
		bool INativeScintilla.StyleGetHotspot(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetHotspot(styleNumber); }
		bool INativeScintilla.StyleGetItalic(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetItalic(styleNumber); }
		int INativeScintilla.StyleGetSize(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetSize(styleNumber); }
		bool INativeScintilla.StyleGetUnderline(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetUnderline(styleNumber); }
		bool INativeScintilla.StyleGetVisible(int styleNumber) { return ((INativeScintilla)mInnerScintilla).StyleGetVisible(styleNumber); }
		void INativeScintilla.StyleSetBack(int styleNumber, int colour) { ((INativeScintilla)mInnerScintilla).StyleSetBack(styleNumber, colour); }
		void INativeScintilla.StyleSetBold(int styleNumber, bool bold) { ((INativeScintilla)mInnerScintilla).StyleSetBold(styleNumber, bold); }
		void INativeScintilla.StyleSetCase(int styleNumber, int caseMode) { ((INativeScintilla)mInnerScintilla).StyleSetCase(styleNumber, caseMode); }
		void INativeScintilla.StyleSetChangeable(int styleNumber, bool changeable) { ((INativeScintilla)mInnerScintilla).StyleSetChangeable(styleNumber, changeable); }
		void INativeScintilla.StyleSetCharacterSet(int styleNumber, int charSet) { ((INativeScintilla)mInnerScintilla).StyleSetCharacterSet(styleNumber, charSet); }
		void INativeScintilla.StyleSetEOLFilled(int styleNumber, bool eolFilled) { ((INativeScintilla)mInnerScintilla).StyleSetEOLFilled(styleNumber, eolFilled); }
		void INativeScintilla.StyleSetFore(int styleNumber, int colour) { ((INativeScintilla)mInnerScintilla).StyleSetFore(styleNumber, colour); }
		void INativeScintilla.StyleSetHotspot(int styleNumber, bool hotspot) { ((INativeScintilla)mInnerScintilla).StyleSetHotspot(styleNumber, hotspot); }
		void INativeScintilla.StyleSetItalic(int styleNumber, bool italic) { ((INativeScintilla)mInnerScintilla).StyleSetItalic(styleNumber, italic); }
		void INativeScintilla.StyleSetSize(int styleNumber, int sizeInPoints) { ((INativeScintilla)mInnerScintilla).StyleSetSize(styleNumber, sizeInPoints); }
		void INativeScintilla.StyleSetUnderline(int styleNumber, bool underline) { ((INativeScintilla)mInnerScintilla).StyleSetUnderline(styleNumber, underline); }
		void INativeScintilla.StyleSetVisible(int styleNumber, bool visible) { ((INativeScintilla)mInnerScintilla).StyleSetVisible(styleNumber, visible); }
		void INativeScintilla.Tab() { ((INativeScintilla)mInnerScintilla).Tab(); }
		int INativeScintilla.TargetAsUtf8(out string s) { return ((INativeScintilla)mInnerScintilla).TargetAsUtf8(out s); }
		void INativeScintilla.TargetFromSelection() { ((INativeScintilla)mInnerScintilla).TargetFromSelection(); }
		int INativeScintilla.TextHeight(int line) { return ((INativeScintilla)mInnerScintilla).TextHeight(line); }
		int INativeScintilla.TextWidth(int styleNumber, string text) { return ((INativeScintilla)mInnerScintilla).TextWidth(styleNumber, text); }
		void INativeScintilla.ToggleCaretSticky() { ((INativeScintilla)mInnerScintilla).ToggleCaretSticky(); }
		void INativeScintilla.ToggleFold(int line) { ((INativeScintilla)mInnerScintilla).ToggleFold(line); }
		void INativeScintilla.Undo() { ((INativeScintilla)mInnerScintilla).Undo(); }
		void INativeScintilla.UpperCase() { ((INativeScintilla)mInnerScintilla).UpperCase(); }
		void INativeScintilla.UsePopUp(bool bEnablePopup) { ((INativeScintilla)mInnerScintilla).UsePopUp(bEnablePopup); }
		void INativeScintilla.UserListShow(int listType, string list) { ((INativeScintilla)mInnerScintilla).UserListShow(listType, list); }
		void INativeScintilla.VCHome() { ((INativeScintilla)mInnerScintilla).VCHome(); }
		void INativeScintilla.VCHomeExtend() { ((INativeScintilla)mInnerScintilla).VCHomeExtend(); }
		void INativeScintilla.VCHomeRectExtend() { ((INativeScintilla)mInnerScintilla).VCHomeRectExtend(); }
		void INativeScintilla.VCHomeWrap() { ((INativeScintilla)mInnerScintilla).VCHomeWrap(); }
		void INativeScintilla.VCHomeWrapExtend() { ((INativeScintilla)mInnerScintilla).VCHomeWrapExtend(); }
		int INativeScintilla.VisibleFromDocLine(int docLine) { return ((INativeScintilla)mInnerScintilla).VisibleFromDocLine(docLine); }
		int INativeScintilla.WordEndPosition(int position, bool onlyWordCharacters) { return ((INativeScintilla)mInnerScintilla).WordEndPosition(position, onlyWordCharacters); }
		void INativeScintilla.WordLeft() { ((INativeScintilla)mInnerScintilla).WordLeft(); }
		void INativeScintilla.WordLeftEnd() { ((INativeScintilla)mInnerScintilla).WordLeftEnd(); }
		void INativeScintilla.WordLeftEndExtend() { ((INativeScintilla)mInnerScintilla).WordLeftEndExtend(); }
		void INativeScintilla.WordLeftExtend() { ((INativeScintilla)mInnerScintilla).WordLeftExtend(); }
		void INativeScintilla.WordPartLeft() { ((INativeScintilla)mInnerScintilla).WordPartLeft(); }
		void INativeScintilla.WordPartLeftExtend() { ((INativeScintilla)mInnerScintilla).WordPartLeftExtend(); }
		void INativeScintilla.WordPartRight() { ((INativeScintilla)mInnerScintilla).WordPartRight(); }
		void INativeScintilla.WordPartRightExtend() { ((INativeScintilla)mInnerScintilla).WordPartRightExtend(); }
		void INativeScintilla.WordRight() { ((INativeScintilla)mInnerScintilla).WordRight(); }
		void INativeScintilla.WordRightEnd() { ((INativeScintilla)mInnerScintilla).WordRightEnd(); }
		void INativeScintilla.WordRightEndExtend() { ((INativeScintilla)mInnerScintilla).WordRightEndExtend(); }
		void INativeScintilla.WordRightExtend() { ((INativeScintilla)mInnerScintilla).WordRightExtend(); }
		int INativeScintilla.WordStartPosition(int position, bool onlyWordCharacters) { return ((INativeScintilla)mInnerScintilla).WordStartPosition(position, onlyWordCharacters); }

		#endregion

		#region Events

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.AutoCSelection
		{
			add { ((INativeScintilla)mInnerScintilla).AutoCSelection += value; }
			remove { ((INativeScintilla)mInnerScintilla).AutoCSelection -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.CallTipClick
		{
			add { ((INativeScintilla)mInnerScintilla).CallTipClick += value; }
			remove { ((INativeScintilla)mInnerScintilla).CallTipClick -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.CharAdded
		{
			add { ((INativeScintilla)mInnerScintilla).CharAdded += value; }
			remove { ((INativeScintilla)mInnerScintilla).CharAdded -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.DoubleClick
		{
			add { ((INativeScintilla)mInnerScintilla).DoubleClick += value; }
			remove { ((INativeScintilla)mInnerScintilla).DoubleClick -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.DwellEnd
		{
			add { ((INativeScintilla)mInnerScintilla).DwellEnd += value; }
			remove { ((INativeScintilla)mInnerScintilla).DwellEnd -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.DwellStart
		{
			add { ((INativeScintilla)mInnerScintilla).DwellStart += value; }
			remove { ((INativeScintilla)mInnerScintilla).DwellStart -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.IndicatorClick
		{
			add { ((INativeScintilla)mInnerScintilla).IndicatorClick += value; }
			remove { ((INativeScintilla)mInnerScintilla).IndicatorClick -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.IndicatorRelease
		{
			add { ((INativeScintilla)mInnerScintilla).IndicatorRelease += value; }
			remove { ((INativeScintilla)mInnerScintilla).IndicatorRelease -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.Key
		{
			add { ((INativeScintilla)mInnerScintilla).Key += value; }
			remove { ((INativeScintilla)mInnerScintilla).Key -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.MacroRecord
		{
			add { ((INativeScintilla)mInnerScintilla).MacroRecord += value; }
			remove { ((INativeScintilla)mInnerScintilla).MacroRecord -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.MarginClick
		{
			add { ((INativeScintilla)mInnerScintilla).MarginClick += value; }
			remove { ((INativeScintilla)mInnerScintilla).MarginClick -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.Modified
		{
			add { ((INativeScintilla)mInnerScintilla).Modified += value; }
			remove { ((INativeScintilla)mInnerScintilla).Modified -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.ModifyAttemptRO
		{
			add { ((INativeScintilla)mInnerScintilla).ModifyAttemptRO += value; }
			remove { ((INativeScintilla)mInnerScintilla).ModifyAttemptRO -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.NeedShown
		{
			add { ((INativeScintilla)mInnerScintilla).NeedShown += value; }
			remove { ((INativeScintilla)mInnerScintilla).NeedShown -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.Painted
		{
			add { ((INativeScintilla)mInnerScintilla).Painted += value; }
			remove { ((INativeScintilla)mInnerScintilla).Painted -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.SavePointLeft
		{
			add { ((INativeScintilla)mInnerScintilla).SavePointLeft += value; }
			remove { ((INativeScintilla)mInnerScintilla).SavePointLeft -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.SavePointReached
		{
			add { ((INativeScintilla)mInnerScintilla).SavePointReached += value; }
			remove { ((INativeScintilla)mInnerScintilla).SavePointReached -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.StyleNeeded
		{
			add { ((INativeScintilla)mInnerScintilla).StyleNeeded += value; }
			remove { ((INativeScintilla)mInnerScintilla).StyleNeeded -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.UpdateUI
		{
			add { ((INativeScintilla)mInnerScintilla).UpdateUI += value; }
			remove { ((INativeScintilla)mInnerScintilla).UpdateUI -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.UriDropped
		{
			add { ((INativeScintilla)mInnerScintilla).UriDropped += value; }
			remove { ((INativeScintilla)mInnerScintilla).UriDropped -= value; }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.UserListSelection
		{
			add { ((INativeScintilla)mInnerScintilla).UserListSelection += value; }
			remove { ((INativeScintilla)mInnerScintilla).UserListSelection -= value; }
		}

		#endregion

#pragma warning restore 618

		#endregion

	}
}
