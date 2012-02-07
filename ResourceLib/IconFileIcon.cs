using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of icon data in a .ico file.
    /// </summary>
    internal class IconFileIcon 
    {
        private Kernel32.FILEGRPICONDIRENTRY _header;
        private DeviceIndependentBitmap _image = new DeviceIndependentBitmap();

        /// <summary>
        /// Icon header.
        /// </summary>
        internal Kernel32.FILEGRPICONDIRENTRY Header
        {
            get
            {
                return _header;
            }
        }

        /// <summary>
        /// Icon bitmap.
        /// </summary>
        internal DeviceIndependentBitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        /// <summary>
        /// New icon data.
        /// </summary>
        internal IconFileIcon()
        {

        }

        internal IconFileIcon(IconImageResource res)
        {
            _header.bColors = res._header.bColors;
            _header.bHeight = res._header.bHeight;
            _header.bReserved = res._header.bReserved;
            _header.bWidth = res._header.bWidth;
            _header.wBitsPerPixel = res._header.wBitsPerPixel;
            _header.dwImageSize = res._header.dwImageSize;
            _header.wPlanes = res._header.wPlanes;
            _image = new DeviceIndependentBitmap(res.Image);
        }

        /// <summary>
        /// Icon width.
        /// </summary>
        internal Byte Width
        {
            get
            {
                return _header.bWidth;
            }
        }

        /// <summary>
        /// Icon height.
        /// </summary>
        internal Byte Height
        {
            get
            {
                return _header.bHeight;
            }
        }

        /// <summary>
        /// Image size in bytes.
        /// </summary>
        internal UInt32 ImageSize
        {
            get
            {
                return _header.dwImageSize;
            }
        }

        /// <summary>
        /// Read a single icon (.ico).
        /// </summary>
        /// <param name="lpData">Pointer to the beginning of this icon's data.</param>
        /// <param name="lpAllData">Pointer to the beginning of all icon data.</param>
        /// <returns>Pointer to the end of this icon's data.</returns>
        internal IntPtr Read(IntPtr lpData, IntPtr lpAllData)
        {
            _header = (Kernel32.FILEGRPICONDIRENTRY)Marshal.PtrToStructure(
                lpData, typeof(Kernel32.FILEGRPICONDIRENTRY));

            IntPtr lpImage = new IntPtr(lpAllData.ToInt32() + _header.dwFileOffset);
            _image.Read(lpImage, _header.dwImageSize);

            return new IntPtr(lpData.ToInt32() + Marshal.SizeOf(_header));
        }


        /// <summary>
        /// Icon size as a string.
        /// </summary>
        /// <returns>Icon size in the width x height format.</returns>
        public override string ToString()
        {
            return string.Format("{0}x{1}", Width, Height);
        }
    }
}
