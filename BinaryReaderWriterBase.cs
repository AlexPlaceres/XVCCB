﻿using System;
using System.IO;

namespace XVCCB.Serialization;

public class BinaryReaderWriterBase
{
    public void Read(byte[] buffer)
    {
        using var stream = new MemoryStream(buffer);
        Read(stream);
    }

    public void Read(string filePath)
    { 
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        Read(stream);
    }

    public virtual void Read(Stream stream)
    {
        throw new NotImplementedException("This method should not be called from the base class");
    }
}
