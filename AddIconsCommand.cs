using System;
using System.Linq;
using System.ComponentModel.Composition;
using Vestris.ResourceLib;

namespace EinarEgilsson.Utilities.Win32Icons
{
    [Export(typeof(ICommand))]
    public class AddIconsCommand : ICommand
    {
        public string Name { get { return "add"; } }
        
        public string HelpText { get; set; }
        
        public void Execute(string filename, string[] args)
        {
            ushort iconMaxId = 0;
            int groupIconMaxId = 0;
            using (var info = new ResourceInfo())
            {
                info.Load(filename);

                ResourceId groupIconId = new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON);
                if (info.Resources.ContainsKey(groupIconId))
                {
                    iconMaxId = info.Resources[groupIconId].OfType<IconDirectoryResource>().Max(idr => idr.Icons.Max(icon => icon.Id));
                    groupIconMaxId = info.Resources[groupIconId].OfType<IconDirectoryResource>().Max(ir => (int)ir.Name.Id);
                    foreach (IconDirectoryResource r in info.Resources[groupIconId])
                    {
                        IntPtr s = r.Name.Id;
                    }
                }

            }

            foreach (string icoFile in args)
            {
                IconDirectoryResource newIcon = new IconDirectoryResource(new IconFile(icoFile));
                groupIconMaxId++;
                newIcon.Name.Id = (IntPtr) groupIconMaxId;
                newIcon.Name.Name = newIcon.Name.Id.ToString();

                foreach (var icon in newIcon.Icons)
                {
                    icon.Id = ++iconMaxId;
                }
                newIcon.SaveTo(filename);
            }
        }
    }
}
