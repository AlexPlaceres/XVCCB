using System;
using System.IO;
using System.Text;
using XVCCB.Serialization;

namespace XVCCB.Data;

public class CameraFileHeader
{
    public char[] Name { get; set; }
    public ushort Version {  get; set; }
    public char[] User {  get; set; }
    public char[] Date { get; set; }

    public void Read(BinaryReader reader)
    {
        Console.WriteLine("\t\tCamera File Header:");

        Name = reader.ReadChars(6);
        Version = reader.ReadUInt16();
        Console.WriteLine("\t\t\t{0}, Version {1}", new string(Name), Version);

        // Skipping The Included 8 Bytes of Padding
        reader.BaseStream.Seek(8, SeekOrigin.Current);

        User = reader.ReadChars(16);
        Date = reader.ReadChars(16);
        Console.WriteLine("\t\t\tCreated by {0} on {1}\n", new string(User), new string(Date));
        
    }
}

public class CameraCommonHeader
{
    public char[] Name { get; set; }
    public short SwitcherIndex { get; set; }
    public int CameraLength {  get; set; }
    public float CameraStartFrame {  get; set; }

    public void Read(BinaryReader reader)
    {
        Console.WriteLine("\t\tCamera Common Header");

        Name = reader.ReadChars(6);
        SwitcherIndex = reader.ReadInt16();
        Console.WriteLine("\t\t\t{0}, Switcher Index = {1}", new string(Name), SwitcherIndex);

        CameraLength = reader.ReadInt32();
        CameraStartFrame = reader.ReadSingle();
        Console.WriteLine("\t\t\tCamera Length = {0}, Camera Starting Frame = {1}\n", CameraLength, CameraStartFrame);

        reader.BaseStream.Seek(16, SeekOrigin.Current);
    }
}

public class CameraMtbHeader
{ 
    public char[] Name { get; set; }
    public int MtbSize {  get; set; }
    public int Size {  get; set; }

    public void Read(BinaryReader reader)
    {
        Name = reader.ReadChars(6);

        // Skips the structure's 2 bytes of padding
        reader.BaseStream.Seek(2, SeekOrigin.Current);

        MtbSize = reader.ReadInt32();
        Size = reader.ReadInt32();

        Console.WriteLine("{0}, MTB Size = {1} Bytes, Size = {2} Bytes", new string(Name), MtbSize, Size);
    }
}



