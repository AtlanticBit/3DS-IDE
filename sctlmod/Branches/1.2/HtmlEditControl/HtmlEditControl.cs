// ------------------------------------------------------------
// Writer, WYSIWYG editor for HTML
// Copyright (c) 2002-2003 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder
// ------------------------------------------------------------
// Based on HTML editor control code
// Copyright (c) 2002-2003 Nikhil Kothari. All rights reserved.
// http://www.nikhilk.net
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

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

    public class HtmlEditControl : Control, IDisposable
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

        private HTMLEditor.HtmlSite site;

        private static IDictionary urlMap;

        private bool isDesignMode;
        private bool designModeDesired;

        private HTMLEditor.HtmlSelection selection;
        private HTMLEditor.HtmlTextFormatting textFormatting;

        private HTMLEditor.NativeMethods.IPersistStreamInit persistStream;
        private HTMLEditor.NativeMethods.IOleUndoManager undoManager;
        private HTMLEditor.NativeMethods.IHTMLEditServices editServices;

        public HtmlEditControl()
        {
            this.firstActivation = true;
            this.TabStop = true;

			this.url = null;
			this.permanentUrl = url;


			this.timer = new Timer();
			this.timer.Interval = 500;
			this.timer.Tick += new EventHandler(this.Timer_Tick);
			this.timer.Start();

			this.Dock = DockStyle.Fill;
			this.TabIndex = 0;
			this.IsDesignMode = true;

			if (this.url == null)
			{
				string tempPath = Path.GetTempFileName();
				if (File.Exists(tempPath))
				{
					File.Delete(tempPath);
				}

				Directory.CreateDirectory(tempPath);

				string tempFileName = Path.ChangeExtension("Document" + documentIndex, ".htm");
				documentIndex++;

				this.url = Path.Combine(tempPath, tempFileName);

				StreamWriter writer = new StreamWriter(this.url);
				writer.Write("<html><body></body></html>");
				writer.Close();
			}

			StreamReader reader = File.OpenText(this.url);
			this.LoadHtml(reader.ReadToEnd(), this.url);
			reader.Close();

			//CommandBarContextMenu contextMenu = new CommandBarContextMenu();
			//this.commandManager.Add("Edit.Undo", contextMenu.Items.AddButton(CommandBarImageResource.Undo, "&Undo", null, Keys.Control | Keys.Z));
			//this.commandManager.Add("Edit.Redo", contextMenu.Items.AddButton(CommandBarImageResource.Redo, "&Redo", null, Keys.Control | Keys.Y));
			//contextMenu.Items.AddSeparator();
			//this.commandManager.Add("Edit.Cut", contextMenu.Items.AddButton(CommandBarImageResource.Cut, "Cu&t", null, Keys.Control | Keys.X));
			//this.commandManager.Add("Edit.Copy", contextMenu.Items.AddButton(CommandBarImageResource.Copy, "&Copy", null, Keys.Control | Keys.C));
			//this.commandManager.Add("Edit.Paste", contextMenu.Items.AddButton(CommandBarImageResource.Paste, "&Paste", null, Keys.Control | Keys.V));
			//this.commandManager.Add("Edit.Delete", contextMenu.Items.AddButton(CommandBarImageResource.Delete, "&Delete", null, Keys.Delete));
			//this.ContextMenu = contextMenu;

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

        internal HTMLEditor.NativeMethods.IHTMLDocument2 HtmlDocument
        {
            get { return this.site.Document; }
        }

        internal HTMLEditor.NativeMethods.IOleCommandTarget CommandTarget
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
            int hr = CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, command, HTMLEditor.NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, arguments, retVal);
            if (hr != HTMLEditor.NativeMethods.S_OK)
            {
                throw new Exception("Execution of MSHTML command ID \'" + command + "\' failed.");
            }
            return retVal[0];
        }

        internal object ExecuteWithUserInterface(int command, object[] arguments)
        {
            object[] retVal = new object[] { null };
            int hr = CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, command, HTMLEditor.NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, arguments, retVal);
            if (hr != HTMLEditor.NativeMethods.S_OK)
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
            HTMLEditor.NativeMethods.tagOLECMD command = new HTMLEditor.NativeMethods.tagOLECMD();
            command.cmdID = commandId;
            this.CommandTarget.QueryStatus(ref HTMLEditor.NativeMethods.Guid_MSHTML, 1, command, 0);
            return (command.cmdf >> 1);
        }

        public HtmlElement GetElementByID(string id)
        {
            HTMLEditor.NativeMethods.IHTMLElement body = this.site.Document.GetBody();
            HTMLEditor.NativeMethods.IHTMLElementCollection children = (HTMLEditor.NativeMethods.IHTMLElementCollection)body.GetAll();
            HTMLEditor.NativeMethods.IHTMLElement element = (HTMLEditor.NativeMethods.IHTMLElement)children.Item(id, 0);

            if (element == null)
            {
                return null;
            }

            return new HTMLEditor.HtmlElement(element, this);
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

            HTMLEditor.NativeMethods.IStream stream = null;

            //First we create a COM stream
            IntPtr hglobal = Marshal.StringToHGlobalUni(content);
            HTMLEditor.NativeMethods.CreateStreamOnHGlobal(hglobal, true, out stream);

            // Initialize a new document if there is nothing to load
            if (stream == null)
            {
                HTMLEditor.NativeMethods.IPersistStreamInit psi = (HTMLEditor.NativeMethods.IPersistStreamInit)this.site.Document;
                Debug.Assert(psi != null, "Expected IPersistStreamInit");
                psi.InitNew();
                psi = null;
            }
            else
            {
                HTMLEditor.NativeMethods.IHTMLDocument2 document = this.site.Document;

                if (url == null)
                {
                    // If there is no specified URL load the document from the stream.
                    HTMLEditor.NativeMethods.IPersistStreamInit psi = (HTMLEditor.NativeMethods.IPersistStreamInit)document;
                    Debug.Assert(psi != null, "Expected IPersistStreamInit");
                    psi.Load(stream);
                    psi = null;
                }
                else
                {
                    // Otherwise we create a moniker and load the stream to that moniker.
                    HTMLEditor.NativeMethods.IPersistMoniker persistMoniker = (HTMLEditor.NativeMethods.IPersistMoniker)document;

                    HTMLEditor.NativeMethods.IMoniker moniker = null;
                    HTMLEditor.NativeMethods.CreateURLMoniker(null, url, out moniker);

                    HTMLEditor.NativeMethods.IBindCtx bindContext = null;
                    HTMLEditor.NativeMethods.CreateBindCtx(0, out bindContext);

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
            CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, HTMLEditor.NativeMethods.IDM_PERSISTDEFAULTVALUES, 0, mshtmlArgs, null);

            mshtmlArgs[0] = true;
            CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, HTMLEditor.NativeMethods.IDM_PROTECTMETATAGS, 0, mshtmlArgs, null);

            mshtmlArgs[0] = true;
            CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, HTMLEditor.NativeMethods.IDM_PRESERVEUNDOALWAYS, 0, mshtmlArgs, null);

            mshtmlArgs[0] = true;
            CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, HTMLEditor.NativeMethods.IDM_NOACTIVATENORMALOLECONTROLS, 0, mshtmlArgs, null);

            mshtmlArgs[0] = true;
            CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, HTMLEditor.NativeMethods.IDM_NOACTIVATEDESIGNTIMECONTROLS, 0, mshtmlArgs, null);

            mshtmlArgs[0] = true;
            CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, HTMLEditor.NativeMethods.IDM_NOACTIVATEJAVAAPPLETS, 0, mshtmlArgs, null);

            mshtmlArgs[0] = true;
            CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, HTMLEditor.NativeMethods.IDM_NOFIXUPURLSONPASTE, 0, mshtmlArgs, null);

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
                this.site.SetFocus();
            }
            else
            {
                this.focusDesired = true;
            }
        }

        /// <summary>We can only activate the MSHTML after our handle has been created, so upon creating the handle, we create and activate HTMLEditor.NativeMethods. If LoadHtml was called prior to this, we do the loading now.</summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.firstActivation)
            {
                this.site = new HTMLEditor.HtmlSite(this);
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

            this.persistStream = (HTMLEditor.NativeMethods.IPersistStreamInit)this.HtmlDocument;

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

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override bool PreProcessMessage(ref Message m)
        {
            bool handled = false;
            if ((m.Msg >= HTMLEditor.NativeMethods.WM_KEYFIRST) && (m.Msg <= HTMLEditor.NativeMethods.WM_KEYLAST))
            {
                // If it's a key down, first see if the key combo is a command key
                if (m.Msg == HTMLEditor.NativeMethods.WM_KEYDOWN)
                {
                    handled = ProcessCmdKey(ref m, (Keys)(int)m.WParam | ModifierKeys);
                }

                if (!handled)
                {
                    int keyCode = (int)m.WParam;

                    // Don't let Trident eat Ctrl-PgUp/PgDn
                    if (((keyCode != (int)Keys.PageUp) && (keyCode != (int)Keys.PageDown)) || ((ModifierKeys & Keys.Control) == 0))
                    {
                        HTMLEditor.NativeMethods.COMMSG cm = new HTMLEditor.NativeMethods.COMMSG();
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
                HTMLEditor.NativeMethods.IHTMLDocument2 document = this.site.Document;

                // First save the document to a stream
                HTMLEditor.NativeMethods.IPersistStreamInit psi = (HTMLEditor.NativeMethods.IPersistStreamInit)document;
                Debug.Assert(psi != null, "Expected IPersistStreamInit");

                HTMLEditor.NativeMethods.IStream stream = null;
                HTMLEditor.NativeMethods.CreateStreamOnHGlobal(HTMLEditor.NativeMethods.NullIntPtr, true, out stream);

                psi.Save(stream, 1);

                // Now copy the stream to the string
                HTMLEditor.NativeMethods.STATSTG stat = new HTMLEditor.NativeMethods.STATSTG();
                stream.Stat(stat, 1);
                int length = (int)stat.cbSize;
                byte[] bytes = new byte[length];

                IntPtr hglobal;
                HTMLEditor.NativeMethods.GetHGlobalFromStream(stream, out hglobal);
                Debug.Assert(hglobal != HTMLEditor.NativeMethods.NullIntPtr, "Failed in GetHGlobalFromStream");

                // First copy the stream to a byte array
                IntPtr pointer = HTMLEditor.NativeMethods.GlobalLock(hglobal);
                if (pointer != HTMLEditor.NativeMethods.NullIntPtr)
                {
                    Marshal.Copy(pointer, bytes, 0, length);

                    HTMLEditor.NativeMethods.GlobalUnlock(hglobal);

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
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_PRINT); }
        }

        public void Print()
        {
            this.ExecuteWithUserInterface(HTMLEditor.NativeMethods.IDM_PRINT, null);
        }

        public bool CanPrintPreview
        {
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_PRINTPREVIEW); }
        }

        public void PrintPreview()
        {
            this.ExecuteWithUserInterface(HTMLEditor.NativeMethods.IDM_PRINTPREVIEW, null);
        }

        public bool CanCopy
        {
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_COPY); }
        }

        public void Copy()
        {
            if (!this.CanCopy)
            {
                throw new InvalidOperationException();
            }

            this.Execute(HTMLEditor.NativeMethods.IDM_COPY);
        }

        public bool CanCut
        {
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_CUT); }
        }

        public void Cut()
        {
            if (!CanCut)
            {
                throw new InvalidOperationException();
            }

            this.Execute(HTMLEditor.NativeMethods.IDM_CUT);
        }

        public bool CanPaste
        {
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_PASTE); }
        }

        public void Paste()
        {
            if (!this.CanPaste)
            {
                throw new InvalidOperationException();
            }

            this.Execute(HTMLEditor.NativeMethods.IDM_PASTE);
        }

        public bool CanDelete
        {
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_DELETE); }
        }

        public void Delete()
        {
            if (!this.CanDelete)
            {
                throw new InvalidOperationException();
            }

            this.Execute(HTMLEditor.NativeMethods.IDM_DELETE);
        }

        public bool CanRedo
        {
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_REDO); }
        }

        public void Redo()
        {
            if (!this.CanRedo)
            {
                throw new InvalidOperationException();
            }

            this.Execute(HTMLEditor.NativeMethods.IDM_REDO);
        }

        public string RedoDescription
        {
            get { return (this.CanRedo) ? this.UndoManager.GetLastRedoDescription() : string.Empty; }
        }

        public bool CanUndo
        {
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_UNDO); }
        }

        public void Undo()
        {
            if (!this.CanUndo)
            {
                throw new InvalidOperationException();
            }

            this.Execute(HTMLEditor.NativeMethods.IDM_UNDO);
        }

        public string UndoDescription
        {
            get { return (this.CanUndo) ? this.UndoManager.GetLastUndoDescription() : string.Empty; }
        }

        public bool CanSelectAll
        {
            get { return this.IsEnabled(HTMLEditor.NativeMethods.IDM_SELECTALL); }
        }

        public void SelectAll()
        {
            this.Execute(HTMLEditor.NativeMethods.IDM_SELECTALL);
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
                    this.Execute(HTMLEditor.NativeMethods.IDM_2D_POSITION, args);
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

        private HTMLEditor.NativeMethods.IHTMLEditServices MSHTMLEditServices
        {
            get
            {
                if (this.editServices == null)
                {
                    HTMLEditor.NativeMethods.IServiceProvider serviceProvider = this.HtmlDocument as HTMLEditor.NativeMethods.IServiceProvider;
                    Debug.Assert(serviceProvider != null);
                    Guid shtmlGuid = new Guid(0x3050f7f9, 0x98b5, 0x11cf, 0xbb, 0x82, 0x00, 0xaa, 0x00, 0xbd, 0xce, 0x0b);
                    Guid intGuid = (typeof(HTMLEditor.NativeMethods.IHTMLEditServices)).GUID;

                    IntPtr editServicePtr = HTMLEditor.NativeMethods.NullIntPtr;
                    int hr = serviceProvider.QueryService(ref shtmlGuid, ref intGuid, out editServicePtr);
                    Debug.Assert((hr == HTMLEditor.NativeMethods.S_OK) && (editServicePtr != HTMLEditor.NativeMethods.NullIntPtr), "Did not get IHTMLEditService");
                    if ((hr == HTMLEditor.NativeMethods.S_OK) && (editServicePtr != HTMLEditor.NativeMethods.NullIntPtr))
                    {
                        this.editServices = (HTMLEditor.NativeMethods.IHTMLEditServices)Marshal.GetObjectForIUnknown(editServicePtr);
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
                    if (this.persistStream.IsDirty() == HTMLEditor.NativeMethods.S_OK)
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
                    this.Execute(HTMLEditor.NativeMethods.IDM_SETDIRTY, new object[] { value });
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
                    int hr = CommandTarget.Exec(ref HTMLEditor.NativeMethods.Guid_MSHTML, HTMLEditor.NativeMethods.IDM_MULTIPLESELECTION, 0, args, null);
                    Debug.Assert(hr == HTMLEditor.NativeMethods.S_OK);
                }
            }
        }

        private HTMLEditor.NativeMethods.IOleUndoManager UndoManager
        {
            get
            {
                if (this.undoManager == null)
                {
                    HTMLEditor.NativeMethods.IServiceProvider serviceProvider = this.HtmlDocument as HTMLEditor.NativeMethods.IServiceProvider;
                    Debug.Assert(serviceProvider != null);
                    Guid undoManagerGuid = typeof(HTMLEditor.NativeMethods.IOleUndoManager).GUID;
                    Guid undoManagerGuid2 = typeof(HTMLEditor.NativeMethods.IOleUndoManager).GUID;
                    IntPtr undoManagerPtr = HTMLEditor.NativeMethods.NullIntPtr;
                    int hr = serviceProvider.QueryService(ref undoManagerGuid2, ref undoManagerGuid, out undoManagerPtr);
                    if ((hr == HTMLEditor.NativeMethods.S_OK) && (undoManagerPtr != HTMLEditor.NativeMethods.NullIntPtr))
                    {
                        this.undoManager = (HTMLEditor.NativeMethods.IOleUndoManager)Marshal.GetObjectForIUnknown(undoManagerPtr);
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
            HTMLEditor.NativeMethods.IHTMLSelectionObject selectionObj = this.site.Document.GetSelection();

            // Check if a selection actually exists.
            bool selectionExists = false;
            if (selectionObj != null)
            {
                selectionExists = selectionObj.GetSelectionType().Equals("Text");
            }
            HTMLEditor.NativeMethods.IHTMLTxtRange textRange = null;
            if (selectionExists)
            {
                object o = selectionObj.CreateRange();
                textRange = o as HTMLEditor.NativeMethods.IHTMLTxtRange;
            }
            if (textRange == null)
            {
                // If no selection exists, select the entire body.
                HTMLEditor.NativeMethods.IHtmlBodyElement bodyElement = this.site.Document.GetBody() as HTMLEditor.NativeMethods.IHtmlBodyElement;
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
                textRange = selectionObj.CreateRange() as HTMLEditor.NativeMethods.IHTMLTxtRange;

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
                HTMLEditor.NativeMethods.IHTMLTxtRange selection = this.Selection.Selection as HTMLEditor.NativeMethods.IHTMLTxtRange;

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
                    return this.IsEnabled(HTMLEditor.NativeMethods.IDM_HYPERLINK);
                }
            }
        }

        public void InsertHyperlink(string url, string description)
        {
            Selection.SynchronizeSelection();
            if (url == null)
            {
                try
                {
                    this.Execute(HTMLEditor.NativeMethods.IDM_HYPERLINK);
                }
                catch { }
            }
            else
            {
                if (((this.Selection.Type == HtmlSelectionType.TextSelection) || (this.Selection.Type == HtmlSelectionType.Empty)) && (this.Selection.Length == 0))
                {
                    InsertHtml("<a href=\"" + url + "\">" + description + "</a>");
                }
                else
                {
                    this.ExecuteWithUserInterface(HTMLEditor.NativeMethods.IDM_HYPERLINK, new object[] { url });
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
                    HTMLEditor.NativeMethods.IHtmlControlRange controlRange = (HTMLEditor.NativeMethods.IHtmlControlRange)this.Selection.Selection;
                    int selectedItemCount = controlRange.GetLength();
                    if (selectedItemCount == 1)
                    {
                        HTMLEditor.NativeMethods.IHTMLElement element = controlRange.Item(0);
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
                this.Execute(HTMLEditor.NativeMethods.IDM_IMAGE, new object[] { url });
            }
        }

        public void InsertImage()
        {
            this.ExecuteWithUserInterface(HTMLEditor.NativeMethods.IDM_IMAGE, null);
        }

        public void InsertHtml(string html)
        {
            this.Selection.SynchronizeSelection();
            if (this.Selection.Type == HtmlSelectionType.ElementSelection)
            {
                //If it's a control range, we can only insert if we are in a div or td
                HTMLEditor.NativeMethods.IHtmlControlRange controlRange = (HTMLEditor.NativeMethods.IHtmlControlRange)Selection.Selection;
                int selectedItemCount = controlRange.GetLength();
                if (selectedItemCount == 1)
                {
                    HTMLEditor.NativeMethods.IHTMLElement element = controlRange.Item(0);
                    if ((String.Compare(element.GetTagName(), "div", true) == 0) || (String.Compare(element.GetTagName(), "td", true) == 0))
                    {
                        element.InsertAdjacentHTML("beforeEnd", html);
                    }
                }
            }
            else
            {
                HTMLEditor.NativeMethods.IHTMLTxtRange textRange = (HTMLEditor.NativeMethods.IHTMLTxtRange)Selection.Selection;
                textRange.PasteHTML(html);
            }
        }

        public HTMLEditor.HtmlSelection Selection
        {
            get
            {
                if (this.selection == null)
                {
                    this.selection = new HTMLEditor.HtmlSelection(this);
                }

                return this.selection;
            }
        }

        public HTMLEditor.HtmlTextFormatting TextFormatting
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
                    this.textFormatting = new HTMLEditor.HtmlTextFormatting(this);
                }

                return this.textFormatting;
            }
        }

		private static int documentIndex = 1;
		
		// private string url;
		private string permanentUrl;
		private Timer timer;

		string findSearchText = string.Empty;
		bool findSearchCase = false;
		bool findSearchWhole = false;
		bool findSearchDirection = false;



		public  void _Dispose()
		{
			this.timer.Stop();
			this.timer.Tick -= new EventHandler(this.Timer_Tick);

			this.Dispose();
			

			if (this.permanentUrl == null)
			{
				string tempPath = Path.GetDirectoryName(this.url);
				if (Directory.Exists(tempPath))
				{
					Directory.Delete(tempPath, true);
				}
			}
		}

        public HtmlEditControl HtmlControl
		{
			get { return this; }
		}

		public void Save()
		{
			if (this.permanentUrl != null)
			{
				this.SaveAs(this.permanentUrl);
			}
			else
			{
				this.SaveAs();
			}
		}

		public void SaveAs()
		{
			SaveFileDialog dialog = new SaveFileDialog();
			//dialog.Filter = Resource.GetString("HtmlFilter");
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string url = dialog.FileName;
				this.SaveAs(url);
			}
		}

		private void SaveAs(string url)
		{
			Cursor currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			// TODO url != fileName

			// XHTML Formatting
			using (StreamWriter writer = new StreamWriter(url))
			{
				HtmlFormatter formatter = new HtmlFormatter();
				formatter.Format(this.SaveHtml(), writer);
			}

			// BUGFIX Special character corruption
			// insert file header bytes to resolve corruption
			FileStream fs = new FileStream(url, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			byte[] buffer = new byte[fs.Length];
			fs.Read(buffer,0,(int)fs.Length);
			fs.Close();
			//	if (!buffer[0].Equals( Convert.ToByte("EF", 16) ))  // 239
			//	{ // because this is always true remove the statement
			try 
			{
				int byteCount = 3;
				fs = new FileStream(url, FileMode.OpenOrCreate, FileAccess.ReadWrite);
				fs.SetLength( buffer.Length + byteCount );
				byte[] characters = new byte[] { Convert.ToByte("EF", 16), 
												Convert.ToByte("BB", 16), 
												Convert.ToByte("BF", 16) };   //  239, 187, 191
				fs.Write(characters, 0, byteCount);
				fs.Write(buffer, 0, buffer.Length);
			}
			finally
			{
				if (fs != null) 
				{
					fs.Close();
				}
			}

            
			this.permanentUrl = url;
			this.url = url;

			Cursor.Current = currentCursor;
		}

		public bool Execute(string commandState)
		{
			if (this.IsHandleCreated && this.IsReady)
			{
				switch (commandState)
				{
					case "File.Print":
						this.Print();
						return true;
	
					case "File.PrintPreview":
						this.PrintPreview();
						return true;

					case "Edit.Undo":
						this.Undo();
						return true;

					case "Edit.Redo":
						this.Redo();
						return true;

					case "Edit.Cut":
						this.Cut();
						return true;

					case "Edit.Copy":
						this.Copy();
						return true;

					case "Edit.Paste":
						this.Paste();
						return true;

					case "Edit.Delete":
						this.Delete();
						return true;

					case "Edit.SelectAll":
						this.SelectAll();
						return true;

					case "Format.ForeColor.White":
						this.TextFormatting.ForeColor = Color.White;
						return true;

					case "Format.ForeColor.Red":
						this.TextFormatting.ForeColor = Color.Red;
						return true;

					case "Format.ForeColor.Green":
						this.TextFormatting.ForeColor = Color.FromArgb(0, 255, 0);
						return true;

					case "Format.ForeColor.Blue":
						this.TextFormatting.ForeColor = Color.Blue;
						return true;

					case "Format.ForeColor.Yellow":
						this.TextFormatting.ForeColor = Color.Yellow;
						return true;

					case "Format.ForeColor.Black":
						this.TextFormatting.ForeColor = Color.Black;
						return true;

					case "Format.ForeColor":
					{
						ColorDialog dialog = new ColorDialog();
						dialog.Color = this.TextFormatting.ForeColor;
						if (dialog.ShowDialog() == DialogResult.OK)
						{
							this.TextFormatting.ForeColor = dialog.Color;
						}
					}
						return true;

					case "Format.BackColor.Black":
						this.TextFormatting.BackColor = Color.Black;
						return true;

					case "Format.BackColor.Red":
						this.TextFormatting.BackColor = Color.Red;
						return true;

					case "Format.BackColor.Green":
						this.TextFormatting.BackColor = Color.FromArgb(0, 255, 0);
						return true;

					case "Format.BackColor.Blue":
						this.TextFormatting.BackColor = Color.Blue;
						return true;

					case "Format.BackColor.Yellow":
						this.TextFormatting.BackColor = Color.Yellow;
						return true;

					case "Format.BackColor.White":
						this.TextFormatting.BackColor = Color.White;
						return true;

					case "Format.BackColor":
					{
						ColorDialog dialog = new ColorDialog();
						dialog.Color = this.TextFormatting.BackColor;
						if (dialog.ShowDialog() == DialogResult.OK)
						{
							this.TextFormatting.BackColor = dialog.Color;
						}
					}
						return true;

					case "Format.Font":
						// this.TextFormatting.FontName = (commandState as CommandComboBoxState).Value;
						return true;

					case "Format.FontSize":
						// this.TextFormatting.FontSize = (HtmlFontSize)Int32.Parse((commandState as CommandComboBoxState).Value);
						return true;

					case "Format.Bold":
						this.TextFormatting.ToggleBold();
						return true;

					case "Format.Italic":
						this.TextFormatting.ToggleItalics();
						return true;

					case "Format.Underline":
						this.TextFormatting.ToggleUnderline();
						return true;

					case "Format.Strikethrough":
						this.TextFormatting.ToggleStrikethrough();
						return true;

					case "Format.Superscript":
						this.TextFormatting.ToggleSuperscript();
						return true;

					case "Format.Subscript":
						this.TextFormatting.ToggleSubscript();
						return true;

					case "Format.AlignLeft":
						this.TextFormatting.Alignment = HtmlAlignment.Left;
						return true;

					case "Format.AlignCenter":
						this.TextFormatting.Alignment = HtmlAlignment.Center;
						return true;

					case "Format.AlignRight":
						this.TextFormatting.Alignment = HtmlAlignment.Right;
						return true;

					case "Format.OrderedList":
						this.TextFormatting.HtmlFormat = HtmlFormat.OrderedList;
						return true;

					case "Format.UnorderedList":
						this.TextFormatting.HtmlFormat = HtmlFormat.UnorderedList;
						return true;

					case "Format.Indent":
						this.TextFormatting.Indent();
						return true;

					case "Format.Unindent":
						this.TextFormatting.Unindent();
						return true;

					case "Edit.InsertHyperlink":
						this.InsertHyperlink();
						return true;

					case "Edit.InsertPicture":
						this.InsertPicture();
						return true;

					case "Edit.InsertDateTime":
						this.InsertDateTime();
						return true;

					case "Edit.Find":
						this.Find();
						return true;

					case "Edit.FindNext":
						this.FindNext();
						return true;
					
					case "Edit.Replace":
						this.Replace();
						return true;
				}
			}

			return false;
		}

		public bool QueryStatus(string commandState)
		{
			if (this.IsHandleCreated && this.IsReady)
			{

				switch (commandState)
				{
					case "File.Print":
						// commandState.IsEnabled = this.CanPrint;
						return true;
	
					case "File.PrintPreview":
						// commandState.IsEnabled = this.CanPrintPreview;
						return true;

					case "Edit.Undo":
						//commandState.IsEnabled = this.CanUndo;
						//commandState.Text = "&Undo " + this.UndoDescription;
						return true;

					case "Edit.Redo":
						//commandState.IsEnabled = this.CanRedo;
						//commandState.Text = "&Redo " + this.RedoDescription;
						return true;

					case "Edit.Copy":
						//commandState.IsEnabled = this.CanCopy;
						return true;

					case "Edit.Cut":
						//commandState.IsEnabled = this.CanCut;
						return true;

					case "Edit.Paste":
						//commandState.IsEnabled = this.CanPaste;
						return true;

					case "Edit.Delete":
						//commandState.IsEnabled = this.CanDelete;
						return true;

					case "Edit.SelectAll":
						//commandState.IsEnabled = this.CanSelectAll;
						return true;

					case "Edit.Find":
					case "Edit.FindNext":
					case "Edit.Replace":
						//commandState.IsEnabled = true;
						return true;

					case "Edit.InsertHyperlink":
					case "Edit.InsertPicture":
					case "Edit.InsertDateTime":
						//commandState.IsEnabled = this.CanInsertHtml;
						return true;

					case "Format.Font":
						//commandState.IsEnabled = this.TextFormatting.CanSetFontName;
						//(commandState as CommandComboBoxState).Value = this.TextFormatting.FontName;
						return true;

					case "Format.FontSize":
						//commandState.IsEnabled = this.TextFormatting.CanSetFontSize;
						//(commandState as CommandComboBoxState).Value = ((int)this.TextFormatting.FontSize).ToString();
						return true;

					case "Format.Bold":
						//commandState.IsEnabled = this.TextFormatting.CanToggleBold;
						//checkBoxState.IsChecked = this.TextFormatting.IsBold;
						return true;

					case "Format.Italic":
						//commandState.IsEnabled = this.TextFormatting.CanToggleItalic;
						//checkBoxState.IsChecked = this.TextFormatting.IsItalic;
						return true;

					case "Format.Underline":
						//commandState.IsEnabled = this.TextFormatting.CanToggleUnderline;
						//checkBoxState.IsChecked = this.TextFormatting.IsUnderline;
						return true;

					case "Format.Strikethrough":
						//commandState.IsEnabled = this.TextFormatting.CanToggleStrikethrough;
						//checkBoxState.IsChecked = this.TextFormatting.IsStrikethrough;
						return true;

					case "Format.Subscript":
						//commandState.IsEnabled = this.TextFormatting.CanToggleSubscript;
						//checkBoxState.IsChecked = this.TextFormatting.IsSubscript;
						return true;

					case "Format.Superscript":
						//commandState.IsEnabled = this.TextFormatting.CanToggleSuperscript;
						//checkBoxState.IsChecked = this.TextFormatting.IsSuperscript;
						return true;

					case "Format.AlignLeft":
						//commandState.IsEnabled = this.TextFormatting.CanAlign(HtmlAlignment.Left);
						//checkBoxState.IsChecked = (this.TextFormatting.Alignment == HtmlAlignment.Left);
						return true;

					case "Format.AlignRight":
						//commandState.IsEnabled = this.TextFormatting.CanAlign(HtmlAlignment.Right);
						//checkBoxState.IsChecked = (this.TextFormatting.Alignment == HtmlAlignment.Right);
						return true;

					case "Format.AlignCenter":
						//commandState.IsEnabled = this.TextFormatting.CanAlign(HtmlAlignment.Center);
						//checkBoxState.IsChecked = (this.TextFormatting.Alignment == HtmlAlignment.Center);
						return true;

					case "Format.OrderedList":
						//commandState.IsEnabled = this.TextFormatting.CanSetHtmlFormat;
						//checkBoxState.IsChecked = (this.TextFormatting.HtmlFormat == HtmlFormat.OrderedList);
						return true;

					case "Format.UnorderedList":
						//commandState.IsEnabled = this.TextFormatting.CanSetHtmlFormat;
						//checkBoxState.IsChecked = (this.TextFormatting.HtmlFormat == HtmlFormat.UnorderedList);
						return true;

					case "Format.Indent":
						//commandState.IsEnabled = this.TextFormatting.CanIndent;
						return true;

					case "Format.Unindent":
						//commandState.IsEnabled = this.TextFormatting.CanUnindent;
						return true;

					case "Format.ForeColor":
					case "Format.ForeColor.Black":
					case "Format.ForeColor.Yellow":
					case "Format.ForeColor.Red":
					case "Format.ForeColor.Green":
					case "Format.ForeColor.Blue":
					case "Format.ForeColor.White":
						//commandState.IsEnabled = this.TextFormatting.CanSetForeColor;
						return true;

					case "Format.BackColor":
					case "Format.BackColor.Black":
					case "Format.BackColor.Yellow":
					case "Format.BackColor.Red":
					case "Format.BackColor.Green":
					case "Format.BackColor.Blue":
					case "Format.BackColor.White":
						//commandState.IsEnabled = this.TextFormatting.CanSetBackColor;
						return true;
				}
			}

			return false;
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			//this.commandManager.QueryStatus();
		}

		private void InsertHyperlink() 
		{
			this.InsertHyperlink(null, null);
		}

		private void InsertPicture()
		{
			this.InsertImage();
		}

		private void InsertDateTime(){
            /*
			InsertDateTimeDialog dialog = new InsertDateTimeDialog();
			if ( dialog.Run() ) {
				this.InsertHtml(dialog.Format);
			}
             */
        }

		private void Find() {
            /*
			FindDialog dialog = new FindDialog();
			if ( findSearchText != string.Empty ) {
				dialog.SearchText = this.findSearchText;
				dialog.IsCaseChecked = this.findSearchCase;
				dialog.IsWholeChecked = this.findSearchWhole;
				dialog.IsUp = this.findSearchDirection;
			}
			if ( dialog.Run() ) {
				this.findSearchText = dialog.SearchText;
				this.findSearchCase = dialog.IsCaseChecked;
				this.findSearchWhole = dialog.IsWholeChecked;
				this.findSearchDirection = dialog.IsUp;
				if ( !this.Find(this.findSearchText, this.findSearchCase, this.findSearchWhole, this.findSearchDirection) ) {
					MessageBox.Show("Finished searching the document.", Resource.GetString("ApplicationName"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
             * */
		}

		private void Replace() {
            /*
			ReplaceDialog dialog = new ReplaceDialog(this);
			dialog.ShowDialog();
             * */
		}

		private void FindNext() {
            /*
			if ( this.findSearchText != string.Empty ) {
				this.Find(this.findSearchText, this.findSearchCase, this.findSearchWhole, this.findSearchDirection);
			}
             */
		}

    }
}
