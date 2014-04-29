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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Security;
using Vestris.ResourceLib;

namespace CreateWindowsIcons
{
    public class WindowsIconsCreator
    {
        //Start here because if the user has selected an app icon in VS then
        //it will get the id 32512, which is some kind of special id for app
        //icons. Explorer shows the first icon in the file by default, so
        //we'll give our inserted icons names that are higher than 32512 so
        //the originally selected icon is the first one in the file.
        private const int StartIconId = 40000;

        public void CreateIcons(string assemblyFile, string strongNameKeyFile)
        {
            if (!File.Exists(assemblyFile))
            {
                throw new FileNotFoundException("The file " + assemblyFile + " doesn't exist!");
            }

            Assembly assembly = Assembly.ReflectionOnlyLoad(File.ReadAllBytes(assemblyFile));
            Dictionary<string, byte[]> iconFiles = GetIconFiles(assembly);


            //Verify that the assemblyFile is signed to begin with. We don't support signing unsigned assemblies,
            //only re-signing them.
            if (!string.IsNullOrEmpty(strongNameKeyFile))
            {
                if (assembly.GetName().GetPublicKey().Length == 0)
                {
                    throw new ArgumentException("Assembly is not strong named, CreateWindowsIcons can only re-sign assemblies, not sign unsigned assemblies.");
                }
            }

            ushort iconMaxId = GetMaxIconId(assemblyFile);

            int groupIconIdCounter = StartIconId;
            foreach (string icoFile in iconFiles.Keys)
            {
                var iconData = iconFiles[icoFile];
                groupIconIdCounter++;
                IconDirectoryResource newIcon = new IconDirectoryResource(new IconFile(iconData));
                newIcon.Name.Id = new IntPtr(groupIconIdCounter);
                foreach (var icon in newIcon.Icons)
                {
                    icon.Id = ++iconMaxId;
                }
                Console.WriteLine(" {0} {1} created in {2}", newIcon.Name.Id, Path.GetFileName(icoFile), Path.GetFileName(assemblyFile));
                newIcon.SaveTo(assemblyFile);
            }

            if (strongNameKeyFile != null)
            {
                ResignAssembly(assemblyFile, strongNameKeyFile);
            }
            Console.WriteLine("Successfully create {0} Windows icons in {1}", iconFiles.Count, Path.GetFileName(assemblyFile));
        }

        /// <summary>
        /// Re-signs the assemblyFile with a strong key after the icons have been inserted.
        /// Throws an error if the assemblyFile wasn't signed before, we don't handle signing
        /// for the first time, only re-signing.
        /// </summary>
        private static void ResignAssembly(string assemblyFile, string strongNameKeyFile)
        {
            new StrongName(File.ReadAllBytes(strongNameKeyFile)).Sign(assemblyFile);
        }

        /// <summary>
        /// Get the max icon id currently in the assemblyFile so we don't overwrite
        /// the existing icons with our new icons
        /// </summary>
        private static ushort GetMaxIconId(string assemblyFile)
        {
            using (var info = new ResourceInfo())
            {
                info.Load(assemblyFile);

                ResourceId groupIconId = new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON);
                if (info.Resources.ContainsKey(groupIconId))
                {
                    return info.Resources[groupIconId].OfType<IconDirectoryResource>().Max(idr => idr.Icons.Max(icon => icon.Id));
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the number of icons currently in the file
        /// </summary>
        public int GetIconCount(string assemblyFile)
        {
            using (var info = new ResourceInfo())
            {
                info.Load(assemblyFile);

                ResourceId groupIconId = new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON);
                if (info.Resources.ContainsKey(groupIconId))
                {
                    return info.Resources[groupIconId].OfType<IconDirectoryResource>().Count();
                }
            }
            return 0;
        }

        /// <summary>
        /// Read .ico files from managed resources in an assemblyFile file
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, byte[]> GetIconFiles(Assembly assembly)
        {
            var result = new Dictionary<string, byte[]>();
            var iconResources = assembly.GetManifestResourceNames().Where(name => name.EndsWith((".ico")));
            foreach (var iconResource in iconResources)
            {
                using (var stream = assembly.GetManifestResourceStream(iconResource))
                {
                    Debug.Assert(stream != null);
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    result.Add(iconResource, bytes);
                }
            }
            return result;
        }
    }
}
