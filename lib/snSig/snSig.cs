// (c) 2005 Richard Grimes
// snSig library used to get information about a strong name signature of
// a .NET assembly

using System;
using System.IO;

// Used to obtain the file offset and the size of the strong name signature
// and the strong name data directory
public class StrongNameSignature
{
    // Locations and sizes of various things in the PE file
    const int pePos = 0x003c;
    const int numSectOffset = 0x02;
    const int peIdentSize = 0x04;
    const int coffHeaderSize = 0x14;
    const int dataDirectoryOffset = 0x60;
    const int dataDirectoryLength = 0x08;
    const int clrHeaderIndex = 0x0e;
    const int strongNameSigOffset = 0x20;
    const int sectionHeaderSize = 0x28;
    const int sectionSizeOffSet = 0x08;
    const int sectionRVAOffset = 0x0c;
    const int sectionRawOffset = 0x14;

    const uint pe_header_size = 0x178u;


    int strongNameSigSize = 0;
    // Returns the size of the strong name signature
    public int Size
    {
        get { return strongNameSigSize; }
    }

    int strongNameSig = 0;
    // Returns the offset in the file of the strong name signature
    public int Address
    {
        get { return strongNameSig; }
    }

    int strongNameDataDirectory = 0;
    // Returns the offset of the strong name data directory
    public int StrongNameDataDirectory
    {
        get { return strongNameDataDirectory; }
    }

    public int TextSectionPointer { get; set; }
    public int HeaderSize { get; set; }
    FileStream fs;
    // Passed an open file of a .NET assembly
    public StrongNameSignature(FileStream file)
    {
        fs = file;

        // Find the pointer to the PE identifier before the COFF header 
        int coffHeader = GetInt(pePos);
        // Get the number of sections from the COFF header
        short numSections = GetWord(coffHeader + peIdentSize + numSectOffset);

        HeaderSize = (int) (pe_header_size + numSections*sectionHeaderSize);

        // Calculate the location of the PE header
        int peHeader = coffHeader + coffHeaderSize + peIdentSize;
        // Determine the location of the data directories
        int dataDirectories = peHeader + dataDirectoryOffset;
        // Determine the location of the CLR directory
        int clrHeaderDD = dataDirectories + (dataDirectoryLength * clrHeaderIndex);

        // Read the RVA of the CLR header
        int clrHeader = GetInt(clrHeaderDD);
        // Read the size of the CLR header
        int clrHeaderSize = GetInt(clrHeaderDD + 4);

        // Note that CLR header is stored in the .text section, so read this section
        // so that we can convert between RVA and actual file offsets
        int textRVA = 0;
        int textRaw = 0;
        int textSize = 0;
        // Determine the location of the section headers
        int sections = clrHeaderDD + (2 * dataDirectoryLength);
        // Iterate through the section headers until we find the header for the 
        // .text section
        byte[] bText = { 0x2e, 0x74, 0x65, 0x78, 0x74, 0x00, 0x00, 0x00 };
        for (int idx = 0; idx < numSections; ++idx)
        {
            // Read the first eight bytes which will have the name of the section
            // Note that this is NOT a NUL terminated string, the section name
            // can be 8 characters.
            int sectionStart = sections + (idx * sectionHeaderSize);
            byte[] eightBuf = GetEightBytes(sectionStart);
            if (!Compare(eightBuf, bText)) continue;

            // We have the right section so get the values
            textSize = GetInt(sectionStart + sectionSizeOffSet);
            textRVA = GetInt(sectionStart + sectionRVAOffset);
            textRaw = GetInt(sectionStart + sectionRawOffset);
        }

        TextSectionPointer = textRaw;
        // Convert RVA to file offset
        clrHeader = clrHeader - textRVA + textRaw;
        // Calculate the file offset of the strong name data directory 
        strongNameDataDirectory = clrHeader + strongNameSigOffset;
        // Get the RVA of the strong name signature
        strongNameSig = GetInt(clrHeader + strongNameSigOffset);
        // Convert RVA to file offset
        strongNameSig = strongNameSig - textRVA + textRaw;
        // Get the size of the strong name signature
        strongNameSigSize = GetInt(clrHeader + strongNameSigOffset + 4);
    }

    // Do a character by character comparison
    private static bool Compare(byte[] lhs, byte[] rhs)
    {
        int size = (lhs.Length > rhs.Length) ? rhs.Length : lhs.Length;
        for (int idx = 0; idx < size; ++idx)
        {
            if (lhs[idx] != rhs[idx]) return false;
        }
        return true;
    }
    // Read a 16-bit value from the file
    private short GetWord(int pos)
    {
        byte[] wordBuf = new byte[2];
        fs.Seek(pos, SeekOrigin.Begin);
        fs.Read(wordBuf, 0, wordBuf.Length);
        return BitConverter.ToInt16(wordBuf, 0);
    }
    // Read a 32-bit value from the file
    private int GetInt(int pos)
    {
        byte[] intBuf = new byte[4];
        fs.Seek(pos, SeekOrigin.Begin);
        fs.Read(intBuf, 0, intBuf.Length);
        return BitConverter.ToInt32(intBuf, 0);
    }
    // Read an eight byte array from the file
    private byte[] GetEightBytes(int pos)
    {
        byte[] eightBuf = new byte[8];
        fs.Seek(pos, SeekOrigin.Begin);
        fs.Read(eightBuf, 0, eightBuf.Length);
        return eightBuf;
    }
}