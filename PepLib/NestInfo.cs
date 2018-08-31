using System;
using System.IO;
using PepLib.IO;

namespace PepLib
{
    public class NestInfo
    {
        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateLastModified { get; set; }

        public StatusType Status { get; set; }

        public int LoopCount { get; set; }

        public int ProgramCount { get; set; }

        public int PlateCount { get; set; }

        public string Comments { get; set; }

        public string Customer { get; set; }

        public string ProgrammedBy { get; set; }

        public int MaterialNumber { get; set; }

        public string MaterialGrade { get; set; }

        public string Notes { get; set; }

        public string DefaultPlateSize { get; set; }

        public string Kerf { get; set; }

        public string PostedAs { get; set; }

        public string Errors { get; set; }

        public string UserDefined1 { get; set; }

        public string UserDefined2 { get; set; }

        public string UserDefined3 { get; set; }

        public string UserDefined4 { get; set; }

        public string UserDefined5 { get; set; }

        public string UserDefined6 { get; set; }

        public static NestInfo Load(string nestFile)
        {
            var reader = new NestInfoReader();
            reader.Read(nestFile);
            return reader.Info;
        }

        public static NestInfo Load(Stream stream)
        {
            var reader = new NestInfoReader();
            reader.Read(stream);
            return reader.Info;
        }

        public static bool TryLoad(string nestFile, out NestInfo nestInfo)
        {
            try
            {
                nestInfo = Load(nestFile);
            }
            catch (Exception)
            {
                nestInfo = null;
                return false;
            }

            return true;
        }

        public static bool TryLoad(Stream stream, out NestInfo nestInfo)
        {
            try
            {
                nestInfo = Load(stream);
            }
            catch (Exception)
            {
                nestInfo = null;
                return false;
            }

            return true;
        }
    }
}