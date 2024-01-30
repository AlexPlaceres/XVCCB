using System;
using System.IO;

namespace XVCCB.Utilities;

public static class ParseExtensions
{
    public static void Align(this BinaryReader reader, uint BlockSize)
    {
        long Offset = reader.BaseStream.Position;
        long alignmentPaddingSize = BlockSize + (BlockSize * (Offset / BlockSize)) - Offset;

        if (alignmentPaddingSize > 0 && alignmentPaddingSize < BlockSize)
        {
            //Console.WriteLine("\t\t  Aligning...");
            //Console.WriteLine("\t\t  Current Offset = {0}", Offset);
            //Console.WriteLine("\t\t  Seeking forward by {0} bytes to align to nearest {1} byte block", alignmentPaddingSize, BlockSize);

            reader.BaseStream.Seek(alignmentPaddingSize, SeekOrigin.Current);
            //Console.WriteLine("\t\t  New Position: {0}", reader.BaseStream.Position);
        }
        else
        {
            //Console.WriteLine("\t\t  No Alignment Needed");
            //Console.WriteLine("\t\t  Current Offset = {0}", Offset);
        }
    }

    
    public static string ReadNullTerminatedString(this BinaryReader reader)
    {
        int numChars = 0;
        long returnPoint = reader.BaseStream.Position;

        // Increments numChars until null termination is reached
        while (reader.ReadChar() != 0x0)
        {
            numChars += 1;
        }

        // Returns to string's starting point and reads characters into char array
        reader.BaseStream.Position = returnPoint;
        char[] charStr = reader.ReadChars(numChars+1);
        

        return new string(charStr);

    }
}


public class FrameBit
{
    public ulong FirstHalf {  get; set; }
    public ulong SecondHalf {  get; set; }

    public void Read(BinaryReader reader)
    {
        FirstHalf = reader.ReadUInt64();
        SecondHalf = reader.ReadUInt64();
    }
}