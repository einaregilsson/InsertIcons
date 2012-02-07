using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //var info = new ResourceInfo();
            //info.Load(filename);

            //if (info.Resources.ContainsKey(new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON)))
            //{
            //    foreach (IconDirectoryResource groupIcon in info.Resources[new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON)])
            //    {
            //        if ((int)groupIcon.Name.Id > (int)groupIconIdCounter)
            //        {
            //            groupIconIdCounter = groupIcon.Name.Id;
            //        }
            //        foreach (var icon in groupIcon.Icons)
            //        {
            //            iconIdCounter = Math.Max(iconIdCounter, icon.Id);
            //        }
            //    }
            //}

        }
    }
}
