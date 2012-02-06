using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestris.ResourceLib;
using System.Reflection;
using System.IO;

namespace EinarEgilsson.Utilities.Win32Icons
{
    class Program
    {
        static void Main(string[] args)
        {
            //string file = args[0];
            var winIconFile = new WinIconAssembly(args[0]);
            string newIcon = Path.Combine(Path.GetDirectoryName(args[0]), "icons", "cmd.ico");

            winIconFile.AddIcon(newIcon);
            newIcon = Path.Combine(Path.GetDirectoryName(args[0]), "icons", "sec.ico");

            winIconFile.AddIcon(newIcon);
        }
    }
}
