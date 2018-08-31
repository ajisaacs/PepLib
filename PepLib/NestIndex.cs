using System.Collections.Generic;
using System.IO;
using System.Text;
using PepLib.IO;

namespace PepLib
{
    public class NestIndex
    {
        public string Directory { get; set; }

        public List<NestInfo> Entries;

        public NestIndex()
        {
            Entries = new List<NestInfo>();
        }

        public string GetPath(NestInfo entry)
        {
            return Path.Combine(Directory, entry.Name + ".zip");
        }

        public static NestIndex LoadFromDir(string directory)
        {
            var file = Path.Combine(directory, "pepfiles.lfn");
            return Load(file);
        }

        public static NestIndex Load(string file)
        {
            if (!File.Exists(file))
                return null;

            var index = new NestIndex() { Directory = Path.GetDirectoryName(file) };
            var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var reader = new StreamReader(stream);
            var buffer = new char[4000];

            while (reader.Read(buffer, 0, buffer.Length) > 0)
            {
                var memstream = new MemoryStream(Encoding.Default.GetBytes(buffer));
                var inforeader = new NestInfoReader();

                inforeader.Read(memstream);
                index.Entries.Add(inforeader.Info);
            }

            reader.Close();

            return index;
        }

        public static NestIndex Build(string directory)
        {
            var index = new NestIndex() { Directory = directory };

            foreach (var file in System.IO.Directory.GetFiles(directory, "*.zip"))
            {
                var reader = new NestInfoReader();
                reader.Read(file);
                index.Entries.Add(reader.Info);
            }

            return index;
        }
    }
}
