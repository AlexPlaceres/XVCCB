using System;
using System.IO;
using System.Text;
using XVCCB.Serialization;
using XVCCB.Utilities;

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

    public void Write(BinaryWriter writer)
    { 
        writer.Write(Name);
        writer.Write(Version);
        writer.BaseStream.Seek(8, SeekOrigin.Current);
        writer.Write(User);
        writer.Write(Date);
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

    public void Write(BinaryWriter writer)
    {
        writer.Write(Name);
        writer.Write(SwitcherIndex);
        writer.Write(CameraLength);
        writer.Write(CameraStartFrame);
        writer.BaseStream.Seek(16, SeekOrigin.Current);
        
    }
}

public class CameraPartHeader
{
    public char[] Name { get; set; }
    public byte CameraNum {  get; set; }

    public void Read(BinaryReader reader)
    {
        Console.WriteLine("Camera Part Header:");

        Name = reader.ReadChars(6);
        CameraNum = reader.ReadByte();
        Console.WriteLine("  {0}, Camera Num = {1}", new string(Name), CameraNum);

        // Skips structure's 9 bytes of padding
        reader.BaseStream.Seek(9, SeekOrigin.Current);
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(Name);
        writer.Write(CameraNum);
        writer.WriteAlignmentPadding(16);
    }
    
}

public class CameraPart
{
    public CameraPartHeader camPartHeader { get; set; } = new();
    public char[] Name { get; set; }
    public float ApertureV {  get; set; }
    public float ApertureH { get; set; }
    public float Near { get; set; }
    public float Far {  get; set; }
    public byte RootNum {  get; set; }
    public byte MoveNum {  get; set; }
    public byte IntNum {  get; set; }
    public int RootAddress {  get; set; }
    public int MoveAddress { get; set; }
    public int IntAddress {  get; set; }
    public short FlenIndex {  get; set; }
    public short RolIndex {  get; set; }

    public void Read(BinaryReader reader)
    { 
        camPartHeader.Read(reader);

        Name = reader.ReadChars(32);
        Console.WriteLine("    Name = {0}", new string(Name));

        ApertureV = reader.ReadSingle();
        ApertureH = reader.ReadSingle();
        Near = reader.ReadSingle();
        Far = reader.ReadSingle();
        Console.WriteLine("    Vertical Aperture = {0,10:0.0000000}, Horizontal Aperture = {1,10:0.0000000}, Near = {2,10:0.0000000}, Far = {3,10:0.0000000}", ApertureV, ApertureH, Near, Far);

        RootNum = reader.ReadByte();
        MoveNum = reader.ReadByte();
        IntNum = reader.ReadByte();
        reader.BaseStream.Seek(1, SeekOrigin.Current); // 1 Byte of padding in the structure
        Console.WriteLine("    Root Num = {0}, Move Num = {1}, Int Num = {2}", RootNum, MoveNum, IntNum);

        RootAddress = reader.ReadInt32();
        MoveAddress = reader.ReadInt32();
        IntAddress = reader.ReadInt32();
        Console.WriteLine("    Root Address = {0}, Move Address = {1}, Int Address = {2}", RootAddress, MoveAddress, IntAddress);

        FlenIndex = reader.ReadInt16();
        RolIndex = reader.ReadInt16();
        Console.WriteLine("    Focal Length Index = {0}, Roll Index = {1}", FlenIndex, RolIndex);

        // 12 Bytes of padding at the end of the structure
        reader.BaseStream.Seek(12, SeekOrigin.Current);

    }

    public void Write(BinaryWriter writer)
    {
        camPartHeader.Write(writer);
        writer.Write(Name);
        writer.Write(ApertureV);
        writer.Write(ApertureH);
        writer.Write(Near);
        writer.Write(Far);

        writer.Write(RootNum);
        writer.Write(MoveNum);
        writer.Write(IntNum);
        writer.WriteAlignmentPadding(4);

        writer.Write(RootAddress);
        writer.Write(MoveAddress);
        writer.Write(IntAddress);

        writer.Write(FlenIndex);
        writer.Write(RolIndex);
        writer.WriteAlignmentPadding(16);
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

    public void Write(BinaryWriter writer)
    { 
        writer.Write(Name);
        writer.BaseStream.Seek(2, SeekOrigin.Current);

        writer.Write(MtbSize);
        writer.Write(Size);
    }
}



