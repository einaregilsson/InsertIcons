using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestris.ResourceLib;
using System.Reflection;
using System.IO;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace EinarEgilsson.Utilities.Win32Icons
{
    class Program
    {
        static int Main(string[] args)
        {


            return new Program().Run(args);
        }
        public int Run(string[] args)
        {
            Compose();
            if (args.Length == 0)
            {
                PrintUsage();
                return 0;
            }

            ICommand cmd = Commands.FirstOrDefault(c => c.Name == args[0]);
            if (cmd == null)
            {
                Console.Error.WriteLine("error: Unknown command: {0}. Type win32icons help for more information", args[0]);
                return 1;
            }

            if (args.Length == 1)
            {
                Console.WriteLine("error: Missing filename argument. Type win32icons help for more information");
                return 1;
            }
            try
            {
                cmd.Execute(args[1], args.Skip(2).ToArray());
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("error: {0}", ex.Message);
                return 1;
            }
        }

        [ImportMany]
        private List<ICommand> Commands { get; set; }

        private void PrintUsage()
        {
            Console.WriteLine(@"
Usage: win32icons <command> <filename> [arg1, arg2, ..., argn]

Type win32icons help for a list of commands and their arguments.
");
        }

        private void Compose()
        {
            var first = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(first);
            container.ComposeParts(this);
            Commands.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
        }
    }
}
