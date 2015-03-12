#region Using Directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion Using Directives


namespace ScintillaNET
{
    /// <summary>
    ///     Provides data for the <see cref="Scintilla.AnnotationChanged" /> event.
    /// </summary>
    public class AnnotationChangedEventArgs : EventArgs
    {
        #region Fields

        private int _lineIndex;

        #endregion Fields


        #region Properties

        /// <summary>
        ///     Gets the index of the document line containing the changed annotation.
        /// </summary>
        /// <returns>The zero-based index of the document line containing the changed annotation.</returns>
        public int LineIndex
        {
            get
            {
                return _lineIndex;
            }
        }

        #endregion Properties


        #region Consructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AnnotationChangedEventArgs" /> class.
        /// </summary>
        /// <param name="lineIndex">The document line index of the annotation that changed.</param>
        public AnnotationChangedEventArgs(int lineIndex)
        {
            _lineIndex = lineIndex;
        }

        #endregion Constructors
    }
}
