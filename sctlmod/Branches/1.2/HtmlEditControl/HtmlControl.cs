// ------------------------------------------------------------
// Writer, WYSIWYG editor for HTML
// Copyright (c) 2002-2003 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder
// ------------------------------------------------------------
// Based on HTML editor control code
// Copyright (c) 2002-2003 Nikhil Kothari. All rights reserved.
// http://www.nikhilk.net
// ------------------------------------------------------------
namespace HTMLEditor
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
	using System.Runtime.InteropServices;
	using System.Security;
	using System.Security.Permissions;
	using System.Text;
    using System.Windows;
    using System.Windows.Forms;

    public class HtmlControl : Control
    {
		private static readonly object readyStateCompleteEvent = new object();

		private bool scriptEnabled;

        private bool firstActivation;
        private bool isReady;
        private bool isCreated;

        // These allow a user to load the document before displaying
        private bool desiredLoad;
        private string desiredContent;
        private string desiredUrl;

		private bool absolutePositioningEnabled;
		private bool absolutePositioningDesired;

		private bool multipleSelectionEnabled;
		private bool multipleSelectionDesired;

        private bool focusDesired;

        private string url;
        private object scriptObject;

        private HtmlSite site;

        private static IDictionary urlMap;

		private bool isDesignMode;
		private bool designModeDesired;

		private HtmlSelection selection;
		private HtmlTextFormatting textFormatting;

		private NativeMethods.IPersistStreamInit persistStream;
		private NativeMethods.IOleUndoManager undoManager;
		private NativeMethods.IHTMLEditServices editServices;

        public HtmlControl()
        {
            this.firstActivation = true;
			this.TabStop = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.url != null)
				{
					UrlMap[this.url] = null;
				}
			}

			base.Dispose(disposing);
		}

		public event EventHandler ReadyStateComplete
		{
			add { this.Events.AddHandler(readyStateCompleteEvent, value); }
			remove { this.Events.RemoveHandler(readyStateCompleteEvent, value); }
		}

		protected bool IsCreated 
        {
            get { return this.isCreated; }
        }

        public bool IsReady 
        {
            get { return this.isReady; }
        }

		internal NativeMethods.IHTMLDocument2 HtmlDocument
		{
			get { return this.site.Document; }
		}

        internal NativeMethods.IOleCommandTarget CommandTarget 
        {
            get { return this.site.CommandTarget; }
        }

        public bool ScriptEnabled 
        {
            get { return this.scriptEnabled; }
            set { this.scriptEnabled = value; }
        }

        public object ScriptObject 
        {
            get { return this.scriptObject; }
            set { this.scriptObject = value; }
        }

        public virtual string Url 
        {
            get { return this.url; }
        }

        internal static IDictionary UrlMap 
        {
            get 
            {
                if (urlMap == null) 
                {
                    urlMap = new HybridDictionary(true);
                }

				return urlMap;
            }
        }

        /// <summary>Executes the specified command in MSHTML.</summary>
        /// <param name="command"></param>
        internal object Execute(int command) 
        {
            return this.Execute(command, null);
        }

        internal object Execute(int command, object[] arguments) 
        {
			object[] retVal = new object[] { null };
            int hr = CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, command, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, arguments, retVal);
            if (hr != NativeMethods.S_OK) 
            {
                throw new Exception("Execution of MSHTML command ID \'" + command + "\' failed.");
            }
			return retVal[0];
        }

		internal object ExecuteWithUserInterface(int command, object[] arguments)
		{
			object[] retVal = new object[] { null };
			int hr = CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, command, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, arguments, retVal);
			if (hr != NativeMethods.S_OK)
			{
				throw new Exception("Execution of MSHTML command ID \'" + command + "\' failed.");
			}
			return retVal[0];
		}

		internal bool IsEnabled(int commandId)
		{
			return ((this.GetCommandInfo(commandId) & 1) != 0);
		}

		internal bool IsChecked(int commandId)
		{
			return ((this.GetCommandInfo(commandId) & 2) != 0);
		}

		internal int GetCommandInfo(int commandId)
		{
			NativeMethods.tagOLECMD command = new NativeMethods.tagOLECMD();
			command.cmdID = commandId;
			this.CommandTarget.QueryStatus(ref NativeMethods.Guid_MSHTML, 1, command, 0);
			return (command.cmdf >> 1);
		}

        public HtmlElement GetElementByID(string id) 
        {
            NativeMethods.IHTMLElement body = this.site.Document.GetBody();
            NativeMethods.IHTMLElementCollection children = (NativeMethods.IHTMLElementCollection)body.GetAll();
            NativeMethods.IHTMLElement element = (NativeMethods.IHTMLElement)children.Item(id, 0);

            if (element == null) 
            {
                return null;
            }

            return new HtmlElement(element, this);
        }

        /// <summary>Loads HTML content from a stream into this control.</summary>
        /// <param name="stream"></param>
        public void LoadHtml(Stream stream) 
        {
            if (stream == null) 
            {
                throw new ArgumentNullException("LoadHtml : You must specify a non-null stream for content");
            }

            StreamReader reader = new StreamReader(stream);
            this.LoadHtml(reader.ReadToEnd());
        }

        /// <summary>Loads HTML content from a string into this control.</summary>
        /// <param name="content"></param>
        public void LoadHtml(string content) 
        {
            this.LoadHtml(content, null);
        }

		/*
        public void LoadHtml(string content, string url) 
        {
            this.LoadHtml(content, url, null);
        }
        */

        // REVIEW: Add a load method for stream and url

        /// <summary>
        /// Loads HTML content from a string into this control identified by the specified URL.
        /// If MSHTML has not yet been created, the loading is postponed until MSHTML has been created.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="url"></param>
        public void LoadHtml(string content, string url) 
        {
            if (content == null) 
            {
                content = "";
            }

            if (!this.isCreated) 
            {
                this.desiredContent = content;
                this.desiredUrl = url;
                this.desiredLoad = true;
                return;
            }

            NativeMethods.IStream stream = null;

            //First we create a COM stream
            IntPtr hglobal = Marshal.StringToHGlobalUni(content);
            NativeMethods.CreateStreamOnHGlobal(hglobal, true, out stream);

            // Initialize a new document if there is nothing to load
            if (stream == null) 
            {
                NativeMethods.IPersistStreamInit psi = (NativeMethods.IPersistStreamInit) this.site.Document;
                Debug.Assert(psi != null, "Expected IPersistStreamInit");
                psi.InitNew();
                psi = null;
            }
            else 
            {
                NativeMethods.IHTMLDocument2 document = this.site.Document;

                if (url == null) 
                {
					// If there is no specified URL load the document from the stream.
					NativeMethods.IPersistStreamInit psi = (NativeMethods.IPersistStreamInit)document;
                    Debug.Assert(psi != null, "Expected IPersistStreamInit");
                    psi.Load(stream);
                    psi = null;
                }
                else 
                {
                    // Otherwise we create a moniker and load the stream to that moniker.
					NativeMethods.IPersistMoniker persistMoniker = (NativeMethods.IPersistMoniker) document;

					NativeMethods.IMoniker moniker = null;
					NativeMethods.CreateURLMoniker(null, url, out moniker);

					NativeMethods.IBindCtx bindContext = null;
					NativeMethods.CreateBindCtx(0, out bindContext);

					persistMoniker.Load(1, moniker, bindContext, 0);

					persistMoniker = null;
					moniker = null;
					bindContext = null;
                }
            }

            this.url = url;
        }

        /// <summary>
        /// Allow editors to perform actions when the MSHTML document is created
        /// and before it's activated
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCreated(EventArgs e) 
        {
			if (e == null)
			{
				throw new ArgumentNullException("You must specify a non-null EventArgs for OnCreated");
			}

			object[] mshtmlArgs = new object[1];

			mshtmlArgs[0] = true;
			CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, NativeMethods.IDM_PERSISTDEFAULTVALUES, 0, mshtmlArgs, null);

			mshtmlArgs[0] = true;
			CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, NativeMethods.IDM_PROTECTMETATAGS, 0, mshtmlArgs, null);

			mshtmlArgs[0] = true;
			CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, NativeMethods.IDM_PRESERVEUNDOALWAYS, 0, mshtmlArgs, null);

			mshtmlArgs[0] = true;
			CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, NativeMethods.IDM_NOACTIVATENORMALOLECONTROLS, 0, mshtmlArgs, null);

			mshtmlArgs[0] = true;
			CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, NativeMethods.IDM_NOACTIVATEDESIGNTIMECONTROLS, 0, mshtmlArgs, null);

			mshtmlArgs[0] = true;
			CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, NativeMethods.IDM_NOACTIVATEJAVAAPPLETS, 0, mshtmlArgs, null);

			mshtmlArgs[0] = true;
			CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, NativeMethods.IDM_NOFIXUPURLSONPASTE, 0, mshtmlArgs, null);

			// Set the design mode to the last desired design mode
			if (this.designModeDesired)
			{
				this.IsDesignMode = this.designModeDesired;
				this.designModeDesired = false;
			}
        }

        protected override void OnGotFocus(EventArgs e) 
        {
            base.OnGotFocus(e);

            if (this.IsReady) 
            {
                if (this.site != null) this.site.SetFocus();
            }
            else 
            {
				this.focusDesired = true;
            }
        }

        /// <summary>We can only activate the MSHTML after our handle has been created, so upon creating the handle, we create and activate NativeMethods. If LoadHtml was called prior to this, we do the loading now.</summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.firstActivation)
            {
                this.site = new HtmlSite(this);
                this.site.CreateHtml();

                this.isCreated = true;

                this.OnCreated(EventArgs.Empty);

                this.site.ActivateHtml();
                this.firstActivation = false;

                if (this.desiredLoad)
                {
                    this.LoadHtml(this.desiredContent, this.desiredUrl);
                    this.desiredLoad = false;
                }
            }
        }

		protected override void OnHandleDestroyed(EventArgs e)
		{
			this.site.DeactivateHtml();
			this.site.CloseHtml();
			this.site = null;

			base.OnHandleDestroyed(e);
		}

        /// <summary>Called when the control has just become ready.</summary>
        /// <param name="e"></param>
        protected internal virtual void OnReadyStateComplete(EventArgs e) 
        {
            this.isReady = true;

            EventHandler handler = (EventHandler)this.Events[readyStateCompleteEvent];
            if (handler != null) 
            {
                handler(this, e);
            }

            if (this.focusDesired) 
            {
                this.focusDesired = false;
                this.site.ActivateHtml();
                this.site.SetFocus();
            }

			// HtmlEditor

			this.persistStream = (NativeMethods.IPersistStreamInit)this.HtmlDocument;

			this.Selection.SynchronizeSelection();

			// Set the mutiple selection mode to the last desired multiple selection mode
			if (this.multipleSelectionDesired)
			{
				this.MultipleSelectionEnabled = this.multipleSelectionDesired;
			}

			// Set the absolute positioning mode to the last desired absolute position mode
			if (this.absolutePositioningDesired)
			{
				this.AbsolutePositioningEnabled = this.absolutePositioningDesired;
			}
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)] 
        public override bool PreProcessMessage(ref Message m) 
        {
            bool handled = false;
            if ((m.Msg >= NativeMethods.WM_KEYFIRST) && (m.Msg <= NativeMethods.WM_KEYLAST)) 
            {
                // If it's a key down, first see if the key combo is a command key
                if (m.Msg == NativeMethods.WM_KEYDOWN) 
                {
                    handled = ProcessCmdKey(ref m, (Keys)(int) m.WParam | ModifierKeys);
                }

                if (!handled) 
                {
                    int keyCode = (int)m.WParam;

                    // Don't let Trident eat Ctrl-PgUp/PgDn
					if (((keyCode != (int) Keys.PageUp) && (keyCode != (int) Keys.PageDown)) || ((ModifierKeys & Keys.Control) == 0)) 
                    {
                        NativeMethods.COMMSG cm = new NativeMethods.COMMSG();
                        cm.hwnd = m.HWnd;
                        cm.message = m.Msg;
                        cm.wParam = m.WParam;
                        cm.lParam = m.LParam;

						handled = this.site.TranslateAccelarator(cm);
                    }
                    else 
                    {
                        // WndProc for Ctrl-PgUp/PgDn is never called so call it directly here
                        this.WndProc(ref m);
                        handled = true;
                    }
                }
            }

            if (!handled) 
            {
                handled = base.PreProcessMessage(ref m);
            }

            return handled;
        }

        /// <summary>Saves the HTML contained in control to a string and return it.</summary>
        /// <returns>string - The HTML in the control</returns>
        public string SaveHtml() 
        {
            if (!this.IsCreated) 
            {
                throw new Exception("HtmlControl.SaveHtml : No HTML to save!");
            }

            string content = String.Empty;

            try 
            {
                NativeMethods.IHTMLDocument2 document = this.site.Document;

                // First save the document to a stream
                NativeMethods.IPersistStreamInit psi = (NativeMethods.IPersistStreamInit)document;
                Debug.Assert(psi != null, "Expected IPersistStreamInit");

                NativeMethods.IStream stream = null;
                NativeMethods.CreateStreamOnHGlobal(NativeMethods.NullIntPtr, true, out stream);

                psi.Save(stream, 1);

                // Now copy the stream to the string
                NativeMethods.STATSTG stat = new NativeMethods.STATSTG();
                stream.Stat(stat, 1);
                int length = (int)stat.cbSize;
                byte[] bytes = new byte[length];

                IntPtr hglobal;
                NativeMethods.GetHGlobalFromStream(stream, out hglobal);
                Debug.Assert(hglobal != NativeMethods.NullIntPtr, "Failed in GetHGlobalFromStream");

                // First copy the stream to a byte array
                IntPtr pointer = NativeMethods.GlobalLock(hglobal);
                if (pointer != NativeMethods.NullIntPtr) 
                {
                    Marshal.Copy(pointer, bytes, 0, length);

                    NativeMethods.GlobalUnlock(hglobal);

                    // Then create the string from the byte array (use a StreamReader to eat the preamble in the UTF8 encoding case)
                    StreamReader streamReader = null;
                    try 
                    {
                        streamReader = new StreamReader(new MemoryStream(bytes), Encoding.Default);
                        content = streamReader.ReadToEnd();
                    }
                    finally 
                    {
                        if (streamReader != null) 
                        {
                            streamReader.Close();
                        }
                    }
                }
            }
            catch (Exception exception) 
            {
                Debug.Fail("HtmlControl.SaveHtml" + exception.ToString());
                content = String.Empty;
            }
            finally 
            {
            }

            if (content == null) 
            {
                content = String.Empty;
            }
            
            return content;

			/*
            HtmlFormatter formatter = new HtmlFormatter();
            StringWriter writer = new StringWriter();
            formatter.Format(content, writer);
            return writer.ToString();
			*/
        }

        /// <summary>Saves the HTML contained in the control to a stream.</summary>
        /// <param name="stream"></param>
        public void SaveHtml(Stream stream) 
        {
            if (stream == null) 
            {
                throw new ArgumentNullException("SaveHtml : Must specify a non-null stream to which to save");
            }

            string content = SaveHtml();

            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(content);
            writer.Flush();
		}

		public bool CanPrint
		{
			get { return this.IsEnabled(NativeMethods.IDM_PRINT); }	
		}
	
		public void Print()
		{
			this.ExecuteWithUserInterface(NativeMethods.IDM_PRINT, null);
		}

		public bool CanPrintPreview
		{
			get { return this.IsEnabled(NativeMethods.IDM_PRINTPREVIEW); }	
		}
		
		public void PrintPreview()
		{
			this.ExecuteWithUserInterface(NativeMethods.IDM_PRINTPREVIEW, null);
		}

		public bool CanCopy
		{
			get { return this.IsEnabled(NativeMethods.IDM_COPY); }
		}

		public void Copy()
		{
			if (!this.CanCopy)
			{
				throw new InvalidOperationException();
			}

			this.Execute(NativeMethods.IDM_COPY);
		}

		public bool CanCut
		{
			get { return this.IsEnabled(NativeMethods.IDM_CUT); }
		}

		public void Cut()
		{
			if (!CanCut)
			{
				throw new InvalidOperationException();
			}

			this.Execute(NativeMethods.IDM_CUT);
		}

		public bool CanPaste
		{
			get { return this.IsEnabled(NativeMethods.IDM_PASTE); }
		}

		public void Paste()
		{
			if (!this.CanPaste)
			{
				throw new InvalidOperationException();
			}

			this.Execute(NativeMethods.IDM_PASTE);
		}

		public bool CanDelete
		{
			get { return this.IsEnabled(NativeMethods.IDM_DELETE); }
		}

		public void Delete()
		{
			if (!this.CanDelete)
			{
				throw new InvalidOperationException();
			}

			this.Execute(NativeMethods.IDM_DELETE);
		}

		public bool CanRedo
		{
			get { return this.IsEnabled(NativeMethods.IDM_REDO); }
		}

		public void Redo()
		{
			if (!this.CanRedo)
			{
				throw new InvalidOperationException();
			}

			this.Execute(NativeMethods.IDM_REDO);
		}

		public string RedoDescription
		{
			get { return (this.CanRedo) ? this.UndoManager.GetLastRedoDescription() : string.Empty; }
		}

		public bool CanUndo
		{
			get { return this.IsEnabled(NativeMethods.IDM_UNDO); }
		}

        public void Undo() 
        {
            if (!this.CanUndo) 
            {
                throw new InvalidOperationException();
            }

            this.Execute(NativeMethods.IDM_UNDO);
        }

		public string UndoDescription
		{
			get { return (this.CanUndo) ? this.UndoManager.GetLastUndoDescription() : string.Empty; }
		}

		public bool CanSelectAll
		{
			get { return this.IsEnabled(NativeMethods.IDM_SELECTALL); }
		}

		public void SelectAll()
		{
			this.Execute(NativeMethods.IDM_SELECTALL);
		}

		/// <summary>Enables or disables absolute position for the entire editor.</summary>
		public bool AbsolutePositioningEnabled
		{
			get { return this.absolutePositioningEnabled; }

			set
			{
				// If the control isn't ready to be put into abs pos mode,
				// set a flag and put it in abs pos mode when it is ready
				this.absolutePositioningDesired = value;
				if (!this.IsCreated)
				{
					return;
				}
				else
				{
					//Turn abs pos mode on or off depending on the new value
					this.absolutePositioningEnabled = value;
					object[] args = new object[] { this.absolutePositioningEnabled };
					this.Execute(NativeMethods.IDM_2D_POSITION, args);
				}
			}
		}

		public bool IsDesignMode
		{
			get { return this.isDesignMode; }

			set
			{
				// Only execute this if we aren't already in design mode
				if (this.isDesignMode != value)
				{
					// If the control isn't ready to be put into design mode,
					// set a flag and put it in design mode when it is ready
					if (!IsCreated)
					{
						this.designModeDesired = value;
					}
					else
					{
						//Turn design mode on or off depending on the new value
						this.isDesignMode = value;
						this.HtmlDocument.SetDesignMode((this.isDesignMode ? "on" : "off"));
					}
				}
			}
		}

		private NativeMethods.IHTMLEditServices MSHTMLEditServices
		{
			get
			{
				if (this.editServices == null)
				{
					NativeMethods.IServiceProvider serviceProvider = this.HtmlDocument as NativeMethods.IServiceProvider;
					Debug.Assert(serviceProvider != null);
					Guid shtmlGuid = new Guid(0x3050f7f9,0x98b5,0x11cf,0xbb,0x82,0x00,0xaa,0x00,0xbd,0xce,0x0b);
					Guid intGuid = (typeof(NativeMethods.IHTMLEditServices)).GUID;

					IntPtr editServicePtr = NativeMethods.NullIntPtr;
					int hr = serviceProvider.QueryService(ref shtmlGuid, ref intGuid, out editServicePtr);
					Debug.Assert((hr == NativeMethods.S_OK) && (editServicePtr != NativeMethods.NullIntPtr), "Did not get IHTMLEditService");
					if ((hr == NativeMethods.S_OK) && (editServicePtr != NativeMethods.NullIntPtr))
					{
						this.editServices = (NativeMethods.IHTMLEditServices)Marshal.GetObjectForIUnknown(editServicePtr);
						Marshal.Release(editServicePtr);
					}
				}

				return this.editServices;
			}
		}

		public virtual bool IsDirty
		{
			get
			{
				if (this.IsDesignMode && this.IsReady && (this.persistStream != null))
				{
					// TODO After a load, this is no longer true, what do we do???
					if (this.persistStream.IsDirty() == NativeMethods.S_OK)
					{
						return true;
					}
				}

				return false;
			}

			set
			{
				if (this.IsReady)
				{
					this.Execute(NativeMethods.IDM_SETDIRTY, new object[] { value });
				}
			}
		}

		/// <summary>Indicates if multiple selection is enabled in the editor. Also places MSHTML into multiple selection mode if set to true.</summary>
		public bool MultipleSelectionEnabled
		{
			get { return this.multipleSelectionEnabled; }

			set
			{
				// If the control isn't ready yet, postpone setting multiple selection
				this.multipleSelectionDesired = value;
				if (!IsReady)
				{
					return;
				}
				else
				{
					// Create an objects array to pass parameters to the MSHTML command target
					this.multipleSelectionEnabled = value;
					object[] args = new object[] { this.multipleSelectionEnabled };
					int hr = CommandTarget.Exec(ref NativeMethods.Guid_MSHTML, NativeMethods.IDM_MULTIPLESELECTION, 0, args, null);
					Debug.Assert(hr == NativeMethods.S_OK);
				}
			}
		}

		private NativeMethods.IOleUndoManager UndoManager
		{
			get
			{
				if (this.undoManager == null)
				{
					NativeMethods.IServiceProvider serviceProvider = this.HtmlDocument as NativeMethods.IServiceProvider;
					Debug.Assert(serviceProvider != null);
					Guid undoManagerGuid = typeof(NativeMethods.IOleUndoManager).GUID;
					Guid undoManagerGuid2 = typeof(NativeMethods.IOleUndoManager).GUID;
					IntPtr undoManagerPtr = NativeMethods.NullIntPtr;
					int hr = serviceProvider.QueryService(ref undoManagerGuid2, ref undoManagerGuid, out undoManagerPtr);
					if ((hr == NativeMethods.S_OK) && (undoManagerPtr != NativeMethods.NullIntPtr))
					{
						this.undoManager = (NativeMethods.IOleUndoManager)Marshal.GetObjectForIUnknown(undoManagerPtr);
						Marshal.Release(undoManagerPtr);
					}
				}
				return this.undoManager;
			}
		}

		/// <summary>Finds the specified string in the content of the control and selects it if it exists.</summary>
		/// <param name="searchString">The string to find</param>
		/// <param name="matchCase">Set to true to match casing of the string.</param>
		/// <param name="wholeWord">Set to true to only find whole words.</param>
		/// <returns></returns>
		public bool Find(string searchString, bool matchCase, bool wholeWord, bool searchUp)
		{
			NativeMethods.IHTMLSelectionObject selectionObj = this.site.Document.GetSelection();

			// Check if a selection actually exists.
			bool selectionExists = false;
			if (selectionObj != null)
			{
				selectionExists = selectionObj.GetSelectionType().Equals("Text");
			}
			NativeMethods.IHTMLTxtRange textRange = null;
			if (selectionExists)
			{
				object o = selectionObj.CreateRange();
				textRange = o as NativeMethods.IHTMLTxtRange;
			}
			if (textRange == null)
			{
				// If no selection exists, select the entire body.
				NativeMethods.IHtmlBodyElement bodyElement = this.site.Document.GetBody() as NativeMethods.IHtmlBodyElement;
				Debug.Assert(bodyElement != null, "Couldn't get body element in HtmlControl.Find");
				selectionExists = false;
				textRange = bodyElement.createTextRange();
			}

			// Set up the bounds of the search.
			if (searchUp)
			{
				// If we're search up in the document.
				if (selectionExists)
				{
					// If a selection exists, move the range's end to one character before the selection.
					textRange.MoveEnd("character", -1);
				}

				// Move the range's beginning to the start of the document.
				int temp = 1;
				while (temp == 1)
				{
					temp = textRange.MoveStart("textedit", -1);
				}
			}
			else
			{
				// If we're searching down in the document.
				if (selectionExists)
				{
					// If a selection exists, start one char after the selection.
					textRange.MoveStart("character", 1);
				}

				// Move the range's end to the end of the document.
				int temp = 1;
				while (temp == 1)
				{
					temp = textRange.MoveEnd("textedit", 1);
				}
			}

			// Set up the flags for matching case and whole word search
			int flags = (matchCase ? 0x4 : 0) | (wholeWord ? 0x2 : 0);
			int direction = searchUp ? -10000000 : 10000000;

			// Do the search.
			bool success = textRange.FindText(searchString, direction, flags);

			if (success)
			{
				// If we succeeded, select the text, scroll it into view, and we're done!
				textRange.Select();
				textRange.ScrollIntoView(true);
				return true;
			}
			else if (selectionExists)
			{
				// If we only searched a portion of the document we need to wrap around the document...
				textRange = selectionObj.CreateRange() as NativeMethods.IHTMLTxtRange;

				// Set up the bounds of the search.
				if (searchUp)
				{
					// If we're searching up in the document. Start one char after the selection.
					textRange.MoveStart("character", 1);

					// Move the range's end to the end of the document.
					int temp = 1;
					while (temp == 1)
					{
						temp = textRange.MoveEnd("textedit", 1);
					}
				}
				else
				{
					// If we're searching down in the document. Move the range's end to one character before the selection.
					textRange.MoveEnd("character", -1);

					// Move the range's beginning to the start of the document.
					int temp = 1;
					while (temp == 1)
					{
						temp = textRange.MoveStart("textedit", -1);
					}
				}

				success = textRange.FindText(searchString, direction, flags);
				if (success)
				{
					// If we succeeded, select the text, scroll it into view, and we're done!
					textRange.Select();
					textRange.ScrollIntoView(true);
					return true;
				}
			}
			return false;
		}

		public bool Replace(string searchString, string replaceString, bool matchCase, bool wholeWord, bool searchUp)
		{
			// Synchronize first so the selection length is correct
			this.Selection.SynchronizeSelection();

			// Only perform a replace if something is already selected
			if ((this.Selection.Type == HtmlSelectionType.TextSelection) && (this.Selection.Length > 0))
			{
				NativeMethods.IHTMLTxtRange selection = this.Selection.Selection as NativeMethods.IHTMLTxtRange;

				// Set up the flags for matching case and whole word search
				int flags = (matchCase ? 0x4 : 0) | (wholeWord ? 0x2 : 0);
				int direction = searchUp ? -10000000 : 10000000;
				// Ensure that the selected text conatins the search string
				if (selection.FindText(searchString, direction, flags))
				{
					// If it does, replace the text
					selection.SetText(replaceString);
				}
			}

			// Find the next instance of the string
			return this.Find(searchString, matchCase, wholeWord, searchUp);
		}

		public bool CanInsertHyperlink
		{
			get
			{
				if (((this.Selection.Type == HtmlSelectionType.TextSelection) || (this.Selection.Type == HtmlSelectionType.Empty)) && (this.Selection.Length == 0))
				{
					return this.CanInsertHtml;
				}
				else
				{
					return this.IsEnabled(NativeMethods.IDM_HYPERLINK);
				}
			}
		}

		public void InsertHyperlink(string url, string description)
		{
			Selection.SynchronizeSelection();
			if (url == null) {
				try {
					this.Execute(NativeMethods.IDM_HYPERLINK);
				}
				catch {}
			}
			else {
				if (((this.Selection.Type == HtmlSelectionType.TextSelection) || (this.Selection.Type == HtmlSelectionType.Empty)) && (this.Selection.Length == 0)) {
					InsertHtml("<a href=\""+url+"\">"+description+"</a>");
				}
				else {
					this.ExecuteWithUserInterface(NativeMethods.IDM_HYPERLINK, new object[] { url });
				}
			}
		}

		public bool CanInsertHtml
		{
			get
			{
				if (this.Selection.Type == HtmlSelectionType.ElementSelection)
				{
					//If this is a control range, we can only insert HTML if we're in a div or span
					NativeMethods.IHtmlControlRange controlRange = (NativeMethods.IHtmlControlRange)this.Selection.Selection;
					int selectedItemCount = controlRange.GetLength();
					if (selectedItemCount == 1)
					{
						NativeMethods.IHTMLElement element = controlRange.Item(0);
						if ((String.Compare(element.GetTagName(), "div", true) == 0) || (String.Compare(element.GetTagName(), "td", true) == 0))
						{
							return true;
						}
					}
				}
				else
				{
					// If this is a text range, we can definitely insert HTML.
					return true;
				}
				return false;
			}
		}

		public void InsertImage(string url)
		{
			Selection.SynchronizeSelection();

			if (((this.Selection.Type == HtmlSelectionType.TextSelection) || (this.Selection.Type == HtmlSelectionType.Empty)) && (this.Selection.Length == 0))
			{
				this.InsertHtml("<img src=\"" + url + "\"/>");
			}
			else
			{
				this.Execute(NativeMethods.IDM_IMAGE, new object[] { url });
			}
		}

		public void InsertImage() {
			this.ExecuteWithUserInterface(NativeMethods.IDM_IMAGE, null);
		}

		public void InsertHtml(string html)
		{
			this.Selection.SynchronizeSelection();
			if (this.Selection.Type == HtmlSelectionType.ElementSelection)
			{
				//If it's a control range, we can only insert if we are in a div or td
				NativeMethods.IHtmlControlRange controlRange = (NativeMethods.IHtmlControlRange)Selection.Selection;
				int selectedItemCount = controlRange.GetLength();
				if (selectedItemCount == 1)
				{
					NativeMethods.IHTMLElement element = controlRange.Item(0);
					if ((String.Compare(element.GetTagName(), "div", true) == 0) || (String.Compare(element.GetTagName(), "td", true) == 0))
					{
						element.InsertAdjacentHTML("beforeEnd", html);
					}
				}
			}
			else
			{
				NativeMethods.IHTMLTxtRange textRange = (NativeMethods.IHTMLTxtRange)Selection.Selection;
				textRange.PasteHTML(html);
			}
		}

		public HtmlSelection Selection
		{
			get
			{
				if (this.selection == null)
				{
					this.selection = new HtmlSelection(this);
				}

				return this.selection;
			}
		}

		public HtmlTextFormatting TextFormatting
		{
			get
			{
				/*
				if (!this.IsReady)
				{
					throw new InvalidOperationException();
				}*/

				if (this.textFormatting == null)
				{
					this.textFormatting = new HtmlTextFormatting(this);
				}

				return this.textFormatting;
			}
		}
	}
}
