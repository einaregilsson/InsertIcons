/*
MIT License

CreateWindowsIcons, a program to add multiple icons to .NET applications. 

Copyright (c) 2012 Einar Egilsson
http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/
 
This program includes other MIT licensed code from the following projects:

   * ResourceLib (https://github.com/dblock/resourcelib), to insert the icons.
   * Mono (http://mono-project.com), for strong name signing assemblies.

You may freely use and distribute this software under the terms of the following license agreement.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Text.RegularExpressions;

namespace CreateWindowsIcons
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0 || args.Length == 1 && Regex.IsMatch(args[0], @"^/(\?|h|help)$"))
            {
                PrintUsage();
                return -1;
            }

            try
            {
                string assembly = args[0];
                string strongNameKeyFile = args.Length > 1 ? args[1] : null;

                new WindowsIconsCreator().CreateIcons(assembly, strongNameKeyFile);
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("error: {0}", ex.Message);
                return 1;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"
CreateWindowsIcons  Copyright (C) 2012 Einar Egilsson

See http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/ 
for more information about this program and how to use it.

Usage: CreateWindowsIcons <assemblyfile> [<keyfile>]

<assemblyfile>    A .NET assembly (.exe or .dll) that you want to add
                  Windows icons to. Any .ico files that have been embedded
                  as managed resources to the assembly will be converted
                  to Windows icons that can be used for shortcuts, jump
                  lists and other things.

<keyfile>         This parameter is optional. If it is included then the
                  assembly will be re-signed with a strong name after 
                  inserting the icons.
                  
                  Note that this should only be used to re-sign
                  assemblies that were signed before inserting icons
                  into them. If you pass in the <keyfile> parameter for
                  a file that was not signed before then the program 
                  will exit with an error message.
");
        }
    }
}
