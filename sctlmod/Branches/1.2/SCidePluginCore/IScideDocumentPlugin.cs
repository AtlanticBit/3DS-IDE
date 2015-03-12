using System;
using System.Collections.Generic;
using System.Text;
using Scintilla.Forms;
using Scintilla.Configuration;
using Scintilla.Configuration.SciTE;
using WeifenLuo.WinFormsUI.Docking;

namespace SCide
{
    public interface IScideDocumentPlugin
    {
        Guid PluginID { get; }
        string Name { get; }
        string Description { get; }

        void Initialize(IScideMDI scideMDI);
        void Finalize(IScideMDI scideMDI);
    }
}
