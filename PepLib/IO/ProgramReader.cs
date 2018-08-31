using System.Collections.Generic;
using System.IO;
using System.Text;
using PepLib.Codes;

namespace PepLib.IO
{
    internal sealed class ProgramReader
    {
        private const int BufferSize = 200;
        private int codeIndex;
        private CodeBlock block;
        private CodeSection section;

        public Program Program { get; private set; }

        public ProgramReader()
        {
            Program = new Program();
        }

        public ProgramReader(Program program)
        {
            Program = program;
        }

        public void Read(Stream stream)
        {
            foreach (var line in GetLines(stream))
            {
                block = ParseBlock(line);
                ProcessCurrentBlock();
            }
        }

        private IEnumerable<string> GetLines(Stream stream)
        {
            var buffer = new byte[BufferSize];

            while (stream.Read(buffer, 0, BufferSize) > 0)
            {
                yield return Encoding.ASCII.GetString(buffer);
            }
        }

        private CodeBlock ParseBlock(string line)
        {
            var block = new CodeBlock();
            Code code = null;

            for (int i = 0; i < line.Length; ++i)
            {
                var c = line[i];

                if (char.IsLetter(c))
                    block.Add((code = new Code(c)));
                else if (c == ':')
                {
                    block.Add((new Code(c, line.Remove(0, i + 1).Trim())));
                    break;
                }
                else if (code != null)
                    code.Value += c;
            }

            return block;
        }

        private void ProcessCurrentBlock()
        {
            var code = GetFirstCode();

            while (code != null)
            {
                switch (code.Id)
                {
                    case ':':
                        Program.Add(new Comment(code.Value));
                        code = GetNextCode();
                        break;

                    case 'G':
                        int value = int.Parse(code.Value);

                        switch (value)
                        {
                            case 0:
                            case 1:
                                section = CodeSection.Line;
                                ReadLine(value == 0);
                                code = GetCurrentCode();
                                break;

                            case 2:
                            case 3:
                                section = CodeSection.Arc;
                                ReadArc(value == 2 ? RotationType.CW : RotationType.CCW);
                                code = GetCurrentCode();
                                break;

                            case 92:
                                section = CodeSection.SubProgram;
                                ReadSubProgram();
                                code = GetCurrentCode();
                                break;

                            case 40:
                                Program.Add(new SetKerf() { Kerf = KerfType.None });
                                code = GetNextCode();
                                break;

                            case 41:
                                Program.Add(new SetKerf() { Kerf = KerfType.Left });
                                code = GetNextCode();
                                break;

                            case 42:
                                Program.Add(new SetKerf() { Kerf = KerfType.Right });
                                code = GetNextCode();
                                break;

                            default:
                                code = GetNextCode();
                                break;
                        }
                        break;

                    case 'F':
                        Program.Add(new SetFeedrate() { Value = double.Parse(code.Value) });
                        code = GetNextCode();
                        break;

                    default:
                        code = GetNextCode();
                        break;
                }
            }
        }

        private void ReadLine(bool isRapid)
        {
            double x = 0;
            double y = 0;
            var type = EntityType.Cut;

            while (section == CodeSection.Line)
            {
                var code = GetNextCode();

                if (code == null)
                {
                    section = CodeSection.Unknown;
                    break;
                }

                switch (code.Id)
                {
                    case 'X':
                        x = double.Parse(code.Value);
                        break;

                    case 'Y':
                        y = double.Parse(code.Value);
                        break;

                    case ':':
                    {
                        var value = code.Value.Trim().ToUpper();

                        switch (value)
                        {
                            case "EXTERNAL LEAD-IN":
                                type = EntityType.ExternalLeadin;
                                break;

                            case "EXTERNAL LEAD-OUT":
                                type = EntityType.ExternalLeadout;
                                break;

                            case "INTERNAL LEAD-IN":
                                type = EntityType.InternalLeadin;
                                break;

                            case "INTERNAL LEAD-OUT":
                                type = EntityType.InternalLeadout;
                                break;

                            case "DISPLAY":
                                type = EntityType.Display;
                                break;
                        }
                        break;
                    }

                    default:
                        section = CodeSection.Unknown;
                        break;
                }
            }

            if (isRapid)
                Program.Add(new RapidMove(x, y));
            else
                Program.Add(new LinearMove(x, y) { Type = type });
        }

