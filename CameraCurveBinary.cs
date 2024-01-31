using System;
using System.IO;
using System.Text;
using XVCCB.Serialization;


namespace XVCCB.Data.Binary;


public class CameraCurveBinary : BinaryReaderWriterBase
{
    public SectionDataBinary SecData { get; set; } = new();
    public CameraFileHeader CamFileHeader { get; set; } = new();
    public CameraCommonHeader CamComHeader { get; set; } = new();
    public MotionTrackBinary MotTrackBin { get; set; } = new();
    public CameraPart CamPart { get; set; } = new();

    public override void Read(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, true);

        SecData.Read(reader);
        CamFileHeader.Read(reader);
        CamComHeader.Read(reader);
        MotTrackBin.Read(reader);

        reader.BaseStream.Position = MotTrackBin.StartingPoint + MotTrackBin.camMtbHeader.Size;
        CamPart.Read(reader);
    }

    public override void Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.ASCII, true);
        SecData.Write(writer);
        CamFileHeader.Write(writer);
        CamComHeader.Write(writer);
        MotTrackBin.Write(writer);

        CamPart.Write(writer);
    }

    public void Read(BinaryReader reader)
    { 
        throw new NotImplementedException();
    }

    public void Write(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}