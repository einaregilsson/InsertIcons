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
        internal IconFile(byte[] data)
        {
            LoadFrom(data);
        }

        /// <summary>
        /// Load from a .ico file.
        /// </summary>
        /// <param name="bytes">A byte array containing an icon (.ico) file.</param>
        internal void LoadFrom(byte[] bytes)
        {

            IntPtr lpData = Marshal.AllocHGlobal(bytes.Length);
            try
            {
                Marshal.Copy(bytes, 0, lpData, bytes.Length);
                Read(lpData);
            }
            finally
            {
                Marshal.FreeHGlobal(lpData);
            }
        }   
        /// <summary>
        /// Load from a .ico file.
        /// </summary>
        /// <param name="filename">An existing icon (.ico) file.</param>
        internal void LoadFrom(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);
            LoadFrom(data);
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
