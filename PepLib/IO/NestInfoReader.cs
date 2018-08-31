using System;
using System.IO;
using System.Text;

namespace PepLib.IO
{
    internal sealed class NestInfoReader
    {
        public NestInfo Info { get; private set; }

        public NestInfoReader()
        {
            Info = new NestInfo();
        }

        public NestInfoReader(NestInfo info)
        {
            Info = info;
        }

        public void Read(Stream stream)
        {
            var binReader = new BinaryReader(stream);

            Info.Name = ReadString(0xC8, ref stream);
            Info.DateCreated = DateTime.Parse(ReadString(0xA, ref stream));
            Info.DateLastModified = DateTime.Parse(ReadString(0xA, ref stream));
            Info.LoopCount = binReader.ReadInt16();
            Info.ProgramCount = binReader.ReadInt16();
            Info.Customer = ReadString(0x40, ref stream);
            Info.ProgrammedBy = ReadString(0x40, ref stream);
            Info.Comments = ReadString(0x40, ref stream);

            // skip 2 bytes
            stream.Seek(0x2, SeekOrigin.Current);

            Info.MaterialNumber = int.Parse(ReadString(0x40, ref stream));
            Info.MaterialGrade = ReadString(0x10, ref stream);

            // skip 2 bytes
            stream.Seek(0x2, SeekOrigin.Current);

            Info.Notes = ReadString(0x400, ref stream);
            Info.PostedAs = ReadString(0x64, ref stream);
            Info.Errors = ReadString(0x64, ref stream);
            Info.UserDefined1 = ReadString(0x20, ref stream);
            Info.UserDefined2 = ReadString(0x20, ref stream);
            Info.UserDefined3 = ReadString(0x20, ref stream);
            Info.UserDefined4 = ReadString(0x40, ref stream);
            Info.UserDefined5 = ReadString(0x40, ref stream);
            Info.UserDefined6 = ReadString(0x40, ref stream);
            Info.DefaultPlateSize = ReadString(0x1E, ref stream);
            Info.Kerf = ReadString(0x3, ref stream);

            // skip 4 bytes
            stream.Seek(0x4, SeekOrigin.Current);

            switch (ReadByte(ref stream))
            {
                case 0:
                    Info.Status = StatusType.ToBeCut;
                    break;

                case 1:
                    Info.Status = StatusType.Quote;
                    break;

                case 2:
                    Info.Status = StatusType.HasBeenCut;
                    break;

                case 3:
                    Info.Status = StatusType.Temp;
                    break;

                default:
                    Info.Status = StatusType.ToBeCut;
                    break;
            }

            // skip 16 bytes
            stream.Seek(16, SeekOrigin.Current);

            Info.PlateCount = binReader.ReadInt16();
        }

        public void Read(string nestFile)
        {
            if (!File.Exists(nestFile))
            {
                var msg = string.Format("File Not Found: {0}", nestFile);
                throw new FileNotFoundException(msg);
            }

            Stream stream = null;
            string name;

            try
            {
                ZipHelper.ExtractByExtension(nestFile, ".dir", out name, out stream);
                Read(stream);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        private static string ReadString(int length, ref Stream stream)
        {
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return Encoding.Default.GetString(buffer).Trim();
        }

        private static byte ReadByte(ref Stream stream)
        {
            var buffer = new byte[0x1];
            stream.Read(buffer, 0, 1);
            return buffer[0];
        }
    }
}
