using XVCCB.Data.Binary;
using XVCCB.Utilities;


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
            }

            GetActiveFrames(FrameFields, reader);

        }

        Console.WriteLine("\n");
    }

    private void GetActiveFrames(FrameBit[] Fields, BinaryReader reader)
    {

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

    }

    public void Write(BinaryWriter writer, float frameCount)
    {
        writer.Write((uint)CurveId);
        writer.Write(unknown_0);
        writer.Write(NumFrameBits);

        if (NumFrameBits <= 0)
        { 
            writer.Write(unknown_2);
        }
        else
        {
            writer.WriteAlignmentPadding(16);

            WriteKeyFrames(writer, frameCount);




            foreach (KeyValuePair<uint, float> kvp in KeyFrames)
            {
                writer.Write(kvp.Value);
            }
        }
    }

    private void WriteKeyFrames(BinaryWriter writer, float frameCount)
    {
        uint writtenBits = 0;
        uint writtenFrames = 0;

        while (writtenFrames < frameCount)
        {
            FrameBit frameBitField = new FrameBit();
            for (uint i = 0; i < 64; i++)
            {
                if (KeyFrames.ContainsKey(writtenBits + i))
                {
                    frameBitField.FirstHalf |= 0x1UL << (int)i;
                }

                if (KeyFrames.ContainsKey(writtenBits + 64 + i))
                {
                    frameBitField.SecondHalf |= 0x1UL << (int)(i);

                }
                writtenFrames += 2;
            }
            writtenBits += 128;
            frameBitField.Write(writer);
        }
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

    public string Name { get; set; }

    public void Read(BinaryReader reader)
    { 
        FullNameId = reader.ReadUInt32();
        NameId = reader.ReadUInt32();

        Name = ReadNodeName(reader);

        Console.WriteLine("\t     Node: {0}", Name);

        JointLength = reader.ReadSingle();
        CurveCount = reader.ReadUInt16();
        Flags = reader.ReadUInt16();
        

        Curves = new MTBCurve[CurveCount];

        if (CurveCount > 0)
        {
            Console.WriteLine("\t\tNumber of Curves: {0}", CurveCount);

            for (int i = 0; i < CurveCount; i++)
            {
                Console.WriteLine("\t\tCurve {0}", i);
                Curves[i] = new MTBCurve();
                Curves[i].Read(reader);
            }
        }
        else
        {
            Console.WriteLine("\t\tNo Curves");
            Console.WriteLine("\n");
        }
        
    }

    private string ReadNodeName(BinaryReader reader)
    {
        long returnPoint = reader.BaseStream.Position;
        reader.BaseStream.Position = (returnPoint - 4) + ((long)(NameId & 0x00FFFFFF));

        string result = reader.ReadNullTerminatedString();

        reader.BaseStream.Position = returnPoint;

        return result;
    }

    public void Write(BinaryWriter writer, float frameCount)
    {
        writer.Write(FullNameId);
        writer.Write(NameId);
        writer.Write(JointLength);
        writer.Write(CurveCount);
        writer.Write(Flags);

        foreach (MTBCurve curve in Curves)
        {
            curve.Write(writer, frameCount);
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

    public void Write(BinaryWriter writer)
    {
        writer.Write(FrameRate);
        writer.Write(TotalFrames);
        writer.Write(CurveNodeCount);
        writer.Write(PhysicsNodeCount);
        writer.Write(Status);
        writer.Write(Offset);

    }
}

public class MotionTrackBinary
{
    public CameraMtbHeader camMtbHeader { get; set; } = new();
    public SectionDataBinary File_Header { get; set; } = new();
    public MTBDataHeader Data_Header { get; set; } = new();
    public MTBNodeHeader[] Nodes {  get; set; }
    public long StartingPoint {  get; set; }

    public void Read(BinaryReader reader)
    {
        Console.WriteLine("Motion Track Binary:");
        StartingPoint = reader.BaseStream.Position;

        camMtbHeader.Read(reader);
        File_Header.Read(reader);
        Data_Header.Read(reader);

        Console.WriteLine("Nodes:");
        Nodes = new MTBNodeHeader[Data_Header.CurveNodeCount];
        for (int i = 0; i < Nodes.Length; i++)
        {
            Console.WriteLine("\t   Node {0}", i + 1);
            Nodes[i] = new MTBNodeHeader();
            Nodes[i].Read(reader);
        }

    }

    public void Write(BinaryWriter writer)
    { 
        camMtbHeader.Write(writer);
        File_Header.Write(writer);
        Data_Header.Write(writer);

        foreach (MTBNodeHeader node in Nodes)
        {
            node.Write(writer, Data_Header.TotalFrames);
        }

        writer.Write(File_Header.StrName.ToCharArray());
        foreach (MTBNodeHeader node in Nodes)
        {
            writer.Write(node.Name.ToCharArray());
        }
        writer.WriteAlignmentPadding(16);
    }

}