using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PepLib.IO
{
    public sealed class NestReader
    {
        public Nest Nest { get; private set; }

        private readonly Dictionary<string, Stream> plates;
        private readonly Dictionary<string, Stream> loops;

        public NestReader()
            : this(new Nest())
        {
        }

        public NestReader(Nest nest)
        {
            Nest = nest;
            plates = new Dictionary<string, Stream>();
            loops = new Dictionary<string, Stream>();
        }

        public void Read(Stream stream)
        {
            const string plateExtensionPattern = "plate-\\d\\d\\d";
            const string loopExtensionPattern = "loop-\\d\\d\\d";

            var zipStream = new ZipInputStream(stream);

            ZipEntry theEntry;

            while ((theEntry = zipStream.GetNextEntry()) != null)
            {
                var size = 2048;
                var data = new byte[size];
                var memstream = new MemoryStream();

                while (true)
                {
                    size = zipStream.Read(data, 0, data.Length);

                    if (size > 0)
                    {
                        memstream.Write(data, 0, size);
                        memstream.Flush();
                    }
                    else break;
                }

                memstream.Seek(0, SeekOrigin.Begin);

                var extension = Path.GetExtension(theEntry.FileName);

                switch (extension)
                {
                    case ".dir":
                        LoadInfo(memstream);
                        memstream.Close();
                        continue;

                    case ".report":
                        LoadReport(memstream);
                        memstream.Close();
                        continue;

                    case ".dwg-info":
                        LoadDrawingInfo(memstream);
                        memstream.Close();
                        continue;

                    default:
                        Debug.WriteLine("Unknown file: " + theEntry.FileName);
                        break;
                }

                if (Regex.IsMatch(extension, loopExtensionPattern))
                    loops.Add(theEntry.FileName, memstream);
                else if (Regex.IsMatch(extension, plateExtensionPattern))
                    plates.Add(theEntry.FileName, memstream);
            }

            zipStream.Close();

            foreach (var loop in loops)
                Nest.Loops.Add(ReadLoop(loop.Key, loop.Value));

            Nest.ResolveLoops();

            foreach (var plate in plates)
                Nest.Plates.Add(ReadPlate(plate.Key, plate.Value));
        }

        public void Read(string nestFile)
        {
            if (!File.Exists(nestFile))
            {
                var msg = string.Format("File Not Found: {0}", nestFile);
                throw new FileNotFoundException(msg);
            }

            Stream stream = null;

            try
            {
                stream = new FileStream(nestFile, FileMode.Open);
                Read(stream);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        private void LoadInfo(Stream stream)
        {
            try
            {
                Nest.Info = NestInfo.Load(stream);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.StackTrace);
            }
        }

        private void LoadReport(Stream stream)
        {
            try
            {
                Nest.Report = Report.Load(stream);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.StackTrace);
            }
        }

        private void LoadDrawingInfo(Stream stream)
        {
            var buffer = new byte[2000];

            while (stream.Read(buffer, 0, buffer.Length) > 0)
            {
                var name = Encoding.Default.GetString(buffer, 200, 200).Trim();
                var qty = BitConverter.ToInt32(buffer, 432);

                var drawing = new NestDrawing
                {
                    Name = name,
                    QtyRequired = qty
                };

                Nest.Drawings.Add(drawing);
            }
        }

        private Loop ReadLoop(string name, Stream stream)
        {
            var reader = new LoopReader();
            reader.Read(name, stream);

            return reader.Loop;
        }

        private Plate ReadPlate(string name, Stream stream)
        {
            var reader = new PlateReader();
            reader.Read(name, stream, Nest);

            return reader.Plate;
        }
    }
}
