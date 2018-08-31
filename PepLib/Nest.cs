using System;
using System.Collections.Generic;
using System.IO;
using PepLib.Codes;
using PepLib.IO;

namespace PepLib
{
    public class Nest
    {
        public Nest()
        {
            Info = new NestInfo();
            Report = new Report();
            Loops = new List<Loop>();
            Plates = new List<Plate>();
            Drawings = new List<NestDrawing>();
        }

        public NestInfo Info { get; set; }

        public Report Report { get; set; }

        public List<Loop> Loops { get; set; }

        public List<Plate> Plates { get; set; }

        public List<NestDrawing> Drawings { get; set; }

        public void ResolveLoops()
        {
            for (int i = 0; i < Loops.Count; ++i)
            {
                var loop = Loops[i];
                ResolveLoops(loop);
            }
        }

        private void ResolveLoops(Program pgm)
        {
            for (int i = 0; i < pgm.Count; ++i)
            {
                var code = pgm[i];

                if (code.CodeType() != CodeType.SubProgramCall)
                    continue;

                var subpgmcall = (SubProgramCall)code;

                var loop = GetLoop(subpgmcall.LoopId);

                if (loop == null)
                    throw new Exception("Loop not found");

                subpgmcall.Loop = loop;
            }
        }

        public int GetQtyNested(string drawing)
        {
            int qty = 0;

            foreach (var plate in Plates)
                qty += plate.GetQtyNested(drawing);

            return qty;
        }

        private string GetLoopName(int loopId)
        {
            return string.Format("{0}.loop-{1}", Info.Name, loopId.ToString().PadLeft(3, '0')); 
        }

        private Loop GetLoop(string name)
        {
            for (int i = 0; i < Loops.Count; ++i)
            {
                if (Loops[i].Name == name)
                    return Loops[i];
            }

            return null;
        }

        public Loop GetLoop(int id)
        {
            string name = GetLoopName(id);
            return GetLoop(name);
        }

        public static Nest Load(string nestfile)
        {
            var reader = new NestReader();
            reader.Read(nestfile);
            return reader.Nest;
        }

        public static Nest Load(Stream stream)
        {
            var reader = new NestReader();
            reader.Read(stream);
            return reader.Nest;
        }

        public static bool TryLoad(string nestfile, out Nest nest)
        {
            try
            {
                nest = Load(nestfile);
            }
            catch (Exception)
            {
                nest = null;
                return false;
            }

            return true;
        }

        public static bool TryLoad(Stream stream, out Nest nest)
        {
            try
            {
                nest = Load(stream);
            }
            catch (Exception)
            {
                nest = null;
                return false;
            }

            return true;
        }

        #region NestInfo wrapper properties

        public string Name
        {
            get { return Info.Name; }
            set { Info.Name = value; }
        }

        public DateTime CreationDate
        {
            get { return Info.DateCreated; }
            set { Info.DateCreated = value; }
        }

        public DateTime LastModifiedDate
        {
            get { return Info.DateLastModified; }
            set { Info.DateLastModified = value; }
        }

        public StatusType Status
        {
            get { return Info.Status; }
            set { Info.Status = value; }
        }

        public int LoopCount
        {
            get { return Info.LoopCount; }
            set { Info.LoopCount = value; }
        }

        public int PlateCount
        {
            get { return Info.PlateCount; }
            set { Info.PlateCount = value; }
        }

        public string Comment
        {
            get { return Info.Comments; }
            set { Info.Comments = value; }
        }

        public string Customer
        {
            get { return Info.Customer; }
            set { Info.Customer = value; }
        }

        public string ProgrammedBy
        {
            get { return Info.ProgrammedBy; }
            set { Info.ProgrammedBy = value; }
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

        public string Notes
        {
            get { return Info.Notes; }
            set { Info.Notes = value; }
        }

        public string DefaultPlateSize
        {
            get { return Info.DefaultPlateSize; }
            set { Info.DefaultPlateSize = value; }
        }

        public string Kerf
        {
            get { return Info.Kerf; }
            set { Info.Kerf = value; }
        }

        public string PostedAs
        {
            get { return Info.PostedAs; }
            set { Info.PostedAs = value; }
        }

        public string Errors
        {
            get { return Info.Errors; }
            set { Info.Errors = value; }
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
