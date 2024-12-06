namespace TikTovenaar.DataAccess
{
    public class RequestFailedException(string msg) : Exception(msg)
    {
        public string Error { get; } = msg;
    }
}