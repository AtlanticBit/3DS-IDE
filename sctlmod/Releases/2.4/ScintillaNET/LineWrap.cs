#region Using Directives

using System;
using System.ComponentModel;

#endregion Using Directives


namespace ScintillaNET
{
    [TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public class LineWrap : TopLevelHelper
    {
        #region Methods

        private void ResetLayoutCache()
        {
            LayoutCache = LineCache.Caret;
        }


        private void ResetMode()
        {
            Mode = WrapMode.None;
        }


        private void ResetPositionCacheSize()
        {
            PositionCacheSize = 1024;
        }


        private void ResetStartIndent()
        {
            StartIndent = 0;
        }


        private void ResetVisualFlags()
        {
            VisualFlags = WrapVisualFlag.None;
        }


        private void ResetVisualFlagsLocation()
        {
            VisualFlagsLocation = WrapVisualLocation.Default;
        }


        internal bool ShouldSerialize()
        {
            return ShouldSerializeLayoutCache() ||
                ShouldSerializePositionCacheSize() ||
                ShouldSerializeStartIndent() ||
                ShouldSerializeVisualFlags() ||
                ShouldSerializeVisualFlagsLocation() ||
                ShouldSerializeMode();
        }


        private bool ShouldSerializeLayoutCache()
        {
            return LayoutCache != LineCache.Caret;
        }


        private bool ShouldSerializeMode()
        {
            return Mode != WrapMode.None;
        }


        private bool ShouldSerializePositionCacheSize()
        {
            return PositionCacheSize != 1024;
        }


        private bool ShouldSerializeStartIndent()
        {
            return StartIndent != 0;
        }


        private bool ShouldSerializeVisualFlags()
        {
            return VisualFlags != WrapVisualFlag.None;
        }


        private bool ShouldSerializeVisualFlagsLocation()
        {
            return VisualFlagsLocation != WrapVisualLocation.Default;
        }

        #endregion Methods


        #region Properties

        public LineCache LayoutCache
        {
            get
            {
                return (LineCache)NativeScintilla.GetLayoutCache();
            }
            set
            {
                NativeScintilla.SetLayoutCache((int)value);
            }
        }


        public WrapMode Mode
        {
            get
            {
                return (WrapMode)NativeScintilla.GetWrapMode();
            }
            set
            {
                NativeScintilla.SetWrapMode((int)value);
            }
        }


        public int PositionCacheSize
        {
            get
            {
                return NativeScintilla.GetPositionCache();
            }
            set
            {
                NativeScintilla.SetPositionCache(value);
            }
        }


        public int StartIndent
        {
            get
            {
                return NativeScintilla.GetWrapStartIndent();
            }
            set
            {
                NativeScintilla.SetWrapStartIndent(value);
            }
        }


        [Editor(typeof(ScintillaNET.Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public WrapVisualFlag VisualFlags
        {
            get
            {
                return (WrapVisualFlag)NativeScintilla.GetWrapVisualFlags();
            }
            set
            {
                NativeScintilla.SetWrapVisualFlags((int)value);
            }
        }


        [Editor(typeof(ScintillaNET.Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public WrapVisualLocation VisualFlagsLocation
        {
            get
            {
                return (WrapVisualLocation)NativeScintilla.GetWrapVisualFlagsLocation();
            }
            set
            {
                NativeScintilla.SetWrapVisualFlagsLocation((int)value);
            }
        }

        #endregion Properties


        #region Constructors

        internal LineWrap(Scintilla scintilla) : base(scintilla) { }

        #endregion Constructors
    }
}
