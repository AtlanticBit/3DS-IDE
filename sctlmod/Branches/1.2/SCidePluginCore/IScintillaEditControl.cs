using System;
using System.Collections.Generic;
using System.Text;
using Scintilla;
using Scintilla.Forms;
using Scintilla.Configuration;
using Scintilla.Configuration.SciTE;

namespace SCide
{
    public interface IScintillaEditControl
    {
        ScintillaControl ScintillaEditor { get; }
    }
}
