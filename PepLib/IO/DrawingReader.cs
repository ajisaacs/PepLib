using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace PepLib.IO
{
    public sealed class DrawingReader
    {
        public Drawing Drawing { get; private set; }

        public DrawingReader()
        {
            Drawing = new Drawing();
        }

        public DrawingReader(Drawing drawing)
        {
            Drawing = drawing;
        }

        public void Read(Stream stream)
        {
            var drawing = new Drawing();
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
                }

                if (Regex.IsMatch(extension, "loop-\\d\\d\\d"))
                    drawing.Loops.Add(ReadLoop(theEntry.FileName, memstream));

                memstream.Close();
            }

            zipStream.Close();
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
                Drawing.Info = DrawingInfo.Load(stream);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.StackTrace);
            }
        }

        private Loop ReadLoop(string name, Stream stream)
        {
            var reader = new LoopReader();
            reader.Read(name, stream);

            return reader.Loop;
        }
    }
}
