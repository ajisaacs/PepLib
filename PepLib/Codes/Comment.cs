
namespace PepLib.Codes
{
    public class Comment : ICode
    {
        public Comment()
        {
        }

        public Comment(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public CodeType CodeType()
        {
            return Codes.CodeType.Comment;
        }

        public ICode Clone()
        {
            return new Comment(Value);
        }

        public override string ToString()
        {
            return ':' + Value;
        }
    }
}
