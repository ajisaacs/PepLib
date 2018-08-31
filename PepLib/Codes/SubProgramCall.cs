
namespace PepLib.Codes
{
    public class SubProgramCall : ICode
    {
        private double rotation;
        private Loop loop;

        public SubProgramCall()
        {
        }

        public SubProgramCall(int loopId, int repeatCount, double rotation)
        {
            LoopId = loopId;
            RepeatCount = repeatCount;
            Rotation = rotation;
        }

        /// <summary>
        /// The id associated with the current set loop.
        /// </summary>
        public int LoopId { get; set; }

        /// <summary>
        /// Number of times the loop is cut.
        /// </summary>
        public int RepeatCount { get; set; }

        /// <summary>
        /// Gets or sets the loop associated with the loop id.
        /// </summary>
        public Loop Loop
        {
            get { return loop; }
            set
            {
                loop = (Loop)value.Clone();
                UpdateLoopRotation();
            }
        }

        /// <summary>
        /// Gets or sets the current rotation of the loop in degrees.
        /// </summary>
        public double Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                UpdateLoopRotation();
            }
        }

        private void UpdateLoopRotation()
        {
            if (loop != null)
            {
                var diffAngle = AngleConverter.ToRadians(rotation) - loop.Rotation;

                if (!diffAngle.IsEqualTo(0.0))
                    loop.Rotate(diffAngle);
            }
        }

        public CodeType CodeType()
        {
            return Codes.CodeType.SubProgramCall;
        }

        public ICode Clone()
        {
            return new SubProgramCall(LoopId, RepeatCount, Rotation)
            {
                Loop = Loop
            };
        }

        public override string ToString()
        {
            return string.Format("G92 L{0} R{1} P{2}", LoopId, RepeatCount, Rotation);
        }
    }
}
