using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestris.ResourceLib;
using System.Reflection;
using System.IO;

namespace ResourceIcons
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = @"..\..\..\IconExe\bin\release\IconExe.exe";
            //IconDirectoryResource r1 = new IconDirectoryResource();
            //r1.LoadFrom(file);
            IconDirectoryResource res = new IconDirectoryResource(new IconFile(@"C:\users\einar\desktop\sec.ico"));
            res.Name.Id = (IntPtr)1;
            res.Name.Name = "John";
            res.SaveTo(file);
            res = new IconDirectoryResource(new IconFile(@"C:\users\einar\desktop\cmd.ico"));
            res.Name.Name = "Foo";
            res.Name.Id = (IntPtr)2;
            res.SaveTo(file);
 
        }
    }
}
