using System;
using System.Text;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Resource utilities.
    /// </summary>
    internal abstract class ResourceUtil
    {
        /// <summary>
        /// Pad data to a WORD.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        /// <returns>New position within the binary stream.</returns>
        internal static long PadToWORD(BinaryWriter w)
        {
            long pos = w.BaseStream.Position;

            if (pos % 2 != 0)
            {
                long count = 2 - pos % 2;
                Pad(w, (UInt16)count);
                pos += count;
            }

            return pos;
        }

        /// <summary>
        /// Write a value at a given position.
        /// Used to write a size of data in an earlier located header.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        /// <param name="value">Value to write.</param>
        /// <param name="address">Address to write the value at.</param>
        internal static void WriteAt(BinaryWriter w, long value, long address)
        {
            long cur = w.BaseStream.Position;
            w.Seek((int) address, SeekOrigin.Begin);
            w.Write((UInt16) value);
            w.Seek((int) cur, SeekOrigin.Begin);
        }

        /// <summary>
        /// Pad bytes.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        /// <param name="len">Number of bytes to write.</param>
        /// <returns>New position within the stream.</returns>
        internal static long Pad(BinaryWriter w, UInt16 len)
        {
            while (len-- > 0)
                w.Write((byte) 0);
            return w.BaseStream.Position;
        }

        /// <summary>
        /// Neutral language ID.
        /// </summary>
        internal static UInt16 NEUTRALLANGID
        {
            get
            {
                return MAKELANGID(Kernel32.LANG_NEUTRAL, Kernel32.SUBLANG_NEUTRAL);
            }
        }

        /// <summary>
        /// US-English language ID.
        /// </summary>
        internal static UInt16 USENGLISHLANGID
        {
            get
            {
                return ResourceUtil.MAKELANGID(Kernel32.LANG_ENGLISH, Kernel32.SUBLANG_ENGLISH_US);
            }
        }

        /// <summary>
        /// Make a language ID from a primary language ID (low-order 10 bits) and a sublanguage (high-order 6 bits).
        /// </summary>
        /// <param name="primary">Primary language ID.</param>
        /// <param name="sub">Sublanguage ID.</param>
        /// <returns>Microsoft language ID.</returns>
        internal static UInt16 MAKELANGID(int primary, int sub)
        {
            return (UInt16) ((((UInt16)sub) << 10) | ((UInt16)primary));
        }

        /// <summary>
        /// Return the primary language ID from a Microsoft language ID.
        /// </summary>
        /// <param name="lcid">Microsoft language ID</param>
        /// <returns>primary language ID (low-order 10 bits)</returns>
        internal static UInt16 PRIMARYLANGID(UInt16 lcid)
        {
            return (UInt16) (((UInt16)lcid) & 0x3ff);
        }

        /// <summary>
        /// Return the sublanguage ID from a Microsoft language ID.
        /// </summary>
        /// <param name="lcid">Microsoft language ID.</param>
        /// <returns>Sublanguage ID (high-order 6 bits).</returns>
        internal static UInt16 SUBLANGID(UInt16 lcid)
        {
            return (UInt16) (((UInt16)lcid) >> 10);
        }
    }
}
