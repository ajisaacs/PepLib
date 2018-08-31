using Ionic.Zip;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PepLib
{
    public static class ZipHelper
    {
        /// <summary>
        /// Returns the files that match the specified pattern.
        /// </summary>
        /// <param name="file">Input zip file.</param>
        /// <param name="pattern">Pattern to match.</param>
        /// <param name="names">Names of the files that match the pattern.</param>
        /// <param name="streams">Data of the files that match the pattern.</param>
        /// <returns></returns>
        public static int ExtractByPattern(string file, string pattern, out string[] names, out Stream[] streams)
        {
            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var zipStream = new ZipInputStream(fileStream);
            var nameList = new List<string>();
            var streamList = new List<Stream>();

            ZipEntry theEntry;

            while ((theEntry = zipStream.GetNextEntry()) != null)
            {
                if (!Regex.IsMatch(theEntry.FileName, pattern))
                    continue;

                nameList.Add(theEntry.FileName);

                var memstream = new MemoryStream();
                var size = 2048;
                var data = new byte[size];

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
                streamList.Add(memstream);
            }

            zipStream.Close();

            names = nameList.ToArray();
            streams = streamList.ToArray();

            return streams.Length;
        }

        /// <summary>
        /// Returns the first file found that matches the specified file extension.
        /// </summary>
        /// <param name="file">Input zip file.</param>
        /// <param name="extension">Extension to match.</param>
        /// <param name="name">The name of the file that matches the file extension.</param>
        /// <param name="stream">The data of the file that matches the file extension.</param>
        /// <returns></returns>
        public static bool ExtractByExtension(string file, string extension, out string name, out Stream stream)
        {
            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var zipStream = new ZipInputStream(fileStream);
            var memstream = new MemoryStream();

            ZipEntry theEntry;

            while ((theEntry = zipStream.GetNextEntry()) != null)
            {
                if (Path.GetExtension(theEntry.FileName) != extension)
                    continue;

                int size = 2048;
                var data = new byte[size];

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

                zipStream.Close();
                memstream.Seek(0, SeekOrigin.Begin);

                stream = memstream;
                name = theEntry.FileName;

                return true;
            }

            zipStream.Close();
            memstream.Close();

            stream = null;
            name = null;

            return false;
        }
    }
}
