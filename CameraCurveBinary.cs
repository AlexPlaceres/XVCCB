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

    public override void Read(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        SecData.Read(reader);
        CamFileHeader.Read(reader);
        CamComHeader.Read(reader);
        MotTrackBin.Read(reader);
    }

    public void Read(BinaryReader reader)
    { 
        throw new NotImplementedException();
    }
}