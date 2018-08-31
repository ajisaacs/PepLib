
namespace PepLib.Codes
{
    public class SetFeedrate : ICode
    {
        public SetFeedrate()
        {
        }

        public SetFeedrate(double value)
        {
            Value = value;
        }

        public double Value { get; set; }

        public CodeType CodeType()
        {
            return Codes.CodeType.SetFeedrate;
        }

        public ICode Clone()
        {
            return new SetFeedrate(Value);
        }

        public override string ToString()
        {
            return string.Format("F{0}", Value);
        }
    }
}
