using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestris.ResourceLib;

namespace EinarEgilsson.Utilities.Win32Icons
{
    public class WinIconAssembly
    {
        public string Filename { get; private set; }

        public WinIconAssembly(string assemblyFilename)
        {
            Filename = assemblyFilename;
        }

        public void AddIcon(string iconFilename)
        {
            IconDirectoryResource res = new IconDirectoryResource(new IconFile(iconFilename));
            res.SaveTo(Filename);
        }
    }
}
