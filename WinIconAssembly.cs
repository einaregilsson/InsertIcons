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
        private ushort iconIdCounter;
        private IntPtr groupIconIdCounter;
        public WinIconAssembly(string assemblyFilename)
        {
            Filename = assemblyFilename;
            var info = new ResourceInfo();
            info.Load(Filename);
            
            if (info.Resources.ContainsKey(new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON))) {
                foreach (IconDirectoryResource groupIcon in info.Resources[new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON)])
                {
                    if ((int)groupIcon.Name.Id > (int)groupIconIdCounter) {
                        groupIconIdCounter = groupIcon.Name.Id;
                    }
                    foreach (var icon in groupIcon.Icons)
                    {
                        iconIdCounter= Math.Max(iconIdCounter, icon.Id);
                    }
                }
            }
        }

        public void AddIcon(string iconFilename)
        {
            IconDirectoryResource res = new IconDirectoryResource(new IconFile(iconFilename));
            res.Name.Id = (IntPtr)((int)groupIconIdCounter + 1);
            res.Name.Name = res.Name.Id.ToString();
            foreach (var icon in res.Icons)
            {
                icon.Id = ++iconIdCounter;
            }
            res.SaveTo(Filename);
        }
    }
}
