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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Vestris.ResourceLib;
using System.IO;
using Mono.Security;

[assembly: AssemblyTitle("InsertIcons")]
[assembly: AssemblyDescription("Add multiple win32 icons to .NET assembly files")]
[assembly: AssemblyProduct("InsertIcons")]
[assembly: AssemblyCopyright("Copyright © 2012 Einar Egilsson")]
[assembly: ComVisible(false)]
[assembly: Guid("56c67123-d4f9-4baf-910e-b4c8a1b7fd7d")]
[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1.0.0")]


namespace InsertIcons
{
    public class Program
    {
        //Start here because if the user has selected an app icon in VS then
        //it will get the id 32512, which is some kind of special id for app
        //icons. Explorer shows the first icon in the file by default, so
        //we'll give our inserted icons names that are higher than 32512 so
        //the originally selected icon is the first one in the file.
        private const int StartIconId = 40000;

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

                if (!File.Exists(assembly))
                {
                    throw new FileNotFoundException("The file " + args[0] + " doesn't exist!");
                }
                List<string> iconFiles = GetIconFiles(args);
                VerifyIconFiles(iconFiles);


                string strongNameKeyFile = args.Length > 2 ? args[2] : null;
                //Verify that the assembly is signed to begin with. We don't support signing unsigned assemblies,
                //only re-signing them.
                if (strongNameKeyFile != null)
                {
                    using (var stream = new FileStream(assembly, FileMode.Open, FileAccess.Read)) {
                        var signature = new StrongName().StrongHash(stream, StrongName.StrongNameOptions.Signature);
                        if (signature.SignaturePosition == 0 && signature.SignatureLength == 0)
                        {
                            throw new ArgumentException("Assembly is not strong named, InsertIcons can only re-sign assemblies, not sign unsigned assemblies." );
                        }
                    }
                }
                ushort iconMaxId = GetMaxIconId(assembly);

                int groupIconIdCounter = StartIconId;
                foreach (string icoFile in iconFiles)
                {
                    groupIconIdCounter++;
                    IconDirectoryResource newIcon = new IconDirectoryResource(new IconFile(icoFile));
                    newIcon.Name.Id = new IntPtr(groupIconIdCounter);
                    foreach (var icon in newIcon.Icons)
                    {
                        icon.Id = ++iconMaxId;
                    }
                    Console.WriteLine(" {0} {1} inserted into {2}", newIcon.Name.Id, Path.GetFileName(icoFile), Path.GetFileName(assembly));
                    newIcon.SaveTo(assembly);
                }

                if (strongNameKeyFile != null)
                {
                    ResignAssembly(assembly, strongNameKeyFile);
                }
                Console.WriteLine("Successfully inserted {0} icons into {1}", iconFiles.Count, Path.GetFileName(assembly));
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("error: {0}", ex.Message);
                return 1;
            }

        }

        /// <summary>
        /// Re-signs the assembly with a strong key after the icons have been inserted.
        /// Throws an error if the assembly wasn't signed before, we don't handle signing
        /// for the first time, only re-signing.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="strongNameKey"></param>
        private static void ResignAssembly(string assembly, string strongNameKey)
        {
            new StrongName(File.ReadAllBytes(strongNameKey)).Sign(assembly);
        }

        /// <summary>
        /// Get the max icon id currently in the assembly so we don't overwrite
        /// the existing icons with our new icons
        /// </summary>
        private static ushort GetMaxIconId(string assembly)
        {
            using (var info = new ResourceInfo())
            {
                info.Load(assembly);

                ResourceId groupIconId = new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON);
                if (info.Resources.ContainsKey(groupIconId))
                {
                    return info.Resources[groupIconId].OfType<IconDirectoryResource>().Max(idr => idr.Icons.Max(icon => icon.Id));
                }
            }
            return 0;
        }

        private static void VerifyIconFiles(List<string> iconFiles)
        {
            foreach (string icoFile in iconFiles)
            {
                if (!File.Exists(icoFile))
                {
                    throw new FileNotFoundException("The file " + icoFile + " doesn't exist!");
                }
                else if (!icoFile.ToLower().EndsWith(".ico"))
                {
                    throw new ArgumentException("The file " + icoFile + " is not an icon file!");
                }
            }
        }

        /// <summary>
        /// Parse the 'icons' parameter and return a list of .ico files.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static List<string> GetIconFiles(string[] args)
        {
            if (args.Length == 1)
            {
                return Console.In.ReadToEnd().Split('\n').Select(l => l.Trim()).Where(l => l != "").ToList();
            }

            string param = args[1];
            if (param.ToLower().EndsWith(".ico") || param.Contains(';'))
            {
                return param.Split(';').ToList();
            }

            if (Directory.Exists(param))
            {
                return Directory.GetFiles(param, "*.ico").OrderBy(s => s).ToList();
            }

            if (File.Exists(param))
            {
                var list = File.ReadAllLines(param).Select(l => l.Trim()).Where(l => l != "").ToList();
                string listFileFolder = Path.GetDirectoryName(param);
                string currentDirectory = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(listFileFolder);
                for (int i = 0; i < list.Count; i++)
                {
                    string iconFile = list[i];
                    if (!Path.IsPathRooted(iconFile))
                    {
                        list[i] = Path.GetFullPath(iconFile);
                    }
                }
                Directory.SetCurrentDirectory(currentDirectory);
                return list;
            }
            throw new ArgumentException("Invalid icon argument: " + param);
        }


        private static void PrintUsage()
        {
            Console.WriteLine(@"
InsertIcons  Copyright (C) 2012 Einar Egilsson

See http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/ 
for more information about this program and how to use it.

Usage: InsertIcons <assemblyfile> <icons> [<keyfile>]

<assemblyfile>    A .NET assembly (or any PE file really) that you want
                  to add icons to.

<icons>           The <icons> parameter can accept 4 different types
                  of arguments:
                  
                  1. It can be a list of .ico files seperated by ;, e.g.
                     icon1.ico;icon2.ico;icon3.ico.
                     
                  2. It can be a directory name, in which case all .ico
                     files in the directory will be added, in 
                     alphabetical order.

                  3. It can be the name of a text file which contains a
                     list of icons to add to the assembly. The file
                     should simply have one .ico file path on each line.
   
                  4. It can be left out completely, in which case the
                     program reads filenames from the standard input
                     stream. This allows for filenames to be piped into
                     the program, e.g. 
                          
                         dir /b /s *.ico | InsertIcons myfile.exe

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
