using System;
using System.IO;
using System.Text;

namespace PepLib.IO
{
    public sealed class DrawingInfoReader
    {
        public DrawingInfo Info { get; private set; }

        public DrawingInfoReader()
        {
            Info = new DrawingInfo();
        }

        public DrawingInfoReader(DrawingInfo info)
        {
            Info = info;
        }

        public void Read(Stream stream)
        {
            Info.Name = ReadString(0xC8, ref stream);
            Info.Revision = ReadString(0x20, ref stream);
            Info.CreationDate = DateTime.Parse(ReadString(0xA, ref stream));
            Info.LastModifiedDate = DateTime.Parse(ReadString(0xA, ref stream));
            Info.LastReferenceDate = DateTime.Parse(ReadString(0xA, ref stream));
            Info.Description = ReadString(0xC8, ref stream);
            Info.Customer = ReadString(0x40, ref stream);
            Info.Comment = ReadString(0x40, ref stream);
            Info.Notes = ReadString(0x400, ref stream);
            Info.Grain = (GrainType)ReadByte(ref stream);

            stream.Seek(0x9, SeekOrigin.Current);

            Info.MaterialNumber = int.Parse(ReadString(0x40, ref stream));
            Info.MaterialGrade = ReadString(0x10, ref stream);
            Info.ProgrammedBy = ReadString(0x40, ref stream);
            Info.CreatedBy = ReadString(0x40, ref stream);
            Info.Type = (DrawingType)ReadByte(ref stream);

            stream.Seek(0x4, SeekOrigin.Current);

            Info.Errors = ReadString(0x64, ref stream);
            Info.Hardness = ReadString(0x20, ref stream);
            Info.Specification = ReadString(0x40, ref stream);

            stream.Seek(0x2, SeekOrigin.Current);

            Info.UserDefined1 = ReadString(0x20, ref stream);
            Info.UserDefined2 = ReadString(0x20, ref stream);
            Info.UserDefined3 = ReadString(0x20, ref stream);
            Info.UserDefined4 = ReadString(0x40, ref stream);
            Info.UserDefined5 = ReadString(0x40, ref stream);
            Info.UserDefined6 = ReadString(0x40, ref stream);
            Info.MachineNumber = ReadByte(ref stream);

            stream.Seek(0x1, SeekOrigin.Current);

            Info.Application = (ApplicationType)ReadByte(ref stream);
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
