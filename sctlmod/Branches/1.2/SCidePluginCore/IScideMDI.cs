using System;
using System.Collections.Generic;
using System.Text;
using Scintilla.Forms;
using Scintilla.Configuration;
using Scintilla.Configuration.SciTE;
using WeifenLuo.WinFormsUI.Docking;

namespace SCide
{
    public interface IScideMDI
    {
        FindForm FindDialog { get; }
        ReplaceForm ReplaceDialog { get; }
        IScintillaConfig Configuration { get; }
        DockPanel DockPanel { get; }
        //SciTEProperties Properties { get; }
    }
}
