using PepLib;
using PepLib.Codes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pep.Controls
{
    public class LayoutView : Control
    {
        private float scale;

        private const int BorderWidth = 50;
        private const float ZoomInFactor = 1.3f;
        private const float ZoomOutFactor = 1.0f / ZoomInFactor;

        private bool pathsNeedUpdated;
        private GraphicsPath[] paths;

        private PointF origin;
        private Point lastPoint;

        private readonly Brush layoutFillBrush;
        private readonly Brush loopFillBrush;

        private readonly Pen layoutBorderPen;
        private readonly Pen loopBorderPen;
        private readonly Pen rapidPen;
        private readonly Pen originPen;
        private readonly Pen edgeSpacingPen;
        private ToolTip tooltip;
        private bool isPanning;

        private string selectedDrawing;

        private readonly Font loopIdFont;
        private Vector curpos;
        private ProgrammingMode mode;

        public Vector CurrentPoint { get; private set; }

        public Plate Plate { get; set; }

        public LayoutView()
        {
            paths = new GraphicsPath[0];

            layoutFillBrush = new SolidBrush(Color.White);
            loopFillBrush = new SolidBrush(Color.FromArgb(130, 204, 130));
            loopFillBrush = new SolidBrush(Color.FromArgb(130, 204, 130));

            tooltip = new ToolTip();
            tooltip.UseAnimation = false;
            tooltip.UseFading = false;

            layoutBorderPen = new Pen(Color.Gray);
            loopBorderPen = new Pen(Color.Green);
            rapidPen = new Pen(Color.DodgerBlue) { DashPattern = new float[] { 10, 10 } };
            originPen = new Pen(Color.Gray);
            edgeSpacingPen = new Pen(Color.FromArgb(180, 180, 180)) { DashPattern = new float[] { 3, 3 } };

            loopIdFont = new Font(DefaultFont, FontStyle.Bold | FontStyle.Underline);

            scale = 1.0f;
            origin = new PointF();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint, true);

            DrawRapid = false;
            DrawBounds = false;
            FillParts = true;
            Plate = new Plate(60, 120);

            Cursor = Cursors.Cross;

            pathsNeedUpdated = true;
        }

        public bool DrawRapid { get; set; }

        public bool DrawBounds { get; set; }

        public bool FillParts { get; set; }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ZoomToFit();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            float multiplier = Math.Abs(e.Delta / 120.0f);

            if (e.Delta > 0)
                ZoomToPoint(e.Location, (float)Math.Pow(ZoomInFactor, multiplier));
            else
                ZoomToPoint(e.Location, (float)Math.Pow(ZoomOutFactor, multiplier));

            pathsNeedUpdated = true;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Location == lastPoint)
                return;

            CurrentPoint = PointControlToWorld(e.Location);

            if (e.Button == MouseButtons.Middle)
            {
                var diffx = e.X - lastPoint.X;
                var diffy = e.Y - lastPoint.Y;

                origin.X += diffx;
                origin.Y += diffy;

                Invalidate();
            }

            lastPoint = e.Location;

            ShowTooltipForPartAtLocation(e.Location);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                isPanning = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                isPanning = false;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            Focus();

            if (e.Button == MouseButtons.Left)
            {
                var part = GetPartAtPoint(e.Location);

                selectedDrawing = part?.DrawingName;
                Invalidate();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button == MouseButtons.Middle)
                ZoomToFit();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            OnKeyDown(new KeyEventArgs(keyData));
            return base.ProcessDialogKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                selectedDrawing = null;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            curpos = new Vector();
            e.Graphics.TranslateTransform(origin.X, origin.Y);
            DrawPlate(e.Graphics);
            DrawOrigin(e.Graphics);
        }

        private void DrawPlateInfo(Graphics g)
        {
            var font = new Font(Font.FontFamily, 10, FontStyle.Bold | FontStyle.Italic);

            var location = new PointF(5, -LengthWorldToGui(Plate.Size.Height) - font.GetHeight());

            g.DrawString(Plate.Size.ToString() + ", Qty: " + Plate.Duplicates, font, Brushes.Gray, location);
        }

        private void ShowTooltipForPartAtLocation(Point pt)
        {
            if (isPanning)
            {
                tooltip.Hide(this);
                return;
            }

            var part = GetPartAtPoint(pt);

            if (part == null)
            {
                tooltip.Hide(this);
                return;
            }

            tooltip.ToolTipTitle = part.DrawingName;

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Qty on Sheet:\t{0}", Plate.GetQtyNested(part.DrawingName)));
            sb.AppendLine(string.Format("Loop Name:\t{0}", part.Name));
            sb.AppendLine(string.Format("Location:\t({0}, {1})", part.Location.X, part.Location.Y));
            sb.AppendLine(string.Format("Rotation:\t{0}", AngleConverter.ToDegrees(part.Rotation)));
            tooltip.Show(sb.ToString(), this, pt.X + 2, pt.Y + 2);
        }

        private void DrawOrigin(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var w = 7.0f;
            var hw = w / 2.0f;

            var rect = new RectangleF(
                -hw,
                -hw,
                w, w);

            g.FillEllipse(Brushes.Orange, rect);
            g.DrawEllipse(Pens.Red, rect);

            g.SmoothingMode = SmoothingMode.HighSpeed;
        }

        private void DrawPlate(Graphics g)
        {
            Debug.WriteLine(Plate.Size.ToString());
            var plateRect = new RectangleF
            {
                Width = LengthWorldToGui(Plate.Size.Width),
                Height = LengthWorldToGui(Plate.Size.Height)
            };

            var edgeSpacingRect = new RectangleF
            {
                Width = LengthWorldToGui(Plate.Size.Width - Plate.EdgeSpacing.Left - Plate.EdgeSpacing.Right),
                Height = LengthWorldToGui(Plate.Size.Height - Plate.EdgeSpacing.Top - Plate.EdgeSpacing.Bottom)
            };

            switch (Plate.Quadrant)
            {
                case 1:
                    plateRect.Location = PointWorldToGraph(0, 0);
                    edgeSpacingRect.Location = PointWorldToGraph(
                        Plate.EdgeSpacing.Left,
                        Plate.EdgeSpacing.Bottom);
                    break;

                case 2:
                    plateRect.Location = PointWorldToGraph(-Plate.Size.Width, 0);
                    edgeSpacingRect.Location = PointWorldToGraph(
                        Plate.EdgeSpacing.Left - Plate.Size.Width,
                        Plate.EdgeSpacing.Bottom);
                    break;

                case 3:
                    plateRect.Location = PointWorldToGraph(-Plate.Size.Width, -Plate.Size.Height);
                    edgeSpacingRect.Location = PointWorldToGraph(
                        Plate.EdgeSpacing.Left - Plate.Size.Width,
                        Plate.EdgeSpacing.Bottom - Plate.Size.Height);
                    break;

                case 4:
                    plateRect.Location = PointWorldToGraph(0, -Plate.Size.Height);
                    edgeSpacingRect.Location = PointWorldToGraph(
                        Plate.EdgeSpacing.Left,
                        Plate.EdgeSpacing.Bottom - Plate.Size.Height);
                    break;

                default:
                    return;
            }

            plateRect.Y -= plateRect.Height;
            edgeSpacingRect.Y -= edgeSpacingRect.Height;

            g.FillRectangle(layoutFillBrush, plateRect);

            var viewBounds = new RectangleF(-origin.X, -origin.Y, Width, Height);

            if (!edgeSpacingRect.Contains(viewBounds))
            {
                g.DrawRectangle(edgeSpacingPen,
                   edgeSpacingRect.X,
                   edgeSpacingRect.Y,
                   edgeSpacingRect.Width,
                   edgeSpacingRect.Height);
            }

            g.DrawRectangle(layoutBorderPen,
                plateRect.X,
                plateRect.Y,
                plateRect.Width,
                plateRect.Height);

            DrawParts(g);
            DrawPlateInfo(g);
        }

        private void DrawParts(Graphics g)
        {
            if (pathsNeedUpdated || paths.Length != Plate.Parts.Count)
                UpdatePaths();

            var viewBounds = new RectangleF(-origin.X, -origin.Y, Width, Height);

            for (int i = 0; i < Plate.Parts.Count; ++i)
            {
                var part = Plate.Parts[i];
                var path = paths[i];
                var pathBounds = path.GetBounds();

                if (!pathBounds.IntersectsWith(viewBounds))
                    continue;

                if (DrawBounds)
                    DrawBox(g, part.BoundingBox);

                if (part.DrawingName == selectedDrawing)
                {
                    var brush = new SolidBrush(Color.Aqua);
                    var pen = new Pen(Color.Blue);

                    if (FillParts)
                        g.FillPath(brush, path);

                    g.DrawPath(pen, path);
                }
                else
                {
                    if (FillParts)
                        g.FillPath(loopFillBrush, path);

                    g.DrawPath(loopBorderPen, path);
                }

                var pt = PointWorldToGraph(part.AbsoluteReferencePoint);

                g.DrawString((i + 1).ToString(), loopIdFont, Brushes.Black, pt.X, pt.Y);
            }

            if (DrawRapid)
                DrawRapids(g);
        }

        private void DrawProgram(GraphicsPath g, PepLib.Program pgm)
        {
            mode = pgm.Mode;

            foreach (var code in pgm)
            {
                switch (code.CodeType())
                {
                    case CodeType.CircularMove:
                        {
                            var arc = (CircularMove)code;
                            DrawArc(g, arc);
                            break;
                        }

                    case CodeType.LinearMove:
                        {
                            var line = (LinearMove)code;
                            DrawLine(g, line);
                            break;
                        }

                    case CodeType.RapidMove:
                        {
                            var rapid = (RapidMove)code;
                            DrawLine(g, rapid);
                            break;
                        }

                    case CodeType.SubProgramCall:
                        {
                            var tmpmode = mode;
                            var subpgm = (SubProgramCall)code;

                            if (subpgm.Loop != null)
                            {
                                g.StartFigure();
                                DrawProgram(g, subpgm.Loop);
                            }

                            mode = tmpmode;
                            break;
                        }
                }
            }
        }

        private void DrawLine(GraphicsPath g, LinearMove line)
        {
            var pt = line.EndPoint;

            if (mode == ProgrammingMode.Incremental)
                pt += curpos;

            var pt1 = PointWorldToGraph(curpos);
            var pt2 = PointWorldToGraph(pt);

            g.AddLine(pt1, pt2);

            if (line.Type == EntityType.ExternalLeadin || line.Type == EntityType.ExternalLeadout ||
                line.Type == EntityType.InternalLeadin || line.Type == EntityType.InternalLeadout)
            {
                g.CloseFigure();
            }

            curpos = pt;
        }

        private void DrawLine(GraphicsPath g, RapidMove line)
        {
            var pt = line.EndPoint;

            if (mode == ProgrammingMode.Incremental)
                pt += curpos;

            g.CloseFigure();

            curpos = pt;
        }

        private void DrawArc(GraphicsPath g, CircularMove arc)
        {
            var endpt = arc.EndPoint;
            var center = arc.CenterPoint;

            if (mode == ProgrammingMode.Incremental)
            {
                endpt += curpos;
                center += curpos;
            }

            // start angle in degrees
            var startAngle = AngleConverter.ToDegrees(Math.Atan2(
                curpos.Y - center.Y,
                curpos.X - center.X));

            // end angle in degrees
            var endAngle = AngleConverter.ToDegrees(Math.Atan2(
                endpt.Y - center.Y,
                endpt.X - center.X));

            endAngle = NormalizeAngle(endAngle);
            startAngle = NormalizeAngle(startAngle);

            if (arc.Rotation == RotationType.CCW && endAngle < startAngle)
                endAngle += 360.0;
            else if (arc.Rotation == RotationType.CW && startAngle < endAngle)
                startAngle += 360.0;

            var dx = endpt.X - center.X;
            var dy = endpt.Y - center.Y;

            var radius = Math.Sqrt(dx * dx + dy * dy);

            var pt = PointWorldToGraph(center.X - radius, center.Y + radius);
            var size = LengthWorldToGui(radius * 2.0);

            if (startAngle.IsEqualTo(endAngle))
            {
                g.AddEllipse(pt.X, pt.Y, size, size);
            }
            else
            {
                var sweepAngle = -(endAngle - startAngle);

                g.AddArc(pt.X, pt.Y, size, size,
                    (float)-startAngle, (float)sweepAngle);

                if (arc.Type == EntityType.ExternalLeadin || arc.Type == EntityType.ExternalLeadout ||
                    arc.Type == EntityType.InternalLeadin || arc.Type == EntityType.InternalLeadout)
                {
                    // hack to not have the graphics path fill the leadin/leadout area.
                    g.AddArc(pt.X, pt.Y, size, size, (float)(-startAngle + sweepAngle), (float)-sweepAngle);
                    g.CloseFigure();
                }
            }

            curpos = endpt;
        }

        private void DrawRapids(Graphics g)
        {
            var pos = new Vector(0, 0);

            for (int i = 0; i < Plate.Parts.Count; ++i)
            {
                var part = Plate.Parts[i];
                var pgm = part.Program;

                DrawLine(g, pos, part.Location, rapidPen);
                pos = part.Location;
                DrawRapids(g, pgm, ref pos);
            }
        }

        private void DrawRapids(Graphics g, PepLib.Program pgm, ref Vector pos)
        {
            for (int i = 0; i < pgm.Count; ++i)
            {
                var code = pgm[i];

                if (code.CodeType() == CodeType.SubProgramCall)
                {
                    var subpgm = (SubProgramCall)code;
                    var loop = subpgm.Loop;

                    if (loop != null)
                        DrawRapids(g, loop, ref pos);
                }
                else
                {
                    var motion = code as Motion;

                    if (motion != null)
                    {

                        if (pgm.Mode == ProgrammingMode.Incremental)
                        {
                            var endpt = motion.EndPoint + pos;

                            if (code.CodeType() == CodeType.RapidMove)
                                DrawLine(g, pos, endpt, rapidPen);
                            pos = endpt;
                        }
                        else
                        {
                            if (code.CodeType() == CodeType.RapidMove)
                                DrawLine(g, pos, motion.EndPoint, rapidPen);
                            pos = motion.EndPoint;
                        }
                    }
                }
            }
        }

        private void DrawLine(Graphics g, Vector pt1, Vector pt2, Pen pen)
        {
            var point1 = PointWorldToGraph(pt1);
            var point2 = PointWorldToGraph(pt2);

            g.DrawLine(pen, point1, point2);
        }

        private void DrawBox(Graphics g, Box box)
        {
            var rect = new RectangleF()
            {
                Location = PointWorldToGraph(box.Location),
                Width = LengthWorldToGui(box.Width),
                Height = LengthWorldToGui(box.Height)
            };

            g.DrawRectangle(Pens.Orange, rect.X, rect.Y - rect.Height, rect.Width, rect.Height);
        }

        private void UpdatePaths()
        {
            paths = new GraphicsPath[Plate.Parts.Count];

            for (int i = 0; i < Plate.Parts.Count; ++i)
            {
                var part = Plate.Parts[i];
                var path = new GraphicsPath();

                curpos = part.Location;
                DrawProgram(path, part.Program);

                paths[i] = path;
            }

            pathsNeedUpdated = false;
        }

        public float LengthWorldToGui(double length)
        {
            return scale * (float)length;
        }

        public double LengthGuiToWorld(float length)
        {
            return length / scale;
        }

        /// <summary>
        /// Returns a point with coordinates relative to the control from the graph.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Point PointGraphToControl(float x, float y)
        {
            return new Point((int)(x + origin.X), (int)(y + origin.Y));
        }

        /// <summary>
        /// Returns a point with coordinates relative to the control from the graph.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Point PointGraphToControl(PointF pt)
        {
            return PointGraphToControl(pt.X, pt.Y);
        }

        /// <summary>
        /// Returns a point with coordinates relative to the control from the world.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Point PointWorldToControl(double x, double y)
        {
            return PointGraphToControl(PointWorldToGraph(x, y));
        }

        /// <summary>
        /// Returns a point with coordinates relative to the control from the world.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Point PointWorldToControl(Vector pt)
        {
            return PointGraphToControl(PointWorldToGraph(pt.X, pt.Y));
        }

        /// <summary>
        /// Returns a point with coordinates relative to the graph from the control.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public PointF PointControlToGraph(int x, int y)
        {
            return new PointF(x - origin.X, y - origin.Y);
        }

        /// <summary>
        /// Returns a point with coordinates relative to the graph from the control.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public PointF PointControlToGraph(Point pt)
        {
            return PointControlToGraph(pt.X, pt.Y);
        }

        /// <summary>
        /// Returns a point with coordinates relative to the graph from the world.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public PointF PointWorldToGraph(double x, double y)
        {
            return new PointF(scale * (float)x, -scale * (float)y);
        }

        /// <summary>
        /// Returns a point with coordinates relative to the graph from the world.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public PointF PointWorldToGraph(Vector pt)
        {
            return PointWorldToGraph(pt.X, pt.Y);
        }

        /// <summary>
        /// Returns a point with coordinates relative to the world from the control.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector PointControlToWorld(int x, int y)
        {
            return PointGraphToWorld(PointControlToGraph(x, y));
        }

        /// <summary>
        /// Returns a point with coordinates relative to the world from the control.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector PointControlToWorld2(int x, int y)
        {
            return PointGraphToWorld2(PointControlToGraph(x, y));
        }

        /// <summary>
        /// Returns a point with coordinates relative to the world from the control.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Vector PointControlToWorld(Point pt)
        {
            return PointGraphToWorld(PointControlToGraph(pt.X, pt.Y));
        }

        /// <summary>
        /// Returns a point with coordinates relative to the world from the control.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Vector PointControlToWorld2(Point pt)
        {
            return PointGraphToWorld2(PointControlToGraph(pt.X, pt.Y));
        }

        /// <summary>
        /// Returns a point with coordinates relative to the world from the graph.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector PointGraphToWorld(float x, float y)
        {
            return new Vector(
                MathHelper.RoundToNearest(x / scale, 0.03125),
                MathHelper.RoundToNearest(y / -scale, 0.03125));
        }

        /// <summary>
        /// Returns a point with coordinates relative to the world from the graph.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector PointGraphToWorld2(float x, float y)
        {
            return new Vector(x / scale, y / -scale);
        }

        /// <summary>
        /// Returns a point with coordinates relative to the world from the graph.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Vector PointGraphToWorld(PointF pt)
        {
            return PointGraphToWorld(pt.X, pt.Y);
        }

        /// <summary>
        /// Returns a point with coordinates relative to the world from the graph.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Vector PointGraphToWorld2(PointF pt)
        {
            return PointGraphToWorld2(pt.X, pt.Y);
        }

        public void RequestPathUpdate()
        {
            pathsNeedUpdated = true;
        }

        public void ZoomToPoint(Point pt, float zoomFactor)
        {
            var pt2 = PointControlToWorld(pt);

            origin.X -= (float)(pt2.X * zoomFactor - pt2.X) * scale;
            origin.Y += (float)(pt2.Y * zoomFactor - pt2.Y) * scale;

            scale *= zoomFactor;

            pathsNeedUpdated = true;

            Invalidate();
        }

        public void ZoomToFit()
        {
            ZoomToArea(Plate.GetBoundingBox(true));
        }

        public void ZoomToPlate()
        {
            float px;
            float py;

            switch (Plate.Quadrant)
            {
                case 1:
                    px = 0;
                    py = 0;
                    break;

                case 2:
                    px = (float)-Plate.Size.Width;
                    py = 0;
                    break;

                case 3:
                    px = (float)-Plate.Size.Width;
                    py = (float)-Plate.Size.Height;
                    break;

                case 4:
                    px = 0;
                    py = (float)-Plate.Size.Height;
                    break;

                default:
                    return;
            }

            ZoomToArea(px, py, (float)Plate.Size.Width, (float)Plate.Size.Height);
        }

        public void ZoomToArea(Box box)
        {
            ZoomToArea(box.X, box.Y, box.Width, box.Height);
        }

        public void ZoomToArea(double x, double y, double width, double height)
        {
            if (width <= 0 || height <= 0)
                return;

            var a = (Height - BorderWidth) / height;
            var b = (Width - BorderWidth) / width;

            scale = (float)(a < b ? a : b);

            var px = LengthWorldToGui(x);
            var py = LengthWorldToGui(y);
            var pw = LengthWorldToGui(width);
            var ph = LengthWorldToGui(height);

            origin.X = (Width - pw) * 0.5f - px;
            origin.Y = (Height + ph) * 0.5f + py;

            pathsNeedUpdated = true;

            Invalidate();
        }

        private static double NormalizeAngle(double angle)
        {
            double r = angle % 360.0;
            return r < 0 ? 360.0 + r : r;
        }

        public Part GetPartAtPoint(Point pt)
        {
            if (pathsNeedUpdated || paths.Length != Plate.Parts.Count)
                UpdatePaths();

            var pt2 = new PointF(pt.X - origin.X, pt.Y - origin.Y);

            for (int i = Plate.Parts.Count - 1; i >= 0; i--)
            {
                var path = paths[i];

                if (path.IsVisible(pt2))
                    return Plate.Parts[i];
            }

            return null;
        }
    }
}
