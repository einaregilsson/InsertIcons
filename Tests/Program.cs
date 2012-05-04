/*
MIT License

InsertIcons, a program to add multiple icons to .NET applications. 

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace InsertIcons.Tests
{
    /// <summary>
    /// Simple test runner for the unit tests.
    /// </summary>
    class Program
    {
        public static int Main()
        {
            TextWriter stdOut = Console.Out;
            TextWriter stdErr = Console.Error;
            int success = 0, fail = 0;
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.GetCustomAttributes(typeof(NUnit.Framework.TestFixtureAttribute), true).Length == 0)
                {
                    continue;
                }
                Console.WriteLine("\r\n" + t.Name);
                foreach (var method in t.GetMethods())
                {
                    if (method.GetCustomAttributes(typeof(NUnit.Framework.TestAttribute), true).Length > 0)
                    {
                        Console.Write("    " + method.Name);
                        try
                        {
                            Console.SetOut(new StringWriter());
                            Console.SetError(new StringWriter());
                            method.Invoke(Activator.CreateInstance(t), new object[0]);
                            Console.SetOut(stdOut);
                            WriteColored("\r    " + method.Name, ConsoleColor.Green);
                            success++;
                        }
                        catch (TargetInvocationException ex)
                        {
                            Console.SetOut(stdOut);
                            WriteColored("\r    " + method.Name, ConsoleColor.Red);
                            Console.WriteLine(ex.InnerException.Message);
                            fail++;
                        }
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("RESULTS: {0} Succeeded, {1} Failed", success, fail);
            Console.ReadKey();
            return 0;
        }

        static void WriteColored(string msg, ConsoleColor color)
        {
            ConsoleColor normal = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = normal;
        }

    }
}
