using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a .ico file.
    /// </summary>
    internal class IconFile
    {
        /// <summary>
        /// Resource type.
        /// </summary>
        internal enum GroupType
        {
            /// <summary>
            /// Icon.
            /// </summary>
            Icon = 1,
            /// <summary>
            /// Cursor.
            /// </summary>
            Cursor = 2
        }

        Kernel32.FILEGRPICONDIR _header = new Kernel32.FILEGRPICONDIR();
        List<IconFileIcon> _icons = new List<IconFileIcon>();

        /// <summary>
        /// Type of the group icon resource.
        /// </summary>
        internal GroupType Type
        {
            get
            {
                return (GroupType)_header.wType;
            }
            set
            {
                _header.wType = (byte)value;
            }
        }

        /// <summary>
        /// Collection of icons in an .ico file.
        /// </summary>
        internal List<IconFileIcon> Icons
        {
            get
            {
                return _icons;
            }
            set
            {
                _icons = value;
            }
        }

        /// <summary>
        /// An existing .ico file.
        /// </summary>
        /// <param name="filename">An existing icon (.ico) file.</param>
        internal IconFile(string filename)
        {
            LoadFrom(filename);
        }

        internal IconFile(IconDirectoryResource res)
        {
            _icons.Clear();

            _header = new Kernel32.FILEGRPICONDIR();
            _header.wCount = res._header.wImageCount;
            _header.wReserved = res._header.wReserved;
            _header.wType = res._header.wType;

            for (int i = 0; i < _header.wCount; i++)
            {
                _icons.Add(new IconFileIcon(res.Icons[0]));
            }
        }

        /// <summary>
        /// Load from a .ico file.
        /// </summary>
        /// <param name="filename">An existing icon (.ico) file.</param>
        internal void LoadFrom(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);

            IntPtr lpData = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, lpData, data.Length);
                Read(lpData);
            }
            finally
            {
                Marshal.FreeHGlobal(lpData);
            }
        }

        /// <summary>
        /// Read icons.
        /// </summary>
        /// <param name="lpData">Pointer to the beginning of a FILEGRPICONDIR structure.</param>
        /// <returns>Pointer to the end of a FILEGRPICONDIR structure.</returns>
        internal IntPtr Read(IntPtr lpData)
        {
            _icons.Clear();

            _header = (Kernel32.FILEGRPICONDIR)Marshal.PtrToStructure(
                lpData, typeof(Kernel32.FILEGRPICONDIR));

            IntPtr lpEntry = new IntPtr(lpData.ToInt32() + Marshal.SizeOf(_header));

            for (int i = 0; i < _header.wCount; i++)
            {
                IconFileIcon iconFileIcon = new IconFileIcon();
                lpEntry = iconFileIcon.Read(lpEntry, lpData);
                _icons.Add(iconFileIcon);
            }

            return lpEntry;
        }
    }
}
