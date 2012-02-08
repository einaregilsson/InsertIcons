using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestris.ResourceLib;
using System.Reflection;
using System.IO;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace EinarEgilsson.Utilities.InsertIcons
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }
            try
            {
                string assembly = args[0];
                string[] iconFiles = args.Skip(1).ToArray();
                ushort iconMaxId = 0;
                int maxGroupIconId = 0;
                using (var info = new ResourceInfo())
                {
                    info.Load(assembly);

                    ResourceId groupIconId = new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON);
                    if (info.Resources.ContainsKey(groupIconId))
                    {
                        iconMaxId = info.Resources[groupIconId].OfType<IconDirectoryResource>().Max(idr => idr.Icons.Max(icon => icon.Id));
                        maxGroupIconId = info.Resources[groupIconId].OfType<IconDirectoryResource>().Max(idr => int.Parse(idr.Name.Name));
                    }
                }
                foreach (string icoFile in iconFiles)
                {
                    maxGroupIconId++;
                    while (IsSystemIconId(maxGroupIconId))
                    {
                        maxGroupIconId++;
                    }
                    IconDirectoryResource newIcon = new IconDirectoryResource(new IconFile(icoFile));
                    newIcon.Name.Id = new IntPtr(maxGroupIconId);
                    foreach (var icon in newIcon.Icons)
                    {
                        icon.Id = ++iconMaxId;
                    }
                    newIcon.SaveTo(assembly);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("error: {0}", ex.Message);
                return 1;
            }
        }
        private static bool IsSystemIconId(int id) {
            return id >= 32512 && id <= 32518;
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"
Usage: InsertIcons <assemblyfile> [iconfile1 iconfile2 ...]

<assemblyfile>    A .NET assembly (or any PE file really) that you want
                  to add icons to.

[iconfile 1-n]    One or more .ico files that you want to insert into
                  the assembly.
");
        }
    }
}
