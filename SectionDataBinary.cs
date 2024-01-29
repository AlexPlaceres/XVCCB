using System;
using System.IO;
using System.Text;
using XVCCB.Serialization;

namespace XVCCB.Data;

public class SectionDataBinary
{
    public char[] Type { get; set; }
    public char[] SubType { get; set; }
    public uint Version { get; set; }
    public BinaryEndianType EndianType { get; set; }
    public byte AlignmentBits {  get; set; }
    public ushort Offset { get; set; }
    public ulong Size { get; set; }
    public ulong DateTime {  get; set; }
    public char[] Name { get; set; }

    public void Read(BinaryReader reader)
    {
        Console.WriteLine("Section Data Binary:");

        Type = reader.ReadChars(4);
        SubType = reader.ReadChars(4);
        Version = reader.ReadUInt32();
        Console.WriteLine("\tType: {0}, SubType: {1}, Version: {2}", new String(Type), new String(SubType), Version);

        EndianType = (BinaryEndianType)reader.ReadByte();
        AlignmentBits = reader.ReadByte();

        Offset = reader.ReadUInt16();
        Size = reader.ReadUInt64();
        DateTime = reader.ReadUInt64();
        Console.WriteLine("\tFile Size = {0} Bytes, Offset = {1} Bytes\n", Size, Offset);

        Name = reader.ReadChars(16);
    }
}
