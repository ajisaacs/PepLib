﻿using System.IO;
using PepLib.Codes;

namespace PepLib.IO
{
    internal sealed class PlateReader
    {
        public Plate Plate { get; private set; }

        public PlateReader()
        {
            Plate = new Plate();
            Plate.Duplicates = 1;
        }

        public PlateReader(Plate plate)
        {
            Plate = plate;
        }

        public void Read(string name, Stream stream, Nest nest)
        {
            var pos = new Vector(0, 0);
            var pgm = Program.Load(stream);

            Plate.Name = name;

            for (int i = 0; i < pgm.Count; i++)
            {
                var block = pgm[i];

                switch (block.CodeType())
                {
                    case CodeType.CircularMove:
                        {
                            var arc = (CircularMove)block;
                            pos = arc.EndPoint;
                            break;
                        }

                    case CodeType.LinearMove:
                        {
                            var line = (LinearMove)block;
                            pos = line.EndPoint;
                            break;
                        }

                    case CodeType.RapidMove:
                        {
                            var rapid = (RapidMove)block;
                            pos = rapid.EndPoint;
                            break;
                        }

                    case CodeType.Comment:
                        {
                            var comment = (Comment)block;
                            LoadInfo(comment.Value);
                            break;
                        }

                    case CodeType.SubProgramCall:
                        {
                            var subpgm = (SubProgramCall)block;
                            var loop = nest.GetLoop(subpgm.LoopId);
                            var part = Part.Create(loop, pos, AngleConverter.ToRadians(subpgm.Rotation));

                            var nextBlock = pgm[i + 1];

                            if (nextBlock.CodeType() == CodeType.Comment)
                            {
                                var comment = nextBlock as Comment;
                                
                                if (comment.Value == "DISPLAY ONLY")
                                {
                                    part.IsDisplayOnly = true;
                                    i++;
                                }
                            }

                            Plate.Parts.Add(part);
                            break;
                        }
                }
            }
        }

        private void LoadInfo(string value)
        {
            if (value.StartsWith("POSTED FILES"))
                ParsePostedFiles(value);

            else if (value.StartsWith("HEAT LOT"))
                ParseHeatLot(value);

            else if (value.StartsWith("SPACING"))
                ParseSpacing(value);

            else if (value.StartsWith("CUT A TOTAL OF "))
                ParseNumberOfDuplicates(value);

            else if (value.StartsWith("EDGES,"))
                ParseEdgeSpacing(value);

            else if (value.StartsWith("PLATE SCALING"))
                ParsePlateSize(value);

            else if (value.StartsWith("MACHINE"))
                ParseMachine(value);

            else if (value.StartsWith("MATERIAL"))
                ParseMaterial(value);

            else if (value.StartsWith("GRADE"))
                ParseGrade(value);

            else if (value.StartsWith("DESCRIPTION"))
                ParseDescription(value);

            else if (value.StartsWith("PLATE THICKNESS"))
                ParseThickness(value);

            else if (value.StartsWith("DENSITY"))
                ParseDensity(value);

            else if (value.StartsWith("TORCHES"))
                ParseTorchCount(value);
        }

        private void ParseNumberOfDuplicates(string data)
        {
            var parts = data.Split(' ');

            if (parts.Length != 7)
                return;

            int dup;
            int.TryParse(parts[4], out dup);

            Plate.Duplicates = dup;
        }

        private void ParsePostedFiles(string data)
        {
            if (data.Length < 14)
                return;

            Plate.PostedFiles = data.Remove(0, 14).Trim();
        }

        private void ParseHeatLot(string data)
        {
            if (data.Length < 9)
                return;

            Plate.HeatLot = data.Remove(0, 9).Trim();
        }

        private void ParseSpacing(string data)
        {
            var parts = data.Split('=');

            if (parts.Length != 2)
                return;

            double spacing;
            double.TryParse(parts[1], out spacing);

            Plate.PartSpacing = spacing;
        }

        private void ParseEdgeSpacing(string data)
        {
            var parts = data.Split(',');

            if (parts.Length != 5)
                return;

            var leftSplit = parts[1].Split('=');
            if (leftSplit.Length == 2)
            {
                double x;
                double.TryParse(leftSplit[1], out x);
                Plate.EdgeSpacing.Left = x;
            }

            var bottomSplit = parts[2].Split('=');
            if (bottomSplit.Length == 2)
            {
                double x;
                double.TryParse(bottomSplit[1], out x);
                Plate.EdgeSpacing.Bottom = x;
            }

            var rightSplit = parts[3].Split('=');
            if (rightSplit.Length == 2)
            {
                double x;
                double.TryParse(rightSplit[1], out x);
                Plate.EdgeSpacing.Right = x;
            }

            var topSplit = parts[4].Split('=');
            if (topSplit.Length == 2)
            {
                double x;
                double.TryParse(topSplit[1], out x);
                Plate.EdgeSpacing.Top = x;
            }
        }

        private void ParsePlateSize(string data)
        {
            var quadrantIndex = data.IndexOf("QUADRANT");

            if (quadrantIndex != -1)
            {
                var plateData = data.Remove(quadrantIndex);
                var plateDataSplit = plateData.Split('=');
                if (plateDataSplit.Length == 2)
                {
                    Size plateSize;
                    Size.TryParse(plateDataSplit[1], out plateSize);
                    Plate.Size = plateSize;
                }

                var quadrantData = data.Remove(0, quadrantIndex);
                var quadrantDataSplit = quadrantData.Split('=');
                if (quadrantDataSplit.Length == 2)
                {
                    int quadrant;
                    int.TryParse(quadrantDataSplit[1], out quadrant);
                    Plate.Quadrant = quadrant;
                }
            }
            else
            {
                var plateDataSplit = data.Split('=');
                if (plateDataSplit.Length == 2)
                {
                    Size plateSize;
                    Size.TryParse(plateDataSplit[1], out plateSize);
                    Plate.Size = plateSize;
                }
            }
        }

        private void ParseMachine(string data)
        {
            var parts = data.Split(',');

            if (parts.Length != 2)
                return;

            var machineSplit = parts[0].Split('=');
            if (machineSplit.Length == 2)
            {
                int num;
                int.TryParse(machineSplit[1], out num);
                Plate.Machine.Id = num;
            }

            Plate.Machine.Name = parts[1].Trim();
        }

        private void ParseMaterial(string data)
        {
            var parts = data.Split('=');

            if (parts.Length != 2)
                return;

            int material;
            int.TryParse(parts[1], out material);

            Plate.Material.Id = material;
        }

        private void ParseGrade(string data)
        {
            var parts = data.Split('=');

            if (parts.Length != 2)
                return;

            Plate.Material.Grade = parts[1].Trim();
        }

        private void ParseDescription(string data)
        {
            var parts = data.Split('=');

            if (parts.Length != 2)
                return;

            Plate.Description = parts[1].Trim();
        }

        private void ParseThickness(string data)
        {
            var parts = data.Split('=');

            if (parts.Length != 2)
                return;

            double thickness;
            double.TryParse(parts[1], out thickness);

            Plate.Thickness = thickness;
        }

        private void ParseDensity(string data)
        {
            var parts = data.Split('=');

            if (parts.Length != 2)
                return;

            double density;
            double.TryParse(parts[1], out density);

            Plate.Material.Density = density;
        }

        private void ParseTorchCount(string data)
        {
            var parts = data.Split('=');

            if (parts.Length != 2)
                return;

            int torchCount;
            int.TryParse(parts[1], out torchCount);

            Plate.TorchCount = torchCount;
        }
    }
}