using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace EinarEgilsson.Utilities.Win32Icons
{
    interface ICommand
    {
        string Name { get; }
        string HelpText { get; }
        void Execute(string filename, string[] args);
    }
}
