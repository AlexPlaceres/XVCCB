using System;
using System.IO;
using System.Text;
using XVCCB.Data.Binary;
using XVCCB.Serialization;
using XVCCB.Utilities;
using System.Numerics;


namespace XVCCB.Data;

public class MTBCurve
{ 
    public MTBCurveId CurveId {  get; set; }
    public ushort unknown_0 {  get; set; }
    public ushort NumFrameBits {  get; set; }
    public float unknown_2 { get; set; } = 0f;
    public FrameBit[] FrameFields { get; set; }
    public SortedDictionary<uint, float> KeyFrames { get; set; } = new SortedDictionary<uint, float>();

    public void Read(BinaryReader reader)
    {
        CurveId = (MTBCurveId)reader.ReadUInt32();
        unknown_0 = reader.ReadUInt16();
        NumFrameBits = reader.ReadUInt16();
        Console.WriteLine("\t\t Curve Id = {0}, Number of FrameBits = {1}", CurveId, NumFrameBits);

        if (NumFrameBits == 0)
        {
            
            unknown_2 = reader.ReadSingle();
            Console.WriteLine("\t\t Unknown Value = {0}", unknown_2);
        }
        else
        {
            // Aligns to nearest 16 byte block if necessary
            reader.Align(16);

            FrameFields = new FrameBit[NumFrameBits];
            for (int i = 0; i < NumFrameBits; i++)
            {
                FrameFields[i] = new FrameBit();
                FrameFields[i].Read(reader);
                //Console.WriteLine("\t\tCounted Frame Field");

            }

            GetActiveFrames(FrameFields, reader);

            Console.WriteLine("\t\t Transforms:");
            Transforms = new float[TotalFrames];
            for (int i = 0; i < TotalFrames; i++)
            {
                Transforms[i] = reader.ReadSingle();
                Console.WriteLine("\t\t  {0}", Transforms[i]);
            }


        }

        Console.WriteLine("\n");
    }

    public void GetActiveFrames(FrameBit[] Fields, BinaryReader reader)
    {
        uint total = 0;

        uint keyFrameIndex = 0;

        for (int i = 0; i < Fields.Length; i++)
        {
            for (int j = 0; j < 64; j++)
            {
                if (((Fields[i].FirstHalf >> j) & 0x1) == 0x1)
                {

                    KeyFrames.Add((uint)(keyFrameIndex + j), 0.0f);

                }

                if (((Fields[i].SecondHalf >> j) & 0x1) == 0x1)
                {

                    KeyFrames.Add((uint)(keyFrameIndex + 64 + j), 0.0f);
                }
            }

            keyFrameIndex += 128;
        }

        Console.WriteLine("\t\t Number of Keyframes = {0}", KeyFrames.Count);
        Console.WriteLine("\t\t Transforms:");
        var keys = new List<uint>(KeyFrames.Keys);
        foreach (uint key in keys)
        {
            KeyFrames[key] = reader.ReadSingle();
            Console.WriteLine("\t\t  Frame {0,2} = {1,10:0.0000000}", key, KeyFrames[key]);
        }

        return total;
    }

}

public class MTBNodeHeader
{ 
    public uint FullNameId {  get; set; }
    public uint NameId {  get; set; }
    public float JointLength {  get; set; }
    public ushort CurveCount {  get; set; }
    public ushort Flags {  get; set; }
    public MTBCurve[] Curves { get; set; }

    public void Read(BinaryReader reader)
    { 
        FullNameId = reader.ReadUInt32();
        NameId = reader.ReadUInt32();
        JointLength = reader.ReadSingle();
        CurveCount = reader.ReadUInt16();
        Flags = reader.ReadUInt16();
        Console.WriteLine("\t\tNumber of Curves: {0}", CurveCount);

        Console.WriteLine("\t\t Curves:");
        int i = 0;
        int testVal = CurveCount;
        Curves = new MTBCurve[testVal];
        while (i < testVal)
        {
            Console.WriteLine("\t\tCurve {0}", i + 1);
            Curves[i] = new MTBCurve();
            Curves[i].Read(reader);

            i += 1;
        }
    }

}

public class MTBDataHeader
{ 
    public float FrameRate {  get; set; }
    public float TotalFrames {  get; set; }
    public ushort CurveNodeCount {  get; set; }
    public ushort PhysicsNodeCount {  get; set; }
    public uint Status {  get; set; }
    public uint Offset {  get; set; }

    public void Read(BinaryReader reader)
    {
        Console.WriteLine("Data Header:");

        FrameRate = reader.ReadSingle();
        TotalFrames = reader.ReadSingle();
        Console.WriteLine("Frame Rate = {0}, Total Frames = {1}", FrameRate, TotalFrames);

        CurveNodeCount = reader.ReadUInt16();
        PhysicsNodeCount = reader.ReadUInt16();
        Console.WriteLine("Number of Curve Nodes: {0}, Number of Physics Nodes: {1}", CurveNodeCount, PhysicsNodeCount);

        Status = reader.ReadUInt32();
        Offset = reader.ReadUInt32();
    }

}

public class MotionTrackBinary
{
    public CameraMtbHeader camMtbHeader { get; set; } = new();
    public SectionDataBinary File_Header { get; set; } = new();
    public MTBDataHeader Data_Header { get; set; } = new();
    public MTBNodeHeader[] Nodes {  get; set; }

    public void Read(BinaryReader reader)
    {
        Console.WriteLine("Motion Track Binary:");

        camMtbHeader.Read(reader);
        File_Header.Read(reader);
        Data_Header.Read(reader);

        Console.WriteLine("Nodes:");
        int TestValue = Data_Header.CurveNodeCount;
        Nodes = new MTBNodeHeader[TestValue];
        int i = 0;
        while (i < TestValue)
        {
            Console.WriteLine("\tNode {0}", i + 1);
            Nodes[i] = new MTBNodeHeader();
            Nodes[i].Read(reader);

            i += 1;
        }

    }



}