        private void ReadArc(RotationType rotation)
        {
            double x = 0;
            double y = 0;
            double i = 0;
            double j = 0;
            var type = EntityType.Cut;

            while (section == CodeSection.Arc)
            {
                var code = GetNextCode();

                if (code == null)
                {
                    section = CodeSection.Unknown;
                    break;
                }

                switch (code.Id)
                {
                    case 'X':
                        x = double.Parse(code.Value);
                        break;

                    case 'Y':
                        y = double.Parse(code.Value);
                        break;

                    case 'I':
                        i = double.Parse(code.Value);
                        break;

                    case 'J':
                        j = double.Parse(code.Value);
                        break;

                    case ':':
                        {
                            var value = code.Value.Trim().ToUpper();

                            switch (value)
                            {
                                case "EXTERNAL LEAD-IN":
                                    type = EntityType.ExternalLeadin;
                                    break;

                                case "EXTERNAL LEAD-OUT":
                                    type = EntityType.ExternalLeadout;
                                    break;

                                case "INTERNAL LEAD-IN":
                                    type = EntityType.InternalLeadin;
                                    break;

                                case "INTERNAL LEAD-OUT":
                                    type = EntityType.InternalLeadout;
                                    break;

                                case "DISPLAY":
                                    type = EntityType.Display;
                                    break;
                            }
                            break;
                        }

                    default:
                        section = CodeSection.Unknown;
                        break;
                }
            }

            Program.Add(new CircularMove()
            {
                EndPoint = new Vector(x, y),
                CenterPoint = new Vector(i, j),
                Rotation = rotation,
                Type = type
            });
        }

        private void ReadSubProgram()
        {
            int l = 0;
            int r = 0;
            double p = 0;

            while (section == CodeSection.SubProgram)
            {
                var code = GetNextCode();

                if (code == null)
                {
                    section = CodeSection.Unknown;
                    break;
                }

                switch (code.Id)
                {
                    case 'L':
                        l = int.Parse(code.Value);
                        break;

                    case 'R':
                        r = int.Parse(code.Value);
                        break;

                    case 'P':
                        p = double.Parse(code.Value);
                        break;

                    default:
                        section = CodeSection.Unknown;
                        break;
                }
            }

            Program.Add(new SubProgramCall() { LoopId = l, RepeatCount = r, Rotation = p });
        }

        private Code GetNextCode()
        {
            codeIndex++;

            if (codeIndex >= block.Count)
                return null;

            return block[codeIndex];
        }

        private Code GetCurrentCode()
        {
            if (codeIndex >= block.Count)
                return null;

            return block[codeIndex];
        }

        private Code GetFirstCode()
        {
            if (block.Count == 0)
                return null;

            codeIndex = 0;

            return block[codeIndex];
        }

        private class Code
        {
            public Code(char id)
            {
                Id = id;
                Value = string.Empty;
            }

            public Code(char id, string value)
            {
                Id = id;
                Value = value;
            }

            public char Id { get; private set; }

            public string Value { get; set; }

            public override string ToString()
            {
                return Id + Value;
            }
        }

        private class CodeBlock : List<Code>
        {
            public void Add(char id, string value)
            {
                Add(new Code(id, value));
            }

            public override string ToString()
            {
                var builder = new StringBuilder();

                foreach (var code in this)
                    builder.Append(code.ToString() + " ");

                return builder.ToString();
            }
        }

        private enum CodeSection
        {
            Unknown,
            Arc,
            Line,
            SubProgram
        }
    }
}
