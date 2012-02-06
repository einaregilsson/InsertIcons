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
            string file = args[0];
            var winIconFile = new WinIconAssembly(file);
            winIconFile.AddIcon(args[1]);
        }
    }
}
