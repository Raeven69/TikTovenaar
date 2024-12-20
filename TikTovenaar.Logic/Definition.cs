namespace TikTovenaar.Logic
{
    public class Definition(string category, string meaning)
    {
        public string Category { get; } = category;
        public string Meaning { get; } = meaning;
    }
}