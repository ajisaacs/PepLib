namespace PepLib.Codes
{
    public interface ICode
    {
        CodeType CodeType();
        ICode Clone();
    }
}
