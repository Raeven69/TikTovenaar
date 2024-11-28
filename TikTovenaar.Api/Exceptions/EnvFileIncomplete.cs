namespace TikTovenaar.Api.Exceptions
{
    public class EnvFileIncomplete(List<string> missingKeys) : Exception("Missing .env keys: " + string.Join(", ", missingKeys)) {}
}
