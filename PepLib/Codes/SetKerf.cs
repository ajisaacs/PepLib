
namespace PepLib.Codes
{
    public class SetKerf : ICode
    {
        public SetKerf(KerfType kerf = KerfType.Left)
        {
            Kerf = kerf;
        }

        public KerfType Kerf { get; set; }

        public CodeType CodeType()
        {
            return Codes.CodeType.SetKerf;
        }

        public ICode Clone()
        {
            return new SetKerf(Kerf);
        }

        public override string ToString()
        {
            if (Kerf == KerfType.None)
                return "G40";

            if (Kerf == KerfType.Left)
                return "G41";

            return "G42";
        }
    }
}
