using System;
using System.Collections.Generic;
using System.IO;
using PepLib.IO;

namespace PepLib
{
    public class Drawing
    {
        public DrawingInfo Info { get; set; }

        public List<Loop> Loops { get; set; }

        public Drawing()
        {
            Loops = new List<Loop>();
        }

        public static Drawing Load(string nestfile)
        {
            var reader = new DrawingReader();
            reader.Read(nestfile);
            return reader.Drawing;
        }

        public static Drawing Load(Stream stream)
        {
            var reader = new DrawingReader();
            reader.Read(stream);
            return reader.Drawing;
        }

        public static bool TryLoad(string nestfile, out Drawing drawing)
        {
            try
            {
                drawing = Load(nestfile);
            }
            catch (Exception)
            {
                drawing = null;
                return false;
            }

            return true;
        }

        public static bool TryLoad(Stream stream, out Drawing drawing)
        {
            try
            {
                drawing = Load(stream);
            }
            catch (Exception)
            {
                drawing = null;
                return false;
            }

            return true;
        }

        #region DrawingInfo wrapper properties

        public string Name
        {
            get { return Info.Name; }
            set { Info.Name = value; }
        }

        public string Revision
        {
            get { return Info.Revision; }
            set { Info.Revision = value; }
        }

        public string Customer
        {
            get { return Info.Customer; }
            set { Info.Customer = value; }
        }

        public string Description
        {
            get { return Info.Description; }
            set { Info.Description = value; }
        }

        public string Comment
        {
            get { return Info.Comment; }
            set { Info.Comment = value; }
        }

        public string Notes
        {
            get { return Info.Notes; }
            set { Info.Notes = value; }
        }

        public string Source
        {
            get { return Info.Source; }
            set { Info.Source = value; }
        }

        public DateTime CreationDate
        {
            get { return Info.CreationDate; }
            set { Info.CreationDate = value; }
        }

        public DateTime LastModifiedDate
        {
            get { return Info.LastModifiedDate; }
            set { Info.LastModifiedDate = value; }
        }

        public DateTime LastReferenceDate
        {
            get { return Info.LastReferenceDate; }
            set { Info.LastReferenceDate = value; }
        }

        public int MachineNumber
        {
            get { return Info.MachineNumber; }
            set { Info.MachineNumber = value; }
        }

        public ApplicationType Application
        {
            get { return Info.Application; }
            set { Info.Application = value; }
        }

        public int MaterialNumber
        {
            get { return Info.MaterialNumber; }
            set { Info.MaterialNumber = value; }
        }

        public string MaterialGrade
        {
            get { return Info.MaterialGrade; }
            set { Info.MaterialGrade = value; }
        }

        public string Specification
        {
            get { return Info.Specification; }
            set { Info.Specification = value; }
        }

        public string Hardness
        {
            get { return Info.Hardness; }
            set { Info.Hardness = value; }
        }

        public GrainType Grain
        {
            get { return Info.Grain; }
            set { Info.Grain = value; }
        }

        public string ProgrammedBy
        {
            get { return Info.ProgrammedBy; }
            set { Info.ProgrammedBy = value; }
        }

        public string CreatedBy
        {
            get { return Info.CreatedBy; }
            set { Info.CreatedBy = value; }
        }

        public string Errors
        {
            get { return Info.Errors; }
            set { Info.Errors = value; }
        }

        public DrawingType Type
        {
            get { return Info.Type; }
            set { Info.Type = value; }
        }

        public string UserDefined1
        {
            get { return Info.UserDefined1; }
            set { Info.UserDefined1 = value; }
        }

        public string UserDefined2
        {
            get { return Info.UserDefined2; }
            set { Info.UserDefined2 = value; }
        }

        public string UserDefined3
        {
            get { return Info.UserDefined3; }
            set { Info.UserDefined3 = value; }
        }

        public string UserDefined4
        {
            get { return Info.UserDefined4; }
            set { Info.UserDefined4 = value; }
        }

        public string UserDefined5
        {
            get { return Info.UserDefined5; }
            set { Info.UserDefined5 = value; }
        }

        public string UserDefined6
        {
            get { return Info.UserDefined6; }
            set { Info.UserDefined6 = value; }
        }

        #endregion
    }
}
