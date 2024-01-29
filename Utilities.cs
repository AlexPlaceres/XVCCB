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
            Console.WriteLine("\t\t  Aligning...");
            Console.WriteLine("\t\t  Current Offset = {0}", Offset);
            Console.WriteLine("\t\t  Seeking forward by {0} bytes to align to nearest {1} byte block", alignmentPaddingSize, BlockSize);

            reader.BaseStream.Seek(alignmentPaddingSize, SeekOrigin.Current);
            Console.WriteLine("\t\t  New Position: {0}", reader.BaseStream.Position);
        }
        else
        {
            Console.WriteLine("\t\t  No Alignment Needed");
            Console.WriteLine("\t\t  Current Offset = {0}", Offset);
        }
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