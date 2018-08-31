using System;
using System.IO;
using System.Text;
using PepLib.IO;

namespace PepLib
{
    public class DrawingInfo
    {
        public string Name { get; set; }

        public string Revision { get; set; }

        public string Customer { get; set; }

        public string Description { get; set; }

        public string Comment { get; set; }

        public string Notes { get; set; }

        public string Source { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public DateTime LastReferenceDate { get; set; }

        public int MachineNumber { get; set; }

        public ApplicationType Application { get; set; }

        public int MaterialNumber { get; set; }

        public string MaterialGrade { get; set; }

        public string Specification { get; set; }

        public string Hardness { get; set; }

        public GrainType Grain { get; set; }

        public string ProgrammedBy { get; set; }

        public string CreatedBy { get; set; }

        public string Errors { get; set; }

        public DrawingType Type { get; set; }

        public string UserDefined1 { get; set; }

        public string UserDefined2 { get; set; }

        public string UserDefined3 { get; set; }

        public string UserDefined4 { get; set; }

        public string UserDefined5 { get; set; }

        public string UserDefined6 { get; set; }

        public static DrawingInfo Load(string nestFile)
        {
            var reader = new DrawingInfoReader();
            reader.Read(nestFile);
            return reader.Info;
        }

        public static DrawingInfo Load(Stream stream)
        {
            var reader = new DrawingInfoReader();
            reader.Read(stream);
            return reader.Info;
        }

        public static bool TryLoad(string nestfile, out DrawingInfo drawingInfo)
        {
            try
            {
                drawingInfo = Load(nestfile);
            }
            catch (Exception)
            {
                drawingInfo = null;
                return false;
            }

            return true;
        }

        public static bool TryLoad(Stream stream, out DrawingInfo drawingInfo)
        {
            try
            {
                drawingInfo = Load(stream);
            }
            catch (Exception)
            {
                drawingInfo = null;
                return false;
            }

            return true;
        }
    }
}

