﻿using System;
using System.IO;
using PepLib.Codes;

namespace PepLib.IO
{
    internal sealed class LoopReader
    {
        public Loop Loop { get; private set; }

        public LoopReader()
        {
            Loop = new Loop();
        }

        public LoopReader(Loop loop)
        {
            Loop = loop;
        }

        public void Read(string name, Stream stream)
        {
            var pgm = Program.Load(stream);

            Loop.Name = name;
            Loop.AddRange(pgm);
            LoadInfo();
        }

        private void LoadInfo()
        {
            for (int i = Loop.Count - 1; i >= 0; --i)
            {
                var code = Loop[i];

                if (code.CodeType() != CodeType.Comment)
                    continue;

                var comment = (Comment)code;

                if (LoadInfo(comment.Value))
                    Loop.RemoveAt(i);
            }
        }

        private bool LoadInfo(string value)
        {
            if (value.StartsWith("REF"))
            {
                ParseReferenceData(value);
                return true;
            }

            if (value.StartsWith("DRAWING"))
            {
                ParseDrawingData(value);
                return true;
            }

            if (value.StartsWith("DXF"))
            {
                ParseDxfData(value);
                return true;
            }

            return false;
        }

        private void ParseReferenceData(string data)
        {
            var parts = data.Split(',');

            if (parts.Length != 3)
                return;

            int xindex = parts[0].IndexOf('X');
            parts[0] = parts[0].Remove(0, xindex);

            double x = 0;
            double y = 0;

            var xsplit = parts[0].Split('=');

            if (xsplit.Length == 2)
                x = ReadDouble(xsplit[1]);

            var ysplit = parts[1].Split('=');

            if (ysplit.Length == 2)
                y = ReadDouble(ysplit[1]);

            var datesplit = parts[2].Split('=');

            if (datesplit.Length == 2)
            {
                DateTime date;
                DateTime.TryParse(datesplit[1], out date);
                Loop.LastReferenceDate = date;
            }

            Loop.ReferencePoint = new Vector(x, y);
        }

        private void ParseDrawingData(string data)
        {
            var index = data.IndexOf('=');

            if (index == -1)
                Loop.DrawingName = string.Empty;

            Loop.DrawingName = data.Remove(0, index + 1).Trim();
        }

        private void ParseDxfData(string data)
        {
            var index = data.IndexOf('=');

            if (index == -1)
                Loop.DxfPath = string.Empty;

            Loop.DxfPath = data.Remove(0, index + 1).Trim();
        }

        private static double ReadDouble(string s, double defaultValue = 0.0)
        {
            double f;

            if (!double.TryParse(s, out f))
                return defaultValue;

            return f;
        }
    }
}