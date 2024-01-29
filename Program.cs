using System;
using System.IO;
using XVCCB.Data.Binary;
using XVCCB.Serialization;

public static class Program
{
    
    static void Main(string[] args)
    {
        string ccbFilePath;
        ccbFilePath = "C:\\Users\\Mongo\\source\\repos\\XVCCB\\testFiles\\qt_ch02_0030_02_ev000_cut1.ccb";

        if (args.Length == 0)
        {
            Array.Resize<string>(ref args, 1);
            args[0] = ccbFilePath;
            
        }



        foreach (var ccbPath in args)
        { 
            string fileExtension = Path.GetExtension(ccbPath);

            if (fileExtension.Equals(".ccb", StringComparison.OrdinalIgnoreCase))
            {
                string fileName = Path.GetFileName(ccbPath);
                Console.WriteLine("Reading Camera Curve Binary: {0}\n", fileName);

                CameraCurveBinary ccb = ReadFromBinary(ccbPath);

                Console.WriteLine("Finished!");

            }
            else
            {
                throw new IOException("Not a valid Camera Curve Binary (.ccb)");
            }

            Console.WriteLine("\n\n");
        }

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }

    public static CameraCurveBinary ReadFromBinary(string path)
    { 
        CameraCurveBinary cameraCurveBinary = new CameraCurveBinary();

        using (FileStream stream = new FileStream(path, FileMode.Open ))
        { 
            cameraCurveBinary.Read(stream);
        }

        return cameraCurveBinary;
    }
}

