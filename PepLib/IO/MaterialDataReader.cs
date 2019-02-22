using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PepLib.IO
{
    public class MaterialDataReader
    {
        public List<MaterialData> Materials { get; set; }

        public MaterialDataReader()
        {
            Materials = new List<MaterialData>();
        }

        public void Read(Stream stream)
        {
            const int dataLength = 2000;
            var count = stream.Length / dataLength;
            var binreader = new BinaryReader(stream);

            for (int i = 0; i < count; i++)
            {
                var data = new MaterialData();

                int id;
                int.TryParse(ReadString(64, ref stream), out id);
                data.Id = id;
                data.Grade = ReadString(16, ref stream);
                data.Name = ReadString(200, ref stream);
                data.Density = binreader.ReadDouble();
                stream.Seek(8, SeekOrigin.Current);
                data.Thickness = binreader.ReadDouble();

                Materials.Add(data);

                stream.Position = i * dataLength;
            }
        }

        public void Read(string file)
        {
            if (!File.Exists(file))
            {
                var msg = string.Format("File Not Found: {0}", file);
                throw new FileNotFoundException(msg);
            }

            Stream stream = null;

            try
            {
                stream = File.OpenRead(file);
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

    public class MaterialData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Grade { get; set; }

        public double Density { get; set; }

        public double Thickness { get; set; }
    }
}
