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
    using System.Diagnostics;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class HtmlSelection 
    {
        private static readonly string DesignTimeLockAttribute = "Design_Time_Lock";

		public event EventHandler SelectionChanged;

        private HtmlControl control;

        private NativeMethods.IHTMLDocument2 document;
        private HtmlSelectionType type;
        private int selectionLength;
        private string text;
        private object selection;
        private ArrayList items;
        private ArrayList elements;
        private bool sameParentValid;
        // private int minZIndex;
        private int maxZIndex;

        public HtmlSelection(HtmlControl control) 
        {
            this.control = control;
            // this.minZIndex = 100;
            this.maxZIndex = 99;
        }

        // Indicates if the current selection can be aligned.
        public bool CanAlign 
        {
            get 
            {
                if (this.items.Count < 2) 
                {
                    return false;
                }
                if (this.type == HtmlSelectionType.ElementSelection) 
                {
                    foreach (NativeMethods.IHTMLElement elem in this.items) 
                    {
                        //First check if they are all absolutely positioned
                        if (!IsElement2DPositioned(elem)) 
                        {
                            return false;
                        }

                        //Then check if none of them are locked
                        if (IsElementLocked(elem)) 
                        {
                            return false;
                        }
                    }
                    //Then check if they all have the same parent
                    if (!SameParent) 
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        // Indicates if the current selection can be size-matched.
        public bool CanMatchSize 
        {
            get 
            {
                if (this.items.Count < 2) 
                {
                    return false;
                }
                if (this.type == HtmlSelectionType.ElementSelection) 
                {
                    foreach (NativeMethods.IHTMLElement elem in this.items) 
                    {
                        //Then check if none of them are locked
                        if (IsElementLocked(elem)) 
                        {
                            return false;
                        }
                    }
                    //Then check if they all have the same parent
                    if (!SameParent) 
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        // Indicates if the current selection have it's z-index modified.
        public bool CanChangeZIndex 
        {
            get 
            {
                if (this.items.Count == 0) 
                {
                    return false;
                }
                if (this.type == HtmlSelectionType.ElementSelection) 
                {
                    foreach (NativeMethods.IHTMLElement elem in this.items) 
                    {
                        //First check if they are all absolutely positioned
                        if (!IsElement2DPositioned(elem)) 
                        {
                            return false;
                        }
                    }
                    //Then check if they all have the same parent
                    if (!SameParent) 
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        // Indicates if the current selection can be wrapped in HTML tags.
        public bool CanWrapSelection 
        {
            get 
            {
                if ((this.selectionLength != 0) && (Type == HtmlSelectionType.TextSelection)) 
                {
                    return true;
                }
                return false;
            }
        }

        protected HtmlControl Control
        {
            get { return this.control; }
        }

        public ICollection Elements 
        {
            get 
            {
                if (this.elements == null) 
                {
                    this.elements = new ArrayList();
                    foreach (NativeMethods.IHTMLElement element in this.items) 
                    {
                        object wrapper = this.CreateElementWrapper(element);
                        if (wrapper != null) 
                        {
                            this.elements.Add(wrapper);
                        }
                    }
                }
                return this.elements;
            }
        }

        internal ICollection Items 
        {
            get { return this.items; }
        }

        public int Length 
        {
            get { return this.selectionLength; }
        }

        // Indicates if all items in the selection have the same parent element.
        private bool SameParent 
        {
            get 
            {
                if (!this.sameParentValid) 
                {
                    IntPtr primaryParentElementPtr = NativeMethods.NullIntPtr;

                    foreach (NativeMethods.IHTMLElement elem in this.items) 
                    {
                        //Check if all items have the same parent by doing pointer equality
                        NativeMethods.IHTMLElement parentElement = elem.GetParentElement();
                        IntPtr parentElementPtr = Marshal.GetIUnknownForObject(parentElement);
                        //If we haven't gotten a primary parent element (ie, this is the first time through the loop)
                        //Remember what the this parent element is
                        if (primaryParentElementPtr == NativeMethods.NullIntPtr) 
                        {
                            primaryParentElementPtr = parentElementPtr;
                        }
                        else 
                        {
                            //Check the pointers
                            if (primaryParentElementPtr != parentElementPtr) 
                            {
                                Marshal.Release(parentElementPtr);
                                if (primaryParentElementPtr != NativeMethods.NullIntPtr) 
                                {
                                    Marshal.Release(primaryParentElementPtr);
                                }
                                this.sameParentValid = false;
                                return this.sameParentValid;
                            }
                            Marshal.Release(parentElementPtr);
                        }
                    }
                    if (primaryParentElementPtr != NativeMethods.NullIntPtr) 
                    {
                        Marshal.Release(primaryParentElementPtr);
                    }
                    this.sameParentValid = true;
                }
                return this.sameParentValid;
            }
        }

        /// <summary>
        /// Returns the MSHTML selection object (IHTMLTxtRange or IHTMLControlRange)
        /// Does not synchronize the selection!!!  Uses the selection from the last synchronization
        /// </summary>
        protected internal object Selection 
        {
            get { return this.selection; }
        }

        /// <summary>Returns the text contained in the selection if there is a text selection.</summary>
        public string Text 
        {
            get 
            {
                if (this.type == HtmlSelectionType.TextSelection) 
                {
                    return this.text;
                }
                return null;
            }
        }

        /// <summary>
        /// The HtmlSelectionType of the selection
        /// </summary>
        public HtmlSelectionType Type 
        {
            get 
            {
                return this.type;
            }
        }

        public void ClearSelection() 
        {
            this.control.Execute(NativeMethods.IDM_CLEARSELECTION);
        }

        internal object CreateElementWrapper(NativeMethods.IHTMLElement element) 
        {
            return new HtmlElement(element, this.control);
        }

		/*
        // Returns info about the absolute positioning of the selection.
        public HtmlCommandInfo GetAbsolutePositionInfo() 
        {
            return this.control.GetCommandInfo(NativeMethods.IDM_ABSOLUTE_POSITION);
        }
		*/

		/*
        // Returns info about the design time lock state of the selection
        public HtmlCommandInfo GetLockInfo() 
        {
            if (this.type == HtmlSelectionType.ElementSelection) 
            {
                foreach (NativeMethods.IHTMLElement elem in this.items) 
                {
                    //We only need to check that all elements are absolutely positioned
                    if (!IsElement2DPositioned(elem)) 
                    {
                        return (HtmlCommandInfo)0;
                    }

                    if (IsElementLocked(elem)) 
                    {
                        return HtmlCommandInfo.Checked | HtmlCommandInfo.Enabled;
                    }
                    return HtmlCommandInfo.Enabled;
                }
            }
            return (HtmlCommandInfo)0;
        }
		*/

        public string GetOuterHtml() 
        {
            Debug.Assert(this.Items.Count == 1, "Can't get OuterHtml of more than one element");

            string outerHtml = string.Empty;
            try 
            {
                outerHtml = ((NativeMethods.IHTMLElement)this.items[0]).GetOuterHTML();

                // Call this twice because, in the first call, Trident will call OnContentSave, which calls SetInnerHtml, but
                // the outer HTML it returns does not include that new inner HTML.
                outerHtml = ((NativeMethods.IHTMLElement)this.items[0]).GetOuterHTML();
            }
            catch 
            {
            }

            return outerHtml;
        }

        public ArrayList GetParentHierarchy(object o) 
        {
            NativeMethods.IHTMLElement current = this.GetHtmlElement(o);
            if (current == null) 
            {
                return null;
            }

            string tagName = current.GetTagName().ToLower();
            if (tagName.Equals("body")) 
            {
                return null;
            }

            ArrayList ancestors = new ArrayList();

            current = current.GetParentElement();
            while ((current != null) && (current.GetTagName().ToLower().Equals("body") == false)) 
            {
                HtmlElement element = new HtmlElement(current, this.control);
                if (IsSelectableElement(element)) 
                {
                    ancestors.Add(element);
                }
                current = current.GetParentElement();
            }

            // Don't add the body tag to the hierarchy if we aren't in full document mode
            if (current != null) 
            {
                HtmlElement element = new HtmlElement(current, this.control);
                if (IsSelectableElement(element)) 
                {
                    ancestors.Add(element);
                }
            }

            return ancestors;
        }

        internal NativeMethods.IHTMLElement GetHtmlElement(object o) 
        {
            if (o is HtmlElement) 
            {
                return ((HtmlElement) o).Peer;
            }
            return null;
        }

        /// <summary>Convenience method for checking if the specified element is absolutely positioned.</summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private bool IsElement2DPositioned(NativeMethods.IHTMLElement element) 
        {
            NativeMethods.IHTMLElement2 elem2 = (NativeMethods.IHTMLElement2) element;
            NativeMethods.IHTMLCurrentStyle style = elem2.GetCurrentStyle();
            string position = style.GetPosition();
			return ((position != null) && (position.ToLower() == "absolute"));
        }

        // Convenience method for checking if the specified element has a design time lock.
        private bool IsElementLocked(NativeMethods.IHTMLElement element) 
        {
            object[] attribute = new object[1];
            element.GetAttribute(DesignTimeLockAttribute, 0, attribute);
            if (attribute[0] == null) 
            {
                NativeMethods.IHTMLStyle style = element.GetStyle();
                attribute[0] = style.GetAttribute(DesignTimeLockAttribute,0);
            }

			return ((attribute[0] != null) && (attribute[0] is string));
        }

        protected virtual bool IsSelectableElement(HtmlElement element) 
        {
            return (true || (element.Name.ToLower() != "body"));
        }

        protected virtual void OnSelectionChanged(EventArgs e) 
        {
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, e);
			}
        }

        public bool SelectElement(object o) 
        {
            ArrayList list = new ArrayList(1);
            list.Add(o);
            return SelectElements(list);
        }

        public bool SelectElements(ICollection elements) 
        {
            NativeMethods.IHTMLElement body = this.control.HtmlDocument.GetBody();
            NativeMethods.IHTMLTextContainer container = body as NativeMethods.IHTMLTextContainer;
            Debug.Assert(container != null);
            object controlRange = container.createControlRange();

            NativeMethods.IHtmlControlRange htmlControlRange = controlRange as NativeMethods.IHtmlControlRange;
            Debug.Assert(htmlControlRange != null);
            if (htmlControlRange == null) 
            {
                return false;
            }

            NativeMethods.IHtmlControlRange2 htmlControlRange2 = controlRange as NativeMethods.IHtmlControlRange2;
            Debug.Assert(htmlControlRange2 != null);
            if (htmlControlRange2 == null) 
            {
                return false;
            }

            int hr = 0;
            foreach (object o in elements) 
            {
                NativeMethods.IHTMLElement element = this.GetHtmlElement(o);
                if (element == null) 
                {
                    return false;
                }
                hr = htmlControlRange2.addElement(element);
                if (hr != NativeMethods.S_OK) 
                {
                    break;
                }
            }
            if (hr == NativeMethods.S_OK) 
            {
                //If it succeeded, simply select the control range
                htmlControlRange.Select();
            }
            else 
            {
                // elements like DIV and SPAN, w/o layout, cannot be added to a control selelction.
                NativeMethods.IHtmlBodyElement bodyElement = (NativeMethods.IHtmlBodyElement)body;
                NativeMethods.IHTMLTxtRange textRange = bodyElement.createTextRange();
                if (textRange != null) 
                {
                    foreach (object o in elements) 
                    {
                        try 
                        {
                            NativeMethods.IHTMLElement element = this.GetHtmlElement(o);
                            if (element == null) 
                            {
                                return false;
                            }
                            textRange.MoveToElementText(element);
                        }
                        catch 
                        {
                        }
                    }
                    textRange.Select();
                }
            }
            return true;
        }

        //        /// <summary>
        //        /// Sends all selected items to the back
        //        /// </summary>
        //        public void SendToBack() {
        //            //TODO: How do we compress the ZIndexes so they never go out of the range of an int
        //            SynchronizeSelection();
        //            if (this.type == HtmlSelectionType.ElementSelection) {
        //                if (this.items.Count > 1) {
        //                    //We have to move all items to the back, and maintain their ordering, so
        //                    //Find the maximum ZIndex in the group
        //                    int max = _minZIndex;
        //                    int count = this.items.Count;
        //                    NativeMethods.IHTMLStyle[] styles = new NativeMethods.IHTMLStyle[count];
        //                    int[] zIndexes = new int[count];
        //                    for (int i = 0; i < count; i++) {
        //                        NativeMethods.IHTMLElement elem = (NativeMethods.IHTMLElement)this.items[i];
        //                        styles[i] = elem.GetStyle();
        //                        zIndexes[i] = (int)styles[i].GetZIndex();
        //                        if (zIndexes[i] > max) {
        //                            max = zIndexes[i];
        //                        }
        //                    }
        //                    //Calculate how far the first element has to be moved in order to be in the back
        //                    int offset = max - (_minZIndex - 1);
        //                    BatchedUndoUnit unit = this.control.OpenBatchUndo("Align Left");
        //                    try {
        //                        //Then send all items in the selection that far back
        //                        for (int i = 0; i < count; i++) {
        //                            int newPos = zIndexes[i] - offset;
        //                            if (zIndexes[i] == _maxZIndex) {
        //                                _maxZIndex--;
        //                            }
        //                            styles[i].SetZIndex(newPos);
        //                            if (newPos < _minZIndex) {
        //                                _minZIndex = newPos;
        //                            }
        //                        }
        //                    }
        //                    catch (Exception e) {
        //                        System.Windows.Forms.MessageBox.Show(e.ToString(),"Exception");
        //                    }
        //                    finally {
        //                        unit.Close();
        //                    }
        //                }
        //                else {
        //                    NativeMethods.IHTMLElement elem = (NativeMethods.IHTMLElement)this.items[0];
        //                    object zIndex = elem.GetStyle().GetZIndex();
        //                    if ((zIndex != null) && !(zIndex is DBNull)) {
        //                        if ((int)zIndex == _minZIndex) {
        //                            // if the element is already in the back do nothing.
        //                            return;
        //                        }
        //
        //                        if ((int)zIndex == _maxZIndex) {
        //                            _maxZIndex--;
        //                        }
        //                    }
        //                    elem.GetStyle().SetZIndex(--_minZIndex);
        //                }
        //            }
        //        }

        public void SetOuterHtml(string outerHtml) 
        {
            Debug.Assert(Items.Count == 1, "Can't get OuterHtml of more than one element");
			NativeMethods.IHTMLElement element = (NativeMethods.IHTMLElement) this.items[0];
            element.SetOuterHTML(outerHtml);
        }

        /// <summary>Synchronizes the selection state held in this object with the selection state in MSHTML.</summary>
        /// <returns>true if the selection has changed</returns>
        public bool SynchronizeSelection() 
        {
            if (this.document == null) 
            {
                this.document = this.control.HtmlDocument;
            }

            NativeMethods.IHTMLSelectionObject selection = this.document.GetSelection();
			object currentSelection = null;
            try 
            {
                currentSelection = selection.CreateRange();
            }
            catch 
            {
            }

            ArrayList oldItems = this.items;
            HtmlSelectionType oldType = this.type;
            int oldLength = this.selectionLength;

			// Default to an empty selection
            this.type = HtmlSelectionType.Empty;
            this.selectionLength = 0;
            if (currentSelection != null) 
            {
                this.selection = currentSelection;
                this.items = new ArrayList();
                //If it's a text selection
                if (currentSelection is NativeMethods.IHTMLTxtRange) 
                {
                    NativeMethods.IHTMLTxtRange textRange = (NativeMethods.IHTMLTxtRange) currentSelection;
                    NativeMethods.IHTMLElement parentElement = textRange.ParentElement();

					// If the document is in full document mode or we're selecting a non-body tag, allow it to select
					// otherwise, leave the selection as empty (since we don't want the body tag to be selectable on an ASP.NET User Control)
                    if (this.IsSelectableElement(new HtmlElement(parentElement, this.control))) 
                    {
                        // Add the parent of the text selection
                        if (parentElement != null) 
                        {
                            this.text = textRange.GetText();
                            if (this.text != null) 
                            {
                                this.selectionLength = this.text.Length;
                            }
                            else 
                            {
                                this.selectionLength = 0;
                            }
                            this.type = HtmlSelectionType.TextSelection;
                            this.items.Add(parentElement);
                        }
                    }
                }
				// If it is a control selection
                else if (currentSelection is NativeMethods.IHtmlControlRange) 
                {
                    NativeMethods.IHtmlControlRange controlRange = (NativeMethods.IHtmlControlRange) currentSelection;
                    int selectedCount = controlRange.GetLength();
                    
					// Add all elements selected
                    if (selectedCount > 0) 
                    {
                        this.type = HtmlSelectionType.ElementSelection;
                        for (int i = 0; i < selectedCount; i++) 
                        {
                            NativeMethods.IHTMLElement currentElement = controlRange.Item(i);
                            this.items.Add(currentElement);
                        }
                        this.selectionLength = selectedCount;
                    }
                }
            }
            
			this.sameParentValid = false;

            bool selectionChanged = false;

			//Now check if there was a change of selection
            //If the two selections have different lengths, then the selection has changed
            if (this.type != oldType) 
            {
                selectionChanged = true;
            }
            else if (this.selectionLength != oldLength) 
            {
                selectionChanged = true;
            }
            else 
            {
                if (this.items != null) 
                {
                    //If the two selections have a different element, then the selection has changed
                    for (int i = 0; i < this.items.Count; i++) 
                    {
                        if (this.items[i] != oldItems[i]) 
                        {
                            selectionChanged = true;
                            break;
                        }
                    }
                }
            }

            if (selectionChanged) 
            {
                //Set this.elements to null so no one can retrieve a dirty copy of the selection element wrappers
                this.elements = null;

                OnSelectionChanged(EventArgs.Empty);
                return true;
            }

            return false;
        }

		/// <summary>Toggle the absolute positioning state of the selected items.</summary>
        public void ToggleAbsolutePosition() 
        {
			this.control.Execute(NativeMethods.IDM_ABSOLUTE_POSITION, new object[] { !this.control.IsChecked(NativeMethods.IDM_ABSOLUTE_POSITION) });
            SynchronizeSelection();
            if (this.type == HtmlSelectionType.ElementSelection) 
            {
                foreach (NativeMethods.IHTMLElement elem in this.items) 
                {
                    elem.GetStyle().SetZIndex(++this.maxZIndex);
                }
            }
        }

        /// <summary>Toggle the design time lock state of the selected items.</summary>
        public void ToggleLock() 
        {
            //Switch the lock on each item
            foreach (NativeMethods.IHTMLElement element in this.items) 
            {
                NativeMethods.IHTMLStyle style = element.GetStyle();
                if (IsElementLocked(element)) 
                {
                    // We need to remove attributes off the element and the style because of a bug in Trident
                    element.RemoveAttribute(DesignTimeLockAttribute, 0);
                    style.RemoveAttribute(DesignTimeLockAttribute, 0);
                }
                else 
                {
                    //We need to add attributes to the element and the style because of a bug in Trident
                    element.SetAttribute(DesignTimeLockAttribute, "true", 0);
                    style.SetAttribute(DesignTimeLockAttribute, "true", 0);
                }
            }
        }

        public void WrapSelection(string tag) 
        {
            this.WrapSelection(tag, null);
        }

        public void WrapSelection(string elementName, IDictionary attributes) 
        {
            //Create a string for all the attributes
            string attributeString = String.Empty;
            if (attributes != null) 
            {
                foreach (string key in attributes.Keys) 
                {
                    attributeString += key + "=\"" + attributes[key] + "\" ";
                }
            }
            this.SynchronizeSelection();
            if (this.type == HtmlSelectionType.TextSelection) 
            {
                NativeMethods.IHTMLTxtRange textRange = (NativeMethods.IHTMLTxtRange)this.Selection;
                string oldText = textRange.GetHtmlText();
                if (oldText == null) 
                {
                    oldText = string.Empty;
                }

				string newText = "<" + elementName + " " + attributeString + ">" + oldText + "</" + elementName + ">";
                textRange.PasteHTML(newText);
            }
        }

        public void WrapSelectionInDiv() 
        {
            WrapSelection("div");
        }

        public void WrapSelectionInSpan() 
        {
            WrapSelection("span");
        }

        public void WrapSelectionInBlockQuote() 
        {
            WrapSelection("blockquote");
        }

        public void WrapSelectionInHyperlink(string url) 
        {
			this.control.Execute(NativeMethods.IDM_HYPERLINK, new object[] { url });
        }

		public bool CanRemoveHyperlink
		{
			get { return this.control.IsEnabled(NativeMethods.IDM_UNLINK); }
		}

        public void RemoveHyperlink() 
        {
            this.control.Execute(NativeMethods.IDM_UNLINK);
        }
    }
}